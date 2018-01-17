//---------------------------------------------------------------------
// <copyright file="EntryReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Contracts.JsonLight;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestModels = Microsoft.Test.OData.Utils.Metadata.TestModels;

    /// <summary>
    /// Tests reading of various entry payloads.
    /// </summary>
    [TestClass, TestCase]
    public class EntryReaderTests : ODataReaderTestCase
    {
        private PayloadReaderTestDescriptor.Settings jsonLightSettings;

        [InjectDependency]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings JsonLightSettings
        {
            get { return this.jsonLightSettings; }
            set { this.jsonLightSettings = value; this.jsonLightSettings.ExpectedResultSettings.ObjectModelToPayloadElementConverter = new JsonLightObjectModelToPayloadElementConverter(); }
        }

        // TODO: Add ETag format independent tests
        [TestMethod, TestCategory("Reader.Entries"), Variation(Description = "Test the the reading of primitive properties on entry payloads.")]
        public void PrimitivePropertiesOnEntryTest()
        {
            EdmModel model = new EdmModel();
            var entityType = new EdmEntityType("TestModel", "EntityType");
            entityType.AddKeys(entityType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false)));
            entityType.AddStructuralProperty("__number", EdmCoreModel.Instance.GetInt32(false));
            entityType.AddStructuralProperty("null", EdmCoreModel.Instance.GetString(true));
            entityType.AddStructuralProperty("number", EdmCoreModel.Instance.GetInt32(false));
            entityType.AddStructuralProperty("string", EdmCoreModel.Instance.GetString(true));
            entityType.AddStructuralProperty("bool", EdmCoreModel.Instance.GetBoolean(false));
            for (int i = 1; i <= 100; i++)
            {
                entityType.AddStructuralProperty("prop" + i.ToString(), EdmCoreModel.Instance.GetInt32(false));
            }
            model.AddElement(entityType);
            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            container.AddEntitySet("EntitySet", entityType);
            model.AddElement(container);


            // Few hand-crafted payloads
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                // No primitive properties
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.EntityType").PrimitiveProperty("ID", 1),
                    PayloadEdmModel = model
                },
                // Simple primitive property
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.EntityType").PrimitiveProperty("ID", 1).PrimitiveProperty("__number", 42),
                    PayloadEdmModel = model
                },
                // Null primitive property
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.EntityType").PrimitiveProperty("ID", 1).PrimitiveProperty("null", null),
                    PayloadEdmModel = model
                },
                // Multiple primitive properties
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.EntityType").PrimitiveProperty("ID", 1).PrimitiveProperty("number", 42).PrimitiveProperty("string", "some").PrimitiveProperty("bool", true),
                    PayloadEdmModel = model
                },
                // Many primitive properties
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = new EntityInstance("TestModel.EntityType", false)
                                        {
                                            Properties = Enumerable.Range(1, 100)
                                                .Select(i => PayloadBuilder.PrimitiveProperty("prop" + i.ToString(), i)).ToList()
                                        }.PrimitiveProperty("ID", 1),
                    PayloadEdmModel = model
                },
            };

            // Add all primitive values as well
            testDescriptors = testDescriptors.Concat(TestValues.CreatePrimitiveValuesWithMetadata().Select(primitiveValue =>
                {
                    PropertyInstance property = PayloadBuilder.Property("propertyName", primitiveValue);
                    EntityInstance entityInstance = PayloadBuilder.Entity("TestModel.EntityType");
                    entityInstance.Add(property);

                    EdmModel newModel = new EdmModel();
                    var tempType = new EdmEntityType("TestModel", "EntityType");
                    tempType.AddKeys(tempType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false)));
                    var valueType = primitiveValue.GetAnnotation<EntityModelTypeAnnotation>().EdmModelType;
                    tempType.AddStructuralProperty("propertyName", valueType);
                    newModel.AddElement(tempType);
                    var tempContainer = new EdmEntityContainer("TestModel", "DefalutContainer");
                    tempContainer.AddEntitySet("EntitySet", tempType);
                    newModel.AddElement(tempContainer);

                    return new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = entityInstance.PrimitiveProperty("ID", 1),
                        PayloadEdmModel = newModel
                    };
                }));

            // Generate interesting payloads around the entry
            testDescriptors = testDescriptors.SelectMany(td => this.PayloadGenerator.GenerateReaderPayloads(td));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Entries"), Variation(Description = "Test the reading of various kinds of properties on a single entry in JSON Light.")]
        public void VariousPropertyKindsOnEntryJsonLightTest()
        {
            EdmModel model = new EdmModel();

            // Generate interesting payloads around the entry
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = PayloadReaderTestDescriptorGenerator.CreateEntityInstanceDescriptors(this.JsonLightSettings, model, true)
                .SelectMany(td => this.PayloadGenerator.GenerateReaderPayloads(td));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Entries"), Variation(Description = "Test the reading of open properties.")]
        public void OpenPropertiesTest()
        {
            // Interesting values to test as open properties.
            // Only test complex and collection values here, since open primitive properties rely on format specific primitive type support.
            // The open primitive properties tests are thus format specific and are here:
            //   JSON - PrimitiveValueReaderJsonTests.UntypedPrimitiveValueTest
            //   ATOM - PrimitiveValueReaderAtomTests.PrimitiveValueWithoutType
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = PayloadReaderTestDescriptorGenerator.CreateComplexValueTestDescriptors(this.Settings, true, false);

            // Add spatial open property tests
            testDescriptors = testDescriptors.Concat(new[]
                {
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.PrimitiveValue(GeographyFactory.Point(10, 20, 30, 40).Build())
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.PrimitiveValue(GeometryFactory.Point(10, 20, 30, 40).Build())
                    }
                });

            // Add couple of hand-crafted payloads
            testDescriptors = testDescriptors.Concat(new[]
                {
                    // Open complex null value
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.ComplexValue("TestModel.NonEmptyComplexType", true),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_ComplexValueWithPropertyTypeAnnotation", "odata.type"),
                    },
                });

            testDescriptors = testDescriptors
                // Wrap the property in an open entity
                .Select(td =>
                {
                    EdmModel model = (EdmModel)td.PayloadEdmModel;
                    model = model == null ? new EdmModel() : (EdmModel)Test.OData.Utils.Metadata.MetadataUtils.Clone(model);
                    var entityType = model.EntityType("OpenEntityType", "TestModel", null, false, true);
                    entityType.AddKeys(entityType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
                    entityType.AddStructuralProperty("DateTimeProperty", EdmPrimitiveTypeKind.DateTimeOffset);

                    var complexType = model.ComplexType("NonEmptyComplexType");
                    complexType.AddStructuralProperty("P1", EdmPrimitiveTypeKind.Int32);
                    complexType.AddStructuralProperty("P2", EdmCoreModel.Instance.GetString(true));
                    model = model.Fixup();

                    return new PayloadReaderTestDescriptor(td)
                    {
                        PayloadElement = PayloadBuilder.Entity("TestModel.OpenEntityType")
                            .PrimitiveProperty("ID", 42)
                            .Property("OpenProperty", td.PayloadElement),
                        PayloadEdmModel = model
                    };
                });

            // Add a couple of hand crafted payloads
            {
                EdmModel model = new EdmModel();
                var entityType = model.EntityType("OpenEntityType", null, null, false, true);
                entityType.KeyProperty("ID", EdmCoreModel.Instance.GetInt32(false));
                model = model.Fixup();

                testDescriptors = testDescriptors.Concat(new PayloadReaderTestDescriptor[]
                {
                    // Open stream property is not allowed.
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity("TestModel.OpenEntityType").PrimitiveProperty("ID", 42)
                            .StreamProperty("OpenProperty", "http://odata.org/readlink"),
                        PayloadEdmModel = model,
                        ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_OpenStreamProperty", "OpenProperty"),
                        // TODO: In JSON we recognize this as a complex property - once we make a decision about the bug enable the test for JSON.
                        SkipTestConfiguration = tc => true
                    },
                    // Open deferred navigation property is not allowed.
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity("TestModel.OpenEntityType").PrimitiveProperty("ID", 42)
                            .NavigationProperty("OpenProperty", "http://odata.org/navprop"),
                        PayloadEdmModel = model,
                        ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_OpenNavigationProperty", "OpenProperty", "TestModel.OpenEntityType"),
                        // TODO: In JSON we recognize this as a complex property - once we make a decision about the bug enable the test for JSON.
                        SkipTestConfiguration = tc => true
                    },
                    // Open expanded navigation property (entry) is not allowed.
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity("TestModel.OpenEntityType").PrimitiveProperty("ID", 42)
                            .ExpandedNavigationProperty("OpenProperty", PayloadBuilder.Entity("TestModel.OpenEntityType")),
                        PayloadEdmModel = model,
                        ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_OpenNavigationProperty", "OpenProperty", "TestModel.OpenEntityType"),
                        // This can't work in JSON as it is recognized as a complex value - and will fail for different reasons
                        SkipTestConfiguration = tc => true
                    },
                    // Open expanded navigation property (feed) is not allowed.
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity("TestModel.OpenEntityType").PrimitiveProperty("ID", 42)
                            .ExpandedNavigationProperty("OpenProperty", PayloadBuilder.EntitySet()),
                        PayloadEdmModel = model,
                        ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_OpenNavigationProperty", "OpenProperty", "TestModel.OpenEntityType"),
                        // This can't work in JSON as it may be recognized as a complex value - and will fail for different reasons
                        SkipTestConfiguration = tc =>true
                    },
                    // Open property with same name as non-open property
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity("TestModel.OpenEntityType").PrimitiveProperty("ID", 42).PrimitiveProperty("DateTimeProperty", new DateTimeOffset(DateTime.Now))
                            .PrimitiveProperty("DateTimeProperty", new DateTimeOffset(DateTime.Now.AddDays(1.0))),
                        PayloadEdmModel = model,
                        ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "DateTimeProperty"),
                        // In JSON Light this fails for different reasons, related to missing/multiple type annotations (depending on how it is serialised)
                        SkipTestConfiguration = tc => tc.Format == ODataFormat.Json,
                    },
                });
            }

            testDescriptors = testDescriptors.SelectMany(td => this.PayloadGenerator.GenerateReaderPayloads(td));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Entries"), Variation(Description = "Verifies duplicate property name checking on entries.")]
        public void DuplicatePropertyNamesTest()
        {
            EdmModel model = new EdmModel();
            var entityType = new EdmEntityType("TestModel", "DuplicateEntityType");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(entityType);
            var complexType = new EdmComplexType("TestModel", "DuplicateComplexType");
            complexType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            model.AddElement(complexType);
            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            container.AddEntitySet("ClosedEntitySet", entityType);
            model.AddElement(container);

            PropertyInstance primitiveProperty = PayloadBuilder.PrimitiveProperty("DuplicateProperty", 42);
            PropertyInstance complexProperty = PayloadBuilder.Property("DuplicateProperty",
                PayloadBuilder.ComplexValue("TestModel.DuplicateComplexType").PrimitiveProperty("Name", "foo"));
            PropertyInstance collectionProperty = PayloadBuilder.Property("DuplicateProperty",
                PayloadBuilder.PrimitiveMultiValue(EntityModelUtils.GetCollectionTypeName("Edm.String")).WithTypeAnnotation(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false))));
            PropertyInstance streamProperty = PayloadBuilder.StreamProperty("DuplicateProperty", "http://odata.org/readlink", "http://odata.org/editlink");
            PropertyInstance navigationProperty = PayloadBuilder.NavigationProperty("DuplicateProperty", "http://odata.org/navlink")
                .IsCollection(true)
                .WithTypeAnnotation(entityType);
            PropertyInstance expandedFeedProperty = PayloadBuilder.ExpandedNavigationProperty("DuplicateProperty",
                PayloadBuilder.EntitySet(new EntityInstance[] { PayloadBuilder.Entity("TestModel.DuplicateEntityType").PrimitiveProperty("Id", 1) }))
                .IsCollection(true)
                .WithTypeAnnotation(entityType);
            PropertyInstance associationLinkProperty = PayloadBuilder.NavigationProperty("DuplicateProperty", null, "http://odata.org/assoclink");

            PropertyInstance[] allProperties = new[] { primitiveProperty, complexProperty, collectionProperty, streamProperty, navigationProperty, expandedFeedProperty, associationLinkProperty };
            PropertyInstance[] propertiesWithPossibleDuplication = new[] { primitiveProperty, complexProperty, navigationProperty, expandedFeedProperty };
            PropertyInstance[] propertiesWithNoDuplication = new[] { collectionProperty, streamProperty, associationLinkProperty };

            IEnumerable<DuplicatePropertySet> duplicatePropertySets;

            // Those which may allow duplication
            duplicatePropertySets = propertiesWithPossibleDuplication
                .Variations(2).Select(properties => new DuplicatePropertySet { Properties = properties, DuplicationPotentiallyAllowed = true });

            // Then for each in those which don't allow duplication try it against all the others
            duplicatePropertySets = duplicatePropertySets.Concat(propertiesWithNoDuplication.SelectMany(
                propertyWithNoDuplication => allProperties.SelectMany(otherProperty =>
                    new[]
                    {
                        new DuplicatePropertySet { Properties = new [] { propertyWithNoDuplication, otherProperty }, DuplicationPotentiallyAllowed = false },
                        new DuplicatePropertySet { Properties = new [] { otherProperty, propertyWithNoDuplication }, DuplicationPotentiallyAllowed = false },
                    })));

            this.CombinatorialEngineProvider.RunCombinations(
                duplicatePropertySets,
                new bool[] { false, true },
                new bool[] { true, false },
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (duplicatePropertySet, useServerBehavior, useMetadata, testConfiguration) =>
                {
                    PropertyInstance firstProperty = duplicatePropertySet.Properties.ElementAt(0);
                    PropertyInstance secondProperty = duplicatePropertySet.Properties.ElementAt(1);

                    // Non-metadata parsing is not supported in JSON.
                    if (!useMetadata)
                    {
                        return;
                    }

                    if (firstProperty.ElementType != secondProperty.ElementType)
                    {
                        return;
                    }

                    // Association links are only recognized in response payloads and MPV >= V3
                    if ((testConfiguration.IsRequest) &&
                        duplicatePropertySet.Properties.Any(p => object.ReferenceEquals(p, associationLinkProperty)))
                    {
                        return;
                    }

                    // Steam properties are only recognized in response >=V3 payloads
                    if ((testConfiguration.IsRequest) &&
                        duplicatePropertySet.Properties.Any(p => object.ReferenceEquals(p, streamProperty)))
                    {
                        return;
                    }

                    // Copy the test config
                    testConfiguration = new ReaderTestConfiguration(testConfiguration);
                    if (useServerBehavior)
                    {
                        testConfiguration.MessageReaderSettings.Validations &= ~(ValidationKinds.ThrowOnDuplicatePropertyNames | ValidationKinds.ThrowIfTypeConflictsWithMetadata);
                        testConfiguration.MessageReaderSettings.ClientCustomTypeResolver = null;
                    }

                    // Create a descriptor with the first property
                    PayloadReaderTestDescriptor testDescriptor = new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = firstProperty,
                        PayloadEdmModel = useMetadata ? (EdmModel)Test.OData.Utils.Metadata.MetadataUtils.Clone(model) : null,
                    };

                    // Now generate entity around it
                    testDescriptor = testDescriptor.InEntity(2, 2);

                    // Now add the second property to it
                    ((EntityInstance)testDescriptor.PayloadElement).Add(secondProperty);

                    // We expect failure only if we don't allow duplicates or if the property kind doesn't allow duplicates ever.
                    // In JSON with WCF DS Service behavior where duplicates are removed very soon and thus we never fail on them.
                    if ((!duplicatePropertySet.DuplicationPotentiallyAllowed || !useServerBehavior))
                    {
                        testDescriptor.ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "DuplicateProperty");
                    }

                    if (firstProperty.ElementType == ODataPayloadElementType.NavigationPropertyInstance && secondProperty.ElementType == ODataPayloadElementType.NavigationPropertyInstance)
                    {
                        NavigationPropertyInstance navigationFirstProperty = (NavigationPropertyInstance)firstProperty;
                        NavigationPropertyInstance navigationSecondProperty = (NavigationPropertyInstance)secondProperty;

                        // If one of the properties is an association link and the other is a navigation link, that combination is allowed and it's not an error.
                        // Just skip that combination.
                        if ((navigationFirstProperty.Value != null && navigationFirstProperty.AssociationLink == null && navigationSecondProperty.Value == null && navigationSecondProperty.AssociationLink != null) ||
                            (navigationFirstProperty.Value == null && navigationFirstProperty.AssociationLink != null && navigationSecondProperty.Value != null && navigationSecondProperty.AssociationLink == null))
                        {
                            return;
                        }

                        if (testConfiguration.Format == ODataFormat.Json)
                        {
                            if ((navigationFirstProperty.Value is DeferredLink || navigationSecondProperty.Value is DeferredLink) &&
                                (navigationFirstProperty.Value is ExpandedLink || navigationSecondProperty.Value is ExpandedLink))
                            {
                                testDescriptor.ExpectedException = null;
                                testDescriptor.ExpectedResultNormalizers.Add(
                                    (tc) => (tc.IsRequest)
                                        ? (Func<ODataPayloadElement, ODataPayloadElement>)((payload) => EnsureAnnotationsBeforeProperties(DuplicateNavigationPropertiesToLinkCollection(payload)))
                                        : (payload) => EnsureAnnotationsBeforeProperties(DuplicateNavigationPropertiesToExpandedLink(payload)));
                            }
                            else if (navigationFirstProperty.AssociationLink != null && navigationSecondProperty.AssociationLink != null)
                            {
                                testDescriptor.ExpectedException = ODataExpectedExceptions.ODataException(
                                    "DuplicateAnnotationForPropertyNotAllowed",
                                    JsonLightConstants.ODataAssociationLinkUrlAnnotationName,
                                    "DuplicateProperty");
                            }
                        }

                        // In requests if both are navigation properties (not association links)
                        // If both are expanded, then fail (that's not allowed), but if one is not expanded, that is allowed (binding in POST scenario)
                        // Note that this only works because we use expanded feed, if we would add expanded entry, then the expanded entry can't be allowed along with a deferred link.
                        else if (navigationFirstProperty.AssociationLink == null && navigationSecondProperty.AssociationLink == null &&
                            testConfiguration.IsRequest &&
                            (navigationFirstProperty.Value is DeferredLink || navigationSecondProperty.Value is DeferredLink))
                        {
                            testDescriptor.ExpectedException = null;
                        }
                    }

                    if (firstProperty.ElementType == ODataPayloadElementType.NamedStreamInstance && secondProperty.ElementType == ODataPayloadElementType.NamedStreamInstance)
                    {
                        if (testConfiguration.Format == ODataFormat.Json)
                        {
                            testDescriptor.ExpectedException = ODataExpectedExceptions.ODataException("DuplicateAnnotationForPropertyNotAllowed", JsonLightConstants.ODataMediaEditLinkAnnotationName, "DuplicateProperty");
                        }
                    }

                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        testDescriptor.PayloadElement = EnsureAnnotationsBeforeProperties(testDescriptor.PayloadElement);
                    }

                    testDescriptor.RunTest(testConfiguration);
                });
        }

        private sealed class DuplicatePropertySet
        {
            public IEnumerable<PropertyInstance> Properties { get; set; }
            public bool DuplicationPotentiallyAllowed { get; set; }
        }

        [TestMethod, TestCategory("Reader.Entries"), Variation(Description = "Verify correct behavior in regard to duplicate navigation links. This is targetted on the binding scenarios.")]
        public void DuplicateNavigationLinkTest()
        {
            IEdmModel model = TestModels.BuildTestModel();

            IEnumerable<DuplicateNavigationLinkTestCase> testCases = new[]
            {
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Two expanded links, both collection",
                    Properties = new PropertyInstance[] {
                        PayloadBuilder.ExpandedNavigationProperty(
                            "CityHall",
                            PayloadBuilder.EntitySet().InsertAt(0, PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1), PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 2))),
                        PayloadBuilder.ExpandedNavigationProperty(
                            "CityHall",
                            PayloadBuilder.EntitySet()) },
                    ExpectedResultCallback =
                        (tc) => new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                                {
                                    ExpectedException = (tc.Format == ODataFormat.Json)
                                        ? ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "CityHall")
                                        : (tc.IsRequest)
                                            ? null
                                            : ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "CityHall"),
                                },
                },
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Two expanded links, both singletons",
                    Properties = new PropertyInstance[] {
                        PayloadBuilder.ExpandedNavigationProperty(
                            "PoliceStation",
                            PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1)),
                        PayloadBuilder.ExpandedNavigationProperty(
                            "PoliceStation",
                            null) },
                    ExpectedResultCallback =
                        (tc) => new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                                {
                                    ExpectedException = (tc.Format == ODataFormat.Json)
                                        ? ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "PoliceStation")
                                        : (tc.IsRequest)
                                            ? ODataExpectedExceptions.ODataException("MultipleLinksForSingleton", "PoliceStation")
                                            : ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "PoliceStation"),
                                },
                },
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Two expanded links, one collection one singleton",
                    Properties = new PropertyInstance[] {
                        PayloadBuilder.ExpandedNavigationProperty(
                            "PoliceStation",
                            PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1)),
                        PayloadBuilder.ExpandedNavigationProperty(
                            "PoliceStation",
                            PayloadBuilder.EntitySet()) },
                    ExpectedException = ODataExpectedExceptions.ODataException("MultipleLinksForSingleton", "PoliceStation"),
                    NoMetadataOnly = true
                },
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Expanded singleton, deferred without type",
                    Properties = new PropertyInstance[] {
                        PayloadBuilder.ExpandedNavigationProperty(
                            "PoliceStation",
                            PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1)),
                        PayloadBuilder.DeferredNavigationProperty(
                            "PoliceStation",
                            PayloadBuilder.DeferredLink("http://odata.org/deferred")) },
                    ExpectedResultCallback =
                        (tc) => new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                                {
                                    ExpectedException = (tc.Format == ODataFormat.Json)
                                        ? tc.IsRequest
                                            ? ODataExpectedExceptions.ODataException("ODataJsonLightResourceDeserializer_SingletonNavigationPropertyWithBindingAndValue", "PoliceStation", JsonLightConstants.ODataBindAnnotationName)
                                            : null
                                        : tc.IsRequest
                                            ? ODataExpectedExceptions.ODataException("MultipleLinksForSingleton", "PoliceStation")
                                            : ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "PoliceStation"),
                                }
                },
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Expanded singleton, deferred singleton",
                    Properties = new PropertyInstance[] {
                        PayloadBuilder.ExpandedNavigationProperty(
                            "PoliceStation",
                            PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1)),
                        PayloadBuilder.DeferredNavigationProperty(
                            "PoliceStation",
                            PayloadBuilder.DeferredLink("http://odata.org/deferred").IsCollection(false)) },
                    ExpectedResultCallback =
                        (tc) => new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                                {
                                    ExpectedException = (tc.Format == ODataFormat.Json)
                                        ? tc.IsRequest
                                            ? ODataExpectedExceptions.ODataException("ODataJsonLightResourceDeserializer_SingletonNavigationPropertyWithBindingAndValue", "PoliceStation", JsonLightConstants.ODataBindAnnotationName)
                                            : null
                                        : tc.IsRequest
                                            ? ODataExpectedExceptions.ODataException("MultipleLinksForSingleton", "PoliceStation")
                                            : ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "PoliceStation"),
                                }
                },
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Expanded singleton, deferred collection (no metadata), we will fail on this since we don't allow multiple links for singletons",
                    Properties = new PropertyInstance[] {
                        PayloadBuilder.ExpandedNavigationProperty(
                            "PoliceStation",
                            PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1)),
                        PayloadBuilder.DeferredNavigationProperty(
                            "PoliceStation",
                            PayloadBuilder.DeferredLink("http://odata.org/deferred").IsCollection(true)) },
                    ExpectedException = ODataExpectedExceptions.ODataException("MultipleLinksForSingleton", "PoliceStation"),
                    NoMetadataOnly = true
                },
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Expanded collection, deferred without type - no failures",
                    Properties = new PropertyInstance[] {
                        PayloadBuilder.ExpandedNavigationProperty(
                            "CityHall",
                            PayloadBuilder.EntitySet()),
                        PayloadBuilder.DeferredNavigationProperty(
                            "CityHall",
                            PayloadBuilder.DeferredLink("http://odata.org/deferred")).IsCollection(true) },
                    ExpectedResultNormalizer =
                        (tc) =>
                        {
                            if (tc.Format == ODataFormat.Json)
                            {
                                return (tc.IsRequest) ? (Func<ODataPayloadElement, ODataPayloadElement>)DuplicateNavigationPropertiesToLinkCollection : DuplicateNavigationPropertiesToExpandedLink;
                            }

                            return null;
                        },
                    ExpectedResultCallback =
                        (tc) => new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                                {
                                    ExpectedException = (tc.Format == ODataFormat.Json || tc.IsRequest)
                                        ? null
                                        : ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "CityHall"),
                                }
                },
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Expanded collection, deferred singleton - no failures (binding scenario) (the metadata mismatch is specifically allowed)",
                    Properties = new PropertyInstance[] {
                        PayloadBuilder.ExpandedNavigationProperty(
                            "CityHall",
                            PayloadBuilder.EntitySet()),
                        PayloadBuilder.DeferredNavigationProperty(
                            "CityHall",
                            PayloadBuilder.DeferredLink("http://odata.org/deferred").IsCollection(false)) },
                    // Doesn't work for JSON since the metadata mismatch will cause the parsing to fail
                    // Does not work for deferred link in responses.
                    SkipForConfiguration = tc => tc.IsRequest == false,
                    ExpectedResultNormalizer =
                        (tc) => (tc.Format == ODataFormat.Json) ? DuplicateNavigationPropertiesToLinkCollection : (Func<ODataPayloadElement, ODataPayloadElement>)null,
                    ExpectedResultCallback =
                        (tc) => new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                                {
                                    ExpectedException = (tc.Format == ODataFormat.Json)
                                            ? ODataExpectedExceptions.ODataException("ODataJsonLightResourceDeserializer_StringValueForCollectionBindPropertyAnnotation", "CityHall", JsonLightConstants.ODataBindAnnotationName)
                                            : null,
                                },
                },
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Expanded collection, deferred collection - no failures (binding scenario)",
                    Properties = new PropertyInstance[] {
                        PayloadBuilder.ExpandedNavigationProperty(
                            "CityHall",
                            PayloadBuilder.EntitySet()),
                        PayloadBuilder.DeferredNavigationProperty(
                            "CityHall",
                            PayloadBuilder.DeferredLink("http://odata.org/deferred").IsCollection(true)) },
                    // Doesn't work for JSON since the metadata mismatch will cause the parsing to fail
                    ExpectedResultCallback =
                        (tc) => new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                                {
                                    ExpectedException = (tc.Format == ODataFormat.Json)
                                        ? tc.IsRequest
                                            ? ODataExpectedExceptions.ODataException("ODataJsonLightResourceDeserializer_StringValueForCollectionBindPropertyAnnotation", "CityHall", JsonLightConstants.ODataBindAnnotationName)
                                            : null
                                        : tc.IsRequest
                                            ? null
                                            : ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "CityHall"),
                                },
                },
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Two deferred singletons on a collection property - no failures (binding scenario) (the metadata mismatch is specifically allowed)",
                    Properties = new PropertyInstance[] {
                        PayloadBuilder.DeferredNavigationProperty(
                            "CityHall",
                            PayloadBuilder.DeferredLink("http://odata.org/deferred").IsCollection(false)),
                        PayloadBuilder.DeferredNavigationProperty(
                            "CityHall",
                            PayloadBuilder.DeferredLink("http://odata.org/deferred").IsCollection(false)) },
                    // Doesn't work for JSON since the metadata mismatch will cause the parsing to fail
                    // Does not work for deferred link in responses.
                    SkipForConfiguration = tc => tc.IsRequest == false,
                    ExpectedResultCallback =
                        (tc) => new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                                {
                                    ExpectedException = (tc.Format == ODataFormat.Json)
                                        ? (tc.IsRequest)
                                            ? ODataExpectedExceptions.ODataException("DuplicateAnnotationForPropertyNotAllowed", JsonLightConstants.ODataBindAnnotationName, "CityHall")
                                            : null
                                        : (tc.IsRequest)
                                            ? null
                                            : ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "CityHall"),
                                },
                },
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Two deferred collections on a collection property - no failures (binding scenario)",
                    Properties = new PropertyInstance[] {
                        PayloadBuilder.DeferredNavigationProperty(
                            "CityHall",
                            PayloadBuilder.DeferredLink("http://odata.org/deferred").IsCollection(true)).IsCollection(true),
                        PayloadBuilder.DeferredNavigationProperty(
                            "CityHall",
                            PayloadBuilder.DeferredLink("http://odata.org/deferred").IsCollection(true)).IsCollection(true) },
                    ExpectedResultCallback =
                        (tc) => new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                                {
                                    ExpectedException =
                                        (tc.Format == ODataFormat.Json)
                                            ? (tc.IsRequest)
                                                ? ODataExpectedExceptions.ODataException("DuplicateAnnotationForPropertyNotAllowed", JsonLightConstants.ODataBindAnnotationName, "CityHall")
                                                : ODataExpectedExceptions.ODataException("DuplicateAnnotationForPropertyNotAllowed", JsonLightConstants.ODataNavigationLinkUrlAnnotationName, "CityHall")
                                            : (tc.IsRequest)
                                                ? null
                                                : ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "CityHall"),
                                },
                },
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Deferred collection and deferred singleton - no failures (binding scenario) (the metadata mismatch is specifically allowed)",
                    Properties = new PropertyInstance[] {
                        PayloadBuilder.DeferredNavigationProperty(
                            "CityHall",
                            PayloadBuilder.DeferredLink("http://odata.org/deferred").IsCollection(true)),
                        PayloadBuilder.DeferredNavigationProperty(
                            "CityHall",
                            PayloadBuilder.DeferredLink("http://odata.org/deferred").IsCollection(false)) },
                    // Doesn't work for JSON since the metadata mismatch will cause the parsing to fail
                    // Does not work for deferred link in responses.
                    SkipForConfiguration = tc => tc.IsRequest == false,
                    ExpectedResultCallback =
                        (tc) => new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                                {
                                    ExpectedException =
                                        (tc.Format == ODataFormat.Json)
                                            ? (tc.IsRequest)
                                                ? ODataExpectedExceptions.ODataException("DuplicateAnnotationForPropertyNotAllowed", JsonLightConstants.ODataBindAnnotationName, "CityHall")
                                                : ODataExpectedExceptions.ODataException("DuplicateAnnotationForPropertyNotAllowed", JsonLightConstants.ODataNavigationLinkUrlAnnotationName, "CityHall")
                                            : (tc.IsRequest)
                                                ? null
                                                : ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "CityHall"),
                                },
                },
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Expanded collection, deferred collection and deferred singleton - no failures (binding scenario) (the metadata mismatch is specifically allowed)",
                    Properties = new PropertyInstance[] {
                        PayloadBuilder.ExpandedNavigationProperty(
                            "CityHall",
                            PayloadBuilder.EntitySet()),
                        PayloadBuilder.DeferredNavigationProperty(
                            "CityHall",
                            PayloadBuilder.DeferredLink("http://odata.org/deferred").IsCollection(true)),
                        PayloadBuilder.DeferredNavigationProperty(
                            "CityHall",
                            PayloadBuilder.DeferredLink("http://odata.org/deferred").IsCollection(false)) },
                    // Doesn't work for JSON since the metadata mismatch will cause the parsing to fail
                    // Does not work for deferred link in responses.
                    SkipForConfiguration = tc => tc.IsRequest == false,
                    ExpectedResultCallback =
                        (tc) => new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                                {
                                    ExpectedException =
                                        (tc.Format == ODataFormat.Json)
                                            ? (tc.IsRequest)
                                                ? ODataExpectedExceptions.ODataException("DuplicateAnnotationForPropertyNotAllowed", JsonLightConstants.ODataBindAnnotationName, "CityHall")
                                                : ODataExpectedExceptions.ODataException("DuplicateAnnotationForPropertyNotAllowed", JsonLightConstants.ODataNavigationLinkUrlAnnotationName, "CityHall")
                                            : (tc.IsRequest)
                                                ? null
                                                : ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "CityHall"),
                                },
                },
            };

            // Add association link cases (this should have no effect on the result, since single association link is always allowed)
            testCases = testCases.SelectMany(testCase =>
                new[]
                {
                    testCase,
                    new DuplicateNavigationLinkTestCase(testCase)
                    {
                        DebugDescription = testCase.DebugDescription + " [With association link]",
                        Properties = testCase.Properties.ConcatSingle(PayloadBuilder.NavigationProperty(testCase.Properties[0].Name, null, "http://odata.org/associationlink")).ToArray(),
                        // No association links in requests.
                        SkipForConfiguration = tc => testCase.SkipForConfiguration == null ? tc.IsRequest : testCase.SkipForConfiguration(tc) || tc.IsRequest
                    }
                });

            // Add all permutations since order should not have any effect on the outcome
            testCases = testCases.SelectMany(testCase => testCase.Properties.Permutations().Select(properties =>
                new DuplicateNavigationLinkTestCase(testCase) { Properties = properties }));

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                new bool[] { true, false },
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testCase, withMetadata, testConfiguration) =>
                {
                    // JSON reading requires metadata
                    if (!withMetadata && (testConfiguration.Format == ODataFormat.Json))
                    {
                        return;
                    }

                    if (withMetadata && testCase.NoMetadataOnly)
                    {
                        return;
                    }

                    if (testCase.SkipForConfiguration != null && testCase.SkipForConfiguration(testConfiguration))
                    {
                        return;
                    }

                    EntityInstance entityInstance = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1);
                    foreach (PropertyInstance propertyInstance in testCase.Properties)
                    {
                        entityInstance.Add(propertyInstance);
                    }

                    ExpectedException expectedException = testCase.ExpectedException;
                    if (!testConfiguration.IsRequest && testCase.ExpectedResultCallback == null)
                    {
                        // In responses all duplicates will fail with the same error message since we don't allow any duplicates there.
                        expectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", testCase.Properties[0].Name);
                    }

                    PayloadReaderTestDescriptor testDescriptor = new PayloadReaderTestDescriptor(this.Settings)
                    {
                        DebugDescription = testCase.DebugDescription,
                        PayloadElement = entityInstance,
                        ExpectedException = expectedException,
                        ExpectedResultCallback = testCase.ExpectedResultCallback,
                        PayloadEdmModel = withMetadata ? model : null,
                    };

                    testDescriptor.ExpectedResultNormalizers.Add(
                        tc => (Func<ODataPayloadElement, ODataPayloadElement>)null);
                    testDescriptor.ExpectedResultNormalizers.Add(tc => RemoveContentTypeAnnotationNormalizer.Normalize);

                    if (testCase.ExpectedResultNormalizer != null)
                    {
                        testDescriptor.ExpectedResultNormalizers.Add(testCase.ExpectedResultNormalizer);
                    }

                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        testDescriptor.PayloadElement = EnsureAnnotationsBeforeProperties(testDescriptor.PayloadElement);
                    }

                    testDescriptor.RunTest(testConfiguration);
                });
        }

        private sealed class DuplicateNavigationLinkTestCase
        {
            public PropertyInstance[] Properties { get; set; }
            public ExpectedException ExpectedException { get; set; }
            public Func<ReaderTestConfiguration, ReaderTestExpectedResult> ExpectedResultCallback { get; set; }
            public Func<ReaderTestConfiguration, Func<ODataPayloadElement, ODataPayloadElement>> ExpectedResultNormalizer { get; set; }
            public bool NoMetadataOnly { get; set; }
            public Func<ReaderTestConfiguration, bool> SkipForConfiguration { get; set; }
            public string DebugDescription { get; set; }

            public DuplicateNavigationLinkTestCase()
            {
            }

            public DuplicateNavigationLinkTestCase(DuplicateNavigationLinkTestCase other)
            {
                this.Properties = other.Properties;
                this.ExpectedException = other.ExpectedException;
                this.ExpectedResultCallback = other.ExpectedResultCallback;
                this.ExpectedResultNormalizer = other.ExpectedResultNormalizer;
                this.NoMetadataOnly = other.NoMetadataOnly;
                this.SkipForConfiguration = other.SkipForConfiguration;
                this.DebugDescription = other.DebugDescription;
            }
        }

        private sealed class RemoveContentTypeAnnotationNormalizer : ODataPayloadElementVisitorBase
        {
            public static ODataPayloadElement Normalize(ODataPayloadElement payloadElement)
            {
                new RemoveContentTypeAnnotationNormalizer().Recurse(payloadElement);
                return payloadElement;
            }

            public override void Visit(DeferredLink payloadElement)
            {
                base.Visit(payloadElement);
                payloadElement.RemoveAnnotations(typeof(ContentTypeAnnotation));
            }
        }

        [TestMethod, TestCategory("Reader.Entries"), Variation(Description = "Verify correct parsing of entries with projected types (i.e. properties are missing)")]
        public void ProjectedTypesTest()
        {
            EdmModel model = new EdmModel();
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = PayloadReaderTestDescriptorGenerator.CreateEntityInstanceDescriptors(this.Settings, model, true)
                .Where(td => ((EntityInstance)td.PayloadElement).Properties.Count() > 1);

            var projectedTestDescriptors = testDescriptors.SelectMany(
                (td) =>
                {
                    EntityInstance entity = (EntityInstance)td.PayloadElement;

                    EntityInstance firstPropertyOnly = entity.DeepCopy();
                    firstPropertyOnly.Properties = new[] { entity.Properties.First() };

                    EntityInstance lastPropertyOnly = entity.DeepCopy();
                    lastPropertyOnly.Properties = new[] { entity.Properties.Last() };

                    var projections = new List<PayloadReaderTestDescriptor>
                    {
                        new PayloadReaderTestDescriptor(td)
                        {
                            PayloadElement = firstPropertyOnly,
                            SkipTestConfiguration = tc =>
                                ODataPayloadElementConfigurationValidator.GetSkipTestConfiguration(firstPropertyOnly, ODataPayloadElementConfigurationValidator.AllValidators)(tc),
                        },
                        new PayloadReaderTestDescriptor(td)
                        {
                            PayloadElement = lastPropertyOnly,
                            SkipTestConfiguration = tc => tc.Format == ODataFormat.Json ||
                                ODataPayloadElementConfigurationValidator.GetSkipTestConfiguration(lastPropertyOnly, ODataPayloadElementConfigurationValidator.AllValidators)(tc),
                        },
                    };

                    if (entity.Properties.Count() > 2)
                    {
                        EntityInstance firstAndLastProperties = entity.DeepCopy();
                        firstAndLastProperties.Properties = new[] { entity.Properties.First(), entity.Properties.Last() };
                        projections.Add(new PayloadReaderTestDescriptor(td)
                        {
                            PayloadElement = firstAndLastProperties,
                            SkipTestConfiguration = tc =>
                                ODataPayloadElementConfigurationValidator.GetSkipTestConfiguration(firstAndLastProperties, ODataPayloadElementConfigurationValidator.AllValidators)(tc),
                        });
                    }

                    return projections;
                });

            this.CombinatorialEngineProvider.RunCombinations(
                projectedTestDescriptors,
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Entries"), Variation(Description = "Verify correct parsing of entries with geolocated URIs")]
        public void GeolocatedUriTest()
        {
            // Geolocated URIs are cases where the read/edit/etc URIs for the same resource use different services
            string baseUri = "http://www.test.com/foo.svc/Target";

            EdmModel model = new EdmModel();
            var mleType = new EdmEntityType("TestModel", "MediaLinkType", null, false, false, true);
            mleType.AddKeys(mleType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetString(true)));
            model.AddElement(mleType);

            var entityType = new EdmEntityType("TestModel", "GeolocatedUriType");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetString(true)));
            var navProp = entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "NavProp", Target = mleType, TargetMultiplicity = EdmMultiplicity.Many });
            entityType.AddStructuralProperty("StreamProperty", EdmPrimitiveTypeKind.Stream);
            model.AddElement(entityType);

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            var mleSet = container.AddEntitySet("MediaLinkSet", mleType);
            var entitySet = container.AddEntitySet("GeolocatedUriSet", entityType);
            model.AddElement(container);
            entitySet.AddNavigationTarget(navProp, mleSet);

            var testDescriptors = new[]
            {
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity(entityType.FullName())
                        .PrimitiveProperty("Id", System.Guid.NewGuid().ToString())
                        .Property(PayloadBuilder.NavigationProperty("NavProp", baseUri.Replace("foo", "geo1"), baseUri.Replace("foo", "geo2")).IsCollection(true))
                        .StreamProperty("StreamProperty", baseUri.Replace("foo", "read"), baseUri.Replace("foo", "edit"))
                        .ExpectedEntityType(entityType, entitySet),
                    PayloadEdmModel = model,
                    // No stream properties in requests or <V3 payloads
                    SkipTestConfiguration = tc => tc.IsRequest
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity(mleType.FullName())
                        .EnsureMediaLinkEntry()
                        .PrimitiveProperty("Id", System.Guid.NewGuid().ToString())
                        .StreamContentType("image/jpeg")
                        .StreamEditLink(baseUri.Replace("foo", "edit-stream"))
                        .StreamSourceLink(baseUri.Replace("foo", "source-stream"))
                        .ExpectedEntityType(mleType, mleSet),
                    PayloadEdmModel = model
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                new[] { true, false },
                (testDescriptor, testConfiguration, withBaseUri) =>
                {
                    var actualConfiguration = new ReaderTestConfiguration(testConfiguration);
                    actualConfiguration.MessageReaderSettings.BaseUri = withBaseUri ? new Uri(baseUri) : (testConfiguration.Format != ODataFormat.Json ? null : new Uri(baseUri));
                    testDescriptor.RunTest(actualConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Entries"), Variation(Description = "Verify correct reading of MLEs in Json Light")]
        public void MediaLinkEntryTestJsonLight()
        {
            var testDescriptors = CreateMediaLinkEntry(this.JsonLightSettings);
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        private static IEnumerable<PayloadReaderTestDescriptor> CreateMediaLinkEntry(PayloadReaderTestDescriptor.Settings settings)
        {

            IEdmModel model = TestModels.BuildTestModel();
            IEdmEntityType cityWithMapType = model.FindDeclaredType("TestModel.CityWithMapType") as IEdmEntityType;
            IEdmEntitySet citiesEntitySet = model.EntityContainer.FindEntitySet("Cities");

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // No MR read link - Ensure this does not throw any exception.
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).PrimitiveProperty("Id", 1).AsMediaLinkEntry()
                        .ExpectedEntityType(cityWithMapType, citiesEntitySet),
                    PayloadEdmModel = model,
                },
                // Just MR read link - valid for readers
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).PrimitiveProperty("Id", 1).AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr")
                        .ExpectedEntityType(cityWithMapType, citiesEntitySet),
                    PayloadEdmModel = model,
                },
                // Just MR content type - invalid for ATOM since there's no way to express that in ATOM payload
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).PrimitiveProperty("Id", 1).AsMediaLinkEntry().StreamContentType("mime/type")
                        .ExpectedEntityType(cityWithMapType, citiesEntitySet),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomResourceDeserializer_ContentWithWrongType", "mime/type"),
                    SkipTestConfiguration = (tc) => true
                },
                // MR read link and content type
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).PrimitiveProperty("Id", 1).AsMediaLinkEntry()
                        .StreamSourceLink("http://odata.org/mr").StreamContentType("mime/type")
                        .ExpectedEntityType(cityWithMapType, citiesEntitySet),
                    PayloadEdmModel = model,
                },
                // Just MR edit link - valid for readers
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).PrimitiveProperty("Id", 1).AsMediaLinkEntry().StreamEditLink("http://odata.org/mr")
                        .ExpectedEntityType(cityWithMapType, citiesEntitySet),
                    PayloadEdmModel = model,
                },
                // Just MR etag - valid for JSON reader (no way to represent this in ATOM)
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).PrimitiveProperty("Id", 1).AsMediaLinkEntry().StreamETag("etag")
                        .ExpectedEntityType(cityWithMapType, citiesEntitySet),
                    PayloadEdmModel = model,
                    SkipTestConfiguration = tc => false
                },
                // Just MR edit link and etag - valid for readers
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).PrimitiveProperty("Id", 1).AsMediaLinkEntry()
                        .StreamEditLink("http://odata.org/mr").StreamETag("etag")
                        .ExpectedEntityType(cityWithMapType, citiesEntitySet),
                    PayloadEdmModel = model,
                },
                // All MR properties
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).PrimitiveProperty("Id", 1).AsMediaLinkEntry()
                        .StreamSourceLink("http://odata.org/mrread").StreamContentType("mime/type").StreamEditLink("http://odata.org/mr").StreamETag("etag")
                        .ExpectedEntityType(cityWithMapType, citiesEntitySet),
                    PayloadEdmModel = model,
                },
                // MLE with Stream property
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).PrimitiveProperty("Id", 1).AsMediaLinkEntry()
                        .StreamSourceLink("http://odata.org/mrread").StreamContentType("mime/type").StreamEditLink("http://odata.org/mr").StreamETag("etag")
                        .StreamProperty("Skyline", readLink: "http://odata.org/skyline/read", editLink: "http://odata.org/skyline/edit", contentType: "mime/type", etag: "streamETag")
                        .ExpectedEntityType(cityWithMapType, citiesEntitySet),
                    PayloadEdmModel = model,
                    SkipTestConfiguration = (tc) => tc.IsRequest
                },
                // TODO: Add more tests around values for the MR properties. Readers won't validate anything, so empty values are acceptable
            };

            return testDescriptors;
        }

        [TestMethod, TestCategory("Reader.Entries"), Variation(Description = "Verify error cases of deeply recursive property values.")]
        public void PropertyValueDepthLimitTest()
        {
            // Define a recursive model
            EdmModel model = new EdmModel();
            var complexType = new EdmComplexType("TestModel", "RecursiveComplexType");
            complexType.AddStructuralProperty("PropertyName", new EdmComplexTypeReference(complexType, true));
            model.AddElement(complexType);

            var complexTypeWithCollection = new EdmComplexType("TestModel", "RecursiveComplexTypeWithCollection");
            complexTypeWithCollection.AddStructuralProperty("CollectionProperty", EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexTypeWithCollection, true)));
            model.AddElement(complexTypeWithCollection);

            var entityType = new EdmEntityType("TestModel", "EntityType");
            entityType.AddKeys(entityType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false)));
            entityType.AddStructuralProperty("ComplexPropertyName1", new EdmComplexTypeReference(complexType, true));
            entityType.AddStructuralProperty("ComplexPropertyName2", new EdmComplexTypeReference(complexType, true));
            model.AddElement(entityType);

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            container.AddEntitySet("EntitySet", entityType);
            model.AddElement(container);

            // Note: depth is counted by the number of complex values in the payload.
            int depthLimit = 15;

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // Property with depth exceeding the depth limit, which should fail.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadEdmModel = model,
                    PayloadElement = CreateDeeplyNestedProperty(depthLimit, "TestModel.RecursiveComplexType", "PropertyName"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_MaxDepthOfNestedEntriesExceeded", Convert.ToString(depthLimit)),
                },
                // Property with depth equal to the depth limit, which should pass.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadEdmModel = model,
                    PayloadElement = CreateDeeplyNestedProperty(depthLimit-1, "TestModel.RecursiveComplexType", "PropertyName"),
                },
                // Payload with two separate nested complex value properties, whose combined depth would exceed the limit, but are individually below it.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Entity("TestModel.EntityType")
                        .PrimitiveProperty("ID", 1)
                        .Property("ComplexPropertyName1", CreateDeeplyNestedComplexValues(depthLimit / 2 + 1, "TestModel.RecursiveComplexType", "PropertyName"))
                        .Property("ComplexPropertyName2", CreateDeeplyNestedComplexValues(depthLimit / 2 + 1, "TestModel.RecursiveComplexType", "PropertyName")),
                },
                // Complex values nested inside collections over the depth limit (should fail).
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Property("CollectionProperty", CreateDeeplyNestedComplexValuesWithCollections((depthLimit + 1 )* 2, "TestModel.RecursiveComplexTypeWithCollection", "CollectionProperty")),
                    ExpectedException =  ODataExpectedExceptions.ODataException("ValidationUtils_MaxDepthOfNestedEntriesExceeded", Convert.ToString(depthLimit)),
                },
                // Complex values nested inside collections at the depth limit (should pass).
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Property("CollectionProperty", CreateDeeplyNestedComplexValuesWithCollections(depthLimit * 2, "TestModel.RecursiveComplexTypeWithCollection", "CollectionProperty")),
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    var element = testDescriptor.PayloadElement as PropertyInstance;
                    testConfiguration = new ReaderTestConfiguration(testConfiguration);
                    testConfiguration.MessageReaderSettings.MessageQuotas.MaxNestingDepth = depthLimit;

                    testDescriptor.RunTest(testConfiguration);
                });
        }

        private static IEnumerable<PropertyInstance> undeclaredValueProperties = new[]
            {
                PayloadBuilder.PrimitiveProperty("UndeclaredProperty", "test"),
                PayloadBuilder.PrimitiveProperty("UndeclaredProperty", GeographyFactory.Point(0, 32).Build()),
                PayloadBuilder.Property("UndeclaredProperty", PayloadBuilder.PrimitiveMultiValue().Item("test")),

                // undefined complex type is not supported yet though undeclared complex value (of knonw type) is supported
                //PayloadBuilder.Property("UndeclaredProperty", PayloadBuilder.ComplexMultiValue("Collection(TestModel.Wrong)").Item(PayloadBuilder.ComplexValue("TestModel.Wrong"))),
            };

        private static IEnumerable<PropertyInstance> undeclaredLinkProperties = new PropertyInstance[]
            {
                PayloadBuilder.StreamProperty("UndeclaredProperty", "http://odata.org/readlink"),
                PayloadBuilder.StreamProperty("UndeclaredProperty", null, "http://odata.org/editlink"),
                PayloadBuilder.StreamProperty("UndeclaredProperty", "http://odata.org/readlink", "http://odata.org/editlink"),
                PayloadBuilder.DeferredNavigationProperty("UndeclaredProperty", PayloadBuilder.DeferredLink("http://odata.org/link")),
                PayloadBuilder.DeferredNavigationProperty("UndeclaredProperty", null, PayloadBuilder.DeferredLink("http://odata.org/associationlink")),
                PayloadBuilder.DeferredNavigationProperty("UndeclaredProperty", PayloadBuilder.DeferredLink("http://odata.org/link"), PayloadBuilder.DeferredLink("http://odata.org/associationlink")),
            };

        private static IEnumerable<PropertyInstance> undeclaredExpandedLinkProperties = new PropertyInstance[]
            {
                PayloadBuilder.ExpandedNavigationProperty("UndeclaredProperty", null),
                PayloadBuilder.ExpandedNavigationProperty("UndeclaredProperty", PayloadBuilder.Entity("Wrong")),
                PayloadBuilder.ExpandedNavigationProperty("UndeclaredProperty", PayloadBuilder.EntitySet()),
                PayloadBuilder.ExpandedNavigationProperty("UndeclaredProperty", PayloadBuilder.Entity("Wrong"), null, "http://odata.org/associationLink"),
                PayloadBuilder.ExpandedNavigationProperty("UndeclaredProperty", PayloadBuilder.EntitySet(), null, "http://odata.org/associationLink"),
            };

        private IEnumerable<PayloadReaderTestDescriptor> CreateUndeclaredPropertyTestDescriptors(bool throwOnUndeclaredPropertyForNonOpenType, PayloadReaderTestDescriptor.Settings settings)
        {
            IEdmModel model = TestModels.BuildTestModel();
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = undeclaredValueProperties.SelectMany(undeclaredProperty =>
            {
                if ((undeclaredProperty is PrimitiveProperty)
                    && ((PrimitiveProperty)undeclaredProperty).Value.ClrValue.ToString() == "Microsoft.Spatial.GeographyPointImplementation")
                {
                    return Enumerable.Empty<PayloadReaderTestDescriptor>();
                }

                EntityInstance inEntity = PayloadBuilder.Entity("TestModel.OfficeType")
                    .PrimitiveProperty("Id", 42);

                ComplexInstance inComplex = PayloadBuilder.ComplexValue("TestModel.Address")
                    .PrimitiveProperty("Street", "First");

                return new[]
                        {
                            // In entry
                            new PayloadReaderTestDescriptor(settings)
                            {
                                PayloadElement = inEntity.DeepCopy().Property(undeclaredProperty.DeepCopy()),
                                ExpectedResultPayloadElement = tc => inEntity.DeepCopy().Property(undeclaredProperty.DeepCopy()),
                                PayloadEdmModel = model,
                                ExpectedException = throwOnUndeclaredPropertyForNonOpenType
                                                        ? ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", "UndeclaredProperty", "TestModel.OfficeType")
                                                        : null
                            },
                            // In MLE entry
                            new PayloadReaderTestDescriptor(settings)
                            {
                                PayloadElement = PayloadBuilder.Entity("TestModel.CityWithMapType").PrimitiveProperty("Id", 1).AsMediaLinkEntry().Property(undeclaredProperty.DeepCopy()),
                                ExpectedResultPayloadElement = tc => PayloadBuilder.Entity("TestModel.CityWithMapType").PrimitiveProperty("Id", 1).AsMediaLinkEntry().Property(undeclaredProperty.DeepCopy()),
                                PayloadEdmModel = model,
                                ExpectedException = throwOnUndeclaredPropertyForNonOpenType
                                                        ? ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", "UndeclaredProperty", "TestModel.CityWithMapType")
                                                        : null
                            },
                        };
            });

            testDescriptors = testDescriptors.Concat(undeclaredLinkProperties.SelectMany(undeclaredProperty =>
            {
                return new[]
                    {
                        // In entry
                        new PayloadReaderTestDescriptor(settings)
                        {
                            PayloadElement =
                            PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).Property(undeclaredProperty.DeepCopy()),// :
                            PayloadEdmModel = model,
                            ExpectedException = throwOnUndeclaredPropertyForNonOpenType
                                                    ? ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", "UndeclaredProperty", "TestModel.CityType")
                                                    : null
                        },
                        // In MLE entry
                        new PayloadReaderTestDescriptor(settings)
                        {
                            PayloadElement =
                            PayloadBuilder.Entity("TestModel.CityWithMapType").PrimitiveProperty("Id", 1).AsMediaLinkEntry().Property(undeclaredProperty.DeepCopy()), //:
                            PayloadEdmModel = model,
                            ExpectedException = throwOnUndeclaredPropertyForNonOpenType
                                                    ? ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", "UndeclaredProperty", "TestModel.CityWithMapType")
                                                    : null
                        },
                    };
            }));

            return testDescriptors;
        }

        [TestMethod, TestCategory("Reader.Entries"), Variation(Description = "Change in validation behaviour for collection properties.")]
        public void TypeCheckingOnCollectionPropertyWithModel()
        {
            EdmModel model = new EdmModel();
            var entity = new EdmEntityType("TestNS", "EntityWithCollection");
            entity.AddKeys(entity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            entity.AddStructuralProperty("Collection", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(entity);

            var container = new EdmEntityContainer("TestNS", "DefaultContainer");
            container.AddEntitySet("EntitySet", entity);
            model.AddElement(container);

            EntityInstance entityWithCollection = new EntityInstance("TestNS.EntityWithCollection", false /*isnull*/);
            entityWithCollection.PrimitiveProperty("Id", 1).Property(new PrimitiveMultiValueProperty("Collection",
                new PrimitiveMultiValue("Collection(Edm.String)", false /*isnull*/,
                    new PrimitiveValue[]
                    {
                        new PrimitiveValue(null, "5"),
                        new PrimitiveValue(null, "-32"),
                        new PrimitiveValue(null, "+16"),
                        new PrimitiveValue(null, "0")
                    })));
            entityWithCollection.WithTypeAnnotation(entity);

            EntityInstance expectedCollection = new EntityInstance("TestNS.EntityWithCollection", false /*isnull*/);
            expectedCollection.PrimitiveProperty("Id", 1).Property(new PrimitiveMultiValueProperty("Collection",
                new PrimitiveMultiValue("Collection(Edm.Int32)", false /*isnull*/,
                    new PrimitiveValue[]
                    {
                        new PrimitiveValue(null, 5),
                        new PrimitiveValue(null, -32),
                        new PrimitiveValue(null, 16),
                        new PrimitiveValue(null, 0)
                    }).WithAnnotations(new SerializationTypeNameTestAnnotation() { TypeName = "Collection(Edm.String)" })));
            expectedCollection.WithTypeAnnotation(entity);

            PayloadReaderTestDescriptor testDescriptor = new PayloadReaderTestDescriptor(this.Settings)
            {
                PayloadEdmModel = model,
                PayloadElement = entityWithCollection,
                SkipTestConfiguration = tc => tc.Version < ODataVersion.V4,
                ExpectedResultCallback = tc => new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                {
                    ExpectedPayloadElement = expectedCollection
                }
            };

            this.CombinatorialEngineProvider.RunCombinations(
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                new[] { true, false },
                (testConfiguration, laxMode) =>
                {
                    var descriptor = new PayloadReaderTestDescriptor(testDescriptor);

                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        var testEntityInstance = descriptor.PayloadElement.DeepCopy() as EntityInstance;
                        PropertyInstance testProperty = testEntityInstance.Properties.Single(p => p.Name == "Collection");
                        testProperty.WithPropertyAnnotation(JsonLightConstants.ODataTypeAnnotationName, "Collection(Edm.String)");
                        descriptor.PayloadElement = testEntityInstance;
                    }

                    if (laxMode)
                    {
                        // Run test in Lax mode - type will be inferred from the model and will pass.
                        descriptor.RunTest(testConfiguration.CloneAndApplyBehavior(TestODataBehaviorKind.WcfDataServicesServer));
                    }
                    else
                    {
                        // Run test in Strict mode - type comparison will fail and an exception will be thrown.
                        descriptor.ExpectedResultCallback =
                            (tc) => new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                            {
                                ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatibleType", "Collection(Edm.String)", "Collection(Edm.Int32)")
                            };

                        descriptor.RunTest(testConfiguration);
                    }
                });
        }

        [TestMethod, TestCategory("Reader.Entries"), Variation(Description = "Verifies that UndeclaredPropertyBehavior setting behaves correctly when combined with open type.")]
        public void UndeclaredDeferredLinkOnOpenTypeTest()
        {
            var testCases = undeclaredLinkProperties.Select(
                undeclaredProperty => new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.EntityType").PrimitiveProperty("Id", 42),
                });

            this.RunCombinationsForUndeclaredPropertyBehavior(testCases, false);
        }

        [TestMethod, TestCategory("Reader.Entries"), Variation(Description = "Verifies that UndeclaredPropertyBehavior setting behaves correctly when combined with open type.")]
        public void UndeclaredExpandedLinkOnOpenTypeTestInJsonLight()
        {
            var testCases = new[]
                            {
                                new PayloadReaderTestDescriptor(this.Settings)
                                {
                                    PayloadElement = PayloadBuilder.Entity("TestModel.EntityType").PrimitiveProperty("Id", 42).ExpandedNavigationProperty("UndeclaredProperty", null),
                                    ExpectedResultPayloadElement = t => PayloadBuilder.Entity("TestModel.EntityType").PrimitiveProperty("Id", 42).PrimitiveProperty("UndeclaredProperty", null),
                                },
                            };

            this.RunCombinationsForUndeclaredPropertyBehavior(testCases, true, tc => tc.Format == ODataFormat.Json);
        }

        private void RunCombinationsForUndeclaredPropertyBehavior(IEnumerable<PayloadReaderTestDescriptor> testCases, bool throwOnUndeclaredPropertyForNonOpenType, Func<ReaderTestConfiguration, bool> additionalConfigurationFilter = null)
        {
            EdmModel model = new EdmModel();
            var entity = new EdmEntityType("TestModel", "EntityType", null, false, true);
            entity.AddKeys(entity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(entity);
            var complexType = new EdmComplexType("TestModel", "ComplexType");
            model.AddElement(complexType);

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            container.AddEntitySet("EntitySet", entity);
            model.AddElement(container);
            model.Fixup();

            var testCaseList = testCases.ToList();
            testCaseList.ForEach(t => t.PayloadEdmModel = model);

            this.CombinatorialEngineProvider.RunCombinations(
                testCaseList,
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => !tc.IsRequest && (additionalConfigurationFilter == null || additionalConfigurationFilter(tc))),
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = new ReaderTestConfiguration(testConfiguration);
                    if (!throwOnUndeclaredPropertyForNonOpenType)
                    {
                        testConfiguration.MessageReaderSettings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
                    }

                    testDescriptor.RunTest(testConfiguration);
                });
        }

        /// <summary>
        /// Constructs a nested chain of complex properties in order to test recursive depth limits.
        /// </summary>
        /// <param name="depth">The number of complex values deep to make the chain.</param>
        /// <param name="complexTypeName">The name of the type of the complex value.</param>
        /// <param name="propertyName">The name of the property whose value is a complex type.</param>
        /// <returns>A PropertyInstance with the requested levels of depth.</returns>
        private static PropertyInstance CreateDeeplyNestedProperty(int depth, string complexTypeName, string propertyName)
        {
            // Wrap the recursively generated complex value in a property
            return PayloadBuilder.Property(propertyName, CreateDeeplyNestedComplexValues(depth, complexTypeName, propertyName));
        }

        /// <summary>
        /// Constructs a nested chain of complex values in order to test recursive depth limits.
        /// </summary>
        /// <param name="depth">The number of complex values deep to make the chain.</param>
        /// <param name="complexTypeName">The name of the type of the complex value.</param>
        /// <param name="propertyName">The name of the property whose value is a complex type.</param>
        /// <returns>A complex value with the requested levels of depth (or a null primitive if depth is 0).</returns>
        private static TypedValue CreateDeeplyNestedComplexValues(int depth, string complexTypeName, string propertyName)
        {
            if (depth <= 0)
            {
                return PayloadBuilder.PrimitiveValue(null);
            }

            return PayloadBuilder.ComplexValue(complexTypeName)
                    .Property(PayloadBuilder.Property(propertyName,
                        CreateDeeplyNestedComplexValues(depth - 1, complexTypeName, propertyName)));
        }

        /// <summary>
        /// Constructs a nested chain of complex values inside collections in order to test recursive depth limits.
        /// </summary>
        /// <param name="depth">The number of complex and collection values deep to make the chain.</param>
        /// <param name="complexTypeName">The name of the type of the complex value.</param>
        /// <param name="propertyName">The name of the property whose value is a complex type.</param>
        /// <returns>A complex collection with the requested levels of depth.</returns>
        private static ODataPayloadElement CreateDeeplyNestedComplexValuesWithCollections(int depth, string complexTypeName, string propertyName)
        {
            if (depth <= 0)
            {
                throw new ArgumentException("The value of 'depth' must be greater than zero.");
            }
            if (depth == 2)
            {
                return PayloadBuilder.ComplexMultiValue(EntityModelUtils.GetCollectionTypeName(complexTypeName))
                .Item(PayloadBuilder.ComplexValue(complexTypeName));
            }
            if (depth == 1)
            {
                return PayloadBuilder.ComplexMultiValue(EntityModelUtils.GetCollectionTypeName(complexTypeName));
            }

            return PayloadBuilder.ComplexMultiValue(EntityModelUtils.GetCollectionTypeName(complexTypeName))
                .Item(PayloadBuilder.ComplexValue(complexTypeName).Property(propertyName, CreateDeeplyNestedComplexValuesWithCollections(depth - 2, complexTypeName, propertyName)));
        }

        private static ODataPayloadElement EnsureAnnotationsBeforeProperties(ODataPayloadElement payload)
        {
            var complexPayload = payload.DeepCopy() as ComplexInstance;
            ExceptionUtilities.CheckObjectNotNull(complexPayload, "Payload is not ComplexInstance");

            complexPayload.Properties = complexPayload.Properties
                 .OrderBy(p => p.Name)
                 .ThenBy(
                     (p) =>
                     {
                         var navProperty = p as NavigationPropertyInstance;
                         if (navProperty != null)
                         {
                             if (navProperty.Value is DeferredLink || navProperty.AssociationLink != null)
                             {
                                 return 0;
                             }
                             else
                             {
                                 ExpandedLink expandedLink = navProperty.Value as ExpandedLink;
                                 if (expandedLink != null && expandedLink.ExpandedElement != null)
                                 {
                                     var expandedFeed = expandedLink.ExpandedElement as EntitySetInstance;
                                     if (expandedFeed == null || expandedFeed.Count > 0)
                                     {
                                         return 1;
                                     }
                                 }
                             }
                         }

                         return 2;
                     }).ToArray();

            return complexPayload;
        }

        private static ODataPayloadElement DuplicateNavigationPropertiesToLinkCollection(ODataPayloadElement payloadElement)
        {
            var entityInstance = payloadElement as EntityInstance;
            ExceptionUtilities.CheckObjectNotNull(entityInstance, "Payload is not an EntityInstance");

            var navigationProperties = entityInstance.Properties.OfType<NavigationPropertyInstance>();
            ExceptionUtilities.Assert(navigationProperties.Count() == 2, "More than two nav properties found");

            var firstNavProperty = navigationProperties.ElementAt(0);
            var secondNavProperty = navigationProperties.ElementAt(1);

            ExceptionUtilities.Assert(firstNavProperty.Name == secondNavProperty.Name, "Navigation properties do not have the same name");

            entityInstance.Properties = entityInstance.Properties.Where(p => p.Name != firstNavProperty.Name).ToArray();
            entityInstance.NavigationProperty(new NavigationPropertyInstance(firstNavProperty.Name, PayloadBuilder.LinkCollection().Item(firstNavProperty.Value as Link).Item(secondNavProperty.Value as Link)));

            return entityInstance;
        }

        private static ODataPayloadElement DuplicateNavigationPropertiesToExpandedLink(ODataPayloadElement payloadElement)
        {
            var entityInstance = payloadElement as EntityInstance;
            ExceptionUtilities.CheckObjectNotNull(entityInstance, "Payload is not an EntityInstance");

            var navigationProperties = entityInstance.Properties.OfType<NavigationPropertyInstance>();
            ExceptionUtilities.Assert(navigationProperties.Count() == 2, "More than two nav properties found");

            var firstNavProperty = navigationProperties.ElementAt(0);
            var secondNavProperty = navigationProperties.ElementAt(1);
            ExceptionUtilities.Assert(firstNavProperty.Name == secondNavProperty.Name, "Navigation properties do not have the same name");

            var expandedLink = (firstNavProperty.Value is ExpandedLink) ? firstNavProperty.Value as ExpandedLink : secondNavProperty.Value as ExpandedLink;
            var deferredLink = (firstNavProperty.Value is DeferredLink) ? firstNavProperty.Value as DeferredLink : secondNavProperty.Value as DeferredLink;

            ExceptionUtilities.Assert(expandedLink != null && deferredLink != null, "Expected one expanded link and one deferred link");
            expandedLink.UriString = deferredLink.UriString;

            entityInstance.Properties = entityInstance.Properties.Where(p => p.Name != firstNavProperty.Name).ToArray();
            entityInstance.NavigationProperty(new NavigationPropertyInstance(firstNavProperty.Name, expandedLink, PayloadBuilder.DeferredLink(null)));

            return entityInstance;
        }

    }
}
