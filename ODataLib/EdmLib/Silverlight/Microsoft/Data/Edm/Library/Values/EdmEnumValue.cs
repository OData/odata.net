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

using Microsoft.Data.Edm.Values;

namespace Microsoft.Data.Edm.Library.Values
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
