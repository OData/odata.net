//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
