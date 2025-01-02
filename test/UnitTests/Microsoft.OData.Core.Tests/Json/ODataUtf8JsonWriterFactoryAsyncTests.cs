//---------------------------------------------------------------------
// <copyright file="ODataUtf8JsonWriterFactoryAsyncTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public sealed class ODataUtf8JsonWriterFactoryAsyncTests
    {

        #region Argument validation

        [Fact]
        public void CreateAsynchronousJsonWriter_ThrowsIfStreamIsNull()
        {
            ODataUtf8JsonWriterFactory factory = ODataUtf8JsonWriterFactory.Default;

            Assert.Throws<ArgumentNullException>("stream", () => factory.CreateJsonWriter(null, false, Encoding.UTF8));
        }

        [Fact]
        public void CreateAsynchronousJsonWriter_ThrowsIfEncodingIsNull()
        {
            ODataUtf8JsonWriterFactory factory = ODataUtf8JsonWriterFactory.Default;
            using MemoryStream stream = new MemoryStream();

            Assert.Throws<ArgumentNullException>("encoding", () => factory.CreateJsonWriter(stream, false, null));
        }

        #endregion

        public static IEnumerable<object[]> Encodings { get; } =
           new object[][]
           {
                new object[] { Encoding.UTF8 },
                new object[] { Encoding.Unicode },
                new object[] { Encoding.UTF32 },
                new object[] { Encoding.BigEndianUnicode },
                new object[] { Encoding.ASCII }
           };

        [Theory]
        [MemberData(nameof(Encodings))]
        public async Task CreateAsynchronousJsonWriterWithSpecifiedEncoding(Encoding encoding)
        {
            ODataUtf8JsonWriterFactory factory = ODataUtf8JsonWriterFactory.Default;
            using MemoryStream stream = new MemoryStream();
            IJsonWriter jsonWriter = factory.CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            await jsonWriter.StartObjectScopeAsync();
            await jsonWriter.WriteNameAsync("Foo");
            await jsonWriter.WriteValueAsync("Bar");
            await jsonWriter.WriteNameAsync("Fizz");
            await jsonWriter.WriteValueAsync(15.0m);
            await jsonWriter.WriteNameAsync("Buzz");
            await jsonWriter.WriteValueAsync("<\"\n");
            await jsonWriter.EndObjectScopeAsync();

            await jsonWriter.FlushAsync();
            using StreamReader reader = new StreamReader(stream, encoding);
            stream.Seek(0, SeekOrigin.Begin);
            string contents = await reader.ReadToEndAsync();
            Assert.Equal(@"{""Foo"":""Bar"",""Fizz"":15.0,""Buzz"":""<\""\n""}", contents);
        }

        [Theory]
        [MemberData(nameof(Encodings))]
        public async Task CreateAsynchronousJsonWriterWithIeee754Compatibility(Encoding encoding)
        {
            ODataUtf8JsonWriterFactory factory = ODataUtf8JsonWriterFactory.Default;
            using MemoryStream stream = new MemoryStream();

            IJsonWriter jsonWriter = factory.CreateJsonWriter(stream, isIeee754Compatible: true, encoding: encoding);

            await jsonWriter.StartObjectScopeAsync();
            await jsonWriter.WriteNameAsync("Foo");
            await jsonWriter.WriteValueAsync("Bar");
            await jsonWriter.WriteNameAsync("Fizz");
            await jsonWriter.WriteValueAsync(15.0m);
            await jsonWriter.WriteNameAsync("Buzz");
            await jsonWriter.WriteValueAsync("<\"\n");
            await jsonWriter.EndObjectScopeAsync();

            await jsonWriter.FlushAsync();
            using StreamReader reader = new StreamReader(stream, encoding);
            stream.Seek(0, SeekOrigin.Begin);
            string contents = await reader.ReadToEndAsync();
            Assert.Equal(@"{""Foo"":""Bar"",""Fizz"":""15.0"",""Buzz"":""<\""\n""}", contents);
        }

        [Theory]
        [MemberData(nameof(Encodings))]
        public async Task CreateAsynchronousJsonWriterWithCustomEncoder(Encoding encoding)
        {
            ODataUtf8JsonWriterFactory factory = new ODataUtf8JsonWriterFactory(JavaScriptEncoder.UnsafeRelaxedJsonEscaping);
            using MemoryStream stream = new MemoryStream();

            IJsonWriter jsonWriter = factory.CreateJsonWriter(stream, isIeee754Compatible: false, encoding: encoding);

            await jsonWriter.StartObjectScopeAsync();
            await jsonWriter.WriteNameAsync("Foo");
            await jsonWriter.WriteValueAsync("Bar");
            await jsonWriter.WriteNameAsync("Fizz");
            await jsonWriter.WriteValueAsync(15.0m);
            await jsonWriter.WriteNameAsync("Buzz");
            await jsonWriter.WriteValueAsync("<\"\n");
            await jsonWriter.EndObjectScopeAsync();

            await jsonWriter.FlushAsync();
            using StreamReader reader = new StreamReader(stream, encoding);
            stream.Seek(0, SeekOrigin.Begin);
            string contents = await reader.ReadToEndAsync();
            Assert.Equal(@"{""Foo"":""Bar"",""Fizz"":15.0,""Buzz"":""<\""\n""}", contents);
        }

        [Fact]
        public async Task CreatedAsyncJsonWriterShouldNotCloseOutputStream()
        {
            ODataUtf8JsonWriterFactory factory = ODataUtf8JsonWriterFactory.Default;
            using TestStream stream = new TestStream();

            IAsyncDisposable jsonWriter = factory.CreateJsonWriter(stream, false, Encoding.UTF8) as IAsyncDisposable;
            await jsonWriter.DisposeAsync();

            Assert.False(stream.Disposed);
        }

        private sealed class TestStream : MemoryStream
        {
            public bool Disposed { get; private set; }

            public override ValueTask DisposeAsync()
            {
                Disposed = true;
                return base.DisposeAsync();
            }
        }
    }
}
