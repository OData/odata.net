//---------------------------------------------------------------------
// <copyright file="TestEntityTracker.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using Microsoft.OData.Client;
    using System.Collections.Generic;

    /// <summary>
    /// Test Entity Tracker class
    /// </summary>
    internal class TestEntityTracker : EntityTrackerBase
    {
        internal TestEntityTracker()
        {
            // Default settings
            this.TryGetEntityMethodOutState = EntityStates.Detached;
            this.TryGetEntityObject = s => null;
        }

        internal EntityStates TryGetEntityMethodOutState { get; set; }

        internal Func<Uri, object> TryGetEntityObject { get; set; }

        internal Func<object, string, IEnumerable<LinkDescriptor>> GetLinksFunc { get; set; }

        internal Func<EntityDescriptor, bool, EntityDescriptor> InternalAttachEntityDescriptorFunc { get; set; }

        internal Func<object, EntityDescriptor> GetEntityDescriptorFunc { get; set; }

        internal Action<LinkDescriptor, bool> DetachExistingLinkAction { get; set; }

        internal Action<object, string, object, MergeOption> AttachLinkAction { get; set; }

        internal Action<EntityDescriptor, MergeOption> AttachIdentityAction { get; set; }

        internal override object TryGetEntity(Uri resourceUri, out EntityStates state)
        {
            state = this.TryGetEntityMethodOutState;
            return this.TryGetEntityObject(resourceUri);
        }

        internal override IEnumerable<LinkDescriptor> GetLinks(object source, string sourceProperty)
        {
            return this.GetLinksFunc(source, sourceProperty);
        }

        internal override EntityDescriptor InternalAttachEntityDescriptor(EntityDescriptor entityDescriptorFromMaterializer, bool failIfDuplicated)
        {
            return InternalAttachEntityDescriptorFunc(entityDescriptorFromMaterializer, failIfDuplicated);
        }

        internal override EntityDescriptor GetEntityDescriptor(object resource)
        {
            return GetEntityDescriptorFunc(resource);
        }

        internal override void DetachExistingLink(LinkDescriptor existingLink, bool targetDelete)
        {
            DetachExistingLinkAction(existingLink, targetDelete);
        }

        internal override void AttachLink(object source, string sourceProperty, object target, MergeOption linkMerge)
        {
            AttachLinkAction(source, sourceProperty, target, linkMerge);
        }

        internal override void AttachIdentity(EntityDescriptor entityDescriptorFromMaterializer, MergeOption metadataMergeOption)
        {
            AttachIdentityAction(entityDescriptorFromMaterializer, metadataMergeOption);
        }
    }
}
