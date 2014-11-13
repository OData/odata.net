//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
