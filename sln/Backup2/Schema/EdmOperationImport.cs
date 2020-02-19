//---------------------------------------------------------------------
// <copyright file="EdmOperationImport.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    using Microsoft.OData.Edm.Vocabularies;

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
        /// The expression kind supported is <see cref="IEdmPathExpression"/>.</param>
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
