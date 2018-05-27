//---------------------------------------------------------------------
// <copyright file="ODataMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Base class for the internal wrappers around IODataRequestMessageAsync and IODataResponseMessageAsync.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "This class does not own the BufferingReadStream instance.")]
    internal abstract class ODataMessage
    {
        /// <summary>true if the message is being written; false when it is read.</summary>
        private readonly bool writing;

        /// <summary>true if the stream returned should be disposed calls.</summary>
        private readonly bool enableMessageStreamDisposal;

        /// <summary>The maximum size of the message in bytes (or null if no maximum applies).</summary>
        private readonly long maxMessageSize;

        /// <summary>true to use a buffering read stream wrapper around the actual message stream; otherwise false.</summary>
        private bool? useBufferingReadStream;

        /// <summary>The buffering read stream used for payload kind detection; only non-null inside of payload kind detection.</summary>
        private BufferingReadStream bufferingReadStream;

        /// <summary>
        /// Constructs a new ODataMessage.
        /// </summary>
        /// <param name="writing">true if the message is being written; false when it is read.</param>
        /// <param name="enableMessageStreamDisposal">true if the stream returned should be disposed calls.</param>
        /// <param name="maxMessageSize">The maximum size of the message in bytes (or a negative value if no maximum applies).</param>
        protected ODataMessage(bool writing, bool enableMessageStreamDisposal, long maxMessageSize)
        {
            this.writing = writing;
            this.enableMessageStreamDisposal = enableMessageStreamDisposal;
            this.maxMessageSize = maxMessageSize;
        }

        /// <summary>
        /// Returns an enumerable over all the headers for this message.
        /// </summary>
        public abstract IEnumerable<KeyValuePair<string, string>> Headers
        {
            // TODO: do we want to impose a certain order of the headers?
            get;
        }

        /// <summary>
        /// true to use a buffering read stream wrapper around the actual message stream; otherwise false.
        /// </summary>
        protected internal BufferingReadStream BufferingReadStream
        {
            get
            {
                return this.bufferingReadStream;
            }
        }

        /// <summary>
        /// true to use a buffering read stream wrapper around the actual message stream; otherwise false.
        /// </summary>
        protected internal bool? UseBufferingReadStream
        {
            get
            {
                return this.useBufferingReadStream;
            }

            set
            {
                Debug.Assert(!this.writing, "UseBufferingReadStream should only be set when reading.");
                this.useBufferingReadStream = value;
            }
        }

        /// <summary>
        /// Returns a value of an HTTP header.
        /// </summary>
        /// <param name="headerName">The name of the header to get.</param>
        /// <returns>The value of the HTTP header, or null if no such header was present on the message.</returns>
        public abstract string GetHeader(string headerName);

        /// <summary>
        /// Sets the value of an HTTP header.
        /// </summary>
        /// <param name="headerName">The name of the header to set.</param>
        /// <param name="headerValue">The value for the header with name <paramref name="headerName"/>.</param>
        public abstract void SetHeader(string headerName, string headerValue);

        /// <summary>
        /// Get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Intentionally a method.")]
        public abstract Stream GetStream();

#if PORTABLELIB
        /// <summary>
        /// Asynchronously get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Intentionally a method.")]
        public abstract Task<Stream> GetStreamAsync();
#endif

        /// <summary>
        /// Queries the message for the specified interface type.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface to query for.</typeparam>
        /// <returns>The instance of the interface asked for or null if it was not implemented by the message.</returns>
        /// <remarks>We need this method since the input contexts don't get access to the actual instance of the message given to us by the user
        /// instead they get this class, and thus they can't just cast to get to the interface they want.</remarks>
        internal abstract TInterface QueryInterface<TInterface>() where TInterface : class;

        /// <summary>
        /// Synchronously get the stream backing this message.
        /// </summary>
        /// <param name="messageStreamFunc">A function that returns the stream backing the message.</param>
        /// <param name="isRequest">true if the message is a request message; false for a response message.</param>
        /// <returns>The <see cref="Stream"/> backing the message.</returns>
        protected internal Stream GetStream(Func<Stream> messageStreamFunc, bool isRequest)
        {
            // Check whether we have an existing buffering read stream when reading
            if (!this.writing)
            {
                BufferingReadStream existingBufferingReadStream = this.TryGetBufferingReadStream();
                if (existingBufferingReadStream != null)
                {
                    Debug.Assert(this.useBufferingReadStream.HasValue, "UseBufferingReadStream must have been set.");
                    return existingBufferingReadStream;
                }
            }

            // Get the message stream
            Stream messageStream = messageStreamFunc();
            ValidateMessageStream(messageStream, isRequest);

            // When reading, wrap the stream in a byte counting stream if a max message size was specified.
            // When requested, wrap the stream in a non-disposing stream.
            bool needByteCountingStream = !this.writing && this.maxMessageSize > 0;
            if (!this.enableMessageStreamDisposal && needByteCountingStream)
            {
                messageStream = MessageStreamWrapper.CreateNonDisposingStreamWithMaxSize(messageStream, this.maxMessageSize);
            }
            else if (!this.enableMessageStreamDisposal)
            {
                messageStream = MessageStreamWrapper.CreateNonDisposingStream(messageStream);
            }
            else if (needByteCountingStream)
            {
                messageStream = MessageStreamWrapper.CreateStreamWithMaxSize(messageStream, this.maxMessageSize);
            }

            // If a buffering read stream is required, create it now
            if (!this.writing && this.useBufferingReadStream == true)
            {
                Debug.Assert(!this.writing, "The buffering read stream should only be used when reading.");
                this.bufferingReadStream = new BufferingReadStream(messageStream);
                messageStream = this.bufferingReadStream;
            }

            return messageStream;
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously get the stream backing this message.
        /// </summary>
        /// <param name="streamFuncAsync">A function that returns a task for the stream backing the message.</param>
        /// <param name="isRequest">true if the message is a request message; false for a response message.</param>
        /// <returns>A task that when completed returns the stream backing the message.</returns>
        protected internal Task<Stream> GetStreamAsync(Func<Task<Stream>> streamFuncAsync, bool isRequest)
        {
            // Check whether we have an existing buffering read stream when reading
            if (!this.writing)
            {
                Stream existingBufferingReadStream = this.TryGetBufferingReadStream();
                Debug.Assert(!this.writing || existingBufferingReadStream == null, "The buffering read stream should only be used when reading.");
                if (existingBufferingReadStream != null)
                {
                    Debug.Assert(this.useBufferingReadStream.HasValue, "UseBufferingReadStream must have been set.");
                    return TaskUtils.GetCompletedTask(existingBufferingReadStream);
                }
            }

            Task<Stream> task = streamFuncAsync();
            ValidateMessageStreamTask(task, isRequest);

            // Wrap it in a non-disposing stream if requested
            task = task.FollowOnSuccessWith(
                streamTask =>
                {
                    Stream messageStream = streamTask.Result;
                    ValidateMessageStream(messageStream, isRequest);

                    // When reading, wrap the stream in a byte counting stream if a max message size was specified.
                    // When requested, wrap the stream in a non-disposing stream.
                    bool needByteCountingStream = !this.writing && this.maxMessageSize > 0;
                    if (!this.enableMessageStreamDisposal && needByteCountingStream)
                    {
                        messageStream = MessageStreamWrapper.CreateNonDisposingStreamWithMaxSize(messageStream, this.maxMessageSize);
                    }
                    else if (!this.enableMessageStreamDisposal)
                    {
                        messageStream = MessageStreamWrapper.CreateNonDisposingStream(messageStream);
                    }
                    else if (needByteCountingStream)
                    {
                        messageStream = MessageStreamWrapper.CreateStreamWithMaxSize(messageStream, this.maxMessageSize);
                    }

                    return messageStream;
                });

            // When we are reading, also buffer the input stream
            if (!this.writing)
            {
                task = task
                    .FollowOnSuccessWithTask(
                        streamTask =>
                        {
                            return BufferedReadStream.BufferStreamAsync(streamTask.Result);
                        })
                    .FollowOnSuccessWith(
                        streamTask =>
                        {
                            BufferedReadStream bufferedReadStream = streamTask.Result;
                            return (Stream)bufferedReadStream;
                        });

                // If requested also create a buffering stream for payload kind detection
                if (this.useBufferingReadStream == true)
                {
                    task = task.FollowOnSuccessWith(
                        streamTask =>
                        {
                            Stream messageStream = streamTask.Result;
                            this.bufferingReadStream = new BufferingReadStream(messageStream);
                            messageStream = this.bufferingReadStream;
                            return messageStream;
                        });
                }
            }

            return task;
        }
#endif

        /// <summary>
        /// Verifies that setting a header is allowed
        /// </summary>
        /// <remarks>
        /// We allow modifying the headers only if we are writing the message and we are not
        /// detecting the payload kind.
        /// </remarks>
        protected void VerifyCanSetHeader()
        {
            if (!this.writing)
            {
                throw new ODataException(Strings.ODataMessage_MustNotModifyMessage);
            }

            Debug.Assert(this.bufferingReadStream == null, "The buffering stream should only be used when reading.");
        }

        /// <summary>
        /// Validates that a given message stream can be used.
        /// </summary>
        /// <param name="stream">The stream to validate.</param>
        /// <param name="isRequest">true if the message is a request message; false for a response message.</param>
        private static void ValidateMessageStream(Stream stream, bool isRequest)
        {
            if (stream == null)
            {
                string error = isRequest
                    ? Strings.ODataRequestMessage_MessageStreamIsNull
                    : Strings.ODataResponseMessage_MessageStreamIsNull;
                throw new ODataException(error);
            }
        }

#if PORTABLELIB
        /// <summary>
        /// Validates that a given task providing the message stream can be used.
        /// </summary>
        /// <param name="streamTask">The task to validate.</param>
        /// <param name="isRequest">true if the message is a request message; false for a response message.</param>
        private static void ValidateMessageStreamTask(Task<Stream> streamTask, bool isRequest)
        {
            if (streamTask == null)
            {
                string error = isRequest
                    ? Strings.ODataRequestMessage_StreamTaskIsNull
                    : Strings.ODataResponseMessage_StreamTaskIsNull;
                throw new ODataException(error);
            }
        }
#endif

        /// <summary>
        /// Gets the buffering read stream if one is available; otherwise returns null.
        /// </summary>
        /// <returns>The <see cref="BufferingReadStream"/> currently being used or null if no buffering stream is currently being used.</returns>
        private BufferingReadStream TryGetBufferingReadStream()
        {
            if (this.bufferingReadStream == null)
            {
                return null;
            }

            // We already have a buffering read stream; reset it if necessary and return it;
            // if the stream is not buffering anymore we are done with payload kind detection
            // and don't need the buffering stream anymore - we start the actual reading now.
            BufferingReadStream stream = this.bufferingReadStream;
            if (this.bufferingReadStream.IsBuffering)
            {
                this.bufferingReadStream.ResetStream();
            }
            else
            {
                this.bufferingReadStream = null;
            }

            return stream;
        }
    }
}
