using UnityEngine;

namespace DI
{
    public readonly struct ConcretePrefabType<TContract, TComponent> where TComponent : Component,TContract
    {
        private readonly TComponent _prefab;

        public ConcretePrefabType(TComponent prefab)
        {
            _prefab = prefab;
        }
        public ConcretePrefabLazyType<TContract, TComponent> AsSingle()
        {
            ImplementationResolver<TContract>.Set(
                new SingleLazyPrefabImplementation<TComponent>(_prefab));
            return new ConcretePrefabLazyType<TContract, TComponent>();
        }

        public void AsTransient()
        {
            ImplementationResolver<TContract>.Set(
                new TransientPrefabImplementation<TComponent>(_prefab));
        }
    }

    public readonly struct ConcretePrefabLazyType<TContract, TComponent> where TComponent : Component,TContract
    {
        public void NonLazy()
        {
            _ = ImplementationResolver<TContract>.Instance;
        }
    }
}