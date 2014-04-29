//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    using System.Diagnostics;
    using System.Xml;
#if ORCAS
    using System.Xml.XPath;
#endif

    /// <summary>
    /// Xml writer which wraps another writer and fixes prefixes so that the root element is not prefix-qualified (same for everything else with the same prefix).
    /// </summary>
    internal sealed class DefaultNamespaceCompensatingXmlWriter : XmlWriter
    {
        /// <summary>
        /// The wrapped writer.
        /// </summary>
        private readonly XmlWriter writer;

        /// <summary>
        /// The root prefix, once the first element of the document has been written.
        /// </summary>
        private string rootPrefix;

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultNamespaceCompensatingXmlWriter"/>.
        /// </summary>
        /// <param name="writer">The writer to wrap.</param>
        internal DefaultNamespaceCompensatingXmlWriter(XmlWriter writer)
        {
            Debug.Assert(writer != null, "writer != null");
            this.writer = writer;
        }

        /// <summary>
        /// When overridden in a derived class, gets the current xml:lang scope.
        /// </summary>
        /// <returns>
        /// The current xml:lang scope.
        /// </returns>
        public override string XmlLang
        {
            get { return this.writer.XmlLang; }
        }

        /// <summary>
        /// When overridden in a derived class, gets the state of the writer.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Xml.WriteState"/> values.
        /// </returns>
        public override WriteState WriteState
        {
            get { return this.writer.WriteState; }
        }

        /// <summary>
        /// When overridden in a derived class, gets an <see cref="T:System.Xml.XmlSpace"/> representing the current xml:space scope.
        /// </summary>
        /// <returns>
        /// An XmlSpace representing the current xml:space scope.
        /// </returns>
        public override XmlSpace XmlSpace
        {
            get { return this.writer.XmlSpace; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Xml.XmlWriterSettings"/> object used to create this <see cref="T:System.Xml.XmlWriter"/> instance.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Xml.XmlWriterSettings"/> object used to create this writer instance.
        /// </returns>
        public override XmlWriterSettings Settings
        {
            get { return this.writer.Settings; }
        }

#if ORCAS
        /// <summary>
        /// Copies everything from the <see cref="T:System.Xml.XPath.XPathNavigator"/> object to the writer. The position of the <see cref="T:System.Xml.XPath.XPathNavigator"/> remains unchanged.
        /// </summary>
        /// <param name="navigator">The <see cref="T:System.Xml.XPath.XPathNavigator"/> to copy from.</param>
        /// <param name="defattr">true to copy the default attributes; otherwise, false.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="navigator"/> is null.</exception>
        public override void WriteNode(XPathNavigator navigator, bool defattr)
        {
            this.writer.WriteNode(navigator, defattr);
        }
#endif

        /// <summary>
        /// When overridden in a derived class, copies everything from the reader to the writer and moves the reader to the start of the next sibling.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> to read from. </param>
        /// <param name="defattr">true to copy the default attributes from the XmlReader; otherwise, false. </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="reader"/> is null. </exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="reader"/> contains invalid characters. </exception>
        public override void WriteNode(XmlReader reader, bool defattr)
        {
            this.writer.WriteNode(reader, defattr);
        }

        /// <summary>
        /// When overridden in a derived class, writes out all the attributes found at the current position in the <see cref="T:System.Xml.XmlReader"/>.
        /// </summary>
        /// <param name="reader">The XmlReader from which to copy the attributes. </param>
        /// <param name="defattr">true to copy the default attributes from the XmlReader; otherwise, false. </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="reader"/> is null. </exception>
        /// <exception cref="T:System.Xml.XmlException">The reader is not positioned on an element, attribute or XmlDeclaration node. </exception>
        public override void WriteAttributes(XmlReader reader, bool defattr)
        {
            this.writer.WriteAttributes(reader, defattr);
        }

        /// <summary>
        /// When overridden in a derived class, returns the closest prefix defined in the current namespace scope for the namespace URI.
        /// </summary>
        /// <returns>
        /// The matching prefix or null if no matching namespace URI is found in the current scope.
        /// </returns>
        /// <param name="ns">The namespace URI whose prefix you want to find. </param>
        /// <exception cref="T:System.ArgumentException"><paramref name="ns"/> is either null or String.Empty. </exception>
        public override string LookupPrefix(string ns)
        {
            return this.writer.LookupPrefix(ns);
        }

        /// <summary>
        /// When overridden in a derived class, flushes whatever is in the buffer to the underlying streams and also flushes the underlying stream.
        /// </summary>
        public override void Flush()
        {
            this.writer.Flush();
        }

        /// <summary>
        /// When overridden in a derived class, writes out the specified name, ensuring it is a valid NmToken according to the W3C XML 1.0 recommendation (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).
        /// </summary>
        /// <param name="name">The name to write. </param><exception cref="T:System.ArgumentException">
        /// <paramref name="name"/> is not a valid NmToken; or <paramref name="name"/> is either null or String.Empty. </exception>
        public override void WriteNmToken(string name)
        {
            this.writer.WriteNmToken(name);
        }

#if !PORTABLELIB
        /// <summary>
        /// When overridden in a derived class, closes this stream and the underlying stream.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">A call is made to write more output after Close has been called or the result of this call is an invalid XML document. </exception>
        public override void Close()
        {
            this.writer.Close();
        }
#endif

        /// <summary>
        /// When overridden in a derived class, encodes the specified binary bytes as BinHex and writes out the resulting text.
        /// </summary>
        /// <param name="buffer">Byte array to encode. </param><param name="index">The position in the buffer indicating the start of the bytes to write. </param><param name="count">The number of bytes to write. </param><exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is null. </exception><exception cref="T:System.InvalidOperationException">The writer is closed or in error state.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> or <paramref name="count"/> is less than zero. -or-The buffer length minus <paramref name="index"/> is less than <paramref name="count"/>.</exception>
        public override void WriteBinHex(byte[] buffer, int index, int count)
        {
            this.writer.WriteBinHex(buffer, index, count);
        }

        /// <summary>
        /// When overridden in a derived class, writes raw markup manually from a string.
        /// </summary>
        /// <param name="data">String containing the text to write. </param>
        /// <exception cref="T:System.ArgumentException"><paramref name="data"/> is either null or String.Empty. </exception>
        public override void WriteRaw(string data)
        {
            this.writer.WriteRaw(data);
        }

        /// <summary>
        /// When overridden in a derived class, encodes the specified binary bytes as Base64 and writes out the resulting text.
        /// </summary>
        /// <param name="buffer">Byte array to encode. </param><param name="index">The position in the buffer indicating the start of the bytes to write. </param>
        /// <param name="count">The number of bytes to write. </param><exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> or <paramref name="count"/> is less than zero. -or-The buffer length minus <paramref name="index"/> is less than <paramref name="count"/>.</exception>
        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            this.writer.WriteBase64(buffer, index, count);
        }

        /// <summary>
        /// When overridden in a derived class, writes raw markup manually from a character buffer.
        /// </summary>
        /// <param name="buffer">Character array containing the text to write. </param><param name="index">The position within the buffer indicating the start of the text to write. </param>
        /// <param name="count">The number of characters to write. </param><exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> or <paramref name="count"/> is less than zero. -or-The buffer length minus <paramref name="index"/> is less than <paramref name="count"/>.</exception>
        public override void WriteRaw(char[] buffer, int index, int count)
        {
            this.writer.WriteRaw(buffer, index, count);
        }

        /// <summary>
        /// When overridden in a derived class, writes text one buffer at a time.
        /// </summary>
        /// <param name="buffer">Character array containing the text to write. </param><param name="index">The position in the buffer indicating the start of the text to write. </param>
        /// <param name="count">The number of characters to write. </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> or <paramref name="count"/> is less than zero. -or-The buffer length minus <paramref name="index"/> is less than <paramref name="count"/>; the call results in surrogate pair characters being split or an invalid surrogate pair being written.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="buffer"/> parameter value is not valid.</exception>
        public override void WriteChars(char[] buffer, int index, int count)
        {
            this.writer.WriteChars(buffer, index, count);
        }

        /// <summary>
        /// When overridden in a derived class, generates and writes the surrogate character entity for the surrogate character pair.
        /// </summary>
        /// <param name="lowChar">The low surrogate. This must be a value between 0xDC00 and 0xDFFF. </param>
        /// <param name="highChar">The high surrogate. This must be a value between 0xD800 and 0xDBFF. </param>
        /// <exception cref="T:System.ArgumentException">An invalid surrogate character pair was passed. </exception>
        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            this.writer.WriteSurrogateCharEntity(lowChar, highChar);
        }

        /// <summary>
        /// When overridden in a derived class, writes the given text content.
        /// </summary>
        /// <param name="text">The text to write. </param><exception cref="T:System.ArgumentException">The text string contains an invalid surrogate pair. </exception>
        public override void WriteString(string text)
        {
            this.writer.WriteString(text);
        }
        
        /// <summary>
        /// When overridden in a derived class, writes the start of an attribute with the specified prefix, local name, and namespace URI.
        /// </summary>
        /// <param name="prefix">The namespace prefix of the attribute. </param><param name="localName">The local name of the attribute. </param>
        /// <param name="ns">The namespace URI for the attribute. </param>
        /// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections. </exception>
        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            Debug.Assert(ns != AtomConstants.XmlNamespacesNamespace || localName != this.rootPrefix, "Namespace prefix used for root should not be explicitly defined. Prefix was: " + localName);
            this.writer.WriteStartAttribute(prefix, localName, ns);
        }

        /// <summary>
        /// When overridden in a derived class, closes the previous <see cref="M:System.Xml.XmlWriter.WriteStartAttribute(System.String,System.String)"/> call.
        /// </summary>
        public override void WriteEndAttribute()
        {
            this.writer.WriteEndAttribute();
        }

        /// <summary>
        /// When overridden in a derived class, writes out a &lt;![CDATA[...]]&gt; block containing the specified text.
        /// </summary>
        /// <param name="text">The text to place inside the CDATA block. </param>
        /// <exception cref="T:System.ArgumentException">The text would result in a non-well formed XML document. </exception>
        public override void WriteCData(string text)
        {
            this.writer.WriteCData(text);
        }

        /// <summary>
        /// When overridden in a derived class, writes out a comment &lt;!--...--&gt; containing the specified text.
        /// </summary>
        /// <param name="text">Text to place inside the comment. </param>
        /// <exception cref="T:System.ArgumentException">The text would result in a non-well formed XML document. </exception>
        public override void WriteComment(string text)
        {
            this.writer.WriteComment(text);
        }

        /// <summary>
        /// When overridden in a derived class, writes out a processing instruction with a space between the name and text as follows: &lt;?name text?&gt;.
        /// </summary>
        /// <param name="name">The name of the processing instruction. </param>
        /// <param name="text">The text to include in the processing instruction. </param>
        /// <exception cref="T:System.ArgumentException">The text would result in a non-well formed XML document.<paramref name="name"/> is either null or String.Empty.This method is being used to create an XML declaration after <see cref="M:System.Xml.XmlWriter.WriteStartDocument"/> has already been called. </exception>
        public override void WriteProcessingInstruction(string name, string text)
        {
            this.writer.WriteProcessingInstruction(name, text);
        }

        /// <summary>
        /// When overridden in a derived class, writes out an entity reference as &amp;name;.
        /// </summary>
        /// <param name="name">The name of the entity reference. </param>
        /// <exception cref="T:System.ArgumentException"><paramref name="name"/> is either null or String.Empty. </exception>
        public override void WriteEntityRef(string name)
        {
            this.writer.WriteEntityRef(name);
        }

        /// <summary>
        /// When overridden in a derived class, forces the generation of a character entity for the specified Unicode character value.
        /// </summary>
        /// <param name="ch">The Unicode character for which to generate a character entity. </param>
        /// <exception cref="T:System.ArgumentException">The character is in the surrogate pair character range, 0xd800 - 0xdfff. </exception>
        public override void WriteCharEntity(char ch)
        {
            this.writer.WriteCharEntity(ch);
        }

        /// <summary>
        /// When overridden in a derived class, writes out the given white space.
        /// </summary>
        /// <param name="ws">The string of white space characters. </param>
        /// <exception cref="T:System.ArgumentException">The string contains non-white space characters. </exception>
        public override void WriteWhitespace(string ws)
        {
            this.writer.WriteWhitespace(ws);
        }

        /// <summary>
        /// When overridden in a derived class, writes the XML declaration with the version "1.0".
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">This is not the first write method called after the constructor. </exception>
        public override void WriteStartDocument()
        {
            this.writer.WriteStartDocument();
        }

        /// <summary>
        /// When overridden in a derived class, writes the XML declaration with the version "1.0" and the standalone attribute.
        /// </summary>
        /// <param name="standalone">If true, it writes "standalone=yes"; if false, it writes "standalone=no". </param>
        /// <exception cref="T:System.InvalidOperationException">This is not the first write method called after the constructor. </exception>
        public override void WriteStartDocument(bool standalone)
        {
            this.writer.WriteStartDocument(standalone);
        }

        /// <summary>
        /// When overridden in a derived class, closes any open elements or attributes and puts the writer back in the Start state.
        /// </summary>
        /// <exception cref="T:System.ArgumentException">The XML document is invalid. </exception>
        public override void WriteEndDocument()
        {
            this.writer.WriteEndDocument();
        }

        /// <summary>
        /// When overridden in a derived class, writes the DOCTYPE declaration with the specified name and optional attributes.
        /// </summary>
        /// <param name="name">The name of the DOCTYPE. This must be non-empty. </param>
        /// <param name="pubid">If non-null it also writes public override "pubid" "sysid" where <paramref name="pubid"/> and <paramref name="sysid"/> are replaced with the value of the given arguments. </param>
        /// <param name="sysid">If <paramref name="pubid"/> is null and <paramref name="sysid"/> is non-null it writes SYSTEM "sysid" where <paramref name="sysid"/> is replaced with the value of this argument. </param>
        /// <param name="subset">If non-null it writes [subset] where subset is replaced with the value of this argument. </param>
        /// <exception cref="T:System.InvalidOperationException">This method was called outside the prolog (after the root element). </exception>
        /// <exception cref="T:System.ArgumentException">The value for <paramref name="name"/> would result in invalid XML. </exception>
        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            this.writer.WriteDocType(name, pubid, sysid, subset);
        }

        /// <summary>
        /// When overridden in a derived class, writes the specified start tag and associates it with the given namespace and prefix.
        /// </summary>
        /// <param name="prefix">The namespace prefix of the element. </param><param name="localName">The local name of the element. </param>
        /// <param name="ns">The namespace URI to associate with the element. </param>
        /// <exception cref="T:System.InvalidOperationException">The writer is closed. </exception>
        /// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections. </exception>
        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            if (this.rootPrefix == null)
            {
                this.rootPrefix = prefix;
                prefix = string.Empty;
            }
            else if (this.rootPrefix == prefix)
            {
                prefix = string.Empty;
            }

            this.writer.WriteStartElement(prefix, localName, ns);
        }

        /// <summary>
        /// When overridden in a derived class, closes one element and pops the corresponding namespace scope.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">This results in an invalid XML document. </exception>
        public override void WriteEndElement()
        {
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// When overridden in a derived class, closes one element and pops the corresponding namespace scope.
        /// </summary>
        public override void WriteFullEndElement()
        {
            this.writer.WriteFullEndElement();
        }
    }
}
