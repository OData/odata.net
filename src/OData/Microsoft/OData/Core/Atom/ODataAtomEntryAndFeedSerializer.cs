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
    using System.Linq;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// OData ATOM serializer for entries and feeds.
    /// </summary>
    internal sealed class ODataAtomEntryAndFeedSerializer : ODataAtomPropertyAndValueSerializer
    {
        /// <summary>The context uri builder to use.</summary>
        private readonly ODataContextUriBuilder contextUriBuilder;

        /// <summary>
        /// The serializer for writing ATOM metadata for entries.
        /// </summary>
        private readonly ODataAtomEntryMetadataSerializer atomEntryMetadataSerializer;

        /// <summary>
        /// The serializer for writing ATOM metadata for feeds.
        /// </summary>
        private readonly ODataAtomFeedMetadataSerializer atomFeedMetadataSerializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomOutputContext">The output context to write to.</param>
        internal ODataAtomEntryAndFeedSerializer(ODataAtomOutputContext atomOutputContext)
            : base(atomOutputContext)
        {
            this.atomEntryMetadataSerializer = new ODataAtomEntryMetadataSerializer(atomOutputContext);
            this.atomFeedMetadataSerializer = new ODataAtomFeedMetadataSerializer(atomOutputContext);

            // DEVNOTE: grab this early so that any validation errors are thrown at creation time rather than when Write___ is called.
            this.contextUriBuilder = atomOutputContext.CreateContextUriBuilder();
        }

        /// <summary>
        /// Writes the start element for the m:properties element on the entry.
        /// </summary>
        internal void WriteEntryPropertiesStart()
        {
            // <m:properties> if required
            this.XmlWriter.WriteStartElement(
                AtomConstants.ODataMetadataNamespacePrefix,
                AtomConstants.AtomPropertiesElementName,
                AtomConstants.ODataMetadataNamespace);
        }

        /// <summary>
        /// Writes the end element for the m:properties element on the entry.
        /// </summary>
        internal void WriteEntryPropertiesEnd()
        {
            // </m:properties>
            this.XmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes the type name category element for the entry.
        /// </summary>
        /// <param name="typeName">The type name to write.</param>
        /// <param name="entryMetadata">The entry metadata if available.</param>
        internal void WriteEntryTypeName(string typeName, AtomEntryMetadata entryMetadata)
        {
            if (typeName != null)
            {
                AtomCategoryMetadata mergedCategoryMetadata = ODataAtomWriterMetadataUtils.MergeCategoryMetadata(
                    entryMetadata == null ? null : entryMetadata.CategoryWithTypeName,
                    typeName,
                    AtomConstants.ODataSchemeNamespace);
                this.atomEntryMetadataSerializer.WriteCategory(mergedCategoryMetadata);
            }
        }

        /// <summary>
        /// Write the ATOM metadata for an entry
        /// </summary>
        /// <param name="entryMetadata">The entry metadata to write.</param>
        /// <param name="updatedTime">Value for the atom:updated element.</param>
        internal void WriteEntryMetadata(AtomEntryMetadata entryMetadata, string updatedTime)
        {
            this.atomEntryMetadataSerializer.WriteEntryMetadata(entryMetadata, updatedTime);
        }

        /// <summary>
        /// Writes the entry atom:id element.
        /// </summary>
        /// <param name="entryId">The value of the ODataEntry.Id property to write.</param>
        /// <param name="isTransient">If the entry is a transient entry</param>
        internal void WriteEntryId(Uri entryId, bool isTransient)
        {
            string entryIdPlainText;

            if (isTransient)
            {
                entryIdPlainText = AtomUtils.GetTransientId();
            }
            else
            {
                entryIdPlainText = entryId == null ? null : UriUtils.UriToString(entryId);
            }

            // <atom:id>idValue</atom:id>
            // NOTE: do not generate a relative Uri for the ID; it is independent of xml:base
            this.WriteElementWithTextContent(
                AtomConstants.AtomNamespacePrefix,
                AtomConstants.AtomIdElementName,
                AtomConstants.AtomNamespace,
                entryIdPlainText);
        }

        /// <summary>
        /// Writes the read link element for an entry.
        /// </summary>
        /// <param name="readLink">The read link URL.</param>
        /// <param name="entryMetadata">The ATOM entry metatadata for the current entry.</param>
        internal void WriteEntryReadLink(Uri readLink, AtomEntryMetadata entryMetadata)
        {
            Debug.Assert(readLink != null, "readLink != null");

            // we allow additional link metadata to specify the title, type, hreflang or length of the link
            AtomLinkMetadata readLinkMetadata = entryMetadata == null ? null : entryMetadata.SelfLink;

            // <link rel="self" href="LinkHRef" ... />
            this.WriteReadOrEditLink(readLink, readLinkMetadata, AtomConstants.AtomSelfRelationAttributeValue);
        }

        /// <summary>
        /// Writes the edit link element for an entry.
        /// </summary>
        /// <param name="editLink">The edit link URL.</param>
        /// <param name="entryMetadata">The ATOM entry metatadata for the current entry.</param>
        internal void WriteEntryEditLink(Uri editLink, AtomEntryMetadata entryMetadata)
        {
            Debug.Assert(editLink != null, "editLink != null");

            // we allow additional link metadata to specify the title, type, hreflang or length of the link
            AtomLinkMetadata editLinkMetadata = entryMetadata == null ? null : entryMetadata.EditLink;

            // <link rel="edit" href="LinkHRef" .../>
            this.WriteReadOrEditLink(editLink, editLinkMetadata, AtomConstants.AtomEditRelationAttributeValue);
        }

        /// <summary>
        /// Writes the edit-media link for an entry.
        /// </summary>
        /// <param name="mediaResource">The media resource representing the MR of the entry to write.</param>
        internal void WriteEntryMediaEditLink(ODataStreamReferenceValue mediaResource)
        {
            Debug.Assert(mediaResource != null, "mediaResource != null");

            Uri mediaEditLink = mediaResource.EditLink;
            Debug.Assert(mediaEditLink != null || mediaResource.ETag == null, "The default stream edit link and etag should have been validated by now.");
            if (mediaEditLink != null)
            {
                AtomStreamReferenceMetadata streamReferenceMetadata = mediaResource.GetAnnotation<AtomStreamReferenceMetadata>();
                AtomLinkMetadata mediaEditMetadata = streamReferenceMetadata == null ? null : streamReferenceMetadata.EditLink;
                AtomLinkMetadata mergedLinkMetadata =
                    ODataAtomWriterMetadataUtils.MergeLinkMetadata(
                        mediaEditMetadata,
                        AtomConstants.AtomEditMediaRelationAttributeValue,
                        mediaEditLink,
                        null /* title */,
                        null /* mediaType */);

                this.atomEntryMetadataSerializer.WriteAtomLink(mergedLinkMetadata, mediaResource.ETag);
            }
        }

        /// <summary>
        /// Write the metadata for an OData association link; makes sure any duplicate of the link's values duplicated in metadata are equal.
        /// </summary>
        /// <param name="navigationPropertyName">The name of the navigation property whose association link is being written.</param>
        /// <param name="associationLinkUrl">The association link url to write.</param>
        /// <param name="associationLinkMetadata">Atom metadata about this link element. This can be used to customized the link element with additional XML attributes.</param>
        internal void WriteAssociationLink(string navigationPropertyName, Uri associationLinkUrl, AtomLinkMetadata associationLinkMetadata)
        {
            AtomLinkMetadata linkMetadata = associationLinkMetadata;
            string linkRelation = AtomUtils.ComputeODataAssociationLinkRelation(navigationPropertyName);
            AtomLinkMetadata mergedLinkMetadata = ODataAtomWriterMetadataUtils.MergeLinkMetadata(linkMetadata, linkRelation, associationLinkUrl, navigationPropertyName, MimeConstants.MimeApplicationXml);
            this.atomEntryMetadataSerializer.WriteAtomLink(mergedLinkMetadata, null /* etag*/);
        }

        /// <summary>
        /// Writes the navigation link's start element and atom metadata.
        /// </summary>
        /// <param name="navigationLink">The navigation link to write.</param>
        /// <param name="navigationLinkUrlOverride">Url to use for the navigation link. If this is specified the Url property on the <paramref name="navigationLink"/>
        /// will be ignored. If this parameter is null, the Url from the navigation link is used.</param>
        internal void WriteNavigationLinkStart(ODataNavigationLink navigationLink, Uri navigationLinkUrlOverride)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");
            Debug.Assert(!string.IsNullOrEmpty(navigationLink.Name), "The navigation link name was not verified yet.");
            Debug.Assert(navigationLink.Url != null, "The navigation link Url was not verified yet.");
            Debug.Assert(navigationLink.IsCollection.HasValue, "navigationLink.IsCollection.HasValue");

            if (navigationLink.AssociationLinkUrl != null)
            {
                // TODO:Association Link - Add back support for customizing association link element in Atom
                this.WriteAssociationLink(navigationLink.Name, navigationLink.AssociationLinkUrl, null);
            }

            // <atom:link>
            this.XmlWriter.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomLinkElementName, AtomConstants.AtomNamespace);

            string linkRelation = AtomUtils.ComputeODataNavigationLinkRelation(navigationLink);
            string linkType = AtomUtils.ComputeODataNavigationLinkType(navigationLink);
            string linkTitle = navigationLink.Name;

            Uri navigationLinkUrl = navigationLinkUrlOverride ?? navigationLink.Url;
            AtomLinkMetadata linkMetadata = navigationLink.GetAnnotation<AtomLinkMetadata>();
            AtomLinkMetadata mergedMetadata = ODataAtomWriterMetadataUtils.MergeLinkMetadata(linkMetadata, linkRelation, navigationLinkUrl, linkTitle, linkType);
            this.atomEntryMetadataSerializer.WriteAtomLinkAttributes(mergedMetadata, null /* etag */);
        }

        /// <summary>
        /// Write the given feed metadata in atom format
        /// </summary>
        /// <param name="feed">The feed for which to write the meadata or null if it is the metadata of an atom:source element.</param>
        /// <param name="updatedTime">Value for the atom:updated element.</param>
        /// <param name="authorWritten">Set to true if the author element was written, false otherwise.</param>
        internal void WriteFeedMetadata(ODataFeed feed, string updatedTime, out bool authorWritten)
        {
            Debug.Assert(feed != null, "feed != null");
            Debug.Assert(!string.IsNullOrEmpty(updatedTime), "!string.IsNullOrEmpty(updatedTime)");
#if DEBUG
            DateTimeOffset tempDateTimeOffset;
            Debug.Assert(DateTimeOffset.TryParse(updatedTime, out tempDateTimeOffset), "DateTimeOffset.TryParse(updatedTime, out tempDateTimeOffset)");
#endif

            AtomFeedMetadata feedMetadata = feed.GetAnnotation<AtomFeedMetadata>();

            if (feedMetadata == null)
            {
                // create the required metadata elements with default values.

                // <atom:id>idValue</atom:id>
                Debug.Assert(feed.Id != null, "The feed Id should have been validated by now.");
                this.WriteElementWithTextContent(
                    AtomConstants.AtomNamespacePrefix,
                    AtomConstants.AtomIdElementName,
                    AtomConstants.AtomNamespace,
                    UriUtils.UriToString(feed.Id));

                // <atom:title></atom:title>
                this.WriteEmptyElement(
                    AtomConstants.AtomNamespacePrefix,
                    AtomConstants.AtomTitleElementName,
                    AtomConstants.AtomNamespace);

                // <atom:updated>dateTimeOffset</atom:updated>
                this.WriteElementWithTextContent(
                    AtomConstants.AtomNamespacePrefix,
                    AtomConstants.AtomUpdatedElementName,
                    AtomConstants.AtomNamespace,
                    updatedTime);

                authorWritten = false;
            }
            else
            {
                this.atomFeedMetadataSerializer.WriteFeedMetadata(feedMetadata, feed, updatedTime, out authorWritten);
            }
        }

        /// <summary>
        /// Writes the default empty author for a feed.
        /// </summary>
        internal void WriteFeedDefaultAuthor()
        {
            this.atomFeedMetadataSerializer.WriteEmptyAuthor();
        }

        /// <summary>
        /// Writes the next page link for a feed.
        /// </summary>
        /// <param name="feed">The feed to write the next page link for.</param>
        internal void WriteFeedNextPageLink(ODataFeed feed)
        {
            Debug.Assert(feed != null, "feed != null");

            Uri nextPageLink = feed.NextPageLink;
            if (nextPageLink != null)
            {
                // <atom:link rel="next" href="next-page-link" />
                this.WriteFeedLink(
                    feed,
                    AtomConstants.AtomNextRelationAttributeValue,
                    nextPageLink, 
                    (feedMetadata) => feedMetadata == null ? null : feedMetadata.NextPageLink);
            }
        }

        /// <summary>
        /// Writes the delta link for a feed.
        /// </summary>
        /// <param name="feed">The feed to write the delta link for.</param>
        internal void WriteFeedDeltaLink(ODataFeed feed)
        {
            Debug.Assert(feed != null, "feed != null");

            Uri deltaLink = feed.DeltaLink;
            if (deltaLink != null)
            {
                // <atom:link rel="http://docs.oasis-open.org/odata/ns/delta" href="delta-link" />
                this.WriteFeedLink(
                    feed, 
                    AtomConstants.AtomDeltaRelationAttributeValue, 
                    deltaLink, 
                    (feedMetadata) => feedMetadata == null ? null : feedMetadata.Links.FirstOrDefault(link => link.Relation == AtomConstants.AtomDeltaRelationAttributeValue));
            }
        }

        /// <summary>
        /// Writes a feed link.
        /// </summary>
        /// <param name="feed">The feed that contains the link.</param>
        /// <param name="relation">Relation attribute of the link.</param>
        /// <param name="href">href attribute of the link.</param>
        /// <param name="getLinkMetadata">Function to get the AtomLinkMetadata for the feed link.</param>
        internal void WriteFeedLink(ODataFeed feed, string relation, Uri href, Func<AtomFeedMetadata, AtomLinkMetadata> getLinkMetadata)
        {
            AtomFeedMetadata feedMetadata = feed.GetAnnotation<AtomFeedMetadata>();
            AtomLinkMetadata mergedLink = ODataAtomWriterMetadataUtils.MergeLinkMetadata(
                    getLinkMetadata(feedMetadata),
                    relation,
                    href,
                    null, /*title*/
                    null /*mediaType*/);
            this.atomFeedMetadataSerializer.WriteAtomLink(mergedLink, null);
        }

        /// <summary>
        /// Writes a stream property to the ATOM payload
        /// </summary>
        /// <param name="streamProperty">The stream property to create the payload for.</param>
        /// <param name="owningType">The <see cref="IEdmEntityType"/> instance for which the stream property defined on.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        internal void WriteStreamProperty(
            ODataProperty streamProperty,
            IEdmEntityType owningType,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            ProjectedPropertiesAnnotation projectedProperties)
        {
            Debug.Assert(streamProperty != null, "Stream property must not be null.");
            Debug.Assert(streamProperty.Value != null, "The media resource of the stream property must not be null.");

            WriterValidationUtils.ValidatePropertyNotNull(streamProperty);
            string propertyName = streamProperty.Name;
            if (projectedProperties.ShouldSkipProperty(propertyName))
            {
                return;
            }

            WriterValidationUtils.ValidatePropertyName(propertyName);
            duplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(streamProperty);
            IEdmProperty edmProperty = WriterValidationUtils.ValidatePropertyDefined(streamProperty.Name, owningType);
            WriterValidationUtils.ValidateStreamReferenceProperty(streamProperty, edmProperty, this.Version, this.WritingResponse);
            ODataStreamReferenceValue streamReferenceValue = (ODataStreamReferenceValue)streamProperty.Value;
            WriterValidationUtils.ValidateStreamReferenceValue(streamReferenceValue, false /*isDefaultStream*/);
            if (owningType != null && owningType.IsOpen && edmProperty == null)
            {
                ValidationUtils.ValidateOpenPropertyValue(streamProperty.Name, streamReferenceValue);
            }

            AtomStreamReferenceMetadata streamReferenceMetadata = streamReferenceValue.GetAnnotation<AtomStreamReferenceMetadata>();
            string contentType = streamReferenceValue.ContentType;
            string linkTitle = streamProperty.Name;

            Uri readLink = streamReferenceValue.ReadLink;
            if (readLink != null)
            {
                string readLinkRelation = AtomUtils.ComputeStreamPropertyRelation(streamProperty, false);

                AtomLinkMetadata readLinkMetadata = streamReferenceMetadata == null ? null : streamReferenceMetadata.SelfLink;
                AtomLinkMetadata mergedMetadata = ODataAtomWriterMetadataUtils.MergeLinkMetadata(readLinkMetadata, readLinkRelation, readLink, linkTitle, contentType);
                this.atomEntryMetadataSerializer.WriteAtomLink(mergedMetadata, null /* etag */);
            }

            Uri editLink = streamReferenceValue.EditLink;
            if (editLink != null)
            {
                string editLinkRelation = AtomUtils.ComputeStreamPropertyRelation(streamProperty, true);

                AtomLinkMetadata editLinkMetadata = streamReferenceMetadata == null ? null : streamReferenceMetadata.EditLink;
                AtomLinkMetadata mergedMetadata = ODataAtomWriterMetadataUtils.MergeLinkMetadata(editLinkMetadata, editLinkRelation, editLink, linkTitle, contentType);
                this.atomEntryMetadataSerializer.WriteAtomLink(mergedMetadata, streamReferenceValue.ETag);
            }
        }

        /// <summary>
        /// Writes an operation (an action or a function).
        /// </summary>
        /// <param name="operation">The association link to write.</param>
        internal void WriteOperation(ODataOperation operation)
        {
            // checks for null and validates its properties
            WriterValidationUtils.ValidateCanWriteOperation(operation, this.WritingResponse);
            ValidationUtils.ValidateOperationMetadataNotNull(operation);
            ValidationUtils.ValidateOperationTargetNotNull(operation);

            string elementName;
            if (operation is ODataAction)
            {
                elementName = AtomConstants.ODataActionElementName;
            }
            else
            {
                Debug.Assert(operation is ODataFunction, "operation is either an ODataAction or an ODataFunction");
                elementName = AtomConstants.ODataFunctionElementName;
            }

            // <m:action ... or <m:function ...
            this.XmlWriter.WriteStartElement(
                AtomConstants.ODataMetadataNamespacePrefix,
                elementName,
                AtomConstants.ODataMetadataNamespace);

            // write the attributes of the action/function

            // The metadata URI of an ODataOperation can be relative.
            string metadataAttributeValue = this.UriToUrlAttributeValue(operation.Metadata, /*failOnRelativeUriWithoutBaseUri*/ false);

            this.XmlWriter.WriteAttributeString(AtomConstants.ODataOperationMetadataAttribute, metadataAttributeValue);

            if (operation.Title != null)
            {
                this.XmlWriter.WriteAttributeString(AtomConstants.ODataOperationTitleAttribute, operation.Title);
            }

            string targetAttribute = this.UriToUrlAttributeValue(operation.Target);
            this.XmlWriter.WriteAttributeString(AtomConstants.ODataOperationTargetAttribute, targetAttribute);

            // </m:action> or </m:function>
            this.XmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Tries to writes the context URI property for an entry into the payload if one is available.
        /// </summary>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the entry.</param>
        internal void TryWriteEntryContextUri(ODataFeedAndEntryTypeContext typeContext)
        {
            ODataUri odataUri = this.AtomOutputContext.MessageWriterSettings.ODataUri;
            this.WriteContextUriProperty(this.contextUriBuilder.BuildContextUri(ODataPayloadKind.Entry, ODataContextUrlInfo.Create(typeContext, /* isSingle */ true, odataUri)));
        }

        /// <summary>
        /// Tries to writes the context URI property for a feed into the payload if one is available.
        /// </summary>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the feed.</param>
        internal void TryWriteFeedContextUri(ODataFeedAndEntryTypeContext typeContext)
        {
            ODataUri odataUri = this.AtomOutputContext.MessageWriterSettings.ODataUri;
            this.WriteContextUriProperty(this.contextUriBuilder.BuildContextUri(ODataPayloadKind.Feed, ODataContextUrlInfo.Create(typeContext, /* isSingle */ false, odataUri)));
        }

        /// <summary>
        /// Writes the self or edit link.
        /// </summary>
        /// <param name="link">Uri object for the link.</param>
        /// <param name="linkMetadata">The atom link metadata for the link to specify title, type, hreflang and length of the link.</param>
        /// <param name="linkRelation">Relationship value. Either "edit" or "self".</param>
        private void WriteReadOrEditLink(
            Uri link,
            AtomLinkMetadata linkMetadata,
            string linkRelation)
        {
            if (link != null)
            {
                AtomLinkMetadata mergedLinkMetadata = ODataAtomWriterMetadataUtils.MergeLinkMetadata(
                    linkMetadata,
                    linkRelation,
                    link,
                    null /* title */,
                    null /* media type */);

                this.atomEntryMetadataSerializer.WriteAtomLink(mergedLinkMetadata, null /* etag */);
            }
        }
    }
}
