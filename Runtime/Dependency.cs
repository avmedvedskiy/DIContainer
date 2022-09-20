namespace Services
{
    public static class Dependency
    {
        public static TService Single<TService>() =>
            Implementation<TService>.serviceInstance;
    }
}