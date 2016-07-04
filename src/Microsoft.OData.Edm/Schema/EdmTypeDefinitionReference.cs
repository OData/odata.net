//---------------------------------------------------------------------
// <copyright file="EdmTypeDefinitionReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to an EDM type definition.
    /// </summary>
    public class EdmTypeDefinitionReference : EdmTypeReference, IEdmTypeDefinitionReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTypeDefinitionReference"/> class.
        /// </summary>
        /// <param name="typeDefinition">The definition refered to by this reference.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmTypeDefinitionReference(IEdmTypeDefinition typeDefinition, bool isNullable)
            : base(typeDefinition, isNullable)
        {
            this.IsUnbounded = false;
            this.MaxLength = null;
            this.IsUnicode = ComputeDefaultIsUnicode(typeDefinition);
            this.Precision = ComputeDefaultPrecision(typeDefinition);
            this.Scale = ComputeDefaultScale(typeDefinition);
            this.SpatialReferenceIdentifier = ComputeSrid(typeDefinition);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTypeDefinitionReference"/> class.
        /// </summary>
        /// <param name="typeDefinition">The definition refered to by this reference.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        /// <param name="isUnbounded">Denotes whether the length is unbounded.</param>
        /// <param name="maxLength">Maximum length of a value of this type.</param>
        /// <param name="isUnicode">Denotes if string is encoded using Unicode.</param>
        /// <param name="precision">Precision of values with this type.</param>
        /// <param name="scale">Scale of values with this type.</param>
        /// <param name="spatialReferenceIdentifier">Spatial Reference Identifier for the spatial type being created.</param>
        public EdmTypeDefinitionReference(
            IEdmTypeDefinition typeDefinition,
            bool isNullable,
            bool isUnbounded,
            int? maxLength,
            bool? isUnicode,
            int? precision,
            int? scale,
            int? spatialReferenceIdentifier)
            : base(typeDefinition, isNullable)
        {
            this.IsUnbounded = isUnbounded;
            this.MaxLength = maxLength;
            this.IsUnicode = isUnicode;
            this.Precision = precision;
            this.Scale = scale;
            this.SpatialReferenceIdentifier = spatialReferenceIdentifier;
        }

        /// <summary>
        /// Gets a value indicating whether the length of the underlying type
        /// is unbounded where the maximum length depends on the underlying
        /// type itself and <see cref="MaxLength"/> is invalid.
        /// This facet ONLY applies when the underlying type is Edm.Binary,
        /// Edm.Stream or Edm.String.
        /// </summary>
        public bool IsUnbounded { get; private set; }

        /// <summary>
        /// Gets the maximum length of the underlying type. This value is only
        /// effective when <see cref="IsUnbounded"/> is false.
        /// This facet ONLY applies when the underlying type is Edm.Binary,
        /// Edm.Stream or Edm.String.
        /// </summary>
        public int? MaxLength { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the underlying type supports unicode encoding.
        /// This facet ONLY applies when the underlying type is Edm.String.
        /// </summary>
        public bool? IsUnicode { get; private set; }

        /// <summary>
        /// Gets the precision of the underlying type.
        /// This facet ONLY applies when the underlying type is Edm.DateTimeOffset,
        /// Edm.Decimal, Edm.Duration or Edm.TimeOfDay.
        /// </summary>
        public int? Precision { get; private set; }

        /// <summary>
        /// Gets the scale of the underlying type.
        /// This facet ONLY applies when the underlying type is Edm.Decimal.
        /// </summary>
        public int? Scale { get; private set; }

        /// <summary>
        /// Gets the Spatial Reference Identifier of the underlying type.
        /// This facet ONLY applies when the underlying type is a spatial type.
        /// </summary>
        public int? SpatialReferenceIdentifier { get; private set; }

        private static bool? ComputeDefaultIsUnicode(IEdmTypeDefinition typeDefinition)
        {
            if (typeDefinition.UnderlyingType.IsString())
            {
                return true;
            }

            return null;
        }

        private static int? ComputeDefaultPrecision(IEdmTypeDefinition typeDefinition)
        {
            if (typeDefinition.UnderlyingType.IsTemporal())
            {
                return CsdlConstants.Default_TemporalPrecision;
            }

            return null;
        }

        private static int? ComputeDefaultScale(IEdmTypeDefinition typeDefinition)
        {
            if (typeDefinition.UnderlyingType.IsDecimal())
            {
                return CsdlConstants.Default_Scale;
            }

            return null;
        }

        private static int? ComputeSrid(IEdmTypeDefinition typeDefinition)
        {
            if (typeDefinition.UnderlyingType.IsGeography())
            {
                return CsdlConstants.Default_SpatialGeographySrid;
            }

            if (typeDefinition.UnderlyingType.IsGeometry())
            {
                return CsdlConstants.Default_SpatialGeometrySrid;
            }

            return null;
        }
    }
}
