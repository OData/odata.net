//---------------------------------------------------------------------
// <copyright file="FunctionSignature.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Class representing a function signature using EDM types.
    /// </summary>
    internal sealed class FunctionSignature
    {
        /// <summary>
        /// The argument types for this function signature.
        /// </summary>
        private readonly IEdmTypeReference[] argumentTypes;

        /// <summary>
        /// Factories for creating argument types with proper facets.
        /// </summary>
        private CreateArgumentTypeWithFacets[] createArgumentTypesWithFacets;

        /// <summary>
        /// Constructor taking all the argument types, and the factories for creating argument types with proper facets.
        /// </summary>
        /// <param name="argumentTypes">The argument types for this function signature.</param>
        /// <param name="createArgumentTypesWithFacets">Factories for creating argument types with proper facets.</param>
        internal FunctionSignature(
            IEdmTypeReference[] argumentTypes,
            CreateArgumentTypeWithFacets[] createArgumentTypesWithFacets)
        {
            this.argumentTypes = argumentTypes;
            this.createArgumentTypesWithFacets = createArgumentTypesWithFacets;
        }

        /// <summary>
        /// Delegate for creating an argument type with specified facets.
        /// </summary>
        /// <param name="precision">The precision facet.</param>
        /// <param name="scale">The scale facet.</param>
        /// <returns>An argument type with specified facets.</returns>
        internal delegate IEdmTypeReference CreateArgumentTypeWithFacets(int? precision, int? scale);

        /// <summary>
        /// The argument types for this function signature.
        /// </summary>
        internal IEdmTypeReference[] ArgumentTypes
        {
            get
            {
                return this.argumentTypes;
            }
        }

        /// <summary>
        /// Gets the type with specified facets for the index-th argument.
        /// </summary>
        /// <param name="index">Index of the argument for which to get the type for.</param>
        /// <param name="precision">The precision facet.</param>
        /// <param name="scale">The scale facet.</param>
        /// <returns>The type with specified facets for the index-th argument.</returns>
        internal IEdmTypeReference GetArgumentTypeWithFacets(int index, int? precision, int? scale)
        {
            if (createArgumentTypesWithFacets == null)
            {
                return argumentTypes[index];
            }

            var create = createArgumentTypesWithFacets[index];
            return create != null ? create(precision, scale) : argumentTypes[index];
        }
    }
}
