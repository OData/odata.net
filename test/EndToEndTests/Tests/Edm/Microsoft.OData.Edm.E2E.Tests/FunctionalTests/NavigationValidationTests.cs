//---------------------------------------------------------------------
// <copyright file="NavigationValidationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class NavigationValidationTests : EdmLibTestCaseBase
{
    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithSinglePrincipalAndNonNullableDependentProperties_PassesValidation(EdmVersion edmVersion)
    {
var csdlElements = new[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithSinglePrincipalAndNonNullableKeyDependentProperties_PassesValidation(EdmVersion edmVersion)
    {
        var csdlElements = new[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""PersonId"" />
            <PropertyRef Name=""PersonName"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithSinglePrincipalAndMixedNullableDependentProperties_PassesValidation(EdmVersion edmVersion)
    {
        var csdlElements = new[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""true"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithSinglePrincipalAndAllNullableDependentProperties_FailsValidation(EdmVersion edmVersion)
    {
        var csdlElements = new[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""true"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""true"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        // Assert that the error code is as expected
        Assert.Equal(EdmErrorCode.InvalidMultiplicityOfPrincipalEnd, actualErrors.First().ErrorCode);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithMultiplePrincipalsAndNonNullableDependentProperties_FailsValidation(EdmVersion edmVersion)
    {
        var csdlElements = new[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""Collection(DefaultNamespace.Person)"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        // Assert that the error code is as expected
        Assert.Equal(EdmErrorCode.InvalidMultiplicityOfPrincipalEnd, actualErrors.First().ErrorCode);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithMultiplePrincipalsAndMixedNullableDependentProperties_FailsValidation(EdmVersion edmVersion)
    {
        var csdlElements = new[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""true"" />
        <NavigationProperty Name=""ToPerson"" Type=""Collection(DefaultNamespace.Person)"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        // Assert that the error code is as expected
        Assert.Equal(EdmErrorCode.InvalidMultiplicityOfPrincipalEnd, actualErrors.First().ErrorCode);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithMultiplePrincipalsAndAllNullableDependentProperties_FailsValidation(EdmVersion edmVersion)
    {
        var csdlElements = new[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""true"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""true"" />
        <NavigationProperty Name=""ToPerson"" Type=""Collection(DefaultNamespace.Person)"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        // Assert that the error code is as expected
        Assert.Equal(EdmErrorCode.InvalidMultiplicityOfPrincipalEnd, actualErrors.First().ErrorCode);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithZeroOrOnePrincipalAndNonNullableDependentProperties_FailsValidation(EdmVersion edmVersion)
    {
        var csdlElements = new[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        // Assert that the error code is as expected
        Assert.Equal(EdmErrorCode.InvalidMultiplicityOfPrincipalEnd, actualErrors.First().ErrorCode);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithZeroOrOnePrincipalAndNonNullableKeyDependentProperties_FailsValidation(EdmVersion edmVersion)
    {
        var csdlElements = new[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""PersonId"" />
            <PropertyRef Name=""PersonName"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        // Assert that the error code is as expected
        Assert.Equal(EdmErrorCode.InvalidMultiplicityOfPrincipalEnd, actualErrors.First().ErrorCode);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithZeroOrOnePrincipalAndMixedNullableDependentProperties_PassesValidation(EdmVersion edmVersion)
    {
        var csdlElements = new[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""true"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithZeroOrOnePrincipalAndAllNullableDependentProperties_PassesValidation(EdmVersion edmVersion)
    {
        var csdlElements = new[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""true"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""true"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithZeroOrOnePrincipalAndAllNullableKeyDependentProperties_FailsValidation(EdmVersion edmVersion)
    {
        var csdlElements = new[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""PersonId"" />
            <PropertyRef Name=""PersonName"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""true"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""true"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(2, actualErrors.Count());

        // Assert that the error code is as expected
        Assert.All(actualErrors, error => Assert.Equal(EdmErrorCode.InvalidKey, error.ErrorCode));
        Assert.Equal("(19, 10)", actualErrors.First().ErrorLocation.ToString());
        Assert.Equal("(20, 10)", actualErrors.Last().ErrorLocation.ToString());

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithSinglePrincipalAndNonNullableDependentProperties_InModel_PassesValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var badgeEntity = new EdmEntityType("DefaultNamespace", "Badge");
        var badgeId = badgeEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonId = badgeEntity.AddStructuralProperty("PersonId", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonName = badgeEntity.AddStructuralProperty("PersonName", EdmCoreModel.Instance.GetString(false));

        var badgeKey = new List<IEdmStructuralProperty> { badgeId };
        badgeEntity.AddKeys(badgeKey.ToArray());

        var personEntity = new EdmEntityType("DefaultNamespace", "Person");
        var personId = personEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var personName = personEntity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        personEntity.AddKeys(personId, personName);

        model.AddElement(badgeEntity);
        model.AddElement(personEntity);

        EdmNavigationProperty toPerson = ((EdmEntityType)badgeEntity).AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo()
            {
                Name = "ToPerson",
                Target = (EdmEntityType)personEntity,
                TargetMultiplicity = EdmMultiplicity.One,
                DependentProperties = new[]
                {
                        badgeEntity.FindProperty("PersonId") as IEdmStructuralProperty,
                        badgeEntity.FindProperty("PersonName") as IEdmStructuralProperty
                },
                PrincipalProperties = personEntity.Key()
            },
            new EdmNavigationPropertyInfo()
            {
                Name = "ToBadge",
                TargetMultiplicity = EdmMultiplicity.Many
            });

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithSinglePrincipalAndNonNullableKeyDependentProperties_InModel_PassesValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var badgeEntity = new EdmEntityType("DefaultNamespace", "Badge");
        var badgeId = badgeEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonId = badgeEntity.AddStructuralProperty("PersonId", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonName = badgeEntity.AddStructuralProperty("PersonName", EdmCoreModel.Instance.GetString(false));

        var badgeKey = new List<IEdmStructuralProperty>
        {
            badgeId,
            badgePersonId,
            badgePersonName
        };
        badgeEntity.AddKeys(badgeKey.ToArray());

        var personEntity = new EdmEntityType("DefaultNamespace", "Person");
        var personId = personEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var personName = personEntity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        personEntity.AddKeys(personId, personName);

        model.AddElement(badgeEntity);
        model.AddElement(personEntity);

        EdmNavigationProperty toPerson = ((EdmEntityType)badgeEntity).AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo()
            {
                Name = "ToPerson",
                Target = (EdmEntityType)personEntity,
                TargetMultiplicity = EdmMultiplicity.One,
                DependentProperties = new[]
                {
                        badgeEntity.FindProperty("PersonId") as IEdmStructuralProperty,
                        badgeEntity.FindProperty("PersonName") as IEdmStructuralProperty
                },
                PrincipalProperties = personEntity.Key()
            },
            new EdmNavigationPropertyInfo()
            {
                Name = "ToBadge",
                TargetMultiplicity = EdmMultiplicity.Many
            });

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithSinglePrincipalAndMixedNullableDependentProperties_InModel_PassesValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var badgeEntity = new EdmEntityType("DefaultNamespace", "Badge");
        var badgeId = badgeEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonId = badgeEntity.AddStructuralProperty("PersonId", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonName = badgeEntity.AddStructuralProperty("PersonName", EdmCoreModel.Instance.GetString(true));

        var badgeKey = new List<IEdmStructuralProperty> { badgeId };
        badgeEntity.AddKeys(badgeKey.ToArray());

        var personEntity = new EdmEntityType("DefaultNamespace", "Person");
        var personId = personEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var personName = personEntity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        personEntity.AddKeys(personId, personName);

        model.AddElement(badgeEntity);
        model.AddElement(personEntity);

        EdmNavigationProperty toPerson = ((EdmEntityType)badgeEntity).AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo()
            {
                Name = "ToPerson",
                Target = (EdmEntityType)personEntity,
                TargetMultiplicity = EdmMultiplicity.One,
                DependentProperties = new[]
                {
                        badgeEntity.FindProperty("PersonId") as IEdmStructuralProperty,
                        badgeEntity.FindProperty("PersonName") as IEdmStructuralProperty
                },
                PrincipalProperties = personEntity.Key()
            },
            new EdmNavigationPropertyInfo()
            {
                Name = "ToBadge",
                TargetMultiplicity = EdmMultiplicity.Many
            });

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithSinglePrincipalAndAllNullableDependentProperties_InModel_FailsValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var badgeEntity = new EdmEntityType("DefaultNamespace", "Badge");
        var badgeId = badgeEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonId = badgeEntity.AddStructuralProperty("PersonId", EdmCoreModel.Instance.GetInt32(true));
        var badgePersonName = badgeEntity.AddStructuralProperty("PersonName", EdmCoreModel.Instance.GetString(true));

        var badgeKey = new List<IEdmStructuralProperty> { badgeId };
        badgeEntity.AddKeys(badgeKey.ToArray());

        var personEntity = new EdmEntityType("DefaultNamespace", "Person");
        var personId = personEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var personName = personEntity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        personEntity.AddKeys(personId, personName);

        model.AddElement(badgeEntity);
        model.AddElement(personEntity);

        EdmNavigationProperty toPerson = ((EdmEntityType)badgeEntity).AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo()
            {
                Name = "ToPerson",
                Target = (EdmEntityType)personEntity,
                TargetMultiplicity = EdmMultiplicity.One,
                DependentProperties = new[]
                {
                        badgeEntity.FindProperty("PersonId") as IEdmStructuralProperty,
                        badgeEntity.FindProperty("PersonName") as IEdmStructuralProperty
                },
                PrincipalProperties = personEntity.Key()
            },
            new EdmNavigationPropertyInfo()
            {
                Name = "ToBadge",
                TargetMultiplicity = EdmMultiplicity.Many
            });

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.InvalidMultiplicityOfPrincipalEnd, actualErrors.First().ErrorCode);
        Assert.Equal("Because all dependent properties of the navigation 'ToPerson' are nullable, the multiplicity of the principal end must be '0..1'.", actualErrors.First().ErrorMessage);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithMultiplePrincipalsAndNonNullableDependentProperties_InModel_FailsValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var badgeEntity = new EdmEntityType("DefaultNamespace", "Badge");
        var badgeId = badgeEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonId = badgeEntity.AddStructuralProperty("PersonId", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonName = badgeEntity.AddStructuralProperty("PersonName", EdmCoreModel.Instance.GetString(false));

        var badgeKey = new List<IEdmStructuralProperty> { badgeId };
        badgeEntity.AddKeys(badgeKey.ToArray());

        var personEntity = new EdmEntityType("DefaultNamespace", "Person");
        var personId = personEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var personName = personEntity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        personEntity.AddKeys(personId, personName);

        model.AddElement(badgeEntity);
        model.AddElement(personEntity);

        EdmNavigationProperty toPerson = ((EdmEntityType)badgeEntity).AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo()
            {
                Name = "ToPerson",
                Target = (EdmEntityType)personEntity,
                TargetMultiplicity = EdmMultiplicity.Many,
                DependentProperties = new[]
                {
                        badgeEntity.FindProperty("PersonId") as IEdmStructuralProperty,
                        badgeEntity.FindProperty("PersonName") as IEdmStructuralProperty
                },
                PrincipalProperties = personEntity.Key()
            },
            new EdmNavigationPropertyInfo()
            {
                Name = "ToBadge",
                TargetMultiplicity = EdmMultiplicity.Many
            });

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.InvalidMultiplicityOfPrincipalEnd, actualErrors.First().ErrorCode);
        Assert.Equal("Because all dependent properties of the navigation 'ToPerson' are non-nullable, the multiplicity of the principal end must be '1'.", actualErrors.First().ErrorMessage);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithMultiplePrincipalsAndMixedNullableDependentProperties_InModel_FailsValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var badgeEntity = new EdmEntityType("DefaultNamespace", "Badge");
        var badgeId = badgeEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonId = badgeEntity.AddStructuralProperty("PersonId", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonName = badgeEntity.AddStructuralProperty("PersonName", EdmCoreModel.Instance.GetString(true));

        var badgeKey = new List<IEdmStructuralProperty> { badgeId };
        badgeEntity.AddKeys(badgeKey.ToArray());

        var personEntity = new EdmEntityType("DefaultNamespace", "Person");
        var personId = personEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var personName = personEntity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        personEntity.AddKeys(personId, personName);

        model.AddElement(badgeEntity);
        model.AddElement(personEntity);

        EdmNavigationProperty toPerson = ((EdmEntityType)badgeEntity).AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo()
            {
                Name = "ToPerson",
                Target = (EdmEntityType)personEntity,
                TargetMultiplicity = EdmMultiplicity.Many,
                DependentProperties = new[]
                {
                        badgeEntity.FindProperty("PersonId") as IEdmStructuralProperty,
                        badgeEntity.FindProperty("PersonName") as IEdmStructuralProperty
                },
                PrincipalProperties = personEntity.Key()
            },
            new EdmNavigationPropertyInfo()
            {
                Name = "ToBadge",
                TargetMultiplicity = EdmMultiplicity.Many
            });

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.InvalidMultiplicityOfPrincipalEnd, actualErrors.First().ErrorCode);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithMultiplePrincipalsAndAllNullableDependentProperties_InModel_FailsValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var badgeEntity = new EdmEntityType("DefaultNamespace", "Badge");
        var badgeId = badgeEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonId = badgeEntity.AddStructuralProperty("PersonId", EdmCoreModel.Instance.GetInt32(true));
        var badgePersonName = badgeEntity.AddStructuralProperty("PersonName", EdmCoreModel.Instance.GetString(true));

        var badgeKey = new List<IEdmStructuralProperty> { badgeId };
        badgeEntity.AddKeys(badgeKey.ToArray());

        var personEntity = new EdmEntityType("DefaultNamespace", "Person");
        var personId = personEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var personName = personEntity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        personEntity.AddKeys(personId, personName);

        model.AddElement(badgeEntity);
        model.AddElement(personEntity);

        EdmNavigationProperty toPerson = ((EdmEntityType)badgeEntity).AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo()
            {
                Name = "ToPerson",
                Target = (EdmEntityType)personEntity,
                TargetMultiplicity = EdmMultiplicity.Many,
                DependentProperties = new[]
                {
                        badgeEntity.FindProperty("PersonId") as IEdmStructuralProperty,
                        badgeEntity.FindProperty("PersonName") as IEdmStructuralProperty
                },
                PrincipalProperties = personEntity.Key()
            },
            new EdmNavigationPropertyInfo()
            {
                Name = "ToBadge",
                TargetMultiplicity = EdmMultiplicity.Many
            });

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.InvalidMultiplicityOfPrincipalEnd, actualErrors.First().ErrorCode);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithZeroOrOnePrincipalAndNonNullableDependentProperties_InModel_FailsValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var badgeEntity = new EdmEntityType("DefaultNamespace", "Badge");
        var badgeId = badgeEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonId = badgeEntity.AddStructuralProperty("PersonId", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonName = badgeEntity.AddStructuralProperty("PersonName", EdmCoreModel.Instance.GetString(false));

        var badgeKey = new List<IEdmStructuralProperty> { badgeId };
        badgeEntity.AddKeys(badgeKey.ToArray());

        var personEntity = new EdmEntityType("DefaultNamespace", "Person");
        var personId = personEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var personName = personEntity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        personEntity.AddKeys(personId, personName);

        model.AddElement(badgeEntity);
        model.AddElement(personEntity);

        EdmNavigationProperty toPerson = ((EdmEntityType)badgeEntity).AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo()
            {
                Name = "ToPerson",
                Target = (EdmEntityType)personEntity,
                TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                DependentProperties = new[]
                {
                        badgeEntity.FindProperty("PersonId") as IEdmStructuralProperty,
                        badgeEntity.FindProperty("PersonName") as IEdmStructuralProperty
                },
                PrincipalProperties = personEntity.Key()
            },
            new EdmNavigationPropertyInfo()
            {
                Name = "ToBadge",
                TargetMultiplicity = EdmMultiplicity.Many
            });

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.InvalidMultiplicityOfPrincipalEnd, actualErrors.First().ErrorCode);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithZeroOrOnePrincipalAndNonNullableKeyDependentProperties_InModel_FailsValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var badgeEntity = new EdmEntityType("DefaultNamespace", "Badge");
        var badgeId = badgeEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonId = badgeEntity.AddStructuralProperty("PersonId", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonName = badgeEntity.AddStructuralProperty("PersonName", EdmCoreModel.Instance.GetString(false));

        var badgeKey = new List<IEdmStructuralProperty> { badgeId, badgePersonId, badgePersonName };
        badgeEntity.AddKeys(badgeKey.ToArray());

        var personEntity = new EdmEntityType("DefaultNamespace", "Person");
        var personId = personEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var personName = personEntity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        personEntity.AddKeys(personId, personName);

        model.AddElement(badgeEntity);
        model.AddElement(personEntity);

        EdmNavigationProperty toPerson = ((EdmEntityType)badgeEntity).AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo()
            {
                Name = "ToPerson",
                Target = (EdmEntityType)personEntity,
                TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                DependentProperties = new[]
                {
                        badgeEntity.FindProperty("PersonId") as IEdmStructuralProperty,
                        badgeEntity.FindProperty("PersonName") as IEdmStructuralProperty
                },
                PrincipalProperties = personEntity.Key()
            },
            new EdmNavigationPropertyInfo()
            {
                Name = "ToBadge",
                TargetMultiplicity = EdmMultiplicity.Many
            });

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.InvalidMultiplicityOfPrincipalEnd, actualErrors.First().ErrorCode);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithZeroOrOnePrincipalAndMixedNullableDependentProperties_InModel_PassesValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var badgeEntity = new EdmEntityType("DefaultNamespace", "Badge");
        var badgeId = badgeEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonId = badgeEntity.AddStructuralProperty("PersonId", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonName = badgeEntity.AddStructuralProperty("PersonName", EdmCoreModel.Instance.GetString(true));

        var badgeKey = new List<IEdmStructuralProperty> { badgeId };
        badgeEntity.AddKeys(badgeKey.ToArray());

        var personEntity = new EdmEntityType("DefaultNamespace", "Person");
        var personId = personEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var personName = personEntity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        personEntity.AddKeys(personId, personName);

        model.AddElement(badgeEntity);
        model.AddElement(personEntity);

        EdmNavigationProperty toPerson = ((EdmEntityType)badgeEntity).AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo()
            {
                Name = "ToPerson",
                Target = (EdmEntityType)personEntity,
                TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                DependentProperties = new[]
                {
                        badgeEntity.FindProperty("PersonId") as IEdmStructuralProperty,
                        badgeEntity.FindProperty("PersonName") as IEdmStructuralProperty
                },
                PrincipalProperties = personEntity.Key()
            },
            new EdmNavigationPropertyInfo()
            {
                Name = "ToBadge",
                TargetMultiplicity = EdmMultiplicity.Many
            });

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithZeroOrOnePrincipalAndAllNullableDependentProperties_InModel_PassesValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var badgeEntity = new EdmEntityType("DefaultNamespace", "Badge");
        var badgeId = badgeEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonId = badgeEntity.AddStructuralProperty("PersonId", EdmCoreModel.Instance.GetInt32(true));
        var badgePersonName = badgeEntity.AddStructuralProperty("PersonName", EdmCoreModel.Instance.GetString(true));

        var badgeKey = new List<IEdmStructuralProperty> { badgeId };
        badgeEntity.AddKeys(badgeKey.ToArray());

        var personEntity = new EdmEntityType("DefaultNamespace", "Person");
        var personId = personEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var personName = personEntity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        personEntity.AddKeys(personId, personName);

        model.AddElement(badgeEntity);
        model.AddElement(personEntity);

        EdmNavigationProperty toPerson = ((EdmEntityType)badgeEntity).AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo()
            {
                Name = "ToPerson",
                Target = (EdmEntityType)personEntity,
                TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                DependentProperties = new[]
                {
                        badgeEntity.FindProperty("PersonId") as IEdmStructuralProperty,
                        badgeEntity.FindProperty("PersonName") as IEdmStructuralProperty
                },
                PrincipalProperties = personEntity.Key()
            },
            new EdmNavigationPropertyInfo()
            {
                Name = "ToBadge",
                TargetMultiplicity = EdmMultiplicity.Many
            });

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithZeroOrOnePrincipalAndAllNullableKeyDependentProperties_InModel_FailsValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var badgeEntity = new EdmEntityType("DefaultNamespace", "Badge");
        var badgeId = badgeEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var badgePersonId = badgeEntity.AddStructuralProperty("PersonId", EdmCoreModel.Instance.GetInt32(true));
        var badgePersonName = badgeEntity.AddStructuralProperty("PersonName", EdmCoreModel.Instance.GetString(true));

        var badgeKey = new List<IEdmStructuralProperty> { badgeId, badgePersonId, badgePersonName };
        badgeEntity.AddKeys(badgeKey.ToArray());

        var personEntity = new EdmEntityType("DefaultNamespace", "Person");
        var personId = personEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        var personName = personEntity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        personEntity.AddKeys(personId, personName);

        model.AddElement(badgeEntity);
        model.AddElement(personEntity);

        EdmNavigationProperty toPerson = ((EdmEntityType)badgeEntity).AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo()
            {
                Name = "ToPerson",
                Target = (EdmEntityType)personEntity,
                TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                DependentProperties = new[]
                {
                        badgeEntity.FindProperty("PersonId") as IEdmStructuralProperty,
                        badgeEntity.FindProperty("PersonName") as IEdmStructuralProperty
                },
                PrincipalProperties = personEntity.Key()
            },
            new EdmNavigationPropertyInfo()
            {
                Name = "ToBadge",
                TargetMultiplicity = EdmMultiplicity.Many
            });

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(2, actualErrors.Count());

        Assert.All(actualErrors, e => Assert.Equal(EdmErrorCode.InvalidKey, e.ErrorCode));

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    public static TheoryData<EdmVersion, IEnumerable<XElement>> ValidateNavigationNonKeyPrincipalPropertyRefCsdlData()
    {
        return new TheoryData<EdmVersion, IEnumerable<XElement>>()
        {
            { EdmVersion.V40, new[] { @"
    <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Person"">
            <Key>
                <PropertyRef Name=""Id"" />
            </Key>
            <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
            <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
        </EntityType>
        <EntityType Name=""Badge"">
            <Key>
                <PropertyRef Name=""PersonId"" />
                <PropertyRef Name=""PersonName"" />
            </Key>
            <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
            <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
              <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
              <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
            </NavigationProperty>
        </EntityType>
    </Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)) },

            { EdmVersion.V401, new[] { @"
    <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Person"">
            <Key>
                <PropertyRef Name=""Id"" />
            </Key>
            <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
            <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
        </EntityType>
        <EntityType Name=""Badge"">
            <Key>
                <PropertyRef Name=""PersonId"" />
                <PropertyRef Name=""PersonName"" />
            </Key>
            <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
            <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
              <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
              <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
            </NavigationProperty>
        </EntityType>
    </Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)) },

            { EdmVersion.V40, new[] { @"
    <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Person"">
            <Key>
                <PropertyRef Name=""Id"" />
            </Key>
            <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""AdditionalId"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
            <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
        </EntityType>
        <EntityType Name=""Badge"">
            <Key>
                <PropertyRef Name=""PersonId"" />
                <PropertyRef Name=""PersonName"" />
            </Key>
            <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
            <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
              <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""AdditionalId"" />
              <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
            </NavigationProperty>
        </EntityType>
    </Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)) },

            { EdmVersion.V401, new[] { @"
    <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Person"">
            <Key>
                <PropertyRef Name=""Id"" />
            </Key>
            <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""AdditionalId"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
            <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
        </EntityType>
        <EntityType Name=""Badge"">
            <Key>
                <PropertyRef Name=""PersonId"" />
                <PropertyRef Name=""PersonName"" />
            </Key>
            <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
            <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
              <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""AdditionalId"" />
              <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
            </NavigationProperty>
        </EntityType>
    </Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)) },

            { EdmVersion.V40, new[] { @"
    <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Person"">
            <Key>
                <PropertyRef Name=""Id"" />
            </Key>
            <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""AdditionalId"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
            <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
        </EntityType>
        <EntityType Name=""Badge"">
            <Key>
                <PropertyRef Name=""PersonName"" />
            </Key>
            <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
            <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
              <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
            </NavigationProperty>
        </EntityType>
    </Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)) },

            { EdmVersion.V401, new[] { @"
    <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Person"">
            <Key>
                <PropertyRef Name=""Id"" />
            </Key>
            <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""AdditionalId"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
            <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
        </EntityType>
        <EntityType Name=""Badge"">
            <Key>
                <PropertyRef Name=""PersonName"" />
            </Key>
            <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
            <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
            <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
              <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
            </NavigationProperty>
        </EntityType>
    </Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)) }
        };
    }

    [Theory]
    [MemberData(nameof(ValidateNavigationNonKeyPrincipalPropertyRefCsdlData))]
    public void Validate_NavigationWithNonKeyPrincipalPropertyReferences_PassesValidation(EdmVersion edmVersion, IEnumerable<XElement> csdlElements)
    {
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithDuplicateDependentPropertyReferences_FailsValidation(EdmVersion edmVersion)
    {
        var csdlElements = new[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id1"" />
            <PropertyRef Name=""Id2"" />
        </Key>
        <Property Name=""Id1"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Id2"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""PersonId"" />
            <PropertyRef Name=""PersonName"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id1"" />
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id2"" />
        </NavigationProperty>
    </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.All(actualErrors, e => Assert.Equal(EdmErrorCode.DuplicateDependentProperty, e.ErrorCode));

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithDuplicatePrincipalPropertyReferences_PassesValidation(EdmVersion edmVersion)
    {
        var csdlElements = new[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""PersonId"" />
            <PropertyRef Name=""PersonName"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId2"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonId2"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithDuplicateReferentialConstraints_FailsValidation(EdmVersion edmVersion)
    {
        var csdlElements = new[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""PersonId"" />
            <PropertyRef Name=""PersonName"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId2"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.All(actualErrors, e => Assert.Equal(EdmErrorCode.DuplicateDependentProperty, e.ErrorCode));

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithMismatchedPrincipalAndDependentPropertyReferences_FailsValidation(EdmVersion edmVersion)
    {
        var csdlElements = new[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""PersonId"" />
            <PropertyRef Name=""PersonName"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Name"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Id"" />
        </NavigationProperty>
    </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(2, actualErrors.Count());

        Assert.All(actualErrors, e => Assert.Equal(EdmErrorCode.TypeMismatchRelationshipConstraint, e.ErrorCode));

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithEmptyName_FailsValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var entityType = new EdmEntityType("NS", "Entity");
        var entityId = entityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        entityType.AddKeys(entityId);
        model.AddElement(entityType);

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var navigation = entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "", Target = person, TargetMultiplicity = EdmMultiplicity.One });

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.All(actualErrors, e => Assert.Equal(EdmErrorCode.InvalidName, e.ErrorCode));

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithOneMultiplicityContainmentEnd_PassesValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var pet = new EdmEntityType("NS", "Pet");
        var petId = new EdmStructuralProperty(pet, "Id", EdmCoreModel.Instance.GetInt32(false));
        pet.AddProperty(petId);
        pet.AddKeys(petId);
        model.AddElement(pet);

        var onePetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "OnePetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToOnePet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        pet.AddProperty(onePetToPerson);
        person.AddProperty(onePetToPerson.Partner);

        var zeroOrOnePetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ZeroOrOnePetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToZeroOrOnePet", Target = pet, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true });
        pet.AddProperty(zeroOrOnePetToPerson);
        person.AddProperty(zeroOrOnePetToPerson.Partner);

        var manyPetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ManyPetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToManyPet", Target = pet, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true });
        pet.AddProperty(manyPetToPerson);
        person.AddProperty(manyPetToPerson.Partner);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithManyMultiplicityContainmentEnd_FailsValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var pet = new EdmEntityType("NS", "Pet");
        var petId = new EdmStructuralProperty(pet, "Id", EdmCoreModel.Instance.GetInt32(false));
        pet.AddProperty(petId);
        pet.AddKeys(petId);
        model.AddElement(pet);

        var onePetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "OnePetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "ToOnePet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        pet.AddProperty(onePetToPerson);
        person.AddProperty(onePetToPerson.Partner);

        var zeroOrOnePetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ZeroOrOnePetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "ToZeroOrOnePet", Target = pet, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true });
        pet.AddProperty(zeroOrOnePetToPerson);
        person.AddProperty(zeroOrOnePetToPerson.Partner);

        var manyPetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ManyPetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "ToManyPet", Target = pet, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true });
        pet.AddProperty(manyPetToPerson);
        person.AddProperty(manyPetToPerson.Partner);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(3, actualErrors.Count());

        Assert.All(actualErrors, e => Assert.Equal(EdmErrorCode.NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne, e.ErrorCode));

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());;
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithZeroOrOneMultiplicityContainmentEnd_FailsValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var pet = new EdmEntityType("NS", "Pet");
        var petId = new EdmStructuralProperty(pet, "Id", EdmCoreModel.Instance.GetInt32(false));
        pet.AddProperty(petId);
        pet.AddKeys(petId);
        model.AddElement(pet);

        var onePetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "OnePetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
            new EdmNavigationPropertyInfo() { Name = "ToOnePet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        pet.AddProperty(onePetToPerson);
        person.AddProperty(onePetToPerson.Partner);

        var zeroOrOnePetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ZeroOrOnePetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
            new EdmNavigationPropertyInfo() { Name = "ToZeroOrOnePet", Target = pet, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true });
        pet.AddProperty(zeroOrOnePetToPerson);
        person.AddProperty(zeroOrOnePetToPerson.Partner);

        var manyPetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ManyPetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
            new EdmNavigationPropertyInfo() { Name = "ToManyPet", Target = pet, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true });
        pet.AddProperty(manyPetToPerson);
        person.AddProperty(manyPetToPerson.Partner);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(3, actualErrors.Count());

        Assert.All(actualErrors, e => Assert.Equal(EdmErrorCode.NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne, e.ErrorCode));

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithValidRecursiveContainmentEnd_PassesValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var friendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
        person.AddProperty(friendToPerson);
        person.AddProperty(friendToPerson.Partner);

        var parentToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToSelf", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToParent", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
        person.AddProperty(parentToPerson);
        person.AddProperty(parentToPerson.Partner);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithInvalidRecursiveContainmentEnd_FailsValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var friendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
        person.AddProperty(friendToPerson);
        person.AddProperty(friendToPerson.Partner);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.All(actualErrors, e => Assert.Equal(EdmErrorCode.NavigationPropertyWithRecursiveContainmentTargetMustBeOptional, e.ErrorCode));

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithOneMultiplicityRecursiveContainmentEnd_FailsValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var manyFriendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ManyFriendToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToManyFriend", Target = person, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true });
        person.AddProperty(manyFriendToPerson);
        person.AddProperty(manyFriendToPerson.Partner);

        var zeroOrOneFriendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ZeroOrOneFriendToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToZeroOrOneFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true });
        person.AddProperty(zeroOrOneFriendToPerson);
        person.AddProperty(zeroOrOneFriendToPerson.Partner);

        var oneFriendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "OneFriendToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToOneFriend", Target = person, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        person.AddProperty(oneFriendToPerson);
        person.AddProperty(oneFriendToPerson.Partner);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(4, actualErrors.Count());

        Assert.Equal(3, actualErrors.Count(e => EdmErrorCode.NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne == e.ErrorCode));
        Assert.Equal(1, actualErrors.Count(e => EdmErrorCode.NavigationPropertyWithRecursiveContainmentTargetMustBeOptional == e.ErrorCode));

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithManyMultiplicityRecursiveContainmentEnd_FailsValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var manyFriendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ManyFriendToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "ToManyFriend", Target = person, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true });
        person.AddProperty(manyFriendToPerson);
        person.AddProperty(manyFriendToPerson.Partner);

        var zeroOrOneFriendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ZeroOrOneFriendToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "ToZeroOrOneFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true });
        person.AddProperty(zeroOrOneFriendToPerson);
        person.AddProperty(zeroOrOneFriendToPerson.Partner);

        var oneFriendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "OneFriendToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "ToOneFriend", Target = person, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        person.AddProperty(oneFriendToPerson);
        person.AddProperty(oneFriendToPerson.Partner);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(4, actualErrors.Count());

        Assert.Equal(3, actualErrors.Count(e => EdmErrorCode.NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne == e.ErrorCode));
        Assert.Equal(1, actualErrors.Count(e => EdmErrorCode.NavigationPropertyWithRecursiveContainmentTargetMustBeOptional == e.ErrorCode));

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_SingleSimpleContainmentNavigation_PassesValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var pet = new EdmEntityType("NS", "Pet");
        var petId = new EdmStructuralProperty(pet, "Id", EdmCoreModel.Instance.GetInt32(false));
        pet.AddProperty(petId);
        pet.AddKeys(petId);
        model.AddElement(pet);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var personToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One });
        pet.AddProperty(personToPet.Partner);
        person.AddProperty(personToPet);

        var homeToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One });
        pet.AddProperty(homeToPet.Partner);
        home.AddProperty(homeToPet);

        var container = new EdmEntityContainer("NS", "Container");
        var personSet = container.AddEntitySet("PersonSet", person);
        var petSet = container.AddEntitySet("PetSet", pet);
        var homeSet = container.AddEntitySet("HomeSet", home);

        petSet.AddNavigationTarget(personToPet.Partner, personSet);
        petSet.AddNavigationTarget(homeToPet.Partner, homeSet);

        model.AddElement(container);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_TwoContainmentNavigationsWithSameEnd_PassesValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var pet = new EdmEntityType("NS", "Pet");
        var petId = new EdmStructuralProperty(pet, "Id", EdmCoreModel.Instance.GetInt32(false));
        pet.AddProperty(petId);
        pet.AddKeys(petId);
        model.AddElement(pet);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var personToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One });
        pet.AddProperty(personToPet.Partner);
        person.AddProperty(personToPet);

        var homeToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One });
        pet.AddProperty(homeToPet.Partner);
        home.AddProperty(homeToPet);

        var container = new EdmEntityContainer("NS", "Container");
        var personSet = container.AddEntitySet("PersonSet", person);
        var petSet = container.AddEntitySet("PetSet", pet);
        var homeSet = container.AddEntitySet("HomeSet", home);

        petSet.AddNavigationTarget(personToPet.Partner, personSet);
        petSet.AddNavigationTarget(homeToPet.Partner, homeSet);

        model.AddElement(container);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_TwoContainmentNavigationsWithSameEndAddedDifferently_PassesValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var pet = new EdmEntityType("NS", "Pet");
        var petId = new EdmStructuralProperty(pet, "Id", EdmCoreModel.Instance.GetInt32(false));
        pet.AddProperty(petId);
        pet.AddKeys(petId);
        model.AddElement(pet);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var personToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One });
        pet.AddProperty(personToPet.Partner);
        person.AddProperty(personToPet);

        var homeToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One });
        pet.AddProperty(homeToPet.Partner);
        home.AddProperty(homeToPet);

        var container = new EdmEntityContainer("NS", "Container");
        var personSet = container.AddEntitySet("PersonSet", person);
        var petSet = container.AddEntitySet("PetSet", pet);
        var homeSet = container.AddEntitySet("HomeSet", home);

        // [EdmLib] EntitySet.AddNavigationTarget() ordering matters and results into some validation not appearing
        petSet.AddNavigationTarget(personToPet.Partner, personSet);
        petSet.AddNavigationTarget(homeToPet.Partner, homeSet);

        model.AddElement(container);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_ContainmentNavigationWithDifferentEnds_PassesValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var pet = new EdmEntityType("NS", "Pet");
        var petId = new EdmStructuralProperty(pet, "Id", EdmCoreModel.Instance.GetInt32(false));
        pet.AddProperty(petId);
        pet.AddKeys(petId);
        model.AddElement(pet);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var petToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        pet.AddProperty(petToPerson);
        person.AddProperty(petToPerson.Partner);

        var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        person.AddProperty(homeToPerson.Partner);
        home.AddProperty(homeToPerson);

        var container = new EdmEntityContainer("NS", "Container");
        var personSet = container.AddEntitySet("PersonSet", person);
        var petSet = container.AddEntitySet("PetSet", pet);
        var homeSet = container.AddEntitySet("HomeSet", home);

        petSet.AddNavigationTarget(petToPerson, personSet);
        homeSet.AddNavigationTarget(homeToPerson, personSet);
        model.AddElement(container);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_RecursiveContainmentNavigationWithSelfPointingEntitySet_PassesValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var personToFriends = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
        person.AddProperty(personToFriends.Partner);
        person.AddProperty(personToFriends);

        var container = new EdmEntityContainer("NS", "Container");
        model.AddElement(container);
        var personSet = container.AddEntitySet("PersonSet", person);
        // personSet.AddNavigationTarget(personToFriends, personSet);
        personSet.AddNavigationTarget(personToFriends.Partner, personSet);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_RecursiveContainmentNavigationWithInheritedSelfPointingEntitySet_PassesValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var employee = new EdmEntityType("NS", "Employee", person);
        model.AddElement(employee);

        var personToFriends = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
        person.AddProperty(personToFriends.Partner);
        person.AddProperty(personToFriends);

        var container = new EdmEntityContainer("NS", "Container");
        model.AddElement(container);
        var personSet = container.AddEntitySet("PersonSet", person);
        var employeeSet = container.AddEntitySet("EmployeeSet", employee);
        personSet.AddNavigationTarget(personToFriends, employeeSet);
        employeeSet.AddNavigationTarget(personToFriends.Partner, personSet);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_RecursiveContainmentNavigationWithTwoEntitySets_PassesValidation(EdmVersion edmVersion)
    {
        var csdlElements = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Partner=""ToFriend"" />
    <NavigationProperty Name=""ToFriend"" Type=""NS.Person"" Partner=""ToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""FriendSet"" EntityType=""NS.Person"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
    </EntitySet>
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person""/>
  </EntityContainer>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_DerivedContainmentNavigationWithBaseAssociationSet_PassesValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var employee = new EdmEntityType("NS", "Employee", person);
        model.AddElement(employee);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var office = new EdmEntityType("NS", "Office", home);
        model.AddElement(office);

        var officeToEmployee = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToEmployee", Target = employee, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToOffice", Target = office, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        employee.AddProperty(officeToEmployee.Partner);
        office.AddProperty(officeToEmployee);

        var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        person.AddProperty(homeToPerson.Partner);
        home.AddProperty(homeToPerson);

        var container = new EdmEntityContainer("NS", "Container");
        model.AddElement(container);

        var personSet = container.AddEntitySet("PersonSet", person);
        var employeeSet = container.AddEntitySet("EmployeeSet", employee);
        var homeSet = container.AddEntitySet("HomeSet", home);
        var officeSet = container.AddEntitySet("OfficeSet", office);

        officeSet.AddNavigationTarget(officeToEmployee, personSet);
        homeSet.AddNavigationTarget(homeToPerson, personSet);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_DerivedContainmentNavigationWithDerivedAssociationSet_Model_PassesValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var employee = new EdmEntityType("NS", "Employee", person);
        model.AddElement(employee);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var office = new EdmEntityType("NS", "Office", home);
        model.AddElement(office);

        var officeToEmployee = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToEmployee", Target = employee, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToOffice", Target = office, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        employee.AddProperty(officeToEmployee.Partner);
        office.AddProperty(officeToEmployee);

        var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        person.AddProperty(homeToPerson.Partner);
        home.AddProperty(homeToPerson);

        var container = new EdmEntityContainer("NS", "Container");
        model.AddElement(container);

        var personSet = container.AddEntitySet("PersonSet", person);
        var employeeSet = container.AddEntitySet("EmployeeSet", employee);
        var homeSet = container.AddEntitySet("HomeSet", home);
        var officeSet = container.AddEntitySet("OfficeSet", office);

        officeSet.AddNavigationTarget(homeToPerson, employeeSet);
        homeSet.AddNavigationTarget(homeToPerson, personSet);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_DerivedContainmentNavigationWithDerivedAssociationSet_CSDL_PassesValidation(EdmVersion edmVersion)
    {
        var csdlElements = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Employee"" BaseType=""NS.Person"">
    <NavigationProperty Name=""ToOffice"" Type=""NS.Office"" Nullable=""false"" Partner=""ToEmployee"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Home"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToHome"" />
  </EntityType>
  <EntityType Name=""Office"" BaseType=""NS.Home"">
    <NavigationProperty Name=""ToEmployee"" Type=""NS.Employee"" Nullable=""false"" Partner=""ToOffice"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person"" />
    <EntitySet Name=""EmployeeSet"" EntityType=""NS.Employee""/>
    <EntitySet Name=""HomeSet"" EntityType=""NS.Home"" >
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
    </EntitySet>
    <EntitySet Name=""OfficeSet"" EntityType=""NS.Office"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""EmployeeSet"" />
    </EntitySet>
  </EntityContainer>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_DerivedContainmentNavigationWithDerivedAndBaseAssociationSet_Model_PassesValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var employee = new EdmEntityType("NS", "Employee", person);
        model.AddElement(employee);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var office = new EdmEntityType("NS", "Office", home);
        model.AddElement(office);

        var officeToEmployee = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToEmployee", Target = employee, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToOffice", Target = office, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        employee.AddProperty(officeToEmployee.Partner);
        office.AddProperty(officeToEmployee);

        var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        person.AddProperty(homeToPerson.Partner);
        home.AddProperty(homeToPerson);

        var container = new EdmEntityContainer("NS", "Container");
        model.AddElement(container);

        var personSet = container.AddEntitySet("PersonSet", person);
        var employeeSet = container.AddEntitySet("EmployeeSet", employee);
        var homeSet = container.AddEntitySet("HomeSet", home);
        var officeSet = container.AddEntitySet("OfficeSet", office);

        officeSet.AddNavigationTarget(officeToEmployee, personSet);
        //            personSet.AddNavigationTarget(officeToEmployee.Partner, officeSet);
        homeSet.AddNavigationTarget(homeToPerson, employeeSet);
        //            employeeSet.AddNavigationTarget(homeToPerson.Partner, homeSet);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithInvalidEntitySet_FailsValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        person.AddProperty(homeToPerson.Partner);
        home.AddProperty(homeToPerson);

        // Has not been added to model
        var employee = new EdmEntityType("NS", "Employee", person);
        var office = new EdmEntityType("NS", "Office", home);

        var officeToEmployee = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToEmployee", Target = employee, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToOffice", Target = office, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        employee.AddProperty(officeToEmployee.Partner);
        office.AddProperty(officeToEmployee);

        var container = new EdmEntityContainer("NS", "Container");
        model.AddElement(container);

        var personSet = container.AddEntitySet("PersonSet", person);
        var homeSet = container.AddEntitySet("HomeSet", home);
        var employeeSet = container.AddEntitySet("EmployeeSet", employee);
        var officeSet = container.AddEntitySet("OfficeSet", office);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(2, actualErrors.Count());

        Assert.All(actualErrors, e => Assert.Equal(EdmErrorCode.BadUnresolvedType, e.ErrorCode));

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithInvalidAssociationSetEntitySet_FailsValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        person.AddProperty(homeToPerson.Partner);
        home.AddProperty(homeToPerson);

        var employee = new EdmEntityType("NS", "Employee");
        var employeeId = employee.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        employee.AddKeys(employeeId);
        model.AddElement(employee);

        var office = new EdmEntityType("NS", "Office");
        var officeId = office.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        office.AddKeys(officeId);
        model.AddElement(office);

        var officeToEmployee = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToEmployee", Target = employee, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToOffice", Target = office, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        employee.AddProperty(officeToEmployee.Partner);
        office.AddProperty(officeToEmployee);

        var container = new EdmEntityContainer("NS", "Container");
        model.AddElement(container);

        var personSet = container.AddEntitySet("PersonSet", person);
        var homeSet = container.AddEntitySet("HomeSet", home);

        homeSet.AddNavigationTarget(officeToEmployee, personSet);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(2, actualErrors.Count());

        Assert.Equal(EdmErrorCode.UnresolvedNavigationPropertyBindingPath, actualErrors.First().ErrorCode);
        Assert.Equal(EdmErrorCode.NavigationPropertyMappingMustPointToValidTargetForProperty, actualErrors.Last().ErrorCode);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationWithInvalidEntitySetInSingleton_FailsValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        person.AddProperty(homeToPerson.Partner);
        home.AddProperty(homeToPerson);

        var employee = new EdmEntityType("NS", "Employee");
        var employeeId = employee.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        employee.AddKeys(employeeId);
        model.AddElement(employee);

        var office = new EdmEntityType("NS", "Office");
        var officeId = office.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        office.AddKeys(officeId);
        model.AddElement(office);

        var officeToEmployee = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToEmployee", Target = employee, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToOffice", Target = office, TargetMultiplicity = EdmMultiplicity.One });
        employee.AddProperty(officeToEmployee.Partner);
        office.AddProperty(officeToEmployee);

        var container = new EdmEntityContainer("NS", "Container");
        model.AddElement(container);

        var boss = container.AddSingleton("Boss", person);
        var homeSet = container.AddEntitySet("HomeSet", home);

        homeSet.AddNavigationTarget(officeToEmployee, boss);
        boss.AddNavigationTarget(officeToEmployee.Partner, homeSet);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Equal(4, actualErrors.Count());

        Assert.Equal(2, actualErrors.Count(e => e.ErrorCode == EdmErrorCode.NavigationPropertyMappingMustPointToValidTargetForProperty));
        Assert.Equal(2, actualErrors.Count(e => e.ErrorCode == EdmErrorCode.UnresolvedNavigationPropertyBindingPath));

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NavigationPropertyOfCollectionTypeTargetingSingleton_FailsValidation(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var address = new EdmEntityType("NS", "Address");
        var addressId = address.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        address.AddKeys(addressId);
        model.AddElement(address);

        var personToAddress = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "Addresses", Target = address, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Owner", Target = person, TargetMultiplicity = EdmMultiplicity.One });
        person.AddProperty(personToAddress);
        address.AddProperty(personToAddress.Partner);

        var container = new EdmEntityContainer("NS", "Container");
        model.AddElement(container);

        var people = container.AddEntitySet("People", person);
        var singleAddress = container.AddSingleton("SingleAddress", address);

        people.AddNavigationTarget(personToAddress, singleAddress);
        singleAddress.AddNavigationTarget(personToAddress.Partner, people);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.False(validationResult);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.NavigationPropertyOfCollectionTypeMustNotTargetToSingleton, actualErrors.Last().ErrorCode);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());
    }

    [Fact]
    public void Validate_NavigationWithUnknownMultiplicity_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => 
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var friendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Unknown },
                new EdmNavigationPropertyInfo() { Name = "ToFriend", Target = person, TargetMultiplicity = EdmMultiplicity.One });
            person.AddProperty(friendToPerson);
            person.AddProperty(friendToPerson.Partner);
        });
    }

    [Fact]
    public void Validate_NavigationWithUnknownMultiplicityPartner_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var friendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToFriend", Target = person, TargetMultiplicity = EdmMultiplicity.Unknown });
            person.AddProperty(friendToPerson);
            person.AddProperty(friendToPerson.Partner);
        });
    }
}
