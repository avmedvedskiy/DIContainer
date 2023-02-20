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

        public static bool IsMonoBehaviourClass(this TypeDefinition typeReference)
        {
            if (typeReference == null)
                return false;
            if (typeReference.FullName == typeof(MonoBehaviour).FullName)
                return true;
            return IsMonoBehaviourClass(typeReference.BaseType.Resolve());
        }
    }
}