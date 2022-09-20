using JetBrains.Annotations;

namespace Services
{
    public interface IServiceRegister
    {
        void RegisterSingle<TService>(TService service);
    }

    public interface IServiceContainer
    {
        TService Single<TService>();
    }

    public class Locator : IServiceRegister, IServiceContainer
    {
        private class Implementation<TService>
        {
            public static TService serviceInstance;
        }

        private static Locator _instance;
        public static IServiceContainer Container => _instance ??= new Locator();

        public void RegisterSingle<TService>(TService service) =>
            Implementation<TService>.serviceInstance = service;

        public TService Single<TService>() =>
            Implementation<TService>.serviceInstance;
    }
}