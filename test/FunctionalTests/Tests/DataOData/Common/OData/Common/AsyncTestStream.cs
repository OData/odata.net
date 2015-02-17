//---------------------------------------------------------------------
// <copyright file="AsyncTestStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Implements a Stream which supports async operations and makes them behave in an interesting way
    /// </summary>
    public class AsyncTestStream : Stream
    {
        /// <summary>
        /// The behavior of the async method which is requested. Default is Synchronous.
        /// </summary>
        public AsyncMethodBehavior AsyncMethodBehavior { get; set; }

        /// <summary>
        /// The inner stream this stream is wrapping
        /// </summary>
        public Stream InnerStream { get; set; }

        /// <summary>
        /// Optional enumerator which gets invoked before every BeginWrite call to possibly change the async method behavior.
        /// The current value is set as the AsyncMethodBehavior value.
        /// </summary>
        public IEnumerator<AsyncMethodBehavior> AsyncMethodBehaviorsEnumerator { get; set; }

        /// <summary>
        /// Set to true if the stream should fail any write operation.
        /// </summary>
        public bool FailOnWrite { get; set; }

        /// <summary>
        /// Set to true if the stream should fail any read operation.
        /// </summary>
        public bool FailOnRead { get; set; }

        /// <summary>
        /// Set to true if Dispose was called on the class.
        /// </summary>
        public bool Disposed { get; set; }

        /// <summary>
        /// Helper list of interesting async behavior loops.
        /// </summary>
        public static AsyncMethodBehavior[][] InterestingBehaviors = 
        {
            new AsyncMethodBehavior[] { AsyncMethodBehavior.Synchronous },
            new AsyncMethodBehavior[] { AsyncMethodBehavior.AsynchronousImmediateSameThread },
            new AsyncMethodBehavior[] { AsyncMethodBehavior.AsynchronousImmediateDifferentThread },
            new AsyncMethodBehavior[] { AsyncMethodBehavior.AsynchronousDelayed },
            new AsyncMethodBehavior[] { AsyncMethodBehavior.Synchronous, AsyncMethodBehavior.AsynchronousDelayed },
            new AsyncMethodBehavior[] { AsyncMethodBehavior.AsynchronousImmediateDifferentThread, AsyncMethodBehavior.AsynchronousImmediateSameThread, AsyncMethodBehavior.AsynchronousDelayed },
        };

        /// <summary>
        /// Constructor - creates a new inner stream
        /// </summary>
        public AsyncTestStream()
            : this(new MemoryStream())
        {
        }

        /// <summary>
        /// Constructor - takes inner stream as parameter.
        /// </summary>
        /// <param name="innerStream">The stream to wrap.</param>
        public AsyncTestStream(Stream innerStream)
        {
            this.InnerStream = innerStream;
            this.AsyncMethodBehavior = AsyncMethodBehavior.Synchronous;
        }

        /// <summary>
        /// Begins an async write operation on the stream.
        /// </summary>
        /// <param name="buffer">The buffer to write data from.</param>
        /// <param name="offset">Offset in the buffer to start from.</param>
        /// <param name="count">Number of bytes to write.</param>
        /// <param name="callback">Async callback.</param>
        /// <param name="state">Async callback state.</param>
        /// <returns>An async operation representing the write.</returns>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            ExceptionUtilities.Assert(!this.FailOnWrite, "A write operation should never be called on this stream.");
            if (this.AsyncMethodBehaviorsEnumerator!= null)
            {
                this.AsyncMethodBehavior = this.AsyncMethodBehaviorsEnumerator.MoveNext() ? this.AsyncMethodBehaviorsEnumerator.Current : default(AsyncMethodBehavior);
            }

            TestAsyncResult asyncResult = TestAsyncResult.Create(this.AsyncMethodBehavior, callback, state);
            asyncResult.CustomState = (Action)(() => { this.InnerStream.Write(buffer, offset, count); });
            asyncResult.Start();
            return asyncResult;
        }

        /// <summary>
        /// Ends an async write operation on the stream.
        /// </summary>
        /// <param name="asyncResult">The async operation created with BeginWrite call.</param>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            ExceptionUtilities.Assert(!this.FailOnWrite, "A write operation should never be called on this stream.");
            TestAsyncResult testAsyncResult = asyncResult as TestAsyncResult;
            if (testAsyncResult == null)
            {
                throw new ArgumentException("Unexpected IAsyncResult implementation.");
            }

            ((Action)testAsyncResult.CustomState)();
        }

        /// <summary>
        /// Begins an async read operation on the stream.
        /// </summary>
        /// <param name="buffer">The buffer to write the data to.</param>
        /// <param name="offset">Offset in the buffer to start writing at.</param>
        /// <param name="count">Number of bytes to read.</param>
        /// <param name="callback">Async callback.</param>
        /// <param name="state">Async callback state.</param>
        /// <returns>An async operation representing the read.</returns>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            ExceptionUtilities.Assert(!this.FailOnRead, "A read operation should never be called on this stream.");
            if (this.AsyncMethodBehaviorsEnumerator != null)
            {
                this.AsyncMethodBehavior = this.AsyncMethodBehaviorsEnumerator.MoveNext() ? this.AsyncMethodBehaviorsEnumerator.Current : default(AsyncMethodBehavior);
            }

            TestAsyncResult asyncResult = TestAsyncResult.Create(this.AsyncMethodBehavior, callback, state);
            asyncResult.CustomState = (Func<int>)(() => { return this.InnerStream.Read(buffer, offset, count); });
            asyncResult.Start();
            return asyncResult;
        }

        /// <summary>
        /// Ends an async read operation on the stream.
        /// </summary>
        /// <param name="asyncResult">The async operation created with BeginRead call.</param>
        public override int EndRead(IAsyncResult asyncResult)
        {
            ExceptionUtilities.Assert(!this.FailOnRead, "A read operation should never be called on this stream.");
            TestAsyncResult testAsyncResult = asyncResult as TestAsyncResult;
            if (testAsyncResult == null)
            {
                throw new ArgumentException("Unexpected IAsyncResult implementation.");
            }

            return ((Func<int>)testAsyncResult.CustomState)();
        }

        /// <summary>
        /// Determines if the stream can read - this one may support this
        /// </summary>
        public override bool CanRead
        {
            get { return !this.FailOnRead; }
        }

        /// <summary>
        /// Determines if the stream can seek - this one can't
        /// </summary>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// Determines if the stream can write - this one may support this
        /// </summary>
        public override bool CanWrite
        {
            get { return !this.FailOnWrite; }
        }

        /// <summary>
        /// Flush the stream to the underlying storage.
        /// </summary>
        public override void Flush()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the length of the stream, which implementation doesn't support this.
        /// </summary>
        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets the position in the stream, this stream doesn't support seeking, so possition is also unsupported.
        /// </summary>
        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Reads data from the stream. This operation is not supported by this stream.
        /// </summary>
        /// <param name="buffer">The buffer to read the data to.</param>
        /// <param name="offset">The offset in the buffer to write to.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Seeks the stream. This operation is not supported by this stream.
        /// </summary>
        /// <param name="offset">The offset to seek to.</param>
        /// <param name="origin">The origin of the seek operation.</param>
        /// <returns>The new position in the stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the length of the stream. This operation is not supported by this stream.
        /// </summary>
        /// <param name="value">The length in bytes to set.</param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes to the stream. This operation is not supported by this stream.
        /// </summary>
        /// <param name="buffer">The buffer to get data from.</param>
        /// <param name="offset">The offset in the buffer to start from.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">If called from Dispose or finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            this.Disposed = true;
        }
    }
}
