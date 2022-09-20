namespace Services
{
    public interface IServiceContainer
    {
        void RegisterSingle<TService>(TService service);
        TService Single<TService>();
    }
}