//---------------------------------------------------------------------
// <copyright file="CsdlJsonParseHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETSTANDARD2_0
using System;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
    /// <summary>
    /// Provides functionalities for parsing Schema JSON to Csdl elements.
    /// </summary>
    internal static class CsdlJsonParseHelper
    {
        public static CsdlTypeReference ParseCsdlTypeReference(JsonElement element, JsonParserContext context)
        {
            Debug.Assert(element.ValueKind == JsonValueKind.Object);

            // $Type
            // For single-valued terms the value of $Type is the qualified name of the property/term’s type.
            // For collection-valued terms the value of $Type is the qualified name of the term’s item type, and the member $Collection MUST be present with the literal value true.
            // Absence of the $Type member means the type is Edm.String.
            string typeName = element.ParseOptionalProperty("$Type", context, (v, p) => v.ParseAsString(p));
            typeName = typeName == null ? "Edm.String" : typeName;

            // $Collection
            // For collection-valued properties the value of $Type is the qualified name of the property’s item type,
            // and the member $Collection MUST be present with the literal value true.
            bool? collection = element.ParseOptionalProperty("$Collection", context, (v, p) => v.ParseAsBoolean(p));

            // $Nullable,
            // The value of $Nullable is one of the Boolean literals true or false. Absence of the member means false.
            bool? isNullable = element.ParseOptionalProperty("$Nullable", context, (v, p) => v.ParseAsBoolean(p));
            bool nullable = isNullable == null ? false : isNullable.Value;

            // $MaxLength,
            // The value of $MaxLength is a positive integer.
            // CSDL xml defines a symbolic value max that is only allowed in OData 4.0 responses. This symbolic value is not allowed in CDSL JSON documents at all.
            int? maxLength = element.ParseOptionalProperty("$MaxLength", context, (v, p) => v.ParseAsInteger(p));

            // $Unicode,
            // The value of $Unicode is one of the Boolean literals true or false. Absence of the member means true.
            bool? unicode = element.ParseOptionalProperty("$Unicode", context, (v, p) => v.ParseAsBoolean(p));

            // $Precision,
            // The value of $Precision is a number.
            int? precision = element.ParseOptionalProperty("$Precision", context, (v, p) => v.ParseAsInteger(p));

            // $Scale,
            int? scale = element.ParseOptionalProperty("$Scale", context, (v, p) => v.ParseAsInteger(p));

            // $SRID,
            // The value of $SRID is a string containing a number or the symbolic value variable.
            // So far, ODL doesn't support string of SRID.
            int? srid = element.ParseOptionalProperty("$SRID", context, (v, p) => v.ParseAsInteger(p));

            CsdlLocation location = context.Location();
            CsdlTypeReference csdlType = ParseNamedTypeReference(typeName, nullable, false, maxLength, unicode, precision, scale, srid, location);
            if (collection != null && collection.Value)
            {
                csdlType = new CsdlExpressionTypeReference(new CsdlCollectionType(csdlType, location), nullable, location);
            }

            return csdlType;
        }

        private static CsdlNamedTypeReference ParseNamedTypeReference(string typeName, bool isNullable,
             bool isUnbounded,
             int? maxLength,
             bool? unicode,
             int? precision,
             int? scale,
             int? srid,
             CsdlLocation parentLocation)
        {
            EdmPrimitiveTypeKind kind = EdmCoreModel.Instance.GetPrimitiveTypeKind(typeName);
            switch (kind)
            {
                case EdmPrimitiveTypeKind.Boolean:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.Double:
                case EdmPrimitiveTypeKind.Guid:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.Stream:
                case EdmPrimitiveTypeKind.Date:
                case EdmPrimitiveTypeKind.PrimitiveType:
                    return new CsdlPrimitiveTypeReference(kind, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.Binary:
                    return new CsdlBinaryTypeReference(isUnbounded, maxLength, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.TimeOfDay:
                    return new CsdlTemporalTypeReference(kind, precision, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.Decimal:
                    return new CsdlDecimalTypeReference(precision, scale, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.String:
                    return new CsdlStringTypeReference(isUnbounded, maxLength, unicode, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                    return new CsdlSpatialTypeReference(kind, srid, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    return new CsdlSpatialTypeReference(kind, srid, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.None:
                    if (string.Equals(typeName, CsdlConstants.TypeName_Untyped, StringComparison.Ordinal))
                    {
                        return new CsdlUntypedTypeReference(typeName, parentLocation);
                    }

                    break;
            }

            return new CsdlNamedTypeReference(isUnbounded, maxLength, unicode, precision, scale, srid, typeName, isNullable, parentLocation);
        }
    }
}
#endif