//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Xml;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Base class for all OData ATOM serializers.
    /// </summary>
    internal class ODataAtomSerializer : ODataSerializer
    {
        /// <summary>
        /// The ATOM output context to write to.
        /// </summary>
        private ODataAtomOutputContext atomOutputContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomOutputContext">The output context to write to.</param>
        internal ODataAtomSerializer(ODataAtomOutputContext atomOutputContext)
            : base(atomOutputContext)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(atomOutputContext != null, "atomOutputContext != null");

            this.atomOutputContext = atomOutputContext;
        }

        /// <summary>
        /// Flags to describe a set of default namespaces.
        /// </summary>
        [Flags]
        internal enum DefaultNamespaceFlags
        {
            /// <summary>No namespaces.</summary>
            None = 0x00,

            /// <summary>OData namespace.</summary>
            OData = 0x01,

            /// <summary>OData metadata namespace.</summary>
            ODataMetadata = 0x02,

            /// <summary>ATOM namespace</summary>
            Atom = 0x04,

            /// <summary>GeoRss namespace.</summary>
            GeoRss = 0x08,

            /// <summary>GML namespace.</summary>
            Gml = 0x10,

            /// <summary>All default namespaces.</summary>
            All = OData | ODataMetadata | Atom | GeoRss | Gml
        }

        /// <summary>
        /// Returns the <see cref="XmlWriter"/> which is to be used to write the content of the message.
        /// </summary>
        internal XmlWriter XmlWriter
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.atomOutputContext.XmlWriter;
            }
        }

        /// <summary>
        /// The ODataAtomOutputContext used by the serializer.
        /// </summary>
        protected ODataAtomOutputContext AtomOutputContext
        {
            get
            {
                return this.atomOutputContext;
            }
        }

        /// <summary>
        /// Converts the given <paramref name="uri"/> Uri to a string. 
        /// If the provided baseUri is not null and is a base Uri of the <paramref name="uri"/> Uri 
        /// the method returns the string form of the relative Uri.
        /// </summary>
        /// <param name="uri">The Uri to convert.</param>
        /// <returns>The string form of the <paramref name="uri"/> Uri. If the Uri is absolute it returns the
        /// string form of the <paramref name="uri"/>. If the <paramref name="uri"/> Uri is not absolute 
        /// it returns the original string of the Uri.</returns>
        internal string UriToUrlAttributeValue(Uri uri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uri != null, "uri != null");
            return this.UriToUrlAttributeValue(uri, /*failOnRelativeUriWithoutBaseUri*/ true);
        }

        /// <summary>
        /// Converts the given <paramref name="uri"/> Uri to a string. 
        /// If the provided baseUri is not null and is a base Uri of the <paramref name="uri"/> Uri 
        /// the method returns the string form of the relative Uri.
        /// </summary>
        /// <param name="uri">The Uri to convert.</param>
        /// <param name="failOnRelativeUriWithoutBaseUri">If set to true then this method will fail if the uri specified by <paramref name="uri"/> is relative 
        /// and no base uri is specified.</param>
        /// <returns>The string form of the <paramref name="uri"/> Uri. If the Uri is absolute it returns the
        /// string form of the <paramref name="uri"/>. If the <paramref name="uri"/> Uri is not absolute 
        /// it returns the original string of the Uri.</returns>
        internal string UriToUrlAttributeValue(Uri uri, bool failOnRelativeUriWithoutBaseUri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uri != null, "uri != null");

            if (this.UrlResolver != null)
            {
                // The resolver returns 'null' if no custom resolution is desired.
                Uri resultUri = this.UrlResolver.ResolveUrl(this.MessageWriterSettings.BaseUri, uri);
                if (resultUri != null)
                {
                    return UriUtilsCommon.UriToString(resultUri);
                }
            }

            if (!uri.IsAbsoluteUri)
            {
                // NOTE: the only URIs that are allowed to be relative (e.g., failOnRelativeUriWithoutBaseUri is false)
                //       are metadata URIs in operations; for such metadata URIs there is no base URI.
                if (this.MessageWriterSettings.BaseUri == null && failOnRelativeUriWithoutBaseUri)
                {
                    throw new ODataException(
                        ODataErrorStrings.ODataWriter_RelativeUriUsedWithoutBaseUriSpecified(UriUtilsCommon.UriToString(uri)));
                }

                uri = UriUtils.EnsureEscapedRelativeUri(uri);
            }

            return UriUtilsCommon.UriToString(uri);
        }

        /// <summary>
        /// Start writing an ATOM payload.
        /// </summary>
        internal void WritePayloadStart()
        {
            DebugUtils.CheckNoExternalCallers();

            this.XmlWriter.WriteStartDocument();
        }

        /// <summary>
        /// Finish writing an ATOM payload.
        /// </summary>
        /// <remarks>This method MUST NOT be called after writing an in-stream error 
        /// as it would fail on unclosed elements (or try to close them).</remarks>
        internal void WritePayloadEnd()
        {
            DebugUtils.CheckNoExternalCallers();

            this.XmlWriter.WriteEndDocument();
        }

        /// <summary>
        /// Writes a top-level error payload.
        /// </summary>
        /// <param name="error">The error instance to write.</param>
        /// <param name="includeDebugInformation">A flag indicating whether error details should be written (in debug mode only) or not.</param>
        internal void WriteTopLevelError(ODataError error, bool includeDebugInformation)
        {
            Debug.Assert(this.MessageWriterSettings != null, "this.MessageWriterSettings != null");
            DebugUtils.CheckNoExternalCallers();

            this.WritePayloadStart();
            ODataAtomWriterUtils.WriteError(this.XmlWriter, error, includeDebugInformation, this.MessageWriterSettings.MessageQuotas.MaxNestingDepth);
            this.WritePayloadEnd();
        }

        /// <summary>
        /// Write the namespaces for OData (prefix 'd') and OData metadata (prefix 'm')
        /// </summary>
        /// <param name="flags">An enumeration value to indicate what default namespace attributes to write.</param>
        internal void WriteDefaultNamespaceAttributes(DefaultNamespaceFlags flags)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.MessageWriterSettings.Version.HasValue, "Version must be set by now.");

            if ((flags & DefaultNamespaceFlags.Atom) == DefaultNamespaceFlags.Atom)
            {
                this.XmlWriter.WriteAttributeString(
                    AtomConstants.XmlnsNamespacePrefix,
                    AtomConstants.XmlNamespacesNamespace,
                    AtomConstants.AtomNamespace);
            }

            if ((flags & DefaultNamespaceFlags.OData) == DefaultNamespaceFlags.OData)
            {
                this.XmlWriter.WriteAttributeString(
                    AtomConstants.ODataNamespacePrefix,
                    AtomConstants.XmlNamespacesNamespace,
                    this.MessageWriterSettings.WriterBehavior.ODataNamespace);
            }

            if ((flags & DefaultNamespaceFlags.ODataMetadata) == DefaultNamespaceFlags.ODataMetadata)
            {
                this.XmlWriter.WriteAttributeString(
                    AtomConstants.ODataMetadataNamespacePrefix,
                    AtomConstants.XmlNamespacesNamespace,
                    AtomConstants.ODataMetadataNamespace);
            }

            // Only write the spatial namespaces for versions >= V3
            if (this.MessageWriterSettings.Version.Value >= ODataVersion.V3)
            {
                if ((flags & DefaultNamespaceFlags.GeoRss) == DefaultNamespaceFlags.GeoRss)
                {
                    this.XmlWriter.WriteAttributeString(
                        AtomConstants.GeoRssPrefix,
                        AtomConstants.XmlNamespacesNamespace,
                        AtomConstants.GeoRssNamespace);
                }

                if ((flags & DefaultNamespaceFlags.Gml) == DefaultNamespaceFlags.Gml)
                {
                    this.XmlWriter.WriteAttributeString(
                        AtomConstants.GmlPrefix,
                        AtomConstants.XmlNamespacesNamespace,
                        AtomConstants.GmlNamespace);
                }
            }
        }

        /// <summary>
        /// Writes the count.
        /// </summary>
        /// <param name="count">Count value.</param>
        /// <param name="includeNamespaceDeclaration">True if the namespace declaration for the metadata namespace should be included; otherwise false.</param>
        internal void WriteCount(long count, bool includeNamespaceDeclaration)
        {
            DebugUtils.CheckNoExternalCallers();

            this.XmlWriter.WriteStartElement(
                AtomConstants.ODataMetadataNamespacePrefix,
                AtomConstants.ODataCountElementName,
                AtomConstants.ODataMetadataNamespace);

            if (includeNamespaceDeclaration)
            {
                this.WriteDefaultNamespaceAttributes(DefaultNamespaceFlags.ODataMetadata);
            }

            this.XmlWriter.WriteValue(count);
            this.XmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Write the base Uri of the document (if specified) and the namespaces for OData (prefix 'd') and OData metadata (prefix 'm')
        /// </summary>
        internal void WriteBaseUriAndDefaultNamespaceAttributes()
        {
            DebugUtils.CheckNoExternalCallers();

            Uri baseUri = this.MessageWriterSettings.BaseUri;
            if (baseUri != null)
            {
                this.XmlWriter.WriteAttributeString(
                    AtomConstants.XmlBaseAttributeName,
                    AtomConstants.XmlNamespace,
                    baseUri.AbsoluteUri);
            }

            this.WriteDefaultNamespaceAttributes(DefaultNamespaceFlags.All);
        }

        /// <summary>
        /// Writes an Xml element with the specified primitive value as content.
        /// </summary>
        /// <param name="prefix">The prefix for the element's namespace.</param>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="ns">The namespace of the element.</param>
        /// <param name="textContent">The value to be used as element content.</param>
        internal void WriteElementWithTextContent(string prefix, string localName, string ns, string textContent)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(prefix != null, "prefix != null");
            Debug.Assert(!string.IsNullOrEmpty(localName), "!string.IsNullOrEmpty(localName)");
            Debug.Assert(!string.IsNullOrEmpty(ns), "!string.IsNullOrEmpty(ns)");

            this.XmlWriter.WriteStartElement(prefix, localName, ns);

            if (textContent != null)
            {
                ODataAtomWriterUtils.WriteString(this.XmlWriter, textContent);
            }

            this.XmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes an Xml element with empty content.
        /// </summary>
        /// <param name="prefix">The prefix for the element's namespace.</param>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="ns">The namespace of the element.</param>
        internal void WriteEmptyElement(string prefix, string localName, string ns)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(prefix != null, "prefix != null");
            Debug.Assert(!string.IsNullOrEmpty(localName), "!string.IsNullOrEmpty(localName)");
            Debug.Assert(!string.IsNullOrEmpty(ns), "!string.IsNullOrEmpty(ns)");

            this.XmlWriter.WriteStartElement(prefix, localName, ns);
            this.XmlWriter.WriteEndElement();
        }
    }
}
