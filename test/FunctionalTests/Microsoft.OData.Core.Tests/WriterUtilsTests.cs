//---------------------------------------------------------------------
// <copyright file="ODataWriterTypeNameUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class WriterUtilsTests
    {
        private readonly JsonLightTypeNameOracle typeNameOracle = new JsonMinimalMetadataTypeNameOracle();

        #region JSON Light ODataValue type name tests
        [Fact]
        public void TypeNameShouldBeWrittenIfSpatialValueIsMoreDerivedThanMetadataType()
        {
            var typeFromMetadata = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, true);
            var typeFromValue = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false);
            var result = this.typeNameOracle.GetValueTypeNameForWriting(new ODataPrimitiveValue(Microsoft.Spatial.GeographyPoint.Create(42, 42)), 
                typeFromMetadata, 
                typeFromValue, 
                /* isOpenProperty*/ false);
            result.Should().Be("Edm.GeographyPoint");
            WriterUtils.RemoveEdmPrefixFromTypeName(result).Should().Be("GeographyPoint");
        }

        [Fact]
        public void TypeNameShouldNotBeWrittenIfSpatialTypeMatchesTypeInMetadata()
        {
            var typeFromMetadata = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true);
            var typeFromValue = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false);
            this.typeNameOracle.GetValueTypeNameForWriting(new ODataPrimitiveValue(Microsoft.Spatial.GeographyPoint.Create(42, 42)),
                typeFromMetadata,
                typeFromValue,
                /* isOpenProperty*/ false).Should().BeNull();
        }

        [Fact]
        public void TypeNameShouldNotBeWrittenForDeclaredComplexProperty()
        {
            var typeFromMetadata = new EdmComplexTypeReference(new EdmComplexType("Test", "ComplexType"), true);
            var typeFromValue = new EdmComplexTypeReference(new EdmComplexType("Test", "ComplexType"), false);
            this.typeNameOracle.GetValueTypeNameForWriting(new ODataComplexValue {TypeName = "Test.ComplexType"},
                typeFromMetadata,
                typeFromValue,
                /* isOpenProperty*/ false).Should().BeNull();
        }

        [Fact]
        public void TypeNameShouldBeWrittenForUndeclaredComplexProperty()
        {
            var typeFromValue = new EdmComplexTypeReference(new EdmComplexType("Test", "ComplexType"), false);
            this.typeNameOracle.GetValueTypeNameForWriting(new ODataComplexValue {TypeName = "Test.ComplexType"},
                null,
                typeFromValue,
                /* isOpenProperty*/ true).Should().Be("Test.ComplexType");
        }

        [Fact]
        public void TypeNameShouldNotBeWrittenForDeclaredCollectionProperty()
        {
            var typeNameFromMetadata = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true));
            var typeFromValue = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false));
            this.typeNameOracle.GetValueTypeNameForWriting(new ODataCollectionValue() { TypeName = "Collection(String)" },
                typeNameFromMetadata,
                typeFromValue,
                /* isOpenProperty*/ false).Should().BeNull();
        }

        [Fact]
        public void TypeNameShouldBeWrittenForUndeclaredCollectionProperty()
        {
            var typeFromValue = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false));
            var result = this.typeNameOracle.GetValueTypeNameForWriting(new ODataCollectionValue() { TypeName = "Collection(String)" },
                null,
                typeFromValue,
                /* isOpenProperty*/ true);
            result.Should().Be("Collection(Edm.String)");
            WriterUtils.RemoveEdmPrefixFromTypeName(result).Should().Be("Collection(String)");
        }

        [Fact]
        public void TypeNameShouldBeWrittenForCollectionOfDerivedSpatialValues()
        {
            var typeFromMetadata = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, true));
            var typeFromValue = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false));
            this.typeNameOracle.GetValueTypeNameForWriting(new ODataCollectionValue() { TypeName = "Collection(Geography)" },
                typeFromMetadata,
                typeFromValue,
                /* isOpenProperty*/ false).Should().Be("Collection(Edm.GeographyPoint)");
        }

        [Fact]
        public void TypeNameShouldNotBeWrittenForCollectionOfNonDerivedSpatialValues()
        {
            var typeFromMetadata = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true));
            var typeFromValue = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false));
            this.typeNameOracle.GetValueTypeNameForWriting(new ODataCollectionValue() { TypeName = "Collection(GeographyPoint)" },
                typeFromMetadata,
                typeFromValue,
                /* isOpenProperty*/ false).Should().BeNull();
        }

        [Fact]
        public void TypeNameShouldComeFromSerializationTypeNameAnnotationForPrimitiveValue()
        {
            var stna = new SerializationTypeNameAnnotation() {TypeName = "FromSTNA"};
            var value = new ODataPrimitiveValue(42);
            value.SetAnnotation(stna);
            this.typeNameOracle.GetValueTypeNameForWriting(value,
                EdmCoreModel.Instance.GetInt32(true),
                EdmCoreModel.Instance.GetInt32(false),
                /* isOpenProperty*/ false).Should().Be("FromSTNA");
        }

        [Fact]
        public void TypeNameShouldComeFromSerializationTypeNameAnnotationForComplexValue()
        {
            var stna = new SerializationTypeNameAnnotation() {TypeName = "FromSTNA"};
            var value = new ODataComplexValue() {TypeName = "Model.Bla"};
            value.SetAnnotation(stna);
            this.typeNameOracle.GetValueTypeNameForWriting(value,
                new EdmComplexTypeReference(new EdmComplexType("Model", "Bla"), true),
                new EdmComplexTypeReference(new EdmComplexType("Model", "Bla"), false),
                /* isOpenProperty*/ false).Should().Be("FromSTNA");
        }

        [Fact]
        public void TypeNameShouldComeFromSerializationTypeNameAnnotationForCollectionValue()
        {
            var stna = new SerializationTypeNameAnnotation() {TypeName = "FromSTNA"};
            var value = new ODataCollectionValue() {TypeName = "Collection(Edm.String)"};
            value.SetAnnotation(stna);
            this.typeNameOracle.GetValueTypeNameForWriting(value,
                EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)),
                EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false)),
                /* isOpenProperty*/ false).Should().Be("FromSTNA");
        }
        #endregion JSON Light ODataValue type name tests
    }
}
