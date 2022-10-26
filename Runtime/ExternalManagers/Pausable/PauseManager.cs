using System.Collections.Generic;
using UnityEngine;

namespace DI
{
    internal class PauseManager : MonoBehaviour, IPauseManager
    {
        private readonly List<IPausable> _pausables = new();
        private float _pauseTime;

        public void AddListener(IPausable handler)
        {
            _pausables.Add(handler);
        }

        public void RemoveListener(IPausable handler)
        {
            _pausables.Remove(handler);
        }

        private void OnApplicationQuit()
        {
            _pausables.ForEach(x => x.OnApplicationPause());
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                _pausables.ForEach(x => x.OnApplicationPause());
                _pauseTime = Time.realtimeSinceStartup;
            }
            else
            {
                var time = Time.realtimeSinceStartup - _pauseTime;
                _pausables.ForEach(x => x.OnApplicationResume(time));
            }
        }
    }
}