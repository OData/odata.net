//---------------------------------------------------------------------
// <copyright file="JsonFullMetadataTypeNameOracleTests.cs" company="Microsoft">
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
    public class JsonFullMetadataTypeNameOracleTests
    {
        private readonly JsonFullMetadataTypeNameOracle testSubject = new JsonFullMetadataTypeNameOracle();
        private readonly ODataResource entryWithoutTypeName = new ODataResource();
        private readonly ODataResource entryWithTypeName = new ODataResource { TypeName = "TypeNameFromOM" };
        private readonly ODataResource entryWithTypeAnnotationAndTypeName = new ODataResource { TypeName = "TypeNameFromOM" };
        private readonly ODataResource entryWithTypeAnnotationWithoutTypeName = new ODataResource();

        private readonly ODataResourceSet resourceSetWithoutTypeName = new ODataResourceSet();
        private readonly ODataResourceSet resourceSetWithTypeName = new ODataResourceSet { TypeName = "TypeNameFromOM" };
        private readonly ODataResourceSet resourceSetWithTypeAnnotationAndTypeName = new ODataResourceSet { TypeName = "TypeNameFromOM" };
        private readonly ODataResourceSet resourceSetWithTypeAnnotationWithoutTypeName = new ODataResourceSet();

        private readonly EdmComplexTypeReference complexTypeReference = new EdmComplexTypeReference(new EdmComplexType("Namespace", "ComplexTypeName"), false);
        private const string ComplexTypeName = "Namespace.ComplexTypeName";
        private readonly IEdmSpatialTypeReference geographyTypeReference = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, false);
        private readonly IEdmSpatialTypeReference geographyPointTypeReference = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false);
        private readonly IEdmStringTypeReference stringTypeReference = EdmCoreModel.Instance.GetString(false);
        private readonly IEdmPrimitiveTypeReference int16TypeReference = EdmCoreModel.Instance.GetInt16(false);

        public JsonFullMetadataTypeNameOracleTests()
        {
            entryWithTypeAnnotationAndTypeName.TypeAnnotation = new ODataTypeAnnotation("TypeNameFromSTNA");
            entryWithTypeAnnotationWithoutTypeName.TypeAnnotation = new ODataTypeAnnotation("TypeNameFromSTNA");
            resourceSetWithTypeAnnotationAndTypeName.TypeAnnotation = new ODataTypeAnnotation("TypeNameFromSTNA");
            resourceSetWithTypeAnnotationWithoutTypeName.TypeAnnotation = new ODataTypeAnnotation("TypeNameFromSTNA");
        }

        #region Full metadata entity set type name tests
        [Fact]
        public void WhenAnnotationIsSetEntitySetTypeNameShouldAlwaysComeFromAnnotation()
        {
            Assert.Equal("TypeNameFromSTNA", this.testSubject.GetResourceSetTypeNameForWriting("TypeNameFromMetadata", this.resourceSetWithTypeAnnotationAndTypeName, false));
            Assert.Equal("TypeNameFromSTNA", this.testSubject.GetResourceSetTypeNameForWriting(null, this.resourceSetWithTypeAnnotationAndTypeName, false));
            Assert.Equal("TypeNameFromSTNA", this.testSubject.GetResourceSetTypeNameForWriting("TypeNameFromMetadata", this.resourceSetWithTypeAnnotationWithoutTypeName, false));
            Assert.Equal("TypeNameFromSTNA", this.testSubject.GetResourceSetTypeNameForWriting(null, this.resourceSetWithTypeAnnotationWithoutTypeName, false));
        }

        [Fact]
        public void WhenAnnotationIsNotSetEntitySetTypeNameShouldAlwaysComeFromObjectModel()
        {
            Assert.Equal("TypeNameFromOM", this.testSubject.GetResourceSetTypeNameForWriting("TypeNameFromMetadata", this.resourceSetWithTypeName, false));
            Assert.Equal("TypeNameFromOM", this.testSubject.GetResourceSetTypeNameForWriting(null, this.resourceSetWithTypeName, false));
            Assert.Null(this.testSubject.GetResourceSetTypeNameForWriting("TypeNameFromMetadata", this.resourceSetWithoutTypeName, false));
        }

        [Fact]
        public void TypeNameShouldNotBeOmittedWhenEntitySetTypeNameFromAnnotationMatchesExpectedTypeName()
        {
            Assert.Equal("TypeNameFromSTNA", this.testSubject.GetResourceSetTypeNameForWriting("TypeNameFromSTNA", this.resourceSetWithTypeAnnotationAndTypeName, false));
        }

        [Fact]
        public void TypeNameShouldNotBeOmittedWhenEntitySetTypeNameFromObjectModelMatchesExpectedTypeName()
        {
            Assert.Equal("TypeNameFromOM", this.testSubject.GetResourceSetTypeNameForWriting("TypeNameFromOM", this.resourceSetWithTypeName, false));
        }
        #endregion Full metadata entity set type name tests

        #region Full metadata entry type name tests
        [Fact]
        public void WhenAnnotationIsSetTypeNameShouldAlwaysComeFromAnnotation()
        {
            Assert.Equal("TypeNameFromSTNA", this.testSubject.GetResourceTypeNameForWriting("TypeNameFromMetadata", this.entryWithTypeAnnotationAndTypeName));
            Assert.Equal("TypeNameFromSTNA", this.testSubject.GetResourceTypeNameForWriting(null, this.entryWithTypeAnnotationAndTypeName));
            Assert.Equal("TypeNameFromSTNA", this.testSubject.GetResourceTypeNameForWriting("TypeNameFromMetadata", this.entryWithTypeAnnotationWithoutTypeName));
            Assert.Equal("TypeNameFromSTNA", this.testSubject.GetResourceTypeNameForWriting(null, this.entryWithTypeAnnotationWithoutTypeName));
        }

        [Fact]
        public void WhenAnnotationIsNotSetTypeNameShouldAlwaysComeFromObjectModel()
        {
            Assert.Equal("TypeNameFromOM", this.testSubject.GetResourceTypeNameForWriting("TypeNameFromMetadata", this.entryWithTypeName));
            Assert.Equal("TypeNameFromOM", this.testSubject.GetResourceTypeNameForWriting(null, this.entryWithTypeName));
            Assert.Null(this.testSubject.GetResourceTypeNameForWriting("TypeNameFromMetadata", this.entryWithoutTypeName));
        }

        [Fact]
        public void TypeNameShouldNotBeOmittedWhenTypeNameFromAnnotationMatchesExpectedTypeName()
        {
            Assert.Equal("TypeNameFromSTNA", this.testSubject.GetResourceTypeNameForWriting("TypeNameFromSTNA", this.entryWithTypeAnnotationAndTypeName));
        }

        [Fact]
        public void TypeNameShouldNotBeOmittedWhenTypeNameFromObjectModelMatchesExpectedTypeName()
        {
            Assert.Equal("TypeNameFromOM", this.testSubject.GetResourceTypeNameForWriting("TypeNameFromOM", this.entryWithTypeName));
        }
        #endregion Full metadata entry type name tests

        #region Full metadata value type name tests
        [Fact]
        public void ClosedComplexValueShouldReturnTypeName()
        {
            Assert.Equal(ComplexTypeName, this.testSubject.GetResourceTypeNameForWriting(
                this.complexTypeReference.FullName(),
                new ODataResource { TypeName = ComplexTypeName },
                /*isOpen*/ false));
        }

        [Fact]
        public void OpenComplexValueShouldReturnComplexTypename()
        {
            Assert.Equal(ComplexTypeName, this.testSubject.GetResourceTypeNameForWriting(
                null,
                new ODataResource { TypeName = ComplexTypeName },
                /*isOpen*/ false));
        }

        [Fact]
        public void DerivedComplexValueShouldReturnDerivedComplexTypename()
        {
            string DerivedComplexTypeName = "Namespace.DerivedComplexTypeName";
            Assert.Equal(DerivedComplexTypeName, this.testSubject.GetResourceTypeNameForWriting(
                this.complexTypeReference.FullName(),
                new ODataResource { TypeName = DerivedComplexTypeName },
                /*isOpen*/ false));
        }

        [Fact]
        public void DerivedPrimitiveValueShouldReturnDerivedTypeName()
        {
            Assert.Equal("Edm.GeographyPoint", this.testSubject.GetValueTypeNameForWriting(
                new ODataPrimitiveValue(GeographyPoint.Create(42, 42)),
                this.geographyTypeReference,
                this.geographyPointTypeReference,
                /*isOpen*/ false));
        }

        [Fact]
        public void RegularPrimitiveValueShouldReturnTypeName()
        {
            Assert.Equal("Edm.Int16", this.testSubject.GetValueTypeNameForWriting(
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
                complexTypeReference.FullName(),
                complexValue,
                /*isOpen*/ false));
        }
        #endregion Full metadata value type name tests
    }
}
