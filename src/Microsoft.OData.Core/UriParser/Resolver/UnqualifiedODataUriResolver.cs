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
        /// <summary>
        /// Resolve unbound operations based on name.
        /// </summary>
        /// <param name="model">The model to be used.</param>
        /// <param name="identifier">The operation name.</param>
        /// <returns>Resolved operation list.</returns>
        public override IEnumerable<IEdmOperation> ResolveUnboundOperations(IEdmModel model, string identifier)
        {
            if (identifier.Contains(".", StringComparison.Ordinal))
            {
                return base.ResolveUnboundOperations(model, identifier);
            }

            return GetUnBoundOperationsForModel(model, identifier, this.EnableCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
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
            if (identifier.Contains(".", StringComparison.Ordinal))
            {
                return base.ResolveBoundOperations(model, identifier, bindingType);
            }
          
            return GetBoundOperationsForModel(model, identifier, bindingType, this.EnableCaseInsensitive? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        private static IEnumerable<IEdmOperation> GetUnBoundOperationsForModel(IEdmModel model, string operationName, StringComparison strComparison)
        {
            foreach (IEdmSchemaElement schemaElement in model.SchemaElements)
            {
                if (schemaElement is IEdmOperation operation && !operation.IsBound && string.Equals(operationName, operation.Name, strComparison))
                {
                    yield return operation;
                }
            }

            foreach(IEdmModel reference in model.ReferencedModels)
            {
                foreach (IEdmSchemaElement schemaElement in reference.SchemaElements)
                {
                    if (schemaElement is IEdmOperation operation && !operation.IsBound && string.Equals(operationName, operation.Name, strComparison))
                    {
                        yield return operation;
                    }
                }
            }
        }

        private static IEnumerable<IEdmOperation> GetBoundOperationsForModel(IEdmModel model, string operationName, IEdmType bindingType, StringComparison strComparison)
        {
            foreach (IEdmOperation operation in model.FindBoundOperations(bindingType))
            {
                if (string.Equals(operationName, operation.Name, strComparison))
                {
                    yield return operation;
                }
            }
        }

    }
}