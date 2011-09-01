//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Data.Edm.Internal
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

        public void Clear()
        {
            this.wrappedDictionary.Clear();
        }

        public void Remove(T item)
        {
            this.wrappedDictionary.Remove(item);
        }
    }
}
