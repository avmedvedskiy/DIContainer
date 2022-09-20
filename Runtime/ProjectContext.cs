using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    [DefaultExecutionOrder(-1000)]
    public sealed class ProjectContext : MonoBehaviour, IServiceRegister
    {
        [SerializeField] private MonoInstaller[] _installers = { };

        private readonly Locator _locator = new();
        private readonly List<object> _services = new();

        private void Awake()
        {
            InstallBindings();
        }

        private void Start()
        {
            foreach (var service in _services)
            {
                if(service is IInitialize initializedService)
                    initializedService.Initialize();
            }
        }

        private void InstallBindings()
        {
            foreach (var monoInstaller in _installers)
                monoInstaller.InstallBindings(this);
        }

        public void RegisterSingle<TService>(TService service)
        {
            _locator.RegisterSingle(service);
            _services.Add(service);
        }


        private void OnValidate()
        {
            _installers = GetComponentsInChildren<MonoInstaller>();
        }
    }
}