namespace Services
{
    public class Locator
    {
        private class Implementation<TService>
        {
            public static TService serviceInstance;
        }

        public void RegisterSingle<TService>(TService service) => 
            Implementation<TService>.serviceInstance = service;

        public static TService Single<TService>() =>
            Implementation<TService>.serviceInstance;
    }
}