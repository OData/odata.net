//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
