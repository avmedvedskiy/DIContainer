using System.Collections.Generic;

namespace Services
{
    internal class Implementation<TService>
    {
        public static TService serviceInstance;
    }

    public class LocatorContainer:IServiceContainer
    {
        private readonly List<object> _services = new();

        internal IReadOnlyList<object> Services => _services;

        public void RegisterSingle<TService>(TService service)
        {
            Implementation<TService>.serviceInstance = service;
            _services.Add(service);
        }

        public TService Single<TService>() =>
            Implementation<TService>.serviceInstance;
        
        
    }
}