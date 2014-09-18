//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
