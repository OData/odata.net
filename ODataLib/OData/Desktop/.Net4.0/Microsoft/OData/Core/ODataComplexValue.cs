//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
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
