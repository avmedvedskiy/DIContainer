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

    internal struct LazySingleImplementation<TContract, TConcrete> : IImplementation<TContract> where TConcrete : TContract
    {
        private TContract _instance;
        public TContract Instance => _instance ??= Factory.Create<TConcrete>();
    }

    internal readonly struct TransientImplementation<TContract, TConcrete> : IImplementation<TContract> where TConcrete : TContract
    {
        public TContract Instance => Factory.Create<TConcrete>();
    }
}