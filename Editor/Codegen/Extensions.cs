using System;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using ICustomAttributeProvider = Mono.Cecil.ICustomAttributeProvider;

namespace DI.Codegen
{
    internal static class Extensions
    {
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