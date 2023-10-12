namespace DI.ExternalManagers
{
    /// <summary>
    /// Install ITickableManager and IPauseManager
    /// </summary>
    public sealed class MonoEventsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<ITickableManager>()
                .FromComponentInNewGameObject<TickableManager>()
                .AsSingle()
                .NonLazy();
            
            Container
                .Bind<IPauseManager>()
                .FromComponentInNewGameObject<PauseManager>()
                .AsSingle()
                .NonLazy();
        }
    }
}