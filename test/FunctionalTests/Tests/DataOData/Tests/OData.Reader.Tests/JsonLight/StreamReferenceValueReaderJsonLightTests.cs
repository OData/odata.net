//---------------------------------------------------------------------
// <copyright file="StreamReferenceValueReaderJsonTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Json
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
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Edm;
    using TestModels = Microsoft.Test.OData.Utils.Metadata.TestModels;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of various complex value JSON Light payloads.
    /// </summary>
    [TestClass, TestCase]
    public class StreamReferenceValueReaderJsonTests : ODataReaderTestCase
    {
        [InjectDependency]
        public IPayloadGenerator PayloadGenerator { get; set; }

        private PayloadReaderTestDescriptor.Settings settings;

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings
        {
            get { return this.settings; }
            set { this.settings = value; this.settings.ExpectedResultSettings.ObjectModelToPayloadElementConverter = new JsonObjectModelToPayloadElementConverter(); }
        }

        private sealed class StreamPropertyTestCase
        {
            public string DebugDescription { get; set; }
            public string Json { get; set; }
            public EntityInstance ExpectedEntity { get; set; }
            public ExpectedException ExpectedException { get; set; }
            public bool OnlyResponse { get; set; }
            public IEdmTypeReference OwningEntityType { get; set; }
            public bool InvalidOnRequest { get; set; }
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
                    InvalidOnRequest = true,
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", "http://odata.org/test/Cities(1)/Skyline", "http://odata.org/streamproperty/editlink", null, null),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/streamproperty/editlink\"",
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Just read link",
                    InvalidOnRequest = true,
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", "http://odata.org/streamproperty/readlink", "http://odata.org/test/Cities(1)/Skyline", null, null),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaReadLinkAnnotationName) + "\":\"http://odata.org/streamproperty/readlink\""
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Just content type",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, "streamproperty:contenttype", null),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaContentTypeAnnotationName) + "\":\"streamproperty:contenttype\""
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Just ETag",
                    InvalidOnRequest = true,
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", "http://odata.org/test/Cities(1)/Skyline", "http://odata.org/test/Cities(1)/Skyline", null, "streamproperty:etag"),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaETagAnnotationName) + "\":\"streamproperty:etag\""
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Everything",
                    InvalidOnRequest = true,
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", "http://odata.org/streamproperty/readlink", "http://odata.org/streamproperty/editlink", "streamproperty:contenttype", "streamproperty:etag"),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/streamproperty/editlink\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaReadLinkAnnotationName) + "\":\"http://odata.org/streamproperty/readlink\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaContentTypeAnnotationName) + "\":\"streamproperty:contenttype\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaETagAnnotationName) + "\":\"streamproperty:etag\""
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Just custom annotation - should report empty stream property",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", "custom.value") + "\":\"value\""
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Everything with custom annotation - custom annotations should be ignored",
                    InvalidOnRequest = true,
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", "http://odata.org/streamproperty/readlink", "http://odata.org/streamproperty/editlink", "streamproperty:contenttype", "streamproperty:etag"),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/streamproperty/editlink\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", "custom.value") + "\":\"value\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaReadLinkAnnotationName) + "\":\"http://odata.org/streamproperty/readlink\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaContentTypeAnnotationName) + "\":\"streamproperty:contenttype\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaETagAnnotationName) + "\":\"streamproperty:etag\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", "custom.type") + "\":42"
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "With odata.type annotation - should fail",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataTypeAnnotationName) + "\":\"Edm.Stream\"",
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Everything with navigation link URL annotation - should fail",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", "http://odata.org/streamproperty/readlink", "http://odata.org/streamproperty/editlink", "streamproperty:contenttype", "streamproperty:etag"),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/streamproperty/editlink\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaReadLinkAnnotationName) + "\":\"http://odata.org/streamproperty/readlink\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/streamproperty/navlink\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaContentTypeAnnotationName) + "\":\"streamproperty:contenttype\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaETagAnnotationName) + "\":\"streamproperty:etag\"",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonResourceDeserializer_UnexpectedStreamPropertyAnnotation", "Skyline", JsonConstants.ODataNavigationLinkUrlAnnotationName)
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid edit link - wrong primitive",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaEditLinkAnnotationName) + "\":42",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_CannotReadPropertyValueAsString", "42", JsonConstants.ODataMediaEditLinkAnnotationName),
                    OnlyResponse = true,
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid edit link - null",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaEditLinkAnnotationName) + "\":null",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonReaderUtils_AnnotationWithNullValue", JsonConstants.ODataMediaEditLinkAnnotationName),
                    OnlyResponse = true,
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid read link - wrong primitive",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaReadLinkAnnotationName) + "\":true",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_CannotReadPropertyValueAsString", "True", JsonConstants.ODataMediaReadLinkAnnotationName),
                    OnlyResponse = true,
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid read link - null",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaReadLinkAnnotationName) + "\":null",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonReaderUtils_AnnotationWithNullValue", JsonConstants.ODataMediaReadLinkAnnotationName),
                    OnlyResponse = true,
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid ETag - non primitive",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaETagAnnotationName) + "\":[]",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartArray"),
                    OnlyResponse = true,
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid ETag - null",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaETagAnnotationName) + "\":null",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonReaderUtils_AnnotationWithNullValue", JsonConstants.ODataMediaETagAnnotationName),
                    OnlyResponse = true,
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid content type - non primitive",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaContentTypeAnnotationName) + "\":{}",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartObject"),
                    OnlyResponse = true,
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid content type - null",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaContentTypeAnnotationName) + "\":null",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonReaderUtils_AnnotationWithNullValue", JsonConstants.ODataMediaContentTypeAnnotationName),
                    OnlyResponse = true,
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Open stream property",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("OpenSkyline", null, "http://odata.org/streamproperty/editlink", null, null),
                    OwningEntityType = model.FindDeclaredType("TestModel.CityOpenType").ToTypeReference(),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("OpenSkyline", JsonConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/streamproperty/editlink\"",
                    OnlyResponse = true
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Undeclared stream property (disallowed by default)",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("NewSkyline", null, "http://odata.org/streamproperty/editlink", null, null),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("NewSkyline", JsonConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/streamproperty/editlink\"",
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", "NewSkyline", "TestModel.CityType"),
                    OnlyResponse = true
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Stream property declared with non-stream type",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    Json = 
                        "\"" + JsonUtils.GetPropertyAnnotationName("Name", JsonConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/streamproperty/editlink\"",
                    OnlyResponse = true
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Stream property with value",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, "http://odata.org/streamproperty/editlink", null, null),
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaEditLinkAnnotationName) + "\":\"http://odata.org/streamproperty/editlink\"," +
                        "\"Skyline\":\"value\"",
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
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaEditLinkAnnotationName) + "\":\"xxx yyy zzz\"",
                    ExpectedException = null,
                    OnlyResponse = true,
                },
                new StreamPropertyTestCase
                {
                    DebugDescription = "Invalid read link - non-URL",
                    ExpectedEntity = PayloadBuilder.Entity().StreamProperty("Skyline", null, null, null, null),
                    Json = 
                        "\"" + JsonUtils.GetPropertyAnnotationName("Skyline", JsonConstants.ODataMediaReadLinkAnnotationName) + "\":\"xxx yyy zzz\"",
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
                            "\"" + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities/" + entityType.FullName() + "()/$entity\"," +
                            "\"" + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + "\":\"" + entityType.FullName() + "\"," +
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
                    SkipTestConfiguration = tc => testCase.OnlyResponse ? tc.IsRequest : false,
                    InvalidOnRequest = testCase.InvalidOnRequest
                };
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    if (testConfiguration.IsRequest && testDescriptor.InvalidOnRequest)
                    {
                        testDescriptor = new PayloadReaderTestDescriptor(testDescriptor)
                        {
                            ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonResourceDeserializer_StreamPropertyInRequest")
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
