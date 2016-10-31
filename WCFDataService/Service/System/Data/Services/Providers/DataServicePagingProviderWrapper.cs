//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
