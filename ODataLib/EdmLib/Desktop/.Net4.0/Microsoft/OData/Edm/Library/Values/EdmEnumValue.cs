//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Library.Values
{
    /// <summary>
    /// Represents an EDM enumeration type value.
    /// </summary>
    public class EdmEnumValue : EdmValue, IEdmEnumValue
    {
        private readonly IEdmPrimitiveValue value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEnumValue"/> class. 
        /// </summary>
        /// <param name="type">A reference to the enumeration type that describes this value.</param>
        /// <param name="member">The enumeration type value.</param>
        public EdmEnumValue(IEdmEnumTypeReference type, IEdmEnumMember member)
            : this(type, member.Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEnumValue"/> class. 
        /// </summary>
        /// <param name="type">A reference to the enumeration type that describes this value.</param>
        /// <param name="value">The underlying type value.</param>
        public EdmEnumValue(IEdmEnumTypeReference type, IEdmPrimitiveValue value)
            : base(type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the underlying type value of the enumeration type.
        /// </summary>
        public IEdmPrimitiveValue Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.Enum; }
        }
    }
}
