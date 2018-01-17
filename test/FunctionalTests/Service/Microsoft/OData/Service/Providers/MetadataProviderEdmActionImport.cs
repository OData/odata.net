//---------------------------------------------------------------------
// <copyright file="MetadataProviderEdmActionImport.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmActionImport"/> implementation backed by an IDSMP metadata provider.
    /// </summary>
    internal sealed class MetadataProviderEdmActionImport : MetadataProviderEdmOperationImport, IEdmActionImport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataProviderEdmActionImport"/> class.
        /// </summary>
        /// <param name="model">The model this instance belongs to.</param>
        /// <param name="container">The container this instance belongs to.</param>
        /// <param name="action">The edm action object underlying this function import.</param>
        /// <remarks>
        /// This constructor assumes that the entity set for this service operation has already be created.
        /// </remarks>
        internal MetadataProviderEdmActionImport(
            MetadataProviderEdmModel model, 
            MetadataProviderEdmEntityContainer container, 
            MetadataProviderEdmAction action)
            : base(model, container, action)
        {
            this.Action = action;
        }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        public IEdmAction Action { get; private set; }

        /// <summary>
        /// The container element kind; EdmContainerElementKind.ActionImport for operation imports.
        /// </summary>
        public override EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.ActionImport; }
        }
    }
}
