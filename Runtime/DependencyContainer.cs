using System;

namespace Services
{
    internal class DependencyContainer : IDependencyContainer
    {
        public IDependencyImplementation<T> Bind<T>() => new ImplementationContainer<T>();

        public T Resolve<T>() =>
            Implementation<T>.Instance;
    }
    
    internal readonly struct ImplementationContainer<T> : IDependencyImplementation<T>
    {
        public void AsSingle(T implementation) =>
            Implementation<T>.AsSingle(implementation);

        public void AsSingle<TClass>() where TClass : T, new() =>
            Implementation<T>.AsSingle<TClass>();

        public void AsTransient<TClass>() where TClass : T, new() =>
            Implementation<T>.AsTransient<TClass>();
    }

    internal static class Implementation<T>
    {
        private static T _instance;
        private static Func<T> _instanceCreation;
        private static bool _single;

        public static void AsSingle(T implementation)
        {
            _single = true;
            _instance = implementation;
        }

        public static void AsSingle<TClass>() where TClass : T, new()
        {
            _single = true;
            _instance = new TClass();
        }

        public static void AsTransient<TClass>() where TClass : T, new()
        {
            _single = false;
            _instanceCreation = () => new TClass();
        }

        public static T Instance =>
            _single ? _instance : _instanceCreation.Invoke();
    }
}