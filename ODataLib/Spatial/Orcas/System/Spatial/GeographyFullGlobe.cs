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
    /// <summary>
    /// Full Globe
    /// </summary>
    public abstract class GeographyFullGlobe : GeographySurface
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        protected GeographyFullGlobe(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
        }

        /// <summary>
        /// Determines whether this instance and another specified geography instance have the same value. 
        /// </summary>
        /// <param name="other">The geography to compare to this instance.</param>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "null is a valid value")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011", Justification = "Method should not be declared on base")]
        public bool Equals(GeographyFullGlobe other)
        {
            return this.BaseEquals(other) ?? true;
        }

        /// <summary>
        /// Determines whether this instance and another specified geography instance have the same value. 
        /// </summary>
        /// <param name="obj">The geography to compare to this instance.</param>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as GeographyFullGlobe);
        }

        /// <summary>
        /// Get Hashcode
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            return System.Spatial.Geography.ComputeHashCodeFor(this.CoordinateSystem, new[] { 0 });
        }
    }
}
