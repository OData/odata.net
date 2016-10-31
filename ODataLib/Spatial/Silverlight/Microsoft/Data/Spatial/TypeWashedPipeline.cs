//   OData .NET Libraries ver. 5.6.3
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
    using System.Spatial;

    /// <summary>
    /// Internal pipeline Inteface that washes the distinction between Geography and Geometry
    /// </summary>
    internal abstract class TypeWashedPipeline
    {
        /// <summary>
        /// Gets a value indicating whether this instance is geography.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is geography; otherwise, <c>false</c>.
        /// </value>
        public abstract bool IsGeography { get; }

        /// <summary>
        /// Set the coordinate system based on the given EPSG ID
        /// </summary>
        /// <param name="epsgId">The coordinate system ID to set. Null indicates the default should be used</param>
        internal abstract void SetCoordinateSystem(int? epsgId);

        /// <summary>
        /// Setup the pipeline for reuse
        /// </summary>
        internal abstract void Reset();

        /// <summary>
        /// Begin drawing a spatial object
        /// </summary>
        /// <param name="type">The spatial type of the object</param>
        internal abstract void BeginGeo(SpatialType type);

        /// <summary>
        /// Begin drawing a figure
        /// </summary>
        /// <param name="coordinate1">X or Latitude Coordinate</param>
        /// <param name="coordinate2">Y or Longitude Coordinate</param>
        /// <param name="coordinate3">Z Coordinate</param>
        /// <param name="coordinate4">M Coordinate</param>
        internal abstract void BeginFigure(double coordinate1, double coordinate2, double? coordinate3, double? coordinate4);

        /// <summary>
        /// Add a control point to the current figure
        /// </summary>
        /// <param name="coordinate1">First coordinate</param>
        /// <param name="coordinate2">Second coordinate</param>
        /// <param name="coordinate3">Third coordinate</param>
        /// <param name="coordinate4">Fourth coordinate</param>
        internal abstract void LineTo(double coordinate1, double coordinate2, double? coordinate3, double? coordinate4);

        /// <summary>
        /// Ends the current figure
        /// </summary>
        internal abstract void EndFigure();

        /// <summary>
        /// Ends the current spatial object
        /// </summary>
        internal abstract void EndGeo();
    }
}
