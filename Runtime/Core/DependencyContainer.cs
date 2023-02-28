using System.Runtime.CompilerServices;
using UnityEngine;

namespace DI
{
    public class DependencyContainer
    {
        public ContractType<TContact> Bind<TContact>() =>
            new();

        public ConcreteType<TConcrete, TConcrete> BindSelf<TConcrete>() where TConcrete : new() =>
            new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TContract Resolve<TContract>() =>
            ImplementationResolver<TContract>.Instance;
    }

    public readonly struct ContractType<TContract>
    {
        public void FromInstance(TContract implementation) =>
            ImplementationResolver<TContract>.Set(new SingleImplementation<TContract>(implementation));

        public ConcretePrefabType<TContract, TComponent> FromComponentInNewPrefab<TComponent>(TComponent prefab)
            where TComponent : Component,TContract
        {
            return new ConcretePrefabType<TContract, TComponent>(prefab);
        }

        public ConcretePrefabType<TContract, TComponent> FromComponentInNewPrefab<TComponent>(GameObject prefab)
            where TComponent : Component,TContract
        {
            return new ConcretePrefabType<TContract, TComponent>(prefab.GetComponent<TComponent>());
        }

        public ConcreteGameObjectType<TContract, TComponent> FromComponentInNewGameObject<TComponent>()
            where TComponent : Component, TContract
        {
            return new ConcreteGameObjectType<TContract, TComponent>();
        }

        public ConcreteType<TContract, TClass> To<TClass>() where TClass : TContract, new()
        {
            return new ConcreteType<TContract, TClass>();
        }
    }

    public readonly struct ConcretePrefabType<TContract, TComponent> where TComponent : Component
    {
        private readonly TComponent _prefab;

        public ConcretePrefabType(TComponent prefab)
        {
            _prefab = prefab;
        }

        public void AsSingle()
        {
            ImplementationResolver<TContract>.Set(
                new SinglePrefabImplementation<TComponent>(_prefab) as IImplementation<TContract>);
        }

        public void AsTransient()
        {
            ImplementationResolver<TContract>.Set(
                new TransientPrefabImplementation<TComponent>(_prefab) as IImplementation<TContract>);
        }
    }

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