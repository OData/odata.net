//---------------------------------------------------------------------
// <copyright file="GeometryMultiLineStringImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Geometry Multi-LineString
    /// </summary>
    internal class GeometryMultiLineStringImplementation : GeometryMultiLineString
    {
        /// <summary>
        /// Line Strings
        /// </summary>
        private GeometryLineString[] lineStrings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="lineStrings">Line Strings</param>
        internal GeometryMultiLineStringImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator, params GeometryLineString[] lineStrings)
            : base(coordinateSystem, creator)
        {
            this.lineStrings = lineStrings ?? new GeometryLineString[0];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="lineStrings">Line Strings</param>
        internal GeometryMultiLineStringImplementation(SpatialImplementation creator, params GeometryLineString[] lineStrings)
            : this(CoordinateSystem.DefaultGeometry, creator, lineStrings)
        {
        }

        /// <summary>
        /// Is MultiLineString Empty
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return this.lineStrings.Length == 0;
            }
        }

        /// <summary>
        /// Geometry
        /// </summary>
        public override ReadOnlyCollection<Geometry> Geometries
        {
            get { return new ReadOnlyCollection<Geometry>(this.lineStrings); }
        }

        /// <summary>
        /// Line Strings
        /// </summary>
        public override ReadOnlyCollection<GeometryLineString> LineStrings
        {
            get { return new ReadOnlyCollection<GeometryLineString>(this.lineStrings); }
        }

        /// <summary>
        /// Sends the current spatial object to the given pipeline
        /// </summary>
        /// <param name="pipeline">The spatial pipeline</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
        public override void SendTo(GeometryPipeline pipeline)
        {
            base.SendTo(pipeline);
            pipeline.BeginGeometry(SpatialType.MultiLineString);

            for (int i = 0; i < this.lineStrings.Length; ++i)
            {
                this.lineStrings[i].SendTo(pipeline);
            }

            pipeline.EndGeometry();
        }
    }
}
