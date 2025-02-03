//---------------------------------------------------------------------
// <copyright file="JsonMinimalMetadataTypeNameOracleTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Json;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class JsonMinimalMetadataTypeNameOracleTests
    {
        private readonly JsonMinimalMetadataTypeNameOracle testSubject = new JsonMinimalMetadataTypeNameOracle();
        private readonly ODataResource entryWithoutTypeName = new ODataResource();
        private readonly ODataResource entryWithTypeName = new ODataResource { TypeName = "TypeNameFromOM" };
        private readonly ODataResource entryWithTypeAnnotationAndTypeName = new ODataResource { TypeName = "TypeNameFromOM" };
        private readonly ODataResource entryWithTypeAnnotationWithoutTypeName = new ODataResource();

        private readonly EdmComplexTypeReference complexTypeReference = new EdmComplexTypeReference(new EdmComplexType("Namespace", "ComplexTypeName"), false);
        private const string ComplexTypeName = "Namespace.ComplexTypeName";
        private readonly IEdmSpatialTypeReference geographyTypeReference = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, false);
        private readonly IEdmSpatialTypeReference geographyPointTypeReference = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false);
        private readonly IEdmStringTypeReference stringTypeReference = EdmCoreModel.Instance.GetString(false);
        private readonly IEdmPrimitiveTypeReference int16TypeReference = EdmCoreModel.Instance.GetInt16(false);

        public JsonMinimalMetadataTypeNameOracleTests()
        {
            entryWithTypeAnnotationAndTypeName.TypeAnnotation = new ODataTypeAnnotation("TypeNameFromSTNA");
            entryWithTypeAnnotationWithoutTypeName.TypeAnnotation = new ODataTypeAnnotation("TypeNameFromSTNA");
        }

        #region Minimal metadata entry type name tests
        [Fact]
        public void WhenAnnotationIsSetTypeNameShouldAlwaysComeFromAnnotation()
        {
            Assert.Equal("TypeNameFromSTNA", this.testSubject.GetResourceTypeNameForWriting("TypeNameFromMetadata", this.entryWithTypeAnnotationAndTypeName));
            Assert.Equal("TypeNameFromSTNA", this.testSubject.GetResourceTypeNameForWriting(null, this.entryWithTypeAnnotationAndTypeName));
            Assert.Equal("TypeNameFromSTNA", this.testSubject.GetResourceTypeNameForWriting("TypeNameFromMetadata", this.entryWithTypeAnnotationWithoutTypeName));
            Assert.Equal("TypeNameFromSTNA", this.testSubject.GetResourceTypeNameForWriting(null, this.entryWithTypeAnnotationWithoutTypeName));
        }

        [Fact]
        public void WhenAnnotationIsNotSetTypeNameShouldAlwaysComeFromObjectModelWhenTypeNameFromObjectModelDoesNotMatchExpectedTypeName()
        {
            Assert.Equal("TypeNameFromOM", this.testSubject.GetResourceTypeNameForWriting("TypeNameFromMetadata", this.entryWithTypeName));
            Assert.Equal("TypeNameFromOM", this.testSubject.GetResourceTypeNameForWriting(null, this.entryWithTypeName));
            Assert.Null(this.testSubject.GetResourceTypeNameForWriting("TypeNameFromMetadata", this.entryWithoutTypeName));
        }

        [Fact]
        public void TypeNameShouldNotBeOmittedWhenTypeNameFromAnnotationMatchesExpectedTypeName()
        {
            Assert.Equal("TypeNameFromSTNA", this.testSubject.GetResourceTypeNameForWriting("TypeNameFromSTNA", this.entryWithTypeAnnotationAndTypeName));
            Assert.Equal("TypeNameFromSTNA", this.testSubject.GetResourceTypeNameForWriting("TypeNameFromSTNA", this.entryWithTypeAnnotationWithoutTypeName));
        }

        [Fact]
        public void TypeNameShouldBeOmittedWhenTypeNameFromObjectModelMatchesExpectedTypeName()
        {
            Assert.Null(this.testSubject.GetResourceTypeNameForWriting("TypeNameFromOM", this.entryWithTypeName));
        }
        #endregion Minimal metadata entry type name tests

        #region Minimal metadata value type name tests
        [Fact]
        public void ClosedComplexValueShouldReturnNull()
        {
            Assert.Null(this.testSubject.GetResourceTypeNameForWriting(
                this.complexTypeReference.FullName(),
                new ODataResource { TypeName = ComplexTypeName },
                /*isOpen*/ false));
        }

        [Fact]
        public void OpenComplexValueShouldReturnNull()
        {
            Assert.Equal(ComplexTypeName, this.testSubject.GetResourceTypeNameForWriting(
                null,
                new ODataResource { TypeName = ComplexTypeName },
                /*isOpen*/ false));
        }

        // Note: When writing derived complexType value in a payload, we don't have the expected type. 
        // So always write @odata.type for top-level derived complextype.
        [Fact]
        public void DerivedComplexValueShouldAlwaysReturnDerivedComplexTypename()
        {
            string DerivedComplexTypeName = "Namespace.DerivedComplexTypeName";
            Assert.Equal(DerivedComplexTypeName, this.testSubject.GetResourceTypeNameForWriting(
                this.complexTypeReference.FullName(),
                new ODataResource { TypeName = DerivedComplexTypeName },
                /*isOpen*/ false));
        }

        [Fact]
        public void TopLevelDerivedComplexValueShouldReturnDerivedComplexTypename()
        {
            string DerivedComplexTypeName = "Namespace.DerivedComplexTypeName";
            Assert.Equal(DerivedComplexTypeName, this.testSubject.GetResourceTypeNameForWriting(
                null,
                new ODataResource { TypeName = DerivedComplexTypeName },
                /*isOpen*/ false));
        }

        [Fact]
        public void DerivedPrimitiveValueShouldReturnNull()
        {
            Assert.Equal("Edm.GeographyPoint", this.testSubject.GetValueTypeNameForWriting(
                new ODataPrimitiveValue(GeographyPoint.Create(42, 42)),
                this.geographyTypeReference,
                this.geographyPointTypeReference,
                /*isOpen*/ false));
        }

        [Fact]
        public void RegularPrimitiveValueShouldReturnNull()
        {
            Assert.Null(this.testSubject.GetValueTypeNameForWriting(
                new ODataPrimitiveValue((Int16)42),
                this.int16TypeReference,
                this.int16TypeReference,
                /*isOpen*/ false));
        }

        [Fact]
        public void OpenPrimitiveValueShouldReturnTypeName()
        {
            Assert.Equal("Edm.Int16", this.testSubject.GetValueTypeNameForWriting(
                new ODataPrimitiveValue((Int16)42),
                this.int16TypeReference,
                this.int16TypeReference,
                /*isOpen*/ true));
        }

        [Fact]
        public void DeclaredJsonNativeValueShouldReturnNull()
        {
            Assert.Null(this.testSubject.GetValueTypeNameForWriting(
                new ODataPrimitiveValue("string value"),
                this.stringTypeReference,
                this.stringTypeReference,
                /*isOpen*/ false));
        }

        [Fact]
        public void OpenJsonNativeValueShouldReturnNull()
        {
            Assert.Null(this.testSubject.GetValueTypeNameForWriting(
                new ODataPrimitiveValue("string value"),
                this.stringTypeReference,
                this.stringTypeReference,
                /*isOpen*/ true));
        }

        [Fact]
        public void ValueWithTypeNameAnnotationShouldReturnTypeNameFromAnnotation()
        {
            var complexValue = new ODataResource() { TypeName = ComplexTypeName };
            complexValue.TypeAnnotation = new ODataTypeAnnotation("TypeNameFromSTNA");

            Assert.Equal("TypeNameFromSTNA", this.testSubject.GetResourceTypeNameForWriting(
                this.complexTypeReference.FullName(),
                complexValue,
                /*isOpen*/ false));
        }

        #endregion Minimal metadata value type name tests
    }
}
