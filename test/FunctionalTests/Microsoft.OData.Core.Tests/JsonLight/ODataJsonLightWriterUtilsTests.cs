//---------------------------------------------------------------------
// <copyright file="ODataJsonLightWriterUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Json;
using Microsoft.OData.JsonLight;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    /// <summary>
    /// Unit tests for ODataJsonLightWriterUtils
    /// </summary>
    public class ODataJsonLightWriterUtilsTests
    {
        private StringBuilder builder;
        private StringWriter stringWriter;
        private IJsonWriter jsonWriter;
        private IJsonWriterAsync asyncJsonWriter;

        public ODataJsonLightWriterUtilsTests()
        {
            this.builder = new StringBuilder();
            this.stringWriter = new StringWriter(builder);
            this.jsonWriter = new JsonWriter(this.stringWriter, isIeee754Compatible: true);
            this.asyncJsonWriter = new JsonWriter(this.stringWriter, isIeee754Compatible: true);
        }

        [Fact]
        public void WriteValuePropertyName_WritesValuePropertyName()
        {
            this.jsonWriter.StartObjectScope();
            ODataJsonLightWriterUtils.WriteValuePropertyName(this.jsonWriter);
            Assert.Equal("{\"value\":", this.builder.ToString());
        }

        [Fact]
        public void WritePropertyAnnotationName_WritesPropertyAnnotationName()
        {
            this.jsonWriter.StartObjectScope();
            ODataJsonLightWriterUtils.WritePropertyAnnotationName(this.jsonWriter, "FavoriteColor", "odata.type");
            Assert.Equal("{\"FavoriteColor@odata.type\":", this.builder.ToString());
        }

        [Fact]
        public void WriteInstanceAnnotationName_WritesInstanceAnnotationName()
        {
            this.jsonWriter.StartObjectScope();
            ODataJsonLightWriterUtils.WriteInstanceAnnotationName(this.jsonWriter, "odata.type");
            Assert.Equal("{\"@odata.type\":", this.builder.ToString());
        }

        [Fact]
        public async Task WriteValuePropertyNameAsync_WritesValuePropertyName()
        {
            await this.asyncJsonWriter.StartObjectScopeAsync();
            await ODataJsonLightWriterUtils.WriteValuePropertyNameAsync(this.asyncJsonWriter);
            Assert.Equal("{\"value\":", this.builder.ToString());
        }

        [Fact]
        public async Task WritePropertyAnnotationNameAsync_WritesPropertyAnnotationName()
        {
            await this.asyncJsonWriter.StartObjectScopeAsync();
            await ODataJsonLightWriterUtils.WritePropertyAnnotationNameAsync(this.asyncJsonWriter, "FavoriteColor", "odata.type");
            Assert.Equal("{\"FavoriteColor@odata.type\":", this.builder.ToString());
        }

        [Fact]
        public async Task WriteInstanceAnnotationNameAsync_WritesInstanceAnnotationName()
        {
            await this.asyncJsonWriter.StartObjectScopeAsync();
            await ODataJsonLightWriterUtils.WriteInstanceAnnotationNameAsync(this.asyncJsonWriter, "odata.type");
            Assert.Equal("{\"@odata.type\":", this.builder.ToString());
        }
    }
}
