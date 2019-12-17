//---------------------------------------------------------------------
// <copyright file="GeometryPolygonImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Geometry polygon
    /// </summary>
    internal class GeometryPolygonImplementation : GeometryPolygon
    {
        /// <summary>
        /// Rings
        /// </summary>
        private GeometryLineString[] rings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="rings">The rings of this polygon</param>
        internal GeometryPolygonImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator, params GeometryLineString[] rings)
            : base(coordinateSystem, creator)
        {
            this.rings = rings ?? new GeometryLineString[0];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="rings">The rings of this polygon</param>
        internal GeometryPolygonImplementation(SpatialImplementation creator, params GeometryLineString[] rings)
            : this(CoordinateSystem.DefaultGeometry, creator, rings)
        {
        }

        /// <summary>
        /// Is Polygon Empty
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return this.rings.Length == 0;
            }
        }

        /// <summary>
        /// Set of rings
        /// </summary>
        public override ReadOnlyCollection<GeometryLineString> Rings
        {
            get { return new ReadOnlyCollection<GeometryLineString>(this.rings); }
        }

        /// <summary>
        /// Sends the current spatial object to the given pipeline
        /// </summary>
        /// <param name="pipeline">The spatial pipeline</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
        public override void SendTo(GeometryPipeline pipeline)
        {
            base.SendTo(pipeline);
            pipeline.BeginGeometry(SpatialType.Polygon);

            for (int i = 0; i < this.rings.Length; ++i)
            {
                this.rings[i].SendFigure(pipeline);
            }

            pipeline.EndGeometry();
        }
    }
}
