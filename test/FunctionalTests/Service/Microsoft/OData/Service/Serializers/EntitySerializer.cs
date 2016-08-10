//---------------------------------------------------------------------
// <copyright file="EntitySerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    #endregion Namespaces

    /// <summary>Serializes results into the given format using the given message writer.</summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Pending")]
    internal sealed class EntitySerializer : Serializer
    {
        #region Fields

        /// <summary>ODataMessageWriter instance which needs to be used to write the response.</summary>
        private readonly ODataMessageWriter messageWriter;

        /// <summary>The content format.</summary>
        private readonly ODataFormat contentFormat;

        /// <summary>ODataWriter instance of write the feed/entry response.</summary>
        private DataServiceODataWriter dataServicesODataWriter;

        /// <summary>Storage for a lazily-created operation serializer.</summary>
        private OperationSerializer operationSerializer;

        #endregion Fields

        /// <summary>Initializes a new EntitySerializer instance.</summary>
        /// <param name="requestDescription">Request description.</param>
        /// <param name="absoluteServiceUri">Absolute URI to the service entry point.</param>
        /// <param name="service">Service with configuration and provider from which metadata should be gathered.</param>
        /// <param name="httpETagHeaderValue">HTTP ETag header value.</param>
        /// <param name="messageWriter">ODataMessageWriter instance which needs to be used to write the response.</param>
        /// <param name="contentFormat">The content format.</param>
        internal EntitySerializer(
            RequestDescription requestDescription,
            Uri absoluteServiceUri,
            IDataService service,
            string httpETagHeaderValue,
            ODataMessageWriter messageWriter,
            ODataFormat contentFormat)
            : base(requestDescription, absoluteServiceUri, service, httpETagHeaderValue)
        {
            Debug.Assert(service != null, "service != null");
            this.messageWriter = messageWriter;
            this.contentFormat = contentFormat;
        }

        /// <summary>
        /// Gets the operation serializer to use for actions and functions.
        /// </summary>
        private OperationSerializer OperationSerializer
        {
            get
            {
                if (this.operationSerializer == null)
                {
                    Debug.Assert(this.Service != null && this.Service.Provider != null, "this.Service != null && this.Service.Provider != null");
                    Debug.Assert(this.Service.ActionProvider != null, "this.Service.ActionProvider != null");
                    Debug.Assert(this.Service.OperationContext != null, "this.Service.OperationContext != null");

                    this.operationSerializer = new OperationSerializer(
                        this.PayloadMetadataParameterInterpreter,
                        this.PayloadMetadataPropertyManager,
                        this.Service.ActionProvider.AdvertiseServiceAction,
                        this.Service.Provider.ContainerNamespace,
                        this.contentFormat,
                        this.Service.OperationContext.MetadataUri);
                }

                return this.operationSerializer;
            }
        }

        /// <summary>
        /// Flushes the content of the underlying writers
        /// </summary>
        internal override void Flush()
        {
            // Astoria-ODataLib-Integration: Astoria does not call flush before calling the IDataServiceHost.ProcessException method
            // If the request is for an entry/feed and the data source throws an error while these results are being enumerated and written,
            // the server doesn't flush the writer's stream before it calls HandleException and starts writing out the error.
            // To handle this case, we'll make the EntitySerializer expose a method that calls Flush on the underlying ODL writer instance.
            if (this.dataServicesODataWriter != null)
            {
                this.dataServicesODataWriter.Flush();
            }
        }

        /// <summary>Writes a single top-level element.</summary>
        /// <param name="expanded">Expanded properties for the result.</param>
        /// <param name="element">Element to write, possibly null.</param>
        protected override void WriteTopLevelElement(IExpandedResult expanded, object element)
        {
            Debug.Assert(this.RequestDescription.IsSingleResult, "this.RequestDescription.SingleResult");
            Debug.Assert(this.RequestDescription.TargetKind == RequestTargetKind.Resource, "this.RequestDescription.TargetKind == RequestTargetKind.Resource");
            Debug.Assert(element != null, "element != null");

            this.dataServicesODataWriter = this.CreateODataWriter(false /*forFeed*/);

            ResourceType targetResourceType;
            if (this.RequestDescription.TargetSource == RequestTargetSource.EntitySet ||
                this.RequestDescription.TargetSource == RequestTargetSource.ServiceOperation)
            {
                targetResourceType = this.RequestDescription.TargetResourceType;
            }
            else
            {
                Debug.Assert(
                    this.RequestDescription.TargetSource == RequestTargetSource.Property,
                    "TargetSource(" + this.RequestDescription.TargetSource + ") == Property -- otherwise control shouldn't be here.");
                Debug.Assert(this.RequestDescription.Property != null, "this.RequestDescription.Property - otherwise Property source set with no Property specified.");
                Debug.Assert(
                    this.RequestDescription.Property.TypeKind == ResourceTypeKind.EntityType,
                    "SyndicationSerializer.WriteTopLevelElement should only be called for serializing out entity types");

                targetResourceType = this.RequestDescription.Property.ResourceType;
            }

            bool needPop = this.PushSegmentForRoot();
            this.WriteEntry(
                expanded,                               // expanded
                element,                                // element
                false,                                  // resourceInstanceInFeed
                targetResourceType);                    // expectedType

            this.PopSegmentName(needPop);
        }

        /// <summary>Writes multiple top-level elements, possibly none.</summary>
        /// <param name="expanded">Expanded properties for the result.</param>
        /// <param name="elements">Enumerator for elements to write.</param>
        protected override void WriteTopLevelElements(IExpandedResult expanded, QueryResultInfo elements)
        {
            Debug.Assert(elements != null, "elements != null");
            Debug.Assert(!this.RequestDescription.IsSingleResult, "!this.RequestDescription.SingleResult");

            string title;
            if (this.RequestDescription.TargetKind != RequestTargetKind.OpenProperty &&
                this.RequestDescription.TargetSource == RequestTargetSource.Property)
            {
                title = this.RequestDescription.Property.Name;
            }
            else
            {
                title = this.RequestDescription.ContainerName;
            }

            this.dataServicesODataWriter = this.CreateODataWriter(true /*forFeed*/);

            bool needPop = this.PushSegmentForRoot();
            this.WriteFeedElements(
                expanded,
                elements,
                this.RequestDescription.TargetResourceType.ElementType(),
                title,                                      // title
                () => new Uri(this.RequestDescription.LastSegmentInfo.Identifier, UriKind.Relative),
                () => this.RequestDescription.ResultUri,    // absoluteUri
                true);

            this.PopSegmentName(needPop);
        }

        /// <summary>
        /// Gets the association link URL.
        /// </summary>
        /// <param name="entityToSerialize">The entity to serialize.</param>
        /// <param name="navigationProperty">The navigation property.</param>
        /// <returns>The association link url.</returns>
        private static Uri GetAssociationLinkUrl(EntityToSerialize entityToSerialize, ResourceProperty navigationProperty)
        {
            Uri associationLinkUri = RequestUriProcessor.AppendUnescapedSegment(entityToSerialize.SerializedKey.RelativeEditLink, navigationProperty.Name);
            return RequestUriProcessor.AppendEscapedSegment(associationLinkUri, XmlConstants.UriLinkSegment);
        }

        /// <summary>Write the entry element.</summary>
        /// <param name="expanded">Expanded result provider for the specified <paramref name="element"/>.</param>
        /// <param name="element">Element representing the entry element.</param>
        /// <param name="resourceInstanceInFeed">true if the resource instance being serialized is inside a feed; false otherwise.</param>
        /// <param name="expectedType">Expected type of the entry element.</param>
        private void WriteEntry(IExpandedResult expanded, object element, bool resourceInstanceInFeed, ResourceType expectedType)
        {
            Debug.Assert(element != null, "element != null");
            Debug.Assert(expectedType != null && expectedType.ResourceTypeKind == ResourceTypeKind.EntityType, "expectedType != null && expectedType.ResourceTypeKind == ResourceTypeKind.EntityType");
            this.IncrementSegmentResultCount();

            ODataResource entry = new ODataResource();
            if (!resourceInstanceInFeed)
            {
                entry.SetSerializationInfo(new ODataResourceSerializationInfo { NavigationSourceName = this.CurrentContainer.Name, NavigationSourceEntityTypeName = this.CurrentContainer.ResourceType.FullName, ExpectedTypeName = expectedType.FullName });
            }

            string title = expectedType.Name;

            ResourceType actualResourceType = WebUtil.GetNonPrimitiveResourceType(this.Provider, element);
            if (actualResourceType.ResourceTypeKind != ResourceTypeKind.EntityType)
            {
                // making sure that the actual resource type is an entity type
                throw new DataServiceException(500, Microsoft.OData.Service.Strings.BadProvider_InconsistentEntityOrComplexTypeUsage(actualResourceType.FullName));
            }

            EntityToSerialize entityToSerialize = this.WrapEntity(element, actualResourceType);

            // populate the media resource, if the entity is a MLE.
            entry.MediaResource = this.GetMediaResource(entityToSerialize, title);

            // Write the type name
            this.PayloadMetadataPropertyManager.SetTypeName(entry, this.CurrentContainer.ResourceType.FullName, actualResourceType.FullName);

            // Write Id element
            this.PayloadMetadataPropertyManager.SetId(entry, () => entityToSerialize.SerializedKey.Identity);

            // Write "edit" link
            this.PayloadMetadataPropertyManager.SetEditLink(entry, () => entityToSerialize.SerializedKey.RelativeEditLink);

            // Write the etag property, if the type has etag properties
            this.PayloadMetadataPropertyManager.SetETag(entry, () => this.GetETagValue(element, actualResourceType));

            IEnumerable<ProjectionNode> projectionNodes = this.GetProjections();
            if (projectionNodes != null)
            {
                // Filter the projection nodes for the actual type of the entity
                // The projection node might refer to the property in a derived type. If the TargetResourceType of
                // the projection node is not a super type, then we do not want to serialize this property.
                projectionNodes = projectionNodes.Where(projectionNode => projectionNode.TargetResourceType.IsAssignableFrom(actualResourceType));

                // Because we are going to enumerate through these multiple times, create a list.
                projectionNodes = projectionNodes.ToList();
            }

            // Populate the advertised actions
            IEnumerable<ODataAction> actions;
            if (this.TryGetAdvertisedActions(entityToSerialize, resourceInstanceInFeed, out actions))
            {
                foreach (ODataAction action in actions)
                {
                    entry.AddAction(action);
                }
            }

            // Populate all the normal properties
            entry.Properties = this.GetEntityProperties(entityToSerialize, projectionNodes);

            // And start the entry
            var args = new DataServiceODataWriterEntryArgs(entry, element, this.Service.OperationContext);
            this.dataServicesODataWriter.WriteStart(args);

            this.WriteAllNestedComplexProperties(entityToSerialize, projectionNodes);

            // Now write all the navigation properties
            this.WriteNavigationProperties(expanded, entityToSerialize, resourceInstanceInFeed, projectionNodes);

            // And write the end of the entry
            this.dataServicesODataWriter.WriteEnd(args);

#if ASTORIA_FF_CALLBACKS
            this.Service.InternalOnWriteItem(target, element);
#endif
        }

        /// <summary>
        /// Get the stream reference value for media resource (the default stream of an entity).
        /// </summary>
        /// <param name="entityToSerialize">Entity that is currently being serialized.</param>
        /// <param name="title">The title for the element being written.</param>
        /// <returns>
        /// An instance of ODataStreamReferenceValue containing the metadata about the media resource.
        /// </returns>
        private ODataStreamReferenceValue GetMediaResource(EntityToSerialize entityToSerialize, string title)
        {
            Debug.Assert(entityToSerialize.Entity != null, "element != null");
            Debug.Assert(entityToSerialize.ResourceType != null && entityToSerialize.ResourceType.ResourceTypeKind == ResourceTypeKind.EntityType, "type != null && type.ResourceTypeKind == ResourceTypeKind.EntityType");
            Debug.Assert(!string.IsNullOrEmpty(title), "!string.IsNullOrEmpty(title)");

            ODataStreamReferenceValue mediaResource = null;

            // Handle MLE
            if (entityToSerialize.ResourceType.IsMediaLinkEntry)
            {
                string mediaETag;
                Uri readStreamUri;
                string mediaContentType;

                Debug.Assert(entityToSerialize.ResourceType.ResourceTypeKind == ResourceTypeKind.EntityType, "type.ResourceTypeKind == ResourceTypeKind.EntityType");
                this.Service.StreamProvider.GetStreamDescription(entityToSerialize.Entity, null /*null for MLE*/, this.Service.OperationContext, out mediaETag, out readStreamUri, out mediaContentType);
                Debug.Assert(WebUtil.IsETagValueValid(mediaETag, true), "WebUtil.IsETagValueValid(mediaETag, true)");
                Debug.Assert(!string.IsNullOrEmpty(mediaContentType), "!string.IsNullOrEmpty(mediaContentType)");

                mediaResource = new ODataStreamReferenceValue();

                // build the stream's edit link lazily to avoid creating the entity's edit link if it is not needed.
                SimpleLazy<Uri> lazyStreamEditLink = new SimpleLazy<Uri>(() => RequestUriProcessor.AppendEscapedSegment(entityToSerialize.SerializedKey.RelativeEditLink, XmlConstants.UriValueSegment));

                this.PayloadMetadataPropertyManager.SetEditLink(mediaResource, () => lazyStreamEditLink.Value);

                this.PayloadMetadataPropertyManager.SetContentType(mediaResource, mediaContentType);

                // If the stream provider did not provider a read link, then we should use the edit link as the read link.
                this.PayloadMetadataPropertyManager.SetReadLink(mediaResource, () => readStreamUri ?? lazyStreamEditLink.Value);

                if (!string.IsNullOrEmpty(mediaETag))
                {
                    this.PayloadMetadataPropertyManager.SetETag(mediaResource, mediaETag);
                }
            }

            return mediaResource;
        }

        /// <summary>
        /// Get the ODataStreamReferenceValue instance containing the metadata for named stream property.
        /// </summary>
        /// <param name="entityToSerialize">Entity that is currently being serialized.</param>
        /// <param name="namedStreamProperty">Named stream property for which the link element needs to be written.</param>
        /// <returns>
        /// An instance of ODataStreamReferenceValue containing all the metadata about the named stream property.
        /// </returns>
        private ODataStreamReferenceValue GetNamedStreamPropertyValue(EntityToSerialize entityToSerialize, ResourceProperty namedStreamProperty)
        {
            Debug.Assert(entityToSerialize != null, "entityToSerialize != null");
            Debug.Assert(namedStreamProperty != null, "namedStreamProperty != null");
            Debug.Assert(namedStreamProperty.IsOfKind(ResourcePropertyKind.Stream), "namedStreamProperty.IsOfKind(ResourcePropertyKind.Stream)");

            string mediaETag;
            Uri readStreamUri;
            string mediaContentType;
            this.Service.StreamProvider.GetStreamDescription(entityToSerialize.Entity, namedStreamProperty, this.Service.OperationContext, out mediaETag, out readStreamUri, out mediaContentType);
            Debug.Assert(WebUtil.IsETagValueValid(mediaETag, true), "WebUtil.IsETagValueValid(mediaETag, true)");

            ODataStreamReferenceValue streamReferenceValue = new ODataStreamReferenceValue();

            this.PayloadMetadataPropertyManager.SetContentType(streamReferenceValue, mediaContentType);

            this.PayloadMetadataPropertyManager.SetEditLink(
                streamReferenceValue,
                () =>
                {
                    Uri relativeUri = entityToSerialize.SerializedKey.RelativeEditLink;
                    return RequestUriProcessor.AppendUnescapedSegment(relativeUri, namedStreamProperty.Name);
                });

            if (!string.IsNullOrEmpty(mediaETag))
            {
                this.PayloadMetadataPropertyManager.SetETag(streamReferenceValue, mediaETag);
            }

            // Note if readStreamUri is null, the self link for the named stream will be omitted.
            this.PayloadMetadataPropertyManager.SetReadLink(streamReferenceValue, () => readStreamUri);
            return streamReferenceValue;
        }

        /// <summary>
        /// Writes the feed element for the atom payload.
        /// </summary>
        /// <param name="expanded">Expanded properties for the result.</param>
        /// <param name="elements">Collection of entries in the feed element.</param>
        /// <param name="expectedType">ExpectedType of the elements in the collection.</param>
        /// <param name="title">Title of the feed element.</param>
        /// <param name="getRelativeUri">Callback to get the relative uri of the feed.</param>
        /// <param name="getAbsoluteUri">Callback to get the absolute uri of the feed.</param>
        /// <param name="topLevel">True if the feed is the top level feed, otherwise false for the inner expanded feed.</param>
        private void WriteFeedElements(
            IExpandedResult expanded,
            QueryResultInfo elements,
            ResourceType expectedType,
            string title,
            Func<Uri> getRelativeUri,
            Func<Uri> getAbsoluteUri,
            bool topLevel)
        {
            Debug.Assert(elements != null, "elements != null");
            Debug.Assert(expectedType != null && expectedType.ResourceTypeKind == ResourceTypeKind.EntityType, "expectedType != null && expectedType.ResourceTypeKind == ResourceTypeKind.EntityType");
            Debug.Assert(!string.IsNullOrEmpty(title), "!string.IsNullOrEmpty(title)");

            ODataResourceSet feed = new ODataResourceSet();
            feed.SetSerializationInfo(new ODataResourceSerializationInfo { NavigationSourceName = this.CurrentContainer.Name, NavigationSourceEntityTypeName = this.CurrentContainer.ResourceType.FullName, ExpectedTypeName = expectedType.FullName });

            // Write the other elements for the feed
            this.PayloadMetadataPropertyManager.SetId(feed, () => getAbsoluteUri());

            // support for $count
            // in ATOM we write it at the beginning (we always have)
            //   in JSON for backward compatiblity reasons we write it at the end, so we must not fill it here.
            if (topLevel && this.RequestDescription.CountOption == RequestQueryCountOption.CountQuery)
            {
                feed.Count = this.RequestDescription.CountValue;
            }

            var feedArgs = elements.GetDataServiceODataWriterFeedArgs(feed, this.Service.OperationContext);
            this.dataServicesODataWriter.WriteStart(feedArgs);
            object lastObject = null;
            IExpandedResult lastExpandedSkipToken = null;
            while (elements.HasMoved)
            {
                object o = elements.Current;
                IExpandedResult skipToken = this.GetSkipToken(expanded);

                if (o != null)
                {
                    IExpandedResult expandedO = o as IExpandedResult;
                    if (expandedO != null)
                    {
                        expanded = expandedO;
                        o = GetExpandedElement(expanded);
                        skipToken = this.GetSkipToken(expanded);
                    }

                    this.WriteEntry(expanded, o, true /*resourceInstanceInFeed*/, expectedType);
                }

                elements.MoveNext();
                lastObject = o;
                lastExpandedSkipToken = skipToken;
            }

            // After looping through the objects in the sequence, decide if we need to write the next
            // page link and if yes, write it by invoking the delegate
            if (this.NeedNextPageLink(elements))
            {
                this.PayloadMetadataPropertyManager.SetNextPageLink(
                    feed,
                    this.AbsoluteServiceUri,
                    this.GetNextLinkUri(lastObject, lastExpandedSkipToken, getAbsoluteUri()));
            }

            this.dataServicesODataWriter.WriteEnd(feedArgs);
#if ASTORIA_FF_CALLBACKS
            this.Service.InternalOnWriteFeed(feed);
#endif
        }

        /// <summary>Gets properties of the given entity type instance.</summary>
        /// <param name="entityToSerialize">Entity that is currently being serialized.</param>
        /// <param name="projectionNodesForCurrentResourceType">List of all the properties that are being projected for the resource type, or null if no projections are applied.
        ///   The list must be filtered to only nodes which apply to the current resource type.</param>
        /// <returns>The list of properties for the specified entity.</returns>
        private IEnumerable<ODataProperty> GetEntityProperties(EntityToSerialize entityToSerialize, IEnumerable<ProjectionNode> projectionNodesForCurrentResourceType)
        {
            Debug.Assert(entityToSerialize != null, "entityToSerialize != null");
            Debug.Assert(
                projectionNodesForCurrentResourceType == null ||
                projectionNodesForCurrentResourceType.All(projectionNode => projectionNode.TargetResourceType.IsAssignableFrom(entityToSerialize.ResourceType)),
                "The projection nodes must be filtered to the current resource type only.");

            this.RecurseEnter();
            try
            {
                Debug.Assert(this.CurrentContainer != null, "this.CurrentContainer != null");

                if (projectionNodesForCurrentResourceType == null)
                {
                    return this.GetAllEntityProperties(entityToSerialize);
                }
                else
                {
                    // Fill only the properties which we know we will need values for.
                    return this.GetProjectedEntityProperties(entityToSerialize, projectionNodesForCurrentResourceType);
                }
            }
            finally
            {
                // The matching call to RecurseLeave is in a try/finally block not because it's necessary in the
                // presence of an exception (progress will halt anyway), but because it's easier to maintain in the
                // code in the presence of multiple exit points (returns).
                this.RecurseLeave();
            }
        }

        /// <summary>Returns an IEnumerable of ODataProperty instance for all structural properties in the current resource type
        /// and populates the navigation property information along with association links.</summary>
        /// <param name="entityToSerialize">Entity that is currently being serialized.</param>
        /// <returns>Returns an IEnumerable of ODataProperty instance for all structural properties in the current resource type..</returns>
        private IEnumerable<ODataProperty> GetAllEntityProperties(EntityToSerialize entityToSerialize)
        {
            Debug.Assert(entityToSerialize != null, "entityToSerialize != null");

            List<ODataProperty> properties = new List<ODataProperty>(entityToSerialize.ResourceType.Properties.Count);
            foreach (ResourceProperty property in this.Provider.GetResourceSerializableProperties(this.CurrentContainer, entityToSerialize.ResourceType))
            {
                if (property.TypeKind != ResourceTypeKind.EntityType
                    && property.TypeKind != ResourceTypeKind.ComplexType
                    && !(property.TypeKind == ResourceTypeKind.Collection && property.ResourceType.ElementType().ResourceTypeKind == ResourceTypeKind.ComplexType))
                {
                    properties.Add(this.GetODataPropertyForEntityProperty(entityToSerialize, property));
                }
            }

            if (entityToSerialize.ResourceType.IsOpenType)
            {
                foreach (KeyValuePair<string, object> property in this.Provider.GetOpenPropertyValues(entityToSerialize.Entity))
                {
                    string propertyName = property.Key;
                    if (string.IsNullOrEmpty(propertyName))
                    {
                        throw new DataServiceException(500, Microsoft.OData.Service.Strings.Syndication_InvalidOpenPropertyName(entityToSerialize.ResourceType.FullName));
                    }

                    properties.Add(this.GetODataPropertyForOpenProperty(propertyName, property.Value));
                }
            }

            return properties;
        }

        /// <summary>Returns an IEnumerable of ODataProperty instance for all projected properties in the <paramref name="projectionNodesForCurrentResourceType"/>
        /// and populates the navigation property information along with association links.</summary>
        /// <param name="entityToSerialize">Entity that is currently being serialized.</param>
        /// <param name="projectionNodesForCurrentResourceType">List of all the properties that are being projected for the resource type, or null if no projections are applied.
        ///   The list must be filtered to only nodes which apply to the current resource type.</param>
        /// <returns>Returns an IEnumerable of ODataProperty instance for all structural properties in the current resource type.</returns>
        private IEnumerable<ODataProperty> GetProjectedEntityProperties(EntityToSerialize entityToSerialize, IEnumerable<ProjectionNode> projectionNodesForCurrentResourceType)
        {
            Debug.Assert(entityToSerialize != null, "entityToSerialize != null");
            Debug.Assert(
                projectionNodesForCurrentResourceType == null ||
                projectionNodesForCurrentResourceType.All(projectionNode => projectionNode.TargetResourceType.IsAssignableFrom(entityToSerialize.ResourceType)),
                "The projection nodes must be filtered to the current resource type only.");

            List<ODataProperty> properties = new List<ODataProperty>(entityToSerialize.ResourceType.Properties.Count);
            foreach (ProjectionNode projectionNode in projectionNodesForCurrentResourceType)
            {
                string propertyName = projectionNode.PropertyName;
                var resourceProperty = projectionNode.Property;
                if (resourceProperty != null)
                {
                    if (resourceProperty.TypeKind != ResourceTypeKind.EntityType && resourceProperty.TypeKind != ResourceTypeKind.ComplexType
                        && !(resourceProperty.TypeKind == ResourceTypeKind.Collection && resourceProperty.ResourceType.ElementType().ResourceTypeKind == ResourceTypeKind.ComplexType))
                    {
                        properties.Add(this.GetODataPropertyForEntityProperty(entityToSerialize, resourceProperty));
                    }
                }
                else
                {
                    // Now get the property value
                    object propertyValue = WebUtil.GetPropertyValue(this.Provider, entityToSerialize.Entity, entityToSerialize.ResourceType, null, propertyName);
                    properties.Add(this.GetODataPropertyForOpenProperty(propertyName, propertyValue));
                }
            }

            return properties;
        }

        private void WriteAllNestedComplexProperties(EntityToSerialize entityToSerialize, IEnumerable<ProjectionNode> projectionNodesForCurrentResourceType)
        {
            Debug.Assert(entityToSerialize != null, "entityToSerialize != null");

            var properties = projectionNodesForCurrentResourceType == null
                ? this.Provider.GetResourceSerializableProperties(this.CurrentContainer, entityToSerialize.ResourceType)
                : projectionNodesForCurrentResourceType.Select(p=>p.Property);

            foreach (ResourceProperty property in properties)
            {
                if (property != null && (property.TypeKind == ResourceTypeKind.ComplexType
                    || (property.TypeKind == ResourceTypeKind.Collection && property.ResourceType.ElementType().ResourceTypeKind == ResourceTypeKind.ComplexType)))
                {
                    ODataWriterHelper.WriteNestedResourceInfo(this.dataServicesODataWriter.InnerWriter, this.GetODataNestedResourceForComplexProperty(entityToSerialize, property));
                }
            }
        }

        /// <summary>Writes all the navigation properties of the specified entity type.</summary>
        /// <param name="expanded">Expanded properties for the result.</param>
        /// <param name="entityToSerialize">Entity that is currently being serialized.</param>
        /// <param name="resourceInstanceInFeed">true if the resource instance being serialized is inside a feed; false otherwise.</param>
        /// <param name="projectionNodesForCurrentResourceType">List of all the properties that are being projected for the resource type, or null if no projections are applied.
        /// The list must be filtered to only nodes which apply to the current resource type.</param>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "collection", Justification = "Used in debug builds.")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "WebUtil.Dispose is disposing the QueryResultInfo instance for every navigation property.")]
        private void WriteNavigationProperties(IExpandedResult expanded, EntityToSerialize entityToSerialize, bool resourceInstanceInFeed, IEnumerable<ProjectionNode> projectionNodesForCurrentResourceType)
        {
            Debug.Assert(entityToSerialize != null, "entityToSerialize != null");
            Debug.Assert(
                projectionNodesForCurrentResourceType == null ||
                projectionNodesForCurrentResourceType.All(projectionNode => projectionNode.TargetResourceType.IsAssignableFrom(entityToSerialize.ResourceType)),
                "The projection nodes must be filtered to the current resource type only.");

            IEnumerable<ResourceProperty> navProperties =
                projectionNodesForCurrentResourceType == null
                ? this.Provider.GetResourceSerializableProperties(this.CurrentContainer, entityToSerialize.ResourceType).Where(p => p.TypeKind == ResourceTypeKind.EntityType)
                : projectionNodesForCurrentResourceType.Where(p => p.Property != null && p.Property.TypeKind == ResourceTypeKind.EntityType).Select(p1 => p1.Property);

            foreach (ResourceProperty property in navProperties)
            {
                ResourcePropertyInfo navProperty = this.GetNavigationPropertyInfo(expanded, entityToSerialize.Entity, entityToSerialize.ResourceType, property);
                ODataNestedResourceInfo navLink = this.GetNavigationLink(entityToSerialize, navProperty.Property);

                // WCF Data Services show performance degradation with JsonLight when entities have > 25 Navgations on writing entries
                // DEVNOTE: for performance reasons, if the link has no content due to the metadata level, and is not expanded
                // then don't tell ODataLib about it at all.
                if (navLink.Url == null && navLink.AssociationLinkUrl == null && !navProperty.Expand)
                {
                    continue;
                }

                var linkArgs = new DataServiceODataWriterNestedResourceInfoArgs(navLink, this.Service.OperationContext);
                this.dataServicesODataWriter.WriteStart(linkArgs);

                if (navProperty.Expand)
                {
                    object navPropertyValue = navProperty.Value;
                    IExpandedResult navPropertyExpandedResult = navPropertyValue as IExpandedResult;
                    object expandedPropertyValue =
                                navPropertyExpandedResult != null ?
                                GetExpandedElement(navPropertyExpandedResult) :
                                navPropertyValue;

                    bool needPop = this.PushSegmentForProperty(navProperty.Property, entityToSerialize.ResourceType, navProperty.ExpandedNode);

                    // if this.CurrentContainer is null, the target set of the navigation property is hidden.
                    if (this.CurrentContainer != null)
                    {
                        if (navProperty.Property.Kind == ResourcePropertyKind.ResourceSetReference)
                        {
                            IEnumerable enumerable;
                            bool collection = WebUtil.IsElementIEnumerable(expandedPropertyValue, out enumerable);
                            Debug.Assert(collection, "metadata loading must have ensured that navigation set properties must implement IEnumerable");

                            QueryResultInfo queryResults = new QueryResultInfo(enumerable);
                            try
                            {
                                queryResults.MoveNext();
                                Func<Uri> getNavPropertyRelativeUri = () => RequestUriProcessor.AppendUnescapedSegment(entityToSerialize.SerializedKey.RelativeEditLink, navLink.Name);
                                Func<Uri> getNavPropertyAbsoluteUri = () => RequestUriProcessor.AppendUnescapedSegment(entityToSerialize.SerializedKey.AbsoluteEditLink, navLink.Name);
                                this.WriteFeedElements(navPropertyExpandedResult, queryResults, navProperty.Property.ResourceType, navLink.Name, getNavPropertyRelativeUri, getNavPropertyAbsoluteUri, false);
                            }
                            finally
                            {
                                // After the navigation property is completely serialized, dispose the property value.
                                WebUtil.Dispose(queryResults);
                            }
                        }
                        else if (WebUtil.IsNullValue(expandedPropertyValue))
                        {
                            // Write a null reference navigation property
                            var entryArgs = new DataServiceODataWriterEntryArgs(null, null, this.Service.OperationContext);
                            this.dataServicesODataWriter.WriteStart(entryArgs);
                            this.dataServicesODataWriter.WriteEnd(entryArgs);
                        }
                        else
                        {
                            this.WriteEntry(navPropertyExpandedResult, expandedPropertyValue, resourceInstanceInFeed, navProperty.Property.ResourceType);
                        }
                    }

                    this.PopSegmentName(needPop);
                }

                this.dataServicesODataWriter.WriteEnd(linkArgs); // end of navigation property
            }
        }

        /// <summary>
        /// Creates the navigation link for the given navigation property.
        /// </summary>
        /// <param name="entityToSerialize">Entity that is currently being serialized.</param>
        /// <param name="navigationProperty">The metadata for the navigation property.</param>
        /// <returns>The navigation link for the given property.</returns>
        private ODataNestedResourceInfo GetNavigationLink(EntityToSerialize entityToSerialize, ResourceProperty navigationProperty)
        {
            Debug.Assert(entityToSerialize != null, "entityToSerialize != null");
            Debug.Assert(navigationProperty != null, "navigationProperty != null");

            string navLinkName = navigationProperty.Name;
            ODataNestedResourceInfo navLink = new ODataNestedResourceInfo
            {
                Name = navLinkName,
                IsCollection = navigationProperty.Kind == ResourcePropertyKind.ResourceSetReference
            };

            // Always pass the relative uri to the ODatalib. For Json, they will convert the relative uri
            // into absolute uri using the BaseUri property on the ODataWriterSettings. For atom, ODataLib
            // will write the relative uri.
            this.PayloadMetadataPropertyManager.SetUrl(navLink, () => RequestUriProcessor.AppendUnescapedSegment(entityToSerialize.SerializedKey.RelativeEditLink, navLinkName));

            if (this.Service.Configuration.DataServiceBehavior.IncludeAssociationLinksInResponse)
            {
                this.PayloadMetadataPropertyManager.SetAssociationLinkUrl(navLink, () => GetAssociationLinkUrl(entityToSerialize, navigationProperty));
            }

            return navLink;
        }

        /// <summary>Gets ODataProperty for the given <paramref name="property"/>.</summary>
        /// <param name="entityToSerialize">Entity that is currently being serialized.</param>
        /// <param name="property">ResourceProperty instance in question.</param>
        /// <returns>A instance of ODataProperty for the given <paramref name="property"/>.</returns>
        private ODataProperty GetODataPropertyForEntityProperty(EntityToSerialize entityToSerialize, ResourceProperty property)
        {
            Debug.Assert(entityToSerialize != null, "entityToSerialize != null");
            Debug.Assert(property != null && entityToSerialize.ResourceType.Properties.Contains(property), "property != null && currentResourceType.Properties.Contains(property)");

            ODataValue odataPropertyValue;
            if (property.IsOfKind(ResourcePropertyKind.Stream))
            {
                odataPropertyValue = this.GetNamedStreamPropertyValue(entityToSerialize, property);
            }
            else
            {
                object propertyValue = WebUtil.GetPropertyValue(this.Provider, entityToSerialize.Entity, entityToSerialize.ResourceType, property, null);
                odataPropertyValue = this.GetPropertyValue(property.Name, property.ResourceType, propertyValue, false /*openProperty*/);
            }

            ODataProperty odataProperty = new ODataProperty()
            {
                Name = property.Name,
                Value = odataPropertyValue
            };

            ODataPropertyKind kind = property.IsOfKind(ResourcePropertyKind.Key) ? ODataPropertyKind.Key : property.IsOfKind(ResourcePropertyKind.ETag) ? ODataPropertyKind.ETag : ODataPropertyKind.Unspecified;
            if (kind != ODataPropertyKind.Unspecified)
            {
                odataProperty.SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = kind });
            }

            return odataProperty;
        }

        /// <summary>Gets ODataProperty for the given <paramref name="property"/>.</summary>
        /// <param name="entityToSerialize">Entity that is currently being serialized.</param>
        /// <param name="property">ResourceProperty instance in question.</param>
        /// <returns>A instance of ODataProperty for the given <paramref name="property"/>.</returns>
        private ODataNestedResourceInfoWrapper GetODataNestedResourceForComplexProperty(EntityToSerialize entityToSerialize, ResourceProperty property)
        {
            Debug.Assert(entityToSerialize != null, "entityToSerialize != null");
            Debug.Assert(property != null && entityToSerialize.ResourceType.Properties.Contains(property), "property != null && currentResourceType.Properties.Contains(property)");

            ODataItemWrapper odataPropertyResourceWrapper;
            object propertyValue = WebUtil.GetPropertyValue(this.Provider, entityToSerialize.Entity, entityToSerialize.ResourceType, property, null);
            odataPropertyResourceWrapper = this.GetPropertyResourceOrResourceSet(property.Name, property.ResourceType, propertyValue, false /*openProperty*/);

            ODataNestedResourceInfo odataNestedInfo = new ODataNestedResourceInfo()
            {
                Name = property.Name,
                IsCollection = property.Kind == ResourcePropertyKind.Collection,
            };
            odataNestedInfo.SetSerializationInfo(new ODataNestedResourceInfoSerializationInfo() { IsComplex = true });

            return new ODataNestedResourceInfoWrapper()
            {
                NestedResourceInfo = odataNestedInfo,
                NestedResourceOrResourceSet = odataPropertyResourceWrapper
            };
        }

        /// <summary>
        /// Returns the instance of ODataProperty with the given name and property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">Value of the property.</param>
        /// <returns>An instance of ODataProperty for the given property.</returns>
        private ODataProperty GetODataPropertyForOpenProperty(string propertyName, object propertyValue)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            ResourceType propertyResourceType;
            if (WebUtil.IsNullValue(propertyValue))
            {
                propertyResourceType = ResourceType.PrimitiveStringResourceType;
            }
            else
            {
                propertyResourceType = WebUtil.GetResourceType(this.Provider, propertyValue);

                // A null ResourceType indicates a blacklisted type (eg, IntPtr or DateTimeOffset). So fail on it.
                if (propertyResourceType == null)
                {
                    throw new DataServiceException(500, Microsoft.OData.Service.Strings.Syndication_InvalidOpenPropertyType(propertyName));
                }
            }

            ODataProperty odataProperty = new ODataProperty
            {
                Name = propertyName,
                Value = this.GetPropertyValue(propertyName, propertyResourceType, propertyValue, true /*openProperty*/)
            };

            odataProperty.SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Open });
            return odataProperty;
        }

        /// <summary>
        /// Tries to build the set of <see cref="ODataAction"/> to be advertised for the given resource.
        /// </summary>
        /// <param name="entityToSerialize">Entity that is currently being serialized.</param>
        /// <param name="resourceInstanceInFeed">true if the resource instance being serialized is inside a feed; false otherwise.</param>
        /// <param name="actions">The actions to advertise.</param>
        /// <returns>
        /// Whether any actions should be advertised.
        /// </returns>
        private bool TryGetAdvertisedActions(EntityToSerialize entityToSerialize, bool resourceInstanceInFeed, out IEnumerable<ODataAction> actions)
        {
            Debug.Assert(entityToSerialize != null, "entityToSerialize != null");

            if (!this.Service.ActionProvider.IsImplemented)
            {
                actions = null;
                return false;
            }

            // If GetOperationProjections returns null it means no projections are to be applied and all operations
            // for the current segment should be serialized. If it returns non-null only the operations described
            // by the returned projection segments should be serialized.
            ExpandedProjectionNode expandedProjectionNode = this.GetCurrentExpandedProjectionNode();

            List<OperationWrapper> serviceOperationWrapperList;
            if (expandedProjectionNode == null || expandedProjectionNode.ProjectAllOperations)
            {
                // Note that if the service does not implement IDataServiceActionProvider and the MaxProtocolVersion < V3,
                // GetServiceActionsByBindingParameterType() would simply return an empty ServiceOperationWrapper collection.
                serviceOperationWrapperList = this.Service.ActionProvider.GetServiceActionsByBindingParameterType(entityToSerialize.ResourceType);
            }
            else
            {
                serviceOperationWrapperList = expandedProjectionNode.SelectedOperations.GetSelectedOperations(entityToSerialize.ResourceType);
            }

            Debug.Assert(serviceOperationWrapperList != null, "serviceOperationWrappers != null");
            if (serviceOperationWrapperList.Count <= 0)
            {
                actions = null;
                return false;
            }

            actions = this.OperationSerializer.SerializeOperations(entityToSerialize, resourceInstanceInFeed, serviceOperationWrapperList);
            return true;
        }

        /// <summary>
        /// Creates an ODataWriter for writing an entry or a feed.
        /// </summary>
        /// <param name="forFeed">true when writing a feed; false when writing an entry.</param>
        /// <returns>The ODataWriter to use for writing the feed or entry.</returns>
        private DataServiceODataWriter CreateODataWriter(bool forFeed)
        {
            IEdmModel model = this.Service.Provider.GetMetadataProviderEdmModel();
            Debug.Assert(model != null, "model != null");

            IEdmEntitySet entitySet = null;
            IEdmEntityType entityType = null;
            if (!ContentTypeUtil.IsResponseMediaTypeJsonLight(this.Service, /*isEntryOrFeed*/ true))
            {
                entitySet = WebUtil.GetEntitySet(this.Service.Provider, model, this.RequestDescription.TargetResourceSet);
                entityType = (IEdmEntityType)model.FindType(this.RequestDescription.TargetResourceType.FullName);
            }

            ODataWriter odataWriter = forFeed
                ? this.messageWriter.CreateODataResourceSetWriter(entitySet, entityType)
                : this.messageWriter.CreateODataResourceWriter(entitySet, entityType);

            return this.Service.CreateODataWriterWrapper(odataWriter);
        }

        /// <summary>
        /// Wraps the entity in a structure which tracks its type and other information about it.
        /// </summary>
        /// <param name="entity">The entity to wrap.</param>
        /// <param name="resourceType">The type of the entity.</param>
        /// <returns>A structure containing the entity and some other information about it.</returns>
        private EntityToSerialize WrapEntity(object entity, ResourceType resourceType)
        {
            return EntityToSerialize.Create(entity, resourceType, this.CurrentContainer, this.Provider, this.AbsoluteServiceUri);
        }
    }
}
