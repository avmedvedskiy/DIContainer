namespace Services
{
    public interface IDependencyContainer
    {
        TService Resolve<TService>();
        IDependencyImplementation<T> Bind<T>();
    }

    public interface IDependencyImplementation<in T>
    {
        void AsSingle(T implementation);

        void AsSingle<TClass>() where TClass : T, new();

        void AsTransient<TClass>() where TClass : T, new();
    }
}