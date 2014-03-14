//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#if ODATALIB_QUERY
namespace Microsoft.Data.Experimental.OData
#else
namespace Microsoft.Data.OData
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
            DebugUtils.CheckNoExternalCallers();

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
            DebugUtils.CheckNoExternalCallers();

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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();

            if (typeA == null || typeB == null)
            {
                return false;
            }
            else
            {
#if SILVERLIGHT || WINDOWS_PHONE || ORCAS || PORTABLELIB
                return typeA == typeB;
#else
                return typeA.IsEquivalentTo(typeB);
#endif
            }
        }
    }
}
