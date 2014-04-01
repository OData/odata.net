//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Values;

    /// <summary>
    /// Enum node
    /// </summary>
    public class EnumNode : SingleValueNode
    {
        /// <summary>
        /// Cache for the TypeReference after it has been calculated for the current state of the node.
        /// </summary>
        private readonly IEdmTypeReference typeReference;

        /// <summary>
        /// The Edm enum value.
        /// </summary>
        private readonly IEdmEnumValue value;

        /// <summary>
        /// Create an EnumNode
        /// </summary>
        /// <param name="typeReference">edm enum type reference</param>
        /// <param name="enumValue">input enum value</param>
        public EnumNode(IEdmEnumTypeReference typeReference, IEdmEnumValue enumValue)
        {
            this.typeReference = typeReference;
            this.value = enumValue;
        }

        /// <summary>
        /// Edm type reference
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get
            {
                return this.typeReference;
            }
        }

        /// <summary>
        /// Edm enum value
        /// </summary>
        public IEdmEnumValue Value
        {
            get
            {
                return this.value;
            }
        }
    }
}
