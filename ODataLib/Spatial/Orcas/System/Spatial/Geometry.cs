//   Copyright 2011 Microsoft Corporation
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

namespace System.Spatial
{
    using Diagnostics.CodeAnalysis;
    using Runtime.Serialization;

    /// <summary>
    /// Base class of Geography Shapes
    /// </summary>
    public abstract class Geometry : ISpatial
    {
        /// <summary>
        /// The implementation that created this instance.
        /// </summary>
        private readonly SpatialImplementation creator;

        /// <summary>
        /// The CoordinateSystem of this geometry
        /// </summary>
        private readonly CoordinateSystem coordinateSystem;

        /// <summary>
        /// Geometry Constructor
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        protected Geometry(CoordinateSystem coordinateSystem, SpatialImplementation creator)
        {
            Util.CheckArgumentNull(coordinateSystem, "coordinateSystem");
            Util.CheckArgumentNull(creator, "creator");
            this.coordinateSystem = coordinateSystem;
            this.creator = creator;
        }

        /// <summary>
        /// SRID of this instance of geometry
        /// </summary>
        public CoordinateSystem CoordinateSystem
        {
            get
            {
                return this.coordinateSystem;
            }
        }

        /// <summary>
        /// Is Geometry Empty
        /// </summary>
        public abstract bool IsEmpty { get; }

        /// <summary>
        /// Gets the implementation that created this instance.
        /// </summary>
        internal SpatialImplementation Creator
        {
            get { return this.creator; }
        }

        /// <summary>
        /// Sends the current spatial object to the given pipeline
        /// </summary>
        /// <param name="chain">The spatial pipeline</param>
        public virtual void SendTo(GeometryPipeline chain)
        {
            Util.CheckArgumentNull(chain, "chain");
            chain.SetCoordinateSystem(this.coordinateSystem);
        }

        /// <summary>
        /// Check for basic equality due to emptyness, nullness, referential equality and difference in coordinate system
        /// </summary>
        /// <param name="other">The other geography</param>
        /// <returns>Boolean value indicating equality, or null to indicate inconclusion</returns>
        internal bool? BaseEquals(Geometry other)
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
