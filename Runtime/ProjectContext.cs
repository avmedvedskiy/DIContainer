using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Services
{
    [DefaultExecutionOrder(-1000)]
    public sealed class ProjectContext : MonoBehaviour
    {
        private static readonly ServiceContainer _container = new ServiceContainer();
        internal static ServiceContainer Container => _container;

        [SerializeField] private List<MonoInstaller> _installers = new List<MonoInstaller>();

        private void Awake()
        {
            InstallBindings();
        }

        private void Start()
        {
            _container.ForEach<IInitialize>(x => x.Initialize());
        }

        private void InstallBindings()
        {
            foreach (var i in _installers)
                i.InstallBindings(_container);
        }

        //only for internal use
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T GetService<T>()
        {
            return _container.GetService<T>();
        }
    }
}
