namespace DI
{
    internal static class ImplementationResolver<T>
    {
        private static IImplementation<T> _implementation;

        public static void Set(IImplementation<T> implementation)
        {
            _implementation = implementation;
        }

        public static T Instance =>
            _implementation == null ? default : _implementation.Instance;
    }
}