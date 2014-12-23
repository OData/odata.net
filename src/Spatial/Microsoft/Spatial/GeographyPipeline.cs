//   OData .NET Libraries ver. 6.9
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
