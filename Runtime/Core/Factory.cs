using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DI
{
    /* can be cached in runtime, but performance unclear
        /// <summary>
        /// this dictionary will be filled by codegen type GeneratedCachedFactoriesForContainer when
        /// will be called unity RuntimeInitializeLoadType.SubsystemRegistration
        /// </summary>
        public static Dictionary<Type, Func<object>> CachedFactories = new();

        internal static T Create<T>()
        {
            T result ;
            if (CachedFactories.TryGetValue(typeof(T), out var factory))
            {
                //Debug.Log($"Cached factory {typeof(T)}");
                result =  (T)factory.Invoke();
            }
            else
            {
                //Debug.Log("constructorInfo factory");
                var constructorInfo = typeof(T).GetConstructors()[0];
                result = (T)constructorInfo.Invoke(new object[constructorInfo.GetParameters().Length]);
            }
            RegisterToInternalInterfaces(result);
            return result;
        }
     */
    public static class Factory
    {
        private static ITickableManager TickableManager => Dependency.Resolve<ITickableManager>();
        private static IPauseManager PauseManager => Dependency.Resolve<IPauseManager>();

        internal static T Create<T>()
        {
            var constructorInfo = typeof(T).GetConstructors()[0];
            var autoInjected = Attribute.IsDefined(constructorInfo, typeof(InjectAttribute));
            var parameters = constructorInfo.GetParameters();
            T result;
            if (autoInjected || parameters is { Length: 0 })
            {
                result = (T)constructorInfo.Invoke(new object[constructorInfo.GetParameters().Length]);
            }
            else
            {
                result = (T)constructorInfo.Invoke(FillArguments<T>(parameters));
            }

            RegisterToInternalInterfaces(result);
            return result;
        }
        
        internal static T Create<T>(object[] arguments)
        {
            var constructorInfo = typeof(T).GetConstructors()[0];
            var result = (T)constructorInfo.Invoke(arguments);
            RegisterToInternalInterfaces(result);
            return result;
        }

        private static object[] FillArguments<T>(ParameterInfo[] parameters)
        {
            var arguments = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                arguments[i] = ReflectionDependency.ResolveByReflection(parameter.ParameterType);
            }

            return arguments;
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


        private static void RegisterToInternalInterfaces<T>(T result)
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