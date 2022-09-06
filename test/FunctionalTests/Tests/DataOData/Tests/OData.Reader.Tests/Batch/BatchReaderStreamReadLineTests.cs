//---------------------------------------------------------------------
// <copyright file="BatchReaderStreamReadLineTests.cs" company="Microsoft">
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
    /// Tests the ODataBatchReaderStream.ReadLine implementation.
    /// </summary>
    [TestClass, TestCase]
    public class BatchReaderStreamReadLineTests : ODataReaderTestCase
    {
        /// <summary>
        /// Gets or sets the dependency injector.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDependencyInjector Injector { get; set; }

        // Batch stream buffer tests use private reflection and thus cannot run on Silverlight or the phone.
        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Testing the ODataBatchReaderStream.RefillFrom method.")]
        public void BatchReaderStreamReadLineTest()
        {
            IEnumerable<char[]> lineFeeds = new char[][] { BatchReaderStreamTestUtils.DefaultLineFeedChars, new char[] { '\r' }, new char[] { '\n' } };
            IEnumerable<ReadLineTestCase> testCases = lineFeeds.SelectMany(
                lineFeed => new ReadLineTestCase[]
                {
                    new ReadLineTestCase(lineFeed)
                    {
                        DebugDescription = "Read from empty buffer.",
                        PayloadFunc = builder => builder.ResetMemoryStream(),
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                            NumberOfBytesInBuffer = 0,
                        }
                    },
                    new ReadLineTestCase(lineFeed)
                    {
                        DebugDescription = "Read an empty line (fits completely in the buffer).",
                        PayloadFunc = builder => builder.LineFeed().ResetMemoryStream(),
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = lineFeed.Length,
                            NumberOfBytesInBuffer = 0,
                        }
                    },
                    new ReadLineTestCase(lineFeed)
                    {
                        DebugDescription = "Read multiple empty lines (but one less than in the buffer).",
                        ReadIterationCount = 4,
                        PayloadFunc = builder => builder
                            .LineFeed()
                            .LineFeed()
                            .LineFeed()
                            .LineFeed()
                            .LineFeed()
                            .ResetMemoryStream(),
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = lineFeed.Length * 4,
                            NumberOfBytesInBuffer = lineFeed.Length,
                        }
                    },
                    new ReadLineTestCase(lineFeed)
                    {
                        DebugDescription = "Read a non-empty line that fits completely into the buffer.",
                        PayloadFunc = builder => builder.String("Non-empty line").LineFeed().ResetMemoryStream(),
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = BatchReaderStreamTestUtils.IsSingleCarriageReturn(lineFeed) 
                                ? 1
                                : "Non-empty line".Length + lineFeed.Length,
                            NumberOfBytesInBuffer = 0,
                        }
                    },
                    new ReadLineTestCase(lineFeed)
                    {
                        DebugDescription = "Read a non-empty line that aligns exactly with the end of the buffer; '\\r' will return a partial match, '\\n' and '\\r\\n' a full match.",
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length, 'a')
                            .LineFeed()
                            .ResetMemoryStream(),
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            // For a single '\r' we only report a partial match and refill the buffer; turns out that there is no
                            // more data so the current read position is '1' (where '\r' is at position 0). In all other cases
                            // we detect the line feed at the end of the buffer and do not refill it, so the read position is beyond
                            // the last byte in the buffer.
                            ReadPositionInBuffer = BatchReaderStreamTestUtils.IsSingleCarriageReturn(lineFeed) ? 1 : BatchReaderStreamBufferWrapper.BufferLength,
                            NumberOfBytesInBuffer = 0,
                        }
                    },
                    new ReadLineTestCase(lineFeed)
                    {
                        DebugDescription = "First, read a non-empty line that aligns exactly with the end of the buffer." +
                            "Second, read another small line that fits into the second buffer.",
                        ReadIterationCount = 2,
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - lineFeed.Length, 'a')
                            .LineFeed()
                            .FillBytes(10, 'b')
                            .LineFeed()
                            .ResetMemoryStream(),
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            // For a single '\r' we first report a partial match and refill the buffer carrying over the '\r';
                            // After reading the next line, we should be positioned after the second line feed.
                            ReadPositionInBuffer = BatchReaderStreamTestUtils.IsSingleCarriageReturn(lineFeed) ? 1 : 10 + lineFeed.Length,
                            NumberOfBytesInBuffer = 0,
                        }
                    },
                    new ReadLineTestCase(lineFeed)
                    {
                        DebugDescription = "Read a non-empty line that exceeds the initial buffer.",
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength + 100, 'a')
                            .LineFeed()
                            .ResetMemoryStream(),
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = BatchReaderStreamTestUtils.IsSingleCarriageReturn(lineFeed)
                                ? 1
                                : lineFeed.Length == 1 
                                    ? 101 
                                    : 102,
                            NumberOfBytesInBuffer = 0,
                        }
                    },
                    new ReadLineTestCase(lineFeed)
                    {
                        DebugDescription = "Read two non-empty lines that exceed two buffers.",
                        ReadIterationCount = 2,
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength + 100, 'a')
                            .LineFeed()
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength + 100, 'b')
                            .LineFeed()
                            .ResetMemoryStream(),
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            // Expect the position in the third buffer to be 2 * 100 (for the overlapping 100 'a's and 'b's)
                            // and then either 2 or 4 characters for the line feeds.
                            ReadPositionInBuffer = BatchReaderStreamTestUtils.IsSingleCarriageReturn(lineFeed)
                                ? 1
                                : lineFeed.Length == 1 
                                    ? 202 
                                    : 204,
                            NumberOfBytesInBuffer = 0,
                        }
                    },
                    new ReadLineTestCase(lineFeed)
                    {
                        DebugDescription = "Read two non-empty lines that exceed two buffers with additional data in the buffer.",
                        ReadIterationCount = 2,
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength + 100, 'a')
                            .LineFeed()
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength + 100, 'b')
                            .LineFeed()
                            .FillBytes(100, 'c')
                            .LineFeed()
                            .ResetMemoryStream(),
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            // Expect the position in the third buffer to be 2 * 100 (for the overlapping 100 'a's and 'b's)
                            // and then either 2 or 4 characters for the line feeds.
                            ReadPositionInBuffer = lineFeed.Length == 1 ? 202 : 204,
                            // The remaining 'c's in the buffer and a line feed
                            NumberOfBytesInBuffer = 100 + lineFeed.Length,
                        }
                    },
                    new ReadLineTestCase(lineFeed)
                    {
                        DebugDescription = "Read a non-empty line that has no terminating line feed.",
                        PayloadFunc = builder => builder
                            .FillBytes(100, 'a')
                            .ResetMemoryStream(),
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                            NumberOfBytesInBuffer = 0,
                        }
                    },
                    new ReadLineTestCase(lineFeed)
                    {
                        DebugDescription = "Read a non-empty line that has no terminating line feed and spans multiple buffers.",
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength + 100, 'a')
                            .ResetMemoryStream(),
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                            NumberOfBytesInBuffer = 0,
                        }
                    },
                    new ReadLineTestCase(lineFeed)
                    {
                        DebugDescription = "Read beyond the end of the buffer/stream.",
                        ReadIterationCount = 2,
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength + 100, 'a')
                            .LineFeed()
                            .ResetMemoryStream(),
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = BatchReaderStreamTestUtils.IsSingleCarriageReturn(lineFeed)
                                ? 1
                                : 0,
                            NumberOfBytesInBuffer = 0,
                        }
                    },
                });

            // add a few manual test cases
            testCases = testCases.Concat(
                new ReadLineTestCase[]
                {
                    new ReadLineTestCase(BatchReaderStreamTestUtils.DefaultLineFeedChars)
                    {
                        DebugDescription = "Read a non-empty line where the line feed overlaps the buffer boundary.",
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength - 1, 'a')
                            .LineFeed()
                            .ResetMemoryStream(),
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 2,
                            NumberOfBytesInBuffer = 0,
                        }
                    },
                }
            );

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    this.Injector.InjectDependenciesInto(testCase);
                    testCase.Run();
                });
        }

        /// <summary>
        /// Test case class to test the ReadLine method.
        /// </summary>
        private sealed class ReadLineTestCase : BatchReaderStreamTestCase
        {
            private MemoryStream payloadStream;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="lineFeedChars">The line feed characters to use in the test.</param>
            public ReadLineTestCase(char[] lineFeedChars = null)
                : this(null, null, lineFeedChars)
            {
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="batchBoundary">The boundary string to be used for the batch request; or null if a default boundary should be used.</param>
            /// <param name="encoding">The encoding to use to convert bytes to and from strings.</param>
            /// <param name="lineFeedChars">The line feed characters to use in the test.</param>
            public ReadLineTestCase(string batchBoundary = null, Encoding encoding = null, char[] lineFeedChars = null)
                : base(batchBoundary, encoding, lineFeedChars)
            {
                this.ReadIterationCount = 1;
            }

            /// <summary>
            /// The number of times to read from the stream.
            /// </summary>
            public int ReadIterationCount { get; set; }

            /// <summary>
            /// A function to produce the <see cref="MemoryStream"/> holding the payload bytes
            /// given a payload builder.
            /// </summary>
            public override Func<MemoryStreamBatchPayloadBuilder, MemoryStream> PayloadFunc
            {
                get
                {
                    // wrap the payload function from the base to grab the memory stream after it has been created
                    return builder =>
                        {
                            return (this.payloadStream = base.PayloadFunc(builder));
                        };
                }
                set { base.PayloadFunc = value; }
            }

            /// <summary>
            /// Runs the actual test action against the batch reader stream.
            /// </summary>
            /// <param name="streamWrapper">The batch reader stream wrapper to execute the test action against.</param>
            protected override void RunTestAction(BatchReaderStreamWrapper streamWrapper)
            {
                this.Assert.IsTrue(this.ReadIterationCount > 0, "Must read at least once.");

                // Set a batch encoding since we assert that it is set before callign ProcessPartHeader
                streamWrapper.SetBatchEncoding(this.Encoding);

                string line = null;
                for (int i = 0; i < this.ReadIterationCount; ++i)
                {
                    line = streamWrapper.ReadLine();
                }

                this.VerifyResult(streamWrapper, line);
            }

            /// <summary>
            /// Verifies the result of executing the test action.
            /// </summary>
            /// <param name="streamWrapper">The batch reader stream wrapper to verify the result against.</param>
            /// <param name="line">The line read from the stream.</param>
            private void VerifyResult(BatchReaderStreamWrapper streamWrapper, string line)
            {
                this.Assert.IsTrue(this.ReadIterationCount > 0, "Must have read at least once.");

                base.VerifyResult(streamWrapper.BatchBuffer);

                this.Assert.IsNotNull(this.payloadStream, "Must have a payload stream.");
                this.payloadStream.Seek(0, SeekOrigin.Begin);
                byte[] bytes = new byte[this.payloadStream.Length];
                Buffer.BlockCopy(this.payloadStream.GetBuffer(), 0, bytes, 0, (int)this.payloadStream.Length);
                string expectedLine = this.Encoding.GetString(bytes);

                string lineFeed = new string(this.LineFeedChars);

                int lineFeedLength = this.LineFeedChars.Length;
                int startIndex = -lineFeedLength;
                int endIndex = -lineFeedLength;

                for (int i = 0; i < this.ReadIterationCount; ++i)
                {
                    startIndex = endIndex + lineFeedLength;
                    endIndex = expectedLine.IndexOf(lineFeed, startIndex);
                    if (endIndex < 0)
                    {
                        break;
                    }
                }

                startIndex = Math.Max(startIndex, 0);
                if (endIndex >= 0)
                {
                    expectedLine = expectedLine.Substring(startIndex, endIndex - startIndex);
                }
                else
                {
                    expectedLine = expectedLine.Substring(startIndex);
                    if (String.IsNullOrEmpty(expectedLine))
                    {
                        expectedLine = null;
                    }
                }

                this.Assert.AreEqual(expectedLine, line,
                    string.Format("\r\n{0}:\r\nExpected to read line '{1}' but reported value is '{2}'.", this.DebugDescription, expectedLine, line));
            }
        }
    }
}
