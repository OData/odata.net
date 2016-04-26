//---------------------------------------------------------------------
// <copyright file="BufferingXmlReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System;
    using System.Xml;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// The test wrapper of the BufferingXmlReader implementation in the product which is internal.
    /// </summary>
    public class BufferingXmlReader : XmlReader
    {
        /// <summary>
        /// The type of the BufferingXmlReader from the product.
        /// </summary>
        private static Type BufferingXmlReaderType = typeof(Microsoft.OData.ODataAnnotatable).Assembly.GetType("Microsoft.OData.Metadata.BufferingXmlReader");

        /// <summary>
        /// The underlying product instance.
        /// </summary>
        private XmlReader instance;

        /// <summary>
        /// The assertion handler to use to verify certain invariants.
        /// </summary>
        private AssertionHandler assert;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reader">The XML reader to wrap.</param>
        /// <param name="parentXmlBaseUri">The parent xml:base URI to start with.</param>
        /// <param name="documentBaseUri">The base URI for the document.</param>
        /// <param name="disableXmlBase">Flag controling if xml:base attributes should be processed when reading.</param>
        /// <param name="maxInnerErrorDepth">The maximumum number of recursive internalexception elements to allow when reading in-stream errors.</param>
        /// <param name="assert">The assertion handler to use.</param>
        public BufferingXmlReader(XmlReader reader, Uri parentXmlBaseUri, Uri documentBaseUri, bool disableXmlBase, int maxInnerErrorDepth, AssertionHandler assert)
        {
            ExceptionUtilities.CheckArgumentNotNull(reader, "reader");
            this.instance = (XmlReader)ReflectionUtils.CreateInstance(
                BufferingXmlReaderType, 
                new Type[] { typeof(XmlReader), typeof(Uri), typeof(Uri), typeof(bool), typeof(int)}, 
                reader, 
                parentXmlBaseUri,
                documentBaseUri, 
                disableXmlBase, 
                maxInnerErrorDepth);
            this.assert = assert;
        }

        /// <summary>
        /// Returns the number of attributes on the node.
        /// </summary>
        public override int AttributeCount
        {
            get { return this.instance.AttributeCount; }
        }

        /// <summary>
        /// Returns the base URI of the node - note that this is not based on the xml:base attribute, just the input streams.
        /// </summary>
        public override string BaseURI
        {
            get { return this.instance.BaseURI; }
        }

        /// <summary>
        /// Closes the reader and the underlying input.
        /// </summary>
        public override void Close()
        {
            this.instance.Close();
        }

        /// <summary>
        /// Returns the depth of the current node.
        /// </summary>
        public override int Depth
        {
            get { return this.instance.Depth; }
        }

        /// <summary>
        /// Returns true if the end of input was reached.
        /// </summary>
        public override bool EOF
        {
            get { return this.instance.EOF; }
        }

        /// <summary>
        /// Returns the value of an attribute based on its index.
        /// </summary>
        /// <param name="i">The index of the attribute, starts at 0.</param>
        /// <returns>The value of the attribute at index <paramref name="i"/>.</returns>
        public override string GetAttribute(int i)
        {
            return this.instance.GetAttribute(i);
        }

        /// <summary>
        /// Returns the value of an attribute based on its fully qualified name.
        /// </summary>
        /// <param name="name">The local name of the attribute.</param>
        /// <param name="namespaceURI">The namespace URI of the attribute.</param>
        /// <returns>The value of the attribute with specified <paramref name="name"/> and <paramref name="namespaceURI"/>.</returns>
        public override string GetAttribute(string name, string namespaceURI)
        {
            return this.instance.GetAttribute(name, namespaceURI);
        }

        /// <summary>
        /// Returns the value of an attribute based on its name.
        /// </summary>
        /// <param name="name">The name of the attribute. (prefix:localname)</param>
        /// <returns>The value of the attribute with specified <paramref name="name"/>.</returns>
        public override string GetAttribute(string name)
        {
            return this.instance.GetAttribute(name);
        }

        /// <summary>
        /// Returns true if the reader is positioned on an empty element.
        /// </summary>
        public override bool IsEmptyElement
        {
            get { return this.instance.IsEmptyElement; }
        }

        /// <summary>
        /// Returns the local name of the current node.
        /// </summary>
        public override string LocalName
        {
            get { return this.ValidateAtomizedString(this.instance.LocalName); }
        }

        /// <summary>
        /// Looks up a namespace URI based on the prefix.
        /// </summary>
        /// <param name="prefix">The prefix to search for.</param>
        /// <returns>The namespace URI for the specified <paramref name="prefix"/>.</returns>
        public override string LookupNamespace(string prefix)
        {
            return this.instance.LookupNamespace(prefix);
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
            return this.instance.MoveToAttribute(name, ns);
        }

        /// <summary>
        /// Moves the reader to the attribute specified by name.
        /// </summary>
        /// <param name="name">The name of the attribute (prefix:localname).</param>
        /// <returns>true if the attribute specified by <paramref name="name"/> was found and the reader is positioned on it;
        /// false otherwise.</returns>
        public override bool MoveToAttribute(string name)
        {
            return this.instance.MoveToAttribute(name);
        }

        /// <summary>
        /// Moves the reader to the element which owns the current attribute.
        /// </summary>
        /// <returns>true if the reader has moved (that is the current node was an attribute);
        /// false if the reader didn't move (the reader was already positioned on an element or other node).</returns>
        public override bool MoveToElement()
        {
            return this.instance.MoveToElement();
        }

        /// <summary>
        /// Moves the reader to the first attribute of the current element.
        /// </summary>
        /// <returns>true if the reader moved to the first attribute; false if there are no attribute for the current node (the reader didn't move).</returns>
        public override bool MoveToFirstAttribute()
        {
            return this.instance.MoveToFirstAttribute();
        }

        /// <summary>
        /// Moves the reader to the next attribute on the current element.
        /// </summary>
        /// <returns>true if the reader moved to the next attribute (if the node was an element it moves to the first attribute);
        /// false if the reader didn't move (no attributes for the current node).</returns>
        public override bool MoveToNextAttribute()
        {
            return this.instance.MoveToNextAttribute();
        }

        /// <summary>
        /// Returns the nametable used by the reader.
        /// </summary>
        public override XmlNameTable NameTable
        {
            get { return this.instance.NameTable; }
        }

        /// <summary>
        /// Returns the namespace URI of the current node.
        /// </summary>
        public override string NamespaceURI
        {
            get { return this.ValidateAtomizedString(this.instance.NamespaceURI); }
        }

        /// <summary>
        /// Returns the type of the current node.
        /// </summary>
        public override XmlNodeType NodeType
        {
            get { return this.instance.NodeType; }
        }

        /// <summary>
        /// Returns the prefix of the current node.
        /// </summary>
        public override string Prefix
        {
            get { return this.ValidateAtomizedString(this.instance.Prefix); }
        }

        /// <summary>
        /// Reads the next node from the input.
        /// </summary>
        /// <returns>true if another node is available and the reader has moved to it; false if end of input was reached.</returns>
        public override bool Read()
        {
            return this.instance.Read();
        }

        /// <summary>
        /// Reads the next node from the value of an attribute.
        /// </summary>
        /// <returns>true if next node was available; false if end of the attribute value was reached.</returns>
        public override bool ReadAttributeValue()
        {
            return this.instance.ReadAttributeValue();
        }

        /// <summary>
        /// Returns the current state of the reader.
        /// </summary>
        public override ReadState ReadState
        {
            get { return this.instance.ReadState; }
        }

        /// <summary>
        /// Resolves the current entity node.
        /// </summary>
        public override void ResolveEntity()
        {
            this.instance.ResolveEntity();
        }

        /// <summary>
        /// Returns the value of the current node.
        /// </summary>
        public override string Value
        {
            get { return this.instance.Value; }
        }

        /// <summary>
        /// The active XML base URI for the current node.
        /// </summary>
        public Uri XmlBaseUri
        {
            get { return (Uri)ReflectionUtils.GetProperty(this.instance, "XmlBaseUri"); }
        }

        /// <summary>
        /// Puts the reader into the state where it buffers read nodes.
        /// </summary>
        public void StartBuffering()
        {
            ReflectionUtils.InvokeMethod(this.instance, "StartBuffering");
        }

        /// <summary>
        /// Puts the reader into the state where no buffering happen on read.
        /// Either buffered nodes are consumed or new nodes are read (and not buffered).
        /// </summary>
        internal void StopBuffering()
        {
            ReflectionUtils.InvokeMethod(this.instance, "StopBuffering");
        }

        /// <summary>
        /// Validates that the passed in <paramref name="value"/> has been atomized by the nametable.
        /// </summary>
        /// <param name="value">The string value which should have been atomized.</param>
        /// <returns>The <paramref name="value"/> for composability.</returns>
        private string ValidateAtomizedString(string value)
        {
            if (value == null)
            {
                return null;
            }

            string atomizedValue = this.NameTable.Get(value);
            this.assert.AreSame(atomizedValue, value, "The atomized and returned string instances are not the same.");

            return value;
        }
    }
}
