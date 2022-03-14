namespace Services
{
    public struct LazyInject<T>
    {
        private T _service;
        public T Service => _service ?? (_service = ProjectContext.GetService<T>());
    }
}
