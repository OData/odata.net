//---------------------------------------------------------------------
// <copyright file="JsonNoMetadataTypeNameOracleTests.cs" company="Microsoft">
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
    public class JsonNoMetadataTypeNameOracleTests
    {
        private readonly JsonNoMetadataTypeNameOracle testSubject = new JsonNoMetadataTypeNameOracle();
        private readonly ODataResource entryWithoutTypeName = new ODataResource();
        private readonly ODataResource entryWithTypeName = new ODataResource {TypeName = "TypeNameFromOM"};
        private readonly ODataResource entryWithTypeAnnotationAndTypeName = new ODataResource {TypeName = "TypeNameFromOM"};
        private readonly ODataResource entryWithTypeAnnotationWithoutTypeName = new ODataResource();

        private readonly EdmComplexTypeReference complexTypeReference = new EdmComplexTypeReference(new EdmComplexType("Namespace", "ComplexTypeName"), false);
        private const string ComplexTypeName = "Namespace.ComplexTypeName";
        private readonly IEdmSpatialTypeReference geographyTypeReference = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, false);
        private readonly IEdmSpatialTypeReference geographyPointTypeReference = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false);
        private readonly IEdmStringTypeReference stringTypeReference = EdmCoreModel.Instance.GetString(false);
        private readonly IEdmPrimitiveTypeReference int16TypeReference = EdmCoreModel.Instance.GetInt16(false);

        public JsonNoMetadataTypeNameOracleTests()
        {
            entryWithTypeAnnotationAndTypeName.TypeAnnotation = new ODataTypeAnnotation("TypeNameFromSTNA");
            entryWithTypeAnnotationWithoutTypeName.TypeAnnotation = new ODataTypeAnnotation("TypeNameFromSTNA");
        }

        // Note: Type names should never be written in no-metadata mode. These tests ensure the type name is null for every possible combination of inputs.
        #region No metadata entry type name tests
        [Fact]
        public void TypeNameShouldBeNullEvenWhenAnnotationIsSet()
        {
            Assert.Null(this.testSubject.GetResourceTypeNameForWriting("TypeNameFromMetadata", this.entryWithTypeAnnotationAndTypeName));
            Assert.Null(this.testSubject.GetResourceTypeNameForWriting(null, this.entryWithTypeAnnotationAndTypeName));
            Assert.Null(this.testSubject.GetResourceTypeNameForWriting("TypeNameFromMetadata", this.entryWithTypeAnnotationWithoutTypeName));
            Assert.Null(this.testSubject.GetResourceTypeNameForWriting(null, this.entryWithTypeAnnotationWithoutTypeName));
        }

        [Fact]
        public void TypeNameShouldBeNullWhenAnnotationIsNotSetAndObjectModelHasTypeName()
        {
            Assert.Null(this.testSubject.GetResourceTypeNameForWriting("TypeNameFromMetadata", this.entryWithTypeName));
            Assert.Null(this.testSubject.GetResourceTypeNameForWriting(null, this.entryWithTypeName));
        }

        [Fact]
        public void TypeNameShouldBeNullWhenTypeNameFromAnnotationMatchesExpectedTypeName()
        {
            Assert.Null(this.testSubject.GetResourceTypeNameForWriting("TypeNameFromSTNA", this.entryWithTypeAnnotationAndTypeName));
        }

        [Fact]
        public void TypeNameShouldBeNullWhenTypeNameFromObjectModelMatchesExpectedTypeName()
        {
            Assert.Null(this.testSubject.GetResourceTypeNameForWriting("TypeNameFromOM", this.entryWithTypeName));
        }
        #endregion No metadata entry type name tests

        #region No metadata value type name tests
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
            Assert.Null(this.testSubject.GetResourceTypeNameForWriting(
                null,
                new ODataResource { TypeName = ComplexTypeName }, 
                /*isOpen*/ false));
        }

        [Fact]
        public void DerivedComplexValueShouldReturnNull()
        {
            string DerivedComplexTypeName = "Namespace.DerivedComplexTypeName";
            Assert.Null(this.testSubject.GetResourceTypeNameForWriting(
                this.complexTypeReference.FullName(),
                new ODataResource { TypeName = DerivedComplexTypeName },
                /*isOpen*/ false));
        }

        [Fact]
        public void DerivedPrimitiveValueShouldReturnNull()
        {
            Assert.Null(this.testSubject.GetValueTypeNameForWriting(
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
        public void OpenPrimitiveValueShouldReturnNull()
        {
            Assert.Null(this.testSubject.GetValueTypeNameForWriting(
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
        public void ValueWithTypeNameAnnotationShouldReturnNull()
        {
            var complexValue = new ODataResource() {TypeName = ComplexTypeName};
            complexValue.TypeAnnotation = new ODataTypeAnnotation("TypeNameFromSTNA");

            Assert.Null(this.testSubject.GetResourceTypeNameForWriting(
                complexTypeReference.FullName(),
                complexValue,
                /*isOpen*/ false));
        }

        #endregion No metadata value type name tests
    }
}
