namespace Services
{
    public struct LazyInject<T> where T : class
    {
        private T _service;
        public T Service => _service ?? (_service = ProjectContext.GetService<T>());
    }
}
