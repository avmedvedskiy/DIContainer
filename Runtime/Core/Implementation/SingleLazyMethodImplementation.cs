using System;

namespace DI
{
    internal struct SingleLazyMethodImplementation<T> : IImplementation<T>
    {
        private readonly Func<T> _method;
        private T _instance;

        public SingleLazyMethodImplementation(Func<T> method)
        {
            _method = method;
            _instance = default;
        }
        public T Instance => _instance ??= Factory.CreateFromMethod(_method);
    }

    internal readonly struct TransientMethodImplementation<T> : IImplementation<T>
    {
        private readonly Func<T> _method;

        public TransientMethodImplementation(Func<T> method)
        {
            _method = method;
        }

        public T Instance => Factory.CreateFromMethod(_method);
    }
}