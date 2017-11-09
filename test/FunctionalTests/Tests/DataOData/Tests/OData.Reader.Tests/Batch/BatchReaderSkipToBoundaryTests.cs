//---------------------------------------------------------------------
// <copyright file="BatchReaderSkipToBoundaryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests the ODataMultipartMixedBatchReaderStream.SkipToBoundary implementation.
    /// </summary>
    [TestClass, TestCase]
    public class BatchReaderSkipToBoundaryTests : ODataReaderTestCase
    {
        /// <summary>Boundary strings used in the tests.</summary>
        private static readonly IEnumerable<string> BoundaryStrings = new string[] 
        { 
            "-", 
            "a-b-c", 
            "0123456789012345678901234567890123456789012345678901234567890123456789" 
        };

        /// <summary>Line feed characters used in the tests.</summary>
        private static readonly IEnumerable<char[]> LineFeeds = new char[][] 
        { 
            BatchReaderStreamTestUtils.DefaultLineFeedChars, 
            new char[] { '\r' }, 
            new char[] { '\n' } 
        };

        /// <summary>The counts of white spaces used in the tests.</summary>
        private static readonly IEnumerable<int> WhitespaceCounts = new int[] { 0, 1, 10 };

        /// <summary>Array of boolean values indicating whether to precede a boundary delimiter with a line feed or not.</summary>
        private static readonly bool[] LeadingLineFeedForBoundary = new bool[] { true, false };

        /// <summary>
        /// Gets or sets the dependency injector.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDependencyInjector Injector { get; set; }

#if !SILVERLIGHT && !WINDOWS_PHONE
        // Batch stream buffer tests use private reflection and thus cannot run on SilverLight or the phone.
        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Testing the ODataMultipartMixedBatchReaderStream.SkipToBoundary method.")]
        public void BatchReaderSkipToBoundaryTest()
        {
            IEnumerable<SkipToBoundaryTestCase> testCases =
                    this.CreateEmptyBufferTestCases()
                    .Concat(this.CreateNoBoundaryTestCases())
                    .Concat(this.CreateBoundaryAtTheBeginningTestCases())
                    .Concat(this.CreateBoundaryInTheMiddleTestCases())
                    .Concat(this.CreateBoundaryAtTheEndTestCases())
                    .Concat(this.CreateBoundaryAcrossBuffersTestCases())
                    .Concat(this.CreatePartialBoundaryAtStreamEndTestCases());
            
            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    this.Injector.InjectDependenciesInto(testCase);
                    testCase.Run();
                });
        }

        private IEnumerable<SkipToBoundaryTestCase> CreateEmptyBufferTestCases()
        {
            List<SkipToBoundaryTestCase> testCases = new List<SkipToBoundaryTestCase>();
            this.CombinatorialEngineProvider.RunCombinations(
                BoundaryStrings,
                LineFeeds,
                (boundaryString, lineFeed) =>
                {
                    SkipToBoundaryTestCase testCase = new SkipToBoundaryTestCase(boundaryString, lineFeed)
                    {
                        DebugDescription = "Skip to boundary '" + boundaryString + "' in empty buffer.",
                        PayloadFunc = builder => builder.ResetMemoryStream(),
                        ExpectedIsEndBoundary = false,
                        ExpectedBoundaryFound = false,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                            NumberOfBytesInBuffer = 0,
                        }
                    };
                    testCases.Add(testCase);
                });
            return testCases;
        }

        private IEnumerable<SkipToBoundaryTestCase> CreateNoBoundaryTestCases()
        {
            List<SkipToBoundaryTestCase> testCases = new List<SkipToBoundaryTestCase>();
            this.CombinatorialEngineProvider.RunCombinations(
                BoundaryStrings.Where(str => str.Length > 1),     // excluding '-' boundary string since partials would be the empty string which this test does not handle
                LeadingLineFeedForBoundary,
                WhitespaceCounts,
                LineFeeds,
                new bool[] { false, true },
                (boundaryString, leadingBoundaryLineFeed, whitespaceCount, lineFeed, isEndBoundary) =>
                {
                    string almostBoundary = boundaryString.Substring(0, boundaryString.Length - 1);

                    int leadingLineFeedLength = leadingBoundaryLineFeed ? lineFeed.Length : 0;
                    int startBoundaryLength = leadingLineFeedLength + BatchReaderStreamTestUtils.TwoDashesLength + almostBoundary.Length + whitespaceCount + lineFeed.Length;
                    int endBoundaryLength = startBoundaryLength + BatchReaderStreamTestUtils.TwoDashesLength;
                    int almostBoundaryLength = isEndBoundary ? endBoundaryLength : startBoundaryLength;

                    // Use a stream where a string similar to the boundary is included
                    SkipToBoundaryTestCase testCase = new SkipToBoundaryTestCase(boundaryString, lineFeed)
                    {
                        DebugDescription = "Skip to boundary '" + boundaryString + "' in stream with similar string in first buffer.",
                        PayloadFunc = builder => isEndBoundary
                            ? builder
                                .FillBytes(200, 'a')
                                .EndBoundary(almostBoundary, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(200, 'b')
                                .ResetMemoryStream()
                            : builder
                                .FillBytes(200, 'a')
                                .StartBoundary(almostBoundary, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(200, 'b')
                                .ResetMemoryStream(),
                        ExpectedIsEndBoundary = false,
                        ExpectedBoundaryFound = false,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                            NumberOfBytesInBuffer = 0,
                        }
                    };
                    testCases.Add(testCase);

                    // Use a stream where a string similar to the boundary exists in the second buffer
                    testCase = new SkipToBoundaryTestCase(boundaryString, lineFeed)
                    {
                        DebugDescription = "Skip to boundary '" + boundaryString + "' in stream with similar string in second buffer.",
                        PayloadFunc = builder => isEndBoundary
                            ? builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'a')
                                .FillBytes(200, 'b')
                                .EndBoundary(almostBoundary, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(200, 'c')
                                .ResetMemoryStream()
                            : builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'a')
                                .FillBytes(200, 'b')
                                .StartBoundary(almostBoundary, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(200, 'c')
                                .ResetMemoryStream(),
                        ExpectedIsEndBoundary = false,
                        ExpectedBoundaryFound = false,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                            NumberOfBytesInBuffer = 0,
                        }
                    };
                    testCases.Add(testCase);

                    // Use a stream where a string similar to the boundary exists across buffers
                    testCase = new SkipToBoundaryTestCase(boundaryString, lineFeed)
                    {
                        DebugDescription = "Skip to boundary '" + boundaryString + "' in stream with similar string across buffers.",
                        PayloadFunc = builder => isEndBoundary
                            ? builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - 3, 'a')
                                .EndBoundary(almostBoundary, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(200, 'b')
                                .ResetMemoryStream()
                            : builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - 3, 'a')
                                .StartBoundary(almostBoundary, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(200, 'b')
                                .ResetMemoryStream(),
                        ExpectedIsEndBoundary = false,
                        ExpectedBoundaryFound = false,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                            NumberOfBytesInBuffer = 0,
                        }
                    };
                    testCases.Add(testCase);
                });
            return testCases;
        }

        private IEnumerable<SkipToBoundaryTestCase> CreateBoundaryAtTheBeginningTestCases()
        {
            List<SkipToBoundaryTestCase> testCases = new List<SkipToBoundaryTestCase>();
            this.CombinatorialEngineProvider.RunCombinations(
                BoundaryStrings,
                LeadingLineFeedForBoundary,
                WhitespaceCounts,
                LineFeeds,
                new bool[] { false, true },
                (boundaryString, leadingBoundaryLineFeed, whitespaceCount, lineFeed, isEndBoundary) =>
                {
                    int leadingLineFeedLength = leadingBoundaryLineFeed ? lineFeed.Length : 0;
                    int startBoundaryLength = leadingLineFeedLength + BatchReaderStreamTestUtils.TwoDashesLength + boundaryString.Length + whitespaceCount + lineFeed.Length;
                    int endBoundaryLength = startBoundaryLength + BatchReaderStreamTestUtils.TwoDashesLength;
                    int boundaryLength = isEndBoundary ? endBoundaryLength : startBoundaryLength;

                    SkipToBoundaryTestCase testCase = new SkipToBoundaryTestCase(boundaryString, lineFeed)
                    {
                        DebugDescription = "Skip to boundary " + boundaryString + " with " + (isEndBoundary ? "end" : "start") + " delimiter at the beginning.",
                        PayloadFunc = builder => isEndBoundary
                            ? builder.EndBoundary(boundaryString, leadingBoundaryLineFeed, whitespaceCount).FillBytes(BatchReaderStreamBufferWrapper.BufferLength + 200, 'a').ResetMemoryStream()
                            : builder.StartBoundary(boundaryString, leadingBoundaryLineFeed, whitespaceCount).FillBytes(BatchReaderStreamBufferWrapper.BufferLength + 200, 'a').ResetMemoryStream(),
                        ExpectedBoundaryFound = true,
                        ExpectedIsEndBoundary = isEndBoundary,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = boundaryLength,
                            NumberOfBytesInBuffer = BatchReaderStreamBufferWrapper.BufferLength - boundaryLength,
                        }
                    };
                    testCases.Add(testCase);
                });

            return testCases;
        }

        private IEnumerable<SkipToBoundaryTestCase> CreateBoundaryInTheMiddleTestCases()
        {
            List<SkipToBoundaryTestCase> testCases = new List<SkipToBoundaryTestCase>();
            this.CombinatorialEngineProvider.RunCombinations(
                BoundaryStrings,
                LeadingLineFeedForBoundary,
                WhitespaceCounts,
                LineFeeds,
                new bool[] { false, true },
                (boundaryString, leadingBoundaryLineFeed, whitespaceCount, lineFeed, isEndBoundary) =>
                {
                    int leadingLineFeedLength = leadingBoundaryLineFeed ? lineFeed.Length : 0;
                    int startBoundaryLength = leadingLineFeedLength + BatchReaderStreamTestUtils.TwoDashesLength + boundaryString.Length + whitespaceCount + lineFeed.Length;
                    int endBoundaryLength = startBoundaryLength + BatchReaderStreamTestUtils.TwoDashesLength;
                    int boundaryLength = isEndBoundary ? endBoundaryLength : startBoundaryLength;

                    SkipToBoundaryTestCase testCase = new SkipToBoundaryTestCase(boundaryString, lineFeed)
                    {
                        DebugDescription = "Skip to boundary " + boundaryString + " with " + (isEndBoundary ? "end" : "start") + " delimiter in the middle.",
                        PayloadFunc = builder => isEndBoundary
                            ? builder.FillBytes(200, 'a').EndBoundary(boundaryString, leadingBoundaryLineFeed, whitespaceCount).FillBytes(200, 'b').ResetMemoryStream()
                            : builder.FillBytes(200, 'a').StartBoundary(boundaryString, leadingBoundaryLineFeed, whitespaceCount).FillBytes(200, 'b').ResetMemoryStream(),
                        ExpectedBoundaryFound = true,
                        ExpectedIsEndBoundary = isEndBoundary,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 200 + boundaryLength,
                            NumberOfBytesInBuffer = 200,
                        }
                    };
                    testCases.Add(testCase);
                });

            return testCases;
        }

        private IEnumerable<SkipToBoundaryTestCase> CreateBoundaryAtTheEndTestCases()
        {
            List<SkipToBoundaryTestCase> testCases = new List<SkipToBoundaryTestCase>();
            this.CombinatorialEngineProvider.RunCombinations(
                BoundaryStrings,
                LeadingLineFeedForBoundary,
                WhitespaceCounts,
                LineFeeds,
                new bool[] { false, true },
                (boundaryString, leadingBoundaryLineFeed, whitespaceCount, lineFeed, isEndBoundary) =>
                {
                    int leadingLineFeedLength = leadingBoundaryLineFeed ? lineFeed.Length : 0;
                    int startBoundaryLength = leadingLineFeedLength + BatchReaderStreamTestUtils.TwoDashesLength + boundaryString.Length + whitespaceCount + lineFeed.Length;
                    int endBoundaryLength = startBoundaryLength + BatchReaderStreamTestUtils.TwoDashesLength;

                    SkipToBoundaryTestCase testCase = new SkipToBoundaryTestCase(boundaryString, lineFeed)
                    {
                        DebugDescription = "Skip to boundary " + boundaryString + " with " + (isEndBoundary ? "end" : "start") + " delimiter at the end.",
                        PayloadFunc = builder => isEndBoundary
                            ? builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - 1 - endBoundaryLength, 'a')
                                .EndBoundary(boundaryString, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(1, 'b')
                                .ResetMemoryStream()
                            : builder
                                // NOTE: we keep 3 bytes at the end of the buffer here since the algorithm always tries 
                                //       to match the end boundary and otherwise reports a partial match.
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - 3 - startBoundaryLength, 'a')
                                .StartBoundary(boundaryString, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(3, 'b')
                                .ResetMemoryStream(),
                        ExpectedBoundaryFound = true,
                        ExpectedIsEndBoundary = isEndBoundary,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = BatchReaderStreamBufferWrapper.BufferLength - (isEndBoundary ? 1 : 3),
                            NumberOfBytesInBuffer = isEndBoundary ? 1 : 3,
                        }
                    };
                    testCases.Add(testCase);
                });

            return testCases;
        }

        private IEnumerable<SkipToBoundaryTestCase> CreateBoundaryAcrossBuffersTestCases()
        {
            List<SkipToBoundaryTestCase> testCases = new List<SkipToBoundaryTestCase>();
            this.CombinatorialEngineProvider.RunCombinations(
                BoundaryStrings,
                LeadingLineFeedForBoundary,
                WhitespaceCounts,
                LineFeeds,
                new bool[] { false, true },
                (boundaryString, leadingBoundaryLineFeed, whitespaceCount, lineFeed, isEndBoundary) =>
                {
                    int leadingLineFeedLength = leadingBoundaryLineFeed ? lineFeed.Length : 0;
                    int startBoundaryLength = leadingLineFeedLength + BatchReaderStreamTestUtils.TwoDashesLength + boundaryString.Length + whitespaceCount + lineFeed.Length;
                    int endBoundaryLength = startBoundaryLength + BatchReaderStreamTestUtils.TwoDashesLength;
                    int boundaryLength = isEndBoundary ? endBoundaryLength : startBoundaryLength;

                    SkipToBoundaryTestCase testCase = new SkipToBoundaryTestCase(boundaryString, lineFeed)
                    {
                        DebugDescription = "Skip to boundary " + boundaryString + " with " + (isEndBoundary ? "end" : "start") + " delimiter across buffer boundaries.",
                        PayloadFunc = builder => isEndBoundary
                            ? builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - 2, 'a')
                                .EndBoundary(boundaryString, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(100, 'b')
                                .ResetMemoryStream()
                            : builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - 2, 'a')
                                .StartBoundary(boundaryString, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(100, 'b')
                                .ResetMemoryStream(),
                        ExpectedBoundaryFound = true,
                        ExpectedIsEndBoundary = isEndBoundary,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = boundaryLength,
                            NumberOfBytesInBuffer = 100,
                        }
                    };
                    testCases.Add(testCase);
                });

            return testCases;
        }

        private IEnumerable<SkipToBoundaryTestCase> CreatePartialBoundaryAtStreamEndTestCases()
        {
            List<SkipToBoundaryTestCase> testCases = new List<SkipToBoundaryTestCase>();
            this.CombinatorialEngineProvider.RunCombinations(
                BoundaryStrings,
                LeadingLineFeedForBoundary,
                LineFeeds,
                new bool[] { false, true },
                (boundaryString, leadingBoundaryLineFeed, lineFeed, isEndBoundary) =>
                {
                    // Fill the buffer with 'a' bytes and leave some room for the line feed, 
                    // the two dashes and the first byte of the boundary
                    int leadingLineFeedLength = leadingBoundaryLineFeed ? lineFeed.Length : 0;
                    int partialBoundaryLength = leadingLineFeedLength + BatchReaderStreamTestUtils.TwoDashesLength + 1;
                    int bytesBeforePartialBoundary = BatchReaderStreamBufferWrapper.BufferLength - partialBoundaryLength;

                    SkipToBoundaryTestCase testCase = new SkipToBoundaryTestCase(boundaryString, lineFeed)
                    {
                        DebugDescription = "Skip to boundary " + boundaryString + " with partial " + (isEndBoundary ? "end" : "start") + " delimiter at the end of the stream.",
                        PayloadFunc = builder => leadingBoundaryLineFeed
                            ? builder
                                .FillBytes(bytesBeforePartialBoundary, 'a')
                                .LineFeed()
                                .Chars(MemoryStreamBatchPayloadBuilder.BoundaryDelimiter)
                                .Chars(boundaryString[0])
                                .ResetMemoryStream()
                            : builder
                                .FillBytes(bytesBeforePartialBoundary, 'a')
                                .Chars(MemoryStreamBatchPayloadBuilder.BoundaryDelimiter)
                                .Chars(boundaryString[0])
                                .ResetMemoryStream(),
                        ExpectedBoundaryFound = false,
                        ExpectedIsEndBoundary = false,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = partialBoundaryLength,
                            NumberOfBytesInBuffer = 0,
                        }
                    };
                    testCases.Add(testCase);
                });

            return testCases;
        }

        /// <summary>
        /// Test case class to test the SkipToBoundary method of the batch reader stream.
        /// </summary>
        private sealed class SkipToBoundaryTestCase : BatchReaderStreamTestCase
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="batchBoundary">The boundary string to be used for the batch request; or null if a default boundary should be used.</param>
            /// <param name="lineFeedChars">The line feed characters to use in the test.</param>
            public SkipToBoundaryTestCase(string batchBoundary, char[] lineFeedChars = null)
                : this(batchBoundary, null, lineFeedChars)
            {
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="batchBoundary">The boundary string to be used for the batch request; or null if a default boundary should be used.</param>
            /// <param name="encoding">The encoding to use to convert bytes to and from strings.</param>
            /// <param name="lineFeedChars">The line feed characters to use in the test.</param>
            public SkipToBoundaryTestCase(string batchBoundary = null, Encoding encoding = null, char[] lineFeedChars = null)
                : base(batchBoundary, encoding, lineFeedChars)
            {
            }

            /// <summary>
            /// Copy constructor.
            /// </summary>
            /// <param name="other">The test case instance to copy.</param>
            public SkipToBoundaryTestCase(SkipToBoundaryTestCase other)
                : base(other)
            {
                this.ExpectedBoundaryFound = other.ExpectedBoundaryFound;
                this.ExpectedIsEndBoundary = other.ExpectedIsEndBoundary;
            }

            /// <summary>true if we expected to detect an end boundary; 
            /// otherwise false or null to ignore during verification.</summary>
            public bool? ExpectedIsEndBoundary { get; set; }

            /// <summary>true if we expected to detect a parent boundary; 
            /// otherwise false or null to ignore during verification.</summary>
            public bool? ExpectedIsParentBoundary { get; set; }

            /// <summary>true if we expected to find the boundary; 
            /// otherwise false or null to ignore during verification.</summary>
            public bool? ExpectedBoundaryFound { get; set; }

            /// <summary>
            /// Runs the actual test action against the batch reader stream.
            /// </summary>
            /// <param name="streamWrapper">The batch reader stream wrapper to execute the test action against.</param>
            protected override void RunTestAction(BatchReaderStreamWrapper streamWrapper)
            {
                bool isEndBoundary, isParentBoundary;
                bool success = streamWrapper.SkipToBoundary(out isEndBoundary, out isParentBoundary);
                this.VerifyResult(streamWrapper, success, isEndBoundary, isParentBoundary);
            }

            /// <summary>
            /// Verifies that the resulting stream buffer is in the expected state.
            /// </summary>
            /// <param name="streamWrapper">The batch reader stream wrapper to execute the test action against.</param>
            /// <param name="foundBoundary">true if we found the boundary; otherwise false.</param>
            /// <param name="isEndBoundary">true if we detected an end boundary; otherwise false.</param>
            private void VerifyResult(BatchReaderStreamWrapper streamWrapper, bool foundBoundary, bool isEndBoundary, bool isParentBoundary)
            {
                if (this.ExpectedBoundaryFound.HasValue)
                {
                    this.Assert.AreEqual(this.ExpectedBoundaryFound.Value, foundBoundary,
                        string.Format("\r\n{0}:\r\nExpected 'foundBoundary' to be '{1}' but reported value is '{2}'.", this.DebugDescription, this.ExpectedBoundaryFound.Value, foundBoundary));
                }

                if (this.ExpectedIsEndBoundary.HasValue)
                {
                    this.Assert.AreEqual(this.ExpectedIsEndBoundary.Value, isEndBoundary,
                        string.Format("\r\n{0}:\r\nExpected 'isEndBoundary' to be '{1}' but reported value is '{2}'.", this.DebugDescription, this.ExpectedIsEndBoundary.Value, isEndBoundary));
                }

                if (this.ExpectedIsParentBoundary.HasValue)
                {
                    this.Assert.AreEqual(this.ExpectedIsParentBoundary.Value, isParentBoundary,
                        string.Format("\r\n{0}:\r\nExpected 'isParentBoundary' to be '{1}' but reported value is '{2}'.", this.DebugDescription, this.ExpectedIsParentBoundary.Value, isParentBoundary));
                }

                base.VerifyResult(streamWrapper.BatchBuffer);
            }
        }
#endif
    }
}
