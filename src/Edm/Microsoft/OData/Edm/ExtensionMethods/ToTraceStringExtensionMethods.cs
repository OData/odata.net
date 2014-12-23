//   OData .NET Libraries ver. 6.9
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

using System.Linq;
using System.Text;
using Microsoft.OData.Edm.Library;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Contains ToTraceString() extension methods.
    /// </summary>
    public static class ToTraceStringExtensionMethods
    {
        /// <summary>
        /// Returns the text representation of the current object.
        /// </summary>
        /// <param name="schemaType">Reference to the calling object.</param>
        /// <returns>The text representation of the current object.</returns>
        public static string ToTraceString(this IEdmSchemaType schemaType)
        {
            return ((IEdmSchemaElement)schemaType).ToTraceString();
        }

        /// <summary>
        /// Returns the text representation of the current object.
        /// </summary>
        /// <param name="schemaElement">Reference to the calling object.</param>
        /// <returns>The text representation of the current object.</returns>
        public static string ToTraceString(this IEdmSchemaElement schemaElement)
        {
            return schemaElement.FullName();
        }

        /// <summary>
        /// Returns the text representation of the current object.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The text representation of the current object.</returns>
        public static string ToTraceString(this IEdmType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            switch (type.TypeKind)
            {
                case EdmTypeKind.Collection:
                    return ((IEdmCollectionType)type).ToTraceString();
                case EdmTypeKind.EntityReference:
                    return ((IEdmEntityReferenceType)type).ToTraceString();
                default:
                    var schemaType = type as IEdmSchemaType;
                    return schemaType != null ? schemaType.ToTraceString() : EdmConstants.Value_UnknownType;
            }
        }

        /// <summary>
        /// Returns the text representation of the current object.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The text representation of the current object.</returns>
        public static string ToTraceString(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            if (type.Definition != null)
            {
                sb.Append(type.Definition.ToTraceString());
                sb.AppendKeyValue(EdmConstants.FacetName_Nullable, type.IsNullable.ToString());
                if (type.IsPrimitive())
                {
                    sb.AppendFacets(type.AsPrimitive());
                }
            }

            sb.Append(']');
            return sb.ToString();
        }

        /// <summary>
        /// Returns the text representation of the current object.
        /// </summary>
        /// <param name="property">Reference to the calling object.</param>
        /// <returns>The text representation of the current object.</returns>
        public static string ToTraceString(this IEdmProperty property)
        {
            EdmUtil.CheckArgumentNull(property, "property");
            return (property.Name != null ? property.Name : "") + ":" + (property.Type != null ? property.Type.ToTraceString() : "");
        }

        private static string ToTraceString(this IEdmEntityReferenceType type)
        {
            return EdmTypeKind.EntityReference.ToString() + '(' + (type.EntityType != null ? type.EntityType.ToTraceString() : "") + ')';
        }

        private static string ToTraceString(this IEdmCollectionType type)
        {
            return EdmTypeKind.Collection.ToString() + '(' + (type.ElementType != null ? type.ElementType.ToTraceString() : "") + ')';
        }

        private static void AppendFacets(this StringBuilder sb, IEdmPrimitiveTypeReference type)
        {
            switch (type.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Binary:
                    sb.AppendBinaryFacets(type.AsBinary());
                    break;
                case EdmPrimitiveTypeKind.Decimal:
                    sb.AppendDecimalFacets(type.AsDecimal());
                    break;
                case EdmPrimitiveTypeKind.String:
                    sb.AppendStringFacets(type.AsString());
                    break;
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                    sb.AppendTemporalFacets(type.AsTemporal());
                    break;
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    sb.AppendSpatialFacets(type.AsSpatial());
                    break;
            }
        }

        private static void AppendBinaryFacets(this StringBuilder sb, IEdmBinaryTypeReference type)
        {
            if (type.IsUnbounded || type.MaxLength != null)
            {
                sb.AppendKeyValue(EdmConstants.FacetName_MaxLength, (type.IsUnbounded) ? EdmConstants.Value_Max : type.MaxLength.ToString());
            }
        }

        private static void AppendStringFacets(this StringBuilder sb, IEdmStringTypeReference type)
        {
            if (type.IsUnbounded == true || type.MaxLength != null)
            {
                sb.AppendKeyValue(EdmConstants.FacetName_MaxLength, (type.IsUnbounded) ? EdmConstants.Value_Max : type.MaxLength.ToString());
            }

            if (type.IsUnicode != null)
            {
                sb.AppendKeyValue(EdmConstants.FacetName_Unicode, type.IsUnicode.ToString());
            }
        }

        private static void AppendTemporalFacets(this StringBuilder sb, IEdmTemporalTypeReference type)
        {
            if (type.Precision != null)
            {
                sb.AppendKeyValue(EdmConstants.FacetName_Precision, type.Precision.ToString());
            }
        }

        private static void AppendDecimalFacets(this StringBuilder sb, IEdmDecimalTypeReference type)
        {
            if (type.Precision != null)
            {
                sb.AppendKeyValue(EdmConstants.FacetName_Precision, type.Precision.ToString());
            }

            if (type.Scale != null)
            {
                sb.AppendKeyValue(EdmConstants.FacetName_Scale, type.Scale.ToString());
            }
        }

        private static void AppendSpatialFacets(this StringBuilder sb, IEdmSpatialTypeReference type)
        {
            sb.AppendKeyValue(EdmConstants.FacetName_Srid, type.SpatialReferenceIdentifier != null ? type.SpatialReferenceIdentifier.ToString() : EdmConstants.Value_SridVariable);
        }

        private static void AppendKeyValue(this StringBuilder sb, string key, string value)
        {
            sb.Append(' ');
            sb.Append(key);
            sb.Append('=');
            sb.Append(value);
        }
    }
}
