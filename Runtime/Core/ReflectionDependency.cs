using System;
using System.Reflection;

namespace DI
{
    public static class ReflectionDependency
    {
        private static readonly MethodInfo _resolveMethod;
        private static readonly Type[] _resolveArguments;
        
        static ReflectionDependency()
        {
            _resolveMethod = typeof(Dependency)
                .GetMethod(nameof(Dependency.Resolve), BindingFlags.Static | BindingFlags.Public);
            _resolveArguments = new Type[1];
        }
        
        internal static object ResolveByReflection(Type contractType)
        {
            _resolveArguments[0] = contractType;
            MethodInfo genericMethodInfo = _resolveMethod.MakeGenericMethod(_resolveArguments);
            return genericMethodInfo.Invoke(null, null);
        }
    }
}