using System.Runtime.CompilerServices;

namespace Services
{
    /// <summary>
    /// Should be used only for readonly fields
    /// </summary>
    public static class Inject
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Service<T>() => ProjectContext.GetService<T>();
    }
}
