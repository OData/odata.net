﻿//---------------------------------------------------------------------
// <copyright file="ExpressionEvaluationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class ExpressionEvaluationTests : EdmLibTestCaseBase
{
    IEdmModel baseModel;
    IEdmModel vocabularyDefinitionModel;
    IEdmModel operationsModel;
    Dictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> operationsLookup;

    IEdmStructuredValue personValue;
    IEdmStructuredValue professionalValue;

    [Fact]
    public void EvaluateConstantTermOnBaseEntityTypes_ReturnsExpectedValues()
    {
        this.SetupModelsAndValues();

        const string applicationCsdl =
@"<Schema Namespace=""Annotations.Application"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""bar.Int32Value"" Qualifier=""q1.q1"" Int=""100"" />
    </Annotations>
</Schema>";

        IEdmModel applicationModel = this.Parse(applicationCsdl, this.baseModel, this.vocabularyDefinitionModel);
        EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.operationsLookup);

        IEdmTerm termInt32Value = this.vocabularyDefinitionModel.FindTerm("bar.Int32Value");

        IEdmValue annotationOnPerson = applicationModel.GetTermValue(this.personValue, termInt32Value, expressionEvaluator);
        Assert.Equal(100, ((IEdmIntegerValue)annotationOnPerson).Value);

        annotationOnPerson = applicationModel.GetTermValue(this.personValue, termInt32Value, "q1.q1", expressionEvaluator);
        Assert.Equal(100, ((IEdmIntegerValue)annotationOnPerson).Value);

        IEdmValue annotationInherited = applicationModel.GetTermValue(this.professionalValue, termInt32Value, expressionEvaluator);
        Assert.Equal(100, ((IEdmIntegerValue)annotationInherited).Value);

        annotationInherited = applicationModel.GetTermValue(this.professionalValue, termInt32Value, "q1.q1", expressionEvaluator);
        Assert.Equal(100, ((IEdmIntegerValue)annotationInherited).Value);
    }

    [Fact]
    public void EvaluateConstantTermWithMultipleVocabularyAnnotationsOnBaseEntityTypes_ReturnsExpectedValues()
    {
        this.SetupModelsAndValues();

        const string applicationCsdl =
@"<Schema Namespace=""Annotations.Application"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""bar.Int32Value"" Qualifier=""q1.q1"" Int=""100"" />
        <Annotation Term=""bar.Int32Value"" Qualifier=""q2.q2"" Int=""200"" />
    </Annotations>
</Schema>";
        IEdmModel applicationModel = this.Parse(applicationCsdl, this.baseModel, this.vocabularyDefinitionModel);
        EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.operationsLookup);

        IEdmTerm termInt32Value = this.vocabularyDefinitionModel.FindTerm("bar.Int32Value");

        IEdmValue annotationOnPerson = applicationModel.GetTermValue(this.personValue, termInt32Value, "q1.q1", expressionEvaluator);
        Assert.Equal(100, ((IEdmIntegerValue)annotationOnPerson).Value);

        annotationOnPerson = applicationModel.GetTermValue(this.personValue, termInt32Value, "q2.q2", expressionEvaluator);
        Assert.Equal(200, ((IEdmIntegerValue)annotationOnPerson).Value);

        IEdmValue annotationInherited = applicationModel.GetTermValue(this.professionalValue, termInt32Value, "q1.q1", expressionEvaluator);
        Assert.Equal(100, ((IEdmIntegerValue)annotationInherited).Value);

        annotationInherited = applicationModel.GetTermValue(this.professionalValue, termInt32Value, "q2.q2", expressionEvaluator);
        Assert.Equal(200, ((IEdmIntegerValue)annotationInherited).Value);
    }

    [Fact]
    public void EvaluateConstantTermOnDerivedEntityType_ReturnsExpectedValues()
    {
        this.SetupModelsAndValues();

        const string applicationCsdl =
@"<Schema Namespace=""Annotations"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""bar.Int32Value"" Qualifier=""onPerson"" Int=""100"" />
    </Annotations>
    <Annotations Target=""foo.Professional"">
        <Annotation Term=""bar.Int32Value"" Qualifier=""onProfessional"" Int=""999"" />
    </Annotations>
</Schema>";
        IEdmModel applicationModel = this.Parse(applicationCsdl, this.baseModel, this.vocabularyDefinitionModel);
        EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.operationsLookup);

        IEdmTerm termInt32Value = this.vocabularyDefinitionModel.FindTerm("bar.Int32Value");

        IEdmValue annotationOnPerson = applicationModel.GetTermValue(this.personValue, termInt32Value, expressionEvaluator);
        Assert.Equal(100, ((IEdmIntegerValue)annotationOnPerson).Value);

        IEdmValue annotationOnProfessional = applicationModel.GetTermValue(this.professionalValue, termInt32Value, "onProfessional", expressionEvaluator);
        Assert.Equal(999, ((IEdmIntegerValue)annotationOnProfessional).Value);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void EvaluateBinaryConstantTermWithOddDigits_ReturnsEmptyAndValidationErrors(EdmVersion edmVersion)
    {
        this.SetupModelsAndValues();

        const string applicationCsdl =
@"<Schema Namespace=""Annotations"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""bar.BinaryValue"" Binary=""123"" />
    </Annotations>
</Schema>";

        IEdmModel applicationModel = this.Parse(applicationCsdl, this.baseModel, this.vocabularyDefinitionModel);
        EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.operationsLookup);

        IEdmTerm term;
        IEdmValue annotationValue;

        term = this.vocabularyDefinitionModel.FindTerm("bar.BinaryValue");
        annotationValue = applicationModel.GetTermValue(this.personValue, term, expressionEvaluator);
        byte[] v = ((IEdmBinaryConstantExpression)annotationValue).Value;
        Assert.Empty(v);

        Assert.True(((IEdmBinaryConstantExpression)annotationValue).IsBad());

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = applicationModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.InvalidBinary, actualErrors.First().ErrorCode);
        Assert.Equal("The value '123' is not a valid binary value. The value must be a hexadecimal string and must not be prefixed by '0x'.", actualErrors.First().ErrorMessage);
    }

    // [EdmLib] missing constants for spatial
    [Fact]
    public void EvaluateAllSupportedConstantTypesOnEntityType_ReturnsExpectedValues()
    {
        this.SetupModelsAndValues();

        // Not handled: 
        //      <Annotation Term=""bar.GeographyValue"" Geography=""xxx"" />
        //      <Annotation Term=""bar.GeometryValue"" Geometry=""xxx"" />
        const string applicationCsdl =
@"<Schema Namespace=""Annotations"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""bar.BinaryValue"" Binary=""1234"" />        
        <Annotation Term=""bar.BooleanValue"" Bool=""true"" />
        <Annotation Term=""bar.ByteValue"" Int=""255"" />
        <Annotation Term=""bar.DateValue"" Date=""2014-08-08"" />
        <Annotation Term=""bar.DateTimeOffsetValue"" DateTimeOffset=""2001-10-26T19:32:52+00:00"" />
        <Annotation Term=""bar.DecimalValue"" Decimal=""12.345"" />
        <Annotation Term=""bar.DoubleValue"" Float=""3.1416"" />
        <Annotation Term=""bar.GuidValue"" Guid=""4ae71c81-c21a-40a2-8d53-f1a29ed4a2f2"" />
        <Annotation Term=""bar.Int16Value"" Int=""0"" />
        <Annotation Term=""bar.Int32Value"" Int=""100"" />
        <Annotation Term=""bar.Int64Value"" Int=""9999"" />
        <Annotation Term=""bar.DurationValue"" Duration=""PT1M59S"" />
        <Annotation Term=""bar.SByteValue"" Int=""-128"" />
        <Annotation Term=""bar.SingleValue"" Float=""3.1416E10"" />
        <Annotation Term=""bar.StringValue"" String=""I am a string."" />
        <Annotation Term=""bar.TimeOfDayValue"" TimeOfDay=""1:30:59.123"" />
    </Annotations>
</Schema>";
        IEdmModel applicationModel = this.Parse(applicationCsdl, this.baseModel, this.vocabularyDefinitionModel);
        EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.operationsLookup);

        IEdmTerm term;
        IEdmValue annotationValue;

        term = this.vocabularyDefinitionModel.FindTerm("bar.BinaryValue");
        annotationValue = applicationModel.GetTermValue(this.personValue, term, expressionEvaluator);
        byte[] v = ((IEdmBinaryValue)annotationValue).Value;
        Assert.Equal(2, v.Length);
        Assert.Equal(0x12, v[0]);
        Assert.Equal(0x34, v[1]);

        term = this.vocabularyDefinitionModel.FindTerm("bar.BooleanValue");
        annotationValue = applicationModel.GetTermValue(this.personValue, term, expressionEvaluator);
        Assert.True(((IEdmBooleanValue)annotationValue).Value);

        term = this.vocabularyDefinitionModel.FindTerm("bar.ByteValue");
        annotationValue = applicationModel.GetTermValue(this.personValue, term, expressionEvaluator);
        Assert.Equal(255, ((IEdmIntegerValue)annotationValue).Value);

        term = this.vocabularyDefinitionModel.FindTerm("bar.DateValue");
        annotationValue = applicationModel.GetTermValue(this.personValue, term, expressionEvaluator);
        Assert.Equal(new Date(2014, 8, 8), ((IEdmDateValue)annotationValue).Value);

        term = this.vocabularyDefinitionModel.FindTerm("bar.DateTimeOffsetValue");
        annotationValue = applicationModel.GetTermValue(this.personValue, term, expressionEvaluator);
        Assert.Equal(DateTimeOffset.Parse("2001-10-26T19:32:52+00:00"), ((IEdmDateTimeOffsetValue)annotationValue).Value);

        term = this.vocabularyDefinitionModel.FindTerm("bar.DecimalValue");
        annotationValue = applicationModel.GetTermValue(this.personValue, term, expressionEvaluator);
        Assert.Equal(12.345M, ((IEdmDecimalValue)annotationValue).Value);

        term = this.vocabularyDefinitionModel.FindTerm("bar.DoubleValue");
        annotationValue = applicationModel.GetTermValue(this.personValue, term, expressionEvaluator);
        Assert.Equal(3.1416, ((IEdmFloatingValue)annotationValue).Value);

        term = this.vocabularyDefinitionModel.FindTerm("bar.GuidValue");
        annotationValue = applicationModel.GetTermValue(this.personValue, term, expressionEvaluator);
        Assert.Equal(Guid.Parse("4ae71c81-c21a-40a2-8d53-f1a29ed4a2f2"), ((IEdmGuidValue)annotationValue).Value);

        term = this.vocabularyDefinitionModel.FindTerm("bar.Int16Value");
        annotationValue = applicationModel.GetTermValue(this.personValue, term, expressionEvaluator);
        Assert.Equal(0, ((IEdmIntegerValue)annotationValue).Value);

        term = this.vocabularyDefinitionModel.FindTerm("bar.Int32Value");
        annotationValue = applicationModel.GetTermValue(this.personValue, term, expressionEvaluator);
        Assert.Equal(100, ((IEdmIntegerValue)annotationValue).Value);

        term = this.vocabularyDefinitionModel.FindTerm("bar.Int64Value");
        annotationValue = applicationModel.GetTermValue(this.personValue, term, expressionEvaluator);
        Assert.Equal(9999, ((IEdmIntegerValue)annotationValue).Value);

        term = this.vocabularyDefinitionModel.FindTerm("bar.DurationValue");
        annotationValue = applicationModel.GetTermValue(this.personValue, term, expressionEvaluator);
        Assert.Equal(new TimeSpan(0, 1, 59), ((IEdmDurationValue)annotationValue).Value);

        term = this.vocabularyDefinitionModel.FindTerm("bar.SByteValue");
        annotationValue = applicationModel.GetTermValue(this.personValue, term, expressionEvaluator);
        Assert.Equal(-128, ((IEdmIntegerValue)annotationValue).Value);

        term = this.vocabularyDefinitionModel.FindTerm("bar.SingleValue");
        annotationValue = applicationModel.GetTermValue(this.personValue, term, expressionEvaluator);
        Assert.Equal(3.1416E10, ((IEdmFloatingValue)annotationValue).Value);

        term = this.vocabularyDefinitionModel.FindTerm("bar.StringValue");
        annotationValue = applicationModel.GetTermValue(this.personValue, term, expressionEvaluator);
        Assert.Equal("I am a string.", ((IEdmStringValue)annotationValue).Value);

        term = this.vocabularyDefinitionModel.FindTerm("bar.TimeOfDayValue");
        annotationValue = applicationModel.GetTermValue(this.personValue, term, expressionEvaluator);
        Assert.Equal(new TimeOfDay(1, 30, 59, 123), ((IEdmTimeOfDayValue)annotationValue).Value);
    }

    [Fact]
    public void EvaluatePathBasedTermOnEntityType_ReturnsExpectedValues()
    {
        this.SetupModelsAndValues();

        const string applicationCsdl =
@"<Schema Namespace=""Annotations"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""bar.Int32Value"" Qualifier=""CoolnessIndex"" Path=""CoolnessIndex"" />
        <Annotation Term=""bar.Int64Value"" Qualifier=""WorkPhoneLocal"" Path=""ContactInfo/WorkPhone/Local"" />
        <Annotation Term=""bar.StringValue"" Qualifier=""ZipMain"" Path=""Address/Zip/Main"" />
    </Annotations>
</Schema>";
        IEdmModel applicationModel = this.Parse(applicationCsdl, this.baseModel, this.vocabularyDefinitionModel);
        EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.operationsLookup);

        IEdmTerm termInt32Value = this.vocabularyDefinitionModel.FindTerm("bar.Int32Value");
        IEdmValue annotationCoolnessIndex = applicationModel.GetTermValue(this.personValue, termInt32Value, expressionEvaluator);
        Assert.Equal(Int32.MaxValue, ((IEdmIntegerValue)annotationCoolnessIndex).Value);

        IEdmTerm termStringValue = this.vocabularyDefinitionModel.FindTerm("bar.StringValue");
        IEdmValue annotationZipMain = applicationModel.GetTermValue(this.personValue, termStringValue, expressionEvaluator);
        Assert.Equal("98052", ((IEdmStringValue)annotationZipMain).Value);

        IEdmTerm termInt64Value = this.vocabularyDefinitionModel.FindTerm("bar.Int64Value");
        IEdmValue annotationWorkPhoneLocal = applicationModel.GetTermValue(this.personValue, termInt64Value, expressionEvaluator);
        Assert.Equal(9991234, ((IEdmIntegerValue)annotationWorkPhoneLocal).Value);
    }

    [Fact]
    public void EvaluateConstantTermOnProperty_ReturnsExpectedValues()
    {
        this.SetupModelsAndValues();

        const string applicationCsdl =
@"<Schema Namespace=""Annotations"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person/CoolnessIndex"">
        <Annotation Term=""bar.Int32Value"" Qualifier=""HotIndex"" Int=""-1"" />
        <Annotation Term=""bar.StringValue"" String=""Goofy"" />
    </Annotations>
</Schema>";
        IEdmModel applicationModel = this.Parse(applicationCsdl, this.baseModel, this.vocabularyDefinitionModel);
        EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.operationsLookup);
        EdmToClrEvaluator clrEvaluator = new EdmToClrEvaluator(this.operationsLookup);

        var person = this.baseModel.FindType("foo.Person") as IEdmEntityType;
        IEdmProperty property = person.FindProperty("CoolnessIndex");
        IEdmPropertyValue contextPropertyValue = ((IEdmStructuredValue)this.personValue).FindPropertyValue("CoolnessIndex");

        IEdmTerm termInt32Value = this.vocabularyDefinitionModel.FindTerm("bar.Int32Value");
        IEdmTerm termStringValue = this.vocabularyDefinitionModel.FindTerm("bar.StringValue");

        var annotation = property.VocabularyAnnotations(applicationModel).SingleOrDefault(a => a.Term == termInt32Value);
        IEdmExpression expression = annotation.Value;
        IEdmValue annotationHotIndex = expressionEvaluator.Evaluate(expression);
        Assert.Equal(-1, ((IEdmIntegerValue)annotationHotIndex).Value);

        annotationHotIndex = applicationModel.GetTermValue(property, termInt32Value, "HotIndex", expressionEvaluator);
        Assert.Equal(-1, ((IEdmIntegerValue)annotationHotIndex).Value);

        annotationHotIndex = applicationModel.GetTermValue(property, "bar.Int32Value", "HotIndex", expressionEvaluator);
        Assert.Equal(-1, ((IEdmIntegerValue)annotationHotIndex).Value);

        int hotIndex = applicationModel.GetTermValue<int>(property, termInt32Value, "HotIndex", clrEvaluator);
        Assert.Equal(-1, hotIndex);

        hotIndex = applicationModel.GetTermValue<int>(property, "bar.Int32Value", "HotIndex", clrEvaluator);
        Assert.Equal(-1, hotIndex);

        IEdmValue annotationString = applicationModel.GetTermValue(property, termStringValue, expressionEvaluator);
        Assert.Equal("Goofy", ((IEdmStringValue)annotationString).Value);

        annotationString = applicationModel.GetTermValue(property, "bar.StringValue", expressionEvaluator);
        Assert.Equal("Goofy", ((IEdmStringValue)annotationString).Value);

        string stringValue = applicationModel.GetTermValue<string>(property, termStringValue, clrEvaluator);
        Assert.Equal("Goofy", stringValue);

        stringValue = applicationModel.GetTermValue<string>(property, "bar.StringValue", clrEvaluator);
        Assert.Equal("Goofy", stringValue);
    }

    [Fact]
    public void EvaluatePathBasedTermOnProperty_ReturnsExpectedValues()
    {
        this.SetupModelsAndValues();

        const string applicationCsdl =
@"<Schema Namespace=""Annotations"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person/ContactInfo"">
        <Annotation Term=""bar.Int16Value"" Qualifier=""WorkPhoneLocal"" Path=""WorkPhone/Local"" />
    </Annotations>
</Schema>";
        IEdmModel applicationModel = this.Parse(applicationCsdl, this.baseModel, this.vocabularyDefinitionModel);
        EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.operationsLookup);

        var person = this.baseModel.FindType("foo.Person") as IEdmEntityType;
        IEdmProperty property = person.FindProperty("ContactInfo");
        IEdmPropertyValue contextPropertyValue = ((IEdmStructuredValue)this.personValue).FindPropertyValue("ContactInfo");

        IEdmTerm termInt16Value = this.vocabularyDefinitionModel.FindTerm("bar.Int16Value");
        // IEdmValue annotationHotIndex = applicationModel.GetTermValue(contextPropertyValue.Value, termInt16Value, evaluator);
        var annotation = property.VocabularyAnnotations(applicationModel).SingleOrDefault(a => a.Term == termInt16Value);
        IEdmExpression expression = annotation.Value;
        IEdmValue annotationWorkphoneLocal = expressionEvaluator.Evaluate(expression, (IEdmStructuredValue)contextPropertyValue.Value);
        Assert.Equal(9991234, ((IEdmIntegerValue)annotationWorkphoneLocal).Value);
    }

    [Fact]
    public void EvaluateFunctionApplicationTermOnNavigationProperty_ReturnsExpectedValues()
    {
        this.SetupModelsAndValues();

        const string applicationCsdl =
@"<Schema Namespace=""Annotations"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person/Address"">
        <Annotation Term=""bar.StringValue"">
            <Apply Function=""Functions.StringConcat"">
                <Path>Zip/Main</Path>
                <Apply Function=""Functions.StringConcat"">
                    <String>-</String>
                    <Path>Zip/Extension</Path>
                </Apply>
            </Apply>
        </Annotation>
    </Annotations>
</Schema>";
        IEdmModel applicationModel = this.Parse(applicationCsdl, this.baseModel, this.vocabularyDefinitionModel, this.operationsModel);
        EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.operationsLookup);

        var person = this.baseModel.FindType("foo.Person") as IEdmEntityType;
        IEdmProperty property = person.FindProperty("Address");
        IEdmPropertyValue contextPropertyValue = ((IEdmStructuredValue)this.personValue).FindPropertyValue("Address");

        IEdmTerm termStringValue = this.vocabularyDefinitionModel.FindTerm("bar.StringValue");
        var annotationString = property.VocabularyAnnotations(applicationModel).SingleOrDefault(a => a.Term == termStringValue);
        IEdmValue annotationStringValue = expressionEvaluator.Evaluate(annotationString.Value, (IEdmStructuredValue)contextPropertyValue.Value);
        Assert.Equal("98052-0000", ((IEdmStringValue)annotationStringValue).Value);
    }

    [Fact]
    public void EvaluateConditionalTypeTermOnEntityContainer_ReturnsExpectedValues()
    {
        this.SetupModelsAndValues();

        const string applicationCsdl =
@"<Schema Namespace=""Annotations"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.fooContainer"">
        <Annotation Term=""bar.Int32Value"">
            <If>
                <Apply Function=""Functions.True"" />
                <Int>999</Int>
                <Int>30</Int>
            </If>                
        </Annotation>
    </Annotations>
</Schema>";
        IEdmModel applicationModel = this.Parse(applicationCsdl, this.baseModel, this.vocabularyDefinitionModel, this.operationsModel);
        EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.operationsLookup);

        IEdmEntityContainer container = this.baseModel.FindEntityContainer("fooContainer");

        IEdmTerm termInt32Value = this.vocabularyDefinitionModel.FindTerm("bar.Int32Value");
        var valueAnnotation = applicationModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(container, termInt32Value).SingleOrDefault();

        IEdmValue valueOfValueAnnotation = expressionEvaluator.Evaluate(valueAnnotation.Value, this.personValue);
        Assert.Equal(999, ((IEdmIntegerValue)valueOfValueAnnotation).Value);
    }

    [Fact]
    public void EvaluateFunctionApplicationWithOverloadsOnEntitySet_ReturnsExpectedValues()
    {
        this.SetupModelsAndValues();

        const string applicationCsdl =
@"<Schema Namespace=""Annotations"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.fooContainer/PersonSet"">
        <Annotation Term=""bar.StringValue"">
            <If>
                <Apply Function=""Functions.True"" />
                <Apply Function=""Functions.StringConcat"">
                    <String>1</String>
                    <String>2</String>
                    <String>3</String>
                </Apply>
                <String>foo</String>
            </If>                
        </Annotation>
    </Annotations>
</Schema>";
        IEdmModel applicationModel = this.Parse(applicationCsdl, this.baseModel, this.vocabularyDefinitionModel, this.operationsModel);
        EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.operationsLookup);

        IEdmEntitySet personSet = this.baseModel.FindEntityContainer("fooContainer").FindEntitySet("PersonSet");

        IEdmTerm term = this.vocabularyDefinitionModel.FindTerm("bar.StringValue");
        IEdmVocabularyAnnotation valueAnnotation = applicationModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(personSet, term).SingleOrDefault();

        IEdmValue valueOfAnnotation = expressionEvaluator.Evaluate(valueAnnotation.Value, this.personValue);
        Assert.Equal("123", ((IEdmStringValue)valueOfAnnotation).Value);
    }

    [Fact]
    public void EvaluateComplexValueTermOnEntityType_ReturnsExpectedStructuredValue()
    {
        this.SetupModelsAndValues();

        const string applicationCsdl =
@"<Schema Namespace=""Annotations"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""bar.ComplexValue"">
            <Record>
                <PropertyValue Property=""Summary"" String=""the summary."" />
            </Record>
        </Annotation>
    </Annotations>
</Schema>";
        IEdmModel applicationModel = this.Parse(applicationCsdl, this.baseModel, this.vocabularyDefinitionModel, this.operationsModel);
        EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.operationsLookup);

        IEdmTerm termComplexValue = this.vocabularyDefinitionModel.FindTerm("bar.ComplexValue");
        IEdmValue annotationComplexValue = applicationModel.GetTermValue(this.personValue, termComplexValue, expressionEvaluator);

        var structuredValue = (IEdmStructuredValue)annotationComplexValue;
        Assert.NotNull(structuredValue);
        Assert.Single(structuredValue.PropertyValues);
        Assert.Equal("Summary", structuredValue.PropertyValues.ElementAt(0).Name);
        Assert.Equal("the summary.", ((IEdmStringValue)structuredValue.PropertyValues.ElementAt(0).Value).Value);

        var complexReference = structuredValue.Type.AsComplex();
        Assert.NotNull(complexReference);
        Assert.Equal(this.vocabularyDefinitionModel.FindType("bar.Note"), complexReference.ComplexDefinition());
    }

    [Fact]
    public void EvaluateEntityValueTermOnEntityType_ReturnsExpectedStructuredValue()
    {
        this.SetupModelsAndValues();

        const string applicationCsdl =
@"<Schema Namespace=""Annotations"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""bar.EntityValue"">
            <Record>
                <PropertyValue Property=""Description"" String=""the summary."" />
                <PropertyValue Property=""Age"" Int=""15"" />
            </Record>
        </Annotation>
    </Annotations>
</Schema>";

        IEdmModel applicationModel = this.Parse(applicationCsdl, this.baseModel, this.vocabularyDefinitionModel, this.operationsModel);
        EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.operationsLookup);

        IEdmTerm termEntityValue = this.vocabularyDefinitionModel.FindTerm("bar.EntityValue");
        IEdmValue annotationEntityValue = applicationModel.GetTermValue(this.personValue, termEntityValue, expressionEvaluator);

        var structuredValue = (IEdmStructuredValue)annotationEntityValue;
        Assert.NotNull(structuredValue);
        Assert.Equal(2, structuredValue.PropertyValues.Count());
        Assert.Equal("Description", structuredValue.PropertyValues.ElementAt(1).Name);
        Assert.Equal("the summary.", ((IEdmStringValue)structuredValue.PropertyValues.ElementAt(1).Value).Value);

        var entityReference = structuredValue.Type.AsEntity();
        Assert.NotNull(entityReference);
        Assert.Equal(this.vocabularyDefinitionModel.FindType("bar.MoreTransformedPerson"), entityReference.EntityDefinition());
    }

    [Fact]
    public void EvaluateSingleModelAnnotationOnEntityType_ReturnsExpectedAnnotationValue()
    {
        const string applicationCsdl =
@"<Schema Namespace=""bar"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""MoreTransformedPersonTerm"" Type=""bar.MoreTransformedPerson"" />
    <Annotations Target=""bar.Person"">
        <Annotation Term=""bar.MoreTransformedPersonTerm"">
            <Record>
                <PropertyValue Property=""Description"" String=""I know it!"" />
            </Record>
        </Annotation>
    </Annotations>
    <EntityType Name=""MoreTransformedPerson"" BaseType=""bar.TransformedPerson"">
        <Property Name=""Description"" Type=""String"" />
    </EntityType>
    <EntityType Name=""TransformedPerson"">
        <Property Name=""Age"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
    </EntityType>
</Schema>";
        IEdmModel applicationModel = this.Parse(applicationCsdl);
        EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(null);

        var term = applicationModel.FindTerm("bar.MoreTransformedPersonTerm");
        var property = (applicationModel.FindType("bar.MoreTransformedPerson") as IEdmEntityType)?.FindProperty("Description");
        var fakeContextValue = new EdmStructuredValue(new EdmEntityTypeReference((IEdmEntityType)applicationModel.FindType("bar.Person"), true), Enumerable.Empty<IEdmPropertyValue>());
        IEdmValue valueOfAnnotation = applicationModel.GetPropertyValue(fakeContextValue, term, property, expressionEvaluator);

        Assert.Equal("I know it!", ((IEdmStringValue)valueOfAnnotation).Value);
    }

    [Fact]
    public void EvaluateUnboundTerms_ReturnsExpectedValues()
    {
        const string applicationCsdl =
@"<Schema Namespace=""bar"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""bar.Person"">
        <Annotation Term=""bar.RandomTerm"" Path=""Extra"" />
    </Annotations>
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Decoration"" Type=""bar.Decoration"">
            <Annotation Term=""bar.DecorationTerm"" Qualifier=""Goodness"" String=""Excellent"" />
        </Property>
    </EntityType>
    <ComplexType Name=""Decoration"">
        <Property Name=""One"" Type=""Int32"" />
        <Property Name=""Two"" Type=""Int32"" />
    </ComplexType>
</Schema>";

        IEdmModel applicationModel = this.Parse(applicationCsdl);
        IEdmEntityType person = (IEdmEntityType)applicationModel.FindType("bar.Person");
        IEdmProperty personDecoration = person.FindProperty("Decoration");

        EdmToClrEvaluator evaluator = new EdmToClrEvaluator(null);

        List<IEdmPropertyValue> decorationPropertyValues = new List<IEdmPropertyValue>();
        decorationPropertyValues.Add(new EdmPropertyValue("One", new EdmIntegerConstant(1)));
        decorationPropertyValues.Add(new EdmPropertyValue("Two", new EdmIntegerConstant(2)));

        List<IEdmPropertyValue> propertyValues = new List<IEdmPropertyValue>();
        propertyValues.Add(new EdmPropertyValue("Name", new EdmStringConstant("Goober")));
        propertyValues.Add(new EdmPropertyValue("Decoration", new EdmStructuredValue(null, decorationPropertyValues)));
        propertyValues.Add(new EdmPropertyValue("Extra", new EdmStringConstant("Extra value!")));
        IEdmStructuredValue context = new EdmStructuredValue(new EdmEntityTypeReference(person, false), propertyValues);

        string random = applicationModel.GetTermValue<string>(context, "bar.RandomTerm", evaluator);
        Assert.Equal("Extra value!", random);

        IEdmValue randomValue = applicationModel.GetTermValue(context, "bar.RandomTerm", evaluator);
        Assert.Equal("Extra value!", ((IEdmStringValue)randomValue).Value);

        string goodness = applicationModel.GetTermValue<string>(personDecoration, "bar.DecorationTerm", "Goodness", evaluator);
        Assert.Equal("Excellent", goodness);

        IEdmValue goodnessValue = applicationModel.GetTermValue(personDecoration, "bar.DecorationTerm", "Goodness", evaluator);
        Assert.Equal("Excellent", ((IEdmStringValue)goodnessValue).Value);

        IEdmVocabularyAnnotation randomTermAnnotation = applicationModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(person, "bar.RandomTerm").Single();
        random = evaluator.EvaluateToClrValue<string>(randomTermAnnotation.Value, context);
        Assert.Equal("Extra value!", random);
    }

    #region Private

    private IEdmModel Parse(string csdl, params IEdmModel[] referencedModels)
    {
        var csdlElements = new[] { csdl }.Select(XElement.Parse);
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), referencedModels, out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        return edmModel;
    }

    private void SetupModelsAndValues()
    {
        this.SetupOperations();
        this.SetupVocabularyDefinitionModel();

        this.SetupBaseModel();
        this.SetupValues();
    }

    private void SetupOperations()
    {
        const string builtinFunctionsCsdl =
@"<Schema Namespace=""Functions"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name=""StringConcat""><ReturnType Type=""String""/>
    <Parameter Name=""String1"" Type=""String"" />
    <Parameter Name=""String2"" Type=""String"" />
  </Function>
  <Function Name=""StringConcat""><ReturnType Type=""String""/>
    <Parameter Name=""String1"" Type=""String"" />
    <Parameter Name=""String2"" Type=""String"" />
    <Parameter Name=""String3"" Type=""String"" />
  </Function>
  <Function Name=""True""><ReturnType Type=""Boolean"" /></Function>
  <Function Name=""False""><ReturnType Type=""Boolean"" /></Function>
</Schema>";
        this.operationsModel = this.Parse(builtinFunctionsCsdl);

        this.operationsLookup = new Dictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>>();

        IEdmOperation stringConcat2 = this.operationsModel.FindOperations("Functions.StringConcat").Single(f => f.Parameters.Count() == 2);
        this.operationsLookup[stringConcat2] = (a) => new EdmStringConstant(((IEdmStringValue)a[0]).Value + ((IEdmStringValue)a[1]).Value);

        IEdmOperation stringConcat3 = this.operationsModel.FindOperations("Functions.StringConcat").Single(f => f.Parameters.Count() == 3);
        this.operationsLookup[stringConcat3] = (a) => new EdmStringConstant(((IEdmStringValue)a[0]).Value + ((IEdmStringValue)a[1]).Value + ((IEdmStringValue)a[2]).Value);

        IEdmOperation trueOperation = this.operationsModel.FindOperations("Functions.True").Single();
        this.operationsLookup[trueOperation] = (a) => new EdmBooleanConstant(true);

        IEdmOperation falseOperation = this.operationsModel.FindOperations("Functions.False").Single();
        this.operationsLookup[falseOperation] = (a) => new EdmBooleanConstant(false);
    }

    private void SetupVocabularyDefinitionModel()
    {
        const string vocabularyDefinitionModelCsdl =
@"<Schema Namespace=""bar"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""BinaryValue"" Type=""Binary"" />
    <Term Name=""BooleanValue"" Type=""Boolean"" />
    <Term Name=""ByteValue"" Type=""Byte"" />
    <Term Name=""DateTimeOffsetValue"" Type=""DateTimeOffset"" />
    <Term Name=""DateValue"" Type=""Date"" />
    <Term Name=""DecimalValue"" Type=""Decimal"" />
    <Term Name=""DoubleValue"" Type=""Double"" />
    <Term Name=""GuidValue"" Type=""Guid"" />
    <Term Name=""Int16Value"" Type=""Int16"" />
    <Term Name=""Int32Value"" Type=""Int32"" />
    <Term Name=""Int64Value"" Type=""Int64"" />
    <Term Name=""SByteValue"" Type=""SByte"" />
    <Term Name=""SingleValue"" Type=""Single"" />
    <Term Name=""StringValue"" Type=""String"" />
    <Term Name=""TimeOfDayValue"" Type=""TimeOfDay"" />
    <Term Name=""DurationValue"" Type=""Duration"" />
    <Term Name=""GeographyValue"" Type=""Geography"" />
    <Term Name=""GeometryValue"" Type=""Geometry"" />
    <Term Name=""ComplexValue"" Type=""bar.Note"" />
    <Term Name=""EntityValue"" Type=""bar.MoreTransformedPerson"" />    
    
    <EntityType Name=""TransformedPerson"">
        <Property Name=""Age"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""MoreTransformedPerson"" BaseType=""bar.TransformedPerson"">
        <Property Name=""Description"" Type=""String"" />
    </EntityType>

    <ComplexType Name=""Note"">
        <Property Name=""Summary"" Type=""String"" />
    </ComplexType>
</Schema>";
        this.vocabularyDefinitionModel = this.Parse(vocabularyDefinitionModelCsdl);
    }

    private void SetupBaseModel()
    {
        const string baseModelCsdl = @"
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
        <Property Name=""CoolnessIndex"" Type=""Int32"" />
        <Property Name=""Living"" Type=""Boolean"" />
        <Property Name=""Famous"" Type=""Boolean"" />
        <Property Name=""ContactInfo"" Type=""foo.ContactInfo"" />
        <NavigationProperty Name=""Address"" Type=""foo.Address"" Nullable=""false"" />
    </EntityType>
    <ComplexType Name=""ContactInfo"">
        <Property Name=""Email"" Type=""String"" />
        <Property Name=""WorkPhone"" Type=""foo.PhoneNumber"" />
        <Property Name=""HomePhone"" Type=""foo.PhoneNumber"" />
    </ComplexType>
    <ComplexType Name=""PhoneNumber"">
        <Property Name=""Area"" Type=""Int32"" />
        <Property Name=""Local"" Type=""Int64"" />
        <Property Name=""Extension"" Type=""Int32"" />
    </ComplexType>
    <ComplexType Name=""ZipCode"">
        <Property Name=""Main"" Type=""String"" />
        <Property Name=""Extension"" Type=""String"" />
    </ComplexType>
    <EntityType Name=""Address"">
        <Key>
            <PropertyRef Name=""Number"" />
            <PropertyRef Name=""Street"" />
            <PropertyRef Name=""City"" />
            <PropertyRef Name=""State"" />
        </Key>
        <Property Name=""Number"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Street"" Type=""String"" Nullable=""false"" />
        <Property Name=""City"" Type=""String"" Nullable=""false"" />
        <Property Name=""State"" Type=""String"" Nullable=""false"" />
        <Property Name=""Zip"" Type=""foo.ZipCode"" />
    </EntityType>
    <EntityType Name=""Professional"" BaseType=""foo.Person"">
        <Property Name=""Profession"" Type=""String"" />
    </EntityType>
    <EnumType Name=""PersonType"">
        <Member Name=""Extrovert"" />
        <Member Name=""Introvert"" Value=""7"" />
    </EnumType>
    <EntityContainer Name=""fooContainer"">
        <EntitySet Name=""PersonSet"" EntityType=""foo.Person"" />
    </EntityContainer>
</Schema>";
        this.baseModel = this.Parse(baseModelCsdl);
    }

    private void SetupValues()
    {
        IEdmEntityType person = (IEdmEntityType)this.baseModel.FindType("foo.Person");
        IEdmEntityType professional = (IEdmEntityType)this.baseModel.FindType("foo.Professional");
        IEdmEntityType address = (IEdmEntityType)this.baseModel.FindType("foo.Address");

        // ?? no EdmComplexValue
        var zipMain = new EdmPropertyValue("Main", new EdmStringConstant("98052"));
        var zipExtension = new EdmPropertyValue("Extension", new EdmStringConstant("0000"));
        var zipValue = new EdmPropertyValue("Zip", new EdmStructuredValue(null, new EdmPropertyValue[] { zipMain, zipExtension }));

        EdmPropertyValue address1Number = new EdmPropertyValue("Number", new EdmIntegerConstant(null, 1));
        EdmPropertyValue address1Street = new EdmPropertyValue("Street", new EdmStringConstant(null, "Joey Ramone Place"));
        EdmPropertyValue address1City = new EdmPropertyValue("City", new EdmStringConstant(null, "New York"));
        EdmPropertyValue address1State = new EdmPropertyValue("State", new EdmStringConstant(null, "New York"));
        EdmPropertyValue[] address1Properties = new EdmPropertyValue[] { address1Number, address1Street, address1City, address1State, zipValue };
        EdmStructuredValue address1Value = new EdmStructuredValue(new EdmEntityTypeReference(address, true), address1Properties);

        var phoneLocal = new EdmPropertyValue("Local", new EdmIntegerConstant(9991234));
        var phoneValue = new EdmPropertyValue("WorkPhone", new EdmStructuredValue(null, new EdmPropertyValue[] { phoneLocal }));
        var contactInfoValue = new EdmPropertyValue("ContactInfo", new EdmStructuredValue(null, new EdmPropertyValue[] { phoneValue }));

        EdmPropertyValue person1Name = new EdmPropertyValue("Name", new EdmStringConstant(null, "Joey Ramone"));
        EdmPropertyValue person1Birthday = new EdmPropertyValue("Birthday", new EdmDateTimeOffsetConstant(null, new DateTimeOffset(1951, 5, 19, 0, 0, 0, TimeSpan.Zero)));
        EdmPropertyValue person1CoolnessIndex = new EdmPropertyValue("CoolnessIndex", new EdmIntegerConstant(null, Int32.MaxValue));
        EdmPropertyValue person1Living = new EdmPropertyValue("Living", new EdmBooleanConstant(false));
        EdmPropertyValue person1Famous = new EdmPropertyValue("Famous", new EdmBooleanConstant(null, true));
        EdmPropertyValue person1Address = new EdmPropertyValue("Address", address1Value);

        EdmPropertyValue[] person1Properties = new EdmPropertyValue[] { person1Name, person1Birthday, person1CoolnessIndex, person1Living, person1Famous, person1Address, contactInfoValue };
        this.personValue = new EdmStructuredValue(new EdmEntityTypeReference(person, false), person1Properties);

        this.professionalValue = new EdmStructuredValue(new EdmEntityTypeReference(professional, false), person1Properties);
    }

    #endregion
}
