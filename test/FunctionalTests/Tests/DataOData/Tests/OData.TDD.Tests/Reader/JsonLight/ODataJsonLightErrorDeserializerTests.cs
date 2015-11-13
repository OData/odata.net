//---------------------------------------------------------------------
// <copyright file="ODataJsonLightErrorDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Reader.JsonLight
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.JsonLight;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.TDD.Tests.Common.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataJsonLightErrorDeserializerTests
    {
        [TestMethod]
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
            Assert.AreEqual("any target", error.Target);
            Assert.AreEqual(1, error.Details.Count);
            var detail = error.Details.Single();
            Assert.AreEqual("500", detail.ErrorCode);
            Assert.AreEqual("another target", detail.Target);
            Assert.AreEqual("any msg", detail.Message);
        }

        [TestMethod]
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
            Assert.AreEqual("any target", error.Target);
            Assert.AreEqual(1, error.Details.Count);
            var detail = error.Details.Single();
            Assert.AreEqual("500", detail.ErrorCode);
            Assert.AreEqual("another target", detail.Target);
            Assert.AreEqual("any msg", detail.Message);
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
