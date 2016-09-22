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

namespace System.Data.Services.Client.Materialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Data.Services.Client.Metadata;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.OData;
    using DSClient = System.Data.Services.Client;
    
    /// <summary>
    /// Used to materialize entities from an <see cref="ODataEntry"/> to an object.
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
        internal static void ValidatePropertyMatch(ClientPropertyAnnotation property, ODataNavigationLink link)
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
        internal static Type ValidatePropertyMatch(ClientPropertyAnnotation property, ODataNavigationLink link, ClientEdmModel model, bool performEntityCheck)
        {
            Debug.Assert(property != null, "property != null");
            Debug.Assert(link != null, "link != null");

            Type propertyType = null;
            if (link.IsCollection.HasValue)
            {
                if (link.IsCollection.Value)
                {
                    // We need to fail if the payload states that the property is a navigation collection property
                    // and in the client, the property is not a collection property.
                    if (!property.IsEntityCollection)
                    {
                        throw DSClient.Error.InvalidOperation(DSClient.Strings.Deserialize_MismatchAtomLinkFeedPropertyNotCollection(property.PropertyName));
                    }

                    propertyType = property.EntityCollectionItemType;
                }
                else
                {
                    if (property.IsEntityCollection)
                    {
                        throw DSClient.Error.InvalidOperation(DSClient.Strings.Deserialize_MismatchAtomLinkEntryPropertyIsCollection(property.PropertyName));
                    }

                    propertyType = property.PropertyType;
                }
            }

            // If the server type and the client type does not match, we need to throw.
            if (propertyType != null && performEntityCheck)
            {
                if (!ClientTypeUtil.TypeIsEntity(propertyType, model))
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

            ODataFeed feed = atomProperty.Value as ODataFeed;
            ODataEntry entry = atomProperty.Value as ODataEntry;

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

            // ResolvedObject will already be assigned when we have a TargetInstance, for example.
            if (!this.EntityTrackingAdapter.TryResolveExistingEntity(entry, expectedEntryType))
            {
                // If the type is a derived one this call will resolve to derived type, cannot put code in  ResolveByCreatingWithType as this is used by projection and in this case
                // the type cannot be changed or updated
                ClientTypeAnnotation actualType = this.MaterializerContext.ResolveTypeForMaterialization(expectedEntryType, entry.Entry.TypeName);

                this.ResolveByCreatingWithType(entry, actualType.ElementType);
            }

            Debug.Assert(entry.ResolvedObject != null, "entry.ResolvedObject != null -- otherwise ResolveOrCreateInstnace didn't do its job");

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
            Debug.Assert(entry.Entry != null || entry.ForLoadProperty, "ODataEntry should be non-null except for LoadProperty");
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
            Debug.Assert(entry.Entry != null || entry.ForLoadProperty, "ODataEntry should be non-null except for LoadProperty");
            Debug.Assert(property != null, "property != null");
            Debug.Assert(items != null, "items != null");

            object collection = null;

            ClientEdmModel edmModel = this.MaterializerContext.Model;
            ClientTypeAnnotation collectionType = edmModel.GetClientTypeAnnotation(edmModel.GetOrCreateEdmType(property.EntityCollectionItemType));

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
                Debug.Assert(!entry.ForLoadProperty, "LoadProperty should always have ShouldUpateForPayload set to true.");

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
            Debug.Assert(property.IsEntityCollection, "property.IsEntityCollection has to be true - otherwise property isn't a collection");

            // NOTE: in V1, we would have instantiated nested objects before setting them.
            object result;
            result = property.GetValue(instance);

            if (result == null)
            {
                Type collectionType = property.PropertyType;

                // For backward compatiblity we need to have different strategy of collection creation b/w
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
            ODataFeed feed,
            bool includeLinks)
        {
            Debug.Assert(entry.Entry != null, "entry != null");
            Debug.Assert(property != null, "property != null");
            Debug.Assert(feed != null, "feed != null");

            ClientEdmModel edmModel = this.MaterializerContext.Model;
            ClientTypeAnnotation collectionType = edmModel.GetClientTypeAnnotation(edmModel.GetOrCreateEdmType(property.EntityCollectionItemType));

            IEnumerable<ODataEntry> entries = MaterializerFeed.GetFeed(feed).Entries;

            foreach (ODataEntry feedEntry in entries)
            {
                this.Materialize(MaterializerEntry.GetEntry(feedEntry), collectionType.ElementType, includeLinks);
            }

            ProjectionPlan continuationPlan = includeLinks ? 
                ODataEntityMaterializer.CreatePlanForDirectMaterialization(property.EntityCollectionItemType) : 
                ODataEntityMaterializer.CreatePlanForShallowMaterialization(property.EntityCollectionItemType);
            
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
        /// <param name="includeLinks">Whether links that are expanded should be materialized.</param>
        /// <remarks>This is a payload-driven materialization process.</remarks>
        private void MaterializeResolvedEntry(MaterializerEntry entry, bool includeLinks)
        {
            Debug.Assert(entry.Entry != null, "entry != null");
            Debug.Assert(entry.ResolvedObject != null, "entry.ResolvedObject != null -- otherwise not resolved/created!");

            ClientTypeAnnotation actualType = entry.ActualType;

            // While materializing entities, we need to make sure the payload that came in the wire is also an entity.
            // Otherwise we need to fail.
            if (!actualType.IsEntityType)
            {
                throw DSClient.Error.InvalidOperation(DSClient.Strings.AtomMaterializer_InvalidNonEntityType(actualType.ElementTypeName));
            }

            // Note that even if ShouldUpdateFromPayload is false, we will still be creating
            // nested instances (but not their links), so they show up in the data context
            // entries. This keeps this code compatible with V1 behavior.
            this.MaterializeDataValues(actualType, entry.Properties, this.MaterializerContext.IgnoreMissingProperties);

            if (entry.NavigationLinks != null)
            {
                foreach (ODataNavigationLink link in entry.NavigationLinks)
                {
                    var prop = actualType.GetProperty(link.Name, true);
                    if (prop != null)
                    {
                        ValidatePropertyMatch(prop, link, this.MaterializerContext.Model, true /*performEntityCheck*/);
                    }
                }

                if (includeLinks)
                {
                    foreach (ODataNavigationLink link in entry.NavigationLinks)
                    {
                        MaterializerNavigationLink linkState = MaterializerNavigationLink.GetLink(link);

                        // Ignore...
                        if (linkState == null)
                        {
                            continue;
                        }

                        var prop = actualType.GetProperty(link.Name, this.MaterializerContext.IgnoreMissingProperties);

                        if (prop == null)
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
                                Debug.Assert(includeLinks, "includeLinks -- otherwise we shouldn't be materializing this entry");
                                this.Materialize(linkEntry, prop.PropertyType, includeLinks);
                            }

                            if (entry.ShouldUpdateFromPayload)
                            {
                                prop.SetValue(entry.ResolvedObject, linkEntry.ResolvedObject, link.Name, true /* allowAdd? */);
                                this.EntityTrackingAdapter.MaterializationLog.SetLink(entry, prop.PropertyName, linkEntry.ResolvedObject);
                            }
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

                var prop = actualType.GetProperty(e.Name, this.MaterializerContext.IgnoreMissingProperties);
                if (prop == null)
                {
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

            this.MaterializerContext.ResponsePipeline.FireEndEntryEvents(entry);
        }
    }
}
