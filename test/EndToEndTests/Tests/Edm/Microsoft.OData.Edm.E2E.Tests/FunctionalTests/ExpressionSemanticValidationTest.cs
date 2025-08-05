//---------------------------------------------------------------------
// <copyright file="ExpressionSemanticValidationTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class ExpressionSemanticValidationTest : EdmLibTestCaseBase
{
    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NullCannotBeAssertedToNonNullableType_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""NonNullableTerm"" Type=""Edm.Int32"" Nullable=""false"" />
</Schema>
";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.NonNullableTerm"">
      <Null />
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.NullCannotBeAssertedToBeANonNullableType, actualErrors.Last().ErrorCode);
        Assert.Equal("Null value cannot have a non-nullable type.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(6, 8)", actualErrors.Last().ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_PrimitiveExpressionCannotBeReconciledWithNonPrimitiveType_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""VocabComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
  </ComplexType>
  <Term Name=""ComplexTerm"" Type=""Vocab.VocabComplex"" />
</Schema>
";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.ComplexTerm"" Int=""32"" />
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.PrimitiveConstantExpressionNotValidForNonPrimitiveType, actualErrors.Last().ErrorCode);
        Assert.Equal("A primitive expression is incompatible with a non-primitive type.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(5, 6)", actualErrors.Last().ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_IncorrectPrimitiveTypeForTerm_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Int32"" />
</Schema>
";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" String=""borkborkbork"" />
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.Last().ErrorCode);
        Assert.Equal("The primitive expression is not compatible with the asserted type.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(5, 6)", actualErrors.Last().ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_CorrectPrimitiveTerm_DoesNotReturnValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""IntegerTerm"" Type=""Edm.Int64"" />
</Schema>
";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.IntegerTerm"" Int=""32"" />
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Empty(actualErrors);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_CorrectCollectionTerm_DoesNotReturnValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""CollectionTerm"" Type=""Collection(Edm.Int64)"" />
</Schema>
";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.CollectionTerm"">
        <Collection>
            <Int>1</Int>
            <Int>2</Int>
        </Collection>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Empty(actualErrors);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_CorrectRecordTerm_DoesNotReturnValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""VocabComplex"">
    <Property Name=""ID"" Type=""Edm.Int64"" />
    <Property Name=""Prop1"" Type=""Edm.Int64"" />
    <Property Name=""Prop2"" Type=""Edm.String"" />
  </ComplexType>
  <Term Name=""StructuredTerm"" Type=""Vocab.VocabComplex"" />
</Schema>";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.StructuredTerm"">
        <Record>
          <PropertyValue Property=""Prop1"">
            <Int>256</Int>
          </PropertyValue>
          <PropertyValue Property=""Prop2"">
            <String>A road less traveled.</String>
          </PropertyValue>
          <PropertyValue Property=""ID"">
            <Int>1</Int>
          </PropertyValue>
        </Record>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Empty(actualErrors);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_CollectionTermWithIncorrectItemType_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""CollectionTerm"" Type=""Collection(Edm.Int64)"" />
</Schema>
";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.CollectionTerm"">
        <Collection>
            <Int>1</Int>
            <Int>2</Int>
            <String>Not an Int</String>
        </Collection>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.InvalidInteger, actualErrors.Last().ErrorCode);
        Assert.Equal("The value 'Not an Int' is not a valid integer. The value must be a valid 32 bit integer.", actualErrors.Last().ErrorMessage);
        Assert.Equal("(9, 14)", actualErrors.Last().ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_CollectionTermWithIncorrectDeclaredType_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""CollectionTerm"" Type=""Collection(Edm.Int64)"" />
</Schema>
";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.CollectionTerm"">
        <Collection Type=""Collection(Edm.String)"">
            <String>1</String>
            <String>2</String>
        </Collection>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Equal(3, actualErrors.Count());

        Assert.Equal(EdmErrorCode.ExpressionNotValidForTheAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The type of the expression is incompatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(6, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.ElementAt(1).ErrorCode);
        Assert.Equal("The primitive expression is not compatible with the asserted type.", actualErrors.ElementAt(1).ErrorMessage);
        Assert.Equal("(7, 14)", actualErrors.ElementAt(1).ErrorLocation.ToString());

        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.ElementAt(2).ErrorCode);
        Assert.Equal("The primitive expression is not compatible with the asserted type.", actualErrors.ElementAt(2).ErrorMessage);
        Assert.Equal("(8, 14)", actualErrors.ElementAt(2).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_RecordTermWithRenamedProperty_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""VocabComplex"">
    <Property Name=""ID"" Type=""Edm.Int64"" />
    <Property Name=""Prop1"" Type=""Edm.Int64"" />
    <Property Name=""Prop2"" Type=""Edm.String"" />
  </ComplexType>
  <Term Name=""StructuredTerm"" Type=""Vocab.VocabComplex"" />
</Schema>";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.StructuredTerm"">
        <Record>
          <PropertyValue Property=""Prop1"">
            <Int>256</Int>
          </PropertyValue>
          <PropertyValue Property=""PropBAD"">
            <String>A road less traveled.</String>
          </PropertyValue>
          <PropertyValue Property=""ID"">
            <Int>1</Int>
          </PropertyValue>
        </Record>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.RecordExpressionHasExtraProperties, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The type of the record expression is not open and does not contain a property named 'PropBAD'.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(6, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_RecordTermWithMismatchedPropertyType_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""VocabComplex"">
    <Property Name=""ID"" Type=""Edm.Int64"" />
    <Property Name=""Prop1"" Type=""Edm.Int64"" />
    <Property Name=""Prop2"" Type=""Edm.String"" />
  </ComplexType>
  <Term Name=""StructuredTerm"" Type=""Vocab.VocabComplex"" />
</Schema>";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.StructuredTerm"">
        <Record>
          <PropertyValue Property=""Prop1"">
            <Int>256</Int>
          </PropertyValue>
          <PropertyValue Property=""Prop2"">
            <Binary>DEADBEEFCAFE</Binary>
          </PropertyValue>
          <PropertyValue Property=""ID"">
            <Int>1</Int>
          </PropertyValue>
        </Record>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The primitive expression is not compatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(11, 14)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_CollectionElementInconsistentWithDeclaredType_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""UnTypedTerm"" Type=""Collection(Edm.String)"" />
</Schema>
";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.UnTypedTerm"">
        <Collection Type=""Collection(Edm.String)"">
            <String>1</String>
            <Int>2</Int>
        </Collection>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The primitive expression is not compatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(8, 14)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_RecordWithNonStructuredType_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""EnumTerm"" Type =""CSDL.Color"" />
</Schema>";

        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EnumType Name=""Color"">
      <Member Name=""Red""/>
      <Member Name=""Green""/>
      <Member Name=""Blue""/>
      <Member Name=""Yellow""/>
   </EnumType>
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.EnumTerm"">
        <Record Type=""CSDL.Color"" >
          <PropertyValue Property=""Prop1"">
            <Int>256</Int>
          </PropertyValue>
          <PropertyValue Property=""Prop2"">
            <Binary>DEADBEEFCAFE</Binary>
          </PropertyValue>
          <PropertyValue Property=""ID"">
            <Int>1</Int>
          </PropertyValue>
        </Record>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The type 'CSDL.Color' could not be converted to be a 'Structured' type.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(12, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_RecordTermWithMismatchedPropertyTypeForUntypedTerm_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""ComplexTerm"" Type=""CSDL.TermComplex""/>
</Schema>";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""TermComplex"">
    <Property Name=""ID"" Type=""Edm.Int64"" />
    <Property Name=""Prop1"" Type=""Edm.Int64"" />
    <Property Name=""Prop2"" Type=""Edm.String"" />
  </ComplexType>
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.ComplexTerm"">
        <Record Type=""CSDL.TermComplex"">
          <PropertyValue Property=""Prop1"">
            <Int>256</Int>
          </PropertyValue>
          <PropertyValue Property=""Prop2"">
            <Binary>DEADBEEFCAFE</Binary>
          </PropertyValue>
          <PropertyValue Property=""ID"">
            <Int>1</Int>
          </PropertyValue>
        </Record>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The primitive expression is not compatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(16, 14)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_BinaryValueInHexadecimalFormat_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Annotations"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""bar.BinaryValue"" Binary=""0x1234"" />
    </Annotations>
</Schema>";

        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""bar"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""BinaryValue"" Type=""Binary"" />
</Schema>";

        var csdl3 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
    </EntityType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2,csdl3 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.InvalidBinary, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The value '0x1234' is not a valid binary value. The value must be a hexadecimal string and must not be prefixed by '0x'.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(4, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_NoErrorsForComplexTypeTerm_DoesNotReturnValidationError(EdmVersion edmVersion)
    {
        var csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""TermComplex"">
    <Property Name=""ID"" Type=""Edm.Int64"" />
    <Property Name=""Prop1"" Type=""Edm.Int64"" />
    <Property Name=""Prop2"" Type=""Edm.String"" />
  </ComplexType>
  <Term Name=""TermComplexTerm"" Type=""CSDL.TermComplex"" />
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""CSDL.TermComplexTerm"">
        <Record>
            <PropertyValue Property=""Prop1"">
              <Int>256</Int>
            </PropertyValue>
            <PropertyValue Property=""Prop2"">
              <String>HappyHappy</String>
            </PropertyValue>
            <PropertyValue Property=""ID"">
              <Int>1</Int>
            </PropertyValue>
        </Record>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Empty(actualErrors);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_InvalidTypeUsingCastCollectionCsdl_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""FriendInfo"" Type=""NS.Friend"" />
    <ComplexType Name=""Friend"">
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""NickNames"" Type=""Collection(String)"" />
        <Property Name=""Address"" Type=""NS.Address"" />
    </ComplexType>
    <ComplexType Name=""Address"">
        <Property Name=""StreetNumber"" Type=""Edm.Int32"" />
        <Property Name=""StreetName"" Type=""String"" />
    </ComplexType>
    <Annotations Target=""NS.FriendInfo"">
        <Annotation Term=""NS.FriendInfo"">
            <Cast Type=""NS.Address"">
                <Collection>
                    <String>foo</String>
                    <String>bar</String>
                </Collection>
            </Cast>
        </Annotation>
    </Annotations>
</Schema>";

        var csdlElements = new[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionNotValidForTheAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The type of the expression is incompatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(15, 14)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_InvalidTypeUsingCastCollectionModel_ReturnsValidationError(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var address = new EdmComplexType("NS", "Address");
        address.AddStructuralProperty("StreetNumber", EdmCoreModel.Instance.GetInt32(true));
        address.AddStructuralProperty("StreetName", EdmCoreModel.Instance.GetString(true));
        model.AddElement(address);

        var friend = new EdmComplexType("NS", "Friend");
        friend.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
        friend.AddStructuralProperty("NickNames", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
        friend.AddStructuralProperty("Address", new EdmComplexTypeReference(address, true));
        model.AddElement(friend);

        var friendInfo = new EdmTerm("NS", "FriendInfo", new EdmComplexTypeReference(friend, true));
        model.AddElement(friendInfo);

        var valueAnnotationCast = new EdmCastExpression(new EdmCollectionExpression(new EdmStringConstant("foo"), new EdmStringConstant("bar")), new EdmComplexTypeReference(address, true));
        var valueAnnotation = new EdmVocabularyAnnotation(
            friendInfo,
            friendInfo,
            valueAnnotationCast);
        valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        model.AddVocabularyAnnotation(valueAnnotation);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionNotValidForTheAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The type of the expression is incompatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_CastNullableToNonNullableCsdl_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""FriendInfo"" Type=""Collection(NS.Friend)"" />
    <ComplexType Name=""Friend"">
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""NickNames"" Type=""Collection(String)"" />
        <Property Name=""Address"" Type=""NS.Address"" />
    </ComplexType>
    <ComplexType Name=""Address"">
        <Property Name=""StreetNumber"" Type=""Edm.Int32"" />
        <Property Name=""StreetName"" Type=""String"" />
    </ComplexType>
    <Annotations Target=""NS.FriendInfo"">
        <Annotation Term=""NS.FriendInfo"">
            <Cast Type=""NS.Friend"">
                <Collection>
                    <String>foo</String>
                    <String>bar</String>
                </Collection>
            </Cast>
        </Annotation>
    </Annotations>
</Schema>";

        var csdlElements = new[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionNotValidForTheAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The type of the expression is incompatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(15, 14)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_CastNullableToNonNullableModel_ReturnsValidationError(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var address = new EdmComplexType("NS", "Address");
        address.AddStructuralProperty("StreetNumber", EdmCoreModel.Instance.GetInt32(true));
        address.AddStructuralProperty("StreetName", EdmCoreModel.Instance.GetString(true));
        model.AddElement(address);

        var friend = new EdmComplexType("NS", "Friend");
        friend.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
        friend.AddStructuralProperty("NickNames", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
        friend.AddStructuralProperty("Address", new EdmComplexTypeReference(address, true));
        model.AddElement(friend);

        var friendInfo = new EdmTerm("NS", "FriendInfo", EdmCoreModel.GetCollection(new EdmComplexTypeReference(friend, true)));
        model.AddElement(friendInfo);

        var valueAnnotationCast = new EdmCastExpression(new EdmCollectionExpression(new EdmStringConstant("foo"), new EdmStringConstant("bar")), new EdmComplexTypeReference(friend, true));
        var valueAnnotation = new EdmVocabularyAnnotation(
            friendInfo,
            friendInfo,
            valueAnnotationCast);
        valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        model.AddVocabularyAnnotation(valueAnnotation);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionNotValidForTheAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The type of the expression is incompatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_CastNullableToNonNullableOnInlineAnnotationCsdl_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""FriendInfo"" Type=""Collection(NS.Friend)"">
        <Annotation Term=""NS.FriendInfo"">
            <Cast Type=""NS.Friend"">
                <Collection>
                    <String>foo</String>
                    <String>bar</String>
                </Collection>
            </Cast>
        </Annotation>
    </Term>
    <ComplexType Name=""Friend"">
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""NickNames"" Type=""Collection(String)"" />
        <Property Name=""Address"" Type=""NS.Address"" />
    </ComplexType>
    <ComplexType Name=""Address"">
        <Property Name=""StreetNumber"" Type=""Edm.Int32"" />
        <Property Name=""StreetName"" Type=""String"" />
    </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionNotValidForTheAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The type of the expression is incompatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(5, 14)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_CastNullableToNonNullableOnInlineAnnotationModel_ReturnsValidationError(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var address = new EdmComplexType("NS", "Address");
        address.AddStructuralProperty("StreetNumber", EdmCoreModel.Instance.GetInt32(true));
        address.AddStructuralProperty("StreetName", EdmCoreModel.Instance.GetString(true));
        model.AddElement(address);

        var friend = new EdmComplexType("NS", "Friend");
        friend.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
        friend.AddStructuralProperty("NickNames", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
        friend.AddStructuralProperty("Address", new EdmComplexTypeReference(address, true));
        model.AddElement(friend);

        var friendInfo = new EdmTerm("NS", "FriendInfo", EdmCoreModel.GetCollection(new EdmComplexTypeReference(friend, true)));
        model.AddElement(friendInfo);

        var valueAnnotationCast = new EdmCastExpression(new EdmCollectionExpression(new EdmStringConstant("foo"), new EdmStringConstant("bar")), new EdmComplexTypeReference(friend, true));
        var valueAnnotation = new EdmVocabularyAnnotation(
            friendInfo,
            friendInfo,
            valueAnnotationCast);
        valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
        model.AddVocabularyAnnotation(valueAnnotation);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionNotValidForTheAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The type of the expression is incompatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_CastResultFalseEvaluationCsdl_DoesNotReturnValidationError(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Friend"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Address"" Type=""NS.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""StreetNumber"" Type=""Edm.Int32"" />
        <Property Name=""StreetName"" Type=""String"" />
    </ComplexType>
    <Term Name=""FriendTerm"" Type=""NS.Friend""/>
    <Annotations Target=""NS.Friend"">
        <Annotation Term=""NS.FriendTerm"">
            <Record>
                <PropertyValue Property=""Name"" String=""foo"" />
                <PropertyValue Property=""Address"">
                    <Cast Type=""NS.Address"">
                        <Collection>
                            <String>foo</String>
                            <String>bar</String>
                        </Collection>
                    </Cast>
                </PropertyValue>
            </Record>
        </Annotation>
    </Annotations>
</Schema>";

        var csdlElements = new[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Empty(actualErrors);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_CastResultFalseEvaluationModel_DoesNotReturnValidationError(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var address = new EdmComplexType("NS", "Address");
        address.AddStructuralProperty("StreetNumber", EdmCoreModel.Instance.GetInt32(true));
        address.AddStructuralProperty("StreetName", EdmCoreModel.Instance.GetString(true));
        model.AddElement(address);

        var friend = new EdmEntityType("NS", "Friend");
        var friendName = friend.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        friend.AddKeys(friendName);
        var friendAddress = friend.AddStructuralProperty("Address", new EdmComplexTypeReference(address, true));
        model.AddElement(friend);

        var friendAddressCast = new EdmCastExpression(new EdmCollectionExpression(new EdmStringConstant("foo"), new EdmStringConstant("bar")), new EdmComplexTypeReference(address, true));

        var friendTerm = new EdmTerm("NS", "FriendTerm", new EdmEntityTypeReference(friend, true));
        model.AddElement(friendTerm);

        var valueAnnotation = new EdmVocabularyAnnotation(
            friend,
            friendTerm,
            new EdmRecordExpression(
                new EdmPropertyConstructor(friendName.Name, new EdmStringConstant("foo")),
                new EdmPropertyConstructor(friendAddress.Name, friendAddressCast)));

        valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        model.AddVocabularyAnnotation(valueAnnotation);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Empty(actualErrors);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_CastResultTrueEvaluationCsdl_DoesNotReturnValidationError(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Friend"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Address"" Type=""NS.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""StreetNumber"" Type=""Edm.Int32"" />
        <Property Name=""StreetName"" Type=""String"" />
    </ComplexType>
    <Term Name=""FriendTerm"" Type=""NS.Friend"" />
    <Annotations Target=""NS.Friend"">
        <Annotation Term=""NS.FriendTerm"">
            <Record>
                <PropertyValue Property=""Name"" String=""foo"" />
                <PropertyValue Property=""Address"">
                    <Cast Type=""NS.Address"">
                        <Record>
                            <PropertyValue Property=""StreetNumber"" Int=""3"" />
                            <PropertyValue Property=""StreetName"" String=""에O詰　갂คำŚёæ"" />
                        </Record>
                    </Cast>
                </PropertyValue>
            </Record>
        </Annotation>
    </Annotations>
</Schema>";

        var csdlElements = new[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Empty(actualErrors);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_CastResultTrueEvaluationModel_DoesNotReturnValidationError(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var address = new EdmComplexType("NS", "Address");
        address.AddStructuralProperty("StreetNumber", EdmCoreModel.Instance.GetInt32(true));
        address.AddStructuralProperty("StreetName", EdmCoreModel.Instance.GetString(true));
        model.AddElement(address);

        var friend = new EdmEntityType("NS", "Friend");
        var friendName = friend.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        friend.AddKeys(friendName);
        var friendAddress = friend.AddStructuralProperty("Address", new EdmComplexTypeReference(address, true));
        model.AddElement(friend);

        var addressRecord = new EdmRecordExpression(new EdmPropertyConstructor[] {
                new EdmPropertyConstructor("StreetNumber", new EdmIntegerConstant(3)),
                new EdmPropertyConstructor("StreetName", new EdmStringConstant("에O詰　갂คำŚёæ"))
            });

        var friendAddressCast = new EdmCastExpression(addressRecord, new EdmComplexTypeReference(address, true));

        var friendTerm = new EdmTerm("NS", "FriendTerm", new EdmEntityTypeReference(friend, true));
        model.AddElement(friendTerm);

        var valueAnnotation = new EdmVocabularyAnnotation(
            friend,
            friendTerm,
            new EdmRecordExpression(
                new EdmPropertyConstructor(friendName.Name, new EdmStringConstant("foo")),
                new EdmPropertyConstructor(friendAddress.Name, friendAddressCast)));

        valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        model.AddVocabularyAnnotation(valueAnnotation);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Empty(actualErrors);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_InvalidPropertyTypeUsingIsOfOnOutOfLineAnnotationCsdl_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""FriendName"" Type=""Edm.String"" />
    <Annotations Target=""NS.FriendName"">
        <Annotation Term=""NS.FriendName"">
            <IsOf Type=""Edm.String"">
                <String>foo</String>
            </IsOf>
        </Annotation>
    </Annotations>
</Schema>";

        var csdlElements = new[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("Cannot promote the primitive type 'Edm.Boolean' to the specified primitive type 'Edm.String'.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(6, 14)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_InvalidPropertyTypeUsingIsOfOnOutOfLineAnnotationModel_ReturnsValidationError(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var friendName = new EdmTerm("NS", "FriendName", EdmCoreModel.Instance.GetString(true));
        model.AddElement(friendName);

        var valueAnnotation = new EdmVocabularyAnnotation(
            friendName,
            friendName,
            new EdmIsOfExpression(new EdmStringConstant("foo"), EdmCoreModel.Instance.GetString(true)));
        valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        model.AddVocabularyAnnotation(valueAnnotation);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("Cannot promote the primitive type 'Edm.Boolean' to the specified primitive type 'Edm.String'.", actualErrors.ElementAt(0).ErrorMessage);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_InvalidPropertyTypeUsingIsOfOnInlineAnnotationCsdl_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""CarTerm"" Type=""NS.Car"" />
    <ComplexType Name=""Car"">
        <Property Name=""Expensive"" Type=""NS.Bike"" />
        <Annotation Term=""NS.CarTerm"">
            <Record>
                <PropertyValue Property=""Expensive"">
                    <IsOf Type=""Edm.String"">
                        <String>foo</String>
                    </IsOf>
                </PropertyValue>
            </Record>
        </Annotation>
    </ComplexType>
    <ComplexType Name=""Bike"">
        <Property Name=""Color"" Type=""String"" />
    </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionNotValidForTheAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The type of the expression is incompatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(9, 22)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_InvalidPropertyTypeUsingIsOfOnInlineAnnotationModel_ReturnsValidationError(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var bike = new EdmComplexType("NS", "Bike");
        bike.AddStructuralProperty("Color", EdmCoreModel.Instance.GetString(true));
        model.AddElement(bike);

        var car = new EdmComplexType("NS", "Car");
        var carExpensive = car.AddStructuralProperty("Expensive", new EdmComplexTypeReference(bike, true));
        model.AddElement(car);

        var carTerm = new EdmTerm("NS", "CarTerm", new EdmComplexTypeReference(car, true));
        model.AddElement(carTerm);

        var valueAnnotation = new EdmVocabularyAnnotation(
            car,
            carTerm,
            new EdmRecordExpression(
                new EdmPropertyConstructor(carExpensive.Name, new EdmIsOfExpression(new EdmStringConstant("foo"), EdmCoreModel.Instance.GetString(true)))));
        valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
        model.AddVocabularyAnnotation(valueAnnotation);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionNotValidForTheAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The type of the expression is incompatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_IsOfResultFalseEvaluationCsdl_DoesNotReturnValidationError(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""BooleanFlag"" Type=""Edm.Boolean"" />
    <Annotations Target=""NS.BooleanFlag"">
        <Annotation Term=""NS.BooleanFlag"">
            <IsOf Type=""Edm.String"">
                <Int>32</Int>
            </IsOf>
        </Annotation>
    </Annotations>
</Schema>";

        var csdlElements = new[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Empty(actualErrors);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_IsOfResultFalseEvaluationModel_DoesNotReturnValidationError(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var booleanFlag = new EdmTerm("NS", "BooleanFlag", EdmCoreModel.Instance.GetBoolean(true));
        model.AddElement(booleanFlag);

        var valueAnnotation = new EdmVocabularyAnnotation(
            booleanFlag,
            booleanFlag,
            new EdmIsOfExpression(new EdmIntegerConstant(32), EdmCoreModel.Instance.GetString(true)));
        valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        model.AddVocabularyAnnotation(valueAnnotation);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Empty(actualErrors);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_IsOfResultTrueEvaluationCsdl_DoesNotReturnValidationError(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""BooleanFlag"">
        <Property Name=""Flag"" Type=""Edm.Boolean"" />
    </ComplexType>
    <Term Name=""BooleanFlagTerm"" Type=""NS.BooleanFlag"" />
    <Annotations Target=""NS.BooleanFlag"">
        <Annotation Term=""NS.BooleanFlagTerm"">
            <Record>
                <PropertyValue Property=""Flag"">
                    <IsOf Type=""Edm.String"">
                        <String>foo</String>
                    </IsOf>
                </PropertyValue>
            </Record>
        </Annotation>
    </Annotations>
</Schema>";

        var csdlElements = new[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Empty(actualErrors);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_IsOfResultTrueEvaluationModel_DoesNotReturnValidationError(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var booleanFlag = new EdmComplexType("NS", "BooleanFlag");
        var flag = booleanFlag.AddStructuralProperty("Flag", EdmCoreModel.Instance.GetBoolean(true));
        model.AddElement(booleanFlag);

        var booleanFlagTerm = new EdmTerm("NS", "BooleanFlagTerm", new EdmComplexTypeReference(booleanFlag, true));
        model.AddElement(booleanFlagTerm);

        var valueAnnotation = new EdmVocabularyAnnotation(
            booleanFlag,
            booleanFlagTerm,
            new EdmRecordExpression(
                new EdmPropertyConstructor(flag.Name, new EdmIsOfExpression(new EdmStringConstant("foo"), EdmCoreModel.Instance.GetString(true)))));
        valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Empty(actualErrors);

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_IncorrectTypeForCollectionExpression_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
        @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Int32"" />
</Schema>
";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <Collection>
            <String>BorkBorkBork</String>
        </Collection>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.CollectionExpressionNotValidForNonCollectionType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("A collection expression is incompatible with a non-collection type.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(6, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_IncorrectTypeForGuidExpression_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Int32"" />
</Schema>
";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <Guid>4ae71c81-c21a-40a2-8d53-f1a29ed4a2f3</Guid>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The primitive expression is not compatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(6, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_IncorrectTypeForFloatingExpression_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Int32"" />
</Schema>
";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <Float>3.14</Float>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The primitive expression is not compatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(6, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_IncorrectTypeForDecimalExpression_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Int32"" />
</Schema>
";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <Decimal>3.14</Decimal>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The primitive expression is not compatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(6, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_IncorrectTypeForDateExpression_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Int32"" />
</Schema>
";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <Date>2001-10-26</Date>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The primitive expression is not compatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(6, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_IncorrectFormatForDateExpression_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Date"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <Date>01-10-26</Date>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.InvalidDate, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The value '01-10-26' is not a valid date value.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(6, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_IncorrectTypeForDateTimeOffsetExpression_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Int32"" />
</Schema>
";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <DateTimeOffset>2001-10-26T19:32:52+00:00</DateTimeOffset>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The primitive expression is not compatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(6, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_IncorrectTypeForBooleanExpression_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Int32"" />
</Schema>
";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <Bool>True</Bool>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The primitive expression is not compatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(6, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_IncorrectTypeForTimeOfDayExpression_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Date"" />
</Schema>
";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Date"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <TimeOfDay>1:30:05.900</TimeOfDay>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The primitive expression is not compatible with the asserted type.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(6, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_IncorrectFormatForTimeOfDayExpression_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.TimeOfDay"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <TimeOfDay>-1:10:26</TimeOfDay>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.InvalidTimeOfDay, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The value '-1:10:26' is not a valid TimeOfDay value.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(6, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_StringTooLong_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.String"" MaxLength=""5""/>
</Schema>
";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <String>Really long string</String>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.StringConstantLengthOutOfRange, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The value of the string constant is '18' characters long, but the max length of its type is '5'.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(6, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_BinaryTooLong_ReturnsValidationError(EdmVersion edmVersion)
    {
        var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Binary"" MaxLength=""5""/>
</Schema>
";
        var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <Binary>BAADF00DCAFE</Binary>
    </Annotation>
  </ComplexType>
</Schema>";

        var csdlElements = new[] { csdl1, csdl2 }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        // Assert
        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.BinaryConstantLengthOutOfRange, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The value of the binary constant is '6' characters long, but the max length of its type is '5'.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(6, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);
    }
}
