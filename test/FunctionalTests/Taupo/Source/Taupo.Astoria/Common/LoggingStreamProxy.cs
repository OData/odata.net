//---------------------------------------------------------------------
// <copyright file="LoggingStreamProxy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// A stream proxy which logs all bytes read or written to an underlying stream
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Class wraps a stream, so it should end with 'proxy'")]
    public class LoggingStreamProxy : ReadWriteOnlyStreamProxy, IStreamLogger
    {
        private List<byte[]> writeBuffers;
        private MemoryStream writeLog;
        private List<byte[]> readBuffers;
        private MemoryStream readLog;

        /// <summary>
        /// Initializes a new instance of the LoggingStreamProxy class
        /// </summary>
        /// <param name="underlying">The underlying stream</param>
        public LoggingStreamProxy(Stream underlying)
            : this(underlying, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LoggingStreamProxy class
        /// </summary>
        /// <param name="underlying">The underlying stream</param>
        /// <param name="writable">A value indicating whether the stream proxy can be written to</param>
        public LoggingStreamProxy(Stream underlying, bool writable)
            : base(underlying, writable)
        {
            this.writeBuffers = new List<byte[]>();
            this.writeLog = new MemoryStream();
            this.readBuffers = new List<byte[]>();
            this.readLog = new MemoryStream();

            this.CreateWrappedCallbackFunc = this.CreateWrappedCallback;
            this.CreateWrappedResultFunc = this.CreateWrappedResult;
            this.IsClosed = false;
            this.IsEndOfStream = false;
        }

        /// <summary>
        /// Gets the set of buffers that have been written
        /// </summary>
        public ReadOnlyCollection<byte[]> BuffersWritten
        {
            get
            {
                return this.writeBuffers.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the set of buffers that have been read
        /// </summary>
        public ReadOnlyCollection<byte[]> BuffersRead
        {
            get
            {
                return this.readBuffers.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the stream is closed
        /// </summary>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the stream has been read to the end
        /// </summary>
        public bool IsEndOfStream { get; private set; }

        /// <summary>
        /// Gets or sets the function to use when creating wrapped results. Unit test hook only.
        /// </summary>
        internal Func<byte[], int, IAsyncResult, WrappedBeginReadResult> CreateWrappedResultFunc { get; set; }

        /// <summary>
        /// Gets or sets the function to use when creating wrapped callbacks. Unit test hook only.
        /// </summary>
        internal Func<byte[], int, AsyncCallback, AsyncCallback> CreateWrappedCallbackFunc { get; set; }

        /// <summary>
        /// Gets all the bytes that have been written
        /// </summary>
        /// <returns>All the bytes written to the stream</returns>
        public byte[] GetAllBytesWritten()
        {
            return this.writeLog.ToArray();
        }

        /// <summary>
        /// Gets all the bytes that have been read
        /// </summary>
        /// <returns>All the bytes read from the stream</returns>
        public byte[] GetAllBytesRead()
        {
            return this.readLog.ToArray();
        }

        /// <summary>
        /// Writes to the stream and logs the bytes written
        /// </summary>
        /// <param name="buffer">The buffer to write from</param>
        /// <param name="offset">The offset into the buffer</param>
        /// <param name="count">The number of bytes to write</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.AddWriteBuffer(buffer, offset, count);
            base.Write(buffer, offset, count);
        }

        /// <summary>
        /// Begins an async write and logs the bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write from</param>
        /// <param name="offset">The offset into the buffer</param>
        /// <param name="count">The number of bytes to write</param>
        /// <param name="callback">The async callback</param>
        /// <param name="state">The async state</param>
        /// <returns>The async result</returns>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            this.AddWriteBuffer(buffer, offset, count);
            return base.BeginWrite(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Reads from the stream and logs the bytes read
        /// </summary>
        /// <param name="buffer">The buffer to read into</param>
        /// <param name="offset">The offset into the buffer</param>
        /// <param name="count">The number of bytes to try to read</param>
        /// <returns>The number of bytes read</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int readCount = base.Read(buffer, offset, count);
            this.AddReadBuffer(buffer, offset, readCount);
            return readCount;
        }

        /// <summary>
        /// Begins an async read.
        /// </summary>
        /// <param name="buffer">The buffer to read into</param>
        /// <param name="offset">The offset into the buffer</param>
        /// <param name="count">The number of bytes to read</param>
        /// <param name="callback">The async callback</param>
        /// <param name="state">The async state</param>
        /// <returns>The async result</returns>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            var wrappedCallback = this.CreateWrappedCallbackFunc(buffer, offset, callback);
            var result = base.BeginRead(buffer, offset, count, wrappedCallback, state);
            return this.CreateWrappedResultFunc(buffer, offset, result);
        }

        /// <summary>
        /// Ends an async read and logs the bytes read. 
        /// </summary>
        /// <param name="asyncResult">The async result from a BeginRead call</param>
        /// <returns>The number of bytes read</returns>
        public override int EndRead(IAsyncResult asyncResult)
        {
            var wrapped = asyncResult as WrappedBeginReadResult;
            ExceptionUtilities.CheckObjectNotNull(wrapped, "Async result was of unexpected type");
            
            int readCount = base.EndRead(wrapped.RealResult);
            this.AddReadBuffer(wrapped.Buffer, wrapped.Offset, readCount);
            return readCount;
        }

        /// <summary>
        /// Closes the stream
        /// </summary>
        public override void Close()
        {
            this.IsClosed = true;
            base.Close();
        }

        internal WrappedBeginReadResult CreateWrappedResult(byte[] buffer, int offset, IAsyncResult realResult)
        {
            return new WrappedBeginReadResult(buffer, offset, realResult);
        }

        internal AsyncCallback CreateWrappedCallback(byte[] buffer, int offset, AsyncCallback realCallback)
        {
            if (realCallback == null)
            {
                return null;
            }

            return result => realCallback(this.CreateWrappedResultFunc(buffer, offset, result));
        }

        private void AddWriteBuffer(byte[] buffer, int offset, int count)
        {
            this.writeLog.Write(buffer, offset, count);
            this.writeBuffers.Add(buffer.Skip(offset).Take(count).ToArray());
        }

        private void AddReadBuffer(byte[] buffer, int offset, int count)
        {
            if (count == 0)
            {
                this.IsEndOfStream = true;
            }
            else
            {
                this.readLog.Write(buffer, offset, count);
                this.readBuffers.Add(buffer.Skip(offset).Take(count).ToArray());
            }
        }

        /// <summary>
        /// Async result implementation that wraps the result returned from call to BeginRead
        /// </summary>
        internal class WrappedBeginReadResult : IAsyncResult
        {
            /// <summary>
            /// Initializes a new instance of the WrappedBeginReadResult class
            /// </summary>
            /// <param name="buffer">The buffer given to BeginRead</param>
            /// <param name="offset">The offset given to BeginRead</param>
            /// <param name="realResult">The async result returned by the underlying stream</param>
            public WrappedBeginReadResult(byte[] buffer, int offset, IAsyncResult realResult)
            {
                ExceptionUtilities.CheckArgumentNotNull(buffer, "buffer");
                ExceptionUtilities.CheckArgumentNotNull(realResult, "realResult");

                this.Buffer = buffer;
                this.Offset = offset;
                this.RealResult = realResult;
            }

            /// <summary>
            /// Gets the buffer given to BeginRead
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Byte array is the type given to BeginRead.")]
            public byte[] Buffer { get; private set; }

            /// <summary>
            /// Gets the offset given to BeginRead
            /// </summary>
            public int Offset { get; private set; }

            /// <summary>
            /// Gets the real underlying async result
            /// </summary>
            public IAsyncResult RealResult { get; private set; }

            /// <summary>
            /// Gets the async state
            /// </summary>
            public object AsyncState
            {
                get { return this.RealResult.AsyncState; }
            }

            /// <summary>
            /// Gets the async wait handle
            /// </summary>
            public WaitHandle AsyncWaitHandle
            {
                get { return this.RealResult.AsyncWaitHandle; }
            }

            /// <summary>
            /// Gets a value indicating whether or not the operation completed synchronously
            /// </summary>
            public bool CompletedSynchronously
            {
                get { return this.RealResult.CompletedSynchronously; }
            }

            /// <summary>
            /// Gets a value indicating whether or not the operation has completed
            /// </summary>
            public bool IsCompleted
            {
                get { return this.RealResult.IsCompleted; }
            }
        }
    }
}