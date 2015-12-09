//---------------------------------------------------------------------
// <copyright file="BufferingJsonReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Linq;
using Microsoft.OData.Core.Json;
using Xunit;

namespace Microsoft.OData.Core.Tests.Json
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
            var reader = new StringReader(payload);
            var jsonReader = new BufferingJsonReader(reader, "any", 0, ODataFormat.Json, false);
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
