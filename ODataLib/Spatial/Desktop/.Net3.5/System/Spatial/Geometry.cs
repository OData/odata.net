//   OData .NET Libraries ver. 5.6.3
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

namespace System.Spatial
{
#if WINDOWS_PHONE
    using System.Runtime.Serialization;
#endif

    /// <summary>Represents the base class of geography shapes.</summary>
#if WINDOWS_PHONE
    [DataContract]
#endif
    public abstract class Geometry : ISpatial
    {
        /// <summary>
        /// The implementation that created this instance.
        /// </summary>
        private SpatialImplementation creator;

        /// <summary>
        /// The CoordinateSystem of this geometry
        /// </summary>
        private CoordinateSystem coordinateSystem;

        /// <summary>Initializes a new instance of the <see cref="T:System.Spatial.Geometry" /> class.</summary>
        /// <param name="coordinateSystem">The coordinate system of this instance.</param>
        /// <param name="creator">The implementation that created this instance.</param>
        protected Geometry(CoordinateSystem coordinateSystem, SpatialImplementation creator)
        {
            Util.CheckArgumentNull(coordinateSystem, "coordinateSystem");
            Util.CheckArgumentNull(creator, "creator");
            this.coordinateSystem = coordinateSystem;
            this.creator = creator;
        }

        /// <summary>Gets the SRID of this instance of geometry.</summary>
        /// <returns>The SRID of this instance of geometry.</returns>
#if WINDOWS_PHONE
        [DataMember]
#endif
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

        /// <summary>Gets a value that indicates whether geometry is empty.</summary>
        /// <returns>true if the geometry is empty; otherwise, false.</returns>
        public abstract bool IsEmpty { get; }

        /// <summary>
        /// Gets the implementation that created this instance.
        /// </summary>
#if WINDOWS_PHONE
        [DataMember]
#endif
        internal SpatialImplementation Creator
        {
            get { return this.creator; }

            set { this.creator = value; }
        }

        /// <summary>Sends the current spatial object to the given pipeline.</summary>
        /// <param name="chain">The spatial pipeline.</param>
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
