namespace DI
{
    public struct ConcreteType<TContract, TConcrete> where TConcrete : TContract
    {
        public ConcreteLazyType<TContract, TConcrete> AsSingle()
        {
            ImplementationResolver<TContract>.Set(
                new LazySingleImplementation<TContract, TConcrete>());
            return new ConcreteLazyType<TContract, TConcrete>();
        }

        public void AsTransient()
        {
            ImplementationResolver<TContract>.Set(
                new TransientImplementation<TContract, TConcrete>());
        }
    }

    public struct ConcreteLazyType<TContract, TConcrete> where TConcrete : TContract
    {
        public ConcreteLazyType<TContract, TConcrete> WithArguments(params object[] arguments)
        {
            ImplementationResolver<TContract>.Set(
                new LazyArgumentSingleImplementation<TContract,TConcrete>(arguments));
            return new ConcreteLazyType<TContract, TConcrete>();
        }
        
        public void NonLazy()
        {
            _ = ImplementationResolver<TContract>.Instance; //init instance
        }
    }
}