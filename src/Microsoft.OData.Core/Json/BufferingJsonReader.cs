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
    using System.IO;
    using System.Threading.Tasks;

    #endregion Namespaces

    /// <summary>
    /// Reader for the JSON format (http://www.json.org) that supports look-ahead.
    /// </summary>
    internal class BufferingJsonReader : IJsonReader
    {
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations.", Justification = "<Pending>")]
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
            return (value is string || value == null || this.NodeType == JsonNodeType.StartArray || this.NodeType == JsonNodeType.StartObject);
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
        public virtual async Task<bool> CanStreamAsync()
        {
            this.AssertAsynchronous();

            if (!this.isBuffering)
            {
                return await this.innerReader.CanStreamAsync()
                    .ConfigureAwait(false);
            }

            object value = await this.GetValueAsync()
                .ConfigureAwait(false);
            return value is string || value == null || this.NodeType == JsonNodeType.StartArray || this.NodeType == JsonNodeType.StartObject;
        }

        /// <summary>
        /// Asynchronously creates a stream for reading a stream value.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a <see cref="Stream"/> used to read a stream value.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations.", Justification = "<Pending>")]
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
                this.bufferedNodesHead = new BufferedNode(this.innerReader.NodeType, this.innerReader.GetValue());
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
        internal async Task StartBufferingAsync()
        {
            Debug.Assert(!this.isBuffering, "Buffering is already turned on. Must not call StartBuffering again.");
            this.AssertAsynchronous();

            if (this.bufferedNodesHead == null)
            {
                // capture the current state of the reader as the first item in the buffer (if there are none)
                this.bufferedNodesHead = new BufferedNode(
                    this.innerReader.NodeType,
                    await this.innerReader.GetValueAsync().ConfigureAwait(false));
            }
            else
            {
                this.removeOnNextRead = false;
            }

            Debug.Assert(this.bufferedNodesHead != null, "Expected at least the current node in the buffer when starting buffering.");

            // Set the currentBufferedNode to the first node in the list; this means every time we start buffering we reset the
            // position of the current buffered node since in general we don't know how far ahead we have read before and thus don't
            // want to blindly continuing to read. The model is that with every call to StartBufferingAsync you reset the position of the
            // current node in the list and start reading through the buffer again.
            if (this.currentBufferedNode == null)
            {
                this.currentBufferedNode = this.bufferedNodesHead;
            }

            this.isBuffering = true;
        }

        /// <summary>
        /// A method to asynchronously detect whether the current property value represents an in-stream error.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a tuple comprising of:
        /// 1). A value of true if the current value is an in-stream error value; otherwise false.
        /// 2). An <see cref="ODataError"/> instance that was read from the payload.</returns>
        internal async Task<(bool IsReadSuccessfully, ODataError Error)> StartBufferingAndTryToReadInStreamErrorPropertyValueAsync()
        {
            this.AssertNotBuffering();
            this.AssertAsynchronous();

            await this.StartBufferingAsync()
                .ConfigureAwait(false);
            this.parsingInStreamError = true;

            try
            {
                (bool IsReadSuccessfully, ODataError Error) readInStreamErrorPropertyResult = await this.TryReadInStreamErrorPropertyValueAsync()
                    .ConfigureAwait(false);

                return readInStreamErrorPropertyResult;
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
        /// This parsingInStremError field is set to true when trying to parse an in-stream error; in normal operation it is false.
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
        /// This parsingInStremError field is set to true when trying to parse an in-stream error; in normal operation it is false.
        /// </remarks>
        protected async Task<bool> ReadInternalAsync()
        {
            this.AssertAsynchronous();

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
                else if (this.parsingInStreamError)
                {
                    // Read more from the input stream and buffer it
                    result = await this.innerReader.ReadAsync()
                        .ConfigureAwait(false);

                    // Add the new node to the end
                    this.AddNewNodeToTheEndOfBufferedNodesList(
                        this.innerReader.NodeType,
                        await this.innerReader.GetValueAsync().ConfigureAwait(false));
                }
                else
                {
                    // Read the next node from the input stream and check
                    // whether it is an in-stream error
                    result = await this.ReadNextAndCheckForInStreamErrorAsync()
                        .ConfigureAwait(false);
                }
            }
            else if (this.bufferedNodesHead == null)
            {
                // If parsingInStreamError nothing in the buffer; read from the base,
                // else read the next node from the input stream and check
                // whether it is an in-stream error
                result = this.parsingInStreamError ?
                    await this.innerReader.ReadAsync().ConfigureAwait(false) :
                    await this.ReadNextAndCheckForInStreamErrorAsync().ConfigureAwait(false);
            }
            else
            {
                Debug.Assert(!this.parsingInStreamError, "!this.parsingInStreamError");

                // Non-buffering read from the buffer
                result = this.bufferedNodesHead.NodeType != JsonNodeType.EndOfInput;
                this.removeOnNextRead = true;
            }

            return result;
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
        protected virtual async Task ProcessObjectValueAsync()
        {
            Debug.Assert(this.currentBufferedNode.NodeType == JsonNodeType.StartObject, "this.currentBufferedNode.NodeType == JsonNodeType.StartObject");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();
            this.AssertAsynchronous();

            // Only check for in-stream errors if the buffering reader is told to do so
            if (!this.DisableInStreamErrorDetection)
            {
                // Move to the first property of the potential error object (or the EndObject node if no properties exist)
                await this.ReadInternalAsync()
                    .ConfigureAwait(false);

                // We only consider this to be an in-stream error if the object has a single 'error' property
                bool errorPropertyFound = false;
                (bool IsReadSuccessfully, ODataError Error)? readInStreamErrorPropertyResult = null;

                while (this.currentBufferedNode.NodeType == JsonNodeType.Property)
                {
                    // NOTE: The JSON reader already ensures that the value of a property node (which is the name of the property) is a string
                    string propertyName = (string)this.currentBufferedNode.Value;

                    // If we found any other property than the expected in-stream error property, we don't treat it as an in-stream error
                    if (string.CompareOrdinal(this.inStreamErrorPropertyName, propertyName) != 0 || errorPropertyFound)
                    {
                        return;
                    }

                    errorPropertyFound = true;

                    // Position the reader over the property value
                    await this.ReadInternalAsync()
                        .ConfigureAwait(false);

                    readInStreamErrorPropertyResult = await this.TryReadInStreamErrorPropertyValueAsync()
                        .ConfigureAwait(false);
                    if (readInStreamErrorPropertyResult?.IsReadSuccessfully == false)
                    {
                        // This means we thought we saw an in-stream error, but then
                        // we didn't see an intelligible error object, so we give up on reading the in-stream error
                        // and return. We will fail later in some other way. This payload is totally messed up.
                        return;
                    }
                }

                if (errorPropertyFound)
                {
                    throw new ODataErrorException(readInStreamErrorPropertyResult?.Error);
                }
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
                switch (this.currentBufferedNode.NodeType)
                {
                    case JsonNodeType.PrimitiveValue:
                        break;
                    case JsonNodeType.StartArray:
                    case JsonNodeType.StartObject:
                        depth++;
                        break;
                    case JsonNodeType.EndArray:
                    case JsonNodeType.EndObject:
                        Debug.Assert(depth > 0, "Seen too many scope ends.");
                        depth--;
                        break;
                    default:
                        Debug.Assert(
                            this.currentBufferedNode.NodeType != JsonNodeType.EndOfInput,
                            "We should not have reached end of input, since the scopes should be well formed. Otherwise JsonReader should have failed by now.");
                        break;
                }

                this.ReadInternal();
            }
            while (depth > 0);
        }

        /// <summary>
        /// Removes the head node from the buffer.
        /// </summary>
        private void RemoveFirstNodeInBuffer()
        {
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
        }

        /// <summary>
        /// Asynchronously reads the next node from the JSON reader and if a start-object node is detected starts reading ahead and
        /// tries to parse an in-stream error.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true if a new node was found, or false if end of input was reached.</returns>
        private async Task<bool> ReadNextAndCheckForInStreamErrorAsync()
        {
            Debug.Assert(!this.parsingInStreamError, "!this.parsingInStreamError");
            this.AssertAsynchronous();

            this.parsingInStreamError = true;

            try
            {
                // Read the next node in the current reader mode (buffering or non-buffering)
                bool result = await this.ReadInternalAsync()
                    .ConfigureAwait(false);

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
                        await this.StartBufferingAsync()
                            .ConfigureAwait(false);
                    }

                    await this.ProcessObjectValueAsync()
                        .ConfigureAwait(false);

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
        /// Asynchronously try to read an error object from the stream.
        /// </summary>
        /// <param name="error">An <see cref="ODataError"/> instance that was read from the reader or null if none could be read.</param>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a tuple comprising of:
        /// 1). A value of true if an <see cref="ODataError"/> instance that was read; otherwise false.
        /// 2). An <see cref="ODataError"/> instance that was read from the reader or null if none could be read.</returns>
        private async Task<(bool IsReadSuccessfully, ODataError Error)> TryReadInStreamErrorPropertyValueAsync()
        {
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();
            this.AssertAsynchronous();

            // We expect a StartObject node here
            if (this.currentBufferedNode.NodeType != JsonNodeType.StartObject)
            {
                return (false, (ODataError)null);
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
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                            ref propertiesFoundBitmask,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.Code))
                        {
                            return (false, error);
                        }

                        (bool IsReadSuccessfully, string PropertyValue) readErrorCodePropertyResult = await this.TryReadErrorStringPropertyValueAsync()
                            .ConfigureAwait(false);
                        if (!readErrorCodePropertyResult.IsReadSuccessfully)
                        {
                            return (false, error);
                        }

                        error.Code = readErrorCodePropertyResult.PropertyValue;
                        break;

                    case JsonConstants.ODataErrorMessageName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                            ref propertiesFoundBitmask,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.Message))
                        {
                            return (false, error);
                        }

                        (bool IsReadSuccessfully, string PropertyValue) readErrorMessagePropertyResult = await this.TryReadErrorStringPropertyValueAsync()
                            .ConfigureAwait(false);
                        if (!readErrorMessagePropertyResult.IsReadSuccessfully)
                        {
                            return (false, error);
                        }

                        error.Message = readErrorMessagePropertyResult.PropertyValue;
                        break;

                    case JsonConstants.ODataErrorTargetName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                            ref propertiesFoundBitmask,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.Target))
                        {
                            return (false, error);
                        }

                        (bool IsReadSuccessfully, string PropertyValue) readErrorTargetPropertyResult = await this.TryReadErrorStringPropertyValueAsync()
                            .ConfigureAwait(false);
                        if (!readErrorTargetPropertyResult.IsReadSuccessfully)
                        {
                            return (false, error);
                        }

                        error.Target = readErrorTargetPropertyResult.Item2;
                        break;

                    case JsonConstants.ODataErrorDetailsName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                                ref propertiesFoundBitmask,
                                ODataJsonReaderUtils.ErrorPropertyBitMask.Details))
                        {
                            return (false, error);
                        }

                        (bool IsReadSuccessfully, List<ODataErrorDetail> ErrorDetails) readErrorDetailsPropertyResult = await this.TryReadErrorDetailsPropertyValueAsync()
                            .ConfigureAwait(false);
                        if (!readErrorDetailsPropertyResult.IsReadSuccessfully)
                        {
                            return (false, error);
                        }

                        error.Details = readErrorDetailsPropertyResult.ErrorDetails;
                        break;

                    case JsonConstants.ODataErrorInnerErrorName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                            ref propertiesFoundBitmask,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.InnerError))
                        {
                            return (false, error);
                        }

                        Tuple<bool, ODataInnerError> readInnerErrorPropertyResult = await this.TryReadInnerErrorPropertyValueAsync(0 /*recursionDepth */)
                            .ConfigureAwait(false);
                        if (!readInnerErrorPropertyResult.Item1)
                        {
                            return (false, error);
                        }

                        error.InnerError = readInnerErrorPropertyResult.Item2;
                        break;

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
            return (propertiesFoundBitmask != ODataJsonReaderUtils.ErrorPropertyBitMask.None,
                error);
        }

        /// <summary>
        /// Asynchronously try to read an error details collection property.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a tuple comprising of:
        /// 1). A value of true if an <see cref="ODataErrorDetail"/> collection was read; otherwise false.
        /// 2). An <see cref="ODataErrorDetail"/> collection that was read from the reader or null if none could be read.</returns>
        private async Task<(bool IsReadSuccessfully, List<ODataErrorDetail> ErrorDetails)> TryReadErrorDetailsPropertyValueAsync()
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
                return (false, (List<ODataErrorDetail>)null);
            }

            // [
            await ReadInternalAsync()
                .ConfigureAwait(false);

            List<ODataErrorDetail> details = new List<ODataErrorDetail>();

            while (this.currentBufferedNode.NodeType != JsonNodeType.EndArray)
            {
                (bool IsReadSuccessfully, ODataErrorDetail ErrorDetail) readErrorDetailResult = await TryReadErrorDetailAsync()
                    .ConfigureAwait(false);

                if (!readErrorDetailResult.IsReadSuccessfully)
                {
                    return (false, details);
                }

                details.Add(readErrorDetailResult.ErrorDetail);

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
        private async Task<(bool IsReadSuccessfully, ODataErrorDetail ErrorDetail)> TryReadErrorDetailAsync()
        {
            Debug.Assert(
                this.currentBufferedNode.NodeType == JsonNodeType.StartObject,
                "this.currentBufferedNode.NodeType == JsonNodeType.StartObject");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();
            this.AssertAsynchronous();

            if (this.currentBufferedNode.NodeType != JsonNodeType.StartObject)
            {
                return (false, (ODataErrorDetail)null);
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
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                                ref propertiesFoundBitmask,
                                ODataJsonReaderUtils.ErrorPropertyBitMask.Code))
                        {
                            return (false, detail);
                        }

                        (bool IsReadSuccessfully, string PropertyValue) readErrorCodePropertyResult = await this.TryReadErrorStringPropertyValueAsync()
                            .ConfigureAwait(false);
                        if (!readErrorCodePropertyResult.IsReadSuccessfully)
                        {
                            return (false, detail);
                        }

                        detail.Code = readErrorCodePropertyResult.PropertyValue;
                        break;

                    case JsonConstants.ODataErrorTargetName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                                ref propertiesFoundBitmask,
                                ODataJsonReaderUtils.ErrorPropertyBitMask.Target))
                        {
                            return (false, detail);
                        }

                        (bool IsReadSuccessfully, string PropertyValue) readErrorTargetPropertyResult = await this.TryReadErrorStringPropertyValueAsync()
                            .ConfigureAwait(false);
                        if (!readErrorTargetPropertyResult.IsReadSuccessfully)
                        {
                            return (false, detail);
                        }

                        detail.Target = readErrorTargetPropertyResult.PropertyValue;
                        break;

                    case JsonConstants.ODataErrorMessageName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                                ref propertiesFoundBitmask,
                                ODataJsonReaderUtils.ErrorPropertyBitMask.MessageValue))
                        {
                            return (false, detail);
                        }

                        (bool IsReadSuccessfully, string PropertyValue) readErrorMessagePropertyResult = await this.TryReadErrorStringPropertyValueAsync()
                            .ConfigureAwait(false);
                        if (!readErrorMessagePropertyResult.IsReadSuccessfully)
                        {
                            return (false, detail);
                        }

                        detail.Message = readErrorMessagePropertyResult.PropertyValue;
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
        private async Task<Tuple<bool, ODataInnerError>> TryReadInnerErrorPropertyValueAsync(int recursionDepth)
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
                return Tuple.Create(false, (ODataInnerError)null);
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
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                            ref propertiesFoundBitmask,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.MessageValue))
                        {
                            return Tuple.Create(false, innerError);
                        }

                        (bool IsReadSuccessfully, string PropertyValue) readErrorMessagePropertyResult = await this.TryReadErrorStringPropertyValueAsync()
                            .ConfigureAwait(false);
                        if (!readErrorMessagePropertyResult.IsReadSuccessfully)
                        {
                            return Tuple.Create(false, innerError);
                        }

                        innerError.Properties.Add(
                            JsonConstants.ODataErrorInnerErrorMessageName,
                            new ODataPrimitiveValue(readErrorMessagePropertyResult.PropertyValue));
                        break;

                    case JsonConstants.ODataErrorInnerErrorTypeNameName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                            ref propertiesFoundBitmask,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.TypeName))
                        {
                            return Tuple.Create(false, innerError);
                        }

                        (bool IsReadSuccessfully, string PropertyValue) readErrorTypePropertyResult = await this.TryReadErrorStringPropertyValueAsync()
                            .ConfigureAwait(false);
                        if (!readErrorTypePropertyResult.IsReadSuccessfully)
                        {
                            return Tuple.Create(false, innerError);
                        }

                        innerError.Properties.Add(
                            JsonConstants.ODataErrorInnerErrorTypeNameName,
                            new ODataPrimitiveValue(readErrorTypePropertyResult.PropertyValue));
                        break;

                    case JsonConstants.ODataErrorInnerErrorStackTraceName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                            ref propertiesFoundBitmask,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.StackTrace))
                        {
                            return Tuple.Create(false, innerError);
                        }

                        (bool IsReadSuccessfully, string PropertyValue) readErrorStackTracePropertyResult = await this.TryReadErrorStringPropertyValueAsync()
                            .ConfigureAwait(false);
                        if (!readErrorStackTracePropertyResult.IsReadSuccessfully)
                        {
                            return Tuple.Create(false, innerError);
                        }

                        innerError.Properties.Add(
                            JsonConstants.ODataErrorInnerErrorStackTraceName,
                            new ODataPrimitiveValue(readErrorStackTracePropertyResult.PropertyValue));
                        break;

                    case JsonConstants.ODataErrorInnerErrorInnerErrorName:
                    case JsonConstants.ODataErrorInnerErrorName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(
                            ref propertiesFoundBitmask,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.InnerError))
                        {
                            return Tuple.Create(false, innerError);
                        }

                        Tuple<bool, ODataInnerError> readInnerErrorPropertyResult = await TryReadInnerErrorPropertyValueAsync(recursionDepth)
                            .ConfigureAwait(false);
                        if (!readInnerErrorPropertyResult.Item1)
                        {
                            return Tuple.Create(false, innerError);
                        }

                        innerError.InnerError = readInnerErrorPropertyResult.Item2;
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

            return Tuple.Create(true, innerError);
        }

        /// <summary>
        /// Asynchronously try to read the string value of a property.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a tuple comprising of:
        /// 1). A value of true if a string value (or null) was read as property value of the current property; otherwise false.
        /// 2). The string value read if the method returns true; otherwise null.</returns>
        private async Task<(bool IsReadSuccessfully, string PropertyValue)> TryReadErrorStringPropertyValueAsync()
        {
            Debug.Assert(this.currentBufferedNode.NodeType == JsonNodeType.Property, "this.currentBufferedNode.NodeType == JsonNodeType.Property");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();
            this.AssertAsynchronous();

            await this.ReadInternalAsync()
                .ConfigureAwait(false);

            // We expect a string value
            string stringValue = this.currentBufferedNode.Value as string;
            return (
                this.currentBufferedNode.NodeType == JsonNodeType.PrimitiveValue && (this.currentBufferedNode.Value == null || stringValue != null),
                stringValue);
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
        private async Task SkipValueInternalAsync()
        {
            this.AssertAsynchronous();

            int depth = 0;
            do
            {
                switch (this.currentBufferedNode.NodeType)
                {
                    case JsonNodeType.PrimitiveValue:
                        break;
                    case JsonNodeType.StartArray:
                    case JsonNodeType.StartObject:
                        depth++;
                        break;
                    case JsonNodeType.EndArray:
                    case JsonNodeType.EndObject:
                        Debug.Assert(depth > 0, "Seen too many scope ends.");
                        depth--;
                        break;
                    default:
                        Debug.Assert(
                            this.currentBufferedNode.NodeType != JsonNodeType.EndOfInput,
                            "We should not have reached end of input, since the scopes should be well formed. Otherwise JsonReader should have failed by now.");
                        break;
                }

                await this.ReadInternalAsync()
                    .ConfigureAwait(false);
            } while (depth > 0);
        }

        /// <summary>
        /// Adds a new node to the end of the buffered nodes list.
        /// </summary>
        /// <param name="nodeType">The node type.</param>
        /// <param name="value">The node value.</param>
        private void AddNewNodeToTheEndOfBufferedNodesList(JsonNodeType nodeType, object value)
        {
            BufferedNode newNode = new BufferedNode(nodeType, value);
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
        /// Private class used to buffer nodes when reading in buffering mode.
        /// </summary>
        protected internal sealed class BufferedNode
        {
            /// <summary>The type of the node read.</summary>
            private readonly JsonNodeType nodeType;

            /// <summary>The value of the node.</summary>
            private readonly object nodeValue;

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
