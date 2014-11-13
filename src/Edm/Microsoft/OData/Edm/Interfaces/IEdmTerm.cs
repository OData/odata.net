//   OData .NET Libraries ver. 6.8.1
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
    /// Defines EDM term kinds.
    /// </summary>
    public enum EdmTermKind
    {
        /// <summary>
        /// Represents a term with unknown or error kind.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents a term implementing <see cref="IEdmStructuredType"/> and <see cref="IEdmSchemaType"/>.
        /// </summary>
        Type,

        /// <summary>
        /// Represents a term implementing <see cref="IEdmValueTerm"/>.
        /// </summary>
        Value
    }

    /// <summary>
    /// Term to which an annotation can bind.
    /// </summary>
    public interface IEdmTerm : IEdmSchemaElement
    {
        /// <summary>
        /// Gets the kind of a term.
        /// </summary>
        EdmTermKind TermKind { get; }
    }
}
