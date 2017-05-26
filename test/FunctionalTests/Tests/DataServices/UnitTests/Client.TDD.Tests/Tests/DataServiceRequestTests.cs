//---------------------------------------------------------------------
// <copyright file="DataServiceRequestTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Microsoft.OData.Client;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataServiceRequestTests
    {
        [TestMethod]
        public void NoContentMaterializeTestForEntry()
        {
            this.MaterializeTest(HttpStatusCode.NoContent, ODataPayloadKind.Resource);
        }

        [TestMethod]
        public void NoContentMaterializeTestForProperty()
        {
            this.MaterializeTest(HttpStatusCode.NoContent, ODataPayloadKind.Property);
        }

        [TestMethod]
        public void NoContentMaterializeTestForIndividualProperty()
        {
            this.MaterializeTest(HttpStatusCode.NoContent, ODataPayloadKind.IndividualProperty);
        }

        [TestMethod]
        public void NoContentMaterializeTestForValue()
        {
            this.MaterializeTest(HttpStatusCode.NoContent, ODataPayloadKind.Value);
        }

        [TestMethod]
        public void NoContentMaterializeTestForBinaryValue()
        {
            this.MaterializeTest(HttpStatusCode.NoContent, ODataPayloadKind.BinaryValue);
        }
        
        private void MaterializeTest(HttpStatusCode statusCode, ODataPayloadKind payloadKind)
        {
            var uri = new Uri("http://any");
            var context = new DataServiceContext();
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
            Assert.IsNull(materialize.Context);
            Assert.IsNull(materialize.Current);
            var enumerable = materialize.Cast<object>();
            Assert.AreEqual(0, enumerable.Count());
        }

        private class Product
        {
            public int ID { get; set; }
            public int Name { get; set; }
        }
    }
}
