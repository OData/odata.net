//---------------------------------------------------------------------
// <copyright file="ODataFeedEntriesObjectModelAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData;
    #endregion Namespaces

    /// <summary>
    /// An OData object model annotation for feeds to capture all the entries of the feed.
    /// </summary>
    public sealed class ODataFeedEntriesObjectModelAnnotation : List<ODataResource>
    {
    }
}
