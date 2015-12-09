//---------------------------------------------------------------------
// <copyright file="ODataAsynchronousWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataAsynchronousWriterTests
    {
        private const string ServiceDocumentUri = "http://host/service";

        private Stream responseStream;
        private IODataResponseMessage responseMessage;
        private ODataMessageWriter messageWriter;

        private EdmModel userModel;
        private EdmEntityType testType;
        private EdmSingleton singleton;

        [Fact]
        public void WriteCompletedAsyncResponse()
        {
            var asyncWriter = this.TestInit();

            var innerMessage = asyncWriter.CreateResponseMessage();
            innerMessage.StatusCode = 200;
            innerMessage.SetHeader("Content-Type", "application/json");

            var settings = new ODataMessageWriterSettings();
            settings.SetServiceDocumentUri(new Uri(ServiceDocumentUri));
            settings.DisableMessageStreamDisposal = true;

            using (var innerMessageWriter = new ODataMessageWriter(innerMessage, settings, this.userModel))
            {
                var entryWriter = innerMessageWriter.CreateODataEntryWriter(singleton, testType);
                var entry = new ODataEntry() {TypeName = "NS.Test", Properties = new[] {new ODataProperty() {Name = "Id", Value = 1}}};
                entryWriter.WriteStart(entry);
                entryWriter.WriteEnd();
            }

            asyncWriter.Flush();

            var payload = this.TestFinish();
            Assert.Equal("HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nOData-Version: 4.0\r\n\r\n{\"@odata.context\":\"http://host/service/$metadata#MySingleton\",\"Id\":1}", payload);
        }

        [Fact]
        public void CallCreateResponseMessageMoreThanOnceShouldFail()
        {
            var asyncWriter = this.TestInit();
            asyncWriter.CreateResponseMessage();
            Action test = () => asyncWriter.CreateResponseMessage();
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataAsyncWriter_CannotCreateResponseMoreThanOnce);
        }

        private ODataAsynchronousWriter TestInit()
        {
            this.userModel = new EdmModel();

            testType = new EdmEntityType("NS", "Test");
            testType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            this.userModel.AddElement(testType);

            var defaultContainer = new EdmEntityContainer("NS", "DefaultContainer");
            this.userModel.AddElement(defaultContainer);

            this.singleton = new EdmSingleton(defaultContainer, "MySingleton", this.testType);
            defaultContainer.AddElement(this.singleton);

            responseStream = new MemoryStream();
            responseMessage = new InMemoryMessage { Stream = responseStream };
            messageWriter = new ODataMessageWriter(responseMessage);
            return messageWriter.CreateODataAsynchronousWriter();
        }

        private string TestFinish()
        {
            responseStream.Seek(0, SeekOrigin.Begin);
            var streamReader = new StreamReader(responseStream);
            return streamReader.ReadToEnd();
        }
    }
}
