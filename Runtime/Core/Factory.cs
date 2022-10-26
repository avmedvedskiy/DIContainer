using UnityEngine;

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

            //var constructorInfo = typeof(T).GetConstructors()[0];
            //T result = (T)constructorInfo.Invoke(new object[constructorInfo.GetParameters().Length]);
            //return result;
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


        private static void RegisterToInternalInterfaces<T>(T result)
        {
            if (result is ITickable tickable)
                TickableManager.AddListener(tickable);

            if (result is IPausable pausable)
                PauseManager.AddListener(pausable);
        }
    }
}