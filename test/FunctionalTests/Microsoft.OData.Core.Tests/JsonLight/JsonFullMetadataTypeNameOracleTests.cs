//---------------------------------------------------------------------
// <copyright file="JsonFullMetadataTypeNameOracleTests.cs" company="Microsoft">
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
    public class JsonFullMetadataTypeNameOracleTests
    {
        private readonly JsonFullMetadataTypeNameOracle testSubject = new JsonFullMetadataTypeNameOracle();
        private readonly ODataEntry entryWithoutTypeName = new ODataEntry();
        private readonly ODataEntry entryWithTypeName = new ODataEntry { TypeName = "TypeNameFromOM" };
        private readonly ODataEntry entryWithTypeAnnotationAndTypeName = new ODataEntry { TypeName = "TypeNameFromOM" };
        private readonly ODataEntry entryWithTypeAnnotationWithoutTypeName = new ODataEntry();

        private readonly EdmComplexTypeReference complexTypeReference = new EdmComplexTypeReference(new EdmComplexType("Namespace", "ComplexTypeName"), false);
        private const string ComplexTypeName = "Namespace.ComplexTypeName";
        private readonly IEdmSpatialTypeReference geographyTypeReference = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, false);
        private readonly IEdmSpatialTypeReference geographyPointTypeReference = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false);
        private readonly IEdmStringTypeReference stringTypeReference = EdmCoreModel.Instance.GetString(false);
        private readonly IEdmPrimitiveTypeReference int16TypeReference = EdmCoreModel.Instance.GetInt16(false);

        public JsonFullMetadataTypeNameOracleTests()
        {
            entryWithTypeAnnotationAndTypeName.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = "TypeNameFromSTNA" });
            entryWithTypeAnnotationWithoutTypeName.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = "TypeNameFromSTNA" });
        }

        #region Full metadata entry type name tests
        [Fact]
        public void WhenAnnotationIsSetTypeNameShouldAlwaysComeFromAnnotation()
        {
            this.testSubject.GetEntryTypeNameForWriting("TypeNameFromMetadata", this.entryWithTypeAnnotationAndTypeName).Should().Be("TypeNameFromSTNA");
            this.testSubject.GetEntryTypeNameForWriting(null, this.entryWithTypeAnnotationAndTypeName).Should().Be("TypeNameFromSTNA");
            this.testSubject.GetEntryTypeNameForWriting("TypeNameFromMetadata", this.entryWithTypeAnnotationWithoutTypeName).Should().Be("TypeNameFromSTNA");
            this.testSubject.GetEntryTypeNameForWriting(null, this.entryWithTypeAnnotationWithoutTypeName).Should().Be("TypeNameFromSTNA");
        }

        [Fact]
        public void WhenAnnotationIsNotSetTypeNameShouldAlwaysComeFromObjectModel()
        {
            this.testSubject.GetEntryTypeNameForWriting("TypeNameFromMetadata", this.entryWithTypeName).Should().Be("TypeNameFromOM");
            this.testSubject.GetEntryTypeNameForWriting(null, this.entryWithTypeName).Should().Be("TypeNameFromOM");
            this.testSubject.GetEntryTypeNameForWriting("TypeNameFromMetadata", this.entryWithoutTypeName).Should().BeNull();
        }

        [Fact]
        public void TypeNameShouldNotBeOmittedWhenTypeNameFromAnnotationMatchesExpectedTypeName()
        {
            this.testSubject.GetEntryTypeNameForWriting("TypeNameFromSTNA", this.entryWithTypeAnnotationAndTypeName).Should().Be("TypeNameFromSTNA");
        }

        [Fact]
        public void TypeNameShouldNotBeOmittedWhenTypeNameFromObjectModelMatchesExpectedTypeName()
        {
            this.testSubject.GetEntryTypeNameForWriting("TypeNameFromOM", this.entryWithTypeName).Should().Be("TypeNameFromOM");
        }
        #endregion Full metadata entry type name tests

        #region Full metadata value type name tests
        [Fact]
        public void ClosedComplexValueShouldReturnTypeName()
        {
            this.testSubject.GetValueTypeNameForWriting(
                new ODataComplexValue { TypeName = ComplexTypeName },
                this.complexTypeReference,
                this.complexTypeReference,
                /*isOpen*/ false)
                .Should().Be(ComplexTypeName);
        }

        [Fact]
        public void OpenComplexValueShouldReturnComplexTypename()
        {
            this.testSubject.GetValueTypeNameForWriting(
                new ODataComplexValue { TypeName = ComplexTypeName },
                null,
                this.complexTypeReference,
                /*isOpen*/ false)
                .Should().Be(ComplexTypeName);
        }

        [Fact]
        public void DerivedComplexValueShouldReturnDerivedComplexTypename()
        {
            string DerivedComplexTypeName = "Namespace.DerivedComplexTypeName";
            this.testSubject.GetValueTypeNameForWriting(
                new ODataComplexValue { TypeName = DerivedComplexTypeName },
                this.complexTypeReference,
                new EdmComplexTypeReference(new EdmComplexType("Namespace", "DerivedComplexTypeName", this.complexTypeReference.ComplexDefinition(), false), false), 
                /*isOpen*/ false)
                .Should().Be(DerivedComplexTypeName);
        }

        [Fact]
        public void DerivedPrimitiveValueShouldReturnDerivedTypeName()
        {
            this.testSubject.GetValueTypeNameForWriting(
                new ODataPrimitiveValue(GeographyPoint.Create(42, 42)),
                this.geographyTypeReference,
                this.geographyPointTypeReference,
                /*isOpen*/ false)
                .Should().Be("Edm.GeographyPoint");
        }

        [Fact]
        public void RegularPrimitiveValueShouldReturnTypeName()
        {
            this.testSubject.GetValueTypeNameForWriting(
                new ODataPrimitiveValue((Int16) 42),
                this.int16TypeReference,
                this.int16TypeReference,
                /*isOpen*/ false)
                .Should().Be("Edm.Int16");
        }

        [Fact]
        public void OpenPrimitiveValueShouldReturnTypeName()
        {
            this.testSubject.GetValueTypeNameForWriting(
                new ODataPrimitiveValue((Int16)42),
                this.int16TypeReference,
                this.int16TypeReference,
                /*isOpen*/ true)
                .Should().Be("Edm.Int16");
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
        public void ValueWithTypeNameAnnotationShouldReturnTypeNameFromAnnotation()
        {
            var complexValue = new ODataComplexValue() { TypeName = ComplexTypeName };
            complexValue.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = "TypeNameFromSTNA" });

            this.testSubject.GetValueTypeNameForWriting(
                complexValue,
                complexTypeReference,
                complexTypeReference,
                /*isOpen*/ false)
                .Should().Be("TypeNameFromSTNA");
        }
        #endregion Full metadata value type name tests
    }
}
