//---------------------------------------------------------------------
// <copyright file="ODataAtomPropertyAndValueSerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.Atom;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.Atom
{
    public class ODataAtomPropertyAndValueSerializerTests
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");

        private Stream stream;
        private ODataMessageWriterSettings settings;
        private ODataAtomPropertyAndValueSerializer serializer;

        public ODataAtomPropertyAndValueSerializerTests()
        {
            EdmModel model = new EdmModel();

            EdmComplexType complex1 = new EdmComplexType("ns", "complex1");
            complex1.AddProperty(new EdmStructuralProperty(complex1, "p1", EdmCoreModel.Instance.GetInt32(isNullable: false)));
            model.AddElement(complex1);            
            
            EdmComplexType complex2 = new EdmComplexType("ns", "complex2");
            complex2.AddProperty(new EdmStructuralProperty(complex2, "p1", EdmCoreModel.Instance.GetInt32(isNullable: false)));
            model.AddElement(complex2);

            EdmComplexTypeReference complex2Reference = new EdmComplexTypeReference(complex2, isNullable: false);
            EdmCollectionType primitiveCollectionType = new EdmCollectionType(EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Int32, isNullable: false));
            EdmCollectionType complexCollectionType = new EdmCollectionType(complex2Reference);
            EdmCollectionTypeReference primitiveCollectionTypeReference = new EdmCollectionTypeReference(primitiveCollectionType);
            EdmCollectionTypeReference complexCollectionTypeReference = new EdmCollectionTypeReference(complexCollectionType);

            model.AddElement(new EdmTerm("custom", "int", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Int32, isNullable: false)));
            model.AddElement(new EdmTerm("custom", "string", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.String, isNullable: false)));
            model.AddElement(new EdmTerm("custom", "double", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, isNullable: false)));
            model.AddElement(new EdmTerm("custom", "bool", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Boolean, isNullable: true)));
            model.AddElement(new EdmTerm("custom", "decimal", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Decimal, isNullable: false)));
            model.AddElement(new EdmTerm("custom", "timespan", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Duration, isNullable: false)));
            model.AddElement(new EdmTerm("custom", "guid", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Guid, isNullable: false)));
            model.AddElement(new EdmTerm("custom", "complex", complex2Reference));
            model.AddElement(new EdmTerm("custom", "primitiveCollection", primitiveCollectionTypeReference));
            model.AddElement(new EdmTerm("custom", "complexCollection", complexCollectionTypeReference));

            this.stream = new MemoryStream();
            this.settings = new ODataMessageWriterSettings { Version = ODataVersion.V4, ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
            this.settings.SetServiceDocumentUri(ServiceDocumentUri);
            this.serializer = new ODataAtomPropertyAndValueSerializer(this.CreateAtomOutputContext(model, this.stream));
        }

        [Fact]
        public void WriteInstanceAnnotationShouldSkipAnnotationBaseOnAnnotationFilter()
        {
            this.settings = new ODataMessageWriterSettings {Version = ODataVersion.V4, ShouldIncludeAnnotation = name => false};
            this.serializer = new ODataAtomPropertyAndValueSerializer(this.CreateAtomOutputContext(EdmCoreModel.Instance, this.stream));

            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("Namespace.TermName", new ODataPrimitiveValue("string value")), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("");
        }

        #region Writing primitive without metadata
        [Fact]
        public void WritingAIntValuedAnnotationShouldUseAttributeValueNotation()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("Namespace.TermName", new ODataPrimitiveValue(123)), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"Namespace.TermName\" int=\"123\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\" />");
        }

        [Fact]
        public void WritingAStringValuedAnnotationShouldUseAttributeValueNotation()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("Namespace.TermName", new ODataPrimitiveValue("string value")), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"Namespace.TermName\" string=\"string value\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\" />");
        }

        [Fact]
        public void WritingAFloatValuedAnnotationShouldUseAttributeValueNotation()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("Namespace.TermName", new ODataPrimitiveValue(4.2)), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"Namespace.TermName\" float=\"4.2\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\" />");
        }

        [Fact]
        public void WritingABoolValuedAnnotationShouldUseAttributeValueNotation()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("Namespace.TermName", new ODataPrimitiveValue(true)), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"Namespace.TermName\" bool=\"true\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\" />");
        }

        [Fact]
        public void WritingADecimalValuedAnnotationShouldUseAttributeValueNotation()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("Namespace.TermName", new ODataPrimitiveValue((decimal)4.2)), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"Namespace.TermName\" decimal=\"4.2\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\" />");
        }

        [Fact]
        public void WritingATimespanValuedAnnotationShouldNotUseAttributeValueNotation()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("Namespace.TermName", new ODataPrimitiveValue(new TimeSpan(123456))), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"Namespace.TermName\" m:type=\"Duration\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">PT0.0123456S</annotation>");
        }

        [Fact]
        public void WritingAGuidValuedAnnotationShouldNotUseAttributeValueNotation()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("Namespace.TermName", new ODataPrimitiveValue(Guid.Empty)), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"Namespace.TermName\" m:type=\"Guid\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">00000000-0000-0000-0000-000000000000</annotation>");
        }
        #endregion Writing primitive without metadata

        #region Writing primitive with metadata
        [Fact]
        public void WritingAnAnnotationWithExpectedIntTypeShouldUseAttributeValueNotation()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("custom.int", new ODataPrimitiveValue(4)), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"custom.int\" int=\"4\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\" />");
        }

        [Fact]
        public void WritingAnAnnotationWithExpectedStringTypeShouldUseAttributeValueNotation()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("custom.string", new ODataPrimitiveValue("string value")), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"custom.string\" string=\"string value\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\" />");
        }

        [Fact]
        public void WritingAnAnnotationWithExpectedDoubleTypeShouldUseAttributeValueNotation()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("custom.double", new ODataPrimitiveValue(4.2)), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"custom.double\" float=\"4.2\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\" />");
        }

        [Fact]
        public void WritingAnAnnotationWithExpectedBoolTypeShouldUseAttributeValueNotation()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("custom.bool", new ODataPrimitiveValue(true)), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"custom.bool\" bool=\"true\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\" />");
        }

        [Fact]
        public void WritingAnAnnotationWithExpectedDecimalTypeShouldUseAttributeValueNotation()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("custom.decimal", new ODataPrimitiveValue((decimal)4.2)), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"custom.decimal\" decimal=\"4.2\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\" />");
        }

        [Fact]
        public void WritingAnAnnotationWithExpectedDurationTypeShouldNotUseAttributeValueNotation()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("custom.duration", new ODataPrimitiveValue(new TimeSpan(123456))), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"custom.duration\" m:type=\"Duration\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">PT0.0123456S</annotation>");
        }

        [Fact]
        public void WritingAnAnnotationWithExpectedGuidTypeShouldNotUseAttributeValueNotation()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("custom.guid", new ODataPrimitiveValue(Guid.Empty)), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"custom.guid\" m:type=\"Guid\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">00000000-0000-0000-0000-000000000000</annotation>");
        }
        #endregion Writing primitive with metadata

        #region Writing primitive with mismatched metadata
        [Fact]
        public void WritingAnAttributeValueNotationAnnotationWithMismatchedMetadataShouldThrow()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("custom.int", new ODataPrimitiveValue((decimal)4.2)), /*target*/ null);
            Action test = () => this.serializer.WriteInstanceAnnotation(annotation);
            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatiblePrimitiveItemType("Edm.Decimal", "False", "Edm.Int32", "False"));
        }

        [Fact]
        public void WritingANonAttributeValueNotationAnnotationWithMismatchedMetadataShouldThrow()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("custom.guid", new ODataPrimitiveValue(new TimeSpan(123456))), /*target*/ null);
            Action test = () => this.serializer.WriteInstanceAnnotation(annotation);
            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatiblePrimitiveItemType("Edm.Duration", "False", "Edm.Guid", "False"));
        }
        #endregion Writing primitive with mismatched metadata

        #region Writing annotations of non-primitive types without term in metadata
        [Fact]
        public void WritingAComplexValuedAnnotationWithoutMetadataShouldWork()
        {
            ODataComplexValue complex = new ODataComplexValue {Properties = new[] {new ODataProperty {Name = "p1", Value = 123}}, TypeName = "ns.complex1"};
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("Namespace.TermName", complex), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"Namespace.TermName\" m:type=\"#ns.complex1\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\"><d:p1 m:type=\"Int32\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\">123</d:p1></annotation>");
        }

        [Fact]
        public void WritingAComplexCollectionValuedAnnotationWithoutMetadataShouldWork()
        {
            ODataComplexValue complex = new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "p1", Value = 123 } } };
            ODataCollectionValue collection = new ODataCollectionValue { Items = new[] { complex }, TypeName = "Collection(ns.complex1)" };
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("Namespace.TermName", collection), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"Namespace.TermName\" m:type=\"#Collection(ns.complex1)\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\"><m:element><d:p1 m:type=\"Int32\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\">123</d:p1></m:element></annotation>");
        }

        [Fact]
        public void WritingAPrimitiveCollectionValuedAnnotationWithoutMetadataShouldWork()
        {
            ODataCollectionValue collection = new ODataCollectionValue { Items = new[] { 123 }};
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("Namespace.TermName", collection), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"Namespace.TermName\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\"><m:element m:type=\"Int32\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\">123</m:element></annotation>");
        }
        #endregion Writing annotations of non-primitive types without term in metadata

        #region Writing annotations of non-primitive types with term in metadata
        [Fact]
        public void WritingAComplexValuedAnnotationWithMetadataShouldWork()
        {
            ODataComplexValue complex = new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "p1", Value = 123 } }, TypeName = "ns.complex2" };
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("custom.complex", complex), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"custom.complex\" m:type=\"#ns.complex2\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\"><d:p1 m:type=\"Int32\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\">123</d:p1></annotation>");
        }

        [Fact]
        public void WritingAComplexCollectionValuedAnnotationWithMetadataShouldWork()
        {
            ODataComplexValue complex = new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "p1", Value = 123 } } };
            ODataCollectionValue collection = new ODataCollectionValue { Items = new[] { complex }, TypeName = "Collection(ns.complex2)" };
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("custom.complexCollection", collection), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"custom.complexCollection\" m:type=\"#Collection(ns.complex2)\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\"><m:element><d:p1 m:type=\"Int32\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\">123</d:p1></m:element></annotation>");
        }

        [Fact]
        public void WritingAPrimitiveCollectionValuedAnnotationWithMetadataShouldWork()
        {
            ODataCollectionValue collection = new ODataCollectionValue { Items = new[] { 123 } };
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("custom.primitiveCollection", collection), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"custom.primitiveCollection\" m:type=\"#Collection(Int32)\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\"><m:element>123</m:element></annotation>");
        }
        #endregion Writing annotations of non-primitive types with term in metadata

        #region Writing annotations of non-primitive types with mismatched metadata
        [Fact]
        public void WritingAComplexValuedAnnotationWithMismatchedMetadataShouldThrow()
        {
            ODataComplexValue complex = new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "p1", Value = 123 } }, TypeName = "ns.complex1" };
            Action test = () => this.serializer.WriteInstanceAnnotation(AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("custom.complex", complex), /*target*/ null));
            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatibleType("ns.complex1", "ns.complex2"));
        }

        [Fact]
        public void WritingAComplexCollectionValuedAnnotationWithMismatchedMetadataShouldThrow()
        {
            ODataComplexValue complex = new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "p1", Value = 123 } } };
            ODataCollectionValue collection = new ODataCollectionValue { Items = new[] { complex }, TypeName = "Collection(ns.complex1)" };
            Action test = () => this.serializer.WriteInstanceAnnotation(AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("custom.complexCollection", collection), /*target*/ null));
            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatibleType("Collection(ns.complex1)", "Collection(ns.complex2)"));
        }

        [Fact]
        public void WritingAPrimitiveCollectionValuedAnnotationWithMismatchedMetadataShouldThrow()
        {
            ODataCollectionValue collection = new ODataCollectionValue { Items = new[] { 123.4 } };
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("custom.primitiveCollection", collection), /*target*/ null);
            Action test = () => this.serializer.WriteInstanceAnnotation(annotation);
            test.ShouldThrow<ODataException>().WithMessage(Strings.CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName("Edm.Double", "Edm.Int32"));
        }
        #endregion Writing annotations of non-primitive types with mismatched metadata

        #region Writing null values
        [Fact]
        public void WritingANullValuedAnnotationThatIsNotInMetadataShouldWork()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("Namespace.TermName", new ODataNullValue()), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"Namespace.TermName\" m:null=\"true\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\" />");
        }
        
        [Fact]
        public void WritingANullValuedAnnotationThatIsOfNullableTypeInMetadataShouldWork()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("custom.bool", new ODataNullValue()), /*target*/ null);
            this.serializer.WriteInstanceAnnotation(annotation);
            this.ValidatePayload("<annotation term=\"custom.bool\" m:null=\"true\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\" />");
        }

        [Fact]
        public void WritingANullValuedAnnotationThatIsOfNonNullableTypeInMetadataShouldThrow()
        {
            var annotation = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("custom.int", new ODataNullValue()), /*target*/ null);
            Action test = () => this.serializer.WriteInstanceAnnotation(annotation);
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataAtomPropertyAndValueSerializer_NullValueNotAllowedForInstanceAnnotation("custom.int", "Edm.Int32"));
        }
        #endregion Writing null values

        #region Writing multiple instance annotations.
        [Fact]
        public void WriteInstanceAnnotationsShouldWriteAllAnnotationsInTheEnumerable()
        {
            var primitive = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.primitive", new ODataPrimitiveValue(123)), /*target*/ null);
            var complex = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.complex", new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "p1", Value = 123 } }, TypeName = "ns.complex1" }), /*target*/ null);
            var collection = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.collection", new ODataCollectionValue { Items = new[] { 123 } }), /*target*/ null);

            var instanceAnnotationWriteTracker = new InstanceAnnotationWriteTracker();
            this.serializer.XmlWriter.WriteStartElement("wrapper");
            this.serializer.WriteInstanceAnnotations(new[] {primitive, complex, collection}, instanceAnnotationWriteTracker);
            this.serializer.XmlWriter.WriteEndElement();
            this.ValidatePayload(
                "<wrapper>" +
                    "<annotation term=\"ns.primitive\" int=\"123\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\" />" +
                    "<annotation term=\"ns.complex\" m:type=\"#ns.complex1\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\"><d:p1 m:type=\"Int32\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\">123</d:p1></annotation>" +
                    "<annotation term=\"ns.collection\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\"><m:element m:type=\"Int32\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\">123</m:element></annotation>" +
                "</wrapper>");
            instanceAnnotationWriteTracker.IsAnnotationWritten("ns.primitive").Should().BeTrue();
            instanceAnnotationWriteTracker.IsAnnotationWritten("ns.complex").Should().BeTrue();
            instanceAnnotationWriteTracker.IsAnnotationWritten("ns.collection").Should().BeTrue();
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldThrowWhenThereAreDuplicatedTermNames()
        {
            var primitive = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.term", new ODataPrimitiveValue(123)), /*target*/ null);
            var complex = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.term", new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "p1", Value = 123 } }, TypeName = "ns.complex1" }), /*target*/ null);

            var instanceAnnotationWriteTracker = new InstanceAnnotationWriteTracker();
            instanceAnnotationWriteTracker.MarkAnnotationWritten("ns.term");
            Action test = () => this.serializer.WriteInstanceAnnotations(new[] { primitive, complex }, instanceAnnotationWriteTracker);
            test.ShouldThrow<ODataException>().WithMessage(Strings.JsonLightInstanceAnnotationWriter_DuplicateAnnotationNameInCollection("ns.term"));
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldTrackWhichAnnotationHasBeenWriten()
        {
            var primitive = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.primitive", new ODataPrimitiveValue(123)), /*target*/ null);
            var instanceAnnotationWriteTracker = new InstanceAnnotationWriteTracker();
            this.serializer.WriteInstanceAnnotations(new[] { primitive }, instanceAnnotationWriteTracker);
            instanceAnnotationWriteTracker.IsAnnotationWritten("ns.primitive").Should().BeTrue();
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldNotWriteAnnotationsThatHasAlreadyBeenWriten()
        {
            var primitive = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.primitive", new ODataPrimitiveValue(123)), /*target*/ null);
            var complex = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.complex", new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "p1", Value = 123 } }, TypeName = "ns.complex1" }), /*target*/ null);
            var collection = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.collection", new ODataCollectionValue { Items = new[] { 123 } }), /*target*/ null);

            var instanceAnnotationWriteTracker = new InstanceAnnotationWriteTracker();
            instanceAnnotationWriteTracker.MarkAnnotationWritten("ns.primitive");
            instanceAnnotationWriteTracker.MarkAnnotationWritten("ns.collection");
            this.serializer.WriteInstanceAnnotations(new[] { primitive, complex, collection }, instanceAnnotationWriteTracker);
            this.ValidatePayload("<annotation term=\"ns.complex\" m:type=\"#ns.complex1\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\"><d:p1 m:type=\"Int32\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\">123</d:p1></annotation>");

            instanceAnnotationWriteTracker.IsAnnotationWritten("ns.primitive").Should().BeTrue();
            instanceAnnotationWriteTracker.IsAnnotationWritten("ns.complex").Should().BeTrue();
            instanceAnnotationWriteTracker.IsAnnotationWritten("ns.collection").Should().BeTrue();
        }
        #endregion Writing multiple instance annotations.

        #region Writing Top Level Property

        [Fact(Skip = "This test currently fails.")]
        public void SerializeTopLevelPropertyOfNullShouldWork()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = new EdmEntityType("ns", "entityType", baseType: null, isAbstract: false, isOpen: false);
            entityType.AddStructuralProperty("primitivePropertyName", EdmPrimitiveTypeKind.String);
            model.AddElement(entityType);

            ODataProperty primitiveProperty = new ODataProperty { Name = "primitivePropertyName", Value = null };

            string val = SerializeProperty(model, primitiveProperty);
            val.Should().Be("<?xml version=\"1.0\" encoding=\"utf-8\"?><m:value xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\"  m:type=\"Edm.String\" m:null=\"true\"/>");
            
            // To do: add more unit test for null collection and null element.
        }

        [Fact]
        public void SerializeTopLevelPropertyOfPrimitiveTypeShouldWork()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = new EdmEntityType("ns", "entityType", baseType: null, isAbstract: false, isOpen: false);
            entityType.AddStructuralProperty("primitivePropertyName", EdmPrimitiveTypeKind.String);
            model.AddElement(entityType);

            string primitivePropertyValue = "stringValue";
            ODataProperty primitiveProperty = new ODataProperty { Name = "primitivePropertyName", Value = primitivePropertyValue };

            string val = SerializeProperty(model, primitiveProperty);
            val.Should().Be("<?xml version=\"1.0\" encoding=\"utf-8\"?><m:value xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://odata.org/$metadata#Edm.String\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\">stringValue</m:value>");
        }

        [Fact]
        public void SerializeTopLevelPropertyOfComplexTypeShouldWork()
        {
            EdmModel model = new EdmModel();

            EdmComplexType complexType = new EdmComplexType("ns", "complex");
            complexType.AddProperty(new EdmStructuralProperty(complexType, "propertyName1", EdmCoreModel.Instance.GetInt32(isNullable: false)));
            complexType.AddProperty(new EdmStructuralProperty(complexType, "propertyName2", EdmCoreModel.Instance.GetString(isNullable: false)));
            model.AddElement(complexType);

            EdmComplexTypeReference complexReference = new EdmComplexTypeReference(complexType, isNullable: false);

            EdmEntityType entityType = new EdmEntityType("ns", "entityType", baseType: null, isAbstract: false, isOpen: false);
            entityType.AddStructuralProperty("complexPropertyName", complexReference);
            model.AddElement(entityType);

            ODataComplexValue complexValue = new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "propertyName1", Value = 1 }, new ODataProperty {Name = "propertyName2", Value = "stringValue"} }, TypeName = "ns.complex" };
            ODataProperty complexProperty = new ODataProperty { Name = "complexPropertyName", Value = complexValue };

            string val = SerializeProperty(model, complexProperty);
            val.Should().Be("<?xml version=\"1.0\" encoding=\"utf-8\"?><m:value xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://odata.org/$metadata#ns.complex\" m:type=\"#ns.complex\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\"><d:propertyName1 m:type=\"Int32\">1</d:propertyName1><d:propertyName2>stringValue</d:propertyName2></m:value>");
        }

        [Fact]
        public void SerializeTopLevelPropertyOfCollectionTypeShouldWork()
        {
            EdmModel model = new EdmModel();

            EdmComplexType complexType = new EdmComplexType("ns", "complex");
            complexType.AddProperty(new EdmStructuralProperty(complexType, "propertyName1", EdmCoreModel.Instance.GetInt32(isNullable: false)));
            complexType.AddProperty(new EdmStructuralProperty(complexType, "propertyName2", EdmCoreModel.Instance.GetString(isNullable: false)));
            model.AddElement(complexType);

            ODataCollectionValue primitiveCollectionValue = new ODataCollectionValue { Items = new[] { "value1", "value2" }};
            primitiveCollectionValue.TypeName = "Collection(Edm.String)";
            ODataProperty primitiveCollectionProperty = new ODataProperty { Name = "PrimitiveCollectionProperty", Value = primitiveCollectionValue };

            string pval = SerializeProperty(model, primitiveCollectionProperty);
            pval.Should().Be("<?xml version=\"1.0\" encoding=\"utf-8\"?><m:value xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://odata.org/$metadata#Collection(Edm.String)\" m:type=\"#Collection(String)\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\"><m:element>value1</m:element><m:element>value2</m:element></m:value>");
            
            ODataComplexValue complexValue1 = new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "propertyName1", Value = 1 }, new ODataProperty { Name = "propertyName2", Value = "stringValue" } }, TypeName = "ns.complex" };
            ODataComplexValue complexValue2 = new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "propertyName1", Value = 1 }, new ODataProperty { Name = "propertyName2", Value = "stringValue" } }, TypeName = "ns.complex" };
            ODataCollectionValue complexCollectionValue = new ODataCollectionValue { Items = new[] { complexValue1, complexValue2 }, TypeName = "Collection(ns.complex)" };
            ODataProperty complexCollectionProperty = new ODataProperty { Name = "ComplexCollectionProperty", Value = complexCollectionValue };

            string cval = SerializeProperty(model, complexCollectionProperty);
            cval.Should().Be("<?xml version=\"1.0\" encoding=\"utf-8\"?><m:value xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://odata.org/$metadata#Collection(ns.complex)\" m:type=\"#Collection(ns.complex)\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\"><m:element><d:propertyName1 m:type=\"Int32\">1</d:propertyName1><d:propertyName2>stringValue</d:propertyName2></m:element><m:element><d:propertyName1 m:type=\"Int32\">1</d:propertyName1><d:propertyName2>stringValue</d:propertyName2></m:element></m:value>");
        }

        #endregion Writing Top Level Property

        #region Writing properties in open ComplexType
        [Fact]
        public void WritingDeclaredPropertyInOpenComplexTypeShouldWorkAtom()
        {
            EdmModel model = new EdmModel();

            EdmComplexType openAddressType = new EdmComplexType("ns", "OpenAddress", baseType: null, isAbstract: false, isOpen: true);
            openAddressType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String);
            model.AddElement(openAddressType);

            ODataProperty declaredPropertyCountryRegion = new ODataProperty() { Name = "CountryRegion", Value = "China" };

            this.SerializePropertyWithOwningType(model, openAddressType, declaredPropertyCountryRegion).Should().Be("<?xml version=\"1.0\" encoding=\"utf-8\"?><d:CountryRegion xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\">China</d:CountryRegion>");
        }

        [Fact]
        public void WritingDynamicPropertyInOpenComplexTypeShouldWorkAtom()
        {
            EdmModel model = new EdmModel();

            EdmComplexType openAddressType = new EdmComplexType("ns", "OpenAddress", baseType: null, isAbstract: false, isOpen: true);
            model.AddElement(openAddressType);

            ODataProperty undeclaredPropertyCity = new ODataProperty() { Name = "City", Value = "Shanghai" };

            this.SerializePropertyWithOwningType(model, openAddressType, undeclaredPropertyCity).Should().Be("<?xml version=\"1.0\" encoding=\"utf-8\"?><d:City xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\">Shanghai</d:City>");
        }
        #endregion Writing properties in open ComplexType

        private void ValidatePayload(string expectedPayload)
        {
            this.serializer.XmlWriter.Flush();
            this.stream.Position = 0;
            string payload = (new StreamReader(this.stream)).ReadToEnd();
            payload.Should().Be(expectedPayload == "" ? expectedPayload : "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + expectedPayload);
        }

        private string SerializeProperty(EdmModel model, ODataProperty odataProperty)
        {
            MemoryStream outputStream = new MemoryStream();
            ODataAtomOutputContext atomOutputContext = this.CreateAtomOutputContext(model, outputStream);
            var serializer = new ODataAtomPropertyAndValueSerializer(atomOutputContext);

            serializer.WriteTopLevelProperty(odataProperty);
            atomOutputContext.Flush();
            outputStream.Position = 0;
            string result = new StreamReader(outputStream).ReadToEnd();

            return result;
        }

        private string SerializePropertyWithOwningType(EdmModel model, IEdmStructuredType owningType, ODataProperty odataProperty)
        {
            MemoryStream outputStream = new MemoryStream();
            ODataAtomOutputContext atomOutputContext = this.CreateAtomOutputContext(model, outputStream);
            var serializer = new ODataAtomPropertyAndValueSerializer(atomOutputContext);

            serializer.WritePayloadStart();
            serializer.WriteProperties(
                owningType,
                new[] { odataProperty },
                /*isWritingCollection*/ false,
                null,
                null,
                new DuplicatePropertyNamesChecker(allowDuplicateProperties: true, isResponse: true),
                ProjectedPropertiesAnnotation.AllProjectedPropertiesInstance
                );
            serializer.WritePayloadEnd();

            atomOutputContext.Flush();
            outputStream.Position = 0;
            string result = new StreamReader(outputStream).ReadToEnd();

            return result;
        }

        private ODataAtomOutputContext CreateAtomOutputContext(IEdmModel model, Stream stream)
        {
            return new ODataAtomOutputContext(
                ODataFormat.Atom, 
                stream, 
                Encoding.UTF8, 
                this.settings, 
                /*writingResponse*/ true, 
                /*synchronous*/ true, 
                model, 
                /*urlResolver*/ null);
        }
    }
}
