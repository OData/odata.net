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
        /// <summary>The (possibly empty) list of buffered nodes.</summary>
        private readonly LinkedList<BufferedNode> bufferedNodes;

        /// <summary>
        /// A pointer into the bufferedNodes list to track the most recent position of the current buffered node.
        /// </summary>
        private LinkedListNode<BufferedNode> currentBufferedNode;

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
        internal BufferingJsonReader(TextReader reader)
            : base(reader)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(reader != null, "reader != null");

            this.bufferedNodes = new LinkedList<BufferedNode>();
            this.currentBufferedNode = null;
        }

        /// <summary>
        /// The type of the last node read.
        /// </summary>
        /// <remarks>
        /// Depending on whether buffering is on or off this will return the node type of the last
        /// buffered read or the node type of the last unbuffered read.
        /// </remarks>
        internal override JsonNodeType NodeType
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();

                if (this.bufferedNodes.Count > 0)
                {
                    if (this.isBuffering)
                    {
                        Debug.Assert(this.currentBufferedNode != null, "this.currentBufferedNode != null");
                        return this.currentBufferedNode.Value.NodeType;
                    }

                    // in non-buffering mode if we have buffered nodes satisfy the request from the first node there
                    return this.bufferedNodes.First.Value.NodeType;
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
        internal override object Value
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();

                if (this.bufferedNodes.Count > 0)
                {
                    if (this.isBuffering)
                    {
                        Debug.Assert(this.currentBufferedNode != null, "this.currentBufferedNode != null");
                        return this.currentBufferedNode.Value.Value;
                    }

                    // in non-buffering mode if we have buffered nodes satisfy the request from the first node there
                    return this.bufferedNodes.First.Value.Value;
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
        internal override bool Read()
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

            if (this.bufferedNodes.Count == 0)
            {
                // capture the current state of the reader as the first item in the buffer (if there are none)
                this.bufferedNodes.AddFirst(new BufferedNode(base.NodeType, base.Value));
            }
            else
            {
                this.removeOnNextRead = false;
            }

            Debug.Assert(this.bufferedNodes.Count > 0, "Expected at least the current node in the buffer when starting buffering.");

            // Set the currentBufferedNode to the first node in the list; this means every time we start buffering we reset the 
            // position of the current buffered node since in general we don't know how far ahead we have read before and thus don't 
            // want to blindly continuing to read. The model is that with ever call to StartBuffering you reset the position of the
            // current node in the list and start reading through the buffer again.
            if (this.currentBufferedNode == null)
            {
                this.currentBufferedNode = this.bufferedNodes.First;
            }

            this.isBuffering = true;
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
                Debug.Assert(this.bufferedNodes.Count > 0, "If removeOnNextRead is true we must have at least one node in the buffer.");
                this.bufferedNodes.RemoveFirst();
                this.removeOnNextRead = false;
            }

            bool result;
            if (this.isBuffering)
            {
                Debug.Assert(this.currentBufferedNode != null, "this.currentBufferedNode != null");

                if (this.currentBufferedNode.Next != null)
                {
                    Debug.Assert(!this.parsingInStreamError, "!this.parsingInStreamError");
                    this.currentBufferedNode = this.currentBufferedNode.Next;
                    result = true;
                }
                else
                {
                    if (this.parsingInStreamError)
                    {
                        // read more from the input stream and buffer it
                        result = base.Read();
                        this.bufferedNodes.AddLast(new BufferedNode(base.NodeType, base.Value));
                        this.currentBufferedNode = this.bufferedNodes.Last;
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
                if (this.bufferedNodes.Count == 0)
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
                    BufferedNode bufferedNode = this.bufferedNodes.First.Value;
                    result = bufferedNode.NodeType != JsonNodeType.EndOfInput;
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
                    LinkedListNode<BufferedNode> storedPosition = null;
                    if (this.isBuffering)
                    {
                        storedPosition = this.currentBufferedNode;
                    }
                    else
                    {
                        this.StartBuffering();
                    }

                    // read ahead and check for in-stream error
                    ODataError inStreamError;
                    if (this.TryReadError(out inStreamError))
                    {
                        Debug.Assert(inStreamError != null, "inStreamError != null");
                        throw new ODataErrorException(inStreamError);
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
        /// Reads ahead and tries to parse an in-stream error payload.
        /// </summary>
        /// <param name="error">The error that was read from the payload if the method returns true; otherwise null.</param>
        /// <returns>true if an in-stream error was successfully parsed from the payload; otherwise false.</returns>
        private bool TryReadError(out ODataError error)
        {
            Debug.Assert(base.NodeType == JsonNodeType.StartObject, "base.NodeType == JsonNodeType.StartObject");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();

            error = null;

            // only check for in-stream errors if the buffering reader is told to do so
            if (!this.DisableInStreamErrorDetection)
            {
                // move to the first property of the potential error object (or the end-object node if no properties exist)
                this.ReadInternal();

                // we only consider this to be an in-stream error if the object has a single 'error' property
                bool errorPropertyFound = false;
                while (base.NodeType == JsonNodeType.Property)
                {
                    // NOTE: the JSON reader already ensures that the value of a property node (which is the name of the property) is a string
                    string propertyName = (string)base.Value;

                    // if we found any other property than the 'error' property, we don't treat it as an in-stream error
                    if (string.CompareOrdinal(JsonConstants.ODataErrorName, propertyName) != 0 || errorPropertyFound)
                    {
                        return false;
                    }

                    errorPropertyFound = true;

                    if (!this.TryReadErrorPropertyValue(out error))
                    {
                        return false;
                    }
                }

                return errorPropertyFound;
            }

            return false;
        }

        /// <summary>
        /// Try to read an error structure from the stream. Return null if no error structure can be read.
        /// </summary>
        /// <param name="error">An <see cref="ODataError"/> instance that was read from the reader or null if none could be read.</param>
        /// <returns>true if an <see cref="ODataError"/> instance that was read; otherwise false.</returns>
        private bool TryReadErrorPropertyValue(out ODataError error)
        {
            Debug.Assert(base.NodeType == JsonNodeType.Property, "base.NodeType == JsonNodeType.Property");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();

            error = null;

            // position the reader over the property value
            this.ReadInternal();

            // we expect a start-object node here
            if (base.NodeType != JsonNodeType.StartObject)
            {
                return false;
            }

            // read the start-object node
            this.ReadInternal();

            error = new ODataError();

            // we expect one of the supported properties for the value (or end-object)
            ODataJsonReaderUtils.ErrorPropertyBitMask propertiesFoundBitmask = ODataJsonReaderUtils.ErrorPropertyBitMask.None;
            while (base.NodeType == JsonNodeType.Property)
            {
                // NOTE the Json reader already ensures that the value of a property node is a string
                string propertyName = (string)base.Value;
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
                        if (!this.TryReadInnerErrorPropertyValue(out innerError))
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
            Debug.Assert(base.NodeType == JsonNodeType.EndObject, "base.NodeType == JsonNodeType.EndObject");
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
            Debug.Assert(base.NodeType == JsonNodeType.Property, "base.NodeType == JsonNodeType.Property");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();

            // move the reader onto the property value
            this.ReadInternal();

            // we expect a start-object node here
            if (base.NodeType != JsonNodeType.StartObject)
            {
                return false;
            }

            // read the start-object node
            this.ReadInternal();

            // we expect one of the supported properties for the value (or end-object)
            ODataJsonReaderUtils.ErrorPropertyBitMask propertiesFoundBitmask = ODataJsonReaderUtils.ErrorPropertyBitMask.None;
            while (base.NodeType == JsonNodeType.Property)
            {
                // NOTE the Json reader already ensures that the value of a property node is a string
                string propertyName = (string)base.Value;

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

            Debug.Assert(base.NodeType == JsonNodeType.EndObject, "base.NodeType == JsonNodeType.EndObject");

            return true;
        }

        /// <summary>
        /// Try to read an inner error property value.
        /// </summary>
        /// <param name="innerError">An <see cref="ODataInnerError"/> instance that was read from the reader or null if none could be read.</param>
        /// <returns>true if an <see cref="ODataInnerError"/> instance that was read; otherwise false.</returns>
        private bool TryReadInnerErrorPropertyValue(out ODataInnerError innerError)
        {
            Debug.Assert(base.NodeType == JsonNodeType.Property, "base.NodeType == JsonNodeType.Property");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();

            // move the reader onto the property value
            this.ReadInternal();

            // we expect a start-object node here
            if (base.NodeType != JsonNodeType.StartObject)
            {
                innerError = null;
                return false;
            }

            // read the start-object node
            this.ReadInternal();

            innerError = new ODataInnerError();

            // we expect one of the supported properties for the value (or end-object)
            ODataJsonReaderUtils.ErrorPropertyBitMask propertiesFoundBitmask = ODataJsonReaderUtils.ErrorPropertyBitMask.None;
            while (base.NodeType == JsonNodeType.Property)
            {
                // NOTE the Json reader already ensures that the value of a property node is a string
                string propertyName = (string)base.Value;

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
                        if (this.TryReadInnerErrorPropertyValue(out nestedInnerError))
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

            Debug.Assert(base.NodeType == JsonNodeType.EndObject, "base.NodeType == JsonNodeType.EndObject");

            return true;
        }

        /// <summary>
        /// Reads the string value of a property.
        /// </summary>
        /// <param name="stringValue">The string value read if the method returns true; otherwise null.</param>
        /// <returns>true if a string value (or null) was read as property value of the current property; otherwise false.</returns>
        private bool TryReadErrorStringPropertyValue(out string stringValue)
        {
            Debug.Assert(base.NodeType == JsonNodeType.Property, "base.NodeType == JsonNodeType.Property");
            Debug.Assert(this.parsingInStreamError, "this.parsingInStreamError");
            this.AssertBuffering();

            this.ReadInternal();

            // we expect a string value
            stringValue = base.Value as string;
            return base.NodeType == JsonNodeType.PrimitiveValue && (base.Value == null || stringValue != null);
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
                switch (base.NodeType)
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
                            base.NodeType != JsonNodeType.EndOfInput,
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
            private readonly object value;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="nodeType">The type of the node read.</param>
            /// <param name="value">The value of the node.</param>
            internal BufferedNode(JsonNodeType nodeType, object value)
            {
                this.nodeType = nodeType;
                this.value = value;
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
                    return this.value;
                }
            }
        }
    }
}
