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

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a member of an EDM enumeration type.
    /// </summary>
    public class EdmEnumMember : EdmNamedElement, IEdmEnumMember
    {
        private readonly IEdmEnumType declaringType;
        private IEdmPrimitiveValue value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEnumMember"/> class.
        /// </summary>
        /// <param name="declaringType">The type that declares this member.</param>
        /// <param name="name">Name of this enumeration member.</param>
        /// <param name="value">Value of this enumeration member.</param>
        public EdmEnumMember(IEdmEnumType declaringType, string name, IEdmPrimitiveValue value)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(declaringType, "declaringType");
            EdmUtil.CheckArgumentNull(value, "value");

            this.declaringType = declaringType;
            this.value = value;
        }

        /// <summary>
        /// Gets the type that this member belongs to.
        /// </summary>
        public IEdmEnumType DeclaringType
        {
            get { return this.declaringType; }
        }

        /// <summary>
        /// Gets the value of this enumeration type member.
        /// </summary>
        public IEdmPrimitiveValue Value
        {
            get { return this.value; }
        }
    }
}
