//---------------------------------------------------------------------
// <copyright file="EdmSpatialTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a reference to an EDM spatial type.
    /// </summary>
    public class EdmSpatialTypeReference : EdmPrimitiveTypeReference, IEdmSpatialTypeReference
    {
        private readonly int? spatialReferenceIdentifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmSpatialTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmSpatialTypeReference(IEdmPrimitiveType definition, bool isNullable)
            : this(definition, isNullable, null)
        {
            EdmUtil.CheckArgumentNull(definition, "definition");
            switch (definition.PrimitiveKind)
            {
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                    this.spatialReferenceIdentifier = CsdlConstants.Default_SpatialGeographySrid;
                    break;
                
                // In the case of a BadSpatialTypeReference, the PrimitiveTypeKind is none, and we will treat that the same as Geometry.
                default:
                    this.spatialReferenceIdentifier = CsdlConstants.Default_SpatialGeometrySrid;
                    break;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmSpatialTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        /// <param name="spatialReferenceIdentifier">Spatial Reference Identifier for the spatial type being created.</param>
        public EdmSpatialTypeReference(IEdmPrimitiveType definition, bool isNullable, int? spatialReferenceIdentifier)
            : base(definition, isNullable)
        {
            this.spatialReferenceIdentifier = spatialReferenceIdentifier;
        }

        /// <summary>
        /// Gets the precision of this temporal type.
        /// </summary>
        public int? SpatialReferenceIdentifier
        {
            get { return this.spatialReferenceIdentifier; }
        }
    }
}
