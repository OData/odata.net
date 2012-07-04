//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Json
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    #endregion Namespaces

    /// <summary>
    /// Reader for the JSON format (http://www.json.org) that supports look-ahead.
    /// </summary>
    internal sealed class BufferingJsonReader : JsonReader
    {
        /// <summary>
        /// true if the reader should not report duplicate properties inside object records.
        /// false (the default) to report all properties as they are seen.
        /// </summary>
        /// <remarks>
        /// If this is turned on the reader will buffer the entire object record whenever it finds the start of the object record.
        /// It then goes through all its properties and removes duplicates.
        /// It then reports the object record as if there were no duplicates in it.
        /// If there was a duplicate property it will be reported at the position the first occurence of the property was found
        /// but with the value of the last occurence.
        /// This is to implement WCF DS Server compatibility behavior.
        /// </remarks>
        private readonly bool removeDuplicateProperties;

        /// <summary>
        /// The maximumum number of recursive internalexception objects to allow when reading in-stream errors.
        /// </summary>
        private readonly int maxInnerErrorDepth;

        /// <summary>The (possibly empty) list of buffered nodes.</summary>
        /// <remarks>This is a circular linked list where this field points to the first item of the list.</remarks>
        private BufferedNode bufferedNodesHead;

        /// <summary>
        /// A pointer into the bufferedNodes list to track the most recent position of the current buffered node.
        /// </summary>
        private BufferedNode currentBufferedNode;

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
        /// <param name="reader">The text reader to read input characters from.</param>
        /// <param name="removeDuplicateProperties">true if the reader should remove duplicate properties and simulate the WCF DS Server behavior.</param>
        /// <param name="maxInnerErrorDepth">The maximumum number of recursive internalexception objects to allow when reading in-stream errors.</param>
        internal BufferingJsonReader(TextReader reader, bool removeDuplicateProperties, int maxInnerErrorDepth)
            : base(reader)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(reader != null, "reader != null");

            this.removeDuplicateProperties = removeDuplicateProperties;
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
        public override JsonNodeType NodeType
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();

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

                return base.NodeType;
            }
        }

        /// <summary>
        /// The value of the last reported node.
        /// </summary>
        /// <remarks>
        /// Depending on whether buffering is on or off this will return the node type of the last
        /// buffered read or the node type of the last unbuffered read.
        /// </remarks>
        public override object Value
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();

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

                return base.Value;
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
                DebugUtils.CheckNoExternalCallers();

                return this.disableInStreamErrorDetection;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();

                this.disableInStreamErrorDetection = value;
            }
        }

#if DEBUG
        /// <summary>
        /// Flag indicating whether buffering is on or off; debug-only for use in asserts.
        /// </summary>
        internal bool IsBuffering
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();

                return this.isBuffering;
            }
        }
#endif
        /// <summary>
        /// Reads the next node from the input.
        /// </summary>
        /// <returns>true if a new node was found, or false if end of input was reached.</returns>
        public override bool Read()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!this.parsingInStreamError, "!this.parsingInStreamError");

            // read the next node and check whether it is an in-stream error
            return this.ReadInternal();
        }

        /// <summary>
        /// Puts the reader into the state where it buffers read nodes.
        /// </summary>
        internal void StartBuffering()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!this.isBuffering, "Buffering is already turned on. Must not call StartBuffering again.");

            if (this.bufferedNodesHead == null)
            {
                // capture the current state of the reader as the first item in the buffer (if there are none)
                this.bufferedNodesHead = new BufferedNode(base.NodeType, base.Value);
            }
            else
            {
                this.removeOnNextRead = false;
            }

            Debug.Assert(this.bufferedNodesHead != null, "Expected at least the current node in the buffer when starting buffering.");

            // Set the currentBufferedNode to the first node in the list; this means every time we start buffering we reset the 
            // position of the current buffered node since in general we don't know how far ahead we have read before and thus don't 
            // want to blindly continuing to read. The model is that with ever call to StartBuffering you reset the position of the
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
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.isBuffering, "Bookmarks can only be create when in buffering mode.");

            return this.currentBufferedNode;
        }

        /// <summary>
        /// Moves the reader to the bookmarked position.
        /// </summary>
        /// <param name="bookmark">The bookmark object to move to.</param>
        internal void MoveToBookmark(object bookmark)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.isBuffering, "Bookmarks can only be used when in buffering mode.");
            Debug.Assert(bookmark != null, "bookmark != null");

            BufferedNode bookmarkNode = bookmark as BufferedNode;
            Debug.Assert(bookmarkNode != null, "Invalid bookmark.");

