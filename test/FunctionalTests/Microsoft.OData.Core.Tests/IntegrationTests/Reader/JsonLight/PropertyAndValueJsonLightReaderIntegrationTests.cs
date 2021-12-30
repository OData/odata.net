//---------------------------------------------------------------------
// <copyright file="PropertyAndValueJsonLightReaderIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData.Tests.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.Test.OData.DependencyInjection;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.IntegrationTests.Reader.JsonLight
{
    public class PropertyAndValueJsonLightReaderIntegrationTests
    {
        [Fact]
        public void ReadingPayloadInt64SingleDoubleDecimalWithoutSuffix()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = new EdmEntityType("NS", "MyTestEntity");
            EdmStructuralProperty key = entityType.AddStructuralProperty("LongId", EdmPrimitiveTypeKind.Int64, false);
            entityType.AddKeys(key);
            entityType.AddStructuralProperty("FloatId", EdmPrimitiveTypeKind.Single, false);
            entityType.AddStructuralProperty("DoubleId", EdmPrimitiveTypeKind.Double, false);
            entityType.AddStructuralProperty("DecimalId", EdmPrimitiveTypeKind.Decimal, false);

            EdmComplexType complexType = new EdmComplexType("NS", "MyTestComplexType");
            complexType.AddStructuralProperty("CLongId", EdmPrimitiveTypeKind.Int64, false);
            complexType.AddStructuralProperty("CFloatId", EdmPrimitiveTypeKind.Single, false);
            complexType.AddStructuralProperty("CDoubleId", EdmPrimitiveTypeKind.Double, false);
            complexType.AddStructuralProperty("CDecimalId", EdmPrimitiveTypeKind.Decimal, false);
            model.AddElement(complexType);
            EdmComplexTypeReference complexTypeRef = new EdmComplexTypeReference(complexType, true);
            entityType.AddStructuralProperty("ComplexProperty", complexTypeRef);

            entityType.AddStructuralProperty("LongNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt64(false))));
            entityType.AddStructuralProperty("FloatNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetSingle(false))));
            entityType.AddStructuralProperty("DoubleNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetDouble(false))));
            entityType.AddStructuralProperty("DecimalNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetDecimal(false))));
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer_sub");
            EdmEntitySet entitySet = container.AddEntitySet("MyTestEntitySet", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.MyTestEntitySet/$entity\"," +
                "\"@odata.id\":\"http://MyTestEntity\"," +
                "\"LongId\":\"12\"," +
                "\"FloatId\":34.98," +
                "\"DoubleId\":56.01," +
                "\"DecimalId\":\"78.62\"," +
                "\"LongNumbers\":[\"-1\",\"-9223372036854775808\",\"9223372036854775807\"]," +
                "\"FloatNumbers\":[-1,-3.40282347E+38,3.40282347E+38,\"INF\",\"-INF\",\"NaN\"]," +
                "\"DoubleNumbers\":[1.0,-1.7976931348623157E+308,1.7976931348623157E+308,\"INF\",\"-INF\",\"NaN\"]," +
                "\"DecimalNumbers\":[\"0\",\"-79228162514264337593543950335\",\"79228162514264337593543950335\"]," +
                "\"ComplexProperty\":{\"CLongId\":\"1\",\"CFloatId\":1,\"CDoubleId\":-1.0,\"CDecimalId\":\"0.0\"}" +
                "}";

            IEdmModel mainModel = TestUtils.WrapReferencedModelsToMainModel("EntityNs", "MyContainer", model);
            List<ODataResource> entries = new List<ODataResource>();
            ODataNestedResourceInfo navigationLink;
            this.ReadEntryPayload(mainModel, payload, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            navigationLink = (ODataNestedResourceInfo)reader.Item;
                            break;
                        default:
                            break;
                    }
                });
            Assert.Equal(2, entries.Count);
            Assert.Equal(12L, entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "LongId", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal(34.98f, entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "FloatId", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal(56.01d, entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "DoubleId", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal(78.62m, entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "DecimalId", StringComparison.OrdinalIgnoreCase)).Value);

            var complextProperty = entries[1];
            Assert.Equal(1L, complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CLongId", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal(1.0F, complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CFloatId", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal(-1.0D, complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CDoubleId", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal(0.0M, complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CDecimalId", StringComparison.OrdinalIgnoreCase)).Value);

            var longCollection = entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "LongNumbers", StringComparison.OrdinalIgnoreCase)).Value as ODataCollectionValue;
            Assert.NotNull(longCollection);
            Assert.Contains(-1L, longCollection.Items);
            Assert.Contains(long.MinValue, longCollection.Items);
            Assert.Contains(long.MaxValue, longCollection.Items);

            var floatCollection = entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "FloatNumbers", StringComparison.OrdinalIgnoreCase)).Value as ODataCollectionValue;
            Assert.NotNull(floatCollection);
            Assert.Contains(-1F, floatCollection.Items);
            Assert.Contains(float.MinValue, floatCollection.Items);
            Assert.Contains(float.MaxValue, floatCollection.Items);
            Assert.Contains(float.PositiveInfinity, floatCollection.Items);
            Assert.Contains(float.NegativeInfinity, floatCollection.Items);
            Assert.Contains(float.NaN, floatCollection.Items);

            var doubleCollection = entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "DoubleNumbers", StringComparison.OrdinalIgnoreCase)).Value as ODataCollectionValue;
            Assert.NotNull(doubleCollection);
            Assert.Contains(1.0D, doubleCollection.Items);
            Assert.Contains(double.MinValue, doubleCollection.Items);
            Assert.Contains(double.MaxValue, doubleCollection.Items);
            Assert.Contains(double.PositiveInfinity, doubleCollection.Items);
            Assert.Contains(double.NegativeInfinity, doubleCollection.Items);
            Assert.Contains(double.NaN, doubleCollection.Items);

            var decimalCollection = entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "DecimalNumbers", StringComparison.OrdinalIgnoreCase)).Value as ODataCollectionValue;
            Assert.NotNull(decimalCollection);
            Assert.Contains(0M, decimalCollection.Items);
            Assert.Contains(decimal.MinValue, decimalCollection.Items);
            Assert.Contains(decimal.MaxValue, decimalCollection.Items);
        }

        [Fact]
        public void ReadNullableCollectionValue()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = new EdmEntityType("NS", "MyTestEntity");
            EdmStructuralProperty key = entityType.AddStructuralProperty("LongId", EdmPrimitiveTypeKind.Int64, false);
            entityType.AddKeys(key);

            entityType.AddStructuralProperty("NullableIntNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(true))));
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer_sub");
            EdmEntitySet entitySet = container.AddEntitySet("MyTestEntitySet", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.MyTestEntitySet/$entity\"," +
                "\"@odata.id\":\"http://mytest\"," +
                "\"NullableIntNumbers\":[0,null,1,2]" +
                "}";

            IEdmModel mainModel = TestUtils.WrapReferencedModelsToMainModel("EntityNs", "MyContainer", model);
            ODataResource entry = null;
            this.ReadEntryPayload(mainModel, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataResource; });
            Assert.NotNull(entry);

            var intCollection = entry.Properties.FirstOrDefault(
                s => string.Equals(
                    s.Name,
                    "NullableIntNumbers",
                    StringComparison.OrdinalIgnoreCase)).Value as ODataCollectionValue;
            Assert.NotNull(intCollection);
            var list = new List<int?>();
            foreach (var val in intCollection.Items)
            {
                list.Add(val as int?);
            }

            Assert.Equal(0, list[0]);
            Assert.Null(list[1]);
            Assert.Equal(1, (int)list[2]);
            Assert.Equal(2, (int)list[3]);
        }

        [Fact]
        public void ReadOpenCollectionPropertyValue()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = new EdmEntityType("NS", "MyTestEntity", null, false, true);
            EdmStructuralProperty key = entityType.AddStructuralProperty("LongId", EdmPrimitiveTypeKind.Int64, false);
            entityType.AddKeys(key);

            EdmComplexType complexType = new EdmComplexType("NS", "MyTestComplexType");
            complexType.AddStructuralProperty("CLongId", EdmPrimitiveTypeKind.Int64, false);
            model.AddElement(complexType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer_sub");
            EdmEntitySet entitySet = container.AddEntitySet("MyTestEntitySet", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.MyTestEntitySet/$entity\"," +
                "\"@odata.id\":\"http://mytest\"," +
                "\"OpenPrimitiveCollection@odata.type\":\"Collection(Edm.Int32)\"," +
                "\"OpenPrimitiveCollection\":[0,1,2]," +
                "\"OpenComplexTypeCollection@odata.type\":\"Collection(NS.MyTestComplexType)\"," +
                "\"OpenComplexTypeCollection\":[{\"CLongId\":\"1\"},{\"CLongId\":\"1\"}]" +
                "}";

            IEdmModel mainModel = TestUtils.WrapReferencedModelsToMainModel("EntityNs", "MyContainer", model);
            ODataResource entry = null;
            List<ODataResource> complexCollection = new List<ODataResource>();
            ODataNestedResourceInfo nestedOpenCollection = null;
            bool startComplexCollection = false;
            this.ReadEntryPayload(mainModel, payload, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.NestedResourceInfoStart:
                            startComplexCollection = true;
                            break;
                        case ODataReaderState.ResourceEnd:
                            if(startComplexCollection)
                            {
                                complexCollection.Add(reader.Item as ODataResource);
                            }
                            entry = reader.Item as ODataResource;
                            break;
                        case ODataReaderState.NestedResourceInfoEnd:
                            startComplexCollection = false;
                            nestedOpenCollection = reader.Item as ODataNestedResourceInfo;
                            break;
                    }
                });
            Assert.NotNull(entry);

            var intCollection = entry.Properties.FirstOrDefault(
                s => string.Equals(
                    s.Name,
                    "OpenPrimitiveCollection",
                    StringComparison.OrdinalIgnoreCase)).Value as ODataCollectionValue;
            Assert.NotNull(intCollection);
            var list = new List<int?>();
            foreach (var val in intCollection.Items)
            {
                list.Add(val as int?);
            }

            Assert.Equal(0, list[0]);
            Assert.Equal(1, (int)list[1]);
            Assert.Equal(2, (int)list[2]);

            Assert.Equal("OpenComplexTypeCollection", nestedOpenCollection.Name);

            foreach (var val in complexCollection)
            {
                Assert.Equal(1L, val.Properties.FirstOrDefault(s => string.Equals(s.Name, "CLongId", StringComparison.OrdinalIgnoreCase)).Value);
            }
        }

        [Fact]
        public void ReadPayloadThrowExceptionWithConflictBetweenInputformatAndIeee754CompatibleValueForInt64()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = new EdmEntityType("NS", "MyTestEntity");
            EdmStructuralProperty key = entityType.AddStructuralProperty("LongId", EdmPrimitiveTypeKind.Int64, false);
            entityType.AddKeys(key);
            entityType.AddStructuralProperty("FloatId", EdmPrimitiveTypeKind.Single, false);
            entityType.AddStructuralProperty("DoubleId", EdmPrimitiveTypeKind.Double, false);
            entityType.AddStructuralProperty("DecimalId", EdmPrimitiveTypeKind.Decimal, false);

            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer_sub");
            EdmEntitySet entitySet = container.AddEntitySet("MyTestEntitySet", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.MyTestEntitySet/$entity\"," +
                "\"@odata.id\":\"http://MyTestEntity\"," +
                "\"LongId\":\"12\"," +
                "\"FloatId\":34.98," +
                "\"DoubleId\":56.01," +
                "\"DecimalId\":\"78.62\"" +
                "}";

            IEdmModel mainModel = TestUtils.WrapReferencedModelsToMainModel("EntityNs", "MyContainer", model);
            ODataResource entry = null;
            Action test = () => this.ReadEntryPayload(mainModel, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataResource; }, false);
            test.Throws<ODataException>(ODataErrorStrings.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter("Edm.Int64"));
        }

        [Fact]
        public void ReadPayloadThrowExceptionWithConflictBetweenInputformatAndIeee754CompatibleValueForIntDecimal()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = new EdmEntityType("NS", "MyTestEntity");
            entityType.AddStructuralProperty("DecimalId", EdmPrimitiveTypeKind.Decimal, false);

            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer_sub");
            EdmEntitySet entitySet = container.AddEntitySet("MyTestEntitySet", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.MyTestEntitySet/$entity\"," +
                "\"@odata.id\":\"http://MyTestEntity\"," +
                "\"DecimalId\":78.62" +
                "}";

            IEdmModel mainModel = TestUtils.WrapReferencedModelsToMainModel("EntityNs", "MyContainer", model);
            ODataResource entry = null;
            Action test = () => this.ReadEntryPayload(mainModel, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataResource; });
            test.Throws<ODataException>(ODataErrorStrings.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter("Edm.Decimal"));
        }

        [Fact]
        public void ReadingTypeDefinitionPayloadJsonLight()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);

            EdmTypeDefinition weightType = new EdmTypeDefinition("NS", "Weight", EdmPrimitiveTypeKind.Double);
            EdmTypeDefinitionReference weightTypeRef = new EdmTypeDefinitionReference(weightType, false);
            entityType.AddStructuralProperty("Weight", weightTypeRef);

            EdmComplexType complexType = new EdmComplexType("NS", "OpenAddress");
            EdmComplexTypeReference complexTypeRef = new EdmComplexTypeReference(complexType, true);

            EdmTypeDefinition addressType = new EdmTypeDefinition("NS", "Address", EdmPrimitiveTypeKind.String);
            EdmTypeDefinitionReference addressTypeRef = new EdmTypeDefinitionReference(addressType, false);
            complexType.AddStructuralProperty("CountryRegion", addressTypeRef);

            entityType.AddStructuralProperty("Address", complexTypeRef);

            model.AddElement(weightType);
            model.AddElement(addressType);
            model.AddElement(complexType);
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                "\"@odata.id\":\"http://mytest\"," +
                "\"Id\":0," +
                "\"Weight\":60.5," +
                "\"Address\":{\"CountryRegion\":\"China\"}" +
                "}";

            List<ODataResource> entries = new List<ODataResource>();
            ODataNestedResourceInfo navigationLink = null;
            this.ReadEntryPayload(model, payload, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            navigationLink = (ODataNestedResourceInfo)reader.Item;
                            break;
                        default:
                            break;
                    }
                });

            Assert.Equal(2, entries.Count);

            IList<ODataProperty> propertyList = entries[0].Properties.ToList();
            Assert.Equal("Weight", propertyList[1].Name);
            Assert.Equal(60.5, propertyList[1].Value);

            Assert.Equal("Address", navigationLink.Name);
            var address = entries[1];
            Assert.Equal("China", address.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value);
        }

        [Fact]
        public void ReadingTypeDefinitionPayloadWithTypeAnnotationJsonLight()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);

            EdmTypeDefinition weightType = new EdmTypeDefinition("NS", "Weight", EdmPrimitiveTypeKind.Int32);
            EdmTypeDefinitionReference weightTypeRef = new EdmTypeDefinitionReference(weightType, false);
            entityType.AddStructuralProperty("Weight", weightTypeRef);
            entityType.AddStructuralProperty("Height", EdmPrimitiveTypeKind.Int32);

            EdmComplexType complexType = new EdmComplexType("NS", "OpenAddress");
            EdmComplexTypeReference complexTypeRef = new EdmComplexTypeReference(complexType, true);

            EdmTypeDefinition addressType = new EdmTypeDefinition("NS", "Address", EdmPrimitiveTypeKind.String);
            EdmTypeDefinitionReference addressTypeRef = new EdmTypeDefinitionReference(addressType, false);
            complexType.AddStructuralProperty("CountryRegion", addressTypeRef);

            entityType.AddStructuralProperty("Address", complexTypeRef);

            model.AddElement(weightType);
            model.AddElement(addressType);
            model.AddElement(complexType);
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                "\"@odata.id\":\"http://mytest\"," +
                "\"Id\":0," +
                "\"Weight@odata.type\":\"#NS.Weight\"," +
                "\"Weight\":60," +
                "\"Height@odata.type\":\"#NS.Weight\"," +
                "\"Height\":180," +
                "\"Address\":{\"CountryRegion@odata.type\":\"#NS.Address\",\"CountryRegion\":\"China\"}" +
                "}";

            List<ODataResource> entries = new List<ODataResource>();
            ODataNestedResourceInfo navigationLink = null;
            this.ReadEntryPayload(model, payload, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            navigationLink = (ODataNestedResourceInfo)reader.Item;
                            break;
                        default:
                            break;
                    }
                });
            Assert.Equal(2, entries.Count);

            IList<ODataProperty> propertyList = entries[0].Properties.ToList();
            Assert.Equal("Weight", propertyList[1].Name);
            Assert.Equal(60, propertyList[1].Value);

            Assert.Equal("Height", propertyList[2].Name);
            Assert.Equal(180, propertyList[2].Value);

            Assert.Equal("Address", navigationLink.Name);
            var address = entries[1];
            Assert.Equal("China", address.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value);
        }

        [Fact]
        public void ReadingTypeDefinitionPayloadWithMultipleTypeDefinitionJsonLight()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);

            EdmTypeDefinition weightType = new EdmTypeDefinition("NS", "Weight", EdmPrimitiveTypeKind.Double);
            EdmTypeDefinitionReference weightTypeRef = new EdmTypeDefinitionReference(weightType, false);
            entityType.AddStructuralProperty("Weight", weightTypeRef);

            EdmTypeDefinition heightType = new EdmTypeDefinition("NS", "Height", EdmPrimitiveTypeKind.Double);

            EdmComplexType complexType = new EdmComplexType("NS", "OpenAddress");
            EdmComplexTypeReference complexTypeRef = new EdmComplexTypeReference(complexType, true);

            EdmTypeDefinition addressType = new EdmTypeDefinition("NS", "Address", EdmPrimitiveTypeKind.String);
            EdmTypeDefinitionReference addressTypeRef = new EdmTypeDefinitionReference(addressType, false);
            complexType.AddStructuralProperty("CountryRegion", addressTypeRef);

            EdmTypeDefinition cityType = new EdmTypeDefinition("NS", "City", EdmPrimitiveTypeKind.String);

            entityType.AddStructuralProperty("Address", complexTypeRef);

            model.AddElement(weightType);
            model.AddElement(heightType);
            model.AddElement(addressType);
            model.AddElement(cityType);
            model.AddElement(complexType);
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                "\"@odata.id\":\"http://mytest\"," +
                "\"Id\":0," +
                "\"Weight@odata.type\":\"#NS.Height\"," +
                "\"Weight\":60.5," +
                "\"Address\":{\"CountryRegion@odata.type\":\"#NS.City\",\"CountryRegion\":\"China\"}" +
                "}";

            List<ODataResource> entries = new List<ODataResource>();
            ODataNestedResourceInfo navigationLink = null;
            this.ReadEntryPayload(model, payload, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            navigationLink = (ODataNestedResourceInfo)reader.Item;
                            break;
                        default:
                            break;
                    }
                });

            Assert.Equal(2, entries.Count);

            IList<ODataProperty> propertyList = entries[0].Properties.ToList();
            Assert.Equal("Weight", propertyList[1].Name);
            Assert.Equal(60.5, propertyList[1].Value);

            Assert.Equal("Address", navigationLink.Name);
            var address = entries[1];
            Assert.Equal("China", address.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value);
        }

        [Fact]
        public void ReadingTypeDefinitionPayloadWithEdmTypeAnnotationJsonLight()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);

            EdmTypeDefinition weightType = new EdmTypeDefinition("NS", "Weight", EdmPrimitiveTypeKind.Double);
            EdmTypeDefinitionReference weightTypeRef = new EdmTypeDefinitionReference(weightType, false);
            entityType.AddStructuralProperty("Weight", weightTypeRef);

            EdmTypeDefinition heightType = new EdmTypeDefinition("NS", "Height", EdmPrimitiveTypeKind.Double);

            EdmComplexType complexType = new EdmComplexType("NS", "OpenAddress");
            EdmComplexTypeReference complexTypeRef = new EdmComplexTypeReference(complexType, true);

            EdmTypeDefinition addressType = new EdmTypeDefinition("NS", "Address", EdmPrimitiveTypeKind.String);
            EdmTypeDefinitionReference addressTypeRef = new EdmTypeDefinitionReference(addressType, false);
            complexType.AddStructuralProperty("CountryRegion", addressTypeRef);

            entityType.AddStructuralProperty("Address", complexTypeRef);

            model.AddElement(weightType);
            model.AddElement(heightType);
            model.AddElement(addressType);
            model.AddElement(complexType);
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                "\"@odata.id\":\"http://mytest\"," +
                "\"Id\":0," +
                "\"Weight@odata.type\":\"#Edm.Double\"," +
                "\"Weight\":60.5," +
                "\"Address\":{\"CountryRegion@odata.type\":\"#Edm.String\",\"CountryRegion\":\"China\"}" +
                "}";

            List<ODataResource> entries = new List<ODataResource>();
            ODataNestedResourceInfo navigationLink = null;
            this.ReadEntryPayload(model, payload, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            navigationLink = (ODataNestedResourceInfo)reader.Item;
                            break;
                        default:
                            break;
                    }
                });

            Assert.Equal(2, entries.Count);

            IList<ODataProperty> propertyList = entries[0].Properties.ToList();
            Assert.Equal("Weight", propertyList[1].Name);
            Assert.Equal(60.5, propertyList[1].Value);

            Assert.Equal("Address", navigationLink.Name);
            var address = entries[1];
            Assert.Equal("China", address.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value);
        }

        [Fact]
        public void ReadingTypeDefinitionPayloadWithIncompatibleTypeShouldFail()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);

            EdmTypeDefinition weightType = new EdmTypeDefinition("NS", "Weight", EdmPrimitiveTypeKind.Int32);
            EdmTypeDefinitionReference weightTypeRef = new EdmTypeDefinitionReference(weightType, false);
            entityType.AddStructuralProperty("Weight", weightTypeRef);
            entityType.AddStructuralProperty("Height", EdmPrimitiveTypeKind.Int32);

            model.AddElement(weightType);
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                    "\"@odata.id\":\"http://mytest\"," +
                    "\"Id\":0," +
                    "\"Weight@odata.type\":\"#Edm.String\"," +
                    "\"Weight\":60," +
                    "\"Height@odata.type\":\"#NS.Weight\"," +
                    "\"Height\":180" +
                "}";

            Action read = () => this.ReadEntryPayload(model, payload, entitySet, entityType, reader => { });
            read.Throws<ODataException>(Strings.ValidationUtils_IncompatibleType("Edm.String", "NS.Weight"));
        }

        [Fact]
        public void ReadingTypeDefinitionPayloadWithIncompatibleTypeDefinitionShouldFail()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);

            EdmTypeDefinition weightType = new EdmTypeDefinition("NS", "Weight", EdmPrimitiveTypeKind.Int32);
            EdmTypeDefinitionReference weightTypeRef = new EdmTypeDefinitionReference(weightType, false);
            entityType.AddStructuralProperty("Weight", weightTypeRef);

            EdmTypeDefinition heightType = new EdmTypeDefinition("NS", "Height", EdmPrimitiveTypeKind.Double);

            model.AddElement(weightType);
            model.AddElement(heightType);
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                    "\"@odata.id\":\"http://mytest\"," +
                    "\"Id\":0," +
                    "\"Weight@odata.type\":\"#NS.Height\"," +
                    "\"Weight\":60," +
                    "\"Height@odata.type\":\"#NS.Weight\"," +
                    "\"Height\":180" +
                "}";

            Action read = () => this.ReadEntryPayload(model, payload, entitySet, entityType, reader => { });
            read.Throws<ODataException>(Strings.ValidationUtils_IncompatibleType("NS.Height", "NS.Weight"));
        }

        [Fact]
        public void ReadingTypeDefinitionPayloadWithIncompatibleEdmTypeShouldFail()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);

            EdmTypeDefinition weightType = new EdmTypeDefinition("NS", "Weight", EdmPrimitiveTypeKind.Double);
            entityType.AddStructuralProperty("Height", EdmPrimitiveTypeKind.Int32);

            model.AddElement(weightType);
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                    "\"@odata.id\":\"http://mytest\"," +
                    "\"Id\":0," +
                    "\"Height@odata.type\":\"#NS.Weight\"," +
                    "\"Height\":180" +
                "}";

            Action read = () => this.ReadEntryPayload(model, payload, entitySet, entityType, reader => { });
            read.Throws<ODataException>(Strings.ValidationUtils_IncompatibleType("NS.Weight", "Edm.Int32"));
        }

        [Fact]
        public void ReadingNullValueForDynamicCollectionPropertyInEntityTypeShouldFail()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            entityType.AddStructuralProperty("Test", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                    "\"@odata.id\":\"http://mytest\"," +
                    "\"Id\":0," +
                    "\"Test\":null" +
                "}";

            Action read = () => this.ReadEntryPayload(model, payload, entitySet, entityType, reader => { });
            read.Throws<ODataException>("A null value was found for the property named 'Test', which has the expected type 'Collection(Edm.String)[Nullable=True]'. The expected type 'Collection(Edm.String)[Nullable=True]' cannot be null but it can have null values.");
        }

        [Fact]
        public void ReadingNullValueForDeclaredComplexProperty()
        {
            EdmModel model = new EdmModel();

            EdmComplexType complexType = new EdmComplexType("NS", "Address");
            complexType.AddStructuralProperty("CountriesOrRegions", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));
            model.AddElement(complexType);

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            entityType.AddStructuralProperty("Address", new EdmComplexTypeReference(complexType, true));
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                    "\"@odata.id\":\"http://mytest\"," +
                    "\"Id\":0," +
                    "\"Address\":null" +
                "}";

            List<ODataItem> resources = new List<ODataItem>();
            this.ReadEntryPayload(model, payload, entitySet, entityType,
                reader =>
                {
                    if(reader.State == ODataReaderState.ResourceEnd)
                    {
                        resources.Add(reader.Item);
                    }
                });

            Assert.Equal(2, resources.Count);
            Assert.Null(resources.First());
        }

        [Fact]
        public void ReadingNullValueForDeclaredCollectionPropertyInComplexTypeShouldFail()
        {
            EdmModel model = new EdmModel();

            EdmComplexType complexType = new EdmComplexType("NS", "Address");
            complexType.AddStructuralProperty("CountriesOrRegions", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));
            model.AddElement(complexType);

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            entityType.AddStructuralProperty("Address", new EdmComplexTypeReference(complexType, true));
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                    "\"@odata.id\":\"http://mytest\"," +
                    "\"Id\":0," +
                    "\"Address\":{\"CountriesOrRegions\":null}" +
                "}";

            Action read = () => this.ReadEntryPayload(model, payload, entitySet, entityType, reader => { });
            read.Throws<ODataException>("A null value was found for the property named 'CountriesOrRegions', which has the expected type 'Collection(Edm.String)[Nullable=True]'. The expected type 'Collection(Edm.String)[Nullable=True]' cannot be null but it can have null values.");
        }

        [Fact]
        public void ReadingNullValueForNonNullableCollectionOfComplexShouldThrow()
        {
            EdmModel model = new EdmModel();
            EdmComplexType complexType = new EdmComplexType("NS", "Car");
            EdmComplexTypeReference door = new EdmComplexTypeReference(new EdmComplexType("NS","Door"), false);
            complexType.AddStructuralProperty("Doors", new EdmCollectionTypeReference(new EdmCollectionType(door)));
            model.AddElement(complexType);

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            entityType.AddStructuralProperty("Car", new EdmComplexTypeReference(complexType, false));
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                    "\"@odata.id\":\"http://mytest\"," +
                    "\"Id\":0," +
                    "\"Car\": { \"Doors\":[null,null]}" +
                "}";

            List<ODataItem> resources = new List<ODataItem>();
            Action test = () => this.ReadEntryPayload(model, payload, entitySet, entityType,
                reader =>
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        resources.Add(reader.Item);
                    }
                });
            test.Throws<ODataException>("A null value was found with the expected type 'NS.Door[Nullable=False]'. The expected type 'NS.Door[Nullable=False]' does not allow null values.");
        }

        [Fact]
        public void ReadingNullValueForNonNullableCollectionOfPrimitiveShouldThrow()
        {
            EdmModel model = new EdmModel();
            EdmComplexType complexType = new EdmComplexType("NS", "Car");
            complexType.AddStructuralProperty("DoorNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));
            model.AddElement(complexType);

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            entityType.AddStructuralProperty("Car", new EdmComplexTypeReference(complexType, true));
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                    "\"@odata.id\":\"http://mytest\"," +
                    "\"Id\":0," +
                    "\"Car\": { \"DoorNumbers\":[1,null]}" +
                "}";

            List<ODataItem> resources = new List<ODataItem>();
            Action test = () => this.ReadEntryPayload(model, payload, entitySet, entityType,
                reader =>
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        resources.Add(reader.Item);
                    }
                });
            test.Throws<ODataException>("A null value was found with the expected type 'Edm.Int32[Nullable=False]'. The expected type 'Edm.Int32[Nullable=False]' does not allow null values.");
        }

        [Fact]
        public void ReadingNullValueForNonNullableCollectionOfEnumsShouldThrow()
        {
            EdmModel model = new EdmModel();
            EdmComplexType complexType = new EdmComplexType("NS", "Car");

            // enum with flags
            var enumFlagsType = new EdmEnumType("NS", "ColorFlags", EdmPrimitiveTypeKind.Int64, true);
            enumFlagsType.AddMember("Red", new EdmEnumMemberValue(1L));
            enumFlagsType.AddMember("Green", new EdmEnumMemberValue(2L));
            enumFlagsType.AddMember("Blue", new EdmEnumMemberValue(4L));
            enumFlagsType.AddMember("GreenRed", new EdmEnumMemberValue(3L));
            complexType.AddStructuralProperty("DoorColors", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEnumTypeReference(enumFlagsType, false))));
            model.AddElement(complexType);

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            entityType.AddStructuralProperty("Car", new EdmComplexTypeReference(complexType, true));
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                    "\"@odata.id\":\"http://mytest\"," +
                    "\"Id\":0," +
                    "\"Car\": { \"DoorColors\":[ 'NS.Red' ,null]}" +
                "}";

            List<ODataItem> resources = new List<ODataItem>();
            Action test = () => this.ReadEntryPayload(model, payload, entitySet, entityType,
                reader =>
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        resources.Add(reader.Item);
                    }
                });
            test.Throws<ODataException>("A null value was found with the expected type 'NS.ColorFlags[Nullable=False]'. The expected type 'NS.ColorFlags[Nullable=False]' does not allow null values.");
        }

        [Fact]
        public void ReadingNullValueForDynamicCollectionPropertyInOpenStructuralTypeShouldWork()
        {
            EdmModel model = new EdmModel();

            EdmComplexType complexType = new EdmComplexType("NS", "Address", null, false, true);
            complexType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String);
            model.AddElement(complexType);

            EdmEntityType entityType = new EdmEntityType("NS", "Person", null, false, true);
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            entityType.AddStructuralProperty("Address", new EdmComplexTypeReference(complexType, false));
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                    "\"@odata.id\":\"http://mytest\"," +
                    "\"Id\":0," +
                    "\"Test@odata.type\":\"#Collection(Edm.String)\"," +
                    "\"Test\":null," +
                    "\"Address\":{\"CountryRegion\":\"China\",\"Test1@odata.type\":\"#Collection(Edm.Int32)\",\"Test1\":null}" +
                "}";

            ODataResource entry = null;

            this.ReadEntryPayload(model, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataResource; });
        }

        [Fact]
        public void ReadDateTimeOffsetWithCustomFormat()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            model.AddElement(entityType);
            entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            entityType.AddStructuralProperty("Birthday", EdmPrimitiveTypeKind.DateTimeOffset);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                "\"@odata.id\":\"http://mytest\"," +
                "\"Id\":0," +
                "\"Birthday\":\"Thu, 12 Apr 2012 18:43:10 GMT\"" +
                "}";

            ODataResource entry = null;
            var diContainer = ContainerBuilderHelper.BuildContainer(
                builder => builder.AddService<ODataPayloadValueConverter, DateTimeOffsetCustomFormatPrimitivePayloadValueConverter>(ServiceLifetime.Singleton));
            this.ReadEntryPayload(model, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataResource; }, container: diContainer);
            Assert.NotNull(entry);

            IList<ODataProperty> propertyList = entry.Properties.ToList();
            var birthday = propertyList[1].Value as DateTimeOffset?;
            Assert.True(birthday.HasValue);
            Assert.Equal(new DateTimeOffset(2012, 4, 12, 18, 43, 10, TimeSpan.Zero), birthday.Value);
        }

        [Fact]
        public void ReadDateTimeOffsetProperty_WithDifferentCase_WorksAsExpected()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            model.AddElement(entityType);
            entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            entityType.AddStructuralProperty("Birthday", EdmPrimitiveTypeKind.DateTimeOffset);

            EdmEntityContainer container = new EdmEntityContainer("NS", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            // Disable case-insensitive - read as untyped string
            ReadDateTimeOffsetPayloadAndVerify(model, entitySet, entityType, "birthday", false, s =>
            {
                Assert.NotNull(s);
                ODataProperty odataProperty = Assert.Single(s.Properties);
                Assert.Equal("birthday", odataProperty.Name);
                ODataUntypedValue untypedValue = Assert.IsType<ODataUntypedValue>(odataProperty.Value);
                Assert.Equal("\"2021-10-28T21:33:26+08:00\"", untypedValue.RawValue);
            },
            /*readUntypedAsString*/ true);

            // Disable case-insensitive - read as untyped value
            ReadDateTimeOffsetPayloadAndVerify(model, entitySet, entityType, "birthday", false, s =>
            {
                Assert.NotNull(s);
                ODataProperty odataProperty = Assert.Single(s.Properties);
                Assert.Equal("birthday", odataProperty.Name);
                string value = Assert.IsType<string>(odataProperty.Value);
                Assert.Equal("2021-10-28T21:33:26+08:00", value);
            },
            /*readUntypedAsString*/ false);

            // Enable case sensitive
            ReadDateTimeOffsetPayloadAndVerify(model, entitySet, entityType, "birthday", true, s =>
            {
                Assert.NotNull(s);
                ODataProperty odataProperty = Assert.Single(s.Properties);
                Assert.Equal("Birthday", odataProperty.Name);
                Assert.Equal(new DateTimeOffset(2021, 10, 28, 21, 33, 26, TimeSpan.FromHours(8)), odataProperty.Value);
            });
        }

        private void ReadDateTimeOffsetPayloadAndVerify(IEdmModel model, EdmEntitySet entitySet, IEdmEntityType entityType, string propertyName, bool enablePropertyCaseInsensitive,
            Action<ODataResource> verifyAction, bool readUntypedAsString = true)
        {
            string payloadFormat =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#NS.MyContainer.People/$entity\"," +
                "\"@odata.id\":\"http://mytest\"," +
                "\"PROPERTYNAME\":\"2021-10-28T21:33:26+08:00\"" +
                "}";
            string payload = payloadFormat.Replace("PROPERTYNAME", propertyName);

            ODataResource resource = null;
            this.ReadEntryPayload(model, payload, entitySet, entityType,
                reader => { resource = resource ?? reader.Item as ODataResource; }, true, null, enablePropertyCaseInsensitive, readUntypedAsString);

            verifyAction(resource);
        }

        [Fact]
        public void ReadPropertyWithCaseInsensitive_WorksAsExpected()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            model.AddElement(entityType);
            entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            entityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            entityType.AddStructuralProperty("nAme", EdmPrimitiveTypeKind.Int32);
            entityType.AddStructuralProperty("naMe", EdmPrimitiveTypeKind.Double);

            EdmEntityContainer container = new EdmEntityContainer("NS", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            // Match - Edm.String
            ReadPayloadAndVerify(model, entitySet, entityType, "\"Name\":\"abc\"", s =>
            {
                Assert.NotNull(s);
                ODataProperty odataProperty = Assert.Single(s.Properties);
                Assert.Equal("Name", odataProperty.Name);
                Assert.Equal("abc", odataProperty.Value);
            });

            // Match - Edm.Int32
            ReadPayloadAndVerify(model, entitySet, entityType, "\"nAme\":42", s =>
            {
                Assert.NotNull(s);
                ODataProperty odataProperty = Assert.Single(s.Properties);
                Assert.Equal("nAme", odataProperty.Name);
                Assert.Equal(42, odataProperty.Value);
            });

            // Match - Edm.Double
            ReadPayloadAndVerify(model, entitySet, entityType, "\"naMe\":3.14", s =>
            {
                Assert.NotNull(s);
                ODataProperty odataProperty = Assert.Single(s.Properties);
                Assert.Equal("naMe", odataProperty.Name);
                Assert.Equal(3.14, odataProperty.Value);
            });

            // Doesn't match any property, read as untyped.
            ReadPayloadAndVerify(model, entitySet, entityType, "\"name\":\"Dynamic Property\"", s =>
            {
                Assert.NotNull(s);
                ODataProperty odataProperty = Assert.Single(s.Properties);
                Assert.Equal("name", odataProperty.Name);
                ODataUntypedValue untypedValue = Assert.IsType<ODataUntypedValue>(odataProperty.Value);
                Assert.Equal("\"Dynamic Property\"", untypedValue.RawValue);
            });
        }

        private void ReadPayloadAndVerify(IEdmModel model, EdmEntitySet entitySet, IEdmEntityType entityType, string propertyValue, Action<ODataResource> verifyAction)
        {
            string payloadFormat =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#NS.MyContainer.People/$entity\"," +
                "\"@odata.id\":\"http://mytest\"," +
                "PROPERTYVALUE" +
                "}";
            string payload = payloadFormat.Replace("PROPERTYVALUE", propertyValue);

            ODataResource resource = null;
            this.ReadEntryPayload(model, payload, entitySet, entityType,
                reader => { resource = resource ?? reader.Item as ODataResource; }, true, null, enablePropertyCaseInsensitive: true);

            verifyAction(resource);
        }

        private void ReadEntryPayload(IEdmModel userModel, string payload, EdmEntitySet entitySet, IEdmEntityType entityType,
            Action<ODataReader> action, bool isIeee754Compatible = true, IServiceProvider container = null,
            bool enablePropertyCaseInsensitive = false, bool readUntypedAsString = true)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)), Container = container};
            string contentType = isIeee754Compatible
                ? "application/json;odata.metadata=minimal;IEEE754Compatible=true"
                : "application/json;odata.metadata=minimal;IEEE754Compatible=false";
            message.SetHeader("Content-Type", contentType);
            var readerSettings = new ODataMessageReaderSettings
            {
                EnableMessageStreamDisposal = true,
                EnablePropertyNameCaseInsensitive = enablePropertyCaseInsensitive,
                ReadUntypedAsString = readUntypedAsString
            };
            readerSettings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, readerSettings, userModel))
            {
                var reader = msgReader.CreateODataResourceReader(entitySet, entityType);
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }
    }
}
