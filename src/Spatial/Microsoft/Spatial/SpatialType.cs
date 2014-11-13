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

namespace Microsoft.Spatial
{
    /// <summary> Defines a list of allowed OpenGisTypes types.  </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1028", Justification = "byte required for packing")]
    public enum SpatialType : byte
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Point
        /// </summary>
        Point = 1,

        /// <summary>
        /// Line String
        /// </summary>
        LineString = 2,

        /// <summary>
        /// Polygon
        /// </summary>
        Polygon = 3,

        /// <summary>
        /// Multi-Point
        /// </summary>
        MultiPoint = 4,

        /// <summary>
        /// Multi-Line-String
        /// </summary>
        MultiLineString = 5,

        /// <summary>
        /// Multi-Polygon
        /// </summary>
        MultiPolygon = 6,

        /// <summary>
        /// Collection
        /// </summary>
        Collection = 7,

#if CURVE_SUPPORT
    /// <summary>
    /// Circular-String
    /// </summary>
        CircularString = 8,

        /// <summary>
        /// Compound Curve
        /// </summary>
        CompoundCurve = 9,

        /// <summary>
        /// Curve Polygon
        /// </summary>
        CurvePolygon = 10,
#endif

        /// <summary>
        /// Full Globe
        /// </summary>
        FullGlobe = 11
    }
}
