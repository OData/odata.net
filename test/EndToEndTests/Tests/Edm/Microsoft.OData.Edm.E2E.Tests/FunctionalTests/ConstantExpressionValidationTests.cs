//---------------------------------------------------------------------
// <copyright file="ConstantExpressionValidationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class ConstantExpressionValidationTests : EdmLibTestCaseBase
{
   [Fact]
    public void Validate_InvalidIntegerConstantExpressionInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidIntegerConstantAttributeInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidBooleanConstantExpressionInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidBooleanConstantAttributeInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidFloatConstantExpressionInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidFloatConstantAttributeInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidDecimalConstantExpressionInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidDecimalConstantAttributeInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidDurationConstantExpressionInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidDurationConstantAttributeInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidGuidConstantExpressionInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidGuidConstantAttributeInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidDateTimeOffsetConstantExpressionInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidDateTimeOffsetConstantAttributeInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidBinaryConstantAttributeInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidBinaryConstantExpressionInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidDurationValueInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidDurationFormatInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidTypeReferenceForDurationConstantInVocabularyAnnotation_ShouldReturnError()
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

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Act & Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }

   [Fact]
    public void Validate_InvalidMaxDurationConstantAttributeInVocabularyAnnotation_ShouldReturnError()
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
        var parsedCsdl = XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(csdl, EdmVersion.V40), LoadOptions.SetLineInfo);

        bool success = SchemaReader.TryParse(new XElement[] { parsedCsdl }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.False(errors.Any());

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(EdmVersion.V40));

        // Assert
        this.VerifySemanticValidation(testModel, validationRuleSet, expectedErrors);
    }
}
