//---------------------------------------------------------------------
// <copyright file="BatchReaderStreamBufferTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests the buffer implementation used by the ODataMultipartMixedBatchReaderStream.
    /// </summary>
    [TestClass, TestCase]
    public class BatchReaderStreamBufferTests : ODataReaderTestCase
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
        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Testing the ODataBatchReaderStreamBuffer.RefillFrom method.")]
        public void BatchReaderStreamBufferRefillFromTest()
        {
            IEnumerable<RefillFromTestCase> testCases = new RefillFromTestCase[]
            {
                new RefillFromTestCase
                {
                    DebugDescription = "Fill the whole buffer from the stream; basic test case.",
                    PayloadFunc = builder => builder.FillBytes(BatchReaderStreamBufferWrapper.BufferLength).ResetMemoryStream(),
                    RequireEmptyBuffer = true,
                    PreserveFrom = BatchReaderStreamBufferWrapper.BufferLength,
                    ExpectedBufferState = new BatchReaderStreamBufferState
                    {
                        NumberOfBytesInBuffer = BatchReaderStreamBufferWrapper.BufferLength,
                        ReadPositionInBuffer = 0,
                        ExpectedBytesInBuffer = new KeyValuePair<int, byte>[]
                        {
                            new KeyValuePair<int, byte>(0, 0),
                            new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 1, (byte)((BatchReaderStreamBufferWrapper.BufferLength - 1) % byte.MaxValue)),
                        }
                    }
                },
                new RefillFromTestCase
                {
                    DebugDescription = "Fill the buffer in setup; then refill the buffer from the stream",
                    PayloadFunc = builder => builder.FillBytes(BatchReaderStreamBufferWrapper.BufferLength * 2).ResetMemoryStream(),
                    PreserveFrom = BatchReaderStreamBufferWrapper.BufferLength,
                    ExpectedBufferState = new BatchReaderStreamBufferState
                    {
                        NumberOfBytesInBuffer = BatchReaderStreamBufferWrapper.BufferLength,
                        ReadPositionInBuffer = 0,
                        ExpectedBytesInBuffer = new KeyValuePair<int, byte>[]
                        {
                            new KeyValuePair<int, byte>(0, (byte)(BatchReaderStreamBufferWrapper.BufferLength % byte.MaxValue)),
                            new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 1, (byte)((BatchReaderStreamBufferWrapper.BufferLength * 2 - 1) % byte.MaxValue)),
                        }
                    }
                },
                new RefillFromTestCase
                {
                    DebugDescription = "Fill the buffer in setup; then refill the buffer from the stream but keep 10 bytes",
                    PayloadFunc = builder => builder.FillBytes(BatchReaderStreamBufferWrapper.BufferLength * 2).ResetMemoryStream(),
                    PreserveFrom = BatchReaderStreamBufferWrapper.BufferLength - 10,
                    ExpectedBufferState = new BatchReaderStreamBufferState
                    {
                        NumberOfBytesInBuffer = BatchReaderStreamBufferWrapper.BufferLength,
                        ReadPositionInBuffer = 0,
                        ExpectedBytesInBuffer = new KeyValuePair<int, byte>[]
                        {
                            new KeyValuePair<int, byte>(0, (byte)((BatchReaderStreamBufferWrapper.BufferLength - 10) % byte.MaxValue)),
                            new KeyValuePair<int, byte>(10, (byte)(BatchReaderStreamBufferWrapper.BufferLength % byte.MaxValue)),
                            new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 1, (byte)((BatchReaderStreamBufferWrapper.BufferLength * 2 - 11) % byte.MaxValue)),
                        }
                    }
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

        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Testing the ODataBatchReaderStreamBuffer.SkipTo method.")]
        public void BatchReaderStreamBufferSkipToTest()
        {
            IEnumerable<SkipToTestCase> testCases = new SkipToTestCase[]
            {
                new SkipToTestCase
                {
                    DebugDescription = "Skip to the beginning of the buffer",
                    SkipTo = 0,
                    ExpectedBufferState = new BatchReaderStreamBufferState
                    {
                        NumberOfBytesInBuffer = BatchReaderStreamBufferWrapper.BufferLength,
                        ReadPositionInBuffer = 0,
                    }
                },
                new SkipToTestCase
                {
                    DebugDescription = "Skip to the last byte in the buffer",
                    SkipTo = BatchReaderStreamBufferWrapper.BufferLength - 1,
                    ExpectedBufferState = new BatchReaderStreamBufferState
                    {
                        NumberOfBytesInBuffer = 1,
                        ReadPositionInBuffer = BatchReaderStreamBufferWrapper.BufferLength - 1,
                    }
                },
                new SkipToTestCase
                {
                    DebugDescription = "Skip to byte number 20",
                    SkipTo = 20,
                    ExpectedBufferState = new BatchReaderStreamBufferState
                    {
                        NumberOfBytesInBuffer = BatchReaderStreamBufferWrapper.BufferLength - 20,
                        ReadPositionInBuffer = 20,
                    }
                },
                new SkipToTestCase
                {
                    DebugDescription = "Fill the buffer with 30 bytes; skip to byte number 20",
                    PayloadFunc = builder => builder.FillBytes(30).ResetMemoryStream(),
                    SkipTo = 20,
                    ExpectedBufferState = new BatchReaderStreamBufferState
                    {
                        NumberOfBytesInBuffer = 10,
                        ReadPositionInBuffer = 20,
                    }
                },
                new SkipToTestCase
                {
                    DebugDescription = "Fill the buffer with 30 bytes; skip to byte number 30",
                    PayloadFunc = builder => builder.FillBytes(30).ResetMemoryStream(),
                    SkipTo = 30,
                    ExpectedBufferState = new BatchReaderStreamBufferState
                    {
                        NumberOfBytesInBuffer = 0,
                        ReadPositionInBuffer = 30,
                    }
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

        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Testing the ODataBatchReaderStreamBuffer.ScanForLineEnd method.")]
        public void BatchReaderStreamBufferScanForLineEndTest()
        {
            IEnumerable<ScanForLineEndTestCase> testCases = LineFeeds.SelectMany(
                lineFeed => new ScanForLineEndTestCase[]
                    {
                        new ScanForLineEndTestCase(lineFeed)
                        {
                            DebugDescription = "Line feed at the beginning of the buffer (no more bytes following). '\\r' will return a partial match, '\\n' and '\\r\\n' a full match.",
                            PayloadFunc = builder => builder.LineFeed().ResetMemoryStream(),
                            ExpectedScanResult = IsSingleCarriageReturn(lineFeed)
                                ? BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.PartialMatch
                                : BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.Match,
                            ExpectedStartPosition = 0,
                            ExpectedEndPosition = IsSingleCarriageReturn(lineFeed) ? -1 : lineFeed.Length - 1,
                            ExpectedBufferState = new BatchReaderStreamBufferState
                            {
                                ReadPositionInBuffer = 0,
                            }
                        },
                        new ScanForLineEndTestCase(lineFeed)
                        {
                            DebugDescription = "Line feed at the beginning of the buffer (with more bytes following)",
                            PayloadFunc = builder => builder.LineFeed().FillBytes(10).ResetMemoryStream(),
                            ExpectedScanResult = BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.Match,
                            ExpectedStartPosition = 0,
                            ExpectedEndPosition = lineFeed.Length - 1,
                            ExpectedBufferState = new BatchReaderStreamBufferState
                            {
                                ReadPositionInBuffer = 0,
                            }
                        },
                        new ScanForLineEndTestCase(lineFeed)
                        {
                            DebugDescription = "Line feed at the end of the buffer; '\\r' will return a partial match, '\\n' and '\\r\\n' a full match.",
                            PayloadFunc = builder => builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length, 'a')
                                .LineFeed()
                                .ResetMemoryStream(),
                            ExpectedScanResult = IsSingleCarriageReturn(lineFeed)
                                ? BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.PartialMatch
                                : BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.Match,
                            ExpectedStartPosition = BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length,
                            ExpectedEndPosition = IsSingleCarriageReturn(lineFeed) ? -1 : BatchReaderStreamBufferWrapper.BufferLength - 1,
                            ExpectedBufferState = new BatchReaderStreamBufferState
                            {
                                ReadPositionInBuffer = 0,
                            }
                        },
                        new ScanForLineEndTestCase(lineFeed)
                        {
                            DebugDescription = "Line feed at position 10 in the buffer",
                            PayloadFunc = builder => builder.FillBytes(10, 'a').LineFeed().FillBytes(10, 'b').ResetMemoryStream(),
                            ExpectedScanResult = BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.Match,
                            ExpectedStartPosition = 10,
                            ExpectedEndPosition = 10 + lineFeed.Length - 1,
                            ExpectedBufferState = new BatchReaderStreamBufferState
                            {
                                ReadPositionInBuffer = 0,
                            }
                        },
                    });

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    this.Injector.InjectDependenciesInto(testCase);
                    testCase.Run();
                });
        }

        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Testing the ODataBatchReaderStreamBuffer.ScanForBoundary method.")]
        public void BatchReaderStreamBufferScanForBoundaryTest()
        {
            IEnumerable<ScanForBoundaryTestCase> testCases =
                    this.CreateBoundaryAtTheBeginningTestCases()
                    .Concat(this.CreateBoundaryInTheMiddleTestCases())
                    .Concat(this.CreateBoundaryAtTheEndTestCases())
                    .Concat(this.CreatePartialBoundaryTestCases())
                    .Concat(this.CreateMissingBoundaryTestCases());

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    this.Injector.InjectDependenciesInto(testCase);
                    testCase.Run();
                });
        }

        private IEnumerable<ScanForBoundaryTestCase> CreateBoundaryAtTheBeginningTestCases()
        {
            List<ScanForBoundaryTestCase> testCases = new List<ScanForBoundaryTestCase>();
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
                    Func<bool, int> boundaryLength = isEnd => isEnd ? endBoundaryLength : startBoundaryLength;

                    ScanForBoundaryTestCase testCase = new ScanForBoundaryTestCase(lineFeed)
                    {
                        DebugDescription = "Boundary '" + boundaryString + "' at the beginning [isEnd = " + isEndBoundary +
                            ", lineFeed = " + StringUtils.LineFeedString(lineFeed) + ", leadingLineFeed = " + leadingBoundaryLineFeed +
                            ", whitespaceCount = " + whitespaceCount + "].",
                        Boundary = boundaryString,
                        PayloadFunc = builder => isEndBoundary
                            ? builder
                                .EndBoundary(boundaryString, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'a')
                                .ResetMemoryStream()
                            : builder
                                .StartBoundary(boundaryString, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'a')
                                .ResetMemoryStream(),
                        ExpectedScanResult = BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.Match,
                        ExpectedStartPosition = 0,
                        ExpectedEndPosition = boundaryLength(isEndBoundary) - 1,
                        ExpectedIsEndBoundary = isEndBoundary,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0
                        }
                    };
                    testCases.Add(testCase);
                });
            return testCases;
        }

        private IEnumerable<ScanForBoundaryTestCase> CreateBoundaryInTheMiddleTestCases()
        {
            List<ScanForBoundaryTestCase> testCases = new List<ScanForBoundaryTestCase>();
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
                    Func<bool, int> boundaryLength = isEnd => isEnd ? endBoundaryLength : startBoundaryLength;

                    ScanForBoundaryTestCase testCase = new ScanForBoundaryTestCase(lineFeed)
                    {
                        DebugDescription = "Boundary '" + boundaryString + "' in the middle [isEnd = " + isEndBoundary +
                            ", lineFeed = " + StringUtils.LineFeedString(lineFeed) + ", leadingLineFeed = " + leadingBoundaryLineFeed +
                            ", whitespaceCount = " + whitespaceCount + "].",
                        Boundary = boundaryString,
                        PayloadFunc = builder => isEndBoundary
                            ? builder
                                .FillBytes(10, 'a')
                                .EndBoundary(boundaryString, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'b')
                                .ResetMemoryStream()
                            : builder
                                .FillBytes(10, 'a')
                                .StartBoundary(boundaryString, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'b')
                                .ResetMemoryStream(),
                        ExpectedScanResult = BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.Match,
                        ExpectedStartPosition = 10,
                        ExpectedEndPosition = 10 + boundaryLength(isEndBoundary) - 1,
                        ExpectedIsEndBoundary = isEndBoundary,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0
                        }
                    };
                    testCases.Add(testCase);
                });
            return testCases;
        }

        private IEnumerable<ScanForBoundaryTestCase> CreateBoundaryAtTheEndTestCases()
        {
            // NOTE: hand-crafted test cases with different alignments at the end of the buffer.
            List<ScanForBoundaryTestCase> testCases = new List<ScanForBoundaryTestCase>();
            this.CombinatorialEngineProvider.RunCombinations(
                BoundaryStrings,
                LineFeeds,
                (boundaryString, lineFeed) =>
                {
                    testCases.Add(new ScanForBoundaryTestCase(lineFeed)
                    {
                        DebugDescription = "Start boundary without terminating line feed at the very end; " +
                                            "expect partial matches since we did not find a terminating line feed.",
                        Boundary = boundaryString,
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength, 'a')
                            .LineFeed()
                            .Chars(MemoryStreamBatchPayloadBuilder.BoundaryDelimiter)
                            .String(boundaryString)
                            .ResetMemoryStream(),
                        ExpectedScanResult = BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.PartialMatch,
                        ExpectedStartPosition = BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length - BatchReaderStreamTestUtils.TwoDashesLength - boundaryString.Length,
                        ExpectedEndPosition = -1,
                        ExpectedIsEndBoundary = false,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                            ExpectedBytesInBuffer = new KeyValuePair<int, byte>[]
                            {
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength, (byte)'-'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength + 1, (byte)'-'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength + 2, (byte)boundaryString[0]),
                            },
                        }
                    });

                    testCases.Add(new ScanForBoundaryTestCase(lineFeed)
                    {
                        DebugDescription = "Start boundary at the end of the buffer including a terminating line feed; " +
                                            "expect partial matches since the algorithm for a full match would require " +
                                            "enough bytes for the end (!) boundary in the buffer plus the number of bytes " +
                                            "of the longest line feed char sequence.",
                        Boundary = boundaryString,
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength - lineFeed.Length, 'a')
                            .StartBoundary(boundaryString)
                            .ResetMemoryStream(),
                        ExpectedScanResult = BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.PartialMatch,
                        ExpectedStartPosition = BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength - lineFeed.Length,
                        ExpectedEndPosition = -1,
                        ExpectedIsEndBoundary = false,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                            ExpectedBytesInBuffer = new KeyValuePair<int, byte>[]
                            {
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength, (byte)'-'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength + 1, (byte)'-'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength + 2, (byte)boundaryString[0]),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 1, (byte)lineFeed[lineFeed.Length - 1]),
                            },
                        }
                    });

                    testCases.Add(new ScanForBoundaryTestCase(lineFeed)
                    {
                        DebugDescription = "Start boundary at the end of the buffer including a terminating line feed, extra line feed before the boundary; " +
                                            "expect partial matches since the algorithm for a full match would require " +
                                            "enough bytes for the end (!) boundary in the buffer plus the number of bytes " +
                                            "of the longest line feed char sequence.",
                        Boundary = boundaryString,
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength - lineFeed.Length - lineFeed.Length, 'a')
                            .LineFeed()
                            .StartBoundary(boundaryString)
                            .ResetMemoryStream(),
                        ExpectedScanResult = BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.PartialMatch,
                        ExpectedStartPosition = BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength - lineFeed.Length,
                        ExpectedEndPosition = -1,
                        ExpectedIsEndBoundary = false,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                            ExpectedBytesInBuffer = new KeyValuePair<int, byte>[]
                            {
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength - lineFeed.Length, (byte)lineFeed[0]),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength, (byte)'-'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength + 1, (byte)'-'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength + 2, (byte)boundaryString[0]),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 1, (byte)lineFeed[lineFeed.Length - 1]),
                            },
                        }
                    });

                    testCases.Add(new ScanForBoundaryTestCase(lineFeed)
                    {
                        DebugDescription = "Start boundary at the end of the buffer including a terminating line feed " +
                                            "and three extra bytes afterwards (to satisfy the requirement that the trailing dashes " +
                                            "of an end boundary would fit into the buffer); expect full matches always.",
                        Boundary = boundaryString,
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - 3 - lineFeed.Length - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength - lineFeed.Length, 'a')
                            .StartBoundary(boundaryString)
                            .FillBytes(3, 'b')
                            .ResetMemoryStream(),
                        ExpectedScanResult = BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.Match,
                        ExpectedStartPosition = BatchReaderStreamBufferWrapper.BufferLength - 3 - lineFeed.Length - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength - lineFeed.Length,
                        ExpectedEndPosition = BatchReaderStreamBufferWrapper.BufferLength - 3 - 1,
                        ExpectedIsEndBoundary = false,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                            ExpectedBytesInBuffer = new KeyValuePair<int, byte>[]
                            {
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 3 - lineFeed.Length - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength, (byte)'-'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 3 - lineFeed.Length - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength + 1, (byte)'-'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 3 - lineFeed.Length - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength + 2, (byte)boundaryString[0]),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 3 - 1, (byte)lineFeed[lineFeed.Length - 1]),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 3, (byte)'b'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 2, (byte)'b'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 1, (byte)'b'),
                            },
                        }
                    });

                    testCases.Add(new ScanForBoundaryTestCase(lineFeed)
                    {
                        DebugDescription = "End boundary without terminating line feed at the very end; " +
                                            "expect partial matches since we did not find a terminating line feed",
                        Boundary = boundaryString,
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - BatchReaderStreamTestUtils.TwoDashesLength - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength - lineFeed.Length, 'a')
                            .EndBoundary(boundaryString)
                            .ResetMemoryStream(),
                        ExpectedScanResult = BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.PartialMatch,
                        ExpectedStartPosition = BatchReaderStreamBufferWrapper.BufferLength - BatchReaderStreamTestUtils.TwoDashesLength - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength - lineFeed.Length,
                        ExpectedEndPosition = -1,
                        ExpectedIsEndBoundary = false,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                            ExpectedBytesInBuffer = new KeyValuePair<int, byte>[]
                            {
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - BatchReaderStreamTestUtils.TwoDashesLength - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength, (byte)'-'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - BatchReaderStreamTestUtils.TwoDashesLength - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength + 1, (byte)'-'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - BatchReaderStreamTestUtils.TwoDashesLength - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength + 2, (byte)boundaryString[0]),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 2, (byte)'-'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 1, (byte)'-'),
                            },
                        }
                    });

                    testCases.Add(new ScanForBoundaryTestCase(lineFeed)
                    {
                        DebugDescription = "End boundary at the end of the buffer including a terminating line feed; " +
                                            "expect full match or partial match based on the line feed characters." +
                                            "If we use a single lineFeed char expect a partial match because the " +
                                            "algorithm for full matches requires the longest lineFeed char sequence " +
                                            "to fit into the buffer.",
                        Boundary = boundaryString,
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length - BatchReaderStreamTestUtils.TwoDashesLength - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength - lineFeed.Length, 'a')
                            .EndBoundary(boundaryString)
                            .ResetMemoryStream(),
                        ExpectedScanResult = lineFeed.Length == 1
                            ? BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.PartialMatch
                            : BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.Match,
                        ExpectedStartPosition = BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length - BatchReaderStreamTestUtils.TwoDashesLength - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength - lineFeed.Length,
                        ExpectedEndPosition = lineFeed.Length == 1
                            ? -1
                            : BatchReaderStreamBufferWrapper.BufferLength - 1,
                        ExpectedIsEndBoundary = lineFeed.Length == 1 ? false : true,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                            ExpectedBytesInBuffer = new KeyValuePair<int, byte>[]
                            {
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length - BatchReaderStreamTestUtils.TwoDashesLength - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength, (byte)'-'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length - BatchReaderStreamTestUtils.TwoDashesLength -  boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength + 1, (byte)'-'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length - BatchReaderStreamTestUtils.TwoDashesLength - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength + 2, (byte)boundaryString[0]),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 1, (byte)lineFeed[lineFeed.Length - 1]),
                            },
                        }
                    });

                    testCases.Add(new ScanForBoundaryTestCase(lineFeed)
                    {
                        DebugDescription = "End boundary at the end of the buffer including a terminating line feed and " +
                                            "one extra byte afterwards; expect full matches always since we always have " +
                                            "the end boundary, the terminating line feed and one extra byte in the buffer. " +
                                            "That's sufficient for the algorithm to always produce a full match.",
                        Boundary = boundaryString,
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - 1 - lineFeed.Length - BatchReaderStreamTestUtils.TwoDashesLength - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength - lineFeed.Length, 'a')
                            .EndBoundary(boundaryString)
                            .FillBytes(1, 'b')
                            .ResetMemoryStream(),
                        ExpectedScanResult = BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.Match,
                        ExpectedStartPosition = BatchReaderStreamBufferWrapper.BufferLength - 1 - lineFeed.Length - BatchReaderStreamTestUtils.TwoDashesLength - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength - lineFeed.Length,
                        ExpectedEndPosition = BatchReaderStreamBufferWrapper.BufferLength - 1 - 1,
                        ExpectedIsEndBoundary = true,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                            ExpectedBytesInBuffer = new KeyValuePair<int, byte>[]
                            {
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 1 - lineFeed.Length - BatchReaderStreamTestUtils.TwoDashesLength - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength, (byte)'-'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 1 - lineFeed.Length - BatchReaderStreamTestUtils.TwoDashesLength - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength + 1, (byte)'-'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 1 - lineFeed.Length - BatchReaderStreamTestUtils.TwoDashesLength - boundaryString.Length - BatchReaderStreamTestUtils.TwoDashesLength + 2, (byte)boundaryString[0]),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 1 - 1, (byte)lineFeed[lineFeed.Length - 1]),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - 1, (byte)'b'),
                            },
                        }
                    });
                });

            // Test cases where the boundary aligns exactly with the buffer end
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
                    Func<bool, int> boundaryLength = isEnd => isEnd ? endBoundaryLength : startBoundaryLength;

                    bool expectPartialMatch = isEndBoundary
                        // Can't detect whether this is a full line end or not or don't have enough bytes in the
                        // buffer for the max line feed as required by the algorithm
                            ? IsSingleCarriageReturn(lineFeed) || lineFeed.Length + whitespaceCount < /*maxLineFeedLength*/2
                            : IsSingleCarriageReturn(lineFeed) ||
                        // Don't have enough space in the buffer to read a potential end boundary delimiter ('--')
                              lineFeed.Length + whitespaceCount < /*maxLineFeedLength*/2 + /*endDelimiterLength*/BatchReaderStreamTestUtils.TwoDashesLength;

                    testCases.Add(new ScanForBoundaryTestCase(lineFeed)
                    {
                        DebugDescription = "Boundary '" + boundaryString + "' at the end [isEnd = " + isEndBoundary +
                            ", lineFeed = " + StringUtils.LineFeedString(lineFeed) + ", leadingLineFeed = " + leadingBoundaryLineFeed +
                            ", whitespaceCount = " + whitespaceCount + "].",
                        Boundary = boundaryString,
                        PayloadFunc = builder => isEndBoundary
                            ? builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - endBoundaryLength, 'a')
                                .EndBoundary(boundaryString, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'b')
                                .ResetMemoryStream()
                            : builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - startBoundaryLength, 'a')
                                .StartBoundary(boundaryString, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'b')
                                .ResetMemoryStream(),
                        ExpectedScanResult = expectPartialMatch
                                ? BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.PartialMatch
                                : BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.Match,
                        ExpectedStartPosition = BatchReaderStreamBufferWrapper.BufferLength - boundaryLength(isEndBoundary),
                        ExpectedEndPosition = expectPartialMatch
                            ? -1
                            : BatchReaderStreamBufferWrapper.BufferLength - 1,
                        ExpectedIsEndBoundary = expectPartialMatch ? false : isEndBoundary,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                            ExpectedBytesInBuffer = new KeyValuePair<int, byte>[]
                            {
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - boundaryLength(isEndBoundary) + leadingLineFeedLength, (byte)'-'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - boundaryLength(isEndBoundary) + leadingLineFeedLength+ 1, (byte)'-'),
                                new KeyValuePair<int, byte>(BatchReaderStreamBufferWrapper.BufferLength - boundaryLength(isEndBoundary) + leadingLineFeedLength+ 2, (byte)boundaryString[0]),
                            },
                        }
                    });

                    testCases.Add(new ScanForBoundaryTestCase(lineFeed)
                    {
                        DebugDescription = "Boundary '" + boundaryString + "' without terminating line feed at the end [isEnd = " + isEndBoundary +
                            ", lineFeed = " + StringUtils.LineFeedString(lineFeed) + ", leadingLineFeed = " + leadingBoundaryLineFeed +
                            ", whitespaceCount = " + whitespaceCount + "]. Expect partial match.",
                        Boundary = boundaryString,
                        PayloadFunc = builder => isEndBoundary
                            ? builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - boundaryLength(isEndBoundary) + lineFeed.Length, 'a')
                                .EndBoundary(boundaryString, leadingBoundaryLineFeed, whitespaceCount)
                                .ResetMemoryStream()
                            : builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - boundaryLength(isEndBoundary) + lineFeed.Length, 'a')
                                .StartBoundary(boundaryString, leadingBoundaryLineFeed, whitespaceCount)
                                .ResetMemoryStream(),
                        ExpectedScanResult = BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.PartialMatch,
                        ExpectedIsEndBoundary = false,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0
                        }
                    });
                });
            return testCases;
        }

        private IEnumerable<ScanForBoundaryTestCase> CreatePartialBoundaryTestCases()
        {
            List<ScanForBoundaryTestCase> testCases = new List<ScanForBoundaryTestCase>();

            // Partial boundaries at various positions in the buffer
            this.CombinatorialEngineProvider.RunCombinations(
                BoundaryStrings.Where(str => str.Length > 1),     // excluding '-' boundary string since partials would be the empty string which this test does not handle
                LeadingLineFeedForBoundary,
                WhitespaceCounts,
                LineFeeds,
                new bool[] { false, true },
                (boundaryString, leadingBoundaryLineFeed, whitespaceCount, lineFeed, isEndBoundary) =>
                {
                    string boundaryToWrite = boundaryString.Substring(0, boundaryString.Length - 1);
                    int leadingLineFeedLength = leadingBoundaryLineFeed ? lineFeed.Length : 0;
                    int partialStartBoundaryLength = leadingLineFeedLength + BatchReaderStreamTestUtils.TwoDashesLength + boundaryToWrite.Length + whitespaceCount + lineFeed.Length;
                    int partialEndBoundaryLength = partialStartBoundaryLength + BatchReaderStreamTestUtils.TwoDashesLength;
                    Func<bool, int> partialBoundaryLength = isEnd => isEnd ? partialEndBoundaryLength : partialStartBoundaryLength;

                    ScanForBoundaryTestCase testCase = new ScanForBoundaryTestCase(lineFeed)
                    {
                        DebugDescription = "Partial boundary '" + boundaryString + "' at the beginning [isEnd = " + isEndBoundary +
                            ", lineFeed = " + StringUtils.LineFeedString(lineFeed) + ", leadingLineFeed = " + leadingBoundaryLineFeed +
                            ", whitespaceCount = " + whitespaceCount + "].",
                        Boundary = boundaryString,
                        PayloadFunc = builder => isEndBoundary
                            ? builder
                                .EndBoundary(boundaryToWrite, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'b')
                                .ResetMemoryStream()
                            : builder
                                .StartBoundary(boundaryToWrite, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'b')
                                .ResetMemoryStream(),
                        ExpectedScanResult = BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.NoMatch,
                        ExpectedIsEndBoundary = false,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                        }
                    };
                    testCases.Add(testCase);

                    testCase = new ScanForBoundaryTestCase(lineFeed)
                    {
                        DebugDescription = "Partial boundary '" + boundaryString + "' in the middle [isEnd = " + isEndBoundary +
                            ", lineFeed = " + StringUtils.LineFeedString(lineFeed) + ", leadingLineFeed = " + leadingBoundaryLineFeed +
                            ", whitespaceCount = " + whitespaceCount + "].",
                        Boundary = boundaryString,
                        PayloadFunc = builder => isEndBoundary
                            ? builder
                                .FillBytes(10, 'a')
                                .EndBoundary(boundaryToWrite, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'b')
                                .ResetMemoryStream()
                            : builder
                                .FillBytes(10, 'a')
                                .StartBoundary(boundaryToWrite, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'b')
                                .ResetMemoryStream(),
                        ExpectedScanResult = BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.NoMatch,
                        ExpectedIsEndBoundary = false,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                        }
                    };
                    testCases.Add(testCase);

                    testCase = new ScanForBoundaryTestCase(lineFeed)
                    {
                        DebugDescription = "Partial boundary '" + boundaryString + "' at the end [isEnd = " + isEndBoundary +
                            ", lineFeed = " + StringUtils.LineFeedString(lineFeed) + ", leadingLineFeed = " + leadingBoundaryLineFeed +
                            ", whitespaceCount = " + whitespaceCount + "].",
                        Boundary = boundaryString,
                        PayloadFunc = builder => isEndBoundary
                            ? builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - partialBoundaryLength(isEndBoundary), 'a')
                                .EndBoundary(boundaryToWrite, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'b')
                                .ResetMemoryStream()
                            : builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - partialBoundaryLength(isEndBoundary), 'a')
                                .StartBoundary(boundaryToWrite, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'b')
                                .ResetMemoryStream(),
                        ExpectedScanResult = BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.PartialMatch,
                        ExpectedIsEndBoundary = false,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                        }
                    };
                    testCases.Add(testCase);

                });
            return testCases;
        }

        private IEnumerable<ScanForBoundaryTestCase> CreateMissingBoundaryTestCases()
        {
            // TODO: add error tests where none of the expected boundaries can be found
            List<ScanForBoundaryTestCase> testCases = new List<ScanForBoundaryTestCase>();
            this.CombinatorialEngineProvider.RunCombinations(
                LeadingLineFeedForBoundary,
                WhitespaceCounts,
                LineFeeds,
                new bool[] { false, true },
                (leadingBoundaryLineFeed, whitespaceCount, lineFeed, isEndBoundary) =>
                {
                    int leadingLineFeedLength = leadingBoundaryLineFeed ? lineFeed.Length : 0;
                    Func<string, int> startBoundaryLength = boundary => leadingLineFeedLength + BatchReaderStreamTestUtils.TwoDashesLength + boundary.Length + whitespaceCount + lineFeed.Length;
                    Func<string, int> endBoundaryLength = boundary => startBoundaryLength(boundary) + BatchReaderStreamTestUtils.TwoDashesLength;
                    Func<string, bool, int> boundaryLength = (boundary, isEnd) => isEnd ? endBoundaryLength(boundary) : startBoundaryLength(boundary);

                    string batchBoundary = "batch_boundary";
                    string changesetBoundary = "changeset_boundary";
                    string[] boundaries = new string[] { changesetBoundary, batchBoundary };

                    ScanForBoundaryTestCase testCase = new ScanForBoundaryTestCase(lineFeed)
                    {
                        DebugDescription = "Only parent boundary '" + batchBoundary + "' present [isEnd = " + isEndBoundary +
                            ", lineFeed = " + StringUtils.LineFeedString(lineFeed) + ", leadingLineFeed = " + leadingBoundaryLineFeed +
                            ", whitespaceCount = " + whitespaceCount + "].",
                        Boundaries = boundaries,
                        ExpectedIsParentBoundary = true,
                        PayloadFunc = builder => isEndBoundary
                            ? builder
                                .FillBytes(10, 'a')
                                .EndBoundary(batchBoundary, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'b')
                                .ResetMemoryStream()
                            : builder
                                .FillBytes(10, 'a')
                                .StartBoundary(batchBoundary, leadingBoundaryLineFeed, whitespaceCount)
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'b')
                                .ResetMemoryStream(),
                        ExpectedScanResult = BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.Match,
                        ExpectedStartPosition = 10,
                        ExpectedEndPosition = 10 + boundaryLength(batchBoundary, isEndBoundary) - 1,
                        ExpectedIsEndBoundary = isEndBoundary,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0
                        }
                    };
                    testCases.Add(testCase);

                    testCase = new ScanForBoundaryTestCase(lineFeed)
                    {
                        DebugDescription = "Parent boundary '" + batchBoundary + "' partially at the end of the buffer [isEnd = " + isEndBoundary +
                            ", lineFeed = " + StringUtils.LineFeedString(lineFeed) + ", leadingLineFeed = " + leadingBoundaryLineFeed +
                            ", whitespaceCount = " + whitespaceCount + "].",
                        Boundaries = boundaries,
                        PayloadFunc = builder => isEndBoundary
                            ? builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - 10, 'a')
                                .EndBoundary(batchBoundary, leadingBoundaryLineFeed, whitespaceCount)
                                .ResetMemoryStream()
                            : builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - 10, 'a')
                                .StartBoundary(batchBoundary, leadingBoundaryLineFeed, whitespaceCount)
                                .ResetMemoryStream(),
                        ExpectedScanResult = BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.PartialMatch,
                        ExpectedStartPosition = BatchReaderStreamBufferWrapper.BufferLength - 10,
                        ExpectedEndPosition = -1,
                        ExpectedIsEndBoundary = false,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0
                        }
                    };
                    testCases.Add(testCase);

                    testCase = new ScanForBoundaryTestCase(lineFeed)
                    {
                        DebugDescription = "Changeset boundary '" + batchBoundary + "' partially at the end of the buffer [isEnd = " + isEndBoundary +
                            ", lineFeed = " + StringUtils.LineFeedString(lineFeed) + ", leadingLineFeed = " + leadingBoundaryLineFeed +
                            ", whitespaceCount = " + whitespaceCount + "].",
                        Boundaries = boundaries,
                        PayloadFunc = builder => isEndBoundary
                            ? builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - 10, 'a')
                                .EndBoundary(changesetBoundary, leadingBoundaryLineFeed, whitespaceCount)
                                .EndBoundary(batchBoundary, leadingBoundaryLineFeed, whitespaceCount)
                                .ResetMemoryStream()
                            : builder
                                .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - 10, 'a')
                                .StartBoundary(changesetBoundary, leadingBoundaryLineFeed, whitespaceCount)
                                .StartBoundary(batchBoundary, leadingBoundaryLineFeed, whitespaceCount)
                                .ResetMemoryStream(),
                        ExpectedScanResult = BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult.PartialMatch,
                        ExpectedStartPosition = BatchReaderStreamBufferWrapper.BufferLength - 10,
                        ExpectedEndPosition = -1,
                        ExpectedIsEndBoundary = false,
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0
                        }
                    };
                    testCases.Add(testCase);
                });
            return testCases;
        }
