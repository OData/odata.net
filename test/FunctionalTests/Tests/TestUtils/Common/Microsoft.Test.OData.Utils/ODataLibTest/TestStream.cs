//---------------------------------------------------------------------
// <copyright file="TestStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.ODataLibTest
{
    using System;
    using System.IO;
    using Microsoft.Test.OData.Utils.Common;

    /// <summary>
    /// Implementation of a memory stream that tracks the number of calls to Dispose() and may ignore disposing of the inner stream.
    /// We may want to be able to use the inner memory stream even after the ODataLib disposed it for validation purposes.
    /// </summary>
    public class TestStream : Stream
    {
        /// <summary>
        /// Inner stream.
        /// </summary>
        protected Stream innerStream;

        /// <summary>
        /// When set to true, the Dispose method will not dispose the innerStream.
        /// When set to false (default), the innerStream will be disposed by the Dispose() method.
        /// </summary>
        bool ignoreDispose;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestStream()
            : this(new MemoryStream())
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream to wrap.</param>
        /// <param name="ignoreDispose">When set to true, the Dispose method will not dispose the innerStream.
        /// When set to false (default), the innerStream will be disposed by the Dispose() method.</param>
        public TestStream(Stream stream, bool ignoreDispose = false)
        {
            this.innerStream = stream;
            this.ignoreDispose = ignoreDispose;
            this.IgnoreSynchronousError = false;
        }

        /// <summary>
        /// Set to true and the next call to either Write or Read will fail with an exception - simulating failure to write/read to the stream.
        /// </summary>
        public bool FailNextCall { get; set; }

        /// <summary>
        /// Set to true if all synchronous operations should fail on this stream.
        /// </summary>
        public bool FailSynchronousCalls { get; set; }

        /// <summary>
        /// Set to true if all asynchronous operations should fail on this stream.
        /// </summary>
        public bool FailAsynchronousCalls { get; set; }

        /// <summary>
        /// Sets whether to check for sync vs async calls in the message
        /// </summary>
        public bool IgnoreSynchronousError { get; set; }

        /// <summary>
        /// Keeps track of the number of times the Dispose() method is called.
        /// </summary>
        public int DisposeCount { get; set; }

        /// <summary>
        /// Returns the inner stream of this test stream. To be used ONLY for test deserialization.
        /// </summary>
        public Stream InnerStream
        {
            get
            {
                return this.innerStream;
            }
        }

        public override bool CanRead
        {
            get { return this.innerStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return this.innerStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return this.innerStream.CanWrite; }
        }

        public override void Flush()
        {
            this.innerStream.Flush();
        }

        public override long Length
        {
            get { return this.innerStream.Length; }
        }

        public override long Position
        {
            get { return this.innerStream.Position; }
            set { this.innerStream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.FailNextCall)
            {
                this.FailNextCall = false;
                throw new SystemException("Simulated stream failure.");
            }

            if (this.FailSynchronousCalls && !this.IgnoreSynchronousError)
            {
                throw new SystemException("Synchronous calls are not allowed on the stream.");
            }

            return this.innerStream.Read(buffer, offset, count);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (this.FailNextCall)
            {
                this.FailNextCall = false;
                throw new SystemException("Simulated stream failure.");
            }

            if (this.FailAsynchronousCalls)
            {
                throw new SystemException("Asynchronous calls are not allowed on the stream.");
            }

            return this.innerStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return this.innerStream.EndRead(asyncResult);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.FailNextCall)
            {
                this.FailNextCall = false;
                throw new SystemException("Simulated stream failure.");
            }

            if (this.FailSynchronousCalls)
            {
                throw new SystemException("Synchronous calls are not allowed on the stream.");
            }

            this.innerStream.Write(buffer, offset, count);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (this.FailNextCall)
            {
                this.FailNextCall = false;
                throw new SystemException("Simulated stream failure.");
            }

            if (this.FailAsynchronousCalls)
            {
                throw new SystemException("Asynchronous calls are not allowed on the stream.");
            }

            return this.innerStream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            this.innerStream.EndWrite(asyncResult);
        }

        public void Reset()
        {
            ExceptionUtilities.Assert(this.innerStream.CanSeek, "Need seekable stream in order to reset.");
            this.innerStream.Seek(0, SeekOrigin.Begin);
            this.DisposeCount = 0;
        }

        public void CloseInner()
        {
            this.innerStream.Close();
        }

        /// <summary>
        /// Ignore calls to dispose so that the inner stream stays available.
        /// </summary>
        /// <param name="disposing">True if called via IDisposable; false when called from the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DisposeCount++;

                if (!this.ignoreDispose && this.innerStream != null)
                {
                    this.innerStream.Dispose();
                    this.innerStream = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
