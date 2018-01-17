//---------------------------------------------------------------------
// <copyright file="ReadOnlyEnumerable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Collections;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Implementation of IEnumerable which is based on another IEnumerable
    /// but only exposes readonly access to that collection. This class doesn't implement
    /// any other public interfaces or public API unlike most other IEnumerable implementations
    /// which also implement other public interfaces.
    /// </summary>
    internal class ReadOnlyEnumerable : IEnumerable
    {
        /// <summary>
        /// The IEnumerable to wrap.
        /// </summary>
        private readonly IEnumerable sourceEnumerable;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceEnumerable">The enumerable to wrap.</param>
        internal ReadOnlyEnumerable(IEnumerable sourceEnumerable)
        {
            Debug.Assert(sourceEnumerable != null, "sourceEnumerable != null");

            this.sourceEnumerable = sourceEnumerable;
        }

        /// <summary>
        /// Returns the enumerator to iterate through the items.
        /// </summary>
        /// <returns>The enumerator object to use.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.sourceEnumerable.GetEnumerator();
        }
    }
}
