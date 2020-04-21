//---------------------------------------------------------------------
// <copyright file="GeometryCollectionImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Geometry Collection
    /// </summary>
    internal class GeometryCollectionImplementation : GeometryCollection
    {
        /// <summary>
        /// Collection of Geometry instances
        /// </summary>
        private Geometry[] geometryArray;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="geometry">Collection of Geometry instances</param>
        internal GeometryCollectionImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator, params Geometry[] geometry)
            : base(coordinateSystem, creator)
        {
            this.geometryArray = geometry ?? new Geometry[0];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="geometry">Collection of Geometry instances</param>
        internal GeometryCollectionImplementation(SpatialImplementation creator, params Geometry[] geometry)
            : this(CoordinateSystem.DefaultGeometry, creator, geometry)
        {
        }

        /// <summary>
        /// Is Geometry Collection Empty
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return this.geometryArray.Length == 0;
            }
        }

        /// <summary>
        /// Geographies
        /// </summary>
        public override ReadOnlyCollection<Geometry> Geometries
        {
            get { return new ReadOnlyCollection<Geometry>(this.geometryArray); }
        }

        /// <summary>
        /// Sends the current spatial object to the given pipeline
        /// </summary>
        /// <param name="pipeline">The spatial pipeline</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
        public override void SendTo(GeometryPipeline pipeline)
        {
            base.SendTo(pipeline);
            pipeline.BeginGeometry(SpatialType.Collection);
            for (int i = 0; i < this.geometryArray.Length; ++i)
            {
                this.geometryArray[i].SendTo(pipeline);
            }

            pipeline.EndGeometry();
        }
    }
}
