//---------------------------------------------------------------------
// <copyright file="MetadataProviderEdmEntityContainer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmEntityContainer"/> implementation backed by an IDSMP metadata provider.
    /// </summary>
    internal sealed class MetadataProviderEdmEntityContainer : EdmElement, IEdmEntityContainer
    {
        /// <summary>The model this instance belongs to.</summary>
        private readonly MetadataProviderEdmModel model;

        /// <summary>
        /// The name of the entity container.
        /// </summary>
        private readonly string containerNamespace;

        /// <summary>
        /// The name of the entity container.
        /// </summary>
        private readonly string containerName;

        /// <summary>The cache of entity sets.</summary>
        private readonly Dictionary<string, IEdmEntitySet> entitySetCache;

        /// <summary>The cache of operation imports.</summary>
        private readonly Dictionary<string, List<IEdmOperationImport>> operationImportCache;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model">The model this instance belongs to.</param>
        /// <param name="containerName">The name of the entity container.</param>
        /// <param name="entityContainerSchemaNamespace">The namespace of the schema this entity container should be made part of during serialization.</param>
        internal MetadataProviderEdmEntityContainer(MetadataProviderEdmModel model, string containerName, string entityContainerSchemaNamespace)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(!string.IsNullOrEmpty(containerName), "!string.IsNullOrEmpty(containerName)");
            Debug.Assert(!string.IsNullOrEmpty(entityContainerSchemaNamespace), "!string.IsNullOrEmpty(entityContainerSchemaNamespace)");

            this.model = model;
            this.containerName = containerName;
            this.containerNamespace = entityContainerSchemaNamespace;

            this.entitySetCache = new Dictionary<string, IEdmEntitySet>(StringComparer.Ordinal);
            this.operationImportCache = new Dictionary<string, List<IEdmOperationImport>>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Gets a collection of the elements of this entity container.
        /// </summary>
        /// <remarks>
        /// Cache state: EntityContainers required. We only support enumerating the elements once
        ///     the entity container has been completely populated which is the case when all entity containers
        ///     have been cached.
        /// </remarks>
        public IEnumerable<IEdmEntityContainerElement> Elements
        {
            get 
            {
                this.model.AssertCacheState(MetadataProviderState.Full);

                foreach (IEdmEntitySet entitySet in this.entitySetCache.Values)
                {
                    yield return entitySet;
                }

                foreach (IEdmOperationImport operationImport in this.operationImportCache.Values.SelectMany(v => v))
                {
                    yield return operationImport;
                }
            }
        }

        /// <summary>
        /// Gets the schema kind of this element.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.EntityContainer; }
        }

        /// <summary>
        /// Gets the namespace of this element.
        /// </summary>
        public string Namespace
        {
            get { return this.containerNamespace; }
        }

        /// <summary>
        /// Gets the name of this element.
        /// </summary>
        public string Name
        {
            get { return this.containerName; }
        }

        /// <summary>
        /// Searches for an entity set with the given name in this entity container and returns null if no such set exists.
        /// </summary>
        /// <param name="name">The name of the element being found.</param>
        /// <returns>The requested entity set, or null if the entity set does not exist.</returns>
        /// <remarks>
        /// Cache state: EntityContainers required. We only support looking up entity sets once
        ///     all entity containers have been completely populated and cached.
        /// </remarks>
        public IEdmEntitySet FindEntitySet(string name)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(name, "name");

            // NOTE: for the query parser we might want to allow incremental lookup
            //       of entity sets in the future.
            IEdmEntitySet entitySet;
            if (this.entitySetCache.TryGetValue(name, out entitySet))
            {
                return entitySet;
            }

            return this.LazyLoadEntitySet(name);
        }

        /// <summary>
        /// Searches for a singleton with the given name in this entity container and returns null if no such singleton exists.
        /// </summary>
        /// <param name="singletonName">The name of the singleton to search.</param>
        /// <returns>The requested singleton, or null if the singleton does not exist.</returns>
        public IEdmSingleton FindSingleton(string singletonName)
        {
            return null;
        }

        /// <summary>
        /// Searches for operation imports with the given name in this entity container and returns an empty enumerable
        /// if no such operation import exists.
        /// </summary>
        /// <param name="operationName">The name of the operation import being found.</param>
        /// <returns>A group of the requested operation imports, or an empty enumerable if no such operation import exists.</returns>
        /// <remarks>
        /// Cache state: EntityContainers required. We only support looking up an operation imports once
        ///     all entity containers have been completely populated and cached.
        /// </remarks>
        public IEnumerable<IEdmOperationImport> FindOperationImports(string operationName)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(operationName, "operationName");

            List<IEdmOperationImport> operationImports;

            // NOTE: for the query parser we might want to allow incremental lookup
            //       of operation imports in the future.
            if (this.operationImportCache.TryGetValue(operationName, out operationImports))
            {
                return operationImports.AsReadOnly();
            }

            return this.LazyLoadServiceOperationImports(operationName);
        }

        /// <summary>
        /// Load entity set from model's metadata provider.
        /// </summary>
        /// <param name="qualifiedName">The name of the entity set to be loaded.</param>
        /// <returns>Entity set that is loaded.</returns>
        internal IEdmEntitySet LazyLoadEntitySet(string qualifiedName)
        {
            string entitySetName;
            var resourceSet = this.model.MetadataProvider.TryResolveResourceSet(qualifiedName);
            if (resourceSet == null)
            {
                // key of ResourceSet cache maybe containerName.entitySetName or only entitySetName
                var resourceSetWithContainerQualifiedName = this.model.MetadataProvider.TryResolveResourceSet(this.containerName + "." + qualifiedName);
                if (resourceSetWithContainerQualifiedName == null)
                {
                    return null;
                }

                entitySetName = MetadataProviderUtils.GetEntitySetName(resourceSetWithContainerQualifiedName.ResourceSet);
                return this.AddEntitySet(entitySetName, resourceSetWithContainerQualifiedName);
            }

            entitySetName = MetadataProviderUtils.GetEntitySetName(resourceSet.ResourceSet);
            return this.AddEntitySet(entitySetName, resourceSet);
        }

        /// <summary>
        /// Load operation imports from model's metadata provider.
        /// </summary>
        /// <param name="qualifiedName">The name of the entity set to be loaded.</param>
        /// <returns>Operation imports that are loaded.</returns>
        internal List<IEdmOperationImport> LazyLoadServiceOperationImports(string qualifiedName)
        {
            List<IEdmOperationImport> operationImports = new List<IEdmOperationImport>();

            OperationWrapper operationWrapper = this.model.MetadataProvider.TryResolveServiceOperation(qualifiedName);
            if (operationWrapper != null)
            {
                IEdmOperationImport foundOperationImport = this.model.EnsureDefaultEntityContainer().EnsureOperationImport(operationWrapper);
                if (foundOperationImport != null)
                {
                    operationImports.Add(foundOperationImport);
                }
            }
            else
            {
                var operationWrapperQaulified = this.model.MetadataProvider.TryResolveServiceOperation(this.containerName + "." + qualifiedName);
                if (operationWrapperQaulified != null)
                {
                    IEdmOperationImport foundOperationImport = this.model.EnsureDefaultEntityContainer().EnsureOperationImport(operationWrapperQaulified);
                    if (foundOperationImport != null)
                    {
                        operationImports.Add(foundOperationImport);
                    }
                }
            }

            // metadata interface in addition to the action provider interface.
            if (this.model.ActionProviderWrapper != null)
            {
                bool nameIsContainerQualified;
                var operationName = this.model.MetadataProvider.GetNameFromContainerQualifiedName(qualifiedName, out nameIsContainerQualified);
                var operation = this.model.ActionProviderWrapper.TryResolveServiceAction(operationName, MetadataProviderUtils.GetResourceType((IEdmType)null));
                if (operation != null)
                {
                    // Only top level actions will have an operation import.
                    IEdmOperationImport foundOperationImport = this.model.EnsureDefaultEntityContainer().EnsureOperationImport(operation);
                    if (foundOperationImport != null)
                    {
                        operationImports.Add(foundOperationImport);
                    }
                }
            }

            return operationImports;
        }

        /// <summary>
        /// Adds an entity set backed by the <paramref name="resourceSet"/> to the entity container.
        /// </summary>
        /// <param name="entitySetName">The name of the entity set.</param>
        /// <param name="resourceSet">The resource set backing the entity set to be created.</param>
        /// <returns>an instance of IEdmEntitySet that just got added.</returns>
        /// <remarks>
        /// This method will also create the association sets and associations for the entity set.
        /// Materialization state: EntityContainers required. No change in materialization state.
        /// </remarks>
        internal IEdmEntitySet AddEntitySet(string entitySetName, ResourceSetWrapper resourceSet)
        {
            Debug.Assert(!string.IsNullOrEmpty(entitySetName), "!string.IsNullOrEmpty(entitySetName)");
            Debug.Assert(resourceSet != null, "resourceSet != null");

            IEdmEntitySet entitySet = new MetadataProviderEdmEntitySet(this.model, this, resourceSet);
            MetadataProviderUtils.ConvertCustomAnnotations(this.model, resourceSet.CustomAnnotations, entitySet);
            this.entitySetCache.Add(entitySetName, entitySet);
            return entitySet;
        }

        /// <summary>
        /// Add a function import to the entity container.
        /// </summary>
        /// <param name="serviceOperation">The service operation to add to the entity container.</param>
        /// <returns>The newly added or cached function import instance.</returns>
        internal IEdmOperationImport EnsureOperationImport(OperationWrapper serviceOperation)
        {
            Debug.Assert(serviceOperation != null, "serviceOperation != null");
            string functionImportName = serviceOperation.Name;
            Debug.Assert(!string.IsNullOrEmpty(functionImportName), "!string.IsNullOrEmpty(functionImportName)");

            List<IEdmOperationImport> operationImports;
            IEdmOperationImport operationImport = null;
            if (this.operationImportCache.TryGetValue(functionImportName, out operationImports))
            {
                operationImport = operationImports.Cast<MetadataProviderEdmOperationImport>().SingleOrDefault(f => f.ServiceOperation == serviceOperation);
            }

            if (operationImport == null)
            {
                MetadataProviderEdmOperation operation = null;

                if (serviceOperation.Kind == OperationKind.Action || serviceOperation.Method == XmlConstants.HttpMethodPost)
                {
                    operation = new MetadataProviderEdmAction(this.model, serviceOperation, this.Namespace);
                    if (serviceOperation.OperationParameterBindingKind == OperationParameterBindingKind.Never)
                    {
                        operationImport = new MetadataProviderEdmActionImport(this.model, this, (MetadataProviderEdmAction)operation);
                    }
                }
                else
                {
                    Debug.Assert(serviceOperation.Method == XmlConstants.HttpMethodGet, "Method should be a get");
                    operation = new MetadataProviderEdmFunction(this.model, serviceOperation, this.Namespace);
                    if (serviceOperation.OperationParameterBindingKind == OperationParameterBindingKind.Never)
                    {
                        operationImport = new MetadataProviderEdmFunctionImport(this.model, this, (MetadataProviderEdmFunction)operation);
                    }
                }

                if (operationImport != null)
                {
                    if (operationImports == null)
                    {
                        operationImports = new List<IEdmOperationImport>();
                        this.operationImportCache.Add(functionImportName, operationImports);
                    }

                    operationImports.Add(operationImport);
                }

                this.model.AddOperation(operation);
            }

            return operationImport;
        }
    }
}
