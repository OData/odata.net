//---------------------------------------------------------------------
// <copyright file="CaseInsensitiveSchemaElementsCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Cache used to store schema elements using case-normalized names
    /// to speed up case-insensitive model lookups. The cache is populated
    /// up front so that if an item is not found in the cache, we can assume
    /// it doesn't exist in the model. For this reason, it's important that
    /// the model be immutable.
    /// </summary>
    internal sealed class CaseInsensitiveSchemaElementsCache
    {
        private static readonly List<IEdmSchemaElement> emptyList = new List<IEdmSchemaElement>();
        // This cache is meant to be populate up front and remain read-only
        // after that. Therefore, it doesn't need
        // to be a concurrent dictionary.
        private readonly Dictionary<string, List<IEdmSchemaElement>> cache = new Dictionary<string, List<IEdmSchemaElement>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<IEdmSchemaType>> schemaTypesCache = new Dictionary<string, List<IEdmSchemaType>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<IEdmOperation>> operationsCache = new Dictionary<string, List<IEdmOperation>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<IEdmTerm>> termsCache = new Dictionary<string, List<IEdmTerm>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Builds a case-insensitive cache of schema elements from
        /// the specified <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The model whose schema elements to cache. This model should be immutable. See <see cref="ExtensionMethods.MarkAsImmutable(IEdmModel)"/>.</param>
        public CaseInsensitiveSchemaElementsCache(IEdmModel model)
        {
            //Dictionary<string, List<IEdmSchemaElement>> cache = new Dictionary<string, List<IEdmSchemaElement>>(StringComparer.OrdinalIgnoreCase);

            PopulateSchemaElements(model);

            foreach (IEdmModel referencedModel in model.ReferencedModels)
            {
                PopulateSchemaElements(referencedModel);
            }

            //this.cache = cache;
        }

        /// <summary>
        /// Find all schema elements that match the specified <paramref name="qualifiedName"/>.
        /// </summary>
        /// <param name="qualifiedName">The case-insensitive fully qualified name to match.</param>
        /// <returns>The list of elements that matched the <paramref name="qualifiedName"/>.</returns>
        public IReadOnlyList<IEdmSchemaElement> FindElements(string qualifiedName)
        {
            if (cache.TryGetValue(qualifiedName, out List<IEdmSchemaElement> results))
            {
                return results;
            }

            return emptyList;
        }

        /// <summary>
        /// Find all schema elements of type <typeparamref name="T"/> that match the specified <paramref name="qualifiedName"/>.
        /// </summary>
        /// <typeparam name="T">The type of schema elements to match.</typeparam>
        /// <param name="qualifiedName">The case-insensitive fully qualified name to match.</param>
        /// <returns>The list of elements of type <typeparamref name="T"/> that matched the <paramref name="qualifiedName"/>.</returns>
        public IReadOnlyList<T> FindElementsOfType<T>(string qualifiedName) where T : IEdmSchemaElement
        {
            IReadOnlyList<IEdmSchemaElement> elements = FindElements(qualifiedName);

            List<T> results = new List<T>();
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] is T element)
                {
                    results.Add(element);
                }
            }

            return results;
        }

        /// <summary>
        /// Find a unique element of type <typeparamref name="T"/> that matches the <paramref name="qualifiedName"/>.
        /// And exception is thrown if duplicates are found.
        /// </summary>
        /// <typeparam name="T">The type of element to match.</typeparam>
        /// <param name="qualifiedName">The case-insensitive fully qualified name to match.</param>
        /// <param name="duplicateErrorFunc">A function that generates an error message if a duplicate is found.</param>
        /// <returns>The element that was found, or `null` if no match was found.</returns>
        /// <exception cref="ODataException">Thrown if duplicate matches were found.</exception>
        public T FindSingleOfType<T>(string qualifiedName, Func<object, string> duplicateErrorFunc) where T : IEdmSchemaElement
        {
            IReadOnlyList<IEdmSchemaElement> elements = FindElements(qualifiedName);

            if (elements.Count == 0)
            {
                return default;
            }

            T match = default;
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] is T element)
                {
                    if (match == null)
                    {
                        match = element;
                    }
                    else
                    {
                        throw new ODataException(duplicateErrorFunc(qualifiedName));
                    }
                }
            }

            return match;
        }
        
        private static void PopulateSchemaElements(IEdmModel model, Dictionary<string, List<IEdmSchemaElement>> cache)
        {
            foreach (IEdmSchemaElement element in model.SchemaElements)
            {
                string normalizedKey = element.FullName();
                List<IEdmSchemaElement> results;
                if (!cache.TryGetValue(normalizedKey, out results))
                {
                    results = new List<IEdmSchemaElement>();
                    cache[normalizedKey] = results;
                }

                results.Add(element);
            }
        }

        private void PopulateSchemaElements(IEdmModel model)
        {
            foreach (IEdmSchemaElement element in model.SchemaElements)
            {
                if (element is IEdmSchemaType schemaType)
                {
                    AddElementToCache(schemaType, schemaTypesCache);
                }
                else if (element is IEdmOperation operation)
                {
                    AddElementToCache(operation, operationsCache);
                }
                else if (element is IEdmTerm term)
                {
                    AddElementToCache(term, termsCache);
                }
                else
                {
                    AddElementToCache(element, cache);
                }
            }
        }

        private static void AddElementToCache<T>(T element, Dictionary<string, List<T>> cache) where T : IEdmSchemaElement
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

        public List<IEdmSchemaType> FindSchemaTypes(string qualifiedName)
        {
            if (schemaTypesCache.TryGetValue(qualifiedName, out var results))
            {
                return results;
            }

            return null;
        }

        public List<IEdmOperation> FindOperations(string qualifiedName)
        {
            if (operationsCache.TryGetValue(qualifiedName, out var results))
            {
                return results;
            }

            return null;
        }
        public List<IEdmTerm> FindTerms(string qualifiedName)
        {
            if (termsCache.TryGetValue(qualifiedName, out var results))
            {
                return results;
            }

            return null;
        }

    }
}
