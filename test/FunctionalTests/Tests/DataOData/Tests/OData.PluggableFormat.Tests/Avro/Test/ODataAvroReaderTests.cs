//---------------------------------------------------------------------
// <copyright file="ODataAvroReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ENABLE_AVRO
namespace Microsoft.Test.OData.PluggableFormat.Avro.Test
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.Hadoop.Avro;
    using Microsoft.Hadoop.Avro.Container;
    using Microsoft.Hadoop.Avro.Schema;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataAvroReaderTests
    {
        [TestMethod]
        public void TestReadAvroAsODataEntry()
        {
            const string Schema = @"
{
""type"":""record"",
""name"":""TestNS.Person"",
""fields"":
    [
        { ""name"":""Id"", ""type"":""int"" },
        { ""name"":""Title"", ""type"":""string"" },
        { ""name"":""Address"", ""type"":{
                ""name"":""TestNS.Address"",
                ""type"":""record"",
                ""fields"":[
                    { ""name"":""ZipCode"", ""type"":""long"" },
                ]
            } 
        },
    ]
}";
            var serializer = AvroSerializer.CreateGeneric(Schema);

            using (var stream = new MemoryStream())
            {
                var expected = new AvroRecord(serializer.WriterSchema);
                expected["Id"] = -5;
                expected["Title"] = "set";

                var cpxSchema = ((RecordSchema)serializer.WriterSchema).GetField("Address").TypeSchema;
                var cpx = new AvroRecord(cpxSchema);
                cpx["ZipCode"] = 5L;
                expected["Address"] = cpx;

                using (var writer = AvroContainer.CreateGenericWriter(Schema, stream, /*leave open*/true, Codec.Null))
                {
                    using (var streamWriter = new SequentialWriter<object>(writer, 24))
                    {
                        // Serialize the data to stream using the sequential writer
                        streamWriter.Write(expected);
                    }
                }

                stream.Seek(0, SeekOrigin.Begin);
                var avroReader = new ODataAvroReader(this.CreateODataInputContext(stream), false);
                Assert.AreEqual(ODataReaderState.Start, avroReader.State);
                Assert.IsTrue(avroReader.Read());
                Assert.AreEqual(ODataReaderState.ResourceStart, avroReader.State);
                Assert.IsTrue(avroReader.Read());
                Assert.AreEqual(ODataReaderState.NestedResourceInfoStart, avroReader.State);
                Assert.IsTrue(avroReader.Read());
                Assert.AreEqual(ODataReaderState.ResourceStart, avroReader.State);
                Assert.IsTrue(avroReader.Read());
                Assert.AreEqual(ODataReaderState.ResourceEnd, avroReader.State);
                var entry = avroReader.Item as ODataResource;
                Assert.IsNotNull(entry);
                Assert.AreEqual("TestNS.Address", entry.TypeName);
                var zip = entry.Properties.Single();
                Assert.AreEqual(5L, zip.Value);
                Assert.IsTrue(avroReader.Read());
                Assert.AreEqual(ODataReaderState.NestedResourceInfoEnd, avroReader.State);
                Assert.IsTrue(avroReader.Read());
                Assert.AreEqual(ODataReaderState.ResourceEnd, avroReader.State);
                entry = avroReader.Item as ODataResource;
                Assert.IsNotNull(entry);
                Assert.AreEqual("TestNS.Person", entry.TypeName);
                var properties = entry.Properties.ToList();
                Assert.AreEqual(2, properties.Count);
                Assert.AreEqual("Id", properties[0].Name);
                Assert.AreEqual(-5, properties[0].Value);
                Assert.AreEqual("Title", properties[1].Name);
                Assert.AreEqual("set", properties[1].Value);
                Assert.IsFalse(avroReader.Read());
                Assert.AreEqual(ODataReaderState.Completed, avroReader.State);
            }
        }

        [TestMethod]
        public void TestReadAvroAsODataFeed()
        {
            const string Schema = @"
{""type"":""array"",
""items"":
{
""type"":""record"",
""name"":""TestNS.Person"",
""fields"":
    [
        { ""name"":""Id"", ""type"":""int"" },
        { ""name"":""Title"", ""type"":""string"" },
    ]
}
}";
            var serializer = AvroSerializer.CreateGeneric(Schema);

            using (var stream = new MemoryStream())
            {
                var arraySchema = (ArraySchema)serializer.WriterSchema;
                var recordSchema = arraySchema.ItemSchema;

                var rec1 = new AvroRecord(recordSchema);
                rec1["Id"] = 1;
                rec1["Title"] = "s1";

                var rec2 = new AvroRecord(recordSchema);
                rec2["Id"] = 2;
                rec2["Title"] = "s2";

                var array = new[] { rec1, rec2 };

                using (var writer = AvroContainer.CreateGenericWriter(Schema, stream, /*leave open*/true, Codec.Null))
                using (var seqWriter = new SequentialWriter<object>(writer, 24))
                {
                    seqWriter.Write(array);
                }

                stream.Seek(0, SeekOrigin.Begin);
                var avroReader = new ODataAvroReader(this.CreateODataInputContext(stream), true);
                Assert.AreEqual(ODataReaderState.Start, avroReader.State);
                Assert.IsTrue(avroReader.Read());
                Assert.AreEqual(ODataReaderState.ResourceSetStart, avroReader.State);
                Assert.IsTrue(avroReader.Read());

                // Entry 1
                Assert.AreEqual(ODataReaderState.ResourceStart, avroReader.State);
                Assert.IsTrue(avroReader.Read());
                Assert.AreEqual(ODataReaderState.ResourceEnd, avroReader.State);
                var entry = avroReader.Item as ODataResource;
                Assert.IsNotNull(entry);
                var properties = entry.Properties.ToList();
                Assert.AreEqual(2, properties.Count);
                Assert.AreEqual("Id", properties[0].Name);
                Assert.AreEqual(1, properties[0].Value);
                Assert.AreEqual("Title", properties[1].Name);
                Assert.AreEqual("s1", properties[1].Value);

                // Entry 2
                Assert.IsTrue(avroReader.Read());
                Assert.AreEqual(ODataReaderState.ResourceStart, avroReader.State);
                Assert.IsTrue(avroReader.Read());
                Assert.AreEqual(ODataReaderState.ResourceEnd, avroReader.State);
                entry = avroReader.Item as ODataResource;
                Assert.IsNotNull(entry);
                properties = entry.Properties.ToList();
                Assert.AreEqual(2, properties.Count);
                Assert.AreEqual("Id", properties[0].Name);
                Assert.AreEqual(2, properties[0].Value);
                Assert.AreEqual("Title", properties[1].Name);
                Assert.AreEqual("s2", properties[1].Value);
                Assert.IsTrue(avroReader.Read());
                Assert.AreEqual(ODataReaderState.ResourceSetEnd, avroReader.State);

                Assert.IsFalse(avroReader.Read());
                Assert.AreEqual(ODataReaderState.Completed, avroReader.State);
            }
        }

        [TestMethod]
        public void TestReadAvroAsODataEmptyFeed()
        {
            string Schema = @"
{""type"":""array"",
""items"":
{
""type"":""record"",
""name"":""TestNS.Person"",
""fields"":
    [
        { ""name"":""Id"", ""type"":""int"" },
        { ""name"":""Title"", ""type"":""string"" },
    ]
}
}";
            using (var stream = new MemoryStream())
            {
                using (var writer = AvroContainer.CreateGenericWriter(Schema, stream, /*leave open*/true, Codec.Null))
                using (var seqWriter = new SequentialWriter<object>(writer, 24))
                {
                    seqWriter.Write(new AvroRecord[] { });
                }

                stream.Seek(0, SeekOrigin.Begin);

                var avroReader = new ODataAvroReader(this.CreateODataInputContext(stream), true);
                Assert.AreEqual(ODataReaderState.Start, avroReader.State);
                Assert.IsTrue(avroReader.Read());
                Assert.AreEqual(ODataReaderState.ResourceSetStart, avroReader.State);
                Assert.IsTrue(avroReader.Read());
                Assert.AreEqual(ODataReaderState.ResourceSetEnd, avroReader.State);
                Assert.IsFalse(avroReader.Read());
                Assert.AreEqual(ODataReaderState.Completed, avroReader.State);
            }
        }

        [TestMethod]
        public void TestReadInvalidAvroInput()
        {
            string Schema = @"
{""type"":""array"",
""items"":
{
""type"":""record"",
""name"":""TestNS.Person"",
""fields"":
    [
        { ""name"":""Id"", ""type"":""int"" },
        { ""name"":""Title"", ""type"":""string"" },
    ]
}
}";

            using (var stream = new MemoryStream())
            {
                using (var writer = AvroContainer.CreateGenericWriter(Schema, stream, /*leave open*/true, Codec.Null))
                using (var seqWriter = new SequentialWriter<object>(writer, 24))
                {
                }

                stream.Seek(0, SeekOrigin.Begin);

                var avroReader = new ODataAvroReader(this.CreateODataInputContext(stream), true);
                Assert.AreEqual(ODataReaderState.Start, avroReader.State);
                Assert.IsFalse(avroReader.Read());
                Assert.AreEqual(ODataReaderState.Exception, avroReader.State);
            }
        }


        [TestMethod]
        public void TestReadAvroAsPrimitiveCollection()
        {
            const string Schema = @"{""type"":""array"", ""items"":""long""}";

            var stream = new MemoryStream();
            using (var writer = AvroContainer.CreateGenericWriter(Schema, stream, /*leave open*/true, Codec.Null))
            using (var seqWriter = new SequentialWriter<object>(writer, 24))
            {
                // Serialize the data to stream using the sequential writer
                seqWriter.Write(new[] { 6L, 8 });
                seqWriter.Flush();
            }

            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            var property = this.CreateODataInputContext(stream).ReadProperty(null, null);
        }

        [TestMethod]
        public void TestReadAvroAsCollection()
        {
            const string Schema = @"{""type"":""array"", ""items"":""long""}";

            var stream = new MemoryStream();
            using (var writer = AvroContainer.CreateGenericWriter(Schema, stream, /*leave open*/true, Codec.Null))
            using (var seqWriter = new SequentialWriter<object>(writer, 24))
            {
                // Serialize the data to stream using the sequential writer
                seqWriter.Write(new[] { 6L, 8 });
                seqWriter.Flush();
            }

            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            var reader = new ODataAvroCollectionReader(this.CreateODataInputContext(stream));
            Assert.AreEqual(ODataCollectionReaderState.Start, reader.State);
            Assert.IsTrue(reader.Read());
            Assert.AreEqual(ODataCollectionReaderState.CollectionStart, reader.State);
            Assert.IsTrue(reader.Read());
            Assert.AreEqual(ODataCollectionReaderState.Value, reader.State);
            Assert.AreEqual(6L, (long)reader.Item);
            Assert.IsTrue(reader.Read());
            Assert.AreEqual(ODataCollectionReaderState.Value, reader.State);
            Assert.IsTrue(reader.Read());
            Assert.AreEqual(ODataCollectionReaderState.CollectionEnd, reader.State);
            Assert.IsFalse(reader.Read());
            Assert.AreEqual(ODataCollectionReaderState.Completed, reader.State);
        }

        [TestMethod]
        public void ReadAvroAsParameter()
        {
            EdmEntityType personType = new EdmEntityType("TestNS", "Person");
            personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            personType.AddStructuralProperty("Title", EdmPrimitiveTypeKind.String);

            var operation = new EdmAction("NS", "op1", null);
            operation.AddParameter("p1", EdmCoreModel.Instance.GetString(false));
            operation.AddParameter("p2", new EdmEntityTypeReference(personType, false));

            const string Schema = @"{
""type"":""record"",
""name"":""NS.op1Parameter"",
""fields"":
    [
        { ""name"":""p1"", ""type"":""string"" },
        { ""name"":""p2"", ""type"":
                {""type"":""record"",
                ""name"":""TestNS.Person"",
                ""fields"":
                    [
                        { ""name"":""Id"", ""type"":""int"" },
                        { ""name"":""Title"", ""type"":""string"" },
                    ]
                }
        }
    ]
}";
            var stream = new MemoryStream();
            using (var writer = AvroContainer.CreateGenericWriter(Schema, stream, /*leave open*/true, Codec.Null))
            using (var seqWriter = new SequentialWriter<object>(writer, 24))
            {
                RecordSchema parameterSchema = (RecordSchema)AvroSerializer.CreateGeneric(Schema).WriterSchema;
                AvroRecord ar = new AvroRecord(parameterSchema);
                ar["p1"] = "dat";
                var personSchema = parameterSchema.GetField("p2").TypeSchema;
                AvroRecord person = new AvroRecord(personSchema);
                person["Id"] = 5;
                person["Title"] = "per1";
                ar["p2"] = person;

                seqWriter.Write(ar);
                seqWriter.Flush();
            }

            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            var reader = new ODataAvroParameterReader(this.CreateODataInputContext(stream), operation);
            Assert.AreEqual(ODataParameterReaderState.Start, reader.State);
            Assert.IsTrue(reader.Read());
            Assert.AreEqual(ODataParameterReaderState.Value, reader.State);
            Assert.AreEqual("p1", reader.Name);
            Assert.AreEqual("dat", reader.Value);
            Assert.IsTrue(reader.Read());
            Assert.AreEqual(ODataParameterReaderState.Resource, reader.State);
            Assert.AreEqual("p2", reader.Name);
            var ew = reader.CreateResourceReader();
            Assert.AreEqual(ODataReaderState.Start, ew.State);
            Assert.IsTrue(ew.Read());
            Assert.AreEqual(ODataReaderState.ResourceStart, ew.State);
            Assert.IsTrue(ew.Read());
            Assert.AreEqual(ODataReaderState.ResourceEnd, ew.State);
            var entry = ew.Item as ODataResource;
            Assert.IsFalse(ew.Read());
            Assert.AreEqual(ODataReaderState.Completed, ew.State);

            Assert.IsNotNull(entry);
            var properties = entry.Properties.ToList();
            Assert.AreEqual(2, properties.Count);
            Assert.AreEqual("Id", properties[0].Name);
            Assert.AreEqual(5, properties[0].Value);
            Assert.AreEqual("Title", properties[1].Name);
            Assert.AreEqual("per1", properties[1].Value);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(ODataParameterReaderState.Completed, reader.State);
        }

        [TestMethod]
        public void ReadAvroAsError()
        {
            const string Schema = @"{
""type"":""record"",
""name"":""OData.Error"",
""fields"":
    [
        { ""name"":""ErrorCode"", ""type"":""string"" },
        { ""name"":""Message""  , ""type"":""string"" },

    ]
}";
            var stream = new MemoryStream();
            using (var writer = AvroContainer.CreateGenericWriter(Schema, stream, /*leave open*/true, Codec.Null))
            using (var seqWriter = new SequentialWriter<object>(writer, 24))
            {
                RecordSchema parameterSchema = (RecordSchema)AvroSerializer.CreateGeneric(Schema).WriterSchema;
                AvroRecord ar = new AvroRecord(parameterSchema);
                ar["ErrorCode"] = "e1";
                ar["Message"] = "m1";

                seqWriter.Write(ar);
                seqWriter.Flush();
            }

            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            var error = this.CreateODataInputContext(stream).ReadError();
            Assert.AreEqual("e1", error.ErrorCode);
            Assert.AreEqual("m1", error.Message);
        }

        private ODataAvroInputContext CreateODataInputContext(Stream stream)
        {
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = stream,
                MediaType = new ODataMediaType("avro", "binary"),
                Encoding = Encoding.UTF8,
                IsAsync = false,
                IsResponse = true,
            };

            return new ODataAvroInputContext(
                AvroFormat.Avro,
                messageInfo,
                new ODataMessageReaderSettings());
        }
    }
}
#endif