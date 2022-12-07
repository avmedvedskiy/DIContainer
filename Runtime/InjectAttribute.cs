using System;
using JetBrains.Annotations;

namespace DI
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Constructor | AttributeTargets.Method)]
    [MeansImplicitUse]
    public class InjectAttribute : Attribute
    {
    }
}