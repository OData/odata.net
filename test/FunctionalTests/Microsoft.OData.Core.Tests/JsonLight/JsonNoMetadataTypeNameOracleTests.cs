//---------------------------------------------------------------------
// <copyright file="JsonNoMetadataTypeNameOracleTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Core.Tests.JsonLight
{
    public class JsonNoMetadataTypeNameOracleTests
    {
        private readonly JsonNoMetadataTypeNameOracle testSubject = new JsonNoMetadataTypeNameOracle();
        private readonly ODataEntry entryWithoutTypeName = new ODataEntry();
        private readonly ODataEntry entryWithTypeName = new ODataEntry {TypeName = "TypeNameFromOM"};
        private readonly ODataEntry entryWithTypeAnnotationAndTypeName = new ODataEntry {TypeName = "TypeNameFromOM"};
        private readonly ODataEntry entryWithTypeAnnotationWithoutTypeName = new ODataEntry();

        private readonly EdmComplexTypeReference complexTypeReference = new EdmComplexTypeReference(new EdmComplexType("Namespace", "ComplexTypeName"), false);
        private const string ComplexTypeName = "Namespace.ComplexTypeName";
        private readonly IEdmSpatialTypeReference geographyTypeReference = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, false);
        private readonly IEdmSpatialTypeReference geographyPointTypeReference = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false);
        private readonly IEdmStringTypeReference stringTypeReference = EdmCoreModel.Instance.GetString(false);
        private readonly IEdmPrimitiveTypeReference int16TypeReference = EdmCoreModel.Instance.GetInt16(false);

        public JsonNoMetadataTypeNameOracleTests()
        {
            entryWithTypeAnnotationAndTypeName.SetAnnotation(new SerializationTypeNameAnnotation{TypeName="TypeNameFromSTNA"});
            entryWithTypeAnnotationWithoutTypeName.SetAnnotation(new SerializationTypeNameAnnotation{TypeName="TypeNameFromSTNA"});
        }

        // Note: Type names should never be written in no-metadata mode. These tests ensure the type name is null for every possible combination of inputs.
        #region No metadata entry type name tests
        [Fact]
        public void TypeNameShouldBeNullEvenWhenAnnotationIsSet()
        {
            this.testSubject.GetEntryTypeNameForWriting("TypeNameFromMetadata", this.entryWithTypeAnnotationAndTypeName).Should().BeNull();
            this.testSubject.GetEntryTypeNameForWriting(null, this.entryWithTypeAnnotationAndTypeName).Should().BeNull();
            this.testSubject.GetEntryTypeNameForWriting("TypeNameFromMetadata", this.entryWithTypeAnnotationWithoutTypeName).Should().BeNull();
            this.testSubject.GetEntryTypeNameForWriting(null, this.entryWithTypeAnnotationWithoutTypeName).Should().BeNull();
        }

        [Fact]
        public void TypeNameShouldBeNullWhenAnnotationIsNotSetAndObjectModelHasTypeName()
        {
            this.testSubject.GetEntryTypeNameForWriting("TypeNameFromMetadata", this.entryWithTypeName).Should().BeNull();
            this.testSubject.GetEntryTypeNameForWriting(null, this.entryWithTypeName).Should().BeNull();
        }

        [Fact]
        public void TypeNameShouldBeNullWhenTypeNameFromAnnotationMatchesExpectedTypeName()
        {
            this.testSubject.GetEntryTypeNameForWriting("TypeNameFromSTNA", this.entryWithTypeAnnotationAndTypeName).Should().BeNull();
        }

        [Fact]
        public void TypeNameShouldBeNullWhenTypeNameFromObjectModelMatchesExpectedTypeName()
        {
            this.testSubject.GetEntryTypeNameForWriting("TypeNameFromOM", this.entryWithTypeName).Should().BeNull();
        }
        #endregion No metadata entry type name tests

        #region No metadata value type name tests
        [Fact]
        public void ClosedComplexValueShouldReturnNull()
        {
            this.testSubject.GetValueTypeNameForWriting(
                new ODataComplexValue {TypeName = ComplexTypeName}, 
                this.complexTypeReference, 
                this.complexTypeReference, 
                /*isOpen*/ false)
                .Should().BeNull();
        }   
        
        [Fact]
        public void OpenComplexValueShouldReturnNull()
        {
            this.testSubject.GetValueTypeNameForWriting(
                new ODataComplexValue {TypeName = ComplexTypeName}, 
                null, 
                this.complexTypeReference,
                /*isOpen*/ false)
                .Should().BeNull();
        }

        [Fact]
        public void DerivedComplexValueShouldReturnNull()
        {
            string DerivedComplexTypeName = "Namespace.DerivedComplexTypeName";
            this.testSubject.GetValueTypeNameForWriting(
                new ODataComplexValue { TypeName = DerivedComplexTypeName },
                this.complexTypeReference,
                new EdmComplexTypeReference(new EdmComplexType("Namespace", "DerivedComplexTypeName", this.complexTypeReference.ComplexDefinition(), false), false),
                /*isOpen*/ false)
                .Should().BeNull();
        }

        [Fact]
        public void DerivedPrimitiveValueShouldReturnNull()
        {
            this.testSubject.GetValueTypeNameForWriting(
                new ODataPrimitiveValue(GeographyPoint.Create(42, 42)),
                this.geographyTypeReference,
                this.geographyPointTypeReference,
                /*isOpen*/ false)
                .Should().BeNull();
        }

        [Fact]
        public void RegularPrimitiveValueShouldReturnNull()
        {
            this.testSubject.GetValueTypeNameForWriting(
                new ODataPrimitiveValue((Int16)42),
                this.int16TypeReference,
                this.int16TypeReference,
                /*isOpen*/ false)
                .Should().BeNull();
        }

        [Fact]
        public void OpenPrimitiveValueShouldReturnNull()
        {
            this.testSubject.GetValueTypeNameForWriting(
                new ODataPrimitiveValue((Int16)42),
                this.int16TypeReference,
                this.int16TypeReference,
                /*isOpen*/ true)
                .Should().BeNull();
        }

        [Fact]
        public void DeclaredJsonNativeValueShouldReturnNull()
        {
            this.testSubject.GetValueTypeNameForWriting(
                new ODataPrimitiveValue("string value"),
                this.stringTypeReference,
                this.stringTypeReference,
                /*isOpen*/ false)
                .Should().BeNull();
        }

        [Fact]
        public void OpenJsonNativeValueShouldReturnNull()
        {
            this.testSubject.GetValueTypeNameForWriting(
                new ODataPrimitiveValue("string value"),
                this.stringTypeReference,
                this.stringTypeReference,
                /*isOpen*/ true)
                .Should().BeNull();
        }

        [Fact]
        public void ValueWithTypeNameAnnotationShouldReturnNull()
        {
            var complexValue = new ODataComplexValue() {TypeName = ComplexTypeName};
            complexValue.SetAnnotation(new SerializationTypeNameAnnotation{TypeName = "TypeNameFromSTNA"});

            this.testSubject.GetValueTypeNameForWriting(
                complexValue,
                complexTypeReference,
                complexTypeReference,
                /*isOpen*/ false)
                .Should().BeNull();
        }

        #endregion No metadata value type name tests
    }
}
