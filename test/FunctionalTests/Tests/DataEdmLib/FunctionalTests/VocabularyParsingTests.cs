//---------------------------------------------------------------------
// <copyright file="VocabularyParsingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace EdmLibTests.FunctionalTests
{
    #if SILVERLIGHT
    using Microsoft.Silverlight.Testing;
#endif
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.OData.Edm.Vocabularies.Community.V1;
    using Microsoft.OData.Edm.Vocabularies.V1;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VocabularyParsingTests : EdmLibTestCaseBase
    {
        public VocabularyParsingTests()
        {
            this.EdmVersion = EdmVersion.V40;
        }

        [TestMethod]
        public void Parsing_Simple_Terms()
        {
            this.PerfomCommonParserTest(VocabularyTestModelBuilder.SimpleTerms());
        }

        [TestMethod]
        public void Parsing_Simple_Term_WithDuplicate()
        {
            this.PerfomCommonParserTest(VocabularyTestModelBuilder.SimpleTermWithDuplicate());
        }

        [TestMethod]
        public void Parsing_Simple_Term_InV10()
        {
            var model = VocabularyTestModelBuilder.SimpleTermInV10();
            var definitionCsdls = new VocabularyDefinitionCsdlGenerator().GenerateDefinitionCsdl(EdmVersion.V40, model);
            IEdmModel parsedModel = this.GetParserResult(definitionCsdls);
            this.VerifyParsedModel(model, parsedModel);
        }

        [TestMethod]
        public void Parsing_Simple_VocabularyAnnotation()
        {
            this.PerfomCommonParserTest(VocabularyTestModelBuilder.SimpleVocabularyAnnotation());
        }

        [TestMethod]
        public void Parsing_Simple_VocabularyAnnotation_WithQualifiers()
        {
            this.PerfomCommonParserTest(VocabularyTestModelBuilder.SimpleVocabularyAnnotationWithQualifiers());
        }

        [TestMethod]
        public void Parsing_Simple_VocabularyAnnotation_Confict()
        {
            this.PerfomCommonParserTest(VocabularyTestModelBuilder.SimpleVocabularyAnnotationConfict());
        }

        [TestMethod]
        public void Parsing_Multiple_VocabularyAnnotations()
        {
            this.PerfomCommonParserTest(VocabularyTestModelBuilder.MultipleVocabularyAnnotations());
        }

        // [EdmLib] vocabulary annotation Parsing does not work on EntityContainer
        [TestMethod]
        public void Parsing_Simple_VocabularyAnnotation_OnContainerAndEntitySet()
        {
            this.PerfomCommonParserTest(VocabularyTestModelBuilder.SimpleVocabularyAnnotationOnContainerAndEntitySet());
        }

        [TestMethod]
        public void Parsing_Structured_VocabularyAnnotation()
        {
            this.PerfomCommonParserTest(VocabularyTestModelBuilder.StructuredVocabularyAnnotation());
        }

        [TestMethod]
        public void Parsing_TermOfStructuredDataType()
        {
            this.PerfomCommonParserTest(VocabularyTestModelBuilder.TermOfStructuredDataType());
        }

        [TestMethod]
        public void Parsing_TypeTermWithNavigation()
        {
            this.PerfomCommonParserTest(VocabularyTestModelBuilder.TypeTermWithNavigation());
        }

        [TestMethod]
        public void Parsing_TypeTermWithInheritance()
        {
            this.PerfomCommonParserTest(VocabularyTestModelBuilder.TypeTermWithInheritance());
        }

        [TestMethod]
        public void Parsing_VocabularyAnnotationWithRecord()
        {
            this.PerfomCommonParserTest(VocabularyTestModelBuilder.VocabularyAnnotationWithRecord());
        }

        [TestMethod]
        public void Parsing_InvalidPropertyVocabularyAnnotation()
        {
            var model = this.GetParserResult(VocabularyTestModelBuilder.InvalidPropertyVocabularyAnnotationCsdl());

            var expectedErrors = new EdmLibTestErrors()
            {
                {6, 10, EdmErrorCode.InvalidInteger}
            };

            this.VerifySemanticValidation(model, expectedErrors);
        }

        private void VerifyParsedModel(IEdmModel expectedModel, IEdmModel parsedModel)
        {
            var comparer = new VocabularyModelComparer();
            var compareErrorMessages = comparer.CompareModels(expectedModel, parsedModel);

            compareErrorMessages.ToList().ForEach(Console.WriteLine);
            Assert.AreEqual(0, compareErrorMessages.Count, "comparision errors");
        }

        private void PerfomCommonParserTest(IEdmModel testModel)
        {
            var csdls = new List<XElement>();
            csdls.AddRange(new VocabularyDefinitionCsdlGenerator().GenerateDefinitionCsdl(this.EdmVersion, testModel));
            csdls.Add(new VocabularyApplicationCsdlGenerator().GenerateApplicationCsdl(this.EdmVersion, testModel));
            IEdmModel parsedModel = this.GetParserResult(csdls);
            this.VerifyParsedModel(testModel, parsedModel);
        }

        [TestMethod]
        public void Parsing_InlineVocabularyAnnotationEntityTypeUsingAlias()
        {
            var inlineValueAnnotationEntityType = VocabularyTestModelBuilder.InlineVocabularyAnnotationEntityTypeUsingAlias();
            var model = this.GetParserResult(inlineValueAnnotationEntityType);

            var stringTerm = model.FindTerm("AnnotationNamespace.StringTerm");
            Assert.IsNotNull(stringTerm, "Invalid term name.");
            Assert.IsTrue(stringTerm.Type.IsString(), "Invalid term type.");

            var carVocabularyAnnotations = model.FindEntityType("DefaultNamespace.Car").VocabularyAnnotations(model);
            Assert.AreEqual(1, carVocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var annotationFound = this.CheckForVocabularyAnnotation(carVocabularyAnnotations, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("foo") });
            Assert.IsTrue(annotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parsing_OutOfLineVocabularyAnnotationEntityType()
        {
            var outOfLineAnnotationEntityType = VocabularyTestModelBuilder.OutOfLineAnnotationEntityType();
            var model = this.GetParserResult(outOfLineAnnotationEntityType);

            var stringTerm = model.FindTerm("AnnotationNamespace.StringTerm");
            Assert.IsNotNull(stringTerm, "Invalid term name.");
            Assert.IsTrue(stringTerm.Type.IsString(), "Invalid term type.");

            var carType = model.FindEntityType("DefaultNamespace.Car");
            var carVocabularyAnnotations = carType.VocabularyAnnotations(model);
            Assert.AreEqual(1, carVocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(carVocabularyAnnotations, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("foo") }, "DefaultNamespace.Car");
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parsing_OutOfLineAnnotationEntityProperty()
        {
            var outOfLineValueAnnotationEntityProperty = VocabularyTestModelBuilder.OutOfLineAnnotationEntityProperty();
            var model = this.GetParserResult(outOfLineValueAnnotationEntityProperty);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(2, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var carEntityType = model.FindEntityType("DefaultNamespace.Car");
            var carVocabularyAnnotations = carEntityType.VocabularyAnnotations(model);
            Assert.AreEqual(0, carVocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var wheelsVocabularyAnnotations = carEntityType.Properties().Where(x => x.Name == "Wheels").SingleOrDefault().VocabularyAnnotations(model);
            Assert.AreEqual(1, wheelsVocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var ownerEntityType = model.FindEntityType("DefaultNamespace.Owner");
            var ownerVocabularyAnnotations = ownerEntityType.VocabularyAnnotations(model);
            Assert.AreEqual(0, ownerVocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var ownerNameVocabularyAnnotations = ownerEntityType.Properties().Where(x => x.Name == "Name").SingleOrDefault().VocabularyAnnotations(model);
            Assert.AreEqual(1, ownerNameVocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(ownerNameVocabularyAnnotations, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("foo") });
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parsing_InlineAnnotationEntityProperty()
        {
            var inlineAnnotationEntityProperty = VocabularyTestModelBuilder.InlineAnnotationEntityProperty();
            var model = this.GetParserResult(inlineAnnotationEntityProperty);

            var carEntityType = model.FindEntityType("DefaultNamespace.Car");
            var carVocabularyAnnotations = carEntityType.VocabularyAnnotations(model);
            Assert.AreEqual(0, carVocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var wheelsVocabularyAnnotations = carEntityType.Properties().Where(x => x.Name.Equals("Wheels")).SingleOrDefault().VocabularyAnnotations(model);
            Assert.AreEqual(1, wheelsVocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(wheelsVocabularyAnnotations, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("foo") });
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");

            var ownerEntityType = model.FindEntityType("DefaultNamespace.Owner");
            var ownerVocabularyAnnotations = ownerEntityType.VocabularyAnnotations(model);
            Assert.AreEqual(0, ownerVocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var ownerIdVocabularyAnnotations = ownerEntityType.Properties().Where(x => x.Name.Equals("Id")).SingleOrDefault().VocabularyAnnotations(model);
            Assert.AreEqual(1, ownerIdVocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

        }

        [TestMethod]
        public void Parsing_OutOfLineAnnotationNavigationProperty()
        {
            var outOfLineAnnotationNavigationProperty = VocabularyTestModelBuilder.OutOfLineAnnotationNavigationProperty();
            var model = this.GetParserResult(outOfLineAnnotationNavigationProperty);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(2, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var ownerEntityType = model.FindEntityType("DefaultNamespace.Owner");
            var ownerVocabularyAnnotations = ownerEntityType.VocabularyAnnotations(model);
            Assert.AreEqual(0, ownerVocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var carNavigationVocabularyAnnotations = ownerEntityType.NavigationProperties().Where(x => x.Name == "Car").SingleOrDefault().VocabularyAnnotations(model);
            Assert.AreEqual(1, carNavigationVocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var carEntityType = model.FindEntityType("DefaultNamespace.Car");
            var carVocabularyAnnotations = carEntityType.VocabularyAnnotations(model);
            Assert.AreEqual(0, carVocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var ownerNavigationVocabularyAnnotations = carEntityType.NavigationProperties().Where(x => x.Name == "Owner").SingleOrDefault().VocabularyAnnotations(model);
            Assert.AreEqual(1, ownerNavigationVocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");
        }

        [TestMethod]
        public void Parsing_InlineAnnotationNavigationProperty()
        {
            var inLineAnnotationNavigationProperty = VocabularyTestModelBuilder.InlineAnnotationNavigationProperty();
            var model = this.GetParserResult(inLineAnnotationNavigationProperty);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(2, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var ownerEntityType = model.FindEntityType("DefaultNamespace.Owner");
            var ownerVocabularyAnnotations = ownerEntityType.VocabularyAnnotations(model);
            Assert.AreEqual(0, ownerVocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var carNavigationVocabularyAnnotations = ownerEntityType.NavigationProperties().Where(x => x.Name == "Car").SingleOrDefault().VocabularyAnnotations(model);
            Assert.AreEqual(1, carNavigationVocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var carEntityType = model.FindEntityType("DefaultNamespace.Car");
            var carVocabularyAnnotations = carEntityType.VocabularyAnnotations(model);
            Assert.AreEqual(0, carVocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var ownerNavigationVocabularyAnnotations = carEntityType.NavigationProperties().Where(x => x.Name == "Owner").SingleOrDefault().VocabularyAnnotations(model);
            Assert.AreEqual(1, ownerNavigationVocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(ownerNavigationVocabularyAnnotations, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("foo") });
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parsing_InlineAnnotationEntityContainer()
        {
            var inLineAnnotationEntityContainer = VocabularyTestModelBuilder.InlineAnnotationEntityContainer();
            var model = this.GetParserResult(inLineAnnotationEntityContainer);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var entityContainerVocabularyAnnotation = model.EntityContainer.VocabularyAnnotations(model);

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(entityContainerVocabularyAnnotation, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("foo") });
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parsing_OutOfLineAnnotationEntityContainer()
        {
            var outOfLineAnnotationEntityProperty = VocabularyTestModelBuilder.OutOfLineAnnotationEntityContainer();
            var model = this.GetParserResult(outOfLineAnnotationEntityProperty);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(2, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var entityContainerVocabularyAnnotation = model.EntityContainer.VocabularyAnnotations(model);

            List<PropertyValue> listOfPeople = new List<PropertyValue> { 
                                                                            new PropertyValue("Joe"), 
                                                                            new PropertyValue("Mary"), 
                                                                            new PropertyValue("Justin") 
                                                                        };
            var valueAnnotationFound = this.CheckForVocabularyAnnotation(entityContainerVocabularyAnnotation, EdmExpressionKind.Collection, listOfPeople);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");

            List<PropertyValue> address = new List<PropertyValue>   { 
                                                                        new PropertyValue("Street", "foo"), 
                                                                        new PropertyValue("City", "bar")
                                                                    };

            valueAnnotationFound = this.CheckForVocabularyAnnotation(entityContainerVocabularyAnnotation, EdmExpressionKind.Record, address);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parsing_InlineAnnotationComplexType()
        {
            var inLineAnnotationComplexType = VocabularyTestModelBuilder.InlineAnnotationComplexType();
            var model = this.GetParserResult(inLineAnnotationComplexType);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var pet = model.SchemaElements.Where(x => x.Name.Equals("Pet")).SingleOrDefault() as IEdmComplexType;
            Assert.IsNotNull(pet, "Invalid complex type.");
            var petVocabularyAnnotation = pet.VocabularyAnnotations(model);
            Assert.AreEqual(1, petVocabularyAnnotation.Count(), "Invalid count of vocabulary annotation.");

            List<PropertyValue> address = new List<PropertyValue>   { 
                                                                        new PropertyValue("Street", "foo"), 
                                                                        new PropertyValue("City", "bar")
                                                                    };

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(petVocabularyAnnotation, EdmExpressionKind.Record, address);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parsing_OutOfLineAnnotationComplexType()
        {
            var outOfLineAnnotationComplexType = VocabularyTestModelBuilder.OutOfLineAnnotationComplexType();
            var model = this.GetParserResult(outOfLineAnnotationComplexType);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var pet = model.SchemaElements.Where(x => x.Name.Equals("Pet")).SingleOrDefault() as IEdmComplexType;
            Assert.IsNotNull(pet, "Invalid complex type.");
            var petVocabularyAnnotation = pet.VocabularyAnnotations(model);
            Assert.AreEqual(1, petVocabularyAnnotation.Count(), "Invalid count of vocabulary annotation.");

            List<PropertyValue> listOfPeople = new List<PropertyValue> { 
                                                                            new PropertyValue("Joe"), 
                                                                            new PropertyValue("Mary"), 
                                                                            new PropertyValue("Justin") 
                                                                        };
            var valueAnnotationFound = this.CheckForVocabularyAnnotation(petVocabularyAnnotation, EdmExpressionKind.Collection, listOfPeople);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parse_InlineAnnotationComplexTypeProperty()
        {
            var inLineAnnotationComplexTypeProperty = VocabularyTestModelBuilder.InlineAnnotationComplexTypeProperty();
            var model = this.GetParserResult(inLineAnnotationComplexTypeProperty);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var pet = model.SchemaElements.Where(x => x.Name.Equals("Pet")).SingleOrDefault() as IEdmComplexType;
            Assert.IsNotNull(pet, "Invalid complex type.");

            var petVocabularyAnnotation = pet.VocabularyAnnotations(model);
            Assert.AreEqual(0, petVocabularyAnnotation.Count(), "Invalid count of vocabulary annotation.");

            var nameVocabularyAnnotation = pet.Properties().Where(x => x.Name.Equals("Name")).FirstOrDefault().VocabularyAnnotations(model);
            Assert.AreEqual(1, nameVocabularyAnnotation.Count(), "Invalid count of vocabulary annotation.");

            List<PropertyValue> listOfPeople = new List<PropertyValue> { 
                                                                            new PropertyValue("Joe"), 
                                                                            new PropertyValue("Mary"), 
                                                                            new PropertyValue("Justin") 
                                                                        };

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(nameVocabularyAnnotation, EdmExpressionKind.Collection, listOfPeople);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parsing_OutOfLineAnnotationComplexTypeProperty()
        {
            var outOfLineAnnotationComplexTypeProperty = VocabularyTestModelBuilder.OutOfLineAnnotationComplexTypeProperty();
            var model = this.GetParserResult(outOfLineAnnotationComplexTypeProperty);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var pet = model.SchemaElements.Where(x => x.Name.Equals("Pet")).SingleOrDefault() as IEdmComplexType;
            Assert.IsNotNull(pet, "Invalid complex type.");

            var petVocabularyAnnotation = pet.VocabularyAnnotations(model);
            Assert.AreEqual(0, petVocabularyAnnotation.Count(), "Invalid count of vocabulary annotation.");

            var ownerIdVocabularyAnnotation = pet.Properties().Where(x => x.Name.Equals("OwnerId")).FirstOrDefault().VocabularyAnnotations(model);
            Assert.AreEqual(1, ownerIdVocabularyAnnotation.Count(), "Invalid count of vocabulary annotation.");

            List<PropertyValue> address = new List<PropertyValue>   { 
                                                                        new PropertyValue("Street", "foo"), 
                                                                        new PropertyValue("City", "bar")
                                                                    };

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(ownerIdVocabularyAnnotation, EdmExpressionKind.Record, address);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parse_InlineAnnotationFunction()
        {
            var inLineAnnotationFunction = VocabularyTestModelBuilder.InlineAnnotationFunction();
            var model = this.GetParserResult(inLineAnnotationFunction);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var peopleCount = model.SchemaElements.Where(x => x.Name.Equals("PeopleCount")).SingleOrDefault() as IEdmOperation;
            Assert.IsNotNull(peopleCount, "Invalid function.");

            var peopleCountVocabularyAnnotation = peopleCount.VocabularyAnnotations(model);
            Assert.AreEqual(1, peopleCountVocabularyAnnotation.Count(), "Invalid count of vocabulary annotation.");

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(peopleCountVocabularyAnnotation, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("3") });
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parsing_OutOfLineAnnotationFunction()
        {
            var outOfLineAnnotationFunction = VocabularyTestModelBuilder.OutOfLineAnnotationFunction();
            var model = this.GetParserResult(outOfLineAnnotationFunction);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var peopleCount = model.SchemaElements.Where(x => x.Name.Equals("PeopleCount")).SingleOrDefault() as IEdmOperation;
            Assert.IsNotNull(peopleCount, "Invalid function.");

            var peopleCountVocabularyAnnotation = peopleCount.VocabularyAnnotations(model);
            Assert.AreEqual(1, peopleCountVocabularyAnnotation.Count(), "Invalid count of vocabulary annotation.");

            List<PropertyValue> listOfAge = new List<PropertyValue> { 
                                                                        new PropertyValue("39"), 
                                                                        new PropertyValue("12"), 
                                                                        new PropertyValue("51") 
                                                                    };

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(peopleCountVocabularyAnnotation, EdmExpressionKind.Collection, listOfAge);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parse_InlineAnnotationOperationParameter()
        {
            var inLineAnnotationOperationParameter = VocabularyTestModelBuilder.InlineAnnotationOperationParameter();
            var model = this.GetParserResult(inLineAnnotationOperationParameter);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var peopleCount = model.SchemaElements.Where(x => x.Name.Equals("PeopleCount")).SingleOrDefault() as IEdmOperation;
            Assert.IsNotNull(peopleCount, "Invalid function.");

            var peopleCountVocabularyAnnotation = peopleCount.VocabularyAnnotations(model);
            Assert.AreEqual(0, peopleCountVocabularyAnnotation.Count(), "Invalid count of vocabulary annotation.");

            var peopleListVocabularyAnnotation = peopleCount.Parameters.Where(x => x.Name.Equals("PeopleList")).SingleOrDefault().VocabularyAnnotations(model);
            Assert.AreEqual(1, peopleListVocabularyAnnotation.Count(), "Invalid count of vocabulary annotation.");

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(peopleListVocabularyAnnotation, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("3") });
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parsing_OutOfLineAnnotationOperationParameter()
        {
            var outOfLineAnnotationOperationParameter = VocabularyTestModelBuilder.OutOfLineAnnotationOperationParameter();
            var model = this.GetParserResult(outOfLineAnnotationOperationParameter);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var peopleCount = model.SchemaElements.Where(x => x.Name.Equals("PeopleCount")).SingleOrDefault() as IEdmOperation;
            Assert.IsNotNull(peopleCount, "Invalid function.");

            var peopleCountVocabularyAnnotation = peopleCount.VocabularyAnnotations(model);
            Assert.AreEqual(0, peopleCountVocabularyAnnotation.Count(), "Invalid count of vocabulary annotation.");

            var peopleListVocabularyAnnotation = peopleCount.Parameters.Where(x => x.Name.Equals("PeopleList")).SingleOrDefault().VocabularyAnnotations(model);
            Assert.AreEqual(1, peopleListVocabularyAnnotation.Count(), "Invalid count of vocabulary annotation.");

            List<PropertyValue> listOfAge = new List<PropertyValue> { 
                                                                        new PropertyValue("39"), 
                                                                        new PropertyValue("12"), 
                                                                        new PropertyValue("51") 
                                                                    };

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(peopleListVocabularyAnnotation, EdmExpressionKind.Collection, listOfAge);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parse_InlineAnnotationFunctionImport()
        {
            var inLineAnnotationFunctionImport = VocabularyTestModelBuilder.InlineAnnotationFunctionImport();
            var model = this.GetParserResult(inLineAnnotationFunctionImport);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var entityContainer = model.EntityContainer;
            Assert.IsNotNull(entityContainer, "Invalid entity container.");
            Assert.AreEqual(0, entityContainer.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            var medianAge = entityContainer.FindOperationImports("MedianAge").SingleOrDefault() as IEdmOperationImport;
            Assert.IsNotNull(medianAge, "Invalid function import name.");
            Assert.AreEqual(1, medianAge.VocabularyAnnotations(model).Count(), "Invalid count of vocabulary annotation.");
            var medianAgeVocabularyAnnotation = medianAge.VocabularyAnnotations(model);

            List<PropertyValue> listOfAge = new List<PropertyValue> { 
                                                                            new PropertyValue("39"), 
                                                                            new PropertyValue("12"), 
                                                                            new PropertyValue("51") 
                                                                        };

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(medianAgeVocabularyAnnotation, EdmExpressionKind.Collection, listOfAge);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parsing_OutOfLineAnnotationFunctionImport()
        {
            var outOfLineAnnotationFunctionImport = VocabularyTestModelBuilder.OutOfLineAnnotationFunctionImport();
            var model = this.GetParserResult(outOfLineAnnotationFunctionImport);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var entityContainer = model.EntityContainer;
            Assert.IsNotNull(entityContainer, "Invalid entity container.");
            Assert.AreEqual(0, entityContainer.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            var medianAge = entityContainer.FindOperationImports("MedianAge").SingleOrDefault() as IEdmOperationImport;
            Assert.IsNotNull(medianAge, "Invalid function import name.");
            Assert.AreEqual(1, medianAge.VocabularyAnnotations(model).Count(), "Invalid count of vocabulary annotation.");
            var medianAgeVocabularyAnnotation = medianAge.VocabularyAnnotations(model);

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(medianAgeVocabularyAnnotation, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("3") });
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parse_InlineAnnotationFunctionImportParameter()
        {
            var inLineAnnotationFunctionImportParameter = VocabularyTestModelBuilder.InlineAnnotationFunctionImportParameter();
            var model = this.GetParserResult(inLineAnnotationFunctionImportParameter);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var entityContainer = model.EntityContainer;
            Assert.IsNotNull(entityContainer, "Invalid entity container.");
            Assert.AreEqual(0, entityContainer.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            var medianAge = entityContainer.FindOperationImports("MedianAge").SingleOrDefault() as IEdmOperationImport;
            Assert.IsNotNull(medianAge, "Invalid function import name.");
            Assert.AreEqual(0, medianAge.VocabularyAnnotations(model).Count(), "Invalid count of vocabulary annotation.");

            var peopleAge = medianAge.Operation.Parameters.Where(x => x.Name.Equals("PeopleAge")).SingleOrDefault() as IEdmOperationParameter;
            Assert.IsNotNull(peopleAge, "Invalid function import name.");
            var peopleAgeVocabularyAnnotation = peopleAge.VocabularyAnnotations(model);
            Assert.AreEqual(1, peopleAgeVocabularyAnnotation.Count(), "Invalid count of vocabulary annotation.");

            List<PropertyValue> listOfAge = new List<PropertyValue> { 
                                                                            new PropertyValue("39"), 
                                                                            new PropertyValue("12"), 
                                                                            new PropertyValue("51") 
                                                                        };

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(peopleAgeVocabularyAnnotation, EdmExpressionKind.Collection, listOfAge);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parsing_OutOfLineAnnotationFunctionImportParameter()
        {
            var outOfLineAnnotationFunctionImportParameter = VocabularyTestModelBuilder.OutOfLineAnnotationFunctionImportParameter();
            var model = this.GetParserResult(outOfLineAnnotationFunctionImportParameter);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var entityContainer = model.EntityContainer;
            Assert.IsNotNull(entityContainer, "Invalid entity container.");
            Assert.AreEqual(0, entityContainer.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            var medianAge = entityContainer.FindOperationImports("MedianAge").SingleOrDefault() as IEdmOperationImport;
            Assert.IsNotNull(medianAge, "Invalid function import name.");
            Assert.AreEqual(0, medianAge.VocabularyAnnotations(model).Count(), "Invalid count of vocabulary annotation.");

            var peopleAge = medianAge.Operation.Parameters.Where(x => x.Name.Equals("PeopleAge")).SingleOrDefault() as IEdmOperationParameter;
            Assert.IsNotNull(peopleAge, "Invalid function import name.");
            var peopleAgeVocabularyAnnotation = peopleAge.VocabularyAnnotations(model);
            Assert.AreEqual(1, peopleAgeVocabularyAnnotation.Count(), "Invalid count of vocabulary annotation.");

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(peopleAgeVocabularyAnnotation, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("3") });
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parsing_InlineAnnotationEntitySet()
        {
            var inLineAnnotationEntitySet = VocabularyTestModelBuilder.InlineAnnotationEntitySet();
            var model = this.GetParserResult(inLineAnnotationEntitySet);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var entityContainer = model.EntityContainer;
            Assert.IsNotNull(entityContainer, "Invalid entity container.");
            Assert.AreEqual(0, entityContainer.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            var petDog = entityContainer.FindEntitySet("PetDog");
            Assert.IsNotNull(petDog, "Invalid entity set name.");
            var petDogVocabularyAnnotation = petDog.VocabularyAnnotations(model);
            Assert.AreEqual(1, petDogVocabularyAnnotation.Count(), "Invalid count of vocabulary annotation.");

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(petDogVocabularyAnnotation, EdmExpressionKind.IntegerConstant, new List<PropertyValue> { new PropertyValue("22") });
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parsing_OutOfLineAnnotationEntitySet()
        {
            var outOfLineAnnotationEntitySet = VocabularyTestModelBuilder.OutOfLineAnnotationEntitySet();
            var model = this.GetParserResult(outOfLineAnnotationEntitySet);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");

            var entityContainer = model.EntityContainer;
            Assert.IsNotNull(entityContainer, "Invalid entity container.");
            Assert.AreEqual(0, entityContainer.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            var petDog = entityContainer.FindEntitySet("PetDog");
            Assert.IsNotNull(petDog, "Invalid entity set name.");
            var petDogVocabularyAnnotation = petDog.VocabularyAnnotations(model);
            Assert.AreEqual(1, petDogVocabularyAnnotation.Count(), "Invalid count of vocabulary annotation.");

            List<PropertyValue> address = new List<PropertyValue>   { 
                                                                        new PropertyValue("Street", "foo"), 
                                                                        new PropertyValue("City", "bar")
                                                                    };

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(petDogVocabularyAnnotation, EdmExpressionKind.Record, address);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parsing_InlineAnnotationEnumType()
        {
            var inLineAnnotationEnumType = VocabularyTestModelBuilder.InlineAnnotationEnumType();
            var model = this.GetParserResult(inLineAnnotationEnumType);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var popularity = model.SchemaElements.Where(x => x.Name.Equals("Popularity")).Single() as IEdmEnumType;
            Assert.IsNotNull(popularity, "Invalid enum type.");

            var popularityVocabularyAnnotation = popularity.VocabularyAnnotations(model);
            Assert.AreEqual(1, popularityVocabularyAnnotation.Count(), "Invalid vocabulary annotation count.");

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(popularityVocabularyAnnotation, EdmExpressionKind.IntegerConstant, new List<PropertyValue> { new PropertyValue("22") });
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parsing_OutOfLineAnnotationEnumType()
        {
            var outOfLineAnnotationEnumType = VocabularyTestModelBuilder.OutOfLineAnnotationEnumType();
            var model = this.GetParserResult(outOfLineAnnotationEnumType);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var popularity = model.SchemaElements.Where(x => x.Name.Equals("Popularity")).Single() as IEdmEnumType;
            Assert.IsNotNull(popularity, "Invalid enum type.");

            var popularityVocabularyAnnotation = popularity.VocabularyAnnotations(model);
            Assert.AreEqual(1, popularityVocabularyAnnotation.Count(), "Invalid vocabulary annotation count.");

            List<PropertyValue> address = new List<PropertyValue>   { 
                                                                        new PropertyValue("Street", "foo"), 
                                                                        new PropertyValue("City", "bar")
                                                                    };

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(popularityVocabularyAnnotation, EdmExpressionKind.Record, address);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parsing_InlineAnnotationEnumMember()
        {
            var model = this.GetParserResult(VocabularyTestModelBuilder.inlineAnnotationEnumMember());
            var valueAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, valueAnnotations.Count(), "Invalid annotation count.");

            //Call FindVocabularyAnnotations to get vocabulary annotations.
            var enumType = model.FindDeclaredType("DefaultNamespace.MyEnumType") as IEdmEnumType;
            var vocabularyAnnotatable = enumType.Members.First();
            var valueAnnotations1 = model.FindVocabularyAnnotations(vocabularyAnnotatable);
            Assert.AreEqual(1, valueAnnotations1.Count(), "Invalid annotation count.");

            var intValue = valueAnnotations1.First().Value as IEdmIntegerConstantExpression;
            Assert.IsNotNull(intValue, "Invalid integer value.");
            Assert.AreEqual(5, intValue.Value, "Invalid integer value.");
        }

        [TestMethod]
        public void Parsing_InlineAnnotationTerm()
        {
            var inLineAnnotationValueTerm = VocabularyTestModelBuilder.InlineAnnotationTerm();
            var model = this.GetParserResult(inLineAnnotationValueTerm);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var valueTerm = model.FindTerm("DefaultNamespace.ValueTerm");
            Assert.IsNotNull(valueTerm, "Invalid term.");

            var valueTermVocabularyAnnotation = valueTerm.VocabularyAnnotations(model);
            Assert.AreEqual(1, valueTermVocabularyAnnotation.Count(), "Invalid vocabulary annotation count.");

            List<PropertyValue> address = new List<PropertyValue>   { 
                                                                        new PropertyValue("Street", "foo"), 
                                                                        new PropertyValue("City", "bar")
                                                                    };

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(valueTermVocabularyAnnotation, EdmExpressionKind.Record, address);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void Parsing_OutOfLineAnnotationTerm()
        {
            var outOfLineAnnotationValueTerm = VocabularyTestModelBuilder.OutOfLineAnnotationTerm();
            var model = this.GetParserResult(outOfLineAnnotationValueTerm);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var valueTerm = model.FindTerm("DefaultNamespace.ValueTerm");
            Assert.IsNotNull(valueTerm, "Invalid term.");

            var valueTermVocabularyAnnotation = valueTerm.VocabularyAnnotations(model);
            Assert.AreEqual(1, valueTermVocabularyAnnotation.Count(), "Invalid vocabulary annotation count.");

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(valueTermVocabularyAnnotation, EdmExpressionKind.IntegerConstant, new List<PropertyValue> { new PropertyValue("22") }, "Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm");
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        //[TestMethod, Variation(Id = 327, SkipReason = @"[EdmLib] Vocabulary annotation within vocabulary annotation can not be parsed by csdlreader -- postponed")]
        public void Parsing_InlineAnnotationInVocabularyAnnotation()
        {
            var inLineAnnotationInValueAnnotation = VocabularyTestModelBuilder.InlineAnnotationInVocabularyAnnotation();
            var model = this.GetParserResult(inLineAnnotationInValueAnnotation);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(2, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var entityContainer = model.FindEntityContainer("Container");
            Assert.IsNotNull(entityContainer, "Invalid entity container.");

            var entityContainerVocabularyAnnotation = entityContainer.VocabularyAnnotations(model);
            Assert.AreEqual(1, entityContainerVocabularyAnnotation.Count(), "Invalid vocabulary annotation count.");

            //To do - get vocabulary annotation from annotation
        }

        [TestMethod]
        public void Parsing_OutOfLineAnnotationInvalidAnnotationTarget()
        {
            var outOfLineAnnotationInvalidAnnotationTarget = VocabularyTestModelBuilder.OutOfLineAnnotationInvalidAnnotationTarget();
            var model = this.GetParserResult(outOfLineAnnotationInvalidAnnotationTarget);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
            var valueAnnotationFound = this.CheckForVocabularyAnnotation(vocabularyAnnotations, EdmExpressionKind.IntegerConstant, new List<PropertyValue> { new PropertyValue("22") }, "BadUnresolvedType:bar.foo");
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");

            var container = model.FindEntityContainer("Container");
            Assert.IsNotNull(container, "Invalid entity container.");
            Assert.AreEqual(0, container.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            var dogType = model.FindEntityType("DefaultNamespace.Dog");
            Assert.IsNotNull(dogType, "Invalid entity type.");
            Assert.AreEqual(0, dogType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            var dogName = dogType.FindProperty("Name");
            Assert.IsNotNull(dogName, "Invalid entity type property.");
            Assert.AreEqual(0, dogName.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            var ageTerm = model.FindTerm("AnnotationNamespace.Age");
            Assert.IsNotNull(ageTerm, "Invalid term.");
            Assert.AreEqual(0, ageTerm.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            var personAddressType = model.FindEntityType("AnnotationNamespace.PersonAddress");
            Assert.IsNotNull(personAddressType, "Invalid term.");
            Assert.AreEqual(0, personAddressType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            var addressType = model.FindType("AnnotationNamespace.Address");
            Assert.IsNotNull(addressType, "Invalid term.");
            Assert.AreEqual(0, addressType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");
        }

        [TestMethod]
        public void Parsing_OutOfLineAnnotationNamespaceAsAnnotationTarget()
        {
            var outOfLineAnnotationNamespaceAsAnnotationTarget = VocabularyTestModelBuilder.OutOfLineAnnotationNamespaceAsAnnotationTarget();
            var model = this.GetParserResult(outOfLineAnnotationNamespaceAsAnnotationTarget);

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            List<PropertyValue> personAddress = new List<PropertyValue>   { 
                                                                        new PropertyValue("Street", "foo"), 
                                                                        new PropertyValue("City", "bar")
                                                                    };

            // BadUnresolvedType:.DefaultNamespace is not good name, but is to test the annotation is there.
            var valueAnnotationFound = this.CheckForVocabularyAnnotation(vocabularyAnnotations, EdmExpressionKind.Record, personAddress, "BadUnresolvedType:.DefaultNamespace");
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");

            var container = model.FindEntityContainer("Container");
            Assert.IsNotNull(container, "Invalid entity container.");
            Assert.AreEqual(0, container.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            var dogType = model.FindEntityType("DefaultNamespace.Dog");
            Assert.IsNotNull(dogType, "Invalid entity type.");
            Assert.AreEqual(0, dogType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            var dogName = dogType.FindProperty("Name");
            Assert.IsNotNull(dogName, "Invalid entity type property.");
            Assert.AreEqual(0, dogName.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");
        }

        [TestMethod]
        public void Parsing_AnnotationTargetToStringValue()
        {
            var outOfLineAnnotationEntityType = VocabularyTestModelBuilder.OutOfLineAnnotationEntityType();
            var entityTypeModel = this.GetParserResult(outOfLineAnnotationEntityType);

            var entityTypeAnnotations = entityTypeModel.FindEntityType("DefaultNamespace.Car").VocabularyAnnotations(entityTypeModel);
            Assert.AreEqual(1, entityTypeAnnotations.Count(), "Invalid annotation count.");

            Assert.AreEqual("DefaultNamespace.Car", entityTypeAnnotations.ElementAt(0).Target.ToString(), "Invalid target ToString.");

            var outOfLineValueAnnotationEntityProperty = VocabularyTestModelBuilder.OutOfLineAnnotationEntityProperty();
            var entityPropertyModel = this.GetParserResult(outOfLineValueAnnotationEntityProperty);

            var entityPropertyAnnotation = entityPropertyModel.FindEntityType("DefaultNamespace.Car").FindProperty("Wheels").VocabularyAnnotations(entityPropertyModel);
            Assert.AreEqual(1, entityPropertyAnnotation.Count(), "Invalid annotation count.");
            Assert.AreEqual("Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsProperty", entityPropertyAnnotation.ElementAt(0).Target.ToString(), "Invalid target ToString.");

            entityPropertyAnnotation = entityPropertyModel.FindEntityType("DefaultNamespace.Owner").FindProperty("Name").VocabularyAnnotations(entityPropertyModel);
            Assert.AreEqual(1, entityPropertyAnnotation.Count(), "Invalid annotation count.");
            Assert.AreEqual("Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsProperty", entityPropertyAnnotation.ElementAt(0).Target.ToString(), "Invalid target ToString.");
        }

        [TestMethod]
        public void VocabularyAnnotation_NoTerm()
        {
            string csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Edm.Int32"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Annotation Qualifier=""q3.q3"" Int=""300"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Qualifier=""q1.q1"" Int=""100"" />
  </Annotations>
</Schema>
";
            List<string> csdls = new List<string>() { csdl };
            IEdmModel edmModel;
            IEnumerable<EdmError> errors;
            var isParsed = SchemaReader.TryParse(csdls.Select(n => XElement.Parse(n).CreateReader()), out edmModel, out errors);
            Assert.IsFalse(isParsed, "SchemaReader.TryParse failed");
            Assert.AreEqual(2, errors.Count(), "Invalid error count.");
            Assert.AreEqual(EdmErrorCode.MissingAttribute, errors.ElementAt(0).ErrorCode, "Invalid error code.");
            Assert.AreEqual(EdmErrorCode.MissingAttribute, errors.ElementAt(1).ErrorCode, "Invalid error code.");
        }

        [TestMethod]
        public void ParsingVocabularyAnnotationWithTrueBooleanValue()
        {
            var csdls = new List<string> { @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Boolean"" />
    <Annotations Target=""Self.Note"">
        <Annotation Term=""My.NS1.Note"">
            <Bool>TRUE</Bool>
        </Annotation>
        <Annotation Term=""My.NS1.Note"">
            <Bool>true</Bool>
        </Annotation>
        <Annotation Term=""My.NS1.Note"">
            <Bool>tRuE</Bool>
        </Annotation>
        <Annotation Term=""My.NS1.Note"" Bool=""TRUE"" />
        <Annotation Term=""My.NS1.Note"" Bool=""true"" />
        <Annotation Term=""My.NS1.Note"" Bool=""TrUe"" />
    </Annotations>
</Schema>" };

            var model = this.GetParserResult(csdls);
            var valueAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(6, valueAnnotations.Count(), "Invalid annotation count.");

            foreach (var valueAnnotation in valueAnnotations)
            {
                var boolValue = valueAnnotation.Value as IEdmBooleanConstantExpression;
                Assert.IsNotNull(boolValue, "Invalid boolean value.");
                Assert.AreEqual(true, boolValue.Value, "Invalid boolean value.");
            }
        }

        [TestMethod]
        public void ParsingVocabularyAnnotationWithFalseBooleanValue()
        {
            var csdls = new List<string> { @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Boolean"" />
    <Annotations Target=""Self.Note"">
        <Annotation Term=""My.NS1.Note"">
            <Bool>FALSE</Bool>
        </Annotation>
        <Annotation Term=""My.NS1.Note"">
            <Bool>false</Bool>
        </Annotation>
        <Annotation Term=""My.NS1.Note"">
            <Bool>fAlSe</Bool>
        </Annotation>
        <Annotation Term=""My.NS1.Note"" Bool=""FALSE"" />
        <Annotation Term=""My.NS1.Note"" Bool=""false"" />
        <Annotation Term=""My.NS1.Note"" Bool=""FaLsE"" />
    </Annotations>
</Schema>" };

            var model = this.GetParserResult(csdls);
            var valueAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(6, valueAnnotations.Count(), "Invalid annotation count.");

            foreach (var valueAnnotation in valueAnnotations)
            {
                var boolValue = valueAnnotation.Value as IEdmBooleanConstantExpression;
                Assert.IsNotNull(boolValue, "Invalid boolean value.");
                Assert.AreEqual(false, boolValue.Value, "Invalid boolean value.");
            }
        }

        [TestMethod]
        public void ConstructibleVocabularyOutOfLineVocabularyAnnotationWithoutExpressionCsdl()
        {
            var outOfLineAnnotationNamespaceAsAnnotationTarget = VocabularyTestModelBuilder.OutOfLineVocabularyAnnotationWithoutExpressionCsdl();
            var model = this.GetParserResult(outOfLineAnnotationNamespaceAsAnnotationTarget);
            var errors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull }
            };

            this.VerifySemanticValidation(model, errors);
        }

        [TestMethod]
        public void ConstructibleVocabularyAddingInlineVocabularyAnnotationToExistingElement()
        {
            EdmModel model = VocabularyTestModelBuilder.InlineAnnotationSimpleModel();

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var container = model.FindEntityContainer("Container") as EdmEntityContainer;
            Assert.IsNotNull(container, "Invalid entity container name.");

            EdmTerm listOfNamesTerm = new EdmTerm("AnnotationNamespace", "ListOfNames", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
            var collectionOfNames = new EdmCollectionExpression(
                new EdmStringConstant("Joe"),
                new EdmStringConstant("Mary"),
                new EdmLabeledExpression("Justin", new EdmStringConstant("Justin")));

            EdmVocabularyAnnotation valueAnnotation = new EdmVocabularyAnnotation(
                container,
                listOfNamesTerm,
                collectionOfNames);
            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(valueAnnotation);

            vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(2, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            List<PropertyValue> listOfNames = new List<PropertyValue>   { 
                                                                            new PropertyValue("Joe"), 
                                                                            new PropertyValue("Mary"),
                                                                            new PropertyValue("Justin")
                                                                        };

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(vocabularyAnnotations, EdmExpressionKind.Collection, listOfNames);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");

            var containerVocabularyAnnotations = container.VocabularyAnnotations(model);
            valueAnnotationFound = this.CheckForVocabularyAnnotation(containerVocabularyAnnotations, EdmExpressionKind.Collection, listOfNames);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void TestCoreOptimisticConcurrencyInlineAnnotation()
        {
            EdmModel model = new EdmModel();

            EdmEntityContainer container = new EdmEntityContainer("DefaultNamespace", "Container");
            EdmEntityType personType = new EdmEntityType("DefaultNamespace", "Person");
            EdmStructuralProperty propertyId = personType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            personType.AddKeys(propertyId);
            IEdmStructuralProperty concurrencyProperty = personType.AddStructuralProperty("Concurrency", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(personType);
            container.AddEntitySet("People", personType);
            model.AddElement(container);
            container.AddEntitySet("Students", personType);

            IEdmEntitySet peopleSet = model.FindDeclaredEntitySet("People");
            model.SetOptimisticConcurrencyAnnotation(peopleSet, new[] { concurrencyProperty });
            model.SetOptimisticConcurrencyAnnotation(peopleSet, new[] { concurrencyProperty });

            IEdmEntitySet studentSet = model.FindDeclaredEntitySet("Students");
            model.SetOptimisticConcurrencyAnnotation(studentSet, new[] { concurrencyProperty });

            IEnumerable<EdmError> errors;
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            model.TryWriteSchema(xw, out errors);
            xw.Flush();
            xw.Close();
            var actual = sw.ToString();

            const string expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Concurrency"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""People"" EntityType=""DefaultNamespace.Person"">
      <Annotation Term=""Org.OData.Core.V1.OptimisticConcurrency"">
        <Collection>
          <PropertyPath>Concurrency</PropertyPath>
        </Collection>
      </Annotation>
    </EntitySet>
    <EntitySet Name=""Students"" EntityType=""DefaultNamespace.Person"">
      <Annotation Term=""Org.OData.Core.V1.OptimisticConcurrency"">
        <Collection>
          <PropertyPath>Concurrency</PropertyPath>
        </Collection>
      </Annotation>
    </EntitySet>
  </EntityContainer>
</Schema>";

            Assert.AreEqual(expected, actual);
        }

        // TODO: Make 'Org.OData.Core.V1" a reserved namespace, and turn 'Core' ot 'Org.OData.Core.V1'
        [TestMethod]
        public void TestCoreDescriptionAnnotation()
        {
            EdmModel model = new EdmModel();

            const string personTypeDescription = "this is person type.";
            const string personTypeLongDescription = "this is person type, but with a longer description.";
            const string overwritePersonDescription = "this is overwritten person type.";
            const string propertyIdDescription = "this is property Id.";
            const string structuralPropertyDescription = "this is structural property.";
            const string stringTermDescription = "this is string term.";
            const string entitySetDescription = "this is entitySet.";
            const string singletonDescription = "this is singleton.";

            EdmEntityContainer container = new EdmEntityContainer("Ns", "Container");
            var personType = new EdmEntityType("Ns", "Person");
            var propertyId = personType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            personType.AddKeys(propertyId);
            var structuralProperty = personType.AddStructuralProperty("Concurrency", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(personType);
            var entitySet = container.AddEntitySet("People", personType);
            var driverSet = container.AddEntitySet("Drivers", personType);

            var stringTerm = new EdmTerm("Ns", "HomeAddress", EdmCoreModel.Instance.GetString(true));
            model.AddElement(stringTerm);

            var singleton = container.AddSingleton("Boss", personType);
            model.AddElement(container);

            model.SetDescriptionAnnotation(personType, personTypeDescription);
            model.SetDescriptionAnnotation(personType, overwritePersonDescription);
            model.SetLongDescriptionAnnotation(personType, personTypeLongDescription);
            model.SetDescriptionAnnotation(structuralProperty, structuralPropertyDescription);
            model.SetDescriptionAnnotation(propertyId, propertyIdDescription);
            model.SetDescriptionAnnotation(stringTerm, stringTermDescription);
            model.SetDescriptionAnnotation(entitySet, entitySetDescription);
            model.SetDescriptionAnnotation(singleton, singletonDescription);

            Assert.AreEqual(overwritePersonDescription, model.GetDescriptionAnnotation(personType));
            Assert.AreEqual(personTypeLongDescription, model.GetLongDescriptionAnnotation(personType));
            Assert.AreEqual(structuralPropertyDescription, model.GetDescriptionAnnotation(structuralProperty));
            Assert.AreEqual(propertyIdDescription, model.GetDescriptionAnnotation(propertyId));
            Assert.AreEqual(stringTermDescription, model.GetDescriptionAnnotation(stringTerm));
            Assert.AreEqual(entitySetDescription, model.GetDescriptionAnnotation(entitySet));
            Assert.AreEqual(singletonDescription, model.GetDescriptionAnnotation(singleton));
            Assert.IsNull(model.GetDescriptionAnnotation(driverSet));

            const string expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Ns"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"">
      <Annotation Term=""Org.OData.Core.V1.Description"" String=""this is property Id."" />
    </Property>
    <Property Name=""Concurrency"" Type=""Edm.Int32"">
      <Annotation Term=""Org.OData.Core.V1.Description"" String=""this is structural property."" />
    </Property>
    <Annotation Term=""Org.OData.Core.V1.Description"" String=""this is overwritten person type."" />
    <Annotation Term=""Org.OData.Core.V1.LongDescription"" String=""this is person type, but with a longer description."" />
  </EntityType>
  <Term Name=""HomeAddress"" Type=""Edm.String"">
    <Annotation Term=""Org.OData.Core.V1.Description"" String=""this is string term."" />
  </Term>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""People"" EntityType=""Ns.Person"">
      <Annotation Term=""Org.OData.Core.V1.Description"" String=""this is entitySet."" />
    </EntitySet>
    <EntitySet Name=""Drivers"" EntityType=""Ns.Person"" />
    <Singleton Name=""Boss"" Type=""Ns.Person"">
      <Annotation Term=""Org.OData.Core.V1.Description"" String=""this is singleton."" />
    </Singleton>
  </EntityContainer>
</Schema>";

            IEnumerable<EdmError> errors;
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            model.TryWriteSchema(xw, out errors);
            xw.Flush();
            xw.Close();
            var actual = sw.ToString();

            Assert.AreEqual(expected, actual);

            IEdmModel parsedModel;
            IEnumerable<EdmError> errs;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(expected)) }, out parsedModel, out errs);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(!errors.Any(), "No errors");

            Assert.AreEqual(null, parsedModel.GetDescriptionAnnotation(personType));
            Assert.AreEqual(null, parsedModel.GetDescriptionAnnotation(structuralProperty));
            Assert.AreEqual(null, parsedModel.GetDescriptionAnnotation(propertyId));
            Assert.AreEqual(null, parsedModel.GetDescriptionAnnotation(stringTerm));
            Assert.AreEqual(null, parsedModel.GetDescriptionAnnotation(entitySet));
            Assert.AreEqual(null, parsedModel.GetDescriptionAnnotation(singleton));

            var parsedPersonType = parsedModel.FindType("Ns.Person") as IEdmEntityType;
            Assert.IsNotNull(parsedPersonType);
            Assert.AreEqual(overwritePersonDescription, parsedModel.GetDescriptionAnnotation(parsedPersonType));
            Assert.AreEqual(personTypeLongDescription, parsedModel.GetLongDescriptionAnnotation(parsedPersonType));
            Assert.AreEqual(structuralPropertyDescription, parsedModel.GetDescriptionAnnotation((parsedPersonType.FindProperty("Concurrency"))));
            Assert.AreEqual(propertyIdDescription, parsedModel.GetDescriptionAnnotation(parsedPersonType.FindProperty("Id")));
            Assert.AreEqual(stringTermDescription, parsedModel.GetDescriptionAnnotation(parsedModel.FindTerm("Ns.HomeAddress")));
            Assert.AreEqual(entitySetDescription, parsedModel.GetDescriptionAnnotation(parsedModel.FindDeclaredEntitySet("People")));
            Assert.AreEqual(singletonDescription, parsedModel.GetDescriptionAnnotation(parsedModel.FindDeclaredSingleton("Boss")));
            Assert.IsNull(parsedModel.GetDescriptionAnnotation(parsedModel.FindDeclaredEntitySet("Drivers")));
        }

        [TestMethod]
        public void TestCapabilitiesChangeTrackingInlineAnnotationOnEntitySet()
        {
            EdmModel model = new EdmModel();

            EdmEntityType deptType = new EdmEntityType("DefaultNamespace", "Dept");
            EdmStructuralProperty deptId = deptType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            deptType.AddKeys(deptId);
            model.AddElement(deptType);

            EdmEntityType personType = new EdmEntityType("DefaultNamespace", "Person");
            EdmStructuralProperty personId = personType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            personType.AddKeys(personId);
            IEdmStructuralProperty ageProperty = personType.AddStructuralProperty("Age", EdmCoreModel.Instance.GetInt32(true));
            IEdmNavigationProperty myDeptProperty = personType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "MyDept", Target = deptType, TargetMultiplicity = EdmMultiplicity.One });
            model.AddElement(personType);

            EdmEntityContainer container = new EdmEntityContainer("DefaultNamespace", "Container");
            container.AddEntitySet("Depts", deptType);
            container.AddEntitySet("People", personType);
            model.AddElement(container);

            IEdmEntitySet peopleSet = model.FindDeclaredEntitySet("People");
            IEdmEntitySet deptSet = model.FindDeclaredEntitySet("Depts");
            model.SetChangeTrackingAnnotation(peopleSet, true, new[] { ageProperty }, new[] { myDeptProperty });
            model.SetChangeTrackingAnnotation(deptSet, false, null, null);

            IEnumerable<EdmError> errors;
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            model.TryWriteSchema(xw, out errors);
            xw.Flush();
            xw.Close();
            var actual = sw.ToString();

            const string expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Dept"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </EntityType>
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Age"" Type=""Edm.Int32"" />
    <NavigationProperty Name=""MyDept"" Type=""DefaultNamespace.Dept"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Depts"" EntityType=""DefaultNamespace.Dept"">
      <Annotation Term=""Org.OData.Capabilities.V1.ChangeTracking"">
        <Record>
          <PropertyValue Property=""Supported"" Bool=""false"" />
          <PropertyValue Property=""FilterableProperties"">
            <Collection />
          </PropertyValue>
          <PropertyValue Property=""ExpandableProperties"">
            <Collection />
          </PropertyValue>
        </Record>
      </Annotation>
    </EntitySet>
    <EntitySet Name=""People"" EntityType=""DefaultNamespace.Person"">
      <Annotation Term=""Org.OData.Capabilities.V1.ChangeTracking"">
        <Record>
          <PropertyValue Property=""Supported"" Bool=""true"" />
          <PropertyValue Property=""FilterableProperties"">
            <Collection>
              <PropertyPath>Age</PropertyPath>
            </Collection>
          </PropertyValue>
          <PropertyValue Property=""ExpandableProperties"">
            <Collection>
              <NavigationPropertyPath>MyDept</NavigationPropertyPath>
            </Collection>
          </PropertyValue>
        </Record>
      </Annotation>
    </EntitySet>
  </EntityContainer>
</Schema>";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ParsingCapabilitiesChangeTrackingInlineAnnotationOnEntitySet()
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Dept"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </EntityType>
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Age"" Type=""Edm.Int32"" />
    <NavigationProperty Name=""MyDept"" Type=""DefaultNamespace.Dept"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Depts"" EntityType=""DefaultNamespace.Dept"">
      <Annotation Term=""Org.OData.Capabilities.V1.ChangeTracking"">
        <Record>
          <PropertyValue Property=""Supported"" Bool=""false"" />
          <PropertyValue Property=""FilterableProperties"">
            <Collection />
          </PropertyValue>
          <PropertyValue Property=""ExpandableProperties"">
            <Collection />
          </PropertyValue>
        </Record>
      </Annotation>
    </EntitySet>
    <EntitySet Name=""People"" EntityType=""DefaultNamespace.Person"">
      <Annotation Term=""Org.OData.Capabilities.V1.ChangeTracking"">
        <Record>
          <PropertyValue Property=""Supported"" Bool=""true"" />
          <PropertyValue Property=""FilterableProperties"">
            <Collection>
              <PropertyPath>Age</PropertyPath>
            </Collection>
          </PropertyValue>
          <PropertyValue Property=""ExpandableProperties"">
            <Collection>
              <NavigationPropertyPath>MyDept</NavigationPropertyPath>
            </Collection>
          </PropertyValue>
        </Record>
      </Annotation>
    </EntitySet>
  </EntityContainer>
</Schema>";

            IEdmModel parsedModel;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out parsedModel, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(!errors.Any(), "No errors");

            // EntitySet: People
            var peopleSet = parsedModel.FindDeclaredEntitySet("People");
            IEdmVocabularyAnnotation annotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(peopleSet, CapabilitiesVocabularyModel.ChangeTrackingTerm).FirstOrDefault();
            Assert.IsNotNull(annotation);
            IEdmRecordExpression record = annotation.Value as IEdmRecordExpression;
            Assert.IsNotNull(record);
            IEdmBooleanConstantExpression supported = record.FindProperty("Supported").Value as IEdmBooleanConstantExpression;
            Assert.IsNotNull(supported);
            Assert.IsTrue(supported.Value);
            IEdmCollectionExpression filterable = record.FindProperty("FilterableProperties").Value as IEdmCollectionExpression;
            Assert.IsNotNull(filterable);
            Assert.AreEqual(((IEdmPathExpression)filterable.Elements.Single()).PathSegments.Single(), "Age");
            IEdmCollectionExpression expandable = record.FindProperty("ExpandableProperties").Value as IEdmCollectionExpression;
            Assert.IsNotNull(expandable);
            Assert.AreEqual(((IEdmPathExpression)expandable.Elements.Single()).PathSegments.Single(), "MyDept");

            // EntitySet: Depts
            var deptsSet = parsedModel.FindDeclaredEntitySet("Depts");
            annotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(deptsSet, CapabilitiesVocabularyModel.ChangeTrackingTerm).FirstOrDefault();
            Assert.IsNotNull(annotation);
            record = annotation.Value as IEdmRecordExpression;
            Assert.IsNotNull(record);
            supported = record.FindProperty("Supported").Value as IEdmBooleanConstantExpression;
            Assert.IsNotNull(supported);
            Assert.IsFalse(supported.Value);
            filterable = record.FindProperty("FilterableProperties").Value as IEdmCollectionExpression;
            Assert.IsNotNull(filterable);
            Assert.IsFalse(filterable.Elements.Any());
            expandable = record.FindProperty("ExpandableProperties").Value as IEdmCollectionExpression;
            Assert.IsNotNull(expandable);
            Assert.IsFalse(expandable.Elements.Any());
        }

        [TestMethod]
        public void TestCapabilitiesChangeTrackingInlineAnnotationOnEntityContainer()
        {
            EdmModel model = new EdmModel();

            EdmEntityType deptType = new EdmEntityType("DefaultNamespace", "Dept");
            IEdmStructuralProperty deptId = deptType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            deptType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            deptType.AddKeys(deptId);
            model.AddElement(deptType);

            EdmEntityType personType = new EdmEntityType("DefaultNamespace", "Person");
            EdmStructuralProperty personId = personType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            personType.AddKeys(personId);
            personType.AddStructuralProperty("Age", EdmCoreModel.Instance.GetInt32(true));
            personType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "MyDept", Target = deptType, TargetMultiplicity = EdmMultiplicity.One });
            deptType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "MyPeople", Target = personType, TargetMultiplicity = EdmMultiplicity.Many });
            model.AddElement(personType);

            EdmEntityContainer container = new EdmEntityContainer("DefaultNamespace", "Container");
            container.AddEntitySet("Depts", deptType);
            container.AddEntitySet("People", personType);
            model.AddElement(container);

            model.SetChangeTrackingAnnotation(container, true);

            IEnumerable<EdmError> errors;
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            model.TryWriteSchema(xw, out errors);
            xw.Flush();
            xw.Close();
            var actual = sw.ToString();

            const string expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Dept"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    <NavigationProperty Name=""MyPeople"" Type=""Collection(DefaultNamespace.Person)"" />
  </EntityType>
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Age"" Type=""Edm.Int32"" />
    <NavigationProperty Name=""MyDept"" Type=""DefaultNamespace.Dept"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Depts"" EntityType=""DefaultNamespace.Dept"" />
    <EntitySet Name=""People"" EntityType=""DefaultNamespace.Person"" />
    <Annotation Term=""Org.OData.Capabilities.V1.ChangeTracking"">
      <Record>
        <PropertyValue Property=""Supported"" Bool=""true"" />
        <PropertyValue Property=""FilterableProperties"">
          <Collection />
        </PropertyValue>
        <PropertyValue Property=""ExpandableProperties"">
          <Collection />
        </PropertyValue>
      </Record>
    </Annotation>
  </EntityContainer>
</Schema>";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ParsingCapabilitiesChangeTrackingInlineAnnotationOnEntityContainer()
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Dept"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    <NavigationProperty Name=""MyPeople"" Type=""Collection(DefaultNamespace.Person)"" />
  </EntityType>
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Age"" Type=""Edm.Int32"" />
    <NavigationProperty Name=""MyDept"" Type=""DefaultNamespace.Dept"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Depts"" EntityType=""DefaultNamespace.Dept"" />
    <EntitySet Name=""People"" EntityType=""DefaultNamespace.Person"" />
    <Annotation Term=""Org.OData.Capabilities.V1.ChangeTracking"">
      <Record>
        <PropertyValue Property=""Supported"" Bool=""true"" />
        <PropertyValue Property=""FilterableProperties"">
          <Collection />
        </PropertyValue>
        <PropertyValue Property=""ExpandableProperties"">
          <Collection />
        </PropertyValue>
      </Record>
    </Annotation>
  </EntityContainer>
</Schema>";

            IEdmModel parsedModel;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out parsedModel, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(!errors.Any(), "No errors");

            var container = parsedModel.FindEntityContainer("Container");
            IEdmVocabularyAnnotation annotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(container, CapabilitiesVocabularyModel.ChangeTrackingTerm).FirstOrDefault();
            Assert.IsNotNull(annotation);
            IEdmRecordExpression record = annotation.Value as IEdmRecordExpression;
            Assert.IsNotNull(record);
            IEdmBooleanConstantExpression supported = record.FindProperty("Supported").Value as IEdmBooleanConstantExpression;
            Assert.IsNotNull(supported);
            Assert.IsTrue(supported.Value);
            IEdmCollectionExpression filterable = record.FindProperty("FilterableProperties").Value as IEdmCollectionExpression;
            Assert.IsNotNull(filterable);
            Assert.IsFalse(filterable.Elements.Any());
            IEdmCollectionExpression expandable = record.FindProperty("ExpandableProperties").Value as IEdmCollectionExpression;
            Assert.IsNotNull(expandable);
            Assert.IsFalse(expandable.Elements.Any());
        }

        [TestMethod]
        public void TestCommunityAlternateKeysInlineAnnotationOnEntityType()
        {
            EdmModel model = new EdmModel();

            var book = new EdmEntityType("ns", "book");
            model.AddElement(book);
            var prop1 = book.AddStructuralProperty("prop1", EdmPrimitiveTypeKind.Int32, false);
            var prop2 = book.AddStructuralProperty("prop2", EdmPrimitiveTypeKind.Int32, false);
            var prop3 = book.AddStructuralProperty("prop3", EdmPrimitiveTypeKind.Int32, false);
            var prop4 = book.AddStructuralProperty("prop4", EdmPrimitiveTypeKind.Int32, false);
            book.AddKeys(prop1);
            model.AddAlternateKeyAnnotation(book, new Dictionary<string, IEdmProperty> { { "s2", prop2 } });
            model.AddAlternateKeyAnnotation(book, new Dictionary<string, IEdmProperty> { { "s3", prop3 }, { "s4", prop4 } });

            IEnumerable<EdmError> errors;
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            model.TryWriteSchema(xw, out errors);
            xw.Flush();
            xw.Close();
            var actual = sw.ToString();

            const string expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""ns"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""book"">
    <Key>
      <PropertyRef Name=""prop1"" />
    </Key>
    <Property Name=""prop1"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""prop2"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""prop3"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""prop4"" Type=""Edm.Int32"" Nullable=""false"" />
    <Annotation Term=""OData.Community.Keys.V1.AlternateKeys"">
      <Collection>
        <Record Type=""OData.Community.Keys.V1.AlternateKey"">
          <PropertyValue Property=""Key"">
            <Collection>
              <Record Type=""OData.Community.Keys.V1.PropertyRef"">
                <PropertyValue Property=""Alias"" String=""s2"" />
                <PropertyValue Property=""Name"" PropertyPath=""prop2"" />
              </Record>
            </Collection>
          </PropertyValue>
        </Record>
        <Record Type=""OData.Community.Keys.V1.AlternateKey"">
          <PropertyValue Property=""Key"">
            <Collection>
              <Record Type=""OData.Community.Keys.V1.PropertyRef"">
                <PropertyValue Property=""Alias"" String=""s3"" />
                <PropertyValue Property=""Name"" PropertyPath=""prop3"" />
              </Record>
              <Record Type=""OData.Community.Keys.V1.PropertyRef"">
                <PropertyValue Property=""Alias"" String=""s4"" />
                <PropertyValue Property=""Name"" PropertyPath=""prop4"" />
              </Record>
            </Collection>
          </PropertyValue>
        </Record>
      </Collection>
    </Annotation>
  </EntityType>
</Schema>";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ParsingCommunityAlternateKeysInlineAnnotationOnEntityType()
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""ns"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""book"">
    <Key>
      <PropertyRef Name=""prop1"" />
    </Key>
    <Property Name=""prop1"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""prop2"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""prop3"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""prop4"" Type=""Edm.Int32"" Nullable=""false"" />
    <Annotation Term=""OData.Community.Keys.V1.AlternateKeys"">
      <Collection>
        <Record Type=""OData.Community.Keys.V1.AlternateKey"">
          <PropertyValue Property=""Key"">
            <Collection>
              <Record Type=""OData.Community.Keys.V1.PropertyRef"">
                <PropertyValue Property=""Alias"" String=""s2"" />
                <PropertyValue Property=""Name"" PropertyPath=""prop2"" />
              </Record>
            </Collection>
          </PropertyValue>
        </Record>
        <Record Type=""OData.Community.Keys.V1.AlternateKey"">
          <PropertyValue Property=""Key"">
            <Collection>
              <Record Type=""OData.Community.Keys.V1.PropertyRef"">
                <PropertyValue Property=""Alias"" String=""s3"" />
                <PropertyValue Property=""Name"" PropertyPath=""prop3"" />
              </Record>
              <Record Type=""OData.Community.Keys.V1.PropertyRef"">
                <PropertyValue Property=""Alias"" String=""s4"" />
                <PropertyValue Property=""Name"" PropertyPath=""prop4"" />
              </Record>
            </Collection>
          </PropertyValue>
        </Record>
      </Collection>
    </Annotation>
  </EntityType>
</Schema>";

            IEdmModel parsedModel;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out parsedModel, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(!errors.Any(), "No errors");

            var bookType = parsedModel.FindDeclaredType("ns.book");
            Assert.IsNotNull(bookType);
            IEdmVocabularyAnnotation annotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(bookType, AlternateKeysVocabularyModel.AlternateKeysTerm).FirstOrDefault();
            Assert.IsNotNull(annotation);
            IEdmCollectionExpression collect = annotation.Value as IEdmCollectionExpression;
            Assert.IsNotNull(collect);
            Assert.AreEqual(2, collect.Elements.Count());

            // #1
            var record = collect.Elements.First() as IEdmRecordExpression;
            Assert.IsNotNull(record);
            IEdmCollectionExpression keyCollection = record.FindProperty("Key").Value as IEdmCollectionExpression;
            Assert.IsNotNull(keyCollection);
            Assert.AreEqual(1, keyCollection.Elements.Count());
            IEdmRecordExpression element = keyCollection.Elements.First() as IEdmRecordExpression;
            Assert.IsNotNull(element);
            IEdmStringConstantExpression alias = element.FindProperty("Alias").Value as IEdmStringConstantExpression;
            Assert.IsNotNull(alias);
            Assert.AreEqual("s2", alias.Value);
            IEdmPathExpression name = element.FindProperty("Name").Value as IEdmPathExpression;
            Assert.IsNotNull(name);
            Assert.AreEqual("prop2", name.PathSegments.Single());

            // #2
            record = collect.Elements.Last() as IEdmRecordExpression;
            Assert.IsNotNull(record);
            keyCollection = record.FindProperty("Key").Value as IEdmCollectionExpression;
            Assert.IsNotNull(keyCollection);
            Assert.AreEqual(2, keyCollection.Elements.Count());

            // #2.1
            element = keyCollection.Elements.First() as IEdmRecordExpression;
            Assert.IsNotNull(element);
            alias = element.FindProperty("Alias").Value as IEdmStringConstantExpression;
            Assert.IsNotNull(alias);
            Assert.AreEqual("s3", alias.Value);
            name = element.FindProperty("Name").Value as IEdmPathExpression;
            Assert.IsNotNull(name);
            Assert.AreEqual("prop3", name.PathSegments.Single());

            // #2.2
            element = keyCollection.Elements.Last() as IEdmRecordExpression;
            Assert.IsNotNull(element);
            alias = element.FindProperty("Alias").Value as IEdmStringConstantExpression;
            Assert.IsNotNull(alias);
            Assert.AreEqual("s4", alias.Value);
            name = element.FindProperty("Name").Value as IEdmPathExpression;
            Assert.IsNotNull(name);
            Assert.AreEqual("prop4", name.PathSegments.Single());
        }

        [TestMethod]
        public void EnumAnnotationSerializationTest()
        {
            EdmModel model = new EdmModel();

            EdmEntityContainer container = new EdmEntityContainer("Ns", "Container");
            var personType = new EdmEntityType("Ns", "Person");
            var propertyId = personType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            personType.AddKeys(propertyId);
            var structuralProperty = personType.AddStructuralProperty("Concurrency", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(personType);
            var entitySet = container.AddEntitySet("People", personType);
            var permissionType = new EdmEnumType("Ns", "Permissions", true);
            var read = permissionType.AddMember("Read", new EdmEnumMemberValue(1));
            var write = permissionType.AddMember("Write", new EdmEnumMemberValue(2));
            var delete = permissionType.AddMember("Delete", new EdmEnumMemberValue(4));
            model.AddElement(permissionType);

            var enumTerm = new EdmTerm("Ns", "Permission", new EdmEnumTypeReference(permissionType, false));
            model.AddElement(enumTerm);
            model.AddElement(container);

            EdmVocabularyAnnotation personIdAnnotation = new EdmVocabularyAnnotation(propertyId, enumTerm, new EdmEnumMemberExpression(read));
            personIdAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(personIdAnnotation);

            EdmVocabularyAnnotation structuralPropertyAnnotation = new EdmVocabularyAnnotation(structuralProperty, enumTerm, new EdmEnumMemberExpression(read, write));
            structuralPropertyAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(structuralPropertyAnnotation);

            EdmVocabularyAnnotation entitySetAnnotation = new EdmVocabularyAnnotation(entitySet, enumTerm, new EdmEnumMemberExpression(read, write, delete));
            structuralPropertyAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(entitySetAnnotation);

            var idAnnotationValue = (model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(propertyId, enumTerm).FirstOrDefault().Value as IEdmEnumMemberExpression).EnumMembers;
            var structuralPropertyAnnotationValue = (model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(structuralProperty, enumTerm).FirstOrDefault().Value as IEdmEnumMemberExpression).EnumMembers;
            var entitySetAnnotationValue = (model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(entitySet, enumTerm).FirstOrDefault().Value as IEdmEnumMemberExpression).EnumMembers;
            var enumTypeAnnotationValue = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(permissionType, enumTerm).FirstOrDefault();

            Assert.AreEqual(1, idAnnotationValue.Count());
            Assert.AreEqual(2, structuralPropertyAnnotationValue.Count());
            Assert.AreEqual(3, entitySetAnnotationValue.Count());
            Assert.IsNull(enumTypeAnnotationValue);

            const string expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Ns"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"">
      <Annotation Term=""Ns.Permission"">
        <EnumMember>Ns.Permissions/Read</EnumMember>
      </Annotation>
    </Property>
    <Property Name=""Concurrency"" Type=""Edm.Int32"">
      <Annotation Term=""Ns.Permission"">
        <EnumMember>Ns.Permissions/Read Ns.Permissions/Write</EnumMember>
      </Annotation>
    </Property>
  </EntityType>
  <EnumType Name=""Permissions"" IsFlags=""true"">
    <Member Name=""Read"" Value=""1"" />
    <Member Name=""Write"" Value=""2"" />
    <Member Name=""Delete"" Value=""4"" />
  </EnumType>
  <Term Name=""Permission"" Type=""Ns.Permissions"" Nullable=""false"" />
  <EntityContainer Name=""Container"">
    <EntitySet Name=""People"" EntityType=""Ns.Person"" />
  </EntityContainer>
  <Annotations Target=""Ns.Container/People"">
    <Annotation Term=""Ns.Permission"">
      <EnumMember>Ns.Permissions/Read Ns.Permissions/Write Ns.Permissions/Delete</EnumMember>
    </Annotation>
  </Annotations>
</Schema>";

            IEnumerable<EdmError> errors;
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            model.TryWriteSchema(xw, out errors);
            xw.Flush();
            xw.Close();
            var actual = sw.ToString();

            Assert.IsTrue(!errors.Any(), "No errors");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EnumAnnotationParsingTest()
        {
            const string edmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Ns"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"">
      <Annotation Term=""Ns.Permission"">
        <EnumMember>Ns.Permissions/Read</EnumMember>
      </Annotation>
    </Property>
    <Property Name=""Concurrency"" Type=""Edm.Int32"">
      <Annotation Term=""Ns.Permission"">
        <EnumMember>Ns.Permissions/Read Ns.Permissions/Write</EnumMember>
      </Annotation>
    </Property>
    <NavigationProperty Name=""Friends"" Type=""Collection(Ns.Person)""/>
  </EntityType>
  <EnumType Name=""Permissions"" IsFlags=""true"">
    <Member Name=""Read"" Value=""1"" />
    <Member Name=""Write"" Value=""2"" />
    <Member Name=""Delete"" Value=""4"" />
  </EnumType>
  <Term Name=""Permission"" Type=""Ns.Permissions"" Nullable=""false"" />
  <EntityContainer Name=""Container"">
    <EntitySet Name=""People"" EntityType=""Ns.Person"">
        <Annotation Term=""Test.NavigationRestrictions"">
            <Record>
                <PropertyValue Property=""Navigability"">
                    <EnumMember>Test.NavigationType/None Test.NavigationType/Single Test.NavigationType/Recursive</EnumMember>
                </PropertyValue>
                <PropertyValue Property=""RestrictedProperties"">
                    <Collection>
                        <Record>
                            <PropertyValue Property=""NavigationProperty"" NavigationPropertyPath=""Friends""/>
                            <PropertyValue Property=""Navigability"" EnumMember=""Test.NavigationType/Recursive"" />
                        </Record>
                    </Collection>
                </PropertyValue>
            </Record>
        </Annotation>
    </EntitySet>
  </EntityContainer>
  <Annotations Target=""Ns.Container/People"">
    <Annotation Term=""Ns.Permission"" EnumMember=""Ns.Permissions/Read Ns.Permissions/Write Ns.Permissions/Delete"" />
  </Annotations>
</Schema>";

            IEdmModel parsedModel;
            IEnumerable<EdmError> errs;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(edmx)) }, out parsedModel, out errs);
            Assert.IsTrue(parsed, "parsed");

            var parsedPersonType = parsedModel.FindType("Ns.Person") as IEdmEntityType;
            var term = parsedModel.FindDeclaredTerm("Ns.Permission");
            var parsedIdAnnotationValue = (parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedPersonType.FindProperty("Id"), term).FirstOrDefault().Value as IEdmEnumMemberExpression).EnumMembers;
            var parsedStructuralAnnotationValue = (parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedPersonType.FindProperty("Concurrency"), term).FirstOrDefault().Value as IEdmEnumMemberExpression).EnumMembers;
            var parsedEntitySetAnnotationValue = (parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedModel.FindDeclaredEntitySet("People"), term).FirstOrDefault().Value as IEdmEnumMemberExpression).EnumMembers;

            Assert.AreEqual(1, parsedIdAnnotationValue.Count());
            Assert.AreEqual(1, parsedIdAnnotationValue.Single().Value.Value);

            Assert.AreEqual(2, parsedStructuralAnnotationValue.Count());
            Assert.AreEqual(1, parsedStructuralAnnotationValue.First().Value.Value);
            Assert.AreEqual(2, parsedStructuralAnnotationValue.Last().Value.Value);

            Assert.AreEqual(3, parsedEntitySetAnnotationValue.Count());
            Assert.AreEqual(1, parsedEntitySetAnnotationValue.First().Value.Value);
            Assert.AreEqual(2, parsedEntitySetAnnotationValue.ElementAt(1).Value.Value);
            Assert.AreEqual(4, parsedEntitySetAnnotationValue.Last().Value.Value);

            var peopleAnnotations = parsedModel.FindVocabularyAnnotations(parsedModel.FindDeclaredEntitySet("People"));
            Assert.AreEqual(2, peopleAnnotations.Count());
            var navigabilityAnnotation = peopleAnnotations.First();
            IEdmRecordExpression navigabilityRecord = (IEdmRecordExpression)navigabilityAnnotation.Value;
            var navigabilityProperty = navigabilityRecord.FindProperty("Navigability");
            var navigabilityEnumExpression = (IEdmEnumMemberExpression)navigabilityProperty.Value;

            Assert.AreEqual(3, navigabilityEnumExpression.EnumMembers.Count());
            Assert.AreEqual("None", navigabilityEnumExpression.EnumMembers.First().Name);
            Assert.AreEqual("Single", navigabilityEnumExpression.EnumMembers.ElementAt(1).Name);
            Assert.AreEqual("Recursive", navigabilityEnumExpression.EnumMembers.ElementAt(2).Name);

            var innerNavigabilityProperty =
                ((IEdmRecordExpression)((IEdmCollectionExpression)navigabilityRecord.FindProperty("RestrictedProperties").Value).Elements
                    .First()).FindProperty("Navigability");

            var innerNavigabilityEnumExpression = (IEdmEnumMemberExpression)innerNavigabilityProperty.Value;

            Assert.AreEqual(1, innerNavigabilityEnumExpression.EnumMembers.Count());
            Assert.AreEqual("Recursive", innerNavigabilityEnumExpression.EnumMembers.First().Name);

        }

        [TestMethod]
        public void EnumAnnotationEvaluationFailedTest()
        {
            const string edmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Ns"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"">
      <Annotation Term=""Ns.Permission"">
        <EnumMember>Ns.Permissions/ReadWrite</EnumMember>
      </Annotation>
    </Property>
    <Property Name=""Concurrency"" Type=""Edm.Int32"">
      <Annotation Term=""Ns.Permission"">
        <EnumMember>Ns.Permissions/Read Ns.Permissions/Modify</EnumMember>
      </Annotation>
    </Property>
  </EntityType>
  <EnumType Name=""Permissions"" IsFlags=""true"">
    <Member Name=""Read"" Value=""1"" />
    <Member Name=""Write"" Value=""2"" />
    <Member Name=""Delete"" Value=""4"" />
  </EnumType>
  <Term Name=""Permission"" Type=""Ns.Permissions"" Nullable=""false"" />
  <EntityContainer Name=""Container"">
    <EntitySet Name=""People"" EntityType=""Ns.Person"" />
  </EntityContainer>
  <Annotations Target=""Ns.Container/People"">
    <Annotation Term=""Ns.Permission"" EnumMember=""Ns.Permissions/Read Ns.Permissions/Write Ns.Permissions/Delete"" />
  </Annotations>
</Schema>";

            IEdmModel parsedModel;
            IEnumerable<EdmError> errs;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(edmx)) }, out parsedModel, out errs);
            Assert.IsTrue(parsed, "parsed");

            var parsedPersonType = parsedModel.FindType("Ns.Person") as IEdmEntityType;
            var term = parsedModel.FindDeclaredTerm("Ns.Permission");
            var parsedIdAnnotationValue = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedPersonType.FindProperty("Id"), term).FirstOrDefault().Value as IEdmEnumMemberExpression;
            var parsedIdAnnotationErrors = parsedIdAnnotationValue.Errors();
            var parsedStructuralAnnotationValue = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedPersonType.FindProperty("Concurrency"), term).FirstOrDefault().Value as IEdmEnumMemberExpression;
            var parsedStructuralAnnotationErrors = parsedStructuralAnnotationValue.Errors();
            var parsedEntitySetAnnotationValue = (parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedModel.FindDeclaredEntitySet("People"), term).FirstOrDefault().Value as IEdmEnumMemberExpression).EnumMembers;

            Assert.IsNull(parsedIdAnnotationValue.EnumMembers);
            Assert.AreEqual("'Ns.Permissions/ReadWrite' is not a valid enum member path.", parsedIdAnnotationErrors.Last().ErrorMessage);

            Assert.IsNull(parsedStructuralAnnotationValue.EnumMembers);
            Assert.AreEqual("'Ns.Permissions/Read Ns.Permissions/Modify' is not a valid enum member path.", parsedStructuralAnnotationErrors.Last().ErrorMessage);

            Assert.AreEqual(3, parsedEntitySetAnnotationValue.Count());
            Assert.AreEqual(1, parsedEntitySetAnnotationValue.First().Value.Value);
            Assert.AreEqual(2, parsedEntitySetAnnotationValue.ElementAt(1).Value.Value);
            Assert.AreEqual(4, parsedEntitySetAnnotationValue.Last().Value.Value);
        }

        [TestMethod]
        public void EnumAnnotationParsingFailedTest()
        {
            const string edmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Ns"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"">
      <Annotation Term=""Ns.Permission"">
        <EnumMember>Ns.Permissions/Read</EnumMember>
      </Annotation>
    </Property>
    <Property Name=""Concurrency"" Type=""Edm.Int32"">
      <Annotation Term=""Ns.Permission"">
        <EnumMember>Ns.Permissions/Read Ns.Color/Write</EnumMember>
      </Annotation>
    </Property>
  </EntityType>
  <EnumType Name=""Permissions"" IsFlags=""true"">
    <Member Name=""Read"" Value=""1"" />
    <Member Name=""Write"" Value=""2"" />
    <Member Name=""Delete"" Value=""4"" />
  </EnumType>
  <Term Name=""Permission"" Type=""Ns.Permissions"" Nullable=""false"" />
  <EntityContainer Name=""Container"">
    <EntitySet Name=""People"" EntityType=""Ns.Person"" />
  </EntityContainer>
  <Annotations Target=""Ns.Container/People"">
    <Annotation Term=""Ns.Permission"" EnumMember=""Ns1.Permissions/Read Ns.Permissions/Write Ns.Permissions/Delete"" />
  </Annotations>
</Schema>";

            IEdmModel parsedModel;
            IEnumerable<EdmError> errs;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(edmx)) }, out parsedModel, out errs);
            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(2, errs.Count());
            Assert.AreEqual("'Ns.Permissions/Read Ns.Color/Write' is not a valid enum member path.", errs.First().ErrorMessage);
            Assert.AreEqual("'Ns1.Permissions/Read Ns.Permissions/Write Ns.Permissions/Delete' is not a valid enum member path.", errs.Last().ErrorMessage);
        }

        [TestMethod]
        public void AnnotationValueAsEnumMemberExpressionSerializationTest()
        {
            EdmModel model = new EdmModel();

            EdmEntityContainer container = new EdmEntityContainer("Ns", "Container");
            var personType = new EdmEntityType("Ns", "Person");
            var propertyId = personType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            personType.AddKeys(propertyId);
            var structuralProperty = personType.AddStructuralProperty("Concurrency", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(personType);
            var entitySet = container.AddEntitySet("People", personType);
            var permissionType = new EdmEnumType("Ns", "Permissions", true);
            var read = permissionType.AddMember("Read", new EdmEnumMemberValue(1));
            var write = permissionType.AddMember("Write", new EdmEnumMemberValue(2));
            var delete = permissionType.AddMember("Delete", new EdmEnumMemberValue(4));
            model.AddElement(permissionType);

            var enumTerm = new EdmTerm("Ns", "Permission", new EdmEnumTypeReference(permissionType, false));
            model.AddElement(enumTerm);
            model.AddElement(container);

            EdmVocabularyAnnotation personIdAnnotation = new EdmVocabularyAnnotation(propertyId, enumTerm, new EdmEnumMemberExpression(read));
            personIdAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(personIdAnnotation);

            EdmVocabularyAnnotation structuralPropertyAnnotation = new EdmVocabularyAnnotation(structuralProperty, enumTerm, new EdmEnumMemberExpression(read, write));
            structuralPropertyAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(structuralPropertyAnnotation);

            EdmVocabularyAnnotation entitySetAnnotation = new EdmVocabularyAnnotation(entitySet, enumTerm, new EdmEnumMemberExpression(delete));
            structuralPropertyAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(entitySetAnnotation);

            var idAnnotationValue = (model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(propertyId, enumTerm).FirstOrDefault().Value as IEdmEnumMemberExpression).EnumMembers.Single();
            var structuralPropertyAnnotationValue = (model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(structuralProperty, enumTerm).FirstOrDefault().Value as IEdmEnumMemberExpression).EnumMembers;
            var entitySetAnnotationValue = (model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(entitySet, enumTerm).FirstOrDefault().Value as IEdmEnumMemberExpression).EnumMembers.Single();
            var enumTypeAnnotationValue = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(permissionType, enumTerm).FirstOrDefault();

            Assert.AreEqual(read, idAnnotationValue);
            Assert.AreEqual(2, structuralPropertyAnnotationValue.Count());
            Assert.AreEqual(delete, entitySetAnnotationValue);
            Assert.IsNull(enumTypeAnnotationValue);

            const string expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Ns"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"">
      <Annotation Term=""Ns.Permission"">
        <EnumMember>Ns.Permissions/Read</EnumMember>
      </Annotation>
    </Property>
    <Property Name=""Concurrency"" Type=""Edm.Int32"">
      <Annotation Term=""Ns.Permission"">
        <EnumMember>Ns.Permissions/Read Ns.Permissions/Write</EnumMember>
      </Annotation>
    </Property>
  </EntityType>
  <EnumType Name=""Permissions"" IsFlags=""true"">
    <Member Name=""Read"" Value=""1"" />
    <Member Name=""Write"" Value=""2"" />
    <Member Name=""Delete"" Value=""4"" />
  </EnumType>
  <Term Name=""Permission"" Type=""Ns.Permissions"" Nullable=""false"" />
  <EntityContainer Name=""Container"">
    <EntitySet Name=""People"" EntityType=""Ns.Person"" />
  </EntityContainer>
  <Annotations Target=""Ns.Container/People"">
    <Annotation Term=""Ns.Permission"">
      <EnumMember>Ns.Permissions/Delete</EnumMember>
    </Annotation>
  </Annotations>
</Schema>";

            IEnumerable<EdmError> errors;
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            model.TryWriteSchema(xw, out errors);
            xw.Flush();
            xw.Close();
            var actual = sw.ToString();

            Assert.IsTrue(!errors.Any(), "No errors");
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void EnumMemberReferenceAnnotationParsingFailedTest()
        {
            const string edmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Ns"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"">
      <Annotation Term=""Ns.Permission"">
        <EnumMemberReference>Ns.Permissions/Read</EnumMemberReference>
      </Annotation>
    </Property>
    <Property Name=""Concurrency"" Type=""Edm.Int32"" />
  </EntityType>
  <EnumType Name=""Permissions"" IsFlags=""true"">
    <Member Name=""Read"" Value=""1"" />
    <Member Name=""Write"" Value=""2"" />
    <Member Name=""Delete"" Value=""4"" />
  </EnumType>
  <Term Name=""Permission"" Type=""Ns.Permissions"" Nullable=""false"" />
  <EntityContainer Name=""Container"">
    <EntitySet Name=""People"" EntityType=""Ns.Person"" />
  </EntityContainer>
  <Annotations Target=""Ns.Container/People"">
    <Annotation Term=""Ns.Permission"" EnumMemberReference=""Ns1.Permissions/Read Ns.Permissions/Write Ns.Permissions/Delete"" />
  </Annotations>
</Schema>";

            IEdmModel parsedModel;
            IEnumerable<EdmError> errs;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(edmx)) }, out parsedModel, out errs);
            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(2, errs.Count());
            Assert.AreEqual("The schema element 'EnumMemberReference' was not expected in the given context.", errs.First().ErrorMessage);
            Assert.AreEqual("The attribute 'EnumMemberReference' was not expected in the given context.", errs.Last().ErrorMessage);
        }

        [TestMethod]
        public void TestCoreIsLanguageDependentAnnotation()
        {
            EdmModel model = new EdmModel();

            EdmEntityContainer container = new EdmEntityContainer("Ns", "Container");
            var personType = new EdmEntityType("Ns", "Person");
            var propertyId = personType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            personType.AddKeys(propertyId);
            var structuralProperty = personType.AddStructuralProperty("Concurrency", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(personType);
            var entitySet = container.AddEntitySet("People", personType);

            var stringTerm = new EdmTerm("Ns", "HomeAddress", EdmCoreModel.Instance.GetString(true));
            model.AddElement(stringTerm);
            model.AddElement(container);

            IEdmTerm term = CoreVocabularyModel.IsLanguageDependentTerm;
            IEdmBooleanConstantExpression trueExpression = new EdmBooleanConstant(true);
            IEdmBooleanConstantExpression falseExpression = new EdmBooleanConstant(false);

            EdmVocabularyAnnotation personIdAnnotation = new EdmVocabularyAnnotation(propertyId, term, trueExpression);
            personIdAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(personIdAnnotation);

            EdmVocabularyAnnotation structuralPropertyAnnotation = new EdmVocabularyAnnotation(structuralProperty, term, falseExpression);
            structuralPropertyAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(structuralPropertyAnnotation);

            EdmVocabularyAnnotation stringTermAnnotation = new EdmVocabularyAnnotation(stringTerm, term, trueExpression);
            stringTermAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(stringTermAnnotation);

            var idAnnotationValue = (model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(propertyId, term).FirstOrDefault().Value as IEdmBooleanConstantExpression).Value;
            var structuralPropertyAnnotationValue = (model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(structuralProperty, term).FirstOrDefault().Value as IEdmBooleanConstantExpression).Value;
            var stringTermAnnotationValue = (model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(stringTerm, term).FirstOrDefault().Value as IEdmBooleanConstantExpression).Value;
            var entitySetAnnotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(entitySet, term).FirstOrDefault();

            Assert.AreEqual(true, idAnnotationValue);
            Assert.AreEqual(false, structuralPropertyAnnotationValue);
            Assert.AreEqual(true, stringTermAnnotationValue);
            Assert.IsNull(entitySetAnnotation);

            const string expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Ns"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"">
      <Annotation Term=""Org.OData.Core.V1.IsLanguageDependent"" Bool=""true"" />
    </Property>
    <Property Name=""Concurrency"" Type=""Edm.Int32"">
      <Annotation Term=""Org.OData.Core.V1.IsLanguageDependent"" Bool=""false"" />
    </Property>
  </EntityType>
  <Term Name=""HomeAddress"" Type=""Edm.String"">
    <Annotation Term=""Org.OData.Core.V1.IsLanguageDependent"" Bool=""true"" />
  </Term>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""People"" EntityType=""Ns.Person"" />
  </EntityContainer>
</Schema>";

            IEnumerable<EdmError> errors;
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            model.TryWriteSchema(xw, out errors);
            xw.Flush();
            xw.Close();
            var actual = sw.ToString();

            Assert.AreEqual(expected, actual);

            IEdmModel parsedModel;
            IEnumerable<EdmError> errs;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(expected)) }, out parsedModel, out errs);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(!errors.Any(), "No errors");

            var idAnnotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(propertyId, term).FirstOrDefault();
            var propertyAnnotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(structuralProperty, term).FirstOrDefault();
            var termAnnotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(stringTerm, term).FirstOrDefault();
            entitySetAnnotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(entitySet, term).FirstOrDefault();

            Assert.AreEqual(null, propertyAnnotation);
            Assert.AreEqual(null, idAnnotation);
            Assert.AreEqual(null, termAnnotation);
            Assert.AreEqual(null, entitySetAnnotation);

            var parsedPersonType = parsedModel.FindType("Ns.Person") as IEdmEntityType;
            idAnnotationValue = (parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedPersonType.FindProperty("Id"), term).FirstOrDefault().Value as IEdmBooleanConstantExpression).Value;
            structuralPropertyAnnotationValue = (parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedPersonType.FindProperty("Concurrency"), term).FirstOrDefault().Value as IEdmBooleanConstantExpression).Value;
            stringTermAnnotationValue = (parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedModel.FindTerm("Ns.HomeAddress"), term).FirstOrDefault().Value as IEdmBooleanConstantExpression).Value;
            entitySetAnnotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedModel.FindDeclaredEntitySet("People"), term).FirstOrDefault();

            Assert.IsNotNull(parsedPersonType);
            Assert.AreEqual(false, structuralPropertyAnnotationValue);
            Assert.AreEqual(true, idAnnotationValue);
            Assert.AreEqual(true, stringTermAnnotationValue);
            Assert.IsNull(entitySetAnnotation);
        }

        [TestMethod]
        public void ConstructibleVocabularyAddingOutOfLineVocabularyAnnotationToExistingElement()
        {
            EdmModel model = VocabularyTestModelBuilder.InlineAnnotationSimpleModel();

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var container = model.FindEntityContainer("Container") as EdmEntityContainer;
            Assert.IsNotNull(container, "Invalid entity container name.");

            var person = model.FindEntityType("AnnotationNamespace.Person") as EdmEntityType;
            Assert.IsNotNull(person, "Invalid entity type.");

            EdmTerm personTerm = new EdmTerm("AnnotationNamespace", "PersonTerm", new EdmEntityTypeReference(person, true));
            EdmRecordExpression recordOfPerson = new EdmRecordExpression(
                new EdmPropertyConstructor("Id", new EdmIntegerConstant(22)),
                new EdmPropertyConstructor("Name", new EdmStringConstant("Johnny")));

            EdmVocabularyAnnotation valueAnnotation = new EdmVocabularyAnnotation(
                container,
                personTerm,
                recordOfPerson);
            model.AddVocabularyAnnotation(valueAnnotation);

            vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(2, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            List<PropertyValue> listOfNames = new List<PropertyValue>   { 
                                                                            new PropertyValue("Id", "22"), 
                                                                            new PropertyValue("Name", "Johnny")
                                                                        };

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(vocabularyAnnotations, EdmExpressionKind.Record, listOfNames);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");

            var containerVocabularyAnnotations = container.VocabularyAnnotations(model);
            valueAnnotationFound = this.CheckForVocabularyAnnotation(containerVocabularyAnnotations, EdmExpressionKind.Record, listOfNames);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void ConstructibleVocabularyAddingVocabularyAnnotationToNewElement()
        {
            EdmModel model = VocabularyTestModelBuilder.InlineAnnotationSimpleModel();

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var container = model.FindEntityContainer("Container") as EdmEntityContainer;
            Assert.IsNotNull(container, "Invalid entity container name.");

            var carType = model.FindEntityType("DefaultNamespace.Car");
            Assert.IsNotNull(carType, "Invalid entity type.");

            EdmEntitySet carSet = container.AddEntitySet("CarSet", carType);

            var person = model.FindEntityType("AnnotationNamespace.Person") as EdmEntityType;
            Assert.IsNotNull(person, "Invalid entity type.");

            EdmTerm personTerm = new EdmTerm("AnnotationNamespace", "PersonTerm", new EdmEntityTypeReference(person, true));
            EdmRecordExpression recordOfPerson = new EdmRecordExpression(
                new EdmPropertyConstructor("Id", new EdmIntegerConstant(22)),
                new EdmPropertyConstructor("Name", new EdmStringConstant("Johnny")));

            EdmVocabularyAnnotation valueAnnotation = new EdmVocabularyAnnotation(
                carSet,
                personTerm,
                recordOfPerson);

            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(valueAnnotation);

            vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(2, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            List<PropertyValue> listOfNames = new List<PropertyValue>   { 
                                                                            new PropertyValue("Id", "22"), 
                                                                            new PropertyValue("Name", "Johnny")
                                                                        };

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(vocabularyAnnotations, EdmExpressionKind.Record, listOfNames);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");

            var containerCarSet = container.EntitySets().Where(x => x.Name.Equals("CarSet")).SingleOrDefault() as EdmEntitySet;
            Assert.IsNotNull(containerCarSet, "Entity set did not get added to container properly.");

            var carSetVocabularAnnotation = containerCarSet.VocabularyAnnotations(model);
            Assert.AreEqual(1, carSetVocabularAnnotation.Count(), "Invalid vocabulary annotation count.");

            valueAnnotationFound = this.CheckForVocabularyAnnotation(carSetVocabularAnnotation, EdmExpressionKind.Record, listOfNames);
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");
        }

        [TestMethod]
        public void ConstructibleVocabularyRemovingAnnotationToExistElement()
        {
            var model = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(VocabularyTestModelBuilder.InlineAnnotationSimpleModel());
            Assert.AreEqual(1, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var carType = model.FindEntityType("DefaultNamespace.Car");
            Assert.IsNotNull(carType, "Invalid entity type.");
            var carWheels = carType.FindProperty("Wheels");
            Assert.IsNotNull(carWheels, "Invalid entity type property.");

            var carWheelsVocabularyAnnotations = carWheels.VocabularyAnnotations(model);
            Assert.AreEqual(1, carWheelsVocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var valueAnnotation = carWheelsVocabularyAnnotations.ElementAt(0);
            model.RemoveVocabularyAnnotation(valueAnnotation);
            Assert.AreEqual(0, carWheelsVocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
            Assert.AreEqual(0, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
        }

        [TestMethod]
        public void ConstructibleVocabularyRemovingVocabularyAnnotationToNewElement()
        {
            var model = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(VocabularyTestModelBuilder.InlineAnnotationSimpleModel());
            Assert.AreEqual(1, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var carType = model.FindEntityType("DefaultNamespace.Car");
            Assert.IsNotNull(carType, "Invalid entity type.");
            Assert.AreEqual(0, carType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            EdmTerm hiddenName = new EdmTerm("AnnotationNamespace", "HiddenName", EdmCoreModel.Instance.GetString(true));
            model.WrappedModel.AddElement(hiddenName);

            EdmVocabularyAnnotation valueAnnotation = new EdmVocabularyAnnotation(
                carType,
                hiddenName,
                new EdmStringConstant("Gray"));

            model.WrappedModel.AddVocabularyAnnotation(valueAnnotation);
            Assert.AreEqual(2, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
            Assert.AreEqual(1, carType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(carType.VocabularyAnnotations(model), EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("Gray") });
            Assert.IsTrue(valueAnnotationFound, "Annotation cannot be found.");

            model.RemoveVocabularyAnnotation(valueAnnotation);
            Assert.AreEqual(1, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
            Assert.AreEqual(0, carType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");
        }

        [TestMethod]
        public void ConstructibleVocabularyAddingVocabularyAnnotationAndDeleteTargetedElement()
        {
            var model = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(VocabularyTestModelBuilder.InlineAnnotationSimpleModel());

            var vocabularyAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var container = model.FindEntityContainer("Container") as EdmEntityContainer;
            Assert.IsNotNull(container, "Invalid entity container name.");

            var stringTerm = model.FindTerm("AnnotationNamespace.StringTerm") as EdmTerm;
            Assert.IsNotNull(stringTerm, "Invalid term.");

            EdmVocabularyAnnotation valueAnnotation = new EdmVocabularyAnnotation(
                container,
                stringTerm,
                new EdmStringConstant("foo"));
            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.WrappedModel.AddVocabularyAnnotation(valueAnnotation);
            Assert.AreEqual(2, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(vocabularyAnnotations, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("foo") });
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");

            var containerVocabularyAnnotations = container.VocabularyAnnotations(model);
            valueAnnotationFound = this.CheckForVocabularyAnnotation(containerVocabularyAnnotations, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("foo") });
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");

            model.RemoveElement(container);
            model.FindDeclaredVocabularyAnnotations(container).ToList().ForEach(a => model.RemoveVocabularyAnnotation(a));

            Assert.AreEqual(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
        }

        [TestMethod]
        public void ConstructibleVocabularyEditingVocabularyAnnotationToExistElement()
        {
            EdmModel model = VocabularyTestModelBuilder.InlineAnnotationSimpleModel();
            Assert.AreEqual(1, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var carType = model.FindEntityType("DefaultNamespace.Car");
            Assert.IsNotNull(carType, "Invalid entity type.");
            var carWheels = carType.FindProperty("Wheels");
            Assert.IsNotNull(carWheels, "Invalid entity type property.");

            var carWheelsVocabularyAnnotations = carWheels.VocabularyAnnotations(model);
            Assert.AreEqual(1, carWheelsVocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(carWheelsVocabularyAnnotations, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("foo") });
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");

            var valueAnnotation = carWheelsVocabularyAnnotations.ElementAt(0) as FunctionalTests.MutableVocabularyAnnotation;
            Assert.IsNotNull(valueAnnotation, "Invalid annotation.");

            Assert.AreEqual(0, carType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            valueAnnotation.Target = carType;
            valueAnnotation.Value = new EdmStringConstant("bar");
            model.AddVocabularyAnnotation(valueAnnotation);

            Assert.AreEqual(1, carWheels.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");
            Assert.AreEqual(1, carType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            valueAnnotationFound = this.CheckForVocabularyAnnotation(carType.VocabularyAnnotations(model), EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("bar") });
            Assert.IsTrue(valueAnnotationFound, "Annotation can't be found.");

            Assert.AreEqual(2, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
        }

        [TestMethod]
        public void ConstructibleVocabularyEditingVocabularyAnnotationToNewElement()
        {
            EdmModel model = VocabularyTestModelBuilder.InlineAnnotationSimpleModel();
            Assert.AreEqual(1, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var carType = model.FindEntityType("DefaultNamespace.Car");
            Assert.IsNotNull(carType, "Invalid entity type.");

            Assert.AreEqual(0, carType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            EdmTerm hiddenName = new EdmTerm("AnnotationNamespace", "HiddenName", EdmCoreModel.Instance.GetString(true));
            model.AddElement(hiddenName);

            var valueAnnotation = new FunctionalTests.MutableVocabularyAnnotation()
            {
                Target = carType,
                Term = hiddenName,
                Value = new EdmStringConstant("Gray")
            };

            model.AddVocabularyAnnotation(valueAnnotation);
            Assert.AreEqual(2, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
            Assert.AreEqual(1, carType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(carType.VocabularyAnnotations(model), EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("Gray") });
            Assert.IsTrue(valueAnnotationFound, "Annotation cannot be found.");

            var container = model.FindEntityContainer("Container");
            Assert.IsNotNull(container, "Invalid entity container name.");

            valueAnnotation.Target = container;
            valueAnnotation.Value = new EdmStringConstant("Blue");
            model.AddVocabularyAnnotation(valueAnnotation);

            valueAnnotationFound = this.CheckForVocabularyAnnotation(model.VocabularyAnnotations, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("Blue") });
            Assert.IsTrue(valueAnnotationFound, "Annotation cannot be found.");

            valueAnnotationFound = this.CheckForVocabularyAnnotation(container.VocabularyAnnotations(model), EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("Blue") });
            Assert.IsTrue(valueAnnotationFound, "Annotation cannot be found.");

            Assert.AreEqual(1, carType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");
            Assert.AreEqual(1, container.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");
            Assert.AreEqual(3, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
        }

        [TestMethod]
        public void ConstructibleVocabularyAddingVocabularyAnnotationWithEnum()
        {
            EdmModel model = VocabularyTestModelBuilder.InlineAnnotationSimpleModel();
            Assert.AreEqual(1, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var container = model.FindEntityContainer("Container") as EdmEntityContainer;
            Assert.IsNotNull(container, "Invalid name for entity container.");
            Assert.AreEqual(0, container.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            EdmEnumType popularity = new EdmEnumType("DefaultNamespace", "Popularity", EdmPrimitiveTypeKind.String, false);
            EdmEnumMember very = popularity.AddMember("Very", new EdmEnumMemberValue(3));
            model.AddElement(popularity);

            EdmTerm popularityTerm = new EdmTerm("DefaultNamespace", "PopularityTerm", new EdmEnumTypeReference(popularity, false));
            model.AddElement(popularityTerm);

            EdmVocabularyAnnotation valueAnnotation = new EdmVocabularyAnnotation(
                container,
                popularityTerm,
                new EdmEnumMemberExpression(very));

            model.AddVocabularyAnnotation(valueAnnotation);

            Assert.AreEqual(1, container.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");
            Assert.AreEqual(2, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var valueAnnotationFound = this.CheckForVocabularyAnnotation(model.VocabularyAnnotations, EdmExpressionKind.EnumMember, new List<PropertyValue>() { new PropertyValue("3") });
            Assert.IsTrue(valueAnnotationFound, "Annotation cannot be found.");

            valueAnnotationFound = this.CheckForVocabularyAnnotation(container.VocabularyAnnotations(model), EdmExpressionKind.EnumMember, new List<PropertyValue>() { new PropertyValue("3") });
            Assert.IsTrue(valueAnnotationFound, "Annotation cannot be found.");
        }

        [TestMethod]
        public void ConstructibleVocabularyRemovingInvalidAnnotation()
        {
            var model = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(VocabularyTestModelBuilder.InlineAnnotationSimpleModel());
            Assert.AreEqual(1, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

            var address = model.SchemaElements.Where(x => x.Name.Equals("Address")).SingleOrDefault() as EdmComplexType;
            var addressStreet = address.FindProperty("Street");
            Assert.IsNotNull(address, "Invalid complex type property.");
            Assert.AreEqual(0, addressStreet.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

            EdmEntityType petType = new EdmEntityType("DefaultNamespace", "Pet");
            EdmStructuralProperty petBreed = petType.AddStructuralProperty("Breed", EdmCoreModel.Instance.GetString(false));
            model.WrappedModel.AddElement(petType);

            var petTerm = new EdmTerm("DefaultNamespace", "PetTerm", new EdmEntityTypeReference(petType, false));
            model.WrappedModel.AddElement(petType);

            EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(
                addressStreet,
                petTerm,
                new EdmRecordExpression(new EdmPropertyConstructor(petBreed.Name, new EdmStringConstant("Fluffy"))));
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);

            model.RemoveVocabularyAnnotation(annotation);

            Assert.AreEqual(1, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
            Assert.AreEqual(0, addressStreet.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");
        }

        [TestMethod]
        public void ParsingSimpleVocabularyAnnotationWithComplexTypeCsdl()
        {
            var csdls = VocabularyTestModelBuilder.SimpleVocabularyAnnotationWithComplexTypeCsdl();
            var model = this.GetParserResult(csdls);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid error count.");

            var valueAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, valueAnnotations.Count(), "Invalid annotation count.");

            var person = model.FindType("ßÆœÇèÒöæ.Person") as IEdmStructuredType;
            var valueTerm = model.FindTerm("ßÆœÇèÒöæ.PersonInfo");
            var valueAnnotation = valueAnnotations.First().Value as IEdmRecordExpression;

            this.ValidateAnnotationWithExactPropertyAsType(model, person, valueAnnotation.Properties);
        }

        [TestMethod]
        public void ParsingVocabularyAnnotationComplexTypeWithFewerPropertiesCsdl()
        {
            var csdls = VocabularyTestModelBuilder.VocabularyAnnotationComplexTypeWithFewerPropertiesCsdl();
            var model = this.GetParserResult(csdls);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid error count.");

            var valueAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, valueAnnotations.Count(), "Invalid annotation count.");

            var person = model.FindType("NS.Person") as IEdmStructuredType;
            var valueTerm = model.FindTerm("NS.PersonInfo");
            var valueAnnotation = valueAnnotations.First().Value as IEdmRecordExpression;

            this.ValidateAnnotationWithFewerPropertyThanType(model, person, valueAnnotation.Properties);
        }

        [TestMethod]
        public void ParsingVocabularyAnnotationComplexTypeWithNullValuesCsdl()
        {
            var csdls = VocabularyTestModelBuilder.VocabularyAnnotationComplexTypeWithNullValuesCsdl();
            var model = this.GetParserResult(csdls);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid error count.");

            var valueAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, valueAnnotations.Count(), "Invalid annotation count.");

            var person = model.FindType("NS.Person") as IEdmStructuredType;
            var valueTerm = model.FindTerm("NS.PersonInfo");
            var valueAnnotation = valueAnnotations.First().Value as IEdmRecordExpression;

            this.ValidateAnnotationWithExactPropertyAsType(model, person, valueAnnotation.Properties);
        }

        [TestMethod]
        public void ParsingPathInAnOverloadedFunctionCsdl()
        {
            var csdls = VocabularyTestModelBuilder.PathInAnOverloadedFunctionCsdl();
            var model = this.GetParserResult(csdls);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid error count.");

            var valueAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, valueAnnotations.Count(), "Invalid annotation count.");

            var annotation = valueAnnotations.First();
            var appliedFunction = ((IEdmApplyExpression)annotation.Value).AppliedFunction;
            Assert.AreEqual("Edm.Int32", appliedFunction.ReturnType.FullName(), "Correct function return type");
        }

        [TestMethod]
        public void ParsingPathInAnOverloadedFunctionWithAmbiguousPrimitivesCsdl()
        {
            var csdls = VocabularyTestModelBuilder.ParsingPathInAnOverloadedFunctionWithAmbiguousPrimitivesCsdl();
            var model = this.GetParserResult(csdls);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid error count.");

            var valueAnnotations = model.VocabularyAnnotations;
            Assert.AreEqual(1, valueAnnotations.Count(), "Invalid annotation count.");

            var annotation = valueAnnotations.First();
            var appliedFunction = ((IEdmApplyExpression)annotation.Value).AppliedFunction;
            Assert.AreEqual("Edm.Int32", appliedFunction.ReturnType.FullName(), "Correct function return type");
        }

        [TestMethod]
        public void ParsingPathInAnOverloadedFunctionWithUnresolvedPathCsdl()
        {
            var csdls = VocabularyTestModelBuilder.ParsingPathInAnOverloadedFunctionWithUnresolvedPathCsdl();
            var model = this.GetParserResult(csdls);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);

            var expectedErrors = new EdmLibTestErrors() 
            {
                {24, 8, EdmErrorCode.BadUnresolvedOperation },
            };

            this.CompareErrors(errors, expectedErrors);
        }

        [TestMethod]
        public void ParsingUnresolvedOperationParameterTarget()
        {
            const string csdl = @"
<Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Annotations Target=""Namespace.NonResolvingContainer/NonResolvingFunction(Edm.String, Edm.Double)/NonResolvingParameter"">
    <Annotation Term=""Org.OData.Documentation.V1.Description"" String=""Sample String Value"" />
  </Annotations>
</Schema>";
            var csdls = new List<string>() { csdl };
            IEdmModel edmModel;
            IEnumerable<EdmError> errors;
            bool isParsed = SchemaReader.TryParse(csdls.Select(n => XElement.Parse(n).CreateReader()), out edmModel, out errors);

            Assert.IsTrue(isParsed, "Expected the CSDL to be successfully parsed.");
            Assert.AreEqual(1, edmModel.VocabularyAnnotations.Count(), "Expected CSDL to have exactly one vocabulary annotation.");
            var annotation = edmModel.VocabularyAnnotations.First();
            var targetErrors = annotation.Target.Errors();

            var expectedErrors = new EdmLibTestErrors() 
            {
                {0, 0, EdmErrorCode.BadUnresolvedParameter },
            };

            this.CompareErrors(targetErrors, expectedErrors);
        }

        [TestMethod]
        public void ParsingUnresolvedParameterOfResolvedFunctionTarget()
        {
            const string csdl = @"
<Schema Namespace=""Namespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Action Name=""FunctionName""><ReturnType Type=""Edm.Int32""/>
        <Parameter Name=""Parameter1"" Type=""Edm.String"" />
        <Parameter Name=""Parameter2"" Type=""Edm.Double"" />
      </Action>
  <EntityContainer Name=""ContainerName"">
        <ActionImport Name=""FunctionName"" Action=""Namespace.FunctionName"" />
  </EntityContainer>
  <Annotations Target=""Namespace.ContainerName/FunctionName(Edm.String, Edm.Double)/NonResolvingParameter"">
    <Annotation Term=""Org.OData.Documentation.V1.Description"" String=""Sample String Value"" />
  </Annotations>
</Schema>";
            var csdls = new List<string>() { csdl };
            IEdmModel edmModel;
            IEnumerable<EdmError> errors;
            bool isParsed = SchemaReader.TryParse(csdls.Select(n => XElement.Parse(n).CreateReader()), out edmModel, out errors);

            Assert.IsTrue(isParsed, "Expected the CSDL to be successfully parsed.");
            Assert.AreEqual(1, edmModel.VocabularyAnnotations.Count(), "Expected CSDL to have exactly one vocabulary annotation.");
            var annotation = edmModel.VocabularyAnnotations.First();
            var targetErrors = annotation.Target.Errors();

            var expectedErrors = new EdmLibTestErrors() 
            {
                {0, 0, EdmErrorCode.BadUnresolvedParameter },
            };

            this.CompareErrors(targetErrors, expectedErrors);
        }

        [TestMethod]
        public void ParsingResolvedOperationParameterTarget()
        {
            const string csdl = @"
<Schema Namespace=""Namespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Action Name=""FunctionName""><ReturnType Type=""Edm.Int32""/>
        <Parameter Name=""Parameter1"" Type=""Edm.String"" />
        <Parameter Name=""Parameter2"" Type=""Edm.Double"" />
      </Action>

  <EntityContainer Name=""ContainerName"">
        <ActionImport Name=""FunctionName"" Action=""Namespace.FunctionName"" />
  </EntityContainer>
  <Annotations Target=""Namespace.FunctionName(Edm.String, Edm.Double)/Parameter1"">
    <Annotation Term=""Org.OData.Documentation.V1.Description"" String=""Sample String Value"" />
  </Annotations>
</Schema>";
            var csdls = new List<string>() { csdl };
            IEdmModel edmModel;
            IEnumerable<EdmError> errors;
            bool isParsed = SchemaReader.TryParse(csdls.Select(n => XElement.Parse(n).CreateReader()), out edmModel, out errors);

            Assert.IsTrue(isParsed, "Expected the CSDL to be successfully parsed.");
            Assert.AreEqual(1, edmModel.VocabularyAnnotations.Count(), "Expected CSDL to have exactly one vocabulary annotation.");
            var annotation = edmModel.VocabularyAnnotations.First();
            var targetErrors = annotation.Target.Errors();

            var expectedErrors = new EdmLibTestErrors()
            {
                // No errors expected.
            };

            this.CompareErrors(targetErrors, expectedErrors);
        }

        [TestMethod]
        public void TestEnumMemberReferencingExtraEnumType()
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""TestNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Person"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" />
        <Annotation Term=""TestNS.OutColor"">
          <EnumMember>TestNS2.Color/Blue TestNS2.Color/Cyan</EnumMember>
        </Annotation>
      </EntityType>
      <Term Name=""OutColor"" Type=""TestNS2.Color"" />
    </Schema>
    <Schema Namespace=""TestNS2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EnumType Name=""Color"" IsFlags=""true"">
        <Member Name=""Cyan"" Value=""1"" />
        <Member Name=""Blue"" Value=""2"" />
      </EnumType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            IEnumerable<EdmError> validationErrors;

            #region try build model
            var edmModel = new EdmModel();
            var personType = new EdmEntityType("TestNS", "Person");
            edmModel.AddElement(personType);
            var pid = personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false);
            personType.AddKeys(pid);
            personType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            var colorType = new EdmEnumType("TestNS2", "Color", true);
            edmModel.AddElement(colorType);
            colorType.AddMember("Cyan", new EdmEnumMemberValue(1));
            colorType.AddMember("Blue", new EdmEnumMemberValue(2));
            var outColorTerm = new EdmTerm("TestNS", "OutColor", new EdmEnumTypeReference(colorType, true));
            edmModel.AddElement(outColorTerm);
            var exp = new EdmEnumMemberExpression(
                new EdmEnumMember(colorType, "Blue", new EdmEnumMemberValue(2)),
                new EdmEnumMember(colorType, "Cyan", new EdmEnumMemberValue(1))
            );

            var annotation = new EdmVocabularyAnnotation(personType, outColorTerm, exp);
            annotation.SetSerializationLocation(edmModel, EdmVocabularyAnnotationSerializationLocation.Inline);
            edmModel.SetVocabularyAnnotation(annotation);
            var stream = new MemoryStream();

            Assert.IsTrue(edmModel.Validate(out errors));

            using (var xw = XmlWriter.Create(stream, new XmlWriterSettings() { Indent = true }))
            {
                Assert.IsTrue(CsdlWriter.TryWriteCsdl(edmModel, xw, CsdlTarget.OData, out errors));
            }

            stream.Seek(0, SeekOrigin.Begin);
            using (var sr = new StreamReader(stream))
            {
                Assert.AreEqual(csdl, sr.ReadToEnd());
            }
            #endregion


            Assert.IsTrue(CsdlReader.TryParse(XmlReader.Create(new StringReader(csdl)), out model, out errors), "parsed");
            Assert.IsTrue(model.Validate(out validationErrors));

            TestEnumMember(model);
        }

        [TestMethod]
        public void TestEnumMemberReferencingExtraEnumTypeWithModelRef()
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/TestNS2.xml"">
    <edmx:Include Namespace=""TestNS2"" Alias=""TestA"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""TestNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Person"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" />
        <Annotation Term=""TestNS.OutColor"">
          <EnumMember>TestNS2.Color/Blue TestNS2.Color/Cyan</EnumMember>
        </Annotation>
      </EntityType>
      <Term Name=""OutColor"" Type=""TestNS2.Color"" />
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            const string csdl2 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""TestNS2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EnumType Name=""Color"" IsFlags=""true"">
        <Member Name=""Cyan"" Value=""1"" />
        <Member Name=""Blue"" Value=""2"" />
      </EnumType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            IEnumerable<EdmError> validationErrors;

            Assert.IsTrue(
                CsdlReader.TryParse(
                    XmlReader.Create(new StringReader(csdl)),
                    uri => XmlReader.Create(new StringReader(csdl2)),
                    out model,
                    out errors),
                "parsed");
            Assert.IsTrue(model.Validate(out validationErrors));

            TestEnumMember(model);
        }

        private void TestEnumMember(IEdmModel model)
        {
            var color = (IEdmEnumType)model.FindType("TestNS2.Color");
            var person = (IEdmEntityType)model.FindType("TestNS.Person");
            var annotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(person, "TestNS.OutColor").Single();
            var memberExp = (IEdmEnumMemberExpression)annotation.Value;
            var members = memberExp.EnumMembers.ToList();
            Assert.AreEqual(2, members.Count);
            Assert.AreEqual(color, members[0].DeclaringType);
            Assert.AreEqual("Blue", members[0].Name);
            Assert.AreEqual(color, members[1].DeclaringType);
            Assert.AreEqual("Cyan", members[1].Name);
        }

        private void ValidateAnnotationWithExactPropertyAsType(IEdmModel model, IEdmStructuredType actualType, IEnumerable<IEdmPropertyConstructor> annotationProperties)
        {
            Assert.AreEqual(actualType.Properties().Count(), annotationProperties.Count(), "Annotation does not have the same count of properties as it's declared type.");

            foreach (var annotationProperty in annotationProperties)
            {
                var annotationName = annotationProperty.Name;
                var actualProperty = actualType.FindProperty(annotationName);
                Assert.IsNotNull(actualProperty, "Invalid property name.");

                Action<IEdmModel, IEdmStructuredType, IEnumerable<IEdmPropertyConstructor>> validateAnnotation = (m, t, l) => this.ValidateAnnotationWithExactPropertyAsType(m, t, l);
                ValidateAnnotationProperty(model, actualProperty, annotationProperty, validateAnnotation);
            }
        }

        private void ValidateAnnotationWithFewerPropertyThanType(IEdmModel model, IEdmStructuredType actualType, IEnumerable<IEdmPropertyConstructor> annotationProperties)
        {
            foreach (var annotationProperty in annotationProperties)
            {
                var annotationName = annotationProperty.Name;
                var actualProperty = actualType.FindProperty(annotationName);
                Assert.IsNotNull(actualProperty, "Invalid property name.");

                Action<IEdmModel, IEdmStructuredType, IEnumerable<IEdmPropertyConstructor>> validateAnnotation = (m, t, l) => this.ValidateAnnotationWithFewerPropertyThanType(m, t, l);
                ValidateAnnotationProperty(model, actualProperty, annotationProperty, validateAnnotation);
            }
        }

        private void ValidateAnnotationWithMorePropertyThanType(IEdmModel model, IEdmStructuredType actualType, IEnumerable<IEdmPropertyConstructor> annotationProperties)
        {
            var actualProperties = actualType.Properties();

            foreach (var actualProperty in actualProperties)
            {
                var annotationProperty = annotationProperties.Where(n => n.Name.Equals(actualProperty.Name)).FirstOrDefault();
                Assert.IsNotNull(annotationProperty, "Invalid property name.");

                Action<IEdmModel, IEdmStructuredType, IEnumerable<IEdmPropertyConstructor>> validateAnnotation = (m, t, l) => this.ValidateAnnotationWithMorePropertyThanType(m, t, l);
                ValidateAnnotationProperty(model, actualProperty, annotationProperty, validateAnnotation);
            }
        }
        private void ValidateAnnotationProperty(IEdmModel model, IEdmProperty actualProperty, IEdmPropertyConstructor annotationProperty, Action<IEdmModel, IEdmStructuredType, IEnumerable<IEdmPropertyConstructor>> validateAnnotation)
        {
            if (annotationProperty.Value.ExpressionKind.Equals(EdmExpressionKind.Record))
            {
                if (IsPropertyNonNullExpressionOrNonNullable(actualProperty.Type.IsNullable, annotationProperty.Value.ExpressionKind))
                {
                    var actualPropertyType = model.FindType(actualProperty.Type.Definition.ToString()) as IEdmStructuredType;
                    var annotationPropertyValue = (annotationProperty.Value as IEdmRecordExpression).Properties;

                    validateAnnotation(model, actualPropertyType, annotationPropertyValue);
                }
            }
            else if (annotationProperty.Value.ExpressionKind.Equals(EdmExpressionKind.Collection))
            {
                var actualPropertyElementType = (actualProperty.Type.Definition as IEdmCollectionType).ElementType;
                var actualElementExpressionKind = this.GetPrimitiveExpressionKind(actualPropertyElementType.Definition.ToString());
                var annotationPropertyElements = (annotationProperty.Value as IEdmCollectionExpression).Elements;

                foreach (var element in annotationPropertyElements)
                {
                    if (IsPropertyNonNullExpressionOrNonNullable(actualPropertyElementType.IsNullable, element.ExpressionKind))
                    {
                        Assert.AreEqual(actualElementExpressionKind, element.ExpressionKind, "Invalid expression kind.");
                    }
                }
            }
            else
            {
                var actualPropertyType = this.GetPrimitiveExpressionKind(actualProperty.Type.Definition.ToString());

                if (IsPropertyNonNullExpressionOrNonNullable(actualProperty.Type.IsNullable, annotationProperty.Value.ExpressionKind))
                {
                    Assert.AreEqual(actualPropertyType, annotationProperty.Value.ExpressionKind, "Invalid expression kind.");
                }
            }
        }

        private static bool IsPropertyNonNullExpressionOrNonNullable(bool actualPropertyNullable, EdmExpressionKind checkPropertyExpression)
        {
            return (actualPropertyNullable == true && checkPropertyExpression != EdmExpressionKind.Null) || actualPropertyNullable == false;
        }

        private EdmExpressionKind GetPrimitiveExpressionKind(string type)
        {
            if (type.Equals("Edm.String"))
            {
                return EdmExpressionKind.StringConstant;
            }
            else if (type.Equals("Edm.Int32"))
            {
                return EdmExpressionKind.IntegerConstant;
            }

            return EdmExpressionKind.None;
        }

        private class AnnotationProperty
        {
            public string Name { get; set; }
            public EdmExpressionKind ExpressionKind { get; set; }
            public List<PropertyValue> Values { get; set; }
        }

        private class PropertyValue
        {
            public string Name { get; set; }
            public string Value { get; set; }

            public PropertyValue(string value)
            {
                this.Name = null;
                this.Value = value;
            }

            public PropertyValue(string name, string value)
            {
                this.Name = name;
                this.Value = value;
            }
        }

        private IEnumerable<PropertyValue> SortPropertyValueList(List<PropertyValue> propertyValues)
        {
            return propertyValues.OrderBy(x => null == x.Name ? x.Value : x.Value + x.Name);
        }

        private bool AreEquivalent(PropertyValue actual, PropertyValue expected)
        {
            return actual.Name == expected.Name && actual.Value == expected.Value;
        }

        private bool AreEquivalent(List<PropertyValue> actualValues, List<PropertyValue> expectedValues)
        {
            int actualCount = actualValues.Count();
            int expectedCount = expectedValues.Count();

            if (actualCount > 0 && expectedCount > 0 && actualCount == expectedCount)
            {
                var sortedActualValues = this.SortPropertyValueList(actualValues).ToList();
                var sortedExpectedValues = this.SortPropertyValueList(expectedValues).ToList();

                for (int i = 0; i < actualCount; i++)
                {
                    if (!this.AreEquivalent(sortedActualValues[i], sortedExpectedValues[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return actualCount == expectedCount;
        }

        private bool CheckForVocabularyAnnotation(IEnumerable<IEdmVocabularyAnnotation> vocabularyAnnotations, EdmExpressionKind expressionKind, List<PropertyValue> values)
        {
            return this.CheckForVocabularyAnnotation(vocabularyAnnotations, expressionKind, values, null);
        }

        private bool CheckForVocabularyAnnotation(IEnumerable<IEdmVocabularyAnnotation> vocabularyAnnotations, EdmExpressionKind expressionKind, List<PropertyValue> values, string target)
        {
            bool annotationFounded = false;
            List<PropertyValue> annotationValues;

            foreach (IEdmVocabularyAnnotation valueAnnotation in vocabularyAnnotations)
            {
                if (null != valueAnnotation && (null == target || valueAnnotation.Target.ToString().Equals(target)))
                {
                    annotationValues = GetAnnotationValueList(expressionKind, valueAnnotation);
                    annotationFounded = AreEquivalent(values, annotationValues);

                    if (annotationFounded)
                    {
                        break;
                    }
                }
            }

            return annotationFounded;
        }

        private List<PropertyValue> GetAnnotationValueList(EdmExpressionKind expressionKind, IEdmVocabularyAnnotation valueAnnotation)
        {
            return this.GetAnnotationValueList(expressionKind, valueAnnotation.Value);
        }

        private List<PropertyValue> GetAnnotationValueList(EdmExpressionKind expressionKind, IEdmExpression expression)
        {
            List<PropertyValue> annotationValues = new List<PropertyValue>();

            switch (expressionKind)
            {
                case EdmExpressionKind.EnumMember:
                    var enumReferenceExpression = expression as IEdmEnumMemberExpression;
                    if (null != enumReferenceExpression)
                    {
                        var enumValue = enumReferenceExpression.EnumMembers.Single().Value as EdmEnumMemberValue;

                        if (null != enumValue)
                        {
                            annotationValues.Add(new PropertyValue(enumValue.Value.ToString()));
                        }
                    }
                    break;
                case EdmExpressionKind.Collection:
                    var collectionExpression = expression as IEdmCollectionExpression;
                    if (null != collectionExpression)
                    {
                        foreach (var element in collectionExpression.Elements)
                        {
                            var annotationValue = this.GetAnnotationValue(element.ExpressionKind, element);

                            if (null != annotationValue)
                            {
                                annotationValues.Add(annotationValue);
                            }
                        }
                    }
                    break;
                case EdmExpressionKind.Record:
                    var recordExpression = expression as IEdmRecordExpression;
                    if (null != recordExpression)
                    {
                        foreach (var property in recordExpression.Properties)
                        {
                            var annotationValue = this.GetAnnotationValue(property.Value.ExpressionKind, property.Value);

                            if (null != annotationValue)
                            {
                                annotationValue.Name = property.Name;
                                annotationValues.Add(annotationValue);
                            }
                        }
                    }
                    break;
                default:
                    var value = this.GetAnnotationValue(expressionKind, expression);
                    if (null != value)
                    {
                        annotationValues.Add(value);
                    }
                    break;
            }

            return annotationValues;
        }

        private PropertyValue GetAnnotationValue(EdmExpressionKind expressionKind, IEdmExpression expression)
        {
            PropertyValue annotationValue = null;

            switch (expressionKind)
            {
                case EdmExpressionKind.StringConstant:
                    var stringExpression = expression as IEdmStringConstantExpression;
                    if (null != stringExpression)
                    {
                        annotationValue = new PropertyValue(stringExpression.Value.ToString());
                    }
                    break;
                case EdmExpressionKind.IntegerConstant:
                    var integerExpression = expression as IEdmIntegerConstantExpression;
                    if (null != integerExpression)
                    {
                        annotationValue = new PropertyValue(integerExpression.Value.ToString());
                    }
                    break;
                case EdmExpressionKind.Labeled:
                    {
                        IEdmLabeledExpression labeled = (IEdmLabeledExpression)expression;
                        return this.GetAnnotationValue(labeled.Expression.ExpressionKind, labeled.Expression);
                    }
            }

            return annotationValue;
        }
    }
}
