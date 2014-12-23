//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#if ODATALIB_QUERY
namespace Microsoft.OData.Query
#else
namespace Microsoft.OData.Core
#endif
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Utility methods for working with CLR types.
    /// </summary>
    internal static class TypeUtils
    {
        /// <summary>Checks whether the specified type is a generic nullable type.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if <paramref name="type"/> is nullable; false otherwise.</returns>
        internal static bool IsNullableType(Type type)
        {
            //// This is a copy of WebUtil.IsNullableType from the product.

            return type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>Gets a non-nullable version of the specified type.</summary>
        /// <param name="type">Type to get non-nullable version for.</param>
        /// <returns>
        /// <paramref name="type"/> if type is a reference type or a 
        /// non-nullable type; otherwise, the underlying value type.
        /// </returns>
        internal static Type GetNonNullableType(Type type)
        {
            //// This is a copy of RequestQueryParser.GetNonNullableType from the product.

            return Nullable.GetUnderlyingType(type) ?? type;
        }

        /// <summary>
        /// Checks whether the specified <paramref name="type"/> can be assigned null. If it is a non-nullable
        /// value type it creates the corresonding nullable type and returns it.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>The <paramref name="type"/> if it allows null or the corresponding nullable type.</returns>
        internal static Type GetNullableType(Type type)
        {
            Debug.Assert(type != null, "type != null");

            if (!TypeAllowsNull(type))
            {
                type = typeof(Nullable<>).MakeGenericType(type);
            }

            return type;
        }

        /// <summary>Checks whether the specified <paramref name='type' /> can be assigned null.</summary>
        /// <param name='type'>Type to check.</param>
        /// <returns>true if type is a reference type or a Nullable type; false otherwise.</returns>
        internal static bool TypeAllowsNull(Type type)
        {
            Debug.Assert(type != null, "type != null");

            //// This is a copy of WebUtil.TypeAllowsNull from the product.

            return !type.IsValueType() || IsNullableType(type);
        }

        /// <summary>
        /// Determines if two CLR types are equivalent.
        /// </summary>
        /// <param name="typeA">First type to compare.</param>
        /// <param name="typeB">Second type to compare.</param>
        /// <returns>true if the types are equivalent (they both represent the same type), or false otherwise.</returns>
        /// <remarks>This method abstracts away the necessity to call Type.IsEquivalentTo method in .NET 4 and higher but 
        /// use simple reference equality on platforms which don't have that method (like Silverlight).</remarks>
        internal static bool AreTypesEquivalent(Type typeA, Type typeB)
        {
            if (typeA == null || typeB == null)
            {
                return false;
            }
            else
            {
                return typeA == typeB;
            }
        }
    }
}
