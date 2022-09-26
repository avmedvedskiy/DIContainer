namespace Services
{
    public struct LazyDependency<T>
    {
        private T _value;
        public T Value => _value ??= Dependency.Resolve<T>();
    }
}