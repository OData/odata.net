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
    using System;
    using System.Spatial;

    /// <summary>
    /// Adapter from the type washed API to Geography, where it assumes that coord1 is Latitude.
    /// </summary>
    internal class TypeWashedToGeographyLatLongPipeline : TypeWashedPipeline
    {
        /// <summary>
        /// The pipeline to redirect the calls to
        /// </summary>
        private readonly GeographyPipeline output;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="output">The pipeline to redirect the calls to</param>
        public TypeWashedToGeographyLatLongPipeline(SpatialPipeline output)
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
            this.output.BeginFigure(new GeographyPosition(coordinate1, coordinate2, coordinate3, coordinate4));
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
            this.output.LineTo(new GeographyPosition(coordinate1, coordinate2, coordinate3, coordinate4));
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
