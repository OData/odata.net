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
using FluentAssertions;
using Microsoft.OData.Core.Tests.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.IntegrationTests.Reader.JsonLight
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
            ODataEntry entry = null;
            this.ReadEntryPayload(mainModel, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            Assert.NotNull(entry);
            entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "LongId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(12L, "value should be in correct type.");
            entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "FloatId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(34.98f, "value should be in correct type.");
            entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "DoubleId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(56.01d, "value should be in correct type.");
            entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "DecimalId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(78.62m, "value should be in correct type.");

            var complextProperty = entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "ComplexProperty", StringComparison.OrdinalIgnoreCase)).Value as ODataComplexValue;
            complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CLongId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1L, "value should be in correct type.");
            complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CFloatId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1.0F, "value should be in correct type.");
            complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CDoubleId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(-1.0D, "value should be in correct type.");
            complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CDecimalId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(0.0M, "value should be in correct type.");

            var longCollection = entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "LongNumbers", StringComparison.OrdinalIgnoreCase)).Value.As<ODataCollectionValue>();
            longCollection.Items.Should().Contain(-1L).And.Contain(long.MinValue).And.Contain(long.MaxValue);
            var floatCollection = entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "FloatNumbers", StringComparison.OrdinalIgnoreCase)).Value.As<ODataCollectionValue>();
            floatCollection.Items.Should().Contain(-1F).And.Contain(float.MinValue).And.Contain(float.MaxValue).And.Contain(float.PositiveInfinity).And.Contain(float.NegativeInfinity).And.Contain(float.NaN);
            var doubleCollection = entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "DoubleNumbers", StringComparison.OrdinalIgnoreCase)).Value.As<ODataCollectionValue>();
            doubleCollection.Items.Should().Contain(1.0D).And.Contain(double.MinValue).And.Contain(double.MaxValue).And.Contain(double.PositiveInfinity).And.Contain(double.NegativeInfinity).And.Contain(double.NaN);
            var decimalCollection = entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "DecimalNumbers", StringComparison.OrdinalIgnoreCase)).Value.As<ODataCollectionValue>();
            decimalCollection.Items.Should().Contain(0M).And.Contain(decimal.MinValue).And.Contain(decimal.MaxValue);
        }

        [Fact]
        public void ReadingComplexInheritWithModel()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = new EdmEntityType("NS", "MyTestEntity");
            EdmStructuralProperty key = entityType.AddStructuralProperty("LongId", EdmPrimitiveTypeKind.Int64, false);
            entityType.AddKeys(key);

            EdmComplexType complexType = new EdmComplexType("NS", "MyComplexType");
            complexType.AddStructuralProperty("CLongId", EdmPrimitiveTypeKind.Int64, false);

            EdmComplexType derivedComplexType = new EdmComplexType("NS", "MyDerivedComplexType", complexType, false);
            derivedComplexType.AddStructuralProperty("CFloatId", EdmPrimitiveTypeKind.Single, false);

            EdmComplexType derivedDerivedComplexType = new EdmComplexType("NS", "MyDerivedDerivedComplexType", derivedComplexType, false);
            derivedDerivedComplexType.AddStructuralProperty("DerivedCFloatId", EdmPrimitiveTypeKind.Single, false);

            model.AddElement(complexType);
            model.AddElement(derivedComplexType);
            model.AddElement(derivedDerivedComplexType);

            EdmComplexTypeReference complexTypeRef = new EdmComplexTypeReference(complexType, true);
            entityType.AddStructuralProperty("ComplexProperty", complexTypeRef);
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer_sub");
            EdmEntitySet entitySet = container.AddEntitySet("MyTestEntitySet", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.MyTestEntitySet/$entity\"," +
                "\"@odata.id\":\"http://MyTestEntity\"," +
                "\"LongId\":\"12\"," +
                "\"ComplexProperty\":{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"CLongId\":\"1\",\"CFloatId\":1}" +
                "}";

            IEdmModel mainModel = TestUtils.WrapReferencedModelsToMainModel("EntityNs", "MyContainer", model);
            ODataEntry entry = null;
            this.ReadEntryPayload(mainModel, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            Assert.NotNull(entry);
            entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "LongId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(12L, "value should be in correct type.");

            var complextProperty = entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "ComplexProperty", StringComparison.OrdinalIgnoreCase)).Value as ODataComplexValue;
            complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CLongId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1L, "value should be in correct type.");
            complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CFloatId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1.0F, "value should be in correct type.");

            const string payload2 =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.MyTestEntitySet/$entity\"," +
                "\"@odata.id\":\"http://MyTestEntity\"," +
                "\"LongId\":\"12\"," +
                "\"ComplexProperty\":{\"@odata.type\":\"#NS.MyDerivedDerivedComplexType\",\"CLongId\":\"1\",\"CFloatId\":1,\"DerivedCFloatId\":1}" +
                "}";

            entry = null;
            this.ReadEntryPayload(mainModel, payload2, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            Assert.NotNull(entry);
            entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "LongId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(12L, "value should be in correct type.");

            complextProperty = entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "ComplexProperty", StringComparison.OrdinalIgnoreCase)).Value as ODataComplexValue;
            complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CLongId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1L, "value should be in correct type.");
            complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "DerivedCFloatId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1.0F, "value should be in correct type.");
        }

        [Fact]
        public void ReadingComplexInheritInCollection()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = new EdmEntityType("NS", "MyTestEntity");
            EdmStructuralProperty key = entityType.AddStructuralProperty("LongId", EdmPrimitiveTypeKind.Int64, false);
            entityType.AddKeys(key);

            EdmComplexType complexType = new EdmComplexType("NS", "MyComplexType");
            complexType.AddStructuralProperty("CLongId", EdmPrimitiveTypeKind.Int64, false);

            EdmComplexType derivedComplexType = new EdmComplexType("NS", "MyDerivedComplexType", complexType, false);
            derivedComplexType.AddStructuralProperty("CFloatId", EdmPrimitiveTypeKind.Single, false);

            model.AddElement(complexType);
            model.AddElement(derivedComplexType);

            EdmComplexTypeReference complexTypeRef = new EdmComplexTypeReference(complexType, true);
            entityType.AddStructuralProperty("ComplexCollectionProperty", new EdmCollectionTypeReference(new EdmCollectionType(complexTypeRef)));
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer_sub");
            EdmEntitySet entitySet = container.AddEntitySet("MyTestEntitySet", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.MyTestEntitySet/$entity\"," +
                "\"@odata.id\":\"http://MyTestEntity\"," +
                "\"LongId\":\"12\"," +
                "\"ComplexCollectionProperty\":[{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"CLongId\":\"1\",\"CFloatId\":1},{\"CLongId\":\"1\"},{\"CLongId\":\"1\",\"CFloatId\":1,\"@odata.type\":\"#NS.MyDerivedComplexType\"}]" +
                "}";

            IEdmModel mainModel = TestUtils.WrapReferencedModelsToMainModel("EntityNs", "MyContainer", model);
            ODataEntry entry = null;
            this.ReadEntryPayload(mainModel, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            Assert.NotNull(entry);

            var intCollection = entry.Properties.FirstOrDefault(
                s => string.Equals(
                    s.Name,
                    "ComplexCollectionProperty",
                    StringComparison.OrdinalIgnoreCase)).Value.As<ODataCollectionValue>();
            var list = new List<int?>();

            Boolean derived = true;
            foreach (var val in intCollection.Items)
            {
                var complextProperty = val as ODataComplexValue;
                complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CLongId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1L, "value should be in correct type.");
                if (derived) complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CFloatId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1.0F, "value should be in correct type.");
                derived = false;
            }

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
            ODataEntry entry = null;
            this.ReadEntryPayload(mainModel, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
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
            ODataEntry entry = null;
            this.ReadEntryPayload(mainModel, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
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
            Assert.Equal(1, (int)list[1]);
            Assert.Equal(2, (int)list[2]);

            var complexCollection = entry.Properties.FirstOrDefault(
                s => string.Equals(
                    s.Name,
                    "OpenComplexTypeCollection",
                    StringComparison.OrdinalIgnoreCase)).Value.As<ODataCollectionValue>();

            foreach (var val in complexCollection.Items)
            {
                ((ODataComplexValue)val).Properties.FirstOrDefault(s => string.Equals(s.Name, "CLongId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1L, "value should be in correct type.");
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
            ODataEntry entry = null;
            Action test = () => this.ReadEntryPayload(mainModel, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; }, false);
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
            ODataEntry entry = null;
            Action test = () => this.ReadEntryPayload(mainModel, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            test.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter("Edm.Decimal"));
        }

        [Fact]
        public void ReadingPayloadOpenComplexTypeJsonLight()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);

            EdmComplexType complexType = new EdmComplexType("NS", "OpenAddress", null, false, true);
            complexType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String, false);
            EdmComplexTypeReference complexTypeRef = new EdmComplexTypeReference(complexType, true);

            entityType.AddStructuralProperty("Address", complexTypeRef);

            model.AddElement(complexType);
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer_sub");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                "\"@odata.id\":\"http://mytest\"," +
                "\"Id\":\"0\"," +
                "\"Address\":{\"CountryRegion\":\"China\",\"City\":\"Shanghai\"}" +
                "}";

            IEdmModel mainModel = TestUtils.WrapReferencedModelsToMainModel("EntityNs", "MyContainer", model);
            ODataEntry entry = null;

            this.ReadEntryPayload(mainModel, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            Assert.NotNull(entry);

            var address = entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "Address", StringComparison.OrdinalIgnoreCase)).Value as ODataComplexValue;
            address.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo("China", "value should be in correct type.");
            address.Properties.FirstOrDefault(s => string.Equals(s.Name, "City", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo("Shanghai", "value should be in correct type.");
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

            ODataEntry entry = null;

            this.ReadEntryPayload(model, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            Assert.NotNull(entry);

            IList<ODataProperty> propertyList = entry.Properties.ToList();
            propertyList[1].Name.Should().Be("Weight");
            propertyList[1].Value.Should().Be(60.5);

            var address = propertyList[2].Value as ODataComplexValue;
            address.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value.Should().Be("China");
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

            ODataEntry entry = null;

            this.ReadEntryPayload(model, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            Assert.NotNull(entry);

            IList<ODataProperty> propertyList = entry.Properties.ToList();
            propertyList[1].Name.Should().Be("Weight");
            propertyList[1].Value.Should().Be(60);

            propertyList[2].Name.Should().Be("Height");
            propertyList[2].Value.Should().Be(180);

            var address = propertyList[3].Value as ODataComplexValue;
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

            ODataEntry entry = null;

            this.ReadEntryPayload(model, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            Assert.NotNull(entry);

            IList<ODataProperty> propertyList = entry.Properties.ToList();
            propertyList[1].Name.Should().Be("Weight");
            propertyList[1].Value.Should().Be(60.5);

            var address = propertyList[2].Value as ODataComplexValue;
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

            ODataEntry entry = null;

            this.ReadEntryPayload(model, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            Assert.NotNull(entry);

            IList<ODataProperty> propertyList = entry.Properties.ToList();
            propertyList[1].Name.Should().Be("Weight");
            propertyList[1].Value.Should().Be(60.5);

            var address = propertyList[2].Value as ODataComplexValue;
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

            ODataEntry entry = null;

            this.ReadEntryPayload(model, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
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

            model.SetPayloadValueConverter(new DateTimeOffsetCustomFormatPrimitivePayloadValueConverter());

            ODataEntry entry = null;
            this.ReadEntryPayload(model, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            Assert.NotNull(entry);

            IList<ODataProperty> propertyList = entry.Properties.ToList();
            var birthday = propertyList[1].Value as DateTimeOffset?;
            birthday.HasValue.Should().BeTrue();
            birthday.Value.Should().Be(new DateTimeOffset(2012, 4, 12, 18, 43, 10, TimeSpan.Zero));
        }

        private void ReadEntryPayload(IEdmModel userModel, string payload, EdmEntitySet entitySet, IEdmEntityType entityType, Action<ODataReader> action, bool isIeee754Compatible = true)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            string contentType = isIeee754Compatible
                ? "application/json;odata.metadata=minimal;IEEE754Compatible=true"
                : "application/json;odata.metadata=minimal;IEEE754Compatible=false";
            message.SetHeader("Content-Type", contentType);
            var readerSettings = new ODataMessageReaderSettings { DisableMessageStreamDisposal = false };
            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, readerSettings, userModel))
            {
                var reader = msgReader.CreateODataEntryReader(entitySet, entityType);
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }
    }
}
