//---------------------------------------------------------------------
// <copyright file="CustomObjectContext.IDataServiceStreamProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.ObjectContextStubs
{
    #region Namespaces
    using System;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.IO;
    using ocs = AstoriaUnitTests.ObjectContextStubs;
    #endregion

    /// <summary>
    /// There are no comments for CustomObjectContext in the schema.
    /// </summary>
    public partial class CustomObjectContext : IDataServiceStreamProvider
    {
        public static readonly string DummyContentType = "DummyType/DummySubType";
        public static readonly Uri DummyReadStreamUri = new Uri("http://localhost/dummyuri/", UriKind.Absolute);
        #region IDataServiceStreamProvider Members

        public int StreamBufferSize
        {
            get { return 1; }
        }

        public Stream GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            throw new NotImplementedException();
        }

        public Stream GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            throw new NotImplementedException();
        }

        public void DeleteStream(object entity, DataServiceOperationContext operationContext)
        {
            throw new NotImplementedException();
        }

        public string GetStreamContentType(object entity, DataServiceOperationContext operationContext)
        {
            return DummyContentType;
        }

        public Uri GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
        {
            return DummyReadStreamUri;
        }

        public string GetStreamETag(object entity, DataServiceOperationContext operationContext)
        {
            return null;
        }

        public string ResolveType(string entitySetName, DataServiceOperationContext operationContext)
        {
            if (entitySetName == "CustomerBlobs")
            {
                return typeof(ocs.Hidden.CustomerBlob).Name;
            }
            else
            {
                throw new ArgumentException("Unrecognized entity set name", "entitySetName");
            }
        }

        #endregion
    }
}