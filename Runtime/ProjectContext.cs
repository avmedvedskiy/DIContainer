using System;
using UnityEngine;

namespace Services
{
    [DefaultExecutionOrder(-1000)]
    public sealed class ProjectContext : MonoBehaviour
    {
        [SerializeField] private MonoInstaller[] _installers = { };
        private readonly LocatorContainer _locator = new();
        private static bool _isInit;

        private void Awake()
        {
            if (_isInit)
                throw new Exception("ProjectContext already init");
            
            InstallBindings();
            DontDestroyOnLoad(gameObject);
            _isInit = true;
        }

        private void InstallBindings()
        {
            foreach (var monoInstaller in _installers)
                monoInstaller.InstallBindings(_locator);
        }

        private void OnValidate()
        {
            _installers = GetComponentsInChildren<MonoInstaller>();
        }
    }
}