using UnityEngine;

namespace DI
{
    internal struct SingleLazyPrefabImplementation<T> : IImplementation<T> where T : Component
    {
        private readonly T _prefab;
        private T _instance;

        public SingleLazyPrefabImplementation(T prefab)
        {
            _prefab = prefab;
            _instance = default;
        }

        public T Instance => _instance ??= _instance = Factory.CreateFromPrefab(_prefab);
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
}