//---------------------------------------------------------------------
// <copyright file="DefaultStreamBasedJsonWriterFactoryAsyncTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP3_1_OR_GREATER
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
    public sealed class DefaultStreamBasedJsonWriterFactoryAsyncTests
    {

        #region Argument validation

        [Fact]
        public void CreateAsynchronousJsonWriter_ThrowsIfStreamIsNull()
        {
            DefaultStreamBasedJsonWriterFactory factory = DefaultStreamBasedJsonWriterFactory.Default;

            Assert.Throws<ArgumentNullException>("stream", () => factory.CreateAsynchronousJsonWriter(null, false, Encoding.UTF8));
        }

        [Fact]
        public void CreateAsynchronousJsonWriter_ThrowsIfEncodingIsNull()
        {
            DefaultStreamBasedJsonWriterFactory factory = DefaultStreamBasedJsonWriterFactory.Default;
            using MemoryStream stream = new MemoryStream();

            Assert.Throws<ArgumentNullException>("encoding", () => factory.CreateAsynchronousJsonWriter(stream, false, null));
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
            DefaultStreamBasedJsonWriterFactory factory = DefaultStreamBasedJsonWriterFactory.Default;
            using MemoryStream stream = new MemoryStream();
            IJsonWriterAsync jsonWriter = factory.CreateAsynchronousJsonWriter(stream, isIeee754Compatible: false, encoding);

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
            Assert.Equal(@"{""Foo"":""Bar"",""Fizz"":15.0,""Buzz"":""\u003C\u0022\n""}", contents);
        }

        [Theory]
        [MemberData(nameof(Encodings))]
        public async Task CreateAsynchronousJsonWriterWithIeee754Compatibility(Encoding encoding)
        {
            DefaultStreamBasedJsonWriterFactory factory = DefaultStreamBasedJsonWriterFactory.Default;
            using MemoryStream stream = new MemoryStream();

            IJsonWriterAsync jsonWriter = factory.CreateAsynchronousJsonWriter(stream, isIeee754Compatible: true, encoding: encoding);

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
            Assert.Equal(@"{""Foo"":""Bar"",""Fizz"":""15.0"",""Buzz"":""\u003C\u0022\n""}", contents);
        }

        [Theory]
        [MemberData(nameof(Encodings))]
        public async Task CreateAsynchronousJsonWriterWithCustomEncoder(Encoding encoding)
        {
            DefaultStreamBasedJsonWriterFactory factory = new DefaultStreamBasedJsonWriterFactory(JavaScriptEncoder.UnsafeRelaxedJsonEscaping);
            using MemoryStream stream = new MemoryStream();

            IJsonWriterAsync jsonWriter = factory.CreateAsynchronousJsonWriter(stream, isIeee754Compatible: false, encoding: encoding);

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
            DefaultStreamBasedJsonWriterFactory factory = DefaultStreamBasedJsonWriterFactory.Default;
            using TestStream stream = new TestStream();

            IAsyncDisposable jsonWriter = factory.CreateAsynchronousJsonWriter(stream, false, Encoding.UTF8) as IAsyncDisposable;
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
#endif
