//---------------------------------------------------------------------
// <copyright file="ODataJsonLightEntryAndFeedDeSerializerUndecalredAnnotationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData;
using Microsoft.OData.Json;
using Microsoft.OData.Edm;
using Microsoft.OData.Tests;
using Xunit;

namespace Microsoft.Test.OData.TDD.Tests.Reader.JsonLight
{
    public class ODataJsonLightEntryAndFeedDeserializerUndeclaredAnnotationTests
    {
        private ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings
        {
            ShouldIncludeAnnotation = (annotationName) => true,
            Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType
        };

        private ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings
        {
            ShouldIncludeAnnotation = (annotationName) => true,
            Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType
        };

        // ----------- begin of edm for entry reader -----------
        private EdmModel serverModel;
        private EdmEntityType serverEntityType;
        private EdmEntityType serverOpenEntityType;
        private EdmEntitySet serverEntitySet;
        private EdmEntitySet serverOpenEntitySet;

        public ODataJsonLightEntryAndFeedDeserializerUndeclaredAnnotationTests()
        {
            this.serverModel = new EdmModel();
            var addressType = new EdmComplexType("Server.NS", "Address");
            addressType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            addressType.AddProperty(new EdmStructuralProperty(addressType, "MyEdmUntypedProp3", EdmCoreModel.Instance.GetUntyped()));
            this.serverModel.AddElement(addressType);

            // non-open entity type
            this.serverEntityType = new EdmEntityType("Server.NS", "ServerEntityType");
            this.serverModel.AddElement(this.serverEntityType);
            this.serverEntityType.AddKeys(this.serverEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.serverEntityType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, true));
            this.serverEntityType.AddStructuralProperty("MyEdmUntypedProp1", EdmCoreModel.Instance.GetUntyped());

            // open entity type
            this.serverOpenEntityType = new EdmEntityType("Server.NS", "ServerOpenEntityType",
                baseType: null, isAbstract: false, isOpen: true);
            this.serverModel.AddElement(this.serverOpenEntityType);
            this.serverOpenEntityType.AddKeys(this.serverOpenEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.serverOpenEntityType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, true));
            this.serverOpenEntityType.AddStructuralProperty("MyEdmUntypedProp2", EdmCoreModel.Instance.GetUntyped());

            EdmEntityContainer container = new EdmEntityContainer("Server.NS", "container1");
            this.serverEntitySet = container.AddEntitySet("serverEntitySet", this.serverEntityType);
            this.serverOpenEntitySet = container.AddEntitySet("serverOpenEntitySet", this.serverOpenEntityType);
            this.serverModel.AddElement(container);

