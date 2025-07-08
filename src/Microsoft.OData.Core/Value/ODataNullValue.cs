//---------------------------------------------------------------------
// <copyright file="ODataNullValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Represents a null property value.
    /// </summary>
    public sealed class ODataNullValue : ODataValue
    {
        /// <summary>
        /// Returns a singleton instance of <see cref="ODataNullValue"/>.
        /// </summary>
        public static readonly ODataNullValue Instance = new ODataNullValue();

        /// <summary>
        /// Indicates whether the given value is a null value.
        /// </summary>
        /// <value> true, since this object always represents a null value. </value>
        internal override bool IsNullValue
        {
            get
            {
                return true;
            }
        }
    }
}