using System;
using Mono.Cecil;
using ICustomAttributeProvider = Mono.Cecil.ICustomAttributeProvider;

namespace Editor
{
    internal static class Helpers
    {
        public static CustomAttribute GetCustomAttribute<T>(ICustomAttributeProvider instance)
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
        
        public static MethodReference MakeGenericMethod(MethodReference self, params TypeReference[] arguments)
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