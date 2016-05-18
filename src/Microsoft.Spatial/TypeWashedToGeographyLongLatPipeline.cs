//---------------------------------------------------------------------
// <copyright file="TypeWashedToGeographyLongLatPipeline.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary>
    /// Adapter from the type washed API to Geography, where it assumes that coord1 is Longitude.
    /// </summary>
    internal class TypeWashedToGeographyLongLatPipeline : TypeWashedPipeline
    {
        /// <summary>
        /// The pipeline to redirect the calls to
        /// </summary>
        private readonly GeographyPipeline output;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="output">The pipeline to redirect the calls to</param>
        public TypeWashedToGeographyLongLatPipeline(SpatialPipeline output)
        {
            this.output = output;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is geography.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is geography; otherwise, <c>false</c>.
        /// </value>
        public override bool IsGeography
        {
            get { return true; }
        }

        /// <summary>
        /// Set the coordinate system based on the given ID
        /// </summary>
        /// <param name="epsgId">The coordinate system ID to set. Null indicates the default should be used</param>
        internal override void SetCoordinateSystem(int? epsgId)
        {
            var coordinateSystem = CoordinateSystem.Geography(epsgId);
            this.output.SetCoordinateSystem(coordinateSystem);
        }

        /// <summary>
        /// Setup the pipeline for reuse
        /// </summary>
        internal override void Reset()
        {
            output.Reset();
        }

        /// <summary>
        /// Begin drawing a spatial object
        /// </summary>
        /// <param name="type">The spatial type of the object</param>
        internal override void BeginGeo(SpatialType type)
        {
            this.output.BeginGeography(type);
        }

        /// <summary>
        /// Begin drawing a figure
        /// </summary>
        /// <param name="coordinate1">1st Coordinate</param>
        /// <param name="coordinate2">2nd Coordinate</param>
        /// <param name="coordinate3">3rd Coordinate</param>
        /// <param name="coordinate4">4th Coordinate</param>
        internal override void BeginFigure(double coordinate1, double coordinate2, double? coordinate3, double? coordinate4)
        {
            this.output.BeginFigure(new GeographyPosition(coordinate2, coordinate1, coordinate3, coordinate4));
        }

        /// <summary>
        /// Draw a line to a point in the specified coordinate
        /// </summary>
        /// <param name="coordinate1">1st Coordinate</param>
        /// <param name="coordinate2">2nd Coordinate</param>
        /// <param name="coordinate3">3rd Coordinate</param>
        /// <param name="coordinate4">4th Coordinate</param>
        internal override void LineTo(double coordinate1, double coordinate2, double? coordinate3, double? coordinate4)
        {
            this.output.LineTo(new GeographyPosition(coordinate2, coordinate1, coordinate3, coordinate4));
        }

        /// <summary>
        /// Ends the current figure
        /// </summary>
        internal override void EndFigure()
        {
            this.output.EndFigure();
        }

        /// <summary>
        /// Ends the current spatial object
        /// </summary>
        internal override void EndGeo()
        {
            this.output.EndGeography();
        }
    }
}