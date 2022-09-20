using UnityEngine;

namespace Services
{
    [DefaultExecutionOrder(-1000)]
    public sealed class ProjectContext : MonoBehaviour
    {
        [SerializeField] private MonoInstaller[] _installers = { };
        private IServiceRegister ServiceRegister => Locator.Container as IServiceRegister;

        private void Start()
        {
            InstallBindings();
        }

        private void InstallBindings()
        {
            foreach (var monoInstaller in _installers)
                monoInstaller.InstallBindings(ServiceRegister);
        }

        private void OnValidate()
        {
            _installers = GetComponentsInChildren<MonoInstaller>();
        }
    }
}