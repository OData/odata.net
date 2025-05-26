//---------------------------------------------------------------------
// <copyright file="ConstantExpressionValidationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class ConstantExpressionValidationTests : EdmLibTestCaseBase
{
    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_InvalidIntegerConstantExpressionInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
        {
            { null, null, EdmErrorCode.InvalidInteger }
        };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Int32"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Int>foo</Int>
        </Annotation>
    </Annotations>
</Schema>";

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

        Assert.Equal(EdmErrorCode.InvalidInteger, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'foo' is not a valid integer. The value must be a valid 32 bit integer.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(6, 14)", actualErrors.Last().ErrorLocation.ToString());

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
    public void Validate_InvalidIntegerConstantAttributeInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
        {
            { null, null, EdmErrorCode.InvalidInteger }
        };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Int32"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Int=""foo"" />
    </Annotations>
</Schema>";

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

        Assert.Equal(EdmErrorCode.InvalidInteger, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'foo' is not a valid integer. The value must be a valid 32 bit integer.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(5, 10)", actualErrors.Last().ErrorLocation.ToString());

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
    public void Validate_InvalidBooleanConstantExpressionInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
        {
            { null, null, EdmErrorCode.InvalidBoolean }
        };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Boolean"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Bool>foo</Bool>
        </Annotation>
    </Annotations>
</Schema>";

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

        Assert.Equal(EdmErrorCode.InvalidBoolean, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'foo' is not a valid boolean. The value must be 'true' or 'false'.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(6, 14)", actualErrors.Last().ErrorLocation.ToString());

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
    public void Validate_InvalidBooleanConstantAttributeInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
        {
            { null, null, EdmErrorCode.InvalidBoolean }
        };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Boolean"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Bool=""foo"" />
    </Annotations>
</Schema>";

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

        Assert.Equal(EdmErrorCode.InvalidBoolean, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'foo' is not a valid boolean. The value must be 'true' or 'false'.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(5, 10)", actualErrors.Last().ErrorLocation.ToString());

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
    public void Validate_InvalidFloatConstantExpressionInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidFloatingPoint }
            };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Double"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Float>foo</Float>
        </Annotation>
    </Annotations>
</Schema>";

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

        Assert.Equal(EdmErrorCode.InvalidFloatingPoint, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'foo' is not a valid floating point value.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(6, 14)", actualErrors.Last().ErrorLocation.ToString());

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
    public void Validate_InvalidFloatConstantAttributeInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidFloatingPoint }
            };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Double"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Float=""foo"" />
    </Annotations>
</Schema>";

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

        Assert.Equal(EdmErrorCode.InvalidFloatingPoint, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'foo' is not a valid floating point value.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(5, 10)", actualErrors.Last().ErrorLocation.ToString());

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
    public void Validate_InvalidDecimalConstantExpressionInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDecimal }
            };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Decimal"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Decimal>foo</Decimal>
        </Annotation>
    </Annotations>
</Schema>";

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
       
        Assert.Equal(EdmErrorCode.InvalidDecimal, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'foo' is not a valid decimal.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(6, 14)", actualErrors.Last().ErrorLocation.ToString());

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
    public void Validate_InvalidDecimalConstantAttributeInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDecimal }
            };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Decimal"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Decimal=""foo"" />
    </Annotations>
