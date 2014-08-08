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

namespace System.Data.Services.Providers
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
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

        /// <summary>The cache of function imports.</summary>
        private readonly Dictionary<string, List<IEdmFunctionImport>> functionImportCache;

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
            this.functionImportCache = new Dictionary<string, List<IEdmFunctionImport>>(StringComparer.Ordinal);
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

                foreach (IEdmFunctionImport functionImport in this.functionImportCache.Values.SelectMany(v => v))
                {
                    yield return functionImport;
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
            return this.entitySetCache.TryGetValue(name, out entitySet) ? entitySet : null;
        }

        /// <summary>
        /// Searches for function imports with the given name in this entity container and returns an empty enumerable
        /// if no such function import exists.
        /// </summary>
        /// <param name="name">The name of the function import being found.</param>
        /// <returns>A group of the requested function imports, or an empty enumerable if no such function import exists.</returns>
        /// <remarks>
        /// Cache state: EntityContainers required. We only support looking up an function imports once
        ///     all entity containers have been completely populated and cached.
        /// </remarks>
        public IEnumerable<IEdmFunctionImport> FindFunctionImports(string name)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(name, "name");

            List<IEdmFunctionImport> functionImports;

            // NOTE: for the query parser we might want to allow incremental lookup
            //       of function imports in the future.
            return this.functionImportCache.TryGetValue(name, out functionImports) ? functionImports.AsReadOnly() : Enumerable.Empty<IEdmFunctionImport>();
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
        internal IEdmFunctionImport EnsureFunctionImport(OperationWrapper serviceOperation)
        {
            Debug.Assert(serviceOperation != null, "serviceOperation != null");
            string functionImportName = serviceOperation.Name;
            Debug.Assert(!string.IsNullOrEmpty(functionImportName), "!string.IsNullOrEmpty(functionImportName)");

            List<IEdmFunctionImport> functionImports;
            IEdmFunctionImport functionImport = null;
            if (!this.functionImportCache.TryGetValue(functionImportName, out functionImports))
            {
                functionImports = new List<IEdmFunctionImport>();
                this.functionImportCache.Add(functionImportName, functionImports);
            }
            else
            {
                functionImport = functionImports.Cast<MetadataProviderEdmFunctionImport>().SingleOrDefault(f => f.ServiceOperation == serviceOperation);
            }

            if (functionImport == null)
            {
                functionImport = new MetadataProviderEdmFunctionImport(this.model, this, serviceOperation);
                functionImports.Add(functionImport);
            }
            
            return functionImport;
        }
    }
}
