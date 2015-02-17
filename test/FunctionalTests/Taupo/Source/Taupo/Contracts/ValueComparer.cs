//---------------------------------------------------------------------
// <copyright file="ValueComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Contains value comparer which compares byte arrays based on the content.
    /// </summary>
    public static class ValueComparer
    {
        private static readonly Comparer instance = new Comparer();

        /// <summary>
        /// Gets the sole instance of the value comparer.
        /// </summary>
        /// <value>The value comparer.</value>
        public static IEqualityComparer<object> Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Contains methods to support the comparison of values for equality.
        /// Compares byte arrays based on the content.
        /// </summary>
        private class Comparer : IEqualityComparer<object>
        {
            /// <summary>
            /// Returns a hash code for the specified object.
            /// </summary>
            /// <param name="obj">The value for which a hash code is to be returned.</param>
            /// <returns>
            /// A hash code for the specified object, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public int GetHashCode(object obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                int result = 0;

                var valueAsByte = obj as byte[];
                if (null != valueAsByte)
                {
                    int n = Math.Min(valueAsByte.Length, 7);
                    for (int i = 0; i < n; i++)
                    {
                        result = (result << 5) ^ valueAsByte[i];
                    }
                }
                else
                {
                    result ^= obj.GetHashCode();
                }

                return result;
            }

            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>
            ///     <c>true</c> true if the specified objects are equal; otherwise, <c>false</c>.
            /// </returns>
            bool IEqualityComparer<object>.Equals(object x, object y)
            {
                var value1AsBinary = x as byte[];
                var value2AsBinary = y as byte[];
                bool value1IsBinary = value1AsBinary != null;
                bool value2IsBinary = value2AsBinary != null;

                if (value1IsBinary != value2IsBinary)
                {
                    return false;
                }

                if (value1IsBinary)
                {
                    if (ReferenceEquals(value1AsBinary, value2AsBinary))
                    {
                        return true;
                    }

                    if (value1AsBinary.Length != value2AsBinary.Length)
                    {
                        return false;
                    }

                    for (int i = 0; i < value1AsBinary.Length; i++)
                    {
                        if (value1AsBinary[i] != value2AsBinary[i])
                        {
                            return false;
                        }
                    }

                    return true;
                }

                bool value1IsEmptyData = x == EmptyData.Value;
                bool value2IsEmptyData = y == EmptyData.Value;
                if (value1IsEmptyData && value2IsEmptyData)
                {
                    return true;
                }
                else if (value1IsEmptyData)
                {
                    return IsEmptyEnumerable(y);
                }
                else if (value2IsEmptyData)
                {
                    return IsEmptyEnumerable(x);
                }

                return Equals(x, y);
            }

            private static bool IsEmptyEnumerable(object o)
            {
                var enumerable = o as IEnumerable;
                if (enumerable == null)
                {
                    return false;
                }

                return !enumerable.Cast<object>().Any();
            }
        }
    }
}