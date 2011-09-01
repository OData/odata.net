//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Microsoft.Data.Edm.Csdl;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a reference to an EDM spatial type.
    /// </summary>
    public class EdmSpatialTypeReference : EdmPrimitiveTypeReference, IEdmSpatialTypeReference
    {
        private readonly int?spatialReferenceIdentifier;

        /// <summary>
        /// Initializes a new instance of the EdmSpatialTypeReference class.
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
                case EdmPrimitiveTypeKind.Point:
                case EdmPrimitiveTypeKind.LineString:
                case EdmPrimitiveTypeKind.Polygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.MultiPolygon:
                case EdmPrimitiveTypeKind.MultiLineString:
                case EdmPrimitiveTypeKind.MultiPoint:
                    this.spatialReferenceIdentifier = CsdlConstants.Default_SpatialGeographySrid;
                    break;
                // In the case of a BadSpatialTypeReference, the PrimitiveTypeKind is none, and we will treat that the same as Geometry.
                default:
                    this.spatialReferenceIdentifier = CsdlConstants.Default_SpatialGeometrySrid;
                    break;
            }
        }

        /// <summary>
        /// Initializes a new instance of the EdmSpatialTypeReference class.
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
