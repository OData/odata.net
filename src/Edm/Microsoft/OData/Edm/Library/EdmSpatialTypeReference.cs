//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
