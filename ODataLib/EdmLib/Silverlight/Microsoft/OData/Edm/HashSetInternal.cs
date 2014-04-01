//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
