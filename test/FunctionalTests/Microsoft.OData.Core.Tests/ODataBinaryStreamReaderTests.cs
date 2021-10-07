//---------------------------------------------------------------------
// <copyright file="ODataBinaryStreamReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Tests
{
    /// <summary>
    /// Unit tests for <see cref="ODataBinaryStreamReader"/>.
    /// </summary>
    public class ODataBinaryStreamReaderTests
    {
        private MemoryStream stream;
        private const int maxLength = 10;

        public ODataBinaryStreamReaderTests()
        {
            this.stream = new MemoryStream(Encoding.UTF8.GetBytes("AQIDBAUGBwgJAA=="));
            this.stream.Position = 0;
        }

        private int ReadChars(char[] chars, int offset, int maxLength)
        {
            var buffer = new byte[maxLength];
            int bytesRead = this.stream.Read(buffer, offset, maxLength);
            buffer.CopyTo(chars, 0);

            return bytesRead;
        }

        private async Task<int> ReadCharsAsync(char[] chars, int offset, int maxLength)
        {
            var buffer = new byte[maxLength];
            int bytesRead = await this.stream.ReadAsync(buffer, offset, maxLength);
            buffer.CopyTo(chars, 0);

            return bytesRead;
        }

        [Fact]
        public void ODataBinaryStreamReaderShouldRead()
        {
            using (var reader = new ODataBinaryStreamReader(new Func<char[], int, int, int>(this.ReadChars)))
            {
                var buffer = new byte[maxLength];

                var bytesRead = reader.Read(buffer, 0, maxLength);

                Assert.Equal(bytesRead, maxLength);
                Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }, buffer);
            }
        }

        [Fact]
        public async Task ODataBinaryStreamReaderShouldReadAsync()
        {
            using (var reader = new ODataBinaryStreamReader(new Func<char[], int, int, Task<int>>(this.ReadCharsAsync)))
            {
                var buffer = new byte[maxLength];

                var bytesRead = await reader.ReadAsync(buffer, 0, maxLength);

                Assert.Equal(bytesRead, maxLength);
                Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }, buffer);
            }
        }
    }
}