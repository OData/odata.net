//---------------------------------------------------------------------
// <copyright file="ODataAtomAnnotationReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using FluentAssertions;
using Microsoft.OData.Core.Atom;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.Atom
{
    public class ODataAtomAnnotationReaderTests
    {
        private XmlReader xmlReader;
        private EdmModel model;
        private Func<string, bool> shouldIncludeAnnotation;

        public ODataAtomAnnotationReaderTests()
        {
            this.model = new EdmModel();
            this.model.AddElement(new EdmComplexType("foo", "complex"));
            EdmComplexType complexType = new EdmComplexType("ns", "complex");
            this.model.AddElement(complexType);
            EdmComplexTypeReference complexTypeReference = new EdmComplexTypeReference(complexType, isNullable: false);
            EdmCollectionType primitiveCollectionType = new EdmCollectionType(EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Guid, isNullable: false));
            EdmCollectionType complexCollectionType = new EdmCollectionType(complexTypeReference);
            EdmCollectionTypeReference primitiveCollectionTypeReference = new EdmCollectionTypeReference(primitiveCollectionType);
            EdmCollectionTypeReference complexCollectionTypeReference = new EdmCollectionTypeReference(complexCollectionType);

            this.model.AddElement(new EdmTerm("custom", "primitive", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, isNullable: false)));
            this.model.AddElement(new EdmTerm("custom", "complex", complexTypeReference));
            this.model.AddElement(new EdmTerm("custom", "primitiveCollection", primitiveCollectionTypeReference));
            this.model.AddElement(new EdmTerm("custom", "complexCollection", complexCollectionTypeReference));

            this.shouldIncludeAnnotation = (annotationName) => true;
        }

        #region Tests for basic annotation structure
        [Fact]
        public void AnnotationWithoutTermShouldThrow()
        {
            Action testSubject = () => ReadAnnotation("<m:annotation />");
            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.AtomInstanceAnnotation_MissingTermAttributeOnAnnotationElement);
        }

        [Fact]
        public void UnrecognizedAttributesShouldBeIgnored()
        {
            var testResult = ReadAnnotation("<m:annotation term=\"my.namespace.term\" string=\"value\" m:unrecognized=\"bla\" unrecognized=\"bla\" d:unrecognized=\"bla\" />");
            testResult.Value.As<ODataPrimitiveValue>().Value.Should().Be("value");
        }

        [Fact]
        public void TargetAttributeShouldBeReported()
        {
            var testResult = ReadAnnotation("<m:annotation term=\"my.namespace.term\" target=\"PropertyName\" string=\"value\" />");
            testResult.Target.Should().Be("PropertyName");
        }

        [Fact]
        public void AnnotationWithStringAttributeAndNonStringTypeShouldThrow()
        {
            Action testSubject = () => ReadAnnotation("<m:annotation term=\"my.namespace.term\" m:type=\"Edm.Int32\" string=\"value\" />");
            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.AtomInstanceAnnotation_AttributeValueNotationUsedWithIncompatibleType("Edm.Int32", "string"));
        }

        [Fact]
        public void AnnotationWithIntAttributeAndNonIntTypeShouldThrow()
        {
            Action testSubject = () => ReadAnnotation("<m:annotation term=\"my.namespace.term\" m:type=\"Edm.Double\" int=\"31\" />");
            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.AtomInstanceAnnotation_AttributeValueNotationUsedWithIncompatibleType("Edm.Double", "int"));
        }

        [Fact]
        public void AnnotationWithIntAndStringAttributesShouldThrow()
        {
            Action testSubject = () => ReadAnnotation("<m:annotation term=\"my.namespace.term\" int=\"42\" string=\"42\" />");
            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.AtomInstanceAnnotation_MultipleAttributeValueNotationAttributes);
        }

        [Fact]
        public void AnnotationWithStringAttributeAndStringTypeShouldNotThrow()
        {
            Action testSubject = () => ReadAnnotation("<m:annotation term=\"my.namespace.term\" m:type=\"Edm.String\" string=\"value\" />");
            testSubject.ShouldNotThrow();
        }

        [Fact]
        public void AnnotationWithStringAttributeAndElementContentShouldThrow()
        {
            // Note: even if the values specified in the attribute and in the element content are the same, we still throw (since otherwise we have to do a string equality check every time this appears).
            Action testSubject = () => ReadAnnotation("<m:annotation term=\"my.namespace.term\" string=\"value\">value</m:annotation>");
            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.AtomInstanceAnnotation_AttributeValueNotationUsedOnNonEmptyElement("string"));
        }
        #endregion Tests for basic annotation structure

        #region Tests to check that the XmlReader is moved passed the annotation element.
        [Fact]
        public void ReadAnnotationShouldAdvanceXmlReaderPastEmptyElement()
        {
            ReadAnnotation("<m:annotation term=\"my.namespace.term\" string=\"something\" /><other_element />");
            this.xmlReader.IsStartElement("other_element").Should().BeTrue("the xml reader should be positioned on the start tag of the next element in the fragment.");
        }

        [Fact]
        public void ReadAnnotationShouldAdvanceXmlReaderPastNonEmptyElement()
        {
            ReadAnnotation("<m:annotation term=\"my.namespace.term\">something</m:annotation><other_element />");
            this.xmlReader.IsStartElement("other_element").Should().BeTrue("the xml reader should be positioned on the start tag of the next element in the fragment.");
        }

        [Fact]
        public void ReadAnnotationShouldNotAdvanceXmlReaderPastSkippedAnnotationElement()
        {
            this.shouldIncludeAnnotation = (annotationName) => false;

            var reader = CreateODataAtomAnnotationReader("<m:annotation term=\"some.term.name\" /><nextElement />");
            AtomInstanceAnnotation annotation;
            reader.TryReadAnnotation(out annotation);

            // Because TryReadAnnotation returns false and does not actual read the annotation when the annotation is skipped,
            // the m:annotation element should not be consumed by the xml reader.

            this.xmlReader.NodeType.Should().Be(XmlNodeType.Element, "the xml reader should not have been left on an attribute (or any other type of) node.");
            this.xmlReader.Name.Should().Be("m:annotation", "the xml reader should not have advanced to the next element");
        }
        #endregion Tests to check that the XmlReader is moved passed the annotation element.

        #region Tests for m:null in annotation
        [Fact]
        public void IfNullAttributeIsPresentAndTrueAttributeValueShouldBeNull()
        {
            AtomInstanceAnnotation testResult = ReadAnnotation("<m:annotation term=\"my.namespace.term\" m:null=\"true\" />");
            testResult.Value.Should().BeOfType<ODataNullValue>();
        }

        [Fact]
        public void IfNullAttributeIsPresentAndTrueElementContentShouldBeIgnored()
        {
            // Note: we ignore element content (instead of failing) when m:null is true to be consistent with the decision 
            // made to have this behavior when reading properties.
            AtomInstanceAnnotation testResult = ReadAnnotation("<m:annotation term=\"my.namespace.term\" m:null=\"true\">Some content that doesn't make sense if the value is null</m:annotation>");
            testResult.Value.Should().BeOfType<ODataNullValue>();
        }

        [Fact]
        public void IfNullAttributeIsPresentAndTrueStringValueShouldBeIgnored()
        {
            // We ignore this (instead of fail) to be consistent with ignoring element content (see above).
            AtomInstanceAnnotation testResult = ReadAnnotation("<m:annotation term=\"my.namespace.term\" m:null=\"true\" string=\"something\" />");
            testResult.Value.Should().BeOfType<ODataNullValue>();
        }

        [Fact]
        public void IfNullAttributeIsPresentAndFalseStringValueShouldBeRead()
        {
            AtomInstanceAnnotation testResult = ReadAnnotation("<m:annotation term=\"my.namespace.term\" m:null=\"false\" string=\"something\" />");
            testResult.Value.Should().BeOfType<ODataPrimitiveValue>();
            testResult.Value.As<ODataPrimitiveValue>().Value.Should().Be("something");
        }
        #endregion Tests for m:null in annotation

        #region Tests for annotation whose value is primitive
        [Fact]
        public void ShouldReadStringValueFromAttribute()
        {
            ReadAnnotation("<m:annotation term=\"my.namespace.term\" string=\"value\" />").Value.As<ODataPrimitiveValue>().Value.Should().Be("value");
        }

        [Fact]
        public void ShouldReadIntValueFromAttribute()
        {
            ReadAnnotation("<m:annotation term=\"my.namespace.term\" int=\"42\" />").Value.As<ODataPrimitiveValue>().Value.Should().Be(42);
        }

        [Fact]
        public void ShouldReadDecimalValueFromAttribute()
        {
            ReadAnnotation("<m:annotation term=\"my.namespace.term\" decimal=\"42.2\" />").Value.As<ODataPrimitiveValue>().Value.Should().Be((Decimal)42.2);
        }

        [Fact]
        public void ShouldReadBoolValueFromAttribute()
        {
            ReadAnnotation("<m:annotation term=\"my.namespace.term\" bool=\"true\" />").Value.As<ODataPrimitiveValue>().Value.Should().Be(true);
        }

        [Fact]
        public void ShouldReadFloatValueFromAttribute()
        {
            ReadAnnotation("<m:annotation term=\"my.namespace.term\" float=\"1.25\" />").Value.As<ODataPrimitiveValue>().Value.Should().Be(1.25);
        }

        [Fact]
        public void ShouldThrowIfPrimitiveValueAttributeNotParseableAsThatType()
        {
            Action testSubject = () => ReadAnnotation("<m:annotation term=\"my.namespace.term\" int=\"42.5\" />");
            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ReaderValidationUtils_CannotConvertPrimitiveValue(42.5, "Edm.Int32"));
        }

        [Fact]
        public void ShouldThrowIfElementContentIsNotPrimitive()
        {
            // Comment: not the most intuitive error message if a user runs into this scenario.
            Action testSubject = () => ReadAnnotation("<m:annotation term=\"my.namespace.term\" m:type=\"Edm.Decimal\"><m:element>42</m:element></m:annotation>");
            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.XmlReaderExtension_InvalidNodeInStringValue("Element"));
        }

        [Fact]
        public void ShouldThrowIfElementContentIsNotPrimitiveAndNoTypeAvailable()
        {
            // Comment: not the most intuitive error message if a user runs into this scenario.
            AtomInstanceAnnotation tmp = ReadAnnotation("<m:annotation term=\"my.namespace.term\"><d:element>42</d:element></m:annotation>");
            tmp.TermName.Should().Be("my.namespace.term");
            tmp.Value.As<ODataComplexValue>().Properties.Select(s=>s.Value).Cast<string>().Single().Should().Be("42");
        }

        [Fact]
        public void ShouldReadStringValueFromElementContent()
        {
            ReadAnnotation("<m:annotation term=\"my.namespace.term\">value</m:annotation>").Value.As<ODataPrimitiveValue>().Value.Should().Be("value");
        }

        [Fact]
        public void ShouldReadIntValueFromElementContent()
        {
            ReadAnnotation("<m:annotation term=\"my.namespace.term\" m:type=\"Edm.Int32\">42</m:annotation>").Value.As<ODataPrimitiveValue>().Value.Should().Be(42);
        }

        [Fact]
        public void ShouldReadInt64ValueFromElementContent()
        {
            ReadAnnotation("<m:annotation term=\"my.namespace.term\" m:type=\"Edm.Int64\">42</m:annotation>").Value.As<ODataPrimitiveValue>().Value.Should().Be((Int64)42);
        }

        [Fact]
        public void ShouldThrowIfEmptyElementWithPrimitiveTypeSpecified()
        {
            Action testSubject = () => ReadAnnotation("<m:annotation term=\"my.namespace.term\" m:type=\"Edm.Decimal\" />");
            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ReaderValidationUtils_CannotConvertPrimitiveValue("", "Edm.Decimal"));
        }

        [Fact]
        public void ShouldIgnoreTypeAttributeIfNotInMetadataNamesapce()
        {
            ReadAnnotation("<m:annotation term=\"my.namespace.term\" type=\"Edm.Int32\">42</m:annotation>").Value.As<ODataPrimitiveValue>().Value.Should().Be("42");
        }

        [Fact]
        public void IfTermIsDefinedInMetadataThenPrimitiveValueShouldUseTypeFromTermMetadata()
        {
            this.model.AddElement(new EdmTerm("Namespace", "DefinedTerm", EdmPrimitiveTypeKind.Int16));

            this.ReadAnnotation("<m:annotation term=\"Namespace.DefinedTerm\">42</m:annotation>").Value.As<ODataPrimitiveValue>().Value.Should().BeOfType<Int16>();
        }
        #endregion Tests for annotation whose value is primitive

        #region Tests for annotation whose value is complex
        private AtomInstanceAnnotation ReadComplexValueAnnotation()
        {
            this.AddMyModelAddressComplexTypeToModel();

            return ReadAnnotation("<m:annotation term=\"my.namespace.term\" m:type=\"MyModel.Address\"><d:StreetNumber>123</d:StreetNumber><d:StreetName>Main St</d:StreetName></m:annotation>");
        }

        private IEdmComplexType AddMyModelAddressComplexTypeToModel()
        {
            var complexType = new EdmComplexType("MyModel", "Address");
            complexType.AddStructuralProperty("StreetNumber", EdmPrimitiveTypeKind.Int32);
            complexType.AddStructuralProperty("StreetName", EdmPrimitiveTypeKind.String);
            this.model.AddElement(complexType);

            return complexType;
        }

        [Fact]
        public void ComplexValueAnnotationShouldBeReportedAsComplexValue()
        {
            ReadComplexValueAnnotation().Value.Should().BeOfType<ODataComplexValue>();
        }

        [Fact]
        public void ComplexValueAnnotationShouldReportCorrectNumberOfProperties()
        {
            ReadComplexValueAnnotation().Value.As<ODataComplexValue>().Properties.Should().HaveCount(2);
        }

        [Fact]
        public void ComplexValueAnnotationShouldReportPropertyUsingTypeDeclaredInMetadata()
        {
            // Verify that the complex type was understood by checking that the street number was interpreted as an int.
            ReadComplexValueAnnotation().Value.As<ODataComplexValue>().Properties.Should().Contain(property => property.Name == "StreetNumber" && property.Value is int);
        }

        [Fact]
        public void ComplexValueAnnotationShouldReportTypeName()
        {
            ReadComplexValueAnnotation().Value.As<ODataComplexValue>().TypeName.Should().Be("MyModel.Address");
        }

        [Fact]
        public void ComplexValueAnnotationShouldNotHaveSerializationTypeNameAnnotation()
        {
            // Since the wire type name matches what's reporting in ODataComplexValue.TypeName, we don't add a STNA.
            var stna = ReadComplexValueAnnotation().Value.GetAnnotation<SerializationTypeNameAnnotation>();
            stna.Should().BeNull();
        }

        [Fact]
        public void AnnotationWithinComplexValuedAnnotationShouldBeIgnored()
        {
            // If we start reporting this in the future, don't fail if something's invalid. Otherwise it would be a breaking change. 
            // This applies equally to regular complex properties.
            this.AddMyModelAddressComplexTypeToModel();

            var complexValueAnnotation = this.ReadAnnotation(
                @"<m:annotation term=""my.namespace.term"" m:type=""#MyModel.Address"">
                    <d:StreetNumber>123</d:StreetNumber>
                    <d:StreetName>Main St</d:StreetName>
                    <m:annotation term=""my.namespace.innerterm"" string=""howdy"" />
                 </m:annotation>");

            complexValueAnnotation.Value.GetInstanceAnnotations().Should().BeEmpty();
        }

        [Fact]
        public void ComplexValueAnnotationWithNoElementContentShouldStillBeRead()
        {
            this.model.AddElement(new EdmComplexType("MyModel", "EmptyComplexType"));

            var complexValueAnnotation = this.ReadAnnotation(
                @"<m:annotation term=""my.namespace.term"" m:type=""#MyModel.EmptyComplexType"">
                 </m:annotation>");

            complexValueAnnotation.Value.As<ODataComplexValue>().Properties.Should().BeEmpty();
        }

        [Fact]
        public void EmptyComplexValueAnnotationWithShouldStillBeRead()
        {
            this.model.AddElement(new EdmComplexType("MyModel", "EmptyComplexType"));

            var complexValueAnnotation = this.ReadAnnotation(
                @"<m:annotation term=""my.namespace.term"" m:type=""#MyModel.EmptyComplexType"" />");

            complexValueAnnotation.Value.As<ODataComplexValue>().Properties.Should().BeEmpty();
        }

        [Fact]
        public void IfTermIsDefinedInMetadataThenComplexValueShouldUseTypeFromTermMetadata()
        {
            var addressType = this.AddMyModelAddressComplexTypeToModel();
            this.model.AddElement(new EdmTerm("Namespace", "DefinedTerm", new EdmComplexTypeReference(addressType, /*isNullable*/ false)));

            var complexValueAnnotation = this.ReadAnnotation(
                @"<m:annotation term=""Namespace.DefinedTerm"">
                    <d:StreetNumber>123</d:StreetNumber>
                    <d:StreetName>Main St</d:StreetName>
                 </m:annotation>");

            complexValueAnnotation.Value.Should().BeOfType<ODataComplexValue>();
            ODataComplexValue complexValue = (ODataComplexValue)complexValueAnnotation.Value;
            complexValue.TypeName.Should().Be("MyModel.Address");

            // Since the type name wasn't on the wire, there should be a serialization type name annotation indicating that.
            complexValue.GetAnnotation<SerializationTypeNameAnnotation>().Should().NotBeNull();
            complexValue.GetAnnotation<SerializationTypeNameAnnotation>().TypeName.Should().BeNull();

            // By checking that an int was read, we're verifying that the MyModel.Address type was correctly applied to the annotation. 
            // If the type wasn't correctly applied, the StreetNumber property would have been interpreted as a string.
            complexValue.Properties.Should().Contain((property) => property.Value is int);
        }

        [Fact]
        public void IfTermIsDefinedInMetadataAndWireTypeIsPresentThenIncompatibleTypesShouldThrow()
        {
            var addressType = this.AddMyModelAddressComplexTypeToModel();
            this.model.AddElement(new EdmTerm("Namespace", "DefinedTerm", new EdmComplexTypeReference(addressType, /*isNullable*/ false)));
            this.model.AddElement(new EdmComplexType("Other", "IncompatibleType"));

            // The expected type from the term definition in the model is "MyModel.Address", but the payload type is specified as "Other.IncompatibleType".
            Action testSubject = () => this.ReadAnnotation(
                @"<m:annotation term=""Namespace.DefinedTerm"" m:type=""#Other.IncompatibleType"">
                    <d:StreetNumber>123</d:StreetNumber>
                    <d:StreetName>Main St</d:StreetName>
                 </m:annotation>");

            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ValidationUtils_IncompatibleType("Other.IncompatibleType", "MyModel.Address"));
        }

        [Fact]
        public void IfTermIsDefinedInMetadataAndWireTypeIsPresentThenCompatibleTypesShouldNotThrow()
        {
            var addressType = this.AddMyModelAddressComplexTypeToModel();
            this.model.AddElement(new EdmTerm("Namespace", "DefinedTerm", new EdmComplexTypeReference(addressType, /*isNullable*/ false)));

            // The expected type from the term definition in the model is "MyModel.Address", and the payload type is redundantly specified as "MyModel.Address".
            var complexValueAnnotation = this.ReadAnnotation(
                @"<m:annotation term=""Namespace.DefinedTerm"" m:type=""#MyModel.Address"">
                    <d:StreetNumber>123</d:StreetNumber>
                    <d:StreetName>Main St</d:StreetName>
                 </m:annotation>");

            complexValueAnnotation.Should().NotBeNull();
            complexValueAnnotation.Value.Should().BeOfType<ODataComplexValue>();
        }

        #endregion Tests for annotation whose value is complex

        #region Tests for annotation whose value is a collection
        private AtomInstanceAnnotation ReadCollectionValueAnnotation()
        {
            return ReadAnnotation(
                @"<m:annotation term=""my.namespace.term"" m:type=""#Collection(Edm.Int64)"">
                    <m:element>123</m:element>
                    <m:element>42</m:element>
                  </m:annotation>");
        }

        [Fact]
        public void CollectionValueAnnotationShouldBeReportedAsCollectionValue()
        {
            ReadCollectionValueAnnotation().Value.Should().BeOfType<ODataCollectionValue>();
        }

        [Fact]
        public void CollectionValueAnnotationShouldReportCorrectNumberOfItems()
        {
            ReadCollectionValueAnnotation().Value.As<ODataCollectionValue>().Items.Should().HaveCount(2);
        }

        [Fact]
        public void ItemsInCollectionValueAnnotationShouldHaveItemsMatchingTypeDeclaredOnTheWire()
        {
            ReadCollectionValueAnnotation().Value.As<ODataCollectionValue>().Items.Should().ContainItemsAssignableTo<Int64>();
        }

        [Fact]
        public void CollectionValueAnnotationShouldReportTypeName()
        {
            ReadCollectionValueAnnotation().Value.As<ODataCollectionValue>().TypeName.Should().Be("Collection(Edm.Int64)");
        }

        [Fact]
        public void CollectionValueAnnotationShouldNotHaveSerializationTypeNameAnnotation()
        {
            // Since the wire type name matches what's reported in ODataCollectionValue.TypeName, we don't add a STNA.
            var stna = ReadCollectionValueAnnotation().Value.GetAnnotation<SerializationTypeNameAnnotation>();
            stna.Should().BeNull();
        }

        [Fact]
        public void CollectionValueAnnotationWithoutElementsShouldStillBeReported()
        {
            var collectionValueAnnotation = ReadAnnotation(
                @"<m:annotation term=""my.namespace.term"" m:type=""#Collection(Edm.Int64)""> </m:annotation>");

            collectionValueAnnotation.Value.Should().BeOfType<ODataCollectionValue>();
            collectionValueAnnotation.Value.As<ODataCollectionValue>().Items.Should().BeEmpty();
        }

        [Fact]
        public void EmptyCollectionValueAnnotationShouldStillBeReported()
        {
            var collectionValueAnnotation = ReadAnnotation(
                @"<m:annotation term=""my.namespace.term"" m:type=""#Collection(Edm.Int64)"" />");

            collectionValueAnnotation.Value.Should().BeOfType<ODataCollectionValue>();
            collectionValueAnnotation.Value.As<ODataCollectionValue>().Items.Should().BeEmpty();
        }

        [Fact]
        public void AnnotationWithinCollectionValuedAnnotationShouldBeIgnored()
        {
            // If we start reporting this in the future, don't fail if something's invalid. Otherwise it would be a breaking change. 
            // This applies equally to regular collection properties.
            var collectionValueAnnotation = this.ReadAnnotation(
                @"<m:annotation term=""my.namespace.term"" m:type=""#Collection(Edm.Int64)"">
                    <m:element>123</m:element>
                    <m:annotation term=""my.namespace.innerterm"" string=""howdy"" />
                 </m:annotation>");

            collectionValueAnnotation.Value.GetInstanceAnnotations().Should().BeEmpty();
        }

        #endregion Tests for annotation whose value is a collection

        #region Tests of instance annotation filter setting
        [Fact]
        public void NullInstanceAnnotationFilterShouldCompletelyIgnoreInstanceAnnotation()
        {
            this.shouldIncludeAnnotation = null;

            // Normally, this annotation should throw since its missing the required 'term' attribute. But don't throw if the user said to ignore all annotations.
            var reader = CreateODataAtomAnnotationReader("<m:annotation />");
            AtomInstanceAnnotation annotation;
            reader.TryReadAnnotation(out annotation).Should().BeFalse();
            annotation.Should().BeNull();
        }

        [Fact]
        public void ShouldStopReadingInstanceAnnotationIfUserDefinedFilterReturnsFalse()
        {
            this.shouldIncludeAnnotation = (annotationName) => false;

            // Normally, this annotation would normally throw since the "int" and "string" attributes can't both be present.
            // But don't throw here since the user said to ignore this annotation.
            var reader = CreateODataAtomAnnotationReader("<m:annotation int=\"45\" string=\"bla\" term=\"some.term.name\" />");
            AtomInstanceAnnotation annotation;
            reader.TryReadAnnotation(out annotation).Should().BeFalse();
            annotation.Should().BeNull();
        }

        [Fact]
        public void ShouldReadInstanceAnnotationIfUserDefinedFilterReturnsTrue()
        {
            this.shouldIncludeAnnotation = (annotationName) => true;

            var annotation = this.ReadAnnotation("<m:annotation term=\"some.term.name\" int=\"45\" />");
            annotation.Should().NotBeNull();
            annotation.TermName.Should().Be("some.term.name");
        }
        #endregion Tests of instance annotation filter setting

        #region Currently ignored tests
        [Fact(Skip = "This test currently fails.")]
        public void ShouldReadStringValueFromAttributeIfElementContentIsEmpty()
        {
            // This test currently fails.
            ReadAnnotation("<m:annotation term=\"my.namespace.term\" string=\"value\"></m:annotation>").Value.As<ODataPrimitiveValue>().Value.Should().Be("value");
        }

        [Fact(Skip = "Not yet implemented.")]
        public void ComplexValueAnnotationWithTypeNotInModelShouldStillBeRead()
        {
            // Not yet implemented : allow complex values with unrecognized types to be deserialized if they occur as the value of an annotation.
            var annotation = ReadAnnotation(
                "<m:annotation term=\"my.namespace.term\" m:type=\"#Unknown.ComplexType\"><d:StreetNumber m:type=\"Edm.Int32\">123</d:StreetNumber><d:StreetName>Main St</d:StreetName></m:annotation>");

            annotation.Value.Should().BeOfType<ODataComplexValue>();
            annotation.Value.As<ODataComplexValue>().Properties.Should().HaveCount(2);
        }

        [Fact(Skip = "Not yet implemented.")]
        public void ComplexValueAnnotationWithNoTypeShouldStillBeRead()
        {
            // Not yet implemented : allow complex values with no type to be deserialized if they occur as the value of an annotation.
            // Question: Can we even do this? How would we distinguish from collection values? 
            var annotation = ReadAnnotation(
                "<m:annotation term=\"my.namespace.term\"><d:StreetNumber m:type=\"Edm.Int32\">123</d:StreetNumber><d:StreetName>Main St</d:StreetName></m:annotation>");

            annotation.Value.Should().BeOfType<ODataComplexValue>();
            annotation.Value.As<ODataComplexValue>().Properties.Should().HaveCount(2);
        }
        #endregion Currently ignored tests

        #region Tests for annotation payload type different from expected type on the model
        [Fact]
        public void ReadNullAnnotationValueForNonNullableAnnotationShouldThrow()
        {
            Action test = () => ReadAnnotation("<m:annotation term=\"custom.primitive\" m:null=\"true\" />");
            test.ShouldThrow<ODataException>().WithMessage(Strings.ReaderValidationUtils_NullNamedValueForNonNullableType("custom.primitive", "Edm.Double"));
        }

        [Fact]
        public void ReadAttributeValueNotationPrimitiveAnnotationWithCompatibleModelTypeShouldConvertTypesCorrectly()
        {
            var annotation = ReadAnnotation("<m:annotation term=\"custom.primitive\" int=\"123\" />");
            annotation.Value.As<ODataPrimitiveValue>().Value.Should().BeOfType<double>();
        }

        [Fact]
        public void ReadAttributeValueNotationPrimitiveAnnotationWithMisMatchingModelTypeShouldThrow()
        {
            Action test = () => ReadAnnotation("<m:annotation term=\"custom.primitive\" decimal=\"123\" />");
            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatibleType("Edm.Decimal", "Edm.Double"));
        }

        [Fact]
        public void ReadNonAttributeValueNotationPrimitiveAnnotationWithCompatibleModelTypeShouldConvertTypesCorrectly()
        {
            var annotation = ReadAnnotation("<m:annotation term=\"custom.primitive\" m:type=\"Edm.Int32\">123</m:annotation>");
            annotation.Value.As<ODataPrimitiveValue>().Value.Should().BeOfType<double>();
        }

        [Fact]
        public void ReadNonAttributeValueNotationPrimitiveAnnotationWithMisMatchingModelTypeShouldThrow()
        {
            Action test = () => ReadAnnotation("<m:annotation term=\"custom.primitive\" m:type=\"Edm.Guid\">...</m:annotation>");
            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatibleType("Edm.Guid", "Edm.Double"));
        }

        [Fact]
        public void ReadComplexAnnotationWithMisMatchingModelTypeShouldThrow()
        {
            Action test = () => ReadAnnotation("<m:annotation term=\"custom.complex\" m:type=\"#foo.complex\"></m:annotation>");
            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatibleType("foo.complex", "ns.complex"));
        }

        [Fact]
        public void ReadComplexCollectionAnnotationWithMisMatchingModelTypeShouldThrow()
        {
            Action test = () => ReadAnnotation("<m:annotation term=\"custom.complexCollection\" m:type=\"#Collection(foo.complex)\"></m:annotation>");
            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatibleType("Collection(foo.complex)", "Collection(ns.complex)"));
        }

        [Fact]
        public void ReadPrimitiveCollectionAnnotationWithMisMatchingModelTypeShouldThrow()
        {
            Action test = () => ReadAnnotation("<m:annotation term=\"custom.primitiveCollection\" m:type=\"#Collection(Edm.String)\"></m:annotation>");
            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatibleType("Collection(Edm.String)", "Collection(Edm.Guid)"));
        }

        [Fact]
        public void LookupEdmTypeByAttributeValueNotationNameShouldReturnNonNullableTypeReferences()
        {
            AtomInstanceAnnotation.LookupEdmTypeByAttributeValueNotationName("int").IsNullable.Should().BeFalse();
            AtomInstanceAnnotation.LookupEdmTypeByAttributeValueNotationName("string").IsNullable.Should().BeFalse();
            AtomInstanceAnnotation.LookupEdmTypeByAttributeValueNotationName("float").IsNullable.Should().BeFalse();
            AtomInstanceAnnotation.LookupEdmTypeByAttributeValueNotationName("bool").IsNullable.Should().BeFalse();
            AtomInstanceAnnotation.LookupEdmTypeByAttributeValueNotationName("decimal").IsNullable.Should().BeFalse();
        }

        #endregion Tests for annotation payload type different from expected type on the model

        private AtomInstanceAnnotation ReadAnnotation(string annotationElementText)
        {
            var parser = this.CreateODataAtomAnnotationReader(annotationElementText);

            AtomInstanceAnnotation annotation;
            parser.TryReadAnnotation(out annotation).Should().BeTrue();
            return annotation;
        }

        private ODataAtomAnnotationReader CreateODataAtomAnnotationReader(string annotationElementText)
        {
            // Create a dummy root node wrapping the annotation element in order to define the namespace prefix mappings.
            string xmlText = "<dummy xmlns:m=\"" + AtomConstants.ODataMetadataNamespace + "\" xmlns:d=\"" + AtomConstants.ODataNamespace + "\" xmlns=\"" + AtomConstants.AtomNamespace + "\">" + annotationElementText + "</dummy>";

            var inputContext = new ODataAtomInputContext(
                ODataFormat.Atom,
                new MemoryStream(Encoding.UTF8.GetBytes(xmlText)),
                Encoding.UTF8,
                new ODataMessageReaderSettings { ShouldIncludeAnnotation = this.shouldIncludeAnnotation },
                true,
                true,
                this.model,
                null);
            this.xmlReader = inputContext.XmlReader;

            var parser = new ODataAtomAnnotationReader(inputContext, new ODataAtomPropertyAndValueDeserializer(inputContext));

            // Position the xml reader on the dummy element.
            this.xmlReader.Read();

            // Read over the start element of the dummy element (and position the reader on the m:annotation start tag).
            this.xmlReader.Read();
            return parser;
        }
    }
}
