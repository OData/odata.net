//---------------------------------------------------------------------
// <copyright file="ReferenceEqualityComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Common
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Equality comparer that uses reference equality
    /// </summary>
    public class ReferenceEqualityComparer : EqualityComparer<object>
    {
        /// <summary>
        /// returns if the two objects are the same instance
        /// </summary>
        /// <param name="x">first object to compare</param>
        /// <param name="y">second object to compare</param>
        /// <returns>true if equal, false if not</returns>
        public override bool Equals(object x, object y)
        {
            return object.ReferenceEquals(x, y);
        }

        /// <summary>
        /// returns the hashcode of the object
        /// </summary>
        /// <param name="obj">object to get hashcode</param>
        /// <returns>hashcode of object</returns>
        public override int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}