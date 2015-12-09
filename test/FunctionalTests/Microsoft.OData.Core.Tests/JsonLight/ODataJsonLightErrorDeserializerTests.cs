//---------------------------------------------------------------------
// <copyright file="ODataJsonLightErrorDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.JsonLight
{
    public class ODataJsonLightErrorDeserializerTests
    {
        [Fact]
        public void ReadTopLevelError_Works()
        {
            // Arrange
            const string payload =
                @"{""error"":{""code"":"""",""message"":"""",""target"":""any target"","
                + @"""details"":[{""code"":""500"",""target"":""another target"",""message"":""any msg""}]}}";
            var context = GetInputContext(payload);
            var deserializer = new ODataJsonLightErrorDeserializer(context);

            // Act
            var error = deserializer.ReadTopLevelError();
            
            // Assert
            Assert.Equal("any target", error.Target);
            Assert.Equal(1, error.Details.Count);
            var detail = error.Details.Single();
            Assert.Equal("500", detail.ErrorCode);
            Assert.Equal("another target", detail.Target);
            Assert.Equal("any msg", detail.Message);
        }

        [Fact]
        public void ReadTopLevelErrorAsync_Works()
        {
            // Arrange
            const string payload =
                @"{""error"":{""code"":"""",""message"":"""",""target"":""any target"","
                + @"""details"":[{""code"":""500"",""target"":""another target"",""message"":""any msg""}]}}";
            var context = GetInputContext(payload);
            var deserializer = new ODataJsonLightErrorDeserializer(context);

            // Act
            var error = deserializer.ReadTopLevelErrorAsync().Result;

            // Assert
            Assert.Equal("any target", error.Target);
            Assert.Equal(1, error.Details.Count);
            var detail = error.Details.Single();
            Assert.Equal("500", detail.ErrorCode);
            Assert.Equal("another target", detail.Target);
            Assert.Equal("any msg", detail.Message);
        }

        private ODataJsonLightInputContext GetInputContext(string payload)
        {
            return new ODataJsonLightInputContext(
                ODataFormat.Json,
                new MemoryStream(Encoding.UTF8.GetBytes(payload)),
                JsonLightUtils.JsonLightStreamingMediaType,
                Encoding.UTF8,
                new ODataMessageReaderSettings(),
                /*readingResponse*/ true,
                /*synchronous*/ true,
                new EdmModel(),
                /*urlResolver*/ null);
        }
    }
}
