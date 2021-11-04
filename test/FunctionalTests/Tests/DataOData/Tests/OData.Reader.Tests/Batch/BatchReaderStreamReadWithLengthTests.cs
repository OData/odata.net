//---------------------------------------------------------------------
// <copyright file="BatchReaderStreamReadWithLengthTests.cs" company="Microsoft">
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
    /// Tests the ODataBatchReaderStream.ReadWithLength implementation.
    /// </summary>
    [TestClass, TestCase]
    public class BatchReaderStreamReadWithLengthTests : ODataReaderTestCase
    {
        /// <summary>
        /// Gets or sets the dependency injector.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDependencyInjector Injector { get; set; }

        // Batch stream buffer tests use private reflection and thus cannot run on Silverlight or the phone.
        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Testing the ODataBatchReaderStream.ReadWithLength method.")]
        public void BatchReaderStreamReadWithLengthTest()
        {
            IEnumerable<char[]> lineFeeds = new char[][] { BatchReaderStreamTestUtils.DefaultLineFeedChars, new char[] { '\r' }, new char[] { '\n' } };
            IEnumerable<ReadWithLengthTestCase> testCases = lineFeeds.SelectMany(
                lineFeed => new ReadWithLengthTestCase[]
                {
                    new ReadWithLengthTestCase(lineFeed)
                    {
                        DebugDescription = "Read 0 bytes from empty buffer.",
                        PayloadFunc = builder => builder.ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = 10,
                                BufferOffset = 0,
                                NumberOfBytesToRead = 0,
                                ExpectedNumberOfBytesRead = 0,
                            }
                        },
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 0,
                            NumberOfBytesInBuffer = 0,
                        }
                    },
                    new ReadWithLengthTestCase(lineFeed)
                    {
                        DebugDescription = "Read 10 bytes from empty buffer; this should fail since the # of bytes to read should never be greater than the stream length.",
                        PayloadFunc = builder => builder.ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = 10,
                                BufferOffset = 0,
                                NumberOfBytesToRead = 10,
                                ExpectedNumberOfBytesRead = 0,
                            }
                        },
                        ExpectedException = ODataExpectedExceptions.ODataException("General_InternalError", "ODataBatchReaderStreamBuffer_ReadWithLength"),
                    },
                    new ReadWithLengthTestCase(lineFeed)
                    {
                        DebugDescription = "Read line feed from buffer.",
                        PayloadFunc = builder => builder.LineFeed().ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = 10,
                                BufferOffset = 0,
                                NumberOfBytesToRead = lineFeed.Length,
                                ExpectedNumberOfBytesRead = lineFeed.Length,
                            }
                        },
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = lineFeed.Length,
                            NumberOfBytesInBuffer = 0,
                        }
                    },
                    new ReadWithLengthTestCase(lineFeed)
                    {
                        DebugDescription = "Read line feed from buffer with more data in the buffer and an offset of 5.",
                        PayloadFunc = builder => builder.LineFeed().FillBytes(10, 'a').ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = 10,
                                BufferOffset = 5,
                                NumberOfBytesToRead = lineFeed.Length,
                                ExpectedNumberOfBytesRead = lineFeed.Length,
                            }
                        },
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = lineFeed.Length,
                            NumberOfBytesInBuffer = 10,
                        }
                    },
                    new ReadWithLengthTestCase(lineFeed)
                    {
                        DebugDescription = "Read 10 characters from buffer.",
                        PayloadFunc = builder => builder
                            .FillBytes(100, 'a')
                            .ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = 10,
                                BufferOffset = 0,
                                NumberOfBytesToRead = 10,
                                ExpectedNumberOfBytesRead = 10,
                            }
                        },
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 10,
                            NumberOfBytesInBuffer = 90,
                        }
                    },
                    new ReadWithLengthTestCase(lineFeed)
                    {
                        DebugDescription = "Read all of the buffer with an offset of 10.",
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'a')
                            .ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = 10 + BatchReaderStreamBufferWrapper.BufferLength,
                                BufferOffset = 10,
                                NumberOfBytesToRead = BatchReaderStreamBufferWrapper.BufferLength,
                                ExpectedNumberOfBytesRead = BatchReaderStreamBufferWrapper.BufferLength,
                            }
                        },
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = BatchReaderStreamBufferWrapper.BufferLength,
                            NumberOfBytesInBuffer = 0,
                        }
                    },
                    new ReadWithLengthTestCase(lineFeed)
                    {
                        DebugDescription = "Read more than fits into a single buffer with an offset of 10 (so that no bytes are left).",
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'a')
                            .FillBytes(100, 'b')
                            .ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = 10 + BatchReaderStreamBufferWrapper.BufferLength + 100,
                                BufferOffset = 10,
                                NumberOfBytesToRead = BatchReaderStreamBufferWrapper.BufferLength + 100,
                                ExpectedNumberOfBytesRead = BatchReaderStreamBufferWrapper.BufferLength + 100,
                            }
                        },
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 100,
                            NumberOfBytesInBuffer = 0,
                        }
                    },
                    new ReadWithLengthTestCase(lineFeed)
                    {
                        DebugDescription = "Read more than fits into a single buffer with an offset of 10 (with 50 bytes left in the buffer).",
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'a')
                            .FillBytes(100, 'b')
                            .FillBytes(50, 'c')
                            .ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = 10 + BatchReaderStreamBufferWrapper.BufferLength + 100,
                                BufferOffset = 10,
                                NumberOfBytesToRead = BatchReaderStreamBufferWrapper.BufferLength + 100,
                                ExpectedNumberOfBytesRead = BatchReaderStreamBufferWrapper.BufferLength + 100,
                            }
                        },
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 100,
                            NumberOfBytesInBuffer = 50,
                        }
                    },
                    new ReadWithLengthTestCase(lineFeed)
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
                    },
                    new ReadWithLengthTestCase(lineFeed)
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
                    },
                    new ReadWithLengthTestCase(lineFeed)
                    {
                        DebugDescription = "Read more than is in the stream.",
                        PayloadFunc = builder => builder
                            .FillBytes(100, 'a')
                            .ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = BatchReaderStreamBufferWrapper.BufferLength,
                                BufferOffset = 0,
                                NumberOfBytesToRead = BatchReaderStreamBufferWrapper.BufferLength,
                                ExpectedNumberOfBytesRead = 100,
                            }
                        },
                        ExpectedException = ODataExpectedExceptions.ODataException("General_InternalError", "ODataBatchReaderStreamBuffer_ReadWithLength"),
                    },
                    new ReadWithLengthTestCase(lineFeed)
                    {
                        DebugDescription = "Read a boundary delimiter as part of the data; will not stop the reading since we are reading based on length.",
                        PayloadFunc = builder => builder
                            .FillBytes(100, 'a')
                            .StartBoundary("boundary_string")
                            .FillBytes(100, 'b')
                            .ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = 200,
                                BufferOffset = 0,
                                NumberOfBytesToRead = 100 + lineFeed.Length + BatchReaderStreamTestUtils.TwoDashesLength + "boundary_string".Length  + lineFeed.Length + 50,
                                ExpectedNumberOfBytesRead = 100 + lineFeed.Length + BatchReaderStreamTestUtils.TwoDashesLength + "boundary_string".Length  + lineFeed.Length + 50,
                            }
                        },
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 100 + lineFeed.Length + BatchReaderStreamTestUtils.TwoDashesLength + "boundary_string".Length  + lineFeed.Length + 50,
                            NumberOfBytesInBuffer = 50,
                        }
                    },
                    new ReadWithLengthTestCase(lineFeed)
                    {
                        DebugDescription = "Read 10 characters from buffer, then another 10 and another 10, exhausting the buffer.",
                        PayloadFunc = builder => builder
                            .FillBytes(30, 'a')
                            .ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = 10,
                                BufferOffset = 0,
                                NumberOfBytesToRead = 10,
                                ExpectedNumberOfBytesRead = 10,
                            },
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = 10,
                                BufferOffset = 0,
                                NumberOfBytesToRead = 10,
                                ExpectedNumberOfBytesRead = 10,
                            },
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = 10,
                                BufferOffset = 0,
                                NumberOfBytesToRead = 10,
                                ExpectedNumberOfBytesRead = 10,
                            }
                        },
                        ExpectedBufferState = new BatchReaderStreamBufferState
                        {
                            ReadPositionInBuffer = 30,
                            NumberOfBytesInBuffer = 0,
                        }
                    },
                    new ReadWithLengthTestCase(lineFeed)
                    {
                        DebugDescription = "Read all characters from buffer, then attempts to read more.",
                        PayloadFunc = builder => builder
                            .FillBytes(BatchReaderStreamBufferWrapper.BufferLength, 'a')
                            .ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = BatchReaderStreamBufferWrapper.BufferLength,
                                BufferOffset = 0,
                                NumberOfBytesToRead = BatchReaderStreamBufferWrapper.BufferLength,
                                ExpectedNumberOfBytesRead = BatchReaderStreamBufferWrapper.BufferLength,
                            },
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = BatchReaderStreamBufferWrapper.BufferLength,
                                BufferOffset = 0,
                                NumberOfBytesToRead = 1,
                                ExpectedNumberOfBytesRead = 0,
                            },
                        },
                        ExpectedException = ODataExpectedExceptions.ODataException("General_InternalError", "ODataBatchReaderStreamBuffer_ReadWithLength"),
                    },
                    new ReadWithLengthTestCase(lineFeed)
                    {
                        DebugDescription = "Read 10 characters from buffer, then more than is in the stream.",
                        PayloadFunc = builder => builder
                            .FillBytes(30, 'a')
                            .ResetMemoryStream(),
                        ReadDescriptors = new BatchReaderStreamReadDescriptor[]
                        {
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = 10,
                                BufferOffset = 0,
                                NumberOfBytesToRead = 10,
                                ExpectedNumberOfBytesRead = 10,
                            },
                            new BatchReaderStreamReadDescriptor
                            {
                                BufferSize = 21,
                                BufferOffset = 0,
                                NumberOfBytesToRead = 21,
                                ExpectedNumberOfBytesRead = 10,
                            },
                        },
                        ExpectedException = ODataExpectedExceptions.ODataException("General_InternalError", "ODataBatchReaderStreamBuffer_ReadWithLength"),
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
        
        /// <summary>
        /// Test case class to test the ReadWithLength method.
        /// </summary>
        internal class ReadWithLengthTestCase : BatchReaderStreamTestCase
        {
            private MemoryStream payloadStream;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="lineFeedChars">The line feed characters to use in the test.</param>
            public ReadWithLengthTestCase(char[] lineFeedChars = null)
                : this(null, null, lineFeedChars)
            {
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="batchBoundary">The boundary string to be used for the batch request; or null if a default boundary should be used.</param>
            /// <param name="encoding">The encoding to use to convert bytes to and from strings.</param>
            /// <param name="lineFeedChars">The line feed characters to use in the test.</param>
            public ReadWithLengthTestCase(string batchBoundary = null, Encoding encoding = null, char[] lineFeedChars = null)
                : base(batchBoundary, encoding, lineFeedChars)
            {
            }

            /// <summary>
            /// Copy constructor.
            /// </summary>
            /// <param name="other">The test case instance to copy.</param>
            public ReadWithLengthTestCase(ReadWithLengthTestCase other)
                : base(other)
            {
            }

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
            /// The descriptors of the read operations to be executed against the stream.
            /// </summary>
            public IEnumerable<BatchReaderStreamReadDescriptor> ReadDescriptors { get; set; }

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
                    int actualBytesRead = streamWrapper.ReadWithLength(buffer, readDescriptor.BufferOffset, readDescriptor.NumberOfBytesToRead);
                    this.VerifyResult(readDescriptor, buffer, actualBytesRead);
                }

                base.VerifyResult(streamWrapper.BatchBuffer);
            }

            /// <summary>
            /// Verifies the result of executing the test action.
            /// </summary>
            /// <param name="readDescriptor">The descriptors of the read operations to be executed against the stream.</param>
            /// <param name="buffer">The byte buffer with the bytes read.</param>
            /// <param name="actualNumberOfBytesRead">The actual numbers of bytes read.</param>
            /// <param name="line">The line read from the stream.</param>
            protected void VerifyResult(
                BatchReaderStreamReadDescriptor readDescriptor, 
                byte[] buffer, 
                int actualNumberOfBytesRead)
            {
                // Check that we read the expected number of bytes
                this.Assert.AreEqual(readDescriptor.ExpectedNumberOfBytesRead, actualNumberOfBytesRead,
                    string.Format("\r\n{0}:\r\nExpected to read '{1}' bytes but the actual number of bytes read is '{2}'.", this.DebugDescription, readDescriptor.ExpectedNumberOfBytesRead, actualNumberOfBytesRead));

                // Now make sure the bytes we read are correct
                this.Assert.IsNotNull(this.payloadStream, "Must have a payload stream.");
                byte[] sourceBytes = this.payloadStream.GetBuffer();

                for (int i = 0; i < readDescriptor.ExpectedNumberOfBytesRead; ++i)
                {
                    this.Assert.AreEqual(sourceBytes[readDescriptor.SourceStreamOffset + i], buffer[readDescriptor.BufferOffset + i],
                        string.Format("\r\n{0}:\r\nExpected to find '{1}' at position '{2}' but the actual value is '{3}'.", 
                        this.DebugDescription, 
                        sourceBytes[readDescriptor.SourceStreamOffset + i],
                        i,
                        buffer[i]));
                }
            }
        }
    }
}
