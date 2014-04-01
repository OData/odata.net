//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.Spatial
{
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>Represents the geometry multi-point.</summary>
    public abstract class GeometryMultiPoint : GeometryCollection
    {
        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.Spatial.GeometryMultiPoint" /> class.</summary>
        /// <param name="coordinateSystem">The coordinate system of this instance.</param>
        /// <param name="creator">The implementation that created this instance.</param>
        protected GeometryMultiPoint(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
        }

        /// <summary>Gets a collection of points.</summary>
        /// <returns>A collection of points.</returns>
        public abstract ReadOnlyCollection<GeometryPoint> Points { get; }

        /// <summary>Determines whether this instance and another specified geometry instance have the same value.</summary>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        /// <param name="other">The geometry to compare to this instance.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "null is a valid value")]
        public bool Equals(GeometryMultiPoint other)
        {
            return this.BaseEquals(other) ?? this.Points.SequenceEqual(other.Points);
        }

        /// <summary>Determines whether this instance and the specified object have the same value.</summary>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        /// <param name="obj">The object to compare to this instance.</param>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as GeometryMultiPoint);
        }

        /// <summary>Gets the hash code.</summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Microsoft.Spatial.Geography.ComputeHashCodeFor(this.CoordinateSystem, this.Points);
        }
    }
}
