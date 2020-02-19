//---------------------------------------------------------------------
// <copyright file="IPrimitiveValueConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Class for defining a primitive value conversion for a type definition.
    /// Suppose a type definition defines a primitive type X (underlying type) as a new type Y,
    /// and the type Y has a logically corresponding CLR type Z,
    /// the ConvertToUnderlyingType method converts value from Z to X
    /// and the ConvertFromUnderlyingType method converts value from X to Z.
    /// </summary>
    public interface IPrimitiveValueConverter
    {
        /// <summary>
        /// Converts the given primitive value from the CLR type to the underlying type defined in a type definition.
        /// </summary>
        /// <param name="value">The given CLR value.</param>
        /// <returns>The converted value of the underlying type.</returns>
        object ConvertToUnderlyingType(object value);

        /// <summary>
        /// Converts the given primitive value from the underlying type to the CLR type defined in a type definition.
        /// </summary>
        /// <param name="value">The given value of the CLR type.</param>
        /// <returns>The converted CLR value.</returns>
        object ConvertFromUnderlyingType(object value);
    }
}
