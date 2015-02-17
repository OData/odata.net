//---------------------------------------------------------------------
// <copyright file="TypeUtilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Objects
{
    using System;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Common Type Utilities
    /// </summary>
    public static class TypeUtilities
    {
        private static Type[] primitiveTypes = new Type[] 
        {
                typeof(bool),
                typeof(string),
                typeof(byte[]),
                typeof(Guid),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(byte), 
                typeof(sbyte),
                typeof(short), 
                typeof(ushort), 
                typeof(int), 
                typeof(uint), 
                typeof(long), 
                typeof(ulong), 
                typeof(float), 
                typeof(double), 
                typeof(decimal)
        };

        /// <summary>
        /// Determines whether the specified type is primitive
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>
        /// <c>true</c> if the specified type is primitive; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPrimitiveType(Type type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            if (type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
            }

            return primitiveTypes.Contains(type) || type.FullName.StartsWith("Microsoft.Spatial", StringComparison.Ordinal);
        }
    }
}