//---------------------------------------------------------------------
// <copyright file="ODataAsyncReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Reader
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.TDD.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataAsyncReaderTests
    {
        private const string ServiceDocumentUri = "http://host/service";

        private Stream responseStream;
        private IODataResponseMessage responseMessage;
        private ODataMessageReader messageReader;

        private static EdmModel userModel;
        private static EdmEntityType testType;
        private static EdmSingleton singleton;

        static ODataAsyncReaderTests()
        {
            userModel = new EdmModel();

            testType = new EdmEntityType("NS", "Test");
            testType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            userModel.AddElement(testType);

            var defaultContainer = new EdmEntityContainer("NS", "DefaultContainer");
            userModel.AddElement(defaultContainer);

            singleton = new EdmSingleton(defaultContainer, "MySingleton", testType);
            defaultContainer.AddElement(singleton);
        }

        [TestMethod]
        public void ReadCompletedAsyncResponse()
        {
            string payload = "HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nOData-Version: 4.0\r\n\r\n{\"@odata.context\":\"http://host/service/$metadata#MySingleton\",\"Id\":1}";

            var asyncReader = this.CreateAsyncReader(payload);
            var asyncResponse = asyncReader.CreateResponseMessage();

            Assert.AreEqual(200, asyncResponse.StatusCode);
            Assert.AreEqual("application/json", asyncResponse.GetHeader("Content-Type"));
            Assert.AreEqual("4.0", asyncResponse.GetHeader("OData-Version"));

            using (var innerMessageReader = new ODataMessageReader(asyncResponse, new ODataMessageReaderSettings(), userModel))
            {
                var reader = innerMessageReader.CreateODataEntryReader();

                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.EntryEnd)
                    {
                        ODataEntry entry = reader.Item as ODataEntry;
                        Assert.AreEqual(1, entry.Properties.Single(p => p.Name == "Id").Value);
                    }
                }
            }
        }

        [TestMethod]
        public void CallCreateResponseMessageMoreThanOnceShouldFail()
        {
            string payload = "HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nOData-Version: 4.0\r\n\r\n{\"@odata.context\":\"http://host/service/$metadata#MySingleton\",\"Id\":1}";
            var asyncReader = this.CreateAsyncReader(payload);
            asyncReader.CreateResponseMessage();
            Action test = () => asyncReader.CreateResponseMessage();
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataAsyncReader_UnexpectedEndOfInput);
        }

        [TestMethod]
        public void ReadingIncompleteResponseMessageShouldFail()
        {
            string payload = "HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nOData-Version: 4.0\r\n";
            var asyncReader = this.CreateAsyncReader(payload);
            Action test = () => asyncReader.CreateResponseMessage();
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataAsyncReader_UnexpectedEndOfInput);
        }

        [TestMethod]
        public void ReadingBadStatusCodeShouldFail()
        {
            string payload = "HTTP/1.1 200.1 OK\r\nContent-Type: application/json\r\nOData-Version: 4.0\r\n\r\n{\"@odata.context\":\"http://host/service/$metadata#MySingleton\",\"Id\":1}";
            var asyncReader = this.CreateAsyncReader(payload);
            Action test = () => asyncReader.CreateResponseMessage();
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataAsyncReader_NonIntegerHttpStatusCode("200.1"));
        }

        [TestMethod]
        public void ReadingBadNewLineShouldFail()
        {
            string payload = "HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nOData-Version: 4.0\r\n\n{\"@odata.context\":\"http://host/service/$metadata#MySingleton\",\"Id\":1}";
            var asyncReader = this.CreateAsyncReader(payload);
            Action test = () => asyncReader.CreateResponseMessage();
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataAsyncReader_InvalidNewLineEncountered('\n'));
        }

        [TestMethod]
        public void ReadingBadHttpVersionShouldFail()
        {
            string payload = "HTTP/1.0 200 OK\r\nContent-Type: application/json\r\nOData-Version: 4.0\r\n\r\n{\"@odata.context\":\"http://host/service/$metadata#MySingleton\",\"Id\":1}";
            var asyncReader = this.CreateAsyncReader(payload);
            Action test = () => asyncReader.CreateResponseMessage();
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataAsyncReader_InvalidHttpVersionSpecified("HTTP/1.0", ODataConstants.HttpVersionInAsync));
        }

        [TestMethod]
        public void ReadingBadResponseLineVersionShouldFail()
        {
            string payload = "HTTP/1.1 200OK\r\nContent-Type: application/json\r\nOData-Version: 4.0\r\n\r\n{\"@odata.context\":\"http://host/service/$metadata#MySingleton\",\"Id\":1}";
            var asyncReader = this.CreateAsyncReader(payload);
            Action test = () => asyncReader.CreateResponseMessage();
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataAsyncReader_InvalidResponseLine("HTTP/1.1 200OK"));
        }

        private ODataAsynchronousReader CreateAsyncReader(string payload)
        {
            responseStream = new MemoryStream(Encoding.Default.GetBytes(payload));

            responseMessage = new InMemoryMessage { Stream = responseStream };
            responseMessage.SetHeader("Content-Type", "application/http");
            responseMessage.SetHeader("Content-Transfer-Encoding", "binary");

            this.messageReader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), userModel);

            return this.messageReader.CreateODataAsynchronousReader();
        }
    }
}
