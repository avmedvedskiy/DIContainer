using UnityEngine;

namespace DI
{
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