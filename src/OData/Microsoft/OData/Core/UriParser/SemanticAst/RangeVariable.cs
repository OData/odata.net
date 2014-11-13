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

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces
    using System;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// A RangeVariable, which represents an iterator variable either over a collection, either of entities or not.
    /// Exists outside of the main SemanticAST, but hooked in via a RangeVariableReferenceNode (either Non-Entity or Entity).
    /// </summary>
    public abstract class RangeVariable : ODataAnnotatable
    {
        /// <summary>
        /// Gets the name of the associated rangeVariable.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the type of entity referenced by this rangeVariable
        /// </summary>
        public abstract IEdmTypeReference TypeReference { get; }

        /// <summary>
        /// Gets the kind of this rangeVariable.
        /// </summary>
        public abstract int Kind { get; }
    }
}
