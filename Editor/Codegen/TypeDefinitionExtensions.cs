using System;
using System.Linq;
using Mono.Cecil;
using UnityEngine;

namespace DI.Codegen
{
    internal static class TypeDefinitionExtensions
    {
        public static MethodDefinition FindParentMethod(this TypeDefinition typeDefinition, string methodName)
        {
            if (typeDefinition == null)
                return null;
            var method = typeDefinition.Methods.FirstOrDefault(x => x.Name == methodName);
            if (method != null)
                return method;

            return FindParentMethod(typeDefinition.BaseType?.Resolve(), methodName);
        }

        public static Type Convert(this TypeReference typeReference)
        {
            return Type.GetType($"{typeReference.FullName}, {typeReference.Module.Assembly.FullName}"); 
        }

        public static bool IsMonoBehaviourClass(this TypeDefinition typeReference) 
            => IsNestedTypeOf(typeReference, typeof(MonoBehaviour));

        public static bool IsNestedTypeOf(this TypeDefinition typeReference, Type type)
        {
            if (typeReference == null)
                return false;
            if (typeReference.FullName == type.FullName)
                return true;
            return IsNestedTypeOf(typeReference.BaseType?.Resolve(),type);
        }
    }
}