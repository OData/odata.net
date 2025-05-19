//---------------------------------------------------------------------
// <copyright file="VocabularyParsingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.Community.V1;
using Microsoft.OData.Edm.Vocabularies.V1;

public class VocabularyParsingTests : EdmLibTestCaseBase
{
    public VocabularyParsingTests()
    {
        this.EdmVersion = EdmVersion.V40;
    }

    [Fact]
    public void Parsing_Simple_Terms()
    {
        this.PerfomCommonParserTest(VocabularyTestModelBuilder.SimpleTerms());
    }

    [Fact]
    public void Parsing_Simple_Term_WithDuplicate()
    {
        this.PerfomCommonParserTest(VocabularyTestModelBuilder.SimpleTermWithDuplicate());
    }

    [Fact]
    public void Parsing_Simple_Term_InV10()
    {
        var model = VocabularyTestModelBuilder.SimpleTermInV10();
        var definitionCsdls = new VocabularyDefinitionCsdlGenerator().GenerateDefinitionCsdl(EdmVersion.V40, model);
        IEdmModel parsedModel = this.GetParserResult(definitionCsdls);
        this.VerifyParsedModel(model, parsedModel);
    }

    [Fact]
    public void Parsing_Simple_VocabularyAnnotation()
    {
        this.PerfomCommonParserTest(VocabularyTestModelBuilder.SimpleVocabularyAnnotation());
    }

    [Fact]
    public void Parsing_Simple_VocabularyAnnotation_WithQualifiers()
    {
        this.PerfomCommonParserTest(VocabularyTestModelBuilder.SimpleVocabularyAnnotationWithQualifiers());
    }

    [Fact]
    public void Parsing_Simple_VocabularyAnnotation_Confict()
    {
        this.PerfomCommonParserTest(VocabularyTestModelBuilder.SimpleVocabularyAnnotationConfict());
    }

    [Fact]
    public void Parsing_Multiple_VocabularyAnnotations()
    {
        this.PerfomCommonParserTest(VocabularyTestModelBuilder.MultipleVocabularyAnnotations());
    }

    // [EdmLib] vocabulary annotation Parsing does not work on EntityContainer
    [Fact]
    public void Parsing_Simple_VocabularyAnnotation_OnContainerAndEntitySet()
    {
        this.PerfomCommonParserTest(VocabularyTestModelBuilder.SimpleVocabularyAnnotationOnContainerAndEntitySet());
    }

    [Fact]
    public void Parsing_Structured_VocabularyAnnotation()
    {
        this.PerfomCommonParserTest(VocabularyTestModelBuilder.StructuredVocabularyAnnotation());
    }

    [Fact]
    public void Parsing_TermOfStructuredDataType()
    {
        this.PerfomCommonParserTest(VocabularyTestModelBuilder.TermOfStructuredDataType());
    }

    [Fact]
    public void Parsing_TypeTermWithNavigation()
    {
        this.PerfomCommonParserTest(VocabularyTestModelBuilder.TypeTermWithNavigation());
    }

    [Fact]
    public void Parsing_TypeTermWithInheritance()
    {
        this.PerfomCommonParserTest(VocabularyTestModelBuilder.TypeTermWithInheritance());
    }

    [Fact]
    public void Parsing_VocabularyAnnotationWithRecord()
    {
        this.PerfomCommonParserTest(VocabularyTestModelBuilder.VocabularyAnnotationWithRecord());
    }

