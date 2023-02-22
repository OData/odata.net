//---------------------------------------------------------------------
// <copyright file="NormalizedModelElementsCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Cache used to store model elements using case-normalized names
    /// to speed up case-insensitive model lookups. The cache is populated
    /// up front so that if an item is not found in the cache, we can assume
    /// it doesn't exist in the model. For this reason, it's important that
    /// the model be immutable.
    /// </summary>
    internal sealed class NormalizedModelElementsCache
    {
        // We create different caches for different types of schema elements because all current usage request schema elements
        // of specific types. If we were to use a single dictionary <string, ISchemaElement> we would need
        // to do additional work (and allocations) during lookups to filter the results to the subset that matches the request type.
        private readonly Dictionary<string, List<IEdmSchemaType>> schemaTypesCache = new Dictionary<string, List<IEdmSchemaType>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<IEdmOperation>> operationsCache = new Dictionary<string, List<IEdmOperation>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<IEdmTerm>> termsCache = new Dictionary<string, List<IEdmTerm>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<IEdmNavigationSource>> navigationSourcesCache = new Dictionary<string, List<IEdmNavigationSource>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<IEdmOperationImport>> operationImportsCache = new Dictionary<string, List<IEdmOperationImport>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Builds a case-insensitive cache of schema elements from
        /// the specified <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The model whose schema elements to cache. This model should be immutable. See <see cref="ExtensionMethods.MarkAsImmutable(IEdmModel)"/>.</param>
        public NormalizedModelElementsCache(IEdmModel model)
        {
            Debug.Assert(model != null);

            PopulateContainerElements(model);

            PopulateSchemaElements(model);

            foreach (IEdmModel referencedModel in model.ReferencedModels)
            {
                PopulateSchemaElements(referencedModel);
            }
        }

        /// <summary>
        /// Find all schema types that match the <paramref name="qualifiedName"/>.
        /// </summary>
        /// <param name="qualifiedName">The case-insensitive fully qualified name to match.</param>
        /// <returns>A list of matching schema types, or null if no schema type matches.</returns>
        public List<IEdmSchemaType> FindSchemaTypes(string qualifiedName)
        {
            if (schemaTypesCache.TryGetValue(qualifiedName, out List<IEdmSchemaType> results))
            {
                return results;
            }

            return null;
        }

        /// <summary>
        /// Find all operations that match the <paramref name="qualifiedName"/>.
        /// </summary>
        /// <param name="qualifiedName">The case-insensitive fully qualified name to match.</param>
        /// <returns>A list of matching operations, or null if no operation matches.</returns>
        public List<IEdmOperation> FindOperations(string qualifiedName)
        {
            if (operationsCache.TryGetValue(qualifiedName, out List<IEdmOperation> results))
            {
                return results;
            }

            return null;
        }

        /// <summary>
        /// Find all vocabulary terms that match the <paramref name="qualifiedName"/>.
        /// </summary>
        /// <param name="qualifiedName">The case-insensitive fully qualified name to match.</param>
        /// <returns>A list of matching terms, or null if no operation matches.</returns>
        public List<IEdmTerm> FindTerms(string qualifiedName)
        {
            if (termsCache.TryGetValue(qualifiedName, out List<IEdmTerm> results))
            {
                return results;
            }

            return null;
        }

        /// <summary>
        /// Find all navigation sources that match the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The case-insensitive name to match.</param>
        /// <returns>A list of matching navigation sources, or null if no navigation source matches the name.</returns>
        public List<IEdmNavigationSource> FindNavigationSources(string name)
        {
            if (navigationSourcesCache.TryGetValue(name, out List<IEdmNavigationSource> results))
            {
                return results;
            }

            return null;
        }

        /// <summary>
        /// Find all operation imports that match the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The case-insensitive name to match.</param>
        /// <returns>A list of matching operation imports, or null if no operation import matches the name.</returns>
        public List<IEdmOperationImport> FindOperationImports(string name)
        {
            if (operationImportsCache.TryGetValue(name, out List<IEdmOperationImport> results))
            {
                return results;
            }

            return null;
        }

        private void PopulateSchemaElements(IEdmModel model)
        {
            foreach (IEdmSchemaElement element in model.SchemaElements)
            {
                if (element is IEdmSchemaType schemaType)
                {
                    AddSchemaElementToCache(schemaType, schemaTypesCache);
                }
                else if (element is IEdmOperation operation)
                {
                    AddSchemaElementToCache(operation, operationsCache);
                }
                else if (element is IEdmTerm term)
                {
                    AddSchemaElementToCache(term, termsCache);
                }
            }
        }

        private void PopulateContainerElements(IEdmModel model)
        {
            if (model.EntityContainer is null)
            {
                return;
            }

            foreach (IEdmEntityContainerElement element in model.EntityContainer.Elements)
            {
                if (element is IEdmOperationImport operationImport)
                {
                    AddContainerElementToCache(operationImport, operationImportsCache);
                }
                else if (element is IEdmNavigationSource navigationSource)
                {
                    AddContainerElementToCache(navigationSource, navigationSourcesCache);
                }
            }
        }

        private static void AddSchemaElementToCache<T>(T element, Dictionary<string, List<T>> cache) where T : IEdmSchemaElement
        {
            List<T> results;
            string normalizedKey = element.FullName();
            if (!cache.TryGetValue(normalizedKey, out results))
            {
                results = new List<T>();
                cache[normalizedKey] = results;
            }

            results.Add(element);
        }

        private static void AddContainerElementToCache<T>(T element, Dictionary<string, List<T>> cache) where T : IEdmNamedElement
        {
            List<T> results;
            string normalizedKey = element.Name;
            if (!cache.TryGetValue(normalizedKey, out results))
            {
                results = new List<T>();
                cache[normalizedKey] = results;
            }

            results.Add(element);
        }
    }
}
