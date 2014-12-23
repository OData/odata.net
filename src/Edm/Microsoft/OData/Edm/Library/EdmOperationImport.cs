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
