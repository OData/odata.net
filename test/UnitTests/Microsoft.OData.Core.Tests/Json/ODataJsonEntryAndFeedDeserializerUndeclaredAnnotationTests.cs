//---------------------------------------------------------------------
// <copyright file="ODataJsonEntryAndFeedDeSerializerUndecalredAnnotationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.Tests;
using Microsoft.OData.Tests.ScenarioTests.Roundtrip;
using Microsoft.OData.Tests.ScenarioTests.Roundtrip.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using Xunit;

namespace Microsoft.Test.OData.TDD.Tests.Reader.Json
{
    public class ODataJsonEntryAndFeedDeserializerUndeclaredAnnotationTests
    {
        private ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings
        {
            ShouldIncludeAnnotation = (annotationName) => true,
            Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType
        };

        private ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings
        {
            ShouldIncludeAnnotationInternal = (annotationName) => true,
            Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType
        };

        // ----------- begin of edm for entry reader -----------
        private EdmModel serverModel;
        private EdmEntityType serverEntityType;
        private EdmEntityType serverOpenEntityType;
        private EdmEntitySet serverEntitySet;
        private EdmEntitySet serverOpenEntitySet;

        public ODataJsonEntryAndFeedDeserializerUndeclaredAnnotationTests()
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
  ""undeclaredComplex1"":{
    ""@odata.type"":""#Server.NS.Address"",
    ""@instance.AnnotationName_"":""instance value_234"",
    ""@odata.unknownname1"":""od unkown value 1"",
    ""@my.Annotation1"":""my custom value 1"",
    ""@instanceAnnotation1.term1"":""custom annotation value 1"",
    ""undeclaredComplex1@odata.unknownname1"":""od unkown value _234"",
    ""undeclaredComplex1@my.Annotation1"":""my custom value _234"",
    ""undeclaredComplex1@instanceAnnotation1.term1"":""custom annotation value _234"",
    ""undeclaredComplex1@odata.type"":""#Server.NS.UnknownType1"",
    ""undeclaredComplex1"":""hello this is a string."",
    ""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}
        }";

            const string expected = "{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity\",\"@instance.AnnotationName_\":\"instance value_\",\"Id\":61880128,\"UndeclaredFloatId@odata.type\":\"#Decimal\",\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"@odata.type\":\"#Server.NS.Address\",\"@instance.AnnotationName_\":\"instance value_234\",\"@my.Annotation1\":\"my custom value 1\",\"@instanceAnnotation1.term1\":\"custom annotation value 1\",\"undeclaredComplex1@odata.type\":\"#Server.NS.UnknownType1\",\"undeclaredComplex1@odata.unknownname1\":\"od unkown value _234\",\"undeclaredComplex1@my.Annotation1\":\"my custom value _234\",\"undeclaredComplex1@instanceAnnotation1.term1\":\"custom annotation value _234\",\"undeclaredComplex1\":\"hello this is a string.\",\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}";

