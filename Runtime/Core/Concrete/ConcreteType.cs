using UnityEngine;

namespace DI
{
    public readonly struct ConcreteType<TContract, TConcrete> where TConcrete : TContract
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

    public readonly struct ConcreteLazyType<TContract, TConcrete> where TConcrete : TContract
    {
        public void NonLazy()
        {
            _ = ImplementationResolver<TContract>.Instance; //init instance
        }
    }
}