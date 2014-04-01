//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents an EDM operation import.
    /// </summary>
    public abstract class EdmOperationImport : EdmFunctionBase, IEdmOperationImport
    {
        private readonly IEdmEntityContainer container;
        private readonly IEdmExpression entitySet;
        private readonly bool isSideEffecting;
        private readonly bool isComposable;
        private readonly bool isBindable;

        /// <summary>
        /// Initializes a new instance of <see cref="EdmOperationImport"/> class (side-effecting, non-composable, non-bindable).
        /// </summary>
        /// <param name="container">An <see cref="IEdmEntityContainer"/> containing this operation import.</param>
        /// <param name="name">Name of the operation import.</param>
        /// <param name="returnType">Return type of the operation import.</param>
        protected EdmOperationImport(IEdmEntityContainer container, string name, IEdmTypeReference returnType)
            : this(container, name, returnType, null, true, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EdmOperationImport"/> class (side-effecting, non-composable, non-bindable).
        /// </summary>
        /// <param name="container">An <see cref="IEdmEntityContainer"/> containing this operation import.</param>
        /// <param name="name">Name of the operation import.</param>
        /// <param name="returnType">Return type of the operation import.</param>
        /// <param name="entitySet">An entity set containing entities returned by this operation import. 
        /// The two expression kinds supported are <see cref="IEdmEntitySetReferenceExpression"/> and <see cref="IEdmPathExpression"/>.</param>
        protected EdmOperationImport(IEdmEntityContainer container, string name, IEdmTypeReference returnType, IEdmExpression entitySet)
            : this(container, name, returnType, entitySet, true, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EdmOperationImport"/> class.
        /// </summary>
        /// <param name="container">An <see cref="IEdmEntityContainer"/> containing this operation import.</param>
        /// <param name="name">Name of the operation import.</param>
        /// <param name="returnType">Return type of the operation import.</param>
        /// <param name="entitySet">An entity set containing entities returned by this operation import. 
        /// The two expression kinds supported are <see cref="IEdmEntitySetReferenceExpression"/> and <see cref="IEdmPathExpression"/>.</param>
        /// <param name="isSideEffecting">A value indicating whether this operation import has side-effects.</param>
        /// <param name="isComposable">A value indicating whether this operation import can be composed inside expressions.</param>
        /// <param name="isBindable">A value indicating whether this operation import can be used as an extension method for the type of the first parameter of this operation import.</param>
        protected EdmOperationImport(
            IEdmEntityContainer container, 
            string name, 
            IEdmTypeReference returnType, 
            IEdmExpression entitySet, 
            bool isSideEffecting, 
            bool isComposable, 
            bool isBindable)
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
        /// Gets a value indicating whether this operation import has side-effects.
        /// <see cref="IsSideEffecting"/> cannot be set to true if <see cref="IsComposable"/> is set to true.
        /// </summary>
        public bool IsSideEffecting
        {
            get { return this.isSideEffecting; }
        }

        /// <summary>
        /// Gets a value indicating whether this operation import can be composed inside expressions.
        /// <see cref="IsComposable"/> cannot be set to true if <see cref="IsSideEffecting"/> is set to true.
        /// </summary>
        public bool IsComposable
        {
            get { return this.isComposable; } 
        }

        /// <summary>
        /// Gets a value indicating whether this operation import can be used as an extension method for the type of the first parameter of this operation import.
        /// </summary>
        public bool IsBindable
        {
            get { return this.isBindable; }
        }

        /// <summary>
        /// Gets the entity set containing entities returned by this operation import.
        /// </summary>
        public IEdmExpression EntitySet
        {
            get { return this.entitySet; }
        }

        /// <summary>
        /// Gets the kind of this operation, which is always FunctionImport.
        /// </summary>
        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.OperationImport; }
        }

        /// <summary>
        /// Gets the container of this operation.
        /// </summary>
        public IEdmEntityContainer Container
        {
            get { return this.container; }
        }
    }
}
