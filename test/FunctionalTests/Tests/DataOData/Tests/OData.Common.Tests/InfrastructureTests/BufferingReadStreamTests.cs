//---------------------------------------------------------------------
// <copyright file="BufferingReadStreamTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.InfrastructureTests
{
// Tests use private reflection and thus cannot run on Silverlight or the phone.

    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the BufferingReadStream class
    /// </summary>
    [TestClass, TestCase]
    public class BufferingReadStreamTests : ODataTestCase
    {
        private static readonly byte[] testBytes = new byte[byte.MaxValue];

        static BufferingReadStreamTests()
        {
            for (byte i=0; i < byte.MaxValue; ++i)
            {
                testBytes[i] = i;
            }
        }

        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [TestMethod, Variation(Description = "Verifies that the stream only allows reading and fails on all other methods and properties.")]
        public void StaticBehaviorTest()
        {
            BufferingReadStreamTestWrapper bufferedReadStream = new BufferingReadStreamTestWrapper(new MemoryStream());
            this.Assert.IsTrue(bufferedReadStream.CanRead, "CanRead should be true.");
            this.Assert.IsFalse(bufferedReadStream.CanSeek, "CanSeek should be false.");
            this.Assert.IsFalse(bufferedReadStream.CanWrite, "CanWrite should be false.");
        }

        [TestMethod, Variation(Description = "Verifies that the BufferingReadStream can handle all kinds of combinations of buffering and non-buffering reads.")]
        public void BufferingReadStreamTest()
        {
            IEnumerable<ReadOperation[]> bufferingReads = new ReadOperation[][]
            {
                new ReadOperation[]
                {
                    // No buffering reads
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 10, ExpectedBytesRead = 10, ExpectedResult = 9 }
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 4 },
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 9 },
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 14 },
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 19 },
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 4 },
                    new ReadOperation { Count = 8, ExpectedBytesRead = 8, ExpectedResult = 12 },
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = byte.MaxValue, ExpectedBytesRead = byte.MaxValue, ExpectedResult = byte.MaxValue - 1 },
                },
            };

            IEnumerable<ReadOperation[]> replayReads = new ReadOperation[][]
            {
                new ReadOperation[]
                {
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 4},
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 10, ExpectedBytesRead = 10, ExpectedResult = 9 }
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 20, ExpectedBytesRead = 20, ExpectedResult = 19 }
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 2, ExpectedBytesRead = 2, ExpectedResult = 1 },
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 6 },
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 2, ExpectedBytesRead = 2, ExpectedResult = 1 },
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 6 },
                    new ReadOperation { Count = 3, ExpectedBytesRead = 3, ExpectedResult = 9 },
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 1, ExpectedBytesRead = 1, ExpectedResult = 0 },
                    new ReadOperation { Count = 1, ExpectedBytesRead = 1, ExpectedResult = 1 },
                    new ReadOperation { Count = 1, ExpectedBytesRead = 1, ExpectedResult = 2 },
                    new ReadOperation { Count = 1, ExpectedBytesRead = 1, ExpectedResult = 3 },
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 4 },
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 9 },
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 14 },
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 19 },
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 10, ExpectedBytesRead = 10, ExpectedResult = 9 },
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 14 },
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 10, ExpectedBytesRead = 10, ExpectedResult = 9 },
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 14 },
                    new ReadOperation { Count = 10, ExpectedBytesRead = 10, ExpectedResult = 24 },
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 10, ExpectedBytesRead = 10, ExpectedResult = 9 },
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 14 },
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 19 },
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 4 },
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 9 },
                    new ReadOperation { Count = 10, ExpectedBytesRead = 10, ExpectedResult = 19 },
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 10, ExpectedBytesRead = 10, ExpectedResult = 9 },
                    new ReadOperation { Count = 10, ExpectedBytesRead = 10, ExpectedResult = 19 },
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 20, ExpectedBytesRead = 20, ExpectedResult = 19 },
                    new ReadOperation { Count = 20, ExpectedBytesRead = 20, ExpectedResult = 39 },
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = byte.MaxValue, ExpectedBytesRead = byte.MaxValue, ExpectedResult = byte.MaxValue - 1 },
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = byte.MaxValue, ExpectedBytesRead = byte.MaxValue, ExpectedResult = byte.MaxValue - 1 },
                    new ReadOperation { Count = 5, ExpectedBytesRead = 0, ExpectedResult = byte.MaxValue - 1 },
                    new ReadOperation { Count = 5, ExpectedBytesRead = 0, ExpectedResult = byte.MaxValue - 1 },
                    new ReadOperation { Count = 5, ExpectedBytesRead = 0, ExpectedResult = byte.MaxValue - 1 },
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 4 },
                    new ReadOperation { Count = 10, ExpectedBytesRead = 10, ExpectedResult = 14 },
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 4 },
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 9 },
                    new ReadOperation { Count = 10, ExpectedBytesRead = 10, ExpectedResult = 19 },
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 5, ExpectedBytesRead = 5, ExpectedResult = 4 },
                    new ReadOperation { Count = byte.MaxValue - 5, ExpectedBytesRead = byte.MaxValue - 5, ExpectedResult = byte.MaxValue - 1 },
                },
                new ReadOperation[]
                {
                    new ReadOperation { Count = 20, ExpectedBytesRead = 20, ExpectedResult = 19 },
                    new ReadOperation { Count = byte.MaxValue - 20, ExpectedBytesRead = byte.MaxValue - 20, ExpectedResult = byte.MaxValue - 1 },
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                bufferingReads,
                // Adding the null case to completely skip the buffering replay reads
                replayReads.ConcatSingle(null),
                replayReads,
                (bufferingRead, bufferingReplayRead, replayRead) =>
                {
                    MemoryStream testStream = new MemoryStream(testBytes);
                    BufferingReadStreamTestWrapper bufferingReadStream = new BufferingReadStreamTestWrapper(testStream);

                    byte[] targetBuffer = new byte[1000];
                    int currentOffset = 0;

                    foreach (ReadOperation readOp in bufferingRead)
                    {
                        int bytesRead = bufferingReadStream.Read(targetBuffer, currentOffset, readOp.Count);
                        this.Assert.AreEqual(readOp.ExpectedBytesRead, bytesRead, "Bytes read differs.");

                        currentOffset += bytesRead;
                        this.Assert.AreEqual(readOp.ExpectedResult, targetBuffer[currentOffset - 1], "Last byte read differs.");
                    }

                    if (bufferingReplayRead != null)
                    {
                        bufferingReadStream.ResetStream();

                        targetBuffer = new byte[1000];
                        currentOffset = 0;

                        foreach (ReadOperation readOp in bufferingReplayRead)
                        {
                            int bytesRead = bufferingReadStream.Read(targetBuffer, currentOffset, readOp.Count);
                            this.Assert.AreEqual(readOp.ExpectedBytesRead, bytesRead, "Bytes read differs.");

                            currentOffset += bytesRead;
                            this.Assert.AreEqual(readOp.ExpectedResult, targetBuffer[currentOffset - 1], "Last byte read differs.");
                        }
                    }

                    bufferingReadStream.StopBuffering();

                    targetBuffer = new byte[1000];
                    currentOffset = 0;

                    foreach (ReadOperation readOp in replayRead)
                    {
                        int bytesRead = bufferingReadStream.Read(targetBuffer, currentOffset, readOp.Count);
                        this.Assert.AreEqual(readOp.ExpectedBytesRead, bytesRead, "Bytes read differs.");

                        currentOffset += bytesRead;
                        this.Assert.AreEqual(readOp.ExpectedResult, targetBuffer[currentOffset - 1], "Last byte read differs.");
                    }
                });
        }

        private struct ReadOperation
        {
            public int Count { get; set; }
            public int ExpectedBytesRead { get; set; }
            public byte ExpectedResult { get; set; }
        }

        /// <summary>
        /// Wrapper class which uses private reflection to access the internal BufferingReadStream class in the product
        /// </summary>
        private sealed class BufferingReadStreamTestWrapper : Stream, IDisposable
        {
            private static readonly Type bufferingReadStreamType = typeof(ODataAnnotatable).Assembly.GetType("Microsoft.OData.BufferingReadStream");

            private Stream bufferingReadStream;

            public BufferingReadStreamTestWrapper(Stream inputStream)
            {
                this.bufferingReadStream = (Stream)ReflectionUtils.CreateInstance(bufferingReadStreamType, inputStream);
            }

            public override bool CanRead { get { return this.bufferingReadStream.CanRead; } }
            public override bool CanSeek { get { return this.bufferingReadStream.CanSeek; } }
            public override bool CanWrite { get { return this.bufferingReadStream.CanWrite; } }
            public override void Flush() { this.bufferingReadStream.Flush(); }
            public override long Length { get { return this.bufferingReadStream.Length; } }
            public override long Position
            {
                get { return this.bufferingReadStream.Position; }
                set { this.bufferingReadStream.Position = value; }
            }
            public override int Read(byte[] buffer, int offset, int count) { return this.bufferingReadStream.Read(buffer, offset, count); }
            public override long Seek(long offset, SeekOrigin origin) { return this.bufferingReadStream.Seek(offset, origin); }
            public override void SetLength(long value) { this.bufferingReadStream.SetLength(value); }
            public override void Write(byte[] buffer, int offset, int count) { this.bufferingReadStream.Write(buffer, offset, count); }

            public bool IsBuffering { get { return (bool)ReflectionUtils.GetProperty(this.bufferingReadStream, "IsBuffering"); } }
            public void ResetStream() { ReflectionUtils.InvokeMethod(this.bufferingReadStream, "ResetStream"); }
            public void StopBuffering() { ReflectionUtils.InvokeMethod(this.bufferingReadStream, "StopBuffering"); }

            protected override void Dispose(bool disposing) { this.bufferingReadStream.Dispose(); }
        }
    }
}
