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
