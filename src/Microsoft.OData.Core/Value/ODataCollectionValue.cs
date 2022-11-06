//---------------------------------------------------------------------
// <copyright file="ODataCollectionValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Collections;
    using System.Collections.Generic;

    #endregion Namespaces

    /// <summary>
    /// OData representation of a Collection.
    /// </summary>
    public sealed class ODataCollectionValue : ODataValue
    {
        /// <summary>Gets or sets the type of the collection value.</summary>
        /// <returns>The type of the collection value.</returns>
        public string TypeName
        {
            get;
            set;
        }

        /// <summary>Gets or sets the items in the bag value.</summary>
        /// <returns>The items in the bag value.</returns>
        public IEnumerable<object> Items
        {
            get;
            set;
        }
    }
}
