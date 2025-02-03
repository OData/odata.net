//---------------------------------------------------------------------
// <copyright file="ODataUtf8JsonWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using static Microsoft.OData.Json.ODataUtf8JsonWriter;
using Xunit;
using System.IO;

namespace Microsoft.OData.Core.Tests.Json
{
    public class ODataUtf8JsonWriterStreamTests
    {
        [Fact]
        public void CanRead_ReturnsFalse()
        {
            var stream = new ODataUtf8JsonWriteStream(null); // Passing null for the ODataUtf8JsonWriter parameter as it's not relevant for this test
            Assert.False(stream.CanRead);
        }

        [Fact]
        public void CanSeek_ReturnsFalse()
        {
            var stream = new ODataUtf8JsonWriteStream(null);
            Assert.False(stream.CanSeek);
        }

        [Fact]
        public void CanWrite_ReturnsTrue()
        {
            var stream = new ODataUtf8JsonWriteStream(null);
            Assert.True(stream.CanWrite);
        }

        [Fact]
        public void Length_ThrowsNotSupportedException()
        {
            var stream = new ODataUtf8JsonWriteStream(null);
            Assert.Throws<NotSupportedException>(() => stream.Length);
        }

        [Fact]
        public void Position_Get_ThrowsNotSupportedException()
        {
            var stream = new ODataUtf8JsonWriteStream(null);
            Assert.Throws<NotSupportedException>(() => stream.Position);
        }

        [Fact]
        public void Position_Set_ThrowsNotSupportedException()
        {
            var stream = new ODataUtf8JsonWriteStream(null);
            Assert.Throws<NotSupportedException>(() => stream.Position = 0);
        }

        [Fact]
        public void Read_ThrowsNotSupportedException()
        {
            var stream = new ODataUtf8JsonWriteStream(null);
            byte[] buffer = new byte[10];
            Assert.Throws<NotSupportedException>(() => stream.Read(buffer, 0, buffer.Length));
        }

        [Fact]
        public void Seek_ThrowsNotSupportedException()
        {
            var stream = new ODataUtf8JsonWriteStream(null);
            Assert.Throws<NotSupportedException>(() => stream.Seek(0, SeekOrigin.Begin));
        }

        [Fact]
        public void SetLength_ThrowsNotSupportedException()
        {
            var stream = new ODataUtf8JsonWriteStream(null);
            Assert.Throws<NotSupportedException>(() => stream.SetLength(10));
        }
    }
}
