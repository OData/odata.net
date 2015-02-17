//---------------------------------------------------------------------
// <copyright file="ApplyDefaultNamespaceFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Applies default namespace to all nominal types that don't explicitly define it.
    /// </summary>
    public class ApplyDefaultNamespaceFixup : NamedItemFixupBase
    {
        /// <summary>
        /// Initializes a new instance of the ApplyDefaultNamespaceFixup class with given default namespace name.
        /// </summary>
        /// <param name="defaultNamespaceName">Default namespace name</param>
        public ApplyDefaultNamespaceFixup(string defaultNamespaceName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(defaultNamespaceName, "defaultNamespaceName");
            this.DefaultNamespaceName = defaultNamespaceName;
        }

        /// <summary>
        /// Gets or sets default namespace to apply.
        /// </summary>
        public string DefaultNamespaceName { get; set; }

        /// <summary>
        /// Only override the item's Namespace if it is null
        /// </summary>
        /// <param name="item">The item to be fixed up</param>        
        protected override void FixupNamedItem(INamedItem item)
        {
            if (item.NamespaceName == null)
            {
                item.NamespaceName = this.DefaultNamespaceName;
            }
        }

        /// <summary>
        /// Check if the item that has Name and Namespace is valid
        /// </summary>
        /// <param name="item">The item to be checked</param>
        /// <returns>true if the item is valid, false otherwise.</returns>        
        protected override bool IsNamedItemValid(INamedItem item)
        {
            return item.NamespaceName != null;
        }
    }
}
