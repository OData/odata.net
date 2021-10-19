//---------------------------------------------------------------------
// <copyright file="UnqualifiedODataUriResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Resolver that supports bound function calls.
    /// </summary>
    public class UnqualifiedODataUriResolver : ODataUriResolver
    {
        private static readonly ConcurrentDictionary<IEdmModel, ConcurrentDictionary<string, IEnumerable<IEdmSchemaElement>>> schemaElementCache = new ConcurrentDictionary<IEdmModel, ConcurrentDictionary<string, IEnumerable<IEdmSchemaElement>>>();

        /// <summary>
        /// Resolve unbound operations based on name.
        /// </summary>
        /// <param name="model">The model to be used.</param>
        /// <param name="identifier">The operation name.</param>
        /// <returns>Resolved operation list.</returns>
        public override IEnumerable<IEdmOperation> ResolveUnboundOperations(IEdmModel model, string identifier)
        {
            if (identifier.Contains("."))
            {
                return base.ResolveUnboundOperations(model, identifier);
            }

            return FindAcrossModels<IEdmOperation>(model, identifier, this.EnableCaseInsensitive)
                    .Where(operation => !operation.IsBound);
        }

        /// <summary>
        /// Resolve bound operations based on name.
        /// </summary>
        /// <param name="model">The model to be used.</param>
        /// <param name="identifier">The operation name.</param>
        /// <param name="bindingType">The type operation was binding to.</param>
        /// <returns>Resolved operation list.</returns>
        public override IEnumerable<IEdmOperation> ResolveBoundOperations(IEdmModel model, string identifier, IEdmType bindingType)
        {
            if (identifier.Contains("."))
            {
                return base.ResolveBoundOperations(model, identifier, bindingType);
            }

            return FindAcrossModels<IEdmOperation>(model, identifier, this.EnableCaseInsensitive)
                .Where(operation =>
                    operation.IsBound
                    && operation.Parameters.Any()
                    && operation.HasEquivalentBindingType(bindingType));
        }
               
        private static IEnumerable<T> FindAcrossModels<T>(IEdmModel model, String qualifiedName, bool caseInsensitive) where T : IEdmSchemaElement
        {
            IList<T> results = new List<T>();

            ConcurrentDictionary<string, IEnumerable<IEdmSchemaElement>> nameDict;
            IEnumerable<IEdmSchemaElement> nameResults;

            if (schemaElementCache.TryGetValue(model, out nameDict) )
            {
                if (nameDict.TryGetValue(qualifiedName, out nameResults))
                {
                    return nameResults as IList<T>;
                }                
            }
            else
            {
                schemaElementCache.TryAdd(model, new ConcurrentDictionary<string, IEnumerable<IEdmSchemaElement>>());
            }
            
            StringComparison strComparison = caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            GetSchemaElements(model, qualifiedName, results, strComparison);

            foreach (IEdmModel reference in model.ReferencedModels)
            {
                GetSchemaElements(reference, qualifiedName, results, strComparison);
            }

            schemaElementCache[model].TryAdd(qualifiedName, results as IEnumerable<IEdmSchemaElement>);

            return results;
        }

        private static void GetSchemaElements<T>(IEdmModel model, string qualifiedName, IList<T> results, StringComparison strComparison) where T : IEdmSchemaElement
        {
            foreach (IEdmSchemaElement edmSchemaElement in model.SchemaElements)
            {
                if (string.Equals(qualifiedName, edmSchemaElement.Name, strComparison) && edmSchemaElement is T schemaElement)
                {
                    results.Add(schemaElement);
                }
            }
        }
    }
}