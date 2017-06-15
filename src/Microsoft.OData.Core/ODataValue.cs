//---------------------------------------------------------------------
// <copyright file="ODataValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Represents the value of a property.
    /// </summary>
    public abstract class ODataValue : ODataItem
    {
        /// <summary>
        /// Indicates whether the given value is a null value.
        /// </summary>
        /// <value> true if the value is an ODataNullValue, false otherwise. </value>
        internal virtual bool IsNullValue
        {
            get
            {
                return false;
            }
        }
    }
}