using UnityEngine;

namespace DI
{
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

    internal struct LazySingleImplementation<T> : IImplementation<T> where T : new()
    {
        private T _instance;
        public T Instance => _instance ??= Factory.Create<T>();
    }

    internal readonly struct TransientImplementation<T> : IImplementation<T> where T : new()
    {
        public T Instance => Factory.Create<T>();
    }

    internal readonly struct SinglePrefabImplementation<T> : IImplementation<T> where T : Component
    {
        public SinglePrefabImplementation(T prefab)
        {
            Instance = Factory.CreateFromPrefab(prefab);
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

        public T Instance => Factory.CreateFromPrefab(_prefab);
    }


    internal struct SingleLazyGameObjectImplementation<T> : IImplementation<T> where T : Component
    {
        private T _instance;
        public T Instance => _instance ??= Factory.CreateNewGameObject<T>();
    }

    internal readonly struct TransientGameObjectImplementation<T> : IImplementation<T> where T : Component
    {
        public T Instance => Factory.CreateNewGameObject<T>();
    }
}