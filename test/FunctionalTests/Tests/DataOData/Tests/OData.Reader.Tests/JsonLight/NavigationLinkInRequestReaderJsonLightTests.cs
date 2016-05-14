//---------------------------------------------------------------------
// <copyright file="NavigationLinkInRequestReaderJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of navigation links in requests (entity reference links and expanded) in JSON Light.
    /// </summary>
    [TestClass, TestCase]
    public class NavigationLinksInRequestReaderJsonLightTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the reading of singleton navigation links in requests on entry payloads.")]
        public void SingletonNavigationLinkInRequestTest()
        {
            IEnumerable<NavigationLinkTestCase> testCases = new[]
            {
                new NavigationLinkTestCase
                {
                    DebugDescription = "Just binding link",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataBindAnnotationName) + "\":\"http://odata.org/referencelink1\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("PoliceStation", "http://odata.org/referencelink1"),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "PoliceStation", false } },
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Binding link with invalid value - null",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataBindAnnotationName) + "\":null",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("PoliceStation", "http://odata.org/referencelink1"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightReaderUtils_AnnotationWithNullValue", JsonLightConstants.ODataBindAnnotationName),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Binding link with invalid value - number",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataBindAnnotationName) + "\":42",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("PoliceStation", "http://odata.org/referencelink1"),
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_CannotReadPropertyValueAsString", "42", JsonLightConstants.ODataBindAnnotationName),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Binding link with invalid value - array",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataBindAnnotationName) + "\":[]",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("PoliceStation", "http://odata.org/referencelink1"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_EmptyBindArray", JsonLightConstants.ODataBindAnnotationName),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Binding link with invalid value - array of strings",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataBindAnnotationName) + "\":[\"http://odata.org/referencelink1\"]",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("PoliceStation", "http://odata.org/referencelink1"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_ArrayValueForSingletonBindPropertyAnnotation", "PoliceStation", JsonLightConstants.ODataBindAnnotationName),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Binding link with invalid value - object",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataBindAnnotationName) + "\":{}",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("PoliceStation", "http://odata.org/referencelink1"),
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartObject"),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Binding link with custom annotation before",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation") + "\":\"value\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataBindAnnotationName) + "\":\"http://odata.org/referencelink1\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("PoliceStation", "http://odata.org/referencelink1"),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "PoliceStation", false } },
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Binding link with custom annotation after",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataBindAnnotationName) + "\":\"http://odata.org/referencelink1\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation") + "\":\"value\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("PoliceStation", "http://odata.org/referencelink1"),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "PoliceStation", false } },
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Just custom annotation - should fail - we need either expanded value or binding",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation") + "\":\"value\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("PoliceStation", "http://odata.org/referencelink1"),
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightEntryAndFeedDeserializer_NavigationPropertyWithoutValueAndEntityReferenceLink",
                        "PoliceStation",
                        JsonLightConstants.ODataBindAnnotationName),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Non-binding OData annotation on nav. prop - should fail",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataTypeAnnotationName) + "\":\"TestModel.OfficeType\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("PoliceStation", "http://odata.org/referencelink1"),
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightEntryAndFeedDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation",
                        "PoliceStation",
                        JsonLightConstants.ODataTypeAnnotationName,
                        JsonLightConstants.ODataBindAnnotationName),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry",
                    Json = "\"PoliceStation\":{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1}",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { ExpandedElement = PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1) }, null)),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "PoliceStation", false } },
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded null entry",
                    Json = "\"PoliceStation\":null",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { ExpandedElement = new EntityInstance(null, true) }, null)),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "PoliceStation", false } },
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded singleton link with array value - invalid",
                    Json = "\"PoliceStation\":[]",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_CannotReadSingletonNavigationPropertyValue", "StartArray", "PoliceStation"),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded singleton link with number value - invalid",
                    Json = "\"PoliceStation\":42",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_CannotReadNavigationPropertyValue", "PoliceStation"),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with binding - invalid",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataBindAnnotationName) + "\":\"http://odata.org/referencelink1\"," +
                        "\"PoliceStation\":{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1}",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { ExpandedElement = PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1) }, null)),
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightEntryAndFeedDeserializer_SingletonNavigationPropertyWithBindingAndValue",
                        "PoliceStation",
                        JsonLightConstants.ODataBindAnnotationName),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with non-binding annotation - invalid",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataTypeAnnotationName) + "\":\"TestModel.OfficeType\"," +
                        "\"PoliceStation\":{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1}",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { ExpandedElement = PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1) }, null)),
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightEntryAndFeedDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation",
                        "PoliceStation",
                        JsonLightConstants.ODataTypeAnnotationName,
                        JsonLightConstants.ODataBindAnnotationName),
                },
            };

            this.RunNavigationLinkInRequestTest(testCases);
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the reading of singleton navigation links in requests on entry payloads.")]
        public void SingletonRelativeNavigationLinkInRequestTest()
        {
            IEnumerable<NavigationLinkTestCase> testCases = new[]
            {
                new NavigationLinkTestCase
                {
                    DebugDescription = "Binding link with relative URI - no odata.context annotation in payload.",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataBindAnnotationName) + "\":\"yyy\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("PoliceStation", "yyy"),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "PoliceStation", false } },
                },
            };

            this.RunNavigationLinkInRequestTest(testCases);

            testCases = new[]
            {
                new NavigationLinkTestCase
                {
                    DebugDescription = "Binding link with relative URI - with odata.context annotation in payload.",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataBindAnnotationName) + "\":\"yyy\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("PoliceStation", "yyy"),
                    ExpectedException = null,
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "PoliceStation", false } },
                },
            };

            this.RunNavigationLinkInRequestTest(testCases, withMetadataAnnotation: true);
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the reading of collection navigation links in requests on entry payloads.")]
        public void CollectionNavigationLinkInRequestTest()
        {
            IEnumerable<NavigationLinkTestCase> testCases = new[]
            {
                new NavigationLinkTestCase
                {
                    DebugDescription = "Just binding links - one",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataBindAnnotationName) + "\":[\"http://odata.org/referencelink1\"]",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("CityHall", "http://odata.org/referencelink1"),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "CityHall", true } },
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Just binding links - multiple",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataBindAnnotationName) + "\":[" +
                        "\"http://odata.org/referencelink1\"," +
                        "\"http://odata.org/referencelink2\"," +
                        "\"http://odata.org/referencelink3\"]",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new LinkCollection(
                        PayloadBuilder.DeferredLink("http://odata.org/referencelink1"),
                        PayloadBuilder.DeferredLink("http://odata.org/referencelink2"),
                        PayloadBuilder.DeferredLink("http://odata.org/referencelink3")))),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "CityHall", true } },
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Empty binding array - invalid",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataBindAnnotationName) + "\":[]",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_EmptyBindArray", JsonLightConstants.ODataBindAnnotationName),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Invalid binding annotation value - object",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataBindAnnotationName) + "\":{}",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartObject"),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Invalid binding annotation value - null",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataBindAnnotationName) + "\":null",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightReaderUtils_AnnotationWithNullValue", JsonLightConstants.ODataBindAnnotationName),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Invalid binding annotation value - number",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataBindAnnotationName) + "\":42",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_CannotReadPropertyValueAsString", "42", JsonLightConstants.ODataBindAnnotationName),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Invalid binding annotation value - string - we need array for collection nav. prop.",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataBindAnnotationName) + "\":\"http://odata.org/referencelink1\"",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_StringValueForCollectionBindPropertyAnnotation", "CityHall", JsonLightConstants.ODataBindAnnotationName),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Invalid binding item value - object",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataBindAnnotationName) + "\":[{}]",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartObject"),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Invalid binding item value - array",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataBindAnnotationName) + "\":[[]]",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartArray"),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Invalid binding item value - null",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataBindAnnotationName) + "\":[null]",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightReaderUtils_AnnotationWithNullValue", JsonLightConstants.ODataBindAnnotationName),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Invalid binding item value - number",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataBindAnnotationName) + "\":[42]",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_CannotReadPropertyValueAsString", "42", JsonLightConstants.ODataBindAnnotationName),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Binding links with custom annotation before",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", "custom.annotation") + "\":null," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataBindAnnotationName) + "\":[\"http://odata.org/referencelink1\"]",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("CityHall", "http://odata.org/referencelink1"),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "CityHall", true } },
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Binding links with custom annotation after",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataBindAnnotationName) + "\":[\"http://odata.org/referencelink1\"]," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", "custom.annotation") + "\":null",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("CityHall", "http://odata.org/referencelink1"),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "CityHall", true } },
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Collection navigation property with just custom annotation - invalid",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", "custom.annotation") + "\":null",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightEntryAndFeedDeserializer_NavigationPropertyWithoutValueAndEntityReferenceLink",
                        "CityHall",
                        JsonLightConstants.ODataBindAnnotationName),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed - empty",
                    Json = "\"CityHall\":[]",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new ExpandedLink() { ExpandedElement = PayloadBuilder.EntitySet() })),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "CityHall", true } },
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed - one entry",
                    Json = "\"CityHall\":[{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1}]",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new ExpandedLink() { ExpandedElement = PayloadBuilder.EntitySet()
                        .Append(PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1))})),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "CityHall", true } },
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed - multiple entries",
                    Json = 
                        "\"CityHall\":[" +
                            "{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1}," +
                            "{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1}," +
                            "{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1}" +
                        "]",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new ExpandedLink() { ExpandedElement = PayloadBuilder.EntitySet()
                        .Append(PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1))
                        .Append(PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1))
                        .Append(PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1))})),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "CityHall", true } },
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with custom annotation",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", "custom.annotation") + "\":null," +
                        "\"CityHall\":[{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1}]",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new ExpandedLink() { ExpandedElement = PayloadBuilder.EntitySet()
                        .Append(PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1))})),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "CityHall", true } },
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with next link before - should fail",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNextLinkAnnotationName) + "\":\"http://odata.org/nextlink\"," +
                        "\"CityHall\":[{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1}]",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightEntryAndFeedDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation",
                        "CityHall",
                        JsonLightConstants.ODataNextLinkAnnotationName,
                        JsonLightConstants.ODataBindAnnotationName),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with next link after - should fail",
                    Json = 
                        "\"CityHall\":[{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1}]," + 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNextLinkAnnotationName) + "\":\"http://odata.org/nextlink\"",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_UnexpectedPropertyAnnotation", "CityHall", JsonLightConstants.ODataNextLinkAnnotationName),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded empty feed with binding link",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataBindAnnotationName) + "\":[\"http://odata.org/referencelink1\"]," +
                        "\"CityHall\":[]",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new LinkCollection(
                        new DeferredLink() { UriString = "http://odata.org/referencelink1" },
                        new ExpandedLink() { ExpandedElement = PayloadBuilder.EntitySet() }))),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "CityHall", true } },
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded empty feed with binding links",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataBindAnnotationName) + "\":[" +
                            "\"http://odata.org/referencelink1\"," +
                            "\"http://odata.org/referencelink2\"," +
                            "\"http://odata.org/referencelink3\"" +
                            "]," +
                        "\"CityHall\":[]",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new LinkCollection(
                        new DeferredLink() { UriString = "http://odata.org/referencelink1" },
                        new DeferredLink() { UriString = "http://odata.org/referencelink2" },
                        new DeferredLink() { UriString = "http://odata.org/referencelink3" },
                        new ExpandedLink() { ExpandedElement = PayloadBuilder.EntitySet() }))),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "CityHall", true } },
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with multiple entries and multiple binding links",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataBindAnnotationName) + "\":[" +
                            "\"http://odata.org/referencelink1\"," +
                            "\"http://odata.org/referencelink2\"," +
                            "\"http://odata.org/referencelink3\"" +
                            "]," +
                        "\"CityHall\":[" +
                            "{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1}," +
                            "{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1}," +
                            "{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1}" +
                        "]",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new LinkCollection(
                        new DeferredLink() { UriString = "http://odata.org/referencelink1" },
                        new DeferredLink() { UriString = "http://odata.org/referencelink2" },
                        new DeferredLink() { UriString = "http://odata.org/referencelink3" },
                        new ExpandedLink() { ExpandedElement = PayloadBuilder.EntitySet()
                            .Append(PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1))
                            .Append(PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1))
                            .Append(PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1)) }))),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "CityHall", true } },
                },
            };

            this.RunNavigationLinkInRequestTest(testCases);
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the reading of collection navigation links in requests on entry payloads.")]
        public void CollectionRelativeNavigationLinkInRequestTest()
        {
            IEnumerable<NavigationLinkTestCase> testCases = new[]
            {
                new NavigationLinkTestCase
                {
                    DebugDescription = "Invalid binding item value - relative uri",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataBindAnnotationName) + "\":[\"yyy\"]",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("CityHall", "yyy"),
                    ExpectedException = null,
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "CityHall", true } },
                },
            };

            this.RunNavigationLinkInRequestTest(testCases);

            testCases = new[]
            {
                new NavigationLinkTestCase
                {
                    DebugDescription = "Invalid binding item value - relative uri",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataBindAnnotationName) + "\":[\"yyy\"]",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("CityHall", "yyy"),
                    ExpectedException = null,
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "CityHall", true } },
                },
            };

            this.RunNavigationLinkInRequestTest(testCases, withMetadataAnnotation: true);
        }

        private void RunNavigationLinkInRequestTest(IEnumerable<NavigationLinkTestCase> testCases, bool withMetadataAnnotation = false)
        {
            IEdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            this.CombinatorialEngineProvider.RunCombinations(
                testCases.Select(testCase => testCase.ToTestDescriptor(this.Settings, model, withMetadataAnnotation)),
                // Entity reference links are request only
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    var testConfigClone = new ReaderTestConfiguration(testConfiguration);
                    testDescriptor.RunTest(testConfigClone);
                });
        }

        private sealed class NavigationLinkTestCase
        {
            public string DebugDescription { get; set; }
            public string Json { get; set; }
            public EntityInstance ExpectedEntity { get; set; }
            public ExpectedException ExpectedException { get; set; }
            public IDictionary<string, bool?> ExpectedIsCollectionValues { get; set; }

            public PayloadReaderTestDescriptor ToTestDescriptor(PayloadReaderTestDescriptor.Settings settings, IEdmModel model, bool withMetadataAnnotation = false)
            {
                IEdmEntityContainer container = model.FindEntityContainer("DefaultContainer");
                IEdmEntitySet citiesEntitySet = container.FindEntitySet("Cities");
                IEdmType cityType = model.FindType("TestModel.CityType");

                EntityInstance entity = PayloadBuilder.Entity("TestModel.CityType")
                    .ExpectedEntityType(cityType, citiesEntitySet)
                    .JsonRepresentation(
                        "{" +
                        (withMetadataAnnotation ? ("\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://http://odata.org/test/$metadata#DefaultContainer.Cities/$entity\",") : string.Empty) +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.CityType\"," +
                            this.Json +
                            ",\"Id\":1" +
                        "}");

                foreach (PropertyInstance property in this.ExpectedEntity.Properties)
                {
                    entity.Add(property.DeepCopy());
                }

                entity.Add(PayloadBuilder.PrimitiveProperty("Id", 1));

                return new NavigationLinkTestCaseDescriptor(settings)
                {
                    DebugDescription = this.DebugDescription,
                    PayloadElement = entity,
                    PayloadEdmModel = model,
                    ExpectedException = this.ExpectedException,
                    ExpectedIsCollectionValues = this.ExpectedIsCollectionValues,
                };
            }
        }
    }
}
