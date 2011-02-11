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

namespace System.Data.OData.Atom
{
    #region Namespaces.
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    #endregion Namespaces.

    /// <summary>
    /// Helper methods used by the OData writer for the ATOM format.
    /// </summary>
    internal static class ODataAtomWriterUtils
    {
        /// <summary>
        /// Flags to describe a set of default namespaces.
        /// </summary>
        [Flags]
        private enum DefaultNamespaceFlags
        {
            /// <summary>No namespaces.</summary>
            None = 0x00,

            /// <summary>OData namespace.</summary>
            OData = 0x01,

            /// <summary>OData metadata namespace.</summary>
            ODataMetadata = 0x02,

            /// <summary>ATOM namespace</summary>
            Atom = 0x04,

            /// <summary>All default namespaces.</summary>
            All = OData | ODataMetadata | Atom
        }

        /// <summary>
        /// Creates a new XmlWriterSettings instance using the encoding.
        /// </summary>
        /// <param name="odataSettings">Configuration settings of the OData writer.</param>
        /// <param name="encoding"> Encoding to  use in the writer settings.</param>
        /// <returns>The Xml writer settings to use for this writer.</returns>
        internal static XmlWriterSettings CreateXmlWriterSettings(ODataWriterSettings odataSettings, Encoding encoding)
        {
            DebugUtils.CheckNoExternalCallers();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CheckCharacters = odataSettings.CheckCharacters;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.OmitXmlDeclaration = false;
            if (encoding != null)
            {
                settings.Encoding = encoding;
            }

            settings.NewLineHandling = NewLineHandling.Entitize;

            settings.Indent = odataSettings.Indent;
            
            // we do not want to close the underlying stream when the OData writer is closed since we don't own the stream.
            settings.CloseOutput = false;

            return settings;
        }

        /// <summary>
        /// Write the base Uri of the document (if specified) and the namespaces for OData (prefix 'd') and OData metadata (prefix 'm')
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="baseUri">The Xml base Uri for the document.</param>
        internal static void WriteBaseUriAndDefaultNamespaceAttributes(XmlWriter writer, Uri baseUri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            if (baseUri != null)
            {
                writer.WriteAttributeString(
                    AtomConstants.XmlBaseAttributeName,
                    AtomConstants.XmlNamespace,
                    baseUri.AbsoluteUri);
            }

            WriteDefaultNamespaceAttributes(writer, DefaultNamespaceFlags.All);
        }

        /// <summary>
        /// Writes the count.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="count">Count value.</param>
        /// <param name="includeNamespaceDeclaration">True if the namespace declaration for the metadata namespace should be included; otherwise false.</param>
        internal static void WriteCount(XmlWriter writer, long count, bool includeNamespaceDeclaration)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            writer.WriteStartElement(
                AtomConstants.ODataMetadataNamespacePrefix,
                AtomConstants.ODataCountElementName,
                AtomConstants.ODataMetadataNamespace);

            if (includeNamespaceDeclaration)
            {
                WriteDefaultNamespaceAttributes(writer, DefaultNamespaceFlags.ODataMetadata);
            }

            writer.WriteValue(count);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes an Xml element with the specified primitive value as content.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="prefix">The prefix for the element's namespace.</param>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="ns">The namespace of the element.</param>
        /// <param name="textContent">The value to be used as element content.</param>
        internal static void WriteElementWithTextContent(XmlWriter writer, string prefix, string localName, string ns, string textContent)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(prefix != null, "prefix != null");
            Debug.Assert(!string.IsNullOrEmpty(localName), "!string.IsNullOrEmpty(localName)");
            Debug.Assert(!string.IsNullOrEmpty(ns), "!string.IsNullOrEmpty(ns)");

            writer.WriteStartElement(prefix, localName, ns);

