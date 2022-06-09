//---------------------------------------------------------------------
// <copyright file="DefaultStreamBasedJsonWriterFactoryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP3_1_OR_GREATER
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public sealed class DefaultJsonWriterFromStreamFactoryTests
    {
        public static IEnumerable<object[]> Encodings
           => new object[][]
           {
                new object[] { Encoding.UTF8 },
                new object[] { Encoding.Unicode },
                new object[] { Encoding.UTF32 },
                new object[] { Encoding.BigEndianUnicode },
                new object[] { Encoding.ASCII }
           };

        [Theory]
        [MemberData(nameof(Encodings))]
        public void CreatesJsonWriterWithSpecifiedEncoding(Encoding encoding)
        {
            DefaultStreamBasedJsonWriterFactory factory = DefaultStreamBasedJsonWriterFactory.Instance;
            using MemoryStream stream = new MemoryStream();
            IJsonWriter jsonWriter = factory.CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            jsonWriter.StartObjectScope();
            jsonWriter.WriteName("Foo");
            jsonWriter.WriteValue("Bar");
            jsonWriter.WriteName("Fizz");
            jsonWriter.WriteValue(15.0m);
            jsonWriter.EndObjectScope();

            jsonWriter.Flush();
            using StreamReader reader = new StreamReader(stream, encoding);
            stream.Seek(0, SeekOrigin.Begin);
            string contents = reader.ReadToEnd();
            Assert.Equal(@"{""Foo"":""Bar"",""Fizz"":15.0}", contents);
        }

        [Theory]
        [MemberData(nameof(Encodings))]
        public void CreatesJsonWriterWithIeee754Compatibility(Encoding encoding)
        {
            DefaultStreamBasedJsonWriterFactory factory = DefaultStreamBasedJsonWriterFactory.Instance;
            using MemoryStream stream = new MemoryStream();

            IJsonWriter jsonWriter = factory.CreateJsonWriter(stream, isIeee754Compatible: true, encoding: encoding);

            jsonWriter.StartObjectScope();
            jsonWriter.WriteName("Foo");
            jsonWriter.WriteValue("Bar");
            jsonWriter.WriteName("Fizz");
            jsonWriter.WriteValue(15.0m);
            jsonWriter.EndObjectScope();

            jsonWriter.Flush();
            using StreamReader reader = new StreamReader(stream, encoding);
            stream.Seek(0, SeekOrigin.Begin);
            string contents = reader.ReadToEnd();
            Assert.Equal(@"{""Foo"":""Bar"",""Fizz"":""15.0""}", contents);
        }
    }
}
#endif
