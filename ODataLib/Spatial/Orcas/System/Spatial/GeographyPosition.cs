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
    /// Represents one position in the Geographyal coordinate system
    /// </summary>
    //// DEVNOTE(pqian):
    //// Initially we had this as a value-type (struct) since it's rarely used outside of call parameters
    //// However, since we need to remove the parameterless constructor, we now define it as a refererence type.
    public class GeographyPosition : IEquatable<GeographyPosition>
    {
        /// <summary>lattitude portion of position</summary>
        private readonly double latitude;

        /// <summary>longitude portion of position</summary>
        private readonly double longitude;

        /// <summary>arbitrary measure associated with a position</summary>
        private readonly double? m;

        /// <summary>altitude portion of position</summary>
        private readonly double? z;

        /// <summary>
        /// Creates a GeographyPosition from components
        /// </summary>
        /// <param name="latitude">lattitude portion of position</param>
        /// <param name="longitude">longitude portion of position</param>
        /// <param name="z">altitude portion of position</param>
        /// <param name="m">arbitrary measure associated with a position</param>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z, m make sense in context")]
        public GeographyPosition(double latitude, double longitude, double? z, double? m)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.z = z;
            this.m = m;
        }

        /// <summary>
        /// Creates a GeographyPosition from components
        /// </summary>
        /// <param name="latitude">lattitude portion of position</param>
        /// <param name="longitude">longitude portion of position</param>
        public GeographyPosition(double latitude, double longitude)
            : this(latitude, longitude, null, null)
        {
        }
        
        /// <summary>
        /// lattitude portion of position
        /// </summary>
        public double Latitude
        {
            get { return latitude; }
        }

        /// <summary>
        /// longitude portion of position
        /// </summary>
        public double Longitude
        {
            get { return longitude; }
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
        public static bool operator ==(GeographyPosition left, GeographyPosition right)
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
        public static bool operator !=(GeographyPosition left, GeographyPosition right)
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

            if (obj.GetType() != typeof(GeographyPosition))
            {
                return false;
            }

            return Equals((GeographyPosition)obj);
        }

        /// <summary>
        /// Equality comparison
        /// </summary>
        /// <param name="other">other position</param>
        /// <returns>true if each pair of coordinates is equal</returns>
        public bool Equals(GeographyPosition other)
        {
            return other != null && other.latitude.Equals(latitude) && other.longitude.Equals(longitude) && other.z.Equals(z) &&
                   other.m.Equals(m);
        }

        /// <summary>
        /// Computes a hash code
        /// </summary>
        /// <returns>a hash code</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = latitude.GetHashCode();
                result = (result * 397) ^ longitude.GetHashCode();
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
            return String.Format(System.Globalization.CultureInfo.InvariantCulture, "GeographyPosition(latitude:{0}, longitude:{1}, z:{2}, m:{3})", this.latitude, this.longitude, this.z.HasValue ? this.z.ToString() : "null", this.m.HasValue ? this.m.ToString() : "null");
        }
    }
}
