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
using Microsoft.OData.Edm.Library;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Reader
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

            this.defaultSettings = new ODataMessageReaderSettings { BaseUri = new Uri("http://serviceRoot/"), EnableAtom = true };
            this.settingsWithConversionDisabled = new ODataMessageReaderSettings(this.defaultSettings) { DisablePrimitiveTypeConversion = true, EnableAtom = true };
        }

        [Fact]
        public void AtomShouldConvertOpenPropertyValueToPayloadSpecifiedTypeEvenIfConversionIsDisabled()
        {
            this.ReadPropertyValueInAtom("OpenProperty", "AQ==", "Edm.Binary", this.settingsWithConversionDisabled).Should().BeAssignableTo<byte[]>();
        }

        [Fact]
        public void AtomShouldConvertDeclaredPropertyValueToPayloadSpecifiedTypeEvenIfConversionIsDisabled()
        {
            this.ReadPropertyValueInAtom("String", "AQ==", "Edm.Binary", this.settingsWithConversionDisabled).Should().BeAssignableTo<byte[]>();
        }

        [Fact]
        public void AtomShouldNotConvertDeclaredPropertyValueToMetadataTypeIfConversionIsDisabled()
        {
            this.ReadPropertyValueInAtom("Binary", "AQ==", null, this.settingsWithConversionDisabled).Should().BeAssignableTo<string>();
        }

        [Fact]
        public void AtomShouldConvertDeclaredPropertyValueToMetadataTypeByDefault()
        {
            this.ReadPropertyValueInAtom("Binary", "AQ==", null, this.defaultSettings).Should().BeAssignableTo<byte[]>();
        }

        [Fact]
        public void AtomShouldFailIfPayloadTypeDoesNotMatchMetadataTypeByDefault()
        {
            Action readWithWrongType = () => this.ReadPropertyValueInAtom("String", "AQ==", "Edm.Binary", this.defaultSettings);
            readWithWrongType.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ValidationUtils_IncompatibleType("Edm.Binary", "Edm.String"));
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

        private object ReadPropertyValueInAtom(string propertyName, string propertyValue, string typeName, ODataMessageReaderSettings settings)
        {
            var payload = CreateAtomPayload(propertyName, propertyValue, typeName);
            var property = this.ReadPropertyOfEntry(payload, propertyName, settings, "application/atom+xml");
            return property.Value;
        }

        private static string CreateAtomPayload(string propertyName, string value, string type)
        {
            const string format = @"
                <entry xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <content type=""application/xml"">
                    <m:properties>
                        <d:{0}{2}>{1}</d:{0}>
                    </m:properties>
                    </content>
                </entry>";
            return string.Format(format, propertyName, value, type == null ? null : string.Format(" m:type=\"{0}\"", type));
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
            var entryReader = reader.CreateODataEntryReader(this.entityType);
            entryReader.Read().Should().BeTrue();
            entryReader.State.Should().Be(ODataReaderState.EntryStart);
            entryReader.Read().Should().BeTrue();
            entryReader.State.Should().Be(ODataReaderState.EntryEnd);
            entryReader.Item.Should().BeAssignableTo<ODataEntry>();

            entryReader.Item.As<ODataEntry>().Properties.Should().Contain(p => p.Name == propertyName);
            var property = entryReader.Item.As<ODataEntry>().Properties.Single(p => p.Name == propertyName);
            return property;
        }
    }
}