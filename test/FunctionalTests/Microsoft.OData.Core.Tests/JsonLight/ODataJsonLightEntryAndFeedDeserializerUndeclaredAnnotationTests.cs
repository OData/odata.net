//---------------------------------------------------------------------
// <copyright file="ODataJsonLightEntryAndFeedDeSerializerUndecalredAnnotationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
namespace Microsoft.Test.OData.TDD.Tests.Reader.JsonLight
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using FluentAssertions;
    using Microsoft.OData;
    using Microsoft.OData.Json;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Tests;
    using Xunit;

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

            entry.Properties.Count().Should().Be(2);
            entry.InstanceAnnotations.Count.Should().Be(1);
            complex1.Properties.Count().Should().Be(3);
            complex1.InstanceAnnotations.Count().Should().Be(1);

            complex1.TypeName.Should().Be("Server.NS.Address");
            ODataProperty undeclaredComplex1Prop = complex1.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"));
            (undeclaredComplex1Prop.Value as ODataUntypedValue).RawValue.Should().Be("\"hello this is a string.\"");
            undeclaredComplex1Prop.TypeAnnotation.TypeName.Should().Be("Server.NS.UnknownType1");
            undeclaredComplex1Prop.InstanceAnnotations.Count().Should().Be(3);
            undeclaredComplex1Prop.InstanceAnnotations.First().Value.As<ODataPrimitiveValue>().Value.Should().Be("od unkown value _234");
            undeclaredComplex1Prop.InstanceAnnotations.Last().Value.As<ODataPrimitiveValue>().Value.Should().Be("custom annotation value _234");

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

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity\",\"@instance.AnnotationName_\":\"instance value_\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"@odata.type\":\"#Server.NS.Address\",\"@instance.AnnotationName_\":\"instance value_234\",\"undeclaredComplex1@odata.type\":\"#Server.NS.UnknownType1\",\"undeclaredComplex1@odata.unknownname1\":\"od unkown value _234\",\"undeclaredComplex1@my.Annotation1\":\"my custom value _234\",\"undeclaredComplex1@instanceAnnotation1.term1\":\"custom annotation value _234\",\"undeclaredComplex1\":\"hello this is a string.\",\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
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

            entry.Properties.Count().Should().Be(2);
            complex1.Properties.Count().Should().Be(3);
            complex1.InstanceAnnotations.Count().Should().Be(1);

            complex1.TypeName.Should().Be("Server.NS.Address");
            ODataProperty undeclaredComplex1Prop = complex1.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"));
            (undeclaredComplex1Prop.Value as ODataUntypedValue).RawValue.Should().Be("\"hello this is a string.\"");
            undeclaredComplex1Prop.TypeAnnotation.TypeName.Should().Be("Server.NS.UnknownType1");
            undeclaredComplex1Prop.InstanceAnnotations.Count().Should().Be(3);
            undeclaredComplex1Prop.InstanceAnnotations.First().Value.As<ODataPrimitiveValue>().Value.Should().Be("od unkown value _234");
            undeclaredComplex1Prop.InstanceAnnotations.Last().Value.As<ODataPrimitiveValue>().Value.Should().Be("custom annotation value _234");

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

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"@instance.AnnotationName_\":\"instance value_\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"@odata.type\":\"#Server.NS.Address\",\"@instance.AnnotationName_\":\"instance value_234\",\"undeclaredComplex1@odata.type\":\"#Server.NS.UnknownType1\",\"undeclaredComplex1@odata.unknownname1\":\"od unkown value _234\",\"undeclaredComplex1@my.Annotation1\":\"my custom value _234\",\"undeclaredComplex1@instanceAnnotation1.term1\":\"custom annotation value _234\",\"undeclaredComplex1\":\"hello this is a string.\",\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
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

            entry.Properties.Count().Should().Be(2);
            ODataProperty val = entry.Properties.Last();
            val.Value.As<ODataUntypedValue>().Should().NotBeNull();
            val.InstanceAnnotations.Count().Should().Be(2);
            val.TypeAnnotation.TypeName.Should().Be("NS1.unknownTypeName123");
            val.InstanceAnnotations.First().Value.As<ODataPrimitiveValue>().Value.Should().Be("uknown odata.xxx value1");
            complex1.Should().BeNull();

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(payload);
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

            entry.Properties.Count().Should().Be(2);
            complex1.Properties.Count().Should().Be(2);
            ODataProperty val = complex1.Properties
                .First(s => string.Equals("UndeclaredBool", s.Name));
            val.ODataValue.FromODataValue().Should().Be(false);

            val.InstanceAnnotations.Count().Should().Be(2);
            val.InstanceAnnotations.First(a => a.Name == "NS1.abcdefg").Value.As<ODataPrimitiveValue>().Value.Should().Be("unknown abcdefghijk value2");
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

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredBool@odata.unknownName1\":\"unknown odata.xxx value1\",\"UndeclaredBool@NS1.abcdefg\":\"unknown abcdefghijk value2\",\"UndeclaredBool\":false}}");
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

            entry.Properties.Count().Should().Be(2);
            complex1.Properties.Count().Should().Be(2);
            ODataProperty val = complex1.Properties
                .First(s => string.Equals("UndeclaredStreet", s.Name));
            val.ODataValue.FromODataValue().Should().Be("No.10000000999,Zixing Rd Minhang");

            val.InstanceAnnotations.Count().Should().Be(2);
            val.InstanceAnnotations.First(a => a.Name == "NS1.abcdefg").Value.As<ODataPrimitiveValue>().Value.Should().Be("unknown abcdefghijk value2");

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

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet@odata.unknownName1\":\"unknown odata.xxx value1\",\"UndeclaredStreet@NS1.abcdefg\":\"unknown abcdefghijk value2\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
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

            entry.Properties.Count().Should().Be(2);
            complex1.Properties.Count().Should().Be(2);
            ODataProperty val = complex1.Properties
                .First(s => string.Equals("UndeclaredStreetNo", s.Name));
            val.ODataValue.FromODataValue().Should().Be(12d);

            val.InstanceAnnotations.Count().Should().Be(2);
            val.InstanceAnnotations.First(a => a.Name == "NS1.abcdefg").Value.As<ODataPrimitiveValue>().Value.Should().Be("unknown abcdefghijk value2");

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

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreetNo@odata.type\":\"#Double\",\"UndeclaredStreetNo@odata.unknownName1\":\"unknown odata.xxx value1\",\"UndeclaredStreetNo@NS1.abcdefg\":\"unknown abcdefghijk value2\",\"UndeclaredStreetNo\":12.0}}");
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

            entry.Properties.Count().Should().Be(1);
            complex1.TypeName.Should().Be("Server.NS.Address");
            complex1.Properties.Count().Should().Be(2);
            complex1.Properties
                .First(s => string.Equals("UndeclaredStreet", s.Name)).Value.As<ODataUntypedValue>()
                .RawValue.Should().Be(@"""No.10000000999,Zixing Rd Minhang""");
            complex1.InstanceAnnotations.Count().Should().Be(0);

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

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredAddress1\":{\"@odata.type\":\"#Server.NS.Address\",\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
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

            entry.Properties.Count().Should().Be(3);
            ODataProperty val = entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1"));
            val.ODataValue.As<ODataCollectionValue>().Items.Cast<string>().Count().Should().Be(3);

            val.InstanceAnnotations.Count().Should().Be(2);
            val.InstanceAnnotations.First(a => a.Name == "NS1.abcdefg").Value.As<ODataPrimitiveValue>().Value.Should().Be("unknown abcdefghijk value2");
            complex1.Properties.Count().Should().Be(2);

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

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"UndeclaredCollection1@odata.type\":\"#Collection(String)\",\"UndeclaredCollection1@odata.unknownName1\":\"unknown odata.xxx value1\",\"UndeclaredCollection1@NS1.abcdefg\":\"unknown abcdefghijk value2\",\"UndeclaredCollection1\":[\"email1@163.com\",\"email2@gmail.com\",\"email3@gmail2.com\"],\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
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

            entry.Properties.Count().Should().Be(2);
            ODataProperty val = entry.Properties.Last();
            val.Value.As<ODataUntypedValue>().RawValue.Should().Be("null");
            val.InstanceAnnotations.Count().Should().Be(2);
            val.TypeAnnotation.TypeName.Should().Be("Server.NS.UndefComplex1");
            val.InstanceAnnotations.First().Value.As<ODataPrimitiveValue>().Value.Should().Be("unknown odata.xxx value1");
            val.InstanceAnnotations.Last().Value.As<ODataPrimitiveValue>().Value.Should().Be("unknown abcdefghijk value2");
            complex1.Should().BeNull();

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredAddress1@odata.type\":\"#Server.NS.UndefComplex1\",\"UndeclaredAddress1@odata.unknownName1\":\"unknown odata.xxx value1\",\"UndeclaredAddress1@NS1.abcdefg\":\"unknown abcdefghijk value2\",\"UndeclaredAddress1\":null}");
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

            entry.Properties.Count().Should().Be(2);
            entry.Properties.First(s => string.Equals("UndeclaredFloatId", s.Name)).Value.As<ODataUntypedValue>().RawValue.Should().Be("12.3"); // numeric
            complex1.Properties.Count().Should().Be(2);
            complex1.Properties
                .First(s => string.Equals("UndeclaredStreet", s.Name)).Value.As<ODataUntypedValue>() // string
                .RawValue.Should().Be(@"""No.10000000999,Zixing Rd Minhang""");
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

            entry.Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.As<ODataUntypedValue>().RawValue
                .Should().Be(@"{""@odata.type"":""#Server.NS.AddressInValid"",""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}");
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

            entry.Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.As<ODataUntypedValue>().RawValue
                .Should().Be(@"{""@odata.type"":""#Server.NS.AddressInValid"",""Street"":""No.999,Zixing Rd Minhang"",""innerComplex1"":{""innerProp1"":null,""inerProp2"":""abc""},""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}");
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

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1")).Value.As<ODataUntypedValue>().RawValue
              .Should().Be(@"[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""]");
            complex1.Properties.Count().Should().Be(2);
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

            entry.Properties.Count().Should().Be(3);
            Assert.Equal("null", entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredType1")).ODataValue.As<ODataUntypedValue>().RawValue);
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

            entry.Properties.Count().Should().Be(2);
            complex1.Properties.Count().Should().Be(2);
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

            entry.Properties.Count().Should().Be(2);
            complex1.Properties.Count().Should().Be(2);
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

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1")).ODataValue.As<ODataCollectionValue>().Items
                .Cast<string>().Count().Should().Be(3);
            complex1.Properties.Count().Should().Be(2);
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

            entry.Properties.Count().Should().Be(3);
            ODataProperty val = entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"));
            val.Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}");
            val.InstanceAnnotations.Count().Should().Be(2);
            val.InstanceAnnotations.First().Value.As<ODataPrimitiveValue>().Value.Should().Be("unknown odata.xxx value1");
            val.InstanceAnnotations.Last().Value.As<ODataPrimitiveValue>().Value.Should().Be("unknown abcdefghijk value2");
            complex1.Properties.Count().Should().Be(2);

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

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1@odata.unknownName1\":\"unknown odata.xxx value1\",\"undeclaredComplex1@NS1.abcdefg\":\"unknown abcdefghijk value2\",\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
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
            entry.Properties.Last().Value.As<ODataUntypedValue>().RawValue
                .Should().Be(@"{""@odata.type"":""#Server.NS.AddressUndeclared"",""Street"":""No.999,Zixing Rd Minhang""}");
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

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1")).Value.As<ODataUntypedValue>()
                .RawValue.Should().Be(@"{}");
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

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1")).Value.As<ODataUntypedValue>().RawValue
                .Should().Be(@"[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""]");
            complex1.Properties.Count().Should().Be(2);
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

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1")).Value.As<ODataUntypedValue>()
                .RawValue.Should().Be(@"[]");
            complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredStreet"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"""No.10000000999,Zixing Rd Minhang""");
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

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}");
            complex1.Properties.Count().Should().Be(2);
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

            entry.Properties.Count().Should().Be(4);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}");
            entry.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp1"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp12"":""bbb222"",""abc"":null}");
            entry.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp1"))
                .InstanceAnnotations.Single(s => s.Name == "NS1.abc").Value.As<ODataPrimitiveValue>()
                .Value.Should().Be(1908);

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

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"MyEdmUntypedProp1@NS1.abc\":1908,\"MyEdmUntypedProp1\":{\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
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

            entry.Properties.Count().Should().Be(4);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}");
            entry.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp2"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp12"":""bbb222"",""abc"":null}");
            entry.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp2"))
                .InstanceAnnotations.Single(s => s.Name == "NS1.abc").Value.As<ODataPrimitiveValue>()
                .Value.Should().Be(1908);

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

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"MyEdmUntypedProp2@NS1.abc\":1908,\"MyEdmUntypedProp2\":{\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
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

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}");
            complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredStreet"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"""No.10000000999,Zixing Rd Minhang""");
            complex1.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp3"))
               .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp12"":""bbb222"",""abc"":null}");
            complex1.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp3"))
                .InstanceAnnotations.Single(s => s.Name == "NS1.abc").Value.As<ODataPrimitiveValue>()
                .Value.Should().Be(1908);

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

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\",\"MyEdmUntypedProp3@NS1.abc\":1908,\"MyEdmUntypedProp3\":{\"MyProp12\":\"bbb222\",\"abc\":null}}}");
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

            entry.Properties.Count().Should().Be(4);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}");
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp1"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp12"":""bbb222"",""abc"":null}");
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp1"))
                .InstanceAnnotations.Single(s => s.Name == "NS1.helloworld").Value.As<ODataPrimitiveValue>()
                .Value.Should().Be(true);
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp1")).TypeAnnotation.TypeName.Should().Be("Edm.Untyped");

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

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"UndeclaredMyEdmUntypedProp1@odata.type\":\"#Untyped\",\"UndeclaredMyEdmUntypedProp1@NS1.helloworld\":true,\"UndeclaredMyEdmUntypedProp1\":{\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
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

            entry.Properties.Count().Should().Be(4);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}");
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp2"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp12"":""bbb222"",""abc"":null}");
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp2"))
                .InstanceAnnotations.Single(s => s.Name == "NS1.helloworld").Value.As<ODataPrimitiveValue>()
                .Value.Should().Be(true);
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp2"))
                .TypeAnnotation.TypeName.Should().Be("Edm.Untyped");

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

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"UndeclaredMyEdmUntypedProp2@odata.type\":\"#Untyped\",\"UndeclaredMyEdmUntypedProp2@NS1.helloworld\":true,\"UndeclaredMyEdmUntypedProp2\":{\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
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

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}");
            complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp3"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp12"":""bbb222"",""abc"":null}");
            complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp3"))
                .InstanceAnnotations.Single(s => s.Name == "NS1.helloworld").Value.As<ODataPrimitiveValue>()
                .Value.Should().Be(true);
            complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp3"))
                .TypeAnnotation.TypeName.Should().Be("Edm.Untyped");
            complex2.Should().BeNull();

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

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\",\"UndeclaredMyEdmUntypedProp3@odata.type\":\"#Untyped\",\"UndeclaredMyEdmUntypedProp3@NS1.helloworld\":true,\"UndeclaredMyEdmUntypedProp3\":{\"MyProp12\":\"bbb222\",\"abc\":null}}}");
        }
        #endregion

        private static void AdvanceReaderToFirstProperty(BufferingJsonReader bufferingJsonReader)
        {
            // Read start and then over the object start.
            bufferingJsonReader.Read();
            bufferingJsonReader.Read();
            bufferingJsonReader.NodeType.Should().Be(JsonNodeType.Property);
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
