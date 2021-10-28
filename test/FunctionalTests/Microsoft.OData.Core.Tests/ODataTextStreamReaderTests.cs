//---------------------------------------------------------------------
// <copyright file="ODataTextStreamReaderTests.cs" company="Microsoft">
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
    /// Unit tests for <see cref="ODataTextStreamReader"/>.
    /// </summary>
    public class ODataTextStreamReaderTests
    {
        private MemoryStream stream;
        private const int maxLength = 14;

        public ODataTextStreamReaderTests()
        {
            this.stream = new MemoryStream(Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog."));
            this.stream.Position = 16;
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
        public void ODataTextStreamReaderShouldRead()
        {
            using (var reader = new ODataTextStreamReader(new StreamReaderDelegate(this.ReadChars)))
            {
                var charBuffer = new char[maxLength];

                var charsRead = reader.Read(charBuffer, 0, maxLength);

                Assert.Equal(charsRead, maxLength);
                Assert.Equal("fox jumps over", new string(charBuffer));
            }
        }

        [Fact]
        public async Task ODataTextStreamReaderShouldReadAsync()
        {
            using (var reader = new ODataTextStreamReader(new AsyncStreamReaderDelegate(this.ReadCharsAsync)))
            {
                var charBuffer = new char[maxLength];

                var charsRead = await reader.ReadAsync(charBuffer, 0, maxLength);

                Assert.Equal(charsRead, maxLength);
                Assert.Equal("fox jumps over", new string(charBuffer));
            }
        }
    }
}
