//---------------------------------------------------------------------
// <copyright file="EdmToTaupoPrimitiveDataTypeConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Edmlib
{
    using System.Collections.Generic;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Converts primitive data type from Edm term into Taupo term
    /// </summary>
    public static class EdmToTaupoPrimitiveDataTypeConverter
    {
        /// <summary>
        /// The default value that the Taupo parser/serializer uses for MaxLength='Max'
        /// </summary>
        public static readonly int MaxLengthMaxTaupoDefaultValue = -1;

        private static Dictionary<EdmPrimitiveTypeKind, PrimitiveDataType> facetlessDataTypeLookup = new Dictionary<EdmPrimitiveTypeKind, PrimitiveDataType>
        {
            { EdmPrimitiveTypeKind.Boolean, EdmDataTypes.Boolean },
            { EdmPrimitiveTypeKind.Byte, EdmDataTypes.Byte },
            { EdmPrimitiveTypeKind.Double, EdmDataTypes.Double },
            { EdmPrimitiveTypeKind.Guid, EdmDataTypes.Guid },
            { EdmPrimitiveTypeKind.Int16, EdmDataTypes.Int16 },
            { EdmPrimitiveTypeKind.Int32, EdmDataTypes.Int32 },
            { EdmPrimitiveTypeKind.Int64, EdmDataTypes.Int64 },
            { EdmPrimitiveTypeKind.SByte, EdmDataTypes.SByte },
            { EdmPrimitiveTypeKind.Single, EdmDataTypes.Single },
            { EdmPrimitiveTypeKind.Stream, EdmDataTypes.Stream },
            { EdmPrimitiveTypeKind.Geography, EdmDataTypes.Geography },
            { EdmPrimitiveTypeKind.GeographyPoint, EdmDataTypes.GeographyPoint },
            { EdmPrimitiveTypeKind.GeographyLineString, EdmDataTypes.GeographyLineString },
            { EdmPrimitiveTypeKind.GeographyPolygon, EdmDataTypes.GeographyPolygon },
            { EdmPrimitiveTypeKind.GeographyCollection, EdmDataTypes.GeographyCollection },
            { EdmPrimitiveTypeKind.GeographyMultiPolygon, EdmDataTypes.GeographyMultiPolygon },
            { EdmPrimitiveTypeKind.GeographyMultiLineString, EdmDataTypes.GeographyMultiLineString },
            { EdmPrimitiveTypeKind.GeographyMultiPoint, EdmDataTypes.GeographyMultiPoint },
            { EdmPrimitiveTypeKind.Geometry, EdmDataTypes.Geometry },
            { EdmPrimitiveTypeKind.GeometryPoint, EdmDataTypes.GeometryPoint },
            { EdmPrimitiveTypeKind.GeometryLineString, EdmDataTypes.GeometryLineString },
            { EdmPrimitiveTypeKind.GeometryPolygon, EdmDataTypes.GeometryPolygon },
            { EdmPrimitiveTypeKind.GeometryCollection, EdmDataTypes.GeometryCollection },
            { EdmPrimitiveTypeKind.GeometryMultiPolygon, EdmDataTypes.GeometryMultiPolygon },
            { EdmPrimitiveTypeKind.GeometryMultiLineString, EdmDataTypes.GeometryMultiLineString },
            { EdmPrimitiveTypeKind.GeometryMultiPoint, EdmDataTypes.GeometryMultiPoint },
        };

        /// <summary>
        /// Converts primitive data type from Edm term into Taupo term
        /// </summary>
        /// <param name="edmPrimitiveTypeReference">The TypeReference (Edm term)</param>
        /// <returns>The DataType (Taupo term)</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "maxLength", Justification = "variable name")]
        public static DataType ConvertToTaupoPrimitiveDataType(IEdmPrimitiveTypeReference edmPrimitiveTypeReference)
        {
            PrimitiveDataType result = null;
            EdmPrimitiveTypeKind primitiveKind = edmPrimitiveTypeReference.PrimitiveKind();
            if (!facetlessDataTypeLookup.TryGetValue(primitiveKind, out result))
            {
                switch (primitiveKind)
                {
                    case EdmPrimitiveTypeKind.Binary:
                        var edmBinary = edmPrimitiveTypeReference.AsBinary();
                        result = EdmDataTypes.Binary(edmBinary.MaxLength);
                        break;

                    case EdmPrimitiveTypeKind.DateTimeOffset:
                        var edmDateTimeOffset = edmPrimitiveTypeReference.AsTemporal();
                        result = EdmDataTypes.DateTimeOffset(edmDateTimeOffset.Precision);
                        break;

                    case EdmPrimitiveTypeKind.Decimal:
                        var edmDecimal = edmPrimitiveTypeReference.AsDecimal();
                        result = EdmDataTypes.Decimal(edmDecimal.Precision, edmDecimal.Scale);
                        break;

                    case EdmPrimitiveTypeKind.String:
                        var edmString = edmPrimitiveTypeReference.AsString();
                        var maxLength = edmString.MaxLength;
                        if (edmString.IsUnbounded == true)
                        {
                            maxLength = MaxLengthMaxTaupoDefaultValue; 
                        }

                        result = EdmDataTypes.String(maxLength, edmString.IsUnicode);
                        break;

                    case EdmPrimitiveTypeKind.Duration:
                        var edmTime = edmPrimitiveTypeReference.AsTemporal();
                        result = EdmDataTypes.Time(edmTime.Precision);
                        break;

                    default:
                        throw new TaupoInvalidOperationException("unexpected Edm Primitive Type Kind: " + primitiveKind);
                }
            }

            return result.Nullable(edmPrimitiveTypeReference.IsNullable);
        }
    }
}
