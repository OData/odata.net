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
    using System.Collections.ObjectModel;
    using System.Linq;
#if WINDOWS_PHONE
    using System.Runtime.Serialization;
#endif

    /// <summary>Represents a geography line string consist of an array of geo points.</summary>
#if WINDOWS_PHONE
    [DataContract]
#endif
    public abstract class GeographyLineString : GeographyCurve
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Spatial.GeographyLineString" /> class.</summary>
        /// <param name="coordinateSystem">The coordinate system of this instance.</param>
        /// <param name="creator">The implementation that created this instance.</param>
        protected GeographyLineString(CoordinateSystem coordinateSystem, SpatialImplementation creator)
            : base(coordinateSystem, creator)
        {
        }

        /// <summary>Gets the point list.</summary>
        /// <returns>The point list.</returns>
        public abstract ReadOnlyCollection<GeographyPoint> Points { get; }

        /// <summary>Determines whether this instance and another specified geography instance have the same value.</summary>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        /// <param name="other">The geography to compare to this instance.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "null is a valid value")]
        public bool Equals(GeographyLineString other)
        {
            return this.BaseEquals(other) ?? this.Points.SequenceEqual(other.Points);
        }

        /// <summary>Determines whether this instance and the specified object have the same value.</summary>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        /// <param name="obj">The object to compare to this instance.</param>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as GeographyLineString);
        }

        /// <summary>Gets the hash code.</summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Geography.ComputeHashCodeFor(this.CoordinateSystem, this.Points);
        }
    }
}
