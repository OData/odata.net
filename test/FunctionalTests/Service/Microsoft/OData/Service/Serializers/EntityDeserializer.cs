//---------------------------------------------------------------------
// <copyright file="EntityDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// Implements deserializer for entities.
    /// </summary>
    internal sealed class EntityDeserializer : ODataMessageReaderDeserializer
    {
        private readonly IDictionary<ODataResource, ODataEntryAnnotation> entryAnnotations =
            new Dictionary<ODataResource, ODataEntryAnnotation>();
        private readonly IDictionary<ODataResourceSet, ODataFeedAnnotation> feedAnnotations =
            new Dictionary<ODataResourceSet, ODataFeedAnnotation>();
        private readonly IDictionary<ODataNestedResourceInfo, ODataNavigationLinkAnnotation> navigationLinkAnnotations =
            new Dictionary<ODataNestedResourceInfo, ODataNavigationLinkAnnotation>();

        /// <summary>
        /// Initializes a new instance of <see cref="EntityDeserializer"/>.
        /// </summary>
        /// <param name="update">true if we're reading an update operation; false if not.</param>
        /// <param name="dataService">Data service for which the deserializer will act.</param>
        /// <param name="tracker">Tracker to use for modifications.</param>
        /// <param name="requestDescription">The request description to use.</param>
        internal EntityDeserializer(bool update, IDataService dataService, UpdateTracker tracker, RequestDescription requestDescription)
            : base(update, dataService, tracker, requestDescription, true /*enableODataServerBehavior*/)
        {
        }

        /// <summary>
        /// Reads the input request payload and returns the WCF DS value representation of it.
        /// </summary>
        /// <param name="segmentInfo">Info about the request to read.</param>
        /// <returns>The WCF DS representation of the value read.</returns>
        protected override object Read(SegmentInfo segmentInfo)
        {
            Debug.Assert(segmentInfo != null, "segmentInfo != null");
            Debug.Assert(segmentInfo.TargetKind == RequestTargetKind.Resource, "The EntityDeserializer only supports Resource target kinds.");

            ResourceType expectedResourceType = segmentInfo.TargetResourceType;
            Debug.Assert(expectedResourceType != null, "To read an entity we must know the expected resource type of the entity to read.");
            Debug.Assert(expectedResourceType.ResourceTypeKind == ResourceTypeKind.EntityType, "Only entity types can be used as types for entities.");
            IEdmEntityType expectedEntityType = (IEdmEntityType)this.GetSchemaType(expectedResourceType);

            MetadataProviderEdmModel metadataProviderEdmModel = this.Service.Provider.GetMetadataProviderEdmModel();
            Debug.Assert(metadataProviderEdmModel.Mode == MetadataProviderEdmModelMode.Serialization, "Model expected to be in serialization mode.");

            IEdmEntitySet expectedEntitySet = WebUtil.GetEntitySet(this.Service.Provider, metadataProviderEdmModel, segmentInfo.TargetResourceSet);
            ODataReader odataReader = this.MessageReader.CreateODataResourceReader(expectedEntitySet, expectedEntityType);
            AssertReaderFormatIsExpected(this.MessageReader, ODataFormat.Json);

            // Read the entry and all its children into a tree. We use annotation to connect the items into the tree.
            // Note that we must cache the entire payload to preserve call order for navigation properties.
            // Due to the fact that the payload order on the wire is arbitrary, but we always set all non-navigation properties first
            // and then apply all navigation properties, we must cache the entire tree.
            ODataResource topLevelEntry = this.ReadEntry(odataReader, segmentInfo);

            ODataEntryAnnotation topLevelEntryAnnotation = this.entryAnnotations[topLevelEntry];
            Debug.Assert(topLevelEntryAnnotation != null, "Each entry we read must have the entry annotation.");
            this.RecurseEnter();
            this.ApplyEntityProperties(segmentInfo, topLevelEntry, topLevelEntryAnnotation);
            this.RecurseLeave();

            return topLevelEntryAnnotation;
        }

        /// <summary>
        /// Reads an entry from the <paramref name="odataReader"/> and all it's children including expanded entries and feeds.
        /// </summary>
        /// <param name="odataReader">The ODataReader to read from.</param>
        /// <param name="topLevelSegmentInfo">The segment info for the top-level entry to read.</param>
        /// <returns>The <see cref="ODataResource"/> with annotations which store the navigation links and their expanded values.</returns>
        private ODataResource ReadEntry(ODataReader odataReader, SegmentInfo topLevelSegmentInfo)
        {
            Debug.Assert(odataReader != null, "odataReader != null");
            Debug.Assert(odataReader.State == ODataReaderState.Start, "The ODataReader must not have been used yet.");
            Debug.Assert(topLevelSegmentInfo != null, "topLevelSegmentInfo != null");

            ODataResource topLevelEntry = null;
            Stack<ODataItem> itemsStack = new Stack<ODataItem>();
            while (odataReader.Read())
            {
                // Don't let the item stack grow larger than we can process later. Also lets us to fail and report the recursion depth limit error before ODL does.
                if (itemsStack.Count >= RecursionLimit)
                {
                    throw DataServiceException.CreateDeepRecursion(RecursionLimit);
                }

                switch (odataReader.State)
                {
                    case ODataReaderState.ResourceStart:
                        ODataResource entry = (ODataResource)odataReader.Item;
                        ODataEntryAnnotation entryAnnotation = null;
                        if (entry != null)
                        {
                            entryAnnotation = new ODataEntryAnnotation();
                            this.entryAnnotations[entry] = entryAnnotation;
                        }

                        if (itemsStack.Count == 0)
                        {
                            Debug.Assert(entry != null, "The top-level entry can never be null.");
                            topLevelEntry = entry;

                            // For top-level entry, create the entry resource here.
                            // This is needed since the creation of the resource may fail (especially if this is an update in which case
                            // we evaluate the URL query, which may return no results and thus we would fail with 404)
                            // and we need to report that failure rather then potential other failures caused by the properties in the entry in question.
                            this.CreateEntityResource(topLevelSegmentInfo, entry, entryAnnotation, /*topLevel*/ true);
                        }
                        else
                        {
                            ODataItem parentItem = itemsStack.Peek();
                            ODataResourceSet parentFeed = parentItem as ODataResourceSet;
                            if (parentFeed != null)
                            {
                                ODataFeedAnnotation parentFeedAnnotation = this.feedAnnotations[parentFeed];
                                Debug.Assert(parentFeedAnnotation != null, "Every feed we added to the stack should have the feed annotation on it.");
                                parentFeedAnnotation.Add(entry);
                            }
                            else
                            {
                                ODataNestedResourceInfo parentNavigationLink = (ODataNestedResourceInfo)parentItem;
                                ODataNavigationLinkAnnotation parentNavigationLinkAnnotation = this.navigationLinkAnnotations[parentNavigationLink];
                                Debug.Assert(parentNavigationLinkAnnotation != null, "Every navigation link we added to the stack should have the navigation link annotation on it.");

                                Debug.Assert(parentNavigationLink.IsCollection == false, "Only singleton navigation properties can contain entry as their child.");
                                Debug.Assert(parentNavigationLinkAnnotation.Count == 0, "Each navigation property can contain only one entry as its direct child.");
                                parentNavigationLinkAnnotation.Add(entry);
                            }
                        }

                        itemsStack.Push(entry);
                        break;

                    case ODataReaderState.ResourceEnd:
                        Debug.Assert(itemsStack.Count > 0 && itemsStack.Peek() == odataReader.Item, "The entry which is ending should be on the top of the items stack.");
                        itemsStack.Pop();
                        break;

                    case ODataReaderState.NestedResourceInfoStart:
                        ODataNestedResourceInfo navigationLink = (ODataNestedResourceInfo)odataReader.Item;
                        Debug.Assert(navigationLink != null, "Navigation link should never be null.");

                        this.navigationLinkAnnotations[navigationLink] = new ODataNavigationLinkAnnotation();
                        Debug.Assert(itemsStack.Count > 0, "Navigation link can't appear as top-level item.");
                        {
                            ODataResource parentEntry = (ODataResource)itemsStack.Peek();
                            ODataEntryAnnotation parentEntryAnnotation = this.entryAnnotations[parentEntry];
                            Debug.Assert(parentEntryAnnotation != null, "Every entry we added to the stack should have the navigation link annotation on it.");
                            parentEntryAnnotation.Add(navigationLink);
                        }

                        itemsStack.Push(navigationLink);
                        break;

                    case ODataReaderState.NestedResourceInfoEnd:
                        Debug.Assert(itemsStack.Count > 0 && itemsStack.Peek() == odataReader.Item, "The navigation link which is ending should be on the top of the items stack.");
                        itemsStack.Pop();
                        break;

                    case ODataReaderState.ResourceSetStart:
                        ODataResourceSet feed = (ODataResourceSet)odataReader.Item;
                        Debug.Assert(feed != null, "Feed should never be null.");

                        this.feedAnnotations[feed] = new ODataFeedAnnotation();
                        Debug.Assert(itemsStack.Count > 0, "Since we always start reading entry, we should never get a feed as the top-level item.");
                        {
                            ODataNestedResourceInfo parentNavigationLink = (ODataNestedResourceInfo)itemsStack.Peek();
                            ODataNavigationLinkAnnotation parentNavigationLinkAnnotation = this.navigationLinkAnnotations[parentNavigationLink];
                            Debug.Assert(parentNavigationLinkAnnotation != null, "Every navigation link we added to the stack should have the navigation link annotation on it.");

                            Debug.Assert(parentNavigationLink.IsCollection == true, "Only collection navigation properties can contain feed as their child.");
                            parentNavigationLinkAnnotation.Add(feed);
                        }

                        itemsStack.Push(feed);
                        break;

                    case ODataReaderState.ResourceSetEnd:
                        Debug.Assert(itemsStack.Count > 0 && itemsStack.Peek() == odataReader.Item, "The feed which is ending should be on the top of the items stack.");
                        itemsStack.Pop();
                        break;

                    case ODataReaderState.EntityReferenceLink:
                        ODataEntityReferenceLink entityReferenceLink = (ODataEntityReferenceLink)odataReader.Item;
                        Debug.Assert(entityReferenceLink != null, "Entity reference link should never be null.");

                        Debug.Assert(itemsStack.Count > 0, "Entity reference link should never be reported as top-level item.");
                        {
                            ODataNestedResourceInfo parentNavigationLink = (ODataNestedResourceInfo)itemsStack.Peek();
                            ODataNavigationLinkAnnotation parentNavigationLinkAnnotation = this.navigationLinkAnnotations[parentNavigationLink];
                            Debug.Assert(parentNavigationLinkAnnotation != null, "Every navigation link we added to the stack should have the navigation link annotation on it.");

                            parentNavigationLinkAnnotation.Add(entityReferenceLink);
                        }

                        break;

                    default:
                        Debug.Assert(false, "We should never get here, it means the ODataReader reported a wrong state.");
                        break;
                }
            }

            Debug.Assert(odataReader.State == ODataReaderState.Completed, "We should have consumed all of the input by now.");
            Debug.Assert(topLevelEntry != null, "A top level entry should have been read by now.");
            return topLevelEntry;
        }

        /// <summary>
        /// Creates or gets an entity resource token instance based on the data from entry in the payload.
        /// The resource is then set on the entry annotation.
        /// </summary>
        /// <param name="segmentInfo">The segment info describing the entity in question.</param>
        /// <param name="entry">The OData entry instance read from the payload.</param>
        /// <param name="entryAnnotation">The entry annotation for the entry to process.</param>
        /// <param name="topLevel">true if this is a top-level entry, false otherwise.</param>
        private void CreateEntityResource(SegmentInfo segmentInfo, ODataResource entry, ODataEntryAnnotation entryAnnotation, bool topLevel)
        {
            Debug.Assert(segmentInfo != null, "segmentInfo != null");
            Debug.Assert(entry != null, "Null entries should not be tried to translated to entity instances, instead they should be handled separately.");
            Debug.Assert(entryAnnotation != null, "entryAnnotation != null");

            // Note that this method does not call RecurseEnter and RecurseLeave.
            // It is going to be called when reading the top-level entry during entry reading (in which case it would count 1, and not fail anyway)
            // and then during entity materialization, but then we're going to call the ApplyEntityProperties on all the entities
            // and so we will count the recursion limits there instead.

            // Update the object count everytime you encounter a new resource.
            this.CheckAndIncrementObjectCount();

            // Get the type from the entry.
            ResourceType entityResourceType = this.GetEntryResourceType(entry, segmentInfo.TargetResourceType);
            Debug.Assert(entityResourceType != null
                && (entityResourceType.ResourceTypeKind == ResourceTypeKind.EntityType
                || entityResourceType.ResourceTypeKind == ResourceTypeKind.ComplexType), "Each entity must have an entity type.");

            // We have the actual type info from the payload. Update the request/response DSV based on the particular type.
            this.UpdateAndCheckRequestResponseDSV(entityResourceType, topLevel);

            if (entityResourceType.ResourceTypeKind != ResourceTypeKind.ComplexType
                && segmentInfo.TargetKind == RequestTargetKind.OpenProperty)
            {
                // Open navigation properties are not supported on OpenTypes.
                throw DataServiceException.CreateBadRequestError(Microsoft.OData.Service.Strings.OpenNavigationPropertiesNotSupportedOnOpenTypes(segmentInfo.Identifier));
            }

            Debug.Assert(segmentInfo.TargetKind == RequestTargetKind.OpenProperty
                || segmentInfo.TargetKind == RequestTargetKind.Resource, "Only resource targets can accept entity payloads.");

            object entityResource;

            if (entityResourceType.ResourceTypeKind == ResourceTypeKind.EntityType)
            {
                if (this.Update)
                {
                    Debug.Assert(topLevel, "Updates don't allow deep payload, only deep links.");

                    // If it's a top level resource, then it cannot be null.
                    // [Astoria-ODataLib-Integration] WCF DS Server doesn't check ETags if an ATOM payload entry has no content and no links (and it's a V1 entry)
                    // We decided to break WCF DS and always check ETag - seems like a security issue.
                    entityResource = this.GetObjectFromSegmentInfo(
                        entityResourceType,
                        segmentInfo,
                        true /*verifyETag*/,
                        true /*checkForNull*/,
                        this.Service.OperationContext.RequestMessage.HttpVerb == HttpVerbs.PUT /*replaceResource*/);
                }
                else
                {
                    // Check for append rights whenever we need to create a resource.
                    DataServiceConfiguration.CheckResourceRights(segmentInfo.TargetResourceSet, EntitySetRights.WriteAppend);
                    // Create new instance of the entity.
                    entityResource = this.Updatable.CreateResource(segmentInfo.TargetResourceSet == null ? null : segmentInfo.TargetResourceSet.Name, entityResourceType.FullName);
                }
            }
            else
            {
                // For complex type, always create a new instance instead of update the original one.
                entityResource = this.Updatable.CreateResource(segmentInfo.TargetResourceSet == null ? null : segmentInfo.TargetResourceSet.Name, entityResourceType.FullName);
            }

            entryAnnotation.EntityResource = entityResource;
            entryAnnotation.EntityResourceType = entityResourceType;
        }

        /// <summary>
        /// Applies properties and navigation links to the entity resource token instance based on the data from entry in the payload.
        /// </summary>
        /// <param name="segmentInfo">The segment info describing the entity in question.</param>
        /// <param name="entry">The OData entry instance read from the payload.</param>
        /// <param name="entryAnnotation">The entry annotation for the entry to process.</param>
        private void ApplyEntityProperties(SegmentInfo segmentInfo, ODataResource entry, ODataEntryAnnotation entryAnnotation)
        {
            Debug.Assert(segmentInfo != null, "segmentInfo != null");
            Debug.Assert(entry != null, "Null entries should not be tried to translated to entity instances, instead they should be handled separately.");
            Debug.Assert(entryAnnotation != null, "entryAnnotation != null");

            // Note that recursion count is handled by the caller of this method.
            // We need this since we need to call it around the entire CreateEntryResource + ApplyEntityProperties combo for the nested entries
            // and will call it separately for the top-level entry at the very root.
            object entityResource = entryAnnotation.EntityResource;
            ResourceType entityResourceType = entryAnnotation.EntityResourceType;

            // First apply all non-navigation properties.
            this.ApplyValueProperties(entry, entityResourceType, entityResource);

            this.ApplyComplexProperties(entryAnnotation, entityResourceType, entityResource);
            // And then apply all navigation properties.
            this.ApplyNavigationProperties(entryAnnotation, segmentInfo.TargetResourceSet, entityResourceType, entityResource);

            Debug.Assert(this.Tracker != null, "this.Tracker != null");

            if (entityResourceType.ResourceTypeKind == ResourceTypeKind.EntityType)
            {
                // Track the operation
                if (this.Update)
                {
                    // Originally JSON used to fire the change interceptor only if something actually changed
                    // We decided to make it consistent and fire the change interceptor always.
                    this.Tracker.TrackAction(entityResource, segmentInfo.TargetResourceSet, UpdateOperations.Change);
                }
                else
                {
                    this.Tracker.TrackAction(entityResource, segmentInfo.TargetResourceSet, UpdateOperations.Add);
                }
            }
        }

        /// <summary>
        /// Create the entity resource update token and applies properties and navigation links to the entity resource token instance 
        /// based on the data from entry in the payload.
        /// </summary>
        /// <param name="segmentInfo">The segment info describing the entity in question.</param>
        /// <param name="entry">The OData entry instance read from the payload.</param>
        /// <returns>The entity resource update token for the created entity.</returns>
        /// <remarks>This method should only be called on nested entries!</remarks>
        private object CreateNestedEntityAndApplyProperties(SegmentInfo segmentInfo, ODataResource entry)
        {
            Debug.Assert(segmentInfo != null, "segmentInfo != null");
            Debug.Assert(entry != null, "entry != null");

            ODataEntryAnnotation entryAnnotation = this.entryAnnotations[entry];
            Debug.Assert(entryAnnotation != null, "Each entry we read must have our entry annotation.");

            this.RecurseEnter();
            this.CreateEntityResource(segmentInfo, entry, entryAnnotation, /*topLevel*/ false);
            this.ApplyEntityProperties(segmentInfo, entry, entryAnnotation);
            this.RecurseLeave();

            return entryAnnotation.EntityResource;
        }

        /// <summary>
        /// Gets the resource type for an entry based on the type name from the payload.</summary>
        /// <param name="entry">The entry to get the type for.</param>
        /// <param name="expectedType">Expected base type for the entity.</param>
        /// <returns>Resolved type.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDireclyAccessPayloadMetadataProperties", Justification = "This component is allowed to access these properties.")]
        private ResourceType GetEntryResourceType(ODataResource entry, ResourceType expectedType)
        {
            string payloadTypeName = entry.TypeName;

            // either payloadTypeName or expectedType
            if (string.IsNullOrEmpty(payloadTypeName))
            {
                Debug.Assert(entry != null, "entry != null");
                Debug.Assert(expectedType != null, "We must always have an expected type for entities.");
                Debug.Assert(expectedType.ResourceTypeKind == ResourceTypeKind.EntityType
                    || expectedType.ResourceTypeKind == ResourceTypeKind.ComplexType, "Expected type for entities must be an structured type.");
            }

            ResourceType resourceType;

            // Note that we can't rely on the ODataLib handling in this case completely.
            // In the WCF DS Server mode the ODataLib uses the lax validation, which means that it doesn't actually
            // validate almost anything (to follow the same logic as used for complex types).
            // So we have to implement the validation logic here on top of ODataLib.
            // We also can't use the type name reported by the entry property always, as that is the resolved type name.
            // It's safer to use the actual type name from the payload if it's available.
            if (entry.TypeAnnotation != null)
            {
                payloadTypeName = entry.TypeAnnotation.TypeName;
            }

            // If the type is not specified in the payload, we assume the type to be the expected type.
            if (String.IsNullOrEmpty(payloadTypeName))
            {
                // Check if the expected type takes part in inheritance.
                resourceType = expectedType;
                if (this.Service.Provider.HasDerivedTypes(resourceType))
                {
                    throw DataServiceException.CreateBadRequestError(Microsoft.OData.Service.Strings.BadRequest_TypeInformationMustBeSpecifiedForInhertiance);
                }
            }
            else
            {
                // Otherwise, try and resolve the name specified in the payload.
                resourceType = this.Service.Provider.TryResolveResourceType(payloadTypeName);
                if (resourceType == null)
                {
                    throw DataServiceException.CreateBadRequestError(Microsoft.OData.Service.Strings.BadRequest_InvalidTypeName(payloadTypeName));
                }

                if (expectedType != null && !expectedType.IsAssignableFrom(resourceType))
                {
                    throw DataServiceException.CreateBadRequestError(Microsoft.OData.Service.Strings.BadRequest_InvalidTypeSpecified(payloadTypeName, expectedType.FullName));
                }
            }

            return resourceType;
        }

        /// <summary>
        /// Applies non-navigation properties.
        /// </summary>
        /// <param name="entry">The entry object to read the properties from.</param>
        /// <param name="entityResourceType">The type of the entity to apply the properties to.</param>
        /// <param name="entityResource">The entity resource to apply the properties to.</param>
        private void ApplyValueProperties(ODataResource entry, ResourceType entityResourceType, object entityResource)
        {
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(entityResourceType != null, "entityResourceType != null");
            Debug.Assert(entityResourceType.ResourceTypeKind == ResourceTypeKind.EntityType
                || entityResourceType.ResourceTypeKind == ResourceTypeKind.ComplexType, "Only entity types can be specified for entities.");
            Debug.Assert(entityResource != null, "entityResource != null");

            Debug.Assert(entry.Properties != null, "The ODataLib reader should always populate the ODataResource.Properties collection.");
            foreach (ODataProperty entryProperty in entry.Properties)
            {
                this.ApplyProperty(entryProperty, entityResourceType, entityResource);
            }
        }

        /// <summary>
        /// Applies navigation properties.
        /// </summary>
        /// <param name="entryAnnotation">The entry annotation for the entry to process.</param>
        /// <param name="entityResourceSet">The resource set into which the entity belongs to.</param>
        /// <param name="entityResourceType">The type of the entity to apply the properties to.</param>
        /// <param name="entityResource">The entity resource to apply the properties to.</param>
        private void ApplyNavigationProperties(
            ODataEntryAnnotation entryAnnotation,
            ResourceSetWrapper entityResourceSet,
            ResourceType entityResourceType,
            object entityResource)
        {
            Debug.Assert(entryAnnotation != null, "entryAnnotation != null");
            Debug.Assert(entityResourceType != null, "entityResourceType != null");
            Debug.Assert(entityResourceType.ResourceTypeKind == ResourceTypeKind.EntityType
                || entityResourceType.ResourceTypeKind == ResourceTypeKind.ComplexType, "Only entity types can be specified for entities.");
            Debug.Assert(entityResource != null, "entityResource != null");

            foreach (ODataNestedResourceInfo navigationLink in entryAnnotation)
            {
                ResourceProperty navigationProperty = entityResourceType.TryResolvePropertyName(navigationLink.Name, exceptKind: ResourcePropertyKind.Stream);
                if (navigationProperty != null)
                {
                    Debug.Assert(navigationProperty != null, "ODataLib reader should have already validated that all navigation properties are declared and none is open.");
                    Debug.Assert(navigationProperty.TypeKind == ResourceTypeKind.EntityType
                        || navigationProperty.TypeKind == ResourceTypeKind.ComplexType
                        || (navigationProperty.TypeKind == ResourceTypeKind.Collection
                        && navigationProperty.ResourceType.ElementType().ResourceTypeKind == ResourceTypeKind.ComplexType), "Navigation properties should already be validated by ODataLib to be of entity types.");

                    if (navigationProperty.ResourceType.ResourceTypeKind == ResourceTypeKind.EntityType)
                    {
                        this.ApplyNavigationProperty(navigationLink, entityResourceSet, entityResourceType, navigationProperty, entityResource);
                    }
                }
            }
        }

        private void ApplyComplexProperties(
            ODataEntryAnnotation entryAnnotation,
            ResourceType entityResourceType,
            object entityResource)
        {
            Debug.Assert(entryAnnotation != null, "entryAnnotation != null");
            Debug.Assert(entityResourceType != null, "entityResourceType != null");
            Debug.Assert(entityResourceType.ResourceTypeKind == ResourceTypeKind.ComplexType
                || entityResourceType.ResourceTypeKind == ResourceTypeKind.EntityType, "Only entity types can be specified for entities.");
            Debug.Assert(entityResource != null, "entityResource != null");

            foreach (ODataNestedResourceInfo navigationLink in entryAnnotation)
            {
                ResourceProperty navigationProperty = entityResourceType.TryResolvePropertyName(navigationLink.Name, exceptKind: ResourcePropertyKind.Stream);
                bool isUndeclaredProperty = (navigationProperty == null);
                if (isUndeclaredProperty || navigationProperty.TypeKind == ResourceTypeKind.ComplexType)
                {
                    this.ApplyComplexProperty(navigationLink, entityResourceType, navigationProperty, entityResource);
                }
            }
        }

        /// <summary>
        /// Applies a navigation link as a navigation property.
        /// </summary>
        /// <param name="navigationLink">The navigation link read from the payload to apply.</param>
        /// <param name="entityResourceSet">The resource set into which the entity belongs to.</param>
        /// <param name="entityResourceType">The type of the entity to apply the properties to.</param>
        /// <param name="navigationProperty">The navigation property which coresponds with the navigation link.</param>
        /// <param name="entityResource">The entity resource to apply the properties to.</param>
        private void ApplyNavigationProperty(
            ODataNestedResourceInfo navigationLink,
            ResourceSetWrapper entityResourceSet,
            ResourceType entityResourceType,
            ResourceProperty navigationProperty,
            object entityResource)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");
            Debug.Assert(entityResourceSet != null, "entityResourceSet != null");
            Debug.Assert(entityResourceType != null, "entityResourceType != null");
            Debug.Assert(entityResourceType.ResourceTypeKind == ResourceTypeKind.EntityType
                || entityResourceType.ResourceTypeKind == ResourceTypeKind.ComplexType, "Only entity types can be specified for entities.");
            Debug.Assert(navigationProperty != null
                && (navigationProperty.TypeKind == ResourceTypeKind.EntityType
                || navigationProperty.TypeKind == ResourceTypeKind.ComplexType)
                , "navigationProperty != null && navigationProperty.TypeKind == ResourceTypeKind.EntityType");
            Debug.Assert(navigationLink.Name == navigationProperty.Name, "The navigationProperty must have the same name as the navigationLink to apply.");
            Debug.Assert(entityResource != null, "entityResource != null");

            ODataNavigationLinkAnnotation navigationLinkAnnotation = this.navigationLinkAnnotations[navigationLink];
            Debug.Assert(navigationLinkAnnotation != null, "navigationLinkAnnotation != null");
            Debug.Assert(navigationLinkAnnotation.Count > 0, "Each navigation link must have at least one child in request.");
            Debug.Assert(navigationLink.IsCollection.HasValue, "We should know the cardinality of the navigation link by now.");

            // We have to allow IsCollection=false for ResourceSetReference properties as well.
            Debug.Assert(
                navigationLink.IsCollection == true || (navigationLinkAnnotation.Count == 1),
                "If the navigation link is a singleton, then there must be exactly one child in it and the property must be a singleton as well.");
            Debug.Assert(
                navigationLink.IsCollection == false || (navigationProperty.Kind == ResourcePropertyKind.ResourceSetReference),
                "If the navigation link is a collection, then the property must be a collection as well.");

            // Just loop through the navigation link content
            // Note that if the navigation link is a singleton it must have just one item in it, so it's OK to loop anyway.
            foreach (ODataItem childItem in navigationLinkAnnotation)
            {
                ODataEntityReferenceLink entityReferenceLink = childItem as ODataEntityReferenceLink;
                if (entityReferenceLink != null)
                {
                    this.ApplyEntityReferenceLinkInNavigationProperty(navigationProperty, entityResource, entityReferenceLink);
                    continue;
                }

                ResourceSetWrapper targetResourceSet = null;

                if (entityResourceType.ResourceTypeKind == ResourceTypeKind.EntityType)
                {
                    // Get the target resource set for the navigation property, this also causes validation of the resource set and thus migth fail.
                    // WCF DS Used to do this in ATOM only if the link was expanded, which was a security issue we decided to fix, so now we do it always.
                    targetResourceSet = this.GetNavigationPropertyTargetResourceSet(entityResourceSet, entityResourceType, navigationProperty);
                }

                ODataResourceSet feed = childItem as ODataResourceSet;
                if (feed != null)
                {
                    this.ApplyFeedInNavigationProperty(navigationProperty, targetResourceSet, entityResource, feed);
                    continue;
                }

                // It must be entry by now.
                ODataResource entry = (ODataResource)childItem;
                this.ApplyEntryInNavigationProperty(navigationProperty, targetResourceSet, entityResource, entry);
            }
        }

        private void ApplyComplexProperty(
            ODataNestedResourceInfo navigationLink,
            ResourceType entityResourceType,
            ResourceProperty navigationProperty,
            object entityResource)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");
            Debug.Assert(entityResourceType != null, "entityResourceType != null");
            Debug.Assert(entityResourceType.ResourceTypeKind == ResourceTypeKind.ComplexType ||
            entityResourceType.ResourceTypeKind == ResourceTypeKind.EntityType, "Only entity types can be specified for entities.");
            Debug.Assert(navigationProperty == null || navigationProperty.TypeKind == ResourceTypeKind.ComplexType
                , "navigationProperty == null || navigationProperty.TypeKind == ResourceTypeKind.ComplexType");
            Debug.Assert(navigationProperty == null || navigationLink.Name == navigationProperty.Name, "The navigationProperty must have the same name as the navigationLink to apply.");
            Debug.Assert(entityResource != null, "entityResource != null");

            ODataNavigationLinkAnnotation navigationLinkAnnotation = this.navigationLinkAnnotations[navigationLink];
            Debug.Assert(navigationLinkAnnotation != null, "navigationLinkAnnotation != null");
            Debug.Assert(navigationLinkAnnotation.Count > 0, "Each navigation link must have at least one child in request.");
            Debug.Assert(navigationLink.IsCollection.HasValue, "We should know the cardinality of the navigation link by now.");

            // We have to allow IsCollection=false for ResourceSetReference properties as well.
            Debug.Assert(
                navigationLink.IsCollection == true || (navigationLinkAnnotation.Count == 1),
                "If the navigation link is a singleton, then there must be exactly one child in it and the property must be a singleton as well.");
            Debug.Assert(
                navigationLink.IsCollection == false || (navigationProperty == null || navigationProperty.Kind == ResourcePropertyKind.ResourceSetReference || navigationProperty.Kind == ResourcePropertyKind.Collection),
                "If the navigation link is a collection, then the property must be a collection as well.");

            // Just loop through the navigation link content
            // Note that if the navigation link is a singleton it must have just one item in it, so it's OK to loop anyway.
            foreach (ODataItem childItem in navigationLinkAnnotation)
            {
                ODataResourceSet feed = childItem as ODataResourceSet;
                if (feed != null)
                {
                    this.ApplyFeedInCollectionProperty(navigationProperty, navigationLink.Name, entityResource, feed);
                    continue;
                }

                // It must be entry by now.
                ODataResource entry = (ODataResource)childItem;

                this.ApplyEntryInComplexProperty(navigationProperty, navigationLink.Name, entityResource, entry);
            }
        }



        /// <summary>
        /// Applies an entity reference link (value of a navigation link) to an entity.
        /// </summary>
        /// <param name="navigationProperty">The navigation property for which the entity reference link was specified.</param>
        /// <param name="entityResource">The entity resource to apply the value to.</param>
        /// <param name="entityReferenceLink">The entity reference link to apply.</param>
        private void ApplyEntityReferenceLinkInNavigationProperty(ResourceProperty navigationProperty, object entityResource, ODataEntityReferenceLink entityReferenceLink)
        {
            Debug.Assert(
                navigationProperty != null && navigationProperty.TypeKind == ResourceTypeKind.EntityType,
                "navigationProperty != null && navigationProperty.TypeKind == ResourceTypeKind.EntityType");
            Debug.Assert(entityResource != null, "entityResource != null");
            Debug.Assert(entityReferenceLink != null, "entityReferenceLink != null");

            // Ignore entity reference links without any URL (this is effectively ATOM only since in JSON there always has to be some
            // URL, null would cause failure when reading it, in ATOM this is the case of a link without any href and no content).
            if (entityReferenceLink.Url == null)
            {
                return;
            }

            string linkUrl = UriUtil.UriToString(entityReferenceLink.Url);

            // Resolve the link URL and set it to the navigation property.
            this.SetResourceReferenceToUrl(entityResource, navigationProperty, linkUrl);

        }

        /// <summary>
        /// Applies a feed which is the content of a navigation property to the specified entity resource.
        /// </summary>
        /// <param name="navigationProperty">The navigation property for which the feed was specified.</param>
        /// <param name="targetResourceSet">The resource set of the target of the navigation property.</param>
        /// <param name="entityResource">The entity resource to apply the value to.</param>
        /// <param name="feed">The feed to apply.</param>
        /// <remarks>Note that the targetResourceSet will be filled for non-ATOM formats, but it will be null for ATOM.</remarks>
        private void ApplyFeedInNavigationProperty(
            ResourceProperty navigationProperty,
            ResourceSetWrapper targetResourceSet,
            object entityResource,
            ODataResourceSet feed)
        {
            Debug.Assert(
                navigationProperty != null && navigationProperty.TypeKind == ResourceTypeKind.EntityType,
                "navigationProperty != null && navigationProperty.TypeKind == ResourceTypeKind.EntityType");
            Debug.Assert(
                navigationProperty.Kind == ResourcePropertyKind.ResourceSetReference,
                "ODataLib reader should never report a feed for a singleton navigation property.");
            Debug.Assert(targetResourceSet != null, "targetResourceSet != null");
            Debug.Assert(entityResource != null, "entityResource != null");
            Debug.Assert(feed != null, "feed != null");

            ODataFeedAnnotation feedAnnotation = this.feedAnnotations[feed];
            Debug.Assert(feedAnnotation != null, "Each feed we create should gave annotation on it.");

            // Deep insert is not allowed in updates.
            // In JSON we must not fail due to deep updates if the feed is empty. In JSON empty array is reported as an empty feed
            // but there's no telling if it means empty deep update array or empty array of bindings.
            if (this.Update && feedAnnotation.Count > 0)
            {
                throw DataServiceException.CreateBadRequestError(Microsoft.OData.Service.Strings.BadRequest_DeepUpdateNotSupported);
            }

            Debug.Assert(targetResourceSet != null, "targetResourceSet != null");
            SegmentInfo propertySegmentInfo = CreateSegment(navigationProperty, navigationProperty.Name, targetResourceSet, false /* singleResult */);
            Debug.Assert(propertySegmentInfo.TargetKind != RequestTargetKind.OpenProperty, "Open navigation properties are not supported on OpenTypes.");
            Debug.Assert(
                    propertySegmentInfo.TargetSource == RequestTargetSource.Property &&
                    propertySegmentInfo.TargetKind == RequestTargetKind.Resource &&
                    propertySegmentInfo.SingleResult == false,
                    "Must be navigation set property.");

            // Look through the entries in the feed.
            foreach (ODataResource entryInFeed in feedAnnotation)
            {
                // For each entry create the corresponding entity resource.
                object childEntityResource = this.CreateNestedEntityAndApplyProperties(propertySegmentInfo, entryInFeed);
                Debug.Assert(childEntityResource != null, "Non top-level entries should never produce null entity resource.");

                this.Updatable.AddReferenceToCollection(entityResource, navigationProperty.Name, childEntityResource);
            }
        }

        private void ApplyFeedInCollectionProperty(
            ResourceProperty navigationProperty,
            string propertyName,
            object entityResource,
            ODataResourceSet feed)
        {
            // either has declared property in model or has undeclared property name in payload
            if (string.IsNullOrEmpty(propertyName))
            {
                Debug.Assert(
                    navigationProperty != null && navigationProperty.TypeKind == ResourceTypeKind.EntityType,
                    "navigationProperty != null && navigationProperty.TypeKind == ResourceTypeKind.EntityType");
                Debug.Assert(
                    navigationProperty.Kind == ResourcePropertyKind.ResourceSetReference,
                    "ODataLib reader should never report a feed for a singleton navigation property.");
            }

            Debug.Assert(entityResource != null, "entityResource != null");
            Debug.Assert(feed != null, "feed != null");

            ODataFeedAnnotation feedAnnotation = this.feedAnnotations[feed];
            Debug.Assert(feedAnnotation != null, "Each feed we create should gave annotation on it.");

            // Deep insert is not allowed in updates.
            // In JSON we must not fail due to deep updates if the feed is empty. In JSON empty array is reported as an empty feed
            // but there's no telling if it means empty deep update array or empty array of bindings.
            if (this.Update && feedAnnotation.Count > 0)
            {
                throw DataServiceException.CreateBadRequestError(Microsoft.OData.Service.Strings.BadRequest_DeepUpdateNotSupported);
            }

            SegmentInfo propertySegmentInfo = CreateSegment(navigationProperty, propertyName, null, false /* singleResult */);
            Debug.Assert(propertySegmentInfo.TargetKind != RequestTargetKind.OpenProperty, "Open navigation properties are not supported on OpenTypes.");
            Debug.Assert(
                    propertySegmentInfo.TargetSource == RequestTargetSource.Property &&
                    propertySegmentInfo.TargetKind == RequestTargetKind.Resource &&
                    propertySegmentInfo.SingleResult == false,
                    "Must be navigation set property.");

            IList collectionList = Deserializer.CreateNewCollection();

            this.RecurseEnter();

            // Look through the entries in the feed.
            foreach (ODataResource entryInFeed in feedAnnotation)
            {
                // For each entry create the corresponding entity resource.
                object childEntityResource = this.CreateNestedEntityAndApplyProperties(propertySegmentInfo, entryInFeed);
                Debug.Assert(childEntityResource != null, "Non top-level entries should never produce null entity resource.");

                collectionList.Add(childEntityResource);
            }
            this.RecurseLeave();

            var collection = Deserializer.GetReadOnlyCollection(collectionList, navigationProperty.ResourceType as CollectionResourceType);
            Deserializer.SetOpenPropertyValue(entityResource, propertyName, collection, this.Service);
        }

        /// <summary>
        /// Applies an entry which is the content of a navigation property to the specified entity resource.
        /// </summary>
        /// <param name="navigationProperty">The navigation property for which the feed was specified.</param>
        /// <param name="targetResourceSet">The resource set of the target of the navigation property.</param>
        /// <param name="entityResource">The entity resource to apply the value to.</param>
        /// <param name="entry">The entry to apply. This can be null if the null value should be applied.</param>
        /// <remarks>Note that the targetResourceSet will be filled for non-ATOM formats, but it will be null for ATOM.</remarks>
        private void ApplyEntryInNavigationProperty(
            ResourceProperty navigationProperty,
            ResourceSetWrapper targetResourceSet,
            object entityResource,
            ODataResource entry)
        {
            Debug.Assert(
                navigationProperty != null && (navigationProperty.TypeKind == ResourceTypeKind.EntityType || navigationProperty.TypeKind == ResourceTypeKind.ComplexType),
                "navigationProperty != null && navigationProperty.TypeKind == ResourceTypeKind.EntityType");
            Debug.Assert(
                navigationProperty.Kind == ResourcePropertyKind.ResourceReference
                || navigationProperty.Kind == ResourcePropertyKind.ComplexType,
                "ODataLib reader should never report an entry for a collection navigation property.");
            Debug.Assert(entityResource != null, "entityResource != null");

            // Deep insert is not allowed in updates, unless it's JSON and the deep insert is a null value.
            if (entry != null && this.Update)
            {
                throw DataServiceException.CreateBadRequestError(Microsoft.OData.Service.Strings.BadRequest_DeepUpdateNotSupported);
            }

            if (entry == null)
            {
                this.SetResourceReferenceToNull(entityResource, navigationProperty);
            }
            else
            {
                SegmentInfo propertySegmentInfo = CreateSegment(navigationProperty, navigationProperty.Name, targetResourceSet, true /* singleResult */);
                Debug.Assert(propertySegmentInfo.TargetKind != RequestTargetKind.OpenProperty, "Open navigation properties are not supported on OpenTypes.");

                object childEntityResource = this.CreateNestedEntityAndApplyProperties(propertySegmentInfo, entry);


                this.Updatable.SetReference(entityResource, navigationProperty.Name, childEntityResource);
            }
        }

        private void ApplyEntryInComplexProperty(
            ResourceProperty complexProperty,
            string propertyName,
            object entityResource,
            ODataResource entry)
        {
            // either has declared property in model or has undeclared property name in payload
            if (string.IsNullOrEmpty(propertyName))
            {
                Debug.Assert(
                complexProperty != null && (complexProperty.TypeKind == ResourceTypeKind.EntityType || complexProperty.TypeKind == ResourceTypeKind.ComplexType),
                "complexProperty != null && complexProperty.TypeKind == ResourceTypeKind.EntityType");
                Debug.Assert(
                    complexProperty.Kind == ResourcePropertyKind.ResourceReference
                    || complexProperty.Kind == ResourcePropertyKind.ComplexType,
                    "ODataLib reader should never report an entry for a collection navigation property.");
            }

            Debug.Assert(entityResource != null, "entityResource != null");
            if (entry == null)
            {
                Deserializer.SetOpenPropertyValue(entityResource, propertyName, null, this.Service);
            }
            else
            {
                SegmentInfo propertySegmentInfo = CreateSegment(complexProperty, propertyName, null, true /* singleResult */);

                object childEntityResource = this.CreateNestedEntityAndApplyProperties(propertySegmentInfo, entry);
                Deserializer.SetOpenPropertyValue(entityResource, propertyName, childEntityResource, this.Service);
            }
        }

        /// <summary>
        /// Sets a resource reference to null.
        /// </summary>
        /// <param name="entityResource">The entity resource to set the resource reference on.</param>
        /// <param name="navigationProperty">The navigation property for which to set the reference to null.</param>
        /// <remarks>This verifies that the navigation property is a resource reference and fails otherwise.</remarks>
        private void SetResourceReferenceToNull(object entityResource, ResourceProperty navigationProperty)
        {
            Debug.Assert(entityResource != null, "entityResource != null");
            Debug.Assert(navigationProperty != null, "navigationProperty != null");

            // Update the object count when you are performing a bind operation.
            this.CheckAndIncrementObjectCount();
            if (navigationProperty.Kind == ResourcePropertyKind.ResourceSetReference)
            {
                throw DataServiceException.CreateBadRequestError(Microsoft.OData.Service.Strings.BadRequest_CannotSetCollectionsToNull(navigationProperty.Name));
            }

            // For open properties, we will assume that this is a reference property and set it to null
            Debug.Assert(navigationProperty.Kind == ResourcePropertyKind.ResourceReference
                || navigationProperty.Kind == ResourcePropertyKind.ComplexType, "Only navigation properties are allowed in this method.");
            this.Updatable.SetReference(entityResource, navigationProperty.Name, null);
        }

        /// <summary>
        /// Sets a resource reference to resource referenced by a URL.
        /// </summary>
        /// <param name="entityResource">The entity resource to set the resource reference on.</param>
        /// <param name="navigationProperty">The navigation property for which to set the reference to null.</param>
        /// <param name="url">The URL which points to the resource to set as the value of the navigation property.</param>
        private void SetResourceReferenceToUrl(object entityResource, ResourceProperty navigationProperty, string url)
        {
            Debug.Assert(entityResource != null, "entityResource != null");
            Debug.Assert(navigationProperty != null, "navigationProperty != null");

            // Update the object count when you are performing a bind operation.
            this.CheckAndIncrementObjectCount();

            // Get the referenced resource.
            Uri referencedUri = RequestUriProcessor.GetAbsoluteUriFromReference(url, this.Service.OperationContext);
            RequestDescription requestDescription = RequestUriProcessor.ProcessRequestUri(referencedUri, this.Service, true /*internalQuery*/);

            // Get the resource
            object referencedResource = this.Service.GetResource(requestDescription, requestDescription.SegmentInfos.Count - 1, null);
            if (navigationProperty.Kind == ResourcePropertyKind.ResourceReference)
            {
                this.Updatable.SetReference(entityResource, navigationProperty.Name, referencedResource);
            }
            else
            {
                Debug.Assert(navigationProperty.Kind == ResourcePropertyKind.ResourceSetReference, "Only navigation properties are allowed in this method.");

                // If we are to set the resource to a collection property it must not be null (so check for nulls), otherwise do allow nulls for backward compatibility.
                WebUtil.CheckResourceExists(referencedResource != null, requestDescription.LastSegmentInfo.Identifier);
                this.Updatable.AddReferenceToCollection(entityResource, navigationProperty.Name, referencedResource);
            }
        }

        /// <summary>
        /// Gets a target resource set for the specified navigation property.
        /// </summary>
        /// <param name="parentResourceSet">The resource set of the entity on which the navigation property is accessed.</param>
        /// <param name="parentResourceType">The resourec type of the entity on which the navigation property is accessed.</param>
        /// <param name="navigationProperty">The navigation property to access.</param>
        /// <returns>The target resource set of the navigation property.</returns>
        /// <remarks>This method validates that the resource set is accessible and it performs appropriate version checks.</remarks>
        private ResourceSetWrapper GetNavigationPropertyTargetResourceSet(ResourceSetWrapper parentResourceSet, ResourceType parentResourceType, ResourceProperty navigationProperty)
        {
            Debug.Assert(parentResourceSet != null, "parentResourceSet != null");
            Debug.Assert(parentResourceType != null, "parentResourceType != null");
            Debug.Assert(navigationProperty != null, "navigationProperty != null");

            return this.Service.Provider.GetResourceSet(parentResourceSet, parentResourceType, navigationProperty);
        }

        /// <summary>
        /// The annotation used on ODataResource instances to store the list of navigation links for that entry.
        /// </summary>
        internal sealed class ODataEntryAnnotation : List<ODataNestedResourceInfo>
        {
            /// <summary>The entity resource update token for the entry.</summary>
            internal object EntityResource { get; set; }

            /// <summary>The resolved entity type for the entry.</summary>
            internal ResourceType EntityResourceType { get; set; }
        }

        /// <summary>
        /// The annotation used on ODataResourceSet instances to store the list of entries in that feed.
        /// </summary>
        private sealed class ODataFeedAnnotation : List<ODataResource>
        {
        }

        /// <summary>
        /// The annotation used on ODataNestedResourceInfo instances to store the list of children for that navigation link.
        /// </summary>
        /// <remarks>
        /// A navigation link for a singleton navigation property can only contain one item - either ODataResource or ODataEntityReferenceLink.
        /// A navigation link for a collection navigation property can contain any number of items - each is either ODataResourceSet or ODataEntityReferenceLink.
        /// </remarks>
        private sealed class ODataNavigationLinkAnnotation : List<ODataItem>
        {
        }
    }
}
