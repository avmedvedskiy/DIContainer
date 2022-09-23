namespace Services
{
    public static class Dependency
    {
        public static T Resolve<T>() =>
            ProjectContext.Container.Resolve<T>();
    }
}