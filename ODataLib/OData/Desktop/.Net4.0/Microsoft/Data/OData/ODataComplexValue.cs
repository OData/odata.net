//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
    public sealed class ODataComplexValue : ODataValue
    {
        /// <summary>Gets or sets the properties and values of the complex value.</summary>
        /// <returns>The properties and values of the complex value.</returns>
        public IEnumerable<ODataProperty> Properties
        {
            get;
            set;
        }

        /// <summary>Gets or sets the type of the complex value.</summary>
        /// <returns>The type of the complex value.</returns>
        public string TypeName
        {
            get;
            set;
        }
    }
}
