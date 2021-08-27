//---------------------------------------------------------------------
// <copyright file="AsyncBufferedStreamTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.InfrastructureTests
{
    #region Namespaces
    using System;
    using System.IO;
    using System.Reflection;
     using System.Threading.Tasks;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the AsyncBufferedStream class
    /// </summary>
    [TestClass, TestCase(Name="Async Buffered Stream Tests")]
    public class AsyncBufferedStreamTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [InjectDependency(IsRequired = true)]
        public IExceptionVerifier ExceptionVerifier { get; set; }

        [TestMethod, Variation(Description = "Verifies that the stream only allows writing and fails on all other methods and properties.")]
        public void StaticBehaviorTest()
        {
            AsyncBufferedStreamTestWrapper asyncBufferedStream = new AsyncBufferedStreamTestWrapper(new MemoryStream());
            this.Assert.IsFalse(asyncBufferedStream.CanRead, "CanRead should be false.");
            this.Assert.IsFalse(asyncBufferedStream.CanSeek, "CanSeek should be false.");
            this.Assert.IsTrue(asyncBufferedStream.CanWrite, "CanWrite should be true.");
        }

        [TestMethod, Variation(Description = "Verifies that Dispose throws if FlushAsync (or Clear) was not called.")]
        public void DisposeFlushTest()
        {
            AsyncBufferedStreamTestWrapper asyncBufferedStream = new AsyncBufferedStreamTestWrapper(new MemoryStream());
            // Dispose without writing anything and without FlushAsync should work
            asyncBufferedStream.Dispose();

            asyncBufferedStream = new AsyncBufferedStreamTestWrapper(new MemoryStream());
            asyncBufferedStream.Write(new byte[] { 1 }, 0, 1);
            this.Assert.ExpectedException(
                () => { asyncBufferedStream.Dispose(); },
                ODataExpectedExceptions.ODataException("AsyncBufferedStream_WriterDisposedWithoutFlush"),
                this.ExceptionVerifier);
        }

        [TestMethod, Variation(Description = "Verifies that Dispose does not throw if Clear was called.")]
        public void DisposeClearTest()
        {
            using (AsyncBufferedStreamTestWrapper asyncBufferedStream = new AsyncBufferedStreamTestWrapper(new MemoryStream()))
            {
                asyncBufferedStream.Write(new byte[] { 1 }, 0, 1);
                asyncBufferedStream.Clear();
            }
        }

        [TestMethod, Variation(Description = "Verifies the correct behavior of the FlushAsync method and the buffering nature of the stream.")]
        public void FlushAsyncTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                new[] {
                    new int[] { 0 },
                    new int[] { 1 },
                    new int[] { 1, 20, 1, 0 },
                    new int[] { 100000, 1, 100000, 23 }
                },
                new bool[] { false, true },
                new bool[] { false, true },
                AsyncTestStream.InterestingBehaviors,
                (writeCounts, useBiggerBuffer, repeat, asyncBehaviors) =>
                {
                    AsyncTestStream testStream = new AsyncTestStream()
                    {
                        FailOnRead = true,
                        AsyncMethodBehaviorsEnumerator = asyncBehaviors.EndLessLoop()
                    };
                    AsyncBufferedStreamTestWrapper asyncBufferedStream = new AsyncBufferedStreamTestWrapper(testStream);

                    while (true)
                    {
                        int totalWrittenCount = 0;
                        foreach (int writeCount in writeCounts)
                        {
                            byte[] data = new byte[writeCount + (useBiggerBuffer ? 20 : 0)];
                            for (int i = 0; i < writeCount; i++)
                            {
                                data[i + (useBiggerBuffer ? 10 : 0)] = (byte)((totalWrittenCount + i) % 256);
                            }

                            asyncBufferedStream.Write(data, useBiggerBuffer ? 10 : 0, writeCount);
                            totalWrittenCount += writeCount;
                        }

                        this.Assert.AreEqual(0, testStream.InnerStream.Length, "The buffered stream should not have written anything to the underlying stream yet.");

                        asyncBufferedStream.FlushAsync().Wait();
                        byte[] writtenData = ((MemoryStream)testStream.InnerStream).GetBuffer();
                        this.Assert.AreEqual(totalWrittenCount, testStream.InnerStream.Length, "Wrong number of bytes was written to the stream.");
                        for (int i = 0; i < totalWrittenCount; i++)
                        {
                            this.Assert.AreEqual(i % 256, writtenData[i], "Wrong data written to the stream.");
                        }

                        if (!repeat)
                        {
                            break;
                        }

                        testStream.InnerStream = new MemoryStream();
                        repeat = false;
                    }

                    this.Assert.IsFalse(testStream.Disposed, "The stream should not have been disposed yet.");
                    asyncBufferedStream.Dispose();
                    this.Assert.IsFalse(testStream.Disposed, "The underlying stream should not be disposed.");
                });
        }

        /// <summary>
        /// Wrapper class which uses private reflection to access the internal AsyncBufferedStream class in the product
        /// </summary>
        private class AsyncBufferedStreamTestWrapper : Stream, IDisposable
        {
            private Stream asyncBufferedStream;

            public AsyncBufferedStreamTestWrapper(Stream stream)
            {
                Type asyncBufferedStreamType = typeof(Microsoft.OData.ODataAnnotatable).Assembly.GetType("Microsoft.OData.AsyncBufferedStream");
                
                this.asyncBufferedStream = (Stream)Activator.CreateInstance(
                    asyncBufferedStreamType,
                    BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
                    null,
                    new object[] { stream },
                    null);
            }

            public override bool CanRead { get { return this.asyncBufferedStream.CanRead; } }
            public override bool CanSeek { get { return this.asyncBufferedStream.CanSeek; } }
            public override bool CanWrite { get { return this.asyncBufferedStream.CanWrite; } }
            public override void Flush() { this.asyncBufferedStream.Flush(); }
            public override long Length { get { return this.asyncBufferedStream.Length; } }
            public override long Position
            {
                get { return this.asyncBufferedStream.Position; }
                set { this.asyncBufferedStream.Position = value; }
            }
            public override int Read(byte[] buffer, int offset, int count) { return this.asyncBufferedStream.Read(buffer, offset, count); }
            public override long Seek(long offset, SeekOrigin origin) { return this.asyncBufferedStream.Seek(offset, origin); }
            public override void SetLength(long value) { this.asyncBufferedStream.SetLength(value); }
            public override void Write(byte[] buffer, int offset, int count) { this.asyncBufferedStream.Write(buffer, offset, count); }

            public new Task FlushAsync()
            {
                return (Task)ReflectionUtils.InvokeMethod(this.asyncBufferedStream, "FlushAsync");
            }

            public void Clear()
            {
                ReflectionUtils.InvokeMethod(this.asyncBufferedStream, "Clear");
            }

            protected override void Dispose(bool disposing) { this.asyncBufferedStream.Dispose(); }
        }
    }
}
