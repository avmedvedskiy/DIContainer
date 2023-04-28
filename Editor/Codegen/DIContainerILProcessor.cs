using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
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
            var assemblyDefinition = compiledAssembly.ConvertToAssemblyDefinition();

            if (!WillProcess(compiledAssembly))
                return null;

            Console.WriteLine($"Start Processing = {assemblyDefinition.Name} ");

            var types = assemblyDefinition.GetAllClassesThatUsedInContainer();
            if (types.Count > 0)
            {
                types.AddInjectInConstructor(assemblyDefinition);
                //CachedFactoriesCreator.Create(types, assemblyDefinition);
            }

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
    }
}