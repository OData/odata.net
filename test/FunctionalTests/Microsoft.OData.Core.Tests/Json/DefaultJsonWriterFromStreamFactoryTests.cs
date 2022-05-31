//---------------------------------------------------------------------
// <copyright file="DefaultJsonWriterFromStreamFactoryTests.cs" company="Microsoft">
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
    public class DefaultJsonWriterFromStreamFactoryTests
    {
        [Fact]
        public void CreatesJsonWriterWithUtf8Support()
        {
            DefaultStreamBasedJsonWriterFactory factory = new DefaultStreamBasedJsonWriterFactory();
            MemoryStream stream = new MemoryStream();
            IJsonWriter jsonWriter = factory.CreateJsonWriter(stream, isIeee754Compatible: false, encoding: Encoding.UTF8);

            jsonWriter.StartObjectScope();
            jsonWriter.WriteName("Foo");
            jsonWriter.WriteValue("Bar");
            jsonWriter.WriteName("Fizz");
            jsonWriter.WriteValue(15.0m);
            jsonWriter.EndObjectScope();

            jsonWriter.Flush();
            StreamReader reader = new StreamReader(stream);
            stream.Seek(0, SeekOrigin.Begin);
            string contents = reader.ReadToEnd();
            Assert.Equal(@"{""Foo"":""Bar"",""Fizz"":15.0}", contents);
        }

        [Fact]
        public void CreatesJsonWriterWithIeee754Compatibility()
        {
            DefaultStreamBasedJsonWriterFactory factory = new DefaultStreamBasedJsonWriterFactory();
            MemoryStream stream = new MemoryStream();

            IJsonWriter jsonWriter = factory.CreateJsonWriter(stream, isIeee754Compatible: true, encoding: Encoding.UTF8);

            jsonWriter.StartObjectScope();
            jsonWriter.WriteName("Foo");
            jsonWriter.WriteValue("Bar");
            jsonWriter.WriteName("Fizz");
            jsonWriter.WriteValue(15.0m);
            jsonWriter.EndObjectScope();

            jsonWriter.Flush();
            StreamReader reader = new StreamReader(stream);
            stream.Seek(0, SeekOrigin.Begin);
            string contents = reader.ReadToEnd();
            Assert.Equal(@"{""Foo"":""Bar"",""Fizz"":""15.0""}", contents);
        }

        public static IEnumerable<object[]> UnsupportedEncodings
            => new object[][]
            {
                new object[] { Encoding.Unicode },
                new object[] { Encoding.UTF32 },
                new object[] { Encoding.UTF7 },
                new object[] { Encoding.BigEndianUnicode },
                new object[] { Encoding.ASCII }
            };

        [Theory]
        [MemberData(nameof(UnsupportedEncodings))]
        public void ReturnsNullForUnsupportedEncodings(Encoding encoding)
        {
            DefaultStreamBasedJsonWriterFactory factory = new DefaultStreamBasedJsonWriterFactory();
            MemoryStream stream = new MemoryStream();

            IJsonWriter jsonWriter = factory.CreateJsonWriter(stream, false, encoding);

            Assert.Null(jsonWriter);
        }
    }
}
#endif
