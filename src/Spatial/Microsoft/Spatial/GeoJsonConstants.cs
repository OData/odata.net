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

namespace Microsoft.Data.Spatial
{
    /// <summary>
    /// Constants for the GeoJSON format
    /// See http://geojson.org/geojson-spec.html for full details on GeoJson format.
    /// </summary>
    internal static class GeoJsonConstants
    {
        /// <summary>
        /// Name of the type member that identifies the spatial type.
        /// </summary>
        internal const string TypeMemberName = "type";

        /// <summary>
        /// Value of the type member for Point values.
        /// </summary>
        internal const string TypeMemberValuePoint = "Point";

        /// <summary>
        /// Value of the type member for LineString values.
        /// </summary>
        internal const string TypeMemberValueLineString = "LineString";

        /// <summary>
        /// Value of the type member for Polygon values.
        /// </summary>
        internal const string TypeMemberValuePolygon = "Polygon";

        /// <summary>
        /// Value of the type member for MultiPoint values.
        /// </summary>
        internal const string TypeMemberValueMultiPoint = "MultiPoint";

        /// <summary>
        /// Value of the type member for MultiLineString values.
        /// </summary>
        internal const string TypeMemberValueMultiLineString = "MultiLineString";

        /// <summary>
        /// Value of the type member for MultiPolygon values.
        /// </summary>
        internal const string TypeMemberValueMultiPolygon = "MultiPolygon";

        /// <summary>
        /// Value of the type member for GeometryCollection values.
        /// </summary>
        internal const string TypeMemberValueGeometryCollection = "GeometryCollection";

        /// <summary>
        /// Name of the coordinates member that contains the spatial data.
        /// </summary>
        internal const string CoordinatesMemberName = "coordinates";

        /// <summary>
        /// Name of the geometries member that contains the spatial data.
        /// </summary>
        internal const string GeometriesMemberName = "geometries";

        /// <summary>
        /// Name of the crs member that contains the coordinate reference system details.
        /// </summary>
        internal const string CrsMemberName = "crs";

        /// <summary>
        /// Value of the type member inside of the crs object.
        /// </summary>
        internal const string CrsTypeMemberValue = "name";

        /// <summary>
        /// Name of the name member inside of the properties member in the crs object.
        /// </summary>
        internal const string CrsNameMemberName = "name";

        /// <summary>
        /// Name of the properties member inside of the crs object.
        /// </summary>
        internal const string CrsPropertiesMemberName = "properties";

        /// <summary>
        /// Prefix to use when specifying the coordinate reference system inside the crs object.
        /// </summary>
        internal const string CrsValuePrefix = "EPSG";
    }
}
