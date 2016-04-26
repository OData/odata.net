//---------------------------------------------------------------------
// <copyright file="WriterProjectedPropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for writing entries with projected properties annotations.
    /// </summary>
    [TestClass, TestCase]
    public class WriterProjectedPropertyTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");
        private static readonly ODataResourceSerializationInfo MySerializationInfo = new ODataResourceSerializationInfo()
        {
            ExpectedTypeName = "TestModel.EntityType",
            NavigationSourceEntityTypeName = "TestModel.EntityType",
            NavigationSourceName = "MySet"
        };
        private const string DefaultNamespaceName = "TestModel";
        private static readonly IEdmStringTypeReference StringTypeRef = EdmCoreModel.Instance.GetString(isNullable: false);
        private static readonly IEdmPrimitiveTypeReference Int32TypeRef = EdmCoreModel.Instance.GetInt32(isNullable: false);
        private static readonly IEdmPrimitiveTypeReference Int32NullableTypeRef = EdmCoreModel.Instance.GetInt32(isNullable: true);

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        // We use InternalsVisibleTo here to access internal properties which is not supported for Silverlight or Phone
#if !SILVERLIGHT && !WINDOWS_PHONE
        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies that if projected properties are specified, they are correctly written/skipped.")]
        public void ProjectedPropertiesTest()
        {
            // Start with top-level payloads
            IEnumerable<ProjectedPropertiesTestCase> topLevelTestCases = new ProjectedPropertiesTestCase[]
            {
                // No point testing case where no annotation is specified, as all the other tests do that.
                // It's important to sort these in the payload order, so nav props go first, then named streams and then normal properties
                // Also note that assoc link and nav link might have the same name, in which case they will be twice in the list below
                new ProjectedPropertiesTestCase {
                    TopLevelProjectedProperties = new ProjectedPropertiesAnnotation(new string[0]),
                    ExpectedProperties = new string[0],
                },
                new ProjectedPropertiesTestCase {
                    TopLevelProjectedProperties = new ProjectedPropertiesAnnotation(new string[] { "StringProperty" }),
                    ExpectedProperties = new string[] { "StringProperty" },
                },
                new ProjectedPropertiesTestCase {
                    TopLevelProjectedProperties = new ProjectedPropertiesAnnotation(new string[] { "StringProperty", "NumberProperty" }),
                    ExpectedProperties = new string[] { "StringProperty", "NumberProperty" },
                },
                new ProjectedPropertiesTestCase {
                    TopLevelProjectedProperties = new ProjectedPropertiesAnnotation(new string[] { "SimpleComplexProperty", "PrimitiveCollection" }),
                    ExpectedProperties = new string[] { "SimpleComplexProperty", "PrimitiveCollection" },
                },
                new ProjectedPropertiesTestCase {
                    TopLevelProjectedProperties = new ProjectedPropertiesAnnotation(new string[] { "NamedStream" }),
                    ExpectedProperties = new string[] { "NamedStream" },
                    ResponseOnly = true,
                },
                new ProjectedPropertiesTestCase {
                    TopLevelProjectedProperties = new ProjectedPropertiesAnnotation(new string[] { "AssociationLink" }),
                    ExpectedProperties = new string[] { "AssociationLink" },
                    ResponseOnly = true,
                    IgnoreFormats = new ODataFormat[] { ODataFormat.Json },
                },
                new ProjectedPropertiesTestCase {
                    TopLevelProjectedProperties = new ProjectedPropertiesAnnotation(new string[] { "NamedStream", "StringProperty", "NumberProperty", "SimpleComplexProperty", "DeepComplexProperty", "PrimitiveCollection", "ComplexCollection" }),
                    ExpectedProperties = new string[] { "NamedStream", "StringProperty", "NumberProperty", "SimpleComplexProperty", "DeepComplexProperty", "PrimitiveCollection", "ComplexCollection" },
                    ResponseOnly = true,
                },
                new ProjectedPropertiesTestCase {
                    TopLevelProjectedProperties = new ProjectedPropertiesAnnotation(new string[] { "DeferredNavigation" }),
                    ExpectedProperties = new string[] { "DeferredNavigation", "DeferredNavigation" },
                    ResponseOnly = true,
                },
                new ProjectedPropertiesTestCase {
                    TopLevelProjectedProperties = new ProjectedPropertiesAnnotation(new string[] { "ExpandedEntry" }),
                    ExpectedProperties = new string[] { "ExpandedEntry" },
                },
                new ProjectedPropertiesTestCase {
                    TopLevelProjectedProperties = new ProjectedPropertiesAnnotation(new string[] { "ExpandedFeed" }),
                    ExpectedProperties = new string[] { "ExpandedFeed" },
                },
            };

            // Then create wrapped test cases 
            var nestedTestCases = topLevelTestCases.Select(t =>
                new ProjectedPropertiesTestCase(t)
                {
                    NestedPayload = true,
                    NestedProjectedProperties = t.TopLevelProjectedProperties,
                    TopLevelProjectedProperties = new ProjectedPropertiesAnnotation(new string[] { "Wrapping_ExpandedEntry" }),
                });

            // Create more interesting test cases with multiple annotations
            var manualTestCases = new ProjectedPropertiesTestCase[]
            {
                new ProjectedPropertiesTestCase {
                    DebugDescription = "Non-existant name in top-level annotation is ignored.",
                    TopLevelProjectedProperties = new ProjectedPropertiesAnnotation(new string[] { "NumberProperty", "NamedStream", "NamedStream", "DeferredNavigation", "ExpandedEntry", "ExpandedFeed", "NonExistant" }),
                    ExpectedProperties = new string[] { "DeferredNavigation", "ExpandedEntry", "ExpandedFeed", "NamedStream", "DeferredNavigation", "NumberProperty" },
                    ResponseOnly = true,
                },
                new ProjectedPropertiesTestCase {
                    DebugDescription = "Make sure a top-level annotation with invalid property names or paths projects nothing.",
                    TopLevelProjectedProperties = new ProjectedPropertiesAnnotation(new string[] { string.Empty, null, "Invalid", "SimpleComplexProperty/Name" }),
                    ExpectedProperties = new string[] { },
                },
                new ProjectedPropertiesTestCase {
                    DebugDescription = "Make sure a nested annotation works with a top-level annotation.",
                    NestedPayload = true,
                    TopLevelProjectedProperties = new ProjectedPropertiesAnnotation(new string[] { "Wrapping_ExpandedEntry" }),
                    NestedProjectedProperties = new ProjectedPropertiesAnnotation(new string[] { "StringProperty", "NumberProperty" }),
                    ExpectedProperties = new string[] { "StringProperty", "NumberProperty" },
                },
                new ProjectedPropertiesTestCase {
                    DebugDescription = "Make sure a nested annotation is ignored if a top-level annotation does not include the navigation property.",
                    NestedPayload = true,
                    TopLevelProjectedProperties = new  ProjectedPropertiesAnnotation(new string[] { "Invalid" }),
                    NestedProjectedProperties = new ProjectedPropertiesAnnotation(new string[] { "StringProperty", "NumberProperty" }),
                    ExpectedProperties = new string[0],
                },
            };

            var testCases = topLevelTestCases.Concat(nestedTestCases).Concat(manualTestCases);

            IEdmModel edmModel = this.CreateEdmModel();

            // First create test descriptors that use the model
            var testDescriptors = this.CreateEdmTestDescriptors(testCases, edmModel);

            // Then append the test descriptors without model
            testDescriptors = testDescriptors.Concat(this.CreateEdmTestDescriptors(testCases, /*model*/null));

            //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.TopLevelValuePayload),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => false),
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataEdmPayload(testDescriptor.DeferredLinksToEntityReferenceLinksInRequest(testConfiguration), testConfiguration, this.Assert, this.Logger);
                });
        }

        private IEnumerable<PayloadWriterTestDescriptor<ODataItem>> CreateEdmTestDescriptors(IEnumerable<ProjectedPropertiesTestCase> testCases, IEdmModel model)
        {
            EdmEntitySet entitySet = null;
            EdmEntitySet wrappingEntitySet = null;

            if (model != null)
            {
                var container = model.FindEntityContainer("DefaultContainer");
                entitySet = container.FindEntitySet("EntitySet") as EdmEntitySet;
                wrappingEntitySet = container.FindEntitySet("WrappingEntitySet") as EdmEntitySet;

            }

            return testCases.Select(testCase =>
            {
                var payload = this.CreatePayload(testCase);
                string typeName = ((ODataResource)payload.First()).TypeName;

                EdmEntitySet containerSet;
                switch (typeName)
                {
                    case "TestModel.EntityType":
                        containerSet = entitySet;
                        break;
                    case "TestModel.WrappingEntityType":
                        containerSet = wrappingEntitySet;
                        break;
                    default:
                        throw new TaupoInvalidOperationException("Unexpected type name: " + typeName + ".");
                }

                return new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    payload,
                    (tc) => this.CreateExpectedResults(tc, testCase, /*withModel*/model != null))
                {
                    DebugDescription = testCase.DebugDescription + (model == null ? " (without model)" : " (with model)"),
                    Model = model,
                    PayloadEdmElementContainer = containerSet,
                    SkipTestConfiguration = tc =>
                        testCase.ResponseOnly && tc.IsRequest ||
                        (model == null && tc.Format == ODataFormat.Json) ||
                        (testCase.IgnoreFormats != null && testCase.IgnoreFormats.Contains(tc.Format))
                };
            });
        }

        private IEdmModel CreateEdmModel()
        {
            var model = new EdmModel();

            EdmComplexType simpleComplexType = new EdmComplexType(DefaultNamespaceName, "SimplexComplexType");
            simpleComplexType.AddProperty(new EdmStructuralProperty(simpleComplexType, "Name", StringTypeRef));
            model.AddElement(simpleComplexType);

            EdmComplexType simpleComplexType2 = new EdmComplexType(DefaultNamespaceName, "SimplexComplexType2");
            simpleComplexType2.AddProperty(new EdmStructuralProperty(simpleComplexType2, "Value", Int32NullableTypeRef));
            model.AddElement(simpleComplexType2);

            EdmComplexType nestedComplexType = new EdmComplexType(DefaultNamespaceName, "NestedComplexType");
            nestedComplexType.AddProperty(new EdmStructuralProperty(nestedComplexType, "InnerComplexProperty", new EdmComplexTypeReference(simpleComplexType2, isNullable: false)));
            model.AddElement(nestedComplexType);

            EdmComplexType ratingComplexType = new EdmComplexType(DefaultNamespaceName, "RatingComplexType");
            ratingComplexType.AddProperty(new EdmStructuralProperty(ratingComplexType, "Rating", Int32NullableTypeRef));
            model.AddElement(ratingComplexType);

            EdmEntityType entityType = new EdmEntityType(DefaultNamespaceName, "EntityType");

            EdmEntityType expandedEntryType = new EdmEntityType(DefaultNamespaceName, "ExpandedEntryType");
            expandedEntryType.AddKeys(expandedEntryType.AddStructuralProperty("Id", Int32TypeRef));
            expandedEntryType.AddStructuralProperty("ExpandedEntryName", StringTypeRef);
            expandedEntryType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "ExpandedEntry_DeferredNavigation", Target = entityType, TargetMultiplicity = EdmMultiplicity.One });
            expandedEntryType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "ExpandedEntry_ExpandedFeed", Target = entityType, TargetMultiplicity = EdmMultiplicity.Many });
            model.AddElement(expandedEntryType);

            entityType.AddKeys(entityType.AddStructuralProperty("Id", Int32TypeRef));
            entityType.AddStructuralProperty("StringProperty", StringTypeRef);
            entityType.AddStructuralProperty("NumberProperty", Int32TypeRef);
            entityType.AddStructuralProperty("SimpleComplexProperty", new EdmComplexTypeReference(simpleComplexType, isNullable: false));
            entityType.AddStructuralProperty("DeepComplexProperty", new EdmComplexTypeReference(nestedComplexType, isNullable: false));
            entityType.AddStructuralProperty("PrimitiveCollection", EdmCoreModel.GetCollection(StringTypeRef));
            entityType.AddStructuralProperty("ComplexCollection", EdmCoreModel.GetCollection(new EdmComplexTypeReference(ratingComplexType, isNullable: false)));
            entityType.AddStructuralProperty("NamedStream", EdmPrimitiveTypeKind.Stream, false);
            entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "DeferredNavigation", Target = entityType, TargetMultiplicity = EdmMultiplicity.One });
            entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "AssociationLink", Target = entityType, TargetMultiplicity = EdmMultiplicity.One });
            entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "ExpandedEntry", Target = expandedEntryType, TargetMultiplicity = EdmMultiplicity.One });
            entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "ExpandedFeed", Target = entityType, TargetMultiplicity = EdmMultiplicity.Many });
            model.AddElement(entityType);

            EdmEntityType wrappingEntityType = new EdmEntityType(DefaultNamespaceName, "WrappingEntityType");
            wrappingEntityType.AddKeys(wrappingEntityType.AddStructuralProperty("Wrapping_ID", Int32TypeRef));
            wrappingEntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "Wrapping_ExpandedEntry", Target = entityType, TargetMultiplicity = EdmMultiplicity.One });
            model.AddElement(wrappingEntityType);

            var container = new EdmEntityContainer(DefaultNamespaceName, "DefaultContainer");
            model.AddElement(container);

            container.AddEntitySet("EntitySet", entityType);
            container.AddEntitySet("WrappingEntitySet", wrappingEntityType);

            return model;
        }

        private IEnumerable<ODataItem> CreatePayload(ProjectedPropertiesTestCase testCase)
        {
            // First create the entry itself (it might get wrapped later)
            ODataResource entry = new ODataResource()
            {
                TypeName = "TestModel.EntityType",
                Properties = new List<ODataProperty>()
                    {
                        new ODataProperty { Name = "StringProperty", Value = "foo" },
                        new ODataProperty { Name = "NumberProperty", Value = 42 },
                        new ODataProperty { Name = "SimpleComplexProperty", Value = new ODataComplexValue
                        {
                            TypeName = "TestModel.SimplexComplexType",
                            Properties = new ODataProperty[] {
                                new ODataProperty { Name = "Name", Value = "Bart" }
                        } } },
                        new ODataProperty { Name = "DeepComplexProperty", Value = new ODataComplexValue
                        {
                            TypeName = "TestModel.NestedComplexType",
                            Properties = new ODataProperty[] {
                                new ODataProperty { Name = "InnerComplexProperty", Value = new ODataComplexValue
                                {
                                    TypeName = "TestModel.SimplexComplexType2",
                                    Properties = new ODataProperty[] {
                                        new ODataProperty { Name = "Value", Value = 43 }
                                } } }
                        } } },

                        new ODataProperty { Name = "PrimitiveCollection", Value = new ODataCollectionValue
                        {
                            TypeName = "Collection(Edm.String)",
                            Items = new object[] { "Simpson" }
                        } },
                        new ODataProperty { Name = "ComplexCollection", Value = new ODataCollectionValue
                        {
                            TypeName = "Collection(TestModel.RatingComplexType)",
                            Items = new object[] {
                                new ODataComplexValue
                                {
                                    TypeName = "TestModel.RatingComplexType",
                                    Properties = new ODataProperty[] { new ODataProperty { Name = "Rating", Value = -3 } }
                                }
                        } } }
                    },
                SerializationInfo = MySerializationInfo
            };

            if (testCase.ResponseOnly)
            {
                // Add a stream property for responses
                ((List<ODataProperty>)entry.Properties).Add(new ODataProperty { Name = "NamedStream", Value = new ODataStreamReferenceValue { EditLink = new Uri("http://odata.org/namedstream") } });
            }

            ODataItem[] entryItems = new ODataItem[]
            {
                entry,
                new ODataNestedResourceInfo { Name = "DeferredNavigation", IsCollection = false, Url = new Uri("http://odata.org/deferred"), AssociationLinkUrl = testCase.ResponseOnly ? new Uri("http://odata.org/associationlink2") : null },
                null, // End deferred link

                new ODataNestedResourceInfo { Name = "ExpandedEntry", IsCollection = false, Url = new Uri("http://odata.org/entry") },
                    new ODataResource() 
                    {
                        TypeName = "TestModel.ExpandedEntryType",
                        Properties = new ODataProperty[] {
                            new ODataProperty { Name = "ExpandedEntryName", Value = "bar" }
                        },
                        SerializationInfo = MySerializationInfo
                    },
                        new ODataNestedResourceInfo { Name = "ExpandedEntry_DeferredNavigation", IsCollection = false, Url = new Uri("http://odata.org/deferred") },
                        null, // End deffered link
                        new ODataNestedResourceInfo { Name = "ExpandedEntry_ExpandedFeed", IsCollection = true, Url = new Uri("http://odata.org/feed") },
                            new ODataResourceSet { Id = new Uri("http://test/feedid1"), SerializationInfo = MySerializationInfo },
                            null, // End feed
                        null, // End exanded expanded feed link
                    null, // End expanded entry
                null, // End expanded entry nav link

                new ODataNestedResourceInfo { Name = "ExpandedFeed", IsCollection = true, Url = new Uri("http://odata.org/feed") },
                    new ODataResourceSet { Id = new Uri("http://test/feedid2") },
                        new ODataResource { TypeName = "TestModel.EntityType" },
                        null, // End entry
                        new ODataResource { TypeName = "TestModel.EntityType", SerializationInfo = MySerializationInfo },
                        null, // End entry
                    null, // End expanded feed
                null, // End expanded feed nav link

                null, // End the top-level entry
            };

            ProjectedPropertiesAnnotation projectedProperties = testCase.TopLevelProjectedProperties;

            if (!testCase.NestedPayload)
            {
                this.Assert.IsNull(testCase.NestedProjectedProperties, "For a non-nested payload, no nested annotation must be specified.");
                entry.SetAnnotation(projectedProperties);
                return entryItems;
            }

            // If we are processing a test case for a nested payload, wrap the entry items into a wrapping entry with an expanded navigation link.
            ODataResource wrappingEntry = new ODataResource()
            {
                TypeName = "TestModel.WrappingEntityType",
                Properties = new[] { new ODataProperty { Name = "Wrapping_ID", Value = 1 } },
                SerializationInfo = MySerializationInfo
            };
            IEnumerable<ODataItem> wrappedItems =
                new ODataItem[] { wrappingEntry, new ODataNestedResourceInfo { Name = "Wrapping_ExpandedEntry", IsCollection = false, Url = new Uri("http://odata.org/wrapping") } }
                .Concat(entryItems)
                .Concat(new ODataItem[] { null, null });

            ProjectedPropertiesAnnotation nestedProjectedProperties = testCase.NestedProjectedProperties;
            entry.SetAnnotation(nestedProjectedProperties);
            wrappingEntry.SetAnnotation(projectedProperties);

            return wrappedItems;
        }

        private WriterTestExpectedResults CreateExpectedResults(WriterTestConfiguration testConfiguration, ProjectedPropertiesTestCase testCase, bool withModel)
        {
            if (testCase.ExpectedException != null)
            {
                ExpectedException expectedException = testCase.ExpectedException(withModel);
                if (expectedException != null)
                {
                    return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        ExpectedException2 = expectedException
                    };
                }
            }

            if (testConfiguration.Format == ODataFormat.Json)
            {
                #region JSON Light expected result
                JsonArray expectedJson = new JsonArray();
                foreach (var p in testCase.ExpectedProperties.Distinct().OrderBy(p => p))
                {
                    expectedJson.Add(new JsonPrimitiveValue(p));
                }

                var jsonExpectedResults = new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                {
                    Json = expectedJson.ToText(/*writingJsonLight*/ true, testConfiguration.MessageWriterSettings.Indent),
                    FragmentExtractor = (result) =>
                    {
                        // Everything except association links
                        IEnumerable<string> actualProperties;
                        if (result == null)
                        {
                            actualProperties = new string[0];
                        }
                        else
                        {
                            List<string> propertyNames = new List<string>();

                            foreach (JsonProperty jsonProperty in result.Object().Properties)
                            {
                                string propertyName = jsonProperty.Name;
                                int atIndex = propertyName.IndexOf('@');
                                int dotIndex = propertyName.IndexOf('.');

                                if (dotIndex < 0)
                                {
                                    propertyNames.Add(propertyName);
                                }
                                else if (atIndex >= 0)
                                {
                                    propertyNames.Add(propertyName.Substring(0, atIndex));
                                }
                            }

                            actualProperties = propertyNames.Distinct();
                        }

                        JsonArray r = new JsonArray();
                        foreach (var p in actualProperties.OrderBy(p => p))
                        {
                            r.Add(new JsonPrimitiveValue(p));
                        }

                        return r;
                    }
                };

                if (testCase.NestedPayload)
                {
                    var originalFragmentExtractor = jsonExpectedResults.FragmentExtractor;
                    jsonExpectedResults.FragmentExtractor = (result) =>
                    {
                        // Verify that the Wrapping_ID property is not written
                        JsonObject resultObject = result.Object();
                        this.Assert.IsNull(resultObject.Property("Wrapping_ID"), "No other property but the nav. link should be written.");
                        return originalFragmentExtractor(
                            resultObject.Property("Wrapping_ExpandedEntry") == null
                                ? null
                                : resultObject.PropertyObject("Wrapping_ExpandedEntry"));
                    };
                }

                return jsonExpectedResults;
                #endregion JSON Light expected result
            }
            else
            {
                throw new TaupoInvalidOperationException("The format " + testConfiguration.Format.GetType().FullName + " is not supported.");
            }
        }

        private sealed class ProjectedPropertiesTestCase
        {
            public ProjectedPropertiesTestCase()
            {
            }

            public ProjectedPropertiesTestCase(ProjectedPropertiesTestCase other)
            {
                this.DebugDescription = other.DebugDescription;
                this.TopLevelProjectedProperties = other.TopLevelProjectedProperties;
                this.NestedProjectedProperties = other.NestedProjectedProperties;
                this.ExpectedProperties = other.ExpectedProperties;
                this.ResponseOnly = other.ResponseOnly;
                this.NestedPayload = other.NestedPayload;
                this.IgnoreFormats = other.IgnoreFormats;
                this.ExpectedException = other.ExpectedException;
            }

            public string DebugDescription { get; set; }
            public ProjectedPropertiesAnnotation TopLevelProjectedProperties { get; set; }
            public ProjectedPropertiesAnnotation NestedProjectedProperties { get; set; }
            public string[] ExpectedProperties { get; set; }
            public bool ResponseOnly { get; set; }
            public ODataFormat[] IgnoreFormats { get; set; }
            public bool NestedPayload { get; set; }
            public Func<bool, ExpectedException> ExpectedException { get; set; }
        }
#endif
    }
}
