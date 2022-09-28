# Introduction 

Light-weight container for dynamically providing objects with the dependencies they need. The library is based on the Service Locator pattern.
## Register Mappings to the Container
ProjectContext - contains container and provide way to bind services via Installers. 

Implement own installers inherit MonoInstaller, add all installers into ProjectContext

![image](https://user-images.githubusercontent.com/17832838/192795725-710d4286-880a-4426-b611-2dcbfba51881.png)
```csharp
public class CustomInstaller : MonoInstaller
{
    public override void InstallBindings(DependencyContainer container)
    {

    }
}
```
## DependencyContainer methods
- <b>Bind<TContract></b> - binding a contract type
- <b>BindSelf<TContract></b> - binding a contract-concrete Type

After binding need to set a concrete type
- <b>To<TConctere></b> - setting a concrete realization type
- <b>FromInstance<TConctere></b> - setting instance, Instance can be like a Singleton Scope
- <b>FromComponentInNewPrefab<TConctereComponent></b> - Instantiate prefab and get Component

After set a conctere type set a Scope
- <b>AsSingle</b> - Same instance of ResultType every time ContractType is requested, which it will lazily generate upon first use.
  -<b>NonLazy</b> - Use NonLazy method for creating istance immidiately
- <b>AsTransient</b> - Every time ContractType is requested, the container will execute the given construction method again

## Resolve
For Resolve use Dependency and LazyDependency<TContract> classes
```csharp
var machine = Dependency.Resolve<IStateMachine>();

LazyDependency<IStateMachine> _stateMachine;
_stateMachine.Value //getting service
```

## Example
```csharp
public class CustomInstaller : MonoInstaller
{
    public override void InstallBindings(DependencyContainer container)
    {
        container.Bind<IStateMachine>().To<GameStateMachine>().AsSingle(); // bind IState Machine to GameStateMachine realization as Single
        container.Bind<IUpdater>().FromComponentInNewPrefab(_updater).AsSingle(); // binding IUpdate from prefab as Single
        container.BindSelf<UserStorageService>().AsSingle(); // binding UserStorageService as Single
    }
}
```