            if (textContent != null)
            {
                writer.WriteString(textContent);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes an Xml element with empty content.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="prefix">The prefix for the element's namespace.</param>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="ns">The namespace of the element.</param>
        internal static void WriteEmptyElement(XmlWriter writer, string prefix, string localName, string ns)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(prefix != null, "prefix != null");
            Debug.Assert(!string.IsNullOrEmpty(localName), "!string.IsNullOrEmpty(localName)");
            Debug.Assert(!string.IsNullOrEmpty(ns), "!string.IsNullOrEmpty(ns)");

            writer.WriteStartElement(prefix, localName, ns);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="error">The error instance to write.</param>
        /// <param name="includeDebugInformation">A flag indicating whether error details should be written (in debug mode only) or not.</param>
        internal static void WriteError(XmlWriter writer, ODataError error, bool includeDebugInformation)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(error != null, "error != null");

            string code, message, messageLanguage;
            ODataUtilsInternal.GetErrorDetails(error, out code, out message, out messageLanguage);

            string innerError = includeDebugInformation ? error.InnerError : null;
            WriteError(writer, code, message, messageLanguage, innerError);
        }

        /// <summary>
        /// Write the m:etag attribute with the given string value.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="etag">The string value of the ETag.</param>
        internal static void WriteETag(XmlWriter writer, string etag)
        {
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            // m:null="true"
            writer.WriteAttributeString(
                AtomConstants.ODataMetadataNamespacePrefix,
                AtomConstants.ODataNullAttributeName,
                AtomConstants.ODataMetadataNamespace,
                AtomConstants.AtomTrueLiteral);
        }

        /// <summary>
        /// Writes a named stream to the ATOM payload including an 
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="namedStream">The named stream to create the payload for.</param>
        internal static void WriteNamedStream(XmlWriter writer, Uri baseUri, ODataMediaResource namedStream)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(namedStream != null, "Named stream must not be null.");

            // <atom:link rel="...2007/08/dataservices/mediaresource/name">
            writer.WriteStartElement(
                AtomConstants.AtomNamespacePrefix,
                AtomConstants.AtomLinkElementName,
                AtomConstants.AtomNamespace);

            writer.WriteAttributeString(
                AtomConstants.AtomLinkRelationAttributeName,
                AtomUtils.ComputeNamedStreamRelation(namedStream, false));

            writer.WriteAttributeString(
                AtomConstants.AtomTitleElementName,
                namedStream.Name);

            Uri readLink = namedStream.ReadLink;
            if (readLink != null)
            {
                // TODO, ckerer: should we throw when no source link is provided?
                writer.WriteAttributeString(
                    AtomConstants.AtomHRefAttributeName,
                    AtomUtils.ToUrlAttributeValue(readLink, baseUri));
            }

            writer.WriteAttributeString(
                AtomConstants.AtomTypeAttributeName,
                namedStream.ContentType);

            writer.WriteEndElement();

            if (namedStream.EditLink != null)
            {
                // <atom:link rel="...2007/08/dataservices/edit-media/name">
                writer.WriteStartElement(
                    AtomConstants.AtomNamespacePrefix,
                    AtomConstants.AtomLinkElementName,
                    AtomConstants.AtomNamespace);

                writer.WriteAttributeString(
                    AtomConstants.AtomLinkRelationAttributeName,
                    AtomUtils.ComputeNamedStreamRelation(namedStream, true));

                writer.WriteAttributeString(
                    AtomConstants.AtomTitleElementName,
                    namedStream.Name);

                Uri editLink = namedStream.EditLink;
                if (editLink != null)
                {
                    writer.WriteAttributeString(
                        AtomConstants.AtomHRefAttributeName,
                        AtomUtils.ToUrlAttributeValue(editLink, baseUri));
                }

                writer.WriteAttributeString(
                    AtomConstants.AtomTypeAttributeName,
                    namedStream.ContentType);

                string etag = namedStream.ETag;
                if (etag != null)
                {
                    WriteETag(writer, etag);
                }

                writer.WriteEndElement();
            }
        }
        
        /// <summary>
        /// Writes an association link.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="entry">The entry for which to write the association link.</param>
        /// <param name="associationLink">The association link to write.</param>
        internal static void WriteAssociationLink(XmlWriter writer, Uri baseUri, ODataEntry entry, ODataAssociationLink associationLink)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(associationLink != null, "associationLink != null");

