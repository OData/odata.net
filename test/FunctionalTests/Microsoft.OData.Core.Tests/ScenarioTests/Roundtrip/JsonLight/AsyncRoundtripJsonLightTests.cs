//---------------------------------------------------------------------
// <copyright file="AsyncRoundtripJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Roundtrip.JsonLight
{
    public class AsyncRoundtripJsonLightTests
    {
        private const string ServiceDocumentUri = "http://host/service";

        private static readonly EdmModel userModel;
        private static readonly EdmEntityType testType;
        private static readonly EdmEntityContainer defaultContainer;
        private static readonly EdmSingleton singleton;

        private ODataEntry entry;
        private Stream responseStream;

        static AsyncRoundtripJsonLightTests()
        {
            userModel = new EdmModel();

            testType = new EdmEntityType("NS", "Test");
            testType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            testType.AddStructuralProperty("Dummy", EdmPrimitiveTypeKind.String);
            userModel.AddElement(testType);

            defaultContainer = new EdmEntityContainer("NS", "DefaultContainer");
            userModel.AddElement(defaultContainer);

            singleton = new EdmSingleton(defaultContainer, "MySingleton", testType);
            defaultContainer.AddElement(singleton);
        }

        public AsyncRoundtripJsonLightTests()
        {
            entry = new ODataEntry
            {
                TypeName = "NS.Test",
                Properties = new[]
                {
                    new ODataProperty() { Name = "Id", Value = 1 },
                    new ODataProperty() { Name = "Dummy", Value = "null" }
                }
            };

            responseStream = new MemoryStream();
        }

        [Fact]
        public void AsyncRoundtripJsonLightTest()
        {
            this.WriteEntry();

            responseStream.Position = 0;

            ODataEntry entryRead = this.ReadEntry();

            Assert.True(entry.TypeName == entryRead.TypeName, "TypeName should be equal");
            Assert.True(this.PropertiesEqual(entry.Properties, entryRead.Properties), "Properties should be equal");
        }

        private void WriteEntry()
        {
            var asyncWriter = this.CreateAsyncWriter();

            var innerMessage = asyncWriter.CreateResponseMessage();
            innerMessage.StatusCode = 200;
            innerMessage.SetHeader("Content-Type", "application/json");

            var settings = new ODataMessageWriterSettings();
            settings.SetServiceDocumentUri(new Uri(ServiceDocumentUri));
            settings.DisableMessageStreamDisposal = true;

            using (var innerMessageWriter = new ODataMessageWriter(innerMessage, settings, userModel))
            {
                var entryWriter = innerMessageWriter.CreateODataEntryWriter(singleton, testType);
                entryWriter.WriteStart(entry);
                entryWriter.WriteEnd();
            }

            asyncWriter.Flush();
        }

        private ODataEntry ReadEntry()
        {
            var asyncReader = this.CreateAsyncReader();

            var asyncResponse = asyncReader.CreateResponseMessage();

            using (var innerMessageReader = new ODataMessageReader(asyncResponse, new ODataMessageReaderSettings(), userModel))
            {
                var reader = innerMessageReader.CreateODataEntryReader();

                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.EntryEnd)
                    {
                        return reader.Item as ODataEntry;
                    }
                }
            }

            return null;
        }

        private ODataAsynchronousWriter CreateAsyncWriter()
        {
            IODataResponseMessage responseMessage = new InMemoryMessage { Stream = responseStream };

            var messageWriter = new ODataMessageWriter(responseMessage);
            return messageWriter.CreateODataAsynchronousWriter();
        }

        private ODataAsynchronousReader CreateAsyncReader()
        {
            IODataResponseMessage responseMessage = new InMemoryMessage { Stream = responseStream };
            responseMessage.SetHeader("Content-Type", "application/http");
            responseMessage.SetHeader("Content-Transfer-Encoding", "binary");

            var messageReader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), userModel);
            return messageReader.CreateODataAsynchronousReader();
        }

        private bool PropertiesEqual(IEnumerable<ODataProperty> first, IEnumerable<ODataProperty> second)
        {
            List<ODataProperty> firstList = first.ToList();
            List<ODataProperty> secondList = second.ToList();
            
            if (firstList.Count != secondList.Count)
            {
                return false;
            }

            for (int i = 0; i < firstList.Count; ++i)
            {
                if (!firstList[i].Name.Equals(secondList[i].Name) || !firstList[i].Value.Equals(secondList[i].Value))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
