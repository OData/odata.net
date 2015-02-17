//---------------------------------------------------------------------
// <copyright file="ODataBatchOperation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Batch
{
    #region Namespaces
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary>
    /// Helper class to represent ODataBatch operations as they are read by ODataLib.
    /// </summary>
    public abstract class ODataBatchOperation : ODataBatchPart
    {
        /// <summary>
        /// The headers of the batch request or response operation.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get;
            set;
        }

        /// <summary>
        /// The payload of a response operation.
        /// </summary>
        public object Payload
        {
            get;
            set;
        }
    }
}
