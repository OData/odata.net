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
    /// Represents a reference to an EDM type.
    /// </summary>
    public abstract class EdmTypeReference : EdmElement, IEdmTypeReference
    {
        private readonly IEdmType definition;
        private readonly bool isNullable;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTypeReference"/> class.
        /// </summary>
        /// <param name="definition">Type that describes this value.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        protected EdmTypeReference(IEdmType definition, bool isNullable)
        {
            EdmUtil.CheckArgumentNull(definition, "definition");

            this.definition = definition;
            this.isNullable = isNullable;
        }

        /// <summary>
        /// Gets a value indicating whether this type is nullable.
        /// </summary>
        public bool IsNullable
        {
            get { return this.isNullable; }
        }

        /// <summary>
        /// Gets the definition to which this type refers.
        /// </summary>
        public IEdmType Definition
        {
            get { return this.definition; }
        }

        /// <summary>
        /// Returns the text representation of the current object.
        /// </summary>
        /// <returns>The text representation of the current object.</returns>
        public override string ToString()
        {
            return this.ToTraceString();
        }
    }
}
