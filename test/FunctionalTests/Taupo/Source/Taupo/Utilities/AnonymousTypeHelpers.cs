//---------------------------------------------------------------------
// <copyright file="AnonymousTypeHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Helper class that handles logic for anonymous types. 
    /// </summary>
    public static class AnonymousTypeHelpers
    {
        private static MethodInfo areEqualMethod = typeof(AnonymousTypeHelpers)
            .GetMethod("AreEqual", true, true);

        /// <summary>
        /// Compares two anonymous types.
        /// </summary>
        /// <param name="anonymous1">First anonymous type.</param>
        /// <param name="anonymous2">Second anonymous type.</param>
        /// <returns>True if both types are equal, false otherwise.</returns>
        public static bool EqualsHelper(object anonymous1, object anonymous2)
        {
            if (anonymous2 == null)
            {
                return false;
            }

            foreach (PropertyInfo pi in anonymous1.GetType().GetProperties())
            {
                object v1 = pi.GetValue(anonymous1, null);
                object v2 = pi.GetValue(anonymous2, null);

                if (false.Equals(areEqualMethod.MakeGenericMethod(pi.PropertyType).Invoke(null, new object[] { v1, v2 })))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether two values are equal.
        /// </summary>
        /// <typeparam name="T">Type of values.</typeparam>
        /// <param name="firstValue">First value.</param>
        /// <param name="secondValue">Second value.</param>
        /// <returns>True if values are equal, false otherwise.</returns>
        public static bool AreEqual<T>(T firstValue, T secondValue)
        {
            return EqualityComparer<T>.Default.Equals(firstValue, secondValue);
        }
    }
}