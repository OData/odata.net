//---------------------------------------------------------------------
// <copyright file="BufferingXmlReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Metadata
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Xml;
    #endregion Namespaces

    /// <summary>
    /// XML reader which supports look-ahead.
    /// </summary>
    internal sealed class BufferingXmlReader : XmlReader, IXmlLineInfo
    {
        #region Atomized strings
        /// <summary>The "http://www.w3.org/XML/1998/namespace" namespace for the "xml" prefix.</summary>
        internal readonly string XmlNamespace;

        /// <summary>The "base" name for the XML base attribute.</summary>
        internal readonly string XmlBaseAttributeName;

        /// <summary>XML namespace for data service annotations.</summary>
        internal readonly string ODataMetadataNamespace;

        /// <summary>XML namespace for data services. This is to provide compatibility with WCF DS client which accepts custom data namespace value.</summary>
        internal readonly string ODataNamespace;

        /// <summary>The 'error' local name of the error element.</summary>
        internal readonly string ODataErrorElementName;
        #endregion Atomized strings

        /// <summary>Object converted from this.reader to provide line number and position (if presented).</summary>
        private readonly IXmlLineInfo lineInfo;

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

        /// <summary>The maximum number of recursive internalexception elements to allow when reading in-stream errors.</summary>
        private readonly int maxInnerErrorDepth;

        /// <summary>The base URI for the document.</summary>
        private readonly Uri documentBaseUri;

        /// <summary>A pointer into the bufferedNodes list to track the most recent position of the current buffered node.</summary>
        private LinkedListNode<BufferedNode> currentBufferedNode;

        /// <summary>
        /// A pointer into the linked list of attribute nodes which is only used if the currentBufferedNodeToReport is the attribute value node (not the attribute itself).
        /// In that case it points to the current attribute node.
        /// In all other cases this node is null.</summary>
        private LinkedListNode<BufferedNode> currentAttributeNode;

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
        /// <param name="parentXmlBaseUri">If this reader is wrapping an inner reader of some kind, this parameter should pass the xml:base effective value of the parent.</param>
        /// <param name="documentBaseUri">The base URI for the document.</param>
        /// <param name="disableXmlBase">Flag to control if the xml:base attributes should be processed when reading.</param>
        /// <param name="maxInnerErrorDepth">The maximum number of recursive internalexception elements to allow when reading in-stream errors.</param>
        internal BufferingXmlReader(XmlReader reader, Uri parentXmlBaseUri, Uri documentBaseUri, bool disableXmlBase, int maxInnerErrorDepth)
        {
            Debug.Assert(reader != null, "reader != null");
            Debug.Assert(documentBaseUri == null || documentBaseUri.IsAbsoluteUri, "The document Base URI must be absolute if it's specified.");

            this.reader = reader;
            this.lineInfo = reader as IXmlLineInfo;
            this.documentBaseUri = documentBaseUri;
            this.disableXmlBase = disableXmlBase;
            this.maxInnerErrorDepth = maxInnerErrorDepth;

            XmlNameTable nameTable = this.reader.NameTable;
            this.XmlNamespace = nameTable.Add(ODataMetadataConstants.XmlNamespace);
            this.XmlBaseAttributeName = nameTable.Add(ODataMetadataConstants.XmlBaseAttributeName);
            this.ODataMetadataNamespace = nameTable.Add(ODataMetadataConstants.ODataMetadataNamespace);
            this.ODataNamespace = nameTable.Add(ODataMetadataConstants.ODataNamespace);
            this.ODataErrorElementName = nameTable.Add(ODataMetadataConstants.ODataErrorElementName);

            this.bufferedNodes = new LinkedList<BufferedNode>();
            this.currentBufferedNode = null;
            this.endOfInputBufferedNode = BufferedNode.CreateEndOfInput(this.reader.NameTable);
            this.xmlBaseStack = new Stack<XmlBaseDefinition>();

            // If there's a parent xml:base we need to push it onto our stack so that it's correctly propagated
            // Note that it's not the same as using it as documentBaseUri, since that one is only used to resolve relative xml:base values
            // not to report if there's no xml:base.
            if (parentXmlBaseUri != null)
            {
                this.xmlBaseStack.Push(new XmlBaseDefinition(parentXmlBaseUri, 0));
            }
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
        /// Returns the prefix of the current node.
        /// </summary>
        public override string Prefix
        {
            get
            {
                return this.currentBufferedNodeToReport != null ? this.currentBufferedNodeToReport.Value.Prefix : this.reader.Prefix;
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

        /// <summary>
        /// Returns the number of attributes on the node.
        /// </summary>
        public override int AttributeCount
        {
            get
            {
                if (this.currentBufferedNodeToReport != null)
                {
                    return this.currentBufferedNodeToReport.Value.AttributeNodes != null
                        ? this.currentBufferedNodeToReport.Value.AttributeNodes.Count
                        : 0;
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
                // In ODataLib this reader is always created over a stream and in addition we don't allow DTD, so the base URI can never change.
                // We also don't have a scenario where the BaseURI as reported by the XmlReader, that is the URI of the input stream
                // would be useful.
                // So return null for now, so that nobody can rely on the value even if the underlying reader has some.
                return null;
            }
        }

        /// <summary>
        /// Returns true if the current node has a value.
        /// </summary>
        public override bool HasValue
        {
            get
            {
                if (this.currentBufferedNodeToReport != null)
                {
                    switch (this.NodeType)
                    {
                        case XmlNodeType.Attribute:
                        case XmlNodeType.Text:
                        case XmlNodeType.CDATA:
                        case XmlNodeType.ProcessingInstruction:
                        case XmlNodeType.Comment:
                        case XmlNodeType.DocumentType:
                        case XmlNodeType.Whitespace:
                        case XmlNodeType.SignificantWhitespace:
                        case XmlNodeType.XmlDeclaration:
                            return true;
                        default:
                            return false;
                    }
                }

                return this.reader.HasValue;
            }
        }

        /// <summary>
        /// Explicit IXmlLineInfo.LineNumber implementation.
        /// This property provides current line number of this reader.
        /// </summary>
        int IXmlLineInfo.LineNumber
        {
            get { return this.HasLineInfo() ? lineInfo.LineNumber : 0; }
        }

        /// <summary>
        /// Explicit IXmlLineInfo.LinePosition implementation.
        /// This property provides current line position of this reader.
        /// </summary>
        int IXmlLineInfo.LinePosition
        {
            get { return this.HasLineInfo() ? lineInfo.LinePosition : 0; }
        }

        /// <summary>
        /// The active XML base URI for the current node.
        /// </summary>
        internal Uri XmlBaseUri
        {
            get
            {
                return this.xmlBaseStack.Count > 0 ? this.xmlBaseStack.Peek().BaseUri : null;
            }
        }

        /// <summary>
        /// The active XML base URI for the parent node (parent element) of the current node.
        /// </summary>
        internal Uri ParentXmlBaseUri
        {
            get
            {
                if (this.xmlBaseStack.Count == 0)
                {
                    return null;
                }

                XmlBaseDefinition xmlBaseDefinition = this.xmlBaseStack.Peek();

                // The xml:base stack only keeps record for when the value of the active xml:base changes.
                // So we only need to skip the top one if it's of the same depth as the current node since we want the parent one.
                if (xmlBaseDefinition.Depth == this.Depth)
                {
                    // If there's just one record on the stack, it means we don't have a record for the parent node
                    // so return null then.
                    if (this.xmlBaseStack.Count == 1)
                    {
                        return null;
                    }

                    xmlBaseDefinition = this.xmlBaseStack.Skip(1).First();
                }

                return xmlBaseDefinition.BaseUri;
            }
        }

        /// <summary>
        /// Flag to control whether in-stream errors should be detected when reading.
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

#if DEBUG
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
                    string xmlBaseAttributeValue = this.GetAttributeWithAtomizedName(this.XmlBaseAttributeName, this.XmlNamespace);

                    // We need to treat empty uri as a valid relative URI
                    if (xmlBaseAttributeValue != null)
                    {
                        Uri newBaseUri = new Uri(xmlBaseAttributeValue, UriKind.RelativeOrAbsolute);
                        if (!newBaseUri.IsAbsoluteUri)
                        {
                            // If the top-most xml:base is relative, use the document base URI (The one from the settings)
                            // to make it absolute.
                            if (this.xmlBaseStack.Count == 0)
                            {
                                if (this.documentBaseUri == null)
                                {
                                    // If there's no document base URI we need to fail since there's no way to create an absolute URI now.
                                    throw new ODataException(Strings.ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified(xmlBaseAttributeValue));
                                }

                                newBaseUri = UriUtils.UriToAbsoluteUri(this.documentBaseUri, newBaseUri);
                            }
                            else
                            {
                                // If xml base of the current element is an absolute Uri, it overrides that of the parent node.
                                // If xml base of the current element is a relative Uri, it must be relative to the parent xml base.
                                // For more information, look into section 3 of the following RFC
                                // http://www.w3.org/TR/xmlbase/
                                newBaseUri = UriUtils.UriToAbsoluteUri(this.xmlBaseStack.Peek().BaseUri, newBaseUri);
                            }
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
                this.MoveFromAttributeValueNode();

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
                BufferedNode elementNode = this.GetCurrentElementNode();
                if (elementNode.NodeType == XmlNodeType.Element && elementNode.AttributeNodes.Count > 0)
                {
                    this.currentAttributeNode = null;
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

                LinkedListNode<BufferedNode> attributeNode = this.currentAttributeNode;
                if (attributeNode == null)
                {
                    attributeNode = this.currentBufferedNodeToReport;
                }

                if (attributeNode.Value.NodeType == XmlNodeType.Attribute)
                {
                    if (attributeNode.Next != null)
                    {
                        this.currentAttributeNode = null;
                        this.currentBufferedNodeToReport = attributeNode.Next;
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
                if (this.currentBufferedNodeToReport.Value.NodeType != XmlNodeType.Attribute)
                {
                    return false;
                }

                // If we're already on an attribute value node, return false, since we always report the entire attribute value as one node.
                if (this.currentAttributeNode != null)
                {
                    return false;
                }

                // Otherwise create the new "fake" node for the attribute value
                BufferedNode attributeValueBufferedNode = new BufferedNode(this.currentBufferedNodeToReport.Value.Value, this.currentBufferedNodeToReport.Value.Depth, this.NameTable);
                LinkedListNode<BufferedNode> attributeValueNode = new LinkedListNode<BufferedNode>(attributeValueBufferedNode);
                this.currentAttributeNode = this.currentBufferedNodeToReport;
                this.currentBufferedNodeToReport = attributeValueNode;

                this.ValidateInternalState();
                return true;
            }

            return this.reader.ReadAttributeValue();
        }

        /// <summary>
        /// Returns the value of an attribute based on its index.
        /// </summary>
        /// <param name="i">The index of the attribute, starts at 0.</param>
        /// <returns>The value of the attribute at index <paramref name="i"/>.</returns>
        public override string GetAttribute(int i)
        {
            this.ValidateInternalState();

            if (this.bufferedNodes.Count > 0)
            {
                if (i < 0 || i >= this.AttributeCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(i));
                }

                LinkedListNode<BufferedNode> attributeNode = this.FindAttributeBufferedNode(i);
                return attributeNode == null ? null : attributeNode.Value.Value;
            }

            return this.reader.GetAttribute(i);
        }

        /// <summary>
        /// Returns the value of an attribute based on its fully qualified name.
        /// </summary>
        /// <param name="name">The local name of the attribute.</param>
        /// <param name="namespaceURI">The namespace URI of the attribute.</param>
        /// <returns>The value of the attribute with specified <paramref name="name"/> and <paramref name="namespaceURI"/>.</returns>
        public override string GetAttribute(string name, string namespaceURI)
        {
            this.ValidateInternalState();

            if (this.bufferedNodes.Count > 0)
            {
                LinkedListNode<BufferedNode> attributeNode = this.FindAttributeBufferedNode(name, namespaceURI);
                return attributeNode == null ? null : attributeNode.Value.Value;
            }

            return this.reader.GetAttribute(name, namespaceURI);
        }

        /// <summary>
        /// Returns the value of an attribute based on its name.
        /// </summary>
        /// <param name="name">The name of the attribute. (prefix:localname)</param>
        /// <returns>The value of the attribute with specified <paramref name="name"/>.</returns>
        public override string GetAttribute(string name)
        {
            this.ValidateInternalState();

            if (this.bufferedNodes.Count > 0)
            {
                LinkedListNode<BufferedNode> attributeNode = this.FindAttributeBufferedNode(name);
                return attributeNode == null ? null : attributeNode.Value.Value;
            }

            return this.reader.GetAttribute(name);
        }

        /// <summary>
        /// Looks up a namespace URI based on the prefix.
        /// </summary>
        /// <param name="prefix">The prefix to search for.</param>
        /// <returns>The namespace URI for the specified <paramref name="prefix"/>.</returns>
        public override string LookupNamespace(string prefix)
        {
            throw new NotSupportedException();
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
            this.ValidateInternalState();

            if (this.bufferedNodes.Count > 0)
            {
                LinkedListNode<BufferedNode> attributeNode = this.FindAttributeBufferedNode(name, ns);
                if (attributeNode != null)
                {
                    this.currentAttributeNode = null;
                    this.currentBufferedNodeToReport = attributeNode;
                    this.ValidateInternalState();
                    return true;
                }

                return false;
            }

            return this.reader.MoveToAttribute(name, ns);
        }

        /// <summary>
        /// Moves the reader to the attribute specified by name.
        /// </summary>
        /// <param name="name">The name of the attribute (prefix:localname).</param>
        /// <returns>true if the attribute specified by <paramref name="name"/> was found and the reader is positioned on it;
        /// false otherwise.</returns>
        public override bool MoveToAttribute(string name)
        {
            this.ValidateInternalState();

            if (this.bufferedNodes.Count > 0)
            {
                LinkedListNode<BufferedNode> attributeNode = this.FindAttributeBufferedNode(name);
                if (attributeNode != null)
                {
                    this.currentAttributeNode = null;
                    this.currentBufferedNodeToReport = attributeNode;
                    this.ValidateInternalState();
                    return true;
                }

                return false;
            }

            return this.reader.MoveToAttribute(name);
        }

        /// <summary>
        /// Resolves the current entity node.
        /// </summary>
        public override void ResolveEntity()
        {
            // We don't support entity references, and we should never get a reader which does.
            throw new InvalidOperationException(Strings.ODataException_GeneralError);
        }

        /// <summary>
        /// Explicit IXmlLineInfo.HasLineInfo() implementation.
        /// Check if this reader has line info.
        /// </summary>
        /// <returns>If line info is presented.</returns>
        bool IXmlLineInfo.HasLineInfo()
        {
            return this.HasLineInfo();
        }

        /// <summary>
        /// Puts the reader into the state where it buffers read nodes.
        /// </summary>
        internal void StartBuffering()
        {
            Debug.Assert(!this.isBuffering, "Buffering is already turned on. Must not call StartBuffering again.");
            Debug.Assert(this.NodeType != XmlNodeType.Attribute, "Buffering cannot start on an attribute.");
            this.ValidateInternalState();

            if (this.bufferedNodes.Count == 0)
            {
                // capture the current state of the reader as the first item in the buffer (if there are none)
                this.bufferedNodes.AddFirst(this.BufferCurrentReaderNode());
            }
            else
            {
                this.removeOnNextRead = false;
            }

            Debug.Assert(this.bufferedNodes.Count > 0, "Expected at least the current node in the buffer when starting buffering.");

            // Set the currentBufferedNode to the first node in the list; this means every time we start buffering we reset the
            // position of the current buffered node since in general we don't know how far ahead we have read before and thus don't
            // want to blindly continuing to read. The model is that with every call to StartBuffering you reset the position of the
            // current node in the list and start reading through the buffer again.
            Debug.Assert(this.currentBufferedNode == null, "When starting to buffer, the currentBufferedNode must be null, since we always reset to the beginning of the buffer.");
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
        /// The actual implementation of the Read method. Moves the reader to the next node.
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

                this.MoveFromAttributeValueNode();

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
                        this.bufferedNodes.AddLast(this.BufferCurrentReaderNode());
                        this.currentBufferedNode = this.bufferedNodes.Last;
                        this.currentBufferedNodeToReport = this.currentBufferedNode;
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
                ODataError inStreamError = ODataAtomErrorDeserializer.ReadErrorElement(this, this.maxInnerErrorDepth);
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
        /// Buffers the current reader state into a node.
        /// </summary>
        /// <returns>The newly created buffered node.</returns>
        private BufferedNode BufferCurrentReaderNode()
        {
            if (this.reader.EOF)
            {
                return this.endOfInputBufferedNode;
            }

            BufferedNode resultNode = new BufferedNode(this.reader);

            // If the new node is an element, read all its attributes and buffer those as well
            if (this.reader.NodeType == XmlNodeType.Element)
            {
                while (this.reader.MoveToNextAttribute())
                {
                    resultNode.AttributeNodes.AddLast(new BufferedNode(this.reader));
                }

                this.reader.MoveToElement();
            }

            return resultNode;
        }

        /// <summary>
        /// Returns the current element node (or node which acts like an element, it doesn't have to be of type Element).
        /// </summary>
        /// <returns>The current element node.</returns>
        private BufferedNode GetCurrentElementNode()
        {
            Debug.Assert(this.bufferedNodes.Count > 0, "This method can only be called if we have buffered nodes.");

            if (this.isBuffering)
            {
                Debug.Assert(this.currentBufferedNode != null, "this.currentBufferedNode != null");
                return this.currentBufferedNode.Value;
            }
            else
            {
                // in non-buffering mode if we have buffered nodes satisfy the request from the first node there
                return this.bufferedNodes.First.Value;
            }
        }

        /// <summary>
        /// Finds the buffered node for the attribute specified by its index.
        /// </summary>
        /// <param name="index">The index of the attribute.</param>
        /// <returns>The linked list node of the found attribute, or null if no such attribute could be found.</returns>
        private LinkedListNode<BufferedNode> FindAttributeBufferedNode(int index)
        {
            Debug.Assert(this.bufferedNodes.Count > 0, "This method can only be called if we have buffered nodes.");

            BufferedNode elementNode = this.GetCurrentElementNode();
            if (elementNode.NodeType == XmlNodeType.Element && elementNode.AttributeNodes.Count > 0)
            {
                LinkedListNode<BufferedNode> attributeNode = elementNode.AttributeNodes.First;
                int attributeIndex = 0;
                while (attributeNode != null)
                {
                    if (attributeIndex == index)
                    {
                        return attributeNode;
                    }

                    attributeIndex++;
                    attributeNode = attributeNode.Next;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the buffered node for the attribute specified by its local name and namespace URI.
        /// </summary>
        /// <param name="localName">The local name of the attribute.</param>
        /// <param name="namespaceUri">The namespace URI of the attribute.</param>
        /// <returns>The linked list node of the found attribute, or null if no such attribute could be found.</returns>
        private LinkedListNode<BufferedNode> FindAttributeBufferedNode(string localName, string namespaceUri)
        {
            Debug.Assert(this.bufferedNodes.Count > 0, "This method can only be called if we have buffered nodes.");

            BufferedNode elementNode = this.GetCurrentElementNode();
            if (elementNode.NodeType == XmlNodeType.Element && elementNode.AttributeNodes.Count > 0)
            {
                LinkedListNode<BufferedNode> attributeNode = elementNode.AttributeNodes.First;
                while (attributeNode != null)
                {
                    BufferedNode bufferedAttribute = attributeNode.Value;
                    if (string.CompareOrdinal(bufferedAttribute.NamespaceUri, namespaceUri) == 0 && string.CompareOrdinal(bufferedAttribute.LocalName, localName) == 0)
                    {
                        return attributeNode;
                    }

                    attributeNode = attributeNode.Next;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the buffered node for the attribute specified by its qualified name.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the attribute to find, that is prefix:localName.</param>
        /// <returns>The linked list node of the found attribute, or null if no such attribute could be found.</returns>
        private LinkedListNode<BufferedNode> FindAttributeBufferedNode(string qualifiedName)
        {
            Debug.Assert(this.bufferedNodes.Count > 0, "This method can only be called if we have buffered nodes.");

            BufferedNode elementNode = this.GetCurrentElementNode();
            if (elementNode.NodeType == XmlNodeType.Element && elementNode.AttributeNodes.Count > 0)
            {
                LinkedListNode<BufferedNode> attributeNode = elementNode.AttributeNodes.First;
                while (attributeNode != null)
                {
                    BufferedNode bufferedAttribute = attributeNode.Value;
                    bool hasPrefix = !string.IsNullOrEmpty(bufferedAttribute.Prefix);
                    if ((!hasPrefix && string.CompareOrdinal(bufferedAttribute.LocalName, qualifiedName) == 0) ||
                        (hasPrefix && string.CompareOrdinal(bufferedAttribute.Prefix + ":" + bufferedAttribute.LocalName, qualifiedName) == 0))
                    {
                        return attributeNode;
                    }

                    attributeNode = attributeNode.Next;
                }
            }

            return null;
        }

        /// <summary>
        /// If the reader is positioned on the attribute value node, this moves it to the owning attribute node.
        /// </summary>
        private void MoveFromAttributeValueNode()
        {
            Debug.Assert(this.bufferedNodes.Count > 0, "This method can only be called if we have buffered nodes.");

            if (this.currentAttributeNode != null)
            {
                this.currentBufferedNodeToReport = this.currentAttributeNode;
                this.currentAttributeNode = null;
            }
        }

        /// <summary>
        /// Returns the value of an attribute based on its fully qualified name.
        /// </summary>
        /// <param name="name">The local name of the attribute. This string must already be atomized against the reader's nametable.</param>
        /// <param name="namespaceURI">The namespace URI of the attribute. This string must already be atomized against the reader's nametable.</param>
        /// <returns>The value of the attribute with specified <paramref name="name"/> and <paramref name="namespaceURI"/>.</returns>
        /// <remarks>
        /// Behaves the same as GetAttribute, but it assumes that the parameters are already atomized against our nametable.
        /// This allows the method to be much faster.
        /// </remarks>
        private string GetAttributeWithAtomizedName(string name, string namespaceURI)
        {
            Debug.Assert(this.NodeType == XmlNodeType.Element, "We must be on an element to use this method.");
            Debug.Assert(object.ReferenceEquals(name, this.NameTable.Get(name)), "The name parameter must be already atomized.");
            Debug.Assert(object.ReferenceEquals(namespaceURI, this.NameTable.Get(namespaceURI)), "The namespaceURI parameter must be already atomized.");
            this.ValidateInternalState();

            if (this.bufferedNodes.Count > 0)
            {
                LinkedListNode<BufferedNode> attributeNode = this.GetCurrentElementNode().AttributeNodes.First;
                while (attributeNode != null)
                {
                    BufferedNode bufferedAttribute = attributeNode.Value;
                    if (object.ReferenceEquals(namespaceURI, bufferedAttribute.NamespaceUri) && object.ReferenceEquals(name, bufferedAttribute.LocalName))
                    {
                        return attributeNode.Value.Value;
                    }

                    attributeNode = attributeNode.Next;
                }

                return null;
            }

            // Instead of calling GetAttribute which is quite slow since it always atomizes the parameters (it doesn't know they're already atomized)
            // perform the search ourselves.
            string valueToReturn = null;
            while (this.reader.MoveToNextAttribute())
            {
                if (object.ReferenceEquals(name, this.reader.LocalName) && object.ReferenceEquals(namespaceURI, this.reader.NamespaceURI))
                {
                    valueToReturn = this.reader.Value;
                    break;
                }
            }

            this.reader.MoveToElement();
            return valueToReturn;
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

                if (this.currentAttributeNode != null)
                {
                    Debug.Assert(
                        elementNode.NodeType == XmlNodeType.Element,
                        "If the current reported node is attribute, the current buffered node must be element.");
                    Debug.Assert(this.currentAttributeNode.Value.NodeType == XmlNodeType.Attribute, "The currentAttributeNode must be an attribute.");
                    Debug.Assert(this.currentBufferedNodeToReport.Value.NodeType == XmlNodeType.Text, "The attribute value node must be a text node.");
                }
                else if (this.currentBufferedNodeToReport.Value.NodeType == XmlNodeType.Attribute)
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
        /// Check if this reader has line info.
        /// </summary>
        /// <returns>If line info is presented.</returns>
        private bool HasLineInfo()
        {
            return lineInfo != null && lineInfo.HasLineInfo();
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
                this.Prefix = reader.Prefix;
                this.Value = reader.Value;
                this.Depth = reader.Depth;
                this.IsEmptyElement = reader.IsEmptyElement;
            }

            /// <summary>
            /// Constructor for an attribute value node
            /// </summary>
            /// <param name="value">The value of the attribute value node to create.</param>
            /// <param name="depth">The parent attribute depth.</param>
            /// <param name="nametable">The nametable to use.</param>
            internal BufferedNode(string value, int depth, XmlNameTable nametable)
            {
                Debug.Assert(value != null, "reader != null");

                string emptyString = nametable.Add(string.Empty);
                this.NodeType = XmlNodeType.Text;
                this.NamespaceUri = emptyString;
                this.LocalName = emptyString;
                this.Prefix = emptyString;
                this.Value = value;
                this.Depth = depth + 1;
                this.IsEmptyElement = false;
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
                this.Prefix = emptyString;
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

            /// <summary>The prefix of the buffered node.</summary>
            internal string Prefix { get; private set; }

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
