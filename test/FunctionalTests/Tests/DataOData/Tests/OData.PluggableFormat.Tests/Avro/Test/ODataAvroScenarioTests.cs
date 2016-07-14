//---------------------------------------------------------------------
// <copyright file="ODataAvroScenarioTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ENABLE_AVRO
namespace Microsoft.Test.OData.PluggableFormat.Avro.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Hadoop.Avro;
    using Microsoft.Hadoop.Avro.Container;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataAvroScenarioTests
    {
        private const string ProductNamespace = "Microsoft.Test.OData.PluggableFormat.Avro.Test.Product";
        private static Product product0 = new Product { Id = 5331, Weight = 3.2f };
        private static Product product1 = new Product { Id = 1, Weight = 2.1f };
        private static Product product2 = new Product { Id = 2, Weight = 2.2f };
        private static Product product3 = new Product { Id = 3, Weight = 2.3f };
        private static Address address0 = new Address { Road = "Road1", ZipCode = "Zip1" };

        private static ODataResource entry0 = new ODataResource
            {
                TypeName = ProductNamespace,
                Properties = new[]
                {
                    new ODataProperty{  Name = "Id"     , Value = 5331  },
                    new ODataProperty{  Name = "Weight" , Value = 3.2f  },
                },
            };

        private static ODataResource entry1 = new ODataResource
            {
                TypeName = ProductNamespace,
                Properties = new[]
                {
                    new ODataProperty {Name = "Id"      , Value = 1     },
                    new ODataProperty {Name = "Weight"  , Value = 2.1f  },
                },
            };

        private static ODataResource entry2 = new ODataResource
            {
                TypeName = ProductNamespace,
                Properties = new[]
                    {
                        new ODataProperty {Name = "Id", Value = 2},
                        new ODataProperty {Name = "Weight", Value = 2.2f},
                    },
            };

        private static ODataResource entry3 = new ODataResource
            {
                TypeName = ProductNamespace,
                Properties = new[]
                    {
                        new ODataProperty {Name = "Id", Value = 3},
                        new ODataProperty {Name = "Weight", Value = 2.3f},
                    },
            };

        private static ODataResource complexResource = new ODataResource()
        {
            TypeName = "Microsoft.Test.OData.PluggableFormat.Avro.Test.Address",
            Properties = new[]
                {
                    new ODataProperty{Name = "Road", Value = "Road1"},
                    new ODataProperty{Name = "ZipCode", Value = "Zip1"},
                }
        };

        private static IEdmEntityType EntryType;
        private static IEdmComplexType ComplexType;
        private static IEdmAction AddProduct;
        private static IEdmAction GetMaxId;

        private static IServiceProvider container;

        static ODataAvroScenarioTests()
        {
            var type = new EdmEntityType("Microsoft.Test.OData.PluggableFormat.Avro.Test", "Product");
            type.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false);
            type.AddStructuralProperty("Weight", EdmPrimitiveTypeKind.Single, false);

            EntryType = type;

            var cpx = new EdmComplexType("Microsoft.Test.OData.PluggableFormat.Avro.Test", "Address");
            cpx.AddStructuralProperty("Road", EdmPrimitiveTypeKind.String, false);
            cpx.AddStructuralProperty("ZipCode", EdmPrimitiveTypeKind.String, false);
            ComplexType = cpx;

            var action = new EdmAction("Microsoft.Test.OData.PluggableFormat.Avro.Test", "AddProduct", null);
            action.AddParameter("Product", new EdmEntityTypeReference(EntryType, false));
            action.AddParameter("Location", new EdmComplexTypeReference(ComplexType, false));
            AddProduct = action;

            action = new EdmAction("Microsoft.Test.OData.PluggableFormat.Avro.Test", "GetMaxId", EdmCoreModel.Instance.GetInt32(false));
            action.AddParameter("Products", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(EntryType, false))));
            GetMaxId = action;

            container = ContainerBuilderHelper.BuildContainer(builder =>
                builder.AddService<ODataMediaTypeResolver, AvroMediaTypeResolver>(ServiceLifetime.Singleton));
        }

        [TestMethod]
        public void TestWriteEntry()
        {
            MemoryStream ms = new MemoryStream();

            using (var omw = TestHelper.CreateMessageWriter(ms, container, "avro/binary"))
            {
                var entryWriter = omw.CreateODataResourceWriter(null, EntryType);
                entryWriter.WriteStart(entry0);
                entryWriter.WriteEnd();
                entryWriter.Flush();
            }

            ms.Seek(0, SeekOrigin.Begin);

            Product prd;
            IAvroReader<Product> reader = null;
            try
            {
                reader = AvroContainer.CreateReader<Product>(ms);

                using (var seqReader = new SequentialReader<Product>(reader))
                {
                    reader = null;
                    prd = seqReader.Objects.First();
                }
            }
            finally
            {
                if (reader != null) reader.Dispose();
            }

            Assert.AreEqual(product0, prd);
        }

        [TestMethod]
        public void TestReadEntry()
        {
            MemoryStream ms = new MemoryStream();
            IAvroWriter<Product> writer = null;
            try
            {
                writer = AvroContainer.CreateWriter<Product>(ms, true, new AvroSerializerSettings(), Codec.Null);
                using (var seqWriter = new SequentialWriter<Product>(writer, 24))
                {
                    seqWriter.Write(product0);
                }
            }
            finally
            {
                if (writer != null) writer.Dispose();
            }

            ms.Seek(0, SeekOrigin.Begin);
            ODataResource entry = null;
            using (var omr = TestHelper.CreateMessageReader(ms, container, "avro/binary"))
            {
                var reader = omr.CreateODataResourceReader();
                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        entry = (ODataResource)reader.Item;
                    }
                }
            }

            Assert.IsTrue(TestHelper.EntryEqual(entry0, entry));
        }

        [TestMethod]
        public void TestReadFeed()
        {
            MemoryStream ms = new MemoryStream();
            IAvroWriter<Product[]> writer = null;
            try
            {
                writer = AvroContainer.CreateWriter<Product[]>(ms, true, new AvroSerializerSettings(), Codec.Null);
                using (var seqWriter = new SequentialWriter<Product[]>(writer, 24))
                {
                    seqWriter.Write(new[] { product1, product2, product3 });
                }
            }
            finally
            {
                if (writer != null) writer.Dispose();
            }

            ms.Seek(0, SeekOrigin.Begin);
            var entries = new List<ODataResource>();
            using (var omr = TestHelper.CreateMessageReader(ms, container, "avro/binary"))
            {
                var reader = omr.CreateODataResourceSetReader();
                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        entries.Add((ODataResource)reader.Item);
                    }
                }
            }

            Assert.AreEqual(3, entries.Count);

            Assert.IsTrue(TestHelper.EntryEqual(entry1, entries[0]));
            Assert.IsTrue(TestHelper.EntryEqual(entry2, entries[1]));
            Assert.IsTrue(TestHelper.EntryEqual(entry3, entries[2]));
        }

        [TestMethod]
        public void TestWriteFeed()
        {
            MemoryStream ms = new MemoryStream();

            using (var omw = TestHelper.CreateMessageWriter(ms, container, "avro/binary"))
            {
                var entryWriter = omw.CreateODataResourceSetWriter(null, EntryType);
                entryWriter.WriteStart(new ODataResourceSet());
                entryWriter.WriteStart(entry0);
                entryWriter.WriteEnd();
                entryWriter.WriteStart(entry1);
                entryWriter.WriteEnd();
                entryWriter.WriteStart(entry2);
                entryWriter.WriteEnd();
                entryWriter.WriteStart(entry3);
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                entryWriter.Flush();
            }

            ms.Seek(0, SeekOrigin.Begin);

            Product[] products;
            IAvroReader<Product[]> reader = null;
            try
            {
                reader = AvroContainer.CreateReader<Product[]>(ms);

                using (var seqReader = new SequentialReader<Product[]>(reader))
                {
                    reader = null;
                    products = seqReader.Objects.ToList().Single();
                }
            }
            finally
            {
                if (reader != null) reader.Dispose();
            }

            Assert.AreEqual(product0, products[0]);
            Assert.AreEqual(product1, products[1]);
            Assert.AreEqual(product2, products[2]);
            Assert.AreEqual(product3, products[3]);
        }

        [TestMethod]
        public void TestReadCollection()
        {
            MemoryStream ms = new MemoryStream();

            using (var writer = AvroContainer.CreateWriter<string[]>(ms, true, new AvroSerializerSettings(), Codec.Null))
            using (var seqWriter = new SequentialWriter<string[]>(writer, 24))
            {
                seqWriter.Write(new[] { "p9t", "cjk如" });
            }

            ms.Seek(0, SeekOrigin.Begin);
            var values = new List<string>();
            using (var omr = TestHelper.CreateMessageReader(ms, container, "avro/binary"))
            {
                var reader = omr.CreateODataCollectionReader();
                while (reader.Read())
                {
                    if (reader.State == ODataCollectionReaderState.Value)
                    {
                        values.Add((string)reader.Item);
                    }
                }
            }

            Assert.AreEqual(2, values.Count);
            Assert.AreEqual("p9t", values[0]);
            Assert.AreEqual("cjk如", values[1]);
        }

        [TestMethod]
        public void TestWriteCollection()
        {
            MemoryStream ms = new MemoryStream();
            using (var omw = TestHelper.CreateMessageWriter(ms, container, "avro/binary"))
            {
                var cw = omw.CreateODataCollectionWriter();
                cw.WriteStart(new ODataCollectionStart());
                cw.WriteStart(new ODataCollectionStart());
                cw.WriteItem("s0");
                cw.WriteItem("s1");
                cw.WriteEnd();
            }

            ms.Seek(0, SeekOrigin.Begin);
            IEnumerable<string[]> results = null;
            using (var reader = AvroContainer.CreateReader<string[]>(ms))
            using (var seqReader = new SequentialReader<string[]>(reader))
            {
                results = seqReader.Objects;
            }

            var records = results.ToList();
            Assert.AreEqual(1, records.Count());
            Assert.AreEqual("s0", records[0][0]);
            Assert.AreEqual("s1", records[0][1]);
        }

        [TestMethod]
        public void TestWriteComplexResource()
        {
            MemoryStream ms = new MemoryStream();

            using (var omw = TestHelper.CreateMessageWriter(ms, container, "avro/binary"))
            {
                var entryWriter = omw.CreateODataResourceWriter(null, ComplexType);
                entryWriter.WriteStart(complexResource);
                entryWriter.WriteEnd();
                entryWriter.Flush();
            }

            ms.Seek(0, SeekOrigin.Begin);

            Address addr;
            IAvroReader<Address> reader = null;
            try
            {
                reader = AvroContainer.CreateReader<Address>(ms);

                using (var seqReader = new SequentialReader<Address>(reader))
                {
                    reader = null;
                    addr = seqReader.Objects.First();
                }
            }
            finally
            {
                if (reader != null) reader.Dispose();
            }

            Assert.AreEqual(address0, addr);
        }

        [TestMethod]
        public void TestWriteProperty()
        {
            ODataProperty prop = new ODataProperty{Name = "Road", Value = "Road1"};

            MemoryStream ms = new MemoryStream();
            using (var omw = TestHelper.CreateMessageWriter(ms, container, "avro/binary"))
            {
                omw.WriteProperty(prop);
            }

            ms.Seek(0, SeekOrigin.Begin);

            string road;
            IAvroReader<string> reader = null;
            try
            {
                reader = AvroContainer.CreateReader<string>(ms);

                using (var seqReader = new SequentialReader<string>(reader))
                {
                    reader = null;
                    road = seqReader.Objects.First();
                }
            }
            finally
            {
                if (reader != null) reader.Dispose();
            }

            Assert.AreEqual("Road1", road);
        }

        [TestMethod]
        public void TestWriteParameter()
        {
            MemoryStream ms = new MemoryStream();
            using (var omw = TestHelper.CreateMessageWriter(ms, container, "avro/binary", new EdmModel(), false))
            {
                var opw = omw.CreateODataParameterWriter(AddProduct);
                var ew = opw.CreateResourceWriter("Product");
                ew.WriteStart(entry0);
                ew.WriteEnd();
                ew.Flush();

                var ew1 = opw.CreateResourceWriter("Location");
                ew1.WriteStart(complexResource);
                ew1.WriteEnd();
                ew1.Flush();

                opw.WriteEnd();
                opw.Flush();
            }

            ms.Seek(0, SeekOrigin.Begin);

            AddProductParameter parameter;
            IAvroReader<AddProductParameter> reader = null;
            try
            {
                reader = AvroContainer.CreateReader<AddProductParameter>(ms);

                using (var seqReader = new SequentialReader<AddProductParameter>(reader))
                {
                    reader = null;
                    parameter = seqReader.Objects.First();
                }
            }
            finally
            {
                if (reader != null) reader.Dispose();
            }

            Assert.IsNotNull(parameter);
            Assert.AreEqual(product0, parameter.Product);
            Assert.AreEqual(address0, parameter.Location);
        }

        [TestMethod]
        public void TestWriteParameterWithFeed()
        {
            MemoryStream ms = new MemoryStream();
            using (var omw = TestHelper.CreateMessageWriter(ms, container, "avro/binary", new EdmModel(), false))
            {
                var opw = omw.CreateODataParameterWriter(GetMaxId);
                var ew = opw.CreateResourceSetWriter("Products");
                ew.WriteStart(new ODataResourceSet());
                ew.WriteStart(entry0);
                ew.WriteEnd();
                ew.WriteStart(entry1);
                ew.WriteEnd();
                ew.WriteEnd();
                ew.Flush();

                opw.WriteEnd();
                opw.Flush();
            }

            ms.Seek(0, SeekOrigin.Begin);

            GetMaxIdParameter parameter;
            IAvroReader<GetMaxIdParameter> reader = null;
            try
            {
                reader = AvroContainer.CreateReader<GetMaxIdParameter>(ms);

                using (var seqReader = new SequentialReader<GetMaxIdParameter>(reader))
                {
                    reader = null;
                    parameter = seqReader.Objects.First();
                }
            }
            finally
            {
                if (reader != null) reader.Dispose();
            }

            Assert.IsNotNull(parameter);
            var products = parameter.Products as IList<Product>;
            Assert.IsNotNull(products);
            Assert.AreEqual(product0, products[0]);
            Assert.AreEqual(product1, products[1]);
        }

        [TestMethod]
        public void TestReadParameter()
        {
            MemoryStream ms = new MemoryStream();

            using (var writer = AvroContainer.CreateWriter<AddProductParameter>(ms, true, new AvroSerializerSettings(), Codec.Null))
            using (var seqWriter = new SequentialWriter<AddProductParameter>(writer, 24))
            {
                seqWriter.Write(new AddProductParameter { Location = address0, Product = product0 });
            }

            ms.Seek(0, SeekOrigin.Begin);
            var result = new Dictionary<string, object>();
            using (var omr = TestHelper.CreateMessageReader(ms, container, "avro/binary", new EdmModel()))
            {
                var reader = omr.CreateODataParameterReader(AddProduct);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataParameterReaderState.Value:
                            result.Add(reader.Name, reader.Value);
                            break;
                        case ODataParameterReaderState.Resource:
                            var entryReader = reader.CreateResourceReader();
                            while (entryReader.Read())
                            {
                                if (entryReader.State == ODataReaderState.ResourceEnd)
                                {
                                    result.Add(reader.Name, entryReader.Item);
                                    break;
                                }
                            }
                            break;
                    }
                }
            }

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(TestHelper.EntryEqual(complexResource, (ODataResource)result["Location"]));
            Assert.IsTrue(TestHelper.EntryEqual(entry0, (ODataResource)result["Product"]));
        }

        [TestMethod]
        public void TestReadParameterWithFeed()
        {
            MemoryStream ms = new MemoryStream();

            using (var writer = AvroContainer.CreateWriter<GetMaxIdParameter>(ms, true, new AvroSerializerSettings(), Codec.Null))
            using (var seqWriter = new SequentialWriter<GetMaxIdParameter>(writer, 24))
            {
                seqWriter.Write(new GetMaxIdParameter { Products = new List<Product>() { product0, product1 } });
            }

            ms.Seek(0, SeekOrigin.Begin);
            var result = new Dictionary<string, object>();
            using (var omr = TestHelper.CreateMessageReader(ms, container, "avro/binary", new EdmModel()))
            {
                var reader = omr.CreateODataParameterReader(GetMaxId);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataParameterReaderState.Value:
                            result.Add(reader.Name, reader.Value);
                            break;
                        case ODataParameterReaderState.Resource:
                            var entryReader = reader.CreateResourceReader();
                            while (entryReader.Read())
                            {
                                if (entryReader.State == ODataReaderState.ResourceEnd)
                                {
                                    result.Add(reader.Name, entryReader.Item);
                                    break;
                                }
                            }
                            break;
                        case ODataParameterReaderState.ResourceSet:
                            var feedReader = reader.CreateResourceSetReader();
                            IList<ODataResource> entryList = new List<ODataResource>();
                            while (feedReader.Read())
                            {
                                if (feedReader.State == ODataReaderState.ResourceEnd)
                                {
                                    entryList.Add((ODataResource)feedReader.Item);
                                }
                            }

                            result.Add(reader.Name, entryList);
                            break;
                    }
                }
            }

            Assert.AreEqual(1, result.Count);
            var feed = result["Products"] as IList<ODataResource>;
            Assert.IsNotNull(feed);
            Assert.AreEqual(2, feed.Count);
            Assert.IsTrue(TestHelper.EntryEqual(entry0, feed[0]));
            Assert.IsTrue(TestHelper.EntryEqual(entry1, feed[1]));
        }


        [TestMethod]
        public void TestWriteError()
        {
            ODataError odataError = new ODataError()
            {
                ErrorCode = "404",
                Message = "Not Found",
            };

            MemoryStream ms = new MemoryStream();
            using (var omw = TestHelper.CreateMessageWriter(ms, container, "avro/binary"))
            {
                omw.WriteError(odataError, false);
            }

            ms.Seek(0, SeekOrigin.Begin);

            Error error;
            IAvroReader<Error> reader = null;
            using (reader = AvroContainer.CreateReader<Error>(ms))
            using (var seqReader = new SequentialReader<Error>(reader))
            {
                error = seqReader.Objects.First();
            }

            Assert.IsNotNull(error);
            Assert.AreEqual("404", error.ErrorCode);
            Assert.AreEqual("Not Found", error.Message);
        }

        [TestMethod]
        public void TestReadError()
        {
            MemoryStream ms = new MemoryStream();

            using (var writer = AvroContainer.CreateWriter<Error>(ms, true, new AvroSerializerSettings(), Codec.Null))
            using (var seqWriter = new SequentialWriter<Error>(writer, 24))
            {
                seqWriter.Write(new Error { ErrorCode = "500", Message = "Internal Error" });
            }

            ms.Seek(0, SeekOrigin.Begin);
            ODataError error = null;
            using (var omr = TestHelper.CreateMessageReader(ms, container, "avro/binary", new EdmModel(), true))
            {
                error = omr.ReadError();
            }

            Assert.IsNotNull(error);
            Assert.AreEqual("500", error.ErrorCode);
            Assert.AreEqual("Internal Error", error.Message);
        }
    }
}
#endif