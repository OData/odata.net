//---------------------------------------------------------------------
// <copyright file="WrappedIEnumerator.Extensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Wrappers
{
    using System.Collections;

    /// <content>
    /// Hand-written extension methods for the generated <see cref="WrappedIEnumerator"/> provided for better usability.
    /// </content>
    public partial class WrappedIEnumerator : IEnumerator
    {
        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        /// <value></value>
        /// <returns>The current element in the collection.</returns>
        object IEnumerator.Current
        {
            get { return this.Current; }
        }
    }
}
