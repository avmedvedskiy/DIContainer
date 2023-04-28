using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace DI.Codegen
{
    public static class AssemblyExtensions
    {
        public static List<TypeDefinition> GetAllClassesThatUsedInContainer(this AssemblyDefinition assembly)
        {
            List<TypeDefinition> types = new List<TypeDefinition>();
            var methods = assembly
                .Modules
                .SelectMany(module => module.Types)
                .Where(x => x.IsNestedTypeOf(typeof(MonoInstaller)))
                .SelectMany(type => type.Methods)
                .Where(method => method.HasBody)
                .ToList();

            foreach (var method in methods)
            {
                var methodReferences = GetCallsBind(method);
                foreach (var reference in methodReferences)
                {
                    var type = ((GenericInstanceMethod)reference).GenericArguments[0].Resolve();
                    types.Add(type);
                }
            }

            return types;
        }
        public static void AddInjectInConstructor(this List<TypeDefinition> typeDefinitions, AssemblyDefinition assembly)
        {
            foreach (var typeDefinition in typeDefinitions)
            {
                foreach (var ctor in typeDefinition.Methods.Where(x => x.IsConstructor))
                {
                    if (ctor.CustomAttributes.Any(x => x.AttributeType.Name == nameof(InjectAttribute)))
                        continue;

                    if (!ctor.HasParameters)
                        continue;

                    var customAttribute = assembly.CreateCustomAttribute<InjectAttribute>();
                    ctor.CustomAttributes.Add(customAttribute);
                }
            }
        }
        
        public static TypeDefinition AddStaticClass(this AssemblyDefinition assembly, string name)
        {
            var cachedFactoriesForContainerType = new TypeDefinition(
                string.Empty,
                name,
                TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit,
                assembly.MainModule.ImportReference(typeof(object)));
            
            assembly.MainModule.Types.Add(cachedFactoriesForContainerType);
            
            var staticConstructor = new MethodDefinition(
                ".cctor",
                MethodAttributes.Static | MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                assembly.MainModule.ImportReference(typeof(void)));
            staticConstructor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            cachedFactoriesForContainerType.Methods.Add(staticConstructor);
            return cachedFactoriesForContainerType;
        }

        public static CustomAttribute CreateCustomAttribute<T>(this AssemblyDefinition assembly) where T: Attribute
        {
            var injectAttribute = assembly.MainModule.ImportReference(typeof(T));
            var attributeConstructor = injectAttribute.Resolve().GetConstructors().First().Resolve();
            return new CustomAttribute(assembly.MainModule.ImportReference(attributeConstructor));
        }
        
        public static CustomAttribute CreateCustomAttribute<T>(this AssemblyDefinition assembly, Func<MethodDefinition, bool> condition) where T: Attribute
        {
            var injectAttribute = assembly.MainModule.ImportReference(typeof(T));
            var attributeConstructor = injectAttribute.Resolve().GetConstructors().First(condition).Resolve();
            return new CustomAttribute(assembly.MainModule.ImportReference(attributeConstructor));
        }

        private static IEnumerable<MethodReference> GetCallsBind(MethodDefinition method)
        {
            return method.Body.Instructions
                .Where(instruction => instruction.OpCode == OpCodes.Callvirt || instruction.OpCode == OpCodes.Call)
                .Select(instruction => (MethodReference)instruction.Operand)
                .Where(methodReference => methodReference.FullName.Contains("::To") ||
                                          methodReference.FullName.Contains("::BindSelf"));
        }
    }
}