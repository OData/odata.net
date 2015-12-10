//---------------------------------------------------------------------
// <copyright file="DeltaLinkWriterIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.IntegrationTests.Writer
{
    public class DeltaLinkWriterIntegrationTests
    {
        protected static readonly ODataPrimitiveValue PrimitiveValue1 = new ODataPrimitiveValue(123);
        protected static readonly ODataCollectionValue PrimitiveCollectionValue = new ODataCollectionValue { Items = new[] { "StringValue1", "StringValue2" }, TypeName = "Collection(Edm.String)" };

        protected static readonly EdmModel Model;
        protected static readonly EdmEntitySet EntitySet;
        protected static readonly EdmEntityType EntityType;

        protected static readonly Uri tempUri = new Uri("http://tempuri.org");

        static DeltaLinkWriterIntegrationTests()
        {
            Model = new EdmModel();
            EntityType = new EdmEntityType("TestNamespace", "TestEntityType");
            Model.AddElement(EntityType);

            var keyProperty = new EdmStructuralProperty(EntityType, "ID", EdmCoreModel.Instance.GetInt32(false));
            EntityType.AddKeys(new IEdmStructuralProperty[] { keyProperty });
            EntityType.AddProperty(keyProperty);
            var resourceNavigationProperty = EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "ResourceNavigationProperty", Target = EntityType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
            var resourceSetNavigationProperty = EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "ResourceSetNavigationProperty", Target = EntityType, TargetMultiplicity = EdmMultiplicity.Many });

            var defaultContainer = new EdmEntityContainer("TestNamespace", "DefaultContainer");
            Model.AddElement(defaultContainer);
            EntitySet = new EdmEntitySet(defaultContainer, "TestEntitySet", EntityType);
            EntitySet.AddNavigationTarget(resourceNavigationProperty, EntitySet);
            EntitySet.AddNavigationTarget(resourceSetNavigationProperty, EntitySet);
            defaultContainer.AddElement(EntitySet);
        }

        protected void WriteAnnotationsAndValidatePayload(Action<ODataWriter> action, ODataFormat format, string expectedPayload, bool request, bool createFeedWriter)
        {
            var writerSettings = new ODataMessageWriterSettings { DisableMessageStreamDisposal = true, EnableAtom = true };
            writerSettings.SetContentType(format);
            writerSettings.SetServiceDocumentUri(new Uri("http://www.example.com/"));

            MemoryStream stream = new MemoryStream();
            if (request)
            {
                IODataRequestMessage requestMessageToWrite = new InMemoryMessage { Method = "GET", Stream = stream };
                using (var messageWriter = new ODataMessageWriter(requestMessageToWrite, writerSettings, Model))
                {
                    ODataWriter odataWriter = createFeedWriter ? messageWriter.CreateODataFeedWriter(EntitySet, EntityType) : messageWriter.CreateODataEntryWriter(EntitySet, EntityType);
                    action(odataWriter);
                }
            }
            else
            {
                IODataResponseMessage responseMessageToWrite = new InMemoryMessage { StatusCode = 200, Stream = stream };
                using (var messageWriter = new ODataMessageWriter(responseMessageToWrite, writerSettings, Model))
                {
                    ODataWriter odataWriter = createFeedWriter ? messageWriter.CreateODataFeedWriter(EntitySet, EntityType) : messageWriter.CreateODataEntryWriter(EntitySet, EntityType);
                    action(odataWriter);
                }
            }

            stream.Position = 0;
            string payload = (new StreamReader(stream)).ReadToEnd();
            if (format == ODataFormat.Atom)
            {
                // The <updated> element is computed dynamically, so we remove it from the both the baseline and the actual payload.
                payload = Regex.Replace(payload, "<updated>[^<]*</updated>", "");
                expectedPayload = Regex.Replace(expectedPayload, "<updated>[^<]*</updated>", "");
            }

            Assert.Equal(expectedPayload, payload);
        }
    }
}
