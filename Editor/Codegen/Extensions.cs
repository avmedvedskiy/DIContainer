using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using Unity.CompilationPipeline.Common.ILPostProcessing;
using ICustomAttributeProvider = Mono.Cecil.ICustomAttributeProvider;

namespace DI.Codegen
{
    internal static class Extensions
    {
        public static bool HasReferenceToContainer(this ICompiledAssembly compiledAssembly)
            => compiledAssembly.References.Any(f => Path.GetFileName(f) == "DIContainer.Runtime.dll");
        
        public static void Write(this Collection<Instruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                Console.WriteLine(instruction);
            }
        }

        public static CustomAttribute GetCustomAttribute<T>(this ICustomAttributeProvider instance)
        {
            if (!instance.HasCustomAttributes)
            {
                return null;
            }

            var attributes = instance.CustomAttributes;

            for (var i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                if (attribute.AttributeType.FullName.Equals(typeof(T).FullName, StringComparison.Ordinal))
                {
                    return attribute;
                }
            }

            return null;
        }

        public static MethodReference FindGenericMethod(this ModuleDefinition moduleDefinition, Type type, string name,
            params TypeReference[] arguments)
        {
            return MakeGenericMethod(
                moduleDefinition.ImportReference(type.GetMethod(name,
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)),
                arguments);
        }

        public static AssemblyDefinition ConvertToAssemblyDefinition(this ICompiledAssembly compiledAssembly)
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

        private static MethodReference MakeGenericMethod(MethodReference self, params TypeReference[] arguments)
        {
            if (self.GenericParameters.Count != arguments.Length)
            {
                throw new ArgumentException();
            }

            var instance = new GenericInstanceMethod(self);
            foreach (var argument in arguments)
            {
                instance.GenericArguments.Add(argument);
            }

            return instance;
        }
    }
}