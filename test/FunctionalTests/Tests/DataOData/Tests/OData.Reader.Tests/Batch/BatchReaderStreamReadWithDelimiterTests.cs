//---------------------------------------------------------------------
// <copyright file="BatchReaderStreamReadWithDelimiterTests.cs" company="Microsoft">
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
    /// Tests the ODataBatchReaderStream.ReadWithDelimiter implementation.
    /// </summary>
    [TestClass, TestCase]
    public class BatchReaderStreamReadWithDelimiterTests : ODataReaderTestCase
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

        /// <summary>The counts of whitespaces used in the tests.</summary>
        private static readonly IEnumerable<int> WhitespaceCounts = new int[] { 0, 1, 10 };

        /// <summary>Array of boolean values indicating whether to preceed a boundary delimiter with a line feed or not.</summary>
        private static readonly bool[] LeadingLineFeedForBoundary = new bool[] { true, false };

        /// <summary>
        /// Gets or sets the dependency injector.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDependencyInjector Injector { get; set; }

        // Batch stream buffer tests use private reflection and thus cannot run on Silverlight or the phone.
        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Testing the ODataBatchReaderStream.ReadWithDelimiter method.")]
        public void BatchReaderStreamReadWithDelimiterTest()
        {
            IEnumerable<ReadWithDelimiterTestCase> testCases = 
                    this.CreateEmptyBufferTestCases()
                    .Concat(this.CreateNoDelimiterTestCases())
                    .Concat(this.CreateBoundaryAtTheBeginningTestCases())
                    .Concat(this.CreateBoundaryInTheMiddleTestCases())
                    .Concat(this.CreateBoundaryAtTheEndTestCases())
                    .Concat(this.CreateBoundaryAcrossBuffersTestCases())
                    .Concat(this.CreatePartialBoundaryAtStreamEndTestCases())
                    .Concat(this.CreateErrorTestCases());
                    // TODO: add test cases that do multiple reads against the buffer

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    this.Injector.InjectDependenciesInto(testCase);
                    testCase.Run();
                });
        }

        private IEnumerable<ReadWithDelimiterTestCase> CreateEmptyBufferTestCases()
        {
            List<ReadWithDelimiterTestCase> testCases = new List<ReadWithDelimiterTestCase>();
            this.CombinatorialEngineProvider.RunCombinations(
                LineFeeds,
                new int[] { 0, 100 },
                (lineFeed, bytesToRead) =>
                {
                    ReadWithDelimiterTestCase testCase = new ReadWithDelimiterTestCase(lineFeed)
                    {
                        DebugDescription = "Read " + bytesToRead + " bytes from empty buffer.",
                        PayloadFunc = builder => builder.ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor 
                            { 
                                BufferSize = 100, 
                                BufferOffset = 0, 
                                NumberOfBytesToRead = bytesToRead, 
                                ExpectedNumberOfBytesRead = 0 
                            }
                        },
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

        private IEnumerable<ReadWithDelimiterTestCase> CreateNoDelimiterTestCases()
        {
            List<ReadWithDelimiterTestCase> testCases = new List<ReadWithDelimiterTestCase>();
            this.CombinatorialEngineProvider.RunCombinations(
                LineFeeds,
                new int[] { 0, 100, 200, 300 },
                (lineFeed, bytesToRead) =>
                {
                    ReadWithDelimiterTestCase testCase = new ReadWithDelimiterTestCase(lineFeed)
                    {
                        DebugDescription = "Read " + bytesToRead + " bytes from non-empty buffer without delimiter.",
                        PayloadFunc = builder => builder.FillBytes(200, 'a').ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor 
                            { 
                                BufferSize = 500, 
                                BufferOffset = 0, 
                                NumberOfBytesToRead = bytesToRead, 
                                ExpectedNumberOfBytesRead = Math.Min(bytesToRead, 200) 
                            }
                        },
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = bytesToRead == 300
                                ? 0
                                : Math.Min(bytesToRead, 200),
                            NumberOfBytesInBuffer = bytesToRead == 0 ? 0 : Math.Max(0, 200 - bytesToRead),
                        }
                    };
                    testCases.Add(testCase);
                });
            return testCases;
        }

        private IEnumerable<ReadWithDelimiterTestCase> CreateBoundaryAtTheBeginningTestCases()
        {
            List<ReadWithDelimiterTestCase> testCases = new List<ReadWithDelimiterTestCase>();
            this.CombinatorialEngineProvider.RunCombinations(
                BoundaryStrings,
                LeadingLineFeedForBoundary,
                LineFeeds,
                new bool[] { true, false },
                new int[] { 0, 100, 200, 300 },
                (boundary, leadingBoundaryLineFeed, lineFeed, isEndBoundary, bytesToRead) =>
                {
                    int leadingLineFeedLength = leadingBoundaryLineFeed ? lineFeed.Length : 0;
                    int startBoundaryLength = leadingLineFeedLength + BatchReaderStreamTestUtils.TwoDashesLength + boundary.Length + lineFeed.Length;
                    int endBoundaryLength = startBoundaryLength + BatchReaderStreamTestUtils.TwoDashesLength;
                    Func<bool, int> boundaryLength = isEnd => isEnd ? endBoundaryLength : startBoundaryLength;

                    ReadWithDelimiterTestCase testCase = new ReadWithDelimiterTestCase(lineFeed)
                    {
                        DebugDescription = "Read " + bytesToRead + " bytes from non-empty buffer with delimiter '" + boundary + "' at the beginning.\r\n" +
                            "[isEnd = " + isEndBoundary + ", leadingLineFeed = " + leadingBoundaryLineFeed + ", lineFeed = " + StringUtils.LineFeedString(lineFeed) +
                            ", bytesToRead = " + bytesToRead + "]",
                        BatchBoundary = boundary,
                        PayloadFunc = builder => isEndBoundary
                            ? builder.EndBoundary(boundary, leadingBoundaryLineFeed).FillBytes(200, 'a').ResetMemoryStream()
                            : builder.StartBoundary(boundary, leadingBoundaryLineFeed).FillBytes(200, 'a').ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor { BufferSize = 1000, BufferOffset = 0, NumberOfBytesToRead = bytesToRead, ExpectedNumberOfBytesRead = 0 }
                        },
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                            NumberOfBytesInBuffer = bytesToRead == 0 ? 0 : boundaryLength(isEndBoundary) + 200,
                        }
                    };
                    testCases.Add(testCase);
                });

            return testCases;
        }

        private IEnumerable<ReadWithDelimiterTestCase> CreateBoundaryInTheMiddleTestCases()
        {
            List<ReadWithDelimiterTestCase> testCases = new List<ReadWithDelimiterTestCase>();
            this.CombinatorialEngineProvider.RunCombinations(
                BoundaryStrings,
                LeadingLineFeedForBoundary,
                LineFeeds,
                new bool[] { true, false },
                new int[] { 0, 100, 200, 300 },
                (boundary, leadingBoundaryLineFeed, lineFeed, isEndBoundary, bytesToRead) =>
                {
                    int leadingLineFeedLength = leadingBoundaryLineFeed ? lineFeed.Length : 0;
                    int startBoundaryLength = leadingLineFeedLength + BatchReaderStreamTestUtils.TwoDashesLength + boundary.Length + lineFeed.Length;
                    int endBoundaryLength = startBoundaryLength + BatchReaderStreamTestUtils.TwoDashesLength;
                    Func<bool, int> boundaryLength = isEnd => isEnd ? endBoundaryLength : startBoundaryLength;

                    ReadWithDelimiterTestCase testCase = new ReadWithDelimiterTestCase(lineFeed)
                    {
                        DebugDescription = "Read " + bytesToRead + " bytes from non-empty buffer with delimiter '" + boundary + "' in the middle.\r\n" +
                            "[isEnd = " + isEndBoundary + ", leadingLineFeed = " + leadingBoundaryLineFeed + ", lineFeed = " + StringUtils.LineFeedString(lineFeed) +
                            ", bytesToRead = " + bytesToRead + "]",
                        BatchBoundary = boundary,
                        PayloadFunc = builder => isEndBoundary
                            ? builder.FillBytes(200, 'a').EndBoundary(boundary, leadingBoundaryLineFeed).FillBytes(200 , 'b').ResetMemoryStream()
                            : builder.FillBytes(200, 'a').StartBoundary(boundary, leadingBoundaryLineFeed).FillBytes(200, 'b').ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor 
                            { 
                                BufferSize = 1000, 
                                BufferOffset = 0, 
                                NumberOfBytesToRead = bytesToRead, 
                                ExpectedNumberOfBytesRead = Math.Min(200, bytesToRead) 
                            }
                        },
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = bytesToRead == 0 ? 0 : Math.Min(200, bytesToRead),
                            NumberOfBytesInBuffer = bytesToRead == 0 
                                ? 0 
                                : bytesToRead > 200 
                                    ? 200 + boundaryLength(isEndBoundary) 
                                    : (200 - bytesToRead) + boundaryLength(isEndBoundary) + 200,
                        }
                    };
                    testCases.Add(testCase);
                });

            return testCases;
        }

        private IEnumerable<ReadWithDelimiterTestCase> CreateBoundaryAtTheEndTestCases()
        {
            List<ReadWithDelimiterTestCase> testCases = new List<ReadWithDelimiterTestCase>();
            this.CombinatorialEngineProvider.RunCombinations(
                BoundaryStrings,
                LeadingLineFeedForBoundary,
                LineFeeds,
                new bool[] { true, false },
                new int[] { 0, 100, 200, 300 },
                (boundary, leadingBoundaryLineFeed, lineFeed, isEndBoundary, bytesToRead) =>
                {
                    int leadingLineFeedLength = leadingBoundaryLineFeed ? lineFeed.Length : 0;
                    int startBoundaryLength = leadingLineFeedLength + BatchReaderStreamTestUtils.TwoDashesLength + boundary.Length + lineFeed.Length;
                    int endBoundaryLength = startBoundaryLength + BatchReaderStreamTestUtils.TwoDashesLength;
                    int dataBytesInStartBoundaryBuffer = BatchReaderStreamBufferWrapper.BufferLength - 1 - startBoundaryLength;
                    int dataBytesInEndBoundaryBuffer = dataBytesInStartBoundaryBuffer - BatchReaderStreamTestUtils.TwoDashesLength;
                    Func<bool, int> dataBytesInBuffer = isEnd => isEnd ? dataBytesInEndBoundaryBuffer : dataBytesInStartBoundaryBuffer;

                    ReadWithDelimiterTestCase testCase = new ReadWithDelimiterTestCase(lineFeed)
                    {
                        DebugDescription = "Read " + bytesToRead + " bytes from non-empty buffer with delimiter '" + boundary + "' at the end.\r\n" +
                            "[isEnd = " + isEndBoundary + ", leadingLineFeed = " + leadingBoundaryLineFeed + ", lineFeed = " + StringUtils.LineFeedString(lineFeed) +
                            ", bytesToRead = " + bytesToRead + "]",
                        BatchBoundary = boundary,
                        PayloadFunc = builder => isEndBoundary
                            ? builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - 1 - endBoundaryLength, 'a')
                                .EndBoundary(boundary, leadingBoundaryLineFeed)
                                .FillBytes(1, 'b')
                                .ResetMemoryStream()
                            : builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - 1 - startBoundaryLength, 'a')
                                .StartBoundary(boundary, leadingBoundaryLineFeed)
                                .FillBytes(1, 'b')
                                .ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor 
                            { 
                                BufferSize = BatchReaderStreamBufferWrapper.BufferLength, 
                                BufferOffset = 0, 
                                NumberOfBytesToRead = bytesToRead, 
                                ExpectedNumberOfBytesRead = Math.Min(bytesToRead, dataBytesInBuffer(isEndBoundary))
                            }
                        },
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = Math.Min(bytesToRead, dataBytesInBuffer(isEndBoundary)),
                            NumberOfBytesInBuffer = bytesToRead == 0 ? 0 : BatchReaderStreamBufferWrapper.BufferLength - Math.Min(bytesToRead, dataBytesInBuffer(isEndBoundary)),
                        }
                    };
                    testCases.Add(testCase);
                });

            return testCases;
        }

        private IEnumerable<ReadWithDelimiterTestCase> CreateBoundaryAcrossBuffersTestCases()
        {
            List<ReadWithDelimiterTestCase> testCases = new List<ReadWithDelimiterTestCase>();
            this.CombinatorialEngineProvider.RunCombinations(
                BoundaryStrings,
                LeadingLineFeedForBoundary,
                LineFeeds,
                new bool[] { true, false },
                new int[] { 0, 100, BatchReaderStreamBufferWrapper.BufferLength - 2, BatchReaderStreamBufferWrapper.BufferLength },
                (boundary, leadingBoundaryLineFeed, lineFeed, isEndBoundary, bytesToRead) =>
                {
                    int leadingLineFeedLength = leadingBoundaryLineFeed ? lineFeed.Length : 0;
                    int startBoundaryLength = leadingLineFeedLength + BatchReaderStreamTestUtils.TwoDashesLength + boundary.Length + lineFeed.Length;
                    int endBoundaryLength = startBoundaryLength + BatchReaderStreamTestUtils.TwoDashesLength;
                    Func<bool, int> boundaryLength = isEnd => isEnd ? endBoundaryLength : startBoundaryLength;

                    ReadWithDelimiterTestCase testCase = new ReadWithDelimiterTestCase(lineFeed)
                    {
                        DebugDescription = "Read " + bytesToRead + " bytes from buffer with partial delimiter at the end.\r\n" +
                            "[isEnd = " + isEndBoundary + ", leadingLineFeed = " + leadingBoundaryLineFeed + ", lineFeed = " + StringUtils.LineFeedString(lineFeed) +
                            ", bytesToRead = " + bytesToRead + "]",
                        BatchBoundary = boundary,
                        PayloadFunc = builder => isEndBoundary
                            ? builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - 2, 'a')
                                .EndBoundary(boundary, leadingBoundaryLineFeed)
                                .FillBytes(100, 'b')
                                .ResetMemoryStream()
                            : builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - 2, 'a')
                                .StartBoundary(boundary, leadingBoundaryLineFeed)
                                .FillBytes(100, 'b')
                                .ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor 
                            { 
                                BufferSize = BatchReaderStreamBufferWrapper.BufferLength, 
                                BufferOffset = 0, 
                                NumberOfBytesToRead = bytesToRead, 
                                ExpectedNumberOfBytesRead = Math.Min(bytesToRead, BatchReaderStreamBufferWrapper.BufferLength - 2)
                            }
                        },
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = bytesToRead == BatchReaderStreamBufferWrapper.BufferLength
                                // we refilled the buffer only to find the boundary
                                ? 0
                                // we did not refill the buffer
                                : bytesToRead,
                            NumberOfBytesInBuffer = bytesToRead == 0
                                    // nothing was loaded into the buffer
                                    ? 0
                                    : bytesToRead == BatchReaderStreamBufferWrapper.BufferLength
                                        // we refilled the buffer, read all the 'a' bytes; the boundary and 100 'b' bytes left
                                        ? boundaryLength(isEndBoundary) + 100
                                        // we did not refill the buffer
                                        : BatchReaderStreamBufferWrapper.BufferLength - bytesToRead,
                        }
                    };
                    testCases.Add(testCase);
                });

            return testCases;
        }

        private IEnumerable<ReadWithDelimiterTestCase> CreatePartialBoundaryAtStreamEndTestCases()
        {
            List<ReadWithDelimiterTestCase> testCases = new List<ReadWithDelimiterTestCase>();
            this.CombinatorialEngineProvider.RunCombinations(
                BoundaryStrings,
                LeadingLineFeedForBoundary,
                LineFeeds,
                (boundary, leadingBoundaryLineFeed, lineFeed) =>
                {
                    // Fill the buffer with 'a' bytes and leave some room for the line feed, 
                    // the two dashes and the first byte of the boundary
                    int leadingLineFeedLength = leadingBoundaryLineFeed ? lineFeed.Length : 0;
                    int partialBoundaryLength = leadingLineFeedLength + BatchReaderStreamTestUtils.TwoDashesLength + 1;
                    int bytesBeforePartialBoundary = BatchReaderStreamBufferWrapper.BufferLength - partialBoundaryLength;

                    this.CombinatorialEngineProvider.RunCombinations(
                        new int[] { 0, 100, bytesBeforePartialBoundary, BatchReaderStreamBufferWrapper.BufferLength, BatchReaderStreamBufferWrapper.BufferLength + 100 },
                        (bytesToRead) =>
                        {
                            ReadWithDelimiterTestCase testCase = new ReadWithDelimiterTestCase(lineFeed)
                            {
                                DebugDescription = "Read " + bytesToRead + " bytes from last buffer of stream with partial delimiter at the end.\r\n" +
                                    "[leadingLineFeed = " + leadingBoundaryLineFeed + ", lineFeed = " + StringUtils.LineFeedString(lineFeed) +
                                    ", bytesToRead = " + bytesToRead + "]",
                                BatchBoundary = boundary,
                                PayloadFunc = builder => leadingBoundaryLineFeed
                                    ? builder
                                        .FillBytes(bytesBeforePartialBoundary, 'a')
                                        .LineFeed()
                                        .Chars(MemoryStreamBatchPayloadBuilder.BoundaryDelimiter)
                                        .Chars(boundary[0])
                                        .ResetMemoryStream()
                                    : builder
                                        .FillBytes(bytesBeforePartialBoundary, 'a')
                                        .Chars(MemoryStreamBatchPayloadBuilder.BoundaryDelimiter)
                                        .Chars(boundary[0])
                                        .ResetMemoryStream(),
                                ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                            {
                                new BatchReaderStreamReadDescriptor 
                                { 
                                    BufferSize = BatchReaderStreamBufferWrapper.BufferLength, 
                                    BufferOffset = 0, 
                                    NumberOfBytesToRead = bytesToRead, 
                                    ExpectedNumberOfBytesRead = Math.Min(bytesToRead, BatchReaderStreamBufferWrapper.BufferLength)
                                }
                            },
                                ExpectedBufferState = new BatchReaderStreamBufferState
                                {
                                    ReadPositionInBuffer = bytesToRead < BatchReaderStreamBufferWrapper.BufferLength
                                        // We did not reload the buffer
                                        ? bytesToRead
                                        // We reloaded the buffer and read the remaining partial boundary
                                        : partialBoundaryLength,
                                    NumberOfBytesInBuffer = bytesToRead == 0
                                        // nothing was loaded into the buffer
                                            ? 0
                                            : bytesToRead < BatchReaderStreamBufferWrapper.BufferLength
                                        // We did not reload the buffer
                                                ? BatchReaderStreamBufferWrapper.BufferLength - bytesToRead
                                        // We reloaded the buffer and read all of it
                                                : 0
                                }
                            };
                            testCases.Add(testCase);
                        });
                });

            return testCases;
        }

        private IEnumerable<ReadWithDelimiterTestCase> CreateErrorTestCases()
        {
            List<ReadWithDelimiterTestCase> testCases = new List<ReadWithDelimiterTestCase>();
            this.CombinatorialEngineProvider.RunCombinations(
                LineFeeds,
                (lineFeed) =>
                {
                    testCases.Add(new ReadWithDelimiterTestCase(lineFeed)
                    {
                        DebugDescription = "Read more than fits into a single buffer into a too small target buffer.",
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'a')
                            .FillBytes(100, 'b')
                            .ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = BatchReaderStreamBufferWrapper.BufferLength + 100 - 1,
                                BufferOffset = 0,
                                NumberOfBytesToRead = BatchReaderStreamBufferWrapper.BufferLength + 100,
                                ExpectedNumberOfBytesRead = BatchReaderStreamBufferWrapper.BufferLength + 100,
                            }
                        },
                        ExpectedException = new ExpectedException(typeof(ArgumentException))
                    });

                    testCases.Add(new ReadWithDelimiterTestCase(lineFeed)
                    {
                        DebugDescription = "Read too much data into a buffer with a too large offset.",
                        PayloadFunc = builder => builder
                            .FillBytes(100, 'a')
                            .ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = 200,
                                BufferOffset = 150,
                                NumberOfBytesToRead = 100,
                                ExpectedNumberOfBytesRead = 100,
                            }
                        },
                        ExpectedException = new ExpectedException(typeof(ArgumentException))
                    });
                });

            return testCases;
        }

        /// <summary>
        /// Test case class to test the ReadWithDelimiter method.
        /// </summary>
        private class ReadWithDelimiterTestCase : BatchReaderStreamReadWithLengthTests.ReadWithLengthTestCase
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="lineFeedChars">The line feed characters to use in the test.</param>
            public ReadWithDelimiterTestCase(char[] lineFeedChars = null)
                : this(null, null, lineFeedChars)
            {
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="batchBoundary">The boundary string to be used for the batch request; or null if a default boundary should be used.</param>
            /// <param name="encoding">The encoding to use to convert bytes to and from strings.</param>
            /// <param name="lineFeedChars">The line feed characters to use in the test.</param>
            public ReadWithDelimiterTestCase(string batchBoundary = null, Encoding encoding = null, char[] lineFeedChars = null)
                : base(batchBoundary, encoding, lineFeedChars)
            {
            }

            /// <summary>
            /// Copy constructor.
            /// </summary>
            /// <param name="other">The test case instance to copy.</param>
            public ReadWithDelimiterTestCase(ReadWithDelimiterTestCase other)
                : base(other)
            {
            }

            /// <summary>
            /// Runs the actual test action against the batch reader stream.
            /// </summary>
            /// <param name="streamWrapper">The batch reader stream wrapper to execute the test action against.</param>
            protected override void RunTestAction(BatchReaderStreamWrapper streamWrapper)
            {
                this.Assert.IsNotNull(this.ReadDescriptors, "Must have non-null read descriptors.");

                // Set a batch encoding since we assert that it is set before callign ProcessPartHeader
                streamWrapper.SetBatchEncoding(this.Encoding);

                foreach (BatchReaderStreamReadDescriptor readDescriptor in this.ReadDescriptors)
                {
                    byte[] buffer = new byte[readDescriptor.BufferSize];
                    int actualBytesRead = streamWrapper.ReadWithDelimiter(buffer, readDescriptor.BufferOffset, readDescriptor.NumberOfBytesToRead);
                    base.VerifyResult(readDescriptor, buffer, actualBytesRead);
                }

                base.VerifyResult(streamWrapper.BatchBuffer);
            }
        }
    }
}
