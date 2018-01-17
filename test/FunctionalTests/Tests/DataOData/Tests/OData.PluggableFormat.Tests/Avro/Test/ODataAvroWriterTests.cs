//---------------------------------------------------------------------
// <copyright file="ODataAvroWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ENABLE_AVRO
namespace Microsoft.Test.OData.PluggableFormat.Avro.Test
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.Hadoop.Avro;
    using Microsoft.Hadoop.Avro.Container;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.PluggableFormat.Avro;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataAvroWriterTests
    {
        private static IEdmEntityType TestEntityType;
        private static IEdmComplexType TestComplexType;
        private static ODataResource complex0;
        private static ODataResource entry0;
        private static ODataNestedResourceInfo nestedResource0;
        private static byte[] binary0;
        private static object[] longCollection0;

        static ODataAvroWriterTests()
        {
            var type = new EdmEntityType("NS", "SimpleEntry");
            type.AddStructuralProperty("TBoolean", EdmPrimitiveTypeKind.Boolean, true);
            type.AddStructuralProperty("TInt32", EdmPrimitiveTypeKind.Int32, true);
            type.AddStructuralProperty("TCollection", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt64(false))));
            var cpx = new EdmComplexType("NS", "SimpleComplex");
            cpx.AddStructuralProperty("TBinary", EdmPrimitiveTypeKind.Binary, true);
            cpx.AddStructuralProperty("TString", EdmPrimitiveTypeKind.String, true);
            type.AddStructuralProperty("TComplex", new EdmComplexTypeReference(cpx, true));
            TestEntityType = type;

            binary0 = new byte[] { 4, 7 };
            complex0 = new ODataResource()
            {
                Properties = new[]
                {
                    new ODataProperty {Name = "TBinary", Value = binary0 ,},
                    new ODataProperty {Name = "TString", Value = "iamstr",},
                },
                TypeName = "NS.SimpleComplex"
            };

            longCollection0 = new object[] {7L, 9L};
            var collectionValue0 = new ODataCollectionValue { Items = longCollection0 };

            entry0 = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty {Name = "TBoolean", Value = true,},
                    new ODataProperty {Name = "TInt32", Value = 32,},
                    new ODataProperty {Name = "TCollection", Value = collectionValue0 },
                },
                TypeName = "NS.SimpleEntry"
            };

            nestedResource0 = new ODataNestedResourceInfo() { Name = "TComplex", IsCollection = false };
        }

        [TestMethod]
        public void WriteEntryAsAvroTest()
        {
            MemoryStream ms = new MemoryStream();
            var ctx = this.CreateOutputContext(ms);
            ODataAvroWriter aw = new ODataAvroWriter(ctx, value => ctx.AvroWriter.Write(value), ctx.AvroWriter.UpdateSchema(null, TestEntityType), false);
            aw.WriteStart(entry0);
            aw.WriteStart(nestedResource0);
            aw.WriteStart(complex0);
            aw.WriteEnd();
            aw.WriteEnd();
            aw.WriteEnd();
            aw.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            IEnumerable<object> results;
            IAvroReader<object> reader = null;
            try
            {
                reader = AvroContainer.CreateGenericReader(ms);
                using (var seqReader = new SequentialReader<object>(reader))
                {
                    reader = null;
                    results = seqReader.Objects;
                }
            }
            finally
            {
                if (reader != null) reader.Dispose();
            }

            dynamic record = results.Single() as AvroRecord;
            Assert.IsNotNull(record);
            Assert.AreEqual(true, record.TBoolean);
            Assert.AreEqual(32, record.TInt32);
            var col = record.TCollection as object[];
            Assert.IsNotNull(col);
            Assert.IsTrue(longCollection0.SequenceEqual(col));
            dynamic cpx = record.TComplex as AvroRecord;
            Assert.IsNotNull(cpx);
            Assert.IsTrue(binary0.SequenceEqual((byte[])cpx.TBinary));
            Assert.AreEqual("iamstr", cpx.TString);
        }

        [TestMethod]
        public void WriteFeedAsAvroTest()
        {
            ODataResource entry1 = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty {Name = "TBoolean", Value = true,},
                    new ODataProperty {Name = "TInt32", Value = 32,},
                },
                TypeName = "NS.SimpleEntry"
            };

            ODataResource entry2 = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty {Name = "TBoolean", Value = false,},
                    new ODataProperty {Name = "TInt32", Value = 325,},
                },
                TypeName = "NS.SimpleEntry"
            };

            MemoryStream ms = new MemoryStream();
            var ctx = this.CreateOutputContext(ms);
            ODataAvroWriter aw = new ODataAvroWriter(ctx, value => ctx.AvroWriter.Write(value), null, true);
            aw.WriteStart(new ODataResourceSet());
            aw.WriteStart(entry1);
            aw.WriteEnd();
            aw.WriteStart(entry2);
            aw.WriteEnd();
            aw.WriteEnd();
            aw.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            IEnumerable<object> results;
            IAvroReader<object> reader = null;
            try
            {
                reader = AvroContainer.CreateGenericReader(ms);
                using (var seqReader = new SequentialReader<object>(reader))
                {
                    reader = null;
                    results = seqReader.Objects;
                }
            }
            finally
            {
                if (reader != null) reader.Dispose();
            }

            var records = results.Cast<object[]>().Single();
            Assert.AreEqual(2, records.Count());

            dynamic record = records[0];
            Assert.AreEqual(true, record.TBoolean);
            Assert.AreEqual(32, record.TInt32);

            record = records[1];
            Assert.AreEqual(false, record.TBoolean);
            Assert.AreEqual(325, record.TInt32);
        }

        [TestMethod]
        public void WritePropertyAsAvroTest()
        {
            ODataProperty prop = new ODataProperty()
            {
                Name = "prop1",
                Value = 589.901f
            };

            MemoryStream ms = new MemoryStream();
            this.CreateOutputContext(ms).WriteProperty(prop);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            IEnumerable<object> results = null;

            using (var reader = AvroContainer.CreateGenericReader(ms))
            using (var seqReader = new SequentialReader<object>(reader))
            {
                results = seqReader.Objects;
            }

            var records = results.Cast<float>().ToList();
            Assert.AreEqual(1, records.Count());

            Assert.IsTrue(TestHelper.FloatEqual(589.901f, records[0]));
        }

        [TestMethod]
        public void WriteBinaryPropertyAsAvroTest()
        {
            var expected = new byte[] { 3, 4 };

            ODataProperty prop = new ODataProperty()
            {
                Name = "prop1",
                Value = expected
            };

            MemoryStream ms = new MemoryStream();
            this.CreateOutputContext(ms).WriteProperty(prop);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            IEnumerable<object> results = null;

            using (var reader = AvroContainer.CreateGenericReader(ms))
            using (var seqReader = new SequentialReader<object>(reader))
            {
                results = seqReader.Objects;
            }

            var records = results.Cast<byte[]>().ToList();
            Assert.AreEqual(1, records.Count());
            Assert.IsTrue(expected.SequenceEqual(records[0]));
        }

        [TestMethod]
        public void WritePrimitiveCollectionPropertyAsAvroTest()
        {
            var expected = new object[] { 3, 4 };
            var value = new ODataCollectionValue { Items = expected };

            ODataProperty prop = new ODataProperty
            {
                Name = "prop1",
                Value = value
            };

            MemoryStream ms = new MemoryStream();
            this.CreateOutputContext(ms).WriteProperty(prop);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            IEnumerable<object> results = null;

            using (var reader = AvroContainer.CreateGenericReader(ms))
            using (var seqReader = new SequentialReader<object>(reader))
            {
                results = seqReader.Objects;
            }

            var records = results.Cast<object[]>().ToList();
            Assert.AreEqual(1, records.Count());
            Assert.IsTrue(expected.SequenceEqual(records[0]));
        }

        [TestMethod]
        public void WriteCollectionAsAvroTest()
        {
            MemoryStream ms = new MemoryStream();
            ODataAvroCollectionWriter ocw = new ODataAvroCollectionWriter(this.CreateOutputContext(ms), null);
            ocw.WriteStartAsync(new ODataCollectionStart()).Wait();
            ocw.WriteItemAsync(1).Wait();
            ocw.WriteItemAsync(3).Wait();
            ocw.WriteEndAsync().Wait();
            ocw.FlushAsync().Wait();

            ms.Seek(0, SeekOrigin.Begin);
            IEnumerable<int[]> results = null;
            using (var reader = AvroContainer.CreateReader<int[]>(ms))
            using (var seqReader = new SequentialReader<int[]>(reader))
            {
                results = seqReader.Objects;
            }

            var records = results.ToList();
            Assert.AreEqual(1, records.Count());
            Assert.AreEqual(1, records[0][0]);
            Assert.AreEqual(3, records[0][1]);
        }


        [TestMethod]
        public void WriteOperationParametersAsAvroTest()
        {
            var operation = new EdmAction("NS", "op1", null);
            operation.AddParameter("p1", EdmCoreModel.Instance.GetString(false));
            operation.AddParameter("p2", new EdmEntityTypeReference(TestEntityType, false));

            MemoryStream ms = new MemoryStream();
            var context = this.CreateOutputContext(ms);
            var opw = new ODataAvroParameterWriter(context, operation);
            {
                opw.WriteStart();
                opw.WriteValue("p1", "dat");
                {
                    var ew = opw.CreateResourceWriter("p2");
                    ew.WriteStart(entry0);
                    ew.WriteStart(nestedResource0);
                    ew.WriteStart(complex0);
                    ew.WriteEnd();
                    ew.WriteEnd();
                    ew.WriteEnd();
                    ew.Flush();
                }

                opw.WriteEnd();
                opw.Flush();
            }

            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            IEnumerable<object> results = null;

            using (var reader = AvroContainer.CreateGenericReader(ms))
            using (var seqReader = new SequentialReader<object>(reader))
            {
                results = seqReader.Objects;
            }

            dynamic record = results.Cast<AvroRecord>().Single();
            Assert.AreEqual("dat", record.p1);
            dynamic p2 = record.p2;
            Assert.AreEqual(true, p2.TBoolean);
            Assert.AreEqual(32, p2.TInt32);
            var col = p2.TCollection as object[];
            Assert.IsNotNull(col);
            Assert.IsTrue(longCollection0.SequenceEqual(col));
            dynamic cpx = p2.TComplex as AvroRecord;
            Assert.IsNotNull(cpx);
            Assert.IsTrue(binary0.SequenceEqual((byte[])cpx.TBinary));
            Assert.AreEqual("iamstr", cpx.TString);
        }

        [TestMethod]
        public void WriteErrorAsAvroTest()
        {
            ODataError error = new ODataError()
            {
                ErrorCode = "32",
                Message = "msg1",
            };

            MemoryStream ms = new MemoryStream();
            this.CreateOutputContext(ms).WriteError(error, false);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            IEnumerable<object> results = null;

            using (var reader = AvroContainer.CreateGenericReader(ms))
            using (var seqReader = new SequentialReader<object>(reader))
            {
                results = seqReader.Objects;
            }

            var records = results.Cast<AvroRecord>().ToList();
            Assert.AreEqual(1, records.Count());
            dynamic err = records[0];
            Assert.AreEqual("32", err.ErrorCode);
            Assert.AreEqual("msg1", err.Message);
        }

        private ODataAvroOutputContext CreateOutputContext(Stream stream)
        {
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = stream,
                Encoding = Encoding.UTF8,
                IsAsync = false,
                IsResponse = true,
            };

            return new ODataAvroOutputContext(
                AvroFormat.Avro,
                messageInfo,
                new ODataMessageWriterSettings());
        }
    }
}
#endif