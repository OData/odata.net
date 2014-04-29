//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
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
