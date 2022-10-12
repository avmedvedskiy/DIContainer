using UnityEngine;

namespace DI
{
    public abstract class MonoInstaller : MonoBehaviour
    {
        public abstract void InstallBindings(DependencyContainer container);
    }
}
