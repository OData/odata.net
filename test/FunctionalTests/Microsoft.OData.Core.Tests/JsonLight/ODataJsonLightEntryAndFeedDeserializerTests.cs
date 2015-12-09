//---------------------------------------------------------------------
// <copyright file="ODataJsonLightEntryAndFeedDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.Json;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.JsonLight
{
    public class ODataJsonLightEntryAndFeedDeserializerTests
    {
        private readonly static IEdmModel EdmModel;
        private readonly static ODataMessageReaderSettings MessageReaderSettingsReadAndValidateCustomInstanceAnnotations;
        private readonly static ODataMessageReaderSettings MessageReaderSettingsIgnoreInstanceAnnotations;
        private static readonly EdmAction Action;
        private static readonly EdmActionImport ActionImport;
        private static readonly EdmEntityType EntityType;
        private static readonly EdmEntitySet EntitySet;

        static ODataJsonLightEntryAndFeedDeserializerTests()
        {
            EdmModel tmpModel = new EdmModel();
            EdmComplexType complexType = new EdmComplexType("TestNamespace", "TestComplexType");
            complexType.AddProperty(new EdmStructuralProperty(complexType, "StringProperty", EdmCoreModel.Instance.GetString(false)));
            tmpModel.AddElement(complexType);

            EntityType = new EdmEntityType("TestNamespace", "TestEntityType");
            tmpModel.AddElement(EntityType);
            var keyProperty = new EdmStructuralProperty(EntityType, "ID", EdmCoreModel.Instance.GetInt32(false));
            EntityType.AddKeys(new IEdmStructuralProperty[] { keyProperty });
            EntityType.AddProperty(keyProperty);

            var defaultContainer = new EdmEntityContainer("TestNamespace", "DefaultContainer_sub");
            tmpModel.AddElement(defaultContainer);
            EntitySet = new EdmEntitySet(defaultContainer, "TestEntitySet", EntityType);
            defaultContainer.AddElement(EntitySet);

            Action = new EdmAction("TestNamespace", "DoSomething", null, true, null);
            Action.AddParameter("p1", new EdmEntityTypeReference(EntityType, false));
            tmpModel.AddElement(Action);

            ActionImport = defaultContainer.AddActionImport("DoSomething", Action);

            var serviceOperationFunction = new EdmFunction("TestNamespace", "ServiceOperation", EdmCoreModel.Instance.GetInt32(true));
            defaultContainer.AddFunctionImport("ServiceOperation", serviceOperationFunction);
            tmpModel.AddElement(serviceOperationFunction);

            tmpModel.AddElement(new EdmTerm("custom", "DateTimeOffsetAnnotation", EdmPrimitiveTypeKind.DateTimeOffset));
            tmpModel.AddElement(new EdmTerm("custom", "DateAnnotation", EdmPrimitiveTypeKind.Date));
            tmpModel.AddElement(new EdmTerm("custom", "TimeOfDayAnnotation", EdmPrimitiveTypeKind.TimeOfDay));

            EdmModel = TestUtils.WrapReferencedModelsToMainModel("TestNamespace", "DefaultContainer", tmpModel);
            MessageReaderSettingsReadAndValidateCustomInstanceAnnotations = new ODataMessageReaderSettings { ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
            MessageReaderSettingsIgnoreInstanceAnnotations = new ODataMessageReaderSettings();
        }

        #region Test ReadInstanceAnnotationValue

        [Fact]
        public void ReadInstanceAnnotationValueWillReadNullValueWithoutODataType()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.NullAnnotation\":null}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            deserializer.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, "odata.NullAnnotation").Should().BeNull();
        }

        [Fact]
        public void ReadInstanceAnnotationValueWillReadNullValueWithODataType()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.NullAnnotation\":null}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            duplicatePropertyNamesChecker.AddODataPropertyAnnotation("odata.NullAnnotation", "odata.type", "Edm.Guid");
            deserializer.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, "odata.NullAnnotation").Should().BeNull();
        }

        [Fact]
        public void ReadInstanceAnnotationValueWillReadJsonNativeTypesWithoutODataType()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.Int32Annotation\":123}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            deserializer.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, "odata.Int32Annotation").Should().Be(123);
        }

        [Fact]
        public void ReadInstanceAnnotationValueWillReadNonJsonNativePrimitiveTypesWithoutODataTypeAsStringValue()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@Custom.GuidAnnotation\":\"00000000-0000-0000-0000-000000000000\"}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            deserializer.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, "Custom.GuidAnnotation").Should().Be("00000000-0000-0000-0000-000000000000");
        }

        [Fact]
        public void ReadInstanceAnnotationValueWillReadAsCorrectTypePrimitiveTypesWithODataTypeAnnotation()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@Custom.GuidAnnotation\":\"00000000-0000-0000-0000-000000000000\"}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            duplicatePropertyNamesChecker.AddODataPropertyAnnotation("Custom.GuidAnnotation", "odata.type", "Edm.Guid");
            deserializer.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, "Custom.GuidAnnotation").Should().Be(Guid.Empty);
        }

        [Fact]
        public void ReadInstanceAnnotationValueWillReadAsCorrectTypeFromModel()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@custom.DateTimeOffsetAnnotation\":\"2013-01-25T09:50Z\"}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);

            object value = deserializer.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, "custom.DateTimeOffsetAnnotation");
            value.Should().BeOfType<DateTimeOffset>();
            value.Should().Be(DateTimeOffset.Parse("2013-01-25T09:50Z"));
        }

        [Fact]
        public void ReadDateTypeInstanceAnnotationValueWillReadAsCorrectTypeFromModel()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@custom.DateAnnotation\":\"2013-01-25\"}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);

            object value = deserializer.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, "custom.DateAnnotation");
            value.Should().BeOfType<Date>();
            value.Should().Be(Date.Parse("2013-01-25"));
        }

        [Fact]
        public void ReadTimeOfDayTypeInstanceAnnotationValueWillReadAsCorrectTypeFromModel()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@custom.TimeOfDayAnnotation\":\"12:30:40.900\"}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);

            object value = deserializer.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, "custom.TimeOfDayAnnotation");
            value.Should().BeOfType<TimeOfDay>();
            value.Should().Be(TimeOfDay.Parse("12:30:40.900"));
        }

        [Fact]
        public void ReadInstanceAnnotationValueShouldThrowIfWireTypeAndModelTypeConflict()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@custom.DateTimeOffsetAnnotation\":\"2013-01-25T09:50Z\"}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);

            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            duplicatePropertyNamesChecker.AddODataPropertyAnnotation("custom.DateTimeOffsetAnnotation", "odata.type", "Edm.String");

            Action testSubject = () => deserializer.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, "custom.DateTimeOffsetAnnotation");
            testSubject.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ValidationUtils_IncompatibleType("Edm.String", "Edm.DateTimeOffset"));
        }

        [Fact]
        public void ReadInstanceAnnotationValueWhenODataTypeAnnotationIsMissingForComplexValue()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.ComplexAnnotation\":{}}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            object resultTmp = deserializer.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, "odata.ComplexAnnotation");
            resultTmp.As<ODataComplexValue>().Properties.Count().Should().Be(0);
            resultTmp.As<ODataComplexValue>().TypeName.Should().BeNull();
        }

        [Fact]
        public void ReadInstanceAnnotationValueWhenODataTypeAnnotationIsMissingForCollectionValue()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@OData.CollectionAnnotation\":[]}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            object tmp = deserializer.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, "OData.CollectionAnnotation");
            tmp.As<ODataCollectionValue>().Items.Cast<string>().Count().Should().Be(0);
            tmp.As<ODataCollectionValue>().TypeName.ShouldBeEquivalentTo(null);
        }


        [Fact]
        public void ReadInstanceAnnotationValueWithODataTypePropertyAnnotationShouldThrow()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.ComplexAnnotation\":{\"StringProperty\":\"StringValue\"}}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            duplicatePropertyNamesChecker.AddODataPropertyAnnotation("odata.ComplexAnnotation", "odata.type", "TestNamespace.TestComplexType");
            Action action = () => deserializer.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, "odata.ComplexAnnotation");
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightPropertyAndValueDeserializer_ComplexValueWithPropertyTypeAnnotation("odata.type"));
        }

        [Fact]
        public void ReadInstanceAnnotationValueShouldReadComplexValueWithODataTypeInstanceAnnotation()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.ComplexAnnotation\":{\"@odata.type\":\"#TestNamespace.TestComplexType\",\"StringProperty\":\"StringValue\"}}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            ODataComplexValue value = (ODataComplexValue)deserializer.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, "odata.ComplexAnnotation");
            var odataComplexValue = new ODataComplexValue { TypeName = "TestNamespace.TestComplexType", Properties = new[] { new ODataProperty { Name = "StringProperty", Value = "StringValue" } } };
            TestUtils.AssertODataValueAreEqual(value, odataComplexValue);
        }

        [Fact]
        public void ReadInstanceAnnotationValueShouldReadCollectionOfComplexValuesWithODataTypeInstanceAnnotation()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@custom.complexCollectionAnnotation\":[{\"@odata.type\":\"#TestNamespace.TestComplexType\",\"StringProperty\":\"StringValue\"}]}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            duplicatePropertyNamesChecker.AddODataPropertyAnnotation("custom.complexCollectionAnnotation", "odata.type", "Collection(TestNamespace.TestComplexType)");
            ODataCollectionValue value = (ODataCollectionValue)deserializer.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, "custom.complexCollectionAnnotation");
            var odataComplexValue = new ODataComplexValue { TypeName = "TestNamespace.TestComplexType", Properties = new[] { new ODataProperty { Name = "StringProperty", Value = "StringValue" } } };
            var odataCollectionValue = new ODataCollectionValue { TypeName = "Collection(TestNamespace.TestComplexType)", Items = new[] { odataComplexValue } };
            TestUtils.AssertODataValueAreEqual(value, odataCollectionValue);
        }

        [Fact]
        public void ReadInstanceAnnotationValueShouldReadCollectionOfComplexValues()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@custom.complexCollectionAnnotation\":[{\"StringProperty\":\"StringValue\"}]}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            duplicatePropertyNamesChecker.AddODataPropertyAnnotation("custom.complexCollectionAnnotation", "odata.type", "Collection(TestNamespace.TestComplexType)");
            ODataCollectionValue value = (ODataCollectionValue)deserializer.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, "custom.complexCollectionAnnotation");
            var odataComplexValue = new ODataComplexValue { TypeName = "TestNamespace.TestComplexType", Properties = new[] { new ODataProperty { Name = "StringProperty", Value = "StringValue" } } };
            var odataCollectionValue = new ODataCollectionValue { TypeName = "Collection(TestNamespace.TestComplexType)", Items = new[] { odataComplexValue } };
            TestUtils.AssertODataValueAreEqual(value, odataCollectionValue);
        }

        [Fact]
        public void ReadInstanceAnnotationValueShouldReadCollectionOfPrimitiveValues()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@custom.primitiveCollectionAnnotation\":[1,2,3]}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            duplicatePropertyNamesChecker.AddODataPropertyAnnotation("custom.primitiveCollectionAnnotation", "odata.type", "Collection(Edm.Int32)");
            ODataCollectionValue value = (ODataCollectionValue)deserializer.ReadCustomInstanceAnnotationValue(duplicatePropertyNamesChecker, "custom.primitiveCollectionAnnotation");
            TestUtils.AssertODataValueAreEqual(value, new ODataCollectionValue { TypeName = "Collection(Edm.Int32)", Items = new[] { 1, 2, 3 } });
        }

        #endregion Test ReadInstanceAnnotationValue

        #region Test ReadAndApplyFeedInstanceAnnotationValue

        [Fact]
        public void ReadAndApplyFeedInstanceAnnotationValueShouldThrowOnReservedODataAnnotationNamesNotApplicableToFeeds()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{}");
            Action action = () => deserializer.ReadAndApplyFeedInstanceAnnotationValue("odata.type", new ODataFeed(), null /*duplicatePropertyNamesChecker*/);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.type"));
        }

        [Fact]
        public void ReadAndApplyFeedInstanceAnnotationNumericValueShouldSetCountOnFeed()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.count\":3}");

            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            ODataFeed feed = new ODataFeed();
            deserializer.ReadAndApplyFeedInstanceAnnotationValue("odata.count", feed, duplicatePropertyNamesChecker: null);
            Assert.Equal(3, feed.Count);
        }

        [Fact]
        public void ReadAndApplyFeedInstanceAnnotationStringValueShouldSetCountOnFeed()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.count\":\"3\"}", false, true);

            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            ODataFeed feed = new ODataFeed();
            deserializer.ReadAndApplyFeedInstanceAnnotationValue("odata.count", feed, duplicatePropertyNamesChecker: null);
            Assert.Equal(3, feed.Count);
        }

        [Fact]
        public void ReadAndApplyFeedInstanceAnnotationValueWithConflictShouldThrowException()
        {
            // conflict between string "3" and parameter Ieee754Compatible=false
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.count\":\"3\"}");

            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            ODataFeed feed = new ODataFeed();
            Action test = () => deserializer.ReadAndApplyFeedInstanceAnnotationValue("odata.count", feed, null /*duplicatePropertyNamesChecker*/);
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter("Edm.Int64"));
        }

        [Fact]
        public void ReadAndApplyFeedInstanceAnnotationValueShouldSetNextLinkOnFeed()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.context\":\"http://host/$metadata#TestEntitySet\",\"@odata.nextLink\":\"relativeUrl\"}");
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            deserializer.ReadPayloadStart(ODataPayloadKind.Feed, duplicatePropertyNamesChecker, false /*isReadingNestedPayload*/, false /*allowEmptyPayload*/);
            deserializer.JsonReader.NodeType.Should().Be(JsonNodeType.Property);
            deserializer.JsonReader.Read();
            ODataFeed feed = new ODataFeed();
            deserializer.ReadAndApplyFeedInstanceAnnotationValue("odata.nextLink", feed, null /*duplicatePropertyNamesChecker*/);
            Assert.Equal(new Uri("http://host/relativeUrl", UriKind.Absolute), feed.NextPageLink);
        }

        [Fact]
        public void ReadAndApplyFeedInstanceAnnotationValueShouldSetDeltaLinkOnFeed()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.context\":\"http://host/$metadata#TestEntitySet\",\"@odata.deltaLink\":\"relativeUrl\"}");
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            deserializer.ReadPayloadStart(ODataPayloadKind.Feed, duplicatePropertyNamesChecker, false /*isReadingNestedPayload*/, false /*allowEmptyPayload*/);
            deserializer.JsonReader.NodeType.Should().Be(JsonNodeType.Property);
            deserializer.JsonReader.Read();
            ODataFeed feed = new ODataFeed();
            deserializer.ReadAndApplyFeedInstanceAnnotationValue("odata.deltaLink", feed, null /*duplicatePropertyNamesChecker*/);
            Assert.Equal(new Uri("http://host/relativeUrl", UriKind.Absolute), feed.DeltaLink);
        }

        [Fact]
        public void ReadAndApplyFeedInstanceAnnotationValueShouldSetInstanceAnnotationOnFeed()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@custom.Int32Annotation\":123}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            ODataFeed feed = new ODataFeed();
            deserializer.ReadAndApplyFeedInstanceAnnotationValue("custom.Int32Annotation", feed, duplicatePropertyNamesChecker);
            Assert.Equal(1, feed.InstanceAnnotations.Count);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), feed.InstanceAnnotations.Single(ia => ia.Name == "custom.Int32Annotation").Value);
        }

        #endregion Test ReadAndApplyFeedInstanceAnnotationValue

        #region Test ReadEntryInstanceAnnotation

        [Fact]
        public void ReadEntryInstanceAnnotationShouldThrowOnReservedODataAnnotationNamesNotApplicableToEntries()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.count\":\"123\"}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            Action action = () => deserializer.ReadEntryInstanceAnnotation("odata.count", false /*anyPropertyFound*/, true /*typeAnnotationFound*/, null /*duplicatePropertyNamesChecker*/);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.count"));
        }

        [Fact]
        public void ReadEntryInstanceAnnotationShouldReadCustomInstanceAnnotationValue()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@custom.Int32Annotation\":123}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            object value = deserializer.ReadEntryInstanceAnnotation("custom.Int32Annotation", false /*anyPropertyFound*/, true /*typeAnnotationFound*/, duplicatePropertyNamesChecker);
            Assert.Equal(123, value);
        }

        #endregion Test ReadEntryInstanceAnnotation

        #region Test ApplyEntryInstanceAnnotation

        [Fact]
        public void ApplyEntryInstanceAnnotationShouldThrowOnReservedODataAnnotationNamesNotApplicableToEntries()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.count\":\"123\"}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            Action action = () => deserializer.ApplyEntryInstanceAnnotation(new TestJsonLightReaderEntryState(), "odata.count", 123);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.count"));
        }

        [Fact]
        public void ApplyEntryInstanceAnnotationShouldSetCustomInstanceAnnotationValue()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@custom.Int32Annotation\":123}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var entryState = new TestJsonLightReaderEntryState();
            deserializer.ApplyEntryInstanceAnnotation(entryState, "custom.Int32Annotation", 123);
            Assert.Equal(1, entryState.Entry.InstanceAnnotations.Count);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), entryState.Entry.InstanceAnnotations.Single(ia => ia.Name == "custom.Int32Annotation").Value);
        }

        #endregion Test ApplyEntryInstanceAnnotation

        #region Test ReadTopLevelFeedAnnotations

        [Fact]
        public void ReadTopLevelFeedAnnotationsForFeedStartAndBufferingShouldReadAllInstanceAnnotations()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@custom.before\":123,\"value\":[],\"@custom.after\":456}");
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var feed = new ODataFeed();
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            deserializer.ReadTopLevelFeedAnnotations(feed, duplicatePropertyNamesChecker, true /*forFeedStart*/, true /*readAllFeedProperties*/);
            Assert.Equal(2, feed.InstanceAnnotations.Count);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), feed.InstanceAnnotations.Single(ia => ia.Name == "custom.before").Value);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(456), feed.InstanceAnnotations.Single(ia => ia.Name == "custom.after").Value);
        }

        [Fact]
        public void ReadTopLevelFeedAnnotationsForFeedStartAndBufferingShouldSkipInstanceAnnotationsBasedOnReaderSettings()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@custom.before\":123,\"value\":[],\"@custom.after\":456}", shouldReadAndValidateCustomInstanceAnnotations: false);
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var feed = new ODataFeed();
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            deserializer.ReadTopLevelFeedAnnotations(feed, duplicatePropertyNamesChecker, true /*forFeedStart*/, true /*readAllFeedProperties*/);
            Assert.Equal(0, feed.InstanceAnnotations.Count);
        }

        [Fact]
        public void ReadTopLevelFeedAnnotationsForFeedStartAndNonBufferingShouldReadOnlyInstanceAnnotationsBeforeValue()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@custom.before\":123,\"value\":[],\"@custom.after\":456}");
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var feed = new ODataFeed();
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            deserializer.ReadTopLevelFeedAnnotations(feed, duplicatePropertyNamesChecker, true /*forFeedStart*/, false /*readAllFeedProperties*/);
            Assert.Equal(1, feed.InstanceAnnotations.Count);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), feed.InstanceAnnotations.Single(ia => ia.Name == "custom.before").Value);
        }

        [Fact]
        public void ReadTopLevelFeedAnnotationsForFeedStartAndNonBufferingShouldSkipInstanceAnnotationsBasedOnReaderSettings()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@custom.before\":123,\"value\":[],\"@custom.after\":456}", shouldReadAndValidateCustomInstanceAnnotations: false);
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var feed = new ODataFeed();
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            deserializer.ReadTopLevelFeedAnnotations(feed, duplicatePropertyNamesChecker, true /*forFeedStart*/, false /*readAllFeedProperties*/);
            Assert.Equal(0, feed.InstanceAnnotations.Count);
        }

        [Fact]
        public void ReadTopLevelFeedAnnotationsForFeedEndAndBufferingShouldSkipAllValues()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@custom.before\":123,\"value\":[],\"@custom.after\":456}");
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var feed = new ODataFeed();
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            deserializer.ReadTopLevelFeedAnnotations(feed, duplicatePropertyNamesChecker, false /*forFeedStart*/, true /*readAllFeedProperties*/);
            feed.InstanceAnnotations.Should().BeEmpty();
        }

        [Fact]
        public void ReadTopLevelFeedAnnotationsForFeedEndAndNonBufferingShouldReadRemainingInstanceAnnotationsAfterValue()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@custom.after\":456}");
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var feed = new ODataFeed();
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            deserializer.ReadTopLevelFeedAnnotations(feed, duplicatePropertyNamesChecker, false /*forFeedStart*/, false /*readAllFeedProperties*/);
            Assert.Equal(1, feed.InstanceAnnotations.Count);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(456), feed.InstanceAnnotations.Single(ia => ia.Name == "custom.after").Value);
        }

        [Fact]
        public void ReadTopLevelFeedAnnotationsForFeedEndAndNonBufferingShouldSkipInstanceAnnotationsBasedOnReaderSettings()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@custom.after\":456}", shouldReadAndValidateCustomInstanceAnnotations: false);
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var feed = new ODataFeed();
            var duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false /*allowDuplicateProperties*/, true /*isResponse*/);
            deserializer.ReadTopLevelFeedAnnotations(feed, duplicatePropertyNamesChecker, false /*forFeedStart*/, false /*readAllFeedProperties*/);
            Assert.Equal(0, feed.InstanceAnnotations.Count);
        }

        #endregion Test ReadTopLevelFeedAnnotations

        #region Test ReadEntryContent

        [Fact]
        public void ReadEntryContentShouldReadAndApplyInstanceAnnotations()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odataa.unknown\":123,\"@custom.annotation\":456}");
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var entryState = new TestJsonLightReaderEntryState();
            deserializer.ReadEntryContent(entryState);
            Assert.Equal(2, entryState.Entry.InstanceAnnotations.Count);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), entryState.Entry.InstanceAnnotations.Single(ia => ia.Name == "odataa.unknown").Value);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(456), entryState.Entry.InstanceAnnotations.Single(ia => ia.Name == "custom.annotation").Value);
        }

        [Fact]
        public void ReadEntryContentShouldSkipInstanceAnnotationsBasedOnReaderSettings()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odataa.unknown\":123,\"@custom.annotation\":456}", shouldReadAndValidateCustomInstanceAnnotations: false);
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var entryState = new TestJsonLightReaderEntryState();
            deserializer.ReadEntryContent(entryState);
            Assert.Equal(0, entryState.Entry.InstanceAnnotations.Count);
        }

        #endregion Test ReadEntryContent

        #region Entity properties instance annotation

        [Fact]
        public void ParsingInstanceAnnotationInNonExistingEntityPropertyShouldThrow()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"ID@custom.annotation\":123}");
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var entryState = new TestJsonLightReaderEntryState();
            Action action = () => deserializer.ReadEntryContent(entryState);
            action.ShouldThrow<ODataException>(ErrorStrings.ODataJsonLightEntryAndFeedDeserializer_PropertyWithoutValueWithWrongType("ID", "Edm.Int32"));
        }

        [Fact]
        public void ParsingInstanceAnnotationInEntityPropertyShouldReadEntity()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"ID@custom.annotation\":123,\"ID\":1}");
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var entryState = new TestJsonLightReaderEntryState();
            deserializer.ReadEntryContent(entryState);
            entryState.Entry.Properties.First().InstanceAnnotations.Count.Should().Be(1);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), entryState.Entry.Properties.First().InstanceAnnotations.Single(ia => ia.Name == "custom.annotation").Value);
        }

        [Fact]
        public void ParsingInstanceAnnotationsInEntityPropertyShouldReadEntity()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"ID@Annotation.1\":true,\"ID@Annotation.2\":123,\"ID@Annotation.3\":\"annotation\",\"ID\":1}");
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var entryState = new TestJsonLightReaderEntryState();
            deserializer.ReadEntryContent(entryState);
            entryState.Entry.Properties.First().InstanceAnnotations.Count.Should().Be(3);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(true), entryState.Entry.Properties.First().InstanceAnnotations.Single(ia => ia.Name == "Annotation.1").Value);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), entryState.Entry.Properties.First().InstanceAnnotations.Single(ia => ia.Name == "Annotation.2").Value);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue("annotation"), entryState.Entry.Properties.First().InstanceAnnotations.Single(ia => ia.Name == "Annotation.3").Value);
        }

        [Fact]
        public void ParsingInstanceAnnotationsInEntityPropertyShouldSkipBaseOnSettings()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"ID@Annotation.1\":true,\"ID@Annotation.2\":123,\"ID@Annotation.3\":\"annotation\",\"ID\":1}", false);
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var entryState = new TestJsonLightReaderEntryState();
            deserializer.ReadEntryContent(entryState);
            entryState.Entry.Properties.First().InstanceAnnotations.Count.Should().Be(0);
        }

        #endregion


        private ODataJsonLightInputContext CreateJsonLightInputContext(string payload, bool shouldReadAndValidateCustomInstanceAnnotations, bool isIeee754Compatible)
        {
            ODataMediaType mediaType = isIeee754Compatible
                ? new ODataMediaType("application", "json", new KeyValuePair<string, string>("IEEE754Compatible", "true"))
                : new ODataMediaType("application", "json");

            return new ODataJsonLightInputContext(
                ODataFormat.Json,
                new MemoryStream(Encoding.UTF8.GetBytes(payload)),
                mediaType,
                Encoding.UTF8,
                shouldReadAndValidateCustomInstanceAnnotations ? MessageReaderSettingsReadAndValidateCustomInstanceAnnotations : MessageReaderSettingsIgnoreInstanceAnnotations,
                /*readingResponse*/ true,
                /*synchronous*/ true,
                EdmModel,
                /*urlResolver*/ null);
        }

        private ODataJsonLightEntryAndFeedDeserializer CreateJsonLightEntryAndFeedDeserializer(string payload, bool shouldReadAndValidateCustomInstanceAnnotations = true, bool isIeee754Compatible = false)
        {
            var inputContext = this.CreateJsonLightInputContext(payload, shouldReadAndValidateCustomInstanceAnnotations, isIeee754Compatible);
            return new ODataJsonLightEntryAndFeedDeserializer(inputContext);
        }

        private static void AdvanceReaderToFirstProperty(BufferingJsonReader bufferingJsonReader)
        {
            // Read start and then over the object start.
            bufferingJsonReader.Read();
            bufferingJsonReader.Read();
            bufferingJsonReader.NodeType.Should().Be(JsonNodeType.Property);
        }

        private static void AdvanceReaderToFirstPropertyValue(BufferingJsonReader bufferingJsonReader)
        {
            AdvanceReaderToFirstProperty(bufferingJsonReader);

            // Read over property name
            bufferingJsonReader.Read();
        }
    }
}
