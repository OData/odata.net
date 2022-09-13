//---------------------------------------------------------------------
// <copyright file="EntityTracker.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Client.Metadata;
    using System.Runtime.CompilerServices;
    #endregion Namespaces

    /// <summary>
    /// context
    /// </summary>
    public class EntityTracker : EntityTrackerBase
    {
        /// <summary>Storage for the client model.</summary>
        private readonly ClientEdmModel model;

        #region Resource state management

        /// <summary>Set of tracked resources</summary>
        private ConcurrentDictionary<object, EntityDescriptor> entityDescriptors = new ConcurrentDictionary<object, EntityDescriptor>(EntityEqualityComparer<object>.Instance);
        /// <summary>Set of tracked resources by Identity</summary>
        private ConcurrentDictionary<Uri, EntityDescriptor> identityToDescriptor;
        /// <summary>Set of tracked bindings</summary>
        private ConcurrentDictionary<LinkDescriptor, LinkDescriptor> bindings;

        /// <summary>change order</summary>
        private uint nextChange;
        #endregion

        #region ctor

        /// <summary>
        /// Creates a new instance of EntityTracker class which tracks all instances of entities and links tracked by the context.
        /// </summary>
        /// <param name="maxProtocolVersion">max protocol version that the client understands.</param>
        internal EntityTracker(ClientEdmModel maxProtocolVersion)
        {
            this.model = maxProtocolVersion;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns a collection of all the links (ie. associations) currently being tracked by the context.
        /// If no links are being tracked, a collection with 0 elements is returned.
        /// </summary>
        public IEnumerable<LinkDescriptor> Links
        {
            get
            {
                this.EnsureLinkBindings();
                return this.bindings.Values;
            }
        }

        /// <summary>
        /// Returns a collection of all the resources currently being tracked by the context.
        /// If no resources are being tracked, a collection with 0 elements is returned.
        /// </summary>
        public IEnumerable<EntityDescriptor> Entities
        {
            get
            {
                return this.entityDescriptors.Values;
            }
        }
        #endregion

        #region Entity Methods

        /// <summary>Gets the entity descriptor corresponding to a particular entity</summary>
        /// <param name="entity">Entity for which to find the entity descriptor</param>
        /// <returns>EntityDescriptor for the <paramref name="entity"/> or null if not found</returns>
        public EntityDescriptor TryGetEntityDescriptor(object entity)
        {
            Debug.Assert(entity != null, "entity != null");
            EntityDescriptor entityDescriptor = null;
            this.entityDescriptors.TryGetValue(entity, out entityDescriptor);
            return entityDescriptor;
        }

        /// <summary>
        /// verify the resource being tracked by context
        /// </summary>
        /// <param name="resource">resource</param>
        /// <returns>The given resource.</returns>
        /// <exception cref="InvalidOperationException">if resource is not contained</exception>
        internal override EntityDescriptor GetEntityDescriptor(object resource)
        {
            EntityDescriptor entityDescriptor = this.TryGetEntityDescriptor(resource);
            if (entityDescriptor == null)
            {
                throw Error.InvalidOperation(Strings.Context_EntityNotContained);
            }

            return entityDescriptor;
        }

        /// <summary>
        /// checks whether there is a tracked entity with the given identity.
        /// </summary>
        /// <param name="identity">identity of the entity.</param>
        /// <returns>returns the entity if the identity matches, otherwise returns null.</returns>
        internal EntityDescriptor TryGetEntityDescriptor(Uri identity)
        {
            EntityDescriptor entityDescriptor;
            if (this.identityToDescriptor != null &&
                this.identityToDescriptor.TryGetValue(identity, out entityDescriptor))
            {
                return entityDescriptor;
            }

            return null;
        }

        /// <summary>
        /// Adds the given entity descriptors to the list of the tracked entity descriptors.
        /// </summary>
        /// <param name="descriptor">entity descriptor instance to be added.</param>
        internal void AddEntityDescriptor(EntityDescriptor descriptor)
        {
            try
            {
                this.entityDescriptors.Add(descriptor.Entity, descriptor);
            }
            catch (ArgumentException)
            {
                throw Error.InvalidOperation(Strings.Context_EntityAlreadyContained);
            }
        }

        /// <summary>the work to detach a resource</summary>
        /// <param name="resource">resource to detach</param>
        /// <returns>true if detached</returns>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "flag", Justification = "Local variable used in debug assertion.")]
        internal bool DetachResource(EntityDescriptor resource)
        {
            this.EnsureLinkBindings();

            // Since we are changing the list on the fly, we need to convert it into a list first
            // so that enumeration won't get effected.
            foreach (LinkDescriptor end in this.bindings.Values.Where(resource.IsRelatedEntity).ToList())
            {
                this.DetachExistingLink(
                        end,
                        end.Target == resource.Entity && resource.State == EntityStates.Added);
            }

            resource.ChangeOrder = UInt32.MaxValue;
            resource.State = EntityStates.Detached;
            bool flag = this.entityDescriptors.Remove(resource.Entity);
            Debug.Assert(flag, "should have removed existing entity");
            this.DetachResourceIdentity(resource);

            return true;
        }

        /// <summary>remove the identity attached to the resource</summary>
        /// <param name="resource">resource with an identity to detach to detach</param>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "removed", Justification = "Local variable used in debug assertion.")]
        internal void DetachResourceIdentity(EntityDescriptor resource)
        {
            EntityDescriptor existing = null;
            if ((resource.Identity != null) &&
                this.identityToDescriptor.TryGetValue(resource.Identity, out existing) &&
                Object.ReferenceEquals(existing, resource))
            {
                bool removed = this.identityToDescriptor.Remove(resource.Identity);
                Debug.Assert(removed, "should have removed existing identity");
            }
        }

        #endregion Entity Methods

        #region Link Methods
        /// <summary>
        /// Gets the link descriptor corresponding to a particular link b/w source and target objects
        /// </summary>
        /// <param name="source">Source entity</param>
        /// <param name="sourceProperty">Property of <paramref name="source"/></param>
        /// <param name="target">Target entity</param>
        /// <returns>LinkDescriptor for the relationship b/w source and target entities or null if not found</returns>
        internal LinkDescriptor TryGetLinkDescriptor(object source, string sourceProperty, object target)
        {
            Debug.Assert(source != null, "source != null");
            Debug.Assert(sourceProperty != null, "sourceProperty != null");

            this.EnsureLinkBindings();

            LinkDescriptor link;
            this.bindings.TryGetValue(new LinkDescriptor(source, sourceProperty, target, this.model), out link);
            return link;
        }

        /// <summary>
        /// attach the link with the given source, sourceProperty and target.
        /// </summary>
        /// <param name="source">source entity of the link.</param>
        /// <param name="sourceProperty">name of the property on the source entity.</param>
        /// <param name="target">target entity of the link.</param>
        /// <param name="linkMerge">merge option to be used to merge the link if there is an existing link.</param>
        internal override void AttachLink(object source, string sourceProperty, object target, MergeOption linkMerge)
        {
            LinkDescriptor relation = new LinkDescriptor(source, sourceProperty, target, this.model);
            LinkDescriptor existing = this.TryGetLinkDescriptor(source, sourceProperty, target);
            if (existing != null)
            {
                switch (linkMerge)
                {
                    case MergeOption.AppendOnly:
                        break;

                    case MergeOption.OverwriteChanges:
                        relation = existing;
                        break;

                    case MergeOption.PreserveChanges:
                        if ((existing.State == EntityStates.Added) ||
                            (existing.State == EntityStates.Unchanged) ||
                            (existing.State == EntityStates.Modified && existing.Target != null))
                        {
                            relation = existing;
                        }

                        break;

                    case MergeOption.NoTracking: // public API point should throw if link exists
                        throw Error.InvalidOperation(Strings.Context_RelationAlreadyContained);
                }
            }
            else
            {
                if (this.model.GetClientTypeAnnotation(this.model.GetOrCreateEdmType(source.GetType())).GetProperty(sourceProperty, UndeclaredPropertyBehavior.ThrowException).IsEntityCollection ||
                    ((existing = this.DetachReferenceLink(source, sourceProperty, target, linkMerge)) == null))
                {
                    this.AddLink(relation);
                    this.IncrementChange(relation);
                }
                else if (!((MergeOption.AppendOnly == linkMerge) ||
                           (MergeOption.PreserveChanges == linkMerge && EntityStates.Modified == existing.State)))
                {
                    // AppendOnly doesn't change state or target
                    // OverWriteChanges changes target and state
                    // PreserveChanges changes target if unchanged, leaves modified target and state alone
                    relation = existing;
                }
            }

            relation.State = EntityStates.Unchanged;
        }

        /// <summary>
        /// find and detach link for reference property
        /// </summary>
        /// <param name="source">source entity</param>
        /// <param name="sourceProperty">source entity property name for target entity</param>
        /// <param name="target">target entity</param>
        /// <param name="linkMerge">link merge option</param>
        /// <returns>true if found and not removed</returns>
        internal LinkDescriptor DetachReferenceLink(object source, string sourceProperty, object target, MergeOption linkMerge)
        {
            Debug.Assert(sourceProperty.IndexOf('/') == -1, "sourceProperty.IndexOf('/') == -1");

            LinkDescriptor existing = this.GetLinks(source, sourceProperty).FirstOrDefault();
            if (existing != null)
            {
                if ((target == existing.Target) ||
                    (MergeOption.AppendOnly == linkMerge) ||
                    (MergeOption.PreserveChanges == linkMerge && EntityStates.Modified == existing.State))
                {
                    return existing;
                }

                // Since we don't support deep insert on reference property, no need to check for deep insert.
                this.DetachExistingLink(existing, false);
                Debug.Assert(!this.Links.Any(o => (o.Source == source) && (o.SourceProperty == sourceProperty)), "only expecting one");
            }

            return null;
        }

        /// <summary>
        /// Add the given link to the link descriptor collection
        /// </summary>
        /// <param name="linkDescriptor">link descriptor to add</param>
        /// <exception cref="InvalidOperationException">throws argument exception if the link already exists</exception>
        internal void AddLink(LinkDescriptor linkDescriptor)
        {
            Debug.Assert(linkDescriptor != null, "linkDescriptor != null");
            try
            {
                this.EnsureLinkBindings();
                this.bindings.Add(linkDescriptor, linkDescriptor);
            }
            catch (ArgumentException)
            {
                throw Error.InvalidOperation(Strings.Context_RelationAlreadyContained);
            }
        }

        /// <summary>
        /// Remove the link from the list of tracked link descriptors.
        /// </summary>
        /// <param name="linkDescriptor">link to be removed.</param>
        /// <returns>true if the link was tracked and now removed, otherwise returns false.</returns>
        internal bool TryRemoveLinkDescriptor(LinkDescriptor linkDescriptor)
        {
            this.EnsureLinkBindings();
            return this.bindings.Remove(linkDescriptor);
        }

        /// <summary>
        /// get the related links ignoring target entity
        /// </summary>
        /// <param name="source">source entity</param>
        /// <param name="sourceProperty">source entity's property</param>
        /// <returns>enumerable of related ends</returns>
        internal override IEnumerable<LinkDescriptor> GetLinks(object source, string sourceProperty)
        {
            this.EnsureLinkBindings();
            Debug.Assert(sourceProperty.IndexOf('/') == -1, "sourceProperty.IndexOf('/') == -1");
            return this.bindings.Values.Where(o => (o.Source == source) && (o.SourceProperty == sourceProperty));
        }

        /// <summary>Detach existing link</summary>
        /// <param name="existingLink">link to detach</param>
        /// <param name="targetDelete">true if target is being deleted, false otherwise</param>
        internal override void DetachExistingLink(LinkDescriptor existingLink, bool targetDelete)
        {
            // The target can be null in which case we don't need this check
            if (existingLink.Target != null)
            {
                // Identify the target resource for the link
                EntityDescriptor targetResource = this.GetEntityDescriptor(existingLink.Target);

                // Check if there is a dependency relationship b/w the source and target objects i.e. target can not exist without source link
                // Deep insert requires this check to be made but skip the check if the target object is being deleted
                if (targetResource.IsDeepInsert && !targetDelete)
                {
                    EntityDescriptor parentOfTarget = targetResource.ParentForInsert;
                    if (Object.ReferenceEquals(targetResource.ParentEntity, existingLink.Source) &&
                       (parentOfTarget.State != EntityStates.Deleted ||
                        parentOfTarget.State != EntityStates.Detached))
                    {
                        throw new InvalidOperationException(Strings.Context_ChildResourceExists);
                    }
                }
            }

            if (this.TryRemoveLinkDescriptor(existingLink))
            {   // this link may have been previously detached by a detaching entity
                existingLink.State = EntityStates.Detached;
            }
        }

        #endregion // Link Methods

        /// <summary>response materialization has an identity to attach to the inserted object</summary>
        /// <param name="entityDescriptorFromMaterializer">entity descriptor containing all the information about the entity from the response.</param>
        /// <param name="metadataMergeOption">mergeOption based on which EntityDescriptor will be merged.</param>
        internal override void AttachIdentity(EntityDescriptor entityDescriptorFromMaterializer, MergeOption metadataMergeOption)
        {   // insert->unchanged
            Debug.Assert(entityDescriptorFromMaterializer != null, "entityDescriptorFromMaterializer != null");

            this.EnsureIdentityToResource();

            // resource.State == EntityState.Added or Unchanged for second pass of media link
            EntityDescriptor trackedEntityDescriptor = this.entityDescriptors[entityDescriptorFromMaterializer.Entity];

            // make sure we got the right one - server could override identity and we may be tracking another one already.
            this.ValidateDuplicateIdentity(entityDescriptorFromMaterializer.Identity, trackedEntityDescriptor);

            this.DetachResourceIdentity(trackedEntityDescriptor);

            // While processing the response, we need to find out if the given resource was inserted deep
            // If it was, then we need to change the link state from added to unchanged
            if (trackedEntityDescriptor.IsDeepInsert)
            {
                LinkDescriptor end = this.bindings[trackedEntityDescriptor.GetRelatedEnd()];
                end.State = EntityStates.Unchanged;
            }

            trackedEntityDescriptor.Identity = entityDescriptorFromMaterializer.Identity; // always attach the identity
            AtomMaterializerLog.MergeEntityDescriptorInfo(trackedEntityDescriptor, entityDescriptorFromMaterializer, true /*mergeInfo*/, metadataMergeOption);
            trackedEntityDescriptor.State = EntityStates.Unchanged;
            trackedEntityDescriptor.PropertiesToSerialize.Clear();

            // scenario: successfully (1) delete an existing entity and (2) add a new entity where the new entity has the same identity as deleted entity
            // where the SaveChanges pass1 will now associate existing identity with new entity
            // but pass2 for the deleted entity will not blindly remove the identity that is now associated with the new identity
            this.identityToDescriptor[entityDescriptorFromMaterializer.Identity] = trackedEntityDescriptor;
        }

        /// <summary>use location from header to generate initial edit and identity</summary>
        /// <param name="entity">entity in added state</param>
        /// <param name="identity">identity as specified in the response header - location header or OData-EntityId header.</param>
        /// <param name="editLink">editlink as specified in the response header - location header.</param>
        internal void AttachLocation(object entity, Uri identity, Uri editLink)
        {
            Debug.Assert(entity != null, "null != entity");
            Debug.Assert(editLink != null, "editLink != null");

            this.EnsureIdentityToResource();

            // resource.State == EntityState.Added or Unchanged for second pass of media link
            EntityDescriptor resource = this.entityDescriptors[entity];

            // make sure we got the right one - server could override identity and we may be tracking another one already.
            this.ValidateDuplicateIdentity(identity, resource);

            this.DetachResourceIdentity(resource);

            // While processing the response, we need to find out if the given resource was inserted deep
            // If it was, then we need to change the link state from added to unchanged
            if (resource.IsDeepInsert)
            {
                LinkDescriptor end = this.bindings[resource.GetRelatedEnd()];
                end.State = EntityStates.Unchanged;
            }

            resource.Identity = identity; // always attach the identity
            resource.EditLink = editLink;

            // scenario: successfully batch (1) add a new entity and (2) delete an existing entity where the new entity has the same identity as deleted entity
            // where the SaveChanges pass1 will now associate existing identity with new entity
            // but pass2 for the deleted entity will not blindly remove the identity that is now associated with the new identity
            this.identityToDescriptor[identity] = resource;
        }

        /// <summary>
        /// Attach entity into the context in the Unchanged state.
        /// </summary>
        /// <param name="entityDescriptorFromMaterializer">entity descriptor from the response</param>
        /// <param name="failIfDuplicated">fail for public api else change existing relationship to unchanged</param>
        /// <remarks>Caller should validate descriptor instance.</remarks>
        /// <returns>The attached descriptor, if one already exists in the context and failIfDuplicated is set to false, then the existing instance is returned</returns>
        /// <exception cref="InvalidOperationException">if entity is already being tracked by the context</exception>
        /// <exception cref="InvalidOperationException">if identity is pointing to another entity</exception>
        internal override EntityDescriptor InternalAttachEntityDescriptor(EntityDescriptor entityDescriptorFromMaterializer, bool failIfDuplicated)
        {
            Debug.Assert(entityDescriptorFromMaterializer.Identity != null, "must have identity");
            Debug.Assert(entityDescriptorFromMaterializer.Entity != null && ClientTypeUtil.TypeIsEntity(entityDescriptorFromMaterializer.Entity.GetType(), this.model), "must be entity type to attach");

            this.EnsureIdentityToResource();

            EntityDescriptor trackedEntityDescriptor;
            this.entityDescriptors.TryGetValue(entityDescriptorFromMaterializer.Entity, out trackedEntityDescriptor);

            EntityDescriptor existing;
            this.identityToDescriptor.TryGetValue(entityDescriptorFromMaterializer.Identity, out existing);

            // identity existing & pointing to something else
            if (failIfDuplicated && (trackedEntityDescriptor != null))
            {
                throw Error.InvalidOperation(Strings.Context_EntityAlreadyContained);
            }
            else if (trackedEntityDescriptor != existing)
            {
                throw Error.InvalidOperation(Strings.Context_DifferentEntityAlreadyContained);
            }
            else if (trackedEntityDescriptor == null)
            {
                trackedEntityDescriptor = entityDescriptorFromMaterializer;

                // if resource doesn't exist...
                this.IncrementChange(entityDescriptorFromMaterializer);
                this.entityDescriptors.Add(entityDescriptorFromMaterializer.Entity, entityDescriptorFromMaterializer);
                this.identityToDescriptor.Add(entityDescriptorFromMaterializer.Identity, entityDescriptorFromMaterializer);
            }

            // DEVNOTE(pqian):
            // we used to mark the descriptor as Unchanged
            // but it's now up to the caller to do that
            return trackedEntityDescriptor;
        }

        /// <summary>
        /// Find tracked entity by its resourceUri and update its etag.
        /// </summary>
        /// <param name="resourceUri">resource id</param>
        /// <param name="state">state of entity</param>
        /// <returns>entity if found else null</returns>
        internal override object TryGetEntity(Uri resourceUri, out EntityStates state)
        {
            Debug.Assert(resourceUri != null, "null uri");
            state = EntityStates.Detached;

            EntityDescriptor resource = null;
            if ((this.identityToDescriptor != null) &&
                 this.identityToDescriptor.TryGetValue(resourceUri, out resource))
            {
                state = resource.State;
                Debug.Assert(resource.Entity != null, "null entity");
                return resource.Entity;
            }

            return null;
        }

        /// <summary>
        /// increment the resource change for sorting during submit changes
        /// </summary>
        /// <param name="descriptor">the resource to update the change order</param>
        internal void IncrementChange(Descriptor descriptor)
        {
            descriptor.ChangeOrder = ++this.nextChange;
        }

        /// <summary>create this.identityToResource when necessary</summary>
        private void EnsureIdentityToResource()
        {
            if (this.identityToDescriptor == null)
            {
                System.Threading.Interlocked.CompareExchange(ref this.identityToDescriptor, new ConcurrentDictionary<Uri, EntityDescriptor>(EqualityComparer<Uri>.Default), null);
            }
        }

        /// <summary>create this.bindings when necessary</summary>
        private void EnsureLinkBindings()
        {
            if (this.bindings == null)
            {
                System.Threading.Interlocked.CompareExchange(ref this.bindings, new ConcurrentDictionary<LinkDescriptor, LinkDescriptor>(LinkDescriptor.EquivalenceComparer), null);
            }
        }

        /// <summary>
        /// Ensure an identity is unique and does not point to another resource
        /// </summary>
        /// <param name="identity">The identity</param>
        /// <param name="descriptor">The entity descriptor</param>
        private void ValidateDuplicateIdentity(Uri identity, EntityDescriptor descriptor)
        {
            EntityDescriptor trackedIdentity;
            if (this.identityToDescriptor.TryGetValue(identity, out trackedIdentity) && descriptor != trackedIdentity && trackedIdentity.State != EntityStates.Deleted && trackedIdentity.State != EntityStates.Detached)
            {
                // we checked the state because we do not remove the deleted/detached entity descriptor from the dictionary until we have finished processing all changes
                // So for instance if you delete one entity and add back one with the same ID, they will be a temporary conflict in the dictionary.
                throw Error.InvalidOperation(Strings.Context_DifferentEntityAlreadyContained);
            }
        }
    }

    /// <summary>
    /// An object instance equality comparer
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    internal class EntityEqualityComparer<TObject> : EqualityComparer<TObject> where TObject : class
    {
        public static EntityEqualityComparer<TObject> Instance = new EntityEqualityComparer<TObject>();
        private EntityEqualityComparer() { }
        public override bool Equals(TObject x, TObject y) => ReferenceEquals(x, y);
        public override int GetHashCode(TObject obj) => RuntimeHelpers.GetHashCode(obj);
    }
}
