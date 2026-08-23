//---------------------------------------------------------------------
// <copyright file="ODataUtf8JsonWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Json;
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

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(3, 0)]
        [InlineData(2048, 1)]
        [InlineData(2049, 2)]
        public void DisposeTwice_ReturnsEachRentedBufferOnce(int payloadLength, int expectedRentCount)
        {
            var output = new MemoryStream();
            var jsonWriter = new ODataUtf8JsonWriter(output, false, Encoding.UTF8, leaveStreamOpen: true);
            var arrayPool = new TrackingArrayPool<byte>();
            var stream = new ODataUtf8JsonWriteStream(jsonWriter, arrayPool);
            byte[] payload = Enumerable.Range(0, payloadLength).Select(i => (byte)i).ToArray();

            stream.Write(payload, 0, payload.Length);
            stream.Dispose();
            stream.Dispose();
            jsonWriter.Flush();

            Assert.Equal(Convert.ToBase64String(payload), Encoding.UTF8.GetString(output.ToArray()));
            Assert.Equal(expectedRentCount, arrayPool.RentCount);
            Assert.Equal(arrayPool.RentCount, arrayPool.ReturnCount);
        }

        [Fact]
        public async Task MixedDispose_ReturnsEachRentedBufferOnce()
        {
            var output = new MemoryStream();
            var jsonWriter = new ODataUtf8JsonWriter(output, false, Encoding.UTF8, leaveStreamOpen: true);
            var arrayPool = new TrackingArrayPool<byte>();
            var stream = new ODataUtf8JsonWriteStream(jsonWriter, arrayPool);

            await stream.WriteAsync(new byte[2049], 0, 2049);
            stream.Write(new byte[3], 0, 3);
            await stream.DisposeAsync();
            stream.Dispose();
            await jsonWriter.FlushAsync();

            Assert.Equal(2, arrayPool.RentCount);
            Assert.Equal(arrayPool.RentCount, arrayPool.ReturnCount);
        }
    }

    internal sealed class TrackingArrayPool<T> : ArrayPool<T>
    {
        private readonly HashSet<T[]> rentedArrays = new HashSet<T[]>();
        private readonly HashSet<T[]> returnedArrays = new HashSet<T[]>();

        public int RentCount => this.rentedArrays.Count;

        public int ReturnCount => this.returnedArrays.Count;

        public override T[] Rent(int minimumLength)
        {
            var array = new T[minimumLength];
            this.rentedArrays.Add(array);
            return array;
        }

        public override void Return(T[] array, bool clearArray = false)
        {
            Assert.Contains(array, this.rentedArrays);
            Assert.True(this.returnedArrays.Add(array), "The same pooled array was returned more than once.");
        }
    }
}
