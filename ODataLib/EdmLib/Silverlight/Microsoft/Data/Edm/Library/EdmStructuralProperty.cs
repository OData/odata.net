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
    /// Represents an EDM structural (i.e. non-navigation) property.
    /// </summary>
    public class EdmStructuralProperty : EdmProperty, IEdmStructuralProperty
    {
        private readonly string defaultValueString;
        private readonly EdmConcurrencyMode concurrencyMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmStructuralProperty"/> class.
        /// </summary>
        /// <param name="declaringType">The type that declares this property.</param>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">The type of the property.</param>
        public EdmStructuralProperty(IEdmStructuredType declaringType, string name, IEdmTypeReference type)
            : this(declaringType, name, type, null,  EdmConcurrencyMode.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmStructuralProperty"/> class.
        /// </summary>
        /// <param name="declaringType">The type that declares this property.</param>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="defaultValueString">The default value of this property.</param>
        /// <param name="concurrencyMode">The concurrency mode of this property.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "defaultValue might be confused for an IEdmValue.")]
        public EdmStructuralProperty(IEdmStructuredType declaringType, string name, IEdmTypeReference type, string defaultValueString, EdmConcurrencyMode concurrencyMode)
            : base(declaringType, name, type)
        {
            this.defaultValueString = defaultValueString;
            this.concurrencyMode = concurrencyMode;
        }

        /// <summary>
        /// Gets the default value of this property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", Justification = "defaultValue might be confused for an IEdmValue.")]
        public string DefaultValueString
        {
            get { return this.defaultValueString; }
        }

        /// <summary>
        /// Gets the concurrency mode of this property.
        /// </summary>
        public EdmConcurrencyMode ConcurrencyMode
        {
            get { return this.concurrencyMode; }
        }

        /// <summary>
        /// Gets the kind of this property.
        /// </summary>
        public override EdmPropertyKind PropertyKind
        {
            get { return EdmPropertyKind.Structural; }
        }
    }
}
