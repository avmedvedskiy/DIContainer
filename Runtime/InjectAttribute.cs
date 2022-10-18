using System;

namespace DI
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Constructor | AttributeTargets.Method)]
    public class InjectAttribute : Attribute
    {
    }
}