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

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a reference to an EDM decimal type.
    /// </summary>
    public class EdmDecimalTypeReference : EdmPrimitiveTypeReference, IEdmDecimalTypeReference
    {
        private readonly int? precision;
        private readonly int? scale;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDecimalTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmDecimalTypeReference(IEdmPrimitiveType definition, bool isNullable)
            : this(definition, isNullable, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDecimalTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        /// <param name="precision">Precision of values with this type.</param>
        /// <param name="scale">Scale of values with this type.</param>
        public EdmDecimalTypeReference(IEdmPrimitiveType definition, bool isNullable, int? precision, int? scale)
            : base(definition, isNullable)
        {
            this.precision = precision;
            this.scale = scale;
        }

        /// <summary>
        /// Gets the precision of this type.
        /// </summary>
        public int? Precision
        {
            get { return this.precision; }
        }

        /// <summary>
        /// Gets the scale of this type.
        /// </summary>
        public int? Scale
        {
            get { return this.scale; }
        }
    }
}
