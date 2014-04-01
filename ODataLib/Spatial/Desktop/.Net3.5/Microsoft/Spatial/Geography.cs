//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.Spatial
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>Represents a base class of geography shapes.</summary>
    public abstract class Geography : ISpatial
    {
        /// <summary>
        /// The implementation that created this instance
        /// </summary>
        private SpatialImplementation creator;

        /// <summary>
        /// The CoordinateSystem of this geography
        /// </summary>
        private CoordinateSystem coordinateSystem;

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.Spatial.Geography" /> class.</summary>
        /// <param name="coordinateSystem">The coordinate system of this geography.</param>
        /// <param name="creator">The implementation that created this instance.</param>
        protected Geography(CoordinateSystem coordinateSystem, SpatialImplementation creator)
        {
            Util.CheckArgumentNull(coordinateSystem, "coordinateSystem");
            Util.CheckArgumentNull(creator, "creator");
            this.coordinateSystem = coordinateSystem;
            this.creator = creator;
        }

        /// <summary>Gets the coordinate system of the geography.</summary>
        /// <returns>The coordinate system of the geography.</returns>
        public CoordinateSystem CoordinateSystem
        {
            get
            {
                return this.coordinateSystem;
            }

            internal set
            {
                this.coordinateSystem = value;
            }
        }

        /// <summary>Gets a value that indicates whether the geography is empty.</summary>
        /// <returns>true if the geography is empty; otherwise, false.</returns>
        public abstract bool IsEmpty { get; }

        /// <summary>
        /// Gets the implementation that created this instance.
        /// </summary>
        internal SpatialImplementation Creator
        {
            get { return this.creator; }

            set { this.creator = value; }
        }

        /// <summary>Sends the current spatial object to the given pipeline.</summary>
        /// <param name="chain">The spatial pipeline.</param>
        public virtual void SendTo(GeographyPipeline chain)
        {
            Util.CheckArgumentNull(chain, "chain");
            chain.SetCoordinateSystem(this.coordinateSystem);
        }

        /// <summary>
        /// Computes the hashcode for the given CoordinateSystem and the fields
        /// </summary>
        /// <typeparam name="T">Spatial type instances or doubles for base types (Geography/Geometry types).</typeparam>
        /// <param name="coords">CoordinateSystem instance.</param>
        /// <param name="fields">Spatial type instances or doubles for base types (Geography/Geometry types).</param>
        /// <returns>hashcode for the CoordinateSystem instance and Spatial type instances.</returns>
        internal static int ComputeHashCodeFor<T>(CoordinateSystem coords, IEnumerable<T> fields)
        {
            unchecked
            {
                return fields.Aggregate(coords.GetHashCode(), (current, field) => (current * 397) ^ field.GetHashCode());
            }
        }

        /// <summary>
        /// Check for basic equality due to emptyness, nullness, referential equality and difference in coordinate system
        /// </summary>
        /// <param name="other">The other geography</param>
        /// <returns>Boolean value indicating equality, or null to indicate inconclusion</returns>
        internal bool? BaseEquals(Geography other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (!this.coordinateSystem.Equals(other.coordinateSystem))
            {
                return false;
            }

            if (this.IsEmpty || other.IsEmpty)
            {
                return this.IsEmpty && other.IsEmpty;
            }

            return null;
        }
    }
}
