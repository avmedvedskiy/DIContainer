using System.Runtime.CompilerServices;
using UnityEngine;

namespace DI
{
    public class DependencyContainer
    {
        public ContractType<TContact> Bind<TContact>() =>
            new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TContract Resolve<TContract>() =>
            ImplementationResolver<TContract>.Instance;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Instantiate<T>() => Resolve<T>();
    }
}