//---------------------------------------------------------------------
// <copyright file="DSPStreamProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.IO;

    /// <summary>
    /// Stream provider which only implements the IDataServiceStreamProvider interface
    /// </summary>
    public class DSPStreamProvider : IDataServiceStreamProvider, IDisposable
    {
        /// <summary>
        /// Actual provider to forward calls to.
        /// </summary>
        private IDataServiceStreamProvider provider;

        /// <summary>
        /// Constructs an instance of the stream provider.
        /// </summary>
        /// <param name="streamCache">Storage for the streams</param>
        public DSPStreamProvider(DSPMediaResourceStorage streamCache)
        {
            this.provider = new DSPStreamProvider2(streamCache);
        }

        #region IDisposable implementation

        public void Dispose()
        {
            IDisposable disposable = this.provider as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        #endregion IDisposable implementation

        #region IDataServiceStreamProvider implementation

        public void DeleteStream(object entity, DataServiceOperationContext operationContext)
        {
            this.provider.DeleteStream(entity, operationContext);
        }

        public Stream GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            return this.provider.GetReadStream(entity, etag, checkETagForEquality, operationContext);
        }

        public Uri GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
        {
            return this.provider.GetReadStreamUri(entity, operationContext);
        }

        public string GetStreamContentType(object entity, DataServiceOperationContext operationContext)
        {
            return this.provider.GetStreamContentType(entity, operationContext);
        }

        public string GetStreamETag(object entity, DataServiceOperationContext operationContext)
        {
            return this.provider.GetStreamETag(entity, operationContext);
        }

        public Stream GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            return this.provider.GetWriteStream(entity, etag, checkETagForEquality, operationContext);
        }

        public string ResolveType(string entitySetName, DataServiceOperationContext operationContext)
        {
            return this.provider.ResolveType(entitySetName, operationContext);
        }

        public int StreamBufferSize
        {
            get
            {
                return this.provider.StreamBufferSize;
            }
        }

        #endregion IDataServiceStreamProvider implementation
    }
}