#if DEBUG
            BufferedNode node = this.bufferedNodesHead;
            bool foundBookmark = false;
            do
            {
                if (node == bookmarkNode)
                {
                    foundBookmark = true;
                    break;
                }

                node = node.Next;
            }
            while (node != this.bufferedNodesHead);

            Debug.Assert(foundBookmark, "Tried to move to a bookmark which is already out of scope, bookmarks are only valid inside a single buffering session.");
#endif

            this.currentBufferedNode = bookmarkNode;
        }

        /// <summary>
        /// Puts the reader into the state where no buffering happen on read.
        /// Either buffered nodes are consumed or new nodes are read (and not buffered).
        /// </summary>
        internal void StopBuffering()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.isBuffering, "Buffering is not turned on. Must not call StopBuffering in this state.");

            // NOTE: by turning off buffering the reader is set to the first node in the buffer (if any) and will continue
            //       to read from there. removeOnNextRead is set to true since we captured the original state of the reader
            //       (before starting to buffer) as the first node in the buffer and that node has to be removed on the next read.
            this.isBuffering = false;
            this.removeOnNextRead = true;

            // We set the currentBufferedNode to null here to indicate that we want to reset the position of the current 
            // buffered node when we turn on buffering the next time. So far this (i.e., resetting the position of the buffered
            // node is the only mode the BufferingJsonReader supports. We can make resetting the current node position more explicit
            // if needed.
            this.currentBufferedNode = null;
        }

        /// <summary>
        /// A method to detect whether the current property value represents an in-stream error.
        /// </summary>
        /// <param name="error">The <see cref="ODataError"/> read from the payload.</param>
        /// <returns>true if the current value is an in-stream error value; otherwise false.</returns>
        internal bool TryReadInStreamErrorPropertyValue(out ODataError error)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertNotBuffering();

            error = null;

            this.StartBuffering();
            this.parsingInStreamError = true;

            try
            {
                return this.TryReadErrorPropertyValue(out error);
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
        /// If the parsingInStreamError field is false, the method will read ahead for every StartObject node read from the input to check whether the JSON object 
        /// represents an in-stream error. If so, it throws an <see cref="ODataErrorException"/>. If false, this check will not happen.
        /// This parsingInStremError field is set to true when trying to parse an in-stream error; in normal operation it is false.
        /// </remarks>
        private bool ReadInternal()
        {
            if (this.removeOnNextRead)
            {
                Debug.Assert(this.bufferedNodesHead != null, "If removeOnNextRead is true we must have at least one node in the buffer.");

                // Remove the first node in the list.
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
                        result = base.Read();

                        // Add the new node to the end
                        BufferedNode newNode = new BufferedNode(base.NodeType, base.Value);
                        newNode.Previous = this.bufferedNodesHead.Previous;
                        newNode.Next = this.bufferedNodesHead;
                        this.bufferedNodesHead.Previous.Next = newNode;
                        this.bufferedNodesHead.Previous = newNode;
                        this.currentBufferedNode = newNode;
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
                    result = this.parsingInStreamError ? base.Read() : this.ReadNextAndCheckForInStreamError();
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

                if (base.NodeType == JsonNodeType.StartObject)
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

                    // If the duplicate properties should be removed, do it here.
                    if (this.removeDuplicateProperties)
                    {
                        // The method will look for in-stream errors and throw on them as it performs the deduplication.
                        this.RemoveDuplicateProperties();
                    }
                    else
                    {
                        // read ahead and check for in-stream error
                        this.TryReadErrorAndThrow();
                    }

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
        /// Reads ahead and tries to parse an in-stream error payload. If it finds one it will throw it.
        /// </summary>
        private void TryReadErrorAndThrow()
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

                    // if we found any other property than the 'error' property, we don't treat it as an in-stream error
                    if (string.CompareOrdinal(JsonConstants.ODataErrorName, propertyName) != 0 || errorPropertyFound)
                    {
                        return;
                    }

                    errorPropertyFound = true;

                    // position the reader over the property value
                    this.ReadInternal();

                    if (!this.TryReadErrorPropertyValue(out error))
                    {
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
        /// Removes duplicate properties in the current object record.
        /// </summary>
        /// <remarks>
        /// This method assumes that we are buffering and that the current buffered node is a StartObject.
        /// It then goes, buffers the entire object record (and all its children) and removes duplicate properties (using the WCF DS Server algorithm).
        /// It will remove duplicate properties on any objects in the subtree of the top-level object as well (behaves recursively).
        /// The method also checks for in-stream errors and throws if it finds one.
        /// </remarks>
        private void RemoveDuplicateProperties()
        {
            Debug.Assert(this.removeDuplicateProperties, "The RemoveDuplicateProperties method should only be called if the removal of duplicate properties is turned on.");
            Debug.Assert(this.currentBufferedNode.NodeType == JsonNodeType.StartObject, "this.currentBufferedNode.NodeType == JsonNodeType.StartObject");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();

            // Read ahead the entire object and create a map of properties.
            // This will buffer the object record we are on and all its children (the entire subtree).
            // We store a stack, which holds an item for each object record we find.
            // Each item on this stack is a dictionary
            //   - key is the name of the property of a property in the object record
            //   - value is a List<PropertyDeduplicationRecord>, in this list we store a new record
            //     every time we find a property of a given name for the objet record.
            //     This stores pointer to the property node and pointer to the node after the property value.
            Stack<ObjectRecordPropertyDeduplicationRecord> objectRecordStack = new Stack<ObjectRecordPropertyDeduplicationRecord>();
            do
            {
                if (this.currentBufferedNode.NodeType == JsonNodeType.StartObject)
                {
                    // New object record - add the node to our stack
                    objectRecordStack.Push(new ObjectRecordPropertyDeduplicationRecord());

                    // See if it's an in-stream error
                    BufferedNode startObjectPosition = this.currentBufferedNode;
                    this.TryReadErrorAndThrow();
                    this.currentBufferedNode = startObjectPosition;
                }
                else if (this.currentBufferedNode.NodeType == JsonNodeType.EndObject)
                {
                    // End of object record
                    // Pop the node from our stack
                    ObjectRecordPropertyDeduplicationRecord currentObjectRecord = objectRecordStack.Pop();

                    // If there is a current property, mark its last value node.
                    if (currentObjectRecord.CurrentPropertyRecord != null)
                    {
                        currentObjectRecord.CurrentPropertyRecord.LastPropertyValueNode = this.currentBufferedNode.Previous;
                    }

                    // Now walk the list of properties for the object record and deduplicate them
                    foreach (List<PropertyDeduplicationRecord> propertyDeduplicationRecords in currentObjectRecord.Values)
                    {
                        // If there's just one property of this name - there's nothing to do.
                        if (propertyDeduplicationRecords.Count <= 1)
                        {
                            continue;
                        }

                        // Walk all the properties and each time remove the property we find from its current place and move it
                        // inplace of the first property. Note that since the property names are the same we can replace the property node itself
                        // without losing any information.
                        // Once we walk the entire list the outcome will be that we will have just one property of a given name,
                        // it will be in the position of the first property, but it will be the last property value.
                        // We could completely remove the property occurences which are not first or last, but it's easier to move them
                        // to the first one by one as it keeps the algorithm simple (and the performence difference is small enough).
                        PropertyDeduplicationRecord firstProperty = propertyDeduplicationRecords[0];
                        for (int propertyIndex = 1; propertyIndex < propertyDeduplicationRecords.Count; propertyIndex++)
                        {
                            PropertyDeduplicationRecord currentProperty = propertyDeduplicationRecords[propertyIndex];
                            Debug.Assert((string)firstProperty.PropertyNode.Value == (string)currentProperty.PropertyNode.Value, "The property names must be the same.");

                            // Note that property nodes must be preceded at least by the start object node and followed by the end object node
                            // so we don't have to check for end of list here.
                            // Remove the current property from the list
                            currentProperty.PropertyNode.Previous.Next = currentProperty.LastPropertyValueNode.Next;
                            currentProperty.LastPropertyValueNode.Next.Previous = currentProperty.PropertyNode.Previous;

                            // Now replace the first property with the current property
                            firstProperty.PropertyNode.Previous.Next = currentProperty.PropertyNode;
                            currentProperty.PropertyNode.Previous = firstProperty.PropertyNode.Previous;
                            firstProperty.LastPropertyValueNode.Next.Previous = currentProperty.LastPropertyValueNode;
                            currentProperty.LastPropertyValueNode.Next = firstProperty.LastPropertyValueNode.Next;
                            firstProperty = currentProperty;
                        }
                    }

                    if (objectRecordStack.Count == 0)
                    {
                        break;
                    }
                }
                else if (this.currentBufferedNode.NodeType == JsonNodeType.Property)
                {
                    ObjectRecordPropertyDeduplicationRecord currentObjectRecord = objectRecordStack.Peek();

                    // If there is a previous property record, mark its last value node.
                    if (currentObjectRecord.CurrentPropertyRecord != null)
                    {
                        currentObjectRecord.CurrentPropertyRecord.LastPropertyValueNode = this.currentBufferedNode.Previous;
                    }

                    // Create a new property record for this property node and add it to the object record.
                    currentObjectRecord.CurrentPropertyRecord = new PropertyDeduplicationRecord(this.currentBufferedNode);
                    string propertyName = (string)this.currentBufferedNode.Value;
                    List<PropertyDeduplicationRecord> propertyDeduplicationRecords;
                    if (!currentObjectRecord.TryGetValue(propertyName, out propertyDeduplicationRecords))
                    {
                        propertyDeduplicationRecords = new List<PropertyDeduplicationRecord>();
                        currentObjectRecord.Add(propertyName, propertyDeduplicationRecords);
                    }

                    propertyDeduplicationRecords.Add(currentObjectRecord.CurrentPropertyRecord);
                }
            }
            while (this.ReadInternal());
        }

        /// <summary>
        /// Try to read an error structure from the stream. Return null if no error structure can be read.
        /// </summary>
        /// <param name="error">An <see cref="ODataError"/> instance that was read from the reader or null if none could be read.</param>
        /// <returns>true if an <see cref="ODataError"/> instance that was read; otherwise false.</returns>
        private bool TryReadErrorPropertyValue(out ODataError error)
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
                            error.ErrorCode = errorCode;
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

                        if (!this.TryReadMessagePropertyValue(error))
                        {
                            return false;
                        }

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

        /// <summary>
        /// Try to read the message property value of an error value.
        /// </summary>
        /// <param name="error">An <see cref="ODataError"/> instance to set the read message property values on.</param>
        /// <returns>true if the message property values could be read; otherwise false.</returns>
        private bool TryReadMessagePropertyValue(ODataError error)
        {
            Debug.Assert(error != null, "error != null");
            Debug.Assert(this.currentBufferedNode.NodeType == JsonNodeType.Property, "this.currentBufferedNode.NodeType == JsonNodeType.Property");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();

            // move the reader onto the property value
            this.ReadInternal();

            // we expect a start-object node here
            if (this.currentBufferedNode.NodeType != JsonNodeType.StartObject)
            {
                return false;
            }

            // read the start-object node
            this.ReadInternal();

            // we expect one of the supported properties for the value (or end-object)
            ODataJsonReaderUtils.ErrorPropertyBitMask propertiesFoundBitmask = ODataJsonReaderUtils.ErrorPropertyBitMask.None;
            while (this.currentBufferedNode.NodeType == JsonNodeType.Property)
            {
                // NOTE the Json reader already ensures that the value of a property node is a string
                string propertyName = (string)this.currentBufferedNode.Value;

                switch (propertyName)
                {
                    case JsonConstants.ODataErrorMessageLanguageName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitmask, ODataJsonReaderUtils.ErrorPropertyBitMask.MessageLanguage))
                        {
                            return false;
                        }

                        string lang;
                        if (this.TryReadErrorStringPropertyValue(out lang))
                        {
                            error.MessageLanguage = lang;
                        }
                        else
                        {
                            return false;
                        }

                        break;

                    case JsonConstants.ODataErrorMessageValueName:
                        if (!ODataJsonReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitmask, ODataJsonReaderUtils.ErrorPropertyBitMask.MessageValue))
                        {
                            return false;
                        }

                        string message;
                        if (this.TryReadErrorStringPropertyValue(out message))
                        {
                            error.Message = message;
                        }
                        else
                        {
                            return false;
                        }

                        break;

                    default:
                        // if we find a non-supported property we don't treat this as an error
                        return false;
                }

                this.ReadInternal();
            }

            Debug.Assert(this.currentBufferedNode.NodeType == JsonNodeType.EndObject, "this.currentBufferedNode.NodeType == JsonNodeType.EndObject");

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
                            innerError.Message = message;
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
                            innerError.TypeName = typeName;
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
                            innerError.StackTrace = stackTrace;
                        }
                        else
                        {
                            return false;
                        }

                        break;

                    case JsonConstants.ODataErrorInnerErrorInnerErrorName:
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
        /// Private class used to buffer nodes when reading in buffering mode.
        /// </summary>
        private sealed class BufferedNode
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

        /// <summary>
        /// Private class used to store information necessary to deduplicate properties of a single JSON object record.
        /// </summary>
        /// <remarks>
        /// This class is a dictionary
        /// Key is the name of a property in the object record.
        /// Value is a list of property deduplication records in the order we find the properties in the payload.
        /// </remarks>
        private sealed class ObjectRecordPropertyDeduplicationRecord : Dictionary<string, List<PropertyDeduplicationRecord>>
        {
            /// <summary>
            /// Points to the property record which is currently being constructed.
            /// </summary>
            internal PropertyDeduplicationRecord CurrentPropertyRecord { get; set; }
        }

        /// <summary>
        /// Private class used to store information necessary to deduplicate a single JSON property.
        /// </summary>
        private sealed class PropertyDeduplicationRecord
        {
            /// <summary>
            /// The node in the buffered nodes list which points to the property node
            /// which this deduplication record describes.
            /// </summary>
            private readonly BufferedNode propertyNode;

            /// <summary>
            /// The node in the buffered nodes list which points to the last node of the value of the property node
            /// this deduplication record describes.
            /// </summary>
            private BufferedNode lastPropertyValueNode;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="propertyNode">The property node to create the record for.</param>
            internal PropertyDeduplicationRecord(BufferedNode propertyNode)
            {
                Debug.Assert(propertyNode != null, "propertyNode != null");
                Debug.Assert(propertyNode.NodeType == JsonNodeType.Property, "Only property node can be stored as the property node of the deduplication record.");

                this.propertyNode = propertyNode;
            }

            /// <summary>
            /// The node in the buffered nodes list which points to the property node
            /// which this deduplication record describes.
            /// </summary>
            internal BufferedNode PropertyNode
            {
                get
                {
                    return this.propertyNode;
                }
            }

            /// <summary>
            /// The node in the buffered nodes list which points to the last node of the value of the property node
            /// this deduplication record describes.
            /// </summary>
            /// <remarks>
            /// Observation: Even if the value itself is an object for which we will do the property deduplication and thus we will shuffle its nodes around,
            /// in that case the last value node will point to the end object node which will not change during the deduplication process.
            /// </remarks>
            internal BufferedNode LastPropertyValueNode
            {
                get
                {
                    return this.lastPropertyValueNode;
                }

                set
                {
                    Debug.Assert(this.lastPropertyValueNode == null, "The LastPropertyValueNode can be set only once.");
                    this.lastPropertyValueNode = value;
                }
            }
        }
    }
}
