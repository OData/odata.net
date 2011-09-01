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

namespace Microsoft.Data.OData.Atom
{
    /// <summary>
    /// Caches value of a multivalue item and all its nested properties/items enumerations.
    /// </summary>
    internal sealed class EpmMultiValueItemCache : EpmValueCache
    {
        /// <summary>
        /// The value of the multivalue item being cached.
        /// Currently this will only store complex values, primitive values are not cached as this class but as the value itself.
        /// </summary>
        private readonly object itemValue;

        /// <summary>
        /// Creates a new item cache.
        /// </summary>
        /// <param name="itemValue">The value of the multivalue item to cache.</param>
        internal EpmMultiValueItemCache(object itemValue)
        {
            DebugUtils.CheckNoExternalCallers();
            this.itemValue = itemValue;
        }

        /// <summary>
        /// Returns the value of the item which is cached.
        /// </summary>
        internal object ItemValue
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.itemValue;
            }
        }
    }
}