#endif

        /// <summary>
        /// Check whether the line feed characters consist of a single '\r'.
        /// </summary>
        /// <param name="lineFeed">The line feed characters to check.</param>
        /// <returns>true if the <paramref name="lineFeed"/> is a single carriage return; otherwise false.</returns>
        private static bool IsSingleCarriageReturn(char[] lineFeed)
        {
            return lineFeed.Length == 1 && lineFeed[0] == '\r';
        }

        /// <summary>
        /// Test case class to test the RefillFrom method.
        /// </summary>
        private sealed class RefillFromTestCase : BatchReaderStreamBufferTestCase
        {
            /// <summary>The position to start preserving bytes from.</summary>
            public int PreserveFrom { get; set; }

            public bool RequireEmptyBuffer { get; set; }

            protected override void RunTestAction(BatchReaderStreamBufferWrapper streamBuffer, MemoryStream memoryStream)
            {
                if (!this.RequireEmptyBuffer)
                {
                    streamBuffer.RefillFrom(memoryStream, BatchReaderStreamBufferWrapper.BufferLength);
                }

                streamBuffer.RefillFrom(memoryStream, this.PreserveFrom);
                this.VerifyResult(streamBuffer);
            }
        }

        /// <summary>
        /// Test case class to test the SkipTo method.
        /// </summary>
        private sealed class SkipToTestCase : BatchReaderStreamBufferTestCase
        {
            /// <summary>The position to skip to.</summary>
            public int SkipTo { get; set; }

            /// <summary>
            /// Runs the test action of this test after setting up the input memory stream.
            /// </summary>
            /// <param name="streamBuffer">The batch reader stream buffer to test.</param>
            /// <param name="memoryStream">The memory stream with the input payload.</param>
            protected override void RunTestAction(BatchReaderStreamBufferWrapper streamBuffer, MemoryStream memoryStream)
            {
                // fill the buffer
                streamBuffer.RefillFrom(memoryStream, BatchReaderStreamBufferWrapper.BufferLength);

                streamBuffer.SkipTo(this.SkipTo);
                this.VerifyResult(streamBuffer);
            }
        }

        /// <summary>
        /// Test case class to test the ScanForLineEnd method.
        /// </summary>
        private class ScanForLineEndTestCase : BatchReaderStreamBufferTestCase
        {
            public ScanForLineEndTestCase(char[] lineFeedChars)
                : base(null, lineFeedChars)
            {
            }

            /// <summary>The expected result of scanning the buffer.</summary>
            public BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult ExpectedScanResult { get; set; }

            /// <summary>The expected start position of the string we searched for;
            /// or null to ignore during verification.</summary>
            public int? ExpectedStartPosition { get; set; }

            /// <summary>The expected end position of the string we searched for;
            /// or null to ignore during verification.</summary>
            public int? ExpectedEndPosition { get; set; }

            /// <summary>
            /// Verifies that the resulting stream buffer is in the expected state.
            /// </summary>
            /// <param name="assert">The assertion handler.</param>
            /// <param name="streamBuffer">The stream buffer whose state to verify.</param>
            /// <param name="scanResult">The result of scanning the buffer.</param>
            /// <param name="startPos">The start position of the string we searched for.</param>
            /// <param name="endPos">The end position of the string we searched for.</param>
            public void VerifyResult(BatchReaderStreamBufferWrapper streamBuffer, BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult scanResult, int startPos, int endPos)
            {
                base.VerifyResult(streamBuffer);

                this.Assert.AreEqual(this.ExpectedScanResult, scanResult,
                    string.Format("\r\n{0}:\r\nExpected scan result '{1}' does not match actual scan result '{2}'.", this.DebugDescription, this.ExpectedScanResult.ToString(), scanResult.ToString()));

                if (this.ExpectedStartPosition.HasValue)
                {
                    this.Assert.AreEqual(this.ExpectedStartPosition.Value, startPos,
                        string.Format("\r\n{0}:\r\nExpected start at position '{1}' but reported position is '{2}'.", this.DebugDescription, this.ExpectedStartPosition.Value, startPos));
                }

                if (this.ExpectedEndPosition.HasValue)
                {
                    this.Assert.AreEqual(this.ExpectedEndPosition.Value, endPos,
                        string.Format("\r\n{0}:\r\nExpected end at position '{1}' but reported position is '{2}'.", this.DebugDescription, this.ExpectedEndPosition.Value, endPos));
                }
            }

            /// <summary>
            /// Runs the test action of this test after setting up the input memory stream.
            /// </summary>
            /// <param name="streamBuffer">The batch reader stream buffer to test.</param>
            /// <param name="memoryStream">The memory stream with the input payload.</param>
            protected override void RunTestAction(BatchReaderStreamBufferWrapper streamBuffer, MemoryStream memoryStream)
            {
                // fill the buffer
                streamBuffer.RefillFrom(memoryStream, BatchReaderStreamBufferWrapper.BufferLength);

                int lineEndStart, lineEndEnd;
                var scanResult = streamBuffer.ScanForLineEnd(out lineEndStart, out lineEndEnd);
                this.VerifyResult(streamBuffer, scanResult, lineEndStart, lineEndEnd);
            }
        }

        /// <summary>
        /// Test case class to test the ScanForBoundary method.
        /// </summary>
        private class ScanForBoundaryTestCase : ScanForLineEndTestCase
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            public ScanForBoundaryTestCase(char[] lineFeedChars = null)
                : base(lineFeedChars)
            {
                this.MaxScanLength = BatchReaderStreamBufferWrapper.BufferLength;
            }

            /// <summary>The boundary string to scan for.</summary>
            public string Boundary
            {
                get
                {
                    this.Assert.IsTrue(this.Boundaries.Count() == 1, "Must have exactly 1 boundary for this property to work.");
                    return this.Boundaries.First();
                }
                set
                {
                    this.Boundaries = new string[] { value };
                }
            }

            /// <summary>
            /// The boundary strings to search for; this enumerable is sorted from the inner-most boundary
            /// to the top-most boundary. The boundary strings don't include the leading line terminator or the leading dashes.
            /// </summary>
            public IEnumerable<string> Boundaries { get; set; }

            /// <summary>The maximum number of bytes to scan.</summary>
            public int MaxScanLength { get; set; }

            /// <summary>true if we expected to detect an end boundary;
            /// otherwise false or null to ignore during verification.</summary>
            public bool? ExpectedIsEndBoundary { get; set; }

            /// <summary>true if we expected to detect a parent boundary;
            /// otherwise false or null to ignore during verification.</summary>
            public bool? ExpectedIsParentBoundary { get; set; }

            /// <summary>
            /// Verifies that the resulting stream buffer is in the expected state.
            /// </summary>
            /// <param name="assert">The assertion handler.</param>
            /// <param name="streamBuffer">The stream buffer whose state to verify.</param>
            /// <param name="scanResult">The result of scanning the buffer.</param>
            /// <param name="startPos">The start position of the string we searched for.</param>
            /// <param name="endPos">The end position of the string we searched for.</param>
            /// <param name="isEndBoundary">true if we detected an end boundary; otherwise false.</param>
            /// <param name="isParentBoundary">true if we detected a parent boundary; otherwise false.</param>
            public void VerifyResult(
                BatchReaderStreamBufferWrapper streamBuffer,
                BatchReaderStreamBufferWrapper.ODataBatchReaderStreamScanResult scanResult,
                int startPos,
                int endPos,
                bool isEndBoundary,
                bool isParentBoundary)
            {
                base.VerifyResult(streamBuffer, scanResult, startPos, endPos);

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
            }

            /// <summary>
            /// Runs the test action of this test after setting up the input memory stream.
            /// </summary>
            /// <param name="streamBuffer">The batch reader stream buffer to test.</param>
            /// <param name="memoryStream">The memory stream with the input payload.</param>
            protected override void RunTestAction(BatchReaderStreamBufferWrapper streamBuffer, MemoryStream memoryStream)
            {
                // fill the buffer
                streamBuffer.RefillFrom(memoryStream, BatchReaderStreamBufferWrapper.BufferLength);

                int boundaryStart, boundaryEnd;
                bool isEndBoundary, isParentBoundary;
                var scanResult = streamBuffer.ScanForBoundary(
                    this.Boundaries,
                    int.MaxValue,
                    out boundaryStart,
                    out boundaryEnd,
                    out isEndBoundary,
                    out isParentBoundary);
                this.VerifyResult(streamBuffer, scanResult, boundaryStart, boundaryEnd, isEndBoundary, isParentBoundary);
            }
        }
    }
}
