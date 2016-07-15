//---------------------------------------------------------------------
// <copyright file="WriterPayloads.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Atom;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Json;
    #endregion Namespaces

    /// <summary>
    /// Helper methods to generate interesting writer payloads for specific item/property/value
    /// </summary>

    internal static class WriterPayloads
    {
        /// <summary>
        /// Returns all descriptors where for each test descriptor from the input all payload cases returned by the func are applied.
        /// </summary>
        /// <param name="testDescriptors">The test descriptors to use.</param>
        /// <param name="payloadCasesFunc">The payload case func to use.</param>
        /// <returns>Enumeration of all test descriptors.</returns>
        public static IEnumerable<PayloadWriterTestDescriptor<TOut>> PayloadCases<TIn, TOut>(
            this IEnumerable<PayloadWriterTestDescriptor<TIn>> testDescriptors,
            Func<PayloadWriterTestDescriptor<TIn>, IEnumerable<PayloadWriterTestDescriptor<TOut>>> payloadCasesFunc)
        {
            return testDescriptors.SelectMany(testDescriptor => payloadCasesFunc(testDescriptor));
        }

        /// <summary>
        /// Returns the <paramref name="testDescriptor"/> but modified to work as a top-level value payload.
        /// </summary>
        /// <param name="testDescriptor">The test descriptor to process.</param>
        /// <returns>The <paramref name="testDescriptor"/> suitable as top-level value payload.</returns>
        public static IEnumerable<PayloadWriterTestDescriptor<ODataItem>> TopLevelValuePayload(PayloadWriterTestDescriptor<ODataItem> testDescriptor)
        {
            var payloadCases = new WriterPayloadCase<ODataItem>[] 
            {
                // Top level entry
                new WriterPayloadCase<ODataItem>() 
                { 
                    JsonLightFragmentExtractor = JsonUtils.UnwrapTopLevelValue
                }
            };

            return ApplyPayloadCases<ODataItem>(testDescriptor, payloadCases);
        }

        /// <summary>
        /// Returns all interesting payloads for an entry.
        /// </summary>
        /// <param name="testDescriptor">The test descriptor which will end up writing a single entry.</param>
        /// <returns>Enumeration of test descriptors which will include the original entry in some interesting scenarios.</returns>
        public static IEnumerable<PayloadWriterTestDescriptor<ODataItem>> EntryPayloads(PayloadWriterTestDescriptor<ODataItem> testDescriptor)
        {
            Debug.Assert(testDescriptor.PayloadItems[0] is ODataResource, "The payload does not specify an entry.");

            var payloadCases = new WriterPayloadCase<ODataItem>[] 
            {
                // Feed with a single entry
                new WriterPayloadCase<ODataItem>() {
                    GetPayloadItems = () => new ODataItem[] { ObjectModelUtils.CreateDefaultFeed() }.Concat(testDescriptor.PayloadItems),
                    AtomFragmentExtractor = (testConfiguration, result) =>
                        {
                            return result.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomEntryElementName).First();
                        },
                        //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
                },

                // Feed with three (identical) entries, picking the second one
                new WriterPayloadCase<ODataItem>() {
                    GetPayloadItems = () => new ODataItem[] 
                    { 
                        ObjectModelUtils.CreateDefaultFeed() 
                    }
                    .Concat(testDescriptor.PayloadItems)
                    .Concat(LinqExtensions.FromSingle((ODataItem)null))
                    .Concat(testDescriptor.PayloadItems)
                    .Concat(LinqExtensions.FromSingle((ODataItem)null))
                    .Concat(testDescriptor.PayloadItems)
                    .Concat(LinqExtensions.FromSingle((ODataItem)null)),
                    AtomFragmentExtractor = (testConfiguration, result) =>
                        {
                            var entries = result.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomEntryElementName);
                            Debug.Assert(entries.Count() == 3, "Expected three entries in the feed.");
                            return entries.ElementAt(2);
                        },
                        //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
                },

                // Top-level entry with an expanded link containing a single entry
                new WriterPayloadCase<ODataItem>() {
                    GetPayloadItems = () => new ODataItem[] 
                    { 
                        ObjectModelUtils.CreateDefaultEntry(),
                        ObjectModelUtils.CreateDefaultSingletonLink(),
                    }.Concat(testDescriptor.PayloadItems),
                    AtomFragmentExtractor = (testConfiguration, result) =>
                        {
                            return result.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                .First(e => e.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInlineElementName) != null)
                                .Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInlineElementName)
                                .Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomEntryElementName);
                        },
                        //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
                },

                // Top-level entry with an expanded link containing a feed with a single entry
                new WriterPayloadCase<ODataItem>() {
                    GetPayloadItems = () => new ODataItem[] 
                    { 
                        ObjectModelUtils.CreateDefaultEntry(),
                        ObjectModelUtils.CreateDefaultCollectionLink(),
                        ObjectModelUtils.CreateDefaultFeed()
                    }.Concat(testDescriptor.PayloadItems),
                    AtomFragmentExtractor = (testConfiguration, result) =>
                        {
                            return result.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                .First(e => e.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInlineElementName) != null)
                                .Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInlineElementName)
                                .Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomFeedElementName)
                                .Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomEntryElementName);
                        },
                        //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
                },

                // Top-level entry with an expanded link containing a feed with three entries; picking the second one
                new WriterPayloadCase<ODataItem>() {
                    GetPayloadItems = () => new ODataItem[] 
                    { 
                        ObjectModelUtils.CreateDefaultEntry(),
                        ObjectModelUtils.CreateDefaultCollectionLink(),
                        ObjectModelUtils.CreateDefaultFeed()
                    }
                    .Concat(testDescriptor.PayloadItems)
                    .Concat(LinqExtensions.FromSingle((ODataItem)null))
                    .Concat(testDescriptor.PayloadItems)
                    .Concat(LinqExtensions.FromSingle((ODataItem)null))
                    .Concat(testDescriptor.PayloadItems)
                    .Concat(LinqExtensions.FromSingle((ODataItem)null)),
                    AtomFragmentExtractor = (testConfiguration, result) =>
                        {
                            return result.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                .First(e => e.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInlineElementName) != null)
                                .Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInlineElementName)
                                .Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomFeedElementName)
                                .Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomEntryElementName)
                                .ElementAt(2);
                        },
                        //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
                },

                // Top-level entry with an expanded link containing a feed with a single entry;
                // that entry has another expanded link containing a feed with the payload entry
                new WriterPayloadCase<ODataItem>() {
                    GetPayloadItems = () => new ODataItem[] 
                    { 
                        ObjectModelUtils.CreateDefaultEntry(),
                        ObjectModelUtils.CreateDefaultCollectionLink(),
                        ObjectModelUtils.CreateDefaultFeed(),
                        ObjectModelUtils.CreateDefaultEntry(),
                        ObjectModelUtils.CreateDefaultCollectionLink(),
                        ObjectModelUtils.CreateDefaultFeed(),
                    }.Concat(testDescriptor.PayloadItems),
                    AtomFragmentExtractor = (testConfiguration, result) =>
                        {
                            return result.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                .First(e => e.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInlineElementName) != null)
                                .Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInlineElementName)
                                .Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomFeedElementName)
                                .Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomEntryElementName)
                                .Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                .First(e => e.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInlineElementName) != null)
                                .Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInlineElementName)
                                .Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomFeedElementName)
                                .Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomEntryElementName);
                        },
                        //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
                },
            };

            return ApplyPayloadCases<ODataItem>(testDescriptor, payloadCases);
        }

        /// <summary>
        /// Returns all interesting payloads for a feed.
        /// </summary>
        /// <param name="testDescriptor">The test descriptor which will end up writing a single feed.</param>
        /// <returns>Enumeration of test descriptors which will include the original feed in some interesting scenarios.</returns>
        public static IEnumerable<PayloadWriterTestDescriptor<ODataItem>> FeedPayloads(PayloadWriterTestDescriptor<ODataItem> testDescriptor)
        {
            Debug.Assert(testDescriptor.PayloadItems[0] is ODataResourceSet, "The payload does not specify a feed.");

            var payloadCases = new WriterPayloadCase<ODataItem>[] 
            {
                // Top-level feed with an entry that has an expanded link containing the feed
                new WriterPayloadCase<ODataItem>() 
                {
                    GetPayloadItems = () => new ODataItem[] 
                    {
                        ObjectModelUtils.CreateDefaultFeed(),
                        ObjectModelUtils.CreateDefaultEntry(),
                        ObjectModelUtils.CreateDefaultCollectionLink(),
                    }.Concat(testDescriptor.PayloadItems),
                    AtomFragmentExtractor = (testConfiguration, result) =>
                        {
                            return result.Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomEntryElementName)
                                .Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                .First(e => e.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInlineElementName) != null)
                                .Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInlineElementName)
                                .Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomFeedElementName);
                        },
                        //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
                },

                // Top-level entry with an expanded link containing the feed
                new WriterPayloadCase<ODataItem>() 
                {
                    GetPayloadItems = () => new ODataItem[] 
                    { 
                        ObjectModelUtils.CreateDefaultEntry(),
                        ObjectModelUtils.CreateDefaultCollectionLink(),
                    }.Concat(testDescriptor.PayloadItems),
                    AtomFragmentExtractor = (testConfiguration, result) =>
                        {
                            return result.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                .First(e => e.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInlineElementName) != null)
                                .Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInlineElementName)
                                .Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomFeedElementName);
                        },
                        //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
                },

                // Top-level entry with an expanded link containing a feed with a single entry;
                // that entry has another expanded link containing the payload feed
                new WriterPayloadCase<ODataItem>() {
                    GetPayloadItems = () => new ODataItem[] 
                    { 
                        ObjectModelUtils.CreateDefaultEntry(),
                        ObjectModelUtils.CreateDefaultCollectionLink(),
                        ObjectModelUtils.CreateDefaultFeed(),
                        ObjectModelUtils.CreateDefaultEntry(),
                        ObjectModelUtils.CreateDefaultCollectionLink(),
                    }.Concat(testDescriptor.PayloadItems),
                    AtomFragmentExtractor = (testConfiguration, result) =>
                        {
                            return result.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                .First(e => e.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInlineElementName) != null)
                                .Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInlineElementName)
                                .Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomFeedElementName)
                                .Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomEntryElementName)
                                .Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                .First(e => e.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInlineElementName) != null)
                                .Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInlineElementName)
                                .Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomFeedElementName);
                        },
                        //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
                },
            };

            return ApplyPayloadCases<ODataItem>(testDescriptor, payloadCases);
        }

        /// <summary>
        /// Returns all interesting payloads for a property.
        /// </summary>
        /// <param name="testDescriptor">Test descriptor which will end up writing a single entry with a single property.
        /// The entry is not going to be used, but the property from it will.</param>
        /// <returns>Enumeration of test descriptors which will include the original property in some interesting scenarios.</returns>
        public static IEnumerable<PayloadWriterTestDescriptor<ODataItem>> PropertyPayloads(PayloadWriterTestDescriptor<ODataItem> testDescriptor)
        {
            ODataResource tempEntry = testDescriptor.PayloadItems[0] as ODataResource;
            Debug.Assert(tempEntry != null, "A single entry payload is expected.");
            ODataProperty property = tempEntry.Properties.First();

            // Note - the property can be null - it is a valid test case !!!!

            var payloadCases = new WriterPayloadCase<ODataItem>[] {
                new WriterPayloadCase<ODataItem>() { // Single property on an entry
                    GetPayloadItems = () => { 
                        ODataResource entry = ObjectModelUtils.CreateDefaultEntry(); 
                        entry.Properties = new ODataProperty[] { property }; 
                        return new ODataItem[] { entry }; },
                    AtomFragmentExtractor = (testConfiguration, result) =>
                        {
                            return TestAtomUtils.ExtractPropertiesFromEntry(result).Element(TestAtomConstants.ODataXNamespace + property.Name);
                        },
                    //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
                },

                // TODO: Add other interesting payloads for properties - more properties in an entry, inside a complex property, inside a collection of complex and so on
            };

            return ApplyPayloadCases<ODataItem>(testDescriptor, payloadCases);
        }

        /// <summary>
        /// Returns all interesting payloads for a value.
        /// </summary>
        /// <param name="testDescriptor">Test descriptor which will end up writing a single entry with a single property.
        /// The entry is not going to be used, the property is not going to be used, but the property value from it will.</param>
        /// <returns>Enumeration of test descriptors which will include the original value in some interesting scenarios.</returns>
        public static IEnumerable<PayloadWriterTestDescriptor<ODataItem>> ValuePayloads(PayloadWriterTestDescriptor<ODataItem> testDescriptor)
        {
            ODataResource tempEntry = testDescriptor.PayloadItems[0] as ODataResource;
            Debug.Assert(tempEntry != null, "A single entry payload is expected.");
            ODataProperty property = tempEntry.Properties.First();
            Debug.Assert(property != null, "A single property is expected.");
            object propertyValue = property.Value;

            var payloadCases = new WriterPayloadCase<ODataItem>[] {
                new WriterPayloadCase<ODataItem>() { // Value of a property
                    GetPayloadItems = () => {
                        ODataResource entry = ObjectModelUtils.CreateDefaultEntry(); 
                        entry.Properties = new ODataProperty[] { new ODataProperty() { Name = "TestProperty", Value = propertyValue } }; 
                        return new ODataItem[] { entry }; },
                    AtomFragmentExtractor = (testConfiguration, result) =>
                        {
                            return TestAtomUtils.ExtractPropertiesFromEntry(result).Element(TestAtomConstants.ODataXNamespace + property.Name);
                        },
                    //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
                },
                new WriterPayloadCase<ODataItem>() { // Value as item in a collection
                    ShouldSkip = testConfiguration => propertyValue is ODataCollectionValue,
                    GetPayloadItems = () => {
                        ODataResource entry = ObjectModelUtils.CreateDefaultEntry(); 
                        entry.Properties = new ODataProperty[] { new ODataProperty() { Name = "TestProperty", Value = new ODataCollectionValue() { Items = new object[] { propertyValue } } } }; 
                        return new ODataItem[] { entry }; },
                    AtomFragmentExtractor = (testConfiguration, result) =>
                        {
                            return TestAtomUtils.ExtractPropertiesFromEntry(result).Element(TestAtomConstants.ODataXNamespace + property.Name);
                        },
                    //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
                },

                // TODO: Add other interesting payloads for property values
            };

            // Combine with property payloads to get interesting places where the property itself is used.
            return ApplyPayloadCases<ODataItem>(testDescriptor, payloadCases).PayloadCases(PropertyPayloads);
        }

        /// <summary>
        /// Returns all interesting payloads for a named stream.
        /// </summary>
        /// <param name="testDescriptor">Test descriptor which will end up writing a single entry with a single named stream.
        /// The entry is not going to be used, but the named stream from it will.</param>
        /// <returns>Enumeration of test descriptors which will include the original named stream in some interesting scenarios.</returns>
        public static IEnumerable<PayloadWriterTestDescriptor<ODataItem>> NamedStreamPayloads(PayloadWriterTestDescriptor<ODataItem> testDescriptor)
        {
            ODataResource tempEntry = testDescriptor.PayloadItems[0] as ODataResource;
            Debug.Assert(tempEntry != null, "A single entry payload is expected.");
            ODataProperty namedStreamProperty = tempEntry.Properties.FirstOrDefault(p => p != null && p.Value is ODataStreamReferenceValue);

            // Note - the named stream can be null - it is a valid test case !!!!

            var payloadCases = new WriterPayloadCase<ODataItem>[] {
                new WriterPayloadCase<ODataItem>() { // Single named stream on an entry
                    GetPayloadItems = () => { 
                        ODataResource entry = ObjectModelUtils.CreateDefaultEntry(); 
                        entry.TypeName = "TestModel.EntityWithStreamProperty";
                        entry.Properties = new ODataProperty[] { namedStreamProperty }; 
                        return new ODataItem[] { entry }; },
                    ModelBuilder = (model) => 
                        {
                            model = model.Clone();
                            model.EntityType("EntityWithStreamProperty", "TestModel")
                                .StreamProperty(namedStreamProperty.Name);
                            return model;
                        },
                    AtomFragmentExtractor = (testConfiguration, result) =>
                        {
                            return TestAtomUtils.ExtractNamedStreamLinksFromEntry(result, namedStreamProperty.Name);
                        },
                    JsonLightFragmentExtractor = (testConfiguration, result) =>
                        {
                            return new JsonObject().AddProperties(result.Object().GetPropertyAnnotationsAndProperty(namedStreamProperty.Name));
                        },
                },

                new WriterPayloadCase<ODataItem>() { // Single named stream on an entry with other properties before/after it
                    GetPayloadItems = () => { 
                        ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                        entry.TypeName = "TestModel.EntityWithStreamPropertyAndOtherProperties";
                        entry.Properties = new ODataProperty[] 
                        { 
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Name", Value = "Clemens" },
                            namedStreamProperty,
                            new ODataProperty { 
                                Name = "Address", 
                                Value = new ODataComplexValue 
                                { 
                                    TypeName = "TestModel.AddressType",
                                    Properties = new ODataProperty[] 
                                    { 
                                        new ODataProperty { Name ="Street", Value = "Am Euro Platz"},
                                        new ODataProperty { Name ="City", Value = "Vienna"}
                                    } 
                                }
                            }
                        }; 
                        return new ODataItem[] { entry }; },
                    ModelBuilder = (model) => 
                        {
                            model = model.Clone();
                            var addressType = model.ComplexType("AddressType", "TestModel")
                                .Property("Street", EdmPrimitiveTypeKind.String)
                                .Property("City", EdmPrimitiveTypeKind.String);
                            model.EntityType("EntityWithStreamPropertyAndOtherProperties", "TestModel")
                                .KeyProperty("Id", (EdmTypeReference)EdmCoreModel.Instance.GetInt32(false))
                                .Property("Name", EdmPrimitiveTypeKind.String)
                                .StreamProperty(namedStreamProperty.Name)
                                .Property("Address", new EdmComplexTypeReference(addressType, false));
                            return model;
                        },
                    AtomFragmentExtractor = (testConfiguration, result) =>
                        {
                            return TestAtomUtils.ExtractNamedStreamLinksFromEntry(result, namedStreamProperty.Name);
                        },
                    JsonLightFragmentExtractor = (testConfiguration, result) =>
                        {
                            return new JsonObject().AddProperties(result.Object().GetPropertyAnnotationsAndProperty(namedStreamProperty.Name));
                        },
                },

                new WriterPayloadCase<ODataItem>() { // Multiple named stream properties on an entry
                    GetPayloadItems = () => { 
                        ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                        entry.TypeName = "TestModel.EntityWithSeveralStreamProperties";
                        entry.Properties = new ODataProperty[] 
                        { 
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "__Stream1", Value = new ODataStreamReferenceValue { ReadLink = new Uri("http://odata.org/stream1/readlink") } },
                            new ODataProperty { Name = "__Stream2", Value = new ODataStreamReferenceValue { EditLink = new Uri("http://odata.org/stream2/editlink") } },
                            namedStreamProperty,
                            new ODataProperty { Name = "__Stream3", Value = new ODataStreamReferenceValue { ReadLink = new Uri("http://odata.org/stream3/readlink"), ContentType = "stream3:contenttype" } },
                        }; 
                        return new ODataItem[] { entry }; },
                    ModelBuilder = (model) => 
                        {
                            model = model.Clone();
                            model.EntityType("EntityWithSeveralStreamProperties", "TestModel")
                                .KeyProperty("Id", (EdmTypeReference)EdmCoreModel.Instance.GetInt32(false))
                                .StreamProperty("__Stream1")
                                .StreamProperty("__Stream2")
                                .StreamProperty(namedStreamProperty.Name)
                                .StreamProperty("__Stream3");
                            return model;
                        },
                    AtomFragmentExtractor = (testConfiguration, result) =>
                        {
                            return TestAtomUtils.ExtractNamedStreamLinksFromEntry(result, namedStreamProperty.Name);
                        },
                    JsonLightFragmentExtractor = (testConfiguration, result) =>
                        {
                            return new JsonObject().AddProperties(result.Object().GetPropertyAnnotationsAndProperty(namedStreamProperty.Name));
                        },
                },
            };

            return ApplyPayloadCases<ODataItem>(testDescriptor, payloadCases);
        }

        /// <summary>
        /// Returns all interesting payloads for a navigation link itself. That is the ODataNestedResourceInfo without any subsequent events.
        /// </summary>
        /// <param name="testDescriptor">Test descriptor which will end up writing a single navigation link.</param>
        /// <returns>Enumeration of test descriptors which will include the original navigation link in some interesting scenarios.</returns>
        public static IEnumerable<PayloadWriterTestDescriptor<ODataItem>> NavigationLinkOnlyPayloads(PayloadWriterTestDescriptor<ODataItem> testDescriptor)
        {
            ODataNestedResourceInfo navigationLink = testDescriptor.PayloadItems[0] as ODataNestedResourceInfo;
            Debug.Assert(navigationLink != null, "Navigation link payload expected.");
            Debug.Assert(navigationLink.IsCollection.HasValue, "ODataNestedResourceInfo.IsCollection required.");

            var payloadCases = new WriterPayloadCase<ODataItem>[] {
                new WriterPayloadCase<ODataItem>() { // Just the link as non-expanded
                    GetPayloadItems = () => new ODataItem[] { navigationLink }
                },
                new WriterPayloadCase<ODataItem>() { // The link with expanded entry
                    GetPayloadItems = () => {
                        if (navigationLink.IsCollection.Value)
                        {
                            return new ODataItem[] { navigationLink, ObjectModelUtils.CreateDefaultFeed() };
                        }
                        else
                        {
                            return new ODataItem[] { navigationLink, ObjectModelUtils.CreateDefaultEntry() };
                        }
                    }
                }
            };

            // Apply the cases here and then wrap the link in some entry/feed cases
            return ApplyPayloadCases<ODataItem>(testDescriptor, payloadCases).PayloadCases(NavigationLinkPayloads);
        }

        /// <summary>
        /// Returns all interesting payloads for a navigation link.
        /// </summary>
        /// <param name="testDescriptor">Test descriptor which will end up writing a single navigation link.</param>
        /// <returns>Enumeration of test descriptors which will include the original navigation link in some interesting scenarios.</returns>
        public static IEnumerable<PayloadWriterTestDescriptor<ODataItem>> NavigationLinkPayloads(PayloadWriterTestDescriptor<ODataItem> testDescriptor)
        {
            ODataNestedResourceInfo navigationLink = testDescriptor.PayloadItems[0] as ODataNestedResourceInfo;
            Debug.Assert(navigationLink != null, "Link payload expected.");

            var payloadCases = new WriterPayloadCase<ODataItem>[] {
                new WriterPayloadCase<ODataItem>() { // Single link on top-level entry
                    GetPayloadItems = () => new ODataItem[] { ObjectModelUtils.CreateDefaultEntry() }.Concat(testDescriptor.PayloadItems),
                    AtomFragmentExtractor = (testConfiguration, result) =>
                        {
                            return result.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                .Where(linkElement => linkElement.Attribute(TestAtomConstants.AtomLinkRelationAttributeName).Value.StartsWith(TestAtomConstants.ODataNavigationPropertiesRelatedLinkRelationPrefix))
                                .First();
                        },
                },

                // TODO: Add other interesting payloads for links - in expanded entry, in expanded feed
            };

            return ApplyPayloadCases<ODataItem>(testDescriptor, payloadCases);
        }

        /// <summary>
        /// Applies payload cases to a test descriptor.
        /// </summary>
        /// <param name="testDescriptor">The test descriptor to use.</param>
        /// <param name="payloadCases">The payload cases to use.</param>
        /// <returns>Enumeration of test descriptors which are results of applying all the payload cases to the specified test descriptor.</returns>
        private static IEnumerable<PayloadWriterTestDescriptor<T>> ApplyPayloadCases<T>(PayloadWriterTestDescriptor<T> testDescriptor, IEnumerable<WriterPayloadCase<T>> payloadCases)
        {
            return payloadCases.Select(payloadCase => payloadCase.ApplyToTestDescriptor(testDescriptor));
        }

        /// <summary>
        /// Applies payload cases to a test descriptor.
        /// </summary>
        /// <param name="testDescriptor">The test descriptor to use.</param>
        /// <param name="payloadCases">The payload cases to use.</param>
        /// <returns>Enumeration of test descriptors which are results of applying all the payload cases to the specified test descriptor.</returns>
        private static IEnumerable<PayloadWriterTestDescriptor<TOut>> ApplyPayloadCases<TIn, TOut>(PayloadWriterTestDescriptor<TIn> testDescriptor, IEnumerable<WriterPayloadCase<TIn, TOut>> payloadCases)
        {
            return payloadCases.Select(payloadCase => payloadCase.ApplyToTestDescriptor(testDescriptor));
        }

        /// <summary>
        /// Declares a helper class which stores information about a payload case used to modify original test descriptors
        /// </summary>
        private class WriterPayloadCase<TIn, TOut>
        {
            /// <summary>
            /// If specified the func is called for a given configuration and if it returns true, the payload case for such configuration will be skipped.
            /// </summary>
            public Func<WriterTestConfiguration, bool> ShouldSkip { get; set; }

            /// <summary>
            /// If specified the func will be called to get the payload items programatically
            /// It's the responsibility of the caller to include the original test case items in this collection
            /// </summary>
            public Func<IEnumerable<TOut>> GetPayloadItems { get; set; }

            /// <summary>
            /// Set to true for cases which don't really change the payload at all, so there's nothing generated about them.
            /// </summary>
            public bool NotGenerated { get; set; }

            /// <summary>
            /// If specified this func will be called to extract the ATOM fragment from the modified payload such that the original test case can work with the result.
            /// </summary>
            public Func<WriterTestConfiguration, XElement, XElement> AtomFragmentExtractor { get; set; }

            /// <summary>
            /// If specified this func will be called to extract the JSON Light fragment from the modified payload such that the original test case can work with the result.
            /// </summary>
            public Func<WriterTestConfiguration, JsonValue, JsonValue> JsonLightFragmentExtractor { get; set; }

            /// <summary>
            /// If specified and the test descriptor has a model, this func is called to build a model for the new test case.
            /// </summary>
            public Func<EdmModel, EdmModel> ModelBuilder { get; set; }

            /// <summary>
            /// Applies the payload case to a test descriptor returning a new test descriptor.
            /// </summary>
            /// <param name="testDescriptor">The test descriptor to apply the case to.</param>
            /// <returns>The new test descriptor.</returns>
            internal PayloadWriterTestDescriptor<TOut> ApplyToTestDescriptor(PayloadWriterTestDescriptor<TIn> testDescriptor)
            {
                IEnumerable<TOut> payloadItems = null;
                if (this.GetPayloadItems != null) 
                {
                    payloadItems = this.GetPayloadItems();
                }
                else
                {
                    // for the test descriptor to specify payload items TIn and TOut have to be compatible!
                    Type inputType = typeof(TIn);
                    Type outputType = typeof(TOut);
                    if (inputType != outputType && !outputType.IsAssignableFrom(inputType))
                    {
                        throw new NotSupportedException(
                            "The test descriptor can only specify payload items when they are compatible with the expected output type. " + 
                            inputType.FullName + " is not compatible with " + outputType.FullName + ".");
                    }
                        
                    payloadItems = testDescriptor.PayloadItems.Cast<TOut>();
                }

                EdmModel model = (EdmModel)testDescriptor.Model;
                IEdmElement payloadElementModelContainer = testDescriptor.PayloadEdmElementContainer;
                IEdmElement payloadElementType = testDescriptor.PayloadEdmElementType;
                if (model != null && this.ModelBuilder != null)
                {
                    model = this.ModelBuilder(model);
                }

                return new PayloadWriterTestDescriptor<TOut>(
                    testDescriptor.TestDescriptorSettings,
                    payloadItems,
                    (testConfiguration) =>
                    {
                        if (this.ShouldSkip != null && this.ShouldSkip(testConfiguration)) return null;

                        WriterTestExpectedResults expectedResults = testDescriptor.ExpectedResultCallback(testConfiguration);
                        AtomWriterTestExpectedResults atomResults = expectedResults as AtomWriterTestExpectedResults;
                        if (atomResults != null)
                        {
                            return new AtomWriterTestExpectedResults(atomResults)
                            {
                                FragmentExtractor = this.AtomFragmentExtractor == null ? atomResults.FragmentExtractor :
                                    (result) => atomResults.FragmentExtractor(this.AtomFragmentExtractor(testConfiguration, result)),
                            };
                        }

                        JsonWriterTestExpectedResults jsonResults = expectedResults as JsonWriterTestExpectedResults;
                        if (jsonResults != null)
                        {
                            return new JsonWriterTestExpectedResults(jsonResults)
                            {
                                FragmentExtractor = this.JsonLightFragmentExtractor == null ? jsonResults.FragmentExtractor :
                                    (result) => jsonResults.FragmentExtractor(this.JsonLightFragmentExtractor(testConfiguration, result)),
                            };
                        }

                        return expectedResults;
                    })
                    {
                        SkipTestConfiguration = testDescriptor.SkipTestConfiguration,
                        IsGeneratedPayload = !this.NotGenerated,
                        Model = model,
                        PayloadEdmElementContainer = payloadElementModelContainer,
                        PayloadEdmElementType = payloadElementType,
                    };
            }
        }

        private class WriterPayloadCase<T> : WriterPayloadCase<T, T>
        {
        }
    }
}