            // <atom:link ...
            writer.WriteStartElement(
                AtomConstants.AtomNamespacePrefix,
                AtomConstants.AtomLinkElementName,
                AtomConstants.AtomNamespace);

            ODataAtomWriterMetadataUtils.WriteODataAssociationLinkMetadata(writer, baseUri, entry, associationLink);

            // />
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the self or edit link.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="link">Uri object for the link.</param>
        /// <param name="linkRelation">Relationship value. Either "edit" or "self".</param>
        /// <param name="title">Title for the link.</param>
        internal static void WriteReadOrEditLink(XmlWriter writer, Uri baseUri, Uri link, string linkRelation, string title)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            if (link != null)
            {
                // <atom:link>
                writer.WriteStartElement(
                    AtomConstants.AtomNamespacePrefix,
                    AtomConstants.AtomLinkElementName,
                    AtomConstants.AtomNamespace);

                writer.WriteAttributeString(
                    AtomConstants.AtomLinkRelationAttributeName,
                    linkRelation);

                writer.WriteAttributeString(
                    AtomConstants.AtomTitleElementName,
                    title ?? String.Empty);

                writer.WriteAttributeString(
                    AtomConstants.AtomHRefAttributeName,
                    AtomUtils.ToUrlAttributeValue(link, baseUri));

                // </atom:link>
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Write the given collection of properties.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="metadata">The metadata provider to use or null if no metadata is available.</param>
        /// <param name="owningType">The <see cref="ResourceType"/> of the entry (or null if not metadata is available).</param>
        /// <param name="cachedProperties">Collection of cached properties for the entry.</param>
        /// <param name="version">The protocol version used for writing.</param>
        /// <param name="isWritingCollection">True if we are writing a collection instead of an entry.</param>
        /// <param name="epmValueCache">Cache of values used in EPM so that we avoid multiple enumerations of properties/items. (can be null)</param>
        /// <param name="epmSourcePathSegment">The EPM source path segment which points to the property which sub-properites we're writing. (can be null)</param>
        internal static void WriteProperties(
            XmlWriter writer,
            DataServiceMetadataProviderWrapper metadata,
            ResourceType owningType,
            IEnumerable<ODataProperty> cachedProperties, 
            ODataVersion version, 
            bool isWritingCollection,
            EpmValueCache epmValueCache, 
            EpmSourcePathSegment epmSourcePathSegment)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            if (cachedProperties == null)
            {
                return;
            }

            foreach (ODataProperty property in cachedProperties)
            {
                WriteProperty(writer, metadata, property, owningType, version, false, isWritingCollection, epmValueCache, epmSourcePathSegment);
            }
        }

        /// <summary>
        /// Creates an Xml writer over the specified stream, with the provided settings and encoding.
        /// </summary>
        /// <param name="stream">The stream to create the XmlWriter over.</param>
        /// <param name="odataWriterSettings">The OData writer settings used to control the settings of the Xml writer.</param>
        /// <param name="encoding">The encoding used for writing.</param>
        /// <returns>An <see cref="XmlWriter"/> instance configured with the provided settings and encoding.</returns>
        internal static XmlWriter CreateXmlWriter(Stream stream, ODataWriterSettings odataWriterSettings, Encoding encoding)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(odataWriterSettings != null, "odataWriterSettings != null");

