//---------------------------------------------------------------------
// <copyright file="TypeWashedPipeline.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
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
