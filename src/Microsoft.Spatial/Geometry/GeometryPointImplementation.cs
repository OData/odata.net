//---------------------------------------------------------------------
// <copyright file="GeometryPointImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;

    /// <summary>
    /// Geometry Point
    /// </summary>
    internal class GeometryPointImplementation : GeometryPoint
    {
        /// <summary>
        /// Latitude
        /// </summary>
        private double x;

        /// <summary>
        /// Longitude
        /// </summary>
        private double y;

        /// <summary>
        /// Z
        /// </summary>
        private double? z;

        /// <summary>
        /// M
        /// </summary>
        private double? m;

        /// <summary>
        /// Empty Point constructor
        /// </summary>
        /// <param name="coordinateSystem">CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        internal GeometryPointImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
            this.x = double.NaN;
            this.y = double.NaN;
        }

        /// <summary>
        /// Point constructor
        /// </summary>
        /// <param name="coordinateSystem">CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="x">latitude</param>
        /// <param name="y">longitude</param>
        /// <param name="z">Z</param>
        /// <param name="m">M</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "zvalue and mvalue are spelled correctly")]
        internal GeometryPointImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator, double x, double y, double? z, double? m)
            : base(coordinateSystem, creator)
        {
            if (double.IsNaN(x) || double.IsInfinity(x))
            {
                throw new ArgumentException(Strings.InvalidPointCoordinate(x, "x"));
            }

            if (double.IsNaN(y) || double.IsInfinity(y))
            {
                throw new ArgumentException(Strings.InvalidPointCoordinate(y, "y"));
            }

            this.x = x;
            this.y = y;
            this.z = z;
            this.m = m;
        }

        /// <summary>
        /// Latitude
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "X is meaningful")]
        public override double X
        {
            get
            {
                if (this.IsEmpty)
                {
                    throw new NotSupportedException(Strings.Point_AccessCoordinateWhenEmpty);
                }

                return this.x;
            }
        }

        /// <summary>
        /// Longitude
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Y is meaningful")]
        public override double Y
        {
            get
            {
                if (this.IsEmpty)
                {
                    throw new NotSupportedException(Strings.Point_AccessCoordinateWhenEmpty);
                }

                return this.y;
            }
        }

        /// <summary>
        /// Is Point Empty
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return double.IsNaN(this.x);
            }
        }

        /// <summary>
        /// Nullable Z
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z and m are meaningful")]
        public override double? Z
        {
            get { return this.z; }
        }

        /// <summary>
        /// Nullable M
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z and m are meaningful")]
        public override double? M
        {
            get { return this.m; }
        }

        /// <summary>
        /// Sends the current spatial object to the given pipeline
        /// </summary>
        /// <param name="pipeline">The spatial pipeline</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
        public override void SendTo(GeometryPipeline pipeline)
        {
            base.SendTo(pipeline);
            pipeline.BeginGeometry(SpatialType.Point);
            if (!this.IsEmpty)
            {
                pipeline.BeginFigure(new GeometryPosition(this.x, this.y, this.z, this.m));
                pipeline.EndFigure();
            }

            pipeline.EndGeometry();
        }
    }
}
