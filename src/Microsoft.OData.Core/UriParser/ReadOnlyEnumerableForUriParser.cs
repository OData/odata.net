//---------------------------------------------------------------------
// <copyright file="ReadOnlyEnumerableForUriParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    #region Namespaces
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    #endregion Namespaces

    /// <summary>
    /// Implementation of IEnumerable which is based on another IEnumerable
    /// but only exposes readonly access to that collection. This class doesn't implement
    /// any other public interfaces or public API unlike most other IEnumerable implementations
    /// which also implement other public interfaces.
    /// </summary>
    /// <typeparam name="T">The type of the items in the read-only enumerable.</typeparam>
    internal sealed class ReadOnlyEnumerableForUriParser<T> : IEnumerable<T>
    {
        /// <summary>
        /// The IEnumerable to wrap.
        /// </summary>
        private IEnumerable<T> sourceEnumerable;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceEnumerable">The enumerable to wrap.</param>
        internal ReadOnlyEnumerableForUriParser(IEnumerable<T> sourceEnumerable)
        {
            Debug.Assert(sourceEnumerable != null, "sourceEnumerable != null");

            this.sourceEnumerable = sourceEnumerable;
        }

        /// <summary>
        /// Returns the enumerator to iterate through the items.
        /// </summary>
        /// <returns>The enumerator object to use.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.sourceEnumerable.GetEnumerator();
        }

        /// <summary>
        /// Returns the (non-generic) enumerator to iterate through the items.
        /// </summary>
        /// <returns>The enumerator object to use.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.sourceEnumerable.GetEnumerator();
        }
    }
}