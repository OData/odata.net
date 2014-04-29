//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.Spatial
{
    /// <summary>Represents the pipeline of geography.</summary>
    public abstract class GeographyPipeline
    {
        /// <summary>Begins drawing a spatial object.</summary>
        /// <param name="type">The spatial type of the object.</param>
        public abstract void BeginGeography(SpatialType type);

        /// <summary>Begins drawing a figure.</summary>
        /// <param name="position">The position of the figure.</param>
        public abstract void BeginFigure(GeographyPosition position);

        /// <summary>Draws a point in the specified coordinate.</summary>
        /// <param name="position">The position of the line.</param>
        public abstract void LineTo(GeographyPosition position);

        /// <summary>Ends the current figure.</summary>
        public abstract void EndFigure();

        /// <summary>Ends the current spatial object.</summary>
        public abstract void EndGeography();

        /// <summary>Sets the coordinate system.</summary>
        /// <param name="coordinateSystem">The coordinate system to set.</param>
        public abstract void SetCoordinateSystem(CoordinateSystem coordinateSystem);

        /// <summary>Resets the pipeline.</summary>
        public abstract void Reset();
    }
}
