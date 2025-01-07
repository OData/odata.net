//---------------------------------------------------------------------
// <copyright file="JsonODataAnnotationWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Unit tests for JsonODataAnnotationWriter
    /// </summary>
    public class JsonODataAnnotationWriterTests
    {
        private StringBuilder builder;
        private StringWriter stringWriter;
        private IJsonWriter jsonWriter;
        private JsonODataAnnotationWriter odataAnnotationWriter;

        public JsonODataAnnotationWriterTests()
        {
            this.builder = new StringBuilder();
            this.stringWriter = new StringWriter(builder);
            this.jsonWriter = new JsonWriter(this.stringWriter, isIeee754Compatible: true);
            this.odataAnnotationWriter = new JsonODataAnnotationWriter(
                this.jsonWriter, false, ODataVersion.V4);
        }

        [Fact]
        public async Task WriteInstanceAnnotationNameAsync_WritesInstanceAnnotationName()
        {
            await this.jsonWriter.StartObjectScopeAsync();
            await this.odataAnnotationWriter.WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataType);
            Assert.Equal("{\"@odata.type\":", this.builder.ToString());
        }

        [Theory]
        [InlineData("#NS.Customer", true)]
        [InlineData("NS.Customer", false)]
        public async Task WriteODataTypeInstanceAnnotationAsync_WritesODataTypeInstanceAnnotation(string typeName, bool writeRawValue)
        {
            await this.jsonWriter.StartObjectScopeAsync();
            await this.odataAnnotationWriter.WriteODataTypeInstanceAnnotationAsync(typeName, writeRawValue);
            Assert.Equal("{\"@odata.type\":\"#NS.Customer\"", this.builder.ToString());
        }

        [Fact]
        public async Task WritePropertyAnnotationNameAsync_WritesPropertyAnnotationName()
        {
            await this.jsonWriter.StartObjectScopeAsync();
            await this.odataAnnotationWriter.WritePropertyAnnotationNameAsync("FavoriteColor", ODataAnnotationNames.ODataType);
            Assert.Equal("{\"FavoriteColor@odata.type\":", this.builder.ToString());
        }

        [Fact]
        public async Task WriteODataTypePropertyAnnotationAsync_WritesODataTypePropertyAnnotation()
        {
            await this.jsonWriter.StartObjectScopeAsync();
            await this.odataAnnotationWriter.WriteODataTypePropertyAnnotationAsync("FavoriteColor", "NS.Color");
            Assert.Equal("{\"FavoriteColor@odata.type\":\"#NS.Color\"", this.builder.ToString());
        }
    }
}
