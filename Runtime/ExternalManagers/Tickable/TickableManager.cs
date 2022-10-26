using System.Collections.Generic;
using UnityEngine;

namespace DI
{
    internal class TickableManager : MonoBehaviour, ITickableManager
    {
        private List<ITickable> _tickables = new();
        private TickableManager _tickableManager;

        public void AddListener(ITickable tickable)
        {
            _tickables.Add(tickable);
        }

        public void RemoveListener(ITickable tickable)
        {
            _tickables.Remove(tickable);
        }

        private void Update()
        {
            for (int i = 0; i < _tickables.Count; i++)
            {
                ITickable tickable = _tickables[i];
                if(tickable != null)
                    tickable.Tick(Time.deltaTime);
            }
        }
    }
}