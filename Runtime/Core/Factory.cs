using UnityEngine;

namespace DI
{
    internal static class Factory
    {
        internal static T Create<T>() where T : new()
        {
            T result = new T();
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
            return result;
        }

        internal static T CreateFromPrefab<T>(T prefab) where T : Component
        {
            T result = Object.Instantiate(prefab);
            Object.DontDestroyOnLoad(result.gameObject);
            return result;
        }
    }
}