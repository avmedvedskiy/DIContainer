namespace Services
{
    public static class Dependency
    {
        public static T Resolve<T>() =>
            ImplementationResolver<T>.Instance;
    }
}