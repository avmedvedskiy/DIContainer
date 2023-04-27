using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DI
{
    public static class Dependency
    {
        private static readonly MethodInfo _resolveMethod;
        private static readonly Type[] _resolveArguments;
        
        static Dependency()
        {
            Type type = typeof(Dependency);
            _resolveMethod = type.GetMethod(nameof(Resolve), BindingFlags.Static | BindingFlags.Public);
            _resolveArguments = new Type[1];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContract Resolve<TContract>() =>
            ImplementationResolver<TContract>.Instance;
        
        internal static object ResolveByReflection(Type contractType)
        {
            _resolveArguments[0] = contractType;
            MethodInfo genericMethodInfo = _resolveMethod.MakeGenericMethod(_resolveArguments);
            return genericMethodInfo.Invoke(null, null);
        }
        
        
    }
}