//---------------------------------------------------------------------
// <copyright file="ODataUtf8JsonWriterFactoryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public sealed class ODataUtf8JsonWriterFactoryTests
    {

        #region Argument validation

        [Fact]
        public void CreateJsonWriter_ThrowsIfStreamIsNull()
        {
            ODataUtf8JsonWriterFactory factory = ODataUtf8JsonWriterFactory.Default;

            Assert.Throws<ArgumentNullException>("stream", () => factory.CreateJsonWriter(null, false, Encoding.UTF8));
        }

        [Fact]
        public void CreateJsonWriter_ThrowsIfEncodingIsNull()
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
        public void CreateJsonWriterWithSpecifiedEncoding(Encoding encoding)
        {
            ODataUtf8JsonWriterFactory factory = ODataUtf8JsonWriterFactory.Default;
            using MemoryStream stream = new MemoryStream();
            IJsonWriter jsonWriter = factory.CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            jsonWriter.StartObjectScope();
            jsonWriter.WriteName("Foo");
            jsonWriter.WriteValue("Bar");
            jsonWriter.WriteName("Fizz");
            jsonWriter.WriteValue(15.0m);
            jsonWriter.WriteName("Buzz");
            jsonWriter.WriteValue("<\"\n");
            jsonWriter.EndObjectScope();

            jsonWriter.Flush();
            using StreamReader reader = new StreamReader(stream, encoding);
            stream.Seek(0, SeekOrigin.Begin);
            string contents = reader.ReadToEnd();
            Assert.Equal(@"{""Foo"":""Bar"",""Fizz"":15.0,""Buzz"":""<\""\n""}", contents);
        }

        [Theory]
        [MemberData(nameof(Encodings))]
        public void CreateJsonWriterWithIeee754Compatibility(Encoding encoding)
        {
            ODataUtf8JsonWriterFactory factory = ODataUtf8JsonWriterFactory.Default;
            using MemoryStream stream = new MemoryStream();

            IJsonWriter jsonWriter = factory.CreateJsonWriter(stream, isIeee754Compatible: true, encoding: encoding);

            jsonWriter.StartObjectScope();
            jsonWriter.WriteName("Foo");
            jsonWriter.WriteValue("Bar");
            jsonWriter.WriteName("Fizz");
            jsonWriter.WriteValue(15.0m);
            jsonWriter.WriteName("Buzz");
            jsonWriter.WriteValue("<\"\n");
            jsonWriter.EndObjectScope();

            jsonWriter.Flush();
            using StreamReader reader = new StreamReader(stream, encoding);
            stream.Seek(0, SeekOrigin.Begin);
            string contents = reader.ReadToEnd();
            Assert.Equal(@"{""Foo"":""Bar"",""Fizz"":""15.0"",""Buzz"":""<\""\n""}", contents);
        }

        [Theory]
        [MemberData(nameof(Encodings))]
        public void CreateJsonWriterWithCustomEncoder(Encoding encoding)
        {
            ODataUtf8JsonWriterFactory factory = new ODataUtf8JsonWriterFactory(JavaScriptEncoder.UnsafeRelaxedJsonEscaping);
            using MemoryStream stream = new MemoryStream();

            IJsonWriter jsonWriter = factory.CreateJsonWriter(stream, isIeee754Compatible: false, encoding: encoding);

            jsonWriter.StartObjectScope();
            jsonWriter.WriteName("Foo");
            jsonWriter.WriteValue("Bar");
            jsonWriter.WriteName("Fizz");
            jsonWriter.WriteValue(15.0m);
            jsonWriter.WriteName("Buzz");
            jsonWriter.WriteValue("<\"\n");
            jsonWriter.EndObjectScope();

            jsonWriter.Flush();
            using StreamReader reader = new StreamReader(stream, encoding);
            stream.Seek(0, SeekOrigin.Begin);
            string contents = reader.ReadToEnd();
            Assert.Equal(@"{""Foo"":""Bar"",""Fizz"":15.0,""Buzz"":""<\""\n""}", contents);
        }


        [Fact]
        public void CreatedJsonWriterShouldNotCloseOutputStream()
        {
            ODataUtf8JsonWriterFactory factory = ODataUtf8JsonWriterFactory.Default;
            using TestStream stream = new TestStream();

            IDisposable jsonWriter = factory.CreateJsonWriter(stream, false, Encoding.UTF8) as IDisposable;
            jsonWriter.Dispose();

            Assert.False(stream.Disposed);
        }

        private sealed class TestStream : MemoryStream
        {
            public bool Disposed { get; private set; }
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                Disposed = true;
            }
        }
    }
}
