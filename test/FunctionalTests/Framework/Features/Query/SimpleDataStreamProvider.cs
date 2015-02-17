//---------------------------------------------------------------------
// <copyright file="SimpleDataStreamProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

[[Usings]]

namespace [[ContextNamespace]]
{
    public partial class [[ContextTypeName]] : IDataServiceStreamProvider
    {
        public void DeleteStream(object entity, Microsoft.OData.Service.DataServiceOperationContext operationContext)
        {
            throw new DataServiceException(400, "NYI");
        }

        public Stream GetReadStream(object entity, string etag, bool? checkETagForEquality, Microsoft.OData.Service.DataServiceOperationContext operationContext)
        {
            throw new DataServiceException(400, "NYI");
        }

        public Uri GetReadStreamUri(object entity, Microsoft.OData.Service.DataServiceOperationContext operationContext)
        {
            throw new DataServiceException(400, "NYI");
        }

        public string GetStreamContentType(object entity, Microsoft.OData.Service.DataServiceOperationContext operationContext)
        {
            throw new DataServiceException(400, "NYI");
        }

        public string GetStreamETag(object entity, Microsoft.OData.Service.DataServiceOperationContext operationContext)
        {
            throw new DataServiceException(400, "NYI");
        }

        public Stream GetWriteStream(object entity, string etag, bool? checkETagForEquality, Microsoft.OData.Service.DataServiceOperationContext operationContext)
        {
            throw new DataServiceException(400, "NYI");
        }

        public string ResolveType(string entitySetName, Microsoft.OData.Service.DataServiceOperationContext operationContext)
        {
            throw new DataServiceException(400, "NYI");
        }

        public int StreamBufferSize
        {
            get { throw new DataServiceException(400, "NYI"); }
        }
    }
}
