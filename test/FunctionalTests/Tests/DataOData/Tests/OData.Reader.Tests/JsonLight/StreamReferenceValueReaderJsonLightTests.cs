//---------------------------------------------------------------------
// <copyright file="StreamReferenceValueReaderJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using TestModels = Microsoft.Test.OData.Utils.Metadata.TestModels;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of various complex value JSON Light payloads.
    /// </summary>
    [TestClass, TestCase]
    public class StreamReferenceValueReaderJsonLightTests : ODataReaderTestCase
    {
        [InjectDependency]
        public IPayloadGenerator PayloadGenerator { get; set; }

        private PayloadReaderTestDescriptor.Settings settings;

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings
        {
            get { return this.settings; }
            set { this.settings = value; this.settings.ExpectedResultSettings.ObjectModelToPayloadElementConverter = new JsonLightObjectModelToPayloadElementConverter(); }
        }

        private sealed class StreamPropertyTestCase
        {
            public string DebugDescription { get; set; }
            public string Json { get; set; }
            public EntityInstance ExpectedEntity { get; set; }
            public ExpectedException ExpectedException { get; set; }
            public bool OnlyResponse { get; set; }
            public IEdmTypeReference OwningEntityType { get; set; }
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct reading of stream properties (stream reference values) with fully specified metadata.")]
        public void StreamPropertyTest()
        {
            IEdmModel model = TestModels.BuildTestModel();

            var testCases = new[]
            {
                new StreamPropertyTestCase
                {
                    DebugDescription = "Just edit link",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", "http://odata.org/test/Cities(1)/Skyline", "http://odata.org/streamproperty/editlink", null, null),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/streamproperty/editlink\""
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Just read link",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", "http://odata.org/streamproperty/readlink", "http://odata.org/test/Cities(1)/Skyline", null, null),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaReadLinkAnnotationName) + "\":\"http://odata.org/streamproperty/readlink\""
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Just content type",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", "http://odata.org/test/Cities(1)/Skyline", "http://odata.org/test/Cities(1)/Skyline", "streamproperty:contenttype", null),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaContentTypeAnnotationName) + "\":\"streamproperty:contenttype\""
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Just ETag",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", "http://odata.org/test/Cities(1)/Skyline", "http://odata.org/test/Cities(1)/Skyline", null, "streamproperty:etag"),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaETagAnnotationName) + "\":\"streamproperty:etag\""
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Everything",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", "http://odata.org/streamproperty/readlink", "http://odata.org/streamproperty/editlink", "streamproperty:contenttype", "streamproperty:etag"),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/streamproperty/editlink\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaReadLinkAnnotationName) + "\":\"http://odata.org/streamproperty/readlink\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaContentTypeAnnotationName) + "\":\"streamproperty:contenttype\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaETagAnnotationName) + "\":\"streamproperty:etag\""
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Just custom annotation - should report empty stream property",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", "http://odata.org/test/Cities(1)/Skyline", "http://odata.org/test/Cities(1)/Skyline", null, null),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", "custom.value") + "\":\"value\""
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Everything with custom annotation - custom annotations should be ignored",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", "http://odata.org/streamproperty/readlink", "http://odata.org/streamproperty/editlink", "streamproperty:contenttype", "streamproperty:etag"),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/streamproperty/editlink\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", "custom.value") + "\":\"value\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaReadLinkAnnotationName) + "\":\"http://odata.org/streamproperty/readlink\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaContentTypeAnnotationName) + "\":\"streamproperty:contenttype\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaETagAnnotationName) + "\":\"streamproperty:etag\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", "custom.type") + "\":42"
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "With odata.type annotation - should fail",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataTypeAnnotationName) + "\":\"Edm.Stream\"",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_UnexpectedStreamPropertyAnnotation", "Skyline", JsonLightConstants.ODataTypeAnnotationName)
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Everything with navigation link URL annotation - should fail",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", "http://odata.org/streamproperty/readlink", "http://odata.org/streamproperty/editlink", "streamproperty:contenttype", "streamproperty:etag"),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/streamproperty/editlink\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaReadLinkAnnotationName) + "\":\"http://odata.org/streamproperty/readlink\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/streamproperty/navlink\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaContentTypeAnnotationName) + "\":\"streamproperty:contenttype\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaETagAnnotationName) + "\":\"streamproperty:etag\"",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_UnexpectedStreamPropertyAnnotation", "Skyline", JsonLightConstants.ODataNavigationLinkUrlAnnotationName)
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid edit link - wrong primitive",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaEditLinkAnnotationName) + "\":42",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_CannotReadPropertyValueAsString", "42", JsonLightConstants.ODataMediaEditLinkAnnotationName),
                    OnlyResponse = true,
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid edit link - null",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaEditLinkAnnotationName) + "\":null",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightReaderUtils_AnnotationWithNullValue", JsonLightConstants.ODataMediaEditLinkAnnotationName),
                    OnlyResponse = true,
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid read link - wrong primitive",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaReadLinkAnnotationName) + "\":true",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_CannotReadPropertyValueAsString", "True", JsonLightConstants.ODataMediaReadLinkAnnotationName),
                    OnlyResponse = true,
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid read link - null",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaReadLinkAnnotationName) + "\":null",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightReaderUtils_AnnotationWithNullValue", JsonLightConstants.ODataMediaReadLinkAnnotationName),
                    OnlyResponse = true,
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid ETag - non primitive",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaETagAnnotationName) + "\":[]",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartArray"),
                    OnlyResponse = true,
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid ETag - null",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaETagAnnotationName) + "\":null",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightReaderUtils_AnnotationWithNullValue", JsonLightConstants.ODataMediaETagAnnotationName),
                    OnlyResponse = true,
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid content type - non primitive",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaContentTypeAnnotationName) + "\":{}",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartObject"),
                    OnlyResponse = true,
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid content type - null",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaContentTypeAnnotationName) + "\":null",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightReaderUtils_AnnotationWithNullValue", JsonLightConstants.ODataMediaContentTypeAnnotationName),
                    OnlyResponse = true,
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Open stream property",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("OpenSkyline", null, "http://odata.org/streamproperty/editlink", null, null),
                    OwningEntityType = model.FindDeclaredType("TestModel.CityOpenType").ToTypeReference(),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("OpenSkyline", JsonLightConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/streamproperty/editlink\"",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_OpenPropertyWithoutValue", "OpenSkyline"),
                    OnlyResponse = true
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Undeclared stream property",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("NewSkyline", null, "http://odata.org/streamproperty/editlink", null, null),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("NewSkyline", JsonLightConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/streamproperty/editlink\"",
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", "NewSkyline", "TestModel.CityType"),
                    OnlyResponse = true
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Stream property declared with non-stream type",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Name", null, "http://odata.org/streamproperty/editlink", null, null),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", JsonLightConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/streamproperty/editlink\"",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_PropertyWithoutValueWithWrongType", "Name", "Edm.String"),
                    OnlyResponse = true
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Stream property with value",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, "http://odata.org/streamproperty/editlink", null, null),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/streamproperty/editlink\"," +
                        "\"Skyline\":\"value\"",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_StreamPropertyWithValue", "Skyline"),
                    OnlyResponse = true
                },
            };

            this.RunStreamPropertyTest(model, testCases);
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct reading of stream properties (stream reference values) with fully specified metadata.")]
        public void StreamPropertyTestWithRelativeLinkUris()
        {
            IEdmModel model = TestModels.BuildTestModel();

            var testCases = new[]
            {
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid edit link - non-URL",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaEditLinkAnnotationName) + "\":\"xxx yyy zzz\"",
                    ExpectedException = null,
                    OnlyResponse = true,
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid read link - non-URL",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Skyline", JsonLightConstants.ODataMediaReadLinkAnnotationName) + "\":\"xxx yyy zzz\"",
                    ExpectedException = null,
                    OnlyResponse = true,
                },
            };

            this.RunStreamPropertyTest(model, testCases);
        }

        private void RunStreamPropertyTest(IEdmModel model, IEnumerable<StreamPropertyTestCase> testCases)
        {
            var cityType = model.FindDeclaredType("TestModel.CityType").ToTypeReference();
            var cities = model.EntityContainer.FindEntitySet("Cities");
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = testCases.Select(testCase =>
            {
                IEdmTypeReference entityType = testCase.OwningEntityType ?? cityType;
                EntityInstance entity = PayloadBuilder.Entity(entityType.FullName()).PrimitiveProperty("Id", 1)
                    .JsonRepresentation(
                        "{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities/" + entityType.FullName() + "()/$entity\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"" + entityType.FullName() + "\"," +
                            "\"Id\": 1," +
                            testCase.Json +
                        "}")
                    .ExpectedEntityType(entityType, cities);
                foreach (NamedStreamInstance streamProperty in testCase.ExpectedEntity.Properties.OfType<NamedStreamInstance>())
                {
                    entity.Add(streamProperty.DeepCopy());
                }

                return new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = testCase.DebugDescription,
                    PayloadEdmModel =  model,
                    PayloadElement = entity,
                    ExpectedException = testCase.ExpectedException,
                    SkipTestConfiguration = tc => testCase.OnlyResponse ? tc.IsRequest : false
                };
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    if (testConfiguration.IsRequest)
                    {
                        testDescriptor = new PayloadReaderTestDescriptor(testDescriptor)
                        {
                            ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_StreamPropertyInRequest")
                        };
                    }

                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    var testConfigClone = new ReaderTestConfiguration(testConfiguration);
                    testConfigClone.MessageReaderSettings.BaseUri = null;

                    testDescriptor.RunTest(testConfigClone);
                });
        }
    }
}
