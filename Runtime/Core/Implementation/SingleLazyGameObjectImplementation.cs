using UnityEngine;

namespace DI
{
    internal struct SingleLazyGameObjectImplementation<TContract, TConcrete> : IImplementation<TContract> where TConcrete : Component, TContract
    {
        private TContract _instance;
        public TContract Instance => _instance ??= Factory.CreateNewGameObject<TConcrete>();
    }


    internal readonly struct TransientGameObjectImplementation<T> : IImplementation<T> where T : Component
    {
        public T Instance => Factory.CreateNewGameObject<T>();
    }
}