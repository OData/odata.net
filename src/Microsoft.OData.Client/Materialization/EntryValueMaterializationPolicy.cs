//---------------------------------------------------------------------
// <copyright file="EntryValueMaterializationPolicy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Microsoft.OData;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Edm;
    using DSClient = Microsoft.OData.Client;


    /// <summary>
    /// Used to materialize entities from an <see cref="ODataResource"/> to an object.
    /// </summary>
    internal class EntryValueMaterializationPolicy : StructuralValueMaterializationPolicy
    {
        /// <summary>Collection->Next Link Table for nested links</summary>
        private readonly Dictionary<IEnumerable, DataServiceQueryContinuation> nextLinkTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryValueMaterializationPolicy" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="entityTrackingAdapter">The entity tracking adapter.</param>
        /// <param name="lazyPrimitivePropertyConverter">The lazy primitive property converter.</param>
        /// <param name="nextLinkTable">The next link table.</param>
        internal EntryValueMaterializationPolicy(
            IODataMaterializerContext context,
            EntityTrackingAdapter entityTrackingAdapter,
            DSClient.SimpleLazy<PrimitivePropertyConverter> lazyPrimitivePropertyConverter,
            Dictionary<IEnumerable, DataServiceQueryContinuation> nextLinkTable)
            : base(context, lazyPrimitivePropertyConverter)
        {
            this.nextLinkTable = nextLinkTable;
            this.EntityTrackingAdapter = entityTrackingAdapter;
        }

        /// <summary>
        /// Gets the Entity Materializer Context
        /// </summary>
        internal EntityTrackingAdapter EntityTrackingAdapter { get; private set; }

        /// <summary>
        /// Validates the specified <paramref name="property"/> matches
        /// the parsed <paramref name="link"/>.
        /// </summary>
        /// <param name="property">Property as understood by the type system.</param>
        /// <param name="link">Property as parsed.</param>
        internal static void ValidatePropertyMatch(ClientPropertyAnnotation property, ODataNestedResourceInfo link)
        {
            ValidatePropertyMatch(property, link, null, false /*performEntityCheck*/);
        }

        /// <summary>
        /// Validates the specified <paramref name="property"/> matches
        /// the parsed <paramref name="atomProperty"/>.
        /// </summary>
        /// <param name="property">Property as understood by the type system.</param>
        /// <param name="atomProperty">Property as parsed.</param>
        internal static void ValidatePropertyMatch(ClientPropertyAnnotation property, ODataProperty atomProperty)
        {
            ValidatePropertyMatch(property, atomProperty, null, false /*performEntityCheck*/);
        }

        /// <summary>
        /// Validates the specified <paramref name="property"/> matches
        /// the parsed <paramref name="link"/>.
        /// </summary>
        /// <param name="property">Property as understood by the type system.</param>
        /// <param name="link">Property as parsed.</param>
        /// <param name="model">Client Model.</param>
        /// <param name="performEntityCheck">whether to do the entity check or not.</param>
        /// <returns>The type</returns>
        internal static Type ValidatePropertyMatch(ClientPropertyAnnotation property, ODataNestedResourceInfo link, ClientEdmModel model, bool performEntityCheck)
        {
            Debug.Assert(property != null, "property != null");
            Debug.Assert(link != null, "link != null");

            Type propertyType = null;
            if (link.IsCollection.HasValue)
            {
                if (link.IsCollection.Value)
                {
                    // We need to fail if the payload states that the property is a navigation collection property or a complex collection property
                    // and in the client, the property is not a collection property.
                    if (!property.IsResourceSet)
                    {
                        throw DSClient.Error.InvalidOperation(DSClient.Strings.Deserialize_MismatchAtomLinkFeedPropertyNotCollection(property.PropertyName));
                    }

                    propertyType = property.ResourceSetItemType;
                }
                else
                {
                    if (property.IsResourceSet)
                    {
                        throw DSClient.Error.InvalidOperation(DSClient.Strings.Deserialize_MismatchAtomLinkEntryPropertyIsCollection(property.PropertyName));
                    }

                    propertyType = property.PropertyType;
                }
            }

            // If the server type and the client type does not match, we need to throw.
            // This is a breaking change from V1/V2 where we allowed materialization of entities into non-entities and vice versa
            if (propertyType != null && performEntityCheck)
            {
                if (!ClientTypeUtil.TypeIsStructured(propertyType, model))
                {
                    throw DSClient.Error.InvalidOperation(DSClient.Strings.AtomMaterializer_InvalidNonEntityType(propertyType.ToString()));
                }
            }

            return propertyType;
        }

        /// <summary>
        /// Validates the specified <paramref name="property"/> matches
        /// the parsed <paramref name="atomProperty"/>.
        /// </summary>
        /// <param name="property">Property as understood by the type system.</param>
        /// <param name="atomProperty">Property as parsed.</param>
        /// <param name="model">Client model.</param>
        /// <param name="performEntityCheck">whether to do the entity check or not.</param>
        internal static void ValidatePropertyMatch(ClientPropertyAnnotation property, ODataProperty atomProperty, ClientEdmModel model, bool performEntityCheck)
        {
            Debug.Assert(property != null, "property != null");
            Debug.Assert(atomProperty != null, "atomProperty != null");

            ODataResourceSet feed = atomProperty.Value as ODataResourceSet;
            ODataResource entry = atomProperty.Value as ODataResource;

            if (property.IsKnownType && (feed != null || entry != null))
            {
                throw DSClient.Error.InvalidOperation(DSClient.Strings.Deserialize_MismatchAtomLinkLocalSimple);
            }

            Type propertyType = null;
            if (feed != null)
            {
                // We need to fail if the payload states that the property is a navigation collection property
                // and in the client, the property is not a collection property.
                if (!property.IsEntityCollection)
                {
                    throw DSClient.Error.InvalidOperation(DSClient.Strings.Deserialize_MismatchAtomLinkFeedPropertyNotCollection(property.PropertyName));
                }

                propertyType = property.EntityCollectionItemType;
            }

            if (entry != null)
            {
                if (property.IsEntityCollection)
                {
                    throw DSClient.Error.InvalidOperation(DSClient.Strings.Deserialize_MismatchAtomLinkEntryPropertyIsCollection(property.PropertyName));
                }

                propertyType = property.PropertyType;
            }

            // If the server type and the client type does not match, we need to throw.
            // This is a breaking change from V1/V2 where we allowed materialization of entities into non-entities and vice versa
            if (propertyType != null && performEntityCheck)
            {
                if (!ClientTypeUtil.TypeIsEntity(propertyType, model))
                {
                    throw DSClient.Error.InvalidOperation(DSClient.Strings.AtomMaterializer_InvalidNonEntityType(propertyType.ToString()));
                }
            }
        }

        /// <summary>Materializes the specified <paramref name="entry"/>.</summary>
        /// <param name="entry">Entry with object to materialize.</param>
        /// <param name="expectedEntryType">Expected type for the entry.</param>
        /// <param name="includeLinks">Whether links that are expanded should be materialized.</param>
        /// <remarks>This is a payload-driven materialization process.</remarks>
        internal void Materialize(MaterializerEntry entry, Type expectedEntryType, bool includeLinks)
        {
            Debug.Assert(entry.Entry != null, "entry != null");
            Debug.Assert(
                entry.ResolvedObject == null || entry.ResolvedObject == this.EntityTrackingAdapter.TargetInstance,
                "entry.ResolvedObject == null || entry.ResolvedObject == this.targetInstance -- otherwise getting called twice");
            Debug.Assert(expectedEntryType != null, "expectedType != null");

            // TODO : Need to handle complex type with no tracking and entity with tracking but no id.
            // ResolvedObject will already be assigned when we have a TargetInstance.
            if ((entry.IsTracking && entry.Id == null)
                || !this.EntityTrackingAdapter.TryResolveExistingEntity(entry, expectedEntryType))
            {
                // If the type is a derived one this call will resolve to derived type, cannot put code in  ResolveByCreatingWithType as this is used by projection and in this case
                // the type cannot be changed or updated
                ClientTypeAnnotation actualType = this.MaterializerContext.ResolveTypeForMaterialization(expectedEntryType, entry.Entry.TypeName);

                this.ResolveByCreatingWithType(entry, actualType.ElementType);
            }

            Debug.Assert(entry.ResolvedObject != null, "entry.ResolvedObject != null -- otherwise ResolveOrCreateInstance didn't do its job");

            this.MaterializeResolvedEntry(entry, includeLinks);
        }

        /// <summary>
        /// Applies the values of the <paramref name="items"/> enumeration to the
        /// <paramref name="property"/> of the specified <paramref name="entry"/>.
        /// </summary>
        /// <param name="entry">Entry with collection to be modified.</param>
        /// <param name="property">Collection property on the entry.</param>
        /// <param name="items">Values to apply onto the collection.</param>
        /// <param name="nextLink">Next link for collection continuation.</param>
        /// <param name="continuationPlan">Projection plan for collection continuation.</param>
        /// <param name="isContinuation">Whether this is a continuation request.</param>
        internal void ApplyItemsToCollection(
            MaterializerEntry entry,
            ClientPropertyAnnotation property,
            IEnumerable items,
            Uri nextLink,
            ProjectionPlan continuationPlan,
            bool isContinuation)
        {
            Debug.Assert(entry.Entry != null || entry.ForLoadProperty, "ODataResource should be non-null except for LoadProperty");
            Debug.Assert(property != null, "property != null");
            Debug.Assert(items != null, "items != null");

            IEnumerable<object> itemsEnumerable = ODataEntityMaterializer.EnumerateAsElementType<object>(items);

            // Populate the collection property with items collection.
            object collection = this.PopulateCollectionProperty(entry, property, itemsEnumerable, nextLink, continuationPlan);

            // Get collection of all non-linked elements in collection and remove them except for the ones that were obtained from the response.
            if (this.EntityTrackingAdapter.MergeOption == MergeOption.OverwriteChanges ||
                this.EntityTrackingAdapter.MergeOption == MergeOption.PreserveChanges)
            {
                var linkedItemsInCollection =
                    this.EntityTrackingAdapter
                        .EntityTracker
                        .GetLinks(entry.ResolvedObject, property.PropertyName)
                        .Select(l => new { l.Target, l.IsModified });

                if (collection != null && !property.IsDictionary)
                {
                    var nonLinkedNonReceivedItemsInCollection = ODataEntityMaterializer.EnumerateAsElementType<object>((IEnumerable)collection)
                        .Except(linkedItemsInCollection.Select(i => i.Target))
                        .Except(itemsEnumerable).ToArray();

                    // Since no link exists, we just remove the item from the collection
                    foreach (var item in nonLinkedNonReceivedItemsInCollection)
                    {
                        property.RemoveValue(collection, item);
                    }
                }

                // When the first time a property or collection is being loaded, we remove all items other than the ones that we receive.
                if (!isContinuation)
                {
                    IEnumerable<object> itemsToRemove;

                    if (this.EntityTrackingAdapter.MergeOption == MergeOption.OverwriteChanges)
                    {
                        itemsToRemove = linkedItemsInCollection.Select(i => i.Target);
                    }
                    else
                    {
                        Debug.Assert(
                            this.EntityTrackingAdapter.MergeOption == MergeOption.PreserveChanges,
                            "this.EntityTrackingAdapter.MergeOption == MergeOption.PreserveChanges");

                        itemsToRemove = linkedItemsInCollection
                            .Where(i => !i.IsModified)
                            .Select(i => i.Target);
                    }

                    itemsToRemove = itemsToRemove.Except(itemsEnumerable);

                    foreach (var item in itemsToRemove)
                    {
                        if (collection != null)
                        {
                            property.RemoveValue(collection, item);
                        }

                        this.EntityTrackingAdapter.MaterializationLog.RemovedLink(entry, property.PropertyName, item);
                    }
                }
            }
        }

        /// <summary>Records the fact that a rel='next' link was found for the specified <paramref name="collection"/>.</summary>
        /// <param name="collection">Collection to add link to.</param>
        /// <param name="link">Link (possibly null).</param>
        /// <param name="plan">Projection plan for the collection (null allowed only if link is null).</param>
        internal void FoundNextLinkForCollection(IEnumerable collection, Uri link, ProjectionPlan plan)
        {
            Debug.Assert(plan != null || link == null, "plan != null || link == null");

            if (collection != null && !this.nextLinkTable.ContainsKey(collection))
            {
                DataServiceQueryContinuation continuation = DataServiceQueryContinuation.Create(link, plan);
                this.nextLinkTable.Add(collection, continuation);
                Util.SetNextLinkForCollection(collection, continuation);
            }
        }

        /// <summary>Records the fact that a <paramref name="collection"/> was found but won't be modified.</summary>
        /// <param name="collection">Collection to add link to.</param>
        internal void FoundNextLinkForUnmodifiedCollection(IEnumerable collection)
        {
            if (collection != null && !this.nextLinkTable.ContainsKey(collection))
            {
                this.nextLinkTable.Add(collection, null);
            }
        }

        /// <summary>"Resolved" the entity in the <paramref name="entry"/> by instantiating it.</summary>
        /// <param name="entry">Entry to resolve.</param>
        /// <param name="type">Type to create.</param>
        /// <remarks>
        /// After invocation, entry.ResolvedObject is exactly of type <paramref name="type"/>.
        /// </remarks>
        internal void ResolveByCreatingWithType(MaterializerEntry entry, Type type)
        {
            // TODO: CreateNewInstance needs to do all of these operations otherwise an inadvertent call to CreateNewInstance
            // will create a new entity instance that is not tracked in the context or materialization log. Will need to change this
            // prior to shipping if public
            Debug.Assert(
                 entry.ResolvedObject == null,
                 "entry.ResolvedObject == null -- otherwise we're about to overwrite - should never be called");
            ClientEdmModel edmModel = this.MaterializerContext.Model;
            entry.ActualType = edmModel.GetClientTypeAnnotation(edmModel.GetOrCreateEdmType(type));
            entry.ResolvedObject = this.CreateNewInstance(entry.ActualType.EdmTypeReference, type);
            entry.CreatedByMaterializer = true;
            entry.ShouldUpdateFromPayload = true;
            entry.EntityHasBeenResolved = true;
            this.EntityTrackingAdapter.MaterializationLog.CreatedInstance(entry);
        }

        /// <summary>
        /// Matches the given item type with the corresponding collection element type.
        /// </summary>
        /// <param name="itemType">Item type.</param>
        /// <param name="collectionElementType">Collection element type.</param>
        private static void ValidateCollectionElementTypeIsItemType(Type itemType, Type collectionElementType)
        {
            if (!collectionElementType.IsAssignableFrom(itemType))
            {
                string message = DSClient.Strings.AtomMaterializer_EntryIntoCollectionMismatch(
                    itemType.FullName,
                    collectionElementType.FullName);

                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Materializes the link properties if any with the url in the response payload
        /// </summary>
        /// <param name="actualType">Actual client type that is getting materialized.</param>
        /// <param name="entry">MaterializerEntry instance containing all the links that came in the response.</param>
        private static void ApplyLinkProperties(
            ClientTypeAnnotation actualType,
            MaterializerEntry entry)
        {
            Debug.Assert(actualType != null, "actualType != null");
            Debug.Assert(entry.Entry != null, "entry != null");

            if (entry.ShouldUpdateFromPayload)
            {
                foreach (var linkProperty in actualType.Properties().Where(p => p.PropertyType == typeof(DataServiceStreamLink)))
                {
                    string propertyName = linkProperty.PropertyName;
                    StreamDescriptor streamDescriptor;
                    if (entry.EntityDescriptor.TryGetNamedStreamInfo(propertyName, out streamDescriptor))
                    {
                        // At this time we have materialized the stream link onto the stream descriptor object
                        // we'll always make sure the stream link is the same instance on the entity and the descriptor
                        linkProperty.SetValue(entry.ResolvedObject, streamDescriptor.StreamLink, propertyName, true /*allowAdd*/);
                    }
                }
            }
        }

        /// <summary>
        /// Populates the collection property on the entry's resolved object with the given items enumerator.
        /// </summary>
        /// <param name="entry">Entry with collection to be modified.</param>
        /// <param name="property">Collection property on the entry.</param>
        /// <param name="items">Values to apply onto the collection.</param>
        /// <param name="nextLink">Next link for collection continuation.</param>
        /// <param name="continuationPlan">Projection plan for collection continuation.</param>
        /// <returns>Collection instance that was populated.</returns>
        private object PopulateCollectionProperty(
            MaterializerEntry entry,
            ClientPropertyAnnotation property,
            IEnumerable<object> items,
            Uri nextLink,
            ProjectionPlan continuationPlan)
        {
            Debug.Assert(entry.Entry != null || entry.ForLoadProperty, "ODataResource should be non-null except for LoadProperty");
            Debug.Assert(property != null, "property != null");
            Debug.Assert(items != null, "items != null");

            object collection = null;

            ClientEdmModel edmModel = this.MaterializerContext.Model;
            ClientTypeAnnotation collectionType = edmModel.GetClientTypeAnnotation(edmModel.GetOrCreateEdmType(property.ResourceSetItemType));

            if (entry.ShouldUpdateFromPayload)
            {
                collection = this.GetOrCreateCollectionProperty(entry.ResolvedObject, property, entry.ForLoadProperty);

                foreach (object item in items)
                {
                    // Validate that item can be inserted into collection.
                    ValidateCollectionElementTypeIsItemType(item.GetType(), collectionType.ElementType);

                    property.SetValue(collection, item, property.PropertyName, true /* allowAdd? */);

                    this.EntityTrackingAdapter.MaterializationLog.AddedLink(entry, property.PropertyName, item);
                }

                this.FoundNextLinkForCollection(collection as IEnumerable, nextLink, continuationPlan);
            }
            else
            {
                Debug.Assert(!entry.ForLoadProperty, "LoadProperty should always have ShouldUpdateForPayload set to true.");

                foreach (object item in items)
                {
                    // Validate that item can be inserted into collection.
                    ValidateCollectionElementTypeIsItemType(item.GetType(), collectionType.ElementType);
                }

                this.FoundNextLinkForUnmodifiedCollection(property.GetValue(entry.ResolvedObject) as IEnumerable);
            }

            return collection;
        }

        /// <summary>
        /// Gets or creates a collection property on the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">Instance on which to get/create the collection.</param>
        /// <param name="property">Collection property on the <paramref name="instance"/>.</param>
        /// <param name="forLoadProperty">Is this collection being created for LoadProperty scenario.</param>
        /// <returns>
        /// The collection corresponding to the specified <paramref name="property"/>;
        /// never null.
        /// </returns>
        private object GetOrCreateCollectionProperty(object instance, ClientPropertyAnnotation property, bool forLoadProperty)
        {
            Debug.Assert(instance != null, "instance != null");
            Debug.Assert(property != null, "property != null");
            Debug.Assert(property.IsResourceSet, "property.IsEntityCollection has to be true - otherwise property isn't a collection");

            // NOTE: in V1, we would have instantiated nested objects before setting them.
            object result;
            result = property.GetValue(instance);

            if (result == null)
            {
                Type collectionType = property.PropertyType;

                // For backward compatibility we need to have different strategy of collection creation b/w
                // LoadProperty scenario versus regular collection creation scenario.
                if (forLoadProperty)
                {
                    if (BindingEntityInfo.IsDataServiceCollection(collectionType, this.MaterializerContext.Model))
                    {
                        Debug.Assert(WebUtil.GetDataServiceCollectionOfT(property.EntityCollectionItemType) != null, "DataServiceCollection<> must be available here.");

                        // new DataServiceCollection<property.EntityCollectionItemType>(null, TrackingMode.None)
                        result = Activator.CreateInstance(
                            WebUtil.GetDataServiceCollectionOfT(property.EntityCollectionItemType),
                            null,
                            TrackingMode.None);
                    }
                    else
                    {
                        // Try List<> first because that's what we did in V1/V2, but use the actual property type if it doesn't support List<>
                        Type listCollectionType = typeof(List<>).MakeGenericType(property.EntityCollectionItemType);
                        if (collectionType.IsAssignableFrom(listCollectionType))
                        {
                            collectionType = listCollectionType;
                        }

                        result = Activator.CreateInstance(collectionType);
                    }
                }
                else
                {
                    if (DSClient.PlatformHelper.IsInterface(collectionType))
                    {
                        collectionType = typeof(System.Collections.ObjectModel.Collection<>).MakeGenericType(property.EntityCollectionItemType);
                    }

                    result = this.CreateNewInstance(property.EdmProperty.Type, collectionType);
                }

                // Update the property value on the instance.
                property.SetValue(instance, result, property.PropertyName, false /* add */);
            }

            Debug.Assert(result != null, "result != null -- otherwise GetOrCreateCollectionProperty didn't fall back to creation");
            return result;
        }

        /// <summary>
        /// Applies the values of a nested <paramref name="feed"/> to the collection
        /// <paramref name="property"/> of the specified <paramref name="entry"/>.
        /// </summary>
        /// <param name="entry">Entry with collection to be modified.</param>
        /// <param name="property">Collection property on the entry.</param>
        /// <param name="feed">Values to apply onto the collection.</param>
        /// <param name="includeLinks">Whether links that are expanded should be materialized.</param>
        private void ApplyFeedToCollection(
            MaterializerEntry entry,
            ClientPropertyAnnotation property,
            ODataResourceSet feed,
            bool includeLinks)
        {
            Debug.Assert(entry.Entry != null, "entry != null");
            Debug.Assert(property != null, "property != null");
            Debug.Assert(feed != null, "feed != null");

            ClientEdmModel edmModel = this.MaterializerContext.Model;
            ClientTypeAnnotation collectionType = edmModel.GetClientTypeAnnotation(edmModel.GetOrCreateEdmType(property.ResourceSetItemType));

            IEnumerable<ODataResource> entries = MaterializerFeed.GetFeed(feed).Entries;

            foreach (ODataResource feedEntry in entries)
            {
                this.Materialize(MaterializerEntry.GetEntry(feedEntry), collectionType.ElementType, includeLinks);
            }

            ProjectionPlan continuationPlan = includeLinks ?
                ODataEntityMaterializer.CreatePlanForDirectMaterialization(property.ResourceSetItemType) :
                ODataEntityMaterializer.CreatePlanForShallowMaterialization(property.ResourceSetItemType);

            this.ApplyItemsToCollection(
                entry,
                property,
                entries.Select(e => MaterializerEntry.GetEntry(e).ResolvedObject),
                feed.NextPageLink,
                continuationPlan,
                false);
        }

        /// <summary>Materializes the specified <paramref name="entry"/>.</summary>
        /// <param name="entry">Entry with object to materialize.</param>
        /// <param name="includeLinks">Whether links that are expanded for navigation property should be materialized.</param>
        /// <remarks>This is a payload-driven materialization process.</remarks>
        private void MaterializeResolvedEntry(MaterializerEntry entry, bool includeLinks)
        {
            Debug.Assert(entry.Entry != null, "entry != null");
            Debug.Assert(entry.ResolvedObject != null, "entry.ResolvedObject != null -- otherwise not resolved/created!");

            ClientTypeAnnotation actualType = entry.ActualType;

            // While materializing entities, we need to make sure the payload that came in the wire is also an entity.
            // Otherwise we need to fail.
            // This is a breaking change from V1/V2 where we allowed materialization of entities into non-entities and vice versa
            if (!actualType.IsStructuredType)
            {
                throw DSClient.Error.InvalidOperation(DSClient.Strings.AtomMaterializer_InvalidNonEntityType(actualType.ElementTypeName));
            }

            // Note that even if ShouldUpdateFromPayload is false, we will still be creating
            // nested instances (but not their links), so they show up in the data context
            // entries. This keeps this code compatible with V1 behavior.
            this.MaterializeDataValues(actualType, entry.Properties, this.MaterializerContext.UndeclaredPropertyBehavior);

            if (entry.NestedResourceInfos?.Any() == true)
            {
                foreach (ODataNestedResourceInfo link in entry.NestedResourceInfos)
                {
                    var prop = actualType.GetProperty(link.Name, UndeclaredPropertyBehavior.Support);
                    if (prop != null)
                    {
                        ValidatePropertyMatch(prop, link, this.MaterializerContext.Model, true /*performEntityCheck*/);
                    }
                }

                foreach (ODataNestedResourceInfo link in entry.NestedResourceInfos)
                {
                    MaterializerNavigationLink linkState = MaterializerNavigationLink.GetLink(link);

                    if (linkState == null)
                    {
                        continue;
                    }

                    var prop = actualType.GetProperty(link.Name, this.MaterializerContext.UndeclaredPropertyBehavior);

                    if (prop == null)
                    {
                        if (entry.ShouldUpdateFromPayload)
                        {
                            this.MaterializeDynamicProperty(entry, link);
                        }

                        continue;
                    }

                    // includeLinks is for Navigation property, so we should handle complex property when includeLinks equals false;
                    if (!includeLinks && (prop.IsEntityCollection || prop.EntityCollectionItemType != null))
                    {
                        continue;
                    }

                    if (linkState.Feed != null)
                    {
                        this.ApplyFeedToCollection(entry, prop, linkState.Feed, includeLinks);
                    }
                    else if (linkState.Entry != null)
                    {
                        MaterializerEntry linkEntry = linkState.Entry;

                        if (linkEntry.Entry != null)
                        {
                            this.Materialize(linkEntry, prop.PropertyType, includeLinks);
                        }

                        if (entry.ShouldUpdateFromPayload)
                        {
                            prop.SetValue(entry.ResolvedObject, linkEntry.ResolvedObject, link.Name, true /* allowAdd? */);

                            if (!this.MaterializerContext.Context.DisableInstanceAnnotationMaterialization && linkEntry.ShouldUpdateFromPayload)
                            {
                                // Apply instance annotation for navigation property
                                this.InstanceAnnotationMaterializationPolicy.SetInstanceAnnotations(
                                    prop.PropertyName, linkEntry.Entry, entry.ActualType.ElementType, entry.ResolvedObject);

                                // Apply instance annotation for entity of the navigation property
                                this.InstanceAnnotationMaterializationPolicy.SetInstanceAnnotations(linkEntry.Entry, linkEntry.ResolvedObject);
                            }

                            this.EntityTrackingAdapter.MaterializationLog.SetLink(entry, prop.PropertyName, linkEntry.ResolvedObject);
                        }
                    }
                }
            }

            foreach (var e in entry.Properties)
            {
                if (e.Value is ODataStreamReferenceValue)
                {
                    continue;
                }

                var prop = actualType.GetProperty(e.Name, this.MaterializerContext.UndeclaredPropertyBehavior);
                if (prop == null)
                {
                    if (entry.ShouldUpdateFromPayload)
                    {
                        this.MaterializeDynamicProperty(e, entry.ResolvedObject);
                    }

                    continue;
                }

                if (entry.ShouldUpdateFromPayload)
                {
                    ValidatePropertyMatch(prop, e, this.MaterializerContext.Model, true /*performEntityCheck*/);

                    this.ApplyDataValue(actualType, e, entry.ResolvedObject);
                }
            }

            // apply link values if present
            ApplyLinkProperties(actualType, entry);

            Debug.Assert(entry.ResolvedObject != null, "entry.ResolvedObject != null -- otherwise we didn't do any useful work");

            if (entry.ResolvedObject is IBaseEntityType entity)
            {
                entity.Context = this.EntityTrackingAdapter.Context;

                if (!entry.IsTracking)
                {
                    int? streamDescriptorsCount = entry.EntityDescriptor.StreamDescriptors?.Count;
                    IEdmEntityType entityType =
                        this.EntityTrackingAdapter.Model.FindDeclaredType(entry.Entry.TypeName) as IEdmEntityType;

                    if (streamDescriptorsCount > 0 || entityType?.HasStream == true)
                    {
                        entity.EntityDescriptor = entry.EntityDescriptor;
                    }
                }
            }

            this.MaterializerContext.ResponsePipeline.FireEndEntryEvents(entry);
        }

        /// <summary>Materializes the specified <paramref name="entry"/> as dynamic property.</summary>
        /// <param name="entry">Entry with object to materialize.</param>
        /// <param name="link">Nested resource link as parsed.</param>
        private void MaterializeDynamicProperty(MaterializerEntry entry, ODataNestedResourceInfo link)
        {
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(entry.ResolvedObject != null, "entry.ResolvedObject != null -- otherwise not resolved/created!");
            Debug.Assert(link != null, "link != null");

            ClientEdmModel model = this.MaterializerContext.Model;

            IDictionary<string, object> containerProperty;
            // Stop if owning type is not an open type
            // Or container property is not found
            // Or key with matching name already exists in the dictionary
            if (!ClientTypeUtil.IsInstanceOfOpenType(entry.ResolvedObject, model)
                || !ClientTypeUtil.TryGetContainerProperty(entry.ResolvedObject, out containerProperty)
                || containerProperty.ContainsKey(link.Name))
            {
                return;
            }

            MaterializerNavigationLink linkState = MaterializerNavigationLink.GetLink(link);
            if (linkState == null || (linkState.Entry == null && linkState.Feed == null))
            {
                return;
            }

            // NOTE: ODL (and OData WebApi) support navigational property on complex types
            // That support has not yet been implemented in OData client

            // An entity or entity collection as a dynamic property currently doesn't work as expected 
            // due to the absence of a navigational property definition in the metadata 
            // to express the relationship between that entity and the parent entity - unless they're the same type!
            // Only materialize a nested resource if its a complex or complex collection

            if (linkState.Feed != null)
            {
                string collectionTypeName = linkState.Feed.TypeName; // TypeName represents a collection e.g. Collection(NS.Type)
                string collectionItemTypeName = CommonUtil.GetCollectionItemTypeName(collectionTypeName, false);

                // Highly unlikely, but the method return null if the typeName argument does not meet certain expectations
                if (collectionItemTypeName == null)
                {
                    return;
                }

                Type collectionItemType = ResolveClientTypeForDynamicProperty(collectionItemTypeName, entry.ResolvedObject);

                if (collectionItemType != null && ClientTypeUtil.TypeIsComplex(collectionItemType, model))
                {
                    Type collectionType = typeof(System.Collections.ObjectModel.Collection<>).MakeGenericType(new Type[] { collectionItemType });
                    IList collection = (IList)Util.ActivatorCreateInstance(collectionType);

                    IEnumerable<ODataResource> feedEntries = MaterializerFeed.GetFeed(linkState.Feed).Entries;
                    foreach (ODataResource feedEntry in feedEntries)
                    {
                        MaterializerEntry linkEntry = MaterializerEntry.GetEntry(feedEntry);
                        this.Materialize(linkEntry, collectionItemType, false /*includeLinks*/);
                        collection.Add(linkEntry.ResolvedObject);
                    }

                    containerProperty.Add(link.Name, collection);
                }
            }
            else 
            {
                MaterializerEntry linkEntry = linkState.Entry;
                Type itemType = ResolveClientTypeForDynamicProperty(linkEntry.Entry.TypeName, entry.ResolvedObject);

                if (itemType != null && ClientTypeUtil.TypeIsComplex(itemType, model))
                {
                    this.Materialize(linkEntry, itemType, false /*includeLinks*/);
                    containerProperty.Add(link.Name, linkEntry.ResolvedObject);
                }
            }
        }
    }
}
