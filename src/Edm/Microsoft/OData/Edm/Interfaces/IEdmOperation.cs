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
    using System.Collections.Generic;
    using Microsoft.OData.Edm.Expressions;

    /// <summary>
    /// Represents an EDM operation.
    /// </summary>
    public interface IEdmOperation : IEdmSchemaElement
    {
        /// <summary>
        /// Gets the return type of this function.
        /// </summary>
        IEdmTypeReference ReturnType { get; }

        /// <summary>
        /// Gets the collection of parameters for this function.
        /// </summary>
        IEnumerable<IEdmOperationParameter> Parameters { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is bound.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is bound; otherwise, <c>false</c>.
        /// </value>
        bool IsBound { get; }

        /// <summary>
        /// Gets the entity set path expression.
        /// </summary>
        /// <value>
        /// The entity set path expression.
        /// </value>
        IEdmPathExpression EntitySetPath { get; }

        /// <summary>
        /// Searches for a parameter with the given name, and returns null if no such parameter exists.
        /// </summary>
        /// <param name="name">The name of the parameter being found.</param>
        /// <returns>The requested parameter or null if no such parameter exists.</returns>
        IEdmOperationParameter FindParameter(string name);
    }
}
