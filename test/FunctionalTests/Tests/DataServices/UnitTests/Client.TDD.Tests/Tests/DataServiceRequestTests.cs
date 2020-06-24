//---------------------------------------------------------------------
// <copyright file="DataServiceRequestTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Microsoft.OData;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.TDDUnitTests;
    using Xunit;

    public class DataServiceRequestTests
    {
        [Fact]
        public void NoContentMaterializeTestForEntry()
        {
            this.MaterializeTest(HttpStatusCode.NoContent, ODataPayloadKind.Resource);
        }

        [Fact]
        public void NoContentMaterializeTestForProperty()
        {
            this.MaterializeTest(HttpStatusCode.NoContent, ODataPayloadKind.Property);
        }

        [Fact]
        public void NoContentMaterializeTestForIndividualProperty()
        {
            this.MaterializeTest(HttpStatusCode.NoContent, ODataPayloadKind.IndividualProperty);
        }

        [Fact]
        public void NoContentMaterializeTestForValue()
        {
            this.MaterializeTest(HttpStatusCode.NoContent, ODataPayloadKind.Value);
        }

        [Fact]
        public void NoContentMaterializeTestForBinaryValue()
        {
            this.MaterializeTest(HttpStatusCode.NoContent, ODataPayloadKind.BinaryValue);
        }
        
        private void MaterializeTest(HttpStatusCode statusCode, ODataPayloadKind payloadKind)
        {
            var uri = new Uri("http://any");
            var context = new DataServiceContext().ReConfigureForNetworkLoadingTests();
            var requestInfo = new RequestInfo(context);
            var responseInfo = new ResponseInfo(requestInfo, MergeOption.OverwriteChanges);
            var queryComponents = new QueryComponents(uri, new Version(4, 0), typeof(Product), null, null);
            var responseMessage = new HttpWebResponseMessage(
                new HeaderCollection(),
                (int)statusCode,
                () => new MemoryStream());
            var materialize = DataServiceRequest.Materialize(
                responseInfo,
                queryComponents,
                null,
                "application/json",
                responseMessage,
                payloadKind);
            Assert.Null(materialize.Context);
            Assert.Null(materialize.Current);
            var enumerable = materialize.Cast<object>();
            Assert.Empty(enumerable);
        }

        private class Product
        {
            public int ID { get; set; }
            public int Name { get; set; }
        }
    }
}
