//---------------------------------------------------------------------
// <copyright file="WriteFeedWithoutNavigationTargetTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Writer.JsonLight
{
    public class WriteFeedWithoutNavigationSourceTests
    {
        private EdmModel myModel;
        private MemoryStream stream;

        #region Entities

        private static readonly ODataFeed feed = new ODataFeed();

        private static readonly ODataEntry entry = new ODataEntry
        {
            TypeName = "NS.DeriviedTypeA",
            Properties = new List<ODataProperty>
            {
                new ODataProperty { Name = "AId", Value = 1 }
            },
        };
        #endregion

        #region Test Cases
        [Fact]
        public void WriteFeedMissingNavigationSourceWithSerializationInfoWithModel()
        {
            this.TestInit();

            feed.SerializationInfo = new ODataFeedAndEntrySerializationInfo()
            {
                IsFromCollection = true,
                NavigationSourceEntityTypeName = "NS.BaseType",
                NavigationSourceKind = EdmNavigationSourceKind.UnknownEntitySet,
                NavigationSourceName = null,
            };

            using (var messageWriter = this.CreateMessageWriter(true))
            {
                var writer = messageWriter.CreateODataFeedWriter(entitySet: null, entityType: this.GetBaseType());
                writer.WriteStart(feed);
                writer.WriteStart(entry);
                writer.WriteEnd(); // entry
                writer.WriteEnd(); // feed
                writer.Flush();
            }

            string payload = this.TestFinish();
            payload.Should().Contain("\"@odata.context\":\"http://host/service/$metadata#Collection(NS.BaseType)\"");
        }

        [Fact]
        public void WriteFeedMissingNavigationSourceWithSerializationInfoWithoutModel()
        {
            this.TestInit();

            feed.SerializationInfo = new ODataFeedAndEntrySerializationInfo()
            {
                IsFromCollection = true,
                NavigationSourceEntityTypeName = "NS.BaseType",
                NavigationSourceKind = EdmNavigationSourceKind.UnknownEntitySet,
                NavigationSourceName = null,
            };

            using (var messageWriter = this.CreateMessageWriter(false))
            {
                var writer = messageWriter.CreateODataFeedWriter(entitySet: null, entityType: this.GetBaseType());
                writer.WriteStart(feed);
                writer.WriteStart(entry);
                writer.WriteEnd(); // entry
                writer.WriteEnd(); // feed
                writer.Flush();
            }

            string payload = this.TestFinish();
            payload.Should().Contain("\"@odata.context\":\"http://host/service/$metadata#Collection(NS.BaseType)\"");
        }
        #endregion

        #region Private Methods

        private void TestInit()
        {
            this.stream = new MemoryStream();
        }

        private IEdmModel GetModel()
        {
            if (myModel == null)
            {
                myModel = new EdmModel();

                var baseType = new EdmEntityType("NS", "BaseType", baseType: null, isAbstract: true, isOpen: false);
                myModel.AddElement(baseType);

                var deriviedTypeA = new EdmEntityType("NS", "DeriviedTypeA", baseType);
                deriviedTypeA.AddKeys(deriviedTypeA.AddStructuralProperty("AId", EdmPrimitiveTypeKind.Int32));
                myModel.AddElement(deriviedTypeA);

                var deriviedTypeB = new EdmEntityType("NS", "DeriviedTypeB", baseType);
                deriviedTypeB.AddKeys(deriviedTypeB.AddStructuralProperty("BId", EdmPrimitiveTypeKind.Int32));
                myModel.AddElement(deriviedTypeB);

                var container = new EdmEntityContainer("MyNS", "Default");
                container.AddEntitySet("SetA", deriviedTypeA);
                container.AddEntitySet("SetB", deriviedTypeB);
                myModel.AddElement(container);
            }

            return myModel;
        }

        private string TestFinish()
        {
            this.stream.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(this.stream))
            {
                return reader.ReadToEnd();
            }
        }

        private IEdmEntityType GetBaseType()
        {
            var model = this.GetModel();
            return (IEdmEntityType)model.FindDeclaredType("NS.BaseType");
        }

        private ODataMessageWriter CreateMessageWriter(bool useModel)
        {
            var responseMessage = new TestResponseMessage(this.stream);
            var writerSettings = new ODataMessageWriterSettings { DisableMessageStreamDisposal = true };
            writerSettings.SetServiceDocumentUri(new Uri("http://host/service"));
            var model = useModel ? this.GetModel() : null;
            return new ODataMessageWriter(responseMessage, writerSettings, model);
        }

        private class TestResponseMessage : IODataResponseMessage
        {
            private readonly Dictionary<string, string> headers =
                new Dictionary<string, string>();
            private readonly Stream stream;

            public TestResponseMessage(Stream stream)
            {
                this.stream = stream;
            }

            public IEnumerable<KeyValuePair<string, string>> Headers
            {
                get { return this.headers; }
            }

            public int StatusCode { get; set; }

            public string GetHeader(string headerName)
            {
                string headerValue;
                if (this.headers.TryGetValue(headerName, out headerValue))
                {
                    return headerValue;
                }

                return null;
            }

            public void SetHeader(string headerName, string headerValue)
            {
                this.headers[headerName] = headerValue;
            }

            public Stream GetStream()
            {
                return this.stream;
            }
        }

        #endregion
    }
}
