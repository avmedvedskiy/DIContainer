﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    [DefaultExecutionOrder(-1000)]
    public sealed class ProjectContext : MonoBehaviour
    {
        [SerializeField] private MonoInstaller[] _installers = { };

        private readonly LocatorContainer _locator = new();

        private void Awake()
        {
            InstallBindings();
        }

        private void Start()
        {
            foreach (var service in _locator.Services)
            {
                if (service is IInitialize initializedService)
                    initializedService.Initialize();
            }
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