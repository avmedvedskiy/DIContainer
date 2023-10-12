using UnityEngine;

namespace DI
{
    [DefaultExecutionOrder(-9999)]
    public sealed class ProjectContext : MonoBehaviour
    {
        [SerializeField] private MonoInstaller[] _installers = { };
        internal static DependencyContainer Container { get; private set; }

        private void Awake()
        {
            Container = new DependencyContainer();
            InstallBindings();
            DontDestroyOnLoad(gameObject);
        }
        private void InstallBindings()
        {
            foreach (var monoInstaller in _installers)
            {
                monoInstaller.Container = Container;
                monoInstaller.InstallBindings();
            }
        }

        private void OnValidate()
        {
            _installers = GetComponentsInChildren<MonoInstaller>();
        }
    }
}