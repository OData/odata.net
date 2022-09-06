//---------------------------------------------------------------------
// <copyright file="ConstructibleModelTestsUsingConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConstructibleModelTestsUsingConverter : EdmLibTestCaseBase
    {
        public ConstructibleModelTestsUsingConverter()
        {
            this.EdmVersion = EdmVersion.V40;
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterAssociationFkWithNavigationTest()
        {
            BasicConstructibleModelTestsUsingConverter(ModelBuilder.AssociationFkWithNavigationEdm());
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterAssociationIndependentTest()
        {
            BasicConstructibleModelTestsUsingConverter(ModelBuilder.AssociationIndependentEdm());
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterAssociationOnDeleteModelTest()
        {
            BasicConstructibleModelTestsUsingConverter(ModelBuilder.AssociationOnDeleteModelEdm());
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterAssociationFkTest()
        {
            BasicConstructibleModelTestsUsingConverter(ModelBuilder.AssociationFkEdm());
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterAssociationCompositeFkTest()
        {
            BasicConstructibleModelTestsUsingConverter(ModelBuilder.AssociationCompositeFkEdm());
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterEntityContainerWithEntitySetsTest()
        {
            BasicConstructibleModelTestsUsingConverter(ModelBuilder.EntityContainerWithEntitySetsEdm());
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterEntityContainerWithFunctionImportsTest()
        {
            BasicConstructibleModelTestsUsingConverter(ModelBuilder.EntityContainerWithOperationImportsEdm());
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterEntityInheritanceTreeTest()
        {
            BasicConstructibleModelTestsUsingConverter(ModelBuilder.EntityInheritanceTreeEdm());
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterFunctionWithAllTest()
        {
            BasicConstructibleModelTestsUsingConverter(ModelBuilder.FunctionWithAllEdm());
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterModelWithAllConceptsTest()
        {
            BasicConstructibleModelTestsUsingConverter(ModelBuilder.ModelWithAllConceptsEdm());
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterMultipleNamespacesTest()
        {
            BasicConstructibleModelTestsUsingConverter(ModelBuilder.MultipleNamespacesEdm());
        }

        [TestMethod]
        // [EdmLib] SchemaReader.TryParse uses wrong default values for the string type's facets such as unicode - Fixed
        // [EdmLib] Precision facet should not be required for the temporal and decimal types - Fixed
        public void ConstructibleModelUsingConverterOneComplexWithAllPrimitivePropertyTest()
        {
            BasicConstructibleModelTestsUsingConverter(ModelBuilder.OneComplexWithAllPrimitivePropertyEdm());
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterOneComplexWithNestedComplexTest()
        {
            BasicConstructibleModelTestsUsingConverter(ModelBuilder.OneComplexWithNestedComplexEdm());
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterOneComplexWithOnePropertyTest()
        {
            BasicConstructibleModelTestsUsingConverter(ModelBuilder.OneComplexWithOnePropertyEdm());
        }

        [TestMethod]
        // [EdmLib] SchemaReader.TryParse uses wrong default values for the string type's facets such as unicode - Fixed
        // [EdmLib] Precision facet should not be required for the temporal and decimal types - Fixed
        public void ConstructibleModelUsingConverterOneEntityWithAllPrimitivePropertyTest()
        {
            BasicConstructibleModelTestsUsingConverter(ModelBuilder.OneEntityWithAllPrimitivePropertyEdm());
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterOneEntityWithOnePropertyTest()
        {
            BasicConstructibleModelTestsUsingConverter(ModelBuilder.OneEntityWithOnePropertyEdm());
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterSimpleAllPrimtiveTypesTest()
        {
            BasicConstructibleModelTestsUsingConverter(ModelBuilder.SimpleAllPrimitiveTypes(EdmVersion.V40, true, false));
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterStringMaxLengthTest()
        {
            var edmModel = this.GetParserResult(ModelBuilder.StringWithFacets());
            var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(edmModel);

            var entityType = (EdmEntityType)stockModel.FindType("ModelBuilder.Content");
            var NullableStringUnboundedUnicodeFixedLength = entityType.FindProperty("NullableStringUnboundedUnicode").Type.AsString();
            Assert.IsTrue(NullableStringUnboundedUnicodeFixedLength.IsUnbounded, "The parser failed to process the value, Max, for the maxLength facet");
            Assert.IsNull(NullableStringUnboundedUnicodeFixedLength.MaxLength, "The parser failed to process the value, Max, for the maxLength facet");

            var SimpleString = entityType.FindProperty("SimpleString").Type.AsString();
            Assert.IsFalse(SimpleString.IsUnbounded, "The parser failed to process the value, Max, for the maxLength facet");
            Assert.IsNull(SimpleString.MaxLength, "The parser failed to process the value, Max, for the maxLength facet");

            var NullableStringMaxLength10 = entityType.FindProperty("NullableStringMaxLength10").Type.AsString();
            Assert.IsFalse(NullableStringMaxLength10.IsUnbounded, "The parser failed to process the value, Max, for the maxLength facet");
            Assert.IsNotNull(NullableStringMaxLength10.MaxLength, "The parser failed to process the value, Max, for the maxLength facet");
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterTaupoDefaultModelTest()
        {
            BasicConstructibleModelTestsUsingConverter(ModelBuilder.TaupoDefaultModelEdm());
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterDependentPropertiesWithNoPartnerPropertyTest()
        {
            var testCsdls = ModelBuilder.DependentPropertiesWithNoPartnerProperty();
            var stockModel = this.GenerateStockModel(testCsdls);
            BasicConstructibleModelTestSerializingStockModel(testCsdls, stockModel);
        }

        private void BasicConstructibleModelTestsUsingConverter(IEdmModel model)
        {
            var expectedCsdls = this.GetSerializerResult(model);
            BasicConstructibleModelTestsUsingConverter(expectedCsdls.Select(XElement.Parse));
        }

        private void BasicConstructibleModelTestsUsingConverter(IEnumerable<XElement> testCsdl)
        {
            var tempCsdls = new List<XElement>();
            tempCsdls.AddRange(testCsdl.Select(n => new XElement(n)));

            var stockModel = this.GenerateStockModel(testCsdl);
            BasicConstructibleModelTestSerializingStockModel(tempCsdls, stockModel);
        }

        private IEnumerable<XElement> RemoveImmediateAnnotation(IEnumerable<XElement> csdls)
        {
            var newCsdls = csdls.ToList();
            foreach (var csdl in newCsdls)
            {
                RemoveImmediateAnnotation(csdl);
            }
            return newCsdls;
        }

        private void RemoveImmediateAnnotation(XElement csdl)
        {
            var immediateAnnotations = csdl.Attributes().Where(n => !string.IsNullOrEmpty(n.Name.NamespaceName));
            while(immediateAnnotations.Count() > 0)
            {
                csdl.Attributes(immediateAnnotations.ElementAt(0).Name).Remove();
            }

            foreach (var element in csdl.Elements())
            {
                RemoveImmediateAnnotation(element);
            }
        }

        private void BasicConstructibleModelTestSerializingStockModel(IEnumerable<XElement> testCsdl)
        {
            var stockModel = this.GenerateStockModel(testCsdl);
            BasicConstructibleModelTestSerializingStockModel(testCsdl, stockModel);
        }

        private EdmModel GenerateStockModel(IEnumerable<XElement> csdl)
        {
            //  The following part uses constructors and add methods to build the Stock objects
            var edmModel = this.GetParserResult(csdl);
            var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(edmModel);
            return stockModel;
        }

        private void BasicConstructibleModelTestSerializingStockModel(IEnumerable<XElement> csdl, EdmModel stockModel)
        {
            var expectXElements = csdl.ToList();
            var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n)).ToList();
            new ConstructiveApiCsdlXElementComparer().Compare(expectXElements, actualXElements);
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterODataTestModelBasicTest()
        {
            BasicConstructibleModelTestsUsingConverter(ODataTestModelBuilder.ODataTestModelBasicTest);
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterODataTestModelAnnotationTestWithAnnotations()
        {
            BasicConstructibleModelTestSerializingStockModel(ODataTestModelBuilder.ODataTestModelAnnotationTestWithAnnotations);
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterODataTestModelAnnotationTestWithoutAnnotations()
        {
            BasicConstructibleModelTestsUsingConverter(ODataTestModelBuilder.ODataTestModelAnnotationTestWithoutAnnotations);
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterODataTestModelWithFunctionImport()
        {
            BasicConstructibleModelTestsUsingConverter(ODataTestModelBuilder.ODataTestModelWithFunctionImport);
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterODataTestModelDefaultModels()
        {
            BasicConstructibleModelTestsUsingConverter(RemoveImmediateAnnotation(ODataTestModelBuilder.ODataTestModelDefaultModel));
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterODataTestModelEmptyModel()
        {
            BasicConstructibleModelTestsUsingConverter(ODataTestModelBuilder.ODataTestModelEmptyModel);
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterODataTestModelWithSingleEntityType()
        {
            BasicConstructibleModelTestsUsingConverter(ODataTestModelBuilder.ODataTestModelWithSingleEntityType);
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterODataTestModelWithSingleComplexType()
        {
            BasicConstructibleModelTestsUsingConverter(ODataTestModelBuilder.ODataTestModelWithSingleComplexType);
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterODataTestModelWithCollectionProperty()
        {
            BasicConstructibleModelTestsUsingConverter(ODataTestModelBuilder.ODataTestModelWithCollectionProperty);
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterODataTestModelWithOpenType()
        {
            BasicConstructibleModelTestsUsingConverter(ODataTestModelBuilder.ODataTestModelWithOpenType);
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterODataTestModelWithNamedStream()
        {
            BasicConstructibleModelTestsUsingConverter(ODataTestModelBuilder.ODataTestModelWithNamedStream);
        }

        [TestMethod]
        public void ConstructibleModelUsingConverterEntityTypeWithoutKey()
        {
            BasicConstructibleModelTestsUsingConverter(ODataTestModelBuilder.InvalidCsdl.EntityTypeWithoutKey);
        }

        [TestMethod]
        public void ConstructibleModelODataTestModelBasicTest()
        {
            EdmModel model = new EdmModel();

            EdmComplexType address = new EdmComplexType("TestModel", "Address");
            model.AddElement(address);

            EdmStructuralProperty addressStreet = address.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(true));
            EdmStructuralProperty addressZip = new EdmStructuralProperty(address, "Zip", EdmCoreModel.Instance.GetInt32(false));
            address.AddProperty(addressZip);

            EdmEntityType office = new EdmEntityType("TestModel", "OfficeType");
            EdmStructuralProperty officeId = office.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            office.AddKeys(officeId);
            EdmStructuralProperty officeAddress = office.AddStructuralProperty("Address", new EdmComplexTypeReference(address, false));
            model.AddElement(office);

            EdmEntityType city = new EdmEntityType("TestModel", "CityType");
            EdmStructuralProperty cityId = city.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            city.AddKeys(cityId);
            EdmStructuralProperty cityName = city.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            EdmStructuralProperty citySkyline = city.AddStructuralProperty("Skyline", EdmCoreModel.Instance.GetStream(false));
            EdmStructuralProperty cityMetroLanes = city.AddStructuralProperty("MetroLanes", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
            EdmNavigationProperty cityHall = city.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "CityHall", Target = office, TargetMultiplicity = EdmMultiplicity.Many });
            EdmNavigationProperty dol = city.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "DOL", Target = office, TargetMultiplicity = EdmMultiplicity.Many });
            EdmNavigationProperty policeStation = city.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "PoliceStation", Target = office, TargetMultiplicity = EdmMultiplicity.One });
            model.AddElement(city);

            EdmEntityType cityWithMap = new EdmEntityType("TestModel", "CityWithMapType", city);
            model.AddElement(cityWithMap);

            EdmEntityType cityOpenType = new EdmEntityType("TestModel", "CityOpenType", city, false, true);
            model.AddElement(cityOpenType);

            EdmEntityType person = new EdmEntityType("TestModel", "Person");
            EdmStructuralProperty personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            EdmNavigationProperty friend = person.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "Friend", Target = person, TargetMultiplicity = EdmMultiplicity.Many });

            EdmEntityType employee = new EdmEntityType("TestModel", "Employee", person);
            EdmStructuralProperty companyName = employee.AddStructuralProperty("CompanyName", EdmCoreModel.Instance.GetString(true));
            model.AddElement(employee);

            EdmEntityType manager = new EdmEntityType("TestModel", "Manager", employee);
            EdmStructuralProperty level = manager.AddStructuralProperty("Level", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(manager);

            EdmEntityContainer container = new EdmEntityContainer("TestModel", "DefaultContainer");
            EdmEntitySet officeSet = new EdmEntitySet(container, "OfficeType", office);
            container.AddElement(officeSet);
            EdmEntitySet citySet = container.AddEntitySet("CityType", city);
            EdmEntitySet personSet = container.AddEntitySet("Person", person);

            citySet.AddNavigationTarget(cityHall, officeSet);
            citySet.AddNavigationTarget(dol, officeSet);
            citySet.AddNavigationTarget(policeStation, officeSet);
            personSet.AddNavigationTarget(friend, personSet);

            EdmAction serviceOperationAction = new EdmAction("TestModel", "ServiceOperation1", EdmCoreModel.Instance.GetInt32(true));
            EdmOperationParameter a = new EdmOperationParameter(serviceOperationAction, "a", EdmCoreModel.Instance.GetInt32(true));
            serviceOperationAction.AddParameter(a);
            serviceOperationAction.AddParameter("b", EdmCoreModel.Instance.GetString(true));
            model.AddElement(serviceOperationAction);
            
            container.AddActionImport("ServiceOperation1", serviceOperationAction);
            
            model.AddElement(container);

            this.BasicConstructibleModelTestSerializingStockModel(ODataTestModelBuilder.ODataTestModelBasicTest, model);
        }

        [TestMethod]
        public void ConstructibleModelODataTestModelAnnotationTestWithAnnotations()
        {
            EdmModel model = new EdmModel();

            EdmComplexType address = new EdmComplexType("TestModel", "Address");
            EdmStructuralProperty addressStreet = address.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(true));
            EdmStructuralProperty addressZip = address.AddStructuralProperty("Zip", EdmCoreModel.Instance.GetInt32(false));
            model.SetAnnotationValue(addressZip, "http://docs.oasis-open.org/odata/ns/metadata", "MimeType", new EdmStringConstant(EdmCoreModel.Instance.GetString(false), "text/plain"));
            model.AddElement(address);

            EdmEntityType person = new EdmEntityType("TestModel", "PersonType");
            EdmStructuralProperty personName = person.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            model.SetAnnotationValue(personName, "http://docs.oasis-open.org/odata/ns/metadata", "MimeType", new EdmStringConstant(EdmCoreModel.Instance.GetString(false), "text/plain"));
            EdmStructuralProperty personAddress = person.AddStructuralProperty("Address", new EdmComplexTypeReference(address, false));
            EdmStructuralProperty personPicture = person.AddStructuralProperty("Picture", EdmCoreModel.Instance.GetStream(false));
            EdmStructuralProperty personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            EdmEntityContainer container = new EdmEntityContainer("TestModel", "DefaultContainer");
            EdmEntitySet personSet = container.AddEntitySet("PersonType", person);
            EdmEntitySet peopleSet = container.AddEntitySet("People", person);

            EdmAction serviceOperationAction = new EdmAction("TestModel", "ServiceOperation1", EdmCoreModel.Instance.GetInt32(false));
            EdmOperationParameter a = new EdmOperationParameter(serviceOperationAction, "a", EdmCoreModel.Instance.GetInt32(true));
            EdmOperationParameter b = new EdmOperationParameter(serviceOperationAction, "b", EdmCoreModel.Instance.GetString(true));
            serviceOperationAction.AddParameter(a);
            serviceOperationAction.AddParameter(b);
            model.AddElement(serviceOperationAction);
            EdmOperationImport serviceOperation = new EdmActionImport(container, "ServiceOperation1", serviceOperationAction);
            model.SetAnnotationValue(serviceOperation, "http://docs.oasis-open.org/odata/ns/metadata", "MimeType", new EdmStringConstant(EdmCoreModel.Instance.GetString(false), "img/jpeg"));
            container.AddElement(serviceOperation);
            model.AddElement(container);

            this.BasicConstructibleModelTestSerializingStockModel(ODataTestModelBuilder.ODataTestModelAnnotationTestWithAnnotations, model);
        }

        [TestMethod]
        public void TestingDirectVocabularyAnnotationsWithVariousConstantTypes()
        {
            EdmModel model = new EdmModel();

            EdmEntityContainer container = new EdmEntityContainer("TestModel", "DefaultContainer");
            model.SetAnnotationValue(container, "http://foo.bar.com", "foo", new EdmBooleanConstant(EdmCoreModel.Instance.GetBoolean(false), true));
            model.SetAnnotationValue(container, "http://foo.bar.com", "bar", new EdmFloatingConstant(EdmCoreModel.Instance.GetDouble(false), 3.14));
            model.SetAnnotationValue(container, "http://foo.bar.com", "baz", new EdmGuidConstant(EdmCoreModel.Instance.GetGuid(false), new Guid("12345678-1234-1234-1234-123456781234")));
            model.SetAnnotationValue(container, "http://foo.bar.com", "ham", new EdmBinaryConstant(EdmCoreModel.Instance.GetBinary(false), new byte[] { 13, 14, 10, 13, 11, 14, 14, 15 }));
            model.SetAnnotationValue(container, "http://foo.bar.com", "spam", new EdmDecimalConstant(EdmCoreModel.Instance.GetDecimal(false), (decimal)3.50));
            model.SetAnnotationValue(container, "http://foo.bar.com", "spum", new EdmDateTimeOffsetConstant(EdmCoreModel.Instance.GetDateTimeOffset(false), new DateTimeOffset(2001, 1, 1, 5, 40, 00, new TimeSpan(5, 4, 0))));
            model.SetAnnotationValue(container, "http://foo.bar.com", "spork", new EdmDurationConstant(EdmCoreModel.Instance.GetDuration(false), new TimeSpan(5, 4, 3)));

            model.AddElement(container);

            IEnumerable<XElement> csdl = new []{XElement.Parse(
@"<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"" p2:bar=""3.14"" p2:baz=""12345678-1234-1234-1234-123456781234"" p2:foo=""true"" p2:ham=""0D0E0A0D0B0E0E0F"" p2:spam=""3.5"" p2:spork=""PT5H4M3S"" p2:spum=""2001-01-01T05:40:00+05:04"" xmlns:p2=""http://foo.bar.com"" />
</Schema>", LoadOptions.SetLineInfo)};

            this.BasicConstructibleModelTestSerializingStockModel(csdl, model);
        }

        [TestMethod]
        public void ConstructibleModelODataTestModelAnnotationTestWithoutAnnotation()
        {
            EdmModel model = new EdmModel();

            EdmComplexType address = new EdmComplexType("TestModel", "Address");
            EdmStructuralProperty addressStreet = address.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(true));
            EdmStructuralProperty addressZip = address.AddStructuralProperty("Zip", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(address);

            EdmEntityType person = new EdmEntityType("TestModel", "PersonType");
            EdmStructuralProperty personID = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            EdmStructuralProperty personName = person.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty personAddress = person.AddStructuralProperty("Address", new EdmComplexTypeReference(address, false));
            EdmStructuralProperty personPicture = person.AddStructuralProperty("Picture", EdmCoreModel.Instance.GetStream(false));
            person.AddKeys(personID);
            model.AddElement(person);

            EdmEntityContainer container = new EdmEntityContainer("TestModel", "DefaultContainer");
            EdmEntitySet peopleSet = container.AddEntitySet("People", person);
            EdmEntitySet personSet = container.AddEntitySet("PersonType", person);

            EdmAction serviceOperationAction = new EdmAction("TestModel", "ServiceOperation1", EdmCoreModel.Instance.GetInt32(false));
            EdmOperationParameter a = new EdmOperationParameter(serviceOperationAction, "a", EdmCoreModel.Instance.GetInt32(true));
            EdmOperationParameter b = new EdmOperationParameter(serviceOperationAction, "b", EdmCoreModel.Instance.GetString(true));
            serviceOperationAction.AddParameter(a);
            serviceOperationAction.AddParameter(b);
            model.AddElement(serviceOperationAction);
            container.AddActionImport("ServiceOperation1", serviceOperationAction);
            model.AddElement(container);

            this.BasicConstructibleModelTestSerializingStockModel(ODataTestModelBuilder.ODataTestModelAnnotationTestWithoutAnnotations, model);
        }

        [TestMethod]
        public void ConstructibleModelODataTestModelWithFunctionImport()
        {
            EdmModel model = new EdmModel();

            EdmComplexType complexType = new EdmComplexType("TestModel", "ComplexType");
            EdmStructuralProperty primitiveProperty = complexType.AddStructuralProperty("PrimitiveProperty", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty complexProperty = complexType.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(complexType, false));
            model.AddElement(complexType);

            EdmEntityType entity = new EdmEntityType("TestNS", "EntityType");
            EdmStructuralProperty entityId = entity.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
            entity.AddKeys(entityId);
            EdmStructuralProperty entityComplexProperty = entity.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(complexType, false));
            model.AddElement(entity);

            EdmEnumType enumType = new EdmEnumType("TestNS", "EnumType");
            model.AddElement(enumType);

            EdmEntityContainer container = new EdmEntityContainer("TestNS", "TestContainer");
            EdmAction primitiveOperationAction = new EdmAction("TestNS", "FunctionImport_Primitive", null);
            EdmOperationParameter primitiveParameter = new EdmOperationParameter(primitiveOperationAction, "primitive", EdmCoreModel.Instance.GetString(true));
            primitiveOperationAction.AddParameter(primitiveParameter);
            model.AddElement(primitiveOperationAction);
            container.AddActionImport("FunctionImport_Primitive", primitiveOperationAction);

            EdmAction primitiveCollectionOperationAction = new EdmAction("TestNS", "FunctionImport_PrimitiveCollection", null);
            EdmOperationParameter primitiveCollectionParameter = new EdmOperationParameter(primitiveCollectionOperationAction, "primitiveCollection", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
            primitiveCollectionOperationAction.AddParameter(primitiveCollectionParameter);
            model.AddElement(primitiveCollectionOperationAction);
            container.AddActionImport("FunctionImport_PrimitiveCollection", primitiveCollectionOperationAction);

            EdmAction complexOperationAction = new EdmAction("TestNS", "FunctionImport_Complex", null);
            EdmOperationParameter complexParameter = new EdmOperationParameter(complexOperationAction, "complex", new EdmComplexTypeReference(complexType, true));
            complexOperationAction.AddParameter(complexParameter);
            model.AddElement(complexOperationAction);
            container.AddActionImport("FunctionImport_Complex", complexOperationAction);

            EdmAction complexCollectionOperationAction = new EdmAction("TestNS", "FunctionImport_ComplexCollection", null);
            EdmOperationParameter complexCollectionParameter = new EdmOperationParameter(complexCollectionOperationAction, "complexCollection", EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexType, true)));
            complexCollectionOperationAction.AddParameter(complexCollectionParameter);
            model.AddElement(complexCollectionOperationAction);
            container.AddActionImport("FunctionImport_ComplexCollection", complexCollectionOperationAction);

            EdmAction entityOperationAction = new EdmAction("TestNS", "FunctionImport_Entry", null);
            EdmOperationParameter entityParameter = new EdmOperationParameter(entityOperationAction, "entry", new EdmEntityTypeReference(entity, true));
            entityOperationAction.AddParameter(entityParameter);
            model.AddElement(entityOperationAction);
            container.AddActionImport("FunctionImport_Entry", entityOperationAction);

            EdmAction feedOperationAction = new EdmAction("TestNS", "FunctionImport_Feed", null);
            EdmOperationParameter feedParameter = new EdmOperationParameter(feedOperationAction, "feed", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entity, true)));
            feedOperationAction.AddParameter(feedParameter);
            model.AddElement(feedOperationAction);
            container.AddActionImport("FunctionImport_Feed", feedOperationAction);

            EdmAction streamOperationAction = new EdmAction("TestNS", "FunctionImport_Stream", null);
            EdmOperationParameter streamParameter = new EdmOperationParameter(streamOperationAction, "stream", EdmCoreModel.Instance.GetStream(true));
            streamOperationAction.AddParameter(streamParameter);
            model.AddElement(streamOperationAction);
            container.AddActionImport("FunctionImport_Stream", streamOperationAction);
            
            EdmAction enumOperationAction = new EdmAction("TestNS", "FunctionImport_Enum", null);
            EdmOperationParameter enumParameter = new EdmOperationParameter(enumOperationAction, "enum", new EdmEnumTypeReference(enumType, true));
            enumOperationAction.AddParameter(enumParameter);
            model.AddElement(enumOperationAction);
            container.AddActionImport("FunctionImport_Enum", enumOperationAction);

            model.AddElement(container);

            this.BasicConstructibleModelTestSerializingStockModel(ODataTestModelBuilder.ODataTestModelWithFunctionImport, model);
        }

        [TestMethod]
        public void ConstructibleModelODataTestModelDefaultModel()
        {
            EdmModel model = new EdmModel();

            EdmComplexType aliases = new EdmComplexType("DefaultNamespace", "Aliases");
            EdmStructuralProperty aliasesAlternativeNames = aliases.AddStructuralProperty("AlternativeNames", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false, 10, null, false)));
            model.AddElement(aliases);

            EdmComplexType phone = new EdmComplexType("DefaultNamespace", "Phone");
            EdmStructuralProperty phoneNumber = phone.AddStructuralProperty("PhoneNumber", EdmCoreModel.Instance.GetString(false, 16, null, false));
            EdmStructuralProperty phoneExtension = phone.AddStructuralProperty("Extension", EdmCoreModel.Instance.GetString(false, 16, null, true));
            model.AddElement(phone);

            EdmComplexType contact = new EdmComplexType("DefaultNamespace", "ContactDetails");
            EdmStructuralProperty contactEmailBag = contact.AddStructuralProperty("EmailBag", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false, 32, null, false)));
            EdmStructuralProperty contactAlternativeNames = contact.AddStructuralProperty("AlternativeNames", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false, 10, null, false)));
            EdmStructuralProperty contactAlias = contact.AddStructuralProperty("ContactAlias", new EdmComplexTypeReference(aliases, false));
            EdmStructuralProperty contactHomePhone = contact.AddStructuralProperty("HomePhone", new EdmComplexTypeReference(phone, false));
            EdmStructuralProperty contactWorkPhone = contact.AddStructuralProperty("WorkPhone", new EdmComplexTypeReference(phone, false));
            EdmStructuralProperty contactMobilePhoneBag = contact.AddStructuralProperty("MobilePhoneBag", EdmCoreModel.GetCollection(new EdmComplexTypeReference(phone, false)));
            model.AddElement(contact);

            EdmComplexType category = new EdmComplexType("DefaultNamespace", "ComplexToCategory");
            EdmStructuralProperty categoryTerm = category.AddStructuralProperty("Term", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty categoryScheme = category.AddStructuralProperty("Scheme", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty categoryLabel = category.AddStructuralProperty("Label", EdmCoreModel.Instance.GetString(false));
            model.AddElement(category);

            EdmComplexType dimensions = new EdmComplexType("DefaultNamespace", "Dimensions");
            EdmStructuralProperty dimensionsWidth = dimensions.AddStructuralProperty("Width", EdmCoreModel.Instance.GetDecimal(10, 3, false));
            EdmStructuralProperty dimensionsHeight = dimensions.AddStructuralProperty("Height", EdmCoreModel.Instance.GetDecimal(10, 3, false));
            EdmStructuralProperty dimensionsDepth = dimensions.AddStructuralProperty("Depth", EdmCoreModel.Instance.GetDecimal(10, 3, false));
            model.AddElement(dimensions);

            EdmComplexType concurrency = new EdmComplexType("DefaultNamespace", "ConcurrencyInfo");
            EdmStructuralProperty concurrencyToken = concurrency.AddStructuralProperty("Token", EdmCoreModel.Instance.GetString(false, 20, null, false));
            EdmStructuralProperty concurrencyQueriedDateTime = concurrency.AddStructuralProperty("QueriedDateTime", EdmCoreModel.Instance.GetDateTimeOffset(true));
            model.AddElement(concurrency);

            EdmComplexType audit = new EdmComplexType("DefaultNamespace", "AuditInfo");
            EdmStructuralProperty auditModifiedDate = audit.AddStructuralProperty("ModifiedDate", EdmCoreModel.Instance.GetDateTimeOffset(false));
            EdmStructuralProperty auditModifiedBy = audit.AddStructuralProperty("ModifiedBy", EdmCoreModel.Instance.GetString(false, 50, null, false));
            EdmStructuralProperty auditConcurrency = audit.AddStructuralProperty("Concurrency", new EdmComplexTypeReference(concurrency, false));
            model.AddElement(audit);

            EdmEntityType customer = new EdmEntityType("DefaultNamespace", "Customer");
            EdmStructuralProperty customerId = customer.AddStructuralProperty("CustomerId", EdmCoreModel.Instance.GetInt32(false));
            customer.AddKeys(customerId);
            EdmStructuralProperty customerName = customer.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false, 100, null, false));
            EdmStructuralProperty customerPrimaryContact = customer.AddStructuralProperty("PrimaryContactInfo", new EdmComplexTypeReference(contact, false));
            EdmStructuralProperty customerBackupContact = customer.AddStructuralProperty("BackupContactInfo", EdmCoreModel.GetCollection(new EdmComplexTypeReference(contact, false)));
            EdmStructuralProperty customerAuditing = customer.AddStructuralProperty("Auditing", new EdmComplexTypeReference(audit, false));
            EdmStructuralProperty customerThumbnail = customer.AddStructuralProperty("Thumbnail", EdmCoreModel.Instance.GetStream(false));
            EdmStructuralProperty customerVideo = customer.AddStructuralProperty("Video", EdmCoreModel.Instance.GetStream(false));
            model.AddElement(customer);

            EdmEntityType barcode = new EdmEntityType("DefaultNamespace", "Barcode");
            EdmStructuralProperty barcodeCode = barcode.AddStructuralProperty("Code", EdmCoreModel.Instance.GetInt32(false));
            barcode.AddKeys(barcodeCode);
            EdmStructuralProperty barcodeProductId = barcode.AddStructuralProperty("ProductId", EdmCoreModel.Instance.GetInt32(false));
            EdmStructuralProperty barcodeText = barcode.AddStructuralProperty("Text", EdmCoreModel.Instance.GetString(false));
            model.AddElement(barcode);

            EdmEntityType incorrectScan = new EdmEntityType("DefaultNamespace", "IncorrectScan");
            EdmStructuralProperty incorrectScanId = incorrectScan.AddStructuralProperty("IncorrectScanId", EdmCoreModel.Instance.GetInt32(false));
            incorrectScan.AddKeys(incorrectScanId);
            EdmStructuralProperty incorrectScanExpectedCode = incorrectScan.AddStructuralProperty("ExpectedCode", EdmCoreModel.Instance.GetInt32(false));
            EdmStructuralProperty incorrectScanActualCode = incorrectScan.AddStructuralProperty("ActualCode", EdmCoreModel.Instance.GetInt32(true));
            EdmStructuralProperty incorrectScanDate = incorrectScan.AddStructuralProperty("ScanDate", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDateTimeOffset(false)));
            EdmStructuralProperty incorrectScanDetails = incorrectScan.AddStructuralProperty("Details", EdmCoreModel.Instance.GetString(false));
            model.AddElement(incorrectScan);

            EdmEntityType barcodeDetail = new EdmEntityType("DefaultNamespace", "BarcodeDetail");
            EdmStructuralProperty barcodeDetailCode = barcodeDetail.AddStructuralProperty("Code", EdmCoreModel.Instance.GetInt32(false));
            barcodeDetail.AddKeys(barcodeDetailCode);
            EdmStructuralProperty barcodeDetailRegisteredTo = barcodeDetail.AddStructuralProperty("RegisteredTo", EdmCoreModel.Instance.GetString(false));
            model.AddElement(barcodeDetail);

            EdmEntityType complaint = new EdmEntityType("DefaultNamespace", "Complaint");
            EdmStructuralProperty complaintId = complaint.AddStructuralProperty("ComplaintId", EdmCoreModel.Instance.GetInt32(false));
            complaint.AddKeys(complaintId);
            EdmStructuralProperty complaintCustomerId = complaint.AddStructuralProperty("CustomerId", EdmCoreModel.Instance.GetInt32(true));
            EdmStructuralProperty complaintLogged = complaint.AddStructuralProperty("Logged", EdmCoreModel.Instance.GetDateTimeOffset(false));
            EdmStructuralProperty complaintDetails = complaint.AddStructuralProperty("Details", EdmCoreModel.Instance.GetString(false));
            model.AddElement(complaint);

            EdmEntityType resolution = new EdmEntityType("DefaultNamespace", "Resolution");
            EdmStructuralProperty resolutionId = resolution.AddStructuralProperty("ResolutionId", EdmCoreModel.Instance.GetInt32(false));
            resolution.AddKeys(resolutionId);
            EdmStructuralProperty resolutionDetails = resolution.AddStructuralProperty("Details", EdmCoreModel.Instance.GetString(false));
            model.AddElement(resolution);

            EdmEntityType login = new EdmEntityType("DefaultNamespace", "Login");
            EdmStructuralProperty loginUsername = login.AddStructuralProperty("Username", EdmCoreModel.Instance.GetString(false, 50, null, false));
            login.AddKeys(loginUsername);
            EdmStructuralProperty loginCustomerId = login.AddStructuralProperty("CustomerId", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(login);

            EdmEntityType suspiciousActivity = new EdmEntityType("DefaultNamespace", "SuspiciousActivity");
            EdmStructuralProperty suspiciousActivityId = suspiciousActivity.AddStructuralProperty("SuspiciousActivityId", EdmCoreModel.Instance.GetInt32(false));
            suspiciousActivity.AddKeys(suspiciousActivityId);
            EdmStructuralProperty suspiciousActivityProperty = suspiciousActivity.AddStructuralProperty("Activity", EdmCoreModel.Instance.GetString(false));
            model.AddElement(suspiciousActivity);

            EdmEntityType smartCard = new EdmEntityType("DefaultNamespace", "SmartCard");
            EdmStructuralProperty smartCardUsername = smartCard.AddStructuralProperty("Username", EdmCoreModel.Instance.GetString(false, 50, null, false));
            smartCard.AddKeys(smartCardUsername);
            EdmStructuralProperty smartCardSerial = smartCard.AddStructuralProperty("CardSerial", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty smartCardIssued = smartCard.AddStructuralProperty("Issued", EdmCoreModel.Instance.GetDateTimeOffset(false));
            model.AddElement(smartCard);

            EdmEntityType rsaToken = new EdmEntityType("DefaultNamespace", "RSAToken");
            EdmStructuralProperty rsaTokenSerial = rsaToken.AddStructuralProperty("Serial", EdmCoreModel.Instance.GetString(false, 20, null, false));
            rsaToken.AddKeys(rsaTokenSerial);
            EdmStructuralProperty rsaTokenIssued = rsaToken.AddStructuralProperty("Issued", EdmCoreModel.Instance.GetDateTimeOffset(false));
            model.AddElement(rsaToken);

            EdmEntityType passwordReset = new EdmEntityType("DefaultNamespace", "PasswordReset");
            EdmStructuralProperty passwordResetNo = passwordReset.AddStructuralProperty("ResetNo", EdmCoreModel.Instance.GetInt32(false));
            EdmStructuralProperty passwordResetUsername = passwordReset.AddStructuralProperty("Username", EdmCoreModel.Instance.GetString(false, 50, null, false));
            passwordReset.AddKeys(passwordResetNo);
            passwordReset.AddKeys(passwordResetUsername);
            EdmStructuralProperty passwordResetTempPassword = passwordReset.AddStructuralProperty("TempPassword", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty passwordResetEmailedTo = passwordReset.AddStructuralProperty("EmailedTo", EdmCoreModel.Instance.GetString(false));
            model.AddElement(passwordReset);

            EdmEntityType pageView = new EdmEntityType("DefaultNamespace", "PageView");
            EdmStructuralProperty pageViewId = pageView.AddStructuralProperty("PageViewId", EdmCoreModel.Instance.GetInt32(false));
            pageView.AddKeys(pageViewId);
            EdmStructuralProperty pageViewUsername = pageView.AddStructuralProperty("Username", EdmCoreModel.Instance.GetString(false, 50, null, false));
            EdmStructuralProperty pageViewed = pageView.AddStructuralProperty("Viewed", EdmCoreModel.Instance.GetDateTimeOffset(false));
            EdmStructuralProperty pageViewPageUrl = pageView.AddStructuralProperty("PageUrl", EdmCoreModel.Instance.GetString(false, 500, null, false));
            model.AddElement(pageView);

            EdmEntityType productPageView = new EdmEntityType("DefaultNamespace", "ProductPageView", pageView);
            EdmStructuralProperty productPageViewId = productPageView.AddStructuralProperty("ProductId", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(productPageView);

            EdmEntityType lastLogin = new EdmEntityType("DefaultNamespace", "LastLogin");
            EdmStructuralProperty lastLoginUsername = lastLogin.AddStructuralProperty("Username", EdmCoreModel.Instance.GetString(false, 50, null, false));
            lastLogin.AddKeys(lastLoginUsername);
            EdmStructuralProperty lastLoginLoggedIn = lastLogin.AddStructuralProperty("LoggedIn", EdmCoreModel.Instance.GetDateTimeOffset(false));
            EdmStructuralProperty lastLoginLoggedOut = lastLogin.AddStructuralProperty("LoggedOut", EdmCoreModel.Instance.GetDateTimeOffset(true));
            model.AddElement(lastLogin);

            EdmEntityType message = new EdmEntityType("DefaultNamespace", "Message");
            EdmStructuralProperty messageId = message.AddStructuralProperty("MessageId", EdmCoreModel.Instance.GetInt32(false));
            EdmStructuralProperty messageFromUsername = message.AddStructuralProperty("FromUsername", EdmCoreModel.Instance.GetString(false, 50, null, false));
            message.AddKeys(messageId);
            message.AddKeys(messageFromUsername);
            EdmStructuralProperty messageToUsername = message.AddStructuralProperty("ToUsername", EdmCoreModel.Instance.GetString(false, 50, null, false));
            EdmStructuralProperty messageSent = message.AddStructuralProperty("Sent", EdmCoreModel.Instance.GetDateTimeOffset(false));
            EdmStructuralProperty messageSubject = message.AddStructuralProperty("Subject", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty messageBody = message.AddStructuralProperty("Body", EdmCoreModel.Instance.GetString(true));
            EdmStructuralProperty messageIsRead = message.AddStructuralProperty("IsRead", EdmCoreModel.Instance.GetBoolean(false));
            model.AddElement(message);

            EdmEntityType order = new EdmEntityType("DefaultNamespace", "Order");
            EdmStructuralProperty orderId = order.AddStructuralProperty("OrderId", EdmCoreModel.Instance.GetInt32(false));
            order.AddKeys(orderId);
            EdmStructuralProperty orderCustomerId = order.AddStructuralProperty("CustomerId", EdmCoreModel.Instance.GetInt32(true));
            EdmStructuralProperty orderConcurrency = order.AddStructuralProperty("Concurrency", new EdmComplexTypeReference(concurrency, false));
            model.AddElement(order);

            EdmEntityType orderNote = new EdmEntityType("DefaultNamespace", "OrderNote");
            EdmStructuralProperty orderNoteId = orderNote.AddStructuralProperty("NoteId", EdmCoreModel.Instance.GetInt32(false));
            orderNote.AddKeys(orderNoteId);
            EdmStructuralProperty orderNoteDescription = orderNote.AddStructuralProperty("Note", EdmCoreModel.Instance.GetString(false));
            model.AddElement(orderNote);

            EdmEntityType orderQualityCheck = new EdmEntityType("DefaultNamespace", "OrderQualityCheck");
            EdmStructuralProperty orderQualityCheckId = orderQualityCheck.AddStructuralProperty("OrderId", EdmCoreModel.Instance.GetInt32(false));
            orderQualityCheck.AddKeys(orderQualityCheckId);
            EdmStructuralProperty orderQualityCheckBy = orderQualityCheck.AddStructuralProperty("CheckedBy", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty orderQualityCheckDateTime = orderQualityCheck.AddStructuralProperty("CheckedDateTime", EdmCoreModel.Instance.GetDateTimeOffset(false));
            model.AddElement(orderQualityCheck);

            EdmEntityType orderLine = new EdmEntityType("DefaultNamespace", "OrderLine");
            EdmStructuralProperty orderLineOrderId = orderLine.AddStructuralProperty("OrderId", EdmCoreModel.Instance.GetInt32(false));
            EdmStructuralProperty orderLineProductId = orderLine.AddStructuralProperty("ProductId", EdmCoreModel.Instance.GetInt32(false));
            orderLine.AddKeys(orderLineOrderId);
            orderLine.AddKeys(orderLineProductId);
            EdmStructuralProperty orderLineQuantity = orderLine.AddStructuralProperty("Quantity", EdmCoreModel.Instance.GetInt32(false));
            EdmStructuralProperty orderLineConcurrencyToken = orderLine.AddStructuralProperty("ConcurrencyToken", EdmCoreModel.Instance.GetString(false), null);
            EdmStructuralProperty orderLineStream = orderLine.AddStructuralProperty("OrderLineStream", EdmCoreModel.Instance.GetStream(false));
            model.AddElement(orderLine);

            EdmEntityType backOrderLine = new EdmEntityType("DefaultNamespace", "BackOrderLine", orderLine);
            model.AddElement(backOrderLine);
            EdmEntityType backOrderLine2 = new EdmEntityType("DefaultNamespace", "BackOrderLine2", backOrderLine);
            model.AddElement(backOrderLine2);

            EdmEntityType product = new EdmEntityType("DefaultNamespace", "Product");
            EdmStructuralProperty productId = product.AddStructuralProperty("ProductId", EdmCoreModel.Instance.GetInt32(false));
            product.AddKeys(productId);
            EdmStructuralProperty productDescription = product.AddStructuralProperty("Description", EdmCoreModel.Instance.GetString(false, 1000, true, true));
            EdmStructuralProperty productDimensions = product.AddStructuralProperty("Dimensions", new EdmComplexTypeReference(dimensions, false));
            EdmStructuralProperty productBaseConcurrency = new EdmStructuralProperty( product, "BaseConcurrency", EdmCoreModel.Instance.GetString(false), null);
            product.AddProperty(productBaseConcurrency);
            EdmStructuralProperty productComplexConcurrency = product.AddStructuralProperty("ComplexConcurrency", new EdmComplexTypeReference(concurrency, false));
            EdmStructuralProperty productNestedComplexConcurrency = product.AddStructuralProperty("NestedComplexConcurrency", new EdmComplexTypeReference(audit, false));
            EdmStructuralProperty productPicture = product.AddStructuralProperty("Picture", EdmCoreModel.Instance.GetStream(false));
            model.AddElement(product);

            EdmEntityType discontinuedProduct = new EdmEntityType("DefaultNamespace", "DiscontinuedProduct", product);
            EdmStructuralProperty discontinuedProductDate = discontinuedProduct.AddStructuralProperty("Discontinued", EdmCoreModel.Instance.GetDateTimeOffset(false));
            EdmStructuralProperty discontinuedProductReplacementProductId = discontinuedProduct.AddStructuralProperty("ReplacementProductId", EdmCoreModel.Instance.GetInt32(true));
            EdmStructuralProperty discontinuedProductDiscontinuedPhone = discontinuedProduct.AddStructuralProperty("DiscontinuedPhone", new EdmComplexTypeReference(phone, false));
            model.AddElement(discontinuedProduct);

            EdmEntityType productDetail = new EdmEntityType("DefaultNamespace", "ProductDetail");
            EdmStructuralProperty productDetailProductId = productDetail.AddStructuralProperty("ProductId", EdmCoreModel.Instance.GetInt32(false));
            productDetail.AddKeys(productDetailProductId);
            EdmStructuralProperty productDetails = productDetail.AddStructuralProperty("Details", EdmCoreModel.Instance.GetString(false));
            model.AddElement(productDetail);

            EdmEntityType productReview = new EdmEntityType("DefaultNamespace", "ProductReview");
            EdmStructuralProperty productReviewProductId = productReview.AddStructuralProperty("ProductId", EdmCoreModel.Instance.GetInt32(false));
            EdmStructuralProperty productReviewId = productReview.AddStructuralProperty("ReviewId", EdmCoreModel.Instance.GetInt32(false));
            productReview.AddKeys(productReviewProductId);
            productReview.AddKeys(productReviewId);
            EdmStructuralProperty productReviewDescription = productReview.AddStructuralProperty("Review", EdmCoreModel.Instance.GetString(false));
            model.AddElement(productReview);

            EdmEntityType productPhoto = new EdmEntityType("DefaultNamespace", "ProductPhoto");
            EdmStructuralProperty productPhotoProductId = productPhoto.AddStructuralProperty("ProductId", EdmCoreModel.Instance.GetInt32(false));
            EdmStructuralProperty productPhotoId = productPhoto.AddStructuralProperty("PhotoId", EdmCoreModel.Instance.GetInt32(false));
            productPhoto.AddKeys(productPhotoProductId);
            productPhoto.AddKeys(productPhotoId);
            EdmStructuralProperty productPhotoBinary = productPhoto.AddStructuralProperty("Photo", EdmCoreModel.Instance.GetBinary(false));
            model.AddElement(productPhoto);

            EdmEntityType productWebFeature = new EdmEntityType("DefaultNamespace", "ProductWebFeature");
            EdmStructuralProperty productWebFeatureId = productWebFeature.AddStructuralProperty("FeatureId", EdmCoreModel.Instance.GetInt32(false));
            productWebFeature.AddKeys(productWebFeatureId);
            EdmStructuralProperty productWebFeatureProductId = productWebFeature.AddStructuralProperty("ProductId", EdmCoreModel.Instance.GetInt32(true));
            EdmStructuralProperty productWebFeaturePhotoId = productWebFeature.AddStructuralProperty("PhotoId", EdmCoreModel.Instance.GetInt32(true));
            EdmStructuralProperty productWebFeatureReviewId = productWebFeature.AddStructuralProperty("ReviewId", EdmCoreModel.Instance.GetInt32(false));
            EdmStructuralProperty productWebFeatureHeading = productWebFeature.AddStructuralProperty("Heading", EdmCoreModel.Instance.GetString(false));
            model.AddElement(productWebFeature);

            EdmEntityType supplier = new EdmEntityType("DefaultNamespace", "Supplier");
            EdmStructuralProperty supplierId = supplier.AddStructuralProperty("SupplierId", EdmCoreModel.Instance.GetInt32(false));
            supplier.AddKeys(supplierId);
            EdmStructuralProperty supplierName = supplier.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            model.AddElement(supplier);

            EdmEntityType supplierLogo = new EdmEntityType("DefaultNamespace", "SupplierLogo");
            EdmStructuralProperty supplierLogoSupplierId = supplierLogo.AddStructuralProperty("SupplierId", EdmCoreModel.Instance.GetInt32(false));
            supplierLogo.AddKeys(supplierLogoSupplierId);
            EdmStructuralProperty supplierLogoBinary = supplierLogo.AddStructuralProperty("Logo", EdmCoreModel.Instance.GetBinary(false, 500, false));
            model.AddElement(supplierLogo);

            EdmEntityType supplierInfo = new EdmEntityType("DefaultNamespace", "SupplierInfo");
            EdmStructuralProperty supplierInfoId = supplierInfo.AddStructuralProperty("SupplierInfoId", EdmCoreModel.Instance.GetInt32(false));
            supplierInfo.AddKeys(supplierInfoId);
            EdmStructuralProperty supplierInfoDescription = supplierInfo.AddStructuralProperty("Information", EdmCoreModel.Instance.GetString(false));
            model.AddElement(supplierInfo);

            EdmEntityType customerInfo = new EdmEntityType("DefaultNamespace", "CustomerInfo");
            EdmStructuralProperty customerInfoId = customerInfo.AddStructuralProperty("CustomerInfoId", EdmCoreModel.Instance.GetInt32(false));
            customerInfo.AddKeys(customerInfoId);
            EdmStructuralProperty customerInfoDescription = customerInfo.AddStructuralProperty("Information", EdmCoreModel.Instance.GetString(true));
            model.AddElement(customerInfo);

            EdmEntityType computer = new EdmEntityType("DefaultNamespace", "Computer");
            EdmStructuralProperty computerId = computer.AddStructuralProperty("ComputerId", EdmCoreModel.Instance.GetInt32(false));
            computer.AddKeys(computerId);
            EdmStructuralProperty computerName = computer.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            model.AddElement(computer);

            EdmEntityType computerDetail = new EdmEntityType("DefaultNamespace", "ComputerDetail");
            EdmStructuralProperty computerDetailId = computerDetail.AddStructuralProperty("ComputerDetailId", EdmCoreModel.Instance.GetInt32(false));
            computerDetail.AddKeys(computerDetailId);
            EdmStructuralProperty computerDetailManufacturer = computerDetail.AddStructuralProperty("Manufacturer", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty computerDetailModel = computerDetail.AddStructuralProperty("Model", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty computerDetailSerial = computerDetail.AddStructuralProperty("Serial", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty computerDetailSpecificationsBag = computerDetail.AddStructuralProperty("SpecificationsBag", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false)));
            EdmStructuralProperty computerDetailPurchaseDate = computerDetail.AddStructuralProperty("PurchaseDate", EdmCoreModel.Instance.GetDateTimeOffset(false));
            EdmStructuralProperty computerDetailDimensions = computerDetail.AddStructuralProperty("Dimensions", new EdmComplexTypeReference(dimensions, false));
            model.AddElement(computerDetail);

            EdmEntityType driver = new EdmEntityType("DefaultNamespace", "Driver");
            EdmStructuralProperty driverName = driver.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false, 100, null, false));
            driver.AddKeys(driverName);
            EdmStructuralProperty driverBirthDate = driver.AddStructuralProperty("BirthDate", EdmCoreModel.Instance.GetDateTimeOffset(false));
            model.AddElement(driver);

            EdmEntityType license = new EdmEntityType("DefaultNamespace", "License");
            EdmStructuralProperty licenseName = license.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false, 100, null, false));
            license.AddKeys(licenseName);
            EdmStructuralProperty licenseNumber = license.AddStructuralProperty("LicenseNumber", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty licenseClass = license.AddStructuralProperty("LicenseClass", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty licenseRestrictions = license.AddStructuralProperty("Restrictions", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty licenseExpiration = license.AddStructuralProperty("ExpirationDate", EdmCoreModel.Instance.GetDateTimeOffset(false));
            model.AddElement(license);

            EdmEntityType mappedEntity = new EdmEntityType("DefaultNamespace", "MappedEntityType");
            EdmStructuralProperty mappedEntityId = mappedEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            mappedEntity.AddKeys(mappedEntityId);
            EdmStructuralProperty mappedEntityHref = mappedEntity.AddStructuralProperty("Href", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty mappedEntityTitle = mappedEntity.AddStructuralProperty("Title", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty mappedEntityHrefLang = mappedEntity.AddStructuralProperty("HrefLang", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty mappedEntityType = mappedEntity.AddStructuralProperty("Type", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty mappedEntityLength = mappedEntity.AddStructuralProperty("Length", EdmCoreModel.Instance.GetInt32(false));
            EdmStructuralProperty mappedEntityBagOfPrimitiveToLinks = mappedEntity.AddStructuralProperty("BagOfPrimitiveToLinks", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false)));
            EdmStructuralProperty mappedEntityBagOfDecimals = mappedEntity.AddStructuralProperty("BagOfDecimals", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDecimal(false)));
            EdmStructuralProperty mappedEntityBagOfDoubles = mappedEntity.AddStructuralProperty("BagOfDoubles", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDouble(false)));
            EdmStructuralProperty mappedEntityBagOfSingles = mappedEntity.AddStructuralProperty("BagOfSingles", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetSingle(false)));
            EdmStructuralProperty mappedEntityBagOfBytes = mappedEntity.AddStructuralProperty("BagOfBytes", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetByte(false)));
            EdmStructuralProperty mappedEntityBagOfInt16s = mappedEntity.AddStructuralProperty("BagOfInt16s", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt16(false)));
            EdmStructuralProperty mappedEntityBagOfInt32s = mappedEntity.AddStructuralProperty("BagOfInt32s", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false)));
            EdmStructuralProperty mappedEntityBagOfInt64s = mappedEntity.AddStructuralProperty("BagOfInt64s", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt64(false)));
            EdmStructuralProperty mappedEntityBagOfGuids = mappedEntity.AddStructuralProperty("BagOfGuids", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetGuid(false)));
            EdmStructuralProperty mappedEntityBagOfComplexToCategories = mappedEntity.AddStructuralProperty("BagOfComplexToCategories", EdmCoreModel.GetCollection(new EdmComplexTypeReference(category, false)));
            model.AddElement(mappedEntity);

            EdmEntityType car = new EdmEntityType("DefaultNamespace", "Car");
            EdmStructuralProperty carVin = car.AddStructuralProperty("VIN", EdmCoreModel.Instance.GetInt32(false));
            car.AddKeys(carVin);
            EdmStructuralProperty carDescription = car.AddStructuralProperty("Description", EdmCoreModel.Instance.GetString(false, 100, null, true));
            EdmStructuralProperty carPhoto = car.AddStructuralProperty("Photo", EdmCoreModel.Instance.GetStream(false));
            EdmStructuralProperty carVideo = car.AddStructuralProperty("Video", EdmCoreModel.Instance.GetStream(false));
            model.AddElement(car);

            EdmEntityType person = new EdmEntityType("DefaultNamespace", "Person");
            EdmStructuralProperty personId = person.AddStructuralProperty("PersonId", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            EdmStructuralProperty personName = person.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            model.AddElement(person);

            EdmEntityType employee = new EdmEntityType("DefaultNamespace", "Employee", person);
            EdmStructuralProperty employeeManagerId = employee.AddStructuralProperty("ManagersPersonId", EdmCoreModel.Instance.GetInt32(false));
            EdmStructuralProperty employeeSalary = employee.AddStructuralProperty("Salary", EdmCoreModel.Instance.GetInt32(false));
            EdmStructuralProperty employeeTitle = employee.AddStructuralProperty("Title", EdmCoreModel.Instance.GetString(false));
            model.AddElement(employee);

            EdmEntityType specialEmployee = new EdmEntityType("DefaultNamespace", "SpecialEmployee", employee);
            EdmStructuralProperty specialEmployeeCarsVIN = specialEmployee.AddStructuralProperty("CarsVIN", EdmCoreModel.Instance.GetInt32(false));
            EdmStructuralProperty specialEmployeeBonus = specialEmployee.AddStructuralProperty("Bonus", EdmCoreModel.Instance.GetInt32(false));
            EdmStructuralProperty specialEmployeeIsFullyVested = specialEmployee.AddStructuralProperty("IsFullyVested", EdmCoreModel.Instance.GetBoolean(false));
            model.AddElement(specialEmployee);

            EdmEntityType personMetadata = new EdmEntityType("DefaultNamespace", "PersonMetadata");
            EdmStructuralProperty personMetadataId = personMetadata.AddStructuralProperty("PersonMetadataId", EdmCoreModel.Instance.GetInt32(false));
            personMetadata.AddKeys(personMetadataId);
            EdmStructuralProperty personMetadataPersonId = personMetadata.AddStructuralProperty("PersonId", EdmCoreModel.Instance.GetInt32(false));
            EdmStructuralProperty personMetadataPropertyName = personMetadata.AddStructuralProperty("PropertyName", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty personMetadataPropertyValue = personMetadata.AddStructuralProperty("PropertyValue", EdmCoreModel.Instance.GetString(false));
            model.AddElement(personMetadata);

            EdmNavigationProperty customerToOrders = customer.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Orders", Target = order, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "Customer", TargetMultiplicity = EdmMultiplicity.ZeroOrOne, DependentProperties = new[] { orderCustomerId }, PrincipalProperties = customer.Key() });

            EdmNavigationProperty customerToLogins = customer.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Logins", Target = login, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "Customer", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new [] { loginCustomerId }, PrincipalProperties = customer.Key()});

            EdmNavigationProperty customerToHusband = customer.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Husband", Target = customer, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
                new EdmNavigationPropertyInfo() { Name = "Wife", TargetMultiplicity = EdmMultiplicity.ZeroOrOne });

            EdmNavigationProperty customerToInfo = customer.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Info", Target = customerInfo, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
                new EdmNavigationPropertyInfo() { Name = "Customer", TargetMultiplicity = EdmMultiplicity.One });

            EdmNavigationProperty customerToComplaint = customer.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Complaints", Target = complaint, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "Customer", TargetMultiplicity = EdmMultiplicity.ZeroOrOne, DependentProperties = new[] { complaintCustomerId }, PrincipalProperties = customer.Key() });

            EdmNavigationProperty barcodeToProduct = barcode.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Product", Target = product, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { barcodeProductId }, PrincipalProperties = product.Key() },
                new EdmNavigationPropertyInfo() { Name = "Barcodes", TargetMultiplicity = EdmMultiplicity.Many });

            EdmNavigationProperty barcodeToExpectedIncorrectScans = barcode.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "BadScans", Target = incorrectScan, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "ExpectedBarcode", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { incorrectScanExpectedCode }, PrincipalProperties = barcode.Key() });

            EdmNavigationProperty barcodeToActualIncorrectScans = barcode.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "GoodScans", Target = incorrectScan, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "ActualBarcode", TargetMultiplicity = EdmMultiplicity.ZeroOrOne, DependentProperties = new[] { incorrectScanActualCode }, PrincipalProperties = barcode.Key()});

            EdmNavigationProperty barcodeToBarcodeDetail = barcode.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Detail", Target = barcodeDetail, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
                new EdmNavigationPropertyInfo() { Name = "Barcode", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { barcodeDetailCode }, PrincipalProperties = barcode.Key()});

            EdmNavigationProperty complaintToResolution = complaint.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Resolution", Target = resolution, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
                new EdmNavigationPropertyInfo() { Name = "Complaint", TargetMultiplicity = EdmMultiplicity.One });

            EdmNavigationProperty loginToLastLogin = login.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "LastLogin", Target = lastLogin, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
                new EdmNavigationPropertyInfo() { Name = "Login", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { lastLoginUsername }, PrincipalProperties = login.Key()});

            EdmNavigationProperty loginToSentMessages = login.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "SentMessages", Target = message, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "Sender", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { messageFromUsername }, PrincipalProperties = login.Key()});

            EdmNavigationProperty loginToReceivedMessages = login.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "ReceivedMessages", Target = message, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "Recipient", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { messageToUsername }, PrincipalProperties = login.Key()});

            EdmNavigationProperty loginToOrders = login.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Orders", Target = order, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "Login", TargetMultiplicity = EdmMultiplicity.ZeroOrOne });

            EdmNavigationProperty loginToSmartCard = login.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "SmartCard", Target = smartCard, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
                new EdmNavigationPropertyInfo() { Name = "Login", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { smartCardUsername }, PrincipalProperties = login.Key()});

            EdmNavigationProperty loginToRsaToken = login.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "RSAToken", Target = rsaToken, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
                new EdmNavigationPropertyInfo() { Name = "Login", TargetMultiplicity = EdmMultiplicity.One });

            EdmNavigationProperty loginToPasswordReset = login.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "PasswordResets", Target = passwordReset, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "Login", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { passwordResetUsername }, PrincipalProperties = login.Key()});

            EdmNavigationProperty loginToPageView = login.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "PageViews", Target = pageView, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "Login", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { pageViewUsername }, PrincipalProperties = login.Key()});

            EdmNavigationProperty loginToSuspiciousActivity = login.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "SuspiciousActivity", Target = suspiciousActivity, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "Login", TargetMultiplicity = EdmMultiplicity.ZeroOrOne });

            EdmNavigationProperty smartCardToLastLogin = smartCard.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "LastLogin", Target = lastLogin, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
                new EdmNavigationPropertyInfo() { Name = "SmartCard", TargetMultiplicity = EdmMultiplicity.ZeroOrOne });

            EdmNavigationProperty orderToOrderLines = order.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "OrderLines", Target = orderLine, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "Order", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { orderLineOrderId }, PrincipalProperties = order.Key()});

            EdmNavigationProperty orderToOrderNotes = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "Notes", Target = orderNote, TargetMultiplicity = EdmMultiplicity.Many, OnDelete = EdmOnDeleteAction.Cascade },
                new EdmNavigationPropertyInfo() { Name = "Order", Target = order, TargetMultiplicity = EdmMultiplicity.One });
            order.AddProperty(orderToOrderNotes);
            orderNote.AddProperty(orderToOrderNotes.Partner);

            EdmNavigationProperty orderToOrderQualityCheck = order.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "OrderQualityCheck", Target = orderQualityCheck, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
                new EdmNavigationPropertyInfo() { Name = "Order", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { orderQualityCheckId }, PrincipalProperties = order.Key()});

            EdmNavigationProperty orderLineToProduct = orderLine.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Product", Target = product, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { orderLineProductId }, PrincipalProperties = product.Key()},
                new EdmNavigationPropertyInfo() { Name = "OrderLines", TargetMultiplicity = EdmMultiplicity.Many });

            EdmNavigationProperty productToRelatedProducts = product.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "RelatedProducts", Target = product, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "ProductToRelatedProducts", TargetMultiplicity = EdmMultiplicity.One });

            EdmNavigationProperty productToSuppliers = product.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Suppliers", Target = supplier, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "Products", TargetMultiplicity = EdmMultiplicity.Many });

            EdmNavigationProperty productToProductDetail = product.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Detail", Target = productDetail, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
                new EdmNavigationPropertyInfo() { Name = "Product", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { productDetailProductId }, PrincipalProperties = product.Key()});

            EdmNavigationProperty productToProductReviews = product.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Reviews", Target = productReview, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "Product", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { productReviewProductId }, PrincipalProperties = product.Key()});

            EdmNavigationProperty productToProductPhotos = product.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Photos", Target = productPhoto, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "Product", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { productPhotoProductId }, PrincipalProperties = product.Key()});

            EdmNavigationProperty productReviewToProductWebFeatures = productReview.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Features", Target = productWebFeature, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "Review", TargetMultiplicity = EdmMultiplicity.ZeroOrOne, DependentProperties = new[] { productWebFeatureReviewId, productWebFeatureProductId }, PrincipalProperties = productReview.Key()});

            EdmNavigationProperty productPhotoToProductWebFeatures = productPhoto.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Features", Target = productWebFeature, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "Photo", TargetMultiplicity = EdmMultiplicity.ZeroOrOne, DependentProperties = new[] { productWebFeaturePhotoId, productWebFeatureProductId }, PrincipalProperties = productPhoto.Key()});

            EdmNavigationProperty supplierToSupplierLogo = supplier.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Logo", Target = supplierLogo, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
                new EdmNavigationPropertyInfo() { Name = "Supplier", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { supplierLogoSupplierId }, PrincipalProperties = supplier.Key()});

            EdmNavigationProperty supplierToSupplierInfo = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "SupplierInfo", Target = supplierInfo, TargetMultiplicity = EdmMultiplicity.Many, OnDelete = EdmOnDeleteAction.Cascade },
                new EdmNavigationPropertyInfo() { Name = "Supplier", Target = supplier, TargetMultiplicity = EdmMultiplicity.One });
            supplier.AddProperty(supplierToSupplierInfo);
            supplierInfo.AddProperty(supplierToSupplierInfo.Partner);

            EdmNavigationProperty computerToComputerDetail = computer.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "ComputerDetail", Target = computerDetail, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "Computer", TargetMultiplicity = EdmMultiplicity.One });

            EdmNavigationProperty driverTolicense = driver.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "License", Target = license, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "Driver", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { licenseName }, PrincipalProperties = driver.Key()});

            EdmNavigationProperty personToPersonMetadata = person.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "PersonMetadata", Target = personMetadata, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "Person", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { personMetadataPersonId }, PrincipalProperties = person.Key()});

            EdmNavigationProperty employeeToManager = employee.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Manager", Target = employee, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { employeeManagerId }, PrincipalProperties = employee.Key()},
                new EdmNavigationPropertyInfo() { Name = "EmployeeToManager", TargetMultiplicity = EdmMultiplicity.Many });

            EdmNavigationProperty specialEmployeeToCar = specialEmployee.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Car", Target = car, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { specialEmployeeCarsVIN }, PrincipalProperties = car.Key()},
                new EdmNavigationPropertyInfo() { Name = "SpecialEmployee", TargetMultiplicity = EdmMultiplicity.Many });

            EdmOperation getPrimitiveString = new EdmFunction("DefaultNamespace", "GetPrimitiveString", EdmCoreModel.Instance.GetString(true));
            model.AddElement(getPrimitiveString);
            EdmOperation getSpecificCustomer = new EdmFunction("DefaultNamespace", "GetSpecificCustomer", EdmCoreModel.GetCollection(new EdmEntityTypeReference(customer, true)));
            getSpecificCustomer.AddParameter("Name", EdmCoreModel.Instance.GetString(false));
            model.AddElement(getSpecificCustomer);
            EdmOperation getCustomerCount = new EdmFunction("DefaultNamespace", "GetCustomerCount", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(getCustomerCount);
            EdmOperation getArgumentPlusOne = new EdmFunction("DefaultNamespace", "GetArgumentPlusOne", EdmCoreModel.Instance.GetInt32(true));
            getArgumentPlusOne.AddParameter("arg1", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(getArgumentPlusOne);
            EdmOperation entityProjectionReturnsCollectionOfComplexTypes = new EdmFunction("DefaultNamespace", "EntityProjectionReturnsCollectionOfComplexTypes", EdmCoreModel.GetCollection(new EdmComplexTypeReference(contact, true)));
            model.AddElement(entityProjectionReturnsCollectionOfComplexTypes);

            EdmOperation setArgumentPlusOne = new EdmAction("DefaultNamespace", "SetArgumentPlusOne", EdmCoreModel.Instance.GetInt32(true));
            setArgumentPlusOne.AddParameter("arg1", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(setArgumentPlusOne);

            EdmEntityContainer container = new EdmEntityContainer("DefaultNamespace", "DefaultContainer");
            model.AddElement(container);

            EdmEntitySet carSet = container.AddEntitySet("Car", car);
            EdmEntitySet customerSet = container.AddEntitySet("Customer", customer);
            EdmEntitySet barcodeSet = container.AddEntitySet("Barcode", barcode);
            EdmEntitySet incorrectScanSet = container.AddEntitySet("IncorrectScan", incorrectScan);
            EdmEntitySet barcodeDetailSet = container.AddEntitySet("BarcodeDetail", barcodeDetail);
            EdmEntitySet complaintSet = container.AddEntitySet("Complaint", complaint);
            EdmEntitySet resolutionSet = container.AddEntitySet("Resolution", resolution);
            EdmEntitySet loginSet = container.AddEntitySet("Login", login);
            EdmEntitySet suspiciousActivitySet = container.AddEntitySet("SuspiciousActivity", suspiciousActivity);
            EdmEntitySet smartCardSet = container.AddEntitySet("SmartCard", smartCard);
            EdmEntitySet rsaTokenSet = container.AddEntitySet("RSAToken", rsaToken);
            EdmEntitySet passwordResetSet = container.AddEntitySet("PasswordReset", passwordReset);
            EdmEntitySet pageViewSet = container.AddEntitySet("PageView", pageView);
            EdmEntitySet lastLoginSet = container.AddEntitySet("LastLogin", lastLogin);
            EdmEntitySet messageSet = container.AddEntitySet("Message", message);
            EdmEntitySet orderSet = container.AddEntitySet("Order", order);
            EdmEntitySet orderNoteSet = container.AddEntitySet("OrderNote", orderNote);
            EdmEntitySet orderQualityCheckSet = container.AddEntitySet("OrderQualityCheck", orderQualityCheck);
            EdmEntitySet orderLineSet = container.AddEntitySet("OrderLine", orderLine);
            EdmEntitySet productSet = container.AddEntitySet("Product", product);
            EdmEntitySet productDetailSet = container.AddEntitySet("ProductDetail", productDetail);
            EdmEntitySet productReviewSet = container.AddEntitySet("ProductReview", productReview);
            EdmEntitySet productPhotoSet = container.AddEntitySet("ProductPhoto", productPhoto);
            EdmEntitySet productWebFeatureSet = container.AddEntitySet("ProductWebFeature", productWebFeature);
            EdmEntitySet supplierSet = container.AddEntitySet("Supplier", supplier);
            EdmEntitySet supplierLogoSet = container.AddEntitySet("SupplierLogo", supplierLogo);
            EdmEntitySet supplierInfoSet = container.AddEntitySet("SupplierInfo", supplierInfo);
            EdmEntitySet customerInfoSet = container.AddEntitySet("CustomerInfo", customerInfo);
            EdmEntitySet computerSet = container.AddEntitySet("Computer", computer);
            EdmEntitySet computerDetailSet = container.AddEntitySet("ComputerDetail", computerDetail);
            EdmEntitySet driverSet = container.AddEntitySet("Driver", driver);
            EdmEntitySet licenseSet = container.AddEntitySet("License", license);
            EdmEntitySet mappedEntitySet = container.AddEntitySet("MappedEntityType", mappedEntity);
            EdmEntitySet personSet = container.AddEntitySet("Person", person);
            EdmEntitySet personMetadataSet = container.AddEntitySet("PersonMetadata", personMetadata);

            complaintSet.AddNavigationTarget(customerToComplaint.Partner, customerSet);
            loginSet.AddNavigationTarget(loginToSentMessages, messageSet);
            loginSet.AddNavigationTarget(loginToReceivedMessages, messageSet);
            customerInfoSet.AddNavigationTarget(customerToInfo.Partner, customerSet);
            supplierSet.AddNavigationTarget(supplierToSupplierInfo, supplierInfoSet);
            loginSet.AddNavigationTarget(loginToOrders, orderSet);
            orderSet.AddNavigationTarget(orderToOrderNotes, orderNoteSet);
            orderSet.AddNavigationTarget(orderToOrderQualityCheck, orderQualityCheckSet);
            supplierSet.AddNavigationTarget(supplierToSupplierLogo, supplierLogoSet);
            customerSet.AddNavigationTarget(customerToOrders, orderSet);
            customerSet.AddNavigationTarget(customerToLogins, loginSet);
            loginSet.AddNavigationTarget(loginToLastLogin, lastLoginSet);
            lastLoginSet.AddNavigationTarget(smartCardToLastLogin.Partner, smartCardSet);
            orderSet.AddNavigationTarget(orderToOrderLines, orderLineSet);
            productSet.AddNavigationTarget(orderLineToProduct.Partner, orderLineSet);
            productSet.AddNavigationTarget(productToRelatedProducts, productSet);
            productSet.AddNavigationTarget(productToProductDetail, productDetailSet);
            productSet.AddNavigationTarget(productToProductReviews, productReviewSet);
            productSet.AddNavigationTarget(productToProductPhotos, productPhotoSet);
            productWebFeatureSet.AddNavigationTarget(productPhotoToProductWebFeatures.Partner, productPhotoSet);
            productWebFeatureSet.AddNavigationTarget(productReviewToProductWebFeatures.Partner, productReviewSet);
            complaintSet.AddNavigationTarget(complaintToResolution, resolutionSet);
            barcodeSet.AddNavigationTarget(barcodeToExpectedIncorrectScans, incorrectScanSet);
            customerSet.AddNavigationTarget(customerToHusband.Partner, customerSet);
            barcodeSet.AddNavigationTarget(barcodeToActualIncorrectScans, incorrectScanSet);
            productSet.AddNavigationTarget(barcodeToProduct.Partner, barcodeSet);
            barcodeSet.AddNavigationTarget(barcodeToBarcodeDetail, barcodeDetailSet);
            loginSet.AddNavigationTarget(loginToSuspiciousActivity, suspiciousActivitySet);
            loginSet.AddNavigationTarget(loginToRsaToken, rsaTokenSet);
            loginSet.AddNavigationTarget(loginToSmartCard, smartCardSet);
            loginSet.AddNavigationTarget(loginToPasswordReset, passwordResetSet);
            loginSet.AddNavigationTarget(loginToPageView, pageViewSet);
            computerSet.AddNavigationTarget(computerToComputerDetail, computerDetailSet);
            driverSet.AddNavigationTarget(driverTolicense, licenseSet);
            personSet.AddNavigationTarget(personToPersonMetadata, personMetadataSet);
            productSet.AddNavigationTarget(productToSuppliers, supplierSet);
            carSet.AddNavigationTarget(specialEmployeeToCar.Partner, personSet);
            personSet.AddNavigationTarget(employeeToManager, personSet);

            this.BasicConstructibleModelTestSerializingStockModel(ODataTestModelBuilder.ODataTestModelDefaultModel, model);
        }

        [TestMethod]
        public void ConstructibleModelODataTestModelWithSingleEntityType()
        {
            EdmModel model = new EdmModel();

            EdmEntityType singleton = new EdmEntityType("TestModel", "SingletonEntityType");
            EdmStructuralProperty singletonId = singleton.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            singleton.AddKeys(singletonId);
            EdmStructuralProperty singletonName = singleton.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            model.AddElement(singleton);

            EdmEntityContainer container = new EdmEntityContainer("TestModel", "DefaultContainer");
            EdmEntitySet singletonSet = container.AddEntitySet("SingletonEntityType", singleton);

            model.AddElement(container);

            this.BasicConstructibleModelTestSerializingStockModel(ODataTestModelBuilder.ODataTestModelWithSingleEntityType, model);
        }

        [TestMethod]
        public void ConstructibleModelODataTestModelWithSingleComplexType()
        {
            EdmModel model = new EdmModel();

            EdmEntityContainer container = new EdmEntityContainer("TestModel", "DefaultContainer");
            model.AddElement(container);

            EdmComplexType singleton = new EdmComplexType("TestModel", "SingletonComplexType");
            EdmStructuralProperty city = singleton.AddStructuralProperty("City", EdmCoreModel.Instance.GetString(true));
            model.AddElement(singleton);

            this.BasicConstructibleModelTestSerializingStockModel(ODataTestModelBuilder.ODataTestModelWithSingleComplexType, model);
        }

        [TestMethod]
        public void ConstructibleModelODataTestModelWithMultiValueProperty()
        {
            EdmModel model = new EdmModel();

            EdmEntityContainer container = new EdmEntityContainer("TestModel", "DefaultContainer");
            model.AddElement(container);

            EdmComplexType collectionType = new EdmComplexType("TestModel", "EntityTypeWithCollection");
            EdmStructuralProperty cities = collectionType.AddStructuralProperty("Cities", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
            model.AddElement(collectionType);

            this.BasicConstructibleModelTestSerializingStockModel(ODataTestModelBuilder.ODataTestModelWithCollectionProperty, model);
        }

        [TestMethod]
        public void ConstructibleModelODataTestModelWithOpenType()
        {
            EdmModel model = new EdmModel();

            EdmEntityType openEntity = new EdmEntityType("TestModel", "OpenEntityType", null, false, true);
            EdmStructuralProperty openEntityId = openEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            openEntity.AddKeys(openEntityId);
            model.AddElement(openEntity);

            EdmEntityContainer container = new EdmEntityContainer("TestModel", "DefaultContainer");
            EdmEntitySet openSet = container.AddEntitySet("OpenEntityType", openEntity);
            model.AddElement(container);

            this.BasicConstructibleModelTestSerializingStockModel(ODataTestModelBuilder.ODataTestModelWithOpenType, model);
        }

        [TestMethod]
        public void ConstructibleModelODataTestModelWithNamedStream()
        {
            EdmModel model = new EdmModel();

            EdmEntityType streamEntity = new EdmEntityType("TestModel", "NamedStreamEntityType");
            EdmStructuralProperty streamId = streamEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            streamEntity.AddKeys(streamId);
            EdmStructuralProperty namedStream = streamEntity.AddStructuralProperty("NamedStream", EdmCoreModel.Instance.GetStream(false));
            model.AddElement(streamEntity);

            EdmEntityContainer container = new EdmEntityContainer("TestModel", "DefaultContainer");
            EdmEntitySet streamSet = container.AddEntitySet("NamedStreamEntityType", streamEntity);
            model.AddElement(container);

            this.BasicConstructibleModelTestSerializingStockModel(ODataTestModelBuilder.ODataTestModelWithNamedStream, model);
        }
    }
}
