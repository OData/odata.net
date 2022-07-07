using System;
using Microsoft.Extensions.DependencyInjection;
using OData = Microsoft.OData;

namespace ExperimentsLib
{
    internal class ContainerBuilder : OData.IContainerBuilder
    {
        private readonly ServiceCollection services = new ServiceCollection();

        public OData.IContainerBuilder AddService(OData.ServiceLifetime lifetime, Type serviceType, Type implementationType)
        {
            switch (lifetime)
            {
                case OData.ServiceLifetime.Transient:
                    services.AddTransient(serviceType, implementationType);
                    break;
                case OData.ServiceLifetime.Singleton:
                    services.AddSingleton(serviceType, implementationType);
                    break;
                case OData.ServiceLifetime.Scoped:
                    services.AddScoped(serviceType, implementationType);
                    break;
            }

            return this;
        }

        public OData.IContainerBuilder AddService(OData.ServiceLifetime lifetime, Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            switch (lifetime)
            {
                case OData.ServiceLifetime.Transient:
                    services.AddTransient(serviceType, implementationFactory);
                    break;
                case OData.ServiceLifetime.Singleton:
                    services.AddSingleton(serviceType, implementationFactory);
                    break;
                case OData.ServiceLifetime.Scoped:
                    services.AddScoped(serviceType, implementationFactory);
                    break;
            }

            return this;
        }

        public IServiceProvider BuildContainer()
        {
            return services.BuildServiceProvider();
        }
    }
}
