//---------------------------------------------------------------------
// <copyright file="DSPDataService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using AstoriaUnitTests.Stubs;

    /// <summary>Service definition for data service based on the DataServiceProvider (DSPMetadata, DSPContext).</summary>
    public class DSPServiceDefinition : OpenWebDataServiceDefinition
    {
        /// <summary>The metadata definition for the service.</summary>
        public DSPMetadata Metadata { get; set; }

        /// <summary>The query provider for the service.</summary>
        public DSPResourceQueryProvider ResourceQueryProvider { get; set; }

        /// <summary>The data source for the service.</summary>
        public DSPContext DataSource { get; set; }

        /// <summary>Function which can create a data source.</summary>
        public Func<DSPMetadata, DSPContext> CreateDataSource { get; set; }

        /// <summary>Set to true if the service should allow write access.</summary>
        public bool Writable { get; set; }

        /// <summary>If set to false, GetService() will return null for IDataServiceStreamProvider.</summary>
        public bool SupportMediaResource { get; set; }

        /// <summary>If set to false, GetService() will return null for IDataServiceStreamProvider2.</summary>
        public bool SupportNamedStream { get; set; }

        /// <summary>Media resource storage for the current service.</summary>
        public DSPMediaResourceStorage MediaResourceStorage { get; set; }

        /// <summary>Whether custom paging through IDataServicePagingProvider should be enabled</summary>
        public bool EnableCustomPaging { get; set; }

        /// <summary>Whether expansions should be done with IExpandProvider.</summary>
        public bool SupportIExpandProvider { get; set; }

        /// <summary>This func will be called whenever an expression tree is about to execute. It is given a change to examine the tree
        /// and to process it before it's passed down to the provider's own processing and execution.</summary>
        public Func<Expression, Expression> ExpressionTreeInterceptor { get; set; }

        /// <summary>IDataServiceActionProvider implementation.</summary>
        public DSPActionProvider ActionProvider { get; set; }

        /// <summary>Constructor.</summary>
        public DSPServiceDefinition()
        {
            this.DataServiceType = typeof(DSPDataService);
        }

        /// <summary>Strongly typed static reference to the current service definition.</summary>
        new public static DSPServiceDefinition Current { get { return TestServiceDefinition.Current as DSPServiceDefinition; } }

        /// <summary>Currently used instance of the data source.</summary>
        /// <remarks>This value gets reset every time we need to reset the data source content.</remarks>
        private DSPContext currentDataSource;

        /// <summary>Creates new data source.</summary>
        /// <returns>The newly created data source.</returns>
        public DSPContext CurrentDataSource
        {
            get
            {
                Debug.Assert(!this.Writable || this.CreateDataSource != null, "If the service is writable you must specify the CreateDataSource function.");
                if (this.currentDataSource == null)
                {
                    if (this.CreateDataSource != null)
                    {
                        this.currentDataSource = this.CreateDataSource(this.Metadata);
                    }
                    else
                    {
                        this.currentDataSource = this.DataSource;
                    }
                }

                return this.currentDataSource;
            }
        }

        /// <summary>Clears all changes applied to the service up until now.</summary>
        public void ClearChanges()
        {
            this.currentDataSource = null;
        }

        /// <summary>Called to initialize the service on a given request.</summary>
        /// <param name="request">The request which was not yet used for the service to initialize on.</param>
        protected override void InitializeService(TestWebRequest request)
        {
            base.InitializeService(request);
            request.RegisterForDispose(() => { this.currentDataSource = null; });
        }
    }

    /// <summary>Data service implementation which can defined metadata for the service and stores the data as property bags.</summary>
    public class DSPDataService : OpenWebDataService<DSPContext>, IServiceProvider
    {
        /// <summary>The metadata definition. This also provides the <see cref="IDataServiceMetadataProvider"/> implementation.</summary>
        private DSPMetadata metadata;

        /// <summary>The resource query provider implementation for the service. Implements <see cref="IDataServiceQueryProvider"/>.</summary>
        private DSPResourceQueryProvider resourceQueryProvider;

        /// <summary>Constructor</summary>
        public DSPDataService()
        {
        }

        /// <summary>Abstract method which a derived class implements to create the metadata for the service.</summary>
        /// <returns>The metadata definition for the service. Note that this is called only once per the service lifetime.</returns>
        protected virtual DSPMetadata CreateDSPMetadata()
        {
            return DSPServiceDefinition.Current.Metadata ?? new DSPMetadata(this.GetType().Name, this.GetType().Namespace);
        }

        /// <summary>
        /// Abstract method which a derived class implements to create the query provider for the service.
        /// </summary>
        /// <returns>The query provider for the service.</returns>
        protected virtual DSPResourceQueryProvider CreateDSPResourceQueryProvider()
        {
            return DSPServiceDefinition.Current.ResourceQueryProvider ?? new DSPResourceQueryProvider(this.metadata, DSPServiceDefinition.Current.EnableCustomPaging);
        }

        /// <summary>Creates data source context.</summary>
        /// <returns>The newly created data source context.</returns>
        protected override DSPContext CreateDataSource()
        {
            return DSPServiceDefinition.Current.CurrentDataSource ?? base.CreateDataSource();
        }

        /// <summary>Returns the metadata definition for the service. It will create it if no metadata is available yet.</summary>
        protected DSPMetadata Metadata
        {
            get
            {
                if (this.metadata == null)
                {
                    this.metadata = this.CreateDSPMetadata();
                    this.metadata.SetReadOnly();
                    this.resourceQueryProvider = this.CreateDSPResourceQueryProvider();
                }

                return this.metadata;
            }
        }

        #region IServiceProvider Members

        /// <summary>Returns service implementation.</summary>
        /// <param name="serviceType">The type of the service requested.</param>
        /// <returns>Implementation of such service or null.</returns>
        public virtual object GetService(Type serviceType)
        {
            if (serviceType == typeof(IDataServiceMetadataProvider))
            {
                return this.Metadata;
            }
            else if (serviceType == typeof(IDataServiceQueryProvider))
            {
                return this.resourceQueryProvider;
            }
            else if ((serviceType == typeof(IDataServiceUpdateProvider) || serviceType == typeof(IDataServiceUpdateProvider2)) && DSPServiceDefinition.Current.Writable)
            {
                return new DSPUpdateProvider(this.CurrentDataSource, this.metadata);
            }
            else if (serviceType == typeof(IDataServicePagingProvider) && DSPServiceDefinition.Current.EnableCustomPaging)
            {
                return new AstoriaUnitTests.Tests.PagingProvider(this.resourceQueryProvider);
            }
            else if (serviceType == typeof(IDataServiceStreamProvider2) && DSPServiceDefinition.Current.SupportNamedStream)
            {
                return new DSPStreamProvider2(DSPServiceDefinition.Current.MediaResourceStorage);
            }
            else if (serviceType == typeof(IDataServiceStreamProvider) && (DSPServiceDefinition.Current.SupportMediaResource))
            {
                return new DSPStreamProvider(DSPServiceDefinition.Current.MediaResourceStorage);
            }
            else if (serviceType == typeof(IExpandProvider) && DSPServiceDefinition.Current.SupportIExpandProvider)
            {
                return new DSPExpandProvider(this.resourceQueryProvider);
            }
            else if (serviceType == typeof(IDataServiceActionProvider))
            {
                return DSPServiceDefinition.Current.ActionProvider;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
