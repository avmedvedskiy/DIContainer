using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DI.Codegen.Linker;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Unity.CompilationPipeline.Common.Diagnostics;
using Unity.CompilationPipeline.Common.ILPostProcessing;
using UnityEditor;
using UnityEngine;

namespace DI.Codegen
{
    public class DIContainerILProcessor : ILPostProcessor
    {
        public override ILPostProcessor GetInstance() => this;

        public override bool WillProcess(ICompiledAssembly compiledAssembly)
            => compiledAssembly.HasReferenceToContainer();

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
                SaveClassesInFile(types);
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

        private void SaveClassesInFile(List<TypeDefinition> types)
        {
            var names = types
                .Select(x => Assembly.CreateQualifiedName(x.Module.Assembly.FullName, x.FullName));
            
            LinkerBuildPlayerProcessor.WriteTempFile(names.ToArray());
        }
        
        
    }
}