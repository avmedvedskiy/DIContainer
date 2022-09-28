namespace Services
{
    public struct LazyDependency<TContract>
    {
        private TContract _value;
        public TContract Value => _value ??= Dependency.Resolve<TContract>();
    }
}