//---------------------------------------------------------------------
// <copyright file="ODataCollectionItemsObjectModelAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary>
    /// An OData object model annotation for collections to capture all the items in a collection.
    /// </summary>
    public sealed class ODataCollectionItemsObjectModelAnnotation : List<object>
    {
    }
}
