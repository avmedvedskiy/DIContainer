namespace DI
{
    internal struct SingleImplementation<T> : IImplementation<T>
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
    
    internal struct LazyArgumentSingleImplementation<TContract, TConcrete> : IImplementation<TContract> where TConcrete : TContract
    {
        private readonly object[] _arguments;
        private TContract _instance;
        public TContract Instance => _instance ??= Factory.Create<TConcrete>(_arguments);

        public LazyArgumentSingleImplementation(params object[] arguments)
        {
            _arguments = arguments;
            _instance = default;
        }
    }

    internal struct TransientImplementation<TContract, TConcrete> : IImplementation<TContract> where TConcrete : TContract
    {
        public TContract Instance => Factory.Create<TConcrete>();
    }
}