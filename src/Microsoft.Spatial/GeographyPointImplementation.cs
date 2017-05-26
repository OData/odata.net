//---------------------------------------------------------------------
// <copyright file="GeographyPointImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;

    /// <summary>
    /// This class is an implementation of Geography point.
    /// </summary>
    internal class GeographyPointImplementation : GeographyPoint
    {
        /// <summary>
        /// Latitude
        /// </summary>
        private double latitude;

        /// <summary>
        /// Longitude
        /// </summary>
        private double longitude;

        /// <summary>
        /// Z
        /// </summary>
        private double? z;

        /// <summary>
        /// M
        /// </summary>
        private double? m;

        #region Internal Constructors

        /// <summary>
        /// Point constructor
        /// </summary>
        /// <param name="coordinateSystem">CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="latitude">latitude</param>
        /// <param name="longitude">longitude</param>
        /// <param name="zvalue">Z</param>
        /// <param name="mvalue">M</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "zvalue and mvalue are spelled correctly")]
        internal GeographyPointImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator, double latitude, double longitude, double? zvalue, double? mvalue)
            : base(coordinateSystem, creator)
        {
            if (double.IsNaN(latitude) || double.IsInfinity(latitude))
            {
                throw new ArgumentException(Strings.InvalidPointCoordinate(latitude, "latitude"));
            }

            if (double.IsNaN(longitude) || double.IsInfinity(longitude))
            {
                throw new ArgumentException(Strings.InvalidPointCoordinate(longitude, "longitude"));
            }

            this.latitude = latitude;
            this.longitude = longitude;
            this.z = zvalue;
            this.m = mvalue;
        }

        /// <summary>
        /// Create a empty point
        /// </summary>
        /// <param name="coordinateSystem">CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        internal GeographyPointImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
            this.latitude = double.NaN;
            this.longitude = double.NaN;
        }

        #endregion

        /// <summary>
        /// Latitude
        /// </summary>
        public override double Latitude
        {
            get
            {
                if (this.IsEmpty)
                {
                    throw new NotSupportedException(Strings.Point_AccessCoordinateWhenEmpty);
                }

                return this.latitude;
            }
        }

        /// <summary>
        /// Longitude
        /// </summary>
        public override double Longitude
        {
            get
            {
                if (this.IsEmpty)
                {
                    throw new NotSupportedException(Strings.Point_AccessCoordinateWhenEmpty);
                }

                return this.longitude;
            }
        }

        /// <summary>
        /// Is Point Empty
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return double.IsNaN(this.latitude);
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
        /// Sends the current spatial object to the given sink
        /// </summary>
        /// <param name="pipeline">The spatial pipeline</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
        public override void SendTo(GeographyPipeline pipeline)
        {
            base.SendTo(pipeline);
            pipeline.BeginGeography(SpatialType.Point);
            if (!this.IsEmpty)
            {
                pipeline.BeginFigure(new GeographyPosition(this.latitude, this.longitude, this.z, this.m));
                pipeline.EndFigure();
            }

            pipeline.EndGeography();
        }
    }
}
