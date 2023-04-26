using System;
using UnityEngine;

namespace DI
{
    public readonly struct ContractType<TContract>
    {        
        public ConcreteMethodType<TContract> FromMethod(Func<TContract> implementationMethod)
        {
            return new ConcreteMethodType<TContract>(implementationMethod);
        }
        
        public void FromInstance(TContract implementation) =>
            ImplementationResolver<TContract>.Set(new SingleImplementation<TContract>(implementation));

        public ConcretePrefabType<TContract, TComponent> FromComponentInNewPrefab<TComponent>(TComponent prefab)
            where TComponent : Component,TContract
        {
            return new ConcretePrefabType<TContract, TComponent>(prefab);
        }

        public ConcretePrefabType<TContract, TComponent> FromComponentInNewPrefab<TComponent>(GameObject prefab)
            where TComponent : Component,TContract
        {
            return new ConcretePrefabType<TContract, TComponent>(prefab.GetComponent<TComponent>());
        }

        public ConcreteGameObjectType<TContract, TComponent> FromComponentInNewGameObject<TComponent>()
            where TComponent : Component, TContract
        {
            return new ConcreteGameObjectType<TContract, TComponent>();
        }

        public ConcreteType<TContract, TClass> To<TClass>() where TClass : TContract
        {
            return new ConcreteType<TContract, TClass>();
        }
    }
}