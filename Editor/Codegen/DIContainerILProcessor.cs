using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Unity.CompilationPipeline.Common.Diagnostics;
using Unity.CompilationPipeline.Common.ILPostProcessing;

namespace DI.Codegen
{
    public class DIContainerILProcessor : ILPostProcessor
    {
        public override ILPostProcessor GetInstance() => this;

        public override bool WillProcess(ICompiledAssembly compiledAssembly)
            => HasReferenceToContainer(compiledAssembly);

        private bool HasReferenceToContainer(ICompiledAssembly compiledAssembly)
            => compiledAssembly.References.Any(f => Path.GetFileName(f) == "DIContainer.Runtime.dll");

        public override ILPostProcessResult Process(ICompiledAssembly compiledAssembly)
        {
            var assemblyDefinition = AssemblyDefinitionFor(compiledAssembly);

            if (!WillProcess(compiledAssembly))
                return null;


            Console.WriteLine($"Start Processing = {assemblyDefinition.Name} ");

            FindAllClassesThatUsedInContainer(assemblyDefinition);

            var diagnostics = new List<DiagnosticMessage>();
            List<DiagnosticMessage> messages = new DIProcessor()
                .Process(assemblyDefinition, out bool madeAnyChange);

            diagnostics.AddRange(messages);

            if (!madeAnyChange || diagnostics.Any(d => d.DiagnosticType == DiagnosticType.Error))
            {
                return new ILPostProcessResult(null, diagnostics);
            }

            var pe = new MemoryStream();
            var pdb = new MemoryStream();
            var writerParameters = new WriterParameters
            {
                SymbolWriterProvider = new PortablePdbWriterProvider(), SymbolStream = pdb, WriteSymbols = true,
            };

            Console.WriteLine($"Start Writing = {assemblyDefinition.Name} ");
            assemblyDefinition.Write(pe, writerParameters);

            return new ILPostProcessResult(new InMemoryAssembly(pe.ToArray(), pdb.ToArray()), diagnostics);
        }

        private void FindAllClassesThatUsedInContainer(AssemblyDefinition assembly)
        {
            var methods = assembly
                .Modules
                .SelectMany(module => module.Types)
                .Where(x => x.IsNestedTypeOf(typeof(MonoInstaller)))
                .SelectMany(type => type.Methods)
                .Where(method => method.HasBody)
                .ToList();

            foreach (var method in methods)
            {
                //Console.WriteLine($"Find methods {method.Name} {method.DeclaringType}");
                //method.Body.Instructions.Write();

                var methodReferences = GetCallsBind(method);
                foreach (var reference in methodReferences)
                {
                    //Console.WriteLine($"Find class {((GenericInstanceMethod)reference).GenericArguments[0].Resolve().FullName}");
                    var type = ((GenericInstanceMethod)reference).GenericArguments[0].Resolve();

                    foreach (var ctor in type.Methods.Where(x => x.IsConstructor))
                    {
                        if(ctor.CustomAttributes.Any(x => x.AttributeType.Name == "InjectAttribute"))
                            continue;
                        
                        if(!ctor.HasParameters)
                            continue;
                        
                        var injectAttribute = assembly.MainModule.ImportReference(typeof(InjectAttribute));
                        var attributeConstructor = injectAttribute.Resolve().GetConstructors().First().Resolve();
                        var customAttribute =
                            new CustomAttribute(assembly.MainModule.ImportReference(attributeConstructor));
                        ctor.CustomAttributes.Add(customAttribute);
                    }
                }
            }
        }

        private static IEnumerable<MethodReference> GetCallsBind(MethodDefinition method)
        {
            return method.Body.Instructions
                .Where(instruction => instruction.OpCode == OpCodes.Callvirt || instruction.OpCode == OpCodes.Call)
                .Select(instruction => (MethodReference)instruction.Operand)
                .Where(methodReference => methodReference.FullName.Contains("::To") ||
                                          methodReference.FullName.Contains("::BindSelf"));
        }


        private AssemblyDefinition AssemblyDefinitionFor(ICompiledAssembly compiledAssembly)
        {
            var resolver = new PostProcessorAssemblyResolver(compiledAssembly);
            var readerParameters = new ReaderParameters
            {
                SymbolStream = new MemoryStream(compiledAssembly.InMemoryAssembly.PdbData.ToArray()),
                SymbolReaderProvider = new PortablePdbReaderProvider(),
                AssemblyResolver = resolver,
                ReflectionImporterProvider = new PostProcessorReflectionImporterProvider(),
                ReadingMode = ReadingMode.Immediate,
            };

            var peStream = new MemoryStream(compiledAssembly.InMemoryAssembly.PeData.ToArray());
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(peStream, readerParameters);

            resolver.AddAssemblyDefinitionBeingOperatedOn(assemblyDefinition);

            return assemblyDefinition;
        }
    }
}