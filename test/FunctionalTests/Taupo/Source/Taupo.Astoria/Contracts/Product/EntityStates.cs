//---------------------------------------------------------------------
// <copyright file="EntityStates.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Product
{
    using System;

    /// <summary>
    /// Used in place of Microsoft.OData.Client.EntityStates to avoid a direct dependency on the product.
    /// Represents the enumeration that identifies the state of an entity being tracked by the DataServiceContext. 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames", Justification = "Matching to the enum name in product")]
    public enum EntityStates
    {
        /// <summary>
        /// The entity was added since the last call to BeginSaveChanges. 
        /// </summary>
        Added,

        /// <summary>
        /// The entity was deleted since the last call to BeginSaveChanges. 
        /// </summary>
        Deleted,

        /// <summary>
        /// The entity was detached since the last call to BeginSaveChanges. 
        /// </summary>
        Detached,

        /// <summary>
        /// The entity was modified since the last call to BeginSaveChanges. 
        /// </summary>
        Modified,

        /// <summary>
        /// The entity is unchanged since the last call to BeginSaveChanges. 
        /// </summary>
        Unchanged
    }
}
