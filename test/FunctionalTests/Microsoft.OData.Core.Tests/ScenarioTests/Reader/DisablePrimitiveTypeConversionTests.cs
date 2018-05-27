//---------------------------------------------------------------------
// <copyright file="DisablePrimitiveTypeConversionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.ScenarioTests.Reader
{
    public class DisablePrimitiveTypeConversionTests
    {
        private readonly EdmModel model;
        private readonly ODataMessageReaderSettings settingsWithConversionDisabled;
        private readonly ODataMessageReaderSettings defaultSettings;
        private readonly EdmEntityType entityType;

        public DisablePrimitiveTypeConversionTests()
        {
            this.model = new EdmModel();
            this.entityType = new EdmEntityType("FQ.NS", "EntityType", null, false, true);
            this.entityType.AddStructuralProperty("String", EdmPrimitiveTypeKind.String);
            this.entityType.AddStructuralProperty("Binary", EdmPrimitiveTypeKind.Binary);
            this.model.AddElement(this.entityType);
            var container = new EdmEntityContainer("FQ.NS", "Container");
            this.model.AddElement(container);
            container.AddEntitySet("Entities", this.entityType);

            this.defaultSettings = new ODataMessageReaderSettings { BaseUri = new Uri("http://serviceRoot/") };
            this.settingsWithConversionDisabled = this.defaultSettings.Clone();
            this.settingsWithConversionDisabled.EnablePrimitiveTypeConversion = false;
        }

        [Fact]
        public void JsonLightShouldConvertOpenPropertyValueToPayloadSpecifiedTypeEvenIfConversionIsDisabled()
        {
            this.ReadPropertyValueInJsonLight("OpenProperty", "AQ==", "Edm.Binary", this.settingsWithConversionDisabled).Should().BeAssignableTo<byte[]>();
        }

        [Fact]
        public void JsonLightShouldConvertDeclaredPropertyValueToPayloadSpecifiedTypeEvenIfConversionIsDisabled()
        {
            this.ReadPropertyValueInJsonLight("String", "AQ==", "Edm.Binary", this.settingsWithConversionDisabled).Should().BeAssignableTo<byte[]>();
        }

        [Fact]
        public void JsonLightShouldNotConvertDeclaredPropertyValueToMetadataTypeIfConversionIsDisabled()
        {
            this.ReadPropertyValueInJsonLight("Binary", "AQ==", null, this.settingsWithConversionDisabled).Should().BeAssignableTo<string>();
        }

        [Fact]
        public void JsonLightShouldConvertDeclaredPropertyValueToMetadataTypeByDefault()
        {
            this.ReadPropertyValueInJsonLight("Binary", "AQ==", null, this.defaultSettings).Should().BeAssignableTo<byte[]>();
        }

        [Fact]
        public void JsonLightShouldFailIfPayloadTypeDoesNotMatchMetadataTypeByDefault()
        {
            Action readWithWrongType = () => this.ReadPropertyValueInJsonLight("String", "AQ==", "Edm.Binary", this.defaultSettings);
            readWithWrongType.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ValidationUtils_IncompatibleType("Edm.Binary", "Edm.String"));
        }

        private object ReadPropertyValueInJsonLight(string propertyName, string propertyValue, string typeName, ODataMessageReaderSettings settings)
        {
            var payload = CreateJsonLightPayload(propertyName, propertyValue, typeName);
            var property = this.ReadPropertyOfEntry(payload, propertyName, settings, "application/json;odata.metadata=minimal");
            return property.Value;
        }

        private static string CreateJsonLightPayload(string propertyName, string value, string type)
        {
            const string format = @"
                {{
                    ""@odata.context"": ""http://serviceRoot/$metadata#Entities/$entity"",
                    ""{0}"": ""{1}""
                    {2}
                }}";
            return string.Format(format, propertyName, value, type == null ? null : string.Format(", \"{0}@odata.type\":\"#{1}\"", propertyName, type));
        }


        private ODataProperty ReadPropertyOfEntry(string payload, string propertyName, ODataMessageReaderSettings settings, string contentType)
        {
            var message = new InMemoryMessage { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", contentType);
            var reader = new ODataMessageReader((IODataResponseMessage)message, settings, this.model);
            var entryReader = reader.CreateODataResourceReader(this.entityType);
            entryReader.Read().Should().BeTrue();
            entryReader.State.Should().Be(ODataReaderState.ResourceStart);
            entryReader.Read().Should().BeTrue();
            entryReader.State.Should().Be(ODataReaderState.ResourceEnd);
            entryReader.Item.Should().BeAssignableTo<ODataResource>();

            entryReader.Item.As<ODataResource>().Properties.Should().Contain(p => p.Name == propertyName);
            var property = entryReader.Item.As<ODataResource>().Properties.Single(p => p.Name == propertyName);
            return property;
        }
    }
}