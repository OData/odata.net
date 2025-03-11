//---------------------------------------------------------------------
// <copyright file="IEdmFacetedTypeDefinition.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a definition of an EDM type definition that supports facets.
    /// </summary>
    public interface IEdmFacetedTypeDefinition : IEdmTypeDefinition
    {
        /// <summary>
        /// Gets the optional maximum length type definition facet.
        /// </summary>
        int? MaxLength { get; }

        /// <summary>
        /// Gets the optional unicode type definition facet.
        /// </summary>
        bool? IsUnicode { get; }

        /// <summary>
        /// Gets the optional precision type definition facet.
        /// </summary>
        int? Precision { get; }

        /// <summary>
        /// Gets the optional scale type definition facet.
        /// </summary>
        int? Scale { get; }

        /// <summary>
        /// Gets the optional spatial reference identifier type definition facet.
        /// </summary>
        int? Srid { get; }
    }
}
