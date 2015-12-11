//---------------------------------------------------------------------
// <copyright file="BufferingJsonReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Reader
{
    using System.IO;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BufferingJsonReaderTests
    {
        [TestMethod]
        public void StartBufferingAndTryToReadInStreamErrorPropertyValue_Works()
        {
            // Arrange
            const string payload =
                @"{""code"":"""",""message"":"""",""target"":""any target"","
                + @"""details"":[{""code"":""500"",""target"":""another target"",""message"":""any msg""}]}";
            var reader = new StringReader(payload);
            var jsonReader = new BufferingJsonReader(reader, "any", 0, ODataFormat.Json, false);
            ODataError error;

            // Act
            jsonReader.Read();
            var result = jsonReader.StartBufferingAndTryToReadInStreamErrorPropertyValue(out error);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("any target", error.Target);
            Assert.AreEqual(1, error.Details.Count);
            var detail = error.Details.Single();
            Assert.AreEqual("500", detail.ErrorCode);
            Assert.AreEqual("another target", detail.Target);
            Assert.AreEqual("any msg", detail.Message);
        }
    }
}