            XmlWriterSettings xmlWriterSettings = ODataAtomWriterUtils.CreateXmlWriterSettings(odataWriterSettings, encoding);
            return XmlWriter.Create(stream, xmlWriterSettings);
        }

        /// <summary>
        /// Writes a single property in ATOM format.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="metadata">The metadata provider to use or null if no metadata is available.</param>
        /// <param name="property">The property to write out.</param>
        /// <param name="owningType">The type owning the property (or null if no metadata is available).</param>
        /// <param name="version">The protocol version used for writing.</param>
        /// <param name="isTopLevel">True if writing a top-level property payload; otherwise false.</param>
        /// <param name="isWritingCollection">True if we are writing a collection instead of an entry.</param>
        /// <param name="epmValueCache">Cache of values used in EPM so that we avoid multiple enumerations of properties/items. (can be null)</param>
        /// <param name="epmParentSourcePathSegment">The EPM source path segment which points to the property which sub-property we're writing. (can be null)</param>
        internal static void WriteProperty(
            XmlWriter writer,
            DataServiceMetadataProviderWrapper metadata,
            ODataProperty property,
            ResourceType owningType,
            ODataVersion version, 
            bool isTopLevel,
            bool isWritingCollection,
            EpmValueCache epmValueCache,
            EpmSourcePathSegment epmParentSourcePathSegment)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            ValidationUtils.ValidateProperty(property);
            ResourceProperty resourceProperty = ValidationUtils.ValidatePropertyDefined(property.Name, owningType);

            EpmSourcePathSegment epmSourcePathSegment = null;
            if (epmParentSourcePathSegment != null)
            {
                epmSourcePathSegment = epmParentSourcePathSegment.SubProperties.Where(subProperty => subProperty.PropertyName == property.Name).FirstOrDefault();
            }

            object value = property.Value;

            // TODO: If we implement validation or type conversions the value needs to be converted here
            //       since the next method call needs to know if the value is a string or not in some cases.

            // If EPM tells us to skip this property in content, then we're done here.
            if (!ShouldWritePropertyInContent(value, epmSourcePathSegment, version))
            {
                return;
            }

            // <d:propertyname>
            writer.WriteStartElement(
                isWritingCollection ? string.Empty : AtomConstants.ODataNamespacePrefix,
                property.Name,
                AtomConstants.ODataNamespace);

            if (isTopLevel)
            {
                WriteDefaultNamespaceAttributes(writer, DefaultNamespaceFlags.OData | DefaultNamespaceFlags.ODataMetadata);
            }

            // Null property value.
            if (value == null)
            {
                // verify that MultiValue properties are not null
                if (resourceProperty != null && resourceProperty.Kind == ResourcePropertyKind.MultiValue)
                {
                    throw new ODataException(Strings.ODataWriter_MultiValuePropertiesMustNotHaveNullValue(resourceProperty.Name));
                }

                ODataAtomWriterUtils.WriteNullAttribute(writer);
            }
            else
            {
                ODataComplexValue complexValue = value as ODataComplexValue;
                ResourceType resourcePropertyType = resourceProperty == null ? null : resourceProperty.ResourceType;
                bool isOpenPropertyType = owningType != null && owningType.IsOpenType && resourceProperty == null;

                // Complex properties are written recursively.
                if (complexValue != null)
                {
                    WriteComplexValue(writer, metadata, complexValue, resourcePropertyType, isOpenPropertyType, isWritingCollection, version, epmValueCache, epmSourcePathSegment);
                }
                else
                {
                    ODataMultiValue multiValue = value as ODataMultiValue;
                    if (multiValue != null)
                    {
                        ODataVersionChecker.CheckMultiValueProperties(version, property.Name);
                        WriteMultiValue(writer, metadata, multiValue, resourcePropertyType, isOpenPropertyType, isWritingCollection, version, epmValueCache, epmSourcePathSegment);
                    }
                    else
                    {
                        WritePrimitiveValue(writer, value, resourcePropertyType);
                    }
                }
            }

            // </d:propertyname>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes out the value of a complex property.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="metadata">The metadata provider to use or null if no metadata is available.</param>
        /// <param name="complexValue">The complex value to write.</param>
        /// <param name="metadataType">The metadata type for the complex value.</param>
        /// <param name="isOpenPropertyType">True if the type name belongs to an open property.</param>
        /// <param name="isWritingCollection">True if we are writing a collection instead of an entry.</param>
        /// <param name="version">The protocol version used for writing.</param>
        /// <param name="epmValueCache">Cache of values used in EPM so that we avoid multiple enumerations of properties/items. (can be null)</param>
        /// <param name="epmSourcePathSegment">The EPM source path segment which points to the property we're writing. (can be null)</param>
        internal static void WriteComplexValue(
            XmlWriter writer,
            DataServiceMetadataProviderWrapper metadata,
            ODataComplexValue complexValue,
            ResourceType metadataType,
            bool isOpenPropertyType,
            bool isWritingCollection,
            ODataVersion version,
            EpmValueCache epmValueCache,
            EpmSourcePathSegment epmSourcePathSegment)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(complexValue != null, "complexValue != null");

            string typeName = complexValue.TypeName;

            // resolve the type name to the resource type; if no type name is specified we will use the 
            // type inferred from metadata
            ResourceType complexValueType = MetadataUtils.ResolveTypeName(metadata, metadataType, ref typeName, ResourceTypeKind.ComplexType, isOpenPropertyType);

            if (typeName != null)
            {
                WritePropertyTypeAttribute(writer, typeName);
            }

            WriteProperties(
                writer,
                metadata,
                complexValueType,
                EpmValueCache.GetComplexValueProperties(epmValueCache, epmSourcePathSegment, complexValue, true),
                version,
                isWritingCollection,
                epmValueCache,
                epmSourcePathSegment);
        }

        /// <summary>
        /// Writes a single Uri in response to a $links query.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="link">The associated entity link to write out.</param>
        /// <param name="isTopLevel">
        /// A flag indicating whether the link is written as top-level element or not; 
        /// this controls whether to include namespace declarations etc.
        /// </param>
        internal static void WriteAssociatedEntityLink(XmlWriter writer, ODataAssociatedEntityLink link, bool isTopLevel)
        {
            DebugUtils.CheckNoExternalCallers();

            // <uri ...
            writer.WriteStartElement(string.Empty, AtomConstants.ODataUriElementName, AtomConstants.ODataNamespace);

            if (isTopLevel)
            {
                // xmlns=
                writer.WriteAttributeString(AtomConstants.XmlnsNamespacePrefix, AtomConstants.ODataNamespace);
            }

            writer.WriteString(UriUtils.UriToString(link.Url));

            // </uri>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a set of links (Uris) in response to a $links query; includes optional count and next-page-link information.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="links">The associated entity links to write.</param>
        internal static void WriteAssociatedEntityLinks(XmlWriter writer, ODataAssociatedEntityLinks links)
        {
            DebugUtils.CheckNoExternalCallers();

            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(links != null, "links != null");

            // <links> ...
            writer.WriteStartElement(string.Empty, AtomConstants.ODataLinksElementName, AtomConstants.ODataNamespace);

            // xmlns=
            writer.WriteAttributeString(AtomConstants.XmlnsNamespacePrefix, AtomConstants.ODataNamespace);

            if (links.InlineCount.HasValue)
            {
                // <m:count>
                WriteCount(writer, links.InlineCount.Value, true);
            }

            IEnumerable<ODataAssociatedEntityLink> associatedEntityLinks = links.Links;
            if (associatedEntityLinks != null)
            {
                foreach (ODataAssociatedEntityLink link in associatedEntityLinks)
                {
                    WriteAssociatedEntityLink(writer, link, false);
                }
            }

            if (links.NextLink != null)
            {
                // <d:next>
                writer.WriteElementString(string.Empty, AtomConstants.ODataNextLinkElementName, AtomConstants.ODataNamespace, UriUtils.UriToString(links.NextLink));
            }

            // </links>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a primitive value.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="expectedType">The expected resource type of the primitive value.</param>
        internal static void WritePrimitiveValue(XmlWriter writer, object value, ResourceType expectedType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(value != null, "value != null");

            string typeName;
            if (!MetadataUtils.TryGetPrimitiveTypeName(value, out typeName))
            {
                throw new ODataException(Strings.ODataWriter_UnsupportedPrimitiveType(value.GetType().FullName));
            }

            if (typeName != EdmConstants.EdmStringTypeName)
            {
                WritePropertyTypeAttribute(writer, typeName);
            }

            AtomValueUtils.WritePrimitiveValue(writer, value, expectedType);
        }

        /// <summary>
        /// Writes a service document in ATOM/XML format.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="metadata">The metadata provider to use or null if no metadata is available.</param>
        /// <param name="defaultWorkspace">The default workspace to write in the service document.</param>
        /// <param name="baseUri">The base Uri specified in the writer settings for writing the service document.</param>
        internal static void WriteServiceDocument(
            XmlWriter writer,
            DataServiceMetadataProviderWrapper metadata,
            ODataWorkspace defaultWorkspace,
            Uri baseUri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(defaultWorkspace != null, "defaultWorkspace != null");

            if (baseUri == null)
            {
                // We require a base Uri for writing service documents in Xml format since the Uris for resource collections/entity sets
                // will be relative.
                throw new ODataException(Strings.ODataAtomWriterUtils_BaseUriRequiredForWritingServiceDocument);
            }

            IEnumerable<ODataResourceCollectionInfo> collections =
                ValidationUtils.ValidateWorkspace(metadata == null ? null : metadata.ResourceSets, defaultWorkspace);

            // <app:service>
            writer.WriteStartElement(string.Empty, AtomConstants.AtomPublishingServiceElementName, AtomConstants.AtomPublishingNamespace);

            // xml:base=...
            writer.WriteAttributeString(AtomConstants.XmlBaseAttributeName, AtomConstants.XmlNamespace, baseUri.AbsoluteUri);

            // xmlns=http://www.w3.org/2007/app
            writer.WriteAttributeString(AtomConstants.XmlnsNamespacePrefix, AtomConstants.XmlNamespacesNamespace, AtomConstants.AtomPublishingNamespace);

            // xmlns:atom="http://www.w3.org/2005/Atom"
            writer.WriteAttributeString(
                AtomConstants.NonEmptyAtomNamespacePrefix,
                AtomConstants.XmlNamespacesNamespace,
                AtomConstants.AtomNamespace);

            // <app:workspace>
            writer.WriteStartElement(string.Empty, AtomConstants.AtomPublishingWorkspaceElementName, AtomConstants.AtomPublishingNamespace);

            ODataAtomWriterMetadataUtils.WriteWorkspaceMetadata(writer, defaultWorkspace);

            foreach (ODataResourceCollectionInfo collectionInfo in collections)
            {
                // <app:collection>
                writer.WriteStartElement(string.Empty, AtomConstants.AtomPublishingCollectionElementName, AtomConstants.AtomPublishingNamespace);

                // The name of the collection is the entity set name; The href of the <app:collection> element must be the link for the entity set.
                // Since we model the collection as having a 'Name' (for JSON) we require a base Uri for Atom/Xml.
                writer.WriteAttributeString(AtomConstants.AtomHRefAttributeName, Uri.EscapeUriString(collectionInfo.Name));

                ODataAtomWriterMetadataUtils.WriteCollectionMetadata(writer, collectionInfo);

                // </app:collection>
                writer.WriteEndElement();
            }

            // </app:workspace>
            writer.WriteEndElement();

            // </app:service>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Write the namespaces for OData (prefix 'd') and OData metadata (prefix 'm')
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="flags">An enumeration value to indicate what default namespace attributes to write.</param>
        private static void WriteDefaultNamespaceAttributes(XmlWriter writer, DefaultNamespaceFlags flags)
        {
            DebugUtils.CheckNoExternalCallers();

            if ((flags & DefaultNamespaceFlags.Atom) == DefaultNamespaceFlags.Atom)
            {
                writer.WriteAttributeString(
                    AtomConstants.XmlnsNamespacePrefix,
                    AtomConstants.XmlNamespacesNamespace,
                    AtomConstants.AtomNamespace);
            }

            if ((flags & DefaultNamespaceFlags.OData) == DefaultNamespaceFlags.OData)
            {
                writer.WriteAttributeString(
                    AtomConstants.ODataNamespacePrefix,
                    AtomConstants.XmlNamespacesNamespace,
                    AtomConstants.ODataNamespace);
            }

            if ((flags & DefaultNamespaceFlags.ODataMetadata) == DefaultNamespaceFlags.ODataMetadata)
            {
                writer.WriteAttributeString(
                    AtomConstants.ODataMetadataNamespacePrefix,
                    AtomConstants.XmlNamespacesNamespace,
                    AtomConstants.ODataMetadataNamespace);
            }
        }

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="code">The code of the error.</param>
        /// <param name="message">The message of the error.</param>
        /// <param name="messageLanguage">The language of the message.</param>
        /// <param name="innerError">Inner error details that will be included in debug mode (if present).</param>
        private static void WriteError(XmlWriter writer, string code, string message, string messageLanguage, string innerError)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(code != null, "code != null");
            Debug.Assert(message != null, "message != null");
            Debug.Assert(messageLanguage != null, "messageLanguage != null");

            // <m:error>
            writer.WriteStartElement(AtomConstants.ODataMetadataNamespacePrefix, AtomConstants.ODataErrorElementName, AtomConstants.ODataMetadataNamespace);

            // <m:code>code</m:code>
            writer.WriteElementString(AtomConstants.ODataMetadataNamespacePrefix, AtomConstants.ODataErrorCodeElementName, AtomConstants.ODataMetadataNamespace, code);

            // <m:message>
            writer.WriteStartElement(AtomConstants.ODataMetadataNamespacePrefix, AtomConstants.ODataErrorMessageElementName, AtomConstants.ODataMetadataNamespace);

            // xml:lang="..."
            writer.WriteAttributeString(AtomConstants.XmlNamespacePrefix, AtomConstants.XmlLangAttributeName, AtomConstants.XmlNamespace, messageLanguage);

            writer.WriteString(message);

            // </m:message>
            writer.WriteEndElement();

            // TODO, ckerer: figure out what to do about <m:detail /> that is defined in the functional spec
            if (!string.IsNullOrEmpty(innerError))
            {
                WriteInnerError(writer, innerError);
            }

            // </m:error>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the inner exception information in debug mode.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="innerError">The inner error to write.</param>
        private static void WriteInnerError(XmlWriter writer, string innerError)
        {
            Debug.Assert(writer != null, "writer != null");

            // <m:innererror>
            writer.WriteStartElement(AtomConstants.ODataMetadataNamespacePrefix, AtomConstants.ODataInnerErrorElementName, AtomConstants.ODataMetadataNamespace);

            writer.WriteString(innerError);

            // </m:innererror>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Write the items in a MultiValue in ATOM format.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="metadata">The metadata provider to use or null if no metadata is available.</param>
        /// <param name="multiValue">The MultiValue to write.</param>
        /// <param name="resourcePropertyType">The resource type of the multi value (or null if not metadata is available).</param>
        /// <param name="isOpenPropertyType">True if the type name belongs to an open property.</param>
        /// <param name="isWritingCollection">True if we are writing a collection instead of an entry.</param>
        /// <param name="version">The protocol version used for writing.</param>
        /// <param name="epmValueCache">Cache of values used in EPM so that we avoid multiple enumerations of properties/items. (can be null)</param>
        /// <param name="epmSourcePathSegment">The EPM source path segment which points to the multivalue property we're writing. (can be null)</param>
        private static void WriteMultiValue(
            XmlWriter writer,
            DataServiceMetadataProviderWrapper metadata,
            ODataMultiValue multiValue, 
            ResourceType resourcePropertyType,
            bool isOpenPropertyType,
            bool isWritingCollection,
            ODataVersion version,
            EpmValueCache epmValueCache, 
            EpmSourcePathSegment epmSourcePathSegment)
        {
            Debug.Assert(multiValue != null, "multiValue != null");

            string typeName = multiValue.TypeName;

            // resolve the type name to the resource type; if no type name is specified we will use the 
            // type inferred from metadata
            MultiValueResourceType multiValueType = (MultiValueResourceType)MetadataUtils.ResolveTypeName(metadata, resourcePropertyType, ref typeName, ResourceTypeKind.MultiValue, isOpenPropertyType);

            if (typeName != null)
            {
                WritePropertyTypeAttribute(writer, typeName);
            }

            ResourceType expectedItemType = multiValueType == null ? null : multiValueType.ItemType;

            IEnumerable items = EpmValueCache.GetMultiValueItems(epmValueCache, epmSourcePathSegment, multiValue, true);
            if (items != null)
            {
                foreach (object itemValue in items)
                {
                    object item;
                    EpmMultiValueItemCache epmItemCache = itemValue as EpmMultiValueItemCache;
                    if (epmItemCache != null)
                    {
                        item = epmItemCache.ItemValue;
                    }
                    else
                    {
                        item = itemValue;
                    }

                    ValidationUtils.ValidateMultiValueItem(item);

                    writer.WriteStartElement(AtomConstants.ODataNamespacePrefix, AtomConstants.ODataMultiValueItemElementName, AtomConstants.ODataNamespace);
                    ODataComplexValue complexValue = item as ODataComplexValue;
                    if (complexValue != null)
                    {
                        WriteComplexValue(writer, metadata, complexValue, expectedItemType, false, isWritingCollection, version, epmItemCache, epmSourcePathSegment);
                    }
                    else
                    {
                        ODataMultiValue multiValueItem = item as ODataMultiValue;
                        if (multiValueItem != null)
                        {
                            throw new ODataException(Strings.ODataWriter_NestedMultiValuesAreNotSupported);
                        }
                        else
                        {
                            AtomValueUtils.WritePrimitiveValue(writer, item, expectedItemType);
                        }
                    }

                    writer.WriteEndElement();
                }
            }
        }

        /// <summary>
        /// Writes the m:type attribute for a property given the name of the type.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="typeName">The type name to write.</param>
        private static void WritePropertyTypeAttribute(XmlWriter writer, string typeName)
        {
            // m:type attribute
            writer.WriteAttributeString(
                AtomConstants.ODataMetadataNamespacePrefix,
                AtomConstants.AtomTypeAttributeName,
                AtomConstants.ODataMetadataNamespace,
                typeName);
        }

        /// <summary>
        /// Determines if the property with the specified value should be written into content or not.
        /// </summary>
        /// <param name="propertyValue">The property value to write.</param>
        /// <param name="epmSourcePathSegment">The EPM source path segment for the property being written.</param>
        /// <param name="version">The version of the protocol being used for the response.</param>
        /// <returns>true if the property should be written into content, or false otherwise</returns>
        private static bool ShouldWritePropertyInContent(object propertyValue, EpmSourcePathSegment epmSourcePathSegment, ODataVersion version)
        {
            if (epmSourcePathSegment == null)
            {
                return true;
            }

            EntityPropertyMappingInfo epmInfo = epmSourcePathSegment.EpmInfo;
            if (epmInfo == null)
            {
                return true;
            }

            EntityPropertyMappingAttribute epmAttribute = epmInfo.Attribute;
            Debug.Assert(epmAttribute != null, "Attribute should always be initialized for EpmInfo.");
            if (version <= ODataVersion.V2)
            {
                // In V2 and lower we sometimes write properties into content even if asked not to.
                // If the property value is null, we always write into content
                if (propertyValue == null)
                {
                    return true;
                }

                string stringPropertyValue = propertyValue as string;
                if (stringPropertyValue != null && stringPropertyValue.Length == 0)
                {
                    // If the property value is an empty string and we should be writing it into an ATOM element which does not allow empty string
                    // we write it into content as well.
                    switch (epmAttribute.TargetSyndicationItem)
                    {
                        case SyndicationItemProperty.AuthorEmail:
                        case SyndicationItemProperty.AuthorUri:
                        case SyndicationItemProperty.ContributorEmail:
                        case SyndicationItemProperty.ContributorUri:
                            return true;

                        default:
                            break;
                    }
                }
            }

            return epmAttribute.KeepInContent;
        }
    }
}
