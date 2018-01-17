//---------------------------------------------------------------------
// <copyright file="AtomMaterializerLog.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Client.Materialization;
    using Microsoft.OData.Client.Metadata;

    #endregion Namespaces

    /// <summary>
    /// Use this class to keep a log of changes done by the materializer.
    /// </summary>
    internal class AtomMaterializerLog
    {
        #region Private fields

        /// <summary> The merge option. </summary>
        private readonly MergeOption mergeOption;

        /// <summary> The client edm model. </summary>
        private readonly ClientEdmModel clientEdmModel;

        /// <summary> The entity tracker. </summary>
        private readonly EntityTrackerBase entityTracker;

        /// <summary>Dictionary of identity URI to instances created during previous AppendOnly moves.</summary>
        private readonly Dictionary<Uri, ODataResource> appendOnlyEntries;

        /// <summary>Dictionary of identity URI to tracked entities.</summary>
        private readonly Dictionary<Uri, ODataResource> identityStack;

        /// <summary>List of link descriptors (data for links and state).</summary>
        private readonly List<LinkDescriptor> links;

        /// <summary>Target instance to refresh.</summary>
        private object insertRefreshObject;

        #endregion Private fields

        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="AtomMaterializerLog"/> instance.
        /// </summary>
        /// <param name="mergeOption">The merge option for the log.</param>
        /// <param name="model">The model for the log.</param>
        /// <param name="entityTracker">The entity tracker for the log.</param>
        /// <remarks>
        /// Note that the merge option can't be changed.
        /// </remarks>
        internal AtomMaterializerLog(MergeOption mergeOption, ClientEdmModel model, EntityTrackerBase entityTracker)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(entityTracker != null, "entityTracker != null");

            this.appendOnlyEntries = new Dictionary<Uri, ODataResource>(EqualityComparer<Uri>.Default);
            this.mergeOption = mergeOption;
            this.clientEdmModel = model;
            this.entityTracker = entityTracker;
            this.identityStack = new Dictionary<Uri, ODataResource>(EqualityComparer<Uri>.Default);
            this.links = new List<LinkDescriptor>();
        }

        #endregion Constructors

        #region Internal properties

        /// <summary>Whether changes are being tracked.</summary>
        internal bool Tracking
        {
            get
            {
                return this.mergeOption != MergeOption.NoTracking;
            }
        }

        #endregion Internal properties

        #region Internal methods.

        /// <summary>
        /// This method is used to merge all the metadata that come in the response payload.
        /// </summary>
        /// <param name="trackedEntityDescriptor">entityDescriptor that is getting tracked by the client</param>
        /// <param name="entityDescriptorFromMaterializer">entityDescriptor that is returned by the materializer</param>
        /// <param name="mergeInfo">if true, we will need to merge all entity descriptor info, otherwise not.</param>
        /// <param name="mergeOption">merge option depending on which etag information needs to be merged.</param>
        internal static void MergeEntityDescriptorInfo(EntityDescriptor trackedEntityDescriptor, EntityDescriptor entityDescriptorFromMaterializer, bool mergeInfo, MergeOption mergeOption)
        {
            Debug.Assert(trackedEntityDescriptor != null, "trackedEntityDescriptor != null");
            Debug.Assert(entityDescriptorFromMaterializer != null, "entityDescriptorFromMaterializer != null");

            // In this function, we need to handle 2 cases:
            //  1> Either the 2 function entity descriptors are the same, in which case
            //     we just have to convert the relative uri's into absolute uri's if any
            //  2> If the entity descriptor are not the same, then we need to update the etag
            //     If the merge option is not AppendOnly and merge all entity descriptor if specified
            if (!Object.ReferenceEquals(trackedEntityDescriptor, entityDescriptorFromMaterializer))
            {
                // Keeping the old behavior - merging etags only when the mergeOption is something other than AppendOnly
                if (entityDescriptorFromMaterializer.ETag != null && mergeOption != MergeOption.AppendOnly)
                {
                    trackedEntityDescriptor.ETag = entityDescriptorFromMaterializer.ETag;
                }

                if (mergeInfo)
                {
                    // We need to merge the rest of the metadata irrespective of the MergeOption
                    if (entityDescriptorFromMaterializer.SelfLink != null)
                    {
                        trackedEntityDescriptor.SelfLink = entityDescriptorFromMaterializer.SelfLink;
                    }

                    if (entityDescriptorFromMaterializer.EditLink != null)
                    {
                        trackedEntityDescriptor.EditLink = entityDescriptorFromMaterializer.EditLink;
                    }

                    foreach (LinkInfo linkInfo in entityDescriptorFromMaterializer.LinkInfos)
                    {
                        trackedEntityDescriptor.MergeLinkInfo(linkInfo);
                    }

                    foreach (StreamDescriptor streamInfo in entityDescriptorFromMaterializer.StreamDescriptors)
                    {
                        trackedEntityDescriptor.MergeStreamDescriptor(streamInfo);
                    }

                    trackedEntityDescriptor.ServerTypeName = entityDescriptorFromMaterializer.ServerTypeName;
                }

                // An entity might show actions conditionally based on each request, so get whatever sent by server
                if (entityDescriptorFromMaterializer.OperationDescriptors != null)
                {
                    trackedEntityDescriptor.ClearOperationDescriptors();
                    trackedEntityDescriptor.AppendOperationalDescriptors(entityDescriptorFromMaterializer.OperationDescriptors);
                }

                // TODO: ideally, we should also merge this only based on the merge context, but since for MLE we do POST
                // and PUT as part of the same request, we need to merge it always to make the insert case of MLE work.
                // Once we fix that issue, this also should be moved within the above if statement
                if (entityDescriptorFromMaterializer.ReadStreamUri != null)
                {
                    trackedEntityDescriptor.ReadStreamUri = entityDescriptorFromMaterializer.ReadStreamUri;
                }

                if (entityDescriptorFromMaterializer.EditStreamUri != null)
                {
                    trackedEntityDescriptor.EditStreamUri = entityDescriptorFromMaterializer.EditStreamUri;
                }

                if (entityDescriptorFromMaterializer.ReadStreamUri != null || entityDescriptorFromMaterializer.EditStreamUri != null)
                {
                    trackedEntityDescriptor.StreamETag = entityDescriptorFromMaterializer.StreamETag;
                }
            }
        }

        /// <summary>Applies all accumulated changes to the associated data context.</summary>
        /// <remarks>The log should be cleared after this method successfully executed.</remarks>
        internal void ApplyToContext()
        {
            if (!this.Tracking)
            {
                return;
            }

            foreach (KeyValuePair<Uri, ODataResource> entity in this.identityStack)
            {
                // Try to attach the entity descriptor got from materializer, if one already exists, get the existing reference instead.
                MaterializerEntry entry = MaterializerEntry.GetEntry(entity.Value);

                bool mergeEntityDescriptorInfo = entry.CreatedByMaterializer ||
                                                 entry.ResolvedObject == this.insertRefreshObject ||
                                                 entry.ShouldUpdateFromPayload;

                // Whenever we merge the data, only at those times will be merge the links also
                EntityDescriptor descriptor = this.entityTracker.InternalAttachEntityDescriptor(entry.EntityDescriptor, false /*failIfDuplicated*/);
                AtomMaterializerLog.MergeEntityDescriptorInfo(descriptor, entry.EntityDescriptor, mergeEntityDescriptorInfo, this.mergeOption);

                if (mergeEntityDescriptorInfo)
                {
                    // In AtomMaterializer.TryResolveFromContext, we set AtomEntry.ShouldUpdateFromPayload to true
                    // when even MergeOption is PreserveChanges and entityState is Deleted. But in that case, we cannot
                    // set the entity state to Unchanged, hence need to workaround that one scenario
                    if (this.mergeOption != MergeOption.PreserveChanges || descriptor.State != EntityStates.Deleted)
                    {
                        // we should always reset descriptor's state to Unchanged (old v1 behavior)
                        descriptor.State = EntityStates.Unchanged;
                        descriptor.PropertiesToSerialize.Clear();
                    }
                }
            }

            foreach (LinkDescriptor link in this.links)
            {
                if (EntityStates.Added == link.State)
                {
                    // Added implies collection
                    this.entityTracker.AttachLink(link.Source, link.SourceProperty, link.Target, this.mergeOption);
                }
                else if (EntityStates.Modified == link.State)
                {
                    // Modified implies reference
                    object target = link.Target;
                    if (MergeOption.PreserveChanges == this.mergeOption)
                    {
                        // GetLinks looks up the existing link using just the SourceProperty, the declaring server type name is not significant here.
                        LinkDescriptor end = this.entityTracker.GetLinks(link.Source, link.SourceProperty).SingleOrDefault();
                        if (null != end && null == end.Target)
                        {
                            // leave the SetLink(link.Source, link.SourceProperty, null)
                            continue;
                        }

                        if ((null != target) && (EntityStates.Deleted == this.entityTracker.GetEntityDescriptor(target).State) ||
                            (EntityStates.Deleted == this.entityTracker.GetEntityDescriptor(link.Source).State))
                        {
                            target = null;
                        }
                    }

                    this.entityTracker.AttachLink(link.Source, link.SourceProperty, target, this.mergeOption);
                }
                else
                {
                    // detach link
                    Debug.Assert(EntityStates.Detached == link.State, "not detached link");
                    this.entityTracker.DetachExistingLink(link, false);
                }
            }
        }

        /// <summary>Clears all state in the log.</summary>
        internal void Clear()
        {
            this.identityStack.Clear();
            this.links.Clear();
            this.insertRefreshObject = null;
        }

        /// <summary>
        /// Invoke this method to notify the log that an existing
        /// instance was found while resolving an object.
        /// </summary>
        /// <param name="entry">Entry for instance.</param>
        internal void FoundExistingInstance(MaterializerEntry entry)
        {
            Debug.Assert(entry.Entry != null, "entry != null");
            Debug.Assert(IsEntity(entry), "Existing entries should be entity");
            this.identityStack[entry.Id] = entry.Entry;
        }

        /// <summary>
        /// Invoke this method to notify the log that the
        /// target instance of a "directed" update was found.
        /// </summary>
        /// <param name="entry">Entry found.</param>
        /// <remarks>
        /// The target instance is typically the object that we
        /// expect will get refreshed by the response from a POST
        /// method.
        ///
        /// For example if a create a Customer and POST it to
        /// a service, the response of the POST will return the
        /// re-serialized instance, with (important!) server generated
        /// values and URIs.
        /// </remarks>
        internal void FoundTargetInstance(MaterializerEntry entry)
        {
            Debug.Assert(entry.Entry != null, "entry != null");
            Debug.Assert(entry.ResolvedObject != null, "entry.ResolvedObject != null -- otherwise this is not a target");

            if (IsEntity(entry))
            {
                Debug.Assert(entry.IsTracking, "entry.isTracking == true, otherwise we should not be tracking this entry with the context.");

                this.entityTracker.AttachIdentity(entry.EntityDescriptor, this.mergeOption);
                this.identityStack.Add(entry.Id, entry.Entry);
                this.insertRefreshObject = entry.ResolvedObject;
            }
        }

        /// <summary>Attempts to resolve an entry from those tracked in the log.</summary>
        /// <param name="entry">Entry to resolve.</param>
        /// <param name="existingEntry">
        /// After invocation, an existing entry with the same identity as
        /// <paramref name="entry"/>; possibly null.
        /// </param>
        /// <returns>true if an existing entry was found; false otherwise.</returns>
        internal bool TryResolve(MaterializerEntry entry, out MaterializerEntry existingEntry)
        {
            Debug.Assert(entry.Entry != null, "entry != null");
            Debug.Assert(entry.Id != null, "entry.Id != null");
            Debug.Assert(entry.IsTracking, "Should not be trying to resolve the entry if entry.isTracking is false.");

            ODataResource existingODataEntry;

            if (this.identityStack.TryGetValue(entry.Id, out existingODataEntry))
            {
                existingEntry = MaterializerEntry.GetEntry(existingODataEntry);
                return true;
            }

            if (this.appendOnlyEntries.TryGetValue(entry.Id, out existingODataEntry))
            {
                // The AppendOnly entries are valid only as long as they were not modified
                // between calls to .MoveNext().
                EntityStates state;
                this.entityTracker.TryGetEntity(entry.Id, out state);
                if (state == EntityStates.Unchanged)
                {
                    existingEntry = MaterializerEntry.GetEntry(existingODataEntry);
                    return true;
                }
                else
                {
                    this.appendOnlyEntries.Remove(entry.Id);
                }
            }

            existingEntry = default(MaterializerEntry);
            return false;
        }

        /// <summary>
        /// Invoke this method to notify the log that a new link was
        /// added to a collection.
        /// </summary>
        /// <param name="source">
        /// Instance with the collection to which <paramref name="target"/>
        /// was added.
        /// </param>
        /// <param name="propertyName">Property name for collection.</param>
        /// <param name="target">Object which was added.</param>
        internal void AddedLink(MaterializerEntry source, string propertyName, object target)
        {
            Debug.Assert(source.Entry != null || source.ForLoadProperty, "source != null || source.ForLoadProperty");
            Debug.Assert(propertyName != null, "propertyName != null");

            if (!this.Tracking)
            {
                return;
            }

            if (IsEntity(source) && IsEntity(target, this.clientEdmModel))
            {
                LinkDescriptor item = new LinkDescriptor(source.ResolvedObject, propertyName, target, EntityStates.Added);
                this.links.Add(item);
            }
        }

        /// <summary>
        /// Invoke this method to notify the log that a new instance
        /// was created.
        /// </summary>
        /// <param name="entry">Entry for the created instance.</param>
        internal void CreatedInstance(MaterializerEntry entry)
        {
            Debug.Assert(entry.Entry != null, "entry != null");
            Debug.Assert(entry.ResolvedObject != null, "entry.ResolvedObject != null -- otherwise, what did we create?");
            Debug.Assert(entry.CreatedByMaterializer, "entry.CreatedByMaterializer -- otherwise we shouldn't be calling this");

            if (IsEntity(entry) && entry.IsTracking && !entry.Entry.IsTransient)
            {
                this.identityStack.Add(entry.Id, entry.Entry);
                if (this.mergeOption == MergeOption.AppendOnly)
                {
                    this.appendOnlyEntries.Add(entry.Id, entry.Entry);
                }
            }
        }

        /// <summary>
        /// Invoke this method to notify the log that a link was removed
        /// from a collection.
        /// </summary>
        /// <param name="source">
        /// Instance with the collection from which <paramref name="target"/>
        /// was removed.
        /// </param>
        /// <param name="propertyName">Property name for collection.</param>
        /// <param name="target">Object which was removed.</param>
        internal void RemovedLink(MaterializerEntry source, string propertyName, object target)
        {
            Debug.Assert(source.Entry != null || source.ForLoadProperty, "source != null || source.ForLoadProperty");
            Debug.Assert(propertyName != null, "propertyName != null");

            if (IsEntity(source) && IsEntity(target, this.clientEdmModel))
            {
                Debug.Assert(this.Tracking, "this.Tracking -- otherwise there's an 'if' missing (it happens to be that the assert holds for all current callers");
                LinkDescriptor item = new LinkDescriptor(source.ResolvedObject, propertyName, target, EntityStates.Detached);
                this.links.Add(item);
            }
        }

        /// <summary>
        /// Invoke this method to notify the log that a link was set on
        /// a property.
        /// </summary>
        /// <param name="source">Entry for source object.</param>
        /// <param name="propertyName">Name of property set.</param>
        /// <param name="target">Target object.</param>
        internal void SetLink(MaterializerEntry source, string propertyName, object target)
        {
            Debug.Assert(source.Entry != null || source.ForLoadProperty, "source != null || source.ForLoadProperty");
            Debug.Assert(propertyName != null, "propertyName != null");

            if (!this.Tracking)
            {
                return;
            }

            if (IsEntity(source) && IsEntity(target, this.clientEdmModel))
            {
                Debug.Assert(this.Tracking, "this.Tracking -- otherwise there's an 'if' missing (it happens to be that the assert holds for all current callers");
                LinkDescriptor item = new LinkDescriptor(source.ResolvedObject, propertyName, target, EntityStates.Modified);
                this.links.Add(item);
            }
        }

        #endregion Internal methods.

        #region Private methods.
        /// <summary>
        /// Returns true the specified entry represents an entity.
        /// </summary>
        /// <param name="entry">The materializer entry</param>
        /// <returns>True if the entry represents an entity.</returns>
        private static bool IsEntity(MaterializerEntry entry)
        {
            Debug.Assert(entry.ActualType != null, "Entry with no type added to log");
            return entry.ActualType.IsEntityType;
        }

        /// <summary>
        /// Returns true the specified entry represents an entity.
        /// </summary>
        /// <param name="entity">The resolved instance</param>
        /// <param name="model">The client model.</param>
        /// <returns>True if the entry represents an entity.</returns>
        private static bool IsEntity(object entity, ClientEdmModel model)
        {
            if (entity == null)
            {
                // you can set link to null, we need to track these values
                return true;
            }

            return ClientTypeUtil.TypeIsEntity(entity.GetType(), model);
        }

        #endregion Private methods.
    }
}
