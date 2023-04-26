using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DI
{
    internal static class Factory
    {
        private static ITickableManager TickableManager => Dependency.Resolve<ITickableManager>();
        private static IPauseManager PauseManager => Dependency.Resolve<IPauseManager>();

        internal static T Create<T>() where T : new()
        {
            T result = new T();
            RegisterToInternalInterfaces(result);
            return result;

        }

        internal static T CreateNewGameObject<T>() where T : Component
        {
            GameObject go = new GameObject(typeof(T).Name);
            Object.DontDestroyOnLoad(go);
            T result = go.AddComponent<T>();
            RegisterToInternalInterfaces(result);
            return result;
        }

        internal static T CreateFromPrefab<T>(T prefab) where T : Component
        {
            T result = Object.Instantiate(prefab);
            Object.DontDestroyOnLoad(result.gameObject);
            RegisterToInternalInterfaces(result);
            return result;
        }
        
        internal static T CreateFromMethod<T>(Func<T> method)
        {
            T result = method.Invoke();
            RegisterToInternalInterfaces(result);
            return result;
        }

        internal static T FromInstance<T>(T instance)
        {
            RegisterToInternalInterfaces(instance);
            return instance;
        }


        static void RegisterToInternalInterfaces<T>(T result)
        {
            switch (result)
            {
                case ITickable tickable:
                    TickableManager.AddListener(tickable);
                    break;
                case IPausable pausable:
                    PauseManager.AddListener(pausable);
                    break;
            }
        }
    }
}