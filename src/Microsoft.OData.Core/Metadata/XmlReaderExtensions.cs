//---------------------------------------------------------------------
// <copyright file="XmlReaderExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Metadata
{
    #region Namespaces
    using System.Diagnostics;
    using System.Text;
    using System.Xml;
    #endregion Namespaces

    /// <summary>
    /// Extension methods for the XML reader.
    /// </summary>
    internal static class XmlReaderExtensions
    {
        /// <summary>
        /// Reads the value of the element as a string.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <returns>The string value of the element.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element   - the element to read the value for.
        ///                  XmlNodeType.Attribute - an attribute on the element to read the value for.
        /// Post-Condition:  Any                   - the node after the element.
        ///
        /// This method is similar to ReadElementContentAsString with one difference:
        /// - It ignores Whitespace nodes - this is needed for compatiblity, WCF DS ignores insignificant whitespaces
        ///     it does that by setting the IgnoreWhitespace option on reader settings, ODataLib can't do that
        ///     cause it doesn't always control the creation of the XmlReader, so it has to explicitely ignore
        ///     insignificant whitespaces.
        /// </remarks>
        internal static string ReadElementValue(this XmlReader reader)
        {
            string result = reader.ReadElementContentValue();
            reader.Read();
            return result;
        }

        /// <summary>
        /// Reads the value of the element as a string.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <returns>The string value of the element.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element   - the element to read the value for.
        ///                  XmlNodeType.Attribute - an attribute on the element to read the value for.
        /// Post-Condition:  XmlNodeType.Element    - the element was empty.
        ///                  XmlNodeType.EndElement - the element had some value.
        ///
        /// This method is similar to ReadElementContentAsString with two differences:
        /// - It ignores Whitespace nodes - this is needed for compatiblity, WCF DS ignores insignificant whitespaces
        ///     it does that by setting the IgnoreWhitespace option on reader settings, ODataLib can't do that
        ///     cause it doesn't always control the creation of the XmlReader, so it has to explicitely ignore
        ///     insignificant whitespaces.
        /// - It leaves the reader positioned on the EndElement node (or the start node if it was empty).
        /// </remarks>
        internal static string ReadElementContentValue(this XmlReader reader)
        {
            Debug.Assert(reader != null, "reader != null");
            Debug.Assert(
                reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Attribute,
                "Pre-Condition: XML reader must be on Element or Attribute node.");

            reader.MoveToElement();

            string result = null;
            if (reader.IsEmptyElement)
            {
                result = string.Empty;
            }
            else
            {
                StringBuilder builder = null;
                bool endElementFound = false;
                while (!endElementFound && reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.EndElement:
                            endElementFound = true;
                            break;
                        case XmlNodeType.CDATA:
                        case XmlNodeType.Text:
                        case XmlNodeType.SignificantWhitespace:
                            if (result == null)
                            {
                                result = reader.Value;
                            }
                            else if (builder == null)
                            {
                                builder = new StringBuilder();
                                builder.Append(result);
                                builder.Append(reader.Value);
                            }
                            else
                            {
                                builder.Append(reader.Value);
                            }

                            break;

                        // Ignore comments, whitespaces and processing instructions.
                        case XmlNodeType.Comment:
                        case XmlNodeType.ProcessingInstruction:
                        case XmlNodeType.Whitespace:
                            break;

                        default:
                            throw new ODataException(Strings.XmlReaderExtension_InvalidNodeInStringValue(reader.NodeType));
                    }
                }

                if (builder != null)
                {
                    result = builder.ToString();
                }
                else if (result == null)
                {
                    result = string.Empty;
                }
            }

            Debug.Assert(
                reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.EndElement,
                "Post-Condition: XML reader must be on Element or EndElement node.");
            Debug.Assert(result != null, "The method should never return null since it doesn't handle null values.");
            return result;
        }

        /// <summary>
        /// Determines if the current node's namespace equals to the specified <paramref name="namespaceUri"/>
        /// </summary>
        /// <param name="reader">The XML reader to get the current node from.</param>
        /// <param name="namespaceUri">The namespace URI to compare, this must be a string already atomized in the <paramref name="reader"/> name table.</param>
        /// <returns>true if the current node is in the specified namespace; false otherwise.</returns>
        internal static bool NamespaceEquals(this XmlReader reader, string namespaceUri)
        {
            Debug.Assert(reader != null, "reader != null");
            Debug.Assert(
                reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.EndElement || reader.NodeType == XmlNodeType.Attribute,
                "The namespace of the node should only be tested on Element or Attribute nodes.");
            Debug.Assert(object.ReferenceEquals(reader.NameTable.Get(namespaceUri), namespaceUri), "The namespaceUri was not atomized on this reader.");

            return object.ReferenceEquals(reader.NamespaceURI, namespaceUri);
        }

        /// <summary>
        /// Determines if the current node's local name equals to the specified <paramref name="localName"/>
        /// </summary>
        /// <param name="reader">The XML reader to get the current node from.</param>
        /// <param name="localName">The local name to compare, this must be a string already atomized in the <paramref name="reader"/> name table.</param>
        /// <returns>true if the current node has the specified local name; false otherwise.</returns>
        internal static bool LocalNameEquals(this XmlReader reader, string localName)
        {
            Debug.Assert(reader != null, "reader != null");
            Debug.Assert(
                reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.EndElement || reader.NodeType == XmlNodeType.Attribute,
                "The namespace of the node should only be tested on Element or Attribute nodes.");
            Debug.Assert(object.ReferenceEquals(reader.NameTable.Get(localName), localName), "The localName was not atomized on this reader.");

            return object.ReferenceEquals(reader.LocalName, localName);
        }

        /// <summary>
        /// Reads to the next element encountered in an Xml payload.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> to read from.</param>
        /// <returns>true if the method reached the next element; otherwise false.</returns>
        internal static bool TryReadToNextElement(this XmlReader reader)
        {
            Debug.Assert(reader != null, "reader != null");

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
