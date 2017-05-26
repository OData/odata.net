//---------------------------------------------------------------------
// <copyright file="ODataBatchPart.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Batch
{
    #region Namespaces
    using Microsoft.OData;
    #endregion Namespaces

    /// <summary>
    /// Helper class to represent ODataBatch parts as they are read by ODataLib.
    /// </summary>
    public abstract class ODataBatchPart : ODataAnnotatable
    {
    }
}
