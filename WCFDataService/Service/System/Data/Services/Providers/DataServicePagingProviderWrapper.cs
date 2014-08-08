//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Providers
{
    /// <summary>Wrapper for IDataServicePagingProvider interface discovery.</summary>
    internal sealed class DataServicePagingProviderWrapper
    {
        /// <summary>Service instance.</summary>
        private readonly IDataService service;

        /// <summary>IDataServicePagingProvider interface for the service.</summary>
        private IDataServicePagingProvider pagingProvider;

        /// <summary>Was interface already requested.</summary>
        private bool checkedForIDataServicePagingProvider;

        /// <summary>Constructor.</summary>
        /// <param name="serviceInstance">Service instance.</param>
        public DataServicePagingProviderWrapper(IDataService serviceInstance)
        {
            this.service = serviceInstance;
        }

        /// <summary>Gives reference to IDataServicePagingProvider interface implemented by the service.</summary>
        public IDataServicePagingProvider PagingProviderInterface
        {
            get
            {
                if (!this.checkedForIDataServicePagingProvider)
                {
                    this.pagingProvider = this.service.Provider.GetService<IDataServicePagingProvider>();
                    this.checkedForIDataServicePagingProvider = true;
                }

                return this.pagingProvider;
            }
        }

        /// <summary>Is custom paging enabled for the service for query processing.</summary>
        public bool IsCustomPagedForQuery
        {
            get
            {
                return this.PagingProviderInterface != null;
            }
        }

        /// <summary>Do we need to handle custom paging during serialization.</summary>
        public bool IsCustomPagedForSerialization
        {
            get { return this.checkedForIDataServicePagingProvider && this.pagingProvider != null; }
        }

        /// <summary>
        /// Dispose the pagingProvider provider instance
        /// </summary>
        internal void DisposeProvider()
        {
            if (this.pagingProvider != null)
            {
                WebUtil.Dispose(this.pagingProvider);
                this.pagingProvider = null;
            }
        }
    }
}
