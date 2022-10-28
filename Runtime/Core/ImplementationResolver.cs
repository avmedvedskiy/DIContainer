using System;

namespace DI
{
    internal static class ImplementationResolver<T>
    {
        private static IImplementation<T> _implementation = new DefaultImplementation<T>();

        public static void Set(IImplementation<T> implementation)
        {
            _implementation = implementation;
        }

        public static T Instance => _implementation.Instance;
    }
}