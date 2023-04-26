using UnityEngine;

namespace DI
{
    public abstract class MonoInstaller : MonoBehaviour
    {
        public DependencyContainer Container { get; internal set; }
        public abstract void InstallBindings();
    }
}
