//---------------------------------------------------------------------
// <copyright file="VocabularyValidationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.E2E.Tests.StubEdm;
using Microsoft.OData.Edm.E2E.Tests.VocabularyStubs;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class VocabularyValidationTests : EdmLibTestCaseBase
{

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void SimpleVocabularyAnnotation(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Edm.Int32"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Annotation Term=""My.NS1.Int32Value"" Qualifier=""q3"" Int=""300"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""Self.Int32Value"" Qualifier=""q1"" Int=""100"" />
    <Annotation Term=""My.NS1.Int32Value"" Qualifier=""q2"" Int=""200"" />
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed && !errors.Any());

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any() && !serializationErrors.Any());

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult && !actualErrors.Any());

        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Term_NameConflict(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Edm.Int32"" />
  <Term Name=""Int32Value"" Type=""Edm.Int64"" />
</Schema>";
        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed && !errors.Any());

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.AlreadyDefined, actualErrors.Last().ErrorCode);
        Assert.Equal("An element with the name 'My.NS1.Int32Value' is already defined.", actualErrors.Last().ErrorMessage);

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Term_NameConflict_WithOthers(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Edm.Int32"" />
  <ComplexType Name=""Int32Value"">
    <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.AlreadyDefined, actualErrors.Last().ErrorCode);
        Assert.Equal("An element with the name 'My.NS1.Int32Value' is already defined.", actualErrors.Last().ErrorMessage);

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Term_TypeNotResolvable(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Self.UndefinedType"" />
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.BadUnresolvedType, actualErrors.Last().ErrorCode);
        Assert.Equal("The type 'My.NS1.UndefinedType' could not be found.", actualErrors.Last().ErrorMessage);

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_TargetNotResolvable(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int64Value"" Type=""Int64"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>  
  <Annotations Target=""My.NS1.Undefined"">
    <Annotation Term=""My.NS1.Int64Value"" Qualifier=""q1.q1"" Int=""100"" />
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.BadUnresolvedType, actualErrors.Last().ErrorCode);
        Assert.Equal("The type 'My.NS1.Undefined' could not be found.", actualErrors.Last().ErrorMessage);

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_TermNotResolvable(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Int32"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>  
  <Annotations Target=""My.NS1.Address"">
    <Annotation Term=""My.NS1.UndefinedTerm"" Qualifier=""q1"" Int=""144"" />
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_RecordTypeNotResolvable(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Int32"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>  
  <Annotations Target=""My.NS1.Address"">
    <Annotation Term=""My.NS1.Int32Value"" Qualifier=""q1"">
        <Record Type=""My.Ns1.UndefinedTerm"">
            <PropertyValue Property=""Unknown"" Int=""144"" />
        </Record>
    </Annotation>
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.True(actualErrors.Count() > 0);

        Assert.Equal(EdmErrorCode.BadUnresolvedType, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'foo' is not a valid integer. The value must be a valid 32 bit integer.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(9, 10)", actualErrors.Last().ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_Ambiguous_SameTermNoQualifer(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int64Value"" Type=""Int64"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Annotation Term=""My.NS1.Int64Value"" Int=""300"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.Int64Value"" Int=""100"" />
    <Annotation Term=""My.NS1.Int64Value"" Int=""200"" />
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(2, actualErrors.Count());

        Assert.All(actualErrors, e => Assert.Equal(EdmErrorCode.DuplicateAnnotation, e.ErrorCode));
        Assert.Equal("The value 'foo' is not a valid integer. The value must be a valid 32 bit integer.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(9, 6)", actualErrors.First().ErrorLocation.ToString());
        Assert.Equal("(10, 6)", actualErrors.Last().ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_Ambiguous_SameTermSameQualifer(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int64Value"" Type=""Int64"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Annotation Term=""My.NS1.Int64Value"" Qualifier=""q1.q1"" Int=""300"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.Int64Value"" Qualifier=""q1.q1"" Int=""100"" />
    <Annotation Term=""My.NS1.Int64Value"" Qualifier=""q1.q1"" Int=""200"" />
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(2, actualErrors.Count());

        Assert.All(actualErrors, e => Assert.Equal(EdmErrorCode.DuplicateAnnotation, e.ErrorCode));
        Assert.Equal("The value 'foo' is not a valid integer. The value must be a valid 32 bit integer.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(9, 6)", actualErrors.First().ErrorLocation.ToString());
        Assert.Equal("(10, 6)", actualErrors.Last().ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_TypeExactMatch(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int64Value"" Type=""Int64"" />
  <Term Name=""BooleanValue"" Type=""Boolean"" />
  <Term Name=""StructuredValue"" Type=""My.NS1.Address"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int64"" Nullable=""false""/>
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.Int64Value"" Int=""100"" />
    <Annotation Term=""My.NS1.BooleanValue"" Bool=""false"" />
    <Annotation Term=""My.NS1.StructuredValue"">
        <Record>
          <PropertyValue Property=""Id"" Int=""99"" />
        </Record>
    </Annotation>
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Empty(actualErrors);

        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_TypeNotMatch(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false""/>
  </ComplexType>
  <Term Name=""Int64Value"" Type=""Edm.Int64"" />
  <Term Name=""StructuredValue"" Type=""Self.Address"" />
  <Annotations Target=""Self.Address"">
    <Annotation Term=""Self.StructuredValue"" Int=""100"" />
    <Annotation Term=""Self.Int64Value"" Int=""200"" />
    <Annotation Term=""Self.Int64Value"" Qualifier=""Other"">
        <Record>
          <PropertyValue Property=""Id"" Int=""99"" />
        </Record>
    </Annotation>
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(2, actualErrors.Count());

        Assert.Equal(EdmErrorCode.PrimitiveConstantExpressionNotValidForNonPrimitiveType, actualErrors.First().ErrorCode);
        Assert.Equal(EdmErrorCode.RecordExpressionNotValidForNonStructuredType, actualErrors.Last().ErrorCode);
        Assert.Equal("(9, 6)", actualErrors.First().ErrorLocation.ToString());
        Assert.Equal("(12, 10)", actualErrors.Last().ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_TypeStructure_PropertyNameNotMatch(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StructuredValue"" Type=""My.NS1.Address"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int32"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.StructuredValue"">
        <Record>
          <PropertyValue Property=""Undefined"" Int=""99"" />
        </Record>
    </Annotation>
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.RecordExpressionHasExtraProperties, actualErrors.First().ErrorCode);
        Assert.Equal("(9, 10)", actualErrors.First().ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    // , Variation(Id = 46, SkipReason = @"[EdmLib] [Validator] Validation error is occuring for term property that are nullable -- postponed")]
    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_TypeStructure_NullablePropertyUndeclared(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StructuredValue"" Type=""My.NS1.Address"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""String"" Type=""Edm.String"" Nullable=""true"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.StructuredValue"">
        <Record>
          <PropertyValue Property=""Id"" Int=""99"" />
        </Record>
    </Annotation>
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Empty(actualErrors);

        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_TypeStructure_PropertyTypeNotMatch(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StructuredValue"" Type=""My.NS1.Address"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.StructuredValue"">
        <Record>
          <PropertyValue Property=""Id"" String=""99"" />
        </Record>
    </Annotation>
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.First().ErrorCode);
        Assert.Equal("(10, 12)", actualErrors.Last().ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_TypeStructure_Nested(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StructuredValue"" Type=""My.NS1.Address"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int64"" Nullable=""false""/>
    <Property Name=""DeskPhone"" Type=""My.NS1.Phone"" />
    <Property Name=""MobilePhone"" Type=""My.NS1.Phone"" />
  </ComplexType>  
  <ComplexType Name=""Phone"">
    <Property Name=""Area"" Type=""String"" />
    <Property Name=""Extension"" Type=""String"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.StructuredValue"">
        <Record>
          <PropertyValue Property=""Id"" Int=""99"" />
          <PropertyValue Property=""MobilePhone"">
            <Record>
              <PropertyValue Property=""Area"" String=""425"" />
              <PropertyValue Property=""Extension"" String=""0001234"" />
            </Record>
          </PropertyValue>
          <PropertyValue Property=""DeskPhone"">
            <Record>
              <PropertyValue Property=""Area"" String=""206"" />
              <PropertyValue Property=""Extension"" String=""0004321"" />
            </Record>
          </PropertyValue>
        </Record>
    </Annotation>
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Empty(actualErrors);

        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_TypeStructure_NestedPropertyNotMatch(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StructuredValue"" Type=""My.NS1.Address"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int64"" Nullable=""false""/>
    <Property Name=""DeskPhone"" Type=""My.NS1.Phone"" />
    <Property Name=""MobilePhone"" Type=""My.NS1.Phone"" />
  </ComplexType>  
  <ComplexType Name=""Phone"">
    <Property Name=""Area"" Type=""String"" />
    <Property Name=""Extension"" Type=""String"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.StructuredValue"">
        <Record>
          <PropertyValue Property=""Id"" Int=""99"" />
          <PropertyValue Property=""DeskPhone"" Int=""99"" />
          <PropertyValue Property=""MobilePhone"">
            <Record>
              <PropertyValue Property=""Undefined"" String=""0001234"" />
              <PropertyValue Property=""Area"" Int=""425"" />
            </Record>
          </PropertyValue>
        </Record>
    </Annotation>
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(3, actualErrors.Count());

        Assert.Equal(EdmErrorCode.PrimitiveConstantExpressionNotValidForNonPrimitiveType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.ElementAt(1).ErrorCode);
        Assert.Equal(EdmErrorCode.RecordExpressionHasExtraProperties, actualErrors.ElementAt(2).ErrorCode);
        Assert.Equal("(17, 12)", actualErrors.ElementAt(0).ErrorLocation.ToString());
        Assert.Equal("(21, 16)", actualErrors.ElementAt(1).ErrorLocation.ToString());
        Assert.Equal("(19, 14)", actualErrors.ElementAt(2).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_TypeConvertible(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""SByteValue"" Type=""SByte"" />
  <Term Name=""ByteValue"" Type=""Byte"" />
  <Term Name=""Int16Value"" Type=""Int16"" />
  <Term Name=""Int32Value"" Type=""Int32"" />
  <Term Name=""Int64Value"" Type=""Int64"" />
  <Term Name=""DoubleValue"" Type=""Double"" />
  <Term Name=""SingleValue"" Type=""Single"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int64"" Nullable=""false""/>
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.SByteValue"" Int=""-100"" />
    <Annotation Term=""My.NS1.ByteValue"" Int=""100"" />
    <Annotation Term=""My.NS1.Int16Value"" Int=""-100"" />
    <Annotation Term=""My.NS1.Int32Value"" Int=""-100"" />
    <Annotation Term=""My.NS1.Int64Value"" Int=""-100"" />
    <Annotation Term=""My.NS1.DoubleValue"" Float=""3.1416"" />
    <Annotation Term=""My.NS1.SingleValue"" Float=""3.1416"" />
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Empty(actualErrors);

        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_ValueOutOfRange_BadFormat(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""SByteValue"" Type=""SByte"" />
  <Term Name=""ByteValue"" Type=""Byte"" />
  <Term Name=""Int64Value"" Type=""Int64"" />
  <Term Name=""DoubleValue"" Type=""Double"" />
  <Term Name=""DateTimeValue"" Type=""DateTimeOffset"" />
  <Term Name=""GuidValue"" Type=""Guid"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int64"" Nullable=""false""/>
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.SByteValue"" Int=""-129"" />
    <Annotation Term=""My.NS1.ByteValue"" Int=""256"" />

    <Annotation Term=""My.NS1.Int64Value"" Int=""Not a Number"" />
    <Annotation Term=""My.NS1.DoubleValue"" Float=""Not a Number"" />

    <Annotation Term=""My.NS1.DateTimeValue"" DateTimeOffset=""Not a Date"" />
    <Annotation Term=""My.NS1.GuidValue"" Guid=""4ae71c81-c21a"" />
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(6, actualErrors.Count());

        Assert.Equal(EdmErrorCode.InvalidInteger, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal(EdmErrorCode.InvalidFloatingPoint, actualErrors.ElementAt(1).ErrorCode);
        Assert.Equal(EdmErrorCode.InvalidDateTimeOffset, actualErrors.ElementAt(2).ErrorCode);
        Assert.Equal(EdmErrorCode.InvalidGuid, actualErrors.ElementAt(3).ErrorCode);
        Assert.Equal(EdmErrorCode.IntegerConstantValueOutOfRange, actualErrors.ElementAt(4).ErrorCode);
        Assert.Equal(EdmErrorCode.IntegerConstantValueOutOfRange, actualErrors.ElementAt(5).ErrorCode);

        Assert.Equal("(16, 6)", actualErrors.ElementAt(0).ErrorLocation.ToString());
        Assert.Equal("(17, 6)", actualErrors.ElementAt(1).ErrorLocation.ToString());
        Assert.Equal("(19, 6)", actualErrors.ElementAt(2).ErrorLocation.ToString());
        Assert.Equal("(20, 6)", actualErrors.ElementAt(3).ErrorLocation.ToString());
        Assert.Equal("(13, 6)", actualErrors.ElementAt(4).ErrorLocation.ToString());
        Assert.Equal("(14, 6)", actualErrors.ElementAt(5).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_Path(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Int32"" />
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>  
  <Annotations Target=""Self.Person"">
    <Annotation Term=""My.NS1.Int32Value"">
      <Path>Id</Path>
    </Annotation>
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Empty(actualErrors);

        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_Path_NotValid(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Int32"" />
  <EntityType Name=""Person"">
    <Key><PropertyRef Name=""Id"" /></Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>  
  <Annotations Target=""Self.Person"">
    <Annotation Term=""My.NS1.Int32Value"">
      <Path>Undefined</Path>
    </Annotation>
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Empty(actualErrors);

        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_If(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StringValue"" Type=""String"" />
  <EntityType Name=""Person"">
    <Key><PropertyRef Name=""Id"" /></Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>  
  <Annotations Target=""Self.Person"">
    <Annotation Term=""My.NS1.StringValue"">
      <If>
          <IsType Type=""Int32"">
              <Path>Id</Path>
          </IsType>
          <String>100</String>
          <String>200</String>
      </If>
    </Annotation>
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Empty(actualErrors);

        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_If_TypeNotMatch(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StringValue"" Type=""String"" />
  <EntityType Name=""Person"">
    <Key><PropertyRef Name=""Id"" /></Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>  
  <Annotations Target=""Self.Person"">
    <Annotation Term=""My.NS1.StringValue"">
      <If>
        <Bool>false</Bool>
        <String>100</String>
        <Int>200</Int>
      </If>
    </Annotation>
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("(13, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_IsType_TypeNotResolved(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StringValue"" Type=""String"" />
  <EntityType Name=""Person"">
    <Key><PropertyRef Name=""Id"" /></Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>  
  <Annotations Target=""Self.Person"">
    <Annotation Term=""My.NS1.StringValue"">
      <If>
        <IsType Type=""My.NS1.Undefined"">
          <Path>Id</Path>
        </IsType>
        <String>100</String>
        <Int>200</Int>
      </If>
    </Annotation>
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(2, actualErrors.Count());

        Assert.Equal(EdmErrorCode.BadUnresolvedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.ElementAt(1).ErrorCode);

        Assert.Equal("(11, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());
        Assert.Equal("(15, 10)", actualErrors.ElementAt(1).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_FunctionApplication(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StringValue"" Type=""String"" />
  <EntityType Name=""Person"">
    <Key><PropertyRef Name=""Id"" /></Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <Function Name=""StringConcat"">
    <ReturnType Type=""String""/>
    <Parameter Name=""String1"" Type=""String"" />
    <Parameter Name=""String2"" Type=""String"" />
  </Function>  
  <Annotations Target=""Self.Person"">
    <Annotation Term=""My.NS1.StringValue"">
       <Apply Function=""Self.StringConcat"">
         <LabeledElement Name=""FunctionParameter""><String>-</String></LabeledElement>
         <String>100</String>
       </Apply>
    </Annotation>
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Empty(actualErrors);

        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_FunctionApplication_TypeNotMatch(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StringValue"" Type=""String"" />
  <EntityType Name=""Person"">
    <Key><PropertyRef Name=""Id"" /></Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>  
  <Annotations Target=""Self.Person"">
    <Annotation Term=""My.NS1.StringValue"">
      <If>
        <Bool>false</Bool>
        <String>100</String>
        <Int>200</Int>
      </If>
    </Annotation>
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.BadUnresolvedOperation, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("(16, 9)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateVocabularyWithIncorrectType(EdmVersion edmVersion)
    {
        StubEdmModel model = new StubEdmModel();
        CreateVocabularyDefinitions(model);

        var person = new StubEdmEntityType("NS1", "Person");
        model.Add(person);

        var reviewValueTerm = new StubTerm("NS1", "hReviewTerm");
        reviewValueTerm.Type = new StubEdmTypeReference() { Definition = model.FindType("NS1.hReviewEntity"), IsNullable = true };
        model.Add(reviewValueTerm);

        var reviewValueAnnotation = new StubVocabularyAnnotation()
        {
            Term = reviewValueTerm,
            Value = new StubStringConstantExpression("this should be Record"),
        };

        person.AddVocabularyAnnotation(reviewValueAnnotation);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionNotValidForTheAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("(16, 9)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateTermOnly(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ValueTerm"" Type=""Edm.Int32"" />
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Empty(actualErrors);

        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateTermWithAnnotationTarget(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Simple"" Type=""Edm.Int32"" />
    <Term Name=""Age"" Type=""Edm.Int32"" />
    <Annotations Target=""DefaultNamespace.Simple"">
        <Annotation Term=""DefaultNamespace.Age"" Int=""22"" />
    </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Empty(actualErrors);

        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Fact]
    public void ValidateSimpleVocabularyAnnotation()
    {
        var model = new EdmModel();

        var complexType = new EdmComplexType("Foo", "SimpleType");
        var stringValue = complexType.AddStructuralProperty("StringValue", EdmCoreModel.Instance.GetString(true));
        model.AddElement(complexType);

        var entityType = new EdmEntityType("Foo", "SimpleEntity");
        var intValue = entityType.AddStructuralProperty("Int32Value", EdmCoreModel.Instance.GetInt32(false));
        entityType.AddKeys(intValue);
        model.AddElement(entityType);

        model.AddElement(new EdmTerm("Foo", "SimpleTerm", EdmCoreModel.Instance.GetInt32(true)));
        model.AddElement(new EdmTerm("Foo", "ComplexTerm", new EdmComplexTypeReference(complexType, true)));
        model.AddElement(new EdmTerm("Foo", "EntityTerm", new EdmEntityTypeReference(entityType, true)));

        var simpleTerm = model.FindTerm("Foo.SimpleTerm");
        var complexTerm = model.FindTerm("Foo.ComplexTerm");

        EdmVocabularyAnnotation inlineValueAnnotation = new EdmVocabularyAnnotation(
            simpleTerm,
            simpleTerm,
            new EdmIntegerConstant(1));
        inlineValueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
        model.AddVocabularyAnnotation(inlineValueAnnotation);

        EdmVocabularyAnnotation outOfLineValueAnnotation = new EdmVocabularyAnnotation(
            complexTerm,
            simpleTerm,
            new EdmIntegerConstant(2));
        outOfLineValueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        model.AddVocabularyAnnotation(outOfLineValueAnnotation);

        Assert.Equal(2, model.VocabularyAnnotations.Count());

        model.Validate(out IEnumerable<EdmError> actualErrors);

        Assert.Empty(actualErrors);
    }

    [Fact]
    public void ValidateDefaultAnnotation()
    {
        var model = new EdmModel();

        var complexType = new EdmComplexType("Foo", "SimpleType");
        var stringValue = complexType.AddStructuralProperty("StringValue", EdmCoreModel.Instance.GetString(true));
        model.AddElement(complexType);

        var entityType = new EdmEntityType("Foo", "SimpleEntity");
        var intValue = entityType.AddStructuralProperty("Int32Value", EdmCoreModel.Instance.GetInt32(false));
        entityType.AddKeys(intValue);
        model.AddElement(entityType);

        model.AddElement(new EdmTerm("Foo", "SimpleTerm", EdmCoreModel.Instance.GetInt32(true)));
        model.AddElement(new EdmTerm("Foo", "ComplexTerm", new EdmComplexTypeReference(complexType, true)));
        model.AddElement(new EdmTerm("Foo", "EntityTerm", new EdmEntityTypeReference(entityType, true)));

        var annotation = new MutableVocabularyAnnotation();
        Assert.Throws<InvalidOperationException>(() => model.AddVocabularyAnnotation(annotation));
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateDefaultVocabularyAnnotationWithTargetOnly(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var complexType = new EdmComplexType("Foo", "SimpleType");
        var stringValue = complexType.AddStructuralProperty("StringValue", EdmCoreModel.Instance.GetString(true));
        model.AddElement(complexType);

        var entityType = new EdmEntityType("Foo", "SimpleEntity");
        var intValue = entityType.AddStructuralProperty("Int32Value", EdmCoreModel.Instance.GetInt32(false));
        entityType.AddKeys(intValue);
        model.AddElement(entityType);

        model.AddElement(new EdmTerm("Foo", "SimpleTerm", EdmCoreModel.Instance.GetInt32(true)));
        model.AddElement(new EdmTerm("Foo", "ComplexTerm", new EdmComplexTypeReference(complexType, true)));
        model.AddElement(new EdmTerm("Foo", "EntityTerm", new EdmEntityTypeReference(entityType, true)));

        var valueAnnotation = new MutableVocabularyAnnotation()
        {
            Target = new EdmEntityType("", "")
        };
        model.AddVocabularyAnnotation(valueAnnotation);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(2, actualErrors.Count());

        Assert.Equal(EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The value of the property 'IEdmVocabularyAnnotation.Term' must not be null.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal(EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull, actualErrors.ElementAt(1).ErrorCode);
        Assert.Equal("The value of the property 'IEdmVocabularyAnnotation.Value' must not be null.", actualErrors.ElementAt(1).ErrorMessage);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.False(serializedCsdls.Any());
        Assert.True(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVocabularyAnnotationSerializationLocation.OutOfLine)]
    [InlineData(EdmVocabularyAnnotationSerializationLocation.Inline)]
    public void ValidateLocationAnnotationWithInvalidTargetModel(EdmVocabularyAnnotationSerializationLocation location)
    {
        var model = new EdmModel();

        var complexType = new EdmComplexType("Foo", "SimpleType");
        var stringValue = complexType.AddStructuralProperty("StringValue", EdmCoreModel.Instance.GetString(true));
        model.AddElement(complexType);

        var simpleEntityType = new EdmEntityType("Foo", "SimpleEntity");
        var intValue = simpleEntityType.AddStructuralProperty("Int32Value", EdmCoreModel.Instance.GetInt32(false));
        simpleEntityType.AddKeys(intValue);
        model.AddElement(simpleEntityType);

        model.AddElement(new EdmTerm("Foo", "SimpleTerm", EdmCoreModel.Instance.GetInt32(true)));
        model.AddElement(new EdmTerm("Foo", "ComplexTerm", new EdmComplexTypeReference(complexType, true)));
        model.AddElement(new EdmTerm("Foo", "EntityTerm", new EdmEntityTypeReference(simpleEntityType, true)));
        var simpleTerm = model.FindTerm("Foo.SimpleTerm");
        var entityTerm = model.FindTerm("Foo.EntityTerm");
        var entityType = model.FindType("Foo.SimpleEntity") as EdmEntityType;
        var entityTypeProperty = entityType.FindProperty("Int32Value");

        var invalidType = new EdmComplexType("Foo", "InvalidType");
        var invalidTypeProperty = invalidType.AddStructuralProperty("InvalidValue", EdmCoreModel.Instance.GetString(true));

        var invalidTerm = new EdmTerm("Foo", "InvalidTerm", EdmCoreModel.Instance.GetString(false));

        var invalidEntitySet = new EdmEntitySet(new EdmEntityContainer("", ""), "InvalidEntitySet", entityType);

        var invalidFunction = new EdmFunction("Foo", "InvalidFunction", EdmCoreModel.Instance.GetInt32(true));
        var invalidFunctionParameter = invalidFunction.AddParameter("InvalidParameter", EdmCoreModel.Instance.GetInt32(true));

        model.AddElement(invalidFunction);
        var invalidFunctionImport = new EdmFunctionImport(new EdmEntityContainer("", ""), "InvalidFunctionImport", invalidFunction);

        EdmVocabularyAnnotation valueAnnotationOnContainer = new EdmVocabularyAnnotation(
            new EdmEntityContainer("", ""),
            simpleTerm,
            new EdmIntegerConstant(1));
        valueAnnotationOnContainer.SetSerializationLocation(model, location);
        model.AddVocabularyAnnotation(valueAnnotationOnContainer);

        EdmVocabularyAnnotation valueAnnotationOnEntitySet = new EdmVocabularyAnnotation(
            invalidEntitySet,
            simpleTerm,
            new EdmIntegerConstant(1));
        valueAnnotationOnEntitySet.SetSerializationLocation(model, location);
        model.AddVocabularyAnnotation(valueAnnotationOnEntitySet);

        EdmVocabularyAnnotation valueAnnotationOnType = new EdmVocabularyAnnotation(
            invalidType,
            simpleTerm,
            new EdmIntegerConstant(1));
        valueAnnotationOnType.SetSerializationLocation(model, location);
        model.AddVocabularyAnnotation(valueAnnotationOnType);

        EdmVocabularyAnnotation valueAnnotationOnProperty = new EdmVocabularyAnnotation(
            invalidTypeProperty,
            simpleTerm,
            new EdmIntegerConstant(1));
        valueAnnotationOnProperty.SetSerializationLocation(model, location);
        model.AddVocabularyAnnotation(valueAnnotationOnProperty);

        EdmVocabularyAnnotation valueAnnotationOnTerm = new EdmVocabularyAnnotation(
            invalidTerm,
            simpleTerm,
            new EdmIntegerConstant(1));
        valueAnnotationOnTerm.SetSerializationLocation(model, location);
        model.AddVocabularyAnnotation(valueAnnotationOnTerm);

        EdmVocabularyAnnotation valueAnnotationOnFunction = new EdmVocabularyAnnotation(
            invalidFunction,
            simpleTerm,
            new EdmIntegerConstant(1));
        valueAnnotationOnFunction.SetSerializationLocation(model, location);
        model.AddVocabularyAnnotation(valueAnnotationOnFunction);

        EdmVocabularyAnnotation valueAnnotationOnParameter = new EdmVocabularyAnnotation(
            invalidFunctionParameter,
            simpleTerm,
            new EdmIntegerConstant(1));
        valueAnnotationOnParameter.SetSerializationLocation(model, location);
        model.AddVocabularyAnnotation(valueAnnotationOnParameter);

        EdmVocabularyAnnotation valueAnnotationOnFunctionImport = new EdmVocabularyAnnotation(
            invalidFunctionImport,
            simpleTerm,
            new EdmIntegerConstant(1));
        valueAnnotationOnFunctionImport.SetSerializationLocation(model, location);
        model.AddVocabularyAnnotation(valueAnnotationOnFunctionImport);

        EdmVocabularyAnnotation valueAnnotation = new EdmVocabularyAnnotation(
            new EdmEntityContainer("", ""),
            entityTerm,
            new EdmRecordExpression(new EdmPropertyConstructor(entityTypeProperty.Name, new EdmIntegerConstant(1))));
        valueAnnotation.SetSerializationLocation(model, location);
        model.AddVocabularyAnnotation(valueAnnotation);

        Assert.Equal(9, model.VocabularyAnnotations.Count());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateOutOfLineAnnotationWithValidTargetsInReferencedModel(EdmVersion edmVersion)
    {
        // Test that annotation targets which live in a referenced model don't cause a validation failure
        const string rawModelCsdl = @"
<Schema Namespace=""ReferencedNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""SomeValueTerm"" Type=""Edm.String"" />
  <EntityContainer Name=""SomeContainer"">
    <EntitySet Name=""SomeEntitySet"" EntityType=""ReferencedNS.SomeEntityType"" />
    <ActionImport Name=""SomeFunctionImport"" Action=""ReferencedNS.SomeFunction"" />
  </EntityContainer>
  <EntityType Name=""SomeEntityType"">
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Edm.String"" Nullable=""false"" />
  </EntityType>
  <Action Name=""SomeFunction""><ReturnType Type=""Edm.Int32""/>
    <Parameter Name=""SomeFunctionImportParameter"" Type=""Edm.String"" />
  </Action>
</Schema>";
        const string annotationModelCsdl = @"
<Schema Namespace=""AnnotationNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""CommonValueTerm"" Type=""Edm.String"" />
  <Annotations Target=""ReferencedNS.SomeContainer"">
    <Annotation Term=""AnnotationNS.CommonValueTerm"" String=""Hello world!"" />
  </Annotations>
  <Annotations Target=""ReferencedNS.SomeContainer/SomeEntitySet"">
    <Annotation Term=""AnnotationNS.CommonValueTerm"" String=""Hello world!"" />
  </Annotations>
  <Annotations Target=""ReferencedNS.SomeEntityType"">
    <Annotation Term=""AnnotationNS.CommonValueTerm"" String=""Hello world!"" />
  </Annotations>
  <Annotations Target=""ReferencedNS.SomeValueTerm"">
    <Annotation Term=""AnnotationNS.CommonValueTerm"" String=""Hello world!"" />
  </Annotations>
  <Annotations Target=""ReferencedNS.SomeFunction(Edm.String)"">
    <Annotation Term=""AnnotationNS.CommonValueTerm"" String=""Hello world!"" />
  </Annotations>
  <Annotations Target=""ReferencedNS.SomeContainer/SomeFunctionImport"">
    <Annotation Term=""AnnotationNS.CommonValueTerm"" String=""Hello world!"" />
  </Annotations>
  <Annotations Target=""ReferencedNS.SomeFunction(Edm.String)/SomeFunctionImportParameter"">
    <Annotation Term=""AnnotationNS.CommonValueTerm"" String=""Hello world!"" />
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { rawModelCsdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel rawModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        csdlElements = new string[] { annotationModelCsdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), rawModel, out IEdmModel modelWithAnnotations, out errors);
        Assert.True(isParsed);

        Assert.Equal(7, modelWithAnnotations.VocabularyAnnotations.Count());

        var ruleSet = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = modelWithAnnotations.Validate(ruleSet, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateUnresolvedInlineAnnotationTargets(EdmVersion edmVersion)
    {
        // Test that unresolved annotation terms don't cause a validation failure
        const string csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""CommonValueTerm"" Type=""Edm.String"" />
  <EntityContainer Name=""SomeContainer"">
    <EntitySet Name=""SomeEntitySet"" EntityType=""NS.SomeEntityType"">
      <Annotation Term=""NS.CommonValueTerm"" String=""Hello world!"" />
      <Annotation Term=""NS.UnknownValueTerm"" String=""Hello world!"" />
      <Annotation Term=""RefNS.UnknownValueTerm"" String=""Hello world!""/>
    </EntitySet>
    <ActionImport Name=""SomeFunctionImport"" Action=""NS.SomeFunction"" >
      <Annotation Term=""NS.CommonValueTerm"" String=""Hello world!"" />
      <Annotation Term=""AnnotationNS.UnknownValueTerm"" String=""Hello world!"" />
      <Annotation Term=""RefNS.UnknownValueTerm"" String=""Hello world!""/>
    </ActionImport>
    <Annotation Term=""NS.CommonValueTerm"" String=""Hello world!"">
      <Annotation Term=""AnnotationNS.UnknownValueTerm"" String=""Hello world!"" />
      <Annotation Term=""RefNS.UnknownValueTerm"" String=""Hello world!""/>
    </Annotation>
    <Annotation Term=""AnnotationNS.UnknownValueTerm"" String=""Hello world!"" />
    <Annotation Term=""RefNS.UnknownValueTerm"" String=""Hello world!""/>
  </EntityContainer>
  <EntityType Name=""SomeEntityType"">
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""NS.CommonValueTerm"" String=""Hello world!"" />
      <Annotation Term=""AnnotationNS.UnknownValueTerm"" String=""Hello world!"" />
      <Annotation Term=""RefNS.UnknownValueTerm"" String=""Hello world!""/>
    </Property>
    <Annotation Term=""NS.CommonValueTerm"" String=""Hello world!"" />
    <Annotation Term=""AnnotationNS.UnknownValueTerm"" String=""Hello world!"" />
    <Annotation Term=""RefNS.UnknownValueTerm"" String=""Hello world!""/>
  </EntityType>
  <Action Name=""SomeFunction"">
    <ReturnType Type=""Edm.Int32""/>
    <Parameter Name=""SomeFunctionImportParameter"" Type=""Edm.String"" >
      <Annotation Term=""NS.CommonValueTerm"" String=""Hello world!"" />
      <Annotation Term=""AnnotationNS.UnknownValueTerm"" String=""Hello world!"" />
      <Annotation Term=""RefNS.UnknownValueTerm"" String=""Hello world!""/>
    </Parameter>
    <Annotation Term=""NS.CommonValueTerm"" String=""Hello world!"" />
    <Annotation Term=""AnnotationNS.UnknownValueTerm"" String=""Hello world!"" />
    <Annotation Term=""RefNS.UnknownValueTerm"" String=""Hello world!""/>
  </Action>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        Assert.Equal(21, model.VocabularyAnnotations.Count());

        var ruleSet = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = model.Validate(ruleSet, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
    }


    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateUnresolvedInlineAnnotationTargetsWithUnresolvedTypes(EdmVersion edmVersion)
    {
        // Test that unresolved annotation term types load and raise appropriate validation failures
        const string csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""CommonValueTerm"" Type=""Edm.String"" />
  <EntityContainer Name=""SomeContainer"">
    <EntitySet Name=""SomeEntitySet"" EntityType=""NS.SomeEntityType"">
      <Annotation Term=""RefNS.UnknownValueTerm"">
        <Record Type=""RefNs.UnknownType"">
          <PropertyValue Property=""Property"" String=""PropertyValue"" />
        </Record>
      </Annotation>
    </EntitySet>
    <ActionImport Name=""SomeFunctionImport"" Action=""NS.SomeFunction"" >
      <Annotation Term=""RefNS.UnknownValueTerm"">
        <Record Type=""RefNs.UnknownType"">
          <PropertyValue Property=""Property"" String=""PropertyValue"" />
        </Record>
      </Annotation>
    </ActionImport>
    <Annotation Term=""NS.CommonValueTerm"" String=""Hello world!"">
      <Annotation Term=""RefNS.UnknownValueTerm"">
        <Record Type=""RefNs.UnknownType"">
          <PropertyValue Property=""Property"" String=""PropertyValue"" />
        </Record>
      </Annotation>
    </Annotation>
    <Annotation Term=""RefNS.UnknownValueTerm"">
      <Record Type=""RefNs.UnknownType"">
        <PropertyValue Property=""Property"" String=""PropertyValue"" />
      </Record>
    </Annotation>
  </EntityContainer>
  <EntityType Name=""SomeEntityType"">
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""RefNS.UnknownValueTerm"">
        <Record Type=""RefNs.UnknownType"">
          <PropertyValue Property=""Property"" String=""PropertyValue"" />
        </Record>
      </Annotation>
    </Property>
    <Annotation Term=""RefNS.UnknownValueTerm"">
      <Record Type=""RefNs.UnknownType"">
        <PropertyValue Property=""Property"" String=""PropertyValue"" />
      </Record>
    </Annotation>
  </EntityType>
  <Action Name=""SomeFunction""><ReturnType Type=""Edm.Int32""/>
    <Parameter Name=""SomeFunctionImportParameter"" Type=""Edm.String"" >
      <Annotation Term=""RefNS.UnknownValueTerm"">
        <Record Type=""RefNs.UnknownType"">
          <PropertyValue Property=""Property"" String=""PropertyValue"" />
        </Record>
      </Annotation>
    </Parameter>
    <Annotation Term=""RefNS.UnknownValueTerm"">
      <Record Type=""RefNs.UnknownType"">
        <PropertyValue Property=""Property"" String=""PropertyValue"" />
      </Record>
    </Annotation>
  </Action>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        Assert.Equal(8, model.VocabularyAnnotations.Count());

        var ruleSet = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = model.Validate(ruleSet, out IEnumerable<EdmError> actualErrors);
        Assert.Equal(7, actualErrors.Count());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateAnnotationWithInvalidTerm(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var complexType = new EdmComplexType("Foo", "SimpleType");
        var stringValue = complexType.AddStructuralProperty("StringValue", EdmCoreModel.Instance.GetString(true));
        model.AddElement(complexType);

        var simpleEntity = new EdmEntityType("Foo", "SimpleEntity");
        var intValue = simpleEntity.AddStructuralProperty("Int32Value", EdmCoreModel.Instance.GetInt32(false));
        simpleEntity.AddKeys(intValue);
        model.AddElement(simpleEntity);

        model.AddElement(new EdmTerm("Foo", "SimpleTerm", EdmCoreModel.Instance.GetInt32(true)));
        model.AddElement(new EdmTerm("Foo", "ComplexTerm", new EdmComplexTypeReference(complexType, true)));
        model.AddElement(new EdmTerm("Foo", "EntityTerm", new EdmEntityTypeReference(simpleEntity, true)));

        var simpleTerm = model.FindDeclaredTerm("Foo.SimpleTerm");
        var entityType = model.FindDeclaredType("Foo.SimpleEntity") as EdmEntityType;
        var entityId = entityType.FindProperty("Int32Value");

        EdmVocabularyAnnotation inlineValueAnnotation = new EdmVocabularyAnnotation(
            simpleTerm,
            new EdmTerm("Bar", "Foo", EdmCoreModel.Instance.GetInt32(false)),
            new EdmIntegerConstant(1));
        inlineValueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
        model.AddVocabularyAnnotation(inlineValueAnnotation);

        Assert.Single(model.VocabularyAnnotations);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.BadUnresolvedTerm, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("(Microsoft.OData.Edm.Vocabularies.EdmVocabularyAnnotation)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateDuplicateVocabularyAnnotation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var complexType = new EdmComplexType("Foo", "SimpleType");
        var stringValue = complexType.AddStructuralProperty("StringValue", EdmCoreModel.Instance.GetString(true));
        model.AddElement(complexType);

        var simpleEntity = new EdmEntityType("Foo", "SimpleEntity");
        var intValue = simpleEntity.AddStructuralProperty("Int32Value", EdmCoreModel.Instance.GetInt32(false));
        simpleEntity.AddKeys(intValue);
        model.AddElement(simpleEntity);

        model.AddElement(new EdmTerm("Foo", "SimpleTerm", EdmCoreModel.Instance.GetInt32(true)));
        model.AddElement(new EdmTerm("Foo", "ComplexTerm", new EdmComplexTypeReference(complexType, true)));
        model.AddElement(new EdmTerm("Foo", "EntityTerm", new EdmEntityTypeReference(simpleEntity, true)));

        var simpleTerm = model.FindTerm("Foo.SimpleTerm");

        EdmVocabularyAnnotation inlineValueAnnotation = new EdmVocabularyAnnotation(
            simpleTerm,
            simpleTerm,
            new EdmIntegerConstant(1));
        inlineValueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
        model.AddVocabularyAnnotation(inlineValueAnnotation);

        inlineValueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        model.AddVocabularyAnnotation(inlineValueAnnotation);

        Assert.Equal(2, model.VocabularyAnnotations.Count());

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.DuplicateAnnotation, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("(Microsoft.OData.Edm.Vocabularies.EdmVocabularyAnnotation)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateDuplicateVocabularyAnnotationWithQualifier(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var complexType = new EdmComplexType("Foo", "SimpleType");
        var stringValue = complexType.AddStructuralProperty("StringValue", EdmCoreModel.Instance.GetString(true));
        model.AddElement(complexType);

        var simpleEntity = new EdmEntityType("Foo", "SimpleEntity");
        var intValue = simpleEntity.AddStructuralProperty("Int32Value", EdmCoreModel.Instance.GetInt32(false));
        simpleEntity.AddKeys(intValue);
        model.AddElement(simpleEntity);

        model.AddElement(new EdmTerm("Foo", "SimpleTerm", EdmCoreModel.Instance.GetInt32(true)));
        model.AddElement(new EdmTerm("Foo", "ComplexTerm", new EdmComplexTypeReference(complexType, true)));
        model.AddElement(new EdmTerm("Foo", "EntityTerm", new EdmEntityTypeReference(simpleEntity, true)));

        var simpleTerm = model.FindTerm("Foo.SimpleTerm");

        EdmVocabularyAnnotation inlineValueAnnotation = new EdmVocabularyAnnotation(
            simpleTerm,
            simpleTerm,
            "q1",
            new EdmIntegerConstant(1));
        inlineValueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
        model.AddVocabularyAnnotation(inlineValueAnnotation);

        inlineValueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        model.AddVocabularyAnnotation(inlineValueAnnotation);

        Assert.Equal(2, model.VocabularyAnnotations.Count());

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.DuplicateAnnotation, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("(Microsoft.OData.Edm.Vocabularies.EdmVocabularyAnnotation)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateVocabularyAnnotationWithQualifier(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var complexType = new EdmComplexType("Foo", "SimpleType");
        var stringValue = complexType.AddStructuralProperty("StringValue", EdmCoreModel.Instance.GetString(true));
        model.AddElement(complexType);

        var simpleEntity = new EdmEntityType("Foo", "SimpleEntity");
        var intValue = simpleEntity.AddStructuralProperty("Int32Value", EdmCoreModel.Instance.GetInt32(false));
        simpleEntity.AddKeys(intValue);
        model.AddElement(simpleEntity);

        model.AddElement(new EdmTerm("Foo", "SimpleTerm", EdmCoreModel.Instance.GetInt32(true)));
        model.AddElement(new EdmTerm("Foo", "ComplexTerm", new EdmComplexTypeReference(complexType, true)));
        model.AddElement(new EdmTerm("Foo", "EntityTerm", new EdmEntityTypeReference(simpleEntity, true)));

        var simpleTerm = model.FindTerm("Foo.SimpleTerm");
        var complexTerm = model.FindTerm("Foo.ComplexTerm");

        EdmVocabularyAnnotation inlineValueAnnotation = new EdmVocabularyAnnotation(
            complexTerm,
            simpleTerm,
            "q1",
            new EdmIntegerConstant(1));
        inlineValueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
        model.AddVocabularyAnnotation(inlineValueAnnotation);

        EdmVocabularyAnnotation inlineValueAnnotation2 = new EdmVocabularyAnnotation(
            complexTerm,
            simpleTerm,
            "q2",
            new EdmIntegerConstant(1));
        inlineValueAnnotation2.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        model.AddVocabularyAnnotation(inlineValueAnnotation2);
        Assert.Equal(2, model.VocabularyAnnotations.Count());

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Empty(actualErrors);

        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void VocabularyAnnotation_TypeStructure_NullablePropertyWithNullExpression(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StructuredValue"" Type=""My.NS1.Address"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""String"" Type=""Edm.String"" Nullable=""true"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.StructuredValue"">
        <Record>
          <PropertyValue Property=""Id"" Int=""99"" />
          <PropertyValue Property=""String"">
            <Null />
          </PropertyValue>
        </Record>
    </Annotation>
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Empty(actualErrors);

        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateVocabularyAnnotationPropertyWithNullExpression(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StructuredValue"" Type=""My.NS1.Address"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""String"" Type=""Edm.String"" Nullable=""true"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.StructuredValue"">
        <Record>
          <PropertyValue Property=""Id"" >
            <Null />
          </PropertyValue>
          <PropertyValue Property=""String"">
            <Null />
          </PropertyValue>
        </Record>
    </Annotation>
  </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.NullCannotBeAssertedToBeANonNullableType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("(16, 9)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateAnnotationsWithUnresolvedTermModel(EdmVersion edmVersion)
    {
        var csdlElements = new List<string>() { @"
<Schema Namespace=""NorthwindModel""
        Alias=""NorthwindModel""
        xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Annotations Target=""NorthwindModel.Customers"">
    <Annotation Term=""Catalog.Title"" String=""Customers"" />
  </Annotations>
</Schema>", @"
<Schema Namespace=""NorthwindModel""
        Alias=""NorthwindModel""
        xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
   <Annotations Target=""NorthwindModel.Customers"">
    <Annotation Term=""Catalog.Title"" String=""Customers"" />
  </Annotations>
</Schema>" }.Select(n => XElement.Parse(n));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        model.Validate(out IEnumerable<EdmError> edmErrors);
        Assert.DoesNotContain(edmErrors, n => n.ErrorMessage.Contains("Catalog"));
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateSimpleVocabularyAnnotationWithComplexTypeModel(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var address = new EdmComplexType("ßÆœÇèÒöæ", "नुसौस्वागूूम");
        address.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
        model.AddElement(address);

        var person = new EdmComplexType("ßÆœÇèÒöæ", "Person");
        person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddStructuralProperty("Address", new EdmComplexTypeReference(address, true));
        person.AddStructuralProperty("öøãçšŰŽ", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
        model.AddElement(person);

        var personInfoTerm = new EdmTerm("ßÆœÇèÒöæ", "PersonInfo", new EdmComplexTypeReference(person, true));
        model.AddElement(personInfoTerm);

        var addressRecord = new EdmRecordExpression(new EdmPropertyConstructor("Name", new EdmStringConstant("Microsoft Way")));

        var friendNamesRecord = new EdmCollectionExpression(new EdmStringConstant("伯唯堯帯作停捜桜噂構申表ｱｲｳ￥￥"), new EdmStringConstant("bar"));

        var valueAnnotationRecord = new EdmRecordExpression(
            new EdmPropertyConstructor("Id", new EdmIntegerConstant(7)),
            new EdmPropertyConstructor("Address", addressRecord),
            new EdmPropertyConstructor("öøãçšŰŽ", friendNamesRecord));

        var valueAnnotation = new EdmVocabularyAnnotation(
            personInfoTerm,
            personInfoTerm,
            valueAnnotationRecord);

        valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        model.AddVocabularyAnnotation(valueAnnotation);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Empty(actualErrors);

        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateAnnotationComplexTypeWithMissingAndExtraPropertiesModel(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""PersonInfo"" Type=""NS.Person"" />
    <Term Name=""CollectionOfPersonInfo"" Type=""Collection(NS.Person)"" />
    <ComplexType Name=""Person"">
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""true"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
    <Annotations Target=""NS.PersonInfo"">
        <Annotation Term=""NS.PersonInfo"">
            <Record>
                <PropertyValue Property=""Id"" Int=""1"" />
                <PropertyValue Property=""ExtraProperty"" String=""123"" />
            </Record>
        </Annotation>
        <Annotation Term=""NS.CollectionOfPersonInfo"">
            <Collection>
                <Record>
                    <PropertyValue Property=""Id"" Int=""1"" />
                </Record>
                <Record>
                    <PropertyValue Property=""ExtraProperty"" String=""123"" />
                </Record>
            </Collection>
        </Annotation>
    </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(2, actualErrors.Count());

        Assert.Equal(EdmErrorCode.RecordExpressionHasExtraProperties, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal(EdmErrorCode.RecordExpressionHasExtraProperties, actualErrors.ElementAt(1).ErrorCode);
        Assert.Equal("(11, 14)", actualErrors.ElementAt(0).ErrorLocation.ToString());
        Assert.Equal("(21, 18)", actualErrors.ElementAt(1).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateAnnotationEntityTypeWithMissingAndExtraPropertiesModel(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""PersonInfo"" Type=""NS.Person"" />
    <Term Name=""CollectionOfPersonInfo"" Type=""Collection(NS.Person)"" />
    <Term Name=""OpenPersonInfo"" Type=""NS.OpenPerson"" />
    <Term Name=""CollectionOfOpenPersonInfo"" Type=""Collection(NS.OpenPerson)"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
    </EntityType>
    <EntityType Name=""OpenPerson"" OpenType=""true"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
    </EntityType>
    <Annotations Target=""NS.PersonInfo"">
        <Annotation Term=""NS.PersonInfo"">
            <Record>
                <PropertyValue Property=""Id"" Int=""1"" />
                <PropertyValue Property=""ExtraProperty"" String=""123"" />
            </Record>
        </Annotation>
        <Annotation Term=""NS.CollectionOfPersonInfo"">
            <Collection>
                <Record>
                    <PropertyValue Property=""Id"" Int=""1"" />
                </Record>
                <Record>
                    <PropertyValue Property=""Id"" Int=""1"" />
                    <PropertyValue Property=""ExtraProperty"" String=""123"" />
                </Record>
            </Collection>
        </Annotation>
    </Annotations>
    <Annotations Target=""NS.OpenPersonInfo"">
        <Annotation Term=""NS.OpenPersonInfo"">
            <Record>
                <PropertyValue Property=""Id"" Int=""1"" />
                <PropertyValue Property=""ExtraProperty"" String=""123"" />
            </Record>
        </Annotation>
        <Annotation Term=""NS.CollectionOfOpenPersonInfo"">
            <Collection>
                <Record>
                    <PropertyValue Property=""Id"" Int=""1"" />
                </Record>
                <Record>
                    <PropertyValue Property=""Id"" Int=""1"" />
                    <PropertyValue Property=""ExtraProperty"" String=""123"" />
                </Record>
            </Collection>
        </Annotation>
    </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(2, actualErrors.Count());

        Assert.Equal(EdmErrorCode.RecordExpressionHasExtraProperties, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal(EdmErrorCode.RecordExpressionHasExtraProperties, actualErrors.ElementAt(1).ErrorCode);
        Assert.Equal("(23, 14)", actualErrors.ElementAt(0).ErrorLocation.ToString());
        Assert.Equal("(33, 18)", actualErrors.ElementAt(1).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateVocabularyAnnotationComplexTypeWithNullValuesModel(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var address = new EdmComplexType("NS", "Address");
        address.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
        model.AddElement(address);

        var person = new EdmComplexType("NS", "Person");
        person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(true));
        person.AddStructuralProperty("Address", new EdmComplexTypeReference(address, true));
        person.AddStructuralProperty("FriendNames", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
        model.AddElement(person);

        var personInfoTerm = new EdmTerm("NS", "PersonInfo", new EdmComplexTypeReference(person, true));
        model.AddElement(personInfoTerm);

        var friendNamesRecord = new EdmCollectionExpression(EdmNullExpression.Instance);

        var valueAnnotationRecord = new EdmRecordExpression(
            new EdmPropertyConstructor("Id", EdmNullExpression.Instance),
            new EdmPropertyConstructor("Address", EdmNullExpression.Instance),
            new EdmPropertyConstructor("FriendNames", friendNamesRecord));

        var valueAnnotation = new EdmVocabularyAnnotation(
            personInfoTerm,
            personInfoTerm,
            valueAnnotationRecord);

        valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        model.AddVocabularyAnnotation(valueAnnotation);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Empty(actualErrors);

        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateVocabularyAnnotationWithCollectionComplexTypeModel(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmComplexType("NS", "Person");
        var name = person.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
        model.AddElement(person);

        var friendNames = new EdmTerm("NS", "FriendNames", EdmCoreModel.GetCollection(new EdmComplexTypeReference(person, true)));
        model.AddElement(friendNames);

        var billGatesRecord = new EdmRecordExpression(new EdmPropertyConstructor("Name", new EdmStringConstant("Bill Gates")));
        var steveBRecord = new EdmRecordExpression(new EdmPropertyConstructor("Name", new EdmStringConstant("Steve B")));
        var annotationValue = new EdmCollectionExpression(billGatesRecord, steveBRecord);

        var valueAnnotation = new EdmVocabularyAnnotation(
            person,
            friendNames,
            annotationValue);
        model.AddVocabularyAnnotation(valueAnnotation);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Empty(actualErrors);

        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateVocabularyAnnotationNonNullablePropertyWithNullValueCsdl(EdmVersion edmVersion)
    {
        var expectedErrors = new EdmLibTestErrors()
        {
            { null, null, EdmErrorCode.NullCannotBeAssertedToBeANonNullableType }
        };

        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""FriendNames"" Type=""NS.Person"" />
    <ComplexType Name=""Person"">
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    </ComplexType>  
    <Annotations Target=""NS.Person"">
        <Annotation Term=""NS.FriendNames"">
            <Record>
                <PropertyValue Property=""Name"">
                    <Null />
                </PropertyValue>
            </Record>
        </Annotation>
    </Annotations>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.NullCannotBeAssertedToBeANonNullableType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("(11, 14)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ValidateEntitySetCanBeContainedByMultipleNavigationProperties(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Content"">
      <Key>
          <PropertyRef Name=""ID"" />
      </Key>
      <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
      <NavigationProperty Name=""Fields"" Type=""Collection(NS1.Field)"" ContainsTarget=""true"" />
    </EntityType>
    <EntityType Name=""List"">
      <Key>
          <PropertyRef Name=""ID"" />
      </Key>
     <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
      <NavigationProperty Name=""Fields"" Type=""Collection(NS1.Field)"" ContainsTarget=""true"" />
    </EntityType>
    <EntityType Name=""Field"">
      <Key>
          <PropertyRef Name=""ID"" />
      </Key>
      <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
      <NavigationProperty Name=""Description"" Type=""NS1.List"" Nullable=""false"" />
    </EntityType>
    <EntityContainer Name=""C1"">
      <EntitySet Name=""Contents"" EntityType=""NS1.Content"">
        <NavigationPropertyBinding Path=""Fields"" Target=""ContentFields""/>
      </EntitySet>
      <EntitySet Name=""Lists"" EntityType=""NS1.List"">
        <NavigationPropertyBinding Path=""Fields"" Target=""ListFields""/>
      </EntitySet>
      <EntitySet Name=""ContentFields"" EntityType=""NS1.Field"" />
      <EntitySet Name=""ListFields"" EntityType=""NS1.Field"" />
      <EntitySet Name=""Descriptions"" EntityType=""NS1.Field"" />
    </EntityContainer>
</Schema>";

        IEnumerable<XElement> csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);

        var serializedCsdls = GetSerializerResult(edmModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        var validationResult = edmModel.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Empty(actualErrors);

        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    private static void CreateVocabularyDefinitions(StubEdmModel model)
    {
        var reviewTypeTerm = new StubTypeTerm("NS1", "hReview")
            {
                new StubEdmStructuralProperty("summary") { Type = EdmCoreModel.Instance.GetString(false) },
                new StubEdmStructuralProperty("itemName") { Type = EdmCoreModel.Instance.GetString(false) },
                new StubEdmStructuralProperty("dateReviewed") { Type = EdmCoreModel.Instance.GetDateTimeOffset(false) },
                new StubEdmStructuralProperty("rating") { Type = EdmCoreModel.Instance.GetInt32(false) },
            };
        model.Add(reviewTypeTerm);

        var reviewEntityType = new StubEdmEntityType("NS1", "hReviewEntity")
            {
                new StubEdmStructuralProperty("summary") { Type = EdmCoreModel.Instance.GetString(false) },
                new StubEdmStructuralProperty("itemName") { Type = EdmCoreModel.Instance.GetString(false) },
                new StubEdmStructuralProperty("dateReviewed") { Type = EdmCoreModel.Instance.GetDateTimeOffset(false) },
                new StubEdmStructuralProperty("rating") { Type = EdmCoreModel.Instance.GetInt32(false) },
            };
        model.Add(reviewEntityType);

        var titleValueTerm = new StubTerm("NS1", "Title") { Type = EdmCoreModel.Instance.GetString(true) };
        model.Add(titleValueTerm);

        var displaySizeValueTerm = new StubTerm("NS1", "DisplaySize") { Type = EdmCoreModel.Instance.GetInt32(false) };
        model.Add(displaySizeValueTerm);
    }
}
