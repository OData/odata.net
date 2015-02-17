//---------------------------------------------------------------------
// <copyright file="ODataBatchResponseOperation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Batch
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Helper class to represent OData batch response operations as they are read by ODataLib.
    /// </summary>
    public sealed class ODataBatchResponseOperation : ODataBatchOperation
    {
        /// <summary>
        /// The HTTP status code of the response
        /// </summary>
        public int StatusCode
        {
            get;
            set;
        }
    }
}
