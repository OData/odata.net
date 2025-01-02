//---------------------------------------------------------------------
// <copyright file="ODataJsonWriterUtilsTests.cs" company="Microsoft">
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
    /// Unit tests for ODataJsonWriterUtils
    /// </summary>
    public class ODataJsonWriterUtilsTests
    {
        private StringBuilder builder;
        private StringWriter stringWriter;
        private IJsonWriter jsonWriter;

        public ODataJsonWriterUtilsTests()
        {
            this.builder = new StringBuilder();
            this.stringWriter = new StringWriter(builder);
            this.jsonWriter = new JsonWriter(this.stringWriter, isIeee754Compatible: true);
        }

        [Fact]
        public void WriteValuePropertyName_WritesValuePropertyName()
        {
            this.jsonWriter.StartObjectScope();
            ODataJsonWriterUtils.WriteValuePropertyName(this.jsonWriter);
            Assert.Equal("{\"value\":", this.builder.ToString());
        }

        [Fact]
        public void WritePropertyAnnotationName_WritesPropertyAnnotationName()
        {
            this.jsonWriter.StartObjectScope();
            ODataJsonWriterUtils.WritePropertyAnnotationName(this.jsonWriter, "FavoriteColor", "odata.type");
            Assert.Equal("{\"FavoriteColor@odata.type\":", this.builder.ToString());
        }

        [Fact]
        public void WriteInstanceAnnotationName_WritesInstanceAnnotationName()
        {
            this.jsonWriter.StartObjectScope();
            ODataJsonWriterUtils.WriteInstanceAnnotationName(this.jsonWriter, "odata.type");
            Assert.Equal("{\"@odata.type\":", this.builder.ToString());
        }

        [Fact]
        public async Task WriteValuePropertyNameAsync_WritesValuePropertyName()
        {
            await this.jsonWriter.StartObjectScopeAsync();
            await ODataJsonWriterUtils.WriteValuePropertyNameAsync(this.jsonWriter);
            Assert.Equal("{\"value\":", this.builder.ToString());
        }

        [Fact]
        public async Task WritePropertyAnnotationNameAsync_WritesPropertyAnnotationName()
        {
            await this.jsonWriter.StartObjectScopeAsync();
            await ODataJsonWriterUtils.WritePropertyAnnotationNameAsync(this.jsonWriter, "FavoriteColor", "odata.type");
            Assert.Equal("{\"FavoriteColor@odata.type\":", this.builder.ToString());
        }

        [Fact]
        public async Task WriteInstanceAnnotationNameAsync_WritesInstanceAnnotationName()
        {
            await this.jsonWriter.StartObjectScopeAsync();
            await ODataJsonWriterUtils.WriteInstanceAnnotationNameAsync(this.jsonWriter, "odata.type");
            Assert.Equal("{\"@odata.type\":", this.builder.ToString());
        }
    }
}
