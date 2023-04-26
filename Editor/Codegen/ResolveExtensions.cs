using Mono.Cecil;

namespace DI.Codegen
{
    public static class ResolveExtensions
    {
        public static MethodReference GetDependencyResolveMethod(this ModuleDefinition moduleDefinition,
            params TypeReference[] arguments)
        {
            return moduleDefinition.FindGenericMethod(typeof(Dependency), nameof(Dependency.Resolve), arguments);
        }
    }
}