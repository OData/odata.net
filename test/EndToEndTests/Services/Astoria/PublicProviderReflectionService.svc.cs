//---------------------------------------------------------------------
// <copyright file="PublicProviderReflectionService.svc.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PublicProvider 
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using Microsoft.Test.OData.Framework.TestProviders.Common;
    using Microsoft.Test.OData.Framework.TestProviders.OptionalProviders;
    using Microsoft.Test.OData.Services.AstoriaDefaultService;

    /// <summary>
    /// Reflection service
    /// </summary>
    [ServiceBehavior(IncludeExceptionDetailInFaults=true, Namespace="http://microsoft.com/test/taupo/generated/")]
    public class ReflectionService : DataService<DefaultContainer>, IServiceProvider
    {
        private readonly ReflectionProvider provider;
        private readonly DefaultContainer container;

        /// <summary>
        /// Create an instance of Reflection Service
        /// </summary>
        public ReflectionService()
        {
            container = new DefaultContainer();
            provider = new ReflectionProvider(this, container);
        }

        /// <summary>
        /// Initialize the service
        /// </summary>
        /// <param name="config">The data service configuration</param>
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetEntitySetPageSize("*", 10);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
        }

        /// <summary>
        /// Create the data source
        /// </summary>
        /// <returns>Data source</returns>
        protected override DefaultContainer CreateDataSource()
        {
            return container;
        }

        /// <summary>
        /// Get Person count
        /// </summary>
        /// <returns>The count of Person</returns>
        [WebGet]
        public int GetPersonCount()
        {
            return container.Person.Count();
        }

        /// <summary>
        /// Get Person with the exact name match
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>The Person</returns>
        [WebGet]
        public Person GetPersonByExactName(string name)
        {
            return container.Person.SingleOrDefault(p => p.Name == name);
        }

        /// <summary>
        /// Get Persons with the partial name match
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>The Persons</returns>
        [WebGet]
        public IQueryable<Person> GetPersonsByName(string name)
        {
            return container.Person.Where(p => p.Name.Contains(name));
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// 
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        /// <param name="serviceType">An object that specifies the type of service object to get. </param><filterpriority>2</filterpriority>
        public object GetService(Type serviceType)
        {
            if (((serviceType == typeof(IDataServiceStreamProvider2)) || (serviceType == typeof(IDataServiceStreamProvider))))
            {
                return new InMemoryStreamProvider<ReferenceEqualityComparer>();
            }
            if (serviceType.IsInstanceOfType(provider))
            {
                return provider;
            }
            return null;
        }
    }
}
