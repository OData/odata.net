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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml;
    #endregion Namespaces

    /// <summary>
    /// XML reader which supports look-ahead.
    /// </summary>
    internal sealed class BufferingXmlReader : XmlReader
    {
        #region Atomized strings
        /// <summary>The "http://www.w3.org/XML/1998/namespace" namespace for the "xml" prefix.</summary>
        internal readonly string XmlNamespace;

        /// <summary>The "base" name for the XML base attribute.</summary>
        internal readonly string XmlBaseAttributeName;

        /// <summary>The 'lang' attribute local name of the xml:lang attribute.</summary>
        internal readonly string XmlLangAttributeName;

        /// <summary>XML namespace for data service annotations.</summary>
        internal readonly string ODataMetadataNamespace;

        /// <summary>XML namespace for data services.</summary>
        internal readonly string ODataNamespace;

        /// <summary>The 'error' local name of the error element.</summary>
        internal readonly string ODataErrorElementName;
        #endregion Atomized strings

        /// <summary>The underlying XML reader this buffering reader is wrapping.</summary>
        private readonly XmlReader reader;

        /// <summary>The (possibly empty) list of buffered nodes.</summary>
        /// <remarks>This list stores only non-attribute nodes, attributes are stored in a separate list on an element node.</remarks>
        private readonly LinkedList<BufferedNode> bufferedNodes;

        /// <summary>
        /// A special buffered node instance which represents the end of input.
        /// We always have just one instance and compare references.
        /// </summary>
        private readonly BufferedNode endOfInputBufferedNode;

        /// <summary>Flag to control if the xml:base attributes should be processed when reading.</summary>
        private readonly bool disableXmlBase;

        /// <summary>A pointer into the bufferedNodes list to track the most recent position of the current buffered node.</summary>
        private LinkedListNode<BufferedNode> currentBufferedNode;

        /// <summary>
        /// A pointer either into the bufferedNodes list or into the list of attributes on a buffered element node
        /// which points to the node which should be reported to the user.
        /// </summary>
        private LinkedListNode<BufferedNode> currentBufferedNodeToReport;

        /// <summary>A flag indicating whether the reader is in buffering mode or not.</summary>
        private bool isBuffering;

        /// <summary>
        /// A flag indicating that the last node for non-buffering read was taken from the buffer; we leave the 
        /// node in the buffer until the next Read call.
        /// </summary>
        private bool removeOnNextRead;

        /// <summary>Flag to control whether in-stream errors should be detected when reading.</summary>
        private bool disableInStreamErrorDetection;

        /// <summary>The stack of XML base URI definitions.</summary>
        private Stack<XmlBaseDefinition> xmlBaseStack;

        /// <summary>The XML base stack state when the buffering started. This is only used when in buffering mode.</summary>
        private Stack<XmlBaseDefinition> bufferStartXmlBaseStack;

        /// <summary>Constructor</summary>
        /// <param name="reader">The reader to wrap.</param>
        /// <param name="disableXmlBase">Flag to control if the xml:base attributes should be processed when reading.</param>
        internal BufferingXmlReader(XmlReader reader, bool disableXmlBase)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(reader != null, "reader != null");
            Debug.Assert(reader.NodeType == XmlNodeType.None && !reader.EOF, "The reader should be at the beginning.");

            this.reader = reader;
            this.disableXmlBase = disableXmlBase;

            XmlNameTable nameTable = this.reader.NameTable;
            this.XmlNamespace = nameTable.Add(AtomConstants.XmlNamespace);
            this.XmlBaseAttributeName = nameTable.Add(AtomConstants.XmlBaseAttributeName);
            this.XmlLangAttributeName = nameTable.Add(AtomConstants.XmlLangAttributeName);
            this.ODataMetadataNamespace = nameTable.Add(AtomConstants.ODataMetadataNamespace);
            this.ODataNamespace = nameTable.Add(AtomConstants.ODataNamespace);
            this.ODataErrorElementName = nameTable.Add(AtomConstants.ODataErrorElementName);

            this.bufferedNodes = new LinkedList<BufferedNode>();
            this.currentBufferedNode = null;
            this.endOfInputBufferedNode = BufferedNode.CreateEndOfInput(this.reader.NameTable);
            this.xmlBaseStack = new Stack<XmlBaseDefinition>();
        }

        /// <summary>
        /// Returns the type of the current node.
        /// </summary>
        public override XmlNodeType NodeType
        {
            get
            {
                return this.currentBufferedNodeToReport != null ? this.currentBufferedNodeToReport.Value.NodeType : this.reader.NodeType;
            }
        }

        /// <summary>
        /// Returns true if the reader is positioned on an empty element.
        /// </summary>
        public override bool IsEmptyElement
        {
            get 
            {
                return this.currentBufferedNodeToReport != null ? this.currentBufferedNodeToReport.Value.IsEmptyElement : this.reader.IsEmptyElement;
            }
        }

        /// <summary>
        /// Returns the local name of the current node.
        /// </summary>
        public override string LocalName
        {
            get
            {
                return this.currentBufferedNodeToReport != null ? this.currentBufferedNodeToReport.Value.LocalName : this.reader.LocalName;
            }
        }

        /// <summary>
        /// Returns the namespace URI of the current node.
        /// </summary>
        public override string NamespaceURI
        {
            get 
            {
                return this.currentBufferedNodeToReport != null ? this.currentBufferedNodeToReport.Value.NamespaceUri : this.reader.NamespaceURI;
            }
        }

        /// <summary>
        /// Returns the value of the current node.
        /// </summary>
        public override string Value
        {
            get 
            {
                return this.currentBufferedNodeToReport != null ? this.currentBufferedNodeToReport.Value.Value : this.reader.Value;
            }
        }

        /// <summary>
        /// Returns the depth of the current node.
        /// </summary>
        public override int Depth
        {
            get 
            {
                return this.currentBufferedNodeToReport != null ? this.currentBufferedNodeToReport.Value.Depth : this.reader.Depth;
            }
        }

        /// <summary>
        /// Returns true if the end of input was reached.
        /// </summary>
        public override bool EOF
        {
            get
            {
                return this.currentBufferedNodeToReport != null ? this.IsEndOfInputNode(this.currentBufferedNodeToReport.Value) : this.reader.EOF;
            }
        }

        /// <summary>
        /// Returns the current state of the reader.
        /// </summary>
        /// <remarks>We need to support ReadState in order for Skip to work without us implementing it again.</remarks>
        public override ReadState ReadState
        {
            get
            {
                if (this.currentBufferedNodeToReport != null)
                {
                    if (this.IsEndOfInputNode(this.currentBufferedNodeToReport.Value))
                    {
                        return ReadState.EndOfFile;
                    }

                    return this.currentBufferedNodeToReport.Value.NodeType == XmlNodeType.None ? ReadState.Initial : ReadState.Interactive;
                }
                
                return this.reader.ReadState;
            }
        }

        /// <summary>
        /// Returns the nametable used by the reader.
        /// </summary>
        public override XmlNameTable NameTable
        {
            get
            {
                return this.reader.NameTable;
            }
        }

        #region Unsupported properties in buffering mode
        /// <summary>
        /// Returns the number of attributes on the node.
        /// </summary>
        public override int AttributeCount
        {
            get
            {
                if (this.currentBufferedNodeToReport != null)
                {
                    Debug.Assert(false, "The AttributeCount property on BufferingXmlReader should not be used when in buffering mode.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.BufferingXmlReader_UnsupportedMethodWhileBuffering));
                }
                
                return this.reader.AttributeCount;
            }
        }

        /// <summary>
        /// Returns the base URI of the node - note that this is not based on the xml:base attribute, just the input streams.
        /// </summary>
        public override string BaseURI
        {
            get
            {
                if (this.currentBufferedNodeToReport != null)
                {
                    Debug.Assert(false, "The BaseURI property on BufferingXmlReader should not be used when in buffering mode.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.BufferingXmlReader_UnsupportedMethodWhileBuffering));
                }
                
                return this.reader.BaseURI;
            }
        }

        /// <summary>
        /// Returns true if the node has value.
        /// </summary>
        public override bool HasValue
        {
            get
            {
                if (this.currentBufferedNodeToReport != null)
                {
                    Debug.Assert(false, "The HasValue property on BufferingXmlReader should not be used when in buffering mode.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.BufferingXmlReader_UnsupportedMethodWhileBuffering));
                }
                
                return this.reader.HasValue;
            }
        }

        /// <summary>
        /// Returns the prefix of the current node.
        /// </summary>
        public override string Prefix
        {
            get
            {
                if (this.currentBufferedNodeToReport != null)
                {
                    Debug.Assert(false, "The Prefix property on BufferingXmlReader should not be used when in buffering mode.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.BufferingXmlReader_UnsupportedMethodWhileBuffering));
                }
                
                return this.reader.Prefix;
            }
        }
        #endregion Unsupported properties in buffering mode

        /// <summary>
        /// The active XML base URI for the current node.
        /// </summary>
        internal Uri XmlBaseUri
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();

                return this.xmlBaseStack.Count > 0 ? this.xmlBaseStack.Peek().BaseUri : null;
            }
        }

        /// <summary>
        /// Flag to control whether in-stream errors should be detected when reading.
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
        /// <returns>true if another node is available and the reader has moved to it; false if end of input was reached.</returns>
        public override bool Read()
        {
            // If we were on end element or empty element pop the XML base definition for this level.
            if (!this.disableXmlBase && this.xmlBaseStack.Count > 0)
            {
                XmlNodeType nodeType = this.NodeType;
                if (nodeType == XmlNodeType.Attribute)
                {
                    this.MoveToElement();
                    nodeType = XmlNodeType.Element;
                }

                if (this.xmlBaseStack.Peek().Depth == this.Depth)
                {
                    if (nodeType == XmlNodeType.EndElement || (nodeType == XmlNodeType.Element && this.IsEmptyElement))
                    {
                        this.xmlBaseStack.Pop();
                    }
                }
            }

            bool result = this.ReadInternal(this.disableInStreamErrorDetection);

            // Push a new XML base definition for this level if there's an xml:base attribute.
            if (result && !this.disableXmlBase)
            {
                if (this.NodeType == XmlNodeType.Element)
                {
                    string xmlBaseAttributeValue = this.GetAttributeValue(this.XmlBaseAttributeName, this.XmlNamespace);

                    if (!string.IsNullOrEmpty(xmlBaseAttributeValue))
                    {
                        Uri newBaseUri = new Uri(xmlBaseAttributeValue, UriKind.RelativeOrAbsolute);
                        if (!newBaseUri.IsAbsoluteUri)
                        {
                            if (this.xmlBaseStack.Count == 0)
                            {
                                throw new ODataException(Strings.BufferingXmlReader_TopLevelXmlBaseMustBeAbsolute);
                            }
                            
                            // If xml base of the current element is an absolute Uri, it overrides that of the parent node.
                            // If xml base of the current element is a relative Uri, it must be relative to the parent xml base.
                            // For more information, look into section 3 of the following RFC
                            // http://www.w3.org/TR/xmlbase/
                            newBaseUri = UriUtils.UriToAbsoluteUri(this.xmlBaseStack.Peek().BaseUri, newBaseUri);
                        }
                        
                        this.xmlBaseStack.Push(new XmlBaseDefinition(newBaseUri, this.Depth));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Moves the reader to the element which owns the current attribute.
        /// </summary>
        /// <returns>true if the reader has moved (that is the current node was an attribute);
        /// false if the reader didn't move (the reader was already positioned on an element or other node).</returns>
        public override bool MoveToElement()
        {
            this.ValidateInternalState();

            if (this.bufferedNodes.Count > 0)
            {
                if (this.isBuffering)
                {
                    Debug.Assert(this.currentBufferedNode != null, "this.currentBufferedNode != null");
                    Debug.Assert(this.currentBufferedNodeToReport != null, "this.currentBufferedNodeToReport != null");

                    if (this.currentBufferedNodeToReport.Value.NodeType == XmlNodeType.Attribute)
                    {
                        Debug.Assert(
                            this.currentBufferedNode.Value.NodeType == XmlNodeType.Element, 
                            "If the current node to report is attribute, the current node must be an element.");
                        this.currentBufferedNodeToReport = this.currentBufferedNode;
                        this.ValidateInternalState();
                        return true;
                    }
                    
                    return false;
                }

                // in non-buffering mode if we have buffered nodes satisfy the request from the first node there
                Debug.Assert(
                    this.currentBufferedNodeToReport != null,
                    "Even if we're not buffering, if we are reporting from the first node in the buffer, the current node to report must no be null.");

                if (this.currentBufferedNodeToReport.Value.NodeType == XmlNodeType.Attribute)
                {
                    Debug.Assert(
                        this.bufferedNodes.First.Value.NodeType == XmlNodeType.Element,
                        "If the current node to report is attribute, the current node must be an element.");
                    this.currentBufferedNodeToReport = this.bufferedNodes.First;
                    this.ValidateInternalState();
                    return true;
                }
                
                return false;
            }
            
            return this.reader.MoveToElement();
        }

        /// <summary>
        /// Moves the reader to the first attribute of the current element.
        /// </summary>
        /// <returns>true if the reader moved to the first attribute; false if there are no attribute for the current node (the reader didn't move).</returns>
        public override bool MoveToFirstAttribute()
        {
            this.ValidateInternalState();

            if (this.bufferedNodes.Count > 0)
            {
                BufferedNode elementNode;
                if (this.isBuffering)
                {
                    Debug.Assert(this.currentBufferedNode != null, "this.currentBufferedNode != null");
                    elementNode = this.currentBufferedNode.Value;
                }
                else
                {
                    // in non-buffering mode if we have buffered nodes satisfy the request from the first node there
                    elementNode = this.bufferedNodes.First.Value;
                }

                Debug.Assert(elementNode != null, "elementNode != null");
                if (elementNode.NodeType == XmlNodeType.Element && elementNode.AttributeNodes.Count > 0)
                {
                    this.currentBufferedNodeToReport = elementNode.AttributeNodes.First;
                    this.ValidateInternalState();
                    return true;
                }
                
                return false;
            }
            
            return this.reader.MoveToFirstAttribute();
        }

        /// <summary>
        /// Moves the reader to the next attribute on the current element.
        /// </summary>
        /// <returns>true if the reader moved to the next attribute (if the node was an element it moves to the first attribute);
        /// false if the reader didn't move (no attributes for the current node).</returns>
        public override bool MoveToNextAttribute()
        {
            this.ValidateInternalState();

            if (this.bufferedNodes.Count > 0)
            {
                Debug.Assert(
                    this.currentBufferedNodeToReport != null,
                    "If we are reporting nodes from buffer (doesn't matter if buffering or not), we must have a current node to report.");

                if (this.currentBufferedNodeToReport.Value.NodeType == XmlNodeType.Attribute)
                {
                    if (this.currentBufferedNodeToReport.Next != null)
                    {
                        this.currentBufferedNodeToReport = this.currentBufferedNodeToReport.Next;
                        this.ValidateInternalState();
                        return true;
                    }
                    
                    return false;
                }

                if (this.currentBufferedNodeToReport.Value.NodeType == XmlNodeType.Element)
                {
                    Debug.Assert(
                        (this.isBuffering && this.currentBufferedNodeToReport == this.currentBufferedNode) || (!this.isBuffering && this.currentBufferedNodeToReport == this.bufferedNodes.First),
                        "If the node to report is element: if buffer it must be the current buffered node, otherwise it must be the first node in the buffer.");

                    if (this.currentBufferedNodeToReport.Value.AttributeNodes.Count > 0)
                    {
                        this.currentBufferedNodeToReport = this.currentBufferedNodeToReport.Value.AttributeNodes.First;
                        this.ValidateInternalState();
                        return true;
                    }

                    return false;
                }

                return false;
            }

            return this.reader.MoveToNextAttribute();
        }

        /// <summary>
        /// Reads the next node from the value of an attribute.
        /// </summary>
        /// <returns>true if next node was available; false if end of the attribute value was reached.</returns>
        public override bool ReadAttributeValue()
        {
            this.ValidateInternalState();

            if (this.bufferedNodes.Count > 0)
            {
                Debug.Assert(false, "The ReadAttributeValue method on BufferingXmlReader should not be used when in buffering mode.");
                throw new ODataException(Strings.General_InternalError(InternalErrorCodes.BufferingXmlReader_UnsupportedMethodWhileBuffering));
            }
            
            return this.reader.ReadAttributeValue();
        }

        #region Unsupported methods
        /// <summary>
        /// Closes the reader and the underlying input.
        /// </summary>
        public override void Close()
        {
            Debug.Assert(false, "The Close method on BufferingXmlReader should not be used.");
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.BufferingXmlReader_UnsupportedMethod));
        }

        /// <summary>
        /// Returns the value of an attribute based on its index.
        /// </summary>
        /// <param name="i">The index of the attribute, starts at 0.</param>
        /// <returns>The value of the attribute at index <paramref name="i"/>.</returns>
        public override string GetAttribute(int i)
        {
            Debug.Assert(false, "The GetAttribute(int) method on BufferingXmlReader should not be used.");
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.BufferingXmlReader_UnsupportedMethod));
        }

        /// <summary>
        /// Returns the value of an attribute based on its fully qualified name.
        /// </summary>
        /// <param name="name">The local name of the attribute.</param>
        /// <param name="namespaceURI">The namespace URI of the attribute.</param>
        /// <returns>The value of the attribute with specified <paramref name="name"/> and <paramref name="namespaceURI"/>.</returns>
        public override string GetAttribute(string name, string namespaceURI)
        {
            Debug.Assert(false, "The GetAttribute(string, string) method on BufferingXmlReader should not be used.");
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.BufferingXmlReader_UnsupportedMethod));
        }

        /// <summary>
        /// Returns the value of an attribute based on its name.
        /// </summary>
        /// <param name="name">The name of the attribute. (prefix:localname)</param>
        /// <returns>The value of the attribute with specified <paramref name="name"/>.</returns>
        public override string GetAttribute(string name)
        {
            Debug.Assert(false, "The GetAttribute(string) method on BufferingXmlReader should not be used.");
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.BufferingXmlReader_UnsupportedMethod));
        }

        /// <summary>
        /// Looks up a namespace URI based on the prefix.
        /// </summary>
        /// <param name="prefix">The prefix to search for.</param>
        /// <returns>The namespace URI for the specified <paramref name="prefix"/>.</returns>
        public override string LookupNamespace(string prefix)
        {
            Debug.Assert(false, "The LookupNamespace(string) method on BufferingXmlReader should not be used.");
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.BufferingXmlReader_UnsupportedMethod));
        }

        /// <summary>
        /// Moves the reader to the attribute specified by fully qualified name.
        /// </summary>
        /// <param name="name">The local name of the attribute.</param>
        /// <param name="ns">The namespace URI of the attribute.</param>
        /// <returns>true if the attribute specified by <paramref name="name"/> and <paramref name="ns"/> was found and the reader is positioned on it;
        /// false otherwise.</returns>
        public override bool MoveToAttribute(string name, string ns)
        {
            Debug.Assert(false, "The MoveToAttribute(string, string) method on BufferingXmlReader should not be used.");
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.BufferingXmlReader_UnsupportedMethod));
        }

        /// <summary>
        /// Moves the reader to the attribute specified by name.
        /// </summary>
        /// <param name="name">The name of the attribute (prefix:localname).</param>
        /// <returns>true if the attribute specified by <paramref name="name"/> was found and the reader is positioned on it;
        /// false otherwise.</returns>
        public override bool MoveToAttribute(string name)
        {
            Debug.Assert(false, "The MoveToAttribute(string) method on BufferingXmlReader should not be used.");
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.BufferingXmlReader_UnsupportedMethod));
        }

        /// <summary>
        /// Resolves the current entity node.
        /// </summary>
        public override void ResolveEntity()
        {
            Debug.Assert(false, "The ResolveEntityt method on BufferingXmlReader should not be used.");
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.BufferingXmlReader_UnsupportedMethod));
        }
        #endregion Unsupported methods

        /// <summary>
        /// Puts the reader into the state where it buffers read nodes.
        /// </summary>
        internal void StartBuffering()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!this.isBuffering, "Buffering is already turned on. Must not call StartBuffering again.");
            Debug.Assert(this.NodeType != XmlNodeType.Attribute, "Buffering cannot start on an attribute.");
            this.ValidateInternalState();

            if (this.bufferedNodes.Count == 0)
            {
                // capture the current state of the reader as the first item in the buffer (if there are none)
                this.bufferedNodes.AddFirst(new BufferedNode(this.reader));
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
            Debug.Assert(this.currentBufferedNode == null, "When starting to buffer, the currentBufferedNode must be null, since we always reset to the begining of the buffer.");
            this.currentBufferedNode = this.bufferedNodes.First;

            this.currentBufferedNodeToReport = this.currentBufferedNode;

            // Copy the existing XML base stack to a stored copy, so that we can restart it when we stop buffering.
            int xmlBaseStackCount = this.xmlBaseStack.Count;
            switch (xmlBaseStackCount)
            {
                case 0:
                    this.bufferStartXmlBaseStack = new Stack<XmlBaseDefinition>();
                    break;
                case 1:
                    this.bufferStartXmlBaseStack = new Stack<XmlBaseDefinition>();
                    this.bufferStartXmlBaseStack.Push(this.xmlBaseStack.Peek());
                    break;
                default:
                    XmlBaseDefinition[] array = this.xmlBaseStack.ToArray();
                    this.bufferStartXmlBaseStack = new Stack<XmlBaseDefinition>(xmlBaseStackCount);
                    for (int i = xmlBaseStackCount - 1; i >= 0; i--)
                    {
                        this.bufferStartXmlBaseStack.Push(array[i]);
                    }

                    break;
            }

            this.isBuffering = true;
            this.ValidateInternalState();
        }

        /// <summary>
        /// Puts the reader into the state where no buffering happen on read.
        /// Either buffered nodes are consumed or new nodes are read (and not buffered).
        /// </summary>
        internal void StopBuffering()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.isBuffering, "Buffering is not turned on. Must not call StopBuffering in this state.");
            this.ValidateInternalState();

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
            if (this.bufferedNodes.Count > 0)
            {
                this.currentBufferedNodeToReport = this.bufferedNodes.First;
            }

            // Restore the XML base stack as it was when we started buffering
            this.xmlBaseStack = this.bufferStartXmlBaseStack;
            this.bufferStartXmlBaseStack = null;

            this.ValidateInternalState();
        }

        /// <summary>
        /// Returns the value of an attribute based on its fully qualified name.
        /// </summary>
        /// <param name="localName">The local name of the attribute. Must be atomized.</param>
        /// <param name="namespaceUri">The namespace URI of the attribute. Must be atomized.</param>
        /// <returns>The value of the attribute with specified <paramref name="localName"/> and <paramref name="namespaceUri"/>.</returns>
        /// <remarks>The GetAttribute(string, string) has the same functionality, but we don't implement that since there we could not guarantee
        /// that the names are atomized and thus we would have to perform more costly comparisons.</remarks>
        internal string GetAttributeValue(string localName, string namespaceUri)
        {
            DebugUtils.CheckNoExternalCallers();
            this.ValidateInternalState();
            Debug.Assert(object.ReferenceEquals(this.reader.NameTable.Get(namespaceUri), namespaceUri), "The namespaceUri was not atomized on this reader.");
            Debug.Assert(object.ReferenceEquals(this.reader.NameTable.Get(localName), localName), "The localName was not atomized on this reader.");

            if (this.bufferedNodes.Count > 0)
            {
                BufferedNode elementNode;
                if (this.isBuffering)
                {
                    Debug.Assert(this.currentBufferedNode != null, "this.currentBufferedNode != null");
                    elementNode = this.currentBufferedNode.Value;
                }
                else
                {
                    // in non-buffering mode if we have buffered nodes satisfy the request from the first node there
                    elementNode = this.bufferedNodes.First.Value;
                }

                Debug.Assert(elementNode != null, "elementNode != null");
                if (elementNode.NodeType == XmlNodeType.Element && elementNode.AttributeNodes.Count > 0)
                {
                    LinkedListNode<BufferedNode> attributeNode = elementNode.AttributeNodes.First;
                    while (attributeNode != null)
                    {
                        BufferedNode bufferedAttribute = attributeNode.Value;
                        if (object.ReferenceEquals(bufferedAttribute.NamespaceUri, namespaceUri) && object.ReferenceEquals(bufferedAttribute.LocalName, localName))
                        {
                            return bufferedAttribute.Value;
                        }

                        attributeNode = attributeNode.Next;
                    }
                }

                return null;
            }
            
            return this.reader.GetAttribute(localName, namespaceUri);
        }

        /// <summary>
        /// The actual implementatin of the Read method. Moves the reader to the next node.
        /// </summary>
        /// <param name="ignoreInStreamErrors">true if the reader should not check for in-stream errors; otherwise false.</param>
        /// <returns>true if next node is available and the reader has moved; false if end-of-input was reached.</returns>
        private bool ReadInternal(bool ignoreInStreamErrors)
        {
            this.ValidateInternalState();

            if (this.removeOnNextRead)
            {
                Debug.Assert(this.bufferedNodes.Count > 0, "If removeOnNextRead is true we must have at least one node in the buffer.");
                this.currentBufferedNodeToReport = this.currentBufferedNodeToReport.Next;
                this.bufferedNodes.RemoveFirst();
                this.removeOnNextRead = false;
            }

            bool result;
            if (this.isBuffering)
            {
                Debug.Assert(this.currentBufferedNode != null, "this.currentBufferedNode != null");

                if (this.currentBufferedNode.Next != null)
                {
                    Debug.Assert(!ignoreInStreamErrors, "!ignoreInStreamErrors");
                    this.currentBufferedNode = this.currentBufferedNode.Next;
                    this.currentBufferedNodeToReport = this.currentBufferedNode;
                    result = true;
                }
                else
                {
                    if (ignoreInStreamErrors)
                    {
                        // read more from the input stream and buffer it
                        result = this.reader.Read();
                        this.bufferedNodes.AddLast(result ? new BufferedNode(this.reader) : this.endOfInputBufferedNode);
                        this.currentBufferedNode = this.bufferedNodes.Last;
                        this.currentBufferedNodeToReport = this.currentBufferedNode;

                        // If the new node is an element, read all its attributes and buffer those as well
                        if (this.reader.NodeType == XmlNodeType.Element)
                        {
                            while (this.reader.MoveToNextAttribute())
                            {
                                this.currentBufferedNode.Value.AttributeNodes.AddLast(new BufferedNode(this.reader));
                            }
                        }
                    }
                    else
                    {
                        // read the next node from the input stream and check
                        // whether it is an in-stream error
                        result = this.ReadNextAndCheckForInStreamError();
                    }
                }

                Debug.Assert(
                    this.currentBufferedNode == this.currentBufferedNodeToReport,
                    "After buffered Read() the node is not an attribute and thus the current buffered node and the one to report are the same.");
            }
            else
            {
                if (this.bufferedNodes.Count == 0)
                {
                    // read the next node from the input stream and check
                    // whether it is an in-stream error
                    result = ignoreInStreamErrors ? this.reader.Read() : this.ReadNextAndCheckForInStreamError();
                }
                else
                {
                    Debug.Assert(!ignoreInStreamErrors, "!ignoreInStreamErrors");

                    // non-buffering read from the buffer
                    this.currentBufferedNodeToReport = this.bufferedNodes.First;
                    BufferedNode bufferedNode = this.currentBufferedNodeToReport.Value;
                    result = !this.IsEndOfInputNode(bufferedNode);
                    this.removeOnNextRead = true;
                }
            }

            this.ValidateInternalState();
            return result;
        }

        /// <summary>
        /// Reads the next node from the XML reader and if m:error element node is detected starts reading ahead and
        /// tries to parse an in-stream error.
        /// </summary>
        /// <returns>true if a new node was found, or false if end of input was reached.</returns>
        private bool ReadNextAndCheckForInStreamError()
        {
            // read the next node in the current reader mode (buffering or non-buffering)
            bool result = this.ReadInternal(/*ignoreInStreamErrors*/true);

            if (!this.disableInStreamErrorDetection &&
                this.NodeType == XmlNodeType.Element &&
                this.LocalNameEquals(this.ODataErrorElementName) &&
                this.NamespaceEquals(this.ODataMetadataNamespace))
            {
                // If we find an element m:error node we detected an in-stream error.
                // We read the in-stream error and report it.
                ODataError inStreamError = ODataAtomErrorDeserializer.ReadErrorElement(this);
                Debug.Assert(inStreamError != null, "inStreamError != null");
                throw new ODataErrorException(inStreamError);
            }

            return result;
        }

        /// <summary>
        /// Determines if the specified node is the end of input node.
        /// </summary>
        /// <param name="node">The buffered node to test.</param>
        /// <returns>true if the node is the special end of input node, false otherwise.</returns>
        private bool IsEndOfInputNode(BufferedNode node)
        {
            return object.ReferenceEquals(node, this.endOfInputBufferedNode);
        }

        /// <summary>
        /// Validates internal state of the reader - debug only.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This is a DEBUG only method.")]
        [Conditional("DEBUG")]
        private void ValidateInternalState()
        {
#if DEBUG
            if (this.bufferedNodes.Count > 0)
            {
                Debug.Assert(this.currentBufferedNodeToReport != null, "If we have something in the buffer we must have the current node to report.");

                BufferedNode elementNode;
                if (this.isBuffering)
                {
                    Debug.Assert(this.currentBufferedNode != null, "If buffering we must have current buffered node.");
                    elementNode = this.currentBufferedNode.Value;
                }
                else
                {
                    Debug.Assert(this.currentBufferedNode == null, "If we are not buffer, we must not have any current buffered node.");
                    elementNode = this.bufferedNodes.First.Value;
                }

                Debug.Assert(elementNode.NodeType != XmlNodeType.Attribute, "The current buffered node must never be an attribute.");

                if (this.currentBufferedNodeToReport.Value.NodeType == XmlNodeType.Attribute)
                {
                    Debug.Assert(
                        elementNode.NodeType == XmlNodeType.Element,
                        "If the current reported node is attribute, the current buffered node must be element.");
                }
            }
            else
            {
                Debug.Assert(this.currentBufferedNode == null, "If we don't have anything in the buffer, no buffered node is available.");
                Debug.Assert(this.currentBufferedNodeToReport == null, "If we don't have anything in the buffer, no buffered node is available.");
            }

            Debug.Assert(this.bufferStartXmlBaseStack == null || this.isBuffering, "The buffered XML base stack should only be used when buffering.");
#endif
        }

        /// <summary>
        /// Class representing one buffered XML node
        /// </summary>
        private sealed class BufferedNode
        {
            /// <summary>
            /// The list of attribute nodes, if this node is an element node.
            /// </summary>
            private LinkedList<BufferedNode> attributeNodes;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="reader">The XML reader to get all the interesting values from. The reader
            /// is positioned on the node which the new buffered node should buffer.</param>
            internal BufferedNode(XmlReader reader)
            {
                Debug.Assert(reader != null, "reader != null");

                this.NodeType = reader.NodeType;
                this.NamespaceUri = reader.NamespaceURI;
                this.LocalName = reader.LocalName;
                this.Value = reader.Value;
                this.Depth = reader.Depth;
                this.IsEmptyElement = reader.IsEmptyElement;
            }

            /// <summary>
            /// Constructor for end of input node.
            /// </summary>
            /// <param name="emptyString">The atomized instance of an empty string.</param>
            private BufferedNode(string emptyString)
            {
                this.NodeType = XmlNodeType.None;
                this.NamespaceUri = emptyString;
                this.LocalName = emptyString;
                this.Value = emptyString;

                // Depth is 0 by default
                // IsEmptyElement is false by default
            }

            /// <summary>The type of the buffered node.</summary>
            internal XmlNodeType NodeType { get; private set; }

            /// <summary>The namespace URI of the buffered node.</summary>
            internal string NamespaceUri { get; private set; }

            /// <summary>The local name of the buffered node.</summary>
            internal string LocalName { get; private set; }

            /// <summary>The value of the buffered node.</summary>
            internal string Value { get; private set; }

            /// <summary>The depth of the buffered node.</summary>
            internal int Depth { get; private set; }

            /// <summary>Denotes if the buffered node is an empty element.</summary>
            internal bool IsEmptyElement { get; private set; }

            /// <summary>List of attributes. If the node is not element, this will be null.</summary>
            internal LinkedList<BufferedNode> AttributeNodes
            {
                get
                {
                    if (this.NodeType == XmlNodeType.Element && this.attributeNodes == null)
                    {
                        this.attributeNodes = new LinkedList<BufferedNode>();
                    }

                    return this.attributeNodes;
                }
            }

            /// <summary>
            /// Creates a special node which represents the end of input.
            /// </summary>
            /// <param name="nametable">The nametable of the underlying reader.</param>
            /// <returns>The newly created node.</returns>
            internal static BufferedNode CreateEndOfInput(XmlNameTable nametable)
            {
                string emptyString = nametable.Add(string.Empty);
                return new BufferedNode(emptyString);
            }
        }

        /// <summary>
        /// Helper class to store XML base URI definition for a specific depth of the reader.
        /// </summary>
        private sealed class XmlBaseDefinition
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="baseUri">The XML base URI for the definition.</param>
            /// <param name="depth">The depth of the XML reader for the definition.</param>
            internal XmlBaseDefinition(Uri baseUri, int depth)
            {
                Debug.Assert(baseUri != null, "baseUri != null");
                Debug.Assert(baseUri.IsAbsoluteUri, "Only absolute URIs can be used as base URIs.");

                this.BaseUri = baseUri;
                this.Depth = depth;
            }

            /// <summary>The base URI for this definition.</summary>
            internal Uri BaseUri { get; private set; }

            /// <summary>The depth of the XmlReader on which this XML base is defined.</summary>
            internal int Depth { get; private set; }
        }
    }
}
