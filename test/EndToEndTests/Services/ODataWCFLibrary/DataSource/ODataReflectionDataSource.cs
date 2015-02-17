//---------------------------------------------------------------------
// <copyright file="ODataReflectionDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.DataSource
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;

    [Serializable]
    public abstract class ODataReflectionDataSource : IODataDataSource
    {
        [NonSerialized]
        private readonly Lazy<IODataQueryProvider> queryProviderLazy = new Lazy<IODataQueryProvider>(() => new ODataReflectionQueryProvider(), LazyThreadSafetyMode.None);
        [NonSerialized]
        private readonly Lazy<IODataUpdateProvider> updateProviderLazy = new Lazy<IODataUpdateProvider>(() => new ODataReflectionUpdateProvider(), LazyThreadSafetyMode.None);
        [NonSerialized]
        private readonly Lazy<IODataStreamProvider> streamProviderLazy = new Lazy<IODataStreamProvider>(() => new ODataReflectionStreamProvider(), LazyThreadSafetyMode.None);
        [NonSerialized]
        private IODataQueryProvider queryProvider;
        [NonSerialized]
        private IODataUpdateProvider updateProvider;
        [NonSerialized]
        private IODataStreamProvider streamProvider;
        [NonSerialized]
        private IODataOperationProvider operationProvider;
        [NonSerialized]
        private IEdmModel model;

        public virtual IEdmModel Model
        {
            get { return this.model ?? (this.model = this.CreateModel()); }
        }

        public IODataQueryProvider QueryProvider
        {
            get { return this.queryProvider ?? (this.queryProvider = this.queryProviderLazy.Value); }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                this.queryProvider = value;
            }
        }

        public IODataUpdateProvider UpdateProvider
        {
            get { return this.updateProvider ?? (this.updateProvider = this.updateProviderLazy.Value); }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                this.updateProvider = value;
            }
        }

        public IODataOperationProvider OperationProvider
        {
            get { return this.operationProvider; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                this.operationProvider = value;
            }
        }

        public IODataStreamProvider StreamProvider
        {
            get { return this.streamProvider ?? (this.streamProvider = this.streamProviderLazy.Value); }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                this.streamProvider = value;
            }
        }

        public abstract void Reset();

        public abstract void Initialize();

        protected abstract IEdmModel CreateModel();
    }
}
