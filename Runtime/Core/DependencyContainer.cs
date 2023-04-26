using System.Runtime.CompilerServices;
using UnityEngine;

namespace DI
{
    public class DependencyContainer
    {
        public ContractType<TContact> Bind<TContact>() =>
            new();

        public ConcreteType<TConcrete, TConcrete> BindSelf<TConcrete>() =>
            new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TContract Resolve<TContract>() =>
            ImplementationResolver<TContract>.Instance;
    }
}