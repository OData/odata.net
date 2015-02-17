//---------------------------------------------------------------------
// <copyright file="PublicProviderHybridService.svc.cs" company="Microsoft">
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
    using System.Text.RegularExpressions;
    using Microsoft.Test.OData.Framework.TestProviders.Common;
    using Microsoft.Test.OData.Framework.TestProviders.OptionalProviders;
    using Microsoft.Test.OData.Services.Astoria;
    using Microsoft.Test.OData.Services.AstoriaDefaultService;

    /// <summary>
    /// The hybrid service which merges a EF provider and Reflection provider
    /// </summary>
    [ServiceBehavior(IncludeExceptionDetailInFaults=true, Namespace="http://microsoft.com/test/taupo/generated/")]
    public class HybridService : DataService<HybridDataSource>, IServiceProvider
    {
        private readonly HybridProvider provider;
        private readonly HybridDataSource dataSource;

        /// <summary>
        /// The regex to match the entity name from the query.
        /// </summary>
        static readonly Regex EntityRegex = new Regex(@"(?<entity>[\w]+)(\(\w*\))?\/?", RegexOptions.Compiled);
        
        /// <summary>
        /// The current entity set
        /// </summary>
        internal string EntitySet { get; private set; }

        /// <summary>
        /// The static constructor to ensure the database is deployed.
        /// </summary>
        static HybridService()
        {
            try
            {
                DatabaseHelper.EnsureDatabaseCreated();
            }
            catch (Exception ex)
            {
                Log.Trace(ex);
                throw;
            }
        }

        /// <summary>
        /// Create an instance of class HybridService
        /// </summary>
        public HybridService()
        {
            dataSource = new HybridDataSource
                {
                    DatabaseSource = new AstoriaDefaultServiceDBEntities(),
                    ReflectionDataSource = new DefaultContainer()
                };
            //help EF to find the assembly
            provider = new HybridProvider(this, dataSource);
        }

        /// <summary>
        /// Initialize the service
        /// </summary>
        /// <param name="config"></param>
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
        /// <returns>The data source</returns>
        protected override HybridDataSource CreateDataSource()
        {
            return dataSource;
        }

        /// <summary>
        /// Get EFPerson count
        /// </summary>
        /// <returns>The count of EFPerson</returns>
        [WebGet]
        [ProviderType(Type = "EF")]
        public int GetEFPersonCount()
        {
            return dataSource.DatabaseSource.EFPersons.Count();
        }

        /// <summary>
        /// Get EFPerson with the exact name match
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>The EFPerson</returns>
        [WebGet]
        [ProviderType(Type = "EF")]
        public EFPerson GetEFPersonByExactName(string name)
        {
            return dataSource.DatabaseSource.EFPersons.SingleOrDefault(p => p.Name == name);
        }

        /// <summary>
        /// Get EFPersons with the partial name match
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>The EPPersons</returns>
        [WebGet]
        [ProviderType(Type = "EF")]
        public IQueryable<EFPerson> GetEFPersonsByName(string name)
        {
            return dataSource.DatabaseSource.EFPersons.Where(p => p.Name.Contains(name));
        }

        /// <summary>
        /// Get Person count
        /// </summary>
        /// <returns>The count of Person</returns>
        [WebGet]
        [ProviderType(Type = "Reflection")]
        public int GetPersonCount()
        {
            return dataSource.ReflectionDataSource.Person.Count();
        }

        /// <summary>
        /// Get Person with the exact name match
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>The Person</returns>
        [WebGet]
        [ProviderType(Type = "Reflection")]
        public Person GetPersonByExactName(string name)
        {
            return dataSource.ReflectionDataSource.Person.SingleOrDefault(p => p.Name == name);
        }

        /// <summary>
        /// Get Persons with the partial name match
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>The Persons</returns>
        [WebGet]
        [ProviderType(Type = "Reflection")]
        public IQueryable<Person> GetPersonsByName(string name)
        {
            return dataSource.ReflectionDataSource.Person.Where(p => p.Name.Contains(name));
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
            Log.Trace(serviceType.Name);
            if ((serviceType == typeof(IDataServiceStreamProvider2)) || (serviceType == typeof(IDataServiceStreamProvider)))
            {
                return new InMemoryStreamProvider<ReferenceEqualityComparer>();
            }
            if (serviceType.IsInstanceOfType(provider))
            {
                return provider;
            }
            return null;
        }

        protected override void OnStartProcessingRequest(ProcessRequestArgs args)
        {
            Log.Trace(args.RequestUri);
            if (args.RequestUri.Segments.Length > args.ServiceUri.Segments.Length)
            {
                EntitySet = args.RequestUri.Segments[args.ServiceUri.Segments.Length];
            }

            if (EntitySet != null)
            {
                // The entity set could be
                // XXX, XXX/, XXX(), XXX()/, XXXX(YYY)/
                var match = EntityRegex.Match(EntitySet);
                EntitySet = match.Groups["entity"].Value;
            }
        }
    }
}
