//---------------------------------------------------------------------
// <copyright file="OverrideNamespaceFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Overrides the namespace of all nominal types to a new one.
    /// </summary>
    public class OverrideNamespaceFixup : NamedItemFixupBase
    {
        /// <summary>
        /// Initializes a new instance of the OverrideNamespaceFixup class with the given namespace name.
        /// </summary>
        /// <param name="newNamespaceName">New namespace name to be applied</param>
        public OverrideNamespaceFixup(string newNamespaceName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(newNamespaceName, "newNamespaceName");
            this.NewNamespaceName = newNamespaceName;
        }

        /// <summary>
        /// Gets or sets the new namespace to apply.
        /// </summary>
        public string NewNamespaceName { get; set; }

        /// <summary>
        /// Override the item's Namespace no matter what
        /// </summary>
        /// <param name="item">The item to be fixed up</param>        
        protected override void FixupNamedItem(INamedItem item)
        {
            item.NamespaceName = this.NewNamespaceName;
        }

        /// <summary>
        /// Check if the item that has Name and Namespace is valid
        /// </summary>
        /// <param name="item">The item to be checked</param>
        /// <returns>true if the item is valid, false otherwise.</returns>        
        protected override bool IsNamedItemValid(INamedItem item)
        {
            return item.NamespaceName == this.NewNamespaceName;
        }
    }
}
