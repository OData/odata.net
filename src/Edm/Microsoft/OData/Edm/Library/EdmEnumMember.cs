//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Library
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
