//---------------------------------------------------------------------
// <copyright file="TestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.ODataLibTest
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Test.OData.Utils.Common;

    /// <summary>
    /// The base class for the test message (request and response) implementations.
    /// </summary>
    public abstract class TestMessage
    {
        /// <summary>
        /// The stream with the content of the message.
        /// </summary>
        protected readonly Stream stream;

        /// <summary>
        /// Flags modifying the behavior of the message.
        /// </summary>
        protected readonly TestMessageFlags flags;

        /// <summary>
        /// HTTP headers of the message.
        /// </summary>
        protected Dictionary<string, string> headers = new Dictionary<string, string>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream with the content of the message.</param>
        /// <param name="flags">Flags modifying the behavior of the message.</param>
        protected TestMessage(Stream stream, TestMessageFlags flags = TestMessageFlags.None)
        {
            this.stream = stream;
            this.flags = flags;
            this.StreamRetrieved = false;
            this.IgnoreSynchronousError = false;
        }

        /// <summary>
        /// HTTP headers.
        /// </summary>
        public virtual IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                return this.headers;
            }
        }

        /// <summary>
        /// Sets whether to check for sync vs async calls in the message
        /// </summary>
        public bool IgnoreSynchronousError { get; set; }

        /// <summary>
        /// Set to true once the GetStream or GetStreamAsync was called.
        /// </summary>
        public bool StreamRetrieved
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the message stream as a TestStream.
        /// </summary>
        public virtual  TestStream TestStream
        {
            get
            {
                return this.stream as TestStream;
            }
        }

        /// <summary>
        /// Resets the test message so that it can be used again.
        /// </summary>
        public virtual void Reset()
        {
            this.StreamRetrieved = false;
            TestStream testStream = this.TestStream;
            if (testStream != null)
            {
                testStream.Reset();
            }
            else
            {
                ExceptionUtilities.Assert(this.stream.CanSeek, "Need seekable stream in order to reset.");
                this.stream.Seek(0, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Returns an HTTP header by its name.
        /// </summary>
        /// <param name="headerName">The name of the header to return.</param>
        /// <returns>The value of the header or null if no such header was found.</returns>
        public virtual string GetHeader(string headerName)
        {
            string value;
            if (this.headers.TryGetValue(headerName, out value))
                return value;

            return null;
        }

        /// <summary>
        /// Sets an HTTP header.
        /// </summary>
        /// <param name="headerName">The name of the header to set.</param>
        /// <param name="headerValue">The value of the header or null to remove that header.</param>
        public virtual void SetHeader(string headerName, string headerValue)
        {
            if (this.StreamRetrieved)
            {
                throw new InvalidOperationException("Cannot set a header once the stream has been retrieved using GetStream or GetStreamAsync.");
            }

            if (headerValue == null)
            {
                this.headers.Remove(headerName);
            }
            else
            {
                this.headers[headerName] = headerValue;
            }
        }

        /// <summary>
        /// Returns the content of the message as a stream.
        /// </summary>
        /// <returns>The stream which is to be used for reading or writing the content of the message.</returns>
        public virtual Stream GetStream()
        {
            if ((this.flags & TestMessageFlags.NoSynchronous) == TestMessageFlags.NoSynchronous && !this.IgnoreSynchronousError)
            {
                throw new InvalidOperationException("TestMessage.GetStream() called on message which does not support synchronous operations.");
            }

            if (this.StreamRetrieved)
            {
                throw new InvalidOperationException("Can only get the stream once from the message.");
            }

            if ((this.flags & TestMessageFlags.GetStreamFailure) == TestMessageFlags.GetStreamFailure)
            {
                throw new InvalidOperationException("TestMessage.GetStream() failed.");
            }

            this.StreamRetrieved = true;
            return this.stream;
        }

        /// <summary>
        /// Returns the content of the message as a stream.
        /// </summary>
        /// <returns>A task which when completed returns the stream which is to be used for reading or writing the content of the message.</returns>
        public virtual Task<Stream> GetStreamAsync()
        {
            if ((this.flags & TestMessageFlags.NoAsynchronous) == TestMessageFlags.NoAsynchronous)
            {
                throw new InvalidOperationException("TestMessage.GetStreamAsync() called on message which does not support asynchronous operations.");
            }

            if (this.StreamRetrieved)
            {
                throw new InvalidOperationException("Can only get the stream once from the message.");
            }

            if ((this.flags & TestMessageFlags.GetStreamFailure) == TestMessageFlags.GetStreamFailure)
            {
                throw new InvalidOperationException("TestMessage.GetStreamAsync() failed.");
            }

            this.StreamRetrieved = true;
            return TestTaskUtils.GetCompletedTask(this.stream);
        }
    }
}
