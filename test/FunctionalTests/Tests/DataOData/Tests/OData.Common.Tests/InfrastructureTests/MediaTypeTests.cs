//---------------------------------------------------------------------
// <copyright file="MediaTypeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.InfrastructureTests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for parsing media types (Http content-type headers).
    /// </summary>
    [TestClass, TestCase(Name="Media Type Tests")]
    public class MediaTypeTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [InjectDependency(IsRequired = true)]
        public IExceptionVerifier ExceptionVerifier { get; set; }

        [TestMethod, Variation(Description = "Verifies that we can successfully parse a variety of media types.")]
        public void MediaTypeParsingTest()
        {
            var testCases = new[]
                {
                    new
                    {
                        ContentType = "text/xml",
                        MediaType = "text/xml",
                        MediaTypeCharset = (string)null,
                        Parameters = (KeyValuePair<string, string>[])null
                    },
                    new
                    {
                        ContentType = "text/plain",
                        MediaType = "text/plain",
                        MediaTypeCharset = (string)null,
                        Parameters = (KeyValuePair<string, string>[])null
                    },
                    new
                    {
                        ContentType = "application/json",
                        MediaType = "application/json",
                        MediaTypeCharset = (string)null,
                        Parameters = (KeyValuePair<string, string>[])null
                    },
                    new
                    {
                        ContentType = "image/png",
                        MediaType = "image/png",
                        MediaTypeCharset = (string)null,
                        Parameters = (KeyValuePair<string, string>[])null
                    },
                    new
                    {
                        ContentType = "image/png;foo=",
                        MediaType = "image/png",
                        MediaTypeCharset = (string)null,
                        Parameters = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("foo", string.Empty) }
                    },
                    new
                    {
                        ContentType = @"abc/pqr;charset=""UTF-8""",
                        MediaType = "abc/pqr",
                        MediaTypeCharset = "UTF-8",
                        Parameters = (KeyValuePair<string, string>[])null
                    },
                    new
                    {
                        ContentType = @"text/plain;charset=""UTF-8"";type=feed",
                        MediaType = "text/plain",
                        MediaTypeCharset = "UTF-8",
                        Parameters = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("type", "feed") }
                    },
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    string mediaTypeName, mediaTypeCharset;
                    IList<KeyValuePair<string, string>> parameters = HttpUtilsWrapper.ParseContentType(testCase.ContentType, out mediaTypeName, out mediaTypeCharset);

                    this.Assert.AreEqual(testCase.MediaType, mediaTypeName, "Did not find expected media type.");
                    this.Assert.AreEqual(testCase.MediaTypeCharset, mediaTypeCharset, "Did not find expected media type charset.");

                    if (testCase.Parameters == null)
                    {
                        this.Assert.IsNull(parameters, "Expected null parameters.");
                    }
                    else
                    {
                        this.Assert.IsNotNull(parameters, "Expected non-null parameters.");
                        this.Assert.AreEqual(testCase.Parameters.Length, parameters.Count, "Expected same number of parameters.");
                        this.Assert.IsTrue(testCase.Parameters.All(p => parameters.Contains(p)), "Did not find all expected parameters in the output.");
                    }
                });
        }

        [TestMethod, Variation(Description = "Verifies that we report expected errors for invalid media types.")]
        public void MediaTypeErrorTest()
        {
            var testCases = new[]
                {
                    new
                    {
                        ContentType = (string)null,
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_ContentTypeMissing"),
                    },
                    new
                    {
                        ContentType = string.Empty,
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_ContentTypeMissing"),
                    },
                    new
                    {
                        ContentType = "text",
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeUnspecified", "text"),
                    },
                    new
                    {
                        ContentType = "text/",
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeRequiresSubType", "text/"),
                    },
                    new
                    {
                        ContentType = "text]",
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeRequiresSlash", "text]"),
                    },
                    new
                    {
                        ContentType = "text/plain;foo",
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeMissingParameterValue", "foo"),
                    },
                    new
                    {
                        ContentType = "text/plain foo=bar",
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeRequiresSemicolonBeforeParameter", "text/plain foo=bar"),
                    },
                    new
                    {
                        ContentType = @"text/plain;foo=""bar",
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_ClosingQuoteNotFound", "Content-Type", @"text/plain;foo=""bar", "19"),
                    },
                    new
                    {
                        ContentType = "text/plain;;",
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeMissingParameterName"),
                    },
                    new
                    {
                        ContentType = @"text/plain;foo=unquotedEscape""Char",
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_EscapeCharWithoutQuotes", "Content-Type", @"text/plain;foo=unquotedEscape""Char", "29", "\""),
                    },
                    new
                    {
                        ContentType = @"text/plain;foo=""quotedWithMissingEndQuote",
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_ClosingQuoteNotFound", "Content-Type", @"text/plain;foo=""quotedWithMissingEndQuote", "41"),
                    },
                    new
                    {
                        ContentType = @"text/plain;foo=""quotedWithEscapeCharAtTheEnd\",
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_EscapeCharAtEnd", "Content-Type", @"text/plain;foo=""quotedWithEscapeCharAtTheEnd\", "45", "\\"),
                    },
                    new
                    {
                        ContentType = @"text/plain,application/xml",
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_NoOrMoreThanOneContentTypeSpecified", "text/plain,application/xml"),
                    },
                    new
                    {
                        ContentType = @"*/*",
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("ODataMessageReader_WildcardInContentType", "*/*"),
                    },
                    new
                    {
                        ContentType = @"*/foo",
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("ODataMessageReader_WildcardInContentType", "*/foo"),
                    },
                    new
                    {
                        ContentType = @"foo/*",
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("ODataMessageReader_WildcardInContentType", "foo/*"),
                    },
                    new
                    {
                        ContentType = @"application/atom+xml; q]1001",
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeMissingParameterValue", "q"),
                    }
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    string mediaTypeName, mediaTypeCharset;
                    this.Assert.ExpectedException(
                        () => HttpUtilsWrapper.ParseContentType(testCase.ContentType, out mediaTypeName, out mediaTypeCharset),
                        testCase.ExpectedException,
                        this.ExceptionVerifier);
                });
        }

        private static class HttpUtilsWrapper
        {
            static readonly Type httpUtilsType;
            static readonly Type mediaType;

            static HttpUtilsWrapper()
            {
                httpUtilsType = typeof(ODataAnnotatable).Assembly.GetType("Microsoft.OData.HttpUtils");
                mediaType = typeof(ODataAnnotatable).Assembly.GetType("Microsoft.OData.MediaType");
            }

            internal static IList<KeyValuePair<string, string>> ParseContentType(string contentType, out string mediaTypeName, out string mediaTypeCharset)
            {
                object[] args = new object[3];
                args[0] = contentType;

                object result = ReflectionUtils.InvokeMethod(httpUtilsType, "ReadMimeType", args);

                mediaTypeName = (string)args[1];
                mediaTypeCharset = (string)args[2];

                return (IList<KeyValuePair<string, string>>)result;
            }
        }
    }
}
