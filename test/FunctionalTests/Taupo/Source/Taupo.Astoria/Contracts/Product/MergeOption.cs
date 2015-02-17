//---------------------------------------------------------------------
// <copyright file="MergeOption.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Product
{
    /// <summary>
    /// Used in place of Microsoft.OData.Client.MergeOption to avoid a direct dependency on the product.
    /// Determines the synchronization option for sending or receiving entities to or from a data service. 
    /// </summary>
    public enum MergeOption
    {
        /// <summary>
        /// Append new entities only. Existing entities or their original values will not be modified. No client-side changes are lost in this merge. This is the default behavior. 
        /// </summary>
        AppendOnly,

        /// <summary>
        /// All current values on the client are overwritten with current values from the data service regardless of whether they have been changed on the client. 
        /// </summary>
        OverwriteChanges,

        /// <summary>
        /// Current values that have been changed on the client are not modified, but any unchanged values are updated with current values from the data service. No client-side changes are lost in this merge. 
        /// </summary>
        PreserveChanges,

        /// <summary>
        /// Objects are always loaded from the data source. Any property changes made to objects in the object context are overwritten by the data source values. 
        /// </summary>
        NoTracking
    }
}