            VerifyNestedAnnotationForUndeclaredComplex(payload, this.serverOpenEntitySet, this.serverOpenEntityType, expected);
}

        /// <summary>
        /// For non-open entity
        /// </summary>
        [Fact]
        public void ReadNestedAnnotationInNonOpenEntryUndeclaredComplexTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
  ""@instance.AnnotationName_"":""instance value_"",
  ""undeclaredComplex1"":{
    ""@odata.type"":""#Server.NS.Address"",
    ""@instance.AnnotationName_"":""instance value_234"",
    ""@odata.unknownname1"":""od unkown value 1"",
    ""@my.Annotation1"":""my custom value 1"",
    ""@instanceAnnotation1.term1"":""custom annotation value 1"",
    ""undeclaredComplex1@odata.unknownname1"":""od unkown value _234"",
    ""undeclaredComplex1@my.Annotation1"":""my custom value _234"",
    ""undeclaredComplex1@instanceAnnotation1.term1"":""custom annotation value _234"",
    ""undeclaredComplex1@odata.type"":""#Server.NS.UnknownType1"",
    ""undeclaredComplex1"":""hello this is a string."", 
    ""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";

            const string expected = "{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"@instance.AnnotationName_\":\"instance value_\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"@odata.type\":\"#Server.NS.Address\",\"@instance.AnnotationName_\":\"instance value_234\",\"@my.Annotation1\":\"my custom value 1\",\"@instanceAnnotation1.term1\":\"custom annotation value 1\",\"undeclaredComplex1@odata.type\":\"#Server.NS.UnknownType1\",\"undeclaredComplex1@odata.unknownname1\":\"od unkown value _234\",\"undeclaredComplex1@my.Annotation1\":\"my custom value _234\",\"undeclaredComplex1@instanceAnnotation1.term1\":\"custom annotation value _234\",\"undeclaredComplex1\":\"hello this is a string.\",\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}";

            VerifyNestedAnnotationForUndeclaredComplex(payload, this.serverEntitySet, this.serverEntityType, expected);
       }

        private void VerifyNestedAnnotationForUndeclaredComplex(string payload, EdmEntitySet entitySet, EdmEntityType entityType, string expected)
        {
            ODataResource entry = null;
            ODataResource complex1 = null;
            ODataNestedResourceInfo nestedInfo = null;
            this.ReadEntryPayload(payload, entitySet, entityType, reader =>
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
            },
            settings: new ODataMessageReaderSettings
            {
                ShouldIncludeAnnotation = (annotationName) => true,
                Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType & ~ValidationKinds.ThrowIfTypeConflictsWithMetadata
            });

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(3, complex1.Properties.Count());
            Assert.Equal(3, complex1.InstanceAnnotations.Count); // Be noted, so far, the unknown annotation starting with '@odata.' ignored. It seems unexpected.

            Assert.Equal("Server.NS.Address", complex1.TypeName);
            ODataProperty undeclaredComplex1Prop = Assert.IsType<ODataProperty>(complex1.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal)));
            Assert.Equal("hello this is a string.", undeclaredComplex1Prop.Value);
            Assert.Equal("Server.NS.UnknownType1", undeclaredComplex1Prop.TypeAnnotation.TypeName);
            Assert.Equal(3, undeclaredComplex1Prop.InstanceAnnotations.Count());
            Assert.Equal("od unkown value _234", (undeclaredComplex1Prop.InstanceAnnotations.First().Value as ODataPrimitiveValue).Value);
            Assert.Equal("custom annotation value _234", (undeclaredComplex1Prop.InstanceAnnotations.Last().Value as ODataPrimitiveValue).Value);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(entitySet, entityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(nestedInfo);
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal(expected, result);
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
            ODataNestedResourceInfo nestedInfo = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else
                    {
                        Assert.NotNull(nestedInfo);
                        Assert.Equal("UndeclaredAddress1", nestedInfo.Name);
                        complex1 = (reader.Item as ODataResource);
                    }
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    nestedInfo = (ODataNestedResourceInfo)(reader.Item);
                    // to ensure read to the end of entry
                }
            });

            Assert.Single(entry.Properties);

            Assert.NotNull(nestedInfo);
            Assert.Null(complex1);

            // So far, the ODL doesn't popout the annotation for the null nested resource. As expected, we should get the annotation from the nested resource info, but no such logic yet,
            // See details at: https://github.com/OData/odata.net/issues/3440
            // Assert.Equal(2, nestedInfo.InstanceAnnotations.Count());
            // Assert.Equal("unknown odata.xxx value1", (nestedInfo.InstanceAnnotations.First().Value as ODataPrimitiveValue).Value);
            // Assert.Equal("unknown abcdefghijk value2", (nestedInfo.InstanceAnnotations.Last().Value as ODataPrimitiveValue).Value);

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

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredAddress1@odata.type\":\"#NS1.unknownTypeName123\",\"UndeclaredAddress1\":null}", result);
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
            ODataProperty val = Assert.IsType<ODataProperty>(complex1.Properties
                .First(s => string.Equals("UndeclaredBool", s.Name, StringComparison.Ordinal)));
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
            ODataProperty val = Assert.IsType<ODataProperty>(complex1.Properties
                .First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal)));
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
            ODataProperty val = Assert.IsType<ODataProperty>(complex1.Properties
                .First(s => string.Equals("UndeclaredStreetNo", s.Name, StringComparison.Ordinal)));
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
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,
    ""UndeclaredAddress1"":{
      ""@odata.unknownName1"":""unknown odata.xxx value1"",
      ""@NS1.abcdefg"":""unknown abcdefghijk value2"",
      ""@odata.type"":""#Server.NS.Address"",
      ""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";

            ODataResource entry = null;
            ODataResource undeclaredAddress1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (undeclaredAddress1 == null)
                    {
                        undeclaredAddress1 = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Single(entry.Properties);
            Assert.Equal("Server.NS.Address", undeclaredAddress1.TypeName);
            Assert.Equal(2, undeclaredAddress1.Properties.Count());
            Assert.Equal("No.10000000999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(undeclaredAddress1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal))).Value);

            // So far, the unknown annotation starting with '@odata.' ignored. It seems unexpected.
            // Keep the codes below to track on the fix for unknown '@odata.' annotation
            //Assert.Equal(2, undeclaredAddress1.InstanceAnnotations.Count());
            //Assert.Equal("unknown odata.xxx value1", (undeclaredAddress1.InstanceAnnotations.First().Value as ODataPrimitiveValue).Value);
            //Assert.Equal("unknown abcdefghijk value2", (undeclaredAddress1.InstanceAnnotations.Last().Value as ODataPrimitiveValue).Value);

            // So, before the fix, let's verify the ignorance
            Assert.Equal("unknown abcdefghijk value2", Assert.IsType<ODataPrimitiveValue>(Assert.Single(undeclaredAddress1.InstanceAnnotations).Value).Value);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "UndeclaredAddress1" });
                writer.WriteStart(undeclaredAddress1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredAddress1\":{\"@odata.type\":\"#Server.NS.Address\",\"@NS1.abcdefg\":\"unknown abcdefghijk value2\",\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
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
            ODataProperty val = Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal)));
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

            Assert.Single(entry.Properties);
            ODataProperty val = Assert.IsType<ODataProperty>(entry.Properties.First());
            Assert.Equal(61880128, val.Value);

            Assert.NotNull(nestedInfo);
            Assert.Equal("UndeclaredAddress1", nestedInfo.Name);
            Assert.Equal("Server.NS.UndefComplex1", nestedInfo.TypeAnnotation.TypeName);

            Assert.Null(complex1);

            // So far, the ODL doesn't popout the annotation for the null nested resource. As expected, we should get the annotation from the nested resource info, but no such logic yet,
            // See details at: https://github.com/OData/odata.net/issues/3440
            // Assert.Equal(2, nestedInfo.InstanceAnnotations.Count());
            // Assert.Equal("unknown odata.xxx value1", (nestedInfo.InstanceAnnotations.First().Value as ODataPrimitiveValue).Value);
            // Assert.Equal("unknown abcdefghijk value2", (nestedInfo.InstanceAnnotations.Last().Value as ODataPrimitiveValue).Value);

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

            // It should contain the following annotations but we don't have such logic yet. Keep the codes for later change once we fix the issue at https://github.com/OData/odata.net/issues/3440
            // Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredAddress1@odata.type\":\"#Server.NS.UndefComplex1\",\"UndeclaredAddress1@odata.unknownName1\":\"unknown odata.xxx value1\",\"UndeclaredAddress1@NS1.abcdefg\":\"unknown abcdefghijk value2\",\"UndeclaredAddress1\":null}", result);
            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredAddress1@odata.type\":\"#Server.NS.UndefComplex1\",\"UndeclaredAddress1\":null}", result);
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
            ODataProperty undeclaredFloatIdProp = Assert.IsType<ODataProperty>(entry.Properties.First(s => string.Equals("UndeclaredFloatId", s.Name, StringComparison.Ordinal)));
            Assert.Equal(12.3m, undeclaredFloatIdProp.Value);// numeric
            Assert.Equal(2, undeclaredFloatIdProp.InstanceAnnotations.Count);

            Assert.Equal(2, complex1.Properties.Count());
            Assert.Equal("No.10000000999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(complex1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal))).Value); // string
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
            },
            settings: new ODataMessageReaderSettings
            {
                Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType & ~ValidationKinds.ThrowIfTypeConflictsWithMetadata
            });

            Assert.Single(entry.Properties);

            Assert.NotNull(complex1);
            Assert.Equal("Server.NS.AddressInValid", complex1.TypeName);
            Assert.Equal(2, complex1.Properties.Count());
            Assert.Equal("No.999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(complex1.Properties.First(c => c.Name == "Street")).Value);
            Assert.Equal("No.10000000999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(complex1.Properties.First(c => c.Name == "UndeclaredStreet")).Value);
        }

        [Fact]
        public void ReadNonOpenUnknownTypeInvalidComplexNestedTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1"":"
                + @"{""@odata.type"":""#Server.NS.AddressInValid"",'Street':""No.9,Zixing Rd Minhang"",""innerComplex1"":{""innerProp1"":null,""innerProp2"":'abc'},""UndeclaredStreet"":'No.999,Zixing Rd Minhang'}}";
            ODataResource entry = null;
            ODataResource undeclaredAddress1 = null;
            ODataResource innerComplex1 = null;
            ODataNestedResourceInfo nestedResourceInfo = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else
                    {
                        Assert.NotNull(nestedResourceInfo);
                        if (nestedResourceInfo.Name == "UndeclaredAddress1")
                        {
                            undeclaredAddress1 = (ODataResource)reader.Item;
                        }
                        else if (nestedResourceInfo.Name == "innerComplex1")
                        {
                            innerComplex1 = (ODataResource)reader.Item;
                        }
                    }
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    nestedResourceInfo = (ODataNestedResourceInfo)reader.Item;
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoEnd)
                {
                    nestedResourceInfo = null;
                }
            },
            settings: new ODataMessageReaderSettings
            {
                Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType & ~ValidationKinds.ThrowIfTypeConflictsWithMetadata
            });

            Assert.Single(entry.Properties);

            Assert.NotNull(undeclaredAddress1);
            Assert.Equal("Server.NS.AddressInValid", undeclaredAddress1.TypeName);
            Assert.Equal(2, undeclaredAddress1.Properties.Count());
            Assert.Equal("No.9,Zixing Rd Minhang", Assert.IsType<ODataProperty>(undeclaredAddress1.Properties.First(c => c.Name == "Street")).Value);
            Assert.Equal("No.999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(undeclaredAddress1.Properties.First(c => c.Name == "UndeclaredStreet")).Value);

            Assert.NotNull(innerComplex1);
            Assert.Null(innerComplex1.TypeName);
            Assert.Equal(2, innerComplex1.Properties.Count());
            Assert.Null(Assert.IsType<ODataProperty>(innerComplex1.Properties.First(c => c.Name == "innerProp1")).Value);
            Assert.Equal("abc", Assert.IsType<ODataProperty>(innerComplex1.Properties.First(c => c.Name == "innerProp2")).Value);
        }

        [Fact]
        public void ReadNonOpenUnknownTypeCollectionTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                UndeclaredCollection1:[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            IList<ODataPrimitiveValue> undeclaredCollection1Values = null;
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
                else if (reader.State == ODataReaderState.ResourceSetStart)
                {
                    Assert.Null(undeclaredCollection1Values);
                    undeclaredCollection1Values = new List<ODataPrimitiveValue>();
                }
                else if (reader.State == ODataReaderState.Primitive)
                {
                    Assert.NotNull(undeclaredCollection1Values);
                    undeclaredCollection1Values.Add((ODataPrimitiveValue)reader.Item);
                }
            });

            Assert.Equal(2, entry.Properties.Count());

            Assert.NotNull(undeclaredCollection1Values);
            Assert.Equal(3, undeclaredCollection1Values.Count);
            Assert.Equal(new[] { "email1@163.com", "email2@gmail.com", "email3@gmail2.com" }, undeclaredCollection1Values.Select(u => u.Value));
            Assert.Equal(2, complex1.Properties.Count());
        }

        #endregion

        #region open entity's property unknown name + known value type

        [Fact]
        public void ReadOpenEntryUndeclaredPropertiesWithNullValueTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3, ""UndeclaredType1"":null}";
            ODataResource entry = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                }
            });

            Assert.Equal(3, entry.Properties.Count());
            Assert.Null(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredType1", StringComparison.Ordinal))).Value);
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
            ODataResource undeclaredComplex1 = null;
            ODataResource address = null;
            ODataNestedResourceInfo nestedInfo = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (ODataResource)reader.Item;
                    }
                    else
                    {
                        Assert.NotNull(nestedInfo);
                        if (nestedInfo.Name == "undeclaredComplex1")
                        {
                            undeclaredComplex1 = (ODataResource)reader.Item;
                        }
                        else if (nestedInfo.Name == "Address")
                        {
                            address = (ODataResource)reader.Item;
                        }
                    }
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    nestedInfo = (ODataNestedResourceInfo)reader.Item;
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoEnd)
                {
                    nestedInfo = null;
                }
            });

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(2, undeclaredComplex1.Properties.Count());
            Assert.Equal("Server.NS.Address", undeclaredComplex1.TypeName);

            Assert.Equal(2, address.Properties.Count());
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
            Assert.Equal(3, Assert.IsType<ODataCollectionValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal))).ODataValue).Items.Count());
            Assert.Equal(2, complex1.Properties.Count());
        }
        #endregion

        #region open entity's property unknown name + unknown value type

        [Fact]
        public void ReadOpenEntryUndeclaredComplexPropertiesWithoutODataTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
    ""undeclaredComplex1"":{
      ""@odata.unknownName1"":""unknown odata.xxx value1"",
      ""@NS1.abcdefg"":""unknown abcdefghijk value2"",
      ""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""
    },
    ""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource undeclaredComplex1 = null;
            ODataResource address = null;
            ODataNestedResourceInfo nestedInfo = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else
                    {
                        Assert.NotNull(nestedInfo);
                        if (nestedInfo.Name == "undeclaredComplex1")
                        {
                            undeclaredComplex1 = (ODataResource)reader.Item;
                        }
                        else if (nestedInfo.Name == "Address")
                        {
                            address = (ODataResource)reader.Item;
                        }
                    }
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    nestedInfo = (ODataNestedResourceInfo)reader.Item;
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoEnd)
                {
                    nestedInfo = null;
                }
            });

            Assert.Equal(2, entry.Properties.Count());

            Assert.NotNull(undeclaredComplex1);
            Assert.Equal(2, undeclaredComplex1.Properties.Count());

            // So far, the unknown annotation starting with '@odata.' ignored. It seems unexpected.
            // Keep the codes below to track on the fix for unknown '@odata.' annotation
            //Assert.Equal(2, undeclaredComplex1.InstanceAnnotations.Count());
            //Assert.Equal("unknown odata.xxx value1", (undeclaredComplex1.InstanceAnnotations.First().Value as ODataPrimitiveValue).Value);
            //Assert.Equal("unknown abcdefghijk value2", (undeclaredComplex1.InstanceAnnotations.Last().Value as ODataPrimitiveValue).Value);

            // So, before the fix, let's verify the ignorance
            Assert.Equal("unknown abcdefghijk value2", Assert.IsType<ODataPrimitiveValue>(Assert.Single(undeclaredComplex1.InstanceAnnotations).Value).Value);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "DiffName" }); // use different name intentionally
                writer.WriteStart(undeclaredComplex1);
                writer.WriteEnd();
                writer.WriteEnd();

                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(address);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId@odata.type\":\"#Decimal\",\"UndeclaredFloatId\":12.3,\"DiffName\":{\"@NS1.abcdefg\":\"unknown abcdefghijk value2\",\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
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
            },
            settings: new ODataMessageReaderSettings
            {
                Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType & ~ValidationKinds.ThrowIfTypeConflictsWithMetadata
            });

            Assert.Equal(2, entry.Properties.Count());
            Assert.Single(complex1.Properties);
            Assert.Equal("Server.NS.AddressUndeclared", complex1.TypeName);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredEmptyComplexPropertiesTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                ""undeclaredComplex1"":{}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            ODataNestedResourceInfo nestedResourceInfo = null;
            bool readComplex = false;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else
                    {
                        Assert.NotNull(nestedResourceInfo);
                        readComplex = true;
                        complex1 = (reader.Item as ODataResource);
                    }
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    nestedResourceInfo = (ODataNestedResourceInfo)reader.Item;
                }
            });

            Assert.Equal(2, entry.Properties.Count());
            Assert.True(readComplex);
            Assert.NotNull(complex1);
            Assert.Empty(complex1.Properties);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredCollectionPropertiesWithoutODataTypeAsODataResourceSetTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                                                          UndeclaredCollection1:[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            IList<ODataPrimitiveValue> undeclaredCollection1Values = null;
            ODataResource complex1 = null;
            ODataNestedResourceInfo nestedResourceInfo = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType,
                reader =>
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
                    else if (reader.State == ODataReaderState.ResourceSetStart)
                    {
                        Assert.NotNull(nestedResourceInfo);
                        Assert.Equal("UndeclaredCollection1", nestedResourceInfo.Name);
                        Assert.Null(undeclaredCollection1Values); // guard
                        undeclaredCollection1Values = new List<ODataPrimitiveValue>();
                    }
                    else if (reader.State == ODataReaderState.Primitive)
                    {
                        Assert.NotNull(undeclaredCollection1Values);// guard
                        undeclaredCollection1Values.Add(reader.Item as ODataPrimitiveValue);
                    }
                    else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                    {
                        nestedResourceInfo = (ODataNestedResourceInfo)reader.Item;
                    }
                    else if (reader.State == ODataReaderState.NestedResourceInfoEnd)
                    {
                        nestedResourceInfo = null;
                    }
                },
                new ODataMessageReaderSettings
                {
                    ShouldIncludeAnnotation = (annotationName) => true,
                    Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType,
                    EnableUntypedCollections = true, // Sam: This configuration looks unnecessary? We should consider to remove this configuration when removing/obsolote 'ODataUntypedValue'
                });

            Assert.Equal(2, entry.Properties.Count());
            Assert.NotNull(undeclaredCollection1Values);
            Assert.Equal(3, undeclaredCollection1Values.Count);
            Assert.Equal(new[] { "email1@163.com", "email2@gmail.com", "email3@gmail2.com" }, undeclaredCollection1Values.Select(u => u.Value));

            Assert.Equal(2, complex1.Properties.Count());
        }

        [Fact]
        public void ReadOpenEntryUndeclaredEmptyCollectionPropertiesWithoutODataTypeAsODataResourceSetTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                                                          UndeclaredCollection1:[],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            bool readCollection = false;
            ODataNestedResourceInfo nestedResourceInfo = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else
                    {
                        Assert.NotNull(nestedResourceInfo);
                        Assert.Equal("Address", nestedResourceInfo.Name);

                        complex1 = (reader.Item as ODataResource);
                    }
                }
                else if (reader.State == ODataReaderState.ResourceSetStart)
                {
                    // to set the type annotation for the undeclared collection property
                    Assert.NotNull(nestedResourceInfo);
                    Assert.Equal("UndeclaredCollection1", nestedResourceInfo.Name);
                    readCollection = true;
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    nestedResourceInfo = (ODataNestedResourceInfo)reader.Item;
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoEnd)
                {
                    nestedResourceInfo = null;
                }
            },
            new ODataMessageReaderSettings
            {
                ShouldIncludeAnnotation = (annotationName) => true,
                Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType,
                EnableUntypedCollections = true,// Sam: This configuration looks unnecessary? We should consider to remove this configuration when removing/obsolote 'ODataUntypedValue'. https://github.com/OData/odata.net/issues/3441
            });

            Assert.Equal(2, entry.Properties.Count());
            Assert.True(readCollection);
            Assert.Equal("No.10000000999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredStreet", StringComparison.Ordinal))).Value);
        }
        #endregion

        #region annotation for open entity's property unknown name + unknown value type

        [Fact]
        public void ReadAnnotationInOpenEntryUndeclaredComplexPropertiesWithoutODataTypeTest()
        {
            // Be noted: '@odata.unknowname1' is skip by default during reading, see details at: ODataJsonDeserializer.SkipOverUnknownODataAnnotation(...)
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
  ""@instance.AnnotationName_"":""instance value_"",
  ""undeclaredComplex1"":{
    ""@odata.type"":""Server.NS.UnknownType1"",
    ""@odata.unknownname1"":""od unkown value 1"",
    ""@my.Annotation1"":""my custom value 1"",
    ""@instanceAnnotation1.term1"":""custom annotation value 1"",
    ""MyProp1"":""aaaaaaaaa"",
    ""UndeclaredProp1"":""bbbbbbb""
  },
  ""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";

            ODataResource topResource = null;
            ODataResource undeclaredComplex1 = null;
            ODataResource address = null;
            ODataNestedResourceInfo nestedResourceInfo = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (topResource == null)
                    {
                        topResource = (reader.Item as ODataResource);
                    }
                    else if (nestedResourceInfo != null)
                    {
                        if (nestedResourceInfo.Name == "undeclaredComplex1")
                        {
                            undeclaredComplex1 = (reader.Item as ODataResource);
                        }
                        else
                        {
                            address = (reader.Item as ODataResource);
                        }
                    }
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    nestedResourceInfo = (reader.Item as ODataNestedResourceInfo);
                }
            },
            new ODataMessageReaderSettings
            {
                ShouldIncludeAnnotation = (annotationName) => true,
                Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType & ~ValidationKinds.ThrowIfTypeConflictsWithMetadata
            });

            Assert.Equal(2, topResource.Properties.Count());
            Assert.Equal(12.3m, Assert.IsType<ODataProperty>(topResource.Properties.First(p => p.Name == "UndeclaredFloatId")).Value); // It's read as decimal type

            // Undeclared complex
            Assert.NotNull(undeclaredComplex1);
            Assert.Equal(2, undeclaredComplex1.Properties.Count());
            Assert.Equal("Server.NS.UnknownType1", undeclaredComplex1.TypeName);
            Assert.Equal(2, undeclaredComplex1.InstanceAnnotations.Count);
            Assert.Equal("aaaaaaaaa", Assert.IsType<ODataProperty>(undeclaredComplex1.Properties.First(p => p.Name == "MyProp1")).Value);
            Assert.Equal("bbbbbbb", Assert.IsType<ODataProperty>(undeclaredComplex1.Properties.First(p => p.Name == "UndeclaredProp1")).Value);

            // Declared
            Assert.NotNull(address);
            Assert.Equal(2, address.Properties.Count());
        }

        #endregion

        #region declared Edm.Untyped property + some annotations
        [Fact]
        public void ReadNonOpenEntryEdmUntypedPropertyTest()
        {
            const string payload = @"{
  ""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",
  ""Id"":61880128,
  ""UndeclaredFloatId"":12.3,
  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},
  ""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""},
  ""MyEdmUntypedProp1"":{ ""@NS1.abc"":1908,""MyProp12"":""bbb222"",abc:null}}";

            const string expected = "{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"MyEdmUntypedProp1\":{\"@NS1.abc\":1908,\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}";
            VerifyEntryEdmUntypedProperty(payload, serverEntitySet, serverEntityType, "MyEdmUntypedProp1", expected);
        }

        [Fact]
        public void ReadOpenEntryEdmUntypedPropertyTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""},
            ""MyEdmUntypedProp2@NS1.abc"":1908,MyEdmUntypedProp2:{""MyProp12"":""bbb222"",abc:null}}";

            const string expected = "{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId@odata.type\":\"#Decimal\",\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"MyEdmUntypedProp2\":{\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}";
            VerifyEntryEdmUntypedProperty(payload, this.serverOpenEntitySet, this.serverOpenEntityType, "MyEdmUntypedProp2", expected);
        }

        [Fact]
        public void ReadNonOpenEntryEdmUntypedPropertyInComplexTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},
                                  ""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang"",
                                               ""MyEdmUntypedProp3@NS1.abc"":1908,MyEdmUntypedProp3:{""MyProp12"":""bbb222"",abc:null}}}";

            const string expected = "{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"MyEdmUntypedProp3\":{\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}"; ;
            VerifyEntryEdmUntypedProperty(payload, this.serverEntitySet, this.serverEntityType, "MyEdmUntypedProp3", expected);
        }

        private void VerifyEntryEdmUntypedProperty(string payload, EdmEntitySet entitySet, EdmEntityType entityType, string propertyName, string expected)
        {
            Stack<(ODataNestedResourceInfo, ODataResource)> resources = new Stack<(ODataNestedResourceInfo, ODataResource)>();
            ODataNestedResourceInfo nestedResourceInfo = null;
            this.ReadEntryPayload(payload, entitySet, entityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    resources.Push((nestedResourceInfo, (ODataResource)reader.Item));
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    nestedResourceInfo = (ODataNestedResourceInfo)reader.Item;
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoEnd)
                {
                    nestedResourceInfo = null;
                }
            });

            Assert.Equal(4, resources.Count);

            // MyEdmUntypedProp1
            (ODataNestedResourceInfo, ODataResource) myUntypedProp1 = resources.Pop();
            Assert.Equal(propertyName, myUntypedProp1.Item1.Name);
            Assert.NotNull(myUntypedProp1.Item2);
            Assert.Equal("bbb222", Assert.IsType<ODataProperty>(myUntypedProp1.Item2.Properties.Single(s => s.Name == "MyProp12")).Value);
            Assert.Null(Assert.IsType<ODataProperty>(myUntypedProp1.Item2.Properties.Single(s => s.Name == "abc")).Value);

            // Address
            (ODataNestedResourceInfo, ODataResource) address = resources.Pop();
            Assert.Equal("Address", address.Item1.Name);
            Assert.NotNull(address.Item2);
            Assert.Equal("No.999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(address.Item2.Properties.Single(s => s.Name == "Street")).Value);
            Assert.Equal("No.10000000999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(address.Item2.Properties.Single(s => s.Name == "UndeclaredStreet")).Value);

            // undeclaredComplex1
            (ODataNestedResourceInfo, ODataResource) undeclaredComplex1 = resources.Pop();
            Assert.Equal("undeclaredComplex1", undeclaredComplex1.Item1.Name);
            Assert.NotNull(undeclaredComplex1.Item2);
            Assert.Equal("aaaaaaaaa", Assert.IsType<ODataProperty>(undeclaredComplex1.Item2.Properties.Single(s => s.Name == "MyProp1")).Value);
            Assert.Equal("bbbbbbb", Assert.IsType<ODataProperty>(undeclaredComplex1.Item2.Properties.Single(s => s.Name == "UndeclaredProp1")).Value);

            // top-level entry
            (ODataNestedResourceInfo, ODataResource) topLevel = resources.Pop();
            Assert.Null(topLevel.Item1);
            Assert.Equal(2, topLevel.Item2.Properties.Count());
            Assert.Equal(61880128, Assert.IsType<ODataProperty>(topLevel.Item2.Properties.Single(s => s.Name == "Id")).Value);
            Assert.Equal(12.3m, Assert.IsType<ODataProperty>(topLevel.Item2.Properties.Single(s => s.Name == "UndeclaredFloatId")).Value);

            // write the inner resources as the following order
            Assert.True(resources.Count == 0);
            resources.Push(address);
            resources.Push(myUntypedProp1);
            resources.Push(undeclaredComplex1);

            topLevel.Item2.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(topLevel.Item2);
            string result = this.WriteEntryPayload(entitySet, entityType, writer =>
            {
                writer.WriteStart(topLevel.Item2);
                foreach (var item in resources)
                {
                    writer.WriteStart(item.Item1); // nestedResourceInfo
                    writer.WriteStart(item.Item2);
                    writer.WriteEnd();
                    writer.WriteEnd();
                }

                writer.WriteEnd();
            });

            Assert.Equal(expected, result);
        }
        #endregion

        #region undeclared Edm.Untyped property with odata.type + some annotations
        [Fact]
        public void ReadNonOpenEntryEdmUntypedPropertyODataTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
            ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""},
            ""UndeclaredMyEdmUntypedProp1"":{""@odata.type"":""#Untyped"",""@NS1.helloworld"":true,""MyProp12"":""bbb222"",abc:null}}";

            const string expected = "{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"UndeclaredMyEdmUntypedProp1\":{\"@NS1.helloworld\":true,\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}";
            VerifyEntryEdmUntypedProperty(payload, this.serverEntitySet, this.serverEntityType, "UndeclaredMyEdmUntypedProp1", expected);
        }

        [Fact]
        public void ReadOpenEntryEdmUntypedPropertyODataTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""},
                                  ""UndeclaredMyEdmUntypedProp2"":{""@odata.type"":""#Untyped"",""@NS1.helloworld"":true,""MyProp12"":""bbb222"",abc:null}}";

            const string expected = "{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId@odata.type\":\"#Decimal\",\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"UndeclaredMyEdmUntypedProp2\":{\"@NS1.helloworld\":true,\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}";
            VerifyEntryEdmUntypedProperty(payload, this.serverOpenEntitySet, this.serverOpenEntityType, "UndeclaredMyEdmUntypedProp2", expected);
        }

        [Fact]
        public void ReadNonOpenEntryEdmUntypedPropertyODataTypeInComplexTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},
                                  ""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang"",
                                  ""UndeclaredMyEdmUntypedProp3"":{""@odata.type"":""#Untyped"",""@NS1.helloworld"":true,""MyProp12"":""bbb222"",abc:null}}}";

            const string expected = "{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"UndeclaredMyEdmUntypedProp3\":{\"@NS1.helloworld\":true,\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}";
            VerifyEntryEdmUntypedProperty(payload, this.serverEntitySet, this.serverEntityType, "UndeclaredMyEdmUntypedProp3", expected);
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

        private void ReadEntryPayload(
            string payload,
            EdmEntitySet entitySet, 
            EdmEntityType entityType, 
            Action<ODataReader> action, 
            ODataMessageReaderSettings settings = null)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", "application/json");
            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, settings ?? readerSettings, this.serverModel))
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
