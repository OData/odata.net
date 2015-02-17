//---------------------------------------------------------------------
// <copyright file="ServiceSimulatorFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OData.Service;
using AstoriaUnitTests.Tests.Server.Simulators;
using Microsoft.OData.Service.Providers;
using Microsoft.OData.Service.Caching;

namespace AstoriaUnitTests.Tests.Server
{
    using AstoriaUnitTests.TDD.Tests.Server.Simulators;

    internal class ServiceSimulatorFactory
    {
        private readonly Uri baseUri;

        private DataServiceProviderSimulator provider;
        private DataServiceHostSimulator host;
        private DataServiceBehavior behavior;

        public ServiceSimulatorFactory()
        {
            this.baseUri = new Uri("http://localhost");
            this.host = new DataServiceHostSimulator()
            {
                AbsoluteServiceUri = this.baseUri,
                RequestHttpMethod = "GET",
                RequestAccept = "application/atom+xml,application/xml",
                RequestVersion = "4.0",
                RequestMaxVersion = "4.0",
            };

            this.provider = new DataServiceProviderSimulator();
            this.behavior = new DataServiceBehavior() { MaxProtocolVersion = Microsoft.OData.Client.ODataProtocolVersion.V4 };
        }

        public Uri RequestUri
        {
            get { return this.host.AbsoluteRequestUri; }
        }

        public DataServiceBehavior ServiceBehavior
        {
            get { return this.behavior; }
        }
        
        public void AddQueryArgument(String key, String argument)
        {
            this.host.SetQueryStringItem(key, argument);
        }

        public void ClearQueryArgument()
        {
            this.host.ClearQueryArgument();
        }

        public void SetRequestUri(string relativeUri)
        {
            this.host.AbsoluteRequestUri = new Uri(baseUri, relativeUri);
        }

        public void SetDataSource(IDataSourceSimulator ds)
        {
            this.provider.CurrentDataSource = ds;
        }

        public void AddResourceType(ResourceType type)
        {
            this.provider.AddResourceType(type);
        }

        public void AddResourceSet(ResourceSet set)
        {
            this.provider.AddResourceSet(set);
        }

        public IDataService CreateService()
        {
            return this.CreateService<DataServiceSimulator>();
        }

        /// <summary>
        /// TODO: Refactor this into a factory pattern, if needed
        /// </summary>
        /// <returns>IDataService</returns>
        public IDataService CreateService<T>() where T : DataServiceSimulator, new()
        {
            T dataService = new T();

            //
            // Operation Context
            //
            DataServiceOperationContext operationContext = new DataServiceOperationContext(host);
            operationContext.InitializeAndCacheHeaders(dataService);

            //
            // Configuration
            //
            DataServiceConfiguration config = new DataServiceConfiguration(provider);
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
            config.DataServiceBehavior.AcceptAnyAllRequests = this.behavior.AcceptAnyAllRequests;
            config.DataServiceBehavior.AcceptCountRequests = this.behavior.AcceptCountRequests;
            config.DataServiceBehavior.AcceptProjectionRequests = this.behavior.AcceptProjectionRequests;
            config.DataServiceBehavior.AcceptSpatialLiteralsInQuery = this.behavior.AcceptSpatialLiteralsInQuery;
            config.DataServiceBehavior.IncludeAssociationLinksInResponse = this.behavior.IncludeAssociationLinksInResponse;
            config.DataServiceBehavior.InvokeInterceptorsOnLinkDelete = this.behavior.InvokeInterceptorsOnLinkDelete;
            config.DataServiceBehavior.MaxProtocolVersion = this.behavior.MaxProtocolVersion;
            config.DataServiceBehavior.UseMetadataKeyOrderForBuiltInProviders = this.behavior.UseMetadataKeyOrderForBuiltInProviders;

            DataServiceStaticConfiguration staticConfiguration = new DataServiceStaticConfiguration(dataService.Instance.GetType(), provider);
            IDataServiceProviderBehavior providerBehavior = DataServiceProviderBehavior.CustomDataServiceProviderBehavior;

            var providerWrapper = new DataServiceProviderWrapper(
                new DataServiceCacheItem(
                    config, 
                    staticConfiguration), 
                provider, 
                provider, 
                dataService,
                false);
            dataService.ProcessingPipeline = new DataServiceProcessingPipeline();
            dataService.Provider = providerWrapper;
            providerWrapper.ProviderBehavior = providerBehavior;

            //
            // Service
            //
            operationContext.RequestMessage.InitializeRequestVersionHeaders(config.DataServiceBehavior.MaxProtocolVersion.ToVersion());
            var pipeline = new DataServiceProcessingPipeline();
#if DEBUG
            pipeline.SkipDebugAssert = true;
#endif
            dataService.OperationContext = operationContext;
            dataService.Provider = providerWrapper;
            dataService.ProcessingPipeline = pipeline;
            dataService.Configuration = config;
            dataService.StreamProvider = new DataServiceStreamProviderWrapper(dataService);
            dataService.ActionProvider = DataServiceActionProviderWrapper.Create(dataService);
            return dataService;
        }
    }
}
