//---------------------------------------------------------------------
// <copyright file="AnnotationsOnNonRepresentedElementTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class AnnotationsOnNonRepresentedElementTests : EdmLibTestCaseBase
{
    [Fact]
    public void Should_FailValidation_When_AnnotationAttributesCollide()
    {
        // Arrange
        const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" Alias=""Fuzz"" xmlns:Bogus=""http://bogus.com/schema"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"" Bogus:PropertyAnnotationAttribute=""Just kidding"" Bogus:PropertyAnnotationAttribute=""Just kidding"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Bogus:EntityTypeAnnotation Stuff=""Whatever"" />
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
</Schema>";

        IEdmModel model;
        IEnumerable<EdmError> errors;

        // Act
        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

        // Assert
        Assert.False(parsed);
        Assert.True(errors.Any());
        Assert.Equal("'Bogus:PropertyAnnotationAttribute' is a duplicate attribute name. Line 3, position 78.", errors.First().ErrorMessage);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Should_SupportKeyAnnotationElement_InEdmV40AndV401(EdmVersion edmVersion)
    {
        // Arrange
        const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" Alias=""Fuzz"" xmlns:Bogus=""http://bogus.com/schema"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
      <Bogus:SchemaAnnotation Stuff=""Stuffed"" />
      <Bogus:AnotherSchemaAnnotation Fluff=""Fluffy"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
</Schema>";

        // Act
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, edmVersion), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = testModel.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Empty(actualErrors);

        var serializedCsdls = GetSerializerResult(testModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        IEdmModel? roundtrippedModel = null;
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError>? validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Should_SupportEntityContainerAnnotationElement_InEdmV40AndV401(EdmVersion edmVersion)
    {
        // Arrange
        const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns:Bogus=""http://bogus.com/schema"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityContainer Name=""EC1"">
    <Bogus:SchemaAnnotation Stuff=""Stuffed1"" />
    <Bogus:SchemaAnnotation Stuff=""Stuffed2"" />
    <EntitySet Name=""Set1"" EntityType=""Grumble.Clod"" />
  </EntityContainer>
  <EntityType Name=""Clod"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""Bar"" Type=""Int32"" />
  </EntityType>
</Schema>";

        var expectedErrors = new EdmLibTestErrors()
        {
            { 5, 6, EdmErrorCode.DuplicateDirectValueAnnotationFullName},
        };

        // Act
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, edmVersion), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = testModel.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.True(expectedErrors.Count == actualErrors.Count());
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.DuplicateDirectValueAnnotationFullName, actualErrors.Last().ErrorCode);
        Assert.Equal("An element already has a direct annotation with the namespace 'http://bogus.com/schema' and name 'SchemaAnnotation'.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(5, 6)", actualErrors.Last().ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(testModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Should_FailValidation_When_AnnotationElementFullNameIsNotUnique_InEdmV40AndV401(EdmVersion edmVersion)
    {
        // Arrange
        const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns:Bogus=""http://bogus.com/schema"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""Function1""><ReturnType Type=""Int32"" /></Function>
  <EntityContainer Name=""Container"">
    <FunctionImport Name=""FunctionImport1"" Function=""Bork.Function1"">
       <Bogus:SchemaAnnotation Stuff=""Stuffed"" />
       <Bogus:SchemaAnnotation Stuff=""Fluffy"" />
       <Bogus:AnnotationScheme Stuff=""Fluffy"" />
    </FunctionImport>
  </EntityContainer>
</Schema>";

        var expectedErrors = new EdmLibTestErrors()
        {
            { null, null, EdmErrorCode.DuplicateDirectValueAnnotationFullName}
        };

        // Act
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, edmVersion), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = testModel.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.True(expectedErrors.Count == actualErrors.Count());
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.DuplicateDirectValueAnnotationFullName, actualErrors.Last().ErrorCode);
        Assert.Equal("An element already has a direct annotation with the namespace 'http://bogus.com/schema' and name 'SchemaAnnotation'.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(7, 9)", actualErrors.Last().ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(testModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Should_SupportPropertyRefAnnotationElement_InEdmV40AndV401(EdmVersion edmVersion)
    {
        // Arrange
        const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" Alias=""Fuzz"" xmlns:Bogus=""http://bogus.com/schema"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"">
        <Bogus:SchemaAnnotation Stuff=""Stuffed"" />
        <Bogus:AnotherSchemaAnnotation Fluff=""Fluffy"" />
      </PropertyRef>
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
</Schema>";

        // Act
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, edmVersion), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = testModel.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Empty(actualErrors);

        var serializedCsdls = GetSerializerResult(testModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        IEdmModel? roundtrippedModel = null;
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError>? validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Should_ValidateAnnotationElementRules1_InEdmV40AndV401(EdmVersion edmVersion)
    {
        // Arrange
        var csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns:Bogus=""http://bogus.com/schema"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""Function1""><ReturnType Type=""Int32""/>
        <Bogus:SchemaAnnotation Stuff=""Stuffed"" />
        <Bogus:AnotherSchemaAnnotation Fluff=""Fluffy"" />
    </Function>
    <EntityContainer Name=""Container"">
    <FunctionImport Name=""FunctionImport1"" Function=""Bork.Function1"">
        <Bogus:SchemaAnnotation Stuff=""Stuffed"" />
        <Bogus:AnotherSchemaAnnotation Fluff=""Fluffy"" />
    </FunctionImport>
    </EntityContainer>
</Schema>";

        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V401), LoadOptions.SetLineInfo);
        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        // Assert
        var validationResult = testModel.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Empty(actualErrors);

        var serializedCsdls = GetSerializerResult(testModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        IEdmModel? roundtrippedModel = null;
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError>? validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Should_ValidateAnnotationElementRules2_InEdmV40AndV401(EdmVersion edmVersion)
    {
        // Arrange
        var csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns:Bogus=""http://bogus.com/schema"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""EC1"">
    <Bogus:SchemaAnnotation Stuff=""Stuffed1"" />
    <Bogus:SchemaAnnotation Stuff=""Stuffed2"" />
    <EntitySet Name=""Set1"" EntityType=""Grumble.Clod"" />
    </EntityContainer>
    <EntityType Name=""Clod"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""Bar"" Type=""Int32"" />
    </EntityType>
</Schema>";

        var expectedErrors = new EdmLibTestErrors { { 5, 6, EdmErrorCode.DuplicateDirectValueAnnotationFullName } };

        // Act
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V401), LoadOptions.SetLineInfo);
        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = testModel.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.True(expectedErrors.Count == actualErrors.Count());
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.DuplicateDirectValueAnnotationFullName, actualErrors.Last().ErrorCode);
        Assert.Equal("An element already has a direct annotation with the namespace 'http://bogus.com/schema' and name 'SchemaAnnotation'.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(5, 6)", actualErrors.Last().ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(testModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Should_SupportFunctionImportAnnotationElement_InEdmV40AndV401(EdmVersion edmVersion)
    {
        // Arrange
        const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns:Bogus=""http://bogus.com/schema"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""Function1""><ReturnType Type=""Int32""/>
       <Bogus:SchemaAnnotation Stuff=""Stuffed"" />
       <Bogus:AnotherSchemaAnnotation Fluff=""Fluffy"" />
    </Function>
  <EntityContainer Name=""Container"">
    <FunctionImport Name=""FunctionImport1"" Function=""Bork.Function1"">
       <Bogus:SchemaAnnotation Stuff=""Stuffed"" />
       <Bogus:AnotherSchemaAnnotation Fluff=""Fluffy"" />
    </FunctionImport>
  </EntityContainer>
</Schema>";

        var expectedErrors = new EdmLibTestErrors() { };

        // Act
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, edmVersion), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = testModel.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.True(expectedErrors.Count == actualErrors.Count());
        Assert.Empty(actualErrors);

        var serializedCsdls = GetSerializerResult(testModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError>? validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }
}
