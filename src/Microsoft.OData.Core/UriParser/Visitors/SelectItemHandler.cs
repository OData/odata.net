//---------------------------------------------------------------------
// <copyright file="SelectItemHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;

    /// <summary>
    /// Handler interface for walking a select item tree.
    /// </summary>
    public abstract class SelectItemHandler
    {
        /// <summary>
        /// Handle a WildcardSelectItem
        /// </summary>
        /// <param name="item">the item to Handle</param>
        public virtual void Handle(WildcardSelectItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle a PathSelectItem
        /// </summary>
        /// <param name="item">the item to Handle</param>
        public virtual void Handle(PathSelectItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle a ContainerQualifiedWildcardSelectItem
        /// </summary>
        /// <param name="item">the item to Handle</param>
        public virtual void Handle(NamespaceQualifiedWildcardSelectItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle an ExpandedNavigationSelectItem
        /// </summary>
        /// <param name="item">the item to Handle</param>
        public virtual void Handle(ExpandedNavigationSelectItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle an ExpandedReferenceSelectItem
        /// </summary>
        /// <param name="item">the item to Handle</param>
        public virtual void Handle(ExpandedReferenceSelectItem item)
        {
            throw new NotImplementedException();
        }
    }
}