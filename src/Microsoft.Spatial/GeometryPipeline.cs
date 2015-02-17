//---------------------------------------------------------------------
// <copyright file="GeometryPipeline.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary>Represents the pipeline of geometry.</summary>
    public abstract class GeometryPipeline
    {
        /// <summary>Begins drawing a spatial object.</summary>
        /// <param name="type">The spatial type of the object.</param>
        public abstract void BeginGeometry(SpatialType type);

        /// <summary>Begins drawing a figure.</summary>
        /// <param name="position">The position of the figure.</param>
        public abstract void BeginFigure(GeometryPosition position);

        /// <summary>Draws a point in the specified coordinate.</summary>
        /// <param name="position">The position of the line.</param>
        public abstract void LineTo(GeometryPosition position);

        /// <summary>Ends the current figure.</summary>
        public abstract void EndFigure();

        /// <summary>Ends the current spatial object.</summary>
        public abstract void EndGeometry();

        /// <summary>Sets the coordinate system.</summary>
        /// <param name="coordinateSystem">The coordinate system to set.</param>
        public abstract void SetCoordinateSystem(CoordinateSystem coordinateSystem);

        /// <summary>Resets the pipeline.</summary>
        public abstract void Reset();
    }
}
