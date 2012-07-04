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

    /// <summary>
    /// Represents one position in the Geometry coordinate system
    /// </summary>
    public class GeometryPosition : IEquatable<GeometryPosition>
    {
        /// <summary>arbitrary measure associated with a position</summary>
        private readonly double? m;

        /// <summary>x portion of position</summary>
        private readonly double x;

        /// <summary>y portion of position</summary>
        private readonly double y;

        /// <summary>altitude portion of position</summary>
        private readonly double? z;

        /// <summary>
        /// Creates a GeometryPosition from components
        /// </summary>
        /// <param name="x">x portion of position</param>
        /// <param name="y">y portion of position</param>
        /// <param name="z">altitude portion of position</param>
        /// <param name="m">arbitrary measure associated with a position</param>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z, m make sense in context")]
        public GeometryPosition(double x, double y, double? z, double? m)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.m = m;
        }

        /// <summary>
        /// Creates a GeometryPosition from components
        /// </summary>
        /// <param name="x">x portion of position</param>
        /// <param name="y">y portion of position</param>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y make sense in context")]
        public GeometryPosition(double x, double y)
            : this(x, y, null, null)
        {
        }

        /// <summary>
        /// arbitrary measure associated with a position
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z, m make sense in context")]
        public double? M
        {
            get { return m; }
        }

        /// <summary>
        /// x portion of position
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z, m make sense in context")]
        public double X
        {
            get { return x; }
        }

        /// <summary>
        /// y portion of position
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z, m make sense in context")]
        public double Y
        {
            get { return y; }
        }

        /// <summary>
        /// altitude portion of position
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z, m make sense in context")]
        public double? Z
        {
            get { return z; }
        }

        /// <summary>
        /// Equality comparison
        /// </summary>
        /// <param name="left">first position</param>
        /// <param name="right">second position</param>
        /// <returns>true if each pair of coordinates is equal</returns>
        public static bool operator ==(GeometryPosition left, GeometryPosition right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            else if (ReferenceEquals(right, null))
            {
                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Inequality comparison
        /// </summary>
        /// <param name="left">first position</param>
        /// <param name="right">other position</param>
        /// <returns>true if left is not equal to right</returns>
        public static bool operator !=(GeometryPosition left, GeometryPosition right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Equality comparison
        /// </summary>
        /// <param name="obj">other position</param>
        /// <returns>true if each pair of coordinates is equal</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (obj.GetType() != typeof(GeometryPosition))
            {
                return false;
            }

            return Equals((GeometryPosition)obj);
        }

        /// <summary>
        /// Equality comparison
        /// </summary>
        /// <param name="other">other position</param>
        /// <returns>true if each pair of coordinates is equal</returns>
        public bool Equals(GeometryPosition other)
        {
            return other != null && other.x.Equals(x) && other.y.Equals(y) && other.z.Equals(z) && other.m.Equals(m);
        }

        /// <summary>
        /// Computes a hash code
        /// </summary>
        /// <returns>a hash code</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = x.GetHashCode();
                result = (result * 397) ^ y.GetHashCode();
                result = (result * 397) ^ (z.HasValue ? z.Value.GetHashCode() : 0);
                result = (result * 397) ^ (m.HasValue ? m.Value.GetHashCode() : 0);
                return result;
            }
        }
        
        /// <summary>
        /// Formats this instance to a readable string
        /// </summary>
        /// <returns>The string representation of this instance</returns>
        public override string ToString()
        {
            return String.Format(System.Globalization.CultureInfo.InvariantCulture, "GeometryPosition({0}, {1}, {2}, {3})", this.x, this.y, this.z.HasValue ? this.z.ToString() : "null", this.m.HasValue ? this.m.ToString() : "null");
        }
    }
}
