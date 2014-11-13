//   OData .NET Libraries ver. 6.8.1
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
