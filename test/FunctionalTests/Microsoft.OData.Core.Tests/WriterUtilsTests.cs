//---------------------------------------------------------------------
// <copyright file="ODataWriterTypeNameUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests
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
            WriterUtils.PrefixTypeNameForWriting(result, ODataVersion.V4).Should().Be("#GeographyPoint");
        }

        [Fact]
        public void TypeNameShouldBeWrittenIfSpatialValueIsMoreDerivedThanMetadataType_401()
        {
            var typeFromMetadata = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, true);
            var typeFromValue = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false);
            var result = this.typeNameOracle.GetValueTypeNameForWriting(new ODataPrimitiveValue(Microsoft.Spatial.GeographyPoint.Create(42, 42)),
                typeFromMetadata,
                typeFromValue,
                /* isOpenProperty*/ false);
            result.Should().Be("Edm.GeographyPoint");
            WriterUtils.PrefixTypeNameForWriting(result, ODataVersion.V401).Should().Be("GeographyPoint");
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
            this.typeNameOracle.GetResourceTypeNameForWriting(typeFromMetadata.FullName(),
                new ODataResource { TypeName = "Test.ComplexType" },
                /* isUndeclared*/ false).Should().BeNull();
        }

        [Fact]
        public void TypeNameShouldBeWrittenForUndeclaredComplexProperty()
        {
            var typeFromValue = new EdmComplexTypeReference(new EdmComplexType("Test", "ComplexType"), false);
            this.typeNameOracle.GetResourceTypeNameForWriting(null, 
                new ODataResource { TypeName = "Test.ComplexType" },
                /* isUndeclared*/ true).Should().Be("Test.ComplexType");
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
            WriterUtils.PrefixTypeNameForWriting(result, ODataVersion.V4).Should().Be("#Collection(String)");
        }

        [Fact]
        public void TypeNameShouldBeWrittenForUndeclaredCollectionProperty_401()
        {
            var typeFromValue = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false));
            var result = this.typeNameOracle.GetValueTypeNameForWriting(new ODataCollectionValue() { TypeName = "Collection(String)" },
                null,
                typeFromValue,
                /* isOpenProperty*/ true);
            result.Should().Be("Collection(Edm.String)");
            WriterUtils.PrefixTypeNameForWriting(result, ODataVersion.V401).Should().Be("Collection(String)");
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
            var value = new ODataPrimitiveValue(42);
            value.TypeAnnotation = new ODataTypeAnnotation("FromSTNA");
            this.typeNameOracle.GetValueTypeNameForWriting(value,
                EdmCoreModel.Instance.GetInt32(true),
                EdmCoreModel.Instance.GetInt32(false),
                /* isOpenProperty*/ false).Should().Be("FromSTNA");
        }

        [Fact]
        public void TypeNameShouldComeFromSerializationTypeNameAnnotationForComplexValue()
        {
            var value = new ODataResource() {TypeName = "Model.Bla"};
            value.TypeAnnotation = new ODataTypeAnnotation("FromSTNA");
            this.typeNameOracle.GetResourceTypeNameForWriting("Model.Bla", value, /* isUndeclared */ false).Should().Be("FromSTNA");
        }

        [Fact]
        public void TypeNameShouldComeFromSerializationTypeNameAnnotationForCollectionValue()
        {
            var value = new ODataCollectionValue() {TypeName = "Collection(Edm.String)"};
            value.TypeAnnotation = new ODataTypeAnnotation("FromSTNA");
            this.typeNameOracle.GetValueTypeNameForWriting(value,
                EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)),
                EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false)),
                /* isOpenProperty*/ false).Should().Be("FromSTNA");
        }
        #endregion JSON Light ODataValue type name tests
    }
}
