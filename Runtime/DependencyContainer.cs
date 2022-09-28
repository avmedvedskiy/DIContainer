using UnityEngine;

namespace Services
{
    public class DependencyContainer
    {
        public ContractType<T> Bind<T>() => new();

        public ConcreteType<T, T> BindSelf<T>() where T : new() => new();

        public T Resolve<T>() =>
            ImplementationResolver<T>.Instance;
    }

    public readonly struct ContractType<T>
    {
        public void FromInstance(T implementation)
        {
            ImplementationResolver<T>.Set(new SingleImplementation<T>(implementation));
        }

        public ConcretePrefabType<T, TComponent> FromComponentInNewPrefab<TComponent>(TComponent prefab)
            where TComponent : Component
        {
            return new ConcretePrefabType<T, TComponent>(prefab);
        }

        public ConcretePrefabType<T, TComponent> FromComponentInNewPrefab<TComponent>(GameObject prefab)
            where TComponent : Component
        {
            return new ConcretePrefabType<T, TComponent>(prefab.GetComponent<TComponent>());
        }

        public ConcreteType<T, TClass> To<TClass>() where TClass : T, new()
        {
            return new ConcreteType<T, TClass>();
        }
    }

    public readonly struct ConcretePrefabType<TInterface, TComponent> where TComponent : Component
    {
        private readonly TComponent _prefab;

        public ConcretePrefabType(TComponent prefab)
        {
            _prefab = prefab;
        }

        public void AsSingle()
        {
            ImplementationResolver<TInterface>.Set(
                new SinglePrefabImplementation<TComponent>(_prefab) as IImplementation<TInterface>);
        }

        public void AsTransient()
        {
            ImplementationResolver<TInterface>.Set(
                new TransientPrefabImplementation<TComponent>(_prefab) as IImplementation<TInterface>);
        }
    }

    public readonly struct ConcreteType<TInterface, TClass> where TClass : TInterface, new()
    {
        public ConcreteLazyType<TInterface, TClass> AsSingle()
        {
            ImplementationResolver<TInterface>.Set(
                new LazySingleImplementation<TClass>() as IImplementation<TInterface>);
            return new ConcreteLazyType<TInterface, TClass>();
        }

        public void AsTransient()
        {
            ImplementationResolver<TInterface>.Set(
                new TransientImplementation<TClass>() as IImplementation<TInterface>);
        }
    }

    public readonly struct ConcreteLazyType<TInterface, TClass> where TClass : TInterface, new()
    {
        public void NonLazy()
        {
            ImplementationResolver<TInterface>.Set(
                new SingleImplementation<TClass>(new TClass()) as IImplementation<TInterface>);
        }
    }
}