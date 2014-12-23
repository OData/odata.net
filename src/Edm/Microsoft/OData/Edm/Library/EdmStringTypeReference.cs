//   OData .NET Libraries ver. 6.9
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

using System;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a reference to an EDM string type.
    /// </summary>
    public class EdmStringTypeReference : EdmPrimitiveTypeReference, IEdmStringTypeReference
    {
        private readonly bool isUnbounded;
        private readonly int? maxLength;
        private readonly bool? isUnicode;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmStringTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmStringTypeReference(IEdmPrimitiveType definition, bool isNullable)
            : this(definition, isNullable, false, null, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmStringTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        /// <param name="isUnbounded">Denotes whether the max length is the maximum allowed value.</param>
        /// <param name="maxLength">Maximum length of a value of this type.</param>
        /// <param name="isUnicode">Denotes if string is encoded using Unicode.</param>
        public EdmStringTypeReference(IEdmPrimitiveType definition, bool isNullable, bool isUnbounded, int? maxLength, bool? isUnicode)
            : base(definition, isNullable)
        {
            if (isUnbounded && maxLength != null)
            {
                throw new InvalidOperationException(Edm.Strings.EdmModel_Validator_Semantic_IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull);
            }

            this.isUnbounded = isUnbounded;
            this.maxLength = maxLength;
            this.isUnicode = isUnicode;
        }

        /// <summary>
        /// Gets a value indicating whether this string type specifies the maximum allowed length.
        /// </summary>
        public bool IsUnbounded
        {
            get { return this.isUnbounded; }
        }

        /// <summary>
        /// Gets the maximum length of this string type.
        /// </summary>
        public int? MaxLength
        {
            get { return this.maxLength; }
        }

        /// <summary>
        /// Gets a value indicating whether this string type supports unicode encoding.
        /// </summary>
        public bool? IsUnicode
        {
            get { return this.isUnicode; }
        }
    }
}
