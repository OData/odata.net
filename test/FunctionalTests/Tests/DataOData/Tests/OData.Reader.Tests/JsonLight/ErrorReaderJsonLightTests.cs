//---------------------------------------------------------------------
// <copyright file="ErrorReaderJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for correct handling of top-level and in-stream error payloads in Json Light.
    /// </summary>
    [TestClass, TestCase]
    public class ErrorReaderJsonLightTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [InjectDependency]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Validates correct behavior of the ODataJsonLightReader for top-level errors.")]
        public void TopLevelErrorTest()
        {
            // we don't allow extra properties at the top-level, so the only thing to test is extra properties on inner errors
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // extra properties in inner error
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError())
                        .JsonRepresentation("{ \"error\": { \"innererror\": { \"foo\": \"bar\" } } }"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().Message("msg1"))
                        .JsonRepresentation("{ \"error\": { \"innererror\": { \"message\": \"msg1\", \"foo\": \"bar\" } } }"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().Message("msg1"))
                        .JsonRepresentation("{ \"error\": { \"innererror\": { \"foo\": \"bar\", \"message\": \"msg1\" } } }"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().Message("msg1"))
                        .JsonRepresentation("{ \"error\": { \"innererror\": { \"foo1\": \"bar1\", \"message\": \"msg1\", \"foo2\": \"bar2\" } } }"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    // NOTE: in JSON, we don't fail on unrecognized duplicate properties, but we do in JSON light.
                    DebugDescription = "Unrecognized properties should fail if duplicated.",
                    PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().Message("msg1"))
                        .JsonRepresentation("{ \"error\": { \"innererror\": { \"foo\": \"bar1\", \"message\": \"msg1\", \"foo\": \"bar2\" } } }"),
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "foo")
                },

                // extra properties in nested inner error
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().InnerError(PayloadBuilder.InnerError()))
                        .JsonRepresentation("{ \"error\": { \"innererror\": { \"internalexception\": { \"foo\": \"bar\" } } } }"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().InnerError(PayloadBuilder.InnerError().Message("msg1")))
                        .JsonRepresentation("{ \"error\": { \"innererror\": { \"internalexception\": { \"message\": \"msg1\", \"foo\": \"bar\" } } } }"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().InnerError(PayloadBuilder.InnerError().Message("msg1")))
                        .JsonRepresentation("{ \"error\": { \"innererror\": { \"internalexception\": { \"foo\": \"bar\", \"message\": \"msg1\" } } } }"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().InnerError(PayloadBuilder.InnerError().Message("msg1")))
                        .JsonRepresentation("{ \"error\": { \"innererror\": { \"internalexception\": { \"foo1\": \"bar1\", \"message\": \"msg1\", \"foo2\": \"bar2\" } } } }"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Unrecognized properties in nested inner error should fail if duplicated.",
                    PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().InnerError(PayloadBuilder.InnerError().Message("msg1")))
                        .JsonRepresentation("{ \"error\": { \"innererror\": { \"internalexception\": { \"foo\": \"bar1\", \"message\": \"msg1\", \"foo\": \"bar2\" } } } }"),
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "foo")
                },
            };
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                TestReaderUtils.ODataBehaviorKinds,
                (testDescriptor, testConfiguration, behavior) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behavior));
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Validates correct behavior of the ODataJsonLightReader for top-level errors with annotations.")]
        public void TopLevelErrorAnnotationsTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                #region "error" object scope
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Should not fail on duplicate custom instance annotations inside the \"error\" object.",
                    PayloadElement = PayloadBuilder.Error()
                        .JsonRepresentation(@"
                                            { 
                                                ""error"":
                                                {
                                                    ""@cn.foo"": ""ignored"",
                                                    ""@cn.foo"": ""something else""
                                                }
                                            }"),
                    ExpectedException = null
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Custom property annotation inside the \"error\" object should be ignored.",
                    PayloadElement = PayloadBuilder.Error().Code("123")
                        .JsonRepresentation(@"
                                            { 
                                                ""error"":
                                                {
                                                    ""code@cn.foo"": ""ignored"",
                                                    ""code"": ""123""
                                                }
                                            }"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Should not fail on duplicates of custom property annotation inside the \"error\" object should be ignored.",
                    PayloadElement = PayloadBuilder.Error().Code("123")
                        .JsonRepresentation(@"
                                            { 
                                                ""error"":
                                                {
                                                    ""code@cn.foo"": ""ignored"",
                                                    ""code@cn.foo"": ""something else"",
                                                    ""code"": ""123""
                                                }
                                            }"),
                    ExpectedException = null,
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Property annotation without property inside the \"error\" object should fail.",
                    PayloadElement = PayloadBuilder.Error()
                        .JsonRepresentation(@"
                                            { 
                                                ""error"":
                                                {
                                                    ""code@cn.foo"": ""fail""
                                                }
                                            }"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightErrorDeserializer_PropertyAnnotationWithoutPropertyForError", "code")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "OData instance annotations inside the \"error\" object should be ignored.",
                    PayloadElement = PayloadBuilder.Error()
                        .JsonRepresentation(@"
                                            { 
                                                ""error"":
                                                {
                                                    ""@odata.foo"": ""fail""
                                                }
                                            }"),
                    ExpectedException = (ExpectedException)null
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "OData property annotations inside the \"error\" object should be ignored.",
                    PayloadElement = PayloadBuilder.Error().Code("123")
                        .JsonRepresentation(@"
                                            { 
                                                ""error"":
                                                {
                                                    ""code@odata.foo"": ""fail"",
                                                    ""code"": ""123""
                                                }
                                            }"),
                    ExpectedException = (ExpectedException)null
                },
                
                #endregion "error" object scope

                #region "innererror" object scope
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Custom instance annotation inside the \"innererror\" object should be ignored.",
                    PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError())
                        .JsonRepresentation(@"
                                            { 
                                                ""error"":
                                                {
                                                    ""innererror"": 
                                                    {
                                                        ""@cn.foo"": ""ignored""
                                                    }
                                                }
                                            }"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Should not fail on duplicate custom instance annotations inside the \"innererror\" object.",
                    PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError())
                        .JsonRepresentation(@"
                                            { 
                                                ""error"":
                                                {
                                                    ""innererror"": 
                                                    {
                                                        ""@cn.foo"": ""ignored"",
                                                        ""@cn.foo"": ""something else""
                                                    }
                                                }
                                            }"),
                    ExpectedException = null
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Custom property annotations inside the \"innererror\" object should be ignored.",
                    PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().Message("msg"))
                        .JsonRepresentation(@"
                                            { 
                                                ""error"":
                                                {
                                                    ""innererror"": 
                                                    {
                                                        ""message@cn.foo"": ""ignored"",
                                                        ""message"": ""msg""
                                                    }
                                                }
                                            }"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Should not fail on duplicate custom property annotations inside the \"innererror\" object.",
                    PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().Message("msg"))
                        .JsonRepresentation(@"
                                            { 
                                                ""error"":
                                                {
                                                    ""innererror"": 
                                                    {
                                                        ""message@cn.foo"": ""ignored"",
                                                        ""message@cn.foo"": ""something else"",
                                                        ""message"": ""msg""
                                                    }
                                                }
                                            }"),
                    ExpectedException = null
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Should not fail on custom instance annotations with the same name at different nesting levels inside the \"innererror\" object.",
                    PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().InnerError(PayloadBuilder.InnerError()))
                        .JsonRepresentation(@"
                                            { 
                                                ""error"":
                                                {
                                                    ""@cn.foo"": ""ignored"",
                                                    ""innererror"": 
                                                    {
                                                        ""@cn.foo"": ""ignored"",
                                                        ""internalexception"":
                                                        {
                                                            ""@cn.foo"": ""ignored""
                                                        }
                                                    }
                                                }
                                            }"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Should not fail on custom property annotations with the same name at different nesting levels inside the \"innererror\" object.",
                    PayloadElement = PayloadBuilder.Error()
                                        .Message(string.Empty)
                                        .InnerError(PayloadBuilder.InnerError()
                                            .Message("msg")
                                            .InnerError(PayloadBuilder.InnerError()
                                                .Message("msg")))
                        .JsonRepresentation(@"
                                            { 
                                                ""error"":
                                                {
                                                    ""message@cn.foo"": ""ignored"",
                                                    ""message"": """",
                                                    ""innererror"": 
                                                    {
                                                        ""message@cn.foo"": ""ignored"",
                                                        ""message"": ""msg"",
                                                        ""internalexception"":
                                                        {
                                                            ""message@cn.foo"": ""ignored"",
                                                            ""message"": ""msg""
                                                        }
                                                    }
                                                }
                                            }"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Property annotation without property inside the \"innererror\" object should fail.",
                    PayloadElement = PayloadBuilder.Error()
                        .JsonRepresentation(@"
                                            { 
                                                ""error"":
                                                {
                                                    ""innererror"": 
                                                    {
                                                        ""message@cn.foo"": ""fail""
                                                    }
                                                }
                                            }"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightErrorDeserializer_PropertyAnnotationWithoutPropertyForError", "message")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "OData instance annotations inside the \"innererror\" object should be ignored.",
                    PayloadElement = PayloadBuilder.Error().InnerError(new ODataInternalExceptionPayload())
                        .JsonRepresentation(@"
                                            { 
                                                ""error"":
                                                {
                                                    ""innererror"": 
                                                    {
                                                        ""@odata.foo"": ""fail""
                                                    }
                                                }
                                            }"),
                    ExpectedException = (ExpectedException)null
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Unknown odata property annotations inside the \"innererror\" object should be ignored.",
                    PayloadElement = PayloadBuilder.Error().InnerError(new ODataInternalExceptionPayload().Message("msg"))
                        .JsonRepresentation(@"
                                            { 
                                                ""error"":
                                                {
                                                    ""innererror"": 
                                                    {
                                                        ""message@odata.foo"": ""bar"",
                                                        ""message"": ""msg""
                                                    }
                                                }
                                            }"),
                    ExpectedException = (ExpectedException)null
                },
                #endregion "innererror" object scope

                #region "message" object scope
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Read empty error message.",
                    PayloadElement = PayloadBuilder.Error().Message(string.Empty)
                        .JsonRepresentation(@"
                                            { 
                                                ""error"":
                                                {
                                                    ""message"": """"          
                                                }
                                            }"),
                },
                #endregion "message" object scope
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                TestReaderUtils.ODataBehaviorKinds,
                (testDescriptor, testConfiguration, behavior) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behavior));
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Validates correct behavior of the ODataJsonLightReader for invalid top-level errors without duplicate properties.")]
        public void TopLevelInvalidErrorTestWithoutDuplicateDataProperties()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = JsonReaderPayloads.CreateInvalidErrorDescriptors(this.Settings, /*isJsonLight*/ true);
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                TestReaderUtils.ODataBehaviorKinds,
                (testDescriptor, testConfiguration, behavior) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behavior));
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Validates correct behavior of the ODataJsonLightReader for invalid top-level errors with duplicate properties.")]
        public void TopLevelInvalidErrorTestWithDuplicateDataProperties()
        {
            PayloadReaderTestDescriptor.Settings settings = this.Settings;
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                #region Duplicate properties
                // duplicate 'error' property
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"error\": { \"message\": \"Error message\" }, \"error\": { \"message\": \"Error message\" } }"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonReaderUtils_MultipleErrorPropertiesWithSameName", "error"),
                },

                // duplicate 'code' property
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"error\": { \"code\": \"Error code\", \"code\": \"Error code\" } }"),
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "code"),
                },

                // duplicate 'message' property
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"error\": { \"message\": \"Error message\", \"message\": \"Error message\" } }"),
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "message"),
                },

                // duplicate 'innererror' property
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"error\": { \"code\": \"Error code\", \"innererror\": { }, \"innererror\": { } } }"),
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "innererror"),
                },

                // duplicate 'message' property (on the inner error)
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"error\": { \"code\": \"Error code\", \"innererror\": { \"message\": \"Inner msg\", \"message\": \"Inner msg\" } } }"),
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "message"),
                },

                // duplicate 'type' property (on the inner error)
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"error\": { \"code\": \"Error code\", \"innererror\": { \"type\": \"Some typename\", \"type\": \"Some typename\" } } }"),
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "type"),
                },

                // duplicate 'stacktrace' property (on the inner error)
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"error\": { \"code\": \"Error code\", \"innererror\": { \"stacktrace\": \"stack trace\", \"stacktrace\": \"stack trace\" } } }"),
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "stacktrace"),
                },

                // duplicate 'internalexception' property (on the inner error)
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"error\": { \"code\": \"Error code\", \"innererror\": { \"internalexception\": { }, \"internalexception\": { } } } }"),
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "internalexception"),
                },
                #endregion Duplicate properties
            };
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                // Disable WCF DS Server behavior since in that case duplicate properties are eliminated and the code will not see them.
                TestReaderUtils.ODataBehaviorKinds.Where(behavior => behavior != TestODataBehaviorKind.WcfDataServicesServer),
                (testDescriptor, testConfiguration, behavior) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behavior));
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Validates correct behavior of the ODataJsonLightReader for in-stream errors.")]
        public void InStreamJsonLightErrorTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = PayloadReaderTestDescriptorGenerator.CreateErrorReaderTestDescriptors(this.Settings);

            // convert the payload element to a JSON representation usable for in-stream error testing
            testDescriptors = testDescriptors.Select(td =>
            {
                AnnotatedPayloadElementToJsonLightConverter payloadElementToJsonConverter = new AnnotatedPayloadElementToJsonLightConverter();
                JsonObject jsonObject = (JsonObject)payloadElementToJsonConverter.ConvertToJsonLightValue(td.PayloadElement);
                Debug.Assert(td.ExpectedException == null, "Don't expect errors for regular payloads (without annotation).");

                return new PayloadReaderTestDescriptor(td)
                {
                    PayloadElement = td.PayloadElement.JsonRepresentation(jsonObject)
                };
            });

            testDescriptors = testDescriptors.Select(td => td.ToInStreamErrorTestDescriptor(ODataFormat.Json));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        // TODO: migrate InStreamInvalidErrorTest and InStreamJsonDeeplyRecursiveErrorTest from verbose JSON tests
    }
}
