//---------------------------------------------------------------------
// <copyright file="ODataBinaryStreamWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    /// <summary>
    /// Unit tests for the ODataBinaryStreamWriter class.
    /// </summary>
    public class ODataBinaryStreamWriterTests
    {
        private StringBuilder builder;
        private Ref<char[]> buffer;
        private ODataBinaryStreamWriter writer;

        public ODataBinaryStreamWriterTests()
        {
            this.builder = new StringBuilder();
            this.buffer = new Ref<char[]>();
            this.writer = new ODataBinaryStreamWriter(new StringWriter(this.builder), this.buffer, null);
        }

        [Fact]
        public void WriteByteArray()
        {
            var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

            this.writer.Write(bytes, 0, 4);
            this.writer.Write(bytes, 4, 4);
            this.writer.Write(bytes, 8, 2);
            // Calling Dispose should cause the trailing bytes to be written
            this.writer.Dispose();

            Assert.Equal("AQIDBAUGBwgJAA==", this.builder.ToString());
        }

        [Fact]
        public async Task WriteAsyncByteArray()
        {
            var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

            await this.writer.WriteAsync(bytes, 0, 4, CancellationToken.None);
            await this.writer.WriteAsync(bytes, 4, 4, CancellationToken.None);
            await this.writer.WriteAsync(bytes, 8, 2, CancellationToken.None);
            // Calling Dispose should cause the trailing bytes to be written
            this.writer.Dispose();

            Assert.Equal("AQIDBAUGBwgJAA==", this.builder.ToString());
        }
    }
}
