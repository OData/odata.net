//---------------------------------------------------------------------
// <copyright file="EntityTrackerBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;


    #endregion Namespaces

    /// <summary>
    /// Entity Tracker base, allows more decoupling for testing.
    /// </summary>
    public abstract class EntityTrackerBase
    {
        /// <summary>
        /// Find tracked entity by its resourceUri and update its etag.
        /// </summary>
        /// <param name="resourceUri">resource id</param>
        /// <param name="state">state of entity</param>
        /// <returns>entity if found else null</returns>
        internal abstract object TryGetEntity(Uri resourceUri, out EntityStates state);

        /// <summary>
        /// get the related links ignoring target entity
        /// </summary>
        /// <param name="source">source entity</param>
        /// <param name="sourceProperty">source entity's property</param>
        /// <returns>enumerable of related ends</returns>
        internal abstract IEnumerable<LinkDescriptor> GetLinks(object source, string sourceProperty);

        /// <summary>
        /// Attach entity into the context in the Unchanged state.
        /// </summary>
        /// <param name="entityDescriptorFromMaterializer">entity descriptor from the response</param>
        /// <param name="failIfDuplicated">fail for public api else change existing relationship to unchanged</param>
        /// <remarks>Caller should validate descriptor instance.</remarks>
        /// <returns>The attached descriptor, if one already exists in the context and failIfDuplicated is set to false, then the existing instance is returned</returns>
        /// <exception cref="InvalidOperationException">if entity is already being tracked by the context</exception>
        /// <exception cref="InvalidOperationException">if identity is pointing to another entity</exception>
        internal abstract EntityDescriptor InternalAttachEntityDescriptor(EntityDescriptor entityDescriptorFromMaterializer, bool failIfDuplicated);

        /// <summary>
        /// verify the resource being tracked by context
        /// </summary>
        /// <param name="resource">resource</param>
        /// <returns>The given resource.</returns>
        /// <exception cref="InvalidOperationException">if resource is not contained</exception>
        internal abstract EntityDescriptor GetEntityDescriptor(object resource);

        /// <summary>Detach existing link</summary>
        /// <param name="existingLink">link to detach</param>
        /// <param name="targetDelete">true if target is being deleted, false otherwise</param>
        internal abstract void DetachExistingLink(LinkDescriptor existingLink, bool targetDelete);

        /// <summary>
        /// attach the link with the given source, sourceProperty and target.
        /// </summary>
        /// <param name="source">source entity of the link.</param>
        /// <param name="sourceProperty">name of the property on the source entity.</param>
        /// <param name="target">target entity of the link.</param>
        /// <param name="linkMerge">merge option to be used to merge the link if there is an existing link.</param>
        internal abstract void AttachLink(object source, string sourceProperty, object target, MergeOption linkMerge);

        /// <summary>response materialization has an identity to attach to the inserted object</summary>
        /// <param name="entityDescriptorFromMaterializer">entity descriptor containing all the information about the entity from the response.</param>
        /// <param name="metadataMergeOption">mergeOption based on which EntityDescriptor will be merged.</param>
        internal abstract void AttachIdentity(EntityDescriptor entityDescriptorFromMaterializer, MergeOption metadataMergeOption);
    }
}
