using System;
using UnityEngine;

namespace DI
{
    [DefaultExecutionOrder(-1000)]
    public sealed class ProjectContext : MonoBehaviour
    {
        [SerializeField] private MonoInstaller[] _installers = { };
        internal static DependencyContainer Container { get; private set; }

        private void Awake()
        {
            if (Container != null)
                throw new Exception("ProjectContext already init");

            Container = new DependencyContainer();
            InstallDefaultBindings();
            InstallBindings();
            DontDestroyOnLoad(gameObject);
        }

        private void InstallDefaultBindings()
        {
            Container.Bind<ITickableManager>().FromComponentInNewGameObject<TickableManager>().AsSingle().NonLazy();
            Container.Bind<IPauseManager>().FromComponentInNewGameObject<PauseManager>().AsSingle().NonLazy();
        }

        private void InstallBindings()
        {
            foreach (var monoInstaller in _installers)
                monoInstaller.InstallBindings(Container);
        }

        private void OnValidate()
        {
            _installers = GetComponentsInChildren<MonoInstaller>();
        }
    }
}