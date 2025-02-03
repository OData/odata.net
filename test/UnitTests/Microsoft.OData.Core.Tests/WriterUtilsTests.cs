//---------------------------------------------------------------------
// <copyright file="WriterUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Json;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class WriterUtilsTests
    {
        private readonly JsonTypeNameOracle typeNameOracle = new JsonMinimalMetadataTypeNameOracle();

        #region Json ODataValue type name tests
        [Fact]
        public void TypeNameShouldBeWrittenIfSpatialValueIsMoreDerivedThanMetadataType()
        {
            var typeFromMetadata = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, true);
            var typeFromValue = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false);
            var result = this.typeNameOracle.GetValueTypeNameForWriting(new ODataPrimitiveValue(Microsoft.Spatial.GeographyPoint.Create(42, 42)), 
                typeFromMetadata, 
                typeFromValue, 
                /* isOpenProperty*/ false);
            Assert.Equal("Edm.GeographyPoint", result);
            Assert.Equal("#GeographyPoint", WriterUtils.PrefixTypeNameForWriting(result, ODataVersion.V4));
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
            Assert.Equal("Edm.GeographyPoint", result);
            Assert.Equal("GeographyPoint", WriterUtils.PrefixTypeNameForWriting(result, ODataVersion.V401));
        }

        [Fact]
        public void TypeNameShouldNotBeWrittenIfSpatialTypeMatchesTypeInMetadata()
        {
            var typeFromMetadata = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true);
            var typeFromValue = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false);
            Assert.Null(this.typeNameOracle.GetValueTypeNameForWriting(new ODataPrimitiveValue(Microsoft.Spatial.GeographyPoint.Create(42, 42)),
                typeFromMetadata,
                typeFromValue,
                /* isOpenProperty*/ false));
        }

        [Fact]
        public void TypeNameShouldNotBeWrittenForDeclaredComplexProperty()
        {
            var typeFromMetadata = new EdmComplexTypeReference(new EdmComplexType("Test", "ComplexType"), true);
            var typeFromValue = new EdmComplexTypeReference(new EdmComplexType("Test", "ComplexType"), false);
            Assert.Null(this.typeNameOracle.GetResourceTypeNameForWriting(typeFromMetadata.FullName(),
                new ODataResource { TypeName = "Test.ComplexType" },
                /* isUndeclared*/ false));
        }

        [Fact]
        public void TypeNameShouldBeWrittenForUndeclaredComplexProperty()
        {
            var typeFromValue = new EdmComplexTypeReference(new EdmComplexType("Test", "ComplexType"), false);
            Assert.Equal("Test.ComplexType", this.typeNameOracle.GetResourceTypeNameForWriting(null, 
                new ODataResource { TypeName = "Test.ComplexType" },
                /* isUndeclared*/ true));
        }

        [Fact]
        public void TypeNameShouldNotBeWrittenForDeclaredCollectionProperty()
        {
            var typeNameFromMetadata = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true));
            var typeFromValue = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false));
            Assert.Null(this.typeNameOracle.GetValueTypeNameForWriting(new ODataCollectionValue() { TypeName = "Collection(String)" },
                typeNameFromMetadata,
                typeFromValue,
                /* isOpenProperty*/ false));
        }

        [Fact]
        public void TypeNameShouldBeWrittenForUndeclaredCollectionProperty()
        {
            var typeFromValue = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false));
            var result = this.typeNameOracle.GetValueTypeNameForWriting(new ODataCollectionValue() { TypeName = "Collection(String)" },
                null,
                typeFromValue,
                /* isOpenProperty*/ true);
            Assert.Equal("Collection(Edm.String)", result);
            Assert.Equal("#Collection(String)", WriterUtils.PrefixTypeNameForWriting(result, ODataVersion.V4));
        }

        [Fact]
        public void TypeNameShouldBeWrittenForUndeclaredCollectionProperty_401()
        {
            var typeFromValue = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false));
            var result = this.typeNameOracle.GetValueTypeNameForWriting(new ODataCollectionValue() { TypeName = "Collection(String)" },
                null,
                typeFromValue,
                /* isOpenProperty*/ true);
            Assert.Equal("Collection(Edm.String)", result);
            Assert.Equal("Collection(String)", WriterUtils.PrefixTypeNameForWriting(result, ODataVersion.V401));
        }

        [Fact]
        public void TypeNameShouldBeWrittenForCollectionOfDerivedSpatialValues()
        {
            var typeFromMetadata = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, true));
            var typeFromValue = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false));
            Assert.Equal("Collection(Edm.GeographyPoint)",
                this.typeNameOracle.GetValueTypeNameForWriting(new ODataCollectionValue() { TypeName = "Collection(Geography)" },
                typeFromMetadata,
                typeFromValue,
                /* isOpenProperty*/ false));
        }

        [Fact]
        public void TypeNameShouldNotBeWrittenForCollectionOfNonDerivedSpatialValues()
        {
            var typeFromMetadata = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true));
            var typeFromValue = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false));
            Assert.Null(this.typeNameOracle.GetValueTypeNameForWriting(new ODataCollectionValue() { TypeName = "Collection(GeographyPoint)" },
                typeFromMetadata,
                typeFromValue,
                /* isOpenProperty*/ false));
        }

        [Fact]
        public void TypeNameShouldComeFromSerializationTypeNameAnnotationForPrimitiveValue()
        {
            var value = new ODataPrimitiveValue(42);
            value.TypeAnnotation = new ODataTypeAnnotation("FromSTNA");
            Assert.Equal("FromSTNA", this.typeNameOracle.GetValueTypeNameForWriting(value,
                EdmCoreModel.Instance.GetInt32(true),
                EdmCoreModel.Instance.GetInt32(false),
                /* isOpenProperty*/ false));
        }

        [Fact]
        public void TypeNameShouldComeFromSerializationTypeNameAnnotationForComplexValue()
        {
            var value = new ODataResource() {TypeName = "Model.Bla"};
            value.TypeAnnotation = new ODataTypeAnnotation("FromSTNA");
            Assert.Equal("FromSTNA", this.typeNameOracle.GetResourceTypeNameForWriting("Model.Bla", value, /* isUndeclared */ false));
        }

        [Fact]
        public void TypeNameShouldComeFromSerializationTypeNameAnnotationForCollectionValue()
        {
            var value = new ODataCollectionValue() {TypeName = "Collection(Edm.String)"};
            value.TypeAnnotation = new ODataTypeAnnotation("FromSTNA");
            Assert.Equal("FromSTNA", this.typeNameOracle.GetValueTypeNameForWriting(value,
                EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)),
                EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false)),
                /* isOpenProperty*/ false));
        }
        #endregion Json ODataValue type name tests
    }
}
