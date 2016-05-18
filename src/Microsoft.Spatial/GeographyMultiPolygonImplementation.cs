//---------------------------------------------------------------------
// <copyright file="GeographyMultiPolygonImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Geography Multi-Polygon
    /// </summary>
    internal class GeographyMultiPolygonImplementation : GeographyMultiPolygon
    {
        /// <summary>
        /// Polygons
        /// </summary>
        private GeographyPolygon[] polygons;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="polygons">Polygons</param>
        internal GeographyMultiPolygonImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator, params GeographyPolygon[] polygons)
            : base(coordinateSystem, creator)
        {
            this.polygons = polygons;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="polygons">Polygons</param>
        internal GeographyMultiPolygonImplementation(SpatialImplementation creator, params GeographyPolygon[] polygons)
            : this(CoordinateSystem.DefaultGeography, creator, polygons)
        {
        }

        /// <summary>
        /// Is MultiPolygon Empty
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return this.polygons.Length == 0;
            }
        }

        /// <summary>
        /// Geographies
        /// </summary>
        public override ReadOnlyCollection<Geography> Geographies
        {
            get { return new ReadOnlyCollection<Geography>(this.polygons); }
        }

        /// <summary>
        /// Polygons
        /// </summary>
        public override ReadOnlyCollection<GeographyPolygon> Polygons
        {
            get { return new ReadOnlyCollection<GeographyPolygon>(this.polygons); }
        }

        /// <summary>
        /// Sends the current spatial object to the given sink
        /// </summary>
        /// <param name="pipeline">The spatial pipeline</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
        public override void SendTo(GeographyPipeline pipeline)
        {
            base.SendTo(pipeline);
            pipeline.BeginGeography(SpatialType.MultiPolygon);
            for (int i = 0; i < this.polygons.Length; ++i)
            {
                this.polygons[i].SendTo(pipeline);
            }

            pipeline.EndGeography();
        }
    }
}