            this.writerSettings.SetContentType(ODataFormat.Json);
            this.writerSettings.SetServiceDocumentUri(new Uri("http://www.sampletest.com/"));
        }
        // ----------- end of edm for entry reader ----------- 

        #region test nested annotations for open & non-open entity: property unknown name + known value type

        /// <summary>
        /// For open entity
        /// </summary>
        [Fact]
        public void ReadNestedAnnotationInOpenEntryUndeclaredComplexTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""@instance.AnnotationName_"":""instance value_"",
                                  ""undeclaredComplex1@odata.unknownname1"":""od unkown value 1"",
                                  ""undeclaredComplex1@my.Annotation1"":""my custom value 1"",
                                  ""undeclaredComplex1@instanceAnnotation1.term1"":""custom annotation value 1"",
                                  ""undeclaredComplex1"":{  ""@odata.type"":""#Server.NS.Address"",
                                                              ""@instance.AnnotationName_"":""instance value_234"",
                                                              ""undeclaredComplex1@odata.unknownname1"":""od unkown value _234"",
                                                              ""undeclaredComplex1@my.Annotation1"":""my custom value _234"",
                                                              ""undeclaredComplex1@instanceAnnotation1.term1"":""custom annotation value _234"",
                                                              ""undeclaredComplex1@odata.type"":""#Server.NS.UnknownType1"",
                                                              ""undeclaredComplex1"":""hello this is a string."", 
                                                              ""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            ODataNestedResourceInfo nestedInfo = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                     nestedInfo = (reader.Item as ODataNestedResourceInfo);
                }
            });

            Assert.Equal(2, entry.Properties.Count());
            Assert.Single(entry.InstanceAnnotations);
            Assert.Equal(3, complex1.Properties.Count());
            Assert.Single(complex1.InstanceAnnotations);

            Assert.Equal("Server.NS.Address", complex1.TypeName);
            ODataProperty undeclaredComplex1Prop = complex1.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal));
            Assert.Equal("\"hello this is a string.\"", (undeclaredComplex1Prop.Value as ODataUntypedValue).RawValue);
            Assert.Equal("Server.NS.UnknownType1", undeclaredComplex1Prop.TypeAnnotation.TypeName);
            Assert.Equal(3, undeclaredComplex1Prop.InstanceAnnotations.Count());
            Assert.Equal("od unkown value _234", (undeclaredComplex1Prop.InstanceAnnotations.First().Value as ODataPrimitiveValue).Value);
            Assert.Equal("custom annotation value _234", (undeclaredComplex1Prop.InstanceAnnotations.Last().Value as ODataPrimitiveValue).Value);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(nestedInfo);
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity\",\"@instance.AnnotationName_\":\"instance value_\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"@odata.type\":\"#Server.NS.Address\",\"@instance.AnnotationName_\":\"instance value_234\",\"undeclaredComplex1@odata.type\":\"#Server.NS.UnknownType1\",\"undeclaredComplex1@odata.unknownname1\":\"od unkown value _234\",\"undeclaredComplex1@my.Annotation1\":\"my custom value _234\",\"undeclaredComplex1@instanceAnnotation1.term1\":\"custom annotation value _234\",\"undeclaredComplex1\":\"hello this is a string.\",\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
        }

        /// <summary>
        /// For non-open entity
        /// </summary>
        [Fact]
        public void ReadNestedAnnotationInNonOpenEntryUndeclaredComplexTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""@instance.AnnotationName_"":""instance value_"",
                                  ""undeclaredComplex1@odata.unknownname1"":""od unkown value 1"",
                                  ""undeclaredComplex1@my.Annotation1"":""my custom value 1"",
                                  ""undeclaredComplex1@instanceAnnotation1.term1"":""custom annotation value 1"",
                                  ""undeclaredComplex1"":{  ""@odata.type"":""#Server.NS.Address"",
                                                              ""@instance.AnnotationName_"":""instance value_234"",
                                                              ""undeclaredComplex1@odata.unknownname1"":""od unkown value _234"",
                                                              ""undeclaredComplex1@my.Annotation1"":""my custom value _234"",
                                                              ""undeclaredComplex1@instanceAnnotation1.term1"":""custom annotation value _234"",
                                                              ""undeclaredComplex1@odata.type"":""#Server.NS.UnknownType1"",
                                                              ""undeclaredComplex1"":""hello this is a string."", 
                                                              ""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            ODataNestedResourceInfo nestedInfo = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    nestedInfo = (reader.Item as ODataNestedResourceInfo);
                }
            });

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(3, complex1.Properties.Count());
            Assert.Single(complex1.InstanceAnnotations);

            Assert.Equal("Server.NS.Address", complex1.TypeName);
            ODataProperty undeclaredComplex1Prop = complex1.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal));
            Assert.Equal("\"hello this is a string.\"", (undeclaredComplex1Prop.Value as ODataUntypedValue).RawValue);
            Assert.Equal("Server.NS.UnknownType1", undeclaredComplex1Prop.TypeAnnotation.TypeName);
            Assert.Equal(3, undeclaredComplex1Prop.InstanceAnnotations.Count());
            Assert.Equal("od unkown value _234", (undeclaredComplex1Prop.InstanceAnnotations.First().Value as ODataPrimitiveValue).Value);
            Assert.Equal("custom annotation value _234", (undeclaredComplex1Prop.InstanceAnnotations.Last().Value as ODataPrimitiveValue).Value);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(nestedInfo);
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"@instance.AnnotationName_\":\"instance value_\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"@odata.type\":\"#Server.NS.Address\",\"@instance.AnnotationName_\":\"instance value_234\",\"undeclaredComplex1@odata.type\":\"#Server.NS.UnknownType1\",\"undeclaredComplex1@odata.unknownname1\":\"od unkown value _234\",\"undeclaredComplex1@my.Annotation1\":\"my custom value _234\",\"undeclaredComplex1@instanceAnnotation1.term1\":\"custom annotation value _234\",\"undeclaredComplex1\":\"hello this is a string.\",\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
        }

        #endregion

        #region non-open entity's property unknown name + known value type

        [Fact]
        public void ReadNonOpenNullTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1@odata.type"":""#NS1.unknownTypeName123"",""UndeclaredAddress1@odata.unknownName1"":""uknown odata.xxx value1"",""UndeclaredAddress1@NS1.abcdefg"":""uknown abcdefghijk value2"",""UndeclaredAddress1"":null}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(2, entry.Properties.Count());
            ODataProperty val = entry.Properties.Last();
            Assert.NotNull(val.Value as ODataUntypedValue);
            Assert.Equal(2, val.InstanceAnnotations.Count());
            Assert.Equal("NS1.unknownTypeName123", val.TypeAnnotation.TypeName);
            Assert.Equal("uknown odata.xxx value1", (val.InstanceAnnotations.First().Value as ODataPrimitiveValue).Value);
            Assert.Null(complex1);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            Assert.Equal(payload, result);
        }

        [Fact]
        public void ReadNonOpenknownTypeBoolTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredBool@odata.unknownName1"":""unknown odata.xxx value1"",""UndeclaredBool@NS1.abcdefg"":""unknown abcdefghijk value2"",""UndeclaredBool"":false}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(2, complex1.Properties.Count());
            ODataProperty val = complex1.Properties
                .First(s => string.Equals("UndeclaredBool", s.Name, StringComparison.Ordinal));
            Assert.Equal(false, val.ODataValue.FromODataValue());

            Assert.Equal(2, val.InstanceAnnotations.Count());
            Assert.Equal("unknown abcdefghijk value2", (val.InstanceAnnotations.First(a => a.Name == "NS1.abcdefg").Value as ODataPrimitiveValue).Value);
            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredBool@odata.unknownName1\":\"unknown odata.xxx value1\",\"UndeclaredBool@NS1.abcdefg\":\"unknown abcdefghijk value2\",\"UndeclaredBool\":false}}", result);
        }

        [Fact]
        public void ReadNonOpenknownTypeStringTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet@odata.type"":""Edm.String"",""UndeclaredStreet@odata.unknownName1"":""unknown odata.xxx value1"",""UndeclaredStreet@NS1.abcdefg"":""unknown abcdefghijk value2"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(2, complex1.Properties.Count());
            ODataProperty val = complex1.Properties
                .First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal));
            Assert.Equal("No.10000000999,Zixing Rd Minhang", val.ODataValue.FromODataValue());

            Assert.Equal(2, val.InstanceAnnotations.Count());
            Assert.Equal("unknown abcdefghijk value2", (val.InstanceAnnotations.First(a => a.Name == "NS1.abcdefg").Value as ODataPrimitiveValue).Value);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet@odata.unknownName1\":\"unknown odata.xxx value1\",\"UndeclaredStreet@NS1.abcdefg\":\"unknown abcdefghijk value2\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
        }

        [Fact]
        public void ReadNonOpenknownTypeNumericTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreetNo@odata.type"":""#Double"",""UndeclaredStreetNo@odata.unknownName1"":""unknown odata.xxx value1"",""UndeclaredStreetNo@NS1.abcdefg"":""unknown abcdefghijk value2"",""UndeclaredStreetNo"":""12""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(2, complex1.Properties.Count());
            ODataProperty val = complex1.Properties
                .First(s => string.Equals("UndeclaredStreetNo", s.Name, StringComparison.Ordinal));
            Assert.Equal(12d, val.ODataValue.FromODataValue());

            Assert.Equal(2, val.InstanceAnnotations.Count());
            Assert.Equal("unknown abcdefghijk value2", (val.InstanceAnnotations.First(a => a.Name == "NS1.abcdefg").Value as ODataPrimitiveValue).Value);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreetNo@odata.type\":\"#Double\",\"UndeclaredStreetNo@odata.unknownName1\":\"unknown odata.xxx value1\",\"UndeclaredStreetNo@NS1.abcdefg\":\"unknown abcdefghijk value2\",\"UndeclaredStreetNo\":12.0}}", result);
        }

        [Fact]
        public void ReadNonOpenKnownTypeComplexTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1@odata.unknownName1"":""unknown odata.xxx value1"",""UndeclaredAddress1@NS1.abcdefg"":""unknown abcdefghijk value2"",""UndeclaredAddress1"":{""@odata.type"":""#Server.NS.Address"",""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Single(entry.Properties);
            Assert.Equal("Server.NS.Address", complex1.TypeName);
            Assert.Equal(2, complex1.Properties.Count());
            Assert.Equal(@"""No.10000000999,Zixing Rd Minhang""",
                (complex1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Empty(complex1.InstanceAnnotations);

            // uncomment the below if decide to expose OData information via .InstanceAnnotations
            // complex1.InstanceAnnotations.First().Value.As<ODataPrimitiveValue>().Value.Should().Be("#Server.NS.Address");
            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "UndeclaredAddress1" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredAddress1\":{\"@odata.type\":\"#Server.NS.Address\",\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
        }

        [Fact]
        public void ReadNonOpenKnownTypeCollectionTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                UndeclaredCollection1@odata.type:""Collection(Edm.String)"",""UndeclaredCollection1@odata.unknownName1"":""unknown odata.xxx value1"",""UndeclaredCollection1@NS1.abcdefg"":""unknown abcdefghijk value2"",UndeclaredCollection1:[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(3, entry.Properties.Count());
            ODataProperty val = entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal));
            Assert.Equal(3, (val.ODataValue as ODataCollectionValue).Items.Count());

            Assert.Equal(2, val.InstanceAnnotations.Count());
            Assert.Equal("unknown abcdefghijk value2", (val.InstanceAnnotations.First(a => a.Name == "NS1.abcdefg").Value as ODataPrimitiveValue).Value);
            Assert.Equal(2, complex1.Properties.Count());

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"UndeclaredCollection1@odata.type\":\"#Collection(String)\",\"UndeclaredCollection1@odata.unknownName1\":\"unknown odata.xxx value1\",\"UndeclaredCollection1@NS1.abcdefg\":\"unknown abcdefghijk value2\",\"UndeclaredCollection1\":[\"email1@163.com\",\"email2@gmail.com\",\"email3@gmail2.com\"],\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
        }
        #endregion

        #region non-open entity's property unknown name + unknown value type

        [Fact]
        public void ReadNonOpenUnknownNullTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1@odata.unknownName1"":""unknown odata.xxx value1"",""UndeclaredAddress1@NS1.abcdefg"":""unknown abcdefghijk value2"",""UndeclaredAddress1"":"
                + @"null,""UndeclaredAddress1@odata.type"":""Server.NS.UndefComplex1""}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(2, entry.Properties.Count());
            ODataProperty val = entry.Properties.Last();
            Assert.Equal("null", (val.Value as ODataUntypedValue).RawValue);
            Assert.Equal(2, val.InstanceAnnotations.Count());
            Assert.Equal("Server.NS.UndefComplex1", val.TypeAnnotation.TypeName);
            Assert.Equal("unknown odata.xxx value1", (val.InstanceAnnotations.First().Value as ODataPrimitiveValue).Value);
            Assert.Equal("unknown abcdefghijk value2", (val.InstanceAnnotations.Last().Value as ODataPrimitiveValue).Value);
            Assert.Null(complex1);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredAddress1@odata.type\":\"#Server.NS.UndefComplex1\",\"UndeclaredAddress1@odata.unknownName1\":\"unknown odata.xxx value1\",\"UndeclaredAddress1@NS1.abcdefg\":\"unknown abcdefghijk value2\",\"UndeclaredAddress1\":null}", result);
        }

        [Fact]
        public void ReadNonOpenUnknownTypePrimitiveTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId@odata.unknownName1"":""unknown odata.xxx value1"",""UndeclaredFloatId@NS1.abcdefg"":""unknown abcdefghijk value2"",""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal("12.3", (entry.Properties.First(s => string.Equals("UndeclaredFloatId", s.Name, StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue); // numeric
            Assert.Equal(2, complex1.Properties.Count());
            Assert.Equal(@"""No.10000000999,Zixing Rd Minhang""", 
                (complex1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal)).Value as ODataUntypedValue) // string
                .RawValue);
        }

        [Fact]
        public void ReadNonOpenUnknownTypeInvalidComplexTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1"":"
                + @"{""@odata.type"":""#Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(@"{""@odata.type"":""#Server.NS.AddressInValid"",""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}",
                (entry.Properties.Last().Value as ODataUntypedValue).RawValue);
        }

        [Fact]
        public void ReadNonOpenUnknownTypeInvalidComplexNestedTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1"":"
                + @"{""@odata.type"":""#Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""innerComplex1"":{""innerProp1"":null,""inerProp2"":'abc'},""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(@"{""@odata.type"":""#Server.NS.AddressInValid"",""Street"":""No.999,Zixing Rd Minhang"",""innerComplex1"":{""innerProp1"":null,""inerProp2"":""abc""},""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}",
                (entry.Properties.Last().Value as ODataUntypedValue).RawValue);
        }

        [Fact]
        public void ReadNonOpenUnknownTypeCollectionTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                UndeclaredCollection1:[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal(@"[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""]",
                (entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal(2, complex1.Properties.Count());
        }

        #endregion

        #region open entity's property unknown name + known value type

        [Fact]
        public void ReadOpenEntryUndeclaredPropertiesWithNullValueTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                ""UndeclaredType1"":null}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal("null", (entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredType1", StringComparison.Ordinal)).ODataValue as ODataUntypedValue).RawValue);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredPropertiesTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(2, complex1.Properties.Count());
        }

        [Fact]
        public void ReadOpenEntryUndeclaredComplexPropertiesTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                ""undeclaredComplex1"":{""@odata.type"":""#Server.NS.Address"",""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(2, complex1.Properties.Count());
        }

        [Fact]
        public void ReadOpenEntryUndeclaredCollectionPropertiesTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                UndeclaredCollection1@odata.type:""Collection(Edm.String)"",UndeclaredCollection1:[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal(3, (entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal)).ODataValue as ODataCollectionValue).Items.Count());
            Assert.Equal(2, complex1.Properties.Count());
        }
        #endregion

        #region open entity's property unknown name + unknown value type

        [Fact]
        public void ReadOpenEntryUndeclaredComplexPropertiesWithoutODataTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1@odata.unknownName1"":""unknown odata.xxx value1"",""undeclaredComplex1@NS1.abcdefg"":""unknown abcdefghijk value2"",
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(3, entry.Properties.Count());
            ODataProperty val = entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal));
            Assert.Equal(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}", (val.Value as ODataUntypedValue).RawValue);
            Assert.Equal(2, val.InstanceAnnotations.Count());
            Assert.Equal("unknown odata.xxx value1", (val.InstanceAnnotations.First().Value as ODataPrimitiveValue).Value);
            Assert.Equal("unknown abcdefghijk value2", (val.InstanceAnnotations.Last().Value as ODataPrimitiveValue).Value);
            Assert.Equal(2, complex1.Properties.Count());

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1@odata.unknownName1\":\"unknown odata.xxx value1\",\"undeclaredComplex1@NS1.abcdefg\":\"unknown abcdefghijk value2\",\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredComplexInvalidTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                ""undeclaredComplex1"":{""@odata.type"":""#Server.NS.AddressUndeclared"",""Street"":""No.999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });
            Assert.Equal(@"{""@odata.type"":""#Server.NS.AddressUndeclared"",""Street"":""No.999,Zixing Rd Minhang""}",
                (entry.Properties.Last().Value as ODataUntypedValue).RawValue);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredEmptyComplexPropertiesTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                ""undeclaredComplex1"":{}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal(@"{}", (entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredCollectionPropertiesWithoutODataTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                                                          UndeclaredCollection1:[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal(@"[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""]",
                (entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal(2, complex1.Properties.Count());
        }

        [Fact]
        public void ReadOpenEntryUndeclaredEmptyCollectionPropertiesWithoutODataTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                                                          UndeclaredCollection1:[],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal(@"[]", (entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal)).Value as ODataUntypedValue)
                .RawValue);
            Assert.Equal(@"""No.10000000999,Zixing Rd Minhang""", (complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredStreet", StringComparison.Ordinal))
                .Value as ODataUntypedValue).RawValue);
        }

        #endregion

        #region annotation for open entity's property unknown name + unknown value type

        [Fact]
        public void ReadAnnotationInOpenEntryUndeclaredComplexPropertiesWithoutODataTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                    ""@instance.AnnotationName_"":""instance value_"",
                                  ""undeclaredComplex1@odata.unknownname1"":""od unkown value 1"",
                                  ""undeclaredComplex1@my.Annotation1"":""my custom value 1"",
                                  ""undeclaredComplex1@instanceAnnotation1.term1"":""custom annotation value 1"",
                                  ""undeclaredComplex1@odata.type"":""Server.NS.UnknownType1"",""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}", (entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal))
                .Value as ODataUntypedValue).RawValue);
            Assert.Equal(2, complex1.Properties.Count());
        }

        #endregion

        #region declared Edm.Untyped property + some annotations
        [Fact]
        public void ReadNonOpenEntryEdmUntypedPropertyTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""},
            ""MyEdmUntypedProp1@NS1.abc"":1908,MyEdmUntypedProp1:{""MyProp12"":""bbb222"",abc:null}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            ODataResource complex2 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                    else if (complex2 == null)
                    {
                        complex2 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(4, entry.Properties.Count());
            Assert.Equal(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}",
                (entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal(@"{""MyProp12"":""bbb222"",""abc"":null}",
                (entry.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp1", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal(1908, (entry.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp1", StringComparison.Ordinal))
                .InstanceAnnotations.Single(s => s.Name == "NS1.abc").Value as ODataPrimitiveValue)
                .Value);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"MyEdmUntypedProp1@NS1.abc\":1908,\"MyEdmUntypedProp1\":{\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
        }

        [Fact]
        public void ReadOpenEntryEdmUntypedPropertyTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""},
            ""MyEdmUntypedProp2@NS1.abc"":1908,MyEdmUntypedProp2:{""MyProp12"":""bbb222"",abc:null}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (entry == null)
                {
                    entry = (reader.Item as ODataResource);
                }
                else if (complex1 == null)
                {
                    complex1 = (reader.Item as ODataResource);
                }
            });

            Assert.Equal(4, entry.Properties.Count());
            Assert.Equal(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}", (entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal))
                .Value as ODataUntypedValue).RawValue);
            Assert.Equal(@"{""MyProp12"":""bbb222"",""abc"":null}", (entry.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp2", StringComparison.Ordinal))
                .Value as ODataUntypedValue).RawValue);
            Assert.Equal(1908, (entry.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp2", StringComparison.Ordinal))
                .InstanceAnnotations.Single(s => s.Name == "NS1.abc").Value as ODataPrimitiveValue)
                .Value);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"MyEdmUntypedProp2@NS1.abc\":1908,\"MyEdmUntypedProp2\":{\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
        }

        [Fact]
        public void ReadNonOpenEntryEdmUntypedPropertyInComplexTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},
                                  ""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang"",
                                               ""MyEdmUntypedProp3@NS1.abc"":1908,MyEdmUntypedProp3:{""MyProp12"":""bbb222"",abc:null}}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            ODataResource complex2 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                    else if (complex2 == null)
                    {
                        complex2 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}", (entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal))
                .Value as ODataUntypedValue).RawValue);
            Assert.Equal(@"""No.10000000999,Zixing Rd Minhang""", (complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredStreet", StringComparison.Ordinal))
                .Value as ODataUntypedValue).RawValue);
            Assert.Equal(@"{""MyProp12"":""bbb222"",""abc"":null}", (complex1.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp3", StringComparison.Ordinal))
               .Value as ODataUntypedValue).RawValue);
            Assert.Equal(1908, (complex1.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp3", StringComparison.Ordinal))
                .InstanceAnnotations.Single(s => s.Name == "NS1.abc").Value as ODataPrimitiveValue)
                .Value);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\",\"MyEdmUntypedProp3@NS1.abc\":1908,\"MyEdmUntypedProp3\":{\"MyProp12\":\"bbb222\",\"abc\":null}}}", result);
        }
        #endregion

        #region undeclared Edm.Untyped property with odata.type + some annotations
        [Fact]
        public void ReadNonOpenEntryEdmUntypedPropertyODataTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""},
            ""UndeclaredMyEdmUntypedProp1@NS1.helloworld"":true,
            ""UndeclaredMyEdmUntypedProp1@odata.type"":""#Untyped"",UndeclaredMyEdmUntypedProp1:{""MyProp12"":""bbb222"",abc:null}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (entry == null)
                {
                    entry = (reader.Item as ODataResource);
                }
                else if (complex1 == null)
                {
                    complex1 = (reader.Item as ODataResource);
                }
            });

            Assert.Equal(4, entry.Properties.Count());
            Assert.Equal(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}",
                (entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal(@"{""MyProp12"":""bbb222"",""abc"":null}",
                (entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp1", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal(true,
                (entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp1", StringComparison.Ordinal))
                .InstanceAnnotations.Single(s => s.Name == "NS1.helloworld").Value as ODataPrimitiveValue).Value);
            Assert.Equal("Edm.Untyped", entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp1", StringComparison.Ordinal)).TypeAnnotation.TypeName);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"UndeclaredMyEdmUntypedProp1@odata.type\":\"#Untyped\",\"UndeclaredMyEdmUntypedProp1@NS1.helloworld\":true,\"UndeclaredMyEdmUntypedProp1\":{\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
        }

        [Fact]
        public void ReadOpenEntryEdmUntypedPropertyODataTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""},
            ""UndeclaredMyEdmUntypedProp2@NS1.helloworld"":true,
            ""UndeclaredMyEdmUntypedProp2@odata.type"":""#Untyped"",UndeclaredMyEdmUntypedProp2:{""MyProp12"":""bbb222"",abc:null}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            ODataNestedResourceInfo nestedInfo = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    nestedInfo = (reader.Item as ODataNestedResourceInfo);
                }
            }
            );

            Assert.Equal(4, entry.Properties.Count());
            Assert.Equal(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}",
                (entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal(@"{""MyProp12"":""bbb222"",""abc"":null}",
                (entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp2", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal(true, (entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp2", StringComparison.Ordinal))
                .InstanceAnnotations.Single(s => s.Name == "NS1.helloworld").Value as ODataPrimitiveValue)
                .Value);
            Assert.Equal("Edm.Untyped",
                entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp2", StringComparison.Ordinal)).TypeAnnotation.TypeName);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"UndeclaredMyEdmUntypedProp2@odata.type\":\"#Untyped\",\"UndeclaredMyEdmUntypedProp2@NS1.helloworld\":true,\"UndeclaredMyEdmUntypedProp2\":{\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
        }

        [Fact]
        public void ReadNonOpenEntryEdmUntypedPropertyODataTypeInComplexTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},
                                  ""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang"",
                                               ""UndeclaredMyEdmUntypedProp3@NS1.helloworld"":true,""UndeclaredMyEdmUntypedProp3@odata.type"":""#Untyped"",UndeclaredMyEdmUntypedProp3:{""MyProp12"":""bbb222"",abc:null}}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            ODataResource complex2 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                    else if (complex2 == null)
                    {
                        complex2 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}",
                (entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal(@"{""MyProp12"":""bbb222"",""abc"":null}",
                (complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp3", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal(true,
                (complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp3", StringComparison.Ordinal))
                .InstanceAnnotations.Single(s => s.Name == "NS1.helloworld").Value as ODataPrimitiveValue).Value);
            Assert.Equal("Edm.Untyped", complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp3", StringComparison.Ordinal))
                .TypeAnnotation.TypeName);
            Assert.Null(complex2);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\",\"UndeclaredMyEdmUntypedProp3@odata.type\":\"#Untyped\",\"UndeclaredMyEdmUntypedProp3@NS1.helloworld\":true,\"UndeclaredMyEdmUntypedProp3\":{\"MyProp12\":\"bbb222\",\"abc\":null}}}", result);
        }
        #endregion

        private static void AdvanceReaderToFirstProperty(BufferingJsonReader bufferingJsonReader)
        {
            // Read start and then over the object start.
            bufferingJsonReader.Read();
            bufferingJsonReader.Read();
            Assert.Equal(JsonNodeType.Property, bufferingJsonReader.NodeType);
        }

        private static void AdvanceReaderToFirstPropertyValue(BufferingJsonReader bufferingJsonReader)
        {
            AdvanceReaderToFirstProperty(bufferingJsonReader);

            // Read over property name
            bufferingJsonReader.Read();
        }

        private void ReadEntryPayload(string payload, EdmEntitySet entitySet, EdmEntityType entityType, Action<ODataReader> action)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", "application/json");
            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, readerSettings, this.serverModel))
            {
                var reader = msgReader.CreateODataResourceReader(entitySet, entityType);
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }
        #region writer methods for roundtrip testing

        private string WriteEntryPayload(EdmEntitySet entitySet, EdmEntityType entityType, Action<ODataWriter> action)
        {
            MemoryStream stream = new MemoryStream();
            IODataResponseMessage message = new InMemoryMessage() { Stream = stream };
            message.SetHeader("Content-Type", "application/json");
            writerSettings.SetServiceDocumentUri(new Uri("http://www.sampletest.com"));
            using (var msgReader = new ODataMessageWriter((IODataResponseMessage)message, writerSettings, this.serverModel))
            {
                var writer = msgReader.CreateODataResourceWriter(entitySet, entityType);
                action(writer);

                stream.Seek(0, SeekOrigin.Begin);
                string payload = (new StreamReader(stream)).ReadToEnd();
                return payload;
            }
        }

        #endregion
    }
}
