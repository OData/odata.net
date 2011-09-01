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

namespace Microsoft.Data.OData.Metadata
{
    /// <summary>
    /// Constant values used in the EDM.
    /// </summary>
    internal static class EdmConstants
    {
        #region Edm Primitive Type Names -----------------------------------------------------------------------------/

        /// <summary>namespace for edm primitive types.</summary>
        internal const string EdmNamespace = "Edm";

        /// <summary>edm binary primitive type name</summary>
        internal const string EdmBinaryTypeName = "Edm.Binary";

        /// <summary>edm boolean primitive type name</summary>
        internal const string EdmBooleanTypeName = "Edm.Boolean";

        /// <summary>edm byte primitive type name</summary>
        internal const string EdmByteTypeName = "Edm.Byte";

        /// <summary>edm datetime primitive type name</summary>
        internal const string EdmDateTimeTypeName = "Edm.DateTime";

        /// <summary>Represents a Time instance as an interval measured in milliseconds from an instance of DateTime.</summary>
        internal const string EdmDateTimeOffsetTypeName = "Edm.DateTimeOffset";

        /// <summary>edm decimal primitive type name</summary>
        internal const string EdmDecimalTypeName = "Edm.Decimal";

        /// <summary>edm double primitive type name</summary>
        internal const string EdmDoubleTypeName = "Edm.Double";

        /// <summary>edm guid primitive type name</summary>
        internal const string EdmGuidTypeName = "Edm.Guid";

        /// <summary>edm single primitive type name</summary>
        internal const string EdmSingleTypeName = "Edm.Single";

        /// <summary>edm sbyte primitive type name</summary>
        internal const string EdmSByteTypeName = "Edm.SByte";

        /// <summary>edm int16 primitive type name</summary>
        internal const string EdmInt16TypeName = "Edm.Int16";

        /// <summary>edm int32 primitive type name</summary>
        internal const string EdmInt32TypeName = "Edm.Int32";

        /// <summary>edm int64 primitive type name</summary>
        internal const string EdmInt64TypeName = "Edm.Int64";

        /// <summary>edm string primitive type name</summary>
        internal const string EdmStringTypeName = "Edm.String";

        /// <summary>Represents an interval measured in milliseconds.</summary>
        internal const string EdmTimeTypeName = "Edm.Time";

        /// <summary>edm stream primitive type name</summary>
        internal const string EdmStreamTypeName = "Edm.Stream";

        /// <summary>edm geography primitive type name</summary>
        internal const string EdmGeographyTypeName = "Edm.Geography";

        /// <summary>Represents a geography Point type.</summary>
        internal const string EdmPointTypeName = "Edm.Point";

        /// <summary>Represents a geography LineString type.</summary>
        internal const string EdmLineStringTypeName = "Edm.LineString";

        /// <summary>Represents a geography Polygon type.</summary>
        internal const string EdmPolygonTypeName = "Edm.Polygon";

        /// <summary>Represents a geography GeomCollection type.</summary>
        internal const string EdmGeomCollectionTypeName = "Edm.GeomCollection";

        /// <summary>Represents a geography MultiPolygon type.</summary>
        internal const string EdmMultiPolygonTypeName = "Edm.MultiPolygon";

        /// <summary>Represents a geography MultiLineString type.</summary>
        internal const string EdmMultiLineStringTypeName = "Edm.MultiLineString";

        /// <summary>Represents a geography MultiPoint type.</summary>
        internal const string EdmMultiPointTypeName = "Edm.MultiPoint";

        /// <summary>Represents an arbitrary Geometry type.</summary>
        internal const string EdmGeometryTypeName = "Edm.Geometry";

        /// <summary>Represents a geometry Point type.</summary>
        internal const string EdmGeometricPointTypeName = "Edm.GeometricPoint";

        /// <summary>Represents a geometry LineString type.</summary>
        internal const string EdmGeometricLineStringTypeName = "Edm.GeometricLineString";

        /// <summary>Represents a geometry Polygon type.</summary>
        internal const string EdmGeometricPolygonTypeName = "Edm.GeometricPolygon";

        /// <summary>Represents a geometry GeomCollection type.</summary>
        internal const string EdmGeometricGeomCollectionTypeName = "Edm.GeometricGeomCollection";

        /// <summary>Represents a geometry MultiPolygon type.</summary>
        internal const string EdmGeometricMultiPolygonTypeName = "Edm.GeometricMultiPolygon";

        /// <summary>Represents a geometry MultiLineString type.</summary>
        internal const string EdmGeometricMultiLineStringTypeName = "Edm.GeometricMultiLineString";

        /// <summary>Represents a geometry MultiPoint type.</summary>
        internal const string EdmGeometricMultiPointTypeName = "Edm.GeometricMultiPoint";
        #endregion Edm Primitive Type Names

        #region Edm MultiValue constants -----------------------------------------------------------------------------/

        /// <summary>The qualifier to turn a type name into a MultiValue type name.</summary>
        internal const string MultiValueTypeQualifier = "MultiValue";

        /// <summary>Format string to describe a MultiValue of a given type.</summary>
        internal const string MultiValueTypeFormat = MultiValueTypeQualifier + "({0})";

        #endregion Edm MultiValue constants

        #region CSDL serialization constants
        /// <summary>The attribute name used on entity types to indicate that they are MLEs.</summary>
        internal const string HasStreamAttributeName = "HasStream";

        /// <summary>The attribute name used on service operations and primitive properties to indicate their MIME type.</summary>
        internal const string MimeTypeAttributeName = "MimeType";

        /// <summary>The attribute name used on an entity container to mark it as the default entity container.</summary>
        internal const string IsDefaultEntityContainerAttributeName = "IsDefaultEntityContainer";

        /// <summary>'true' literal</summary>
        internal const string TrueLiteral = "true";

        /// <summary>'false' literal</summary>
        internal const string FalseLiteral = "false";
        #endregion
    }
}
