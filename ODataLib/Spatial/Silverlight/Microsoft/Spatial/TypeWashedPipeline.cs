//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.Data.Spatial
{
    using Microsoft.Spatial;

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
