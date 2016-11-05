//---------------------------------------------------------------------
// <copyright file="JsonLightInstanceAnnotationWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.Spatial;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.Json
{
    /// <summary>
    /// Unit tests for the JsonLightInstanceAnnotationWriter.
    /// 
    /// Uses mocks to test that the methods call the correct functions on JsonWriter and ODataJsonLightValueSerializer.
    /// </summary>
    public class JsonLightInstanceAnnotationWriterTests
    {
        private JsonLightInstanceAnnotationWriter jsonLightInstanceAnnotationWriter;
        private MockJsonWriter jsonWriter;
        private MockJsonLightValueSerializer valueWriter;
        private IEdmModel model;
        private EdmModel referencedModel;

        public JsonLightInstanceAnnotationWriterTests()
        {
            this.jsonWriter = new MockJsonWriter
            {
                WriteNameVerifier = name => { },
                WriteValueVerifier = str => { }
            };

            this.referencedModel = new EdmModel();
            model = TestUtils.WrapReferencedModelsToMainModel(referencedModel);

            // Version will be V3+ in production since it's JSON Light only
            this.valueWriter = new MockJsonLightValueSerializer(jsonWriter, model) { Settings = new ODataMessageWriterSettings { Version = ODataVersion.V4, ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") } };
            this.jsonLightInstanceAnnotationWriter = new JsonLightInstanceAnnotationWriter(this.valueWriter, new JsonMinimalMetadataTypeNameOracle());
        }

        [Fact]
        public void WriteInstanceAnnotation_ForIntegerShouldUsePrimitiveCodePath()
        {
            var integerValue = new ODataPrimitiveValue(123);
            const string term = "some.term";
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                name.Should().Be("@" + term);
                verifierCalls++;
            };
            this.valueWriter.WritePrimitiveVerifier = (value, reference) =>
            {
                value.Should().Be(integerValue.Value);
                reference.Should().BeNull();
                verifierCalls.Should().Be(1);
                verifierCalls++;
            };

            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, integerValue));
            verifierCalls.Should().Be(2);
        }

        [Fact]
        public void WriteInstanceAnnotation_ForDateShouldUsePrimitiveCodePathWithTypeName()
        {
            var date = new ODataPrimitiveValue(new Date(2014, 11, 11));
            const string term = "some.term";
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                if (verifierCalls == 0)
                {
                    name.Should().Be(term + "@odata.type");
                    verifierCalls++;
                }
                else if (verifierCalls == 2)
                {
                    name.Should().Be("@" + term);
                    verifierCalls++;
                }
                else throw new Exception("unexpected call to JsonWriter.WriteName");
            };
            this.jsonWriter.WriteValueVerifier = (value) =>
            {
                verifierCalls.Should().Be(1);
                verifierCalls++;
            };
            this.valueWriter.WritePrimitiveVerifier = (value, reference) =>
            {
                value.Should().Be(date.Value);
                reference.Should().BeNull();
                verifierCalls.Should().Be(3);
                verifierCalls++;
            };

            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, date));
            verifierCalls.Should().Be(4);
        }

        [Fact]
        public void WriteInstanceAnnotation_ForDateTimeOffsetShouldUsePrimitiveCodePathWithTypeName()
        {
            var dateTime = new ODataPrimitiveValue(new DateTimeOffset(2012, 9, 5, 10, 27, 34, TimeSpan.Zero));
            const string term = "some.term";
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                if (verifierCalls == 0)
                {
                    name.Should().Be(term + "@odata.type");
                    verifierCalls++;
                }
                else if (verifierCalls == 2)
                {
                    name.Should().Be("@" + term);
                    verifierCalls++;
                }
                else throw new Exception("unexpected call to JsonWriter.WriteName");
            };
            this.jsonWriter.WriteValueVerifier = (value) =>
            {
                verifierCalls.Should().Be(1);
                verifierCalls++;
            };
            this.valueWriter.WritePrimitiveVerifier = (value, reference) =>
            {
                value.Should().Be(dateTime.Value);
                reference.Should().BeNull();
                verifierCalls.Should().Be(3);
                verifierCalls++;
            };

            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, dateTime));
            verifierCalls.Should().Be(4);
        }

        [Fact]
        public void WriteInstanceAnnotation_ForTimeOfDayShouldUsePrimitiveCodePathWithTypeName()
        {
            var time = new ODataPrimitiveValue(new TimeOfDay(12, 5, 0, 90));
            const string term = "some.term";
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                if (verifierCalls == 0)
                {
                    name.Should().Be(term + "@odata.type");
                    verifierCalls++;
                }
                else if (verifierCalls == 2)
                {
                    name.Should().Be("@" + term);
                    verifierCalls++;
                }
                else throw new Exception("unexpected call to JsonWriter.WriteName");
            };
            this.jsonWriter.WriteValueVerifier = (value) =>
            {
                verifierCalls.Should().Be(1);
                verifierCalls++;
            };
            this.valueWriter.WritePrimitiveVerifier = (value, reference) =>
            {
                value.Should().Be(time.Value);
                reference.Should().BeNull();
                verifierCalls.Should().Be(3);
                verifierCalls++;
            };

            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, time));
            verifierCalls.Should().Be(4);
        }



        [Fact]
        public void WriteInstanceAnnotation_ForSpatialShouldUsePrimitiveCodePathWithTypeName()
        {
            var point = new ODataPrimitiveValue(GeographyPoint.Create(10.5, 5.25));
            const string term = "some.term";
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                if (verifierCalls == 0)
                {
                    name.Should().Be(term + "@odata.type");
                    verifierCalls++;
                }
                else if (verifierCalls == 2)
                {
                    name.Should().Be("@" + term);
                    verifierCalls++;
                }
                else throw new Exception("unexpected call to JsonWriter.WriteName");
            };
            this.jsonWriter.WriteValueVerifier = (value) =>
            {
                verifierCalls.Should().Be(1);
                verifierCalls++;
            };
            this.valueWriter.WritePrimitiveVerifier = (value, reference) =>
            {
                value.Should().Be(point.Value);
                reference.Should().BeNull();
                verifierCalls.Should().Be(3);
                verifierCalls++;
            };

            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, point));
            verifierCalls.Should().Be(4);
        }

        [Fact]
        public void WriteInstanceAnnotation_ForComplexShouldUseComplexCodePath()
        {
            var complexValue = new ODataComplexValue();
            const string term = "some.term";
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                name.Should().Be("@" + term);
                verifierCalls++;
            };
            this.valueWriter.WriteComplexVerifier = (value, reference, isTopLevel, isOpenProperty, dupChecker) =>
            {
                value.Should().Be(complexValue);
                reference.Should().BeNull();
                isTopLevel.Should().BeFalse();
                isOpenProperty.Should().BeTrue();
                verifierCalls.Should().Be(1);
                verifierCalls++;
            };
            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, complexValue));
            verifierCalls.Should().Be(2);
        }

        [Fact]
        public void WriteInstanceAnnotation_ForCollectionShouldUseCollectionCodePath()
        {
            var collectionValue = new ODataCollectionValue() { TypeName = "Collection(String)" };
            collectionValue.SetAnnotation(new SerializationTypeNameAnnotation() { TypeName = null });
            const string term = "some.term";
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                name.Should().Be("@" + term);
                verifierCalls++;
            };
            this.valueWriter.WriteCollectionVerifier = (value, reference, valueTypeReference, isTopLevelProperty, isInUri, isOpenProperty) =>
            {
                value.Should().Be(collectionValue);
                reference.Should().BeNull();
                valueTypeReference.Should().NotBeNull();
                valueTypeReference.IsCollection().Should().BeTrue();
                valueTypeReference.AsCollection().ElementType().IsString().Should().BeTrue();
                isOpenProperty.Should().BeTrue();
                isTopLevelProperty.Should().BeFalse();
                isInUri.Should().BeFalse();
                verifierCalls.Should().Be(1);
                verifierCalls++;
            };
            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, collectionValue));
            verifierCalls.Should().Be(2);
        }

        [Fact]
        public void WriteInstanceAnnotation_ForNullValue()
        {
            const string term = "some.term";
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                name.Should().Be("@" + term);
                verifierCalls++;
            };
            this.valueWriter.WriteNullVerifier = () =>
            {
                verifierCalls.Should().Be(1);
                verifierCalls++;
            };
            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, new ODataNullValue()));
            verifierCalls.Should().Be(2);
        }

        [Fact]
        public void WriteInstanceAnnotationWithNullValueShouldPassIfTheTermIsNullableInTheModel()
        {
            // Add a value term of type Collection(Edm.String) to the model.
            this.referencedModel.AddElement(new EdmTerm(
                "My.Namespace",
                "Nullable",
                EdmCoreModel.Instance.GetInt32(isNullable: true)));

            var verifierCalls = 0;

            const string term = "My.Namespace.Nullable";
            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                name.Should().Be("@" + term);
                verifierCalls++;
            };
            this.valueWriter.WriteNullVerifier = () =>
            {
                verifierCalls.Should().Be(1);
                verifierCalls++;
            };
            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, new ODataNullValue()));
            verifierCalls.Should().Be(2);
        }

        [Fact]
        public void WriteInstanceAnnotationWithNullValueShouldThrowIfTheTermIsNotNullableInTheModel()
        {
            // Add a value term of type Collection(Edm.String) to the model.
            this.referencedModel.AddElement(new EdmTerm(
                "My.Namespace",
                "NotNullable",
                EdmCoreModel.Instance.GetInt32(isNullable: false)));

            string term = "My.Namespace.NotNullable";
            Action action = () => this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, new ODataNullValue()));
            action.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataAtomPropertyAndValueSerializer_NullValueNotAllowedForInstanceAnnotation(term, "Edm.Int32"));
        }

        [Fact]
        public void WriteInstanceAnnotations_EmptyDoesNothing()
        {
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                verifierCalls++;
            };
            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotations(Enumerable.Empty<ODataInstanceAnnotation>());
            verifierCalls.Should().Be(0);
        }

        [Fact]
        public void WriteInstanceAnnotation_AllAnnotationsGetWritten()
        {
            var annotations = new Collection<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            annotations.Add(new ODataInstanceAnnotation("term.two", new ODataPrimitiveValue("456")));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;

            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotations(annotations);
            verifierCalls.Should().Be(4);
        }

        [Fact]
        public void WriteInstanceAnnotations_AnnotationsCannotBeWrittenTwice()
        {
            var annotations = new List<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            annotations.Add(new ODataInstanceAnnotation("term.two", new ODataPrimitiveValue("456")));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;
            InstanceAnnotationWriteTracker tracker = new InstanceAnnotationWriteTracker();

            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotations(annotations, tracker);
            verifierCalls.Should().Be(4);

            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotations(annotations, tracker);
            verifierCalls.Should().Be(4);

            tracker.IsAnnotationWritten("term.one").Should().BeTrue();
            tracker.IsAnnotationWritten("term.two").Should().BeTrue();
        }

        [Fact]
        public void WriteInstanceAnnotationCollection_NewAnnotationsGetWritten()
        {
            var annotations = new List<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;
            InstanceAnnotationWriteTracker tracker = new InstanceAnnotationWriteTracker();

            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotations(annotations, tracker);
            verifierCalls.Should().Be(2);

            annotations.Add(new ODataInstanceAnnotation("term.two", new ODataPrimitiveValue("456")));
            tracker.IsAnnotationWritten("term.two").Should().BeFalse();

            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotations(annotations, tracker);
            verifierCalls.Should().Be(4);

            tracker.IsAnnotationWritten("term.two").Should().BeTrue();
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldThrowOnDuplicatedAnnotationNames()
        {
            var annotations = new List<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(789)));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;

            Action test = () => this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotations(annotations);
            test.ShouldThrow<ODataException>().WithMessage(Strings.JsonLightInstanceAnnotationWriter_DuplicateAnnotationNameInCollection("term.one"));
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldNotThrowOnNamesWithDifferentCasing()
        {
            var annotations = new List<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            annotations.Add(new ODataInstanceAnnotation("term.One", new ODataPrimitiveValue(456)));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;

            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotations(annotations);
            verifierCalls.Should().Be(4);
        }

        [Fact]
        public void WriteInstanceAnnotationsWithTrackerShouldThrowOnDuplicatedAnnotationNames()
        {
            var annotations = new List<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;

            InstanceAnnotationWriteTracker tracker = new InstanceAnnotationWriteTracker();
            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotations(annotations, tracker);
            verifierCalls.Should().Be(2);
            tracker.IsAnnotationWritten("term.one").Should().BeTrue();

            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(456)));
            Action test = () => this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotations(annotations, tracker);
            test.ShouldThrow<ODataException>().WithMessage(Strings.JsonLightInstanceAnnotationWriter_DuplicateAnnotationNameInCollection("term.one"));
        }

        [Fact]
        public void WriteInstanceAnnotationsWithTrackerShouldNotThrowOnNamesWithDifferentCasing()
        {
            var annotations = new List<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;

            InstanceAnnotationWriteTracker tracker = new InstanceAnnotationWriteTracker();
            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotations(annotations, tracker);
            verifierCalls.Should().Be(2);
            tracker.IsAnnotationWritten("term.one").Should().BeTrue();

            annotations.Add(new ODataInstanceAnnotation("term.One", new ODataPrimitiveValue(456)));
            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotations(annotations, tracker);
            verifierCalls.Should().Be(4);
            tracker.IsAnnotationWritten("term.one").Should().BeTrue();
            tracker.IsAnnotationWritten("term.One").Should().BeTrue();
        }

        [Fact]
        public void WriteInstanceAnnotationShouldPassPrimitiveTypeFromModelToUnderlyingWriter()
        {
            // Add a value term of type DateTimeOffset to the model.
            this.referencedModel.AddElement(new EdmTerm("My.Namespace", "DateTimeTerm", EdmPrimitiveTypeKind.DateTimeOffset));
            var instanceAnnotation = new ODataInstanceAnnotation("My.Namespace.DateTimeTerm", new ODataPrimitiveValue(DateTimeOffset.MinValue));

            bool calledWritePrimitive = false;

            this.valueWriter.WritePrimitiveVerifier = (o, reference) =>
            {
                reference.Should().NotBeNull();
                reference.IsDateTimeOffset().Should().BeTrue();
                calledWritePrimitive = true;
            };

            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotation(instanceAnnotation);
            calledWritePrimitive.Should().BeTrue();
        }

        [Fact]
        public void WriteInstanceAnnotationShouldWriteValueTypeIfMoreDerivedThanMetadataType()
        {
            // Add a value term of type Geography to the model.
            this.referencedModel.AddElement(new EdmTerm("My.Namespace", "GeographyTerm", EdmPrimitiveTypeKind.Geography));
            var instanceAnnotation = new ODataInstanceAnnotation("My.Namespace.GeographyTerm", new ODataPrimitiveValue(GeographyPoint.Create(0.0, 0.0)));

            bool writingTypeName = false;
            bool wroteTypeName = false;
            this.jsonWriter.WriteNameVerifier = s =>
            {
                writingTypeName = s.EndsWith("odata.type");
            };

            this.jsonWriter.WriteValueVerifier = s =>
            {
                if (writingTypeName)
                {
                    s.Should().Be("#GeographyPoint");
                    wroteTypeName = true;
                }
            };

            this.valueWriter.WritePrimitiveVerifier = (o, reference) => { };

            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotation(instanceAnnotation);

            wroteTypeName.Should().BeTrue();
        }

        [Fact]
        public void WriteInstanceAnnotationShouldPassComplexTypeFromModelToUnderlyingWriter()
        {
            // Add a value term of a complex type to the model.
            var complexTypeReference = new EdmComplexTypeReference(new EdmComplexType("My.Namespace", "ComplexType"), false /*isNullable*/);
            this.referencedModel.AddElement(new EdmTerm("My.Namespace", "StructuredTerm", complexTypeReference));
            var instanceAnnotation = new ODataInstanceAnnotation("My.Namespace.StructuredTerm", new ODataComplexValue { TypeName = "ComplexType" });

            bool calledWriteComplex = false;

            this.valueWriter.WriteComplexVerifier = (complexValue, typeReference, isTopLevel, isOpenProperty, dupChecker) =>
            {
                typeReference.Should().NotBeNull();
                typeReference.IsComplex().Should().BeTrue();
                typeReference.AsComplex().FullName().Should().Be("My.Namespace.ComplexType");
                calledWriteComplex = true;
            };

            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotation(instanceAnnotation);
            calledWriteComplex.Should().BeTrue();
        }

        [Fact]
        public void WriteInstanceAnnotationShouldPassCollectionTypeFromModelToUnderlyingWriter()
        {
            // Add a value term of type Collection(Edm.String) to the model.
            this.referencedModel.AddElement(new EdmTerm(
                "My.Namespace",
                "CollectionTerm",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false)))));

            var instanceAnnotation = new ODataInstanceAnnotation("My.Namespace.CollectionTerm", new ODataCollectionValue() { TypeName = "Collection(Edm.String)" });

            bool calledWriteCollection = false;

            this.valueWriter.WriteCollectionVerifier = (collectionValue, typeReference, valueTypeReference, isTopLevel, isOpenProperty, dupChecker) =>
            {
                typeReference.Should().NotBeNull();
                typeReference.IsCollection().Should().BeTrue();
                typeReference.AsCollection().ElementType().IsString().Should().BeTrue();
                valueTypeReference.Should().NotBeNull();
                valueTypeReference.IsCollection().Should().BeTrue();
                valueTypeReference.AsCollection().ElementType().IsString().Should().BeTrue();
                calledWriteCollection = true;
            };

            var result = WriteInstanceAnnotation(instanceAnnotation, this.referencedModel);
            result.Should().Contain("{\"@My.Namespace.CollectionTerm\":[]}");
            result.Should().NotContain("odata.type");

            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotation(instanceAnnotation);
            calledWriteCollection.Should().BeTrue();
        }

        #region type name short-span integration tests
        [Fact]
        public void WritingPrimitiveAnnotationWithNonJsonNativeTypeShouldIncludeTypeName()
        {
            var result = WriteInstanceAnnotation(
                new ODataInstanceAnnotation("custom.namespace.MyDateTimeOffsetTerm", new ODataPrimitiveValue(DateTimeOffset.MinValue)),
                EdmCoreModel.Instance);

            result.Should().Contain("\"custom.namespace.MyDateTimeOffsetTerm@odata.type\":\"#DateTimeOffset\"");
        }

        [Fact]
        public void WritingPrimitiveAnnotationWithDeclaredTypeShouldNotIncludeTypeName()
        {
            EdmModel edmModel = new EdmModel();
            edmModel.AddElement(new EdmTerm("custom.namespace", "MyDateTimeOffsetTerm", EdmPrimitiveTypeKind.DateTimeOffset));

            var result = WriteInstanceAnnotation(
                new ODataInstanceAnnotation("custom.namespace.MyDateTimeOffsetTerm", new ODataPrimitiveValue(DateTimeOffset.MinValue)),
                TestUtils.WrapReferencedModelsToMainModel(edmModel));

            result.Should().NotContain("odata.type");
        }

        [Fact]
        public void WritingPrimitiveAnnotationWithTypeMismatchShouldThrow()
        {
            EdmModel edmModel = new EdmModel();
            edmModel.AddElement(new EdmTerm("custom.namespace", "MyDateTimeOffsetTerm", EdmPrimitiveTypeKind.DateTimeOffset));

            // Term is declared to be of type DateTimeOffset, but actual primitive value is a Guid.
            Action testSubject = () => WriteInstanceAnnotation(
                new ODataInstanceAnnotation("custom.namespace.MyDateTimeOffsetTerm", new ODataPrimitiveValue(Guid.Empty)),
                TestUtils.WrapReferencedModelsToMainModel(edmModel));

            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ValidationUtils_IncompatiblePrimitiveItemType("Edm.Guid", /*nullability*/ "False", "Edm.DateTimeOffset", /*nullability*/ "True"));
        }

        [Fact]
        public void WritingComplexAnnotationShouldNotIncludeTypeNameIfDeclaredOnTermMetadata()
        {
            EdmModel edmModel = new EdmModel();

            var complexType = new EdmComplexType("custom.namespace", "Address");
            edmModel.AddElement(complexType);
            edmModel.AddElement(new EdmTerm("custom.namespace", "AddressTerm", new EdmComplexTypeReference(complexType, false)));

            var result = WriteInstanceAnnotation(
                new ODataInstanceAnnotation("custom.namespace.AddressTerm", new ODataComplexValue() { TypeName = "custom.namespace.Address", Properties = Enumerable.Empty<ODataProperty>() }),
                TestUtils.WrapReferencedModelsToMainModel(edmModel));

            result.Should().NotContain("odata.type");
        }

        [Fact]
        public void WritingComplexAnnotationShouldIncludeTypeNameIfNotDeclaredOnTermMetadata()
        {
            EdmModel edmModel = new EdmModel();

            var complexType = new EdmComplexType("custom.namespace", "Address");
            edmModel.AddElement(complexType);

            var result = WriteInstanceAnnotation(
                new ODataInstanceAnnotation("custom.namespace.AddressTerm", new ODataComplexValue() { TypeName = "custom.namespace.Address", Properties = Enumerable.Empty<ODataProperty>() }),
                TestUtils.WrapReferencedModelsToMainModel(edmModel));

            result.Should().Contain("\"@custom.namespace.AddressTerm\":{\"@odata.type\":\"#custom.namespace.Address\"");
        }

        [Fact]
        public void WritingCollectionAnnotationShouldNotIncludeTypeNameIfDeclaredOnTermMetadata()
        {
            EdmModel edmModel = new EdmModel();
            edmModel.AddElement(new EdmTerm("custom.namespace", "CollectionValueTerm", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)))));

            var result = WriteInstanceAnnotation(
                new ODataInstanceAnnotation("custom.namespace.CollectionValueTerm", new ODataCollectionValue { Items = new[] { 42, 54 }, TypeName = "Collection(Int32)" }),
                TestUtils.WrapReferencedModelsToMainModel(edmModel));

            result.Should().Contain("{\"@custom.namespace.CollectionValueTerm\":[42,54]}");
            result.Should().NotContain("odata.type");
        }

        [Fact]
        public void WritingCollectionAnnotationShouldIncludeTypeNameIfNotDeclaredOnTermMetadata()
        {
            EdmModel edmModel = new EdmModel();

            var result = WriteInstanceAnnotation(
                new ODataInstanceAnnotation("custom.namespace.CollectionValueTerm", new ODataCollectionValue { Items = new[] { 42, 54 }, TypeName = "Collection(Int32)" }),
                TestUtils.WrapReferencedModelsToMainModel(edmModel));

            result.Should().Contain("\"custom.namespace.CollectionValueTerm@odata.type\":\"#Collection(Int32)\"");
        }

        [Fact]
        public void WritingComplexAnnotationWithNotDefinedComplexTypeShouldThrow()
        {
            // Note: this behavior may change in future releases, but capturing the behavior shipping in 5.3.
            EdmModel edmModel = new EdmModel();

            Action testSubject = () => WriteInstanceAnnotation(
                new ODataInstanceAnnotation("custom.namespace.AddressTerm", new ODataComplexValue() { TypeName = "custom.namespace.Address", Properties = Enumerable.Empty<ODataProperty>() }),
                TestUtils.WrapReferencedModelsToMainModel(edmModel));

            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ValidationUtils_UnrecognizedTypeName("custom.namespace.Address"));
        }

        [Fact]
        public void WritingComplexAnnotationWithCollectionOfNotDefinedComplexTypeShouldThrow()
        {
            // Note: this behavior may change in future releases, but capturing the behavior shipping in 5.3.
            EdmModel edmModel = new EdmModel();

            Action testSubject = () => WriteInstanceAnnotation(
                new ODataInstanceAnnotation("custom.namespace.CollectionOfAddressTerm", new ODataCollectionValue { Items = Enumerable.Empty<ODataComplexValue>(), TypeName = "Collection(custom.namespace.Address)" }),
                TestUtils.WrapReferencedModelsToMainModel(edmModel));

            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ValidationUtils_UnrecognizedTypeName("Collection(custom.namespace.Address)"));
        }

        private static string WriteInstanceAnnotation(ODataInstanceAnnotation instanceAnnotation, IEdmModel model)
        {
            var stringWriter = new StringWriter();
            var outputContext = new ODataJsonLightOutputContext(
                ODataFormat.Json,
                stringWriter,
                new ODataMessageWriterSettings { Version = ODataVersion.V4, ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") },
                model);

            var valueSerializer = new ODataJsonLightValueSerializer(outputContext);

            // The JSON Writer will complain if there is no active scope, so start an object scope.
            valueSerializer.JsonWriter.StartObjectScope();
            var instanceAnnotationWriter = new JsonLightInstanceAnnotationWriter(valueSerializer, new JsonMinimalMetadataTypeNameOracle());

            // The method under test.
            instanceAnnotationWriter.WriteInstanceAnnotation(instanceAnnotation);

            valueSerializer.JsonWriter.EndObjectScope();
            return stringWriter.ToString();
        }
        #endregion type name short-span integration tests

        [Fact]
        public void WriteInstanceAnnotationShouldWriteAnnotationsThatPassTheAnnotationFilter()
        {
            var annotation = new ODataInstanceAnnotation("ns1.name", new ODataPrimitiveValue(123));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;
            this.valueWriter.Settings = new ODataMessageWriterSettings { Version = ODataVersion.V4, ShouldIncludeAnnotation = name => name == "ns1.name" };

            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotation(annotation);
            verifierCalls.Should().Be(2);
        }

        [Fact]
        public void WriteInstanceAnnotationShouldSkipAnnotationsThatDoesNotPassTheAnnotationFilter()
        {
            var annotation = new ODataInstanceAnnotation("ns1.name", new ODataPrimitiveValue(123));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;
            this.valueWriter.Settings = new ODataMessageWriterSettings { Version = ODataVersion.V4, ShouldIncludeAnnotation = name => name != "ns1.name" };

            this.jsonLightInstanceAnnotationWriter.WriteInstanceAnnotation(annotation);
            verifierCalls.Should().Be(0);
        }

        [Fact]
        public void ShouldNotWriteAnyAnnotationByDefault()
        {
            var defaultValueWriter = new MockJsonLightValueSerializer(jsonWriter, model) { Settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 } };
            var defaultAnnotationWriter = new JsonLightInstanceAnnotationWriter(defaultValueWriter, new JsonMinimalMetadataTypeNameOracle());

            var annotations = new Collection<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            annotations.Add(new ODataInstanceAnnotation("term.two", new ODataPrimitiveValue("456")));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            defaultValueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;

            defaultAnnotationWriter.WriteInstanceAnnotations(annotations);
            verifierCalls.Should().Be(0);
        }

        [Fact]
        public void ShouldWriteAnyAnnotationByDefaultWithIgnoreFilterSetToTrue()
        {
            var defaultValueWriter = new MockJsonLightValueSerializer(jsonWriter, model) { Settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 } };
            var defaultAnnotationWriter = new JsonLightInstanceAnnotationWriter(defaultValueWriter, new JsonMinimalMetadataTypeNameOracle());

            var annotations = new Collection<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            annotations.Add(new ODataInstanceAnnotation("term.two", new ODataPrimitiveValue("456")));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            defaultValueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;

            defaultAnnotationWriter.WriteInstanceAnnotations(annotations, new InstanceAnnotationWriteTracker(), true);
            verifierCalls.Should().Be(4);
        }

        [Fact]
        public void TestWriteInstanceAnnotationsForError()
        {
            var defaultValueWriter = new MockJsonLightValueSerializer(jsonWriter, model) { Settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 } };
            var defaultAnnotationWriter = new JsonLightInstanceAnnotationWriter(defaultValueWriter, new JsonMinimalMetadataTypeNameOracle());

            var annotations = new Collection<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            annotations.Add(new ODataInstanceAnnotation("term.two", new ODataPrimitiveValue("456")));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            defaultValueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;

            defaultAnnotationWriter.WriteInstanceAnnotationsForError(annotations);
            verifierCalls.Should().Be(4);
        }
    }
}
