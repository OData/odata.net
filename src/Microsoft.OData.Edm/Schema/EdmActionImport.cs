//---------------------------------------------------------------------
// <copyright file="EdmActionImport.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM action import.
    /// </summary>
    public class EdmActionImport : EdmOperationImport, IEdmActionImport
    {
        private const string ActionArgumentNullParameterName = "action";

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmActionImport"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        public EdmActionImport(IEdmEntityContainer container, string name, IEdmAction action)
            : this(container, name, action, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmActionImport"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <param name="entitySetExpression">The entity set expression.</param>
        public EdmActionImport(IEdmEntityContainer container, string name, IEdmAction action, IEdmExpression entitySetExpression)
            : base(container, action, name, entitySetExpression)
        {
            EdmUtil.CheckArgumentNull(action, "action");

            this.Action = action;
        }

        /// <summary>
        /// Gets the action type of the import.
        /// </summary>
        public IEdmAction Action { get; private set; }

        /// <summary>
        /// Gets the kind of this actionimport, which is always ActionImport.
        /// </summary>
        public override EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.ActionImport; }
        }

        /// <summary>
        /// Indicates the name of the constructor argument that is passed to EdmOperationImport.
        /// </summary>
        /// <returns>Returns the name of the operation from this import.</returns>
        protected override string OperationArgumentNullParameterName()
        {
            return ActionArgumentNullParameterName;
        }
    }
}
