//---------------------------------------------------------------------
// <copyright file="BufferedReadStreamTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.InfrastructureTests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the BufferedReadStream class
    /// </summary>
    [TestClass, TestCase]
    public class BufferedReadStreamTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [TestMethod, Variation(Description = "Verifies that the stream only allows reading and fails on all other methods and properties.")]
        public void StaticBehaviorTest()
        {
            BufferedReadStreamTestWrapper bufferedReadStream = BufferedReadStreamTestWrapper.BufferStream(new MemoryStream()).WaitForResult();
            this.Assert.IsTrue(bufferedReadStream.CanRead, "CanRead should be true.");
            this.Assert.IsFalse(bufferedReadStream.CanSeek, "CanSeek should be false.");
            this.Assert.IsFalse(bufferedReadStream.CanWrite, "CanWrite should be false.");
        }

        [TestMethod, Variation(Description = "Verifies that the BufferStream can handle all kinds of input streams.")]
        public void BufferStreamAsyncTests()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                new[] {
                    new int[] {},
                    new int[] { 1 },
                    new int[] { 1, 20, 1 },
                    new int[] { 20 * 1024 },
                    new int[] { 100 * 1024 },
                    new int[] { 100 * 1024, 20 }
                },
                AsyncTestStream.InterestingBehaviors,
                (readCounts, asyncBehaviors) =>
                {
                    int streamSize = 0;
                    foreach (int i in readCounts)
                    {
                        streamSize += i;
                    }

                    streamSize *= 3;
                    byte[] data = new byte[streamSize];
                    for (int i = 0; i < streamSize; i++)
                    {
                        data[i] = (byte)(i % 256);
                    }

                    ReadTestStream readTestStream = new ReadTestStream(new MemoryStream(data));
                    readTestStream.ReadSizesEnumerator = readCounts.Length == 0 ? null : readCounts.EndLessLoop();
                    AsyncTestStream asyncTestStream = new AsyncTestStream(readTestStream)
                    {
                        FailOnWrite = true,
                        AsyncMethodBehaviorsEnumerator = asyncBehaviors.EndLessLoop()
                    };

                    var task = BufferedReadStreamTestWrapper.BufferStream(asyncTestStream);
                    BufferedReadStreamTestWrapper bufferedStream = task.WaitForResult();

                    int count = 0;
                    int readCount = 0;
                    do
                    {
                        if (count == data.Length)
                        {
                            // This is to ask for more data than what's in the stream to verify that the stream
                            // correctly returns 0 bytes read at the end.
                            readCount = bufferedStream.Read(new byte[1], 0, 1);
                        }
                        else
                        {
                            readCount = bufferedStream.Read(data, count, data.Length - count);
                        }
                        count += readCount;
                    } while (readCount > 0);
                    this.Assert.AreEqual(streamSize, count, "The stream returned wrong number of bytes.");

                    for (int i = 0; i < streamSize; i++)
                    {
                        this.Assert.AreEqual(i % 256, data[i], "Wrong data written to the stream.");
                    }

                    this.Assert.IsTrue(asyncTestStream.Disposed, "The input stream was not disposed.");
                });
        }

        [TestMethod, Variation(Description = "Verifies that the BufferStream can handle reading data in various sizes.")]
        public void ReadBufferedDataTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                new[] {
                    new int[] { 1 },
                    new int[] { 1, 20, 1 },
                    new int[] { 20 * 1024 },
                    new int[] { 100 * 1024 },
                    new int[] { 100 * 1024, 20 }
                },
                (readCounts) =>
                {
                    int streamSize = 0;
                    foreach (int i in readCounts)
                    {
                        streamSize += i;
                    }

                    streamSize *= 3;
                    byte[] data = new byte[streamSize];
                    for (int i = 0; i < streamSize; i++)
                    {
                        data[i] = (byte)(i % 256);
                    }

                    var task = BufferedReadStreamTestWrapper.BufferStream(new MemoryStream(data));
                    BufferedReadStreamTestWrapper bufferedStream = task.WaitForResult();

                    IEnumerator<int> readCountsEnumerator = readCounts.EndLessLoop();

                    int count = 0;
                    int readCount = 0;
                    byte[] buffer = new byte[streamSize];
                    do
                    {
                        readCountsEnumerator.MoveNext();
                        int readSize = readCountsEnumerator.Current;
                        readCount = bufferedStream.Read(buffer, 0, readSize);
                        this.Assert.IsTrue(count + readCount <= data.Length, "The stream returned more data than expected.");
                        Array.Copy(buffer, 0, data, count, readCount);
                        count += readCount;
                    } while (readCount > 0);
                    this.Assert.AreEqual(streamSize, count, "The stream returned wrong number of bytes.");

                    for (int i = 0; i < streamSize; i++)
                    {
                        this.Assert.AreEqual(i % 256, data[i], "Wrong data written to the stream.");
                    }
                });
        }

        /// <summary>
        /// Wrapper class which uses private reflection to access the internal BufferedReadStream class in the product
        /// </summary>
        private class BufferedReadStreamTestWrapper : Stream, IDisposable
        {
            private Stream bufferedReadStream;

            private BufferedReadStreamTestWrapper(Stream bufferedReadStream)
            {
                this.bufferedReadStream = bufferedReadStream;
            }


            private static Task InvokeContinueWith<TTaskResult, TContinueWithResult>(Task<TTaskResult> t, Func<Stream, TContinueWithResult> func) where TTaskResult : Stream
            {
                return t.ContinueWith((task) => func(task.Result));
            }

            public static Task<BufferedReadStreamTestWrapper> BufferStream(Stream inputStream)
            {
                Type asyncBufferedStreamType = typeof(Microsoft.OData.ODataAnnotatable).Assembly.GetType("Microsoft.OData.BufferedReadStream");

                object task = ReflectionUtils.InvokeMethod(asyncBufferedStreamType, "BufferStreamAsync", inputStream);
                return (Task<BufferedReadStreamTestWrapper>)ReflectionUtils.InvokeGenericMethod(
                    typeof(BufferedReadStreamTestWrapper), 
                    "InvokeContinueWith", 
                    new Type[] { asyncBufferedStreamType, typeof(BufferedReadStreamTestWrapper) },
                    task,
                    (Func<Stream, BufferedReadStreamTestWrapper>)((stream) => new BufferedReadStreamTestWrapper(stream)));
            }

            public override bool CanRead { get { return this.bufferedReadStream.CanRead; } }
            public override bool CanSeek { get { return this.bufferedReadStream.CanSeek; } }
            public override bool CanWrite { get { return this.bufferedReadStream.CanWrite; } }
            public override void Flush() { this.bufferedReadStream.Flush(); }
            public override long Length { get { return this.bufferedReadStream.Length; } }
            public override long Position
            {
                get { return this.bufferedReadStream.Position; }
                set { this.bufferedReadStream.Position = value; }
            }
            public override int Read(byte[] buffer, int offset, int count) { return this.bufferedReadStream.Read(buffer, offset, count); }
            public override long Seek(long offset, SeekOrigin origin) { return this.bufferedReadStream.Seek(offset, origin); }
            public override void SetLength(long value) { this.bufferedReadStream.SetLength(value); }
            public override void Write(byte[] buffer, int offset, int count) { this.bufferedReadStream.Write(buffer, offset, count); }

            public void ResetForReading() { ReflectionUtils.InvokeMethod(this.bufferedReadStream, "ResetForReading"); }

            protected override void Dispose(bool disposing) { this.bufferedReadStream.Dispose(); }
        }
    }
}
