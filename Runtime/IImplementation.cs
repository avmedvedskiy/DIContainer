using UnityEngine;

namespace Services
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
        public T Instance => _instance ??= new T();
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