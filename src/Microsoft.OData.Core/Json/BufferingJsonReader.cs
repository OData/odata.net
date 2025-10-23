//---------------------------------------------------------------------
// <copyright file="BufferingJsonReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    #endregion Namespaces

    /// <summary>
    /// Reader for the JSON format (http://www.json.org) that supports look-ahead.
    /// </summary>
    internal class BufferingJsonReader : IJsonReader
    {
        // Keep pools small; tune via benchmarks.
        private const int MaxPooledNodesPerThread = 256;

        // Per-thread pool: avoids lock contention and cross-thread reuse of linked nodes.
        // Not for thread-safety of the reader instance itself (reader is single-threaded).
        [ThreadStatic]
        private static Stack<BufferedNode> bufferedNodePool;

        /// <summary>The (possibly empty) list of buffered nodes.</summary>
        /// <remarks>This is a circular linked list where this field points to the first item of the list.</remarks>
        protected BufferedNode bufferedNodesHead;

        /// <summary>
        /// A pointer into the bufferedNodes list to track the most recent position of the current buffered node.
        /// </summary>
        protected BufferedNode currentBufferedNode;

        /// <summary>
        /// The inner JSON reader.
        /// </summary>
        private readonly IJsonReader innerReader;

        /// <summary>
        /// The maximum number of recursive internalexception objects to allow when reading in-stream errors.
        /// </summary>
        private readonly int maxInnerErrorDepth;

        /// <summary>The name of the property that denotes an in-stream error.</summary>
        private readonly string inStreamErrorPropertyName;

        /// <summary>A flag indicating whether the reader is in buffering mode or not.</summary>
        private bool isBuffering;

        /// <summary>
        /// A flag indicating that the last node for non-buffering read was taken from the buffer; we leave the
        /// node in the buffer until the next Read call.
        /// </summary>
        private bool removeOnNextRead;

        /// <summary>
        /// Debug flag to ensure we do not re-enter the instance while reading ahead and trying to parse an in-stream error.
        /// </summary>
        private bool parsingInStreamError;

        /// <summary>
        /// true if the parser should check for in-stream errors whenever a start-object node is encountered; otherwise false.
        /// This is set to false for parsing of top-level errors where we don't want the in-stream error detection code to kick in.
        /// </summary>
        private bool disableInStreamErrorDetection;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="innerReader">The inner JSON reader.</param>
        /// <param name="inStreamErrorPropertyName">The name of the property that denotes an in-stream error.</param>
        /// <param name="maxInnerErrorDepth">The maximum number of recursive internalexception objects to allow when reading in-stream errors.</param>
        internal BufferingJsonReader(IJsonReader innerReader, string inStreamErrorPropertyName, int maxInnerErrorDepth)
        {
            Debug.Assert(innerReader != null, "innerReader != null");

            this.innerReader = innerReader;

            this.inStreamErrorPropertyName = inStreamErrorPropertyName;
            this.maxInnerErrorDepth = maxInnerErrorDepth;
            this.bufferedNodesHead = null;
            this.currentBufferedNode = null;
        }

        /// <summary>
        /// The type of the last node read.
        /// </summary>
        /// <remarks>
        /// Depending on whether buffering is on or off this will return the node type of the last
        /// buffered read or the node type of the last unbuffered read.
        /// </remarks>
        public JsonNodeType NodeType
        {
            get
            {
                if (this.bufferedNodesHead != null)
                {
                    if (this.isBuffering)
                    {
                        Debug.Assert(this.currentBufferedNode != null, "this.currentBufferedNode != null");
                        return this.currentBufferedNode.NodeType;
                    }

                    // in non-buffering mode if we have buffered nodes satisfy the request from the first node there
                    return this.bufferedNodesHead.NodeType;
                }

                // This should work okay even in async scenarios since IJsonReaderAsync inherits from IJsonReader?
                return this.innerReader.NodeType;
            }
        }

        /// <summary>
        /// The value of the last reported node.
        /// </summary>
        /// <remarks>
        /// Depending on whether buffering is on or off this will return the node type of the last
        /// buffered read or the node type of the last unbuffered read.
        /// </remarks>
        public object GetValue()
        {
            if (this.bufferedNodesHead != null)
            {
                if (this.isBuffering)
                {
                    Debug.Assert(this.currentBufferedNode != null, "this.currentBufferedNode != null");
                    return this.currentBufferedNode.Value;
                }

                // in non-buffering mode if we have buffered nodes satisfy the request from the first node there
                return this.bufferedNodesHead.Value;
            }

            return this.innerReader.GetValue();
        }

        /// <summary>
        /// if it is IEEE754 compatible
        /// </summary>
        public bool IsIeee754Compatible
        {
            get
            {
                // This should work okay even in async scenarios since IJsonReaderAsync inherits from IJsonReader?
                return this.innerReader.IsIeee754Compatible;
            }
        }

        /// <summary>
        /// true if the parser should check for in-stream errors whenever a start-object node is encountered; otherwise false.
        /// This is set to false for parsing of top-level errors where we don't want the in-stream error detection code to kick in.
        /// </summary>
        internal bool DisableInStreamErrorDetection
        {
            get
            {
                return this.disableInStreamErrorDetection;
            }

            set
            {
                this.disableInStreamErrorDetection = value;
            }
        }

        /// <summary>
        /// Flag indicating whether buffering is on or off; debug-only for use in asserts.
        /// </summary>
        internal bool IsBuffering
        {
            get
            {
                return this.isBuffering;
            }
        }


        /// <summary>
        /// Creates a stream for reading a stream value
        /// </summary>
        /// <returns>A Stream used to read a stream value</returns>
        public virtual Stream CreateReadStream()
        {
            if (!this.isBuffering)
            {
                return this.innerReader.CreateReadStream();
            }

            object value = this.GetValue();
            Stream result = value == null ? Stream.Null :
                new MemoryStream(Convert.FromBase64String((string)value));
            this.innerReader.Read();
            return result;
        }

        /// <summary>
        /// Creates a TextReader for reading a text value.
        /// </summary>
        /// <returns>A TextReader for reading the text value.</returns>
        public virtual TextReader CreateTextReader()
        {
            if (!this.isBuffering)
            {
                return this.innerReader.CreateTextReader();
            }

            object value = this.GetValue();
            TextReader result = new StringReader(value == null ? "" : (string)value);
            this.innerReader.Read();
            return result;
        }

        /// <summary>
        /// Whether or not the current value can be streamed
        /// </summary>
        /// <returns>True if the current value can be streamed, otherwise false</returns>
        public virtual bool CanStream()
        {
            if (!this.isBuffering)
            {
                return this.innerReader.CanStream();
            }

            object value = this.GetValue();
            return value == null || value is string || this.NodeType == JsonNodeType.StartArray || this.NodeType == JsonNodeType.StartObject;
        }

        /// <summary>
        /// Reads the next node from the input.
        /// </summary>
        /// <returns>true if a new node was found, or false if end of input was reached.</returns>
        public bool Read()
        {
            Debug.Assert(!this.parsingInStreamError, "!this.parsingInStreamError");

            // read the next node and check whether it is an in-stream error
            return this.ReadInternal();
        }

        /// <summary>
        /// The value of the last reported node.
        /// </summary>
        /// <remarks>
        /// Depending on whether buffering is on or off this will return the node value of the last
        /// buffered read or the node value of the last unbuffered read.
        /// </remarks>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the node value of the last buffered read if buffering is on or off;
        /// otherwise the node value of the last unbuffered read.</returns>
        public Task<object> GetValueAsync()
        {
            this.AssertAsynchronous();

            if (this.bufferedNodesHead != null)
            {
                if (this.isBuffering)
                {
                    Debug.Assert(this.currentBufferedNode != null, "this.currentBufferedNode != null");
                    return Task.FromResult(this.currentBufferedNode.Value);
                }

                // In non-buffering mode if we have buffered nodes satisfy the request from the first node there
                return Task.FromResult(this.bufferedNodesHead.Value);
            }

            return this.innerReader.GetValueAsync();
        }

        /// <summary>
        /// Asynchronously reads the next node from the input.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true if a new node was found, or false if end of input was reached.</returns>
        public Task<bool> ReadAsync()
        {
            Debug.Assert(!this.parsingInStreamError, "!this.parsingInStreamError");
            this.AssertAsynchronous();

            // Read the next node and check whether it is an in-stream error
            return this.ReadInternalAsync();
        }

        /// <summary>
        /// Asynchronously checks whether or not the current value can be streamed.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// true if the current value can be streamed; otherwise false.</returns>
        public virtual Task<bool> CanStreamAsync()
        {
            this.AssertAsynchronous();

            // If buffering, we already the value locally - refer to GetValueAsync implementation
            if (this.isBuffering)
            {
                Debug.Assert(this.currentBufferedNode != null, "this.currentBufferedNode != null");
                BufferedNode node = this.currentBufferedNode;
                if (node.Value == null || node.Value is string || node.NodeType == JsonNodeType.StartArray || node.NodeType == JsonNodeType.StartObject)
                {
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }

            // Non-buffering: defer to the inner reader
            Task<bool> canStreamTask = this.innerReader.CanStreamAsync();
            if (canStreamTask.IsCompletedSuccessfully)
            {
                return canStreamTask;
            }

            return AwaitCanStreamAsync(canStreamTask);

            static async Task<bool> AwaitCanStreamAsync(Task<bool> canStreamTask)
            {
                return await canStreamTask.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously creates a stream for reading a stream value.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a <see cref="Stream"/> used to read a stream value.</returns>
        public virtual async Task<Stream> CreateReadStreamAsync()
        {
            this.AssertAsynchronous();

            if (!this.isBuffering)
            {
                return await this.innerReader.CreateReadStreamAsync()
                    .ConfigureAwait(false);
            }

            Stream result;

            object value = await this.GetValueAsync()
                .ConfigureAwait(false);

            if (value == null)
            {
                result = Stream.Null;
            }
            else
            {
                result = new MemoryStream(Convert.FromBase64String((string)value));
            }

            await this.innerReader.ReadAsync()
                .ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Asynchronously creates a TextReader for reading a text value.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a <see cref="TextReader"/> for reading the text value.</returns>
        public virtual async Task<TextReader> CreateTextReaderAsync()
        {
            this.AssertAsynchronous();

            if (!this.isBuffering)
            {
                return await this.innerReader.CreateTextReaderAsync()
                    .ConfigureAwait(false);
            }

            TextReader result;

            object value = await this.GetValueAsync()
                .ConfigureAwait(false);

            if (value == null)
            {
                result = new StringReader("");
            }
            else
            {
                result = new StringReader((string)value);
            }


            await this.innerReader.ReadAsync()
                .ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Puts the reader into the state where it buffers read nodes.
        /// </summary>
        internal void StartBuffering()
        {
            Debug.Assert(!this.isBuffering, "Buffering is already turned on. Must not call StartBuffering again.");

            if (this.bufferedNodesHead == null)
            {
                // capture the current state of the reader as the first item in the buffer (if there are none)
                this.bufferedNodesHead = RentNode(this.innerReader.NodeType, this.innerReader.GetValue());
            }
            else
            {
                this.removeOnNextRead = false;
            }

            Debug.Assert(this.bufferedNodesHead != null, "Expected at least the current node in the buffer when starting buffering.");

            // Set the currentBufferedNode to the first node in the list; this means every time we start buffering we reset the
            // position of the current buffered node since in general we don't know how far ahead we have read before and thus don't
            // want to blindly continuing to read. The model is that with every call to StartBuffering you reset the position of the
            // current node in the list and start reading through the buffer again.
            if (this.currentBufferedNode == null)
            {
                this.currentBufferedNode = this.bufferedNodesHead;
            }

            this.isBuffering = true;
        }

        /// <summary>
        /// Creates a bookmark at the current position of the reader.
        /// </summary>
        /// <returns>The bookmark object, it should be treated as a black box by the caller.</returns>
        internal object BookmarkCurrentPosition()
        {
            Debug.Assert(this.isBuffering, "Bookmarks can only be create when in buffering mode.");

            return this.currentBufferedNode;
        }

        /// <summary>
        /// Moves the reader to the bookmarked position.
        /// </summary>
        /// <param name="bookmark">The bookmark object to move to.</param>
        internal void MoveToBookmark(object bookmark)
        {
            Debug.Assert(this.isBuffering, "Bookmarks can only be used when in buffering mode.");
            Debug.Assert(bookmark != null, "bookmark != null");

            BufferedNode bookmarkNode = bookmark as BufferedNode;
            Debug.Assert(bookmarkNode != null, "Invalid bookmark.");
#if DEBUG
            Debug.Assert(this.NodeExistsInCurrentBuffer(bookmarkNode), "Tried to move to a bookmark which is already out of scope, bookmarks are only valid inside a single buffering session.");
#endif

            this.currentBufferedNode = bookmarkNode;
        }

        /// <summary>
        /// Puts the reader into the state where no buffering happen on read.
        /// Either buffered nodes are consumed or new nodes are read (and not buffered).
        /// </summary>
        internal void StopBuffering()
        {
            Debug.Assert(this.isBuffering, "Buffering is not turned on. Must not call StopBuffering in this state.");

            // NOTE: by turning off buffering the reader is set to the first node in the buffer (if any) and will continue
            //       to read from there. removeOnNextRead is set to true since we captured the original state of the reader
            //       (before starting to buffer) as the first node in the buffer and that node has to be removed on the next read.
            this.isBuffering = false;
            this.removeOnNextRead = true;

            // We set the currentBufferedNode to null here to indicate that we want to reset the position of the current
            // buffered node when we turn on buffering the next time. So far this (i.e., resetting the position of the buffered
            // node) is the only mode the BufferingJsonReader supports. We can make resetting the current node position more explicit
            // if needed.
            this.currentBufferedNode = null;
        }

        /// <summary>
        /// A method to detect whether the current property value represents an in-stream error.
        /// </summary>
        /// <param name="error">The <see cref="ODataError"/> read from the payload.</param>
        /// <returns>true if the current value is an in-stream error value; otherwise false.</returns>
        internal bool StartBufferingAndTryToReadInStreamErrorPropertyValue(out ODataError error)
        {
            this.AssertNotBuffering();

            error = null;

            this.StartBuffering();
            this.parsingInStreamError = true;

            try
            {
                return this.TryReadInStreamErrorPropertyValue(out error);
            }
            finally
            {
                this.StopBuffering();
                this.parsingInStreamError = false;
            }
        }

        /// <summary>
        /// Asynchronously puts the reader into the state where it buffers read nodes.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        internal Task StartBufferingAsync()
        {
            Debug.Assert(!this.isBuffering, "Buffering is already turned on. Must not call StartBuffering again.");
            this.AssertAsynchronous();

            if (this.bufferedNodesHead == null)
            {
                // capture the current state of the reader as the first item in the buffer (if there are none)
                Task<object> getValueTask = this.innerReader.GetValueAsync();
                if (!getValueTask.IsCompletedSuccessfully)
                {
                    return AwaitGetValueAndStartBufferingAsync(this, getValueTask);
                }

                this.bufferedNodesHead = RentNode(this.innerReader.NodeType, getValueTask.Result);
                CompleteStartBuffering(this);
                return Task.CompletedTask;
            }

            // Already have a head node
            this.removeOnNextRead = false;
            CompleteStartBuffering(this);
            return Task.CompletedTask;

            static void CompleteStartBuffering(BufferingJsonReader thisParam)
            {
                Debug.Assert(thisParam.bufferedNodesHead != null, "Expected at least the current node in the buffer when starting buffering.");

                // Set the currentBufferedNode to the first node in the list; this means every time we start buffering we reset the
                // position of the current buffered node since in general we don't know how far ahead we have read before and thus don't
                // want to blindly continuing to read. The model is that with every call to StartBufferingAsync you reset the position of the
                // current node in the list and start reading through the buffer again.
                if (thisParam.currentBufferedNode == null)
                {
                    thisParam.currentBufferedNode = thisParam.bufferedNodesHead;
                }

                thisParam.isBuffering = true;
            }

            static async Task AwaitGetValueAndStartBufferingAsync(BufferingJsonReader thisParam, Task<object> getValueTask)
            {
                object value = await getValueTask.ConfigureAwait(false);
                thisParam.bufferedNodesHead = RentNode(thisParam.innerReader.NodeType, value);
                CompleteStartBuffering(thisParam);
            }
        }

        /// <summary>
        /// A method to asynchronously detect whether the current property value represents an in-stream error.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a tuple comprising of:
        /// 1). A value of true if the current value is an in-stream error value; otherwise false.
        /// 2). An <see cref="ODataError"/> instance that was read from the payload.</returns>
        internal async ValueTask<(bool IsReadSuccessfully, ODataError Error)> StartBufferingAndTryToReadInStreamErrorPropertyValueAsync()
        {
            this.AssertNotBuffering();
            this.AssertAsynchronous();

            await this.StartBufferingAsync()
                .ConfigureAwait(false);
            this.parsingInStreamError = true;

            try
            {
                return await this.TryReadInStreamErrorPropertyValueAsync()
                    .ConfigureAwait(false);
            }
            finally
            {
                this.StopBuffering();
                this.parsingInStreamError = false;
            }
        }

        /// <summary>
        /// Reads the next node from the input. If we have still nodes in the buffer, takes the node
        /// from there. Otherwise reads a new node from the underlying reader and buffers it (depending on the current mode).
        /// </summary>
        /// <returns>true if a new node was found, or false if end of input was reached.</returns>
        /// <remarks>
        /// If the parsingInStreamError field is true, the method will read ahead for every StartObject node read from the input to check whether the JSON object
        /// represents an in-stream error. If so, it throws an <see cref="ODataErrorException"/>. If false, this check will not happen.
        /// This parsingInStreamError field is set to true when trying to parse an in-stream error; in normal operation it is false.
        /// </remarks>
        protected bool ReadInternal()
        {
            if (this.removeOnNextRead)
            {
                Debug.Assert(this.bufferedNodesHead != null, "If removeOnNextRead is true we must have at least one node in the buffer.");

                this.RemoveFirstNodeInBuffer();

                this.removeOnNextRead = false;
            }

            bool result;
            if (this.isBuffering)
            {
                Debug.Assert(this.currentBufferedNode != null, "this.currentBufferedNode != null");

                if (this.currentBufferedNode.Next != this.bufferedNodesHead)
                {
                    this.currentBufferedNode = this.currentBufferedNode.Next;
                    result = true;
                }
                else
                {
                    if (this.parsingInStreamError)
                    {
                        // read more from the input stream and buffer it
                        result = this.innerReader.Read();

                        // Add the new node to the end
                        this.AddNewNodeToTheEndOfBufferedNodesList(this.innerReader.NodeType, this.innerReader.GetValue());
                    }
                    else
                    {
                        // read the next node from the input stream and check
                        // whether it is an in-stream error
                        result = this.ReadNextAndCheckForInStreamError();
                    }
                }
            }
            else
            {
                if (this.bufferedNodesHead == null)
                {
                    // if parsingInStreamError nothing in the buffer; read from the base,
                    // else read the next node from the input stream and check
                    // whether it is an in-stream error
                    result = this.parsingInStreamError ? this.innerReader.Read() : this.ReadNextAndCheckForInStreamError();
                }
                else
                {
                    Debug.Assert(!this.parsingInStreamError, "!this.parsingInStreamError");

                    // non-buffering read from the buffer
                    result = this.bufferedNodesHead.NodeType != JsonNodeType.EndOfInput;
                    this.removeOnNextRead = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Called whenever we find a new object value in the payload.
        /// The base class implementation reads ahead and tries to parse it as an in-stream error payload. If it finds one it will throw it.
        /// </summary>
        /// <remarks>
        /// This method is called when the reader is in the buffering mode and can read ahead (buffering) as much as it needs to
        /// once it returns the reader will be returned to the position before the method was called.
        /// The reader is always positioned on a start object when this method is called.
        /// </remarks>
        protected virtual void ProcessObjectValue()
        {
            Debug.Assert(this.currentBufferedNode.NodeType == JsonNodeType.StartObject, "this.currentBufferedNode.NodeType == JsonNodeType.StartObject");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();

            ODataError error = null;

            // only check for in-stream errors if the buffering reader is told to do so
            if (!this.DisableInStreamErrorDetection)
            {
                // move to the first property of the potential error object (or the end-object node if no properties exist)
                this.ReadInternal();

                // we only consider this to be an in-stream error if the object has a single 'error' property
                bool errorPropertyFound = false;
                while (this.currentBufferedNode.NodeType == JsonNodeType.Property)
                {
                    // NOTE: the JSON reader already ensures that the value of a property node (which is the name of the property) is a string
                    string propertyName = (string)this.currentBufferedNode.Value;

                    // if we found any other property than the expected in-stream error property, we don't treat it as an in-stream error
                    if (string.CompareOrdinal(this.inStreamErrorPropertyName, propertyName) != 0 || errorPropertyFound)
                    {
                        return;
                    }

                    errorPropertyFound = true;

                    // position the reader over the property value
                    this.ReadInternal();

                    if (!this.TryReadInStreamErrorPropertyValue(out error))
                    {
                        // This means we thought we saw an in-stream error, but then
                        // we didn't see an intelligible error object, so we give up an reading the in-stream error
                        // and return. We will fail later in some other way. This payload is totally messed up.
                        return;
                    }
                }

                if (errorPropertyFound)
                {
                    throw new ODataErrorException(error);
                }
            }
        }

        /// <summary>
        /// Asynchronously reads the next node from the input. If we have still nodes in the buffer, takes the node
        /// from there. Otherwise reads a new node from the underlying reader and buffers it (depending on the current mode).
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true if a new node was found, or false if end of input was reached.</returns>
        /// <remarks>
        /// If the parsingInStreamError field is true, the method will read ahead for every StartObject node read from the input to check whether the JSON object
        /// represents an in-stream error. If so, it throws an <see cref="ODataErrorException"/>. If false, this check will not happen.
        /// This parsingInStreamError field is set to true when trying to parse an in-stream error; in normal operation it is false.
        /// </remarks>
        protected Task<bool> ReadInternalAsync()
        {
            this.AssertAsynchronous();

            if (this.removeOnNextRead)
            {
                Debug.Assert(this.bufferedNodesHead != null, "If removeOnNextRead is true we must have at least one node in the buffer.");

                this.RemoveFirstNodeInBuffer();
                this.removeOnNextRead = false;
            }

            // Fast path: non-buffering replay from buffer
            if (!this.isBuffering && this.bufferedNodesHead != null)
            {
                Debug.Assert(!this.parsingInStreamError, "!this.parsingInStreamError");

                // Non-buffering read from the buffer
                bool result = this.bufferedNodesHead.NodeType != JsonNodeType.EndOfInput;
                this.removeOnNextRead = true;

                return Task.FromResult(result);
            }

            // Fast path: still walking inside buffer
            if (this.isBuffering)
            {
                Debug.Assert(this.currentBufferedNode != null, "this.currentBufferedNode != null");

                if (this.currentBufferedNode.Next != this.bufferedNodesHead)
                {
                    this.currentBufferedNode = this.currentBufferedNode.Next;
                    return Task.FromResult(true);
                }
            }

            return ReadInternalLocalAsync(this);

            static Task<bool> ReadInternalLocalAsync(BufferingJsonReader thisParam)
            {
                if (thisParam.isBuffering)
                {
                    Debug.Assert(thisParam.currentBufferedNode != null, "thisParam.currentBufferedNode != null");

                    if (thisParam.parsingInStreamError)
                    {
                        // Read more from the input stream and buffer it
                        Task<bool> innerReadTask = thisParam.innerReader.ReadAsync();
                        if (!innerReadTask.IsCompletedSuccessfully)
                        {
                            return AwaitInnerReaderAsync(thisParam, innerReadTask);
                        }

                        bool localResult = innerReadTask.Result;
                        if (!localResult)
                        {
                            return Task.FromResult(false);
                        }

                        return BufferValueIfNeededAsync(thisParam);
                    }
                    else
                    {
                        // Read the next node from the input stream and check
                        // whether it is an in-stream error
                        Task<bool> checkForInStreamErrorTask = thisParam.ReadNextAndCheckForInStreamErrorAsync();
                        if (checkForInStreamErrorTask.IsCompletedSuccessfully)
                        {
                            return Task.FromResult(checkForInStreamErrorTask.Result);
                        }

                        return checkForInStreamErrorTask;
                    }
                }

                // Non-buffering + no buffered nodes yet
                if (thisParam.bufferedNodesHead == null)
                {
                    // If parsingInStreamError nothing in the buffer; read from the base,
                    // else read the next node from the input stream and check
                    // whether it is an in-stream error
                    if (thisParam.parsingInStreamError)
                    {
                        Task<bool> readTask = thisParam.innerReader.ReadAsync();
                        if (!readTask.IsCompletedSuccessfully)
                        {
                            return readTask;
                        }

                        return Task.FromResult(readTask.Result);
                    }
                    else
                    {
                        Task<bool> checkForInStreamErrorTask = thisParam.ReadNextAndCheckForInStreamErrorAsync();

                        if (checkForInStreamErrorTask.IsCompletedSuccessfully)
                        {
                            return Task.FromResult(checkForInStreamErrorTask.Result);
                        }

                        return checkForInStreamErrorTask;
                    }
                }

                // Should not reach here - buffered replay handled above
                bool result = thisParam.bufferedNodesHead.NodeType != JsonNodeType.EndOfInput;
                thisParam.removeOnNextRead = true;

                return Task.FromResult(result);
            }

            static Task<bool> BufferValueIfNeededAsync(BufferingJsonReader thisParam)
            {
                object value = null;
                JsonNodeType localNodeType = thisParam.innerReader.NodeType;
                // GetValueAsync returns null for everything other than primitive value and property nameof
                // This check should help us avoid pointless calls
                // TODO: Verify correctness of this check
                if (localNodeType == JsonNodeType.PrimitiveValue || localNodeType == JsonNodeType.Property)
                {
                    Task<object> getValueTask = thisParam.innerReader.GetValueAsync();
                    if (!getValueTask.IsCompletedSuccessfully)
                    {
                        return AwaitGetValueAsync(thisParam, getValueTask);
                    }

                    value = getValueTask.Result;
                }

                thisParam.AddNewNodeToTheEndOfBufferedNodesList(thisParam.innerReader.NodeType, value);

                return Task.FromResult(true);
            }

            static async Task<bool> AwaitInnerReaderAsync(BufferingJsonReader thisParam, Task<bool> innerReadTask)
            {
                if (!await innerReadTask.ConfigureAwait(false))
                {
                    return false;
                }

                object value = null;
                JsonNodeType localNodeType = thisParam.innerReader.NodeType;
                // GetValueAsync returns null for everything other than primitive value and property nameof
                // This check should help us avoid pointless calls
                // TODO: Verify correctness of this check
                if (localNodeType == JsonNodeType.PrimitiveValue || localNodeType == JsonNodeType.Property)
                {
                    value = await thisParam.innerReader.GetValueAsync().ConfigureAwait(false);
                }

                // Add new node to the end
                thisParam.AddNewNodeToTheEndOfBufferedNodesList(thisParam.innerReader.NodeType, value);

                return true;
            }

            static async Task<bool> AwaitGetValueAsync(BufferingJsonReader thisParam, Task<object> getValueTask)
            {
                object value = await getValueTask.ConfigureAwait(false);
                // Add new node to the end
                thisParam.AddNewNodeToTheEndOfBufferedNodesList(thisParam.innerReader.NodeType, value);

                return true;
            }
        }

        /// <summary>
        /// Called asynchronously whenever we find a new object value in the payload.
        /// The base class implementation reads ahead and tries to parse it as an in-stream error payload. If it finds one it will throw it.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <remarks>
        /// This method is called when the reader is in the buffering mode and can read ahead (buffering) as much as it needs to
        /// once it returns the reader will be returned to the position before the method was called.
        /// The reader is always positioned on a start object when this method is called.
        /// </remarks>
        protected virtual Task ProcessObjectValueAsync()
        {
            Debug.Assert(this.currentBufferedNode.NodeType == JsonNodeType.StartObject, "this.currentBufferedNode.NodeType == JsonNodeType.StartObject");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();
            this.AssertAsynchronous();

            // Only check for in-stream errors if the buffering reader is told to do so
            if (this.DisableInStreamErrorDetection)
            {
                return Task.CompletedTask;
            }

            // Move to the first property of the potential error object (or the EndObject node if no properties exist)
            Task<bool> readInternalTask = this.ReadInternalAsync();
            if (!readInternalTask.IsCompletedSuccessfully)
            {
                return AwaitReadInternalAsync(this, readInternalTask);
            }

            // Fast path: ReadInternalAsync completed synchronously
            return this.DetectAndTryReadInStreamErrorAsync();

            static async Task AwaitReadInternalAsync(BufferingJsonReader thisParam, Task<bool> readInternalTask)
            {
                await readInternalTask.ConfigureAwait(false);
                await thisParam.DetectAndTryReadInStreamErrorAsync();
            }
        }

        /// <summary>
        /// Checks the current buffered object (already positioned at its first child)
        /// for exactly one matching in-stream error property and parses it.
        /// </summary>
        /// <returns>A faulted task on success (error detected); otherwise a completed task.</returns>
        private Task DetectAndTryReadInStreamErrorAsync()
        {
            // We only consider this to be an in-stream error if the object has a single 'error' property
            if (this.currentBufferedNode.NodeType != JsonNodeType.Property)
            {
                return Task.CompletedTask;
            }

            // NOTE: The JSON reader already ensures that the value of a property node (which is the name of the property) is a string
            // First (and only candidate) property must match the in-stream error
            string propertyName = (string)this.currentBufferedNode.Value;
            if (string.CompareOrdinal(this.inStreamErrorPropertyName, propertyName) != 0)
            {
                return Task.CompletedTask;
            }

            // Advance to property value and read error object
            ValueTask<(bool IsReadSuccessfully, ODataError Error)> tryReadInStreamErrorTask = this.TryReadInStreamErrorAsync();
            if (!tryReadInStreamErrorTask.IsCompletedSuccessfully)
            {
                return AwaitTryReadInStreamErrorAsync(this, tryReadInStreamErrorTask);
            }

            (bool isReadSuccessfully, ODataError odataError) = tryReadInStreamErrorTask.Result;
            if (!isReadSuccessfully)
            {
                // This means we thought we saw an in-stream error, but then
                // we didn't see an intelligible error object, so we give up on reading the in-stream error
                // and return. We will fail later in some other way. This payload is totally messed up.
                return Task.CompletedTask;
            }

            // If we found any other property than the expected in-stream error property, we don't treat it as an in-stream error
            // This check preserves same semantic as the while loop in the synchronous ProcessObjectValue
            if (this.currentBufferedNode.NodeType == JsonNodeType.Property)
            {
                return Task.CompletedTask;
            }

            // Single in-stream error property, valid error object, return faulted Task
            return Task.FromException(new ODataErrorException(odataError));

            static async Task AwaitTryReadInStreamErrorAsync(
                BufferingJsonReader thisParam,
                ValueTask<(bool IsReadSuccessfully, ODataError Error)> pendingTryReadInStreamErrorTask)
            {
                (bool isReadSuccessfully, ODataError odataError) = await pendingTryReadInStreamErrorTask.ConfigureAwait(false);
                if (!isReadSuccessfully)
                {
                    return;
                }

                // Additional property invalidates the object as in-stream error
                if (thisParam.currentBufferedNode.NodeType == JsonNodeType.Property)
                {
                    return;
                }

                throw new ODataErrorException(odataError);
            }
        }

        /// <summary>
        /// Advances from the error property name to its value and attempts to parse
        /// the value as an OData error object.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a success flag and the parsed error.
        /// </returns>
        private ValueTask<(bool IsReadSuccessfully, ODataError Error)> TryReadInStreamErrorAsync()
        {
            // Position the reader over the property value
            Task<bool> readInternalTask = this.ReadInternalAsync();
            if (!readInternalTask.IsCompletedSuccessfully)
            {
                return AwaitReadInternalAsync(this, readInternalTask);
            }

            // Read error object
            ValueTask<(bool IsReadSuccessfully, ODataError Error)> tryReadInStreamErrorTask = this.TryReadInStreamErrorPropertyValueAsync();
            if (!tryReadInStreamErrorTask.IsCompletedSuccessfully)
            {
                return tryReadInStreamErrorTask; // Caller will await
            }

            return ValueTask.FromResult(tryReadInStreamErrorTask.Result);

            static async ValueTask<(bool, ODataError)> AwaitReadInternalAsync(BufferingJsonReader thisParam, Task<bool> pendingReadInternalTask)
            {
                await pendingReadInternalTask.ConfigureAwait(false);

                return await thisParam.TryReadInStreamErrorPropertyValueAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Reads the next node from the JSON reader and if a start-object node is detected starts reading ahead and
        /// tries to parse an in-stream error.
        /// </summary>
        /// <returns>true if a new node was found, or false if end of input was reached.</returns>
        private bool ReadNextAndCheckForInStreamError()
        {
            Debug.Assert(!this.parsingInStreamError, "!this.parsingInStreamError");

            this.parsingInStreamError = true;

            try
            {
                // read the next node in the current reader mode (buffering or non-buffering)
                bool result = this.ReadInternal();

                if (this.innerReader.NodeType == JsonNodeType.StartObject)
                {
                    // If we find a StartObject node we have to read ahead and check whether this
                    // JSON object represents an in-stream error. If we are currently in buffering
                    // mode remember the current position in the buffer; otherwise start buffering.
                    bool wasBuffering = this.isBuffering;
                    BufferedNode storedPosition = null;
                    if (this.isBuffering)
                    {
                        storedPosition = this.currentBufferedNode;
                    }
                    else
                    {
                        this.StartBuffering();
                    }

                    this.ProcessObjectValue();

                    // Reset the reader state to non-buffering or to the previously
                    // backed up position in the buffer.
                    if (wasBuffering)
                    {
                        this.currentBufferedNode = storedPosition;
                    }
                    else
                    {
                        this.StopBuffering();
                    }
                }

                return result;
            }
            finally
            {
                this.parsingInStreamError = false;
            }
        }

        /// <summary>
        /// Try to read an error structure from the stream. Return null if no error structure can be read.
        /// </summary>
        /// <param name="error">An <see cref="ODataError"/> instance that was read from the reader or null if none could be read.</param>
        /// <returns>true if an <see cref="ODataError"/> instance that was read; otherwise false.</returns>
        private bool TryReadInStreamErrorPropertyValue(out ODataError error)
        {
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();

            error = null;

            // we expect a start-object node here
            if (this.currentBufferedNode.NodeType != JsonNodeType.StartObject)
            {
                return false;
            }

            // read the start-object node
            this.ReadInternal();

            error = new ODataError();

            // we expect one of the supported properties for the value (or end-object)
            ODataJsonReaderUtils.ErrorPropertyBitMask propertiesFoundBitmask = ODataJsonReaderUtils.ErrorPropertyBitMask.None;
            while (this.currentBufferedNode.NodeType == JsonNodeType.Property)
            {
                // NOTE the Json reader already ensures that the value of a property node is a string
                string propertyName = (string)this.currentBufferedNode.Value;
                switch (propertyName)
                {
                    case JsonConstants.ODataErrorCodeName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitmask, ODataJsonReaderUtils.ErrorPropertyBitMask.Code))
                        {
                            return false;
                        }

                        string errorCode;
                        if (this.TryReadErrorStringPropertyValue(out errorCode))
                        {
                            error.Code = errorCode;
                        }
                        else
                        {
                            return false;
                        }

                        break;

                    case JsonConstants.ODataErrorMessageName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitmask, ODataJsonReaderUtils.ErrorPropertyBitMask.Message))
                        {
                            return false;
                        }

                        string errorMessage;
                        if (this.TryReadErrorStringPropertyValue(out errorMessage))
                        {
                            error.Message = errorMessage;
                        }
                        else
                        {
                            return false;
                        }

                        break;

                    case JsonConstants.ODataErrorTargetName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                            ref propertiesFoundBitmask,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.Target))
                        {
                            return false;
                        }

                        string errorTarget;
                        if (this.TryReadErrorStringPropertyValue(out errorTarget))
                        {
                            error.Target = errorTarget;
                        }
                        else
                        {
                            return false;
                        }

                        break;

                    case JsonConstants.ODataErrorDetailsName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                                ref propertiesFoundBitmask,
                                ODataJsonReaderUtils.ErrorPropertyBitMask.Details))
                        {
                            return false;
                        }

                        ICollection<ODataErrorDetail> details;
                        if (!this.TryReadErrorDetailsPropertyValue(out details))
                        {
                            return false;
                        }

                        error.Details = details;
                        break;

                    case JsonConstants.ODataErrorInnerErrorName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitmask, ODataJsonReaderUtils.ErrorPropertyBitMask.InnerError))
                        {
                            return false;
                        }

                        ODataInnerError innerError;
                        if (!this.TryReadInnerErrorPropertyValue(out innerError, 0 /*recursionDepth */))
                        {
                            return false;
                        }

                        error.InnerError = innerError;
                        break;

                    default:
                        // if we find a non-supported property we don't treat this as an error
                        return false;
                }

                this.ReadInternal();
            }

            // read the end object
            Debug.Assert(this.currentBufferedNode.NodeType == JsonNodeType.EndObject, "this.currentBufferedNode.NodeType == JsonNodeType.EndObject");
            this.ReadInternal();

            // if we don't find any properties it is not a valid error object
            return propertiesFoundBitmask != ODataJsonReaderUtils.ErrorPropertyBitMask.None;
        }

        private bool TryReadErrorDetailsPropertyValue(out ICollection<ODataErrorDetail> details)
        {
            Debug.Assert(
                this.currentBufferedNode.NodeType == JsonNodeType.Property,
                "this.currentBufferedNode.NodeType == JsonNodeType.Property");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();

            // move the reader onto the property value
            this.ReadInternal();

            // we expect a start-array node here
            if (this.currentBufferedNode.NodeType != JsonNodeType.StartArray)
            {
                details = null;
                return false;
            }

            // [
            ReadInternal();

            details = new List<ODataErrorDetail>();

            while (this.currentBufferedNode.NodeType != JsonNodeType.EndArray)
            {
                ODataErrorDetail detail;
                if (!TryReadErrorDetail(out detail))
                {
                    return false;
                }

                details.Add(detail);
                // ] or { (next error detail object)
                ReadInternal();
            }

            return true;
        }

        private bool TryReadErrorDetail(out ODataErrorDetail detail)
        {
            Debug.Assert(
                this.currentBufferedNode.NodeType == JsonNodeType.StartObject,
                "this.currentBufferedNode.NodeType == JsonNodeType.StartObject");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();

            if (this.currentBufferedNode.NodeType != JsonNodeType.StartObject)
            {
                detail = null;
                return false;
            }

            // {
            ReadInternal();

            detail = new ODataErrorDetail();

            // we expect one of the supported properties for the value (or end-object)
            var propertiesFoundBitmask = ODataJsonReaderUtils.ErrorPropertyBitMask.None;
            while (this.currentBufferedNode.NodeType == JsonNodeType.Property)
            {
                var propertyName = (string)this.currentBufferedNode.Value;

                switch (propertyName)
                {
                    case JsonConstants.ODataErrorCodeName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                            ref propertiesFoundBitmask,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.Code))
                        {
                            return false;
                        }

                        string code;
                        if (this.TryReadErrorStringPropertyValue(out code))
                        {
                            detail.Code = code;
                        }
                        else
                        {
                            return false;
                        }

                        break;

                    case JsonConstants.ODataErrorTargetName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                            ref propertiesFoundBitmask,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.Target))
                        {
                            return false;
                        }

                        string target;
                        if (this.TryReadErrorStringPropertyValue(out target))
                        {
                            detail.Target = target;
                        }
                        else
                        {
                            return false;
                        }

                        break;

                    case JsonConstants.ODataErrorMessageName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                            ref propertiesFoundBitmask,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.MessageValue))
                        {
                            return false;
                        }

                        string message;
                        if (this.TryReadErrorStringPropertyValue(out message))
                        {
                            detail.Message = message;
                        }
                        else
                        {
                            return false;
                        }

                        break;

                    default:
                        // if we find a non-supported property in an inner error, we skip it
                        this.SkipValueInternal();
                        break;
                }

                this.ReadInternal();
            }

            Debug.Assert(
                this.currentBufferedNode.NodeType == JsonNodeType.EndObject,
                "this.currentBufferedNode.NodeType == JsonNodeType.EndObject");

            return true;
        }

        /// <summary>
        /// Try to read an inner error property value.
        /// </summary>
        /// <param name="innerError">An <see cref="ODataInnerError"/> instance that was read from the reader or null if none could be read.</param>
        /// <param name="recursionDepth">The number of times this method has been called recursively.</param>
        /// <returns>true if an <see cref="ODataInnerError"/> instance that was read; otherwise false.</returns>
        private bool TryReadInnerErrorPropertyValue(out ODataInnerError innerError, int recursionDepth)
        {
            Debug.Assert(this.currentBufferedNode.NodeType == JsonNodeType.Property, "this.currentBufferedNode.NodeType == JsonNodeType.Property");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();

            ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, this.maxInnerErrorDepth);

            // move the reader onto the property value
            this.ReadInternal();

            // we expect a start-object node here
            if (this.currentBufferedNode.NodeType != JsonNodeType.StartObject)
            {
                innerError = null;
                return false;
            }

            // read the start-object node
            this.ReadInternal();

            innerError = new ODataInnerError();

            // we expect one of the supported properties for the value (or end-object)
            ODataJsonReaderUtils.ErrorPropertyBitMask propertiesFoundBitmask = ODataJsonReaderUtils.ErrorPropertyBitMask.None;
            while (this.currentBufferedNode.NodeType == JsonNodeType.Property)
            {
                // NOTE the Json reader already ensures that the value of a property node is a string
                string propertyName = (string)this.currentBufferedNode.Value;

                switch (propertyName)
                {
                    case JsonConstants.ODataErrorInnerErrorMessageName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitmask, ODataJsonReaderUtils.ErrorPropertyBitMask.MessageValue))
                        {
                            return false;
                        }

                        string message;
                        if (this.TryReadErrorStringPropertyValue(out message))
                        {
                            innerError.Properties.Add(JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue(message));
                        }
                        else
                        {
                            return false;
                        }

                        break;

                    case JsonConstants.ODataErrorInnerErrorTypeNameName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitmask, ODataJsonReaderUtils.ErrorPropertyBitMask.TypeName))
                        {
                            return false;
                        }

                        string typeName;
                        if (this.TryReadErrorStringPropertyValue(out typeName))
                        {
                            innerError.Properties.Add(JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue(typeName));
                        }
                        else
                        {
                            return false;
                        }

                        break;

                    case JsonConstants.ODataErrorInnerErrorStackTraceName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitmask, ODataJsonReaderUtils.ErrorPropertyBitMask.StackTrace))
                        {
                            return false;
                        }

                        string stackTrace;
                        if (this.TryReadErrorStringPropertyValue(out stackTrace))
                        {
                            innerError.Properties.Add(JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue(stackTrace));
                        }
                        else
                        {
                            return false;
                        }

                        break;

                    case JsonConstants.ODataErrorInnerErrorInnerErrorName:
                    case JsonConstants.ODataErrorInnerErrorName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitmask, ODataJsonReaderUtils.ErrorPropertyBitMask.InnerError))
                        {
                            return false;
                        }

                        ODataInnerError nestedInnerError;
                        if (this.TryReadInnerErrorPropertyValue(out nestedInnerError, recursionDepth))
                        {
                            innerError.InnerError = nestedInnerError;
                        }
                        else
                        {
                            return false;
                        }

                        break;

                    default:
                        // if we find a non-supported property in an inner error, we skip it
                        this.SkipValueInternal();
                        break;
                }

                this.ReadInternal();
            }

            Debug.Assert(this.currentBufferedNode.NodeType == JsonNodeType.EndObject, "this.currentBufferedNode.NodeType == JsonNodeType.EndObject");

            return true;
        }

        /// <summary>
        /// Reads the string value of a property.
        /// </summary>
        /// <param name="stringValue">The string value read if the method returns true; otherwise null.</param>
        /// <returns>true if a string value (or null) was read as property value of the current property; otherwise false.</returns>
        private bool TryReadErrorStringPropertyValue(out string stringValue)
        {
            Debug.Assert(this.currentBufferedNode.NodeType == JsonNodeType.Property, "this.currentBufferedNode.NodeType == JsonNodeType.Property");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();

            this.ReadInternal();

            // we expect a string value
            stringValue = this.currentBufferedNode.Value as string;
            return this.currentBufferedNode.NodeType == JsonNodeType.PrimitiveValue && (this.currentBufferedNode.Value == null || stringValue != null);
        }

        /// <summary>
        /// Skips over a JSON value (primitive, object or array) while parsing in-stream errors.
        /// Note that the SkipValue extension method can not be used in this case as this method has to
        /// access the base instance's NodeType and call ReadInternal.
        /// </summary>
        /// <remarks>
        /// Pre-Condition: JsonNodeType.PrimitiveValue, JsonNodeType.StartArray or JsonNodeType.StartObject
        /// Post-Condition: JsonNodeType.PrimitiveValue, JsonNodeType.EndArray or JsonNodeType.EndObject
        /// </remarks>
        private void SkipValueInternal()
        {
            int depth = 0;
            do
            {
                depth = AdjustDepth(depth, this.currentBufferedNode.NodeType);

                this.ReadInternal();
            }
            while (depth > 0);
        }

        /// <summary>
        /// Removes the head node from the buffer.
        /// </summary>
        private void RemoveFirstNodeInBuffer()
        {
            BufferedNode firstNodeInBuffer = this.bufferedNodesHead;

            if (this.bufferedNodesHead.Next == this.bufferedNodesHead)
            {
                Debug.Assert(this.bufferedNodesHead.Previous == this.bufferedNodesHead, "The linked list is corrupted.");
                this.bufferedNodesHead = null;
            }
            else
            {
                this.bufferedNodesHead.Previous.Next = this.bufferedNodesHead.Next;
                this.bufferedNodesHead.Next.Previous = this.bufferedNodesHead.Previous;
                this.bufferedNodesHead = this.bufferedNodesHead.Next;
            }

            ReturnNode(firstNodeInBuffer);
        }

        /// <summary>
        /// Asynchronously reads the next node from the JSON reader and if a start-object node is detected starts reading ahead and
        /// tries to parse an in-stream error.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true if a new node was found, or false if end of input was reached.</returns>
        [SuppressMessage("Design", "CA1031:Do not catch general exception types",
            Justification = "We must (1) reset parsingInStreamError on synchronous failure, and (2) preserve the contract of returning a faulted Task<bool> instead of throwing synchronously.")]
        private Task<bool> ReadNextAndCheckForInStreamErrorAsync()
        {
            Debug.Assert(!this.parsingInStreamError, "!this.parsingInStreamError");
            this.AssertAsynchronous();

            this.parsingInStreamError = true;

            try
            {
                Task<bool> readInternalTask = ReadInternalAsync();
                if (!readInternalTask.IsCompletedSuccessfully)
                {
                    // Asynchronous completion path; continuation will reset parsingInStreamError and fault the Task if needed
                    return AwaitReadInternalAsync(this, readInternalTask);
                }

                // Synchronous completion path
                return ContinueAfterReadInternalAsync(this, readInternalTask.Result);
            }
            catch (Exception ex)
            {
                this.parsingInStreamError = false;
                return Task.FromException<bool>(ex);
            }

            static Task<bool> ContinueAfterReadInternalAsync(BufferingJsonReader thisParam, bool innerResult)
            {
                // If not StartObject we can finish immediately.
                if (thisParam.innerReader.NodeType != JsonNodeType.StartObject)
                {
                    thisParam.parsingInStreamError = false;
                    return Task.FromResult(innerResult);
                }

                // If we find a StartObject node we have to read ahead and check whether this
                // JSON object represents an in-stream error.
                Task checkObjectForInStreamErrorTask;

                try
                {
                    checkObjectForInStreamErrorTask = thisParam.ReadObjectAndCheckForInStreamErrorAsync();
                }
                catch (Exception ex)
                {
                    thisParam.parsingInStreamError = false;
                    return Task.FromException<bool>(ex);
                }

                if (checkObjectForInStreamErrorTask.IsCompletedSuccessfully)
                {
                    // Completed synchronously
                    thisParam.parsingInStreamError = false;
                    return Task.FromResult(innerResult);
                }

                // Asynchronous completion path; continuation will reset this.parsingInStreamError and fault the Task if needed
                return AwaitReadObjectAndCheckForInStreamErrorAsync(thisParam, checkObjectForInStreamErrorTask, innerResult);
            }

            static async Task<bool> AwaitReadInternalAsync(BufferingJsonReader thisParam, Task<bool> pendingReadInternalTask)
            {
                // Any exception propagates as faulted task since method is async
                bool innerResult;

                try
                {
                    innerResult = await pendingReadInternalTask.ConfigureAwait(false);
                }
                catch
                {
                    thisParam.parsingInStreamError = false;
                    throw;
                }

                // May return a completed Task or an async path
                bool finalResult = await ContinueAfterReadInternalAsync(thisParam, innerResult).ConfigureAwait(false);
                Debug.Assert(!thisParam.parsingInStreamError, "parsingInStreamError must be false after ContinueAfterReadInternalAsync.");

                return finalResult;
            }

            static async Task<bool> AwaitReadObjectAndCheckForInStreamErrorAsync(BufferingJsonReader thisParam, Task pendingCheckObjectForInStreamErrorTask, bool result)
            {
                // Any exception propagates as faulted task since method is async
                try
                {
                    await pendingCheckObjectForInStreamErrorTask.ConfigureAwait(false);
                    return result;
                }
                finally
                {
                    thisParam.parsingInStreamError = false;
                }
            }
        }

        /// <summary>
        /// Given the inner reader just produced a StartObject, buffer it (if needed),
        /// probe it for a single in-stream error property, and restore the original buffered position.
        /// </summary>
        /// <returns>A completed or faulted task that represents the asynchronous read operation.</returns>
        private Task ReadObjectAndCheckForInStreamErrorAsync()
        {
            Debug.Assert(
                this.innerReader.NodeType == JsonNodeType.StartObject,
                "this.innerReader.NodeType == JsonNodeType.StartObject");

            BufferedNode storedPosition = null;

            // If we are currently in buffering mode remember the current position in the buffer;
            // otherwise start buffering.
            if (this.isBuffering)
            {
                // Already buffering; remember position - restore on success
                storedPosition = this.currentBufferedNode;

                Task processObjectValueTask = this.ProcessObjectValueAsync();
                if (processObjectValueTask.IsCompletedSuccessfully)
                {
                    // Completed synchronously
                    this.currentBufferedNode = storedPosition;
                    return Task.CompletedTask;
                }

                return AwaitProcessObjectValueAsync(this, storedPosition, processObjectValueTask);
            }
            else
            {
                // Need to start buffering first
                Task startBufferingTask = this.StartBufferingAsync();
                if (!startBufferingTask.IsCompletedSuccessfully)
                {
                    return AwaitStartBufferingThenProcessObjectValueAsync(this, startBufferingTask);
                }

                // StartBufferingAsync completed synchronously
                Task processObjectValueTask = this.ProcessObjectValueAsync();
                if (processObjectValueTask.IsCompletedSuccessfully)
                {
                    // Fast path
                    this.StopBuffering();
                    return Task.CompletedTask;
                }

                return AwaitProcessObjectValueThenStopBufferingAsync(this, processObjectValueTask);
            }

            // We were already buffering. Only restore position on success; on fault keep the advanced position - preserve semantics before refactor
            static async Task AwaitProcessObjectValueAsync(
                BufferingJsonReader thisParam,
                BufferedNode storedPosition,
                Task pendingProcessObjectValueTask)
            {
                await pendingProcessObjectValueTask.ConfigureAwait(false);
                thisParam.currentBufferedNode = storedPosition;
            }

            // If start buffering faults, we propagate and leave buffering turned on - preserve semantics before refactor
            static async Task AwaitStartBufferingThenProcessObjectValueAsync(BufferingJsonReader thisParam, Task pendingStartBufferingTask)
            {
                await pendingStartBufferingTask.ConfigureAwait(false);

                Task processObjectValueTask = thisParam.ProcessObjectValueAsync();
                if (!processObjectValueTask.IsCompletedSuccessfully)
                {
                    await processObjectValueTask.ConfigureAwait(false);
                }

                // On success stop buffering
                thisParam.StopBuffering();
            }

            static async Task AwaitProcessObjectValueThenStopBufferingAsync(BufferingJsonReader thisParam, Task pendingProcessObjectValueTask)
            {
                await pendingProcessObjectValueTask.ConfigureAwait(false);
                thisParam.StopBuffering();
            }
        }

        /// <summary>
        /// Asynchronously try to read an error object from the stream.
        /// </summary>
        /// <param name="error">An <see cref="ODataError"/> instance that was read from the reader or null if none could be read.</param>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a tuple comprising of:
        /// 1). A value of true if an <see cref="ODataError"/> instance that was read; otherwise false.
        /// 2). An <see cref="ODataError"/> instance that was read from the reader or null if none could be read.</returns>
        private async ValueTask<(bool IsReadSuccessfully, ODataError Error)> TryReadInStreamErrorPropertyValueAsync()
        {
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();
            this.AssertAsynchronous();

            // We expect a StartObject node here
            if (this.currentBufferedNode.NodeType != JsonNodeType.StartObject)
            {
                return (false, null);
            }

            // Read the StartObject node
            await this.ReadInternalAsync()
                .ConfigureAwait(false);

            ODataError error = new ODataError();

            // We expect one of the supported properties for the value (or EndObject)
            ODataJsonReaderUtils.ErrorPropertyBitMask propertiesFoundBitmask = ODataJsonReaderUtils.ErrorPropertyBitMask.None;
            while (this.currentBufferedNode.NodeType == JsonNodeType.Property)
            {
                // NOTE: The JSON reader already ensures that the value of a property node is a string
                string propertyName = (string)this.currentBufferedNode.Value;
                switch (propertyName)
                {
                    case JsonConstants.ODataErrorCodeName:
                        {
                            (bool isReadSuccessfully, string propertyValue) = await this.ValidateAndTryReadErrorStringPropertyValueAsync(
                                ODataJsonReaderUtils.ErrorPropertyBitMask.Code,
                                ref propertiesFoundBitmask).ConfigureAwait(false);

                            if (!isReadSuccessfully)
                            {
                                return (false, error);
                            }

                            error.Code = propertyValue;

                            break;
                        }

                    case JsonConstants.ODataErrorMessageName:
                        {
                            (bool isReadSuccessfully, string propertyValue) = await this.ValidateAndTryReadErrorStringPropertyValueAsync(
                                ODataJsonReaderUtils.ErrorPropertyBitMask.Message,
                                ref propertiesFoundBitmask).ConfigureAwait(false);
                            if (!isReadSuccessfully)
                            {
                                return (false, error);
                            }

                            error.Message = propertyValue;

                            break;
                        }

                    case JsonConstants.ODataErrorTargetName:
                        {
                            (bool isReadSuccessfully, string propertyValue) = await this.ValidateAndTryReadErrorStringPropertyValueAsync(
                                ODataJsonReaderUtils.ErrorPropertyBitMask.Target,
                                ref propertiesFoundBitmask).ConfigureAwait(false);
                            if (!isReadSuccessfully)
                            {
                                return (false, error);
                            }

                            error.Target = propertyValue;

                            break;
                        }

                    case JsonConstants.ODataErrorDetailsName:
                        {
                            if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                                ref propertiesFoundBitmask,
                                ODataJsonReaderUtils.ErrorPropertyBitMask.Details))
                            {
                                return (false, error);
                            }

                            (bool isReadSuccessfully, List<ODataErrorDetail> errorDetails) = await this.TryReadErrorDetailsPropertyValueAsync()
                                .ConfigureAwait(false);
                            if (!isReadSuccessfully)
                            {
                                return (false, error);
                            }

                            error.Details = errorDetails;

                            break;
                        }

                    case JsonConstants.ODataErrorInnerErrorName:
                        {
                            if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                                ref propertiesFoundBitmask,
                                ODataJsonReaderUtils.ErrorPropertyBitMask.InnerError))
                            {
                                return (false, error);
                            }

                            (bool isReadSuccessfully, ODataInnerError innerError) = await this.TryReadInnerErrorPropertyValueAsync(
                                recursionDepth: 0).ConfigureAwait(false);
                            if (!isReadSuccessfully)
                            {
                                return (false, error);
                            }

                            error.InnerError = innerError;

                            break;
                        }

                    default:
                        // If we find a non-supported property we don't treat this as an error
                        return (false, error);
                }

                await this.ReadInternalAsync()
                    .ConfigureAwait(false);
            }

            // Read the end object
            Debug.Assert(
                this.currentBufferedNode.NodeType == JsonNodeType.EndObject,
                "this.currentBufferedNode.NodeType == JsonNodeType.EndObject");

            await this.ReadInternalAsync()
                .ConfigureAwait(false);

            // If we don't find any properties it is not a valid error object
            return (propertiesFoundBitmask != ODataJsonReaderUtils.ErrorPropertyBitMask.None, error);
        }

        /// <summary>
        /// Asynchronously try to read an error details collection property.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a tuple comprising of:
        /// 1). A value of true if an <see cref="ODataErrorDetail"/> collection was read; otherwise false.
        /// 2). An <see cref="ODataErrorDetail"/> collection that was read from the reader or null if none could be read.</returns>
        private async ValueTask<(bool IsReadSuccessfully, List<ODataErrorDetail> ErrorDetails)> TryReadErrorDetailsPropertyValueAsync()
        {
            Debug.Assert(
                this.currentBufferedNode.NodeType == JsonNodeType.Property,
                "this.currentBufferedNode.NodeType == JsonNodeType.Property");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();
            this.AssertAsynchronous();

            // Move the reader onto the property value
            await this.ReadInternalAsync()
                .ConfigureAwait(false);

            // We expect a StartArray node here
            if (this.currentBufferedNode.NodeType != JsonNodeType.StartArray)
            {
                return (false, null);
            }

            // [
            await ReadInternalAsync()
                .ConfigureAwait(false);

            List<ODataErrorDetail> details = new List<ODataErrorDetail>();

            while (this.currentBufferedNode.NodeType != JsonNodeType.EndArray)
            {
                (bool isReadSuccessfully, ODataErrorDetail errorDetail) = await TryReadErrorDetailAsync()
                    .ConfigureAwait(false);

                if (!isReadSuccessfully)
                {
                    return (false, details);
                }

                details.Add(errorDetail);

                // ] or { (next error detail object)
                await ReadInternalAsync()
                    .ConfigureAwait(false);
            }

            return (true, details);
        }

        /// <summary>
        /// Asynchronously try to read an error detail object.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a tuple comprising of:
        /// 1). A value of true if an <see cref="ODataErrorDetail"/> instance was read; otherwise false.
        /// 2). An <see cref="ODataErrorDetail"/> instance that was read from the reader or null if none could be read.</returns>
        private async ValueTask<(bool IsReadSuccessfully, ODataErrorDetail ErrorDetail)> TryReadErrorDetailAsync()
        {
            Debug.Assert(
                this.currentBufferedNode.NodeType == JsonNodeType.StartObject,
                "this.currentBufferedNode.NodeType == JsonNodeType.StartObject");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();
            this.AssertAsynchronous();

            if (this.currentBufferedNode.NodeType != JsonNodeType.StartObject)
            {
                return (false, null);
            }

            // {
            await ReadInternalAsync()
                .ConfigureAwait(false);

            ODataErrorDetail detail = new ODataErrorDetail();

            // We expect one of the supported properties for the value (or EndObject)
            var propertiesFoundBitmask = ODataJsonReaderUtils.ErrorPropertyBitMask.None;
            while (this.currentBufferedNode.NodeType == JsonNodeType.Property)
            {
                var propertyName = (string)this.currentBufferedNode.Value;

                switch (propertyName)
                {
                    case JsonConstants.ODataErrorCodeName:
                        {
                            (bool isReadSuccessfully, string propertyValue) = await this.ValidateAndTryReadErrorStringPropertyValueAsync(
                                ODataJsonReaderUtils.ErrorPropertyBitMask.Code,
                                ref propertiesFoundBitmask).ConfigureAwait(false);
                            if (!isReadSuccessfully)
                            {
                                return (false, detail);
                            }

                            detail.Code = propertyValue;
                        }
                        break;

                    case JsonConstants.ODataErrorTargetName:
                        {
                            (bool isReadSuccessfully, string propertyValue) = await this.ValidateAndTryReadErrorStringPropertyValueAsync(
                                ODataJsonReaderUtils.ErrorPropertyBitMask.Target,
                                ref propertiesFoundBitmask).ConfigureAwait(false);
                            if (!isReadSuccessfully)
                            {
                                return (false, detail);
                            }

                            detail.Target = propertyValue;
                        }
                        break;

                    case JsonConstants.ODataErrorMessageName:
                        {
                            (bool isReadSuccessfully, string propertyValue) = await this.ValidateAndTryReadErrorStringPropertyValueAsync(
                                ODataJsonReaderUtils.ErrorPropertyBitMask.MessageValue,
                                ref propertiesFoundBitmask).ConfigureAwait(false);
                            if (!isReadSuccessfully)
                            {
                                return (false, detail);
                            }

                            detail.Message = propertyValue;
                        }
                        break;

                    default:
                        // If we find a non-supported property in an error detail, we skip it
                        await this.SkipValueInternalAsync()
                            .ConfigureAwait(false);
                        break;
                }

                await this.ReadInternalAsync()
                    .ConfigureAwait(false);
            }

            Debug.Assert(
                this.currentBufferedNode.NodeType == JsonNodeType.EndObject,
                "this.currentBufferedNode.NodeType == JsonNodeType.EndObject");

            return (true, detail);
        }

        /// <summary>
        /// Asynchronously try to read an inner error property value.
        /// </summary>
        /// <param name="recursionDepth">The number of times this method has been called recursively.</param>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a tuple comprising of:
        /// 1). A value of true if an <see cref="ODataInnerError"/> instance was read; otherwise false.
        /// 2). An <see cref="ODataInnerError"/> instance that was read from the reader or null if none could be read.</returns>
        private async ValueTask<(bool IsReadSuccessfully, ODataInnerError InnerError)> TryReadInnerErrorPropertyValueAsync(int recursionDepth)
        {
            Debug.Assert(this.currentBufferedNode.NodeType == JsonNodeType.Property, "this.currentBufferedNode.NodeType == JsonNodeType.Property");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();
            this.AssertAsynchronous();

            ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, this.maxInnerErrorDepth);

            // Move the reader onto the property value
            await this.ReadInternalAsync()
                .ConfigureAwait(false);

            // We expect a StartObject node here
            if (this.currentBufferedNode.NodeType != JsonNodeType.StartObject)
            {
                return (false, null);
            }

            // Read the StartObject node
            await this.ReadInternalAsync()
                .ConfigureAwait(false);

            ODataInnerError innerError = new ODataInnerError();

            // We expect one of the supported properties for the value (or EndObject)
            ODataJsonReaderUtils.ErrorPropertyBitMask propertiesFoundBitmask = ODataJsonReaderUtils.ErrorPropertyBitMask.None;
            while (this.currentBufferedNode.NodeType == JsonNodeType.Property)
            {
                // NOTE: The JSON reader already ensures that the value of a property node is a string
                string propertyName = (string)this.currentBufferedNode.Value;

                switch (propertyName)
                {
                    case JsonConstants.ODataErrorInnerErrorMessageName:
                        if (!await ValidateAndTryReadInnerErrorStringPropertyValueAsync(
                            innerError,
                            JsonConstants.ODataErrorInnerErrorMessageName,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.MessageValue,
                            ref propertiesFoundBitmask).ConfigureAwait(false))
                        {
                            return (false, innerError);
                        }
                        
                        break;

                    case JsonConstants.ODataErrorInnerErrorTypeNameName:
                        if (!await ValidateAndTryReadInnerErrorStringPropertyValueAsync(
                            innerError,
                            JsonConstants.ODataErrorInnerErrorTypeNameName,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.TypeName,
                            ref propertiesFoundBitmask).ConfigureAwait(false))
                        {
                            return (false, innerError);
                        }

                        break;

                    case JsonConstants.ODataErrorInnerErrorStackTraceName:
                        if (!await ValidateAndTryReadInnerErrorStringPropertyValueAsync(
                            innerError,
                            JsonConstants.ODataErrorInnerErrorStackTraceName,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.StackTrace,
                            ref propertiesFoundBitmask).ConfigureAwait(false))
                        {
                            return (false, innerError);
                        }

                        break;

                    case JsonConstants.ODataErrorInnerErrorInnerErrorName:
                    case JsonConstants.ODataErrorInnerErrorName:
                        if (!await ValidateAndTryReadNestedInnerErrorAsync(
                            innerError,
                            ref propertiesFoundBitmask,
                            recursionDepth).ConfigureAwait(false))
                        {
                            return (false, innerError);
                        }

                        break;

                    default:
                        // If we find a non-supported property in an inner error, we skip it
                        await this.SkipValueInternalAsync()
                            .ConfigureAwait(false);
                        break;
                }

                await this.ReadInternalAsync()
                    .ConfigureAwait(false);
            }

            Debug.Assert(
                this.currentBufferedNode.NodeType == JsonNodeType.EndObject,
                "this.currentBufferedNode.NodeType == JsonNodeType.EndObject");

            return (true, innerError);
        }

        /// <summary>
        /// Validates the top-level error string property is not a duplicate, then attempts to read its string (or null) value.
        /// </summary>
        /// <param name="propertyBitMask">Bit representing this property.</param>
        /// <param name="propertiesFoundBitmask">Bitmask tracking previously seen properties (updated if successful).</param>
        /// <returns>
        /// (IsReadSuccessfully, propertyValue) where IsReadSuccessfully is false on duplicate or non-string/non-null value.
        /// </returns>
        private ValueTask<(bool IsReadSuccessfully, string propertyValue)> ValidateAndTryReadErrorStringPropertyValueAsync(
            ODataJsonReaderUtils.ErrorPropertyBitMask propertyBitMask,
            ref ODataJsonReaderUtils.ErrorPropertyBitMask propertiesFoundBitmask)
        {
            // Duplicate or already seen property?
            if (!ODataJsonReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitmask, propertyBitMask))
            {
                return ValueTask.FromResult((false, (string)null));
            }

            // TryReadErrorStringPropertyValueAsync has fast path for completed sync reads
            return this.TryReadErrorStringPropertyValueAsync();
        }

        /// <summary>
        /// Validates the inner error string property is not a duplicate, then tries to read it and add it to <paramref name="innerError"/>.
        /// </summary>
        /// <param name="innerError">Target inner error being populated.</param>
        /// <param name="propertyName">Canonical JSON property name (message/type/stacktrace).</param>
        /// <param name="propertyBitMask">Bit representing this property.</param>
        /// <param name="propertiesFoundBitmask">Bitmask tracking previously seen properties (updated if successful).</param>
        /// <returns><c>true</c> if the property was successfully read and added; otherwise <c>false</c>.</returns>
        private ValueTask<bool> ValidateAndTryReadInnerErrorStringPropertyValueAsync(
            ODataInnerError innerError,
            string propertyName,
            ODataJsonReaderUtils.ErrorPropertyBitMask propertyBitMask,
            ref ODataJsonReaderUtils.ErrorPropertyBitMask propertiesFoundBitmask)
        {
            // Duplicate or already seen property?
            if (!ODataJsonReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitmask, propertyBitMask))
            {
                return ValueTask.FromResult(false);
            }

            ValueTask<(bool IsReadSuccessfully, string PropertyValue)> readPropertyValueTask = this.TryReadErrorStringPropertyValueAsync();
            if (readPropertyValueTask.IsCompletedSuccessfully)
            {
                return ValueTask.FromResult(ProcessResult(innerError, propertyName, readPropertyValueTask.Result));
            }

            return AwaitTryReadAndSetInnerErrorStringPropertyValueAsync(this, innerError, propertyName, readPropertyValueTask);

            static bool ProcessResult(
                ODataInnerError targetInnerError,
                string targetPropertyName,
                (bool IsReadSuccessfully, string PropertyValue) result)
            {
                if (!result.IsReadSuccessfully)
                {
                    return false;
                }

                targetInnerError.Properties.Add(targetPropertyName, new ODataPrimitiveValue(result.PropertyValue));

                return true;
            }

            static async ValueTask<bool> AwaitTryReadAndSetInnerErrorStringPropertyValueAsync(
                BufferingJsonReader thisParam,
                ODataInnerError innerError,
                string propertyName,
                ValueTask<(bool IsReadSuccessfully, string PropertyValue)> pendingReadPropertyValueTask)
            {
                (bool IsReadSuccessfully, string PropertyValue) readPropertyValueTask = await pendingReadPropertyValueTask.ConfigureAwait(false);

                return ProcessResult(innerError, propertyName, readPropertyValueTask);
            }
        }

        /// <summary>
        /// Validates the nested inner error property is not a duplicate, then recursively reads and assigns the nested inner error.
        /// </summary>
        /// <param name="innerError">Target inner error receiving the nested InnerError.</param>
        /// <param name="propertiesFoundBitmask">Bitmask tracking previously seen properties (updated if successful).</param>
        /// <param name="recursionDepth">Current recursion depth (used for depth validation).</param>
        /// <returns><c>true</c> if a nested inner error was successfully read and attached; otherwise <c>false</c>.</returns>
        private ValueTask<bool> ValidateAndTryReadNestedInnerErrorAsync(
            ODataInnerError innerError,
            ref ODataJsonReaderUtils.ErrorPropertyBitMask propertiesFoundBitmask,
            int recursionDepth)
        {
            // Duplicate or already seen property?
            if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                ref propertiesFoundBitmask,
                ODataJsonReaderUtils.ErrorPropertyBitMask.InnerError))
            {
                return ValueTask.FromResult(false);
            }

            ValueTask<(bool IsReadSuccessfully, ODataInnerError InnerError)> readPropertyValueTask = this.TryReadInnerErrorPropertyValueAsync(recursionDepth);
            if (readPropertyValueTask.IsCompletedSuccessfully)
            {
                return ValueTask.FromResult(ProcessResult(innerError, readPropertyValueTask.Result));
            }

            return AwaitTryReadInnerErrorPropertyValueAsync(this, innerError, readPropertyValueTask);

            static bool ProcessResult(
                ODataInnerError targetInnerError,
                (bool IsReadSuccessfully, ODataInnerError InnerError) result)
            {
                if (!result.IsReadSuccessfully)
                {
                    return false;
                }

                targetInnerError.InnerError = result.InnerError;

                return true;
            }

            static async ValueTask<bool> AwaitTryReadInnerErrorPropertyValueAsync(
                BufferingJsonReader thisParam,
                ODataInnerError innerError,
                ValueTask<(bool IsReadSuccessfully, ODataInnerError InnerError)> pendingReadPropertyValueTask)
            {
                (bool IsReadSuccessfully, ODataInnerError InnerError) readPropertyValueTask = await pendingReadPropertyValueTask.ConfigureAwait(false);

                return ProcessResult(innerError, readPropertyValueTask);
            }
        }

        /// <summary>
        /// Asynchronously try to read the string value of a property.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a tuple comprising of:
        /// 1). A value of true if a string value (or null) was read as property value of the current property; otherwise false.
        /// 2). The string value read if the method returns true; otherwise null.</returns>
        private ValueTask<(bool IsReadSuccessfully, string PropertyValue)> TryReadErrorStringPropertyValueAsync()
        {
            Debug.Assert(this.currentBufferedNode.NodeType == JsonNodeType.Property,
                "this.currentBufferedNode.NodeType == JsonNodeType.Property");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();
            this.AssertAsynchronous();

            Task readInternalTask = this.ReadInternalAsync();
            if (readInternalTask.IsCompletedSuccessfully)
            {
                string stringValue = this.currentBufferedNode.Value as string;
                bool isReadSuccessfully = this.currentBufferedNode.NodeType == JsonNodeType.PrimitiveValue
                    && (this.currentBufferedNode.Value == null || stringValue != null);

                return ValueTask.FromResult((isReadSuccessfully, stringValue));
            }

            return AwaitReadInternalAsync(this, readInternalTask);

            static async ValueTask<(bool, string)> AwaitReadInternalAsync(BufferingJsonReader thisParam, Task pendingReadInternalTask)
            {
                await pendingReadInternalTask.ConfigureAwait(false);

                string stringValue = thisParam.currentBufferedNode.Value as string;
                bool isReadSuccessfully = thisParam.currentBufferedNode.NodeType == JsonNodeType.PrimitiveValue
                    && (thisParam.currentBufferedNode.Value == null || stringValue != null);

                return (isReadSuccessfully, stringValue);
            }
        }

        /// <summary>
        /// Asynchronously skips over a JSON value (primitive, object or array) while parsing in-stream errors.
        /// Note that the SkipValueAsync extension method can not be used in this case as this method has to
        /// access the base instance's NodeType and call ReadInternalAsync.
        /// </summary>
        /// <remarks>
        /// Pre-Condition: JsonNodeType.PrimitiveValue, JsonNodeType.StartArray or JsonNodeType.StartObject
        /// Post-Condition: JsonNodeType.PrimitiveValue, JsonNodeType.EndArray or JsonNodeType.EndObject
        /// </remarks>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        private ValueTask SkipValueInternalAsync()
        {
            this.AssertAsynchronous();

            int depth = 0;

            while (true)
            {
                depth = AdjustDepth(depth, this.currentBufferedNode.NodeType);

                Task<bool> readInternalTask = this.ReadInternalAsync();
                if (!readInternalTask.IsCompletedSuccessfully)
                {
                    return AwaitSkipValueInternalAsync(this, depth, readInternalTask);
                }

                // After completed synchronous advance, if depth is 0, we're done
                if (depth == 0)
                {
                    return ValueTask.CompletedTask;
                }
            }

            static async ValueTask AwaitSkipValueInternalAsync(BufferingJsonReader thisParam, int depth, Task<bool> pendingReadInternalTask)
            {
                while (true)
                {
                    await pendingReadInternalTask.ConfigureAwait(false);

                    if (depth == 0)
                    {
                        return;
                    }

                    depth = AdjustDepth(depth, thisParam.currentBufferedNode.NodeType);

                    pendingReadInternalTask = thisParam.ReadInternalAsync();
                }
            }
        }

        /// <summary>
        /// Adds a new node to the end of the buffered nodes list.
        /// </summary>
        /// <param name="nodeType">The node type.</param>
        /// <param name="value">The node value.</param>
        private void AddNewNodeToTheEndOfBufferedNodesList(JsonNodeType nodeType, object value)
        {
            BufferedNode newNode = RentNode(nodeType, value);
            newNode.Previous = this.bufferedNodesHead.Previous;
            newNode.Next = this.bufferedNodesHead;
            this.bufferedNodesHead.Previous.Next = newNode;
            this.bufferedNodesHead.Previous = newNode;
            this.currentBufferedNode = newNode;
        }

        /// <summary>
        /// Asserts that the buffering json reader was created for an asynchronous operation.
        /// </summary>
        [DebuggerStepThrough]
        [Conditional("DEBUG")]
        private void AssertAsynchronous()
        {
#if DEBUG
            Debug.Assert(this.innerReader != null, "The method should only be called on an asynchronous buffering json reader.");
#endif
        }

