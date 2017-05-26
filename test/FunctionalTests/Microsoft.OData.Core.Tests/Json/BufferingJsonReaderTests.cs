//---------------------------------------------------------------------
// <copyright file="BufferingJsonReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Linq;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class BufferingJsonReaderTests
    {
        [Fact]
        public void StartBufferingAndTryToReadInStreamErrorPropertyValue_Works()
        {
            // Arrange
            const string payload =
                @"{""code"":"""",""message"":"""",""target"":""any target"","
                + @"""details"":[{""code"":""500"",""target"":""another target"",""message"":""any msg""}]}";
            var stringReader = new StringReader(payload);
            var innerReader = new JsonReader(stringReader, false);
            var jsonReader = new BufferingJsonReader(innerReader, "any", 0);
            ODataError error;

            // Act
            jsonReader.Read();
            var result = jsonReader.StartBufferingAndTryToReadInStreamErrorPropertyValue(out error);

            // Assert
            Assert.True(result);
            Assert.Equal("any target", error.Target);
            Assert.Equal(1, error.Details.Count);
            var detail = error.Details.Single();
            Assert.Equal("500", detail.ErrorCode);
            Assert.Equal("another target", detail.Target);
            Assert.Equal("any msg", detail.Message);
        }
    }
}
