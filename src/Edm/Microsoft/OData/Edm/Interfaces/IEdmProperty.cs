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

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Defines EDM property types.
    /// </summary>
    public enum EdmPropertyKind
    {
        /// <summary>
        /// Represents a property with an unknown or error kind.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents a property implementing <see cref="IEdmStructuralProperty"/>.
        /// </summary>
        Structural,

        /// <summary>
        /// Represents a property implementing <see cref="IEdmNavigationProperty"/>. 
        /// </summary>
        Navigation,
    }

    /// <summary>
    /// Represents an EDM property.
    /// </summary>
    public interface IEdmProperty : IEdmNamedElement, IEdmVocabularyAnnotatable
    {
        /// <summary>
        /// Gets the kind of this property.
        /// </summary>
        EdmPropertyKind PropertyKind { get; }

        /// <summary>
        /// Gets the type of this property.
        /// </summary>
        IEdmTypeReference Type { get; }

        /// <summary>
        /// Gets the type that this property belongs to.
        /// </summary>
        IEdmStructuredType DeclaringType { get; }
    }
}