    [Fact]
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
        Assert.Equal(0, compareErrorMessages.Count, "comparision errors");
    }

    private void PerfomCommonParserTest(IEdmModel testModel)
    {
        var csdls = new List<XElement>();
        csdls.AddRange(new VocabularyDefinitionCsdlGenerator().GenerateDefinitionCsdl(this.EdmVersion, testModel));
        csdls.Add(new VocabularyApplicationCsdlGenerator().GenerateApplicationCsdl(this.EdmVersion, testModel));
        IEdmModel parsedModel = this.GetParserResult(csdls);
        this.VerifyParsedModel(testModel, parsedModel);
    }

    [Fact]
    public void Parsing_InlineVocabularyAnnotationEntityTypeUsingAlias()
    {
        var inlineValueAnnotationEntityType = VocabularyTestModelBuilder.InlineVocabularyAnnotationEntityTypeUsingAlias();
        var model = this.GetParserResult(inlineValueAnnotationEntityType);

        var stringTerm = model.FindTerm("AnnotationNamespace.StringTerm");
        Assert.NotNull(stringTerm, "Invalid term name.");
        Assert.True(stringTerm.Type.IsString(), "Invalid term type.");

        var carVocabularyAnnotations = model.FindEntityType("DefaultNamespace.Car").VocabularyAnnotations(model);
        Assert.Equal(1, carVocabularyAnnotations.Count());

        var annotationFound = this.CheckForVocabularyAnnotation(carVocabularyAnnotations, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("foo") });
        Assert.True(annotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parsing_OutOfLineVocabularyAnnotationEntityType()
    {
        var outOfLineAnnotationEntityType = VocabularyTestModelBuilder.OutOfLineAnnotationEntityType();
        var model = this.GetParserResult(outOfLineAnnotationEntityType);

        var stringTerm = model.FindTerm("AnnotationNamespace.StringTerm");
        Assert.NotNull(stringTerm, "Invalid term name.");
        Assert.True(stringTerm.Type.IsString(), "Invalid term type.");

        var carType = model.FindEntityType("DefaultNamespace.Car");
        var carVocabularyAnnotations = carType.VocabularyAnnotations(model);
        Assert.Equal(1, carVocabularyAnnotations.Count());

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(carVocabularyAnnotations, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("foo") }, "DefaultNamespace.Car");
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parsing_OutOfLineAnnotationEntityProperty()
    {
        var outOfLineValueAnnotationEntityProperty = VocabularyTestModelBuilder.OutOfLineAnnotationEntityProperty();
        var model = this.GetParserResult(outOfLineValueAnnotationEntityProperty);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(2, vocabularyAnnotations.Count());

        var carEntityType = model.FindEntityType("DefaultNamespace.Car");
        var carVocabularyAnnotations = carEntityType.VocabularyAnnotations(model);
        Assert.Equal(0, carVocabularyAnnotations.Count());

        var wheelsVocabularyAnnotations = carEntityType.Properties().Where(x => x.Name == "Wheels").SingleOrDefault().VocabularyAnnotations(model);
        Assert.Equal(1, wheelsVocabularyAnnotations.Count());

        var ownerEntityType = model.FindEntityType("DefaultNamespace.Owner");
        var ownerVocabularyAnnotations = ownerEntityType.VocabularyAnnotations(model);
        Assert.Equal(0, ownerVocabularyAnnotations.Count());

        var ownerNameVocabularyAnnotations = ownerEntityType.Properties().Where(x => x.Name == "Name").SingleOrDefault().VocabularyAnnotations(model);
        Assert.Equal(1, ownerNameVocabularyAnnotations.Count());

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(ownerNameVocabularyAnnotations, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("foo") });
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parsing_InlineAnnotationEntityProperty()
    {
        var inlineAnnotationEntityProperty = VocabularyTestModelBuilder.InlineAnnotationEntityProperty();
        var model = this.GetParserResult(inlineAnnotationEntityProperty);

        var carEntityType = model.FindEntityType("DefaultNamespace.Car");
        var carVocabularyAnnotations = carEntityType.VocabularyAnnotations(model);
        Assert.Equal(0, carVocabularyAnnotations.Count());

        var wheelsVocabularyAnnotations = carEntityType.Properties().Where(x => x.Name.Equals("Wheels")).SingleOrDefault().VocabularyAnnotations(model);
        Assert.Equal(1, wheelsVocabularyAnnotations.Count());

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(wheelsVocabularyAnnotations, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("foo") });
        Assert.True(valueAnnotationFound, "Annotation can't be found.");

        var ownerEntityType = model.FindEntityType("DefaultNamespace.Owner");
        var ownerVocabularyAnnotations = ownerEntityType.VocabularyAnnotations(model);
        Assert.Equal(0, ownerVocabularyAnnotations.Count());

        var ownerIdVocabularyAnnotations = ownerEntityType.Properties().Where(x => x.Name.Equals("Id")).SingleOrDefault().VocabularyAnnotations(model);
        Assert.Equal(1, ownerIdVocabularyAnnotations.Count());

    }

    [Fact]
    public void Parsing_OutOfLineAnnotationNavigationProperty()
    {
        var outOfLineAnnotationNavigationProperty = VocabularyTestModelBuilder.OutOfLineAnnotationNavigationProperty();
        var model = this.GetParserResult(outOfLineAnnotationNavigationProperty);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(2, vocabularyAnnotations.Count());

        var ownerEntityType = model.FindEntityType("DefaultNamespace.Owner");
        var ownerVocabularyAnnotations = ownerEntityType.VocabularyAnnotations(model);
        Assert.Equal(0, ownerVocabularyAnnotations.Count());

        var carNavigationVocabularyAnnotations = ownerEntityType.NavigationProperties().Where(x => x.Name == "Car").SingleOrDefault().VocabularyAnnotations(model);
        Assert.Equal(1, carNavigationVocabularyAnnotations.Count());

        var carEntityType = model.FindEntityType("DefaultNamespace.Car");
        var carVocabularyAnnotations = carEntityType.VocabularyAnnotations(model);
        Assert.Equal(0, carVocabularyAnnotations.Count());

        var ownerNavigationVocabularyAnnotations = carEntityType.NavigationProperties().Where(x => x.Name == "Owner").SingleOrDefault().VocabularyAnnotations(model);
        Assert.Equal(1, ownerNavigationVocabularyAnnotations.Count());
    }

    [Fact]
    public void Parsing_InlineAnnotationNavigationProperty()
    {
        var inLineAnnotationNavigationProperty = VocabularyTestModelBuilder.InlineAnnotationNavigationProperty();
        var model = this.GetParserResult(inLineAnnotationNavigationProperty);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(2, vocabularyAnnotations.Count());

        var ownerEntityType = model.FindEntityType("DefaultNamespace.Owner");
        var ownerVocabularyAnnotations = ownerEntityType.VocabularyAnnotations(model);
        Assert.Equal(0, ownerVocabularyAnnotations.Count());

        var carNavigationVocabularyAnnotations = ownerEntityType.NavigationProperties().Where(x => x.Name == "Car").SingleOrDefault().VocabularyAnnotations(model);
        Assert.Equal(1, carNavigationVocabularyAnnotations.Count());

        var carEntityType = model.FindEntityType("DefaultNamespace.Car");
        var carVocabularyAnnotations = carEntityType.VocabularyAnnotations(model);
        Assert.Equal(0, carVocabularyAnnotations.Count());

        var ownerNavigationVocabularyAnnotations = carEntityType.NavigationProperties().Where(x => x.Name == "Owner").SingleOrDefault().VocabularyAnnotations(model);
        Assert.Equal(1, ownerNavigationVocabularyAnnotations.Count());

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(ownerNavigationVocabularyAnnotations, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("foo") });
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parsing_InlineAnnotationEntityContainer()
    {
        var inLineAnnotationEntityContainer = VocabularyTestModelBuilder.InlineAnnotationEntityContainer();
        var model = this.GetParserResult(inLineAnnotationEntityContainer);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count());

        var entityContainerVocabularyAnnotation = model.EntityContainer.VocabularyAnnotations(model);

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(entityContainerVocabularyAnnotation, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("foo") });
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parsing_OutOfLineAnnotationEntityContainer()
    {
        var outOfLineAnnotationEntityProperty = VocabularyTestModelBuilder.OutOfLineAnnotationEntityContainer();
        var model = this.GetParserResult(outOfLineAnnotationEntityProperty);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(2, vocabularyAnnotations.Count());

        var entityContainerVocabularyAnnotation = model.EntityContainer.VocabularyAnnotations(model);

        List<PropertyValue> listOfPeople = new List<PropertyValue> { 
                                                                        new PropertyValue("Joe"), 
                                                                        new PropertyValue("Mary"), 
                                                                        new PropertyValue("Justin") 
                                                                    };
        var valueAnnotationFound = this.CheckForVocabularyAnnotation(entityContainerVocabularyAnnotation, EdmExpressionKind.Collection, listOfPeople);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");

        List<PropertyValue> address = new List<PropertyValue>   { 
                                                                    new PropertyValue("Street", "foo"), 
                                                                    new PropertyValue("City", "bar")
                                                                };

        valueAnnotationFound = this.CheckForVocabularyAnnotation(entityContainerVocabularyAnnotation, EdmExpressionKind.Record, address);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parsing_InlineAnnotationComplexType()
    {
        var inLineAnnotationComplexType = VocabularyTestModelBuilder.InlineAnnotationComplexType();
        var model = this.GetParserResult(inLineAnnotationComplexType);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count());

        var pet = model.SchemaElements.Where(x => x.Name.Equals("Pet")).SingleOrDefault() as IEdmComplexType;
        Assert.NotNull(pet);
        var petVocabularyAnnotation = pet.VocabularyAnnotations(model);
        Assert.Equal(1, petVocabularyAnnotation.Count());

        List<PropertyValue> address = new List<PropertyValue>   { 
                                                                    new PropertyValue("Street", "foo"), 
                                                                    new PropertyValue("City", "bar")
                                                                };

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(petVocabularyAnnotation, EdmExpressionKind.Record, address);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parsing_OutOfLineAnnotationComplexType()
    {
        var outOfLineAnnotationComplexType = VocabularyTestModelBuilder.OutOfLineAnnotationComplexType();
        var model = this.GetParserResult(outOfLineAnnotationComplexType);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count());

        var pet = model.SchemaElements.Where(x => x.Name.Equals("Pet")).SingleOrDefault() as IEdmComplexType;
        Assert.NotNull(pet);
        var petVocabularyAnnotation = pet.VocabularyAnnotations(model);
        Assert.Equal(1, petVocabularyAnnotation.Count());

        List<PropertyValue> listOfPeople = new List<PropertyValue> { 
                                                                        new PropertyValue("Joe"), 
                                                                        new PropertyValue("Mary"), 
                                                                        new PropertyValue("Justin") 
                                                                    };
        var valueAnnotationFound = this.CheckForVocabularyAnnotation(petVocabularyAnnotation, EdmExpressionKind.Collection, listOfPeople);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parse_InlineAnnotationComplexTypeProperty()
    {
        var inLineAnnotationComplexTypeProperty = VocabularyTestModelBuilder.InlineAnnotationComplexTypeProperty();
        var model = this.GetParserResult(inLineAnnotationComplexTypeProperty);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count());

        var pet = model.SchemaElements.Where(x => x.Name.Equals("Pet")).SingleOrDefault() as IEdmComplexType;
        Assert.NotNull(pet);

        var petVocabularyAnnotation = pet.VocabularyAnnotations(model);
        Assert.Equal(0, petVocabularyAnnotation.Count());

        var nameVocabularyAnnotation = pet.Properties().Where(x => x.Name.Equals("Name")).FirstOrDefault().VocabularyAnnotations(model);
        Assert.Equal(1, nameVocabularyAnnotation.Count());

        List<PropertyValue> listOfPeople = new List<PropertyValue> { 
                                                                        new PropertyValue("Joe"), 
                                                                        new PropertyValue("Mary"), 
                                                                        new PropertyValue("Justin") 
                                                                    };

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(nameVocabularyAnnotation, EdmExpressionKind.Collection, listOfPeople);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parsing_OutOfLineAnnotationComplexTypeProperty()
    {
        var outOfLineAnnotationComplexTypeProperty = VocabularyTestModelBuilder.OutOfLineAnnotationComplexTypeProperty();
        var model = this.GetParserResult(outOfLineAnnotationComplexTypeProperty);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count());

        var pet = model.SchemaElements.Where(x => x.Name.Equals("Pet")).SingleOrDefault() as IEdmComplexType;
        Assert.NotNull(pet);

        var petVocabularyAnnotation = pet.VocabularyAnnotations(model);
        Assert.Equal(0, petVocabularyAnnotation.Count());

        var ownerIdVocabularyAnnotation = pet.Properties().Where(x => x.Name.Equals("OwnerId")).FirstOrDefault().VocabularyAnnotations(model);
        Assert.Equal(1, ownerIdVocabularyAnnotation.Count());

        List<PropertyValue> address = new List<PropertyValue>   { 
                                                                    new PropertyValue("Street", "foo"), 
                                                                    new PropertyValue("City", "bar")
                                                                };

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(ownerIdVocabularyAnnotation, EdmExpressionKind.Record, address);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parse_InlineAnnotationFunction()
    {
        var inLineAnnotationFunction = VocabularyTestModelBuilder.InlineAnnotationFunction();
        var model = this.GetParserResult(inLineAnnotationFunction);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count());

        var peopleCount = model.SchemaElements.Where(x => x.Name.Equals("PeopleCount")).SingleOrDefault() as IEdmOperation;
        Assert.NotNull(peopleCount);

        var peopleCountVocabularyAnnotation = peopleCount.VocabularyAnnotations(model);
        Assert.Equal(1, peopleCountVocabularyAnnotation.Count());

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(peopleCountVocabularyAnnotation, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("3") });
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parsing_OutOfLineAnnotationFunction()
    {
        var outOfLineAnnotationFunction = VocabularyTestModelBuilder.OutOfLineAnnotationFunction();
        var model = this.GetParserResult(outOfLineAnnotationFunction);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count());

        var peopleCount = model.SchemaElements.Where(x => x.Name.Equals("PeopleCount")).SingleOrDefault() as IEdmOperation;
        Assert.NotNull(peopleCount);

        var peopleCountVocabularyAnnotation = peopleCount.VocabularyAnnotations(model);
        Assert.Equal(1, peopleCountVocabularyAnnotation.Count());

        List<PropertyValue> listOfAge = new List<PropertyValue> { 
                                                                    new PropertyValue("39"), 
                                                                    new PropertyValue("12"), 
                                                                    new PropertyValue("51") 
                                                                };

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(peopleCountVocabularyAnnotation, EdmExpressionKind.Collection, listOfAge);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parse_InlineAnnotationOperationParameter()
    {
        var inLineAnnotationOperationParameter = VocabularyTestModelBuilder.InlineAnnotationOperationParameter();
        var model = this.GetParserResult(inLineAnnotationOperationParameter);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count());

        var peopleCount = model.SchemaElements.Where(x => x.Name.Equals("PeopleCount")).SingleOrDefault() as IEdmOperation;
        Assert.NotNull(peopleCount);

        var peopleCountVocabularyAnnotation = peopleCount.VocabularyAnnotations(model);
        Assert.Equal(0, peopleCountVocabularyAnnotation.Count());

        var peopleListVocabularyAnnotation = peopleCount.Parameters.Where(x => x.Name.Equals("PeopleList")).SingleOrDefault().VocabularyAnnotations(model);
        Assert.Equal(1, peopleListVocabularyAnnotation.Count());

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(peopleListVocabularyAnnotation, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("3") });
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parsing_OutOfLineAnnotationOperationParameter()
    {
        var outOfLineAnnotationOperationParameter = VocabularyTestModelBuilder.OutOfLineAnnotationOperationParameter();
        var model = this.GetParserResult(outOfLineAnnotationOperationParameter);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count());

        var peopleCount = model.SchemaElements.Where(x => x.Name.Equals("PeopleCount")).SingleOrDefault() as IEdmOperation;
        Assert.NotNull(peopleCount);

        var peopleCountVocabularyAnnotation = peopleCount.VocabularyAnnotations(model);
        Assert.Equal(0, peopleCountVocabularyAnnotation.Count());

        var peopleListVocabularyAnnotation = peopleCount.Parameters.Where(x => x.Name.Equals("PeopleList")).SingleOrDefault().VocabularyAnnotations(model);
        Assert.Equal(1, peopleListVocabularyAnnotation.Count());

        List<PropertyValue> listOfAge = new List<PropertyValue> { 
                                                                    new PropertyValue("39"), 
                                                                    new PropertyValue("12"), 
                                                                    new PropertyValue("51") 
                                                                };

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(peopleListVocabularyAnnotation, EdmExpressionKind.Collection, listOfAge);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parse_InlineAnnotationFunctionImport()
    {
        var inLineAnnotationFunctionImport = VocabularyTestModelBuilder.InlineAnnotationFunctionImport();
        var model = this.GetParserResult(inLineAnnotationFunctionImport);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count());

        var entityContainer = model.EntityContainer;
        Assert.NotNull(entityContainer);
        Assert.Equal(0, entityContainer.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        var medianAge = entityContainer.FindOperationImports("MedianAge").SingleOrDefault() as IEdmOperationImport;
        Assert.NotNull(medianAge, "Invalid function import name.");
        Assert.Equal(1, medianAge.VocabularyAnnotations(model).Count());
        var medianAgeVocabularyAnnotation = medianAge.VocabularyAnnotations(model);

        List<PropertyValue> listOfAge = new List<PropertyValue> { 
                                                                        new PropertyValue("39"), 
                                                                        new PropertyValue("12"), 
                                                                        new PropertyValue("51") 
                                                                    };

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(medianAgeVocabularyAnnotation, EdmExpressionKind.Collection, listOfAge);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parsing_OutOfLineAnnotationFunctionImport()
    {
        var outOfLineAnnotationFunctionImport = VocabularyTestModelBuilder.OutOfLineAnnotationFunctionImport();
        var model = this.GetParserResult(outOfLineAnnotationFunctionImport);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count());

        var entityContainer = model.EntityContainer;
        Assert.NotNull(entityContainer);
        Assert.Equal(0, entityContainer.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        var medianAge = entityContainer.FindOperationImports("MedianAge").SingleOrDefault() as IEdmOperationImport;
        Assert.NotNull(medianAge, "Invalid function import name.");
        Assert.Equal(1, medianAge.VocabularyAnnotations(model).Count());
        var medianAgeVocabularyAnnotation = medianAge.VocabularyAnnotations(model);

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(medianAgeVocabularyAnnotation, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("3") });
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parse_InlineAnnotationFunctionImportParameter()
    {
        var inLineAnnotationFunctionImportParameter = VocabularyTestModelBuilder.InlineAnnotationFunctionImportParameter();
        var model = this.GetParserResult(inLineAnnotationFunctionImportParameter);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count());

        var entityContainer = model.EntityContainer;
        Assert.NotNull(entityContainer);
        Assert.Equal(0, entityContainer.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        var medianAge = entityContainer.FindOperationImports("MedianAge").SingleOrDefault() as IEdmOperationImport;
        Assert.NotNull(medianAge, "Invalid function import name.");
        Assert.Equal(0, medianAge.VocabularyAnnotations(model).Count());

        var peopleAge = medianAge.Operation.Parameters.Where(x => x.Name.Equals("PeopleAge")).SingleOrDefault() as IEdmOperationParameter;
        Assert.NotNull(peopleAge, "Invalid function import name.");
        var peopleAgeVocabularyAnnotation = peopleAge.VocabularyAnnotations(model);
        Assert.Equal(1, peopleAgeVocabularyAnnotation.Count());

        List<PropertyValue> listOfAge = new List<PropertyValue> { 
                                                                        new PropertyValue("39"), 
                                                                        new PropertyValue("12"), 
                                                                        new PropertyValue("51") 
                                                                    };

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(peopleAgeVocabularyAnnotation, EdmExpressionKind.Collection, listOfAge);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parsing_OutOfLineAnnotationFunctionImportParameter()
    {
        var outOfLineAnnotationFunctionImportParameter = VocabularyTestModelBuilder.OutOfLineAnnotationFunctionImportParameter();
        var model = this.GetParserResult(outOfLineAnnotationFunctionImportParameter);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count());

        var entityContainer = model.EntityContainer;
        Assert.NotNull(entityContainer);
        Assert.Equal(0, entityContainer.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        var medianAge = entityContainer.FindOperationImports("MedianAge").SingleOrDefault() as IEdmOperationImport;
        Assert.NotNull(medianAge, "Invalid function import name.");
        Assert.Equal(0, medianAge.VocabularyAnnotations(model).Count());

        var peopleAge = medianAge.Operation.Parameters.Where(x => x.Name.Equals("PeopleAge")).SingleOrDefault() as IEdmOperationParameter;
        Assert.NotNull(peopleAge, "Invalid function import name.");
        var peopleAgeVocabularyAnnotation = peopleAge.VocabularyAnnotations(model);
        Assert.Equal(1, peopleAgeVocabularyAnnotation.Count());

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(peopleAgeVocabularyAnnotation, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("3") });
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parsing_InlineAnnotationEntitySet()
    {
        var inLineAnnotationEntitySet = VocabularyTestModelBuilder.InlineAnnotationEntitySet();
        var model = this.GetParserResult(inLineAnnotationEntitySet);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count());

        var entityContainer = model.EntityContainer;
        Assert.NotNull(entityContainer);
        Assert.Equal(0, entityContainer.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        var petDog = entityContainer.FindEntitySet("PetDog");
        Assert.NotNull(petDog, "Invalid entity set name.");
        var petDogVocabularyAnnotation = petDog.VocabularyAnnotations(model);
        Assert.Equal(1, petDogVocabularyAnnotation.Count());

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(petDogVocabularyAnnotation, EdmExpressionKind.IntegerConstant, new List<PropertyValue> { new PropertyValue("22") });
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parsing_OutOfLineAnnotationEntitySet()
    {
        var outOfLineAnnotationEntitySet = VocabularyTestModelBuilder.OutOfLineAnnotationEntitySet();
        var model = this.GetParserResult(outOfLineAnnotationEntitySet);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count());

        var entityContainer = model.EntityContainer;
        Assert.NotNull(entityContainer);
        Assert.Equal(0, entityContainer.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        var petDog = entityContainer.FindEntitySet("PetDog");
        Assert.NotNull(petDog, "Invalid entity set name.");
        var petDogVocabularyAnnotation = petDog.VocabularyAnnotations(model);
        Assert.Equal(1, petDogVocabularyAnnotation.Count());

        List<PropertyValue> address = new List<PropertyValue>   { 
                                                                    new PropertyValue("Street", "foo"), 
                                                                    new PropertyValue("City", "bar")
                                                                };

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(petDogVocabularyAnnotation, EdmExpressionKind.Record, address);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parsing_InlineAnnotationEnumType()
    {
        var inLineAnnotationEnumType = VocabularyTestModelBuilder.InlineAnnotationEnumType();
        var model = this.GetParserResult(inLineAnnotationEnumType);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var popularity = model.SchemaElements.Where(x => x.Name.Equals("Popularity")).Single() as IEdmEnumType;
        Assert.NotNull(popularity, "Invalid enum type.");

        var popularityVocabularyAnnotation = popularity.VocabularyAnnotations(model);
        Assert.Equal(1, popularityVocabularyAnnotation.Count(), "Invalid vocabulary annotation count.");

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(popularityVocabularyAnnotation, EdmExpressionKind.IntegerConstant, new List<PropertyValue> { new PropertyValue("22") });
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parsing_OutOfLineAnnotationEnumType()
    {
        var outOfLineAnnotationEnumType = VocabularyTestModelBuilder.OutOfLineAnnotationEnumType();
        var model = this.GetParserResult(outOfLineAnnotationEnumType);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var popularity = model.SchemaElements.Where(x => x.Name.Equals("Popularity")).Single() as IEdmEnumType;
        Assert.NotNull(popularity, "Invalid enum type.");

        var popularityVocabularyAnnotation = popularity.VocabularyAnnotations(model);
        Assert.Equal(1, popularityVocabularyAnnotation.Count(), "Invalid vocabulary annotation count.");

        List<PropertyValue> address = new List<PropertyValue>   { 
                                                                    new PropertyValue("Street", "foo"), 
                                                                    new PropertyValue("City", "bar")
                                                                };

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(popularityVocabularyAnnotation, EdmExpressionKind.Record, address);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parsing_InlineAnnotationEnumMember()
    {
        var model = this.GetParserResult(VocabularyTestModelBuilder.inlineAnnotationEnumMember());
        var valueAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, valueAnnotations.Count(), "Invalid annotation count.");

        //Call FindVocabularyAnnotations to get vocabulary annotations.
        var enumType = model.FindDeclaredType("DefaultNamespace.MyEnumType") as IEdmEnumType;
        var vocabularyAnnotatable = enumType.Members.First();
        var valueAnnotations1 = model.FindVocabularyAnnotations(vocabularyAnnotatable);
        Assert.Equal(1, valueAnnotations1.Count(), "Invalid annotation count.");

        var intValue = valueAnnotations1.First().Value as IEdmIntegerConstantExpression;
        Assert.NotNull(intValue, "Invalid integer value.");
        Assert.Equal(5, intValue.Value, "Invalid integer value.");
    }

    [Fact]
    public void Parsing_InlineAnnotationTerm()
    {
        var inLineAnnotationValueTerm = VocabularyTestModelBuilder.InlineAnnotationTerm();
        var model = this.GetParserResult(inLineAnnotationValueTerm);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var valueTerm = model.FindTerm("DefaultNamespace.ValueTerm");
        Assert.NotNull(valueTerm);

        var valueTermVocabularyAnnotation = valueTerm.VocabularyAnnotations(model);
        Assert.Equal(1, valueTermVocabularyAnnotation.Count(), "Invalid vocabulary annotation count.");

        List<PropertyValue> address = new List<PropertyValue>   { 
                                                                    new PropertyValue("Street", "foo"), 
                                                                    new PropertyValue("City", "bar")
                                                                };

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(valueTermVocabularyAnnotation, EdmExpressionKind.Record, address);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void Parsing_OutOfLineAnnotationTerm()
    {
        var outOfLineAnnotationValueTerm = VocabularyTestModelBuilder.OutOfLineAnnotationTerm();
        var model = this.GetParserResult(outOfLineAnnotationValueTerm);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var valueTerm = model.FindTerm("DefaultNamespace.ValueTerm");
        Assert.NotNull(valueTerm);

        var valueTermVocabularyAnnotation = valueTerm.VocabularyAnnotations(model);
        Assert.Equal(1, valueTermVocabularyAnnotation.Count(), "Invalid vocabulary annotation count.");

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(valueTermVocabularyAnnotation, EdmExpressionKind.IntegerConstant, new List<PropertyValue> { new PropertyValue("22") }, "Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm");
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    //[TestMethod, Variation(Id = 327, SkipReason = @"[EdmLib] Vocabulary annotation within vocabulary annotation can not be parsed by csdlreader -- postponed")]
    public void Parsing_InlineAnnotationInVocabularyAnnotation()
    {
        var inLineAnnotationInValueAnnotation = VocabularyTestModelBuilder.InlineAnnotationInVocabularyAnnotation();
        var model = this.GetParserResult(inLineAnnotationInValueAnnotation);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(2, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var entityContainer = model.FindEntityContainer("Container");
        Assert.NotNull(entityContainer);

        var entityContainerVocabularyAnnotation = entityContainer.VocabularyAnnotations(model);
        Assert.Equal(1, entityContainerVocabularyAnnotation.Count(), "Invalid vocabulary annotation count.");

        //To do - get vocabulary annotation from annotation
    }

    [Fact]
    public void Parsing_OutOfLineAnnotationInvalidAnnotationTarget()
    {
        var outOfLineAnnotationInvalidAnnotationTarget = VocabularyTestModelBuilder.OutOfLineAnnotationInvalidAnnotationTarget();
        var model = this.GetParserResult(outOfLineAnnotationInvalidAnnotationTarget);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
        var valueAnnotationFound = this.CheckForVocabularyAnnotation(vocabularyAnnotations, EdmExpressionKind.IntegerConstant, new List<PropertyValue> { new PropertyValue("22") }, "BadUnresolvedType:bar.foo");
        Assert.True(valueAnnotationFound, "Annotation can't be found.");

        var container = model.FindEntityContainer("Container");
        Assert.NotNull(container);
        Assert.Equal(0, container.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        var dogType = model.FindEntityType("DefaultNamespace.Dog");
        Assert.NotNull(dogType);
        Assert.Equal(0, dogType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        var dogName = dogType.FindProperty("Name");
        Assert.NotNull(dogName, "Invalid entity type property.");
        Assert.Equal(0, dogName.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        var ageTerm = model.FindTerm("AnnotationNamespace.Age");
        Assert.NotNull(ageTerm);
        Assert.Equal(0, ageTerm.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        var personAddressType = model.FindEntityType("AnnotationNamespace.PersonAddress");
        Assert.NotNull(personAddressType);
        Assert.Equal(0, personAddressType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        var addressType = model.FindType("AnnotationNamespace.Address");
        Assert.NotNull(addressType);
        Assert.Equal(0, addressType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");
    }

    [Fact]
    public void Parsing_OutOfLineAnnotationNamespaceAsAnnotationTarget()
    {
        var outOfLineAnnotationNamespaceAsAnnotationTarget = VocabularyTestModelBuilder.OutOfLineAnnotationNamespaceAsAnnotationTarget();
        var model = this.GetParserResult(outOfLineAnnotationNamespaceAsAnnotationTarget);

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        List<PropertyValue> personAddress = new List<PropertyValue>   { 
                                                                    new PropertyValue("Street", "foo"), 
                                                                    new PropertyValue("City", "bar")
                                                                };

        // BadUnresolvedType:.DefaultNamespace is not good name, but is to test the annotation is there.
        var valueAnnotationFound = this.CheckForVocabularyAnnotation(vocabularyAnnotations, EdmExpressionKind.Record, personAddress, "BadUnresolvedType:.DefaultNamespace");
        Assert.True(valueAnnotationFound, "Annotation can't be found.");

        var container = model.FindEntityContainer("Container");
        Assert.NotNull(container);
        Assert.Equal(0, container.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        var dogType = model.FindEntityType("DefaultNamespace.Dog");
        Assert.NotNull(dogType);
        Assert.Equal(0, dogType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        var dogName = dogType.FindProperty("Name");
        Assert.NotNull(dogName, "Invalid entity type property.");
        Assert.Equal(0, dogName.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");
    }

    [Fact]
    public void Parsing_AnnotationTargetToStringValue()
    {
        var outOfLineAnnotationEntityType = VocabularyTestModelBuilder.OutOfLineAnnotationEntityType();
        var entityTypeModel = this.GetParserResult(outOfLineAnnotationEntityType);

        var entityTypeAnnotations = entityTypeModel.FindEntityType("DefaultNamespace.Car").VocabularyAnnotations(entityTypeModel);
        Assert.Equal(1, entityTypeAnnotations.Count(), "Invalid annotation count.");

        Assert.Equal("DefaultNamespace.Car", entityTypeAnnotations.ElementAt(0).Target.ToString(), "Invalid target ToString.");

        var outOfLineValueAnnotationEntityProperty = VocabularyTestModelBuilder.OutOfLineAnnotationEntityProperty();
        var entityPropertyModel = this.GetParserResult(outOfLineValueAnnotationEntityProperty);

        var entityPropertyAnnotation = entityPropertyModel.FindEntityType("DefaultNamespace.Car").FindProperty("Wheels").VocabularyAnnotations(entityPropertyModel);
        Assert.Equal(1, entityPropertyAnnotation.Count(), "Invalid annotation count.");
        Assert.Equal("Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsProperty", entityPropertyAnnotation.ElementAt(0).Target.ToString(), "Invalid target ToString.");

        entityPropertyAnnotation = entityPropertyModel.FindEntityType("DefaultNamespace.Owner").FindProperty("Name").VocabularyAnnotations(entityPropertyModel);
        Assert.Equal(1, entityPropertyAnnotation.Count(), "Invalid annotation count.");
        Assert.Equal("Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsProperty", entityPropertyAnnotation.ElementAt(0).Target.ToString(), "Invalid target ToString.");
    }

    [Fact]
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
        Assert.False(isParsed, "SchemaReader.TryParse failed");
        Assert.Equal(2, errors.Count(), "Invalid error count.");
        Assert.Equal(EdmErrorCode.MissingAttribute, errors.ElementAt(0).ErrorCode);
        Assert.Equal(EdmErrorCode.MissingAttribute, errors.ElementAt(1).ErrorCode);
    }

    [Fact]
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
        Assert.Equal(6, valueAnnotations.Count(), "Invalid annotation count.");

        foreach (var valueAnnotation in valueAnnotations)
        {
            var boolValue = valueAnnotation.Value as IEdmBooleanConstantExpression;
            Assert.NotNull(boolValue, "Invalid boolean value.");
            Assert.Equal(true, boolValue.Value, "Invalid boolean value.");
        }
    }

    [Fact]
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
        Assert.Equal(6, valueAnnotations.Count(), "Invalid annotation count.");

        foreach (var valueAnnotation in valueAnnotations)
        {
            var boolValue = valueAnnotation.Value as IEdmBooleanConstantExpression;
            Assert.NotNull(boolValue, "Invalid boolean value.");
            Assert.Equal(false, boolValue.Value, "Invalid boolean value.");
        }
    }

    [Fact]
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

    [Fact]
    public void ConstructibleVocabularyAddingInlineVocabularyAnnotationToExistingElement()
    {
        EdmModel model = VocabularyTestModelBuilder.InlineAnnotationSimpleModel();

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var container = model.FindEntityContainer("Container") as EdmEntityContainer;
        Assert.NotNull(container, "Invalid entity container name.");

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
        Assert.Equal(2, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        List<PropertyValue> listOfNames = new List<PropertyValue>   { 
                                                                        new PropertyValue("Joe"), 
                                                                        new PropertyValue("Mary"),
                                                                        new PropertyValue("Justin")
                                                                    };

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(vocabularyAnnotations, EdmExpressionKind.Collection, listOfNames);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");

        var containerVocabularyAnnotations = container.VocabularyAnnotations(model);
        valueAnnotationFound = this.CheckForVocabularyAnnotation(containerVocabularyAnnotations, EdmExpressionKind.Collection, listOfNames);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
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

        Assert.Equal(expected, actual);
    }

    // TODO: Make 'Org.OData.Core.V1" a reserved namespace, and turn 'Core' ot 'Org.OData.Core.V1'
    [Fact]
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

        Assert.Equal(overwritePersonDescription, model.GetDescriptionAnnotation(personType));
        Assert.Equal(personTypeLongDescription, model.GetLongDescriptionAnnotation(personType));
        Assert.Equal(structuralPropertyDescription, model.GetDescriptionAnnotation(structuralProperty));
        Assert.Equal(propertyIdDescription, model.GetDescriptionAnnotation(propertyId));
        Assert.Equal(stringTermDescription, model.GetDescriptionAnnotation(stringTerm));
        Assert.Equal(entitySetDescription, model.GetDescriptionAnnotation(entitySet));
        Assert.Equal(singletonDescription, model.GetDescriptionAnnotation(singleton));
        Assert.Null(model.GetDescriptionAnnotation(driverSet));

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

        Assert.Equal(expected, actual);

        IEdmModel parsedModel;
        IEnumerable<EdmError> errs;
        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(expected)) }, out parsedModel, out errs);
        Assert.True(parsed);
        Assert.Empty(errors);

        Assert.Equal(null, parsedModel.GetDescriptionAnnotation(personType));
        Assert.Equal(null, parsedModel.GetDescriptionAnnotation(structuralProperty));
        Assert.Equal(null, parsedModel.GetDescriptionAnnotation(propertyId));
        Assert.Equal(null, parsedModel.GetDescriptionAnnotation(stringTerm));
        Assert.Equal(null, parsedModel.GetDescriptionAnnotation(entitySet));
        Assert.Equal(null, parsedModel.GetDescriptionAnnotation(singleton));

        var parsedPersonType = parsedModel.FindType("Ns.Person") as IEdmEntityType;
        Assert.NotNull(parsedPersonType);
        Assert.Equal(overwritePersonDescription, parsedModel.GetDescriptionAnnotation(parsedPersonType));
        Assert.Equal(personTypeLongDescription, parsedModel.GetLongDescriptionAnnotation(parsedPersonType));
        Assert.Equal(structuralPropertyDescription, parsedModel.GetDescriptionAnnotation((parsedPersonType.FindProperty("Concurrency"))));
        Assert.Equal(propertyIdDescription, parsedModel.GetDescriptionAnnotation(parsedPersonType.FindProperty("Id")));
        Assert.Equal(stringTermDescription, parsedModel.GetDescriptionAnnotation(parsedModel.FindTerm("Ns.HomeAddress")));
        Assert.Equal(entitySetDescription, parsedModel.GetDescriptionAnnotation(parsedModel.FindDeclaredEntitySet("People")));
        Assert.Equal(singletonDescription, parsedModel.GetDescriptionAnnotation(parsedModel.FindDeclaredSingleton("Boss")));
        Assert.Null(parsedModel.GetDescriptionAnnotation(parsedModel.FindDeclaredEntitySet("Drivers")));
    }

    [Fact]
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

        Assert.Equal(expected, actual);
    }

    [Fact]
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
        Assert.True(parsed);
        Assert.Empty(errors);

        // EntitySet: People
        var peopleSet = parsedModel.FindDeclaredEntitySet("People");
        IEdmVocabularyAnnotation annotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(peopleSet, CapabilitiesVocabularyModel.ChangeTrackingTerm).FirstOrDefault();
        Assert.NotNull(annotation);
        IEdmRecordExpression record = annotation.Value as IEdmRecordExpression;
        Assert.NotNull(record);
        IEdmBooleanConstantExpression supported = record.FindProperty("Supported").Value as IEdmBooleanConstantExpression;
        Assert.NotNull(supported);
        Assert.True(supported.Value);
        IEdmCollectionExpression filterable = record.FindProperty("FilterableProperties").Value as IEdmCollectionExpression;
        Assert.NotNull(filterable);
        Assert.Equal(((IEdmPathExpression)filterable.Elements.Single()).PathSegments.Single(), "Age");
        IEdmCollectionExpression expandable = record.FindProperty("ExpandableProperties").Value as IEdmCollectionExpression;
        Assert.NotNull(expandable);
        Assert.Equal(((IEdmPathExpression)expandable.Elements.Single()).PathSegments.Single(), "MyDept");

        // EntitySet: Depts
        var deptsSet = parsedModel.FindDeclaredEntitySet("Depts");
        annotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(deptsSet, CapabilitiesVocabularyModel.ChangeTrackingTerm).FirstOrDefault();
        Assert.NotNull(annotation);
        record = annotation.Value as IEdmRecordExpression;
        Assert.NotNull(record);
        supported = record.FindProperty("Supported").Value as IEdmBooleanConstantExpression;
        Assert.NotNull(supported);
        Assert.False(supported.Value);
        filterable = record.FindProperty("FilterableProperties").Value as IEdmCollectionExpression;
        Assert.NotNull(filterable);
        Assert.False(filterable.Elements.Any());
        expandable = record.FindProperty("ExpandableProperties").Value as IEdmCollectionExpression;
        Assert.NotNull(expandable);
        Assert.False(expandable.Elements.Any());
    }

    [Fact]
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

        Assert.Equal(expected, actual);
    }

    [Fact]
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
        Assert.True(parsed);
        Assert.Empty(errors);

        var container = parsedModel.FindEntityContainer("Container");
        IEdmVocabularyAnnotation annotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(container, CapabilitiesVocabularyModel.ChangeTrackingTerm).FirstOrDefault();
        Assert.NotNull(annotation);
        IEdmRecordExpression record = annotation.Value as IEdmRecordExpression;
        Assert.NotNull(record);
        IEdmBooleanConstantExpression supported = record.FindProperty("Supported").Value as IEdmBooleanConstantExpression;
        Assert.NotNull(supported);
        Assert.True(supported.Value);
        IEdmCollectionExpression filterable = record.FindProperty("FilterableProperties").Value as IEdmCollectionExpression;
        Assert.NotNull(filterable);
        Assert.False(filterable.Elements.Any());
        IEdmCollectionExpression expandable = record.FindProperty("ExpandableProperties").Value as IEdmCollectionExpression;
        Assert.NotNull(expandable);
        Assert.False(expandable.Elements.Any());
    }

    [Fact]
    public void TestAuthorizationAnnotationInlineAndOutLineAnnotationOnEntitySetAndSingleton()
    {
        EdmModel model = new EdmModel();

        EdmEntityType entity = new EdmEntityType("NS", "Entity");
        EdmStructuralProperty deptId = entity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        entity.AddKeys(deptId);
        model.AddElement(entity);

        EdmEntityContainer container = new EdmEntityContainer("NS", "Container");
        var entitySet = container.AddEntitySet("Entities", entity);
        var singleton = container.AddSingleton("Me", entity);
        model.AddElement(container);

        IEdmTerm term = model.FindTerm("Org.OData.Authorization.V1.Authorizations");
        Assert.NotNull(term);

        // OpenIDConnect
        IEdmComplexType complexType = model.FindType("Org.OData.Authorization.V1.OpenIDConnect") as IEdmComplexType;
        Assert.NotNull(complexType);
        IList<IEdmPropertyConstructor> properties = new List<IEdmPropertyConstructor>();
        properties.Add(new EdmPropertyConstructor("IssuerUrl", new EdmStringConstant("http://any")));
        properties.Add(new EdmPropertyConstructor("Name", new EdmStringConstant("OpenIDConnect Name")));
        properties.Add(new EdmPropertyConstructor("Description", new EdmStringConstant("OpenIDConnect Description")));
        EdmRecordExpression record1 = new EdmRecordExpression(new EdmComplexTypeReference(complexType, true), properties);

        complexType = model.FindType("Org.OData.Authorization.V1.Http") as IEdmComplexType;
        Assert.NotNull(complexType);
        properties = new List<IEdmPropertyConstructor>();
        properties.Add(new EdmPropertyConstructor("BearerFormat", new EdmStringConstant("Http BearerFormat")));
        properties.Add(new EdmPropertyConstructor("Scheme", new EdmStringConstant("Http Scheme")));
        properties.Add(new EdmPropertyConstructor("Name", new EdmStringConstant("Http Name")));
        properties.Add(new EdmPropertyConstructor("Description", new EdmStringConstant("Http Description")));
        EdmRecordExpression record2 = new EdmRecordExpression(new EdmComplexTypeReference(complexType, true), properties);

        EdmCollectionExpression collection = new EdmCollectionExpression(record1, record2);

        // for entity set
        EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(entitySet, term, collection);
        annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
        model.SetVocabularyAnnotation(annotation);

        // for singleton
        annotation = new EdmVocabularyAnnotation(singleton, term, collection);
        annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        model.SetVocabularyAnnotation(annotation);

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
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Entities"" EntityType=""NS.Entity"">
      <Annotation Term=""Org.OData.Authorization.V1.Authorizations"">
        <Collection>
          <Record Type=""Org.OData.Authorization.V1.OpenIDConnect"">
            <PropertyValue Property=""IssuerUrl"" String=""http://any"" />
            <PropertyValue Property=""Name"" String=""OpenIDConnect Name"" />
            <PropertyValue Property=""Description"" String=""OpenIDConnect Description"" />
          </Record>
          <Record Type=""Org.OData.Authorization.V1.Http"">
            <PropertyValue Property=""BearerFormat"" String=""Http BearerFormat"" />
            <PropertyValue Property=""Scheme"" String=""Http Scheme"" />
            <PropertyValue Property=""Name"" String=""Http Name"" />
            <PropertyValue Property=""Description"" String=""Http Description"" />
          </Record>
        </Collection>
      </Annotation>
    </EntitySet>
    <Singleton Name=""Me"" Type=""NS.Entity"" />
  </EntityContainer>
  <Annotations Target=""NS.Container/Me"">
    <Annotation Term=""Org.OData.Authorization.V1.Authorizations"">
      <Collection>
        <Record Type=""Org.OData.Authorization.V1.OpenIDConnect"">
          <PropertyValue Property=""IssuerUrl"" String=""http://any"" />
          <PropertyValue Property=""Name"" String=""OpenIDConnect Name"" />
          <PropertyValue Property=""Description"" String=""OpenIDConnect Description"" />
        </Record>
        <Record Type=""Org.OData.Authorization.V1.Http"">
          <PropertyValue Property=""BearerFormat"" String=""Http BearerFormat"" />
          <PropertyValue Property=""Scheme"" String=""Http Scheme"" />
          <PropertyValue Property=""Name"" String=""Http Name"" />
          <PropertyValue Property=""Description"" String=""Http Description"" />
        </Record>
      </Collection>
    </Annotation>
  </Annotations>
</Schema>";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ParsingAuthorizationAnnotationInlineAndOutLineAnnotationOnEntityContainerAndEntitySet()
    {
        const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Entities"" EntityType=""NS.Entity"">
      <Annotation Term=""Org.OData.Authorization.V1.Authorizations"">
        <Collection>
          <Record Type=""Org.OData.Authorization.V1.OpenIDConnect"">
            <PropertyValue Property=""IssuerUrl"" String=""http://any"" />
            <PropertyValue Property=""Name"" String=""OpenIDConnect Name"" />
            <PropertyValue Property=""Description"" String=""OpenIDConnect Description"" />
          </Record>
        </Collection>
      </Annotation>
    </EntitySet>
    <Singleton Name=""Me"" Type=""NS.Entity"" />
  </EntityContainer>
  <Annotations Target=""NS.Container"">
    <Annotation Term=""Org.OData.Authorization.V1.Authorizations"">
      <Collection>
        <Record Type=""Org.OData.Authorization.V1.OAuth2ClientCredentials"">
          <PropertyValue Property=""TokenUrl"" String=""http://TokenUrl"" />
          <PropertyValue Property=""RefreshUrl"" String=""http://RefreshUrl"" />
          <PropertyValue Property=""Name"" String=""OAuth2ClientCredentials Name"" />
          <PropertyValue Property=""Description"" String=""OAuth2ClientCredentials Description"" />
          <PropertyValue Property=""Scopes"">
            <Collection>
              <Record>
                 <PropertyValue Property=""Scope"" String=""Scope1"" />
                 <PropertyValue Property=""Description"" String=""Description 1"" />
              </Record>
            </Collection>
          </PropertyValue>
        </Record>
        <Record Type=""Org.OData.Authorization.V1.Http"">
          <PropertyValue Property=""BearerFormat"" String=""Http BearerFormat"" />
          <PropertyValue Property=""Scheme"" String=""Http Scheme"" />
          <PropertyValue Property=""Name"" String=""Http Name"" />
          <PropertyValue Property=""Description"" String=""Http Description"" />
        </Record>
      </Collection>
    </Annotation>
  </Annotations>
</Schema>";

        IEdmModel parsedModel;
        IEnumerable<EdmError> errors;
        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out parsedModel, out errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmTerm term = parsedModel.FindTerm("Org.OData.Authorization.V1.Authorizations");
        Assert.NotNull(term);

        var container = parsedModel.EntityContainer;
        Assert.NotNull(container);

        // entity container
        IEdmVocabularyAnnotation annotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(container, term).FirstOrDefault();
        Assert.NotNull(annotation);

        IEdmCollectionExpression collection = annotation.Value as IEdmCollectionExpression;
        Assert.NotNull(collection);
        Assert.Equal(2, collection.Elements.Count());

        // First one on container
        IEdmRecordExpression record = collection.Elements.First() as IEdmRecordExpression;
        Assert.NotNull(record);
        Assert.Equal("Org.OData.Authorization.V1.OAuth2ClientCredentials", record.DeclaredType.FullName());

        IEdmStringConstantExpression description = record.FindProperty("Description").Value as IEdmStringConstantExpression;
        Assert.NotNull(description);
        Assert.Equal("OAuth2ClientCredentials Description", description.Value);

        // second one on container
        record = collection.Elements.Last() as IEdmRecordExpression;
        Assert.NotNull(record);
        Assert.Equal("Org.OData.Authorization.V1.Http", record.DeclaredType.FullName());

        description = record.FindProperty("Description").Value as IEdmStringConstantExpression;
        Assert.NotNull(description);
        Assert.Equal("Http Description", description.Value);

        // entity set
        IEdmEntitySet entitySet = container.FindEntitySet("Entities");
        Assert.NotNull(entitySet);

        annotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(entitySet, term).FirstOrDefault();
        Assert.NotNull(annotation);

        collection = annotation.Value as IEdmCollectionExpression;
        Assert.NotNull(collection);
        Assert.Equal(1, collection.Elements.Count());

        record = collection.Elements.First() as IEdmRecordExpression;
        Assert.NotNull(record);
        Assert.Equal("Org.OData.Authorization.V1.OpenIDConnect", record.DeclaredType.FullName());

        description = record.FindProperty("Description").Value as IEdmStringConstantExpression;
        Assert.NotNull(description);
        Assert.Equal("OpenIDConnect Description", description.Value);
    }

    [Fact]
    public void TestValidationAnnotationOpenPropertyTypeConstraintInlineOnEntityType()
    {
        EdmModel model = new EdmModel();

        EdmEntityType openEntityType = new EdmEntityType("NS", "Entity", null, false, true);
        EdmStructuralProperty deptId = openEntityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        openEntityType.AddKeys(deptId);
        model.AddElement(openEntityType);

        IEdmSchemaType qualifiedType = model.FindType("Org.OData.Core.V1.QualifiedTypeName");
        Assert.NotNull(qualifiedType);

        IEdmTerm term = model.FindTerm("Org.OData.Validation.V1.OpenPropertyTypeConstraint");
        Assert.NotNull(term);

        EdmCollectionExpression collection = new EdmCollectionExpression(new EdmStringConstant("NS.MyType1"),
            new EdmStringConstant("NS.MyType2"));

        EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(openEntityType, term, collection);
        annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
        model.SetVocabularyAnnotation(annotation);

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
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity"" OpenType=""true"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Annotation Term=""Org.OData.Validation.V1.OpenPropertyTypeConstraint"">
      <Collection>
        <String>NS.MyType1</String>
        <String>NS.MyType2</String>
      </Collection>
    </Annotation>
  </EntityType>
</Schema>";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ParsingValidationAnnotationOpenPropertyTypeConstraintInlineOnEntityType()
    {
        const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity"" OpenType=""true"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Annotation Term=""Org.OData.Validation.V1.OpenPropertyTypeConstraint"">
      <Collection>
        <String>NS.MyType1</String>
        <String>NS.MyType2</String>
      </Collection>
    </Annotation>
  </EntityType>
</Schema>";

        IEdmModel parsedModel;
        IEnumerable<EdmError> errors;
        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out parsedModel, out errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmTerm term = parsedModel.FindTerm("Org.OData.Validation.V1.OpenPropertyTypeConstraint");
        Assert.NotNull(term);

        var entityType = parsedModel.SchemaElements.OfType<IEdmEntityType>().First();
        Assert.NotNull(entityType);

        // entity type
        IEdmVocabularyAnnotation annotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(entityType, term).FirstOrDefault();
        Assert.NotNull(annotation);

        IEdmCollectionExpression collection = annotation.Value as IEdmCollectionExpression;
        Assert.NotNull(collection);
        Assert.Equal(2, collection.Elements.Count());

        // First value
        IEdmStringConstantExpression first = collection.Elements.First() as IEdmStringConstantExpression;
        Assert.NotNull(first);
        Assert.Equal("NS.MyType1", first.Value);

        // second value
        IEdmStringConstantExpression second = collection.Elements.Last() as IEdmStringConstantExpression;
        Assert.NotNull(second);
        Assert.Equal("NS.MyType2", second.Value);
    }

    [Fact]
    public void ParsingValidationAnnotationDerivedTypeConstraintOutOfLineOnProperty()
    {
        const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity"" OpenType=""true"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Address"" Type=""NS.Address"" />
  </EntityType>
  <ComplexType Name=""Address"" />
  <ComplexType Name=""UsAddress"" BaseType=""NS.Address"" />
  <ComplexType Name=""CnAddress"" BaseType=""NS.Address"" />
  <Annotations Target=""NS.Entity/Address"">
    <Annotation Term=""Org.OData.Validation.V1.DerivedTypeConstraint"">
      <Collection>
        <String>NS.UsAddress</String>
      </Collection>
    </Annotation>
  </Annotations>
</Schema>";

        IEdmModel parsedModel;
        IEnumerable<EdmError> errors;
        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out parsedModel, out errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmTerm term = parsedModel.FindTerm("Org.OData.Validation.V1.DerivedTypeConstraint");
        Assert.NotNull(term);

        var entityType = parsedModel.SchemaElements.OfType<IEdmEntityType>().First();
        Assert.NotNull(entityType);

        var property = entityType.Properties().FirstOrDefault(p => p.Name == "Address");
        Assert.NotNull(property);

        IEdmVocabularyAnnotation annotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(property, term).FirstOrDefault();
        Assert.NotNull(annotation);

        IEdmCollectionExpression collection = annotation.Value as IEdmCollectionExpression;
        Assert.NotNull(collection);
        Assert.Equal(1, collection.Elements.Count());

        IEdmStringConstantExpression value = collection.Elements.Single() as IEdmStringConstantExpression;
        Assert.NotNull(value);
        Assert.Equal("NS.UsAddress", value.Value);
    }

    [Fact]
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

        Assert.Equal(expected, actual);
    }

    [Fact]
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
        Assert.True(parsed);
        Assert.Empty(errors);

        var bookType = parsedModel.FindDeclaredType("ns.book");
        Assert.NotNull(bookType);
        IEdmVocabularyAnnotation annotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(bookType, AlternateKeysVocabularyModel.AlternateKeysTerm).FirstOrDefault();
        Assert.NotNull(annotation);
        IEdmCollectionExpression collect = annotation.Value as IEdmCollectionExpression;
        Assert.NotNull(collect);
        Assert.Equal(2, collect.Elements.Count());

        // #1
        var record = collect.Elements.First() as IEdmRecordExpression;
        Assert.NotNull(record);
        IEdmCollectionExpression keyCollection = record.FindProperty("Key").Value as IEdmCollectionExpression;
        Assert.NotNull(keyCollection);
        Assert.Equal(1, keyCollection.Elements.Count());
        IEdmRecordExpression element = keyCollection.Elements.First() as IEdmRecordExpression;
        Assert.NotNull(element);
        IEdmStringConstantExpression alias = element.FindProperty("Alias").Value as IEdmStringConstantExpression;
        Assert.NotNull(alias);
        Assert.Equal("s2", alias.Value);
        IEdmPathExpression name = element.FindProperty("Name").Value as IEdmPathExpression;
        Assert.NotNull(name);
        Assert.Equal("prop2", name.PathSegments.Single());

        // #2
        record = collect.Elements.Last() as IEdmRecordExpression;
        Assert.NotNull(record);
        keyCollection = record.FindProperty("Key").Value as IEdmCollectionExpression;
        Assert.NotNull(keyCollection);
        Assert.Equal(2, keyCollection.Elements.Count());

        // #2.1
        element = keyCollection.Elements.First() as IEdmRecordExpression;
        Assert.NotNull(element);
        alias = element.FindProperty("Alias").Value as IEdmStringConstantExpression;
        Assert.NotNull(alias);
        Assert.Equal("s3", alias.Value);
        name = element.FindProperty("Name").Value as IEdmPathExpression;
        Assert.NotNull(name);
        Assert.Equal("prop3", name.PathSegments.Single());

        // #2.2
        element = keyCollection.Elements.Last() as IEdmRecordExpression;
        Assert.NotNull(element);
        alias = element.FindProperty("Alias").Value as IEdmStringConstantExpression;
        Assert.NotNull(alias);
        Assert.Equal("s4", alias.Value);
        name = element.FindProperty("Name").Value as IEdmPathExpression;
        Assert.NotNull(name);
        Assert.Equal("prop4", name.PathSegments.Single());
    }

    [Fact]
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

        Assert.Equal(1, idAnnotationValue.Count());
        Assert.Equal(2, structuralPropertyAnnotationValue.Count());
        Assert.Equal(3, entitySetAnnotationValue.Count());
        Assert.Null(enumTypeAnnotationValue);

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

        Assert.Empty(errors);
        Assert.Equal(expected, actual);
    }

    [Fact]
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
        Assert.True(parsed);

        var parsedPersonType = parsedModel.FindType("Ns.Person") as IEdmEntityType;
        var term = parsedModel.FindDeclaredTerm("Ns.Permission");
        var parsedIdAnnotationValue = (parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedPersonType.FindProperty("Id"), term).FirstOrDefault().Value as IEdmEnumMemberExpression).EnumMembers;
        var parsedStructuralAnnotationValue = (parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedPersonType.FindProperty("Concurrency"), term).FirstOrDefault().Value as IEdmEnumMemberExpression).EnumMembers;
        var parsedEntitySetAnnotationValue = (parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedModel.FindDeclaredEntitySet("People"), term).FirstOrDefault().Value as IEdmEnumMemberExpression).EnumMembers;

        Assert.Equal(1, parsedIdAnnotationValue.Count());
        Assert.Equal(1, parsedIdAnnotationValue.Single().Value.Value);

        Assert.Equal(2, parsedStructuralAnnotationValue.Count());
        Assert.Equal(1, parsedStructuralAnnotationValue.First().Value.Value);
        Assert.Equal(2, parsedStructuralAnnotationValue.Last().Value.Value);

        Assert.Equal(3, parsedEntitySetAnnotationValue.Count());
        Assert.Equal(1, parsedEntitySetAnnotationValue.First().Value.Value);
        Assert.Equal(2, parsedEntitySetAnnotationValue.ElementAt(1).Value.Value);
        Assert.Equal(4, parsedEntitySetAnnotationValue.Last().Value.Value);

        var peopleAnnotations = parsedModel.FindVocabularyAnnotations(parsedModel.FindDeclaredEntitySet("People"));
        Assert.Equal(2, peopleAnnotations.Count());
        var navigabilityAnnotation = peopleAnnotations.First();
        IEdmRecordExpression navigabilityRecord = (IEdmRecordExpression)navigabilityAnnotation.Value;
        var navigabilityProperty = navigabilityRecord.FindProperty("Navigability");
        var navigabilityEnumExpression = (IEdmEnumMemberExpression)navigabilityProperty.Value;

        Assert.Equal(3, navigabilityEnumExpression.EnumMembers.Count());
        Assert.Equal("None", navigabilityEnumExpression.EnumMembers.First().Name);
        Assert.Equal("Single", navigabilityEnumExpression.EnumMembers.ElementAt(1).Name);
        Assert.Equal("Recursive", navigabilityEnumExpression.EnumMembers.ElementAt(2).Name);

        var innerNavigabilityProperty =
            ((IEdmRecordExpression)((IEdmCollectionExpression)navigabilityRecord.FindProperty("RestrictedProperties").Value).Elements
                .First()).FindProperty("Navigability");

        var innerNavigabilityEnumExpression = (IEdmEnumMemberExpression)innerNavigabilityProperty.Value;

        Assert.Equal(1, innerNavigabilityEnumExpression.EnumMembers.Count());
        Assert.Equal("Recursive", innerNavigabilityEnumExpression.EnumMembers.First().Name);

    }

    [Fact]
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
        Assert.True(parsed);

        var parsedPersonType = parsedModel.FindType("Ns.Person") as IEdmEntityType;
        var term = parsedModel.FindDeclaredTerm("Ns.Permission");
        var parsedIdAnnotationValue = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedPersonType.FindProperty("Id"), term).FirstOrDefault().Value as IEdmEnumMemberExpression;
        var parsedIdAnnotationErrors = parsedIdAnnotationValue.Errors();
        var parsedStructuralAnnotationValue = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedPersonType.FindProperty("Concurrency"), term).FirstOrDefault().Value as IEdmEnumMemberExpression;
        var parsedStructuralAnnotationErrors = parsedStructuralAnnotationValue.Errors();
        var parsedEntitySetAnnotationValue = (parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedModel.FindDeclaredEntitySet("People"), term).FirstOrDefault().Value as IEdmEnumMemberExpression).EnumMembers;

        Assert.Null(parsedIdAnnotationValue.EnumMembers);
        Assert.Equal("'Ns.Permissions/ReadWrite' is not a valid enum member path.", parsedIdAnnotationErrors.Last().ErrorMessage);

        Assert.Null(parsedStructuralAnnotationValue.EnumMembers);
        Assert.Equal("'Ns.Permissions/Read Ns.Permissions/Modify' is not a valid enum member path.", parsedStructuralAnnotationErrors.Last().ErrorMessage);

        Assert.Equal(3, parsedEntitySetAnnotationValue.Count());
        Assert.Equal(1, parsedEntitySetAnnotationValue.First().Value.Value);
        Assert.Equal(2, parsedEntitySetAnnotationValue.ElementAt(1).Value.Value);
        Assert.Equal(4, parsedEntitySetAnnotationValue.Last().Value.Value);
    }

    [Fact]
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
        Assert.False(parsed);
        Assert.Equal(2, errs.Count());
        Assert.Equal("'Ns.Permissions/Read Ns.Color/Write' is not a valid enum member path.", errs.First().ErrorMessage);
        Assert.Equal("'Ns1.Permissions/Read Ns.Permissions/Write Ns.Permissions/Delete' is not a valid enum member path.", errs.Last().ErrorMessage);
    }

    [Fact]
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

        Assert.Equal(read, idAnnotationValue);
        Assert.Equal(2, structuralPropertyAnnotationValue.Count());
        Assert.Equal(delete, entitySetAnnotationValue);
        Assert.Null(enumTypeAnnotationValue);

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

        Assert.Empty(errors);
        Assert.Equal(expected, actual);
    }


    [Fact]
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
        Assert.False(parsed);
        Assert.Equal(2, errs.Count());
        Assert.Equal("The schema element 'EnumMemberReference' was not expected in the given context.", errs.First().ErrorMessage);
        Assert.Equal("The attribute 'EnumMemberReference' was not expected in the given context.", errs.Last().ErrorMessage);
    }

    [Fact]
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

        Assert.Equal(true, idAnnotationValue);
        Assert.Equal(false, structuralPropertyAnnotationValue);
        Assert.Equal(true, stringTermAnnotationValue);
        Assert.Null(entitySetAnnotation);

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

        Assert.Equal(expected, actual);

        IEdmModel parsedModel;
        IEnumerable<EdmError> errs;
        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(expected)) }, out parsedModel, out errs);
        Assert.True(parsed);
        Assert.Empty(errors);

        var idAnnotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(propertyId, term).FirstOrDefault();
        var propertyAnnotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(structuralProperty, term).FirstOrDefault();
        var termAnnotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(stringTerm, term).FirstOrDefault();
        entitySetAnnotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(entitySet, term).FirstOrDefault();

        Assert.Equal(null, propertyAnnotation);
        Assert.Equal(null, idAnnotation);
        Assert.Equal(null, termAnnotation);
        Assert.Equal(null, entitySetAnnotation);

        var parsedPersonType = parsedModel.FindType("Ns.Person") as IEdmEntityType;
        idAnnotationValue = (parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedPersonType.FindProperty("Id"), term).FirstOrDefault().Value as IEdmBooleanConstantExpression).Value;
        structuralPropertyAnnotationValue = (parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedPersonType.FindProperty("Concurrency"), term).FirstOrDefault().Value as IEdmBooleanConstantExpression).Value;
        stringTermAnnotationValue = (parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedModel.FindTerm("Ns.HomeAddress"), term).FirstOrDefault().Value as IEdmBooleanConstantExpression).Value;
        entitySetAnnotation = parsedModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(parsedModel.FindDeclaredEntitySet("People"), term).FirstOrDefault();

        Assert.NotNull(parsedPersonType);
        Assert.Equal(false, structuralPropertyAnnotationValue);
        Assert.Equal(true, idAnnotationValue);
        Assert.Equal(true, stringTermAnnotationValue);
        Assert.Null(entitySetAnnotation);
    }

    [Fact]
    public void ConstructibleVocabularyAddingOutOfLineVocabularyAnnotationToExistingElement()
    {
        EdmModel model = VocabularyTestModelBuilder.InlineAnnotationSimpleModel();

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var container = model.FindEntityContainer("Container") as EdmEntityContainer;
        Assert.NotNull(container, "Invalid entity container name.");

        var person = model.FindEntityType("AnnotationNamespace.Person") as EdmEntityType;
        Assert.NotNull(person);

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
        Assert.Equal(2, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        List<PropertyValue> listOfNames = new List<PropertyValue>   { 
                                                                        new PropertyValue("Id", "22"), 
                                                                        new PropertyValue("Name", "Johnny")
                                                                    };

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(vocabularyAnnotations, EdmExpressionKind.Record, listOfNames);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");

        var containerVocabularyAnnotations = container.VocabularyAnnotations(model);
        valueAnnotationFound = this.CheckForVocabularyAnnotation(containerVocabularyAnnotations, EdmExpressionKind.Record, listOfNames);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void ConstructibleVocabularyAddingVocabularyAnnotationToNewElement()
    {
        EdmModel model = VocabularyTestModelBuilder.InlineAnnotationSimpleModel();

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var container = model.FindEntityContainer("Container") as EdmEntityContainer;
        Assert.NotNull(container, "Invalid entity container name.");

        var carType = model.FindEntityType("DefaultNamespace.Car");
        Assert.NotNull(carType);

        EdmEntitySet carSet = container.AddEntitySet("CarSet", carType);

        var person = model.FindEntityType("AnnotationNamespace.Person") as EdmEntityType;
        Assert.NotNull(person);

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
        Assert.Equal(2, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        List<PropertyValue> listOfNames = new List<PropertyValue>   { 
                                                                        new PropertyValue("Id", "22"), 
                                                                        new PropertyValue("Name", "Johnny")
                                                                    };

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(vocabularyAnnotations, EdmExpressionKind.Record, listOfNames);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");

        var containerCarSet = container.EntitySets().Where(x => x.Name.Equals("CarSet")).SingleOrDefault() as EdmEntitySet;
        Assert.NotNull(containerCarSet, "Entity set did not get added to container properly.");

        var carSetVocabularAnnotation = containerCarSet.VocabularyAnnotations(model);
        Assert.Equal(1, carSetVocabularAnnotation.Count(), "Invalid vocabulary annotation count.");

        valueAnnotationFound = this.CheckForVocabularyAnnotation(carSetVocabularAnnotation, EdmExpressionKind.Record, listOfNames);
        Assert.True(valueAnnotationFound, "Annotation can't be found.");
    }

    [Fact]
    public void ConstructibleVocabularyRemovingAnnotationToExistElement()
    {
        var model = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(VocabularyTestModelBuilder.InlineAnnotationSimpleModel());
        Assert.Equal(1, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var carType = model.FindEntityType("DefaultNamespace.Car");
        Assert.NotNull(carType);
        var carWheels = carType.FindProperty("Wheels");
        Assert.NotNull(carWheels, "Invalid entity type property.");

        var carWheelsVocabularyAnnotations = carWheels.VocabularyAnnotations(model);
        Assert.Equal(1, carWheelsVocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var valueAnnotation = carWheelsVocabularyAnnotations.ElementAt(0);
        model.RemoveVocabularyAnnotation(valueAnnotation);
        Assert.Equal(0, carWheelsVocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
        Assert.Equal(0, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
    }

    [Fact]
    public void ConstructibleVocabularyRemovingVocabularyAnnotationToNewElement()
    {
        var model = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(VocabularyTestModelBuilder.InlineAnnotationSimpleModel());
        Assert.Equal(1, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var carType = model.FindEntityType("DefaultNamespace.Car");
        Assert.NotNull(carType);
        Assert.Equal(0, carType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        EdmTerm hiddenName = new EdmTerm("AnnotationNamespace", "HiddenName", EdmCoreModel.Instance.GetString(true));
        model.WrappedModel.AddElement(hiddenName);

        EdmVocabularyAnnotation valueAnnotation = new EdmVocabularyAnnotation(
            carType,
            hiddenName,
            new EdmStringConstant("Gray"));

        model.WrappedModel.AddVocabularyAnnotation(valueAnnotation);
        Assert.Equal(2, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
        Assert.Equal(1, carType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(carType.VocabularyAnnotations(model), EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("Gray") });
        Assert.True(valueAnnotationFound, "Annotation cannot be found.");

        model.RemoveVocabularyAnnotation(valueAnnotation);
        Assert.Equal(1, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
        Assert.Equal(0, carType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");
    }

    [Fact]
    public void ConstructibleVocabularyAddingVocabularyAnnotationAndDeleteTargetedElement()
    {
        var model = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(VocabularyTestModelBuilder.InlineAnnotationSimpleModel());

        var vocabularyAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var container = model.FindEntityContainer("Container") as EdmEntityContainer;
        Assert.NotNull(container, "Invalid entity container name.");

        var stringTerm = model.FindTerm("AnnotationNamespace.StringTerm") as EdmTerm;
        Assert.NotNull(stringTerm);

        EdmVocabularyAnnotation valueAnnotation = new EdmVocabularyAnnotation(
            container,
            stringTerm,
            new EdmStringConstant("foo"));
        valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
        model.WrappedModel.AddVocabularyAnnotation(valueAnnotation);
        Assert.Equal(2, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(vocabularyAnnotations, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("foo") });
        Assert.True(valueAnnotationFound, "Annotation can't be found.");

        var containerVocabularyAnnotations = container.VocabularyAnnotations(model);
        valueAnnotationFound = this.CheckForVocabularyAnnotation(containerVocabularyAnnotations, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("foo") });
        Assert.True(valueAnnotationFound, "Annotation can't be found.");

        model.RemoveElement(container);
        model.FindDeclaredVocabularyAnnotations(container).ToList().ForEach(a => model.RemoveVocabularyAnnotation(a));

        Assert.Equal(1, vocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
    }

    [Fact]
    public void ConstructibleVocabularyEditingVocabularyAnnotationToExistElement()
    {
        EdmModel model = VocabularyTestModelBuilder.InlineAnnotationSimpleModel();
        Assert.Equal(1, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var carType = model.FindEntityType("DefaultNamespace.Car");
        Assert.NotNull(carType);
        var carWheels = carType.FindProperty("Wheels");
        Assert.NotNull(carWheels, "Invalid entity type property.");

        var carWheelsVocabularyAnnotations = carWheels.VocabularyAnnotations(model);
        Assert.Equal(1, carWheelsVocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(carWheelsVocabularyAnnotations, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("foo") });
        Assert.True(valueAnnotationFound, "Annotation can't be found.");

        var valueAnnotation = carWheelsVocabularyAnnotations.ElementAt(0) as FunctionalTests.MutableVocabularyAnnotation;
        Assert.NotNull(valueAnnotation);

        Assert.Equal(0, carType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        valueAnnotation.Target = carType;
        valueAnnotation.Value = new EdmStringConstant("bar");
        model.AddVocabularyAnnotation(valueAnnotation);

        Assert.Equal(1, carWheels.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");
        Assert.Equal(1, carType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        valueAnnotationFound = this.CheckForVocabularyAnnotation(carType.VocabularyAnnotations(model), EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("bar") });
        Assert.True(valueAnnotationFound, "Annotation can't be found.");

        Assert.Equal(2, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
    }

    [Fact]
    public void ConstructibleVocabularyEditingVocabularyAnnotationToNewElement()
    {
        EdmModel model = VocabularyTestModelBuilder.InlineAnnotationSimpleModel();
        Assert.Equal(1, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var carType = model.FindEntityType("DefaultNamespace.Car");
        Assert.NotNull(carType);

        Assert.Equal(0, carType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        EdmTerm hiddenName = new EdmTerm("AnnotationNamespace", "HiddenName", EdmCoreModel.Instance.GetString(true));
        model.AddElement(hiddenName);

        var valueAnnotation = new FunctionalTests.MutableVocabularyAnnotation()
        {
            Target = carType,
            Term = hiddenName,
            Value = new EdmStringConstant("Gray")
        };

        model.AddVocabularyAnnotation(valueAnnotation);
        Assert.Equal(2, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
        Assert.Equal(1, carType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(carType.VocabularyAnnotations(model), EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("Gray") });
        Assert.True(valueAnnotationFound, "Annotation cannot be found.");

        var container = model.FindEntityContainer("Container");
        Assert.NotNull(container, "Invalid entity container name.");

        valueAnnotation.Target = container;
        valueAnnotation.Value = new EdmStringConstant("Blue");
        model.AddVocabularyAnnotation(valueAnnotation);

        valueAnnotationFound = this.CheckForVocabularyAnnotation(model.VocabularyAnnotations, EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("Blue") });
        Assert.True(valueAnnotationFound, "Annotation cannot be found.");

        valueAnnotationFound = this.CheckForVocabularyAnnotation(container.VocabularyAnnotations(model), EdmExpressionKind.StringConstant, new List<PropertyValue> { new PropertyValue("Blue") });
        Assert.True(valueAnnotationFound, "Annotation cannot be found.");

        Assert.Equal(1, carType.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");
        Assert.Equal(1, container.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");
        Assert.Equal(3, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
    }

    [Fact]
    public void ConstructibleVocabularyAddingVocabularyAnnotationWithEnum()
    {
        EdmModel model = VocabularyTestModelBuilder.InlineAnnotationSimpleModel();
        Assert.Equal(1, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var container = model.FindEntityContainer("Container") as EdmEntityContainer;
        Assert.NotNull(container, "Invalid name for entity container.");
        Assert.Equal(0, container.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

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

        Assert.Equal(1, container.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");
        Assert.Equal(2, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var valueAnnotationFound = this.CheckForVocabularyAnnotation(model.VocabularyAnnotations, EdmExpressionKind.EnumMember, new List<PropertyValue>() { new PropertyValue("3") });
        Assert.True(valueAnnotationFound, "Annotation cannot be found.");

        valueAnnotationFound = this.CheckForVocabularyAnnotation(container.VocabularyAnnotations(model), EdmExpressionKind.EnumMember, new List<PropertyValue>() { new PropertyValue("3") });
        Assert.True(valueAnnotationFound, "Annotation cannot be found.");
    }

    [Fact]
    public void ConstructibleVocabularyRemovingInvalidAnnotation()
    {
        var model = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(VocabularyTestModelBuilder.InlineAnnotationSimpleModel());
        Assert.Equal(1, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var address = model.SchemaElements.Where(x => x.Name.Equals("Address")).SingleOrDefault() as EdmComplexType;
        var addressStreet = address.FindProperty("Street");
        Assert.NotNull(address, "Invalid complex type property.");
        Assert.Equal(0, addressStreet.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");

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

        Assert.Equal(1, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");
        Assert.Equal(0, addressStreet.VocabularyAnnotations(model).Count(), "Invalid vocabulary annotation count.");
    }

    [Fact]
    public void ParsingSimpleVocabularyAnnotationWithComplexTypeCsdl()
    {
        var csdls = VocabularyTestModelBuilder.SimpleVocabularyAnnotationWithComplexTypeCsdl();
        var model = this.GetParserResult(csdls);

        IEnumerable<EdmError> errors;
        model.Validate(out errors);
        Assert.Equal(0, errors.Count(), "Invalid error count.");

        var valueAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, valueAnnotations.Count(), "Invalid annotation count.");

        var person = model.FindType("ßÆœÇèÒöæ.Person") as IEdmStructuredType;
        var valueTerm = model.FindTerm("ßÆœÇèÒöæ.PersonInfo");
        var valueAnnotation = valueAnnotations.First().Value as IEdmRecordExpression;

        this.ValidateAnnotationWithExactPropertyAsType(model, person, valueAnnotation.Properties);
    }

    [Fact]
    public void ParsingVocabularyAnnotationComplexTypeWithFewerPropertiesCsdl()
    {
        var csdls = VocabularyTestModelBuilder.VocabularyAnnotationComplexTypeWithFewerPropertiesCsdl();
        var model = this.GetParserResult(csdls);

        IEnumerable<EdmError> errors;
        model.Validate(out errors);
        Assert.Equal(0, errors.Count(), "Invalid error count.");

        var valueAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, valueAnnotations.Count(), "Invalid annotation count.");

        var person = model.FindType("NS.Person") as IEdmStructuredType;
        var valueTerm = model.FindTerm("NS.PersonInfo");
        var valueAnnotation = valueAnnotations.First().Value as IEdmRecordExpression;

        this.ValidateAnnotationWithFewerPropertyThanType(model, person, valueAnnotation.Properties);
    }

    [Fact]
    public void ParsingVocabularyAnnotationComplexTypeWithNullValuesCsdl()
    {
        var csdls = VocabularyTestModelBuilder.VocabularyAnnotationComplexTypeWithNullValuesCsdl();
        var model = this.GetParserResult(csdls);

        IEnumerable<EdmError> errors;
        model.Validate(out errors);
        Assert.Equal(0, errors.Count(), "Invalid error count.");

        var valueAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, valueAnnotations.Count(), "Invalid annotation count.");

        var person = model.FindType("NS.Person") as IEdmStructuredType;
        var valueTerm = model.FindTerm("NS.PersonInfo");
        var valueAnnotation = valueAnnotations.First().Value as IEdmRecordExpression;

        this.ValidateAnnotationWithExactPropertyAsType(model, person, valueAnnotation.Properties);
    }

    [Fact]
    public void ParsingPathInAnOverloadedFunctionCsdl()
    {
        var csdls = VocabularyTestModelBuilder.PathInAnOverloadedFunctionCsdl();
        var model = this.GetParserResult(csdls);

        IEnumerable<EdmError> errors;
        model.Validate(out errors);
        Assert.Equal(0, errors.Count(), "Invalid error count.");

        var valueAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, valueAnnotations.Count(), "Invalid annotation count.");

        var annotation = valueAnnotations.First();
        var appliedFunction = ((IEdmApplyExpression)annotation.Value).AppliedFunction;
        Assert.Equal("Edm.Int32", appliedFunction.ReturnType.FullName(), "Correct function return type");
    }

    [Fact]
    public void ParsingPathInAnOverloadedFunctionWithAmbiguousPrimitivesCsdl()
    {
        var csdls = VocabularyTestModelBuilder.ParsingPathInAnOverloadedFunctionWithAmbiguousPrimitivesCsdl();
        var model = this.GetParserResult(csdls);

        IEnumerable<EdmError> errors;
        model.Validate(out errors);
        Assert.Equal(0, errors.Count(), "Invalid error count.");

        var valueAnnotations = model.VocabularyAnnotations;
        Assert.Equal(1, valueAnnotations.Count(), "Invalid annotation count.");

        var annotation = valueAnnotations.First();
        var appliedFunction = ((IEdmApplyExpression)annotation.Value).AppliedFunction;
        Assert.Equal("Edm.Int32", appliedFunction.ReturnType.FullName(), "Correct function return type");
    }

    [Fact]
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

    [Fact]
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

        Assert.True(isParsed, "Expected the CSDL to be successfully parsed.");
        Assert.Equal(1, edmModel.VocabularyAnnotations.Count(), "Expected CSDL to have exactly one vocabulary annotation.");
        var annotation = edmModel.VocabularyAnnotations.First();
        var targetErrors = annotation.Target.Errors();

        var expectedErrors = new EdmLibTestErrors() 
        {
            {0, 0, EdmErrorCode.BadUnresolvedParameter },
        };

        this.CompareErrors(targetErrors, expectedErrors);
    }

    [Fact]
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

        Assert.True(isParsed, "Expected the CSDL to be successfully parsed.");
        Assert.Equal(1, edmModel.VocabularyAnnotations.Count(), "Expected CSDL to have exactly one vocabulary annotation.");
        var annotation = edmModel.VocabularyAnnotations.First();
        var targetErrors = annotation.Target.Errors();

        var expectedErrors = new EdmLibTestErrors() 
        {
            {0, 0, EdmErrorCode.BadUnresolvedParameter },
        };

        this.CompareErrors(targetErrors, expectedErrors);
    }

    [Fact]
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

        Assert.True(isParsed, "Expected the CSDL to be successfully parsed.");
        Assert.Equal(1, edmModel.VocabularyAnnotations.Count(), "Expected CSDL to have exactly one vocabulary annotation.");
        var annotation = edmModel.VocabularyAnnotations.First();
        var targetErrors = annotation.Target.Errors();

        var expectedErrors = new EdmLibTestErrors()
        {
            // No errors expected.
        };

        this.CompareErrors(targetErrors, expectedErrors);
    }

    [Fact]
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

        Assert.True(edmModel.Validate(out errors));

        using (var xw = XmlWriter.Create(stream, new XmlWriterSettings() { Indent = true }))
        {
            Assert.True(CsdlWriter.TryWriteCsdl(edmModel, xw, CsdlTarget.OData, out errors));
        }

        stream.Seek(0, SeekOrigin.Begin);
        using (var sr = new StreamReader(stream))
        {
            Assert.Equal(csdl, sr.ReadToEnd());
        }
        #endregion


        Assert.True(CsdlReader.TryParse(XmlReader.Create(new StringReader(csdl)), out model, out errors));
        Assert.True(model.Validate(out validationErrors));

        TestEnumMember(model);
    }

    [Fact]
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

        Assert.True(
            CsdlReader.TryParse(
                XmlReader.Create(new StringReader(csdl)),
                uri => XmlReader.Create(new StringReader(csdl2)),
                out model,
                out errors),
            "parsed");
        Assert.True(model.Validate(out validationErrors));

        TestEnumMember(model);
    }

    private void TestEnumMember(IEdmModel model)
    {
        var color = (IEdmEnumType)model.FindType("TestNS2.Color");
        var person = (IEdmEntityType)model.FindType("TestNS.Person");
        var annotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(person, "TestNS.OutColor").Single();
        var memberExp = (IEdmEnumMemberExpression)annotation.Value;
        var members = memberExp.EnumMembers.ToList();
        Assert.Equal(2, members.Count);
        Assert.Equal(color, members[0].DeclaringType);
        Assert.Equal("Blue", members[0].Name);
        Assert.Equal(color, members[1].DeclaringType);
        Assert.Equal("Cyan", members[1].Name);
    }

    private void ValidateAnnotationWithExactPropertyAsType(IEdmModel model, IEdmStructuredType actualType, IEnumerable<IEdmPropertyConstructor> annotationProperties)
    {
        Assert.Equal(actualType.Properties().Count(), annotationProperties.Count(), "Annotation does not have the same count of properties as it's declared type.");

        foreach (var annotationProperty in annotationProperties)
        {
            var annotationName = annotationProperty.Name;
            var actualProperty = actualType.FindProperty(annotationName);
            Assert.NotNull(actualProperty, "Invalid property name.");

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
            Assert.NotNull(actualProperty, "Invalid property name.");

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
            Assert.NotNull(annotationProperty, "Invalid property name.");

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
                    Assert.Equal(actualElementExpressionKind, element.ExpressionKind, "Invalid expression kind.");
                }
            }
        }
        else
        {
            var actualPropertyType = this.GetPrimitiveExpressionKind(actualProperty.Type.Definition.ToString());

            if (IsPropertyNonNullExpressionOrNonNullable(actualProperty.Type.IsNullable, annotationProperty.Value.ExpressionKind))
            {
                Assert.Equal(actualPropertyType, annotationProperty.Value.ExpressionKind, "Invalid expression kind.");
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
