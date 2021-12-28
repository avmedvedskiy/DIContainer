using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    [DefaultExecutionOrder(-1000)]
    public sealed class ProjectContext : MonoBehaviour
    {
        internal static ServiceContainer Container { get; } = new ServiceContainer();

        [SerializeField] private List<MonoInstaller> _installers = new List<MonoInstaller>();

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            InstallBindings();
        }

        private void Start()
        {
            Container.ForEach<IInitialize>((x) => x.Initialize());
        }

        private void InstallBindings()
        {
            foreach (var i in _installers)
                i.InstallBindings(Container);
        }

        //only for internal use
        internal static T GetService<T>()
        {
            return Container.GetService<T>();
        }
    }
}
