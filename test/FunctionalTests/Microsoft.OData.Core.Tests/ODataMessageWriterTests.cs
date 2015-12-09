//---------------------------------------------------------------------
// <copyright file="ODataMessageWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using FluentAssertions;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    /// <summary>
    /// Unit tests for ODataMessageWriter.
    /// TODO: These unit tests do not provide complete coverage of ODataMessageWriter.
    /// </summary>
    public class ODataMessageWriterTests
    {
        [Fact]
        public void ConstructorWithRequestMessageAndJsonPaddingSettingEnabledFails()
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings { JsonPCallback = "functionName" };
            Action constructorCall = () => new ODataMessageWriter(new DummyRequestMessage(), settings);
            constructorCall.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_MessageWriterSettingsJsonPaddingOnRequestMessage);
        }

        [Fact]
        public void CreateCollectionWriterWithoutTypeShouldPassForJsonLight()
        {
            var settings = new ODataMessageWriterSettings();
            settings.SetContentType(ODataFormat.Json);
            var writer = new ODataMessageWriter(new DummyRequestMessage(), settings, new EdmModel());
            writer.CreateODataCollectionWriter(null).Should().BeOfType<ODataJsonLightCollectionWriter>();
        }

        [Fact]
        public void CreateCollectionWriterWithEntityCollectionTypeShouldFail()
        {
            var writer = new ODataMessageWriter(new DummyRequestMessage());
            var entityElementType = new EdmEntityTypeReference(new EdmEntityType("Fake", "Fake"), true);
            Action createWriterWithEntityCollectionType = () => writer.CreateODataCollectionWriter(entityElementType);
            createWriterWithEntityCollectionType.ShouldThrow<ODataException>().WithMessage(Strings.ODataMessageWriter_NonCollectionType("Fake.Fake"));
        }

        [Fact]
        public void CreateCollectionWriterWithEnumAsItemType()
        {
            var writer = new ODataMessageWriter(new DummyRequestMessage());
            var entityElementType = new EdmEnumTypeReference(new EdmEnumType("FakeNS", "FakeEnum"), true);
            var collectionWriter = writer.CreateODataCollectionWriter(entityElementType);
            Assert.True(collectionWriter != null, "CreateODataCollectionWriter with enum item type failed.");
        }

        [Fact]
        public void CreateCollectionWriterWithTypeDefinitionAsItemType()
        {
            var writer = new ODataMessageWriter(new DummyRequestMessage());
            var entityElementType = new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "Test", EdmPrimitiveTypeKind.Int32), false);
            var collectionWriter = writer.CreateODataCollectionWriter(entityElementType);
            Assert.True(collectionWriter != null, "CreateODataCollectionWriter with type definition item type failed.");
        }

        [Fact]
        public void CreateMessageWriterShouldNotSetAnnotationFilterWhenODataAnnotationsIsNotSetOnPreferenceAppliedHeader()
        {
            ODataMessageWriter writer = new ODataMessageWriter((IODataResponseMessage)new InMemoryMessage(), new ODataMessageWriterSettings());
            writer.Settings.ShouldIncludeAnnotation.Should().BeNull();
        }

        [Fact]
        public void CreateMessageWriterShouldSetAnnotationFilterWhenODataAnnotationIsSetOnPreferenceAppliedHeader()
        {
            IODataResponseMessage responseMessage = new InMemoryMessage();
            responseMessage.PreferenceAppliedHeader().AnnotationFilter = "*";
            ODataMessageWriter writer = new ODataMessageWriter(responseMessage, new ODataMessageWriterSettings());
            writer.Settings.ShouldIncludeAnnotation.Should().NotBeNull();
        }

        [Fact]
        public void WriteTopLevelUIntPropertyShouldWork()
        {
            var settings = new ODataMessageWriterSettings();
            settings.ODataUri.ServiceRoot = new Uri("http://host/service");
            settings.SetContentType(ODataFormat.Json);
            var model = new EdmModel();
            model.GetUInt32("MyNS", false);
            IODataRequestMessage request = new InMemoryMessage() { Stream = new MemoryStream() };
            var writer = new ODataMessageWriter(request, settings, model);
            Action write = () => writer.WriteProperty(new ODataProperty()
            {
                Name = "Id",
                Value = (UInt32)123
            });
            write.ShouldNotThrow();
            request.GetStream().Position = 0;
            var reader = new StreamReader(request.GetStream());
            string output = reader.ReadToEnd();
            output.Should().Be("{\"@odata.context\":\"http://host/service/$metadata#MyNS.UInt32\",\"value\":123}");
        }

        [Fact]
        public void WriteDeclaredUIntValueShouldWork()
        {
            var settings = new ODataMessageWriterSettings();
            var model = new EdmModel();
            model.GetUInt32("MyNS", false);
            var writer = new ODataMessageWriter(new DummyRequestMessage(), settings, model);
            Action write = () => writer.WriteValue((UInt32)123);
            write.ShouldNotThrow();
        }

        [Fact]
        public void WriteUndeclaredUIntValueShouldFail()
        {
            var settings = new ODataMessageWriterSettings();
            var model = new EdmModel();
            model.GetUInt32("MyNS", false);
            var writer = new ODataMessageWriter(new DummyRequestMessage(), settings, model);
            Action write = () => writer.WriteValue((UInt16)123);
            write.ShouldThrow<ODataException>().WithMessage("The value of type 'System.UInt16' could not be converted to a raw string.");
        }
    }
}
