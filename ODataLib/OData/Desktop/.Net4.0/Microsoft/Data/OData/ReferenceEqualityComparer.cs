//   OData .NET Libraries ver. 5.6.3
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
namespace Microsoft.Data.Experimental.OData
#else
namespace Microsoft.Data.OData
#endif
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
                DebugUtils.CheckNoExternalCallers();

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
