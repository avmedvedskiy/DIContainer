using System;

namespace DI
{
    internal class DefaultImplementation<T> : IImplementation<T>
    {
        public T Instance => default;
    }
    
    internal interface IImplementation<out T>
    {
        T Instance { get; }
    }
}