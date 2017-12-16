//---------------------------------------------------------------------
// <copyright file="BatchReaderStreamProcessPartHeaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests the ODataMultipartMixedBatchReaderStream.ProcessPartHeader implementation.
    /// </summary>
    [TestClass, TestCase]
    public class BatchReaderStreamProcessPartHeaderTests : ODataReaderTestCase
    {
        /// <summary>
        /// Gets or sets the dependency injector.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDependencyInjector Injector { get; set; }

#if !SILVERLIGHT && !WINDOWS_PHONE
        // Batch stream buffer tests use private reflection and thus cannot run on SilverLight or the phone.
        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Testing the ODataMultipartMixedBatchReaderStream.ProcessPartHeader method.")]
        public void BatchReaderStreamProcessPartHeaderTest()
        {
            IEnumerable<PartHeaderTestCase> testCases = new PartHeaderTestCase[]
            {
                #region Missing required headers
                new PartHeaderTestCase
                {
                    DebugDescription = "Read a part header with no headers; this will fail since the Content-Type header is required.",
                    PayloadFunc = builder => builder
                        .LineFeed()
                        .ResetMemoryStream(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_MissingContentTypeHeader"),
                },
                new PartHeaderTestCase
                {
                    DebugDescription = "Read a part header with only a Content-Type header; since there is Content-Transfer-Encoding header this test will fail.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "application/http")
                        .ResetMemoryStream(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_MissingOrInvalidContentEncodingHeader"),
                },
                #endregion
                #region Content-Type header and boundary
                new PartHeaderTestCase
                {
                    DebugDescription = "Read a part header with an invalid content type; this will fail.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "application/xml")
                        .Header("Content-Transfer-Encoding", "binary")
                        .ResetMemoryStream(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_InvalidContentTypeSpecified", "Content-Type", "application/xml", "multipart/mixed", "application/http"),
                },
                new PartHeaderTestCase
                {
                    DebugDescription = "Read a part header with only a Content-Type and Content-Transfer-Encoding header; since there is no empty line after the header we try to continue reading headers but run out of data from the stream.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "application/http")
                        .Header("Content-Transfer-Encoding", "binary")
                        .ResetMemoryStream(),
                },
                #endregion
                #region Content-Transfer-Encoding header
                new PartHeaderTestCase
                {
                    DebugDescription = "Read a part header with an invalid Content-Transfer-Encoding.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "application/http")
                        .Header("Content-Transfer-Encoding", "non-binary")
                        .ResetMemoryStream(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_MissingOrInvalidContentEncodingHeader", "Content-Transfer-Encoding", "binary"),
                },
                #endregion
                #region Line feeds after headers
                new PartHeaderTestCase
                {
                    DebugDescription = "Read an operation part header with a line feed after the header and some content.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "application/http")
                        .Header("Content-Transfer-Encoding", "binary")
                        .LineFeed()
                        .String("Some payload")
                        .ResetMemoryStream(),
                    ExpectChangeSetPart = false,
                    ExpectedHeaders = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/http" },
                        { "Content-Transfer-Encoding", "binary" },
                    }
                },
                new PartHeaderTestCase
                {
                    DebugDescription = "Read an operation part header with no line feed after the header and some content; will fail.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "application/http")
                        .Header("Content-Transfer-Encoding", "binary")
                        .String("Some payload")
                        .ResetMemoryStream(),
                    ExpectChangeSetPart = false,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_InvalidHeaderSpecified", "Some payload"),
                },
                new PartHeaderTestCase
                {
                    DebugDescription = "Read an operation part header with multiple line feeds after the header and some content.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "application/http")
                        .Header("Content-Transfer-Encoding", "binary")
                        .LineFeed()
                        .LineFeed()
                        .String("Some payload")
                        .ResetMemoryStream(),
                    ExpectChangeSetPart = false,
                    ExpectedHeaders = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/http" },
                        { "Content-Transfer-Encoding", "binary" },
                    }
                },
                new PartHeaderTestCase
                {
                    DebugDescription = "Read a change set part header with a line feed after the header and some content.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "multipart/mixed;boundary=my_boundary")
                        .LineFeed()
                        .String("Some payload")
                        .ResetMemoryStream(),
                    ExpectChangeSetPart = true,
                    ExpectedHeaders = new Dictionary<string, string>
                    {
                        { "Content-Type", "multipart/mixed;boundary=my_boundary" },
                    }
                },
                new PartHeaderTestCase
                {
                    DebugDescription = "Read a change set part header with no line feed after the header and some content; will fail.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "multipart/mixed;boundary=my_boundary")
                        .String("Some payload")
                        .ResetMemoryStream(),
                    ExpectChangeSetPart = true,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_InvalidHeaderSpecified", "Some payload"),
                },
                new PartHeaderTestCase
                {
                    DebugDescription = "Read a change set part header with multiple line feeds after the header and some content.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "multipart/mixed;boundary=my_boundary")
                        .LineFeed()
                        .LineFeed()
                        .String("Some payload")
                        .ResetMemoryStream(),
                    ExpectChangeSetPart = true,
                    ExpectedHeaders = new Dictionary<string, string>
                    {
                        { "Content-Type", "multipart/mixed;boundary=my_boundary" },
                    }
                },
                #endregion
                #region Multiple (non-OData) headers
                new PartHeaderTestCase
                {
                    DebugDescription = "Read an operation part header with multiple headers.",
                    PayloadFunc = builder => builder
                        .Header("a", "b")
                        .Header("Content-Type", "application/http")
                        .Header("c", "d")
                        .Header("Content-Transfer-Encoding", "binary")
                        .Header("e", "f")
                        .LineFeed()
                        .String("Some payload")
                        .ResetMemoryStream(),
                    ExpectChangeSetPart = false,
                    ExpectedHeaders = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/http" },
                        { "Content-Transfer-Encoding", "binary" },
                        { "a", "b" },
                        { "c", "d" },
                        { "e", "f" },
                    }
                },
                new PartHeaderTestCase
                {
                    DebugDescription = "Read an operation part header with an invalid header.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "application/http")
                        .Header("Content-Transfer-Encoding", "binary")
                        .String("InvalidHeader Does not use a colon!")
                        .ResetMemoryStream(),
                    ExpectChangeSetPart = false,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_InvalidHeaderSpecified", "InvalidHeader Does not use a colon!"),
                },
                new PartHeaderTestCase
                {
                    DebugDescription = "Read an operation part header with a duplicate header.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "application/http")
                        .Header("Content-Transfer-Encoding", "binary")
                        .Header("a", "b")
                        .Header("a", "c")
                        .LineFeed()
                        .ResetMemoryStream(),
                    ExpectChangeSetPart = false,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_DuplicateHeaderFound", "a"),
                },
                #endregion
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    this.Injector.InjectDependenciesInto(testCase);
                    testCase.Run();
                });
        }

        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Testing the ODataMultipartMixedBatchReaderStream.ProcessPartHeader method.")]
        public void BatchReaderStreamBoundaryHeaderValidationTest()
        {
            IEnumerable<BoundaryHeaderValidationTestCase> testCases = new BoundaryHeaderValidationTestCase[]
            {
                new BoundaryHeaderValidationTestCase
                {
                    DebugDescription = "Read a change set part header with the boundary parameter missing.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "multipart/mixed")
                        .ResetMemoryStream(),
                    ExpectedException = ODataExpectedExceptions.ODataException("MediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads", "multipart/mixed", "boundary"),
                },
                new BoundaryHeaderValidationTestCase
                {
                    DebugDescription = "Read a change set part header with an invalid boundary parameter name.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "multipart/mixed;boundary2=changeset_boundary")
                        .ResetMemoryStream(),
                    ExpectedException = ODataExpectedExceptions.ODataException("MediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads", "multipart/mixed;boundary2=changeset_boundary", "boundary"),
                },
                new BoundaryHeaderValidationTestCase
                {
                    DebugDescription = "Read a change set part header with two boundary parameters.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "multipart/mixed;boundary=changeset_boundary;boundary=another_boundary")
                        .ResetMemoryStream(),
                    ExpectedException = ODataExpectedExceptions.ODataException("MediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads", "multipart/mixed;boundary=changeset_boundary;boundary=another_boundary", "boundary"),
                },
                new BoundaryHeaderValidationTestCase
                {
                    DebugDescription = "Read a change set part header with an empty boundary.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "multipart/mixed;boundary=")
                        .ResetMemoryStream(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_InvalidBatchBoundaryDelimiterLength", string.Empty, "70"),
                },
                new BoundaryHeaderValidationTestCase
                {
                    DebugDescription = "Read a change set part header with multiple parameters (incl. a boundary).",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "multipart/mixed;a=b;boundary=my_boundary;c=d")
                        .ResetMemoryStream(),
                    ExpectChangeSetPart = true,
                    ExpectedChangeSetBoundary = "my_boundary"
                },
                new BoundaryHeaderValidationTestCase
                {
                    DebugDescription = "Read a change set part header where the boundary parameter name has weird casing.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "multipart/mixed;a=b;BoUnDaRY=my_boundary;c=d")
                        .ResetMemoryStream(),
                    ExpectChangeSetPart = true,
                    ExpectedChangeSetBoundary = "my_boundary"
                },
                new BoundaryHeaderValidationTestCase
                {
                    DebugDescription = "Read a change set part with multiple headers.",
                    PayloadFunc = builder => builder
                        .Header("a", "b")
                        .Header("Content-Type", "multipart/mixed;boundary=my_boundary")
                        .Header("c", "d")
                        .Header("e", "f")
                        .LineFeed()
                        .String("Some payload")
                        .ResetMemoryStream(),
                    ExpectChangeSetPart = true,
                    ExpectedChangeSetBoundary = "my_boundary",
                },
                new BoundaryHeaderValidationTestCase
                {
                    DebugDescription = "Read a change set part with an invalid header.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "multipart/mixed;boundary=my_boundary")
                        .String("InvalidHeader Does not use a colon!")
                        .ResetMemoryStream(),
                    ExpectChangeSetPart = true,
                    ExpectedChangeSetBoundary = "my_boundary",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_InvalidHeaderSpecified", "InvalidHeader Does not use a colon!"),
                },
                new BoundaryHeaderValidationTestCase
                {
                    DebugDescription = "Read a change set part with duplicate headers.",
                    PayloadFunc = builder => builder
                        .Header("Content-Type", "multipart/mixed;boundary=my_boundary")
                        .Header("a", "b")
                        .Header("a", "c")
                        .LineFeed()
                        .String("Some payload")
                        .ResetMemoryStream(),
                    ExpectChangeSetPart = true,
                    ExpectedChangeSetBoundary = "my_boundary",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_DuplicateHeaderFound", "a"),
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    this.Injector.InjectDependenciesInto(testCase);
                    testCase.Run();
                });
        }

        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Testing the case sensitivity of header names.")]
        public void BatchReaderStreamHeaderNameValidationTest()
        {
            IEnumerable<PartHeaderTestCase> testCases = new PartHeaderTestCase[]
            {
                new PartHeaderTestCase
                {
                    DebugDescription = "Read two content type headers with different casing.",
                    PayloadFunc = builder => builder
                        .Header("cOnTenT-tYpE", "multipart/mixed;boundary=changeset_boundary")
                        .ResetMemoryStream(),
                    ExpectedHeaders = new Dictionary<string, string> 
                    {
                        { "cOnTenT-tYpE", "multipart/mixed;boundary=changeset_boundary" },
                    }
                },
                new PartHeaderTestCase
                {
                    DebugDescription = "Different case of header names.",
                    PayloadFunc = builder => builder
                        .Header("ab", "b")
                        .Header("content-type", "application/http")
                        .Header("c", "d")
                        .Header("content-transfer-encoding", "binary")
                        .LineFeed()
                        .String("Some payload")
                        .ResetMemoryStream(),
                    ExpectChangeSetPart = false,
                    ExpectedHeaders = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/http" },
                        { "Content-Transfer-Encoding", "binary" },
                        { "AB", "b" },
                        { "c", "d" },
                    }
                },
                new PartHeaderTestCase
                {
                    DebugDescription = "Two headers with different cases; one case-sensitive match.",
                    PayloadFunc = builder => builder
                        .Header("ab", "b1")
                        .Header("Ab", "b2")
                        .Header("content-type", "application/http")
                        .Header("content-transfer-encoding", "binary")
                        .LineFeed()
                        .String("Some payload")
                        .ResetMemoryStream(),
                    ExpectChangeSetPart = false,
                    ExpectedHeaders = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/http" },
                        { "Content-Transfer-Encoding", "binary" },
                        { "Ab", "b2" },
                    },
                },
                new PartHeaderTestCase
                {
                    DebugDescription = "Two headers with different cases; no case-sensitive match.",
                    PayloadFunc = builder => builder
                        .Header("ab", "b1")
                        .Header("Ab", "b2")
                        .Header("content-type", "application/http")
                        .Header("content-transfer-encoding", "binary")
                        .LineFeed()
                        .String("Some payload")
                        .ResetMemoryStream(),
                    ExpectChangeSetPart = false,
                    ExpectedHeaders = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/http" },
                        { "Content-Transfer-Encoding", "binary" },
                        { "AB", "b3" },
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchOperationHeaderDictionary_DuplicateCaseInsensitiveKeys", "AB")
                },
                new PartHeaderTestCase
                {
                    DebugDescription = "Multiple headers with different cases; one case-sensitive match.",
                    PayloadFunc = builder => builder
                        .Header("ab", "b1")
                        .Header("Ab", "b2")
                        .Header("AB", "b3")
                        .Header("content-type", "application/http")
                        .Header("content-transfer-encoding", "binary")
                        .LineFeed()
                        .String("Some payload")
                        .ResetMemoryStream(),
                    ExpectChangeSetPart = false,
                    ExpectedHeaders = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/http" },
                        { "Content-Transfer-Encoding", "binary" },
                        { "AB", "b3" },
                    },
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    this.Injector.InjectDependenciesInto(testCase);
                    testCase.Run();
                });
        }

        /// <summary>
        /// Test case class to test the ProcessPartHeader method.
        /// </summary>
        private sealed class PartHeaderTestCase : BatchReaderStreamTestCase
        {
            /// <summary>true if we expect a change set part; otherwise false.</summary>
            public bool? ExpectChangeSetPart { get; set; }

            /// <summary>The set of expected part headers.</summary>
            public IDictionary<string, string> ExpectedHeaders { get; set; }

            /// <summary>
            /// Runs the test action of this test after setting up the batch reader stream.
            /// </summary>
            /// <param name="streamWrapper">The batch reader stream to test.</param>
            protected override void RunTestAction(BatchReaderStreamWrapper streamWrapper)
            {
                // Set a batch encoding since we assert that it is set before calling ProcessPartHeader
                streamWrapper.SetBatchEncoding(this.Encoding);

                bool isChangeSetPart;
                BatchOperationHeadersWrapper headers = streamWrapper.ReadPartHeaders(out isChangeSetPart);
                this.VerifyResult(streamWrapper, headers, isChangeSetPart);
            }

            /// <summary>
            /// Verifies the result of reading the part headers.
            /// </summary>
            /// <param name="streamWrapper">The stream buffer used to read the headers from.</param>
            /// <param name="headers">The headers read from the part.</param>
            /// <param name="isChangeSetPart">true if we detected a change set part; otherwise false.</param>
            private void VerifyResult(BatchReaderStreamWrapper streamWrapper, BatchOperationHeadersWrapper headers, bool isChangeSetPart)
            {
                base.VerifyResult(streamWrapper.BatchBuffer);

                if (this.ExpectChangeSetPart.HasValue)
                {
                    this.Assert.AreEqual(this.ExpectChangeSetPart.Value, isChangeSetPart,
                        string.Format("\r\n{0}:\r\nExpected change set part is '{1}' but reported value is '{2}'.", this.DebugDescription, this.ExpectChangeSetPart.Value, isChangeSetPart));
                }

                if (this.ExpectedHeaders != null)
                {
                    this.Assert.IsNotNull(headers, string.Format("Expected {0} headers but none were found.", this.ExpectedHeaders.Count()));

                    foreach (KeyValuePair<string, string> expectedHeader in this.ExpectedHeaders)
                    {
                        // check that the expected header is present
                        string actualHeaderValue;
                        if (headers.TryGetValue(expectedHeader.Key, out actualHeaderValue))
                        {
                            this.Assert.AreEqual(expectedHeader.Value, actualHeaderValue,
                                string.Format("Expected value '{0}' for header '{1}' but found '{2}'.", expectedHeader.Value, expectedHeader.Key, actualHeaderValue));
                        }
                        else
                        {
                            this.Assert.Fail(string.Format("Did not find expected header '{0}'.", expectedHeader.Key));
                        }
                    }

                    if (headers.Any(kvp => !this.ExpectedHeaders.Any(kvp2 => string.Compare(kvp.Key, kvp2.Key, /*ignoreCase*/true) == 0)))
                    {
                        string expectedHeaders = string.Join(", ", this.ExpectedHeaders.Select(kvp => kvp.Key).ToArray());
                        string actualHeaders = string.Join(", ", headers.Select(kvp => kvp.Key).ToArray());
                        this.Assert.Fail("Found unexpected headers in the message headers.\r\n"
                            + "Expected headers: " + expectedHeaders + "\r\n"
                            + "Actual headers: " + actualHeaders);
                    }
                }
            }
        }

        /// <summary>
        /// Test case class to test the ProcessPartHeader method.
        /// </summary>
        private sealed class BoundaryHeaderValidationTestCase : BatchReaderStreamTestCase
        {
            /// <summary>The expected change set boundary.</summary>
            public string ExpectedChangeSetBoundary { get; set; }

            /// <summary>true if we expect a change set part; otherwise false.</summary>
            public bool? ExpectChangeSetPart { get; set; }

            /// <summary>
            /// Runs the test action of this test after setting up the batch reader stream.
            /// </summary>
            /// <param name="streamWrapper">The batch reader stream to test.</param>
            protected override void RunTestAction(BatchReaderStreamWrapper streamWrapper)
            {
                // Set a batch encoding since we assert that it is set before calling ProcessPartHeader
                streamWrapper.SetBatchEncoding(this.Encoding);

                bool isChangeSetPart = streamWrapper.ProcessPartHeader();
                this.VerifyResult(streamWrapper, isChangeSetPart);
            }

            /// <summary>
            /// Verifies the result of a boundary header validation test case.
            /// </summary>
            /// <param name="streamWrapper">The stream buffer used to read the headers from.</param>
            /// <param name="isChangeSetPart">true if we detected a change set part; otherwise false.</param>
            private void VerifyResult(BatchReaderStreamWrapper streamWrapper, bool isChangeSetPart)
            {
                base.VerifyResult(streamWrapper.BatchBuffer);

                if (this.ExpectChangeSetPart.HasValue)
                {
                    this.Assert.AreEqual(this.ExpectChangeSetPart.Value, isChangeSetPart,
                        string.Format("\r\n{0}:\r\nExpected change set part is '{1}' but reported value is '{2}'.", this.DebugDescription, this.ExpectChangeSetPart.Value, isChangeSetPart));
                }

                this.Assert.AreEqual(this.ExpectedChangeSetBoundary, streamWrapper.ChangeSetBoundary,
                    string.Format("\r\n{0}:\r\nExpected change set boundary '{1}' but reported value is '{2}'.", this.DebugDescription, this.ExpectedChangeSetBoundary, streamWrapper.ChangeSetBoundary));
            }
        }
#endif
    }
}
