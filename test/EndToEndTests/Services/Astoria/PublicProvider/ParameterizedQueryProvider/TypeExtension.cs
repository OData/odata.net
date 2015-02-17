//---------------------------------------------------------------------
// <copyright file="TypeExtension.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PublicProvider
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extension method for type
    /// </summary>
    static class TypeExtension
    {
        /// <summary>
        /// Get query element type
        /// </summary>
        /// <param name="type">The type of the query</param>
        /// <returns>The element type</returns>
        public static Type GetQueryElementType(this Type type)
        {
            Type ienum = FindIEnumerable(type);
            if (ienum == null)
            {
                //If type is not IEnumerable<T>, then return type
                return type;
            }

            //If type is IEnumerable<T>, then return T
            return ienum.GetGenericArguments()[0];
        }

        /// <summary>
        /// Find the element type if seqType is IEnumerable<T>
        /// </summary>
        /// <param name="seqType">The type</param>
        /// <returns>T if seqType is IEnumerable<T></returns>
        static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
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
            if (ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null)
                    {
                        return ienum;
                    }
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
