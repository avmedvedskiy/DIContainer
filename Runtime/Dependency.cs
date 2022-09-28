namespace Services
{
    public static class Dependency
    {
        public static TContract Resolve<TContract>() =>
            ImplementationResolver<TContract>.Instance;
    }
}