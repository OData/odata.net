//Copyright 2010 Microsoft Corporation
//
//Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 
//
//http://www.apache.org/licenses/LICENSE-2.0 
//
//Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and limitations under the License.


namespace System.Data.Services.Client
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;

    #endregion Namespaces.

    internal class HashSet<T> : Dictionary<T, T>, IEnumerable<T> where T : class
    {
        public HashSet() { }

        public HashSet(IEqualityComparer<T> comparer) : base(comparer) { }

        public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
            : base(comparer)
        {
            this.UnionWith(collection);
        }

        public bool Add(T value) { if (!base.ContainsKey(value)) { base.Add(value, value); return true; } return false; }

        public bool Contains(T value) { return base.ContainsKey(value); }

        new public bool Remove(T value) { return base.Remove(value); }

        new public IEnumerator<T> GetEnumerator() { return base.Keys.GetEnumerator(); }

        public void UnionWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            foreach (T local in other)
            {
                this.Add(local);
            }
        }
    }
}