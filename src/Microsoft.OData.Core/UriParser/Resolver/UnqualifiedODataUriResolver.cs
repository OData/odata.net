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
        private static readonly ConcurrentDictionary<IEdmModel, ConcurrentDictionary<string, IList<IEdmOperation>>> operationCache = new ConcurrentDictionary<IEdmModel, ConcurrentDictionary<string, IList<IEdmOperation>>>();

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

            return FindOperations(model, identifier, this.EnableCaseInsensitive);
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

            return FindOperations(model, identifier, this.EnableCaseInsensitive, true, bindingType);
        }

        private static IEnumerable<IEdmOperation> FindOperations(IEdmModel model, string qualifiedName, bool caseInsensitive, bool isBound = false, IEdmType bindingType = null)
        {
            IList<IEdmOperation> results;
            ConcurrentDictionary<string, IList<IEdmOperation>> nameDict;

            if (!operationCache.TryGetValue(model, out nameDict))
            {
                nameDict = new ConcurrentDictionary<string, IList<IEdmOperation>>();
                operationCache.TryAdd(model, nameDict);
            }

            //Caching safely assuming, case sensitivity for all request will be the same
            if (!nameDict.TryGetValue(qualifiedName, out results))
            {
                results = new List<IEdmOperation>();

                StringComparison strComparison = caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

                GetOperationsForModel(model, qualifiedName, isBound, bindingType, results, strComparison);

                foreach (IEdmModel reference in model.ReferencedModels)
                {
                    GetOperationsForModel(reference, qualifiedName, isBound, bindingType, results, strComparison);
                }

                nameDict.TryAdd(qualifiedName, results);
                operationCache[model] = nameDict;
            }
           
            return results;
        }

        private static void GetOperationsForModel(IEdmModel model, string qualifiedName, bool isBound, IEdmType bindingType, IList<IEdmOperation> results, StringComparison strComparison)
        {
            foreach (IEdmSchemaElement schemaElement in model.SchemaElements)
            {
                if (schemaElement is IEdmOperation operation)
                {
                    if (string.Equals(qualifiedName, operation.Name, strComparison) && ((!isBound && !operation.IsBound) ||
                        (isBound && operation.IsBound && operation.Parameters.Any() && operation.HasEquivalentBindingType(bindingType))))
                    {
                        results.Add(operation);
                    }
                }
            }
        }

    }
}