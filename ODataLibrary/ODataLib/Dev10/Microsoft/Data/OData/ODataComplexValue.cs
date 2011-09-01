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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary>
    /// OData representation of a complex value.
    /// </summary>
    public sealed class ODataComplexValue : ODataAnnotatable
    {
        /// <summary>
        /// The properties and values of the complex value.
        /// </summary>
        public IEnumerable<ODataProperty> Properties
        {
            get;
            set;
        }

        /// <summary>
        /// The type of the complex value.
        /// </summary>
        public string TypeName
        {
            get;
            set;
        }
    }
}
