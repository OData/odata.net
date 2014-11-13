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

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM function import.
    /// </summary>
    public class EdmFunctionImport : EdmFunctionBase, IEdmFunctionImport
    {
        private readonly IEdmEntityContainer container;
        private readonly IEdmExpression entitySet;
        private readonly bool isSideEffecting;
        private readonly bool isComposable;
        private readonly bool isBindable;

        /// <summary>
        /// Initializes a new instance of <see cref="EdmFunctionImport"/> class (side-effecting, non-composable, non-bindable).
        /// </summary>
        /// <param name="container">An <see cref="IEdmEntityContainer"/> containing this function import.</param>
        /// <param name="name">Name of the function import.</param>
        /// <param name="returnType">Return type of the function import.</param>
        public EdmFunctionImport(IEdmEntityContainer container, string name, IEdmTypeReference returnType)
            : this(container, name, returnType, null, true, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EdmFunctionImport"/> class (side-effecting, non-composable, non-bindable).
        /// </summary>
        /// <param name="container">An <see cref="IEdmEntityContainer"/> containing this function import.</param>
        /// <param name="name">Name of the function import.</param>
        /// <param name="returnType">Return type of the function import.</param>
        /// <param name="entitySet">An entity set containing entities returned by this function import. 
        /// The two expression kinds supported are <see cref="IEdmEntitySetReferenceExpression"/> and <see cref="IEdmPathExpression"/>.</param>
        public EdmFunctionImport(IEdmEntityContainer container, string name, IEdmTypeReference returnType, IEdmExpression entitySet)
            : this(container, name, returnType, entitySet, true, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EdmFunctionImport"/> class.
        /// </summary>
        /// <param name="container">An <see cref="IEdmEntityContainer"/> containing this function import.</param>
        /// <param name="name">Name of the function import.</param>
        /// <param name="returnType">Return type of the function import.</param>
        /// <param name="entitySet">An entity set containing entities returned by this function import. 
        /// The two expression kinds supported are <see cref="IEdmEntitySetReferenceExpression"/> and <see cref="IEdmPathExpression"/>.</param>
        /// <param name="isSideEffecting">A value indicating whether this function import has side-effects.</param>
        /// <param name="isComposable">A value indicating whether this functon import can be composed inside expressions.</param>
        /// <param name="isBindable">A value indicating whether this function import can be used as an extension method for the type of the first parameter of this function import.</param>
        public EdmFunctionImport(IEdmEntityContainer container, string name, IEdmTypeReference returnType, IEdmExpression entitySet, bool isSideEffecting, bool isComposable, bool isBindable)
            : base(name, returnType)
        {
            EdmUtil.CheckArgumentNull(container, "container");
            EdmUtil.CheckArgumentNull(name, "name");

            this.container = container;
            this.entitySet = entitySet;
            this.isSideEffecting = isSideEffecting;
            this.isComposable = isComposable;
            this.isBindable = isBindable;
        }

        /// <summary>
        /// Gets a value indicating whether this function import has side-effects.
        /// <see cref="IsSideEffecting"/> cannot be set to true if <see cref="IsComposable"/> is set to true.
        /// </summary>
        public bool IsSideEffecting
        {
            get { return this.isSideEffecting; }
        }

        /// <summary>
        /// Gets a value indicating whether this functon import can be composed inside expressions.
        /// <see cref="IsComposable"/> cannot be set to true if <see cref="IsSideEffecting"/> is set to true.
        /// </summary>
        public bool IsComposable
        {
            get { return this.isComposable; } 
        }

        /// <summary>
        /// Gets a value indicating whether this function import can be used as an extension method for the type of the first parameter of this function import.
        /// </summary>
        public bool IsBindable
        {
            get { return this.isBindable; }
        }

        /// <summary>
        /// Gets the entity set containing entities returned by this function import.
        /// </summary>
        public IEdmExpression EntitySet
        {
            get { return this.entitySet; }
        }

        /// <summary>
        /// Gets the kind of this function, which is always FunctionImport.
        /// </summary>
        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.FunctionImport; }
        }

        /// <summary>
        /// Gets the container of this function.
        /// </summary>
        public IEdmEntityContainer Container
        {
            get { return this.container; }
        }
    }
}
