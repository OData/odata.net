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

namespace System.Data.OData
{
    #region Namespaces.
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    #endregion Namespaces.

    /// <summary>
    /// OData representation of a MultiValue.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class ODataMultiValue
#else
    public sealed class ODataMultiValue
#endif
    {
        /// <summary>
        /// The type of the complex value.
        /// </summary>
        public string TypeName
        {
            get;
            set;
        }

        /// <summary>
        /// The items in the bag value.
        /// </summary>
        public IEnumerable Items
        {
            get;
            set;
        }
    }
}
