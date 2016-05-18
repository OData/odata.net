//---------------------------------------------------------------------
// <copyright file="ReferenceEqualityComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary>
    /// Use this class to compare objects by reference in collections such as
    /// dictionary or hashsets.
    /// </summary>
    /// <typeparam name="T">Type of objects to compare.</typeparam>
    /// <remarks>
    /// Typically accessed statically as eg
    /// ReferenceEqualityComparer&lt;Expression&gt;.Instance.
    /// </remarks>
    internal sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
    {
        #region Private fields
        /// <summary>
        /// Single instance per 'T' for comparison.
        /// </summary>
        private static ReferenceEqualityComparer<T> instance;
        #endregion Private fields

        #region Constructors
        /// <summary>
        /// Initializes a new ReferenceEqualityComparer instance.
        /// </summary>
        private ReferenceEqualityComparer()
        {
        }
        #endregion Constructors

        #region Properties.
        /// <summary>
        /// Returns a singleton instance for this comparer type.
        /// </summary>
        internal static ReferenceEqualityComparer<T> Instance
        {
            get
            {
                if (instance == null)
                {
                    ReferenceEqualityComparer<T> newInstance = new ReferenceEqualityComparer<T>();
                    System.Threading.Interlocked.CompareExchange(ref instance, newInstance, null);
                }

                return instance;
            }
        }
        #endregion Properties.

        #region Methods.
        /// <summary>
        /// Determines whether two objects are the same.
        /// </summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>true if both are the same; false otherwise.</returns>
        public bool Equals(T x, T y)
        {
            return object.ReferenceEquals(x, y);
        }

        /// <summary>
        /// Serves as hashing function for collections.
        /// </summary>
        /// <param name="obj">Object to hash.</param>
        /// <returns>
        /// Hash code for the object; shouldn't change through the lifetime
        /// of <paramref name="obj"/>.
        /// </returns>
        public int GetHashCode(T obj)
        {
            return obj == null ? 0 : obj.GetHashCode();
        }

        #endregion Methods.
    }
}
