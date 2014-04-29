//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;

    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the OData writer for the ATOM format.
    /// </summary>
    internal static class ODataAtomWriterUtils
    {
        /// <summary>
        /// Creates an Xml writer over the specified stream, with the provided settings and encoding.
        /// </summary>
        /// <param name="stream">The stream to create the XmlWriter over.</param>
        /// <param name="messageWriterSettings">The OData message writer settings used to control the settings of the Xml writer.</param>
        /// <param name="encoding">The encoding used for writing.</param>
        /// <returns>An <see cref="XmlWriter"/> instance configured with the provided settings and encoding.</returns>
        internal static XmlWriter CreateXmlWriter(Stream stream, ODataMessageWriterSettings messageWriterSettings, Encoding encoding)
        {
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(messageWriterSettings != null, "messageWriterSettings != null");

            XmlWriterSettings xmlWriterSettings = CreateXmlWriterSettings(messageWriterSettings, encoding);

            XmlWriter writer = XmlWriter.Create(stream, xmlWriterSettings);
            if (messageWriterSettings.AlwaysUseDefaultXmlNamespaceForRootElement)
            {
                writer = new DefaultNamespaceCompensatingXmlWriter(writer);
            }

            return writer;
        }

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="error">The error instance to write.</param>
        /// <param name="includeDebugInformation">A flag indicating whether error details should be written (in debug mode only) or not.</param>
        /// <param name="maxInnerErrorDepth">The maximumum number of nested inner errors to allow.</param>
        internal static void WriteError(XmlWriter writer, ODataError error, bool includeDebugInformation, int maxInnerErrorDepth)
        {
            ErrorUtils.WriteXmlError(writer, error, includeDebugInformation, maxInnerErrorDepth);
        }

        /// <summary>
        /// Write the m:etag attribute with the given string value.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="etag">The string value of the ETag.</param>
        internal static void WriteETag(XmlWriter writer, string etag)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(etag != null, "etag != null");

            writer.WriteAttributeString(
                AtomConstants.ODataMetadataNamespacePrefix,
                AtomConstants.ODataETagAttributeName,
                AtomConstants.ODataMetadataNamespace,
                etag);
        }

        /// <summary>
        /// Write the m:null attribute with a value of 'true'
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        internal static void WriteNullAttribute(XmlWriter writer)
        {
            Debug.Assert(writer != null, "writer != null");

            // m:null="true"
            writer.WriteAttributeString(
                AtomConstants.ODataMetadataNamespacePrefix,
                AtomConstants.ODataNullAttributeName,
                AtomConstants.ODataMetadataNamespace,
                AtomConstants.AtomTrueLiteral);
        }
       
        /// <summary>
        /// Writes raw markup with the given writer, adding the xml:space="preserve" attribute to the element if the markup has leading or trailing whitespace.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="value">A string containing the text to write.</param>
        internal static void WriteRaw(XmlWriter writer, string value)
        {
            Debug.Assert(writer != null, "writer != null");

            WritePreserveSpaceAttributeIfNeeded(writer, value);
            writer.WriteRaw(value);
        }

        /// <summary>
        /// Writes a string with the given writer, adding the xml:space="preserve" attribute to the element if the string has leading or trailing whitespace.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="value">The string to write as element text content.</param>
        internal static void WriteString(XmlWriter writer, string value)
        {
            Debug.Assert(writer != null, "writer != null");

            WritePreserveSpaceAttributeIfNeeded(writer, value);
            writer.WriteString(value);
        }

        /// <summary>
        /// For Atom writer, only prefix the type name with # for payload writting if it is not primitive type.
        /// </summary>
        /// <param name="typeName">The type name to prefix</param>
        /// <returns>The (#) prefixed type name, or the input type name if it is primitive type.</returns>
        internal static string PrefixTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName) || IsPrimitiveType(typeName))
            {
                return typeName;
            }

            Debug.Assert(!typeName.StartsWith(ODataConstants.TypeNamePrefix, StringComparison.Ordinal), "The type name not start with " + ODataConstants.TypeNamePrefix + "before prefix");

            return ODataConstants.TypeNamePrefix + typeName;
        }

        /// <summary>
        /// Creates a new XmlWriterSettings instance using the encoding.
        /// </summary>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <param name="encoding">Encoding to  use in the writer settings.</param>
        /// <returns>The Xml writer settings to use for this writer.</returns>
        private static XmlWriterSettings CreateXmlWriterSettings(ODataMessageWriterSettings messageWriterSettings, Encoding encoding)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CheckCharacters = messageWriterSettings.CheckCharacters;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.OmitXmlDeclaration = false;
            settings.Encoding = encoding ?? MediaTypeUtils.EncodingUtf8NoPreamble;
            settings.NewLineHandling = NewLineHandling.Entitize;

            settings.Indent = messageWriterSettings.Indent;

            // we do not want to close the underlying stream when the OData writer is closed since we don't own the stream.
            settings.CloseOutput = false;

            return settings;
        }

        /// <summary>
        /// Writes an xml:space="preserve" attribute if the given value starts or ends with whitespace.
        /// </summary>
        /// <param name="writer">The writer to use for writing out the attribute string.</param>
        /// <param name="value">The value to check for insignificant whitespace.</param>
        private static void WritePreserveSpaceAttributeIfNeeded(XmlWriter writer, string value)
        {
            Debug.Assert(writer != null, "writer != null");

            if (value == null)
            {
                return;
            }

            int length = value.Length;

            if (length > 0 && (Char.IsWhiteSpace(value[0]) || Char.IsWhiteSpace(value[length - 1])))
            {
                // xml:space="preserve"
                writer.WriteAttributeString(
                    AtomConstants.XmlNamespacePrefix,
                    AtomConstants.XmlSpaceAttributeName,
                    AtomConstants.XmlNamespace,
                    AtomConstants.XmlPreserveSpaceAttributeValue);
            }
        }

        /// <summary>
        /// Determines if the type is primitive type.
        /// </summary>
        /// <param name="typeName">The type name to check</param>
        /// <returns>true if the type is primitive type, else return false</returns>
        private static bool IsPrimitiveType(string typeName)
        {
            return EdmCoreModel.Instance.GetPrimitiveTypeKind(typeName) != EdmPrimitiveTypeKind.None;
        }
    }
}
