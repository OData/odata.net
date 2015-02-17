//---------------------------------------------------------------------
// <copyright file="EdmConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    /// <summary>
    /// Constant values used in the EDM.
    /// </summary>
    public static class EdmConstants
    {
        /// <summary>Fixed EDMX version number to use when writing metadata.</summary>
        internal const double EdmxVersion = 4.0;

        #region Edm Primitive Type Names -----------------------------------------------------------------------------/

        /// <summary>namespace for edm primitive types.</summary>
        public const string EdmNamespace = "Edm";

        /// <summary>edm binary primitive type name</summary>
        public const string EdmBinaryTypeName = "Edm.Binary";

        /// <summary>edm boolean primitive type name</summary>
        public const string EdmBooleanTypeName = "Edm.Boolean";

        /// <summary>edm byte primitive type name</summary>
        public const string EdmByteTypeName = "Edm.Byte";

        /// <summary>edm datetime primitive type name</summary>
        public const string EdmDateTimeTypeName = "Edm.DateTime";

        /// <summary>Represents a Time instance as an interval measured in milliseconds from an instance of DateTime.</summary>
        public const string EdmDateTimeOffsetTypeName = "Edm.DateTimeOffset";

        /// <summary>edm decimal primitive type name</summary>
        public const string EdmDecimalTypeName = "Edm.Decimal";

        /// <summary>edm double primitive type name</summary>
        public const string EdmDoubleTypeName = "Edm.Double";

        /// <summary>edm guid primitive type name</summary>
        public const string EdmGuidTypeName = "Edm.Guid";

        /// <summary>edm single primitive type name</summary>
        public const string EdmSingleTypeName = "Edm.Single";

        /// <summary>edm sbyte primitive type name</summary>
        public const string EdmSByteTypeName = "Edm.SByte";

        /// <summary>edm int16 primitive type name</summary>
        public const string EdmInt16TypeName = "Edm.Int16";

        /// <summary>edm int32 primitive type name</summary>
        public const string EdmInt32TypeName = "Edm.Int32";

        /// <summary>edm int64 primitive type name</summary>
        public const string EdmInt64TypeName = "Edm.Int64";

        /// <summary>edm string primitive type name</summary>
        public const string EdmStringTypeName = "Edm.String";

        /// <summary>Represents an interval measured in milliseconds.</summary>
        public const string EdmTimeTypeName = "Edm.Duration";

        /// <summary>edm stream primitive type name</summary>
        public const string EdmStreamTypeName = "Edm.Stream";

        /// <summary>edm geography primitive type name</summary>
        public const string EdmGeographyTypeName = "Edm.Geography";

        /// <summary>Represents a geography Point type.</summary>
        public const string EdmPointTypeName = "Edm.GeographyPoint";

        /// <summary>Represents a geography LineString type.</summary>
        public const string EdmLineStringTypeName = "Edm.GeographyLineString";

        /// <summary>Represents a geography Polygon type.</summary>
        public const string EdmPolygonTypeName = "Edm.GeographyPolygon";

        /// <summary>Represents a geography GeomCollection type.</summary>
        public const string EdmCollectionTypeName = "Edm.GeographyCollection";

        /// <summary>Represents a geography MultiPolygon type.</summary>
        public const string EdmMultiPolygonTypeName = "Edm.GeographyMultiPolygon";

        /// <summary>Represents a geography MultiLineString type.</summary>
        public const string EdmMultiLineStringTypeName = "Edm.GeographyMultiLineString";

        /// <summary>Represents a geography MultiPoint type.</summary>
        public const string EdmMultiPointTypeName = "Edm.GeographyMultiPoint";

        /// <summary>Represents an arbitrary Geometry type.</summary>
        public const string EdmGeometryTypeName = "Edm.Geometry";

        /// <summary>Represents a geometry Point type.</summary>
        public const string EdmGeometryPointTypeName = "Edm.GeometryPoint";

        /// <summary>Represents a geometry LineString type.</summary>
        public const string EdmGeometryLineStringTypeName = "Edm.GeometryLineString";

        /// <summary>Represents a geometry Polygon type.</summary>
        public const string EdmGeometryPolygonTypeName = "Edm.GeometryPolygon";

        /// <summary>Represents a geometry GeomCollection type.</summary>
        public const string EdmGeometryCollectionTypeName = "Edm.GeometryCollection";

        /// <summary>Represents a geometry MultiPolygon type.</summary>
        public const string EdmGeometryMultiPolygonTypeName = "Edm.GeometryMultiPolygon";

        /// <summary>Represents a geometry MultiLineString type.</summary>
        public const string EdmGeometryMultiLineStringTypeName = "Edm.GeometryMultiLineString";

        /// <summary>Represents a geometry MultiPoint type.</summary>
        public const string EdmGeometryMultiPointTypeName = "Edm.GeometryMultiPoint";
        #endregion Edm Primitive Type Names

        #region Edm Collection constants -----------------------------------------------------------------------------/

        /// <summary>The qualifier to turn a type name into a MultiValue type name.</summary>
        public const string CollectionTypeQualifier = "Collection";

        /// <summary>Format string to describe a MultiValue of a given type.</summary>
        public const string CollectionTypeFormat = CollectionTypeQualifier + "({0})";

        #endregion Edm Collection constants

        #region CSDL serialization constants

        /// <summary>
        /// The namespace for Oasis verion of Edmx
        /// </summary>
        public const string EdmxOasisNamespace = "http://docs.oasis-open.org/odata/ns/edmx";

        /// <summary>The element name of the top-level &lt;Edmx&gt; metadata envelope.</summary>
        public const string EdmxName = "Edmx";

        /// <summary>The attribute name used on entity types to indicate that they are MLEs.</summary>
        public const string HasStreamAttributeName = "HasStream";

        /// <summary>The attribute name used on service operations and primitive properties to indicate their MIME type.</summary>
        public const string MimeTypeAttributeName = "MimeType";

        /// <summary>The attribute name used on service operations to indicate their HTTP method.</summary>
        internal const string HttpMethodAttributeName = "HttpMethod";

        /// <summary>'true' literal</summary>
        public const string TrueLiteral = "true";

        /// <summary>'false' literal</summary>
        public const string FalseLiteral = "false";
        #endregion
    }
}