//---------------------------------------------------------------------
// <copyright file="ServiceDocumentReaderJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces
    using System;
    using System.Linq;
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
    #endregion Namespaces

    /// <summary>
    /// Tests reading of various JSON Light service document payloads.
    /// </summary>
    [TestClass, TestCase]
    public class ServiceDocumentReaderJsonLightTests : ODataReaderTestCase
    {
        private const string baseUri = "http://odata.org/test/";

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the the reading of JSON Light service document payloads.")]
        public void ServiceDocumentReaderJsonLightTest()
        {
            var errorTestCases = new[]
            {
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "unrecognized property in the workspace object.",
                    Json = "{ \"@" + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"unrecognized\": 42, \"value\": [ {\"name\": \"EntitySetName\", \"url\":\"EntitySetLink\"} ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInServiceDocument", "unrecognized", "value")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "unrecognized property in a service document element.",
                    Json = "{ \"@" + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ {\"name\":\"EntitySetName\", \"url\":\"EntitySetLink\", \"unrecognized\": 42} ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInServiceDocumentElement", "unrecognized", "name", "url")
                },

                #region missing required properties
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "service document element without a 'name' (required in JSON light, but doesn't exist in the other formats)",
                    Json = "{ \"@" + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ {\"url\":\"EntitySetLink\"} ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement", "name")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "service document element without a 'url'",
                    Json = "{ \"@" + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ {\"name\":\"EntitySetName\"} ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement", "url")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "null value for 'name'",
                    Json = "{ \"@" + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ {\"name\":null, \"url\":\"EntitySetLink\"} ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement", "name")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "null value for 'url'",
                    Json = "{ \"@" + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ {\"name\":\"EntitySetName\", \"url\":null} ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_ServiceDocumentElementUrlMustNotBeNull")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "no 'value' property",
                    Json = "{ \"@" + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\" }",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightServiceDocumentDeserializer_MissingValuePropertyInServiceDocument", "value")
                },
                #endregion missing required properties

                #region duplicate properties
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "multiple 'value' properties",
                    Json = "{ \"@" + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ {\"name\": \"EntitySetName\", \"url\":\"EntitySetLink\"} ], \"value\": [ {\"name\": \"EntitySetName\", \"url\":\"EntitySetLink\"} ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocument", "value")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "multiple 'name' properties in an entity set.",
                    Json = "{ \"@" + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ {\"name\": \"EntitySetName\", \"url\":\"EntitySetLink\", \"name\": \"Second name\"}]}",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement", "name")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "multiple 'url' properties in an entity set.",
                    Json = "{ \"@" + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ {\"name\": \"EntitySetName\", \"url\":\"EntitySetLink\", \"url\": \"SecondLink\"}]}",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement", "url")
                },

                #endregion duplicate properties/annotations

                #region incorrect json structure
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "invalid top-level JSON node (an array)",
                    Json = "[]",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "StartArray")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "invalid top-level JSON node (a primitive value)",
                    Json = "42",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "invalid value of 'value' property (a primitive value)",
                    Json = "{ \"@" + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": 42 }",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartArray", "PrimitiveValue")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "invalid value of 'value' property (an object value)",
                    Json = "{ \"@" + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": { } }",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartArray", "StartObject")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "invalid value inside the array value of the 'value' property (a nested array value)",
                    Json = "{ \"@" + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ [] ] }",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "StartArray")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "invalid value inside the array value of the 'value' property (integer value)",
                    Json = "{ \"@" + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ 42 ] }",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "invalid value inside the array value of the 'value' property (boolean value)",
                    Json = "{ \"@" + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ true ] }",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "invalid value inside the array value of the 'value' property (string value)",
                    Json = "{ \"@" + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ \"string\" ] }",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "invalid value inside the array value of the 'value' property (null)",
                    Json = "{ \"@" + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ null ] }",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue")
                },
                #endregion incorrect json structure
            };

            var testDescriptors = errorTestCases.Select(tc =>
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection(null, baseUri + "EntitySetLink", "EntitySetName"))
                        .JsonRepresentation(tc.Json),
                    PayloadEdmModel = new EdmModel(),
                    SkipTestConfiguration = testConfiguration => testConfiguration.IsRequest,
                    ExpectedException = tc.ExpectedException,
                    DebugDescription = tc.DebugDescription
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    // clone the ReaderTestConfiguration and set the base URI.
                    testConfiguration = new ReaderTestConfiguration(testConfiguration);
                    testConfiguration.MessageReaderSettings.BaseUri = null;

                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the reading of custom annotations in JSON Lite service document payloads.")]
        public void ServiceDocumentReaderJsonLightCustomAnnotationsTest()
        {
            var testCases = new[]
            {
                #region success test cases
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "Custom property annotation on \"name\" should be ignored.",
                    Json = @"{
                                ""@" + JsonLightConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                """ + JsonLightConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""name@cn.foo"":""ignored"",
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                            }",
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "Custom instance annotation inside a resource collection should be ignored.",
                    Json = @"{
                                ""@" + JsonLightConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                """ + JsonLightConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""@cn.foo"":""ignored"",
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                            }",
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "Custom property annotation on \"value\" should be ignored.",
                    Json = @"{
                                ""@" + JsonLightConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                ""value@cn.foo"": ""ignored"",
                                """ + JsonLightConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                            }",
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "Custom instance annotation on the top level should be ignored.",
                    Json = @"{
                                ""@" + JsonLightConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                ""@cn.foo"": ""ignored"",
                                """ + JsonLightConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                            }",
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "Annotations at different scopes with the same name should be ignored and should not throw.",
                    Json = @"{
                                ""@" + JsonLightConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                ""@cn.foo"": ""ignored"",
                                ""value@cn.foo"": ""ignored"",
                                """ + JsonLightConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""@cn.foo"": ""ignored"",
                                        ""name"":""EntitySetName"",
                                        ""url@cn.foo"": ""ignored"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                            }",
                },
                #endregion success test cases

                #region error test cases
                new JsonLightServiceDocumentReaderTestCase
                {
                    Json = @"{
                                ""@" + JsonLightConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                """ + JsonLightConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""name@cn.foo"":""out of order"",
                                        ""url"":""EntitySetLink"",
                                        ""name"":""EntitySetName""
                                    },
                                ]
                            }",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationWithoutProperty", "name")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    Json = @"{
                                ""@" + JsonLightConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                """ + JsonLightConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""url"":""EntitySetLink"",
                                        ""name"":""EntitySetName""
                                    },
                                ],
                                ""value@cn.foo"":""out of order""
                            }",
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesChecker_PropertyAnnotationAfterTheProperty", "cn.foo", "value")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    Json = @"{
                                ""@" + JsonLightConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                """ + JsonLightConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""name@cn.foo"":""ignored"",
                                        ""name@cn.foo"":""ignored"",
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                             }",
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesChecker_DuplicateAnnotationForPropertyNotAllowed", "cn.foo", "name")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    Json = @"{
                                ""@" + JsonLightConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                """ + JsonLightConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""@cn.foo"":""ignored"",
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink"",
                                        ""@cn.foo"":""ignored""
                                    },
                                ]
                             }",
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesChecker_DuplicateAnnotationNotAllowed", "cn.foo")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    Json = @"{
                                ""@" + JsonLightConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                ""value@cn.foo"":""ignored"",
                                ""value@cn.foo"":""ignored"",
                                """ + JsonLightConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                             }",
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesChecker_DuplicateAnnotationForPropertyNotAllowed", "cn.foo", "value")
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    Json = @"{
                                ""@" + JsonLightConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                ""@cn.foo"":""ignored"",
                                ""@cn.foo"":""ignored"",
                                """ + JsonLightConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                             }",
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesChecker_DuplicateAnnotationNotAllowed", "cn.foo")
                },
                #endregion error test cases
            };

            var testDescriptors = testCases.Select(tc =>
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection(null, baseUri + "EntitySetLink", "EntitySetName"))
                        .JsonRepresentation(tc.Json),
                    PayloadEdmModel = new EdmModel(),
                    SkipTestConfiguration = testConfiguration => testConfiguration.IsRequest,
                    DebugDescription = tc.DebugDescription,
                    ExpectedException = tc.ExpectedException
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    // clone the ReaderTestConfiguration and set the base URI.
                    testConfiguration = new ReaderTestConfiguration(testConfiguration);
                    testConfiguration.MessageReaderSettings.BaseUri = null;

                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the reading of unrecognized odata annotations in JSON Lite service document payloads.")]
        public void ServiceDocumentReaderJsonLightUnrecognizedODataAnnotationsTest()
        {
            var testCases = new[]
            {
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "Unrecognized odata property annotation in a resource collection should be ignored.",
                    Json =  @"{
                                ""@" + JsonLightConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                """ + JsonLightConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""name@odata.foo"":""fail"",
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                            }",
                    ExpectedException = null
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "Unrecognized odata instance annotation in a resource collection should be ignored.",
                    Json =  @"{
                                ""@" + JsonLightConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                """ + JsonLightConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""@odata.foo"":""fail"",
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                            }",
                    ExpectedException = null
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "Unrecognized odata property annotation on 'value' should be ignored. ",
                    Json =  @"{
                                ""@" + JsonLightConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                ""value@odata.foo"": ""fail"",
                                """ + JsonLightConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                            }",
                    ExpectedException = null
                },
                new JsonLightServiceDocumentReaderTestCase
                {
                    DebugDescription = "Unrecognized odata instance annotation on the top level should be ignored.",
                    Json =  @"{
                                ""@" + JsonLightConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                ""@odata.foo"": ""fail"",
                                """ + JsonLightConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                            }",
                    ExpectedException = null
                },
            };

            var testDescriptors = testCases.Select(tc =>
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection(null, baseUri + "EntitySetLink", "EntitySetName"))
                        .JsonRepresentation(tc.Json),
                    PayloadEdmModel = new EdmModel(),
                    SkipTestConfiguration = testConfiguration => testConfiguration.IsRequest,
                    ExpectedException = tc.ExpectedException,
                    DebugDescription = tc.DebugDescription
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    // clone the ReaderTestConfiguration and set the base URI.
                    testConfiguration = new ReaderTestConfiguration(testConfiguration);
                    testConfiguration.MessageReaderSettings.BaseUri = null;

                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        private class JsonLightServiceDocumentReaderTestCase
        {
            public string DebugDescription { get; set; }
            public string Json { get; set; }
            public ExpectedException ExpectedException { get; set; }
        }
    }
}
