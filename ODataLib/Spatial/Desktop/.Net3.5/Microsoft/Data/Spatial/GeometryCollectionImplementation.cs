//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Microsoft.Data.Spatial
{
    using System;
    using System.Collections.ObjectModel;
#if WINDOWS_PHONE
    using System.Runtime.Serialization;
#endif
    using System.Spatial;

    /// <summary>
    /// Geometry Collection
    /// </summary>
#if WINDOWS_PHONE
    [DataContract]
#endif
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

#if WINDOWS_PHONE
        /// <summary>
        /// internal geometry array property to support serializing and de-serializing this instance.
        /// </summary>
        [DataMember]
        internal Geometry[] GeometryArray
        {
            get { return this.geometryArray; }

            set { this.geometryArray = value; }
        }
#endif
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
