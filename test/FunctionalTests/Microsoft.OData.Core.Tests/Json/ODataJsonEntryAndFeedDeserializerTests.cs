//---------------------------------------------------------------------
// <copyright file="ODataJsonEntryAndFeedDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.OData.Json;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.Json
{
    public class ODataJsonEntryAndFeedDeserializerTests
    {
        private readonly static IEdmModel EdmModel;
        private readonly static ODataMessageReaderSettings MessageReaderSettingsReadAndValidateCustomInstanceAnnotations;
        private readonly static ODataMessageReaderSettings MessageReaderSettingsIgnoreInstanceAnnotations;
        private static readonly EdmAction Action;
        private static readonly EdmActionImport ActionImport;
        private static readonly EdmEntityType EntityType;
        private static readonly EdmEntitySet EntitySet;

        static ODataJsonEntryAndFeedDeserializerTests()
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
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@odata.NullAnnotation\":null}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            Assert.Null(deserializer.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, "odata.NullAnnotation"));
        }

        [Fact]
        public void ReadInstanceAnnotationValueWillReadNullValueWithODataType()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@odata.NullAnnotation\":null}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            propertyAndAnnotationCollector.AddODataPropertyAnnotation("odata.NullAnnotation", "odata.type", "Edm.Guid");
            Assert.Null(deserializer.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, "odata.NullAnnotation"));
        }

        [Fact]
        public void ReadInstanceAnnotationValueWillReadJsonNativeTypesWithoutODataType()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@odata.Int32Annotation\":123}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            Assert.Equal(123, deserializer.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, "odata.Int32Annotation"));
        }

        [Fact]
        public void ReadInstanceAnnotationValueWillReadNonJsonNativePrimitiveTypesWithoutODataTypeAsStringValue()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@Custom.GuidAnnotation\":\"00000000-0000-0000-0000-000000000000\"}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            var result = deserializer.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, "Custom.GuidAnnotation");
            Assert.Equal("00000000-0000-0000-0000-000000000000", result);
        }

        [Fact]
        public void ReadInstanceAnnotationValueWillReadAsCorrectTypePrimitiveTypesWithODataTypeAnnotation()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@Custom.GuidAnnotation\":\"00000000-0000-0000-0000-000000000000\"}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            propertyAndAnnotationCollector.AddODataPropertyAnnotation("Custom.GuidAnnotation", "odata.type", "Edm.Guid");
            var result = deserializer.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, "Custom.GuidAnnotation");
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public void ReadInstanceAnnotationValueWillReadAsCorrectTypeFromModel()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@custom.DateTimeOffsetAnnotation\":\"2013-01-25T09:50Z\"}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);

            object value = deserializer.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, "custom.DateTimeOffsetAnnotation");
            var result = Assert.IsType<DateTimeOffset>(value);
            Assert.Equal(DateTimeOffset.Parse("2013-01-25T09:50Z"), result);
        }

        [Fact]
        public void ReadDateTypeInstanceAnnotationValueWillReadAsCorrectTypeFromModel()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@custom.DateAnnotation\":\"2013-01-25\"}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);

            object value = deserializer.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, "custom.DateAnnotation");
            var result = Assert.IsType<Date>(value);
            Assert.Equal(Date.Parse("2013-01-25"), result);
        }

        [Fact]
        public void ReadTimeOfDayTypeInstanceAnnotationValueWillReadAsCorrectTypeFromModel()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@custom.TimeOfDayAnnotation\":\"12:30:40.900\"}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);

            object value = deserializer.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, "custom.TimeOfDayAnnotation");
            var result = Assert.IsType<TimeOfDay>(value);
            Assert.Equal(TimeOfDay.Parse("12:30:40.900"), result);
        }

        [Fact]
        public void ReadInstanceAnnotationValueShouldThrowIfWireTypeAndModelTypeConflict()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@custom.DateTimeOffsetAnnotation\":\"2013-01-25T09:50Z\"}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);

            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            propertyAndAnnotationCollector.AddODataPropertyAnnotation("custom.DateTimeOffsetAnnotation", "odata.type", "Edm.String");

            Action testSubject = () => deserializer.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, "custom.DateTimeOffsetAnnotation");
            testSubject.Throws<ODataException>(ErrorStrings.ValidationUtils_IncompatibleType("Edm.String", "Edm.DateTimeOffset"));
        }

        //[Fact(Skip="Complex instance annotation is not supported")]
        //public void ReadInstanceAnnotationValueWhenODataTypeAnnotationIsMissingForComplexValue()
        //{
        //    var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@odata.ComplexAnnotation\":{}}");
        //    AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
        //    var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
        //    object resultTmp = deserializer.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, "odata.ComplexAnnotation");
        //    resultTmp.As<ODataComplexValue>().Properties.Count().Should().Be(0);
        //    resultTmp.As<ODataComplexValue>().TypeName.Should().BeNull();
        //}

        [Fact]
        public void ReadInstanceAnnotationValueWhenODataTypeAnnotationIsMissingForCollectionValue()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@OData.CollectionAnnotation\":[]}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            object tmp = deserializer.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, "OData.CollectionAnnotation");
            var collectionValue = tmp as ODataCollectionValue;
            Assert.NotNull(collectionValue);
            Assert.Empty(collectionValue.Items);
            Assert.Null(collectionValue.TypeName);
        }


        ////[Fact(Skip="complex instance annotation is not supported")]
        //public void ReadInstanceAnnotationValueWithODataTypePropertyAnnotationShouldThrow()
        //{
        //    var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@odata.ComplexAnnotation\":{\"StringProperty\":\"StringValue\"}}");
        //    AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
        //    var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
        //    propertyAndAnnotationCollector.AddODataPropertyAnnotation("odata.ComplexAnnotation", "odata.type", "TestNamespace.TestComplexType");
        //    Action action = () => deserializer.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, "odata.ComplexAnnotation");
        //    action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonPropertyAndValueDeserializer_ComplexValueWithPropertyTypeAnnotation("odata.type"));
        //}

        //[Fact(Skip="complex instance annotation is not supported")]
        //public void ReadInstanceAnnotationValueShouldReadComplexValueWithODataTypeInstanceAnnotation()
        //{
        //    var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@odata.ComplexAnnotation\":{\"@odata.type\":\"#TestNamespace.TestComplexType\",\"StringProperty\":\"StringValue\"}}");
        //    AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
        //    var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
        //    ODataComplexValue value = (ODataComplexValue)deserializer.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, "odata.ComplexAnnotation");
        //    var odataComplexValue = new ODataComplexValue { TypeName = "TestNamespace.TestComplexType", Properties = new[] { new ODataProperty { Name = "StringProperty", Value = "StringValue" } } };
        //    TestUtils.AssertODataValueAreEqual(value, odataComplexValue);
        //}

        //[Fact(Skip = "complex collection instance annotation is not supported")]
        //public void ReadInstanceAnnotationValueShouldReadCollectionOfComplexValuesWithODataTypeInstanceAnnotation()
        //{
        //    var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@custom.complexCollectionAnnotation\":[{\"@odata.type\":\"#TestNamespace.TestComplexType\",\"StringProperty\":\"StringValue\"}]}");
        //    AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
        //    var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
        //    propertyAndAnnotationCollector.AddODataPropertyAnnotation("custom.complexCollectionAnnotation", "odata.type", "Collection(TestNamespace.TestComplexType)");
        //    ODataCollectionValue value = (ODataCollectionValue)deserializer.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, "custom.complexCollectionAnnotation");
        //    var odataComplexValue = new ODataComplexValue { TypeName = "TestNamespace.TestComplexType", Properties = new[] { new ODataProperty { Name = "StringProperty", Value = "StringValue" } } };
        //    var odataCollectionValue = new ODataCollectionValue { TypeName = "Collection(TestNamespace.TestComplexType)", Items = new[] { odataComplexValue } };
        //    TestUtils.AssertODataValueAreEqual(value, odataCollectionValue);
        //}

        //[Fact(Skip="complex collection instance annotation is not supported")]
        //public void ReadInstanceAnnotationValueShouldReadCollectionOfComplexValues()
        //{
        //    var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@custom.complexCollectionAnnotation\":[{\"StringProperty\":\"StringValue\"}]}");
        //    AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
        //    var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
        //    propertyAndAnnotationCollector.AddODataPropertyAnnotation("custom.complexCollectionAnnotation", "odata.type", "Collection(TestNamespace.TestComplexType)");
        //    ODataCollectionValue value = (ODataCollectionValue)deserializer.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, "custom.complexCollectionAnnotation");
        //    var odataComplexValue = new ODataComplexValue { TypeName = "TestNamespace.TestComplexType", Properties = new[] { new ODataProperty { Name = "StringProperty", Value = "StringValue" } } };
        //    var odataCollectionValue = new ODataCollectionValue { TypeName = "Collection(TestNamespace.TestComplexType)", Items = new[] { odataComplexValue } };
        //    TestUtils.AssertODataValueAreEqual(value, odataCollectionValue);
        //}

        [Fact]
        public void ReadInstanceAnnotationValueShouldReadCollectionOfPrimitiveValues()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@custom.primitiveCollectionAnnotation\":[1,2,3]}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            propertyAndAnnotationCollector.AddODataPropertyAnnotation("custom.primitiveCollectionAnnotation", "odata.type", "Collection(Edm.Int32)");
            ODataCollectionValue value = (ODataCollectionValue)deserializer.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, "custom.primitiveCollectionAnnotation");
            TestUtils.AssertODataValueAreEqual(value, new ODataCollectionValue { TypeName = "Collection(Edm.Int32)", Items = new object[] { 1, 2, 3 } });
        }

        #endregion Test ReadInstanceAnnotationValue

        #region Test ReadAndApplyResourceSetInstanceAnnotationValue

        //[Fact(Skip = "#834: We do not check the odata.type for resource set")]
        //public void ReadAndApplyResourceSetInstanceAnnotationValueShouldThrowOnReservedODataAnnotationNamesNotApplicableToFeeds()
        //{
        //    var deserializer = this.CreateJsonEntryAndFeedDeserializer("{}");
        //    Action action = () => deserializer.ReadAndApplyResourceSetInstanceAnnotationValue("odata.type", new ODataResourceSet(), null /*propertyAndAnnotationCollector*/);
        //    action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.type"));
        //}

        [Fact]
        public void ReadAndApplyFeedInstanceAnnotationNumericValueShouldSetCountOnFeed()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@odata.count\":3}");

            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            ODataResourceSet feed = new ODataResourceSet();
            deserializer.ReadAndApplyResourceSetInstanceAnnotationValue("odata.count", feed, propertyAndAnnotationCollector: null);
            Assert.Equal(3, feed.Count);
        }

        [Fact]
        public void ReadAndApplyFeedInstanceAnnotationStringValueShouldSetCountOnFeed()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@odata.count\":\"3\"}", false, true);

            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            ODataResourceSet feed = new ODataResourceSet();
            deserializer.ReadAndApplyResourceSetInstanceAnnotationValue("odata.count", feed, propertyAndAnnotationCollector: null);
            Assert.Equal(3, feed.Count);
        }

        [Fact]
        public void ReadAndApplyResourceSetInstanceAnnotationValueWithConflictShouldThrowException()
        {
            // conflict between string "3" and parameter Ieee754Compatible=false
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@odata.count\":\"3\"}");

            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            ODataResourceSet feed = new ODataResourceSet();
            Action test = () => deserializer.ReadAndApplyResourceSetInstanceAnnotationValue("odata.count", feed, null /*propertyAndAnnotationCollector*/);
            test.Throws<ODataException>(ErrorStrings.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter("Edm.Int64"));
        }

        [Fact]
        public void ReadAndApplyResourceSetInstanceAnnotationValueShouldSetNextLinkOnFeed()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@odata.context\":\"http://host/$metadata#TestEntitySet\",\"@odata.nextLink\":\"relativeUrl\"}");
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            deserializer.ReadPayloadStart(ODataPayloadKind.ResourceSet, propertyAndAnnotationCollector, false /*isReadingNestedPayload*/, false /*allowEmptyPayload*/);
            Assert.Equal(JsonNodeType.Property, deserializer.JsonReader.NodeType);
            deserializer.JsonReader.Read();
            ODataResourceSet feed = new ODataResourceSet();
            deserializer.ReadAndApplyResourceSetInstanceAnnotationValue("odata.nextLink", feed, null /*propertyAndAnnotationCollector*/);
            Assert.Equal(new Uri("http://host/relativeUrl", UriKind.Absolute), feed.NextPageLink);
        }

        [Fact]
        public void ReadAndApplyResourceSetInstanceAnnotationValueShouldSetDeltaLinkOnFeed()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@odata.context\":\"http://host/$metadata#TestEntitySet\",\"@odata.deltaLink\":\"relativeUrl\"}");
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            deserializer.ReadPayloadStart(ODataPayloadKind.ResourceSet, propertyAndAnnotationCollector, false /*isReadingNestedPayload*/, false /*allowEmptyPayload*/);
            Assert.Equal(JsonNodeType.Property, deserializer.JsonReader.NodeType);
            deserializer.JsonReader.Read();
            ODataResourceSet feed = new ODataResourceSet();
            deserializer.ReadAndApplyResourceSetInstanceAnnotationValue("odata.deltaLink", feed, null /*propertyAndAnnotationCollector*/);
            Assert.Equal(new Uri("http://host/relativeUrl", UriKind.Absolute), feed.DeltaLink);
        }

        [Fact]
        public void ReadAndApplyResourceSetInstanceAnnotationValueShouldSetInstanceAnnotationOnFeed()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@custom.Int32Annotation\":123}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            ODataResourceSet feed = new ODataResourceSet();
            deserializer.ReadAndApplyResourceSetInstanceAnnotationValue("custom.Int32Annotation", feed, propertyAndAnnotationCollector);
            Assert.Equal(1, feed.InstanceAnnotations.Count);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), feed.InstanceAnnotations.Single(ia => ia.Name == "custom.Int32Annotation").Value);
        }

        #endregion Test ReadAndApplyResourceSetInstanceAnnotationValue

        #region Test ReadEntryInstanceAnnotation

        [Fact]
        public void ReadEntryInstanceAnnotationShouldThrowOnReservedODataAnnotationNamesNotApplicableToEntries()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@odata.count\":\"123\"}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            Action action = () => deserializer.ReadEntryInstanceAnnotation("odata.count", false /*anyPropertyFound*/, true /*typeAnnotationFound*/, null /*propertyAndAnnotationCollector*/);
            action.Throws<ODataException>(ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.count"));
        }

        [Fact]
        public void ReadEntryInstanceAnnotationShouldReadCustomInstanceAnnotationValue()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@custom.Int32Annotation\":123}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            object value = deserializer.ReadEntryInstanceAnnotation("custom.Int32Annotation", false /*anyPropertyFound*/, true /*typeAnnotationFound*/, propertyAndAnnotationCollector);
            Assert.Equal(123, value);
        }

        #endregion Test ReadEntryInstanceAnnotation

        #region Test ApplyEntryInstanceAnnotation

        [Fact]
        public void ApplyEntryInstanceAnnotationShouldThrowOnReservedODataAnnotationNamesNotApplicableToEntries()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@odata.count\":\"123\"}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            Action action = () => deserializer.ApplyEntryInstanceAnnotation(new TestJsonReaderEntryState(), "odata.count", 123);
            action.Throws<ODataException>(ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.count"));
        }

        [Fact]
        public void ApplyEntryInstanceAnnotationShouldSetCustomInstanceAnnotationValue()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@custom.Int32Annotation\":123}");
            AdvanceReaderToFirstPropertyValue(deserializer.JsonReader);
            var entryState = new TestJsonReaderEntryState();
            deserializer.ApplyEntryInstanceAnnotation(entryState, "custom.Int32Annotation", 123);
            Assert.Equal(1, entryState.Resource.InstanceAnnotations.Count);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), entryState.Resource.InstanceAnnotations.Single(ia => ia.Name == "custom.Int32Annotation").Value);
        }

        #endregion Test ApplyEntryInstanceAnnotation

        #region Test ReadTopLevelResourceSetAnnotations

        [Fact]
        public void ReadTopLevelResourceSetAnnotationsForResourceSetStartAndBufferingShouldReadAllInstanceAnnotations()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@custom.before\":123,\"value\":[],\"@custom.after\":456}");
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var feed = new ODataResourceSet();
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            deserializer.ReadTopLevelResourceSetAnnotations(feed, propertyAndAnnotationCollector, true /*forResourceSetStart*/, true /*readAllFeedProperties*/);
            Assert.Equal(2, feed.InstanceAnnotations.Count);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), feed.InstanceAnnotations.Single(ia => ia.Name == "custom.before").Value);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(456), feed.InstanceAnnotations.Single(ia => ia.Name == "custom.after").Value);
        }

        [Fact]
        public void ReadTopLevelResourceSetAnnotationsForResourceSetStartAndBufferingShouldSkipInstanceAnnotationsBasedOnReaderSettings()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@custom.before\":123,\"value\":[],\"@custom.after\":456}", shouldReadAndValidateCustomInstanceAnnotations: false);
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var feed = new ODataResourceSet();
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            deserializer.ReadTopLevelResourceSetAnnotations(feed, propertyAndAnnotationCollector, true /*forResourceSetStart*/, true /*readAllFeedProperties*/);
            Assert.Equal(0, feed.InstanceAnnotations.Count);
        }

        [Fact]
        public void ReadTopLevelResourceSetAnnotationsForResourceSetStartAndNonBufferingShouldReadOnlyInstanceAnnotationsBeforeValue()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@custom.before\":123,\"value\":[],\"@custom.after\":456}");
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var feed = new ODataResourceSet();
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            deserializer.ReadTopLevelResourceSetAnnotations(feed, propertyAndAnnotationCollector, true /*forResourceSetStart*/, false /*readAllFeedProperties*/);
            Assert.Equal(1, feed.InstanceAnnotations.Count);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), feed.InstanceAnnotations.Single(ia => ia.Name == "custom.before").Value);
        }

        [Fact]
        public void ReadTopLevelResourceSetAnnotationsForResourceSetStartAndNonBufferingShouldSkipInstanceAnnotationsBasedOnReaderSettings()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@custom.before\":123,\"value\":[],\"@custom.after\":456}", shouldReadAndValidateCustomInstanceAnnotations: false);
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var feed = new ODataResourceSet();
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            deserializer.ReadTopLevelResourceSetAnnotations(feed, propertyAndAnnotationCollector, true /*forResourceSetStart*/, false /*readAllFeedProperties*/);
            Assert.Equal(0, feed.InstanceAnnotations.Count);
        }

        [Fact]
        public void ReadTopLevelResourceSetAnnotationsForFeedEndAndBufferingShouldSkipAllValues()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@custom.before\":123,\"value\":[],\"@custom.after\":456}");
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var feed = new ODataResourceSet();
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            deserializer.ReadTopLevelResourceSetAnnotations(feed, propertyAndAnnotationCollector, false /*forResourceSetStart*/, true /*readAllFeedProperties*/);
            Assert.Empty(feed.InstanceAnnotations);
        }

        [Fact]
        public void ReadTopLevelResourceSetAnnotationsForFeedEndAndNonBufferingShouldReadRemainingInstanceAnnotationsAfterValue()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@custom.after\":456}");
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var feed = new ODataResourceSet();
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            deserializer.ReadTopLevelResourceSetAnnotations(feed, propertyAndAnnotationCollector, false /*forResourceSetStart*/, false /*readAllFeedProperties*/);
            Assert.Equal(1, feed.InstanceAnnotations.Count);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(456), feed.InstanceAnnotations.Single(ia => ia.Name == "custom.after").Value);
        }

        [Fact]
        public void ReadTopLevelResourceSetAnnotationsForFeedEndAndNonBufferingShouldSkipInstanceAnnotationsBasedOnReaderSettings()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@custom.after\":456}", shouldReadAndValidateCustomInstanceAnnotations: false);
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var feed = new ODataResourceSet();
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            deserializer.ReadTopLevelResourceSetAnnotations(feed, propertyAndAnnotationCollector, false /*forResourceSetStart*/, false /*readAllFeedProperties*/);
            Assert.Equal(0, feed.InstanceAnnotations.Count);
        }

        #endregion Test ReadTopLevelResourceSetAnnotations

        #region Test ReadResourceContent

        [Fact]
        public void ReadResourceContentShouldReadAndApplyInstanceAnnotations()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@odataa.unknown\":123,\"@custom.annotation\":456}");
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var entryState = new TestJsonReaderEntryState();
            deserializer.ReadResourceContent(entryState);
            Assert.Equal(2, entryState.Resource.InstanceAnnotations.Count);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), entryState.Resource.InstanceAnnotations.Single(ia => ia.Name == "odataa.unknown").Value);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(456), entryState.Resource.InstanceAnnotations.Single(ia => ia.Name == "custom.annotation").Value);
        }

        [Fact]
        public void ReadResourceContentShouldSkipInstanceAnnotationsBasedOnReaderSettings()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@odataa.unknown\":123,\"@custom.annotation\":456}", shouldReadAndValidateCustomInstanceAnnotations: false);
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var entryState = new TestJsonReaderEntryState();
            deserializer.ReadResourceContent(entryState);
            Assert.Equal(0, entryState.Resource.InstanceAnnotations.Count);
        }

        #endregion Test ReadResourceContent

        #region Entity properties instance annotation

        [Fact]
        public void ParsingInstanceAnnotationInNonExistingEntityPropertyShouldReadAsNestedPropertyInfo()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"ID@custom.annotation\":123}");
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var entryState = new TestJsonReaderEntryState();
            var nestedInfo = deserializer.ReadResourceContent(entryState);
            ODataJsonReaderNestedPropertyInfo nestedPropertyInfo = Assert.IsType<ODataJsonReaderNestedPropertyInfo>(nestedInfo);
            Assert.False(nestedPropertyInfo.WithValue);
            Assert.NotNull(nestedPropertyInfo.NestedProperty);
            Assert.Equal("ID", nestedPropertyInfo.NestedProperty.Name);
            ODataInstanceAnnotation annotation = Assert.Single(nestedPropertyInfo.NestedPropertyInfo.InstanceAnnotations);
            Assert.Equal("custom.annotation", annotation.Name);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), annotation.Value);
        }

        [Fact]
        public void ParsingInstanceAnnotationInEntityPropertyShouldReadEntity()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"ID@custom.annotation\":123,\"ID\":1}");
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var entryState = new TestJsonReaderEntryState();
            deserializer.ReadResourceContent(entryState);
            Assert.Single(entryState.Resource.Properties.First().InstanceAnnotations);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), entryState.Resource.Properties.First().InstanceAnnotations.Single(ia => ia.Name == "custom.annotation").Value);
        }

        [Fact]
        public void ParsingInstanceAnnotationsInEntityPropertyShouldReadEntity()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"ID@Annotation.1\":true,\"ID@Annotation.2\":123,\"ID@Annotation.3\":\"annotation\",\"ID\":1}", shouldReadAndValidateCustomInstanceAnnotations: true);
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var entryState = new TestJsonReaderEntryState();
            deserializer.ReadResourceContent(entryState);
            Assert.Equal(3, entryState.Resource.Properties.First().InstanceAnnotations.Count);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(true), entryState.Resource.Properties.First().InstanceAnnotations.Single(ia => ia.Name == "Annotation.1").Value);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), entryState.Resource.Properties.First().InstanceAnnotations.Single(ia => ia.Name == "Annotation.2").Value);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue("annotation"), entryState.Resource.Properties.First().InstanceAnnotations.Single(ia => ia.Name == "Annotation.3").Value);
        }

        [Fact]
        public void ParsingInstanceAnnotationsInEntityPropertyShouldSkipBaseOnSettings()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"ID@Annotation.1\":true,\"ID@Annotation.2\":123,\"ID@Annotation.3\":\"annotation\",\"ID\":1}", shouldReadAndValidateCustomInstanceAnnotations: false);
            AdvanceReaderToFirstProperty(deserializer.JsonReader);
            var entryState = new TestJsonReaderEntryState();
            deserializer.ReadResourceContent(entryState);
            Assert.Empty(entryState.Resource.Properties.First().InstanceAnnotations);
        }

        #endregion


        private ODataJsonInputContext CreateJsonInputContext(string payload, bool shouldReadAndValidateCustomInstanceAnnotations, bool isIeee754Compatible)
        {
            var mediaType = isIeee754Compatible
                ? new ODataMediaType("application", "json", new KeyValuePair<string, string>("IEEE754Compatible", "true"))
                : new ODataMediaType("application", "json");

            var messageInfo = new ODataMessageInfo
            {
                IsResponse = true,
                MediaType = mediaType,
                IsAsync = false,
                Model = EdmModel,
            };

            return new ODataJsonInputContext(
                new StringReader(payload),
                messageInfo,
                shouldReadAndValidateCustomInstanceAnnotations ? MessageReaderSettingsReadAndValidateCustomInstanceAnnotations : MessageReaderSettingsIgnoreInstanceAnnotations);
        }

        private ODataJsonResourceDeserializer CreateJsonEntryAndFeedDeserializer(string payload, bool shouldReadAndValidateCustomInstanceAnnotations = true, bool isIeee754Compatible = false)
        {
            var inputContext = this.CreateJsonInputContext(payload, shouldReadAndValidateCustomInstanceAnnotations, isIeee754Compatible);
            return new ODataJsonResourceDeserializer(inputContext);
        }

        private static void AdvanceReaderToFirstProperty(BufferingJsonReader bufferingJsonReader)
        {
            // Read start and then over the object start.
            bufferingJsonReader.Read();
            bufferingJsonReader.Read();
            Assert.Equal(JsonNodeType.Property, bufferingJsonReader.NodeType);
        }

        private static void AdvanceReaderToFirstPropertyValue(BufferingJsonReader bufferingJsonReader)
        {
            AdvanceReaderToFirstProperty(bufferingJsonReader);

            // Read over property name
            bufferingJsonReader.Read();
        }
    }
}
