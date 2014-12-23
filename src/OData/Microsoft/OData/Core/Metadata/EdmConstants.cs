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

namespace Microsoft.OData.Core.Metadata
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

        /// <summary>edm date primitive type name</summary>
        internal const string EdmDateTypeName = "Edm.Date";

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
        internal const string EdmDurationTypeName = "Edm.Duration";

        /// <summary>edm stream primitive type name</summary>
        internal const string EdmStreamTypeName = "Edm.Stream";

        /// <summary>edm timeOfDay primitive type name</summary>
        internal const string EdmTimeOfDayTypeName = "Edm.TimeOfDay";

        /// <summary>edm geography primitive type name</summary>
        internal const string EdmGeographyTypeName = "Edm.Geography";

        /// <summary>Represents a geography Point type.</summary>
        internal const string EdmPointTypeName = "Edm.GeographyPoint";

        /// <summary>Represents a geography LineString type.</summary>
        internal const string EdmLineStringTypeName = "Edm.GeographyLineString";

        /// <summary>Represents a geography Polygon type.</summary>
        internal const string EdmPolygonTypeName = "Edm.GeographyPolygon";

        /// <summary>Represents a geography GeomCollection type.</summary>
        internal const string EdmGeographyCollectionTypeName = "Edm.GeographyCollection";

        /// <summary>Represents a geography MultiPolygon type.</summary>
        internal const string EdmMultiPolygonTypeName = "Edm.GeographyMultiPolygon";

        /// <summary>Represents a geography MultiLineString type.</summary>
        internal const string EdmMultiLineStringTypeName = "Edm.GeographyMultiLineString";

        /// <summary>Represents a geography MultiPoint type.</summary>
        internal const string EdmMultiPointTypeName = "Edm.GeographyMultiPoint";

        /// <summary>Represents an arbitrary Geometry type.</summary>
        internal const string EdmGeometryTypeName = "Edm.Geometry";

        /// <summary>Represents a geometry Point type.</summary>
        internal const string EdmGeometryPointTypeName = "Edm.GeometryPoint";

        /// <summary>Represents a geometry LineString type.</summary>
        internal const string EdmGeometryLineStringTypeName = "Edm.GeometryLineString";

        /// <summary>Represents a geometry Polygon type.</summary>
        internal const string EdmGeometryPolygonTypeName = "Edm.GeometryPolygon";

        /// <summary>Represents a geometry GeomCollection type.</summary>
        internal const string EdmGeometryCollectionTypeName = "Edm.GeometryCollection";

        /// <summary>Represents a geometry MultiPolygon type.</summary>
        internal const string EdmGeometryMultiPolygonTypeName = "Edm.GeometryMultiPolygon";

        /// <summary>Represents a geometry MultiLineString type.</summary>
        internal const string EdmGeometryMultiLineStringTypeName = "Edm.GeometryMultiLineString";

        /// <summary>Represents a geometry MultiPoint type.</summary>
        internal const string EdmGeometryMultiPointTypeName = "Edm.GeometryMultiPoint";
        #endregion Edm Primitive Type Names

        #region CSDL serialization constants

        /// <summary>
        /// The namespace for Oasis verion of Edmx
        /// </summary>
        internal const string EdmxOasisNamespace = "http://docs.oasis-open.org/odata/ns/edmx";

        /// <summary>The element name of the top-level &lt;Edmx&gt; metadata envelope.</summary>
        internal const string EdmxName = "Edmx";

        #endregion
    }
}
