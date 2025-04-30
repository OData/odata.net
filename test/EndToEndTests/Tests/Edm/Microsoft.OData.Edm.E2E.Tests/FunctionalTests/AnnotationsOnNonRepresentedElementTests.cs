//---------------------------------------------------------------------
// <copyright file="AnnotationsOnNonRepresentedElementTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
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

    [Fact]
    public void Should_SupportKeyAnnotationElement_InEdmV40()
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

        var expectedErrors = new EdmLibTestErrors();

        // Act
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

    [Fact]
    public void Should_SupportEntityContainerAnnotationElement_InEdmV40()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

    [Fact]
    public void Should_FailValidation_When_AnnotationElementFullNameIsNotUnique()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

    [Fact]
    public void Should_SupportPropertyRefAnnotationElement_InEdmV40()
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

        var expectedErrors = new EdmLibTestErrors();

        // Act
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

    public static TheoryData<string, EdmLibTestErrors> GetAnnotationElementTestData()
    {
        return new TheoryData<string, EdmLibTestErrors>
        {
            {
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
</Schema>",
                new EdmLibTestErrors()
            },
            {
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
</Schema>",
                new EdmLibTestErrors { { 5, 6, EdmErrorCode.DuplicateDirectValueAnnotationFullName } }
            }
        };
    }

    [Theory]
    [MemberData(nameof(GetAnnotationElementTestData))]
    public void Should_ValidateAnnotationElementRules_InEdmV40(string csdl, EdmLibTestErrors expectedErrors)
    {
        // Arrange
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);
        SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

    [Fact]
    public void Should_SupportFunctionImportAnnotationElement_InEdmV40()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

}
