//---------------------------------------------------------------------
// <copyright file="DisablePrimitiveTypeConversionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Text;
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
        public void JsonShouldConvertOpenPropertyValueToPayloadSpecifiedTypeEvenIfConversionIsDisabled()
        {
            var result = this.ReadPropertyValueInJson("OpenProperty", "AQ==", "Edm.Binary", this.settingsWithConversionDisabled);
            Assert.True(result is byte[]);
        }

        [Fact]
        public void JsonShouldConvertDeclaredPropertyValueToPayloadSpecifiedTypeEvenIfConversionIsDisabled()
        {
            var result = this.ReadPropertyValueInJson("String", "AQ==", "Edm.Binary", this.settingsWithConversionDisabled);
            Assert.True(result is byte[]);
        }

        [Fact]
        public void JsonShouldNotConvertDeclaredPropertyValueToMetadataTypeIfConversionIsDisabled()
        {
            var result = this.ReadPropertyValueInJson("Binary", "AQ==", null, this.settingsWithConversionDisabled);
            Assert.True(result is string);
        }

        [Fact]
        public void JsonShouldConvertDeclaredPropertyValueToMetadataTypeByDefault()
        {
            var result = this.ReadPropertyValueInJson("Binary", "AQ==", null, this.defaultSettings);
            Assert.True(result is byte[]);
        }

        [Fact]
        public void JsonShouldFailIfPayloadTypeDoesNotMatchMetadataTypeByDefault()
        {
            Action readWithWrongType = () => this.ReadPropertyValueInJson("String", "AQ==", "Edm.Binary", this.defaultSettings);
            readWithWrongType.Throws<ODataException>(ODataErrorStrings.ValidationUtils_IncompatibleType("Edm.Binary", "Edm.String"));
        }

        private object ReadPropertyValueInJson(string propertyName, string propertyValue, string typeName, ODataMessageReaderSettings settings)
        {
            var payload = CreateJsonPayload(propertyName, propertyValue, typeName);
            var property = this.ReadPropertyOfEntry(payload, propertyName, settings, "application/json;odata.metadata=minimal");
            
            return Assert.IsType<ODataProperty>(property).Value;
        }

        private static string CreateJsonPayload(string propertyName, string value, string type)
        {
            const string format = @"
                {{
                    ""@odata.context"": ""http://serviceRoot/$metadata#Entities/$entity"",
                    ""{0}"": ""{1}""
                    {2}
                }}";
            return string.Format(format, propertyName, value, type == null ? null : string.Format(", \"{0}@odata.type\":\"#{1}\"", propertyName, type));
        }


        private ODataPropertyInfo ReadPropertyOfEntry(string payload, string propertyName, ODataMessageReaderSettings settings, string contentType)
        {
            var message = new InMemoryMessage { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", contentType);
            var reader = new ODataMessageReader((IODataResponseMessage)message, settings, this.model);
            var entryReader = reader.CreateODataResourceReader(this.entityType);
            Assert.True(entryReader.Read());
            Assert.Equal(ODataReaderState.ResourceStart, entryReader.State);
            Assert.True(entryReader.Read());
            Assert.Equal(ODataReaderState.ResourceEnd, entryReader.State);
            ODataResource resource = entryReader.Item as ODataResource;

            Assert.Contains(resource.Properties, p => p.Name == propertyName);
            var property = resource.Properties.Single(p => p.Name == propertyName);
            
            return property;
        }
    }
}