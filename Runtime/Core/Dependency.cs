using System.Runtime.CompilerServices;

namespace DI
{
    public static class Dependency
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContract Resolve<TContract>() =>
            ImplementationResolver<TContract>.Instance;
    }
}