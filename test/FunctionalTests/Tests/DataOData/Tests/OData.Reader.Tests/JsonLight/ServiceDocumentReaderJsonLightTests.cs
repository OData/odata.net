//---------------------------------------------------------------------
// <copyright file="ServiceDocumentReaderJsonTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Json
{
    #region Namespaces
    using System;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of various JSON Light service document payloads.
    /// </summary>
    [TestClass, TestCase]
    public class ServiceDocumentReaderJsonTests : ODataReaderTestCase
    {
        private const string baseUri = "http://odata.org/test/";

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the the reading of JSON Light service document payloads.")]
        public void ServiceDocumentReaderJsonTest()
        {
            var errorTestCases = new[]
            {
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "unrecognized property in the workspace object.",
                    Json = "{ \"@" + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"unrecognized\": 42, \"value\": [ {\"name\": \"EntitySetName\", \"url\":\"EntitySetLink\"} ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonServiceDocumentDeserializer_UnexpectedPropertyInServiceDocument", "unrecognized", "value")
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "unrecognized property in a service document element.",
                    Json = "{ \"@" + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ {\"name\":\"EntitySetName\", \"url\":\"EntitySetLink\", \"unrecognized\": 42} ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonServiceDocumentDeserializer_UnexpectedPropertyInServiceDocumentElement", "unrecognized", "name", "url")
                },

                #region missing required properties
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "service document element without a 'name' (required in JSON light, but doesn't exist in the other formats)",
                    Json = "{ \"@" + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ {\"url\":\"EntitySetLink\"} ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement", "name")
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "service document element without a 'url'",
                    Json = "{ \"@" + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ {\"name\":\"EntitySetName\"} ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement", "url")
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "null value for 'name'",
                    Json = "{ \"@" + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ {\"name\":null, \"url\":\"EntitySetLink\"} ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement", "name")
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "null value for 'url'",
                    Json = "{ \"@" + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ {\"name\":\"EntitySetName\", \"url\":null} ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_ServiceDocumentElementUrlMustNotBeNull")
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "no 'value' property",
                    Json = "{ \"@" + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\" }",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonServiceDocumentDeserializer_MissingValuePropertyInServiceDocument", "value")
                },
                #endregion missing required properties

                #region duplicate properties
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "multiple 'value' properties",
                    Json = "{ \"@" + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ {\"name\": \"EntitySetName\", \"url\":\"EntitySetLink\"} ], \"value\": [ {\"name\": \"EntitySetName\", \"url\":\"EntitySetLink\"} ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonServiceDocumentDeserializer_DuplicatePropertiesInServiceDocument", "value")
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "multiple 'name' properties in an entity set.",
                    Json = "{ \"@" + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ {\"name\": \"EntitySetName\", \"url\":\"EntitySetLink\", \"name\": \"Second name\"}]}",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement", "name")
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "multiple 'url' properties in an entity set.",
                    Json = "{ \"@" + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ {\"name\": \"EntitySetName\", \"url\":\"EntitySetLink\", \"url\": \"SecondLink\"}]}",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement", "url")
                },

                #endregion duplicate properties/annotations

                #region incorrect json structure
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "invalid top-level JSON node (an array)",
                    Json = "[]",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "StartArray")
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "invalid top-level JSON node (a primitive value)",
                    Json = "42",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue")
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "invalid value of 'value' property (a primitive value)",
                    Json = "{ \"@" + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": 42 }",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_UnexpectedNodeDetected", "StartArray", "PrimitiveValue")
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "invalid value of 'value' property (an object value)",
                    Json = "{ \"@" + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": { } }",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_UnexpectedNodeDetected", "StartArray", "StartObject")
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "invalid value inside the array value of the 'value' property (a nested array value)",
                    Json = "{ \"@" + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ [] ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "StartArray")
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "invalid value inside the array value of the 'value' property (integer value)",
                    Json = "{ \"@" + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ 42 ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue")
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "invalid value inside the array value of the 'value' property (boolean value)",
                    Json = "{ \"@" + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ true ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue")
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "invalid value inside the array value of the 'value' property (string value)",
                    Json = "{ \"@" + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ \"string\" ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue")
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "invalid value inside the array value of the 'value' property (null)",
                    Json = "{ \"@" + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\", \"value\": [ null ] }",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue")
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
                this.ReaderTestConfigurationProvider.JsonFormatConfigurations,
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
        public void ServiceDocumentReaderJsonCustomAnnotationsTest()
        {
            var testCases = new[]
            {
                #region success test cases
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "Custom property annotation on \"name\" should be ignored.",
                    Json = @"{
                                ""@" + JsonConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                """ + JsonConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""name@cn.foo"":""ignored"",
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                            }",
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "Custom instance annotation inside a resource collection should be ignored.",
                    Json = @"{
                                ""@" + JsonConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                """ + JsonConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""@cn.foo"":""ignored"",
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                            }",
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "Custom property annotation on \"value\" should be ignored.",
                    Json = @"{
                                ""@" + JsonConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                ""value@cn.foo"": ""ignored"",
                                """ + JsonConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                            }",
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "Custom instance annotation on the top level should be ignored.",
                    Json = @"{
                                ""@" + JsonConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                ""@cn.foo"": ""ignored"",
                                """ + JsonConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                            }",
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "Annotations at different scopes with the same name should be ignored and should not throw.",
                    Json = @"{
                                ""@" + JsonConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                ""@cn.foo"": ""ignored"",
                                ""value@cn.foo"": ""ignored"",
                                """ + JsonConstants.ODataValuePropertyName + @""":
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
                new JsonServiceDocumentReaderTestCase
                {
                    Json = @"{
                                ""@" + JsonConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                """ + JsonConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""name@cn.foo"":""out of order"",
                                        ""url"":""EntitySetLink"",
                                        ""name"":""EntitySetName""
                                    },
                                ]
                            }",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonServiceDocumentDeserializer_PropertyAnnotationWithoutProperty", "name")
                },
                new JsonServiceDocumentReaderTestCase
                {
                    Json = @"{
                                ""@" + JsonConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                """ + JsonConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""url"":""EntitySetLink"",
                                        ""name"":""EntitySetName""
                                    },
                                ],
                                ""value@cn.foo"":""out of order""
                            }",
                    ExpectedException = ODataExpectedExceptions.ODataException("PropertyAnnotationAfterTheProperty", "cn.foo", "value")
                },
                new JsonServiceDocumentReaderTestCase
                {
                    Json = @"{
                                ""@" + JsonConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                """ + JsonConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""name@cn.foo"":""ignored"",
                                        ""name@cn.foo"":""ignored"",
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                             }",
                    ExpectedException = null
                },
                new JsonServiceDocumentReaderTestCase
                {
                    Json = @"{
                                ""@" + JsonConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                """ + JsonConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""@cn.foo"":""ignored"",
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink"",
                                        ""@cn.foo"":""ignored""
                                    },
                                ]
                             }",
                    ExpectedException = null
                },
                new JsonServiceDocumentReaderTestCase
                {
                    Json = @"{
                                ""@" + JsonConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                ""value@cn.foo"":""ignored"",
                                ""value@cn.foo"":""ignored"",
                                """ + JsonConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                             }",
                    ExpectedException = null
                },
                new JsonServiceDocumentReaderTestCase
                {
                    Json = @"{
                                ""@" + JsonConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                ""@cn.foo"":""ignored"",
                                ""@cn.foo"":""ignored"",
                                """ + JsonConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                             }",
                    ExpectedException = null
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
                this.ReaderTestConfigurationProvider.JsonFormatConfigurations,
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
        public void ServiceDocumentReaderJsonUnrecognizedODataAnnotationsTest()
        {
            var testCases = new[]
            {
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "Unrecognized odata property annotation in a resource collection should be ignored.",
                    Json =  @"{
                                ""@" + JsonConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                """ + JsonConstants.ODataValuePropertyName + @""":
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
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "Unrecognized odata instance annotation in a resource collection should be ignored.",
                    Json =  @"{
                                ""@" + JsonConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                """ + JsonConstants.ODataValuePropertyName + @""":
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
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "Unrecognized odata property annotation on 'value' should be ignored. ",
                    Json =  @"{
                                ""@" + JsonConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                ""value@odata.foo"": ""fail"",
                                """ + JsonConstants.ODataValuePropertyName + @""":
                                [
                                    {
                                        ""name"":""EntitySetName"",
                                        ""url"":""EntitySetLink""
                                    },
                                ]
                            }",
                    ExpectedException = null
                },
                new JsonServiceDocumentReaderTestCase
                {
                    DebugDescription = "Unrecognized odata instance annotation on the top level should be ignored.",
                    Json =  @"{
                                ""@" + JsonConstants.ODataContextAnnotationName + @""": ""http://odata.org/test/$metadata"",
                                ""@odata.foo"": ""fail"",
                                """ + JsonConstants.ODataValuePropertyName + @""":
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
                this.ReaderTestConfigurationProvider.JsonFormatConfigurations,
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

        private class JsonServiceDocumentReaderTestCase
        {
            public string DebugDescription { get; set; }
            public string Json { get; set; }
            public ExpectedException ExpectedException { get; set; }
        }
    }
}
