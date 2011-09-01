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
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

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

            /// <summary>GeoRss namespace.</summary>
            GeoRss = 0x08,

            /// <summary>GML namespace.</summary>
            Gml = 0x10,

            /// <summary>All default namespaces.</summary>
            All = OData | ODataMetadata | Atom | GeoRss | Gml
        }

        /// <summary>
        /// Creates a new XmlWriterSettings instance using the encoding.
        /// </summary>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <param name="encoding">Encoding to  use in the writer settings.</param>
        /// <returns>The Xml writer settings to use for this writer.</returns>
        internal static XmlWriterSettings CreateXmlWriterSettings(ODataMessageWriterSettings messageWriterSettings, Encoding encoding)
        {
            DebugUtils.CheckNoExternalCallers();

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
            ErrorUtils.WriteXmlError(writer, error, includeDebugInformation);
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
        /// Writes a named stream to the ATOM payload
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="namedStreamProperty">The named stream property to create the payload for.</param>
        /// <param name="owningType">The <see cref="IEdmEntityType"/> instance for which the named stream property defined on.</param>
        /// <param name="version">The version of OData protocol to use.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        internal static void WriteNamedStreamProperty(
            XmlWriter writer, 
            Uri baseUri, 
            IODataUrlResolver urlResolver,
            ODataProperty namedStreamProperty,
            IEdmEntityType owningType, 
            ODataVersion version,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            ProjectedPropertiesAnnotation projectedProperties)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(namedStreamProperty != null, "Named stream property must not be null.");
            Debug.Assert(namedStreamProperty.Value != null, "The media resource of the named stream property must not be null.");

            WriterValidationUtils.ValidatePropertyNotNull(namedStreamProperty);
            string propertyName = namedStreamProperty.Name;
            if (projectedProperties.ShouldSkipProperty(propertyName))
            {
                return;
            }

            WriterValidationUtils.ValidateProperty(namedStreamProperty);
            duplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(namedStreamProperty);
            IEdmProperty edmProperty = ValidationUtils.ValidatePropertyDefined(namedStreamProperty.Name, owningType);
            WriterValidationUtils.ValidateStreamReferenceProperty(namedStreamProperty, edmProperty, version);
            ODataStreamReferenceValue namedStream = (ODataStreamReferenceValue)namedStreamProperty.Value;
            if (owningType != null && owningType.IsOpen && edmProperty == null)
            {
                ValidationUtils.ValidateOpenPropertyValue(namedStreamProperty.Name, namedStream);
            }

            AtomStreamReferenceMetadata streamReferenceMetadata = namedStream.GetAnnotation<AtomStreamReferenceMetadata>();
            string contentType = namedStream.ContentType;
            string linkTitle = namedStreamProperty.Name;

            Uri readLink = namedStream.ReadLink;
            if (readLink != null)
            {
                string readLinkRelation = AtomUtils.ComputeNamedStreamRelation(namedStreamProperty, false);

                AtomLinkMetadata readLinkMetadata = streamReferenceMetadata == null ? null : streamReferenceMetadata.SelfLink;
                AtomLinkMetadata mergedMetadata = ODataAtomWriterMetadataUtils.MergeLinkMetadata(readLinkMetadata, readLinkRelation, readLink, linkTitle, contentType);
                ODataAtomWriterMetadataUtils.WriteAtomLink(writer, baseUri, urlResolver, mergedMetadata, null /* etag */);
            }

            Uri editLink = namedStream.EditLink;
            if (editLink != null)
            {
                string editLinkRelation = AtomUtils.ComputeNamedStreamRelation(namedStreamProperty, true);

                AtomLinkMetadata editLinkMetadata = streamReferenceMetadata == null ? null : streamReferenceMetadata.EditLink;
                AtomLinkMetadata mergedMetadata = ODataAtomWriterMetadataUtils.MergeLinkMetadata(editLinkMetadata, editLinkRelation, editLink, linkTitle, contentType);
                ODataAtomWriterMetadataUtils.WriteAtomLink(writer, baseUri, urlResolver, mergedMetadata, namedStream.ETag);
            }
        }
       
        /// <summary>
        /// Writes the self or edit link.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="link">Uri object for the link.</param>
        /// <param name="linkMetadata">The atom link metadata for the link to specify title, type, hreflang and length of the link.</param>
        /// <param name="linkRelation">Relationship value. Either "edit" or "self".</param>
        internal static void WriteReadOrEditLink(
            XmlWriter writer, 
            Uri baseUri, 
            IODataUrlResolver urlResolver,
            Uri link, 
            AtomLinkMetadata linkMetadata,
            string linkRelation)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            if (link != null)
            {
                AtomLinkMetadata mergedLinkMetadata = ODataAtomWriterMetadataUtils.MergeLinkMetadata(
                    linkMetadata,
                    linkRelation,
                    link,
                    null /* title */,
                    null /* media type */);

                ODataAtomWriterMetadataUtils.WriteAtomLink(writer, baseUri, urlResolver, mergedLinkMetadata, null /* etag */);
            }
        }

        /// <summary>
        /// Writes the start element for the m:properties element on the entry.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        internal static void WriteEntryPropertiesStart(XmlWriter writer)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            // <m:properties> if required
            writer.WriteStartElement(
                AtomConstants.ODataMetadataNamespacePrefix,
                AtomConstants.AtomPropertiesElementName,
                AtomConstants.ODataMetadataNamespace);
        }

        /// <summary>
        /// Writes the end element for the m:properties element on the entry.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        internal static void WriteEntryPropertiesEnd(XmlWriter writer)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            // </m:properties>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Write the given collection of properties.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="model">The model to use or null if no metadata is available.</param>
        /// <param name="owningType">The <see cref="IEdmStructuredType"/> of the entry (or null if not metadata is available).</param>
        /// <param name="cachedProperties">Collection of cached properties for the entry.</param>
        /// <param name="version">The protocol version used for writing.</param>
        /// <param name="isWritingCollection">true if we are writing a collection instead of an entry.</param>
        /// <param name="isWritingResponse">true if we are writing a response, false if it's a request.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        /// <param name="beforePropertiesAction">Action which is called before the properties are written, if there are any property.</param>
        /// <param name="afterPropertiesAction">Action which is called after the properties are written, if there are any property.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <param name="epmValueCache">Cache of values used in EPM so that we avoid multiple enumerations of properties/items. (can be null)</param>
        /// <param name="epmSourcePathSegment">The EPM source path segment which points to the property which sub-properites we're writing. (can be null)</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        /// <returns>true if anything was written, false otherwise.</returns>
        internal static bool WriteProperties(
            XmlWriter writer,
            IEdmModel model,
            IEdmStructuredType owningType,
            IEnumerable<ODataProperty> cachedProperties, 
            ODataVersion version, 
            bool isWritingCollection,
            bool isWritingResponse,
            ODataWriterBehavior writerBehavior,
            Action<XmlWriter> beforePropertiesAction,
            Action<XmlWriter> afterPropertiesAction,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            EpmValueCache epmValueCache, 
            EpmSourcePathSegment epmSourcePathSegment,
            ProjectedPropertiesAnnotation projectedProperties)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(writerBehavior != null, "writerBehavior != null");

            if (cachedProperties == null)
            {
                return false;
            }

            bool propertyWritten = false;
            foreach (ODataProperty property in cachedProperties)
            {
                propertyWritten |= WriteProperty(
                    writer,
                    model,
                    property,
                    owningType,
                    version,
                    false,
                    isWritingCollection,
                    isWritingResponse,
                    writerBehavior,
                    propertyWritten ? null : beforePropertiesAction,
                    epmValueCache,
                    epmSourcePathSegment,
                    duplicatePropertyNamesChecker,
                    projectedProperties);
            }

            if (afterPropertiesAction != null && propertyWritten)
            {
                afterPropertiesAction(writer);
            }

            return propertyWritten;
        }

        /// <summary>
        /// Creates an Xml writer over the specified stream, with the provided settings and encoding.
        /// </summary>
        /// <param name="stream">The stream to create the XmlWriter over.</param>
        /// <param name="messageWriterSettings">The OData message writer settings used to control the settings of the Xml writer.</param>
        /// <param name="encoding">The encoding used for writing.</param>
        /// <returns>An <see cref="XmlWriter"/> instance configured with the provided settings and encoding.</returns>
        internal static XmlWriter CreateXmlWriter(Stream stream, ODataMessageWriterSettings messageWriterSettings, Encoding encoding)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(messageWriterSettings != null, "messageWriterSettings != null");

            XmlWriterSettings xmlWriterSettings = CreateXmlWriterSettings(messageWriterSettings, encoding);
            return XmlWriter.Create(stream, xmlWriterSettings);
        }

        /// <summary>
        /// Writes a single property in ATOM format.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="model">The model to use or null if no metadata is available.</param>
        /// <param name="property">The property to write out.</param>
        /// <param name="owningType">The type owning the property (or null if no metadata is available).</param>
        /// <param name="version">The protocol version used for writing.</param>
        /// <param name="isTopLevel">true if writing a top-level property payload; otherwise false.</param>
        /// <param name="isWritingCollection">true if we are writing a collection instead of an entry.</param>
        /// <param name="isWritingResponse">true if we are writing a response; false if it's a request.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        /// <param name="beforePropertyAction">Action which is called before the property is written, if it's going to be written.</param>
        /// <param name="epmValueCache">Cache of values used in EPM so that we avoid multiple enumerations of properties/items. (can be null)</param>
        /// <param name="epmParentSourcePathSegment">The EPM source path segment which points to the property which sub-property we're writing. (can be null)</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        /// <returns>true if the property was actually written, false otherwise.</returns>
        internal static bool WriteProperty(
            XmlWriter writer,
            IEdmModel model,
            ODataProperty property,
            IEdmStructuredType owningType,
            ODataVersion version, 
            bool isTopLevel,
            bool isWritingCollection,
            bool isWritingResponse,
            ODataWriterBehavior writerBehavior,
            Action<XmlWriter> beforePropertyAction,
            EpmValueCache epmValueCache,
            EpmSourcePathSegment epmParentSourcePathSegment,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            ProjectedPropertiesAnnotation projectedProperties)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(writerBehavior != null, "writerBehavior != null");

            WriterValidationUtils.ValidatePropertyNotNull(property);

            object value = property.Value;
            string propertyName = property.Name;
            EpmSourcePathSegment epmSourcePathSegment = EpmWriterUtils.GetPropertySourcePathSegment(epmParentSourcePathSegment, propertyName);

            //// TODO: If we implement type conversions the value needs to be converted here
            ////       since the next method call needs to know if the value is a string or not in some cases.

            ODataComplexValue complexValue = value as ODataComplexValue;
            ProjectedPropertiesAnnotation complexValueProjectedProperties = null;
            if (!ShouldWritePropertyInContent(owningType, projectedProperties, propertyName, value, epmSourcePathSegment, version, writerBehavior))
            {
                // If ShouldWritePropertyInContent returns false for a comlex value we have to continue
                // writing the property but set the projectedProperties to an empty array. The reason for this
                // is that we might find EPM on a nested property that has a null value and thus must be written 
                // in content (in which case the parent property also has to be written).
                // This only applies if we have EPM information for the property and in versions < V3.
                if (version <= ODataVersion.V2 && epmSourcePathSegment != null && complexValue != null)
                {
                    Debug.Assert(!projectedProperties.IsPropertyProjected(propertyName), "ShouldWritePropertyInContent must not return false for a projected complex property.");
                    complexValueProjectedProperties = ProjectedPropertiesAnnotation.EmptyProjectedPropertiesMarker;
                }
                else
                {
                    return false;
                }
            }

            WriterValidationUtils.ValidateProperty(property);
            duplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(property);
            IEdmProperty edmProperty = ValidationUtils.ValidatePropertyDefined(propertyName, owningType);

            if (value is ODataStreamReferenceValue)
            {
                throw new ODataException(Strings.ODataWriter_NamedStreamPropertiesMustBePropertiesOfODataEntry(propertyName));
            }

            // Null property value.
            if (value == null)
            {
                ValidationUtils.ValidateNullPropertyValue(edmProperty, writerBehavior);

                // <d:PropertyName
                WritePropertyStart(writer, beforePropertyAction, propertyName, isWritingCollection, isTopLevel);

                // The default behavior is to not write type name for null values.
                if (edmProperty != null && writerBehavior.BehaviorKind != ODataBehaviorKind.Default)
                {
                    string typeName = edmProperty.Type.ODataFullName();

                    if (typeName != Metadata.EdmConstants.EdmStringTypeName)
                    {
                        // For WCF DS Client we write the type name on null values only for primitive types
                        // For WCF DS Server we write the type name on null values always
                        if (edmProperty.Type.IsODataPrimitiveTypeKind() || writerBehavior.BehaviorKind == ODataBehaviorKind.WcfDataServicesServer)
                        {
                            // m:type = 'type name'
                            ODataAtomWriterUtils.WritePropertyTypeAttribute(writer, typeName);
                        }
                    }
                }

                // m:null = 'true'
                ODataAtomWriterUtils.WriteNullAttribute(writer);

                // />
                WritePropertyEnd(writer);
                return true;
            }

            bool isOpenPropertyType = owningType != null && owningType.IsOpen && edmProperty == null;
            if (isOpenPropertyType)
            {
                ValidationUtils.ValidateOpenPropertyValue(propertyName, value);
            }

            IEdmTypeReference propertyTypeReference = edmProperty == null ? null : edmProperty.Type;
            if (complexValue != null)
            {
                // Complex properties are written recursively.
                DuplicatePropertyNamesChecker complexValuePropertyNamesChecker = new DuplicatePropertyNamesChecker(writerBehavior.AllowDuplicatePropertyNames, isWritingResponse);
                if (isTopLevel)
                {
                    // Top-level property must always write the property element
                    Debug.Assert(complexValueProjectedProperties == null, "complexValueProjectedProperties == null");
                    WritePropertyStart(writer, beforePropertyAction, propertyName, isWritingCollection, isTopLevel);
                    WriteComplexValue(
                        writer,
                        model,
                        complexValue,
                        propertyTypeReference,
                        isOpenPropertyType,
                        isWritingCollection,
                        isWritingResponse,
                        writerBehavior,
                        null  /* beforeValueAction */,
                        null  /* afterValueAction */,
                        complexValuePropertyNamesChecker,
                        null  /* multiValueItemTypeName */,
                        version,
                        epmValueCache,
                        epmSourcePathSegment,
                        null);
                    WritePropertyEnd(writer);
                    return true;
                }

                return WriteComplexValue(
                    writer,
                    model,
                    complexValue,
                    propertyTypeReference,
                    isOpenPropertyType,
                    isWritingCollection,
                    isWritingResponse,
                    writerBehavior,
                    (w) => WritePropertyStart(w, beforePropertyAction, propertyName, isWritingCollection, isTopLevel),
                    (w) => WritePropertyEnd(w),
                    complexValuePropertyNamesChecker,
                    null,
                    version,
                    epmValueCache,
                    epmSourcePathSegment,
                    complexValueProjectedProperties);
            }

            ODataMultiValue multiValue = value as ODataMultiValue;
            if (multiValue != null)
            {
                ODataVersionChecker.CheckMultiValueProperties(version, propertyName);

                WritePropertyStart(writer, beforePropertyAction, propertyName, isWritingCollection, isTopLevel);
                WriteMultiValue(
                    writer, 
                    model, 
                    multiValue, 
                    propertyTypeReference, 
                    isOpenPropertyType, 
                    isWritingCollection,
                    isWritingResponse,
                    writerBehavior,
                    version, 
                    epmValueCache, 
                    epmSourcePathSegment);
                WritePropertyEnd(writer);
                return true;
            }

            WritePropertyStart(writer, beforePropertyAction, propertyName, isWritingCollection, isTopLevel);
            WritePrimitiveValue(writer, value, propertyTypeReference);
            WritePropertyEnd(writer);
            return true;
        }

        /// <summary>
        /// Writes out the value of a complex property.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="model">The model to use or null if no metadata is available.</param>
        /// <param name="complexValue">The complex value to write.</param>
        /// <param name="metadataTypeReference">The metadata type for the complex value.</param>
        /// <param name="isOpenPropertyType">true if the type name belongs to an open property.</param>
        /// <param name="isWritingCollection">true if we are writing a collection instead of an entry.</param>
        /// <param name="isWritingResponse">true if we are writing a response, false if it's a request.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        /// <param name="beforeValueAction">Action called before the complex value is written, if it's actually written.</param>
        /// <param name="afterValueAction">Action called after the copmlex value is written, if it's actually written.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <param name="multiValueItemTypeName">Expected MultiValue item type name if this is an item in a MultiValue.</param>
        /// <param name="version">The protocol version used for writing.</param>
        /// <param name="epmValueCache">Cache of values used in EPM so that we avoid multiple enumerations of properties/items. (can be null)</param>
        /// <param name="epmSourcePathSegment">The EPM source path segment which points to the property we're writing. (can be null)</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        /// <returns>true if anything was written, false otherwise.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Nothing to decouple.")]
        internal static bool WriteComplexValue(
            XmlWriter writer,
            IEdmModel model,
            ODataComplexValue complexValue,
            IEdmTypeReference metadataTypeReference,
            bool isOpenPropertyType,
            bool isWritingCollection,
            bool isWritingResponse,
            ODataWriterBehavior writerBehavior,
            Action<XmlWriter> beforeValueAction,
            Action<XmlWriter> afterValueAction,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            string multiValueItemTypeName,
            ODataVersion version,
            EpmValueCache epmValueCache,
            EpmSourcePathSegment epmSourcePathSegment,
            ProjectedPropertiesAnnotation projectedProperties)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(complexValue != null, "complexValue != null");
            Debug.Assert(writerBehavior != null, "writerBehavior != null");

            string typeName = complexValue.TypeName;

            // resolve the type name to the type; if no type name is specified we will use the 
            // type inferred from metadata
            IEdmComplexTypeReference complexTypeReference = 
                WriterValidationUtils.ResolveTypeNameForWriting(model, metadataTypeReference, ref typeName, EdmTypeKind.Complex, isOpenPropertyType).AsComplexOrNull();

            // If the type is the same as the one specified by the parent multivalue, omit the type name, since it's not needed.
            if (typeName != null && string.CompareOrdinal(multiValueItemTypeName, typeName) == 0)
            {
                typeName = null;
            }

            SerializationTypeNameAnnotation serializationTypeNameAnnotation = complexValue.GetAnnotation<SerializationTypeNameAnnotation>();
            if (serializationTypeNameAnnotation != null)
            {
                typeName = serializationTypeNameAnnotation.TypeName;
            }

            Action<XmlWriter> beforeValueCallbackWithTypeName = beforeValueAction;
            if (typeName != null)
            {
                // The beforeValueAction (if specified) will write the actual property element start.
                // So if we are to write the type attribute, we must postpone that after the start element was written.
                // And so we chain the existing action with our type attribute writing and use that
                // as the before action instead.
                if (beforeValueAction != null)
                {
                    beforeValueCallbackWithTypeName = (w) =>
                        {
                            beforeValueAction(w);
                            WritePropertyTypeAttribute(w, typeName);
                        };
                }
                else
                {
                    WritePropertyTypeAttribute(writer, typeName);
                }
            }

            // NOTE: see the comments on ODataWriterBehavior.UseV1ProviderBehavior for more information
            // NOTE: We have to check for ProjectedPropertiesAnnotation.Empty here to avoid filling the cache for
            //       complex values we are writing only to ensure we don't have nested EPM-mapped null values 
            //       that will end up in the content eventually.
            if (writerBehavior != null && 
                writerBehavior.UseV1ProviderBehavior && 
                !object.ReferenceEquals(projectedProperties, ProjectedPropertiesAnnotation.EmptyProjectedPropertiesMarker))
            {
                IEdmComplexType complexType = (IEdmComplexType)complexTypeReference.Definition;
                CachedPrimitiveKeepInContentAnnotation keepInContentCache = complexType.EpmCachedKeepPrimitiveInContent();
                if (keepInContentCache == null)
                {
                    // we are about to write the first value of the given type; compute the keep-in-content information for the primitive properties of this type.
                    List<string> keepInContentPrimitiveProperties = null;

                    // initialize the cache with all primitive properties
                    foreach (IEdmProperty edmProperty in complexType.Properties().Where(p => p.Type.IsODataPrimitiveTypeKind()))
                    {
                        // figure out the keep-in-content value
                        EntityPropertyMappingAttribute entityPropertyMapping = EpmWriterUtils.GetEntityPropertyMapping(epmSourcePathSegment, edmProperty.Name);
                        if (entityPropertyMapping != null && entityPropertyMapping.KeepInContent)
                        {
                            if (keepInContentPrimitiveProperties == null)
                            {
                                keepInContentPrimitiveProperties = new List<string>();
                            }

                            keepInContentPrimitiveProperties.Add(edmProperty.Name);
                        }
                    }

                    complexType.SetAnnotation<CachedPrimitiveKeepInContentAnnotation>(new CachedPrimitiveKeepInContentAnnotation(keepInContentPrimitiveProperties));
                }
            }

            return WriteProperties(
                writer,
                model,
                complexTypeReference == null ? null : complexTypeReference.ComplexDefinition(),
                EpmValueCache.GetComplexValueProperties(epmValueCache, complexValue, true),
                version,
                isWritingCollection,
                isWritingResponse,
                writerBehavior,
                beforeValueCallbackWithTypeName,
                afterValueAction,
                duplicatePropertyNamesChecker,
                epmValueCache,
                epmSourcePathSegment,
                projectedProperties);
        }

        /// <summary>
        /// Writes a single Uri in response to a $links query.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="baseUri">The base Uri used for writing.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        /// <param name="link">The entity reference link to write out.</param>
        /// <param name="isTopLevel">
        /// A flag indicating whether the link is written as top-level element or not; 
        /// this controls whether to include namespace declarations etc.
        /// </param>
        internal static void WriteEntityReferenceLink(
            XmlWriter writer,
            Uri baseUri,
            IODataUrlResolver urlResolver,
            ODataWriterBehavior writerBehavior,
            ODataEntityReferenceLink link,
            bool isTopLevel)
        {
            DebugUtils.CheckNoExternalCallers();

            // For backward compatibility with WCF DS, the client needs to write the to-level uri element in the metadata namespace.
            bool useMetadataNamespace = (writerBehavior.BehaviorKind == ODataBehaviorKind.WcfDataServicesClient) && isTopLevel;
            
            // <uri ...
            writer.WriteStartElement(string.Empty, AtomConstants.ODataUriElementName, useMetadataNamespace ? AtomConstants.ODataMetadataNamespace : AtomConstants.ODataNamespace);

            if (isTopLevel)
            {
                // xmlns=
                writer.WriteAttributeString(AtomConstants.XmlnsNamespacePrefix, useMetadataNamespace ? AtomConstants.ODataMetadataNamespace : AtomConstants.ODataNamespace);
            }

            writer.WriteString(link.Url.ToUrlAttributeValue(baseUri, urlResolver));

            // </uri>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a set of links (Uris) in response to a $links query; includes optional count and next-page-link information.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="baseUri">The base Uri used for writing.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        /// <param name="links">The entity reference links to write.</param>
        internal static void WriteEntityReferenceLinks(
            XmlWriter writer,
            Uri baseUri,
            IODataUrlResolver urlResolver,
            ODataWriterBehavior writerBehavior,
            ODataEntityReferenceLinks links)
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

            IEnumerable<ODataEntityReferenceLink> entityReferenceLinks = links.Links;
            if (entityReferenceLinks != null)
            {
                foreach (ODataEntityReferenceLink link in entityReferenceLinks)
                {
                    WriteEntityReferenceLink(writer, baseUri, urlResolver, writerBehavior, link, false);
                }
            }

            if (links.NextLink != null)
            {
                // <d:next>
                string nextLink = links.NextLink.ToUrlAttributeValue(baseUri, urlResolver);
                writer.WriteElementString(string.Empty, AtomConstants.ODataNextLinkElementName, AtomConstants.ODataNamespace, nextLink);
            }

            // </links>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a primitive value.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="expectedTypeReference">The expected type of the primitive value.</param>
        internal static void WritePrimitiveValue(XmlWriter writer, object value, IEdmTypeReference expectedTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(value != null, "value != null");

            string typeName;
            if (!EdmLibraryExtensions.TryGetPrimitiveTypeName(value, out typeName))
            {
                throw new ODataException(Strings.ValidationUtils_UnsupportedPrimitiveType(value.GetType().FullName));
            }

            if (typeName != Metadata.EdmConstants.EdmStringTypeName)
            {
                WritePropertyTypeAttribute(writer, typeName);
            }

            AtomValueUtils.WritePrimitiveValue(writer, value, expectedTypeReference);
        }

        /// <summary>
        /// Writes a service document in ATOM/XML format.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="defaultWorkspace">The default workspace to write in the service document.</param>
        /// <param name="baseUri">The base Uri specified in the message writer settings for writing the service document.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="writerBehavior">The writer behavior to use.</param>
        internal static void WriteServiceDocument(
            XmlWriter writer,
            ODataWorkspace defaultWorkspace,
            Uri baseUri,
            IODataUrlResolver urlResolver,
            ODataWriterBehavior writerBehavior)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(defaultWorkspace != null, "defaultWorkspace != null");

            IEnumerable<ODataResourceCollectionInfo> collections = WriterValidationUtils.ValidateWorkspace(defaultWorkspace);

            // <app:service>
            writer.WriteStartElement(string.Empty, AtomConstants.AtomPublishingServiceElementName, AtomConstants.AtomPublishingNamespace);

            // xml:base=...
            if (baseUri != null)
            {
                writer.WriteAttributeString(AtomConstants.XmlBaseAttributeName, AtomConstants.XmlNamespace, baseUri.AbsoluteUri);
            }

            // xmlns:atom="http://www.w3.org/2005/Atom"
            writer.WriteAttributeString(
                AtomConstants.NonEmptyAtomNamespacePrefix,
                AtomConstants.XmlNamespacesNamespace,
                AtomConstants.AtomNamespace);

            // xmlns=http://www.w3.org/2007/app
            writer.WriteAttributeString(AtomConstants.XmlnsNamespacePrefix, AtomConstants.XmlNamespacesNamespace, AtomConstants.AtomPublishingNamespace);

            // <app:workspace>
            writer.WriteStartElement(string.Empty, AtomConstants.AtomPublishingWorkspaceElementName, AtomConstants.AtomPublishingNamespace);

            ODataAtomWriterMetadataUtils.WriteWorkspaceMetadata(writer, writerBehavior, defaultWorkspace);

            if (collections != null)
            {
                foreach (ODataResourceCollectionInfo collectionInfo in collections)
                {
                    // <app:collection>
                    writer.WriteStartElement(string.Empty, AtomConstants.AtomPublishingCollectionElementName, AtomConstants.AtomPublishingNamespace);

                    // The name of the collection is the entity set name; The href of the <app:collection> element must be the link for the entity set.
                    // Since we model the collection as having a 'Name' (for JSON) we require a base Uri for Atom/Xml.
                    writer.WriteAttributeString(AtomConstants.AtomHRefAttributeName, collectionInfo.Url.ToUrlAttributeValue(baseUri, urlResolver));
                    
                    ODataAtomWriterMetadataUtils.WriteCollectionMetadata(writer, baseUri, urlResolver, writerBehavior, collectionInfo);

                    // </app:collection>
                    writer.WriteEndElement();
                }
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

            if ((flags & DefaultNamespaceFlags.GeoRss) == DefaultNamespaceFlags.GeoRss)
            {
                writer.WriteAttributeString(
                    AtomConstants.GeoRssPrefix,
                    AtomConstants.XmlNamespacesNamespace,
                    AtomConstants.GeoRssNamespace);
            }

            if ((flags & DefaultNamespaceFlags.Gml) == DefaultNamespaceFlags.Gml)
            {
                writer.WriteAttributeString(
                    AtomConstants.GmlPrefix,
                    AtomConstants.XmlNamespacesNamespace,
                    AtomConstants.GmlNamespace);
            }
        }

        /// <summary>
        /// Write the items in a MultiValue in ATOM format.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="model">The model to use or null if no metadata is available.</param>
        /// <param name="multiValue">The MultiValue to write.</param>
        /// <param name="propertyTypeReference">The type reference of the multi value (or null if not metadata is available).</param>
        /// <param name="isOpenPropertyType">true if the type name belongs to an open property.</param>
        /// <param name="isWritingCollection">true if we are writing a collection instead of an entry.</param>
        /// <param name="isWritingResponse">true if we are writing a response; false if it's a request.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        /// <param name="version">The protocol version used for writing.</param>
        /// <param name="epmValueCache">Cache of values used in EPM so that we avoid multiple enumerations of properties/items. (can be null)</param>
        /// <param name="epmSourcePathSegment">The EPM source path segment which points to the multivalue property we're writing. (can be null)</param>
        private static void WriteMultiValue(
            XmlWriter writer,
            IEdmModel model,
            ODataMultiValue multiValue, 
            IEdmTypeReference propertyTypeReference,
            bool isOpenPropertyType,
            bool isWritingCollection,
            bool isWritingResponse,
            ODataWriterBehavior writerBehavior,
            ODataVersion version,
            EpmValueCache epmValueCache,
            EpmSourcePathSegment epmSourcePathSegment)
        {
            Debug.Assert(multiValue != null, "multiValue != null");
            Debug.Assert(writerBehavior != null, "writerBehavior != null");

            string typeName = multiValue.TypeName;

            // resolve the type name to the type; if no type name is specified we will use the 
            // type inferred from metadata
            IEdmCollectionTypeReference multiValueTypeReference =
                (IEdmCollectionTypeReference)WriterValidationUtils.ResolveTypeNameForWriting(model, propertyTypeReference, ref typeName, EdmTypeKind.Collection, isOpenPropertyType);

            string multiValueItemTypeName = null;
            if (typeName != null)
            {
                multiValueItemTypeName = ValidationUtils.ValidateMultiValueTypeName(typeName);
            }

            SerializationTypeNameAnnotation serializationTypeNameAnnotation = multiValue.GetAnnotation<SerializationTypeNameAnnotation>();
            if (serializationTypeNameAnnotation != null)
            {
                typeName = serializationTypeNameAnnotation.TypeName;
            }

            if (typeName != null)
            {
                WritePropertyTypeAttribute(writer, typeName);
            }

            IEdmTypeReference expectedItemTypeReference = multiValueTypeReference == null ? null : multiValueTypeReference.ElementType();

            IEnumerable items = EpmValueCache.GetMultiValueItems(epmValueCache, multiValue, true);
            if (items != null)
            {
                DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = null;
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
                        if (duplicatePropertyNamesChecker == null)
                        {
                            duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(writerBehavior.AllowDuplicatePropertyNames, isWritingResponse);
                        }

                        WriteComplexValue(
                            writer,
                            model,
                            complexValue,
                            expectedItemTypeReference,
                            false,
                            isWritingCollection,
                            isWritingResponse,
                            writerBehavior,
                            null /* beforeValueAction */,
                            null /* afterValueAction */,
                            duplicatePropertyNamesChecker,
                            multiValueItemTypeName,
                            version,
                            epmItemCache,
                            epmSourcePathSegment,
                            null /* projectedProperties */);

                        duplicatePropertyNamesChecker.Clear();
                    }
                    else
                    {
                        Debug.Assert(!(item is ODataMultiValue), "!(item is ODataMultiValue)");
                        Debug.Assert(!(item is ODataStreamReferenceValue), "!(item is ODataStreamReferenceValue)");
                        AtomValueUtils.WritePrimitiveValue(writer, item, expectedItemTypeReference);
                    }

                    writer.WriteEndElement();
                }
            }
        }

        /// <summary>
        /// Writes the property start element.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="beforePropertyCallback">Action called before anything else is written (if it's not null).</param>
        /// <param name="propertyName">The name of the property to write.</param>
        /// <param name="isWritingCollection">true if we are writing a collection instead of an entry.</param>
        /// <param name="isTopLevel">true if writing a top-level property payload; otherwise false.</param>
        private static void WritePropertyStart(XmlWriter writer, Action<XmlWriter> beforePropertyCallback, string propertyName, bool isWritingCollection, bool isTopLevel)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            if (beforePropertyCallback != null)
            {
                beforePropertyCallback(writer);
            }

            // <d:propertyname>
            writer.WriteStartElement(
                isWritingCollection ? string.Empty : AtomConstants.ODataNamespacePrefix,
                propertyName,
                AtomConstants.ODataNamespace);

            if (isTopLevel)
            {
                WriteDefaultNamespaceAttributes(writer, DefaultNamespaceFlags.OData | DefaultNamespaceFlags.ODataMetadata);
            }
        }

        /// <summary>
        /// Writes the property end element.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        private static void WritePropertyEnd(XmlWriter writer)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            // </d:propertyname>
            writer.WriteEndElement();
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
        /// <param name="owningType">The owning type of the property to be checked.</param>
        /// <param name="projectedProperties">The set of projected properties for the <paramref name="owningType"/></param>
        /// <param name="propertyName">The name of the property to be checked.</param>
        /// <param name="propertyValue">The property value to write.</param>
        /// <param name="epmSourcePathSegment">The EPM source path segment for the property being written.</param>
        /// <param name="version">The version of the protocol being used for the response.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        /// <returns>true if the property should be written into content, or false otherwise</returns>
        private static bool ShouldWritePropertyInContent(
            IEdmStructuredType owningType,
            ProjectedPropertiesAnnotation projectedProperties,
            string propertyName,
            object propertyValue,
            EpmSourcePathSegment epmSourcePathSegment,
            ODataVersion version,
            ODataWriterBehavior writerBehavior)
        {
            Debug.Assert(writerBehavior != null, "writerBehavior != null");

            // check whether the property is projected; if no EPM is specified for the property the projection decides 
            bool propertyProjected = !projectedProperties.ShouldSkipProperty(propertyName);

            bool useV1ProviderBehavior = writerBehavior == null ? false : writerBehavior.UseV1ProviderBehavior;
            if (useV1ProviderBehavior && owningType != null && owningType.IsODataComplexTypeKind())
            {
                IEdmComplexType owningComplexType = (IEdmComplexType)owningType;
                CachedPrimitiveKeepInContentAnnotation keepInContentAnnotation = owningComplexType.EpmCachedKeepPrimitiveInContent();
                if (keepInContentAnnotation != null && keepInContentAnnotation.IsKeptInContent(propertyName))
                {
                    return propertyProjected;
                }
            }

            EntityPropertyMappingAttribute entityPropertyMapping = EpmWriterUtils.GetEntityPropertyMapping(epmSourcePathSegment);
            if (entityPropertyMapping == null)
            {
                return propertyProjected;
            }

            if (version <= ODataVersion.V2)
            {
                // In V2 and lower we sometimes write properties into content even if asked not to.
                // If the property value is null, we always write into content, even if the property was not projected.
                if (propertyValue == null)
                {
                    return true;
                }

                string stringPropertyValue = propertyValue as string;
                if (stringPropertyValue != null && stringPropertyValue.Length == 0)
                {
                    // If the property value is an empty string and we should be writing it into an ATOM element which does not allow empty string
                    // we write it into content as well, also even if the property was not projected.
                    switch (entityPropertyMapping.TargetSyndicationItem)
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

            return entityPropertyMapping.KeepInContent && propertyProjected;
        }
    }
}
