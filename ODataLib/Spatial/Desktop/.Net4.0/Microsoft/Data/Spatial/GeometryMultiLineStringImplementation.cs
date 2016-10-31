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

namespace Microsoft.Data.Spatial
{
    using System;
    using System.Collections.ObjectModel;
#if WINDOWS_PHONE
    using System.Runtime.Serialization;
#endif
    using System.Spatial;

    /// <summary>
    /// Geometry Multi-LineString
    /// </summary>
#if WINDOWS_PHONE
    [DataContract]
#endif
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

#if WINDOWS_PHONE
        /// <summary>
        /// internal GeometryLineString array property to support serializing and de-serializing this instance.
        /// </summary>
        [DataMember]
        internal GeometryLineString[] LineStringsArray
        {
            get { return this.lineStrings; }

            set { this.lineStrings = value; }
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
            pipeline.BeginGeometry(SpatialType.MultiLineString);

            for (int i = 0; i < this.lineStrings.Length; ++i)
            {
                this.lineStrings[i].SendTo(pipeline);
            }

            pipeline.EndGeometry();
        }
    }
}
