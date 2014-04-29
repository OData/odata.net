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

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using Microsoft.Data.Edm;
    #endregion Namespaces

    /// <summary>
    /// Class representing a single key property value in a key lookup.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class KeyPropertyValue
#else
    public sealed class KeyPropertyValue
#endif
    {
        /// <summary>
        /// The key property.
        /// </summary>
        public IEdmProperty KeyProperty
        {
            get;
            set;
        }

        /// <summary>
        /// The value of the key property.
        /// </summary>
        public SingleValueQueryNode KeyValue
        {
            get;
            set;
        }
    }
}
