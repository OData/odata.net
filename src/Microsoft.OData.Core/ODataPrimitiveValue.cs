//---------------------------------------------------------------------
// <copyright file="ODataPrimitiveValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using Microsoft.OData.Metadata;

    /// <summary>
    /// Represents a primitive property value.
    /// </summary>
    public sealed class ODataPrimitiveValue : ODataValue
    {
        /// <summary>
        /// Creates a new primitive value from the given CLR value.
        /// </summary>
        /// <param name="value">The primitive to wrap.</param>
        /// <remarks>The primitive value should not be an instance of ODataValue.</remarks>
        public ODataPrimitiveValue(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(Strings.ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromNull, (Exception)null);
            }

            if (!EdmLibraryExtensions.IsPrimitiveType(value.GetType()))
            {
                throw new ODataException(Strings.ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromUnsupportedValueType(value.GetType()));
            }

            this.Value = value;
        }

        /// <summary>
        /// Gets the underlying CLR object wrapped by this <see cref="ODataPrimitiveValue"/>.
        /// </summary>
        /// <value> The underlying primitive CLR value. </value>
        public object Value { get; private set; }
    }
}