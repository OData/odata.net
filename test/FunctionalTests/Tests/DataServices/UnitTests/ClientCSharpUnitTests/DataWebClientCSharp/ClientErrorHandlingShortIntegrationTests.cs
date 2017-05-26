//---------------------------------------------------------------------
// <copyright file="ClientErrorHandlingShortIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.IO;
    using System.Linq;
    using System.Net;
    using AstoriaUnitTests.ClientExtensions;
    using FluentAssertions;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Text;

    [TestClass]
    public class ClientErrorHandlingShortIntegrationTests
    {
        [TestMethod]
        public void TopBatchReturingODataError()
        {
            const string payload = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<error xmlns=""http://docs.oasis-open.org/odata/ns/metadata"">
  <code></code>
  <message xml:lang=""en-US"">This error must show up in the error returned below</message>
</error>";

            IODataRequestMessage requestMessage = new ODataTestMessage();
            var responseMessage = new ODataTestMessage();
            responseMessage.SetHeader("Content-Type", "application\atom+xml");
            responseMessage.SetHeader("OData-Version", "4.0");
            responseMessage.StatusCode = 400;
            responseMessage.WriteToStream(payload);
            responseMessage.SetHeader("Content-Length", "0");

            var context = new DataServiceContextWithCustomTransportLayer(ODataProtocolVersion.V4, requestMessage, responseMessage);
            context.AddObject("Products", new SimpleNorthwind.Product() { ID = 1 });
            Action test = () => context.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);

            // we should be returning an ODataErrorException instead of the full contents
            test.ShouldThrow<DataServiceRequestException>()
                .WithInnerException<DataServiceClientException>()
                .And.InnerException.Message.Should().Be("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>\r\n<error xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">\r\n  <code></code>\r\n  <message xml:lang=\"en-US\">This error must show up in the error returned below</message>\r\n</error>");
        }

        [TestMethod]
        public void WebExceptionShouldNotBeSurfacedWhenSaveChangesWithBatch()
        {
            IODataRequestMessage requestMessage = new ODataTestMessage();

            var context = new DataServiceContextWithCustomTransportLayer(ODataProtocolVersion.V4, () => requestMessage, () => { throw new WebException("web exception on getting response"); });
            context.AddObject("Products", new SimpleNorthwind.Product() { ID = 1 });
            Action test = () => context.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);

            test.ShouldThrow<WebException>().WithMessage("web exception on getting response");
        }

        [TestMethod]
        public void WebExceptionShouldNotBeSurfacedWhenSaveChangesWithNonBatch()
        {
            IODataRequestMessage requestMessage = new ODataTestMessage();

            var context = new DataServiceContextWithCustomTransportLayer(ODataProtocolVersion.V4, () => requestMessage, () => { throw new WebException("web exception on getting response"); });
            context.AddObject("Products", new SimpleNorthwind.Product() { ID = 1 });
            Action test = () => context.SaveChanges(SaveChangesOptions.None);

            var innerInnerException = test.ShouldThrow<DataServiceRequestException>().WithInnerException<DataServiceClientException>().And.InnerException.InnerException;
            innerInnerException.Should().BeOfType<WebException>();
            innerInnerException.Message.Should().Be("web exception on getting response");
        }

        [TestMethod]
        public void WebExceptionShouldNotBeSurfacedWhenGetResponseThrowsOnBatch()
        {
            IODataRequestMessage requestMessage = new ODataTestMessage();

            var context = new DataServiceContextWithCustomTransportLayer(ODataProtocolVersion.V4, () => requestMessage, () => { throw new WebException("web exception on getting response"); });
            Action test = () =>  context.ExecuteBatch(context.CreateQuery<NorthwindModel.Products>("Products"));

            test.ShouldThrow<WebException>().WithMessage("web exception on getting response");
        }

        /// <summary>
        /// Other than a DataServiceTransportException all other exceptions should just be raised to the user directly.
        /// </summary>
        [TestMethod]
        public void WebExceptionShouldeSurfacedWhenGetResponseThrowsOnNonBatch()
        {
            IODataRequestMessage requestMessage = new ODataTestMessage();

            var context = new DataServiceContextWithCustomTransportLayer(ODataProtocolVersion.V4, () => requestMessage, () => { throw new WebException("web exception on getting response"); });
            Action test = () => context.CreateQuery<NorthwindModel.Products>("Products").ToList();

            var exception = test.ShouldThrow<Exception>().And;

            // Should not be surfacing an internal exception
            exception.Should().BeOfType<WebException>();
            exception.Message.Should().Be("web exception on getting response");
        }


        [TestMethod]
        public void ObjectDisposedExceptionShouldNotBeSurfacedDirectlyOnAddObjectBatchOnFailureInGetStream()
        {
            var test = AddObjectWithInjectedObjectDisposedOnGetStreamO(SaveChangesOptions.BatchWithSingleChangeset);

            test.ShouldThrow<ObjectDisposedException>().WithMessage("Cannot access a disposed object.\r\nObject name: 'Stream already disposed'.");
        }

        [TestMethod]
        public void ObjectDisposedExceptionShouldNotBeSurfacedDirectlyAddObjectInGetStream()
        {
            var test = AddObjectWithInjectedObjectDisposedOnGetStreamO(SaveChangesOptions.None);

            test.ShouldThrow<ObjectDisposedException>().WithMessage("Cannot access a disposed object.\r\nObject name: 'Stream already disposed'."); ;
        }

        [TestMethod]
        public void ObjectDisposedExceptionShouldNotBeSurfacedDirectlyOnBatchOnFailureInGetStream()
        {
            var test = GetQueryWithInjectedObjectDisposedOnGetStream(true /*BatchExecute*/);

            test.ShouldThrow<ObjectDisposedException>().WithMessage("Cannot access a disposed object.\r\nObject name: 'Stream already disposed'."); ;
        }

        [TestMethod]
        public void ObjectDisposedExceptionShouldNotBeSurfacedDirectlyOnNonBatchExecute()
        {
            var test = GetQueryWithInjectedObjectDisposedOnGetStream(false /*BatchExecute*/);

            test.ShouldThrow<DataServiceQueryException>().WithInnerException<ObjectDisposedException>().WithInnerMessage("Cannot access a disposed object.\r\nObject name: 'Stream already disposed'.");
        }

        private static Action AddObjectWithInjectedObjectDisposedOnGetStreamO(SaveChangesOptions saveChangesOptions)
        {
            IODataRequestMessage requestMessage = new ODataTestMessage();
            var responseMessage = CreateResponseMessageWithGetStreamThrowingObjectDisposeException();

            var context = new DataServiceContextWithCustomTransportLayer(ODataProtocolVersion.V4, requestMessage, responseMessage);
            context.AddObject("Products", new SimpleNorthwind.Product() { ID = 1 });
            return  () => context.SaveChanges(saveChangesOptions);
        }

        private static Action GetQueryWithInjectedObjectDisposedOnGetStream(bool useExecuteBatch)
        {
            IODataRequestMessage requestMessage = new ODataTestMessage();
            var responseMessage = CreateResponseMessageWithGetStreamThrowingObjectDisposeException();

            var context = new DataServiceContextWithCustomTransportLayer(ODataProtocolVersion.V4, requestMessage, responseMessage);
            if (useExecuteBatch)
            {
                return () => context.ExecuteBatch(context.CreateQuery<NorthwindModel.Products>("Products"));
            }
            else
            {
                return () => context.CreateQuery<NorthwindModel.Products>("Products").ToList();
            }
        }

        private static TopLevelBatchResponseMessage CreateResponseMessageWithGetStreamThrowingObjectDisposeException()
        {
            var responseMessage = new TopLevelBatchResponseMessage();
            responseMessage.SetHeader("OData-Version", "4.0");
            responseMessage.SetHeader("Content-Type", "application/atom+xml");
            responseMessage.SetHeader("Content-Length", "0");
            responseMessage.StatusCode = 400;
            responseMessage.GetStreamFunc = () => { throw new ObjectDisposedException("Stream already disposed"); };
            return responseMessage;
        }

        public class TopLevelBatchResponseMessage : IODataResponseMessage
        {
            private Dictionary<string, string> headers;

            public TopLevelBatchResponseMessage()
            {
                this.headers = new Dictionary<string, string>();
            }

            public IEnumerable<KeyValuePair<string, string>> Headers
            {
                get { return headers; }
            }

            public int StatusCode { get; set; }

            public Func<Stream> GetStreamFunc { get; set; }

            public string GetHeader(string headerName)
            {
                return this.headers[headerName];
            }

            public void SetHeader(string headerName, string headerValue)
            {
                this.headers[headerName] = headerValue;
            }

            public Stream GetStream()
            {
                return this.GetStreamFunc();
            }
        }
    }
}