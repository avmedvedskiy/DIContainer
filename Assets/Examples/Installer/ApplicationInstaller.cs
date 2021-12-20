namespace Services.Examples
{
    public class ApplicationInstaller : MonoInstaller
    {
        public override void InstallBindings(ServiceContainer container)
        {
            container
                .Single<TestService>();
        }
    }
}