//---------------------------------------------------------------------
// <copyright file="WriterErrorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for writing errors with the ODataWriter or ODataMessageWriter.
    /// </summary>
    [TestClass, TestCase]
    public class WriterErrorTests : ODataWriterTestCase
    {
        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        /// <summary>
        /// The value to use as the writer setting's MaxNestingDepth.
        /// </summary>
        private readonly int customSetRecursionDepthLimit = 3;

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Validates the payloads for various top-level errors written using ODataMessageWriter.WriteError().")]
        public void TopLevelODataMessageWriterErrorTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                this.CreateODataErrorTestDescriptors(/*throwOnRequest*/ true, /*topLevel*/ true),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.MessageQuotas.MaxNestingDepth = this.customSetRecursionDepthLimit;

                    ODataAnnotatedError error = (ODataAnnotatedError)testDescriptor.PayloadItems.Single();

                    TestWriterUtils.WriteAndVerifyTopLevelContent(
                        testDescriptor,
                        testConfiguration,
                        (messageWriter) => messageWriter.WriteError(error.Error, error.IncludeDebugInformation),
                        this.Assert,
                        baselineLogger: this.Logger);
                });
        }

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Validates the payloads for various top-level errors written using an ODataWriter.")]
        public void TopLevelODataWriterErrorTest()
        {
            EdmEntitySet entitySet = null;

            this.CombinatorialEngineProvider.RunCombinations(
                this.CreateODataErrorTestDescriptors(/*throwOnRequest*/ true, /*topLevel*/ false),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.Model = CreateErrorTestModel(out entitySet);
                    testDescriptor.PayloadEdmElementContainer = entitySet;

                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.MessageQuotas.MaxNestingDepth = this.customSetRecursionDepthLimit;
                    TestWriterUtils.WriteAndVerifyODataEdmPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        //// TODO: Add in-stream error tests

        /// <summary>
        /// Creates an empty model so that the JSON lite serializer won't complain.
        /// </summary>
        /// <param name="entitySet">An entity set in the generated model.</param>
        private EdmModel CreateErrorTestModel(out EdmEntitySet entitySet)
        {
            EdmModel model = new EdmModel();

            var customer = new EdmEntityType("TestModel", "Customer");
            model.AddElement(customer);

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            model.AddElement(container);

            entitySet = container.AddEntitySet("Customers", customer);

            return model;
        }

        private IEnumerable<PayloadWriterTestDescriptor<ODataAnnotatedError>> CreateODataErrorTestDescriptors(bool throwOnRequests, bool topLevel)
        {
            var testCases = new[] {
                new {
                    Error = new ODataAnnotatedError { Error = new ODataError() },
                    Code = string.Empty, Message = string.Empty, InnerError = (InnerError)null, ExpectedException = (ExpectedException) null
                },
                new {
                    Error = new ODataAnnotatedError { Error = new ODataError() { ErrorCode = "code1" } },
                    Code = "code1", Message = string.Empty, InnerError = (InnerError)null, ExpectedException = (ExpectedException) null
                },
                new {
                    Error = new ODataAnnotatedError { Error = new ODataError() { Message = "message text" } },
                    Code = string.Empty, Message = "message text", InnerError = (InnerError)null, ExpectedException = (ExpectedException) null
                },
                new {
                    Error = new ODataAnnotatedError { Error = new ODataError() { InnerError = new ODataInnerError { Message = "some inner error" } } },
                    Code = string.Empty, Message = string.Empty, InnerError = (InnerError)null, ExpectedException = (ExpectedException) null
                },
                new {
                    Error = new ODataAnnotatedError { Error = new ODataError() { InnerError = new ODataInnerError { Message = "some inner error" } }, IncludeDebugInformation = true },
                    Code = string.Empty, Message = string.Empty, InnerError = new InnerError { Message = "some inner error" }, ExpectedException = (ExpectedException) null
                },
                new {
                    Error = new ODataAnnotatedError { Error = new ODataError() { ErrorCode = "code42", Message = "message text", InnerError = new ODataInnerError { Message = "some inner error" } } },
                    Code = "code42", Message = "message text", InnerError = (InnerError)null, ExpectedException = (ExpectedException) null
                },
                new {
                    Error = new ODataAnnotatedError { Error = new ODataError() { ErrorCode = "code42", Message = "message text", InnerError = new ODataInnerError { Message = "some inner error" } }, IncludeDebugInformation = true },
                    Code = "code42", Message = "message text", InnerError = new InnerError { Message = "some inner error" }, ExpectedException = (ExpectedException) null
                },
                new {
                    Error = new ODataAnnotatedError
                    {
                        Error = new ODataError()
                        {
                            ErrorCode = "code42",
                            Message = "message text",
                            InnerError = new ODataInnerError
                            {
                                Message = "some inner error",
                                TypeName = "some type name",
                                StackTrace = "some stack trace"
                            }
                        },
                        IncludeDebugInformation = true,
                    },
                    Code = "code42",
                    Message = "message text",
                    InnerError = new InnerError
                    {
                        Message = "some inner error",
                        TypeName = "some type name",
                        StackTrace = "some stack trace"
                    },
                    ExpectedException = (ExpectedException) null
                },
                new {
                    Error = new ODataAnnotatedError
                    {
                        Error = new ODataError()
                        {
                            ErrorCode = "code42",
                            Message = "message text",
                            InnerError = new ODataInnerError
                            {
                                Message = "some inner error",
                                TypeName = "some type name",
                                StackTrace = "some stack trace",
                                InnerError = new ODataInnerError
                                {
                                    Message = "nested inner error",
                                    TypeName = "nested type name",
                                    StackTrace = "nested stack trace",
                                    InnerError = new ODataInnerError
                                    {
                                        Message = "nested nested inner error",
                                        TypeName = "nested nested type name",
                                        StackTrace = "nested nested stack trace"
                                    }
                                }
                            }
                        },
                        IncludeDebugInformation = true,
                    },
                    Code = "code42",
                    Message = "message text",
                    InnerError = new InnerError
                    {
                        Message = "some inner error",
                        TypeName = "some type name",
                        StackTrace = "some stack trace",
                        NestedError = new InnerError
                        {
                            Message = "nested inner error",
                            TypeName = "nested type name",
                            StackTrace = "nested stack trace",
                            NestedError  = new InnerError
                            {
                                Message = "nested nested inner error",
                                TypeName = "nested nested type name",
                                StackTrace = "nested nested stack trace"
                            }
                        }
                    },
                    ExpectedException = (ExpectedException) null
                },
                new {
                    Error = new ODataAnnotatedError { Error = new ODataError() { ErrorCode = "code42", Message = "message text", InnerError = new ODataInnerError(new Exception("some inner error")) }, IncludeDebugInformation = true },
                    Code = "code42", Message = "message text", InnerError = new InnerError { Message = "some inner error", TypeName = "System.Exception" }, ExpectedException = (ExpectedException) null
                },
                new {
                    Error = new ODataAnnotatedError
                    {
                        Error = new ODataError()
                        {
                            ErrorCode = "code42",
                            Message = "message text",
                            InnerError = new ODataInnerError(
                                new Exception("some inner error", new Exception("nested inner error", new Exception("nested nested inner error"))))
                        },
                        IncludeDebugInformation = true,
                    },
                    Code = "code42",
                    Message = "message text",
                    InnerError = new InnerError
                    {
                        Message = "some inner error",
                        TypeName = "System.Exception",
                        NestedError = new InnerError
                        {
                            Message = "nested inner error",
                            TypeName = "System.Exception",
                            NestedError  = new InnerError
                            {
                                Message = "nested nested inner error",
                                TypeName = "System.Exception",
                            }
                        }
                    },
                    ExpectedException = (ExpectedException) null,
                },
                // Max recursion depth is set to 3, so 4 nested inner errors should be invalid.
                new {
                    Error = new ODataAnnotatedError
                    {
                        Error = new ODataError()
                        {
                            ErrorCode = "code42",
                            Message = "message text",
                            InnerError = new ODataInnerError
                            {
                                Message = "some inner error",
                                TypeName = "some type name",
                                StackTrace = "some stack trace",
                                InnerError = new ODataInnerError
                                {
                                    Message = "nested inner error",
                                    TypeName = "nested type name",
                                    StackTrace = "nested stack trace",
                                    InnerError = new ODataInnerError
                                    {
                                        Message = "nested nested inner error",
                                        TypeName = "nested nested type name",
                                        StackTrace = "nested nested stack trace",
                                        InnerError = new ODataInnerError
                                        {
                                            Message = "nested nested nested inner error",
                                            TypeName = "nested nested nested type name",
                                            StackTrace = "nested nested nested stack trace"
                                        }
                                    }
                                }
                            }
                        },
                        IncludeDebugInformation = true,
                    },
                    Code = (string) null,
                    Message = (string) null,
                    InnerError = (InnerError) null,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_RecursionDepthLimitReached", Convert.ToString(this.customSetRecursionDepthLimit))
                }
            };

            ExpectedException errorNotAllowedException = ODataExpectedExceptions.ODataException("ODataMessageWriter_ErrorPayloadInRequest");

            return testCases.Select(tc =>
           {
               return new PayloadWriterTestDescriptor<ODataAnnotatedError>(
                   this.Settings,
                   tc.Error,
                   (testConfiguration) =>
                   {
                       if (testConfiguration.Format == ODataFormat.Json)
                       {
                           if (testConfiguration.IsRequest && throwOnRequests)
                           {
                               return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                               {
                                   ExpectedException2 = errorNotAllowedException
                               };
                           }
                           else if (tc.ExpectedException != null)
                           {
                               return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                               {
                                   ExpectedException2 = tc.ExpectedException
                               };
                           }
                           else
                           {
                               return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                               {
                                   Json = ExpectedJsonLightErrorPayload(tc.Code, tc.Message, tc.InnerError),
                                   FragmentExtractor = (result) => { JsonUtils.TrimSurroundingWhitespaces(result); return result; },
                               };
                           }
                       }
                       else
                       {
                           throw new ODataTestException("Expected results are only implemented for ATOM and JSON lite.");
                       }
                   });
           });
        }

        /// <summary>
        /// Returns the expected ATOM payload for an error.
        /// </summary>
        /// <param name="code">The code of the error.</param>
        /// <param name="message">The error message.</param>
        /// <param name="innerError">The inner error (if null, don't write anything).</param>
        /// <returns>The expected ATOM payload for an error.</returns>
        private static string ExpectedAtomErrorPayload(string code, string message, InnerError innerError)
        {
            return string.Format(
                string.Join(
                    "$(NL)",
                    "<{0}:{2} xmlns:{0}=\"{1}\">",
                    "  {3}",
                    "  <{0}:{4}>{5}</{0}:{4}>{6}",
                    "</{0}:{2}>"
                ),
                TestAtomConstants.ODataMetadataNamespacePrefix,
                TestAtomConstants.ODataMetadataNamespace,
                TestAtomConstants.ODataErrorElementName,
                string.Format(code == string.Empty ?
                    "<{0}:{1} />" :
                    "<{0}:{1}>{2}</{0}:{1}>",
                    TestAtomConstants.ODataMetadataNamespacePrefix,
                    TestAtomConstants.ODataErrorCodeElementName,
                    code),
                TestAtomConstants.ODataErrorMessageElementName,
                message,
                ExpectedAtomInnerErrorPayload(innerError, 0));
        }

        /// <summary>
        /// Returns the expected ATOM payload for an inner error payload.
        /// </summary>
        /// <param name="innerError">The inner error to create the ATOM payload for.</param>
        /// <param name="depth">The depth of the inner error (starting with 0).</param>
        /// <returns>The expected ATOM payload for the <paramref name="innerError"/> payload.</returns>
        private static string ExpectedAtomInnerErrorPayload(InnerError innerError, int depth)
        {
            if (innerError == null)
            {
                return string.Empty;
            }

            return string.Format(
                string.Join(
                    "$(NL)",
                    "  <{0}:{1}>",
                    "    <{0}:{2}>{5}</{0}:{2}>",
                    "    <{0}:{3}>{6}</{0}:{3}>",
                    "    <{0}:{4}>{7}</{0}:{4}>",
                    innerError.NestedError == null ? string.Empty : "    {8}",
                    "  </{0}:{1}>"
                ),
                TestAtomConstants.ODataMetadataNamespacePrefix,
                depth == 0 ? TestAtomConstants.ODataInnerErrorElementName : TestAtomConstants.ODataInnerErrorInnerErrorElementName,
                TestAtomConstants.ODataInnerErrorMessageElementName,
                TestAtomConstants.ODataInnerErrorTypeElementName,
                TestAtomConstants.ODataInnerErrorStackTraceElementName,
                innerError.Message ?? string.Empty,
                innerError.TypeName ?? string.Empty,
                innerError.StackTrace ?? string.Empty,
                ExpectedAtomInnerErrorPayload(innerError.NestedError, depth + 1));
        }


        /// <summary>
        /// Returns the expected JSON Lite payload for an error.
        /// </summary>
        /// <param name="code">The code of the error.</param>
        /// <param name="message">The error message.</param>
        /// <param name="innerError">The inner error (if null, don't write anything).</param>
        /// <returns>The expected JSON Lite payload for an error.</returns>
        private static string ExpectedJsonLightErrorPayload(string code, string message, InnerError innerError)
        {
            return string.Join(
                "$(NL)",
                "{",
                "$(Indent)\"" + JsonLightConstants.ODataErrorPropertyName + "\":{",
                "$(Indent)$(Indent)\"" + JsonConstants.ODataErrorCodeName + "\":\"" + code + "\",\"",
                JsonConstants.ODataErrorMessageName + "\":\"" + message + "\"",
                ExpectedJsonLightInnerErrorPayload(innerError, 0),
                "$(Indent)}",
                "}");
        }

        /// <summary>
        /// Returns the expected JSON Lite payload for an inner error payload.
        /// </summary>
        /// <param name="innerError">The inner error to create the JSON Lite payload for.</param>
        /// <param name="depth">The depth of the inner error (starting with 0).</param>
        /// <returns>The expected JSON Lite payload for the <paramref name="innerError"/> payload.</returns>
        private static string ExpectedJsonLightInnerErrorPayload(InnerError innerError, int depth)
        {
            if (innerError == null)
            {
                return string.Empty;
            }

            string prefix = string.Empty;
            for (int i = 0; i < depth; ++i)
            {
                prefix += "$(Indent)";
            }

            string propertyName = depth == 0
                ? JsonConstants.ODataErrorInnerErrorName
                : JsonConstants.ODataErrorInnerErrorInnerErrorName;

            string innerErrorString = string.Join(
                "$(NL)",
                ",\"" + propertyName + "\":{",
                "$(Indent)$(Indent)$(Indent)" + prefix + "\"" +
                    JsonConstants.ODataErrorInnerErrorMessageName + "\":\"" + (innerError.Message ?? string.Empty) + "\",\"" +
                    JsonConstants.ODataErrorInnerErrorTypeNameName + "\":\"" + (innerError.TypeName ?? string.Empty) + "\",\"" +
                    JsonConstants.ODataErrorInnerErrorStackTraceName + "\":\"" + (innerError.StackTrace ?? string.Empty) + "\"" +
                    ExpectedJsonLightInnerErrorPayload(innerError.NestedError, depth + 1),
                "$(Indent)$(Indent)" + prefix + "}");
            return innerErrorString;
        }

        /// <summary>
        /// Class representing the same information as an <see cref="ODataInnerError"/>.
        /// </summary>
        private sealed class InnerError
        {
            public string Message { get; set; }
            public string TypeName { get; set; }
            public string StackTrace { get; set; }
            public InnerError NestedError { get; set; }
        }
    }
}
