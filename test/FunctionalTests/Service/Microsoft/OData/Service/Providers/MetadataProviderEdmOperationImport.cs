//---------------------------------------------------------------------
// <copyright file="MetadataProviderEdmOperationImport.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;

    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmOperationImport"/> implementation backed by an IDSMP metadata provider.
    /// </summary>
    internal abstract class MetadataProviderEdmOperationImport : EdmElement, IEdmOperationImport
    {
        /// <summary>The model this instance belongs to.</summary>
        private readonly MetadataProviderEdmModel model;

        /// <summary>The container this instance belongs to.</summary>
        private readonly MetadataProviderEdmEntityContainer container;

        /// <summary> EdmOperation that the OperationImport imports into the container. </summary>
        private readonly MetadataProviderEdmOperation edmOperation;

        /// <summary> Gets the entity set path of the function import. </summary>
        private readonly string entitySetPath;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model">The model this instance belongs to.</param>
        /// <param name="container">The container this instance belongs to.</param>
        /// <param name="edmOperation">The edm operation underlying this function import.</param>
        /// <remarks>This constructor assumes that the entity set for this service operation has already be created.</remarks>
        protected internal MetadataProviderEdmOperationImport(
            MetadataProviderEdmModel model, 
            MetadataProviderEdmEntityContainer container,
            MetadataProviderEdmOperation edmOperation)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(container != null, "container != null");
            Debug.Assert(edmOperation != null, "edmOperation != null");

            this.container = container;
            this.model = model;
            this.edmOperation = edmOperation;

            // EntitySetPath=<path string>
            ResourceSetPathExpression resultSetPathExpression = edmOperation.ServiceOperation.ResultSetPathExpression;
            this.entitySetPath = resultSetPathExpression == null ? null : resultSetPathExpression.PathExpression;
        }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        public IEdmOperation Operation
        {
            get { return this.edmOperation; }
        }

        /// <summary>
        /// The entity set underlying the result of the function import or null
        /// if no such entity set exists.
        /// </summary>
        /// <remarks>The property assumes that the entity set has already been created and cached by the model so we can look it up here.</remarks>
        public IEdmExpression EntitySet
        {
            get
            {
                ResourceSetWrapper resourceSet = this.edmOperation.ServiceOperation.ResourceSet;
                
                if (resourceSet != null)
                {
                    return new EdmPathExpression(this.model.EnsureEntitySet(resourceSet).Name);
                }

                if (this.entitySetPath != null)
                {
                    // Construct the entity set path expression holding this.entitySetPath.
                    // Note that no name resolution is happening at this point. The constructed expression will provide the path value for serialization
                    // and it should also work with IEdmOperationImport.TryGetRelativeEntitySetPath extension method.
                    // However, because the entitySetPathExpression.Referenced holds a stub named element some client code might get confused as it can try
                    // casting it to IEdmNavigationProperty.
                    string[] path = this.entitySetPath.Split(ResourceSetPathExpression.PathSeparator);
                    return new EdmPathExpression(path);
                }

                return null;
            }
        }

        /// <summary>
        /// The name of the function import.
        /// </summary>
        public string Name
        {
            get { return this.edmOperation.Name; }
        }

        /// <summary>
        /// Gets the namespace this schema element belongs to.
        /// </summary>
        public string Namespace
        {
            get { return this.container.Namespace; }
        }

        /// <summary>
        /// The container element kind; EdmContainerElementKind.FunctionImport for operation imports.
        /// </summary>
        public abstract EdmContainerElementKind ContainerElementKind { get; }

        /// <summary>
        /// Gets the container of this function import.
        /// </summary>
        public IEdmEntityContainer Container
        {
            // All operation imports are in the default container.
            get { return this.container; }
        }

        /// <summary>
        /// The resource service operation underlying this function import.
        /// </summary>
        internal OperationWrapper ServiceOperation
        {
            get { return this.edmOperation.ServiceOperation; }
        }
    }
}
