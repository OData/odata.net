//---------------------------------------------------------------------
// <copyright file="ODataBatchChangeset.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Batch
{
    #region Namespaces
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary>
    /// Helper class to represent ODataBatch changesets as they are read by ODataLib.
    /// </summary>
    public sealed class ODataBatchChangeset : ODataBatchPart
    {
        /// <summary>
        /// The operations of the changeset.
        /// </summary>
        public IEnumerable<ODataBatchOperation> Operations
        {
            get;
            set;
        }
    }
}
