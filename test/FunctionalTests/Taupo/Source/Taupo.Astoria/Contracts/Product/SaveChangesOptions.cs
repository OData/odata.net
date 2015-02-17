//---------------------------------------------------------------------
// <copyright file="SaveChangesOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Product
{
    using System;

    /// <summary>
    /// Used in place of Microsoft.OData.Service.SaveChangesOptions to avoid a direct dependency on the product.
    /// Indicates change options when BeginSaveChanges is called. 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames", Justification = "Matching to the enum name in product")]
    public enum SaveChangesOptions
    {
        /// <summary>
        /// All pending changes are saved in a single batch request. 
        /// </summary>
        Batch,

        /// <summary>
        /// Pending changes are saved using multiple requests to the server, and the operation continues after an error occurs.
        /// </summary>
        ContinueOnError,

        /// <summary>
        /// Pending changes are saved using multiple requests to the server, but the operation stops on the first failure (default). 
        /// </summary>
        None,

        /// <summary>
        /// Pending updates are made by updating specified values of the entity in the data source with values from the updated entity. 
        /// </summary>
        PatchOnUpdate,

        /// <summary>
        /// Pending updates are made by replacing all values of the entity in the data source with values from the updated entity. 
        /// </summary>
        ReplaceOnUpdate,

        /// <summary>
        /// Unspecified. This is for test use only.
        /// </summary>
        Unspecified
    }
}
