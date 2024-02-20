﻿//---------------------------------------------------------------------
// <copyright file="DefaultStreamBasedJsonWriterFactoryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public sealed class DefaultStreamBasedJsonWriterFactoryTests
    {

        #region Argument validation

        [Fact]
        public void CreateJsonWriter_ThrowsIfStreamIsNull()
        {
            DefaultStreamBasedJsonWriterFactory factory = DefaultStreamBasedJsonWriterFactory.Default;

            Assert.Throws<ArgumentNullException>("stream", () => factory.CreateJsonWriter(null, false, Encoding.UTF8));
        }

        [Fact]
        public void CreateJsonWriter_ThrowsIfEncodingIsNull()
        {
            DefaultStreamBasedJsonWriterFactory factory = DefaultStreamBasedJsonWriterFactory.Default;
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
            DefaultStreamBasedJsonWriterFactory factory = DefaultStreamBasedJsonWriterFactory.Default;
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
            Assert.Equal(@"{""Foo"":""Bar"",""Fizz"":15.0,""Buzz"":""\u003C\u0022\n""}", contents);
        }

        [Theory]
        [MemberData(nameof(Encodings))]
        public void CreateJsonWriterWithIeee754Compatibility(Encoding encoding)
        {
            DefaultStreamBasedJsonWriterFactory factory = DefaultStreamBasedJsonWriterFactory.Default;
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
            Assert.Equal(@"{""Foo"":""Bar"",""Fizz"":""15.0"",""Buzz"":""\u003C\u0022\n""}", contents);
        }

        [Theory]
        [MemberData(nameof(Encodings))]
        public void CreateJsonWriterWithCustomEncoder(Encoding encoding)
        {
            DefaultStreamBasedJsonWriterFactory factory = new DefaultStreamBasedJsonWriterFactory(JavaScriptEncoder.UnsafeRelaxedJsonEscaping);
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
            DefaultStreamBasedJsonWriterFactory factory = DefaultStreamBasedJsonWriterFactory.Default;
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
#endif
