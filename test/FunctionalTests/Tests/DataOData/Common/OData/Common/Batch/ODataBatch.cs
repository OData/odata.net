//---------------------------------------------------------------------
// <copyright file="ODataBatch.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Batch
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData;
    #endregion Namespaces

    /// <summary>
    /// Helper class to represent ODataBatch payloads as they are read by ODataLib.
    /// </summary>
    public sealed class ODataBatch : ODataAnnotatable
    {
        /// <summary>
        /// All the parts of this batch payload.
        /// </summary>
        public IEnumerable<ODataBatchPart> Parts
        {
            get;
            set;
        }
    }
}