#if DEBUG
        /// <summary>
        /// Determines whether the given node exists in the current buffer.
        /// </summary>
        /// <param name="nodeToCheck">The node to test.</param>
        /// <returns>true if the given node was found in the buffer; false otherwise.</returns>
        private bool NodeExistsInCurrentBuffer(BufferedNode nodeToCheck)
        {
            BufferedNode currentNode = this.bufferedNodesHead;

            do
            {
                if (currentNode == nodeToCheck)
                {
                    return true;
                }

                currentNode = currentNode.Next;
            }
            while (currentNode != this.bufferedNodesHead);

            return false;
        }

#endif

        /// <summary>
        /// Adjusts the running nesting depth counter based on the current node type,
        /// incrementing for start scopes and decrementing for end scopes.
        /// </summary>
        /// <param name="currentDepth">The current nesting depth.</param>
        /// <param name="nodeType">The node just encountered.</param>
        /// <returns>The updated depth.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int AdjustDepth(int currentDepth, JsonNodeType nodeType)
        {
            switch (nodeType)
            {
                case JsonNodeType.PrimitiveValue:
                    return currentDepth;

                case JsonNodeType.StartArray:
                case JsonNodeType.StartObject:
                    return currentDepth + 1;

                case JsonNodeType.EndArray:
                case JsonNodeType.EndObject:
                    Debug.Assert(currentDepth > 0, "Seen too many scope ends.");
                    return currentDepth - 1;

                default:
                    Debug.Assert(
                        nodeType != JsonNodeType.EndOfInput,
                        "We should not have reached end of input, since the scopes should be well formed. Otherwise JsonReader should have failed by now.");
                    return currentDepth;
            }
        }

        /// <summary>
        /// Rents a node from the per-thread pool or creates a new instance.
        /// </summary>
        /// <param name="nodeType">The node type.</param>
        /// <param name="nodeValue">The node value.</param>
        /// <returns>A rented or newly created node.</returns>
        private static BufferedNode RentNode(JsonNodeType nodeType, object nodeValue)
        {
            Stack<BufferedNode> nodePool = bufferedNodePool;
            if (nodePool != null && nodePool.Count > 0)
            {
                BufferedNode node = nodePool.Pop();
                node.Reset(nodeType, nodeValue);

                return node;
            }

            return new BufferedNode(nodeType, nodeValue);
        }

        /// <summary>
        /// Returns a node to the per-thread pool (breaking links and clearing value to release references).
        /// </summary>
        /// <param name="node">The node to return.</param>
        private static void ReturnNode(BufferedNode node)
        {
            if (node == null)
            {
                return;
            }

            // Break object graph and value references
            node.Reset(JsonNodeType.None, null);

            Stack<BufferedNode> nodePool = bufferedNodePool ?? new Stack<BufferedNode>();
            if (nodePool.Count < MaxPooledNodesPerThread)
            {
                nodePool.Push(node);
                bufferedNodePool = nodePool;
            }
            // else let it be GC'ed
        }

        /// <summary>
        /// Private class used to buffer nodes when reading in buffering mode.
        /// </summary>
        protected internal sealed class BufferedNode
        {
            /// <summary>The type of the node read.</summary>
            private JsonNodeType nodeType;

            /// <summary>The value of the node.</summary>
            private object nodeValue;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="nodeType">The type of the node read.</param>
            /// <param name="value">The value of the node.</param>
            internal BufferedNode(JsonNodeType nodeType, object value)
            {
                this.nodeType = nodeType;
                this.nodeValue = value;
                this.Previous = this;
                this.Next = this;
            }

            /// <summary>
            /// Reinitializes the node for reuse.
            /// </summary>
            /// <param name="nodeType">The type of the node.</param>
            /// <param name="nodeValue">The value of the node.</param>
            internal void Reset(JsonNodeType nodeType, object nodeValue)
            {
                this.nodeType = nodeType;
                this.nodeValue = nodeValue;
                // Break any previous links
                this.Previous = this;
                this.Next = this;
            }

            /// <summary>
            /// The type of the node read.
            /// </summary>
            internal JsonNodeType NodeType
            {
                get
                {
                    return this.nodeType;
                }
            }

            /// <summary>
            /// The value of the node.
            /// </summary>
            internal object Value
            {
                get
                {
                    return this.nodeValue;
                }
            }

            /// <summary>
            /// The previous node in the list of nodes.
            /// </summary>
            internal BufferedNode Previous { get; set; }

            /// <summary>
            /// The next node in the list of nodes.
            /// </summary>
            internal BufferedNode Next { get; set; }
        }
    }
}
