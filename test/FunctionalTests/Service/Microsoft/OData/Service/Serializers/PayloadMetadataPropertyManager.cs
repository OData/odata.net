//---------------------------------------------------------------------
// <copyright file="PayloadMetadataPropertyManager.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// Component for controlling access to payload metadata properties which may be intentionally omitted from response payloads.
    /// Note: this is currently controlled via a query option, but could be extended further in the future.
    /// </summary>
    internal class PayloadMetadataPropertyManager
    {
        /// <summary>
        /// The payload metadta parameter interpreter to use when deciding whether to include certain payload metadata.
        /// </summary>
        private readonly PayloadMetadataParameterInterpreter interpreter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayloadMetadataPropertyManager"/> class.
        /// </summary>
        /// <param name="interpreter">The interpreter for the query option.</param>
        internal PayloadMetadataPropertyManager(PayloadMetadataParameterInterpreter interpreter)
        {
            Debug.Assert(interpreter != null, "interpreter != null");
            this.interpreter = interpreter;
        }

        /// <summary>
        /// Sets the entry's ETag property if it should be included according to the current query option.
        /// </summary>
        /// <param name="entry">The entry to modify.</param>
        /// <param name="computeETag">The callback to compute the ETag.</param>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDireclyAccessPayloadMetadataProperties", Justification = "This component is allowed to set these properties.")]
        internal void SetETag(ODataResource entry, Func<string> computeETag)
        {
            Debug.Assert(entry != null, "entry != null");
            if (this.interpreter.ShouldIncludeEntryMetadata(PayloadMetadataKind.Entry.ETag))
            {
                entry.ETag = computeETag();
            }
        }

        /// <summary>
        /// Sets the entry's TypeName property if it should be included according to the current query option.
        /// </summary>
        /// <param name="entry">The entry to modify.</param>
        /// <param name="entitySetBaseTypeName">Name of the entity set's base type.</param>
        /// <param name="entryTypeName">Name of the entry's type.</param>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDireclyAccessPayloadMetadataProperties", Justification = "This component is allowed to set these properties.")]
        internal void SetTypeName(ODataResource entry, string entitySetBaseTypeName, string entryTypeName)
        {
            Debug.Assert(entry != null, "entry != null");

            // We should always write this since for derived types, ODL needs to know the typename.
            entry.TypeName = entryTypeName;

            if (this.interpreter.ShouldIncludeEntryTypeName(entitySetBaseTypeName, entryTypeName))
            {
                entry.TypeAnnotation = new ODataTypeAnnotation(entry.TypeName);
            }
            else
            {
                // When we should not write the typename, setting the serialization type name to null
                // so that ODL does not write the type on the wire.
                entry.TypeAnnotation = new ODataTypeAnnotation();
            }
        }

        /// <summary>
        /// Sets the entry's TypeName property if it should be included according to the current query option.
        /// </summary>
        /// <param name="entry">The entry to modify.</param>
        /// <param name="entitySetBaseTypeName">Name of the entity set's base type.</param>
        /// <param name="entryTypeName">Name of the entry's type.</param>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDireclyAccessPayloadMetadataProperties", Justification = "This component is allowed to set these properties.")]
        internal void SetTypeName(ODataResourceSet resourceSet, string entitySetBaseTypeName, string resourceTypeName)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            // We should always write this since for derived types, ODL needs to know the typename.
            resourceSet.TypeName = resourceTypeName;

            if (this.interpreter.ShouldIncludeEntryTypeName(entitySetBaseTypeName, resourceTypeName))
            {
                resourceSet.TypeAnnotation = new ODataTypeAnnotation(resourceSet.TypeName);
            }
            else
            {
                // When we should not write the typename, setting the serialization type name to null
                // so that ODL does not write the type on the wire.
                resourceSet.TypeAnnotation = new ODataTypeAnnotation();
            }
        }

        /// <summary>
        /// Sets the entry's Id property if it should be included according to the current query option.
        /// </summary>
        /// <param name="entry">The entry to modify.</param>
        /// <param name="computeIdentity">The callback to compute the identity.</param>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDireclyAccessPayloadMetadataProperties", Justification = "This component is allowed to set these properties.")]
        internal void SetId(ODataResource entry, Func<Uri> computeIdentity)
        {
            Debug.Assert(entry != null, "entry != null");
            if (this.interpreter.ShouldIncludeEntryMetadata(PayloadMetadataKind.Entry.Id))
            {
                entry.Id = computeIdentity();
            }
        }

        /// <summary>
        /// Sets the entry's EditLink property if it should be included according to the current query option.
        /// </summary>
        /// <param name="entry">The entry to modify.</param>
        /// <param name="computeEditLink">The callback to compute the edit link.</param>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDireclyAccessPayloadMetadataProperties", Justification = "This component is allowed to set these properties.")]
        internal void SetEditLink(ODataResource entry, Func<Uri> computeEditLink)
        {
            Debug.Assert(entry != null, "entry != null");
            if (this.interpreter.ShouldIncludeEntryMetadata(PayloadMetadataKind.Entry.EditLink))
            {
                entry.EditLink = computeEditLink();
            }
        }

        /// <summary>
        /// Sets the feed's Id property if it should be included according to the current query option.
        /// </summary>
        /// <param name="feed">The feed to modify.</param>
        /// <param name="computeIdentity">The callback to compute the identity.</param>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDireclyAccessPayloadMetadataProperties", Justification = "This component is allowed to set these properties.")]
        internal void SetId(ODataResourceSet feed, Func<Uri> computeIdentity)
        {
            Debug.Assert(feed != null, "feed != null");
            if (this.interpreter.ShouldIncludeFeedMetadata(PayloadMetadataKind.Feed.Id))
            {
                feed.Id = computeIdentity();
            }
        }

        /// <summary>
        /// Sets the feed's NextPageLink property.
        /// </summary>
        /// <param name="feed">The feed to modify.</param>
        /// <param name="absoluteServiceUri">The absolute service Uri.</param>
        /// <param name="absoluteNextPageLinkUri">The absolute next link uri.</param>
        internal void SetNextPageLink(ODataResourceSet resourceCollection, Uri absoluteServiceUri, Uri absoluteNextPageLinkUri)
        {
            Debug.Assert(resourceCollection != null, "feed != null");
            Debug.Assert(absoluteServiceUri != null && absoluteServiceUri.IsAbsoluteUri, "absoluteServiceUri != null && absoluteServiceUri.IsAbsoluteUri");
            Debug.Assert(absoluteNextPageLinkUri != null && absoluteNextPageLinkUri.IsAbsoluteUri, "absoluteNextPageLinkUri != null && absoluteNextPageLinkUri.IsAbsoluteUri");
            Debug.Assert(
                UriUtil.UriToString(absoluteNextPageLinkUri).StartsWith(UriUtil.UriToString(absoluteServiceUri), StringComparison.OrdinalIgnoreCase),
                "UriUtil.UriToString(absoluteNextPageLinkUri).StartsWith(UriUtil.UriToString(absoluteServiceUri), StringComparison.OrdinalIgnoreCase)");

            if (this.interpreter.ShouldNextPageLinkBeAbsolute())
            {
                resourceCollection.NextPageLink = absoluteNextPageLinkUri;
            }
            else
            {
                resourceCollection.NextPageLink = absoluteServiceUri.MakeRelativeUri(absoluteNextPageLinkUri);
            }
        }

        /// <summary>
        /// Sets the stream's EditLink property if it should be included according to the current query option.
        /// </summary>
        /// <param name="stream">The stream to modify.</param>
        /// <param name="computeEditLink">The callback to compute the edit link.</param>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDireclyAccessPayloadMetadataProperties", Justification = "This component is allowed to set these properties.")]
        internal void SetEditLink(ODataStreamReferenceValue stream, Func<Uri> computeEditLink)
        {
            Debug.Assert(stream != null, "stream != null");
            if (this.interpreter.ShouldIncludeStreamMetadata(PayloadMetadataKind.Stream.EditLink))
            {
                stream.EditLink = computeEditLink();
            }
        }

        /// <summary>
        /// Sets the stream's ReadLink property if it should be included according to the current query option.
        /// </summary>
        /// <param name="stream">The stream to modify.</param>
        /// <param name="computeReadLink">The callback to compute the read link.</param>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDireclyAccessPayloadMetadataProperties", Justification = "This component is allowed to set these properties.")]
        internal void SetReadLink(ODataStreamReferenceValue stream, Func<Uri> computeReadLink)
        {
            Debug.Assert(stream != null, "stream != null");
            if (this.interpreter.ShouldIncludeStreamMetadata(PayloadMetadataKind.Stream.ReadLink))
            {
                stream.ReadLink = computeReadLink();
            }
        }

        /// <summary>
        /// Sets the stream's ContentType property if it should be included according to the current query option.
        /// </summary>
        /// <param name="stream">The stream to modify.</param>
        /// <param name="contentType">The stream's content type.</param>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDireclyAccessPayloadMetadataProperties", Justification = "This component is allowed to set these properties.")]
        internal void SetContentType(ODataStreamReferenceValue stream, string contentType)
        {
            Debug.Assert(stream != null, "stream != null");
            if (this.interpreter.ShouldIncludeStreamMetadata(PayloadMetadataKind.Stream.ContentType))
            {
                stream.ContentType = contentType;
            }
        }

        /// <summary>
        /// Sets the stream's ETag property if it should be included according to the current query option.
        /// </summary>
        /// <param name="stream">The stream to modify.</param>
        /// <param name="streamETag">The stream's ETag.</param>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDireclyAccessPayloadMetadataProperties", Justification = "This component is allowed to set these properties.")]
        internal void SetETag(ODataStreamReferenceValue stream, string streamETag)
        {
            Debug.Assert(stream != null, "stream != null");
            if (this.interpreter.ShouldIncludeStreamMetadata(PayloadMetadataKind.Stream.ETag))
            {
                stream.ETag = streamETag;
            }
        }

        /// <summary>
        /// Sets the link's Url property if it should be included according to the current query option.
        /// </summary>
        /// <param name="link">The link to modify.</param>
        /// <param name="computeUrl">The callback to compute the url.</param>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDireclyAccessPayloadMetadataProperties", Justification = "This component is allowed to set these properties.")]
        internal void SetUrl(ODataNestedResourceInfo link, Func<Uri> computeUrl)
        {
            Debug.Assert(link != null, "link != null");
            if (this.interpreter.ShouldIncludeNavigationMetadata(PayloadMetadataKind.Navigation.Url))
            {
                link.Url = computeUrl();
            }
        }

        /// <summary>
        /// Sets the link's AssociationLinkUrl property if it should be included according to the current query option.
        /// </summary>
        /// <param name="link">The link to modify.</param>
        /// <param name="computeUrl">The callback to compute the url.</param>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDireclyAccessPayloadMetadataProperties", Justification = "This component is allowed to set these properties.")]
        internal void SetAssociationLinkUrl(ODataNestedResourceInfo link, Func<Uri> computeUrl)
        {
            Debug.Assert(link != null, "link != null");
            if (this.interpreter.ShouldIncludeNavigationMetadata(PayloadMetadataKind.Navigation.AssociationLinkUrl))
            {
                link.AssociationLinkUrl = computeUrl();
            }
        }

        /// <summary>
        /// Sets the value's TypeName property if it should be included according to the current query option.
        /// </summary>
        /// <param name="value">The value to modify.</param>
        /// <param name="actualType">The type reference for the given value.</param>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDirectlyAccessPayloadMetadataProperties", Justification = "This component is allowed to set these properties.")]
        internal void SetTypeName(ODataValue value, ResourceType actualType)
        {
            Debug.Assert(value != null, "value != null");

#if DEBUG
            var collectionValue = value as ODataCollectionValue;
            Debug.Assert(collectionValue == null || !String.IsNullOrEmpty(collectionValue.TypeName), "Type name must be specified in ODataCollectionValue since ODL needs it for validation.");
#endif
            string typeNameToWrite;
            if (this.interpreter.ShouldSpecifyTypeNameAnnotation(value, actualType, out typeNameToWrite))
            {
                value.TypeAnnotation = new ODataTypeAnnotation(typeNameToWrite);
            }
        }

        /// <summary>
        /// Sets the action's Title property if it should be included according to the current query option.
        /// </summary>
        /// <param name="action">The action to modify.</param>
        /// <param name="isAlwaysAvailable">Indicates whether the action is always available. If not, then the value will be set because the action will be passed out to user code.</param>
        /// <param name="title">The title to set.</param>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDireclyAccessPayloadMetadataProperties", Justification = "This component is allowed to set these properties.")]
        internal void SetTitle(ODataAction action, bool isAlwaysAvailable, string title)
        {
            Debug.Assert(action != null, "action != null");
            if (!isAlwaysAvailable || this.interpreter.ShouldIncludeOperationMetadata(PayloadMetadataKind.Operation.Title, () => false))
            {
                action.Title = title;
            }
        }

        /// <summary>
        /// Sets the action's Target property if it should be included according to the current query option.
        /// </summary>
        /// <param name="action">The action to modify.</param>
        /// <param name="isAlwaysAvailable">Indicates whether the action is always available. If not, then the value will be set because the action will be passed out to user code.</param>
        /// <param name="computeTarget">The callback to compute the target.</param>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDireclyAccessPayloadMetadataProperties", Justification = "This component is allowed to set these properties.")]
        internal void SetTarget(ODataAction action, bool isAlwaysAvailable, Func<Uri> computeTarget)
        {
            Debug.Assert(action != null, "action != null");
            if (!isAlwaysAvailable || this.interpreter.ShouldIncludeOperationMetadata(PayloadMetadataKind.Operation.Target, () => false))
            {
                Debug.Assert(computeTarget != null, "computeTarget != null");
                action.Target = computeTarget();
            }
        }

        /// <summary>
        /// Checks whether the action's Title property has changed, and sets it to null if it should not be included according to the current query option.
        /// </summary>
        /// <param name="action">The action to modify.</param>
        /// <param name="originalTitle">The original computed title.</param>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDireclyAccessPayloadMetadataProperties", Justification = "This component is allowed to set these properties.")]
        internal void CheckForUnmodifiedTitle(ODataAction action, string originalTitle)
        {
            if (!this.interpreter.ShouldIncludeOperationMetadata(PayloadMetadataKind.Operation.Title, () => action.Title != originalTitle))
            {
                action.Title = null;
            }
        }

        /// <summary>
        /// Checks whether the action's Target property has changed, and sets it to null if it should not be included according to the current query option.
        /// </summary>
        /// <param name="action">The action to modify.</param>
        /// <param name="computeOriginalTarget">The callback to compute the original target.</param>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDireclyAccessPayloadMetadataProperties", Justification = "This component is allowed to set these properties.")]
        internal void CheckForUnmodifiedTarget(ODataAction action, Func<Uri> computeOriginalTarget)
        {
            if (!this.interpreter.ShouldIncludeOperationMetadata(PayloadMetadataKind.Operation.Target, () => !ReferenceEquals(action.Target, computeOriginalTarget())))
            {
                action.Target = null;
            }
        }
    }
}
