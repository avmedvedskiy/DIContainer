using UnityEngine;

namespace DI
{
    public readonly struct ConcreteType<TContract, TConcrete> where TConcrete : TContract, new()
    {
        public ConcreteLazyType<TContract, TConcrete> AsSingle()
        {
            ImplementationResolver<TContract>.Set(
                new LazySingleImplementation<TConcrete>() as IImplementation<TContract>);
            return new ConcreteLazyType<TContract, TConcrete>();
        }

        public void AsTransient()
        {
            ImplementationResolver<TContract>.Set(
                new TransientImplementation<TConcrete>() as IImplementation<TContract>);
        }
    }

    public readonly struct ConcreteLazyType<TContract, TConcrete> where TConcrete : TContract, new()
    {
        public void NonLazy()
        {
            _ = ImplementationResolver<TContract>.Instance; //init instance
        }
    }
}