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

using Microsoft.OData.Edm;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents an EDM property.
    /// </summary>
    public abstract class EdmProperty : EdmNamedElement, IEdmProperty
    {
        private readonly IEdmStructuredType declaringType;
        private readonly IEdmTypeReference type;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmProperty"/> class.
        /// </summary>
        /// <param name="declaringType">The type that declares this property.</param>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        protected EdmProperty(IEdmStructuredType declaringType, string name, IEdmTypeReference type)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(declaringType, "declaringType");
            EdmUtil.CheckArgumentNull(type, "type");

            this.declaringType = declaringType;
            this.type = type;
        }

        /// <summary>
        /// Gets the type of this property.
        /// </summary>
        public IEdmTypeReference Type
        {
            get { return this.type; }
        }
        
        /// <summary>
        /// Gets the type that this property belongs to.
        /// </summary>
        public IEdmStructuredType DeclaringType
        {
            get { return this.declaringType; }
        }

        /// <summary>
        /// Gets the kind of this property.
        /// </summary>
        public abstract EdmPropertyKind PropertyKind
        {
            get;
        }
    }
}
