//---------------------------------------------------------------------
// <copyright file="WrappedReadOnlyCollection`1.Extensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Wrappers
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;

    /// <content>
    /// Hand-written extensions for the generated <see cref="T:WrappedReadOnlyCollection"/> provided for better usability.
    /// </content>
    public partial class WrappedReadOnlyCollection<T> : IEnumerable<T>
    {
        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator instance.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            IEnumerable enumerable = this.Product as IEnumerable;
            ExceptionUtilities.CheckObjectNotNull(enumerable, "Product enumerable is null");
            return enumerable.GetEnumerator();
        }
    }
}
