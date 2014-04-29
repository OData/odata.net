//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Http
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>Use this class to implement a NameValueCollection over a dictionary.</summary>
    internal class NameValueFromDictionary : Dictionary<string, List<string>>
    {
        /// <summary>Initializes a new NameValueFromDictionary instance.</summary>
        /// <param name="capacity">Initial capacity.</param>
        /// <param name="comparer">Equality comparer for keys.</param>
        public NameValueFromDictionary(int capacity, IEqualityComparer<string> comparer) : base(capacity, comparer)
        {
            Debug.Assert(comparer != null, "comparer != null");
        }

        /// <summary>Adds a new value for the specified key.</summary>
        /// <param name="key">The key to add a value to.</param>
        /// <param name="value">The value to add to the key.</param>
        public void Add(string key, string value)
        {
            Debug.Assert(key != null, "key != null");
            Debug.Assert(value != null, "value != null");

            List<string> valueArray;
            if (this.ContainsKey(key))
            {
                valueArray = base[key];
            }
            else
            {
                valueArray = new List<string>();
            }

            valueArray.Add(value);
            this[key] = valueArray;
        }

        /// <summary>Gets the comma-separated list of values for the specified key name.</summary>
        /// <param name="name">Name to get values for.</param>
        /// <returns>
        /// The comma-separated list of values for the specified <paramref name="name"/>, 
        /// null if empty or not found.
        /// </returns>
        /// <remarks>
        /// This could be improved by pre-computed the length and building into a StringBuilder,
        /// but we expect most headers to have a single value.
        /// </remarks>
        public string Get(string name)
        {
            Debug.Assert(name != null, "name != null");
            string retString = null;
            if (this.ContainsKey(name))
            {
                List<string> valueArray = base[name];
                for (int i = 0; i < valueArray.Count; i++)
                {
                    if (i == 0)
                    {
                        retString = valueArray[i];
                    }
                    else
                    {
                        retString = retString + valueArray[i];
                    }

                    if (i != (valueArray.Count - 1))
                    {
                        retString = retString + ",";
                    }
                }
            }

            return retString;
        }

        /// <summary>Sets the value of the specified name to a single value.</summary>
        /// <param name="key">Header name.</param>
        /// <param name="value">Value.</param>
        public void Set(string key, string value)
        {
            Debug.Assert(key != null, "key != null");
            Debug.Assert(value != null, "value != null");

            List<string> valueArray = new List<string>();
            valueArray.Add(value);
            this[key] = valueArray;
        }
    }
}
