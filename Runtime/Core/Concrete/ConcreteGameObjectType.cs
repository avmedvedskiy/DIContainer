using UnityEngine;

namespace DI
{
    public readonly struct ConcreteGameObjectType<TContract, TComponent> where TComponent : Component, TContract
    {
        public ConcreteGameObjectLazyType<TContract, TComponent> AsSingle()
        {
            ImplementationResolver<TContract>.Set(
                new SingleLazyGameObjectImplementation<TContract, TComponent>());
            return new ConcreteGameObjectLazyType<TContract, TComponent>();
        }

        public void AsTransient()
        {
            ImplementationResolver<TContract>.Set(
                new TransientGameObjectImplementation<TComponent>());
        }
    }

    public readonly struct ConcreteGameObjectLazyType<TContract, TComponent> where TComponent : Component
    {
        public void NonLazy()
        {
            _ = ImplementationResolver<TContract>.Instance;
        }
    }
}