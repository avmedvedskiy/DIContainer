using UnityEngine;
using Object = UnityEngine.Object;

namespace Services
{
    public class DependencyContainer
    {
        public ContractType<T> Bind<T>() => new();

        public ConcreteType<T, T> BindSelf<T>() where T : new() => new();
        public void BindInstance<T>(T instance) => new ContractType<T>().FromInstance(instance);

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
        public void AsSingle()
        {
            ImplementationResolver<TInterface>.Set(
                new SingleImplementation<TClass>(new TClass()) as IImplementation<TInterface>);
        }

        public void AsTransient()
        {
            ImplementationResolver<TInterface>.Set(
                new TransientImplementation<TClass>() as IImplementation<TInterface>);
        }
    }


    internal static class ImplementationResolver<T>
    {
        private static IImplementation<T> _implementation;

        public static void Set(IImplementation<T> implementation)
        {
            _implementation = implementation;
        }

        public static T Instance =>
            _implementation == null ? default : _implementation.Instance;
    }

    internal interface IImplementation<out T>
    {
        T Instance { get; }
    }

    internal readonly struct SingleImplementation<T> : IImplementation<T>
    {
        public T Instance { get; }

        public SingleImplementation(T instance)
        {
            Instance = instance;
        }
    }

    internal readonly struct TransientImplementation<T> : IImplementation<T> where T : new()
    {
        public T Instance => new();
    }

    internal readonly struct SinglePrefabImplementation<T> : IImplementation<T> where T : Component
    {
        public SinglePrefabImplementation(T prefab)
        {
            Instance = Object.Instantiate(prefab);
            Object.DontDestroyOnLoad(Instance.gameObject);
        }

        public T Instance { get; }
    }

    internal readonly struct TransientPrefabImplementation<T> : IImplementation<T> where T : Component
    {
        private readonly T _prefab;

        public TransientPrefabImplementation(T prefab)
        {
            _prefab = prefab;
        }

        public T Instance
        {
            get
            {
                var instance = Object.Instantiate(_prefab);
                Object.DontDestroyOnLoad(instance.gameObject);
                return instance;
            }
        }
    }
}