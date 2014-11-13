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

using Microsoft.Data.Edm.Expressions;

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// Represents an EDM function import.
    /// </summary>
    public interface IEdmFunctionImport : IEdmFunctionBase, IEdmEntityContainerElement
    {
        /// <summary>
        /// Gets a value indicating whether this function import has side-effects.
        /// <see cref="IsSideEffecting"/> cannot be set to true if <see cref="IsComposable"/> is set to true.
        /// </summary>
        bool IsSideEffecting { get; }

        /// <summary>
        /// Gets a value indicating whether this functon import can be composed inside expressions.
        /// <see cref="IsComposable"/> cannot be set to true if <see cref="IsSideEffecting"/> is set to true.
        /// </summary>
        bool IsComposable { get; }

        /// <summary>
        /// Gets a value indicating whether this function import can be used as an extension method for the type of the first parameter of this function import.
        /// </summary>
        bool IsBindable { get; }

        /// <summary>
        /// Gets the entity set containing entities returned by this function import.
        /// </summary>
        IEdmExpression EntitySet { get; }
    }
}
