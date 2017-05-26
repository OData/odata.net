//---------------------------------------------------------------------
// <copyright file="UnqualifiedODataUriResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
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
            if (identifier.Contains("."))
            {
                return base.ResolveUnboundOperations(model, identifier);
            }

            return model.SchemaElements.OfType<IEdmOperation>()
                    .Where(operation => string.Equals(
                            identifier,
                            operation.Name,
                            this.EnableCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)
                    && !operation.IsBound);
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

            return model.SchemaElements.OfType<IEdmOperation>()
                .Where(operation => string.Equals(
                        identifier,
                        operation.Name,
                        this.EnableCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)
                    && operation.IsBound && operation.Parameters.Any()
                    && operation.HasEquivalentBindingType(bindingType));
        }
    }
}