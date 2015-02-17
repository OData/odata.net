//---------------------------------------------------------------------
// <copyright file="ReferenceEqualityComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Static helper class for creating strongly-typed reference-equality based equality comparers
    /// </summary>
    public static class ReferenceEqualityComparer
    {
        /// <summary>
        /// Creates a delegate based equality comparer for the given generic argument using reference equality and object's default GetHashCode implementation
        /// </summary>
        /// <typeparam name="TObject">The generic type for the equlity comparer</typeparam>
        /// <returns>An delegate based equality comparer that uses reference equality</returns>
        public static IEqualityComparer<TObject> Create<TObject>()
        {
            return new DelegateBasedEqualityComparer<TObject>((x, y) => object.ReferenceEquals(x, y));
        }
    }
}