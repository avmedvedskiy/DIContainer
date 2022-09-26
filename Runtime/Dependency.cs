namespace Services
{
    public static class Dependency
    {
        public static T Resolve<T>() where T : class =>
            ImplementationResolver<T>.Instance;
    }
}