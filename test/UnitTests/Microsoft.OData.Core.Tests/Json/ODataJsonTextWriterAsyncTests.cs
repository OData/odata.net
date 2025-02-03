//---------------------------------------------------------------------
// <copyright file="ODataJsonTextWriterAsyncTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Asynchronous unit tests for the ODataJsonTextWriter class.
    /// </summary>
    public class ODataJsonTextWriterAsyncTests
    {
        private StringBuilder builder;
        private Ref<char[]> buffer;
        private ODataJsonTextWriter writer;

        public ODataJsonTextWriterAsyncTests()
        {
            this.builder = new StringBuilder();
            this.buffer = new Ref<char[]>();
            this.writer = new ODataJsonTextWriter(new StringWriter(this.builder), this.buffer, null);
        }

        [Fact]
        public async Task WriteAsyncChar()
        {
            await this.writer.WriteAsync('\\');
            Assert.Equal("\\\\", this.builder.ToString());
        }

        [Fact]
        public async Task WriteAsyncCharSubarray()
        {
            char[] array = new char[] { 'C', ':', '\\', 'W', 'i', 'n', 'd', 'o', 'w', 's' };
            await this.writer.WriteAsync(array);
            Assert.Equal("C:\\\\Windows", this.builder.ToString());
        }

        [Fact]
        public async Task WriteAsyncString()
        {
            await this.writer.WriteAsync("C:\\Windows");
            Assert.Equal("C:\\\\Windows", this.builder.ToString());
        }

        [Fact]
        public async Task WriteLineAsyncChar()
        {
            await this.writer.WriteLineAsync('\\');
            Assert.Equal("\\\\\r\n", this.builder.ToString());
        }

        [Fact]
        public async Task WriteLineAsyncCharSubarray()
        {
            char[] array = new char[] { 'C', ':', '\\', 'W', 'i', 'n', 'd', 'o', 'w', 's' };
            await this.writer.WriteLineAsync(array);
            Assert.Equal("C:\\\\Windows\r\n", this.builder.ToString());
        }

        [Fact]
        public async Task WriteLineAsyncString()
        {
            await this.writer.WriteLineAsync("C:\\Windows");
            Assert.Equal("C:\\\\Windows\r\n", this.builder.ToString());
        }

        [Fact]
        public async Task TestWriteLineAsync()
        {
            await this.writer.WriteLineAsync();
            await this.writer.FlushAsync();
            Assert.Equal("\r\n", this.builder.ToString());
        }
    }
}
