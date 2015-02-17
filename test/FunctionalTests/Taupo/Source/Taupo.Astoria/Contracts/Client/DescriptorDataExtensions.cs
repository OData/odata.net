//---------------------------------------------------------------------
// <copyright file="DescriptorDataExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Product;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// Extension methods for <see cref="EntityDescriptorData"/>.
    /// </summary>
    public static class DescriptorDataExtensions
    {
        /// <summary>
        /// Sets the entity set name.
        /// </summary>
        /// <param name="entityDescriptorData">The entity descriptor data.</param>
        /// <param name="entitySetName">Name of the entity set.</param>
        /// <returns><see cref="EntityDescriptorData"/> on which entity set name has been set.</returns>
        public static EntityDescriptorData SetEntitySetName(this EntityDescriptorData entityDescriptorData, string entitySetName)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityDescriptorData, "entityDescriptorData");

            entityDescriptorData.EntitySetName = entitySetName;

            return entityDescriptorData;
        }

        /// <summary>
        /// Sets the identity.
        /// </summary>
        /// <param name="entityDescriptorData">The entity descriptor data.</param>
        /// <param name="identity">The identity.</param>
        /// <returns><see cref="EntityDescriptorData"/> on which identity has been set.</returns>
        public static EntityDescriptorData SetIdentity(this EntityDescriptorData entityDescriptorData, Uri identity)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityDescriptorData, "entityDescriptorData");

            entityDescriptorData.Identity = identity;

            return entityDescriptorData;
        }

        /// <summary>
        /// Sets the ETag.
        /// </summary>
        /// <param name="entityDescriptorData">The entity descriptor data.</param>
        /// <param name="value">The etag for the entity.</param>
        /// <returns><see cref="EntityDescriptorData"/> on which ETag has been set.</returns>
        public static EntityDescriptorData SetETag(this EntityDescriptorData entityDescriptorData, string value)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityDescriptorData, "entityDescriptorData");

            entityDescriptorData.ETag = value;

            return entityDescriptorData;
        }

        /// <summary>
        /// Sets the parent for insert.
        /// </summary>
        /// <param name="entityDescriptorData">The entity descriptor data.</param>
        /// <param name="parentForInsert">The parent for insert.</param>
        /// <returns><see cref="EntityDescriptorData"/> on which parent for insert has been set.</returns>
        public static EntityDescriptorData SetParentForInsert(this EntityDescriptorData entityDescriptorData, object parentForInsert)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityDescriptorData, "entityDescriptorData");

            entityDescriptorData.ParentForInsert = parentForInsert;

            return entityDescriptorData;
        }

        /// <summary>
        /// Sets the parent property for insert.
        /// </summary>
        /// <param name="entityDescriptorData">The entity descriptor data.</param>
        /// <param name="parentPropertyForInsert">The parent property for insert.</param>
        /// <returns><see cref="EntityDescriptorData"/> on which parent property for insert has been set.</returns>
        public static EntityDescriptorData SetParentPropertyForInsert(this EntityDescriptorData entityDescriptorData, string parentPropertyForInsert)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityDescriptorData, "entityDescriptorData");

            entityDescriptorData.ParentPropertyForInsert = parentPropertyForInsert;

            return entityDescriptorData;
        }

        /// <summary>
        /// Sets the edit link.
        /// </summary>
        /// <param name="entityDescriptorData">The entity descriptor data.</param>
        /// <param name="editLink">The edit link.</param>
        /// <returns><see cref="EntityDescriptorData"/> on which edit link has been set.</returns>
        public static EntityDescriptorData SetEditLink(this EntityDescriptorData entityDescriptorData, Uri editLink)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityDescriptorData, "entityDescriptorData");

            entityDescriptorData.EditLink = editLink;

            return entityDescriptorData;
        }

        /// <summary>
        /// Sets the self link.
        /// </summary>
        /// <param name="entityDescriptorData">The entity descriptor data.</param>
        /// <param name="selfLink">The self link.</param>
        /// <returns><see cref="EntityDescriptorData"/> on which self link has been set.</returns>
        public static EntityDescriptorData SetSelfLink(this EntityDescriptorData entityDescriptorData, Uri selfLink)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityDescriptorData, "entityDescriptorData");

            entityDescriptorData.SelfLink = selfLink;

            return entityDescriptorData;
        }

        /// <summary>
        /// Sets the server type name.
        /// </summary>
        /// <param name="entityDescriptorData">The entity descriptor data.</param>
        /// <param name="serverTypeName">The server type name.</param>
        /// <returns><see cref="EntityDescriptorData"/> on which the server type name has been set.</returns>
        public static EntityDescriptorData SetServerTypeName(this EntityDescriptorData entityDescriptorData, string serverTypeName)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityDescriptorData, "entityDescriptorData");

            entityDescriptorData.ServerTypeName = serverTypeName;

            return entityDescriptorData;
        }

        /// <summary>
        /// Sets the edit link.
        /// </summary>
        /// <param name="streamDescriptorData">The stream descriptor data.</param>
        /// <param name="editLink">The self link.</param>
        /// <returns><see cref="StreamDescriptorData"/> on which edit link has been set.</returns>
        public static StreamDescriptorData SetEditLink(this StreamDescriptorData streamDescriptorData, Uri editLink)
        {
            ExceptionUtilities.CheckArgumentNotNull(streamDescriptorData, "streamDescriptorData");

            streamDescriptorData.EditLink = editLink;

            return streamDescriptorData;
        }

        /// <summary>
        /// Sets the self link.
        /// </summary>
        /// <param name="streamDescriptorData">The stream descriptor data.</param>
        /// <param name="selfLink">The self link.</param>
        /// <returns><see cref="StreamDescriptorData"/> on which self link has been set.</returns>
        public static StreamDescriptorData SetSelfLink(this StreamDescriptorData streamDescriptorData, Uri selfLink)
        {
            ExceptionUtilities.CheckArgumentNotNull(streamDescriptorData, "streamDescriptorData");

            streamDescriptorData.SelfLink = selfLink;

            return streamDescriptorData;
        }

        /// <summary>
        /// Sets the content type
        /// </summary>
        /// <param name="streamDescriptorData">The stream descriptor data.</param>
        /// <param name="contentType">The content type.</param>
        /// <returns><see cref="StreamDescriptorData"/> on which content type has been set.</returns>
        public static StreamDescriptorData SetContentType(this StreamDescriptorData streamDescriptorData, string contentType)
        {
            ExceptionUtilities.CheckArgumentNotNull(streamDescriptorData, "streamDescriptorData");

            streamDescriptorData.ContentType = contentType;

            return streamDescriptorData;
        }

        /// <summary>
        /// Sets the etag
        /// </summary>
        /// <param name="streamDescriptorData">The stream descriptor data.</param>
        /// <param name="value">The etag to set.</param>
        /// <returns><see cref="StreamDescriptorData"/> on which etag has been set.</returns>
        public static StreamDescriptorData SetETag(this StreamDescriptorData streamDescriptorData, string value)
        {
            ExceptionUtilities.CheckArgumentNotNull(streamDescriptorData, "streamDescriptorData");

            streamDescriptorData.ETag = value;

            return streamDescriptorData;
        }

        /// <summary>
        /// Sets the relationship link.
        /// </summary>
        /// <param name="linkInfoData">The link info data.</param>
        /// <param name="relationshipLink">The relationship link.</param>
        /// <returns><see cref="LinkInfoData"/> on which relationship link has been set.</returns>
        public static LinkInfoData SetRelationshipLink(this LinkInfoData linkInfoData, Uri relationshipLink)
        {
            ExceptionUtilities.CheckArgumentNotNull(linkInfoData, "linkInfoData");

            linkInfoData.RelationshipLink = relationshipLink;

            return linkInfoData;
        }

        /// <summary>
        /// Sets the navigation link.
        /// </summary>
        /// <param name="linkInfoData">The link info data.</param>
        /// <param name="navigationLink">The navigation link.</param>
        /// <returns><see cref="LinkInfoData"/> on which navigation link has been set.</returns>
        public static LinkInfoData SetNavigationLink(this LinkInfoData linkInfoData, Uri navigationLink)
        {
            ExceptionUtilities.CheckArgumentNotNull(linkInfoData, "linkInfoData");

            linkInfoData.NavigationLink = navigationLink;

            return linkInfoData;
        }

        /// <summary>
        /// Returns whether or not the given link descriptor data will be sent as a separate request during SaveChanges
        /// </summary>
        /// <param name="linkDescriptorData">The link descriptor data</param>
        /// <returns>True if it will be sent as a separate request, otherwise false</returns>
        public static bool WillTriggerSeparateRequest(this LinkDescriptorData linkDescriptorData)
        {
            ExceptionUtilities.CheckArgumentNotNull(linkDescriptorData, "linkDescriptorData");
            
            // no request will be made for these
            if (linkDescriptorData.State == EntityStates.Unchanged || linkDescriptorData.State == EntityStates.Detached)
            {
                return false;
            }

            // delete link requests are always seperate
            if (linkDescriptorData.State == EntityStates.Deleted)
            {
                return true;
            }

            // link will be folded into the insert
            if (linkDescriptorData.SourceDescriptor.State == EntityStates.Added
                && !linkDescriptorData.SourceDescriptor.IsMediaLinkEntry 
                && linkDescriptorData.TargetDescriptor != null
                && linkDescriptorData.TargetDescriptor.State != EntityStates.Added)
            {
                return false;
            }

            // insert uri will be based on the link
            if (linkDescriptorData.TargetDescriptor != null && linkDescriptorData.TargetDescriptor.ParentPropertyForInsert == linkDescriptorData.SourcePropertyName)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Updates the stream decriptor data with values from response headers
        /// </summary>
        /// <param name="streamDescriptorData">The descriptor data to update</param>
        /// <param name="headers">The response headers</param>
        public static void UpdateFromHeaders(this StreamDescriptorData streamDescriptorData, IDictionary<string, string> headers)
        {
            ExceptionUtilities.CheckArgumentNotNull(streamDescriptorData, "streamDescriptorData");
            ExceptionUtilities.CheckArgumentNotNull(headers, "headers");

            string etag;

            headers.TryGetValue(HttpHeaders.ETag, out etag);
            streamDescriptorData.SetETag(etag);
        }

        /// <summary>
        /// Updates the entity decriptor data with values from response headers
        /// </summary>
        /// <param name="entityDescriptorData">The descriptor data to update</param>
        /// <param name="headers">The response headers</param>
        public static void UpdateFromHeaders(this EntityDescriptorData entityDescriptorData, IDictionary<string, string> headers)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityDescriptorData, "entityDescriptorData");
            ExceptionUtilities.CheckArgumentNotNull(headers, "headers");

            string etag;
            headers.TryGetValue(HttpHeaders.ETag, out etag);
            entityDescriptorData.SetETag(etag); // we do want to set null if the header is not present
            
            string location;
            if (headers.TryGetValue(HttpHeaders.Location, out location))
            {
                entityDescriptorData.SetEditLink(new Uri(location));
            }

            string dataServiceId;
            if (headers.TryGetValue(HttpHeaders.DataServiceId, out dataServiceId))
            {
                entityDescriptorData.SetIdentity(new Uri(dataServiceId, UriKind.RelativeOrAbsolute));
            }
        }

        /// <summary>
        /// Updates the stream decriptor data with values from a response payload
        /// </summary>
        /// <param name="entityDescriptorData">The descriptor data to update</param>
        /// <param name="entityData">A representation of the payload</param>
        /// <param name="contextBaseUri">The base uri of the context</param>
        public static void UpdateFromPayload(this EntityDescriptorData entityDescriptorData, EntityInstance entityData, Uri contextBaseUri)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityDescriptorData, "entityDescriptorData");
            ExceptionUtilities.CheckArgumentNotNull(entityData, "entityData");

            var xmlBase = entityData.Annotations.OfType<XmlBaseAnnotation>().SingleOrDefault();

            entityDescriptorData.SetIdentity(GetAbsoluteUriForLink(contextBaseUri, xmlBase, entityData.Id));
            entityDescriptorData.SetETag(entityData.ETag);
            entityDescriptorData.SetServerTypeName(entityData.FullTypeName);

            var editLink = entityData.EditLink;
            if (editLink != null)
            {
                entityDescriptorData.SetEditLink(GetAbsoluteUriForLink(contextBaseUri, xmlBase, editLink));
            }

            var selfLink = entityData.Annotations.OfType<SelfLinkAnnotation>().SingleOrDefault();
            if (selfLink != null)
            {
                entityDescriptorData.SetSelfLink(GetAbsoluteUriForLink(contextBaseUri, xmlBase, selfLink.Value));
            }

            foreach (var navigation in entityData.Properties.OfType<NavigationPropertyInstance>())
            {
                var linkInfoData = entityDescriptorData.LinkInfos.SingleOrDefault(l => l.Name == navigation.Name);
                if (linkInfoData == null)
                {
                    linkInfoData = entityDescriptorData.CreateLinkInfoData(navigation.Name);
                }

                if (navigation.AssociationLink != null)
                {
                    linkInfoData.RelationshipLink = GetAbsoluteUriForLink(contextBaseUri, xmlBase, navigation.AssociationLink.UriString);
                }

                string navigationLink = null;
                if (navigation.Value != null)
                {
                    if (navigation.IsExpanded)
                    {
                        navigationLink = ((ExpandedLink)navigation.Value).UriString;
                    }
                    else
                    {
                        navigationLink = ((DeferredLink)navigation.Value).UriString;
                    }
                }

                if (navigationLink != null)
                {
                    linkInfoData.NavigationLink = GetAbsoluteUriForLink(contextBaseUri, xmlBase, navigationLink);
                }
            }

            entityDescriptorData.RemoveOperationDescriptorData();
            foreach (var operation in entityData.ServiceOperationDescriptors)
            {
                entityDescriptorData.CreateOperationDescriptorData(
                    new Uri(operation.Metadata, UriKind.RelativeOrAbsolute), 
                    new Uri(operation.Target, UriKind.RelativeOrAbsolute),
                    operation.Title, 
                    operation.IsAction);
            }

            if (entityData.IsMediaLinkEntry())
            {
                if (entityData.StreamSourceLink != null)
                {
                    entityDescriptorData.ReadStreamUri = GetAbsoluteUriForLink(contextBaseUri, xmlBase, entityData.StreamSourceLink);
                }

                if (entityData.StreamEditLink != null)
                {
                    entityDescriptorData.EditStreamUri = GetAbsoluteUriForLink(contextBaseUri, xmlBase, entityData.StreamEditLink);
                }

                entityDescriptorData.StreamETag = entityData.StreamETag;
            }

            foreach (var namedStream in entityData.Properties.OfType<NamedStreamInstance>())
            {
                var streamDescriptor = entityDescriptorData.StreamDescriptors.SingleOrDefault(n => n.Name == namedStream.Name);
                if (streamDescriptor == null)
                {
                    streamDescriptor = entityDescriptorData.CreateStreamDescriptorData(EntityStates.Unchanged, 0, namedStream.Name);
                }

                // apply self link's content type first, as the value from the edit link should 'win'
                if (namedStream.SourceLink != null)
                {
                    streamDescriptor.SelfLink = GetAbsoluteUriForLink(contextBaseUri, xmlBase, namedStream.SourceLink);
                    streamDescriptor.ContentType = namedStream.SourceLinkContentType;
                }

                if (namedStream.EditLink != null)
                {
                    streamDescriptor.EditLink = GetAbsoluteUriForLink(contextBaseUri, xmlBase, namedStream.EditLink);
                    streamDescriptor.ContentType = namedStream.EditLinkContentType;
                }

                streamDescriptor.ETag = namedStream.ETag;
            }

            // TODO: update property values from payload
        }

        /// <summary>
        /// Returns a value indicating whether the descriptor should be updated based on its state and the context's merge option
        /// </summary>
        /// <param name="contextData">The context data</param>
        /// <param name="descriptorData">The descriptor data</param>
        /// <param name="isNewDescriptor">A value indicating whether the descriptor has just been created</param>
        /// <returns>True if changes should be applied, false otherwise</returns>
        public static bool ShouldApplyChangeToDescriptor(this DataServiceContextData contextData, DescriptorData descriptorData, bool isNewDescriptor)
        {
            return isNewDescriptor || contextData.MergeOption == MergeOption.OverwriteChanges || (contextData.MergeOption == MergeOption.PreserveChanges && descriptorData.State == EntityStates.Unchanged);
        }
        
        /// <summary>
        /// Extension method to either find or create a link descriptor with the given values
        /// </summary>
        /// <param name="contextData">The context data</param>
        /// <param name="sourceDescriptor">The source descriptor</param>
        /// <param name="propertyName">The property name</param>
        /// <param name="targetDescriptor">The target descriptor</param>
        /// <returns>A link descriptor with the given values</returns>
        public static LinkDescriptorData MaterializeLinkDescriptor(this DataServiceContextData contextData, EntityDescriptorData sourceDescriptor, string propertyName, EntityDescriptorData targetDescriptor)
        {
            var existingLink = contextData.LinkDescriptorsData.SingleOrDefault(l => l.SourceDescriptor == sourceDescriptor && l.SourcePropertyName == propertyName && l.TargetDescriptor == targetDescriptor);
            if (existingLink == null)
            {
                existingLink = contextData.CreateLinkDescriptorData(EntityStates.Unchanged, 0, sourceDescriptor, propertyName, targetDescriptor);
            }

            return existingLink;
        }
        
        /// <summary>
        /// Gets the absolute uri for the given link href value
        /// </summary>
        /// <param name="baseUri">The context base uri</param>
        /// <param name="xmlBase">The base uri annotation from the payload</param>
        /// <param name="hrefValue">The href value for the link</param>
        /// <returns>The absolute uri for the link, or a relative uri if it cannot be determined</returns>
        internal static Uri GetAbsoluteUriForLink(Uri baseUri, XmlBaseAnnotation xmlBase, string hrefValue)
        {
            var absoluteUri = new Uri(hrefValue, UriKind.RelativeOrAbsolute);
            if (!absoluteUri.IsAbsoluteUri)
            {
                string root;
                if (xmlBase != null)
                {
                    root = xmlBase.Value;
                }
                else if (baseUri != null)
                {
                    ExceptionUtilities.Assert(baseUri.IsAbsoluteUri, "baseUri was not an absolute uri. It was: '{0}'", baseUri.OriginalString);
                    root = baseUri.AbsoluteUri;
                }
                else
                {
                    // TODO: need to be able to read xml:base from feed, cannot currently.
                    // just return a relative uri for now
                    return absoluteUri;
                }

                absoluteUri = new Uri(UriHelpers.ConcatenateUriSegments(root, hrefValue));
            }

            return absoluteUri;
        }
    }
}