//---------------------------------------------------------------------
// <copyright file="JsonLightODataAnnotationWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Json;
using Microsoft.OData.JsonLight;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    /// <summary>
    /// Unit tests for JsonLightODataAnnotationWriter
    /// </summary>
    public class JsonLightODataAnnotationWriterTests
    {
        private StringBuilder builder;
        private StringWriter stringWriter;
        private IJsonWriterAsync asyncJsonWriter;
        private JsonLightODataAnnotationWriter asyncODataAnnotationWriter;

        public JsonLightODataAnnotationWriterTests()
        {
            this.builder = new StringBuilder();
            this.stringWriter = new StringWriter(builder);
            this.asyncJsonWriter = (IJsonWriterAsync)new JsonWriter(this.stringWriter, isIeee754Compatible: true);
            this.asyncODataAnnotationWriter = new JsonLightODataAnnotationWriter(
                this.asyncJsonWriter, false, ODataVersion.V4);
        }

        [Fact]
        public async Task WriteInstanceAnnotationNameAsync_WritesInstanceAnnotationName()
        {
            await this.asyncJsonWriter.StartObjectScopeAsync();
            await this.asyncODataAnnotationWriter.WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataType);
            Assert.Equal("{\"@odata.type\":", this.builder.ToString());
        }

        [Theory]
        [InlineData("#NS.Customer", true)]
        [InlineData("NS.Customer", false)]
        public async Task WriteODataTypeInstanceAnnotationAsync_WritesODataTypeInstanceAnnotation(string typeName, bool writeRawValue)
        {
            await this.asyncJsonWriter.StartObjectScopeAsync();
            await this.asyncODataAnnotationWriter.WriteODataTypeInstanceAnnotationAsync(typeName, writeRawValue);
            Assert.Equal("{\"@odata.type\":\"#NS.Customer\"", this.builder.ToString());
        }

        [Fact]
        public async Task WritePropertyAnnotationNameAsync_WritesPropertyAnnotationName()
        {
            await this.asyncJsonWriter.StartObjectScopeAsync();
            await this.asyncODataAnnotationWriter.WritePropertyAnnotationNameAsync("FavoriteColor", ODataAnnotationNames.ODataType);
            Assert.Equal("{\"FavoriteColor@odata.type\":", this.builder.ToString());
        }

        [Fact]
        public async Task WriteODataTypePropertyAnnotationAsync_WritesODataTypePropertyAnnotation()
        {
            await this.asyncJsonWriter.StartObjectScopeAsync();
            await this.asyncODataAnnotationWriter.WriteODataTypePropertyAnnotationAsync("FavoriteColor", "NS.Color");
            Assert.Equal("{\"FavoriteColor@odata.type\":\"#NS.Color\"", this.builder.ToString());
        }
    }
}
