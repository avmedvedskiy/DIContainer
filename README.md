# Introduction 

Light-weight container for dynamically providing objects with the dependencies they need.
## Register Mappings to the Container
ProjectContext - contains container and provide way to bind services via Installers. 

Implement own installers inherit MonoInstaller, add all installers into ProjectContext

![image](https://user-images.githubusercontent.com/17832838/192795725-710d4286-880a-4426-b611-2dcbfba51881.png)

## DependencyContainer methods
- <b>Bind<TContract></b> - binding a contract type
- <b>BindSelf<TContract></b> - binding a contract-concrete Type

After binding need to set a concrete type
- <b>To<TConctere></b> - setting a concrete realization type
- <b>FromInstance<TConctere></b> - setting instance, Instance can be like a Singleton Scope
- <b>FromComponentInNewPrefab<TConctereComponent></b> - Instantiate prefab and get Component
- <b>FromMethod<TConctereComponent></b> - Create instance type from method

After set a conctere type set a Scope
- <b>AsSingle</b> - Same instance of ResultType every time ContractType is requested, which it will lazily generate upon first use.
  -<b>NonLazy</b> - Use NonLazy method for creating istance immidiately
- <b>AsTransient</b> - Every time ContractType is requested, the container will execute the given construction method again

## Resolve
For Resolve use InjectAttribute for fields and properties(lazy initialization after first using)
Also included injection in Constructor
```csharp
[Inject] private IStateMachine Machine { get; } //will be inited when first use
[Inject] private readonly StoriesService _storiesService; //will be inited in constructor
```

## Example
```csharp
public class CustomInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IStateMachine>().To<GameStateMachine>().AsSingle(); // bind IState Machine to GameStateMachine realization as Single
        Container.Bind<IUpdater>().FromComponentInNewPrefab(_updater).AsSingle(); // binding IUpdate from prefab as Single
        Container.BindSelf<UserStorageService>().AsSingle(); // binding UserStorageService as Single
    }
}
```
