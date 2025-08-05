//---------------------------------------------------------------------
// <copyright file="ConstantExpressionRoundTripTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class ConstantExpressionRoundTripTests : EdmLibTestCaseBase
{
    #region Duration
    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void RoundTrip_InvalidDurationConstantExpressionInVocabularyAnnotation_ShouldReturnDefaultDuration(EdmVersion edmVersion)
    {
        // Arrange
        var expectedCsdl = new XElement[] { XElement.Parse(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""PT0S"" />
    </Annotations>
</Schema>", LoadOptions.SetLineInfo) };

        var inputCsdl = new XElement[] { XElement.Parse(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Duration>foo</Duration>
        </Annotation>
    </Annotations>
</Schema>", LoadOptions.SetLineInfo) };

        // Act & Assert
        var isParsed = SchemaReader.TryParse(inputCsdl.Select(e => e.CreateReader()), out var model, out var errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        var actualCsdl = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var actualXElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdl.ToArray(), edmVersion);
        var expectedXElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdl.ToArray(), edmVersion);

        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void RoundTrip_InvalidDurationConstantAttributeInVocabularyAnnotation_ShouldReturnDefaultDuration(EdmVersion edmVersion)
    {
        // Arrange
        var expectedCsdl = new XElement[] { XElement.Parse(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""PT0S"" />
    </Annotations>
</Schema>", LoadOptions.SetLineInfo) };

        var inputCsdl = new XElement[] { XElement.Parse(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""foo"" />
    </Annotations>
</Schema>", LoadOptions.SetLineInfo) };

        // Act & Assert
        var isParsed = SchemaReader.TryParse(inputCsdl.Select(e => e.CreateReader()), out var model, out var errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        var actualCsdl = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var actualXElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdl.ToArray(), edmVersion);
        var expectedXElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdl.ToArray(), edmVersion);

        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Fact]
    public void RoundTrip_ValidDefaultDurationConstantAttributeInVocabularyAnnotation_ShouldPassValidation()
    {
        // Arrange
        var inputCsdl = new XElement[] { XElement.Parse(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""PT0S"" />
    </Annotations>
</Schema>", LoadOptions.SetLineInfo) };

        // Act & Assert
        var isParsed = SchemaReader.TryParse(inputCsdl.Select(e => e.CreateReader()), out var model, out var errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        model.Validate(EdmConstants.EdmVersion4, out IEnumerable<EdmError> validationErrors);
        Assert.Empty(validationErrors);
    }

    [Fact]
    public void RoundTrip_ValidDurationConstantAttributeInVocabularyAnnotation_ShouldPassValidation()
    {
        // Arrange
        var inputCsdl = new XElement[] { XElement.Parse(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""P1DT1H1M1.001S"" />
    </Annotations>
</Schema>", LoadOptions.SetLineInfo) };

        // Act & Assert
        var isParsed = SchemaReader.TryParse(inputCsdl.Select(e => e.CreateReader()), out var model, out var errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        model.Validate(EdmConstants.EdmVersion4, out IEnumerable<EdmError> validationErrors);
        Assert.Empty(validationErrors);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void RoundTrip_InvalidValueDurationConstantAttributeInVocabularyAnnotation_ShouldReturnDefaultDuration(EdmVersion edmVersion)
    {
        // Arrange
        var expectedCsdl = new XElement[] { XElement.Parse(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""PT0S"" />
    </Annotations>
</Schema>", LoadOptions.SetLineInfo) };

        var inputCsdl = new XElement[] { XElement.Parse(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""10675199.02:48:05.4775807"" />
    </Annotations>
</Schema>", LoadOptions.SetLineInfo) };

        // Act & Assert
        var isParsed = SchemaReader.TryParse(inputCsdl.Select(e => e.CreateReader()), out var model, out var errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        var actualCsdl = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var actualXElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdl.ToArray(), edmVersion);
        var expectedXElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdl.ToArray(), edmVersion);

        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void RoundTrip_InvalidFormatDurationConstantAttributeInVocabularyAnnotation_ShouldReturnDefaultDuration(EdmVersion edmVersion)
    {
        // Arrange
        var expectedCsdl = new XElement[] { XElement.Parse(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""PT0S"" />
    </Annotations>
</Schema>", LoadOptions.SetLineInfo) };

        var inputCsdl = new XElement[] { XElement.Parse(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""1:2:3"" />
    </Annotations>
</Schema>", LoadOptions.SetLineInfo) };

        // Act & Assert
        var isParsed = SchemaReader.TryParse(inputCsdl.Select(e => e.CreateReader()), out var model, out var errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        var actualCsdl = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var actualXElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdl.ToArray(), edmVersion);
        var expectedXElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdl.ToArray(), edmVersion);

        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void RoundTrip_ValidDurationConstantAttributeInModel_ShouldMatchExpectedCsdl(EdmVersion edmVersion)
    {
        // Arrange
        var expectedCsdl = new XElement[] { XElement.Parse(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""P1DT1H1M1.001S"" />
    </Annotations>
</Schema>", LoadOptions.SetLineInfo) };

        var inputModel = new EdmModel();
        var valueTerm = new EdmTerm("DefaultNamespace", "Note", EdmCoreModel.Instance.GetDuration(true));
        inputModel.AddElement(valueTerm);

        var valueAnnotation = new EdmVocabularyAnnotation(
            valueTerm,
            valueTerm,
            new EdmDurationConstant(new TimeSpan(1, 1, 1, 1, 1)));

        valueAnnotation.SetSerializationLocation(inputModel, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        inputModel.AddVocabularyAnnotation(valueAnnotation);

        var inputCsdl = this.GetSerializerResult(inputModel).Select(n => XElement.Parse(n));

        // Act & Assert
        var isParsed = SchemaReader.TryParse(inputCsdl.Select(e => e.CreateReader()), out var model, out var errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        model.Validate(EdmConstants.EdmVersion4, out IEnumerable<EdmError> validationErrors);
        Assert.Empty(validationErrors);

        var actualCsdl = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var actualXElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdl.ToArray(), edmVersion);
        var expectedXElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdl.ToArray(), edmVersion);

        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void RoundTrip_ValidLargeDurationConstantInModel_ShouldMatchExpectedCsdl(EdmVersion edmVersion)
    {
        // Arrange
        var expectedCsdl = new XElement[] { XElement.Parse(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Annotations Target=""DefaultNamespace.Note"">
    <Annotation Term=""DefaultNamespace.Note"" Duration=""P5DT4H40M39.999S"" />
  </Annotations>
  <Term Name=""Note"" Type=""Edm.Duration"" />
</Schema>", LoadOptions.SetLineInfo) };

        var inputModel = new EdmModel();
        var valueTerm = new EdmTerm("DefaultNamespace", "Note", EdmCoreModel.Instance.GetDuration(true));
        inputModel.AddElement(valueTerm);

        var valueAnnotation = new EdmVocabularyAnnotation(
            valueTerm,
            valueTerm,
            new EdmDurationConstant(new TimeSpan(1, 99, 99, 99, 999)));

        valueAnnotation.SetSerializationLocation(inputModel, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        inputModel.AddVocabularyAnnotation(valueAnnotation);

        var inputCsdl = this.GetSerializerResult(inputModel).Select(n => XElement.Parse(n));

        // Act & Assert
        var isParsed = SchemaReader.TryParse(inputCsdl.Select(e => e.CreateReader()), out var model, out var errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        model.Validate(EdmConstants.EdmVersion4, out IEnumerable<EdmError> validationErrors);
        Assert.Empty(validationErrors);

        var actualCsdl = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var actualXElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdl.ToArray(), edmVersion);
        var expectedXElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdl.ToArray(), edmVersion);

        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void RoundTrip_ValidDefaultDurationConstantInModel_ShouldMatchExpectedCsdl(EdmVersion edmVersion)
    {
        // Arrange
        var expectedCsdl = new XElement[] { XElement.Parse(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""PT0S"" />
    </Annotations>
</Schema>", LoadOptions.SetLineInfo) };

        var inputModel = new EdmModel();
        var valueTerm = new EdmTerm("DefaultNamespace", "Note", EdmCoreModel.Instance.GetDuration(true));
        inputModel.AddElement(valueTerm);

        var valueAnnotation = new EdmVocabularyAnnotation(
            valueTerm,
            valueTerm,
            new EdmDurationConstant(new TimeSpan()));

        valueAnnotation.SetSerializationLocation(inputModel, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        inputModel.AddVocabularyAnnotation(valueAnnotation);

        var inputCsdl = this.GetSerializerResult(inputModel).Select(n => XElement.Parse(n));

        // Act & Assert
        var isParsed = SchemaReader.TryParse(inputCsdl.Select(e => e.CreateReader()), out var model, out var errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        model.Validate(EdmConstants.EdmVersion4, out IEnumerable<EdmError> validationErrors);
        Assert.Empty(validationErrors);

        var actualCsdl = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var actualXElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdl.ToArray(), edmVersion);
        var expectedXElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdl.ToArray(), edmVersion);

        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void RoundTrip_InvalidTypeReferenceForDurationConstantInModel_ShouldMatchExpectedCsdl(EdmVersion edmVersion)
    {
        // Arrange
        var expectedCsdl = new XElement[] { XElement.Parse(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Annotations Target=""DefaultNamespace.Note"">
    <Annotation Term=""DefaultNamespace.Note"" Duration=""P5DT4H40M39.999S"" />
  </Annotations>
  <Term Name=""Note"" Type=""Edm.Duration"" />
</Schema>", LoadOptions.SetLineInfo) };

        var inputModel = new EdmModel();
        var valueTerm = new EdmTerm("DefaultNamespace", "Note", EdmCoreModel.Instance.GetDuration(true));
        inputModel.AddElement(valueTerm);

        var valueAnnotation = new EdmVocabularyAnnotation(
            valueTerm,
            valueTerm,
            new EdmDurationConstant(EdmCoreModel.Instance.GetDateTimeOffset(false), new TimeSpan(1, 99, 99, 99, 999)));

        valueAnnotation.SetSerializationLocation(inputModel, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        inputModel.AddVocabularyAnnotation(valueAnnotation);

        var inputCsdl = this.GetSerializerResult(inputModel).Select(n => XElement.Parse(n));

        // Act & Assert
        var isParsed = SchemaReader.TryParse(inputCsdl.Select(e => e.CreateReader()), out var model, out var errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        model.Validate(EdmConstants.EdmVersion4, out IEnumerable<EdmError> validationErrors);
        Assert.Empty(validationErrors);

        var actualCsdl = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var actualXElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdl.ToArray(), edmVersion);
        var expectedXElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdl.ToArray(), edmVersion);

        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    #endregion

    #region Guid

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void RoundTrip_InvalidGuidConstantExpressionInVocabularyAnnotation_ShouldReturnDefaultGuid(EdmVersion edmVersion)
    {
        // Arrange
        var expectedCsdl = new XElement[] { XElement.Parse(@"
    <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Guid"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Guid=""00000000-0000-0000-0000-000000000000"" />
    </Annotations>
    </Schema>", LoadOptions.SetLineInfo) };

        var inputCsdl = new XElement[] { XElement.Parse(@"
    <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Guid"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Guid>invalid-guid</Guid>
        </Annotation>
    </Annotations>
    </Schema>", LoadOptions.SetLineInfo) };

        // Act & Assert
        var isParsed = SchemaReader.TryParse(inputCsdl.Select(e => e.CreateReader()), out var model, out var errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        var actualCsdl = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var actualXElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdl.ToArray(), edmVersion);
        var expectedXElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdl.ToArray(), edmVersion);

        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    #endregion
}
