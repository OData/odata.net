//---------------------------------------------------------------------
// <copyright file="HashSetInternal.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    internal class HashSetInternal<T> : IEnumerable<T>
    {
        private readonly Dictionary<T, object> wrappedDictionary;

        public HashSetInternal()
        {
            this.wrappedDictionary = new Dictionary<T, object>();
        }

        public bool Add(T thingToAdd)
        {
            if (this.wrappedDictionary.ContainsKey(thingToAdd))
            {
                return false;
            }

            this.wrappedDictionary[thingToAdd] = null;
            return true;
        }

        public bool Contains(T item)
        {
            return this.wrappedDictionary.ContainsKey(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.wrappedDictionary.Keys.GetEnumerator();
        }

        public void Remove(T item)
        {
            this.wrappedDictionary.Remove(item);
        }
    }
}
