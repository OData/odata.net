//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    using Microsoft.OData.Edm.Expressions;

    /// <summary>
    /// Represents an EDM operation import.
    /// </summary>
    public abstract class EdmOperationImport : EdmNamedElement, IEdmOperationImport
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EdmOperationImport"/> class.
        /// </summary>
        /// <param name="container">An <see cref="IEdmEntityContainer"/> containing this operation import.</param>
        /// <param name="operation">The operation of the import.</param>
        /// <param name="name">Name of the operation import.</param>
        /// <param name="entitySet">An entity set containing entities returned by this operation import. 
        /// The two expression kinds supported are <see cref="IEdmEntitySetReferenceExpression"/> and <see cref="IEdmPathExpression"/>.</param>
        protected EdmOperationImport(
            IEdmEntityContainer container,
            IEdmOperation operation,
            string name,
            IEdmExpression entitySet)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(container, "container");
            EdmUtil.CheckArgumentNull(operation, this.OperationArgumentNullParameterName());

            this.Container = container;
            this.Operation = operation;
            this.EntitySet = entitySet;
        }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        public IEdmOperation Operation { get; private set; }

        /// <summary>
        /// Gets the entity set containing entities returned by this operation import.
        /// </summary>
        public IEdmExpression EntitySet { get; private set; }

        /// <summary>
        /// Gets the kind of this operation, which is always FunctionImport.
        /// </summary>
        public abstract EdmContainerElementKind ContainerElementKind { get; }
        
        /// <summary>
        /// Gets the container of this operation.
        /// </summary>
        public IEdmEntityContainer Container { get; private set; }

        /// <summary>
        /// Operations the name of the argument null parameter.
        /// </summary>
        /// <returns>A string that is the name of the operation parameter in the derived operation class.</returns>
        protected abstract string OperationArgumentNullParameterName();
    }
}
