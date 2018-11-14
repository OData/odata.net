//---------------------------------------------------------------------
// <copyright file="PropertyAndValueJsonLightReaderIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Tests.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Tests.Evaluation;
using Microsoft.Test.OData.DependencyInjection;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.IntegrationTests.Reader.JsonLight
{
    public class PropertyAndValueJsonLightReaderIntegrationTests
    {
        private static string omitValuesNulls = "omit-values=" + ODataConstants.OmitValuesNulls;

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
            ODataNestedResourceInfo nestedResourceInfo;
            this.ReadEntryPayload(mainModel, payload, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            nestedResourceInfo = (ODataNestedResourceInfo)reader.Item;
                            break;
                        default:
                            break;
                    }
                });
            Assert.Equal(2, entries.Count);
            entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "LongId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(12L, "value should be in correct type.");
            entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "FloatId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(34.98f, "value should be in correct type.");
            entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "DoubleId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(56.01d, "value should be in correct type.");
            entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "DecimalId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(78.62m, "value should be in correct type.");

            var complextProperty = entries[1];
            complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CLongId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1L, "value should be in correct type.");
            complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CFloatId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1.0F, "value should be in correct type.");
            complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CDoubleId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(-1.0D, "value should be in correct type.");
            complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CDecimalId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(0.0M, "value should be in correct type.");

            var longCollection = entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "LongNumbers", StringComparison.OrdinalIgnoreCase)).Value.As<ODataCollectionValue>();
            longCollection.Items.Should().Contain(-1L).And.Contain(long.MinValue).And.Contain(long.MaxValue);
            var floatCollection = entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "FloatNumbers", StringComparison.OrdinalIgnoreCase)).Value.As<ODataCollectionValue>();
            floatCollection.Items.Should().Contain(-1F).And.Contain(float.MinValue).And.Contain(float.MaxValue).And.Contain(float.PositiveInfinity).And.Contain(float.NegativeInfinity).And.Contain(float.NaN);
            var doubleCollection = entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "DoubleNumbers", StringComparison.OrdinalIgnoreCase)).Value.As<ODataCollectionValue>();
            doubleCollection.Items.Should().Contain(1.0D).And.Contain(double.MinValue).And.Contain(double.MaxValue).And.Contain(double.PositiveInfinity).And.Contain(double.NegativeInfinity).And.Contain(double.NaN);
            var decimalCollection = entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "DecimalNumbers", StringComparison.OrdinalIgnoreCase)).Value.As<ODataCollectionValue>();
            decimalCollection.Items.Should().Contain(0M).And.Contain(decimal.MinValue).And.Contain(decimal.MaxValue);
        }

        [Fact]
        public void ReadNullableCollectionValueExpanded()
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

            foreach (bool isNullValuesOmitted in new bool[] {false, true})
            {
                ODataResource entry = null;
                this.ReadEntryPayload(mainModel, payload, entitySet, entityType,
                    reader => { entry = entry ?? reader.Item as ODataResource; }, isNullValuesOmitted);
                Assert.NotNull(entry);

                var intCollection = entry.Properties.FirstOrDefault(
                    s => string.Equals(
                        s.Name,
                        "NullableIntNumbers",
                        StringComparison.OrdinalIgnoreCase)).Value.As<ODataCollectionValue>();
                var list = new List<int?>();
                foreach (var val in intCollection.Items)
                {
                    list.Add(val as int?);
                }

                Assert.Equal(0, list[0]);
                Assert.Null(list[1]);
                Assert.Equal(1, (int) list[2]);
                Assert.Equal(2, (int) list[3]);
            }
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

            foreach (bool isNullValuesOmitted in new bool[] {false, true})
            {
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
                                if (startComplexCollection)
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
                    },
                    nullValuesOmitted: isNullValuesOmitted);
                Assert.NotNull(entry);

                var intCollection = entry.Properties.FirstOrDefault(
                    s => string.Equals(
                        s.Name,
                        "OpenPrimitiveCollection",
                        StringComparison.OrdinalIgnoreCase)).Value.As<ODataCollectionValue>();
                var list = new List<int?>();
                foreach (var val in intCollection.Items)
                {
                    list.Add(val as int?);
                }

                Assert.Equal(0, list[0]);
                Assert.Equal(1, (int) list[1]);
                Assert.Equal(2, (int) list[2]);

                Assert.Equal("OpenComplexTypeCollection", nestedOpenCollection.Name);

                foreach (var val in complexCollection)
                {
                    val.Properties.FirstOrDefault(
                        s => string.Equals(s.Name, "CLongId", StringComparison.OrdinalIgnoreCase))
                        .Value.ShouldBeEquivalentTo(1L, "value should be in correct type.");
                }
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
            test.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter("Edm.Int64"));
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
            test.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter("Edm.Decimal"));
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
            ODataNestedResourceInfo nestedResourceInfo = null;
            this.ReadEntryPayload(model, payload, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            nestedResourceInfo = (ODataNestedResourceInfo)reader.Item;
                            break;
                        default:
                            break;
                    }
                });

            Assert.Equal(2, entries.Count);

            IList<ODataProperty> propertyList = entries[0].Properties.ToList();
            propertyList[1].Name.Should().Be("Weight");
            propertyList[1].Value.Should().Be(60.5);

            nestedResourceInfo.Name.Should().Be("Address");
            var address = entries[1];
            address.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value.Should().Be("China");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ReadingTypeDefinitionPayloadJsonLightWithOmittedNullValues(bool bNullValuesOmitted)
        {
            EdmEntityType entityType;
            EdmEntitySet entitySet;
            EdmModel model = BuildEdmModelForOmittedNullValuesTestCases(out entityType, out entitySet);

            const string payload = @"
            {
                ""@odata.context"":""http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity"",
                ""@odata.id"":""http://mytest"",
                ""Id"":0,
                ""Weight"":60.5,
                ""Address"":{""CountryRegion"":""US"", ""City"":""Redmond""},
                ""Education"":null
            }";

            List<ODataResource> entries = new List<ODataResource>();
            List<ODataNestedResourceInfo> nestedResourceInfos = new List<ODataNestedResourceInfo>();
            string reading = "init";
            this.ReadEntryPayload(model, payload, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            if (reading.Equals("Education", StringComparison.Ordinal))
                            {
                                reader.Item.Should().BeNull();
                            }
                            else
                            {
                                entries.Add(reader.Item as ODataResource);
                            }
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            nestedResourceInfos.Add((ODataNestedResourceInfo)reader.Item);
                            reading = (reader.Item as ODataNestedResourceInfo).Name;
                            break;
                        default:
                            break;
                    }
                },
                nullValuesOmitted: bNullValuesOmitted);

            Assert.Equal(2, entries.Count);

            ODataResource person = entries[0];
            person.Properties.FirstOrDefault(s => string.Equals(s.Name, "Id", StringComparison.Ordinal))
                .Value.Should().Be(0);
            person.Properties.FirstOrDefault(s => string.Equals(s.Name, "Weight", StringComparison.Ordinal))
                .Value.Should().Be(60.5);

            ODataResource address = entries[1];
            address.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.Ordinal))
                .Value.Should().Be("US");
            address.Properties.FirstOrDefault(s => string.Equals(s.Name, "City", StringComparison.Ordinal))
                .Value.Should().Be("Redmond");

            if (bNullValuesOmitted)
            {
                // Omitted value should be restored to null.
                address.Properties.FirstOrDefault(s => string.Equals(s.Name, "ZipCode", StringComparison.Ordinal))
                    .Value.Should().BeNull();
            }
            else
            {
                address.Properties.Any(s => string.Equals(s.Name, "ZipCode", StringComparison.Ordinal)).Should().BeFalse();
            }

            nestedResourceInfos.Count().Should().Be(3);
            nestedResourceInfos.Any(info => info.Name.Equals("Address")).Should().BeTrue();
            nestedResourceInfos.Any(info => info.Name.Equals("Education")).Should().BeTrue();
            nestedResourceInfos.Any(info => info.Name.Equals("Company")).Should().BeTrue();
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
            ODataNestedResourceInfo nestedResourceInfo = null;
            this.ReadEntryPayload(model, payload, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            nestedResourceInfo = (ODataNestedResourceInfo)reader.Item;
                            break;
                        default:
                            break;
                    }
                });
            Assert.Equal(2, entries.Count);

            IList<ODataProperty> propertyList = entries[0].Properties.ToList();
            propertyList[1].Name.Should().Be("Weight");
            propertyList[1].Value.Should().Be(60);

            propertyList[2].Name.Should().Be("Height");
            propertyList[2].Value.Should().Be(180);

            nestedResourceInfo.Name.Should().Be("Address");
            var address = entries[1];
            address.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value.Should().Be("China");
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
            ODataNestedResourceInfo nestedResourceInfo = null;
            this.ReadEntryPayload(model, payload, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            nestedResourceInfo = (ODataNestedResourceInfo)reader.Item;
                            break;
                        default:
                            break;
                    }
                });

            Assert.Equal(2, entries.Count);

            IList<ODataProperty> propertyList = entries[0].Properties.ToList();
            propertyList[1].Name.Should().Be("Weight");
            propertyList[1].Value.Should().Be(60.5);

            nestedResourceInfo.Name.Should().Be("Address");
            var address = entries[1];
            address.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value.Should().Be("China");
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
            ODataNestedResourceInfo nestedResourceInfo = null;
            this.ReadEntryPayload(model, payload, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            nestedResourceInfo = (ODataNestedResourceInfo)reader.Item;
                            break;
                        default:
                            break;
                    }
                });

            Assert.Equal(2, entries.Count);

            IList<ODataProperty> propertyList = entries[0].Properties.ToList();
            propertyList[1].Name.Should().Be("Weight");
            propertyList[1].Value.Should().Be(60.5);

            nestedResourceInfo.Name.Should().Be("Address");
            var address = entries[1];
            address.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value.Should().Be("China");
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
            read.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatibleType("Edm.String", "NS.Weight"));
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
            read.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatibleType("NS.Height", "NS.Weight"));
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
            read.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatibleType("NS.Weight", "Edm.Int32"));
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
            read.ShouldThrow<ODataException>().WithMessage("A null value was found for the property named 'Test', which has the expected type 'Collection(Edm.String)[Nullable=False]'. The expected type 'Collection(Edm.String)[Nullable=False]' does not allow null values.");
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
            read.ShouldThrow<ODataException>().WithMessage("A null value was found for the property named 'CountriesOrRegions', which has the expected type 'Collection(Edm.String)[Nullable=False]'. The expected type 'Collection(Edm.String)[Nullable=False]' does not allow null values.");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ReadingPropertyInTopLevelOrInComplexTypeShouldRestoreOmittedNullValues(
            bool bNullValuesOmitted)
        {
            EdmEntityType entityType;
            EdmEntitySet entitySet;
            EdmModel model = BuildEdmModelForOmittedNullValuesTestCases(out entityType, out entitySet);

            const string payload =@"{
                    ""@odata.context"":""http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity"",
                    ""@odata.id"":""http://mytest"",
                    ""Id"":0,
                    ""Education"":{""Id"":1}
                }";

            List<ODataResource> entries = new List<ODataResource>();
            this.ReadEntryPayload(model, payload, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        default:
                            break;
                    }
                },
                nullValuesOmitted: bNullValuesOmitted);

            Assert.Equal(2, entries.Count);

            ODataResource edu = entries.FirstOrDefault(s => string.Equals(s.TypeName, "NS.Edu", StringComparison.Ordinal));
            edu.Should().NotBeNull();
            edu.Properties.FirstOrDefault(s => string.Equals(s.Name, "Id", StringComparison.Ordinal)).Value.Should().Be(1);

            if (bNullValuesOmitted)
            {
                // null-able collection should be restored.
                edu.Properties.FirstOrDefault(s => string.Equals(s.Name, "Campuses", StringComparison.Ordinal)).Value.Should().BeNull();
                // null-able property value should be restored.
                edu.Properties.FirstOrDefault(s => string.Equals(s.Name, "SchoolName", StringComparison.Ordinal)).Value.Should().BeNull();
                edu.Properties.FirstOrDefault(s => string.Equals(s.Name, "OrgId", StringComparison.Ordinal)).Value.Should().BeNull();
                edu.Properties.FirstOrDefault(s => string.Equals(s.Name, "OrgCategory", StringComparison.Ordinal)).Value.Should().BeNull();

            }
            else
            {
                edu.Properties.Any(s => string.Equals(s.Name, "Campuses", StringComparison.Ordinal)).Should().BeFalse();
                edu.Properties.Any(s => string.Equals(s.Name, "SchoolName", StringComparison.Ordinal)).Should().BeFalse();
                edu.Properties.Any(s => string.Equals(s.Name, "OrgId", StringComparison.Ordinal)).Should().BeFalse();
                edu.Properties.Any(s => string.Equals(s.Name, "OrgCategory", StringComparison.Ordinal)).Should().BeFalse();
            }

            ODataResource person = entries.FirstOrDefault(s => string.Equals(s.TypeName, "NS.Person", StringComparison.Ordinal));
            person.Should().NotBeNull();
            person.Properties.FirstOrDefault(s => string.Equals(s.Name, "Id", StringComparison.Ordinal)).Value.Should().Be(0);

            if (bNullValuesOmitted)
            {
                // omitted null-able property should be restored as null.
                person.Properties.FirstOrDefault(s => string.Equals(s.Name, "Height", StringComparison.Ordinal)).Value.Should().BeNull();
            }
            else
            {
                person.Properties.Any(s => string.Equals(s.Name, "Height", StringComparison.Ordinal)).Should().BeFalse();
            }

            // missing non-null-able property should not be restored.
            person.Properties.Any(s => string.Equals(s.Name, "Weight", StringComparison.Ordinal)).Should().BeFalse();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ReadingPropertyInTopLevelOrInComplexTypeShouldRestoreOmittedNullValuesWithSelectedPropertiesEntireSubTree(
            bool bNullValuesOmitted)
        {
            EdmEntityType entityType;
            EdmEntitySet entitySet;
            EdmModel model = BuildEdmModelForOmittedNullValuesTestCases(out entityType, out entitySet);

            // null-able property Height is not selected, thus should not be restored.
            // null-able property Address is selected, thus should be restored.
            // Property Education is null-able.
            const string payloadWithQueryOption = @"{
                    ""@odata.context"":""http://www.example.com/$metadata#EntityNs.MyContainer.People(Id,UnknownPropX,Education,Address)/$entity"",
                    ""@odata.id"":""http://mytest"",
                    ""Id"":0,
                    ""Education"":{""Id"":1},
                    ""Address"":null
                }";
            const string payloadWithWildcardInQueryOption = @"{
                    ""@odata.context"":""http://www.example.com/$metadata#EntityNs.MyContainer.People(Id,UnknownPropX,Education/*,Address)/$entity"",
                    ""@odata.id"":""http://mytest"",
                    ""Id"":0,
                    ""Education"":{""Id"":1},
                    ""Address"":null
                }";

            foreach (string payload in new string[] {payloadWithQueryOption, payloadWithWildcardInQueryOption})
            {
                List<ODataResource> entries = new List<ODataResource>();
                List<ODataNestedResourceInfo> nestedResourceInfos = new List<ODataNestedResourceInfo>();
                string reading = "init";
                this.ReadEntryPayload(model, payload, entitySet, entityType,
                    reader =>
                    {
                        switch (reader.State)
                        {
                            case ODataReaderState.ResourceStart:
                                if (reading.Equals("Address", StringComparison.Ordinal))
                                {
                                    reader.Item.Should().BeNull();
                                }
                                else
                                {
                                    entries.Add(reader.Item as ODataResource);
                                }
                                break;
                            case ODataReaderState.NestedResourceInfoStart:
                                nestedResourceInfos.Add(reader.Item as ODataNestedResourceInfo);
                                reading = (reader.Item as ODataNestedResourceInfo).Name;
                                break;
                            default:
                                break;
                        }
                    },
                    nullValuesOmitted: bNullValuesOmitted);

                entries.Count.Should().Be(2);
                nestedResourceInfos.Count.Should().Be(2);
                nestedResourceInfos.FirstOrDefault(s => string.Equals(s.Name, "Education", StringComparison.Ordinal))
                    .Should().NotBeNull();
                nestedResourceInfos.FirstOrDefault(s => string.Equals(s.Name, "Address", StringComparison.Ordinal))
                    .Should().NotBeNull();

                // Education
                ODataResource edu =
                    entries.FirstOrDefault(s => string.Equals(s.TypeName, "NS.Edu", StringComparison.Ordinal));
                edu.Should().NotBeNull();
                edu.Properties.FirstOrDefault(s => string.Equals(s.Name, "Id", StringComparison.Ordinal))
                    .Value.Should().Be(1);
                if (bNullValuesOmitted)
                {
                    // null-able collection should be restored.
                    edu.Properties.FirstOrDefault(s => string.Equals(s.Name, "Campuses", StringComparison.Ordinal))
                        .Value.Should().BeNull();
                    // null-able property value should be restored.
                    edu.Properties.FirstOrDefault(s => string.Equals(s.Name, "SchoolName", StringComparison.Ordinal))
                        .Value.Should().BeNull();
                }
                else
                {
                    edu.Properties.Any(s => string.Equals(s.Name, "Campuses", StringComparison.Ordinal)).Should().BeFalse();
                    edu.Properties.Any(s => string.Equals(s.Name, "SchoolName", StringComparison.Ordinal)).Should().BeFalse();
                }

                // Person
                ODataResource person =
                    entries.FirstOrDefault(s => string.Equals(s.TypeName, "NS.Person", StringComparison.Ordinal));
                person.Should().NotBeNull();

                // Verify that unknown property doesn't cause anomaly. And it is restored only when omit-values=nulls is specified.
                if (bNullValuesOmitted)
                {
                    person.Properties.Single(s => string.Equals(s.Name, "UnknownPropX", StringComparison.Ordinal)).Value
                        .Should().BeNull();
                }
                else
                {
                    person.Properties.Any(s => string.Equals(s.Name, "UnknownPropX", StringComparison.Ordinal)).Should().BeFalse();
                }

                person.Properties.FirstOrDefault(s => string.Equals(s.Name, "Id", StringComparison.Ordinal))
                    .Value.Should().Be(0);

                // null-able but not selected properties and not-null-able properties should not be restored.
                person.Properties.Any(s => string.Equals(s.Name, "Height", StringComparison.Ordinal)).Should().BeFalse();
                person.Properties.Any(s => string.Equals(s.Name, "Weight", StringComparison.Ordinal)).Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ReadingPropertyInTopLevelOrInComplexTypeShouldRestoreOmittedNullValuesWithSelectedPropertiesPartialSubTree(
            bool bNullValuesOmitted)
        {
            EdmEntityType entityType;
            EdmEntitySet entitySet;
            EdmModel model = BuildEdmModelForOmittedNullValuesTestCases(out entityType, out entitySet);

            // null-able property Height is not selected, thus should not be restored.
            // null-able property Address is selected, thus should be restored.
            // Property Education is null-able.
            const string payloadWithSelectedPropertiesPartialSubTreeInQueryOption = @"{
                    ""@odata.context"":""http://www.example.com/$metadata#EntityNs.MyContainer.People(Id,%20Education/Id,%20Education/SchoolName,%20Education/UnknownPropX,%20Address)/$entity"",
                    ""@odata.id"":""http://mytest"",
                    ""Id"":0,
                    ""Education"":{""Id"":1}
                }";

            List<ODataResource> entries = new List<ODataResource>();
            List<ODataNestedResourceInfo> nestedResourceInfos = new List<ODataNestedResourceInfo>();
            this.ReadEntryPayload(model, payloadWithSelectedPropertiesPartialSubTreeInQueryOption, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            nestedResourceInfos.Add(reader.Item as ODataNestedResourceInfo);
                            break;
                        default:
                            break;
                    }
                },
                nullValuesOmitted: bNullValuesOmitted);

            entries.Count.Should().Be(2);
            nestedResourceInfos.Count.Should().Be(1);
            nestedResourceInfos.FirstOrDefault(s => string.Equals(s.Name, "Education", StringComparison.Ordinal))
                .Should().NotBeNull();

            // Education
            ODataResource edu =
                entries.FirstOrDefault(s => string.Equals(s.TypeName, "NS.Edu", StringComparison.Ordinal));
            edu.Should().NotBeNull();

            // Verify that unknown property doesn't cause anomaly. And it is restored only when omit-values=nulls is specified.
            if (bNullValuesOmitted)
            {
                edu.Properties.Single(s => string.Equals(s.Name, "UnknownPropX", StringComparison.Ordinal)).Value
                    .Should().BeNull();
            }
            else
            {
                edu.Properties.Any(s => string.Equals(s.Name, "UnknownPropX", StringComparison.Ordinal))
                    .Should().BeFalse();
            }

            edu.Properties.FirstOrDefault(s => string.Equals(s.Name, "Id", StringComparison.Ordinal))
                .Value.Should().Be(1);
            if (bNullValuesOmitted)
            {
                // null-able property value should be restored.
                edu.Properties.FirstOrDefault(s => string.Equals(s.Name, "SchoolName", StringComparison.Ordinal))
                    .Value.Should().BeNull();
            }
            else
            {
                edu.Properties.Any(s => string.Equals(s.Name, "SchoolName", StringComparison.Ordinal)).Should().BeFalse();
            }

            // not selected property should not be restored.
            edu.Properties.Any(s => string.Equals(s.Name, "Campuses", StringComparison.Ordinal)).Should().BeFalse();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ReadingUnknownPropertyOfNonOpenTypeShouldWork(bool bNullValuesOmitted)
        {
            EdmEntityType entityType;
            EdmEntitySet entitySet;
            EdmModel model = BuildEdmModelForOmittedNullValuesTestCases(out entityType, out entitySet);

            // null-able property Height is not selected, thus should not be restored.
            // null-able property Address is selected, thus should be restored.
            // Property Education is null-able.
            const string payloadWithSelectedPropertiesPartialSubTreeInQueryOption = @"{
                    ""@odata.context"":""http://www.example.com/$metadata#EntityNs.MyContainer.People(Id,%20Education/Id,%20Education/SchoolName,%20Education/UnknownPropX,%20Address)/$entity"",
                    ""@odata.id"":""http://mytest"",
                    ""Id"":0,
                    ""Education"":{""Id"":1, ""UnknownPropX"": ""pX""}
                }";

            List<ODataResource> entries = new List<ODataResource>();
            List<ODataNestedResourceInfo> nestedResourceInfos = new List<ODataNestedResourceInfo>();
            this.ReadEntryPayload(model, payloadWithSelectedPropertiesPartialSubTreeInQueryOption, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            nestedResourceInfos.Add(reader.Item as ODataNestedResourceInfo);
                            break;
                        default:
                            break;
                    }
                },
                nullValuesOmitted: bNullValuesOmitted);

            // Education
            ODataResource edu =
                entries.FirstOrDefault(s => string.Equals(s.TypeName, "NS.Edu", StringComparison.Ordinal));
            edu.Should().NotBeNull();

            // Verify that unknown property on non-open type doesn't cause anomaly.
            edu.Properties.Single(s => string.Equals(s.Name, "UnknownPropX", StringComparison.Ordinal)).Value
                .Should().Equals("pX");
        }

        [Fact]
        public void ReadingNormalDynamicCollectionPropertyInOpenStructuralTypeShouldWork()
        {
            EdmEntityType entityType;
            EdmEntitySet entitySet;
            EdmModel model = GetDynamicModel(out entityType, out entitySet);

            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People(Id,%20MyDynamic,%20Address/MyDynamic1)/$entity\"," +
                    "\"@odata.id\":\"http://mytest\"," +
                    "\"Id\":0," +
                    "\"Test@odata.type\":\"#Collection(Edm.String)\"," +
                    "\"Test\":null," +
                    "\"MyDynamic@odata.type\":\"#Collection(Edm.String)\"," +
                    "\"MyDynamic\":[\"mystr\"]," +
                    "\"Address\":{\"CountryRegion\":\"China\",\"Test1@odata.type\":\"#Collection(Edm.Int32)\",\"Test1\":null, \"MyDynamic1@odata.type\":\"#Collection(Edm.Int32)\",\"MyDynamic1\":[1]}" +
                "}";

            List<ODataResource> entries = new List<ODataResource>();
            List<ODataNestedResourceInfo> nestedResourceInfos = new List<ODataNestedResourceInfo>();

            this.ReadEntryPayload(model, payload, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            nestedResourceInfos.Add(reader.Item as ODataNestedResourceInfo);
                            break;
                        default:
                            break;
                    }
                }
            );

            entries.Count.Should().Be(2);
            nestedResourceInfos.Count.Should().Be(1);

            ODataResource person = entries.FirstOrDefault(s => string.Equals(s.TypeName, "NS.Person", StringComparison.Ordinal));
            person.Should().NotBeNull();
            person.Properties.FirstOrDefault(p => string.Equals(p.Name, "MyDynamic", StringComparison.Ordinal)).Value.Should().NotBeNull();

            ODataResource address = entries.FirstOrDefault(s => string.Equals(s.TypeName, "NS.Address", StringComparison.Ordinal));
            address.Should().NotBeNull();
            address.Properties.FirstOrDefault(p => string.Equals(p.Name, "MyDynamic1", StringComparison.Ordinal))
                .Value.Should().NotBeNull();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ReadingNullableDynamicCollectionPropertyInOpenStructuralTypeShouldWork(bool bNullValuesOmitted)
        {
            EdmEntityType entityType;
            EdmEntitySet entitySet;
            EdmModel model = GetDynamicModel(out entityType, out entitySet);

            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People(Id,%20MyDynamic,%20Address/MyDynamic1)/$entity\"," +
                "\"@odata.id\":\"http://mytest\"," +
                "\"Id\":0," +
                "\"Test@odata.type\":\"#Collection(Edm.String)\"," +
                "\"Test\":null," +
                "\"Address\":{\"CountryRegion\":\"China\",\"Test1@odata.type\":\"#Collection(Edm.Int32)\",\"Test1\":null}" +
                "}";

            List<ODataResource> entries = new List<ODataResource>();
            List<ODataNestedResourceInfo> nestedResourceInfos = new List<ODataNestedResourceInfo>();

            this.ReadEntryPayload(model, payload, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            nestedResourceInfos.Add(reader.Item as ODataNestedResourceInfo);
                            break;
                        default:
                            break;
                    }
                },
                nullValuesOmitted: bNullValuesOmitted);

            entries.Count.Should().Be(2);
            nestedResourceInfos.Count.Should().Be(1);

            ODataResource person = entries.FirstOrDefault(s => string.Equals(s.TypeName, "NS.Person", StringComparison.Ordinal));
            person.Should().NotBeNull();
            if (bNullValuesOmitted)
            {
                person.Properties.Any(p => string.Equals(p.Name, "MyDynamic", StringComparison.Ordinal)).Should().BeTrue();
            }
            else
            {
                person.Properties.Any(p => string.Equals(p.Name, "MyDynamic", StringComparison.Ordinal)).Should().BeFalse();
            }

            ODataResource address =entries.FirstOrDefault(s => string.Equals(s.TypeName, "NS.Address", StringComparison.Ordinal));
            address.Should().NotBeNull();
            if (bNullValuesOmitted)
            {
                address.Properties.Any(p => string.Equals(p.Name, "MyDynamic1", StringComparison.Ordinal)).Should().BeTrue();
            }
            else
            {
                address.Properties.Any(p => string.Equals(p.Name, "MyDynamic1", StringComparison.Ordinal)).Should().BeFalse();
            }
        }

        private EdmModel GetDynamicModel(out EdmEntityType entityType, out EdmEntitySet entitySet)
        {
            EdmModel model = new EdmModel();

            EdmComplexType complexType = new EdmComplexType("NS", "Address", null, false, true/*isOpen*/);
            complexType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String);
            model.AddElement(complexType);

            entityType = new EdmEntityType("NS", "Person", null, false, true/*isOpen*/);
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            entityType.AddStructuralProperty("Address", new EdmComplexTypeReference(complexType, false));
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            return model;
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ReadingPropertyInTopLevelOrInComplexTypeShouldRestoreOmittedNullValuesWithSelectedPropertiesPartialSubTreeAndBaseTypeProperty(
            bool bNullValuesOmitted)
        {
            EdmEntityType entityType;
            EdmEntitySet entitySet;
            EdmModel model = BuildEdmModelForOmittedNullValuesTestCases(out entityType, out entitySet);

            // null-able property Address is selected, thus should be restored.
            // Property Education is null-able.
            const string payloadWithWildcardInQueryOption = @"{
                ""@odata.context"":""http://www.example.com/$metadata#EntityNs.MyContainer.People(Id,%20Education/Id,%20Education/OrgId,%20Address)/$entity"",
                ""@odata.id"":""http://mytest"",
                ""Id"":0,
                ""Education"":{""Id"":1}
            }";

            List<ODataResource> entries = new List<ODataResource>();
            List<ODataNestedResourceInfo> nestedResourceInfos = new List<ODataNestedResourceInfo>();
            this.ReadEntryPayload(model, payloadWithWildcardInQueryOption, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            nestedResourceInfos.Add(reader.Item as ODataNestedResourceInfo);
                            break;
                        default:
                            break;
                    }
                },
                nullValuesOmitted: bNullValuesOmitted);

            entries.Count.Should().Be(2);
            nestedResourceInfos.Count.Should().Be(1);
            nestedResourceInfos.FirstOrDefault(s => string.Equals(s.Name, "Education", StringComparison.Ordinal))
                .Should().NotBeNull();

            // Education
            ODataResource edu =
                entries.FirstOrDefault(s => string.Equals(s.TypeName, "NS.Edu", StringComparison.Ordinal));
            edu.Should().NotBeNull();
            edu.Properties.FirstOrDefault(s => string.Equals(s.Name, "Id", StringComparison.Ordinal))
                .Value.Should().Be(1);

            // selected null-able property from base type should be restored to null if omitted.
            if (bNullValuesOmitted)
            {
                edu.Properties.FirstOrDefault(s => string.Equals(s.Name, "OrgId", StringComparison.Ordinal))
                    .Value.Should().BeNull();
            }
            else
            {
                edu.Properties.Any(s => string.Equals(s.Name, "OrgId", StringComparison.Ordinal)).Should().BeFalse();
            }

            // not selected property should not be restored.
            edu.Properties.Any(s => string.Equals(s.Name, "OrgCategory", StringComparison.Ordinal)).Should().BeFalse();
            edu.Properties.Any(s => string.Equals(s.Name, "SchoolName", StringComparison.Ordinal)).Should().BeFalse();
            edu.Properties.Any(s => string.Equals(s.Name, "Campuses", StringComparison.Ordinal)).Should().BeFalse();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ReadingWithSelectExpandClauseShouldRestoreOmittedNullValuesWithSelectedPropertiesPartialSubTree(
            bool bNullValuesOmitted)
        {
            EdmEntityType entityType;
            EdmEntitySet entitySet;
            EdmModel model = BuildEdmModelForOmittedNullValuesTestCases(out entityType, out entitySet);

            // null-able property Height is not selected, thus should not be restored.
            // Property Education is null-able.
            const string payloadWithSelectExpandClauseInQueryOption = @"{
                    ""@odata.context"":""http://www.example.com/$metadata#EntityNs.MyContainer.People(Id,Education/Id,Education/SchoolName,Company/Id,Company/Name)/$entity"",
                    ""@odata.id"":""http://www.example.com/$metadata#EntityNs.MyContainer.People(0)"",
                    ""Id"":0,
                    ""Education"":{""Id"":1},
                    ""Company"":{
                        ""@odata.id"":""http://www.example.com/$metadata#EntityNs.MyContainer.Companies(11)"",
                        ""Id"":11}
                }";

            List<ODataResource> entries = new List<ODataResource>();
            List<ODataNestedResourceInfo> nestedResourceInfos = new List<ODataNestedResourceInfo>();
            this.ReadEntryPayload(model, payloadWithSelectExpandClauseInQueryOption, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            nestedResourceInfos.Add(reader.Item as ODataNestedResourceInfo);
                            break;
                        default:
                            break;
                    }
                },
                nullValuesOmitted: bNullValuesOmitted);

            nestedResourceInfos.Count.Should().Be(2);
            ODataNestedResourceInfo eduNestedResouce =
                nestedResourceInfos.FirstOrDefault(s => string.Equals(s.Name, "Education", StringComparison.Ordinal));

            // Edu is complex type, its Url should be null.
            eduNestedResouce.Should().NotBeNull();
            eduNestedResouce.Url.Should().BeNull();
            // Navigation link for Company entity should be non-null.
            ODataNestedResourceInfo companyLink =
                nestedResourceInfos.FirstOrDefault(s => string.Equals(s.Name, "Company", StringComparison.Ordinal));
            companyLink.Should().NotBeNull();
            companyLink.Url.Should().NotBeNull();

            entries.Count.Should().Be(3);
            // Education
            ODataResource edu =
                entries.FirstOrDefault(s => string.Equals(s.TypeName, "NS.Edu", StringComparison.Ordinal));
            edu.Should().NotBeNull();

            edu.Properties.FirstOrDefault(s => string.Equals(s.Name, "Id", StringComparison.Ordinal))
                .Value.Should().Be(1);
            if (bNullValuesOmitted)
            {
                // null-able property value should be restored.
                edu.Properties.FirstOrDefault(s => string.Equals(s.Name, "SchoolName", StringComparison.Ordinal))
                    .Value.Should().BeNull();
            }
            else
            {
                edu.Properties.Any(s => string.Equals(s.Name, "SchoolName", StringComparison.Ordinal)).Should().BeFalse();
            }
            // not selected property should not be restored.
            edu.Properties.Any(s => string.Equals(s.Name, "Campuses", StringComparison.Ordinal)).Should().BeFalse();

            // Company
            ODataResource company =
                entries.FirstOrDefault(s => string.Equals(s.TypeName, "NS.Cmpny", StringComparison.Ordinal));
            company.Should().NotBeNull();
            company.Properties.FirstOrDefault(s => string.Equals(s.Name, "Id", StringComparison.Ordinal))
                .Value.Should().Be(11);
            if (bNullValuesOmitted)
            {
                company.Properties.FirstOrDefault(s => string.Equals(s.Name, "Name", StringComparison.Ordinal))
                    .Value.Should().BeNull();
            }
            else
            {
                company.Properties.Any(s => string.Equals(s.Name, "Name", StringComparison.Ordinal)).Should().BeFalse();
            }

            // Person
            ODataResource person =
                entries.FirstOrDefault(s => string.Equals(s.TypeName, "NS.Person", StringComparison.Ordinal));
            person.Should().NotBeNull();
            person.Properties.FirstOrDefault(s => string.Equals(s.Name, "Id", StringComparison.Ordinal))
                .Value.Should().Be(0);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ReadingWithSelectExpandClauseShouldHaveNoContentForNullNavigationProperty(
            bool bNullValuesOmitted)
        {
            EdmEntityType entityType;
            EdmEntitySet entitySet;
            EdmModel model = BuildEdmModelForOmittedNullValuesTestCases(out entityType, out entitySet);

            // Context URL with subtree of navigation entity selected.
            const string payloadWithSelectExpandClauseInQueryOption1 = @"{
                    ""@odata.context"":""http://www.example.com/$metadata#EntityNs.MyContainer.People(Id,Education/Id,Education/SchoolName,Company/Id,Company/Name)/$entity"",
                    ""@odata.id"":""http://www.example.com/$metadata#EntityNs.MyContainer.People(0)"",
                    ""Id"":0,
                    ""Education"":{""Id"":1}
                }";

            // Context URL with entire navigation entity selected.
            const string payloadWithSelectExpandClauseInQueryOption2 = @"{
                    ""@odata.context"":""http://www.example.com/$metadata#EntityNs.MyContainer.People(Id,Education/Id,Education/SchoolName,Company)/$entity"",
                    ""@odata.id"":""http://www.example.com/$metadata#EntityNs.MyContainer.People(0)"",
                    ""Id"":0,
                    ""Education"":{""Id"":1}
                }";

            foreach (string payload in new string[]
            {
                payloadWithSelectExpandClauseInQueryOption1,
                payloadWithSelectExpandClauseInQueryOption2
            })
            {
                List<ODataResource> entries = new List<ODataResource>();
                List<ODataNestedResourceInfo> nestedResourceInfos = new List<ODataNestedResourceInfo>();
                this.ReadEntryPayload(model, payload, entitySet, entityType,
                    reader =>
                    {
                        switch (reader.State)
                        {
                            case ODataReaderState.ResourceStart:
                                entries.Add(reader.Item as ODataResource);
                                break;
                            case ODataReaderState.NestedResourceInfoStart:
                                nestedResourceInfos.Add(reader.Item as ODataNestedResourceInfo);
                                break;
                            default:
                                break;
                        }
                    },
                    nullValuesOmitted: bNullValuesOmitted);

                entries.Count.Should().Be(2);
                // Expanded Company entity via null navigation link is not restored.
                // see example: http://services.odata.org/V4/TripPinService/People('ronaldmundy')?$expand=Photo
                entries.Any(s => string.Equals(s.TypeName, "NS.Cmpny", StringComparison.Ordinal)).Should().BeFalse();

                nestedResourceInfos.Count.Should().Be(2);
                // There should be a navigation link reported as missing from payload.
                ODataNestedResourceInfo companyLink =
                    nestedResourceInfos.FirstOrDefault(s => string.Equals(s.Name, "Company", StringComparison.Ordinal));
                companyLink.Should().NotBeNull();
                // Reported missing/un-processed navigation link has URL constructed from metadata and
                // @odata.id annotation of navigation source.
                companyLink.Url.Should().NotBeNull();
            }
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
            birthday.HasValue.Should().BeTrue();
            birthday.Value.Should().Be(new DateTimeOffset(2012, 4, 12, 18, 43, 10, TimeSpan.Zero));
        }

        private void ReadEntryPayload(IEdmModel userModel, string payload, EdmEntitySet entitySet, IEdmEntityType entityType,
            Action<ODataReader> action,
            bool isIeee754Compatible = true,
            IServiceProvider container = null,
            bool nullValuesOmitted = false)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)), Container = container};
            string contentType = isIeee754Compatible
                ? "application/json;odata.metadata=minimal;IEEE754Compatible=true"
                : "application/json;odata.metadata=minimal;IEEE754Compatible=false";
            message.SetHeader("Content-Type", contentType);

            if (nullValuesOmitted)
            {
                message.SetHeader("Preference-Applied", omitValuesNulls);
            }

            var readerSettings = new ODataMessageReaderSettings { EnableMessageStreamDisposal = true };
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

        private EdmModel BuildEdmModelForOmittedNullValuesTestCases(out EdmEntityType entityType, out EdmEntitySet entitySet)
        {
            EdmModel model = new EdmModel();

            entityType = new EdmEntityType("NS", "Person");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

            EdmTypeDefinition weightType = new EdmTypeDefinition("NS", "Wgt", EdmPrimitiveTypeKind.Double);
            EdmTypeDefinitionReference weightTypeRef = new EdmTypeDefinitionReference(weightType, false);
            entityType.AddStructuralProperty("Weight", weightTypeRef);

            EdmTypeDefinition heightType = new EdmTypeDefinition("NS", "Ht", EdmPrimitiveTypeKind.Double);
            EdmTypeDefinitionReference heightTypeRef = new EdmTypeDefinitionReference(heightType, true);
            entityType.AddStructuralProperty("Height", heightTypeRef);

            EdmComplexType addressType = new EdmComplexType("NS", "OpnAddr");
            EdmComplexTypeReference addressTypeRef = new EdmComplexTypeReference(addressType, true);

            EdmTypeDefinition countryRegionType = new EdmTypeDefinition("NS", "CR", EdmPrimitiveTypeKind.String);
            EdmTypeDefinitionReference countryRegionTypeRef = new EdmTypeDefinitionReference(countryRegionType, false);
            addressType.AddStructuralProperty("CountryRegion", countryRegionTypeRef);

            // Address/City
            EdmTypeDefinition cityType = new EdmTypeDefinition("NS", "Cty", EdmPrimitiveTypeKind.String);
            EdmTypeDefinitionReference cityTypeRef = new EdmTypeDefinitionReference(cityType, false);
            addressType.AddStructuralProperty("City", cityTypeRef);

            EdmTypeDefinition zipCodeType = new EdmTypeDefinition("NS", "ZC", EdmPrimitiveTypeKind.String);
            EdmTypeDefinitionReference zipCodeTypeRef = new EdmTypeDefinitionReference(zipCodeType, true);
            addressType.AddStructuralProperty("ZipCode", zipCodeTypeRef);

            entityType.AddStructuralProperty("Address", addressTypeRef);

            // Education
            EdmComplexType organizationType = new EdmComplexType("NS", "Org");
            organizationType.AddStructuralProperty("OrgId", EdmPrimitiveTypeKind.Int32);
            organizationType.AddStructuralProperty("OrgCategory", EdmPrimitiveTypeKind.String);

            EdmComplexType educationType = new EdmComplexType("NS", "Edu", organizationType);
            EdmComplexTypeReference educationTypeRef = new EdmComplexTypeReference(educationType, true);

            // top level null-able complex type properties
            educationType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32); // same property name but inside different type.
            educationType.AddStructuralProperty("Campuses",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));
            educationType.AddStructuralProperty("SchoolName", EdmPrimitiveTypeKind.String);

            entityType.AddStructuralProperty("Education", educationTypeRef);

            // Company entity type.
            EdmEntityType companyEntityType = new EdmEntityType("NS", "Cmpny");
            entityType.AddKeys(companyEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            companyEntityType.AddStructuralProperty("Name",
                new EdmTypeDefinitionReference(
                    new EdmTypeDefinition("NS", "CmpyName", EdmPrimitiveTypeKind.String),
                    true));
            companyEntityType.AddStructuralProperty("Address", addressTypeRef);

            EdmNavigationPropertyInfo companyNav = new EdmNavigationPropertyInfo()
            {
                Name = "Company",
                ContainsTarget = true,
                Target = companyEntityType,
                TargetMultiplicity = EdmMultiplicity.ZeroOrOne
            };

            // Add navigation link from Person to Company.
            entityType.AddUnidirectionalNavigation(companyNav);

            model.AddElement(weightType);
            model.AddElement(heightType);
            model.AddElement(addressType);
            model.AddElement(entityType);
            model.AddElement(companyEntityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            entitySet = container.AddEntitySet("People", entityType);
            container.AddEntitySet("Companies", companyEntityType);
            model.AddElement(container);

            return model;
        }
    }
}
