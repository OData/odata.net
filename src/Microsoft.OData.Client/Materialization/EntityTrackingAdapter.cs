//---------------------------------------------------------------------
// <copyright file="EntityTrackingAdapter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Metadata;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Determines if there is an existing entity or whether a new one is created
    /// </summary>
    internal class EntityTrackingAdapter
    {
        /// <summary>Target instance that the materializer expects to update.</summary>
        private object targetInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityTrackingAdapter" /> class.
        /// </summary>
        /// <param name="entityTracker">The entity tracker.</param>
        /// <param name="mergeOption">The merge option.</param>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        internal EntityTrackingAdapter(EntityTrackerBase entityTracker, MergeOption mergeOption, ClientEdmModel model, DataServiceContext context)
        {
            this.MaterializationLog = new AtomMaterializerLog(mergeOption, model, entityTracker);
            this.MergeOption = mergeOption;
            this.EntityTracker = entityTracker;
            this.Model = model;
            this.Context = context;
        }

        /// <summary>
        /// Gets the value of the MergeOption
        /// </summary>
        internal MergeOption MergeOption { get; private set; }

        /// <summary>
        /// Gets the Context
        /// </summary>
        /// <remarks>Implementation Note, only used in when a new DataServiceCollection,
        ///  would like to remove this dependency but would need to change projection
        ///  plan, might happen in a subsequent refactor
        /// </remarks>
        internal DataServiceContext Context { get; private set; }

        /// <summary>
        /// Gets the materialization log.
        /// </summary>
        internal AtomMaterializerLog MaterializationLog { get; private set; }

        /// <summary>
        /// Gets the entity tracker.
        /// </summary>
        internal EntityTrackerBase EntityTracker { get; private set; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        internal ClientEdmModel Model { get; private set; }

        /// <summary>
        /// Target instance that the materializer expects to update.
        /// </summary>
        internal object TargetInstance
        {
            get
            {
                return this.targetInstance;
            }

            set
            {
                Debug.Assert(value != null, "value != null -- otherwise we have no instance target.");
                this.targetInstance = value;
            }
        }

        /// <summary>Resolved or creates an instance on the specified <paramref name="entry"/>.</summary>
        /// <param name="entry">Entry on which to resolve or create an instance.</param>
        /// <param name="expectedEntryType">Expected type for the <paramref name="entry"/>.</param>
        /// <remarks>
        /// After invocation, the ResolvedObject value of the <paramref name="entry"/>
        /// will be assigned, along with the ActualType value.
        /// </remarks>
        /// <returns>True if an existing entity is found.</returns>
        internal virtual bool TryResolveExistingEntity(MaterializerEntry entry, Type expectedEntryType)
        {
            Debug.Assert(entry.Entry != null, "entry != null");
            Debug.Assert(expectedEntryType != null, "expectedEntryType != null");
            Debug.Assert(entry.EntityHasBeenResolved == false, "entry.EntityHasBeenResolved == false");

            // This will be the case when TargetInstance has been set.
            if (this.TryResolveAsTarget(entry))
            {
                return true;
            }

            if (this.TryResolveAsExistingEntry(entry, expectedEntryType))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to resolve the specified entry as an entry that has already been created in this materialization session or is already in the context.
        /// </summary>
        /// <param name="entry">Entry to resolve.</param>
        /// <param name="expectedEntryType">Expected type of the entry.</param>
        /// <returns>True if the entry was resolved, otherwise False.</returns>
        internal bool TryResolveAsExistingEntry(MaterializerEntry entry, Type expectedEntryType)
        {
            if (entry.Entry.IsTransient)
            {
                return false;
            }

            // Resolution is based on the entry Id, so if we can't use that property, we don't even need to try to resolve it.
            if (entry.IsTracking)
            {
                // The resolver methods below will both need access to Id, so first ensure it's not null
                if (entry.Id == null)
                {
                    throw DSClient.Error.InvalidOperation(DSClient.Strings.Deserialize_MissingIdElement);
                }

                return this.TryResolveAsCreated(entry) || this.TryResolveFromContext(entry, expectedEntryType);
            }

            return false;
        }

        /// <summary>Tries to resolve the object as the target one in a POST refresh.</summary>
        /// <param name="entry">Entry to resolve.</param>
        /// <returns>true if the entity was resolved; false otherwise.</returns>
        private bool TryResolveAsTarget(MaterializerEntry entry)
        {
            if (entry.ResolvedObject == null)
            {
                return false;
            }

            // The only case when the entity hasn't been resolved but
            // it has already been set is when the target instance
            // was set directly to refresh a POST.
            Debug.Assert(
                entry.ResolvedObject == this.TargetInstance,
                "entry.ResolvedObject == this.TargetInstance -- otherwise there we ResolveOrCreateInstance more than once on the same entry");
            Debug.Assert(
                this.MergeOption == MergeOption.OverwriteChanges || this.MergeOption == MergeOption.PreserveChanges,
                "MergeOption.OverwriteChanges and MergeOption.PreserveChanges are the only expected values during SaveChanges");
            ClientEdmModel edmModel = this.Model;
            entry.ActualType = edmModel.GetClientTypeAnnotation(edmModel.GetOrCreateEdmType(entry.ResolvedObject.GetType()));
            this.MaterializationLog.FoundTargetInstance(entry);
            entry.ShouldUpdateFromPayload = this.MergeOption != MergeOption.PreserveChanges;
            entry.EntityHasBeenResolved = true;
            return true;
        }

        /// <summary>Tries to resolve the object as one from the context (only if tracking is enabled).</summary>
        /// <param name="entry">Entry to resolve.</param>
        /// <param name="expectedEntryType">Expected entry type for the specified <paramref name="entry"/>.</param>
        /// <returns>true if the entity was resolved; false otherwise.</returns>
        private bool TryResolveFromContext(MaterializerEntry entry, Type expectedEntryType)
        {
            Debug.Assert(entry.IsTracking, "Should not be trying to resolve the entry from the context if entry.isTracking is false.");

            // We should either create a new instance or grab one from the context.
            if (this.MergeOption != MergeOption.NoTracking)
            {
                EntityStates state;
                entry.ResolvedObject = this.EntityTracker.TryGetEntity(entry.Id, out state);
                if (entry.ResolvedObject != null)
                {
                    if (!expectedEntryType.IsInstanceOfType(entry.ResolvedObject))
                    {
                        throw DSClient.Error.InvalidOperation(DSClient.Strings.Deserialize_Current(expectedEntryType, entry.ResolvedObject.GetType()));
                    }

                    ClientEdmModel edmModel = this.Model;
                    entry.ActualType = edmModel.GetClientTypeAnnotation(edmModel.GetOrCreateEdmType(entry.ResolvedObject.GetType()));
                    entry.EntityHasBeenResolved = true;

                    // Note that deleted items will have their properties overwritten even
                    // if PreserveChanges is used as a merge option.
                    entry.ShouldUpdateFromPayload =
                        this.MergeOption == MergeOption.OverwriteChanges ||
                       (this.MergeOption == MergeOption.PreserveChanges && state == EntityStates.Unchanged) ||
                       (this.MergeOption == MergeOption.PreserveChanges && state == EntityStates.Deleted);

                    this.MaterializationLog.FoundExistingInstance(entry);

                    return true;
                }
            }

            return false;
        }

        /// <summary>Tries to resolve the object from those created in this materialization session.</summary>
        /// <param name="entry">Entry to resolve.</param>
        /// <returns>true if the entity was resolved; false otherwise.</returns>
        private bool TryResolveAsCreated(MaterializerEntry entry)
        {
            Debug.Assert(entry.IsTracking, "Should not be trying to resolve the entry from the current materialization session if entry.isTracking is false.");

            MaterializerEntry existingEntry;
            if (!this.MaterializationLog.TryResolve(entry, out existingEntry))
            {
                return false;
            }

            Debug.Assert(
                existingEntry.ResolvedObject != null,
                "existingEntry.ResolvedObject != null -- how did it get there otherwise?");
            entry.ActualType = existingEntry.ActualType;
            entry.ResolvedObject = existingEntry.ResolvedObject;
            entry.CreatedByMaterializer = existingEntry.CreatedByMaterializer;
            entry.ShouldUpdateFromPayload = existingEntry.ShouldUpdateFromPayload;
            entry.EntityHasBeenResolved = true;
            return true;
        }
    }
}