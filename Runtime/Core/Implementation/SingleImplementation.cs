namespace DI
{
    internal readonly struct SingleImplementation<T> : IImplementation<T>
    {
        public T Instance { get; }

        public SingleImplementation(T instance)
        {
            Instance = Factory.FromInstance(instance);
        }
    }

    internal struct LazySingleImplementation<T> : IImplementation<T> where T : new()
    {
        private T _instance;
        public T Instance => _instance ??= Factory.Create<T>();
    }

    internal readonly struct TransientImplementation<T> : IImplementation<T> where T : new()
    {
        public T Instance => Factory.Create<T>();
    }
}