namespace Services
{
    public interface IServiceRegister
    {
        void RegisterSingle<TService>(TService service);
    }
}