//---------------------------------------------------------------------
// <copyright file="FindMethodTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using EdmLibTests.StubEdm;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Metadata;
#if SILVERLIGHT
    using Microsoft.Silverlight.Testing;
#endif
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FindMethodTests : EdmLibTestCaseBase
    {
        public FindMethodTests()
        {
            EdmVersion = EdmVersion.V40;
        }

        [TestMethod]
        public void FindMethodsTestValidNameCheckModel()
        {
            BasicFindMethodsTest(ModelBuilder.ValidNameCheckModelEdm());
        }

        [TestMethod]
        public void FindMethodsTestMultipleSchemasWithDifferentNamespacesCyclicInvalid()
        {
            BasicFindMethodsTest(FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid());
        }

        [TestMethod]
        public void FindMethodsTestMultipleSchemasWithDifferentNamespaces()
        {
            BasicFindMethodsTest(FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesEdm());
        }

        [TestMethod]
        public void FindMethodsTestMultipleSchemasWithSameNamespace()
        {
            BasicFindMethodsTest(FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace(EdmVersion.V40));
        }

        [TestMethod]
        public void FindSchemaTypeForNoContentModelWithNonExistingSchema()
        {
            var testData = new[] { XElement.Parse(@"
<Schema Namespace=""DefaultNamespace"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" />") };

            IEdmModel testModel;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(
                from xElement in testData select xElement.CreateReader(),
                out testModel,
                out errors);
            Assert.IsTrue(parsed, "SchemaReader.TryParse failed");
            Assert.IsTrue(!errors.Any(), "SchemaReader.TryParse returned errors");
            Assert.IsNull(testModel.FindType("NonExistSchema"), "FindSchemaTypeForNoContentModel failed");
        }

        [TestMethod]
        public void FindSchemaTypeForPrimitiveTypes()
        {
            var testData = this.GetSerializerResult(ModelBuilder.ValidNameCheckModelEdm()).Select(XElement.Parse);

            IEdmModel testModel;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(
                from xElement in testData select xElement.CreateReader(),
                out testModel,
                out errors);
            Assert.IsTrue(parsed, "SchemaReader.TryParse failed");
            Assert.IsTrue(!errors.Any(), "SchemaReader.TryParse returned errors");
            Assert.IsNull(testModel.FindDeclaredType("Edm.Int32"), "FindSchemaType should not return primitive types");
            //Assert.IsNull(testModel.FindType("Int32"), "FindSchemaType should not return primitive types");
            Assert.IsNotNull(testModel.FindType("Edm.Int32"), "FindSchemaType should not return primitive types");
        }

        [TestMethod]
        public void FindMethodTestAssociationCompositeFk()
        {
            this.BasicFindMethodsTest(ModelBuilder.AssociationCompositeFkEdm());
        }

        [TestMethod]
        public void FindMethodTestAssociationFk()
        {
            this.BasicFindMethodsTest(ModelBuilder.AssociationFkEdm());
        }

        [TestMethod]
        public void FindMethodTestAssociationFkWithNavigation()
        {
            this.BasicFindMethodsTest(ModelBuilder.AssociationFkWithNavigationEdm());
        }

        [TestMethod]
        public void FindMethodTestAssociationIndependent()
        {
            this.BasicFindMethodsTest(ModelBuilder.AssociationIndependentEdm());
        }

        [TestMethod]
        public void FindMethodTestAssociationOnDeleteModel()
        {
            this.BasicFindMethodsTest(ModelBuilder.AssociationOnDeleteModelEdm());
        }

        [TestMethod]
        public void FindMethodTestCollectionTypes()
        {
            var testCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(ModelBuilder.CollectionTypes().ToArray(), this.EdmVersion);
            var testModelImmutable = this.GetParserResult(testCsdls);

            this.VerifyFindSchemaElementMethod(testCsdls.Single(), testModelImmutable);
            this.VerifyFindEntityContainer(testCsdls.Single(), testModelImmutable);
            this.VerifyFindEntityContainerElement(testCsdls.Single(), testModelImmutable);

            var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
            testCsdls = this.GetSerializerResult(testModelConstructible).Select(n => XElement.Parse(n));

            this.VerifyFindSchemaElementMethod(testCsdls.Single(), testModelConstructible);
            this.VerifyFindEntityContainer(testCsdls.Single(), testModelConstructible);
            this.VerifyFindEntityContainerElement(testCsdls.Single(), testModelConstructible);
        }

        [TestMethod]
        public void FindMethodTestCollectionTypesWithSimpleType()
        {
            this.BasicFindMethodsTest(ModelBuilder.CollectionTypesWithSimpleType());
        }

        [TestMethod]
        public void FindMethodTestComplexTypeAttributes()
        {
            this.BasicFindMethodsTest(ModelBuilder.ComplexTypeAttributesEdm());
        }

        [TestMethod]
        public void FindMethodTestEntityContainerAttributes()
        {
            this.BasicFindMethodsTest(ModelBuilder.EntityContainerAttributes());
        }

        [TestMethod]
        public void FindMethodTestEntityContainerWithEntitySets()
        {
            this.BasicFindMethodsTest(ModelBuilder.EntityContainerWithEntitySetsEdm());
        }

        [TestMethod]
        public void FindMethodTestEntityContainerWithFunctionImports()
        {
            this.BasicFindMethodsTest(ModelBuilder.EntityContainerWithOperationImportsEdm());
        }

        [TestMethod]
        public void FindMethodTestEntityInheritanceTree()
        {
            this.BasicFindMethodsTest(ModelBuilder.EntityInheritanceTreeEdm());
        }

        [TestMethod]
        public void FindMethodTestFunctionWithAll()
        {
            this.BasicFindMethodsTest(ModelBuilder.FunctionWithAllEdm());
        }

        [TestMethod]
        public void FindMethodTestModelWithAllConcepts()
        {
            this.BasicFindMethodsTest(ModelBuilder.ModelWithAllConceptsEdm());
        }

        [TestMethod]
        public void FindMethodTestMultipleNamespaces()
        {
            this.BasicFindMethodsTest(ModelBuilder.MultipleNamespacesEdm());
        }

        [TestMethod]
        public void FindMethodTestOneComplexWithAllPrimitiveProperty()
        {
            this.BasicFindMethodsTest(ModelBuilder.OneComplexWithAllPrimitivePropertyEdm());
        }

        [TestMethod]
        public void FindMethodTestOneComplexWithNestedComplex()
        {
            this.BasicFindMethodsTest(ModelBuilder.OneComplexWithNestedComplexEdm());
        }

        [TestMethod]
        public void FindMethodTestOneComplexWithOneProperty()
        {
            this.BasicFindMethodsTest(ModelBuilder.OneComplexWithOnePropertyEdm());
        }

        [TestMethod]
        public void FindMethodTestOneEntityWithAllPrimitiveProperty()
        {
            this.BasicFindMethodsTest(ModelBuilder.OneEntityWithAllPrimitivePropertyEdm());
        }

        [TestMethod]
        public void FindMethodTestOneEntityWithOneProperty()
        {
            this.BasicFindMethodsTest(ModelBuilder.OneEntityWithOnePropertyEdm());
        }

        [TestMethod]
        public void FindMethodTestPropertyFacetsCollection()
        {
            this.BasicFindMethodsTest(ModelBuilder.PropertyFacetsCollectionEdm());
        }

        [TestMethod]
        public void FindMethodTestSimpleAllPrimtiveTypes()
        {
            this.BasicFindMethodsTest(ModelBuilder.SimpleAllPrimitiveTypes(this.EdmVersion, true, true));
        }

        [TestMethod]
        public void FindMethodTestSimpleConstrucitveApiTestModel()
        {
            this.BasicFindMethodsTest(ModelBuilder.SimpleConstructiveApiTestModel());
        }

        [TestMethod]
        public void FindMethodTestStringWithFacets()
        {
            this.BasicFindMethodsTest(ModelBuilder.StringWithFacets());
        }

        [TestMethod]
        public void FindMethodTestTaupoDefaultModel()
        {
            this.BasicFindMethodsTest(ModelBuilder.TaupoDefaultModelEdm());
        }

        [TestMethod]
        // [EdmLib] FindProperty should return AmbiguousElementBinding when the model has duplicate properties
        public void FindMethodTestDuplicatePropertyName()
        {
            var edmModel = this.GetParserResult(ValidationTestModelBuilder.DuplicatePropertyName(this.EdmVersion));

            var complexType = edmModel.FindType("CollectionAtomic.ComplexTypeA") as IEdmComplexType;
            var property = complexType.FindProperty("Collection");
            Assert.AreEqual(property.Name, "Collection", "FindProperty should return AmbiguousElementBinding since the name of the property is duplicate.");

            var entityType = edmModel.FindType("CollectionAtomic.ComplexTypeE") as IEdmEntityType;
            property = complexType.FindProperty("Collection");
            Assert.AreEqual(property.Name, "Collection", "FindProperty should return AmbiguousElementBinding since the name of the property is duplicate.");
        }

        //[TestMethod, Variation(Id = 42, SkipReason = @"[EdmLib] When the name of an element is changed, the FindType method should fail when it is called with the old name. -- postponed")]
        public void FindMethodsTestForElementsAfterNameChange()
        {
            var model = new EdmModel();
            var complexType = new StubEdmComplexType("MyNamespace", "ComplexType1");
            model.AddElement(complexType);
            complexType.Name = "ComplexType2";

            Assert.IsTrue(model.SchemaElements.Any(n => n.FullName() == "MyNamespace.ComplexType2"), "There should be ComplexType2 type.");
            Assert.IsTrue(!model.SchemaElements.Any(n => n.FullName() == "MyNamespace.ComplexType1"), "There should be no ComplexType1 type.");
            Assert.IsNull(model.FindType("MyNamespace.ComplexType1"), "You should not be able to find the complex type with the old name");
            Assert.AreEqual(model.FindType("MyNamespace.ComplexType2").FullName(), "MyNamespace.ComplexType2", "You should be able to find the complex type with the new name, ComplexType1");
        }

        [TestMethod]
        public void FindMethodTestTermAndFunctionCsdl()
        {
            this.BasicFindMethodsTest(FindMethodsTestModelBuilder.TermAndFunctionCsdl());
        }

        [TestMethod]
        public void FindMethodTestFunctionOverloadingWithDifferentParametersCountCsdls()
        {
            var csdls = FindMethodsTestModelBuilder.FunctionOverloadingWithDifferentParametersCountCsdl();

            FunctionInfo simpleFunction1 = new FunctionInfo()
            {
                Name = "SimpleFunction",
                Parameters = new ParameterInfoList() { { "Person", "[Edm.String Nullable=True]" } }
            };

            FunctionInfo simpleFunction2 = new FunctionInfo()
            {
                Name = "SimpleFunction",
                Parameters = new ParameterInfoList() { { "Person", "[Edm.String Nullable=True]" },
                                                       { "Count", "[Edm.Int32 Nullable=True]" }}
            };

            List<FunctionInfo> functionInfos = new List<FunctionInfo>() { simpleFunction1, simpleFunction2 };

            FunctionOverloadingCheck(csdls, functionInfos, "DefaultNamespace");
        }

        [TestMethod]
        public void FindMethodTestFunctionImportOverloadingWithDifferentParametersCountCsdl()
        {
            var csdls = FindMethodsTestModelBuilder.FunctionImportOverloadingWithDifferentParametersCountCsdl();

            FunctionInfo simpleFunction1 = new FunctionInfo()
            {
                Name = "SimpleFunction",
                Parameters = new ParameterInfoList() { { "Person", "[Edm.Int32 Nullable=True]" } }
            };

            FunctionInfo simpleFunction2 = new FunctionInfo()
            {
                Name = "SimpleFunction",
                Parameters = new ParameterInfoList() { { "Person", "[Edm.Int32 Nullable=True]" },
                                                       { "Count", "[Edm.Int32 Nullable=True]" }}
            };

            List<FunctionInfo> functionInfos = new List<FunctionInfo>() { simpleFunction1, simpleFunction2 };

            FunctionImportOverloadingCheck(csdls, functionInfos, "Container");
        }

        [TestMethod]
        public void FindMethodTestFunctionImportOverloadingWithComplexParameterCsdl()
        {
            var csdls = FindMethodsTestModelBuilder.FunctionImportOverloadingWithComplexParameterCsdl();

            FunctionInfo simpleFunction1 = new FunctionInfo()
            {
                Name = "MyFunction",
                Parameters = new ParameterInfoList() { { "P1", "[Collection([DefaultNamespace.MyComplexType Nullable=True]) Nullable=True]" },
                                                       { "P2", "[DefaultNamespace.MyEntityType Nullable=True]" },
                                                       { "P3", "[Collection([DefaultNamespace.MyEntityType Nullable=True]) Nullable=True]" }, 
                                                       { "P4", "[Edm.Int32 Nullable=True]" }}
            };

            FunctionInfo simpleFunction2 = new FunctionInfo()
            {
                Name = "MyFunction",
                Parameters = new ParameterInfoList() { { "P1", "[Collection([DefaultNamespace.MyComplexType Nullable=True]) Nullable=True]" },
                                                       { "P2", "[DefaultNamespace.MyEntityType Nullable=True]" },
                                                       { "P3b", "[DefaultNamespace.MyComplexType Nullable=True]" }, 
                                                       { "P4", "[Edm.Int32 Nullable=True]" }}
            };

            List<FunctionInfo> functionInfos = new List<FunctionInfo>() { simpleFunction1, simpleFunction2 };

            FunctionImportOverloadingCheck(csdls, functionInfos, "Container");
        }

        [TestMethod]
        public void FindMethodTestFindFunctionImportThatDoesNotExist()
        {
            var csdls = FindMethodsTestModelBuilder.FunctionImportOverloadingWithComplexParameterCsdl();
            var model = this.GetParserResult(csdls);

            var entityContainer = model.FindEntityContainer("Container");
            var operationImports = entityContainer.FindOperationImports("foobaz");
            Assert.IsNotNull(operationImports, "Invalid find function import result.");
            Assert.AreEqual(0, operationImports.Count(), "Invalid function import count.");
        }

        [TestMethod]
        public void FindMethodTestEntitySetWithSingleNavigationCsdl()
        {
            var csdls = FindMethodsTestModelBuilder.EntitySetWithSingleNavigationCsdl();
            var model = this.GetParserResult(csdls);
            this.VerifySemanticValidation(model, new EdmLibTestErrors());

            var personToItem = model.FindEntityType("DefaultNamespace.Person").NavigationProperties().Where(n => n.Name.Equals("ItemPurchased")).FirstOrDefault();
            var itemToPerson = model.FindEntityType("DefaultNamespace.Item").NavigationProperties().Where(n => n.Name.Equals("Purchaser")).FirstOrDefault();

            var buyerSet = model.EntityContainer.FindEntitySet("BuyerSet");
            var itemSet = model.EntityContainer.FindEntitySet("ItemSet");

            FindNavigationTargetCheck(buyerSet, personToItem, itemSet, itemToPerson);
            this.BasicFindMethodsTest(csdls);
        }

        [TestMethod]
        public void FindMethodTestEntitySetWithTwoNavigationCsdl()
        {
            var csdls = FindMethodsTestModelBuilder.EntitySetWithTwoNavigationCsdl();
            var model = this.GetParserResult(csdls);
            this.VerifySemanticValidation(model, new EdmLibTestErrors());

            var personToItem = model.FindEntityType("DefaultNamespace.Person").NavigationProperties().Where(n => n.Name.Equals("ItemPurchased")).FirstOrDefault();
            var itemToPerson = model.FindEntityType("DefaultNamespace.Item").NavigationProperties().Where(n => n.Name.Equals("Purchaser")).FirstOrDefault();

            var ownerToPet = model.FindEntityType("DefaultNamespace.Person").NavigationProperties().Where(n => n.Name.Equals("Pet")).FirstOrDefault();
            var petToOwner = model.FindEntityType("DefaultNamespace.Pet").NavigationProperties().Where(n => n.Name.Equals("Owner")).FirstOrDefault();

            var buyerSet = model.EntityContainer.FindEntitySet("BuyerSet");
            var itemSet = model.EntityContainer.FindEntitySet("ItemSet");
            var petSet = model.EntityContainer.FindEntitySet("PetSet");

            FindNavigationTargetCheck(buyerSet, personToItem, itemSet, itemToPerson);
            FindNavigationTargetCheck(buyerSet, ownerToPet, petSet, petToOwner);
            this.BasicFindMethodsTest(csdls);
        }

        [TestMethod]
        public void FindMethodTestEntitySetRecursiveNavigationCsdl()
        {
            var csdls = FindMethodsTestModelBuilder.EntitySetRecursiveNavigationCsdl();
            var model = this.GetParserResult(csdls);
            this.VerifySemanticValidation(model, new EdmLibTestErrors());

            var personToFriend = model.FindEntityType("DefaultNamespace.Person").NavigationProperties().Where(n => n.Name.Equals("ToFriend")).FirstOrDefault();
            var friendToPerson = model.FindEntityType("DefaultNamespace.Person").NavigationProperties().Where(n => n.Name.Equals("ToPerson")).FirstOrDefault();

            var personSet = model.EntityContainer.FindEntitySet("PersonSet");

            FindRecursiveNavigationTargetCheck(personSet, personToFriend, friendToPerson);
            this.BasicFindMethodsTest(csdls);
        }

        [TestMethod]
        public void FindMethodTestEntitySetNavigationUsedTwiceCsdl()
        {
            var csdls = FindMethodsTestModelBuilder.EntitySetNavigationUsedTwiceCsdl();
            var model = this.GetParserResult(csdls);
            this.VerifySemanticValidation(model, new EdmLibTestErrors());

            var personToItem = model.FindEntityType("DefaultNamespace.Person").NavigationProperties().Where(n => n.Name.Equals("ItemPurchased")).FirstOrDefault();
            var itemToPerson = model.FindEntityType("DefaultNamespace.Item").NavigationProperties().Where(n => n.Name.Equals("Purchaser")).FirstOrDefault();

            var buyerSet = model.EntityContainer.FindEntitySet("BuyerSet");
            var itemSet = model.EntityContainer.FindEntitySet("ItemSet");
            var secondBuyerSet = model.EntityContainer.FindEntitySet("SecondBuyerSet");
            var secondItemSet = model.EntityContainer.FindEntitySet("SecondItemSet");

            FindNavigationTargetCheck(buyerSet, personToItem, itemSet, itemToPerson);
            FindNavigationTargetCheck(secondBuyerSet, personToItem, secondItemSet, itemToPerson);
        }

        [TestMethod]
        public void FindMethodTestFindVocabularyAnnotationWithTermCsdl()
        {
            var csdls = FindMethodsTestModelBuilder.FindVocabularyAnnotationWithTermCsdl();
            var model = this.GetParserResult(csdls);

            var modelVocabulary = model.VocabularyAnnotations;
            Assert.AreEqual(1, modelVocabulary.Count(), "Invalid vocabulary count.");

            var addressTerm = model.FindTerm("AnnotationNamespace.AddressObject");
            var petType = model.FindType("DefaultNamespace.Pet") as IEdmComplexType;
            this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(addressTerm), model.FindVocabularyAnnotations(addressTerm));
            Assert.AreEqual(0, model.FindDeclaredVocabularyAnnotations(addressTerm).Count(), "Invalid annotation count.");
            this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(petType), model.FindVocabularyAnnotations(petType));
            Assert.AreEqual(1, model.FindDeclaredVocabularyAnnotations(petType).Count(), "Invalid annotation count.");

            var valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(addressTerm, addressTerm);
            Assert.AreEqual(0, valueAnnotationsFound.Count(), "Invalid annotation count.");

            valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, "AnnotationNamespace.Person");
            Assert.AreEqual(0, valueAnnotationsFound.Count(), "Invalid annotation count.");

            valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, "AnnotationNamespace.Nonsense");
            Assert.AreEqual(0, valueAnnotationsFound.Count(), "Invalid annotation count.");

            valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, addressTerm);
            Assert.AreEqual(1, valueAnnotationsFound.Count(), "Invalid annotation count.");

            valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, "AnnotationNamespace.AddressObject");
            Assert.AreEqual(1, valueAnnotationsFound.Count(), "Invalid annotation count.");

            var expectedValueAnnotation = petType.VocabularyAnnotations(model).SingleOrDefault();
            Assert.AreSame(expectedValueAnnotation, valueAnnotationsFound.SingleOrDefault(), "Invalid annotation.");
        }
        
        [TestMethod]
        public void FindMethodTestFindVocabularyAnnotationWithEntityTypeCsdl()
        {
            var csdls = FindMethodsTestModelBuilder.FindVocabularyAnnotationWithEntityTypeCsdl();
            var model = this.GetParserResult(csdls);

            var modelVocabulary = model.VocabularyAnnotations;
            Assert.AreEqual(1, modelVocabulary.Count(), "Invalid vocabulary count.");

            var personEntity = model.FindEntityType("AnnotationNamespace.Person");
            var addressTerm = model.FindTerm("AnnotationNamespace.AddressObject");
            var petType = model.FindType("DefaultNamespace.Pet") as IEdmComplexType;
            this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(personEntity), model.FindVocabularyAnnotations(personEntity));
            Assert.AreEqual(0, model.FindDeclaredVocabularyAnnotations(personEntity).Count(), "Invalid annotation count.");
            this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(addressTerm), model.FindVocabularyAnnotations(addressTerm));
            Assert.AreEqual(0, model.FindDeclaredVocabularyAnnotations(addressTerm).Count(), "Invalid annotation count.");
            this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(petType), model.FindVocabularyAnnotations(petType));
            Assert.AreEqual(1, model.FindDeclaredVocabularyAnnotations(petType).Count(), "Invalid annotation count.");

            var valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(addressTerm, addressTerm);
            Assert.AreEqual(0, valueAnnotationsFound.Count(), "Invalid annotation count.");

            valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, addressTerm);
            Assert.AreEqual(0, valueAnnotationsFound.Count(), "Invalid annotation count.");
        }

        [TestMethod]
        public void FindMethodTestFindVocabularyAnnotationWithComplexTypeCsdl()
        {
            var csdls = FindMethodsTestModelBuilder.FindVocabularyAnnotationWithComplexTypeCsdl();
            var model = this.GetParserResult(csdls);

            var modelVocabulary = model.VocabularyAnnotations;
            Assert.AreEqual(1, modelVocabulary.Count(), "Invalid vocabulary count.");

            var addressTerm = model.FindTerm("AnnotationNamespace.AddressObject");
            var petType = model.FindType("DefaultNamespace.Pet") as IEdmComplexType;
            this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(addressTerm), model.FindVocabularyAnnotations(addressTerm));
            this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(petType), model.FindVocabularyAnnotations(petType));

            var valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(addressTerm, addressTerm);
            Assert.AreEqual(0, valueAnnotationsFound.Count(), "Invalid annotation count.");

            valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, addressTerm);
            Assert.AreEqual(0, valueAnnotationsFound.Count(), "Invalid annotation count.");
        }

        [TestMethod]
        public void FindMethodTestFindVocabularyAnnotationWithMultipleTermsCsdl()
        {
            var csdls = FindMethodsTestModelBuilder.FindVocabularyAnnotationWithMultipleTermsCsdl();
            var model = this.GetParserResult(csdls);

            var modelVocabulary = model.VocabularyAnnotations;
            Assert.AreEqual(3, modelVocabulary.Count(), "Invalid vocabulary count.");

            var personEntity = model.FindEntityType("AnnotationNamespace.Person");
            var addressTerm = model.FindTerm("AnnotationNamespace.AddressObject");
            var petType = model.FindType("DefaultNamespace.Pet") as IEdmComplexType;
            this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(personEntity), model.FindVocabularyAnnotations(personEntity));
            Assert.AreEqual(0, model.FindDeclaredVocabularyAnnotations(personEntity).Count(), "Invalid annotation count.");
            this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(addressTerm), model.FindVocabularyAnnotations(addressTerm));
            Assert.AreEqual(0, model.FindDeclaredVocabularyAnnotations(addressTerm).Count(), "Invalid annotation count.");
            this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(petType), model.FindVocabularyAnnotations(petType));
            Assert.AreEqual(3, model.FindDeclaredVocabularyAnnotations(petType).Count(), "Invalid annotation count.");
            var expectedValueAnnotation = modelVocabulary.Where(n => n.Term == addressTerm);

            var valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(addressTerm, addressTerm);
            Assert.AreEqual(0, valueAnnotationsFound.Count(), "Invalid annotation count.");

            valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, addressTerm);
            Assert.AreEqual(2, valueAnnotationsFound.Count(), "Invalid annotation count.");
            Assert.AreSame(expectedValueAnnotation.ElementAt(0), valueAnnotationsFound.ElementAt(0), "Invalid annotation.");
            Assert.AreSame(expectedValueAnnotation.ElementAt(1), valueAnnotationsFound.ElementAt(1), "Invalid annotation.");
        }

        [TestMethod]
        public void FindMethodTestFindVocabularyAnnotationAcrossModelOutOfLineAnnotationCsdl()
        {
            var csdls = FindMethodsTestModelBuilder.FindVocabularyAnnotationAcrossModelOutOfLineAnnotationCsdl();
            var model = this.GetParserResult(csdls);
            Assert.AreEqual(3, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var valueAnnotationType = model.FindType("DefaultNamespace.ValueAnnotationType");
            this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(valueAnnotationType), model.FindVocabularyAnnotations(valueAnnotationType));

            var valueAnnotationsFound = model.FindVocabularyAnnotations(valueAnnotationType);
            Assert.AreEqual(3, valueAnnotationsFound.Count(), "Invalid annotation count.");

            var valueAnnotationFoundCount = valueAnnotationsFound.Where(n => n.Term.Name.Equals("ValueTermInModel")).Count();
            Assert.AreEqual(1, valueAnnotationFoundCount, "Type annotation cannot be found.");

            valueAnnotationFoundCount = valueAnnotationsFound.Where(n => n.Term.Name.Equals("ValueTermOutOfModel")).Count();
            Assert.AreEqual(1, valueAnnotationFoundCount, "Type annotation cannot be found.");

            valueAnnotationFoundCount = valueAnnotationsFound.Where(n => n.Term.Name.Equals("ValueTermDoesNotExist")).Count();
            Assert.AreEqual(1, valueAnnotationFoundCount, "Type annotation cannot be found.");
        }

        [TestMethod]
        public void FindMethodTestFindVocabularyAnnotationAcrossModelInLineAnnotationCsdl()
        {
            var csdls = FindMethodsTestModelBuilder.FindVocabularyAnnotationAcrossModelInLineAnnotationCsdl();
            var model = this.GetParserResult(csdls);
            Assert.AreEqual(3, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var valueAnnotationType = model.FindType("DefaultNamespace.ValueAnnotationType");
            var personType = model.FindType("DefaultNamespace.PersonInMode");
            this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(valueAnnotationType), model.FindVocabularyAnnotations(valueAnnotationType));
            this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(personType), model.FindVocabularyAnnotations(personType));

            var anntationFound = model.FindVocabularyAnnotations(personType);
            Assert.AreEqual(0, anntationFound.Count(), "Invalid annotation count.");

            anntationFound = model.FindVocabularyAnnotations(valueAnnotationType);
            Assert.AreEqual(3, anntationFound.Count(), "Invalid annotation count.");

            var valueAnnotationsFound = model.FindVocabularyAnnotations(valueAnnotationType);
            Assert.AreEqual(3, valueAnnotationsFound.Count(), "Invalid annotation count.");

            var valueAnnotationFoundCount = valueAnnotationsFound.Where(n => n.Term.Name.Equals("ValueTermInModel")).Count();
            Assert.AreEqual(1, valueAnnotationFoundCount, "Type annotation cannot be found.");

            valueAnnotationFoundCount = valueAnnotationsFound.Where(n => n.Term.Name.Equals("ValueTermOutOfModel")).Count();
            Assert.AreEqual(1, valueAnnotationFoundCount, "Type annotation cannot be found.");

            valueAnnotationFoundCount = valueAnnotationsFound.Where(n => n.Term.Name.Equals("ValueTermDoesNotExist")).Count();
            Assert.AreEqual(1, valueAnnotationFoundCount, "Type annotation cannot be found.");
        }

        [TestMethod]
        public void FindMethodTestFindTermSingleCsdl()
        {
            var csdls = FindMethodsTestModelBuilder.FindTermCsdl();
            var model = this.GetParserResult(csdls);

            var valueTermInModel = model.FindTerm("DefaultNamespace.ValueTermInModel");
            Assert.IsNotNull(valueTermInModel, "Invalid term.");
            Assert.AreSame(valueTermInModel, model.FindTerm("DefaultNamespace.ValueTermInModel"), "Invalid term.");

            var ambiguousValueTerm = model.FindTerm("DefaultNamespace.AmbigousValueTerm");
            Assert.IsNotNull(ambiguousValueTerm, "Invalid ambiguous term.");
            Assert.AreEqual(true, ambiguousValueTerm.IsBad(), "Invalid IsBad value.");
            Assert.AreSame(ambiguousValueTerm, model.FindTerm("DefaultNamespace.AmbigousValueTerm"), "Invalid term.");

            var valueTermInOtherModel = model.FindTerm("AnnotationNamespace.ValueTerm");
            Assert.IsNotNull(valueTermInOtherModel, "Invalid term.");
            Assert.AreSame(valueTermInOtherModel, model.FindTerm("AnnotationNamespace.ValueTerm"), "Invalid term.");

            var valueTermDoesNotExist = model.FindTerm("fooNamespace.ValueTerm");
            Assert.IsNull(valueTermDoesNotExist, "Invalid term.");
        }

        [TestMethod]
        public void FindMethodTestFindTermAmbiguousReferences()
        {
            var model = FindMethodsTestModelBuilder.FindTermModel();
            var referencedModel = this.GetParserResult(FindMethodsTestModelBuilder.FindTermCsdl());
            model.AddReferencedModel(referencedModel);

            var valueTermInModel = model.FindTerm("DefaultNamespace.ValueTermInModel");
            Assert.IsNotNull(valueTermInModel, "Invalid term.");
            Assert.AreSame(valueTermInModel, referencedModel.FindTerm("DefaultNamespace.ValueTermInModel"), "Invalid term.");

            var secondValueTermInModel = model.FindTerm("DefaultNamespace.SecondValueTermInModel");
            Assert.IsNotNull(secondValueTermInModel, "Invalid term.");
            Assert.AreSame(secondValueTermInModel, model.FindTerm("DefaultNamespace.SecondValueTermInModel"), "Invalid term.");

            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.AreNotEqual(0, errors.Count(), "Ambiguous error should occur.");

            var ambiguousValueTerm = model.FindTerm("DefaultNamespace.ReferenceAmbigousValueTerm");
            Assert.IsNotNull(ambiguousValueTerm, "Invalid ambiguous term.");
            Assert.AreEqual(true, ambiguousValueTerm.IsBad(), "Invalid IsBad value.");
        }

        [TestMethod]
        public void FindMethodTestFindTypeComplexTypeCsdl()
        {
            var csdls = FindMethodsTestModelBuilder.FindTypeComplexTypeCsdl();
            var model = this.GetParserResult(csdls);

            var simpleType = model.FindType("DefaultNamespace.SimpleType");
            Assert.IsNotNull(simpleType, "Invalid complex type.");
            Assert.AreSame(simpleType, model.FindType("DefaultNamespace.SimpleType"), "Invalid complex type.");

            var ambiguousType = model.FindType("DefaultNamespace.AmbiguousType");
            Assert.IsNotNull(ambiguousType, "Invalid ambiguous complex type.");
            Assert.AreEqual(true, ambiguousType.IsBad(), "Invalid IsBad value.");
            Assert.AreSame(ambiguousType, model.FindType("DefaultNamespace.AmbiguousType"), "Invalid complex type.");

            var simpleTypeInOtherModel = model.FindType("AnnotationNamespace.SimpleType");
            Assert.IsNotNull(simpleTypeInOtherModel, "Invalid complex type.");
            Assert.AreSame(simpleTypeInOtherModel, model.FindType("AnnotationNamespace.SimpleType"), "Invalid complex type.");

            var complexTypeDoesNotExist = model.FindType("fooNamespace.SimpleType");
            Assert.IsNull(complexTypeDoesNotExist, "Invalid complex type.");
        }

        [TestMethod]
        public void FindMethodTestFindTypeEntityTypeCsdl()
        {
            var csdls = FindMethodsTestModelBuilder.FindTypeEntityTypeCsdl();
            var model = this.GetParserResult(csdls);

            var simpleEntity = model.FindType("DefaultNamespace.SimpleEntity");
            Assert.IsNotNull(simpleEntity, "Invalid entity type.");
            Assert.AreSame(simpleEntity, model.FindType("DefaultNamespace.SimpleEntity"), "Invalid entity type.");

            var ambiguousEntity = model.FindType("DefaultNamespace.AmbiguousEntity");
            Assert.IsNotNull(ambiguousEntity, "Invalid ambiguous entity type.");
            Assert.AreEqual(true, ambiguousEntity.IsBad(), "Invalid IsBad value.");
            Assert.AreSame(ambiguousEntity, model.FindType("DefaultNamespace.AmbiguousEntity"), "Invalid entity type.");

            var simpleEntityInOtherModel = model.FindType("AnnotationNamespace.SimpleEntity");
            Assert.IsNotNull(simpleEntityInOtherModel, "Invalid entity type.");
            Assert.AreSame(simpleEntityInOtherModel, model.FindType("AnnotationNamespace.SimpleEntity"), "Invalid entity type.");

            var entityTypeDoesNotExist = model.FindType("fooNamespace.SimpleEntity");
            Assert.IsNull(entityTypeDoesNotExist, "Invalid entity type.");
        }

        [TestMethod]
        public void FindMethodTestFindTypeModel()
        {
            var model = FindMethodsTestModelBuilder.FindTypeModel();
            var referencedEntityTypeModel = this.GetParserResult(FindMethodsTestModelBuilder.FindTypeEntityTypeCsdl());
            var referencedComplexTypeModel = this.GetParserResult(FindMethodsTestModelBuilder.FindTypeComplexTypeCsdl());
            model.AddReferencedModel(referencedEntityTypeModel);
            model.AddReferencedModel(referencedComplexTypeModel);

            var secondSimpleType = model.FindType("DefaultNamespace.SecondSimpleType");
            Assert.IsNotNull(secondSimpleType, "Invalid complex type.");
            Assert.AreSame(secondSimpleType, model.FindType("DefaultNamespace.SecondSimpleType"), "Invalid complex type.");

            var simpleType = model.FindType("DefaultNamespace.SimpleType");
            Assert.IsNotNull(simpleType, "Invalid complex type.");
            Assert.AreSame(simpleType, referencedComplexTypeModel.FindType("DefaultNamespace.SimpleType"), "Invalid complex type.");

            var ambiguousComplexType = model.FindType("DefaultNamespace.ReferenceAmbiguousType");
            Assert.IsNotNull(ambiguousComplexType, "Invalid ambiguous complex type.");
            Assert.AreEqual(true, ambiguousComplexType.IsBad(), "Invalid IsBad value.");

            var simpleTypeInOtherNamespace = model.FindType("AnnotationNamespace.SimpleType");
            Assert.IsNotNull(simpleTypeInOtherNamespace, "Invalid complex type.");
            Assert.AreSame(simpleTypeInOtherNamespace, referencedComplexTypeModel.FindType("AnnotationNamespace.SimpleType"), "Invalid complex type.");

            var secondSimpleEntity = model.FindType("DefaultNamespace.SecondSimpleEntity");
            Assert.IsNotNull(secondSimpleEntity, "Invalid entity type.");
            Assert.AreSame(secondSimpleEntity, model.FindType("DefaultNamespace.SecondSimpleEntity"), "Invalid entity type.");

            var simpleEntity = model.FindType("DefaultNamespace.SimpleEntity");
            Assert.IsNotNull(simpleEntity, "Invalid entity type.");
            Assert.AreSame(simpleEntity, referencedEntityTypeModel.FindType("DefaultNamespace.SimpleEntity"), "Invalid entity type.");

            var ambiguousEntityType = model.FindType("DefaultNamespace.ReferenceAmbiguousEntity");
            Assert.IsNotNull(ambiguousEntityType, "Invalid ambiguous entity type.");
            Assert.AreEqual(true, ambiguousEntityType.IsBad(), "Invalid IsBad value.");

            var simpleEntityInOtherNamespace = model.FindType("AnnotationNamespace.SimpleEntity");
            Assert.IsNotNull(simpleEntityInOtherNamespace, "Invalid entity type.");
            Assert.AreSame(simpleEntityInOtherNamespace, referencedEntityTypeModel.FindType("AnnotationNamespace.SimpleEntity"), "Invalid entity type.");
        }

        [TestMethod]
        public void FindMethodTestFindTypeDefinitionModel()
        {
            var model = FindMethodsTestModelBuilder.FindTypeModel();
            var referencedTypeDefinitionModel = this.GetParserResult(FindMethodsTestModelBuilder.FindTypeTypeDefinitionCsdl());
            model.AddReferencedModel(referencedTypeDefinitionModel);

            var lengthInDefaultNamespace = model.FindType("DefaultNamespace.Length");
            Assert.IsNotNull(lengthInDefaultNamespace, "Invalid type definition.");
            Assert.AreSame(lengthInDefaultNamespace, model.FindType("DefaultNamespace.Length"), "Invalid type definition.");

            var lengthInOtherNamespace = model.FindType("AnnotationNamespace.Length");
            Assert.IsNotNull(lengthInOtherNamespace, "Invalid type definition.");
            Assert.AreSame(lengthInOtherNamespace, referencedTypeDefinitionModel.FindType("AnnotationNamespace.Length"), "Invalid type definition.");
        }

        [TestMethod]
        public void FindMethodTestFindFunctionAcrossModelsCsdl()
        {
            var csdls = FindMethodsTestModelBuilder.FindFunctionAcrossModelsCsdl();
            var model = this.GetParserResult(csdls);

            var simpleFunctions = model.FindOperations("DefaultNamespace.SimpleFunction");
            Assert.AreEqual(1, simpleFunctions.Count(), "Invalid functions count.");
            var simpleFunction = simpleFunctions.SingleOrDefault();
            Assert.IsNotNull(simpleFunction, "Invalid function.");
            Assert.AreSame(simpleFunction, model.FindOperations("DefaultNamespace.SimpleFunction").SingleOrDefault(), "Invalid function.");

            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            List<EdmError> errorsList = errors.ToList();
            Assert.AreEqual(2, errorsList.Count, "Invalid errors count.");
            Assert.AreEqual(EdmErrorCode.DuplicateFunctions, errorsList[0].ErrorCode, "Invalid error code.");
            Assert.AreEqual(EdmErrorCode.DuplicateFunctions, errorsList[1].ErrorCode, "Invalid error code.");

            var ambiguousFunctions = model.FindOperations("DefaultNamespace.AmbiguousFunction");
            Assert.AreEqual(2, ambiguousFunctions.Count(), "Invalid count of functions.");
            Assert.AreSame(ambiguousFunctions.ElementAt(0), model.FindOperations("DefaultNamespace.AmbiguousFunction").ElementAt(0), "Invalid function.");
            Assert.AreSame(ambiguousFunctions.ElementAt(1), model.FindOperations("DefaultNamespace.AmbiguousFunction").ElementAt(1), "Invalid function.");

            var simpleFunctionInOtherModels = model.FindOperations("AnnotationNamespace.SimpleFunction");
            Assert.AreEqual(1, simpleFunctionInOtherModels.Count(), "Invalid functions count.");
            var simpleFunctionInOtherModel = simpleFunctionInOtherModels.SingleOrDefault();
            Assert.IsNotNull(simpleFunctionInOtherModel, "Invalid function.");
            Assert.AreSame(simpleFunctionInOtherModel, model.FindOperations("AnnotationNamespace.SimpleFunction").SingleOrDefault(), "Invalid function.");

            var functionsDoesNotExist = model.FindOperations("fooNamespace.SimpleFunction");
            Assert.AreEqual(0, functionsDoesNotExist.Count(), "Invalid function.");
        }

        [TestMethod]
        public void FindMethodTestFindFunctionAcrossModelsModel()
        {
            var model = FindMethodsTestModelBuilder.FindFunctionAcrossModelsModel();
            var referencedFunctioneModel = this.GetParserResult(FindMethodsTestModelBuilder.FindFunctionAcrossModelsCsdl());
            model.AddReferencedModel(referencedFunctioneModel);

            var simpleFunctions = model.FindOperations("DefaultNamespace.SimpleFunction");
            Assert.AreEqual(1, simpleFunctions.Count(), "Invalid functions count.");
            var simpleFunction = simpleFunctions.SingleOrDefault();
            Assert.IsNotNull(simpleFunction, "Invalid function.");
            Assert.AreSame(simpleFunction, referencedFunctioneModel.FindOperations("DefaultNamespace.SimpleFunction").SingleOrDefault(), "Invalid function.");

            var secondSimpleFunctions = model.FindOperations("DefaultNamespace.SecondSimpleFunction");
            Assert.AreEqual(1, secondSimpleFunctions.Count(), "Invalid functions count.");
            var secondSimpleFunction = secondSimpleFunctions.SingleOrDefault();
            Assert.IsNotNull(secondSimpleFunction, "Invalid function.");
            Assert.AreSame(secondSimpleFunction, model.FindOperations("DefaultNamespace.SecondSimpleFunction").SingleOrDefault(), "Invalid function.");

            var simpleFunctionInOtherNamespaces = model.FindOperations("AnnotationNamespace.SimpleFunction");
            Assert.AreEqual(1, simpleFunctionInOtherNamespaces.Count(), "Invalid functions count.");
            var simpleFunctionInOtherNamespace = simpleFunctionInOtherNamespaces.SingleOrDefault();
            Assert.IsNotNull(simpleFunctionInOtherNamespace, "Invalid function.");
            Assert.AreSame(simpleFunctionInOtherNamespace, referencedFunctioneModel.FindOperations("AnnotationNamespace.SimpleFunction").SingleOrDefault(), "Invalid complex type.");

            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.AreEqual(1, errors.Count(), "Invalid errors count.");
            Assert.AreEqual(EdmErrorCode.AlreadyDefined, errors.First().ErrorCode, "Invalid error code.");

            var referenceAmbiguousFunctions = model.FindOperations("DefaultNamespace.ReferenceAmbiguousFunction");
            Assert.AreEqual(2, referenceAmbiguousFunctions.Count(), "Invalid count of functions.");

            foreach (var referenceAmbiguousFunction in referenceAmbiguousFunctions)
            {
                Assert.IsNotNull(referenceAmbiguousFunction, "Invalid ambiguous function.");
                Assert.IsFalse(referenceAmbiguousFunction.IsBad(), "Invalid is bad.");
            }
        }

        [TestMethod]
        public void FindMethodTestFindEntityContainerCsdl()
        {
            var csdls = FindMethodsTestModelBuilder.FindEntityContainerCsdl();
            var model = this.GetParserResult(csdls);

            var simpleContainer = model.FindEntityContainer("SimpleContainer");
            Assert.IsNotNull(simpleContainer, "Invalid entity container.");
            Assert.AreSame(simpleContainer, model.FindEntityContainer("SimpleContainer"), "Invalid entity container.");
        }

        [TestMethod]
        public void FindMethodTestFindEntityContainerModel()
        {
            var model = FindMethodsTestModelBuilder.FindEntityContainerModel();
            var referencedEntityContainerModel = this.GetParserResult(FindMethodsTestModelBuilder.FindEntityContainerCsdl());
            model.AddReferencedModel(referencedEntityContainerModel);

            var simpleContainer = model.FindEntityContainer("SimpleContainer");
            Assert.IsNotNull(simpleContainer, "Invalid entity container.");
            Assert.AreSame(simpleContainer, referencedEntityContainerModel.FindEntityContainer("SimpleContainer"), "Invalid entity container.");
        }

        [TestMethod]
        public void FindMethodTestWithReferencesInEdmxParser()
        {
            var referencedEntityContainerModel = this.GetParserResult(FindMethodsTestModelBuilder.FindEntityContainerCsdl());

            var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), new IEdmModel[] { referencedEntityContainerModel }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "no errors");

            var simpleContainer = model.FindEntityContainer("SimpleContainer");
            Assert.IsNotNull(simpleContainer, "Invalid entity container.");
            Assert.AreSame(simpleContainer, referencedEntityContainerModel.FindEntityContainer("SimpleContainer"), "Invalid entity container.");
        }

        [TestMethod]
        public void FindMethodTestFindVocabularyAnnotationAcrossModelVocabularyAnnotationModel()
        {
            var model = FindMethodsTestModelBuilder.FindVocabularyAnnotationAcrossModelAnnotationModel();
            var referenceModel = this.GetParserResult(FindMethodsTestModelBuilder.FindVocabularyAnnotationAcrossModelAnnotationCsdl());
            model.AddReferencedModel(referenceModel);

            var containerOne = model.FindEntityContainer("ContainerOne");
            var containerThree = referenceModel.FindEntityContainer("ContainerThree");
            this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(containerOne), model.FindVocabularyAnnotations(containerOne));
            this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(containerThree).Concat(referenceModel.FindDeclaredVocabularyAnnotations(containerThree)), model.FindVocabularyAnnotations(containerThree));

            var termOneVocabAnnotation = model.FindVocabularyAnnotations(containerOne);
            Assert.AreEqual(1, termOneVocabAnnotation.Count(), "Invalid count of vocabulary annotation.");

            var containerOneVocabAnnotation = containerOne.VocabularyAnnotations(model);
            Assert.AreEqual(1, containerOneVocabAnnotation.Count(), "Invalid count of vocabulary annotation.");
            Assert.AreSame(containerOneVocabAnnotation.SingleOrDefault(), termOneVocabAnnotation.SingleOrDefault(), "Invalid vocabulary annotation.");

            var termThreeVocabAnnotation = model.FindVocabularyAnnotations(containerThree);
            Assert.AreEqual(1, termThreeVocabAnnotation.Count(), "Invalid count of vocabulary annotation.");

            var containerThreeVocabAnnotation = containerThree.VocabularyAnnotations(model);
            Assert.AreEqual(1, containerThreeVocabAnnotation.Count(), "Invalid count of vocabulary annotation.");
            Assert.AreSame(containerThreeVocabAnnotation.SingleOrDefault(), termThreeVocabAnnotation.SingleOrDefault(), "Invalid vocabulary annotation.");
        }

        [TestMethod]
        public void FindMethodTestPrimitiveTypes()
        {
            var parsedModel = this.GetParserResult(ModelBuilder.StringWithFacets());
            var constructibleModel = new EdmModel();

            Assert.IsFalse(parsedModel.SchemaElements.Any(n => n.Namespace == EdmCoreModel.Namespace), "The SchemaElement property should not contain primitive types.");
            Assert.IsFalse(constructibleModel.SchemaElements.Any(n => n.Namespace == EdmCoreModel.Namespace), "The SchemaElement property should not contain primitive types.");

            foreach (var primitiveType in EdmCoreModel.Instance.SchemaElements)
            {
                Assert.IsNull(parsedModel.FindDeclaredType(primitiveType.FullName()), "The FindType method should return null for primitive types.");
                Assert.IsNull(constructibleModel.FindDeclaredType(primitiveType.FullName()), "The FindType method should return null for primitive types.");
                Assert.IsNotNull(parsedModel.FindType(primitiveType.FullName()), "The FindType method should not return null for primitive types across models.");
                Assert.IsNotNull(constructibleModel.FindType(primitiveType.FullName()), "The FindType method should not return null for primitive types across models.");
                Assert.AreEqual(EdmCoreModel.Instance.FindDeclaredType(primitiveType.FullName()), primitiveType, "The EdmCore should contain the primitive type definitions");
            }
        }

        [TestMethod]
        public void FindPropertyOnOverriddenDeclaredPropertiesTest()
        {
            var customEdmComplex = new CustomEdmComplexType("", "");
            customEdmComplex.AddStructuralProperty("Property3", EdmCoreModel.Instance.GetInt16(true));

            Assert.IsNotNull(customEdmComplex.FindProperty("Property3"), "The custom edm complex type should have the property, Property3.");
            Assert.AreEqual(
                                customEdmComplex.FindProperty("Property1").Type.Definition,
                                EdmCoreModel.Instance.SchemaElements.Single(n => n.FullName() == "Edm.Int32"),
                                "The custom edm complex type should have the property, Property1."
                             );

            Assert.AreEqual(
                                customEdmComplex.FindProperty("Property2").Type.Definition,
                                EdmCoreModel.Instance.SchemaElements.Single(n => n.FullName() == "Edm.Int32"),
                                "The custom edm complex type should have the property, Property1."
                             );
            Assert.AreEqual(customEdmComplex.DeclaredProperties.Count(), 3, "The custom edm complex type should have the 3 propertes.");
        }

        [TestMethod]
        public void FindVocabularyAnnotationsIncludingInheritedAnnotationsTest()
        {
            Action<IEnumerable<IEdmVocabularyAnnotation>, IEnumerable<string>> checkAnnotationTerms = (annotations, termNames) =>
                {
                    Assert.IsTrue(!annotations.Select(n => n.Term.FullName()).Except(termNames).Any() && annotations.Count() == termNames.Count(), "The actual annotation terms are not correct.");
                };

            var model = this.GetParserResult(FindMethodsTestModelBuilder.FindVocabularyAnnotationsIncludingInheritedAnnotationsCsdl());
            checkAnnotationTerms(
                                    model.FindVocabularyAnnotationsIncludingInheritedAnnotations(model.FindType("DefaultNamespace.Pet")),
                                    new string[] { "AnnotationNamespace.AddressObject", "AnnotationNamespace.Habitation", "AnnotationNamespace.Unknown" }
                                 );

            checkAnnotationTerms(
                                    model.FindVocabularyAnnotationsIncludingInheritedAnnotations(model.FindType("AnnotationNamespace.Animal")),
                                    new string[] { "AnnotationNamespace.Habitation", "AnnotationNamespace.Unknown" }
                                 );

            checkAnnotationTerms(
                                    model.FindVocabularyAnnotationsIncludingInheritedAnnotations(model.FindType("DefaultNamespace.Child")),
                                    new string[] { }
                                 );

            checkAnnotationTerms(
                                    model.FindVocabularyAnnotationsIncludingInheritedAnnotations((IEdmEntityType)((IEdmEntityType)model.FindType("DefaultNamespace.UnresolvedType")).BaseType),
                                    new string[] { "AnnotationNamespace.AddressObject" }
                                 );

            checkAnnotationTerms(
                                    model.FindVocabularyAnnotationsIncludingInheritedAnnotations(model.FindType("AnnotationNamespace.ComplexTypeWithEntityTypeBase")),
                                    new string[] { "AnnotationNamespace.Habitation" }
                                 );
            this.VerifyThrowsException(typeof(ArgumentNullException), () => model.FindVocabularyAnnotationsIncludingInheritedAnnotations(model.FindType("Unknown.Unknown")));
        }


        //[TestMethod, Variation(Id = 201, SkipReason = @"[EdmLib]  FindVocabularyAnnotationsIncludingInheritedAnnotations does not return all the annotations for the cyclic base types -- postponed")]
        public void FindVocabularyAnnotationsIncludingInheritedAnnotationsRecursiveTest()
        {
            Action<IEnumerable<IEdmVocabularyAnnotation>, IEnumerable<string>> checkAnnotationTerms = (annotations, termNames) =>
                {
                    Assert.IsTrue(!annotations.Select(n => n.Term.FullName()).Except(termNames).Any() && annotations.Count() == termNames.Count(), "The actual annotation terms are not correct.");
                };

            var model = this.GetParserResult(FindMethodsTestModelBuilder.FindVocabularyAnnotationsIncludingInheritedAnnotationsCsdl());

            checkAnnotationTerms(
                                    model.FindVocabularyAnnotationsIncludingInheritedAnnotations(model.FindType("AnnotationNamespace.Base")),
                                    new string[] { "AnnotationNamespace.Unknown", "AnnotationNamespace.Habitation", }
                                 );

            checkAnnotationTerms(
                                    model.FindVocabularyAnnotationsIncludingInheritedAnnotations(model.FindType("AnnotationNamespace.Derived")),
                                    new string[] { "AnnotationNamespace.Unknown", "AnnotationNamespace.Habitation", }
                                 );
        }

        //[TestMethod, Variation(Id = 202, SkipReason = @"[EdmLib] FindDirectlyDerivedTypes and FindAllDerivedTypes does not always return the types in referenced models - postponed ")]
        public void FindDerivedTypeReferencedModelTests()
        {
            IEdmModel masterModel = null;
            IEdmModel testModel = null;

            Action<string, Func<IEdmModel, IEdmStructuredType, IEnumerable<IEdmStructuredType>>> verifyFindDerivedTypes = (typeName, findDerivedType) =>
                {
                    this.VerifyFindDerivedTypes(masterModel, testModel, typeName, findDerivedType);
                };

            Func<IEdmModel, IEdmStructuredType, IEnumerable<IEdmStructuredType>> runFindDirectlyDerivedTypes = (edmModel, type) => edmModel.FindDirectlyDerivedTypes(type);
            Func<IEdmModel, IEdmStructuredType, IEnumerable<IEdmStructuredType>> runFindAllDerivedTypes = (edmModel, type) => edmModel.FindDirectlyDerivedTypes(type);

            var csdls = FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes(this.EdmVersion);
            masterModel = this.GetParserResult(csdls);
            var models = new[] { 
                                  this.GetParserResult(new XElement[] { csdls.ElementAt(0) }), 
                                  this.GetParserResult(new XElement[] { csdls.ElementAt(1) }), 
                                  this.GetParserResult(new XElement[] { csdls.ElementAt(2) }) 
                                };

            testModel = this.GetParserResult(new[] { csdls.ElementAt(0) }, models.ElementAt(1), models.ElementAt(2));
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindAllDerivedTypes);
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindDirectlyDerivedTypes);
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindAllDerivedTypes);
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindDirectlyDerivedTypes);

            testModel = this.GetParserResult(new[] { csdls.ElementAt(1) }, models.ElementAt(0), models.ElementAt(2));
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindAllDerivedTypes);
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindDirectlyDerivedTypes);
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindAllDerivedTypes);
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindDirectlyDerivedTypes);

            testModel = this.GetParserResult(new[] { csdls.ElementAt(2) }, models.ElementAt(0), models.ElementAt(1));
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindAllDerivedTypes);
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindDirectlyDerivedTypes);
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindAllDerivedTypes);
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindDirectlyDerivedTypes);

            testModel = this.GetParserResult(new[] { csdls.ElementAt(2) }, this.GetParserResult(new[] { csdls.ElementAt(1) }, models.ElementAt(0)));
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindAllDerivedTypes);
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindDirectlyDerivedTypes);
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindAllDerivedTypes);
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindDirectlyDerivedTypes);
        }

        //[TestMethod, Variation(Id = 203, SkipReason = @"[EdmLib] FindDirectlyDerivedTypes and FindAllDerivedTypes should return an ambiguous type instance when derived types are ambiguous -- postponed")]
        public void FindDerivedTypeWithAmbigousTypesTests()
        {
            var csdls = FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes(this.EdmVersion);
            IEdmModel masterModel = null;
            IEdmModel testModel = null;

            Action<string, Func<IEdmModel, IEdmStructuredType, IEnumerable<IEdmStructuredType>>> verifyFindDerivedTypes = (typeName, findDerivedType) =>
                {
                    this.VerifyFindDerivedTypes(masterModel, testModel, typeName, findDerivedType);
                };

            Func<IEdmModel, IEdmStructuredType, IEnumerable<IEdmStructuredType>> runFindDirectlyDerivedTypes = (edmModel, type) => edmModel.FindDirectlyDerivedTypes(type);
            Func<IEdmModel, IEdmStructuredType, IEnumerable<IEdmStructuredType>> runFindAllDerivedTypes = (edmModel, type) => edmModel.FindDirectlyDerivedTypes(type);

            var models = new[] { 
                                  this.GetParserResult(new [] { csdls.ElementAt(0) }), 
                                  this.GetParserResult(new [] { csdls.ElementAt(1) }), 
                                  this.GetParserResult(new [] { csdls.ElementAt(2) }) 
                                };

            testModel = this.GetParserResult(csdls.Concat(new[] { csdls.ElementAt(1) }));
            this.VerifyThrowsException(typeof(ArgumentNullException), () => verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindAllDerivedTypes));
            this.VerifyThrowsException(typeof(ArgumentNullException), () => verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindDirectlyDerivedTypes));
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindAllDerivedTypes);
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindDirectlyDerivedTypes);

            testModel = this.GetParserResult(new[] { csdls.ElementAt(0) }, models.ElementAt(2), this.GetParserResult(new[] { csdls.ElementAt(1) }, models.ElementAt(2)));
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindAllDerivedTypes);
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindDirectlyDerivedTypes);
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindAllDerivedTypes);
            verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindDirectlyDerivedTypes);
        }

        //[TestMethod, Variation(Id = 204, SkipReason = @"[EdmLib] FindDirectlyDerivedTypes and FindAllDerivedTypes should throw ArgumentNullException instead of NullReferenceException when their parameters are null -- postponed, [EdmLib] FindDirectlyDerivedTypes and FindAllDerivedTypes should return UnresolvedEntityType or UnresolvedComplexType when it could not resolve the derived types. -- postponed")]
        public void FindDerivedTypeReferencedModelWithUnresolvedTypesTests()
        {
            var csdls = FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes(this.EdmVersion);
            var testModel = this.GetParserResult(new[] { csdls.ElementAt(0), csdls.ElementAt(2) });
            this.VerifyThrowsException(typeof(ArgumentNullException), () => testModel.FindAllDerivedTypes(testModel.FindType("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2") as IEdmStructuredType));
            this.VerifyThrowsException(typeof(ArgumentNullException), () => testModel.FindDirectlyDerivedTypes(testModel.FindType("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2") as IEdmStructuredType));

            Assert.AreEqual(EdmErrorCode.BadUnresolvedEntityType, testModel.FindDirectlyDerivedTypes(testModel.FindType("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2") as IEdmStructuredType).Single().Errors().Single(), "Unresolved type should be returned.");
            Assert.AreEqual(EdmErrorCode.BadUnresolvedEntityType, testModel.FindAllDerivedTypes(testModel.FindType("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2") as IEdmStructuredType).Single().Errors().Single(), "Unresolved type should be returned.");
        }

        private void VerifyFindDerivedTypes(IEdmModel masterModel, IEdmModel testModel, string typeName, Func<IEdmModel, IEdmStructuredType, IEnumerable<IEdmStructuredType>> findDerivedType)
        {
            var expectedResults = findDerivedType(masterModel, masterModel.FindType(typeName) as IEdmStructuredType).Select(n => GetFullName(n));
            var actualResults = findDerivedType(testModel, testModel.FindType(typeName) as IEdmStructuredType).Select(n => GetFullName(n));
            Assert.IsTrue(
                                expectedResults.Count() == actualResults.Count()
                                && !expectedResults.Except(actualResults).Any(), "FindDerivedTypes returns unexpected results."
                            );
        }

        private string GetFullName(IEdmStructuredType type)
        {
            return type as IEdmComplexType != null ? ((IEdmComplexType)type).FullName() : type as IEdmEntityType != null ? ((IEdmEntityType)type).FullName() : null;
        }

        private void FindRecursiveNavigationTargetCheck(IEdmEntitySet entity, IEdmNavigationProperty entityToSelf, IEdmNavigationProperty selfToEntity)
        {
            var entityTarget = entity.FindNavigationTarget(entityToSelf);
            Assert.IsNotNull(entityTarget, "Invalid navigation target.");
            Assert.AreEqual(entity.Name, entityTarget.Name, "Invalid targeted set name.");
            Assert.AreSame(entity, entityTarget, "Invalid targeted set.");

            entityTarget = entity.FindNavigationTarget(selfToEntity);
            Assert.IsNotNull(entityTarget, "Invalid navigation target.");
            Assert.AreEqual(entity.Name, entityTarget.Name, "Invalid targeted set name.");
            Assert.AreSame(entity, entityTarget, "Invalid targeted set.");
        }

        private void FindNavigationTargetCheck(IEdmEntitySet entityOne, IEdmNavigationProperty entityOneToEntityTwo, IEdmEntitySet entityTwo, IEdmNavigationProperty entityTwoToEntityOne)
        {
            var entityOneTarget = entityOne.FindNavigationTarget(entityOneToEntityTwo);
            Assert.IsNotNull(entityOneTarget, "Invalid navigation target.");
            Assert.AreEqual(entityTwo.Name, entityOneTarget.Name, "Invalid targeted set name.");
            Assert.AreSame(entityTwo, entityOneTarget, "Invalid targeted set.");

            entityOneTarget = entityOne.FindNavigationTarget(entityTwoToEntityOne);
            Assert.IsTrue(entityOneTarget is IEdmUnknownEntitySet, "Invalid result.");

            var entityTwoTarget = entityTwo.FindNavigationTarget(entityTwoToEntityOne);
            Assert.IsNotNull(entityTwoTarget, "Invalid navigation target.");
            Assert.AreEqual(entityOne.Name, entityTwoTarget.Name, "Invalid targeted set name.");
            Assert.AreSame(entityOne, entityTwoTarget, "Invalid targeted set.");

            entityTwoTarget = entityTwo.FindNavigationTarget(entityOneToEntityTwo);
            Assert.IsTrue(entityTwoTarget is IEdmUnknownEntitySet, "Invalid result.");
        }

        private void FunctionOverloadingCheck(IEnumerable<XElement> csdls, List<FunctionInfo> functionInfos, string functionNamespace)
        {
            var model = this.GetParserResult(csdls);
            this.VerifySemanticValidation(model, new EdmLibTestErrors());

            foreach (FunctionInfo functionInfo in functionInfos)
            {
                var functionsFound = model.FindOperations(functionNamespace + "." + functionInfo.Name);
                var functionFoundCount = GetOperationMatchedCount(functionInfo, functionsFound);
                Assert.AreEqual(1, functionFoundCount, "Invalid count of function found");
            }
        }

        private void FunctionImportOverloadingCheck(IEnumerable<XElement> csdls, List<FunctionInfo> functionInfos, string entityContainerName)
        {
            var model = this.GetParserResult(csdls);
            this.VerifySemanticValidation(model, new EdmLibTestErrors());
            var entityContainer = model.FindEntityContainer(entityContainerName);

            foreach (FunctionInfo functionInfo in functionInfos)
            {
                var operationsFound = entityContainer.FindOperationImports(functionInfo.Name);
                var functionFoundCount = GetOperationMatchedCount(functionInfo, operationsFound);
                Assert.AreEqual(1, functionFoundCount, "Invalid count of function found");
            }
        }

        private static int GetOperationMatchedCount(FunctionInfo operationInfo, IEnumerable<IEdmOperationImport> operationImportsFound)
        {
            var functionFoundCount = 0;

            foreach (var function in operationImportsFound)
            {
                var parameters = function.Operation.Parameters.ToList();

                if (IsOperationParameterMatched(operationInfo, parameters))
                {
                    functionFoundCount++;
                }
            }
            return functionFoundCount;
        }

        private static int GetOperationMatchedCount(FunctionInfo operationInfo, IEnumerable<IEdmOperation> operationsFound)
        {
            var functionFoundCount = 0;

            foreach (var function in operationsFound)
            {
                var parameters = function.Parameters.ToList();

                if (IsOperationParameterMatched(operationInfo, parameters))
                {
                    functionFoundCount++;
                }
            }
            return functionFoundCount;
        }

        private static bool IsOperationParameterMatched(FunctionInfo functionInfo, List<IEdmOperationParameter> parameters)
        {
            var parametersMatched = 0;

            if (functionInfo.Parameters.Count == parameters.Count)
            {
                for (int i = 0; i < functionInfo.Parameters.Count; i++)
                {
                    bool sameParameter = RemoveParameterFacets(functionInfo.Parameters[i].Type.ToString()).Equals(RemoveParameterFacets(parameters[i].Type.ToString()));
                    if (functionInfo.Parameters[i].Name.Equals(parameters[i].Name) && sameParameter)
                    {
                        parametersMatched++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (parametersMatched == functionInfo.Parameters.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CompareVacabulraryAnnoationsVariations(params IEnumerable<IEdmVocabularyAnnotation>[] addressTerm)
        {
            var baseValue = addressTerm.ElementAt(0);
            Assert.IsFalse(addressTerm.Any(n => n.Count() != baseValue.Count() || baseValue.Except(n).Any()), "The results between FindMethod and its declared version should be same.");
        }

        private static string RemoveParameterFacets(string parameter)
        {
            StringBuilder parameterWithoutFacets = new StringBuilder(string.Empty);
            char[] charDelimiter = new char[] { ' ', '(', ')', '[', ']' };
            int delimiterLocation = 0;

            var splitResults = parameter.Split(charDelimiter, StringSplitOptions.None);

            foreach (string split in splitResults)
            {
                delimiterLocation += split.Length + 1;

                if (split.StartsWith("Nullable=")) // Assuming nullable is part of overloading
                {
                    parameterWithoutFacets.Append(" ");
                    parameterWithoutFacets.Append(split.Trim());
                }
                else if (!split.Contains("="))
                {
                    parameterWithoutFacets.Append(split.Trim());
                }

                if (delimiterLocation <= parameter.Length)
                {
                    if (parameter[delimiterLocation - 1].Equals('('))
                    {
                        parameterWithoutFacets.Append('(');
                    }
                    else if (parameter[delimiterLocation - 1].Equals(')'))
                    {
                        parameterWithoutFacets.Append(')');
                    }
                }
            }

            return parameterWithoutFacets.ToString();
        }

        private class FunctionInfo
        {
            public string Name { get; set; }
            public List<ParameterInfo> Parameters { get; set; }
        }

        private class ParameterInfo
        {
            public string Name { get; set; }
            public string Type { get; set; }

            public ParameterInfo(string name, string type)
            {
                this.Name = name;
                this.Type = type;
            }
        }

        private class ParameterInfoList : List<ParameterInfo>
        {
            public void Add(string name, string type)
            {
                if (!(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type)))
                {
                    base.Add(new ParameterInfo(name, type));
                }
            }
        }

        private class CustomEdmComplexType : EdmEntityType
        {
            public CustomEdmComplexType(string ns, string n)
                : base(ns, n)
            {
            }

            /// <summary>
            /// Gets the properties declared immediately within this type.
            /// </summary>
            public override IEnumerable<IEdmProperty> DeclaredProperties
            {
                get
                {
                    if (!base.DeclaredProperties.Any(n => n.Name == "Property1"))
                    {
                        this.AddStructuralProperty("Property1", EdmCoreModel.Instance.GetInt32(false));
                    }
                    if (!base.DeclaredProperties.Any(n => n.Name == "Property2"))
                    {
                        this.AddStructuralProperty("Property2", EdmCoreModel.Instance.GetInt32(false));
                    }
                    return base.DeclaredProperties;
                }
            }
        }
    }
}
