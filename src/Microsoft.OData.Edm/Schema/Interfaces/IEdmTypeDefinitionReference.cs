//---------------------------------------------------------------------
// <copyright file="IEdmTypeDefinitionReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents references to EDM type definitions.
    /// </summary>
    public interface IEdmTypeDefinitionReference : IEdmTypeReference
    {
        /// <summary>
        /// Gets a value indicating whether the length of the underlying type
        /// is unbounded where the maximum length depends on the underlying
        /// type itself and <see cref="MaxLength"/> is invalid.
        /// This facet ONLY applies when the underlying type is Edm.Binary,
        /// Edm.Stream or Edm.String.
        /// </summary>
        bool IsUnbounded { get; }

        /// <summary>
        /// Gets the maximum length of the underlying type. This value is only
        /// effective when <see cref="IsUnbounded"/> is false.
        /// This facet ONLY applies when the underlying type is Edm.Binary,
        /// Edm.Stream or Edm.String.
        /// </summary>
        int? MaxLength { get; }

        /// <summary>
        /// Gets a value indicating whether the underlying type supports unicode encoding.
        /// This facet ONLY applies when the underlying type is Edm.String.
        /// </summary>
        bool? IsUnicode { get; }

        /// <summary>
        /// Gets the precision of the underlying type.
        /// This facet ONLY applies when the underlying type is Edm.DateTimeOffset,
        /// Edm.Decimal, Edm.Duration or Edm.TimeOfDay.
        /// </summary>
        int? Precision { get; }

        /// <summary>
        /// Gets the scale of the underlying type.
        /// This facet ONLY applies when the underlying type is Edm.Decimal.
        /// </summary>
        int? Scale { get; }

        /// <summary>
        /// Gets the Spatial Reference Identifier of the underlying type.
        /// This facet ONLY applies when the underlying type is a spatial type.
        /// </summary>
        int? SpatialReferenceIdentifier { get; }
    }
}
