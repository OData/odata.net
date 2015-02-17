//---------------------------------------------------------------------
// <copyright file="ODataBatchRequestOperation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Batch
{
    #region Namespaces
    using System;
    #endregion Namespaces

    /// <summary>
    /// Helper class to represent OData batch request operations as they are read by ODataLib.
    /// </summary>
    public sealed class ODataBatchRequestOperation : ODataBatchOperation
    {
        /// <summary>
        /// The HTTP method used in the request operation.
        /// </summary>
        public string HttpMethod
        {
            get;
            set;
        }

        /// <summary>
        /// The URI for the request.
        /// </summary>
        public Uri Url
        {
            get;
            set;
        }
    }
}
