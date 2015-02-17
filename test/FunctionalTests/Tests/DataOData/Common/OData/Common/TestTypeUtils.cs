//---------------------------------------------------------------------
// <copyright file="TestTypeUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Helper methods for working with types and reflection.
    /// </summary>
    public static class TestTypeUtils
    {
        /// <summary>
        /// Checks whether the specified type is a generic nullable type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if <paramref name="type"/> is nullable; false otherwise.</returns>
        public static bool IsNullableType(Type type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Gets a non-nullable version of the specified type.
        /// </summary>
        /// <param name="type">Type to get non-nullable version for.</param>
        /// <returns>
        /// <paramref name="type"/> if type is a reference type or a 
        /// non-nullable type; otherwise, the underlying value type.
        /// </returns>
        public static Type GetNonNullableType(Type type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        /// <summary>
        /// Checks whether the specified <paramref name='type' /> can be assigned null.
        /// </summary>
        /// <param name='type'>Type to check.</param>
        /// <returns>true if type is a reference type or a Nullable type; false otherwise.</returns>
        public static bool TypeAllowsNull(Type type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            return !type.IsValueType || IsNullableType(type);
        }

        /// <summary>
        /// Gets the elementtype for enumerable
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <returns>If the <paramref name="type"/> was IEnumerable then it returns the type of a single element
        /// otherwise it returns null.</returns>
        public static Type GetIEnumerableElementType(Type type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            Type ienum = FindIEnumerable(type);
            if (ienum == null)
            {
                return null;
            }

            return ienum.GetGenericArguments()[0];
        }

        /// <summary>
        /// Finds type that implements IEnumerable so can get elemtent type
        /// </summary>
        /// <param name="seqType">The Type to check</param>
        /// <returns>returns the type which implements IEnumerable</returns>
        public static Type FindIEnumerable(Type seqType)
        {
            ExceptionUtilities.CheckArgumentNotNull(seqType, "seqType");
            if (seqType == null)
            {
                return null;
            }

            if (seqType.IsArray)
            {
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            }

            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }

            Type[] ifaces = seqType.GetInterfaces();
            foreach (Type iface in ifaces)
            {
                Type ienum = FindIEnumerable(iface);
                if (ienum != null)
                {
                    return ienum;
                }
            }

            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }

            return null;
        }
    }
}
