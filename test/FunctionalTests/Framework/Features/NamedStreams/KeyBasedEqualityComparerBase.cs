//---------------------------------------------------------------------
// <copyright file="KeyBasedEqualityComparerBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Equality comparer that uses key-property comparison
    /// </summary>
    public abstract class KeyBasedEqualityComparerBase : EqualityComparer<object>
    {
        /// <summary>
        /// returns if the two objects have the same key values
        /// </summary>
        /// <param name="x">first object to compare</param>
        /// <param name="y">second object to compare</param>
        /// <returns>true if equal, false if not</returns>
        public override bool Equals(object x, object y)
        {
            var aKey = this.GetKeys(x);
            var bKey = this.GetKeys(y);
            return this.CompareKeys(aKey, bKey);
        }

        /// <summary>
        /// returns the hashcode of the object
        /// </summary>
        /// <param name="obj">object to get hashcode</param>
        /// <returns>hashcode of object</returns>
        public override int GetHashCode(object obj)
        {
            var key = this.GetKeys(obj);
            var hashKey = string.Join(",", key.Select(pair => string.Format("{0}={1}", pair.Key, pair.Value, CultureInfo.InvariantCulture)).ToArray());
            return hashKey.GetHashCode();
        }

        /// <summary>
        /// Gets the names of the key properties for the given type
        /// </summary>
        /// <param name="entityType">The type to get keys for</param>
        /// <returns></returns>
        protected abstract string[] GetKeyPropertyNames(Type entityType);

        private IDictionary<string, object> GetKeys(object obj)
        {
            var type = obj.GetType();
            string[] keyPropertyNames = this.GetKeyPropertyNames(type);
            return keyPropertyNames.ToDictionary(k => k, k => type.GetProperty(k).GetValue(obj, null));
        }

        private bool CompareKeys(IDictionary<string, object> aKeys, IDictionary<string, object> bKeys)
        {
            return aKeys.OrderBy(k => k.Key).Select(k => k.Value).SequenceEqual(bKeys.OrderBy(k => k.Key).Select(k => k.Value));
        }
    }
}