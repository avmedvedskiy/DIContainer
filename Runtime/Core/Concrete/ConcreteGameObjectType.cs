using UnityEngine;

namespace DI
{
    public readonly struct ConcreteGameObjectType<TContract, TComponent> where TComponent : Component
    {
        public ConcreteGameObjectLazyType<TContract, TComponent> AsSingle()
        {
            ImplementationResolver<TContract>.Set(
                new SingleLazyGameObjectImplementation<TComponent>() as IImplementation<TContract>);
            return new ConcreteGameObjectLazyType<TContract, TComponent>();
        }

        public void AsTransient()
        {
            ImplementationResolver<TContract>.Set(
                new TransientGameObjectImplementation<TComponent>() as IImplementation<TContract>);
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