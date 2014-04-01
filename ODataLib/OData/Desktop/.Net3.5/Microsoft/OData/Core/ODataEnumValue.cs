//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    /// <summary>
    /// OData enum value
    /// </summary>
    public sealed class ODataEnumValue : ODataValue
    {
        /// <summary>Constructor</summary>
        /// <param name="value">The backing type, can be "3" or "White" or "Black,Yellow,Red".</param>
        public ODataEnumValue(string value)
        {
            this.Value = value;
            this.TypeName = null;
        }

        /// <summary>Constructor</summary>
        /// <param name="value">The backing type, can be "3" or "White" or "Black,Yellow,Red".</param>
        /// <param name="typeName">The type name in edm model.</param>
        public ODataEnumValue(string value, string typeName)
        {
            this.Value = value;
            this.TypeName = typeName;
        }

        /// <summary>Get backing type value, can be "3" or "White" or "Black,Yellow,Red".</summary>
        public string Value { get; private set; }

        /// <summary>Get the type name in edm model.</summary>
        public string TypeName { get; private set; }
    }
}