</Schema>";

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

        Assert.Equal(EdmErrorCode.InvalidDecimal, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'foo' is not a valid decimal.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(5, 10)", actualErrors.Last().ErrorLocation.ToString());

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
    public void Validate_InvalidDurationConstantExpressionInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
        {
            { null, null, EdmErrorCode.InvalidDuration }
        };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Duration>foo</Duration>
        </Annotation>
    </Annotations>
</Schema>";

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

        Assert.Equal(EdmErrorCode.InvalidDuration, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'foo' is not a valid duration value.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(6, 14)", actualErrors.Last().ErrorLocation.ToString());

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
    public void Validate_InvalidDurationConstantAttributeInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
        {
            { null, null, EdmErrorCode.InvalidDuration }
        };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""foo"" />
    </Annotations>
</Schema>";

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

        Assert.Equal(EdmErrorCode.InvalidDuration, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'foo' is not a valid duration value.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(5, 10)", actualErrors.Last().ErrorLocation.ToString());

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
    public void Validate_InvalidGuidConstantExpressionInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
        {
            { null, null, EdmErrorCode.InvalidGuid }
        };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Guid"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Guid>foo</Guid>
        </Annotation>
    </Annotations>
</Schema>";

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

        Assert.Equal(EdmErrorCode.InvalidGuid, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'foo' is not a valid Guid.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(6, 14)", actualErrors.Last().ErrorLocation.ToString());

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
    public void Validate_InvalidGuidConstantAttributeInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
        {
            { null, null, EdmErrorCode.InvalidGuid }
        };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Guid"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Guid=""foo"" />
    </Annotations>
</Schema>";

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

        Assert.Equal(EdmErrorCode.InvalidGuid, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'foo' is not a valid Guid.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(5, 10)", actualErrors.Last().ErrorLocation.ToString());

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
    public void Validate_InvalidDateTimeOffsetConstantExpressionInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
        {
            { null, null, EdmErrorCode.InvalidDateTimeOffset }
        };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.DateTimeOffset"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <DateTimeOffset>foo</DateTimeOffset>
        </Annotation>
    </Annotations>
</Schema>";

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

        Assert.Equal(EdmErrorCode.InvalidDateTimeOffset, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'foo' is not a valid date time offset value.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(6, 14)", actualErrors.Last().ErrorLocation.ToString());

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
    public void Validate_InvalidDateTimeOffsetConstantAttributeInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
        {
            { null, null, EdmErrorCode.InvalidDateTimeOffset }
        };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.DateTimeOffset"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" DateTimeOffset=""foo"" />
    </Annotations>
</Schema>";

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

        Assert.Equal(EdmErrorCode.InvalidDateTimeOffset, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'foo' is not a valid date time offset value.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(5, 10)", actualErrors.Last().ErrorLocation.ToString());

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
    public void Validate_InvalidBinaryConstantAttributeInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidBinary }
            };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Binary"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Binary=""foo"" />
    </Annotations>
</Schema>";

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

        Assert.Equal(EdmErrorCode.InvalidBinary, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'foo' is not a valid binary value. The value must be a hexadecimal string and must not be prefixed by '0x'.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(5, 10)", actualErrors.Last().ErrorLocation.ToString());

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
    public void Validate_InvalidBinaryConstantExpressionInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidBinary }
            };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Binary"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Binary>foo</Binary>
        </Annotation>
    </Annotations>
</Schema>";

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

        Assert.Equal(EdmErrorCode.InvalidBinary, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'foo' is not a valid binary value. The value must be a hexadecimal string and must not be prefixed by '0x'.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(6, 14)", actualErrors.Last().ErrorLocation.ToString());

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
    public void Validate_InvalidDurationValueInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDuration }
            };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""10675199.02:48:05.4775807"" />
    </Annotations>
</Schema>";

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

        Assert.Equal(EdmErrorCode.InvalidDuration, actualErrors.Last().ErrorCode);
        Assert.Equal("The value '10675199.02:48:05.4775807' is not a valid duration value.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(5, 10)", actualErrors.Last().ErrorLocation.ToString());

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
    public void Validate_InvalidDurationFormatInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDuration }
            };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""1:2:3"" />
    </Annotations>
</Schema>";

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

        Assert.Equal(EdmErrorCode.InvalidDuration, actualErrors.Last().ErrorCode);
        Assert.Equal("The value '1:2:3' is not a valid duration value.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(5, 10)", actualErrors.Last().ErrorLocation.ToString());

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
    public void Validate_InvalidTypeReferenceForDurationConstantInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType },
                { null, null, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType }
            };

        var testModel = new EdmModel();
        var valueTerm = new EdmTerm("DefaultNamespace", "Note", EdmCoreModel.Instance.GetDuration(true));
        testModel.AddElement(valueTerm);

        var valueAnnotation = new EdmVocabularyAnnotation(
            valueTerm,
            valueTerm,
            new EdmDurationConstant(EdmCoreModel.Instance.GetDateTimeOffset(false), new TimeSpan(1, 99, 99, 99, 999)));

        valueAnnotation.SetSerializationLocation(testModel, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        testModel.AddVocabularyAnnotation(valueAnnotation);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Act & Assert
        var validationResult = testModel.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.True(expectedErrors.Count == actualErrors.Count());
        Assert.Equal(2, actualErrors.Count());

        Assert.All(actualErrors, e =>
        {
            Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, e.ErrorCode);
            Assert.Equal("(Microsoft.OData.Edm.Vocabularies.EdmDurationConstant)", e.ErrorLocation.ToString());
        });

        Assert.Equal("Cannot promote the primitive type 'Edm.DateTimeOffset' to the specified primitive type 'Edm.Duration'.", actualErrors.First().ErrorMessage);
        Assert.Equal("The primitive expression is not compatible with the asserted type.", actualErrors.Last().ErrorMessage);

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
    public void Validate_InvalidMaxDurationConstantAttributeInVocabularyAnnotation_ShouldReturnError(EdmVersion edmVersion)
    {
        // Arrange
        var expectedErrors = new EdmLibTestErrors()
        {
            { null, null, EdmErrorCode.InvalidDuration }
        };

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""P10775199DT2H48M5.4775807S"" />
    </Annotations>
</Schema>";

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

        Assert.Equal(EdmErrorCode.InvalidDuration, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'P10775199DT2H48M5.4775807S' is not a valid duration value.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(5, 10)", actualErrors.Last().ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(testModel, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }
}
