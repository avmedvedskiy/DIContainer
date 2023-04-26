using System;

namespace DI
{
    public readonly struct ConcreteMethodType<TContract>
    {
        private readonly Func<TContract> _method;

        public ConcreteMethodType(Func<TContract> method)
        {
            _method = method;
        }

        public ConcreteMethodLazyType<TContract> AsSingle()
        {
            ImplementationResolver<TContract>.Set(
                new SingleLazyMethodImplementation<TContract>(_method));
            return new ConcreteMethodLazyType<TContract>();
        }

        public void AsTransient()
        {
            ImplementationResolver<TContract>.Set(
                new TransientMethodImplementation<TContract>(_method));
        }
    }

    public readonly struct ConcreteMethodLazyType<TContract>
    {
        public void NonLazy()
        {
            _ = ImplementationResolver<TContract>.Instance; //init instance
        }
    }
}