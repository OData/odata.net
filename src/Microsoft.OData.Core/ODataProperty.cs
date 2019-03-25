//---------------------------------------------------------------------
// <copyright file="ODataProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Represents a single property of a resource with a value.
    /// </summary>
    public sealed class ODataProperty : ODataPropertyInfo
    {
        /// <summary>Gets or sets the property value.</summary>
        /// <returns>The property value.</returns>
        public object Value
        {
            get
            {
                if (this.ODataValue == null)
                {
                    return null;
                }

                return this.ODataValue.FromODataValue();
            }

            set
            {
                this.ODataValue = value.ToODataValue();
            }
        }

        /// <summary>
        /// Property value, represented as an ODataValue.
        /// </summary>
        /// <remarks>
        /// This value is the same as <see cref="Value"/>, except that primitive types are wrapped
        /// in an instance of ODataPrimitiveValue, and null values are represented by an instance of ODataNullValue.
        /// </remarks>
        internal ODataValue ODataValue
        {
            get; private set;
        }
    }
}
