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

namespace System.Spatial
{
    /// <summary>
    /// This is definition of the GeometryPipeline api
    /// </summary>
    public abstract class GeometryPipeline
    {
        /// <summary>
        /// Begin drawing a spatial object
        /// </summary>
        /// <param name="type">The spatial type of the object</param>
        public abstract void BeginGeometry(SpatialType type);

        /// <summary>
        /// Begin drawing a figure
        /// </summary>
        /// <param name="position">Next position</param>
        public abstract void BeginFigure(GeometryPosition position);

        /// <summary>
        /// Draw a point in the specified coordinate
        /// </summary>
        /// <param name="position">Next position</param>
        public abstract void LineTo(GeometryPosition position);

        /// <summary>
        /// Ends the current figure
        /// </summary>
        public abstract void EndFigure();

        /// <summary>
        /// Ends the current spatial object
        /// </summary>
        public abstract void EndGeometry();

        /// <summary>
        /// Set the coordinate system
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        public abstract void SetCoordinateSystem(CoordinateSystem coordinateSystem);

        /// <summary>
        /// Setup the pipeline for reuse
        /// </summary>
        public abstract void Reset();
    }
}
