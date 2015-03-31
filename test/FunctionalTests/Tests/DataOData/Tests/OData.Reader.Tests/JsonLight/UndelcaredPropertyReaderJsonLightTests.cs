﻿//---------------------------------------------------------------------
// <copyright file="UndelcaredPropertyReaderJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using TestModels = Microsoft.Test.OData.Utils.Metadata.TestModels;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of undeclared properties in JSON Light.
    /// </summary>
    [TestClass, TestCase]
    public class UndeclaredPropertyReaderJsonLightTests : ODataReaderTestCase
    {
        private PayloadReaderTestDescriptor.Settings jsonLightSettings;

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings JsonLightSettings
        {
            get { return this.jsonLightSettings; }
            set { this.jsonLightSettings = value; this.jsonLightSettings.ExpectedResultSettings.ObjectModelToPayloadElementConverter = new JsonLightObjectModelToPayloadElementConverter(); }
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the reading of undeclared navigation links on entry payloads.")]
        public void UndeclaredNavigationLinkTests()
        {
            IEdmModel model = TestModels.BuildTestModel();

            IEnumerable<UndeclaredPropertyTestCase> testCases = new[]
            {
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Just navigation link",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navigationlink\"",
                    IsLink = true,
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("UndeclaredProperty", "http://odata.org/navigationlink")
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Just association link",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/associationlink\"",
                    IsLink = true,
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("UndeclaredProperty", null, "http://odata.org/associationlink")
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Navigation and association link",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navigationlink\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/associationlink\"",
                    IsLink = true,
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("UndeclaredProperty", "http://odata.org/navigationlink", "http://odata.org/associationlink")
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Navigation and association link with custom annotation",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navigationlink\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", "custom.annotation") + "\":null," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/associationlink\"",
                    IsLink = true,
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("UndeclaredProperty", "http://odata.org/navigationlink", "http://odata.org/associationlink")
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Navigation link with another odata.type annotation - should fail",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataTypeAnnotationName) + "\":\"TestModel.OfficeType\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navigationlink\"",
                    IsLink = true,
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_UnexpectedDeferredLinkPropertyAnnotation", "UndeclaredProperty", JsonLightConstants.ODataTypeAnnotationName)
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Association link with another odata.mediaEditLink annotation - should fail",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/mediaeditlink\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/associationlink\"",
                    IsLink = true,
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_UnexpectedDeferredLinkPropertyAnnotation", "UndeclaredProperty", JsonLightConstants.ODataMediaEditLinkAnnotationName)
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Expanded feed navigation link",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navigationlink\"," +
                        "\"UndeclaredProperty\":[]",
                    IsLink = true,
                    IsValue = true,
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("UndeclaredProperty", "http://odata.org/navigationlink"),
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Expanded entry navigation link",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/associationlink\"," +
                        "\"UndeclaredProperty\":{}",
                    IsLink = true,
                    IsValue = true,
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("UndeclaredProperty", null, "http://odata.org/associationlink"),
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Expanded null entry navigation link",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navigationlink\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/associationlink\"," +
                        "\"UndeclaredProperty\":null",
                    IsLink = true,
                    IsValue = true,
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("UndeclaredProperty", "http://odata.org/navigationlink", "http://odata.org/associationlink"),
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Expanded navigation link with wrong value",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navigationlink\"," +
                        "\"UndeclaredProperty\":42",
                    IsLink = true,
                    IsValue = true,
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_CannotReadNavigationPropertyValue", "UndeclaredProperty")
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                TestReaderUtils.ODataUndeclaredPropertyBehaviorKindsCombinations,
                // Undeclared properties are only allowed in responses
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                (testCase, undeclaredPropertyBehaviorKinds, testConfiguration) =>
                {
                    PayloadReaderTestDescriptor testDescriptor = testCase.ToTestDescriptor(this.Settings, model, undeclaredPropertyBehaviorKinds);
                    testConfiguration = new ReaderTestConfiguration(testConfiguration);
                    testConfiguration.MessageReaderSettings.UndeclaredPropertyBehaviorKinds = undeclaredPropertyBehaviorKinds;

                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the reading of undeclared stream properties on entry payloads.")]
        public void UndeclaredStreamPropertyTests()
        {
            IEdmModel model = TestModels.BuildTestModel();

            IEnumerable<UndeclaredPropertyTestCase> testCases = new[]
            {
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Just edit link",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/mediaeditlink\"",
                    IsLink = true,
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("UndeclaredProperty", null, "http://odata.org/mediaeditlink", null, null)
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Just read link",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataMediaReadLinkAnnotationName) + "\":\"http://odata.org/mediareadlink\"",
                    IsLink = true,
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("UndeclaredProperty", "http://odata.org/mediareadlink", null, null, null)
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Just content type",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataMediaContentTypeAnnotationName) + "\":\"media/contenttype\"",
                    IsLink = true,
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("UndeclaredProperty", null, null, "media/contenttype", null)
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Just ETag",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataMediaETagAnnotationName) + "\":\"etag\"",
                    IsLink = true,
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("UndeclaredProperty", null, null, null, "etag")
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Everything",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/mediaeditlink\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataMediaReadLinkAnnotationName) + "\":\"http://odata.org/mediareadlink\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataMediaContentTypeAnnotationName) + "\":\"media/contenttype\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataMediaETagAnnotationName) + "\":\"etag\"",
                    IsLink = true,
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("UndeclaredProperty", "http://odata.org/mediareadlink", "http://odata.org/mediaeditlink", "media/contenttype", "etag")
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Everything with custom annotations",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/mediaeditlink\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataMediaReadLinkAnnotationName) + "\":\"http://odata.org/mediareadlink\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", "custom.annotation") + "\":\"value\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataMediaContentTypeAnnotationName) + "\":\"media/contenttype\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataMediaETagAnnotationName) + "\":\"etag\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", "custom.annotation2") + "\":42",
                    IsLink = true,
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("UndeclaredProperty", "http://odata.org/mediareadlink", "http://odata.org/mediaeditlink", "media/contenttype", "etag")
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Stream property with odata.type annotation",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/mediaeditlink\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataTypeAnnotationName) + "\":\"Edm.Stream\"",
                    IsLink = true,
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_UnexpectedStreamPropertyAnnotation", "UndeclaredProperty", JsonLightConstants.ODataTypeAnnotationName)
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Stream property with a value",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/mediaeditlink\"," +
                        "\"UndeclaredProperty\":null",
                    IsLink = true,
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_StreamPropertyWithValue", "UndeclaredProperty")
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                TestReaderUtils.ODataUndeclaredPropertyBehaviorKindsCombinations,
                // Undeclared properties are only allowed in responses
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                (testCase, undeclaredPropertyBehaviorKinds, testConfiguration) =>
                {
                    var settings = testConfiguration.Format == ODataFormat.Json ? this.JsonLightSettings : this.Settings;
                    PayloadReaderTestDescriptor testDescriptor = testCase.ToTestDescriptor(settings, model, undeclaredPropertyBehaviorKinds);
                    testConfiguration = new ReaderTestConfiguration(testConfiguration);
                    testConfiguration.MessageReaderSettings.UndeclaredPropertyBehaviorKinds = undeclaredPropertyBehaviorKinds;

                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the reading of undeclared value properties on entry payloads.")]
        public void UndeclaredValuePropertyTests()
        {
            IEdmModel model = TestModels.BuildTestModel();

            IEnumerable<UndeclaredPropertyTestCase> testCases = new[]
            {
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Number without type",
                    Json = "\"UndeclaredProperty\":42",
                    IsValue = true,
                    ExpectedEntity = PayloadBuilder.Entity().PrimitiveProperty("UndeclaredProperty", 42),
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "String without type",
                    Json = "\"UndeclaredProperty\":\"value\"",
                    IsValue = true,
                    ExpectedEntity = PayloadBuilder.Entity().PrimitiveProperty("UndeclaredProperty", "value"),
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "null without type",
                    Json = "\"UndeclaredProperty\":null",
                    IsValue = true,
                    ExpectedEntity = PayloadBuilder.Entity().PrimitiveProperty("UndeclaredProperty", null),
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Boolean without type",
                    Json = "\"UndeclaredProperty\":false",
                    IsValue = true,
                    ExpectedEntity = PayloadBuilder.Entity().PrimitiveProperty("UndeclaredProperty", false),
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Number with type",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataTypeAnnotationName) + "\":\"Edm.Int16\"," +
                        "\"UndeclaredProperty\":42",
                    IsValue = true,
                    ExpectedEntity = PayloadBuilder.Entity().PrimitiveProperty("UndeclaredProperty", (short)42),
                },

                // TODO read: Microsoft.OData.Core.ODataException: A type named 'TestModel.Unknown' could not be resolved by the model. When a model is available, each type name must resolve to a valid type. 
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Number with invalid type - should work, the type is ignored",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataTypeAnnotationName) + "\":\"TestModel.Unknown\"," +
                        "\"UndeclaredProperty\":42",
                    IsValue = true,
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_UnrecognizedTypeName", "TestModel.Unknown")
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Number with another odata annotation - should fail",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataBindAnnotationName) + "\":\"http://odata.org/reference\"," +
                        "\"UndeclaredProperty\":42",
                    IsValue = true,
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_UnexpectedDataPropertyAnnotation", "UndeclaredProperty", JsonLightConstants.ODataBindAnnotationName)
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Property without value and with no recognized annotations - should fail",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", "custom.annotation") + "\":\"value\"",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_PropertyWithoutValueWithUnknownType", "UndeclaredProperty")
                },
                new UndeclaredPropertyTestCase
                {
                    DebugDescription = "Property without value and with known annotation - should fail",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("UndeclaredProperty", JsonLightConstants.ODataBindAnnotationName) + "\":\"http://odata.org/reference\"",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_PropertyWithoutValueWithUnknownType", "UndeclaredProperty")
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                TestReaderUtils.ODataUndeclaredPropertyBehaviorKindsCombinations,
                // Undeclared properties are only allowed in responses
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                (testCase, undeclaredPropertyBehaviorKinds, testConfiguration) =>
                {
                    PayloadReaderTestDescriptor testDescriptor = testCase.ToTestDescriptor(this.Settings, model, undeclaredPropertyBehaviorKinds);
                    testConfiguration = new ReaderTestConfiguration(testConfiguration);
                    testConfiguration.MessageReaderSettings.UndeclaredPropertyBehaviorKinds = undeclaredPropertyBehaviorKinds;

                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        private sealed class UndeclaredPropertyTestCase
        {
            public string DebugDescription { get; set; }
            public string Json { get; set; }
            public EntityInstance ExpectedEntity { get; set; }
            public ExpectedException ExpectedException { get; set; }
            public bool IsLink { get; set; }
            public bool IsValue { get; set; }

            public PayloadReaderTestDescriptor ToTestDescriptor(PayloadReaderTestDescriptor.Settings settings, IEdmModel model, ODataUndeclaredPropertyBehaviorKinds undeclaredPropertyBehaviorKinds)
            {
                var cityType = model.FindDeclaredType("TestModel.CityType").ToTypeReference();
                var cities = model.EntityContainer.FindEntitySet("Cities");
                EntityInstance entity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1)
                    .ExpectedEntityType(cityType, cities)
                    .JsonRepresentation(
                        "{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.CityType\"," +
                            "\"Id\":1," +
                            this.Json +
                        "}");
                foreach (PropertyInstance property in this.ExpectedEntity.Properties)
                {
                    entity.Add(property.DeepCopy());
                }

                ExpectedException expectedException = this.ExpectedException;
                if (this.IsLink && !undeclaredPropertyBehaviorKinds.HasFlag(ODataUndeclaredPropertyBehaviorKinds.ReportUndeclaredLinkProperty))
                {
                    expectedException = ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", "UndeclaredProperty", "TestModel.CityType");
                }

                if (this.IsValue && !undeclaredPropertyBehaviorKinds.HasFlag(ODataUndeclaredPropertyBehaviorKinds.IgnoreUndeclaredValueProperty))
                {
                    expectedException = ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", "UndeclaredProperty", "TestModel.CityType");
                }

                return new PayloadReaderTestDescriptor(settings)
                {
                    DebugDescription = this.DebugDescription,
                    PayloadElement = entity,
                    PayloadEdmModel = model,
                    ExpectedException = expectedException
                };
            }
        }
    }
}
