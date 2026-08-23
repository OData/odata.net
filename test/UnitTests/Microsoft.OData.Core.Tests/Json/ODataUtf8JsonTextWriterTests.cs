//---------------------------------------------------------------------
// <copyright file="ODataUtf8JsonTextWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Json;
using static Microsoft.OData.Json.ODataUtf8JsonWriter;
using Xunit;

namespace Microsoft.OData.Core.Tests.Json
{
    public class ODataUtf8JsonTextWriterTests
    {
        [Fact]
        public void Encoding_ThrowsNotImplementedException()
        {
            var stream = new ODataUtf8JsonTextWriter(null);
            Assert.Throws<NotSupportedException>(() => stream.Encoding);
        }

        [Fact]
        public async Task MixedDispose_ReturnsEachRentedBufferOnce()
        {
            var output = new MemoryStream();
            var jsonWriter = new ODataUtf8JsonWriter(output, false, Encoding.UTF8, leaveStreamOpen: true);
            var arrayPool = new TrackingArrayPool<char>();
            var textWriter = new ODataUtf8JsonTextWriter(jsonWriter, arrayPool);

            await textWriter.WriteAsync('a');
            await textWriter.DisposeAsync();
            textWriter.Dispose();

            Assert.Equal(1, arrayPool.RentCount);
            Assert.Equal(arrayPool.RentCount, arrayPool.ReturnCount);
        }
    }
}
