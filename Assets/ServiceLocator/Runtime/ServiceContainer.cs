using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Services
{
    public class ServiceContainer
    {
        private List<object> _services = new List<object>(128);

        public void Single<T>() where T : new() => _services.Add(new T());

        public void Single(object service) => _services.Add(service);

        public void RemoveSingle(object service) => _services.Remove(service);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal T GetService<T>()
        {
            int count = _services.Count;
            for(int i = 0; i < count; i++)
            {
                if (_services[i] is T s)
                    return s;
            }
            return default;
        }

        internal void ForEach<T>(Action<T> action)
        {
            for (int i = 0; i < _services.Count; i++)
            {
                if (_services[i] is T s)
                    action.Invoke(s);
            }
        }

    }
}
