//---------------------------------------------------------------------
// <copyright file="MetadataProviderEdmAction.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmAction"/> implementation backed by an IDSMP metadata provider.
    /// </summary>
    internal sealed class MetadataProviderEdmAction : MetadataProviderEdmOperation, IEdmAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataProviderEdmActionImport"/> class.
        /// </summary>
        /// <param name="model">The model this instance belongs to.</param>
        /// <param name="operation">The resource operation underlying this action import.</param>
        /// <param name="namespaceName">The namespace of the EdmOperation.</param>
        /// <remarks>
        /// This constructor assumes that the entity set for this service operation has already be created.
        /// </remarks>
        internal MetadataProviderEdmAction(
            MetadataProviderEdmModel model, 
            OperationWrapper operation,
            string namespaceName)
            : base(model, operation, namespaceName)
        {
        }

        /// <summary>
        /// Gets the kind of this schema element.
        /// </summary>
        public override EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Action; }
        }
    }
}
