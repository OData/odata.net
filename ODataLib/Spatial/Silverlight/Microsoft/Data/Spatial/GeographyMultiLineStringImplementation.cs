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
    /// Geography Multi-LineString
    /// </summary>
#if WINDOWS_PHONE
    [DataContract]
#endif
    internal class GeographyMultiLineStringImplementation : GeographyMultiLineString
    {
        /// <summary>
        /// Line Strings
        /// </summary>
        private GeographyLineString[] lineStrings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="lineStrings">Line Strings</param>
        internal GeographyMultiLineStringImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator, params GeographyLineString[] lineStrings)
            : base(coordinateSystem, creator)
        {
            this.lineStrings = lineStrings ?? new GeographyLineString[0];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="lineStrings">Line Strings</param>
        internal GeographyMultiLineStringImplementation(SpatialImplementation creator, params GeographyLineString[] lineStrings)
            : this(CoordinateSystem.DefaultGeography, creator, lineStrings)
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
        /// Geographies
        /// </summary>
        public override ReadOnlyCollection<Geography> Geographies
        {
            get { return new ReadOnlyCollection<Geography>(this.lineStrings); }
        }

        /// <summary>
        /// Line Strings
        /// </summary>
        public override ReadOnlyCollection<GeographyLineString> LineStrings
        {
            get { return new ReadOnlyCollection<GeographyLineString>(this.lineStrings); }
        }

#if WINDOWS_PHONE
        /// <summary>
        /// internal GeographyLineString array property to support serializing and de-serializing this instance.
        /// </summary>
        [DataMember]
        internal GeographyLineString[] LineStringsArray
        {
            get { return this.lineStrings; }

            set { this.lineStrings = value; }
        }
#endif
        /// <summary>
        /// Sends the current spatial object to the given sink
        /// </summary>
        /// <param name="pipeline">The spatial pipeline</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
        public override void SendTo(GeographyPipeline pipeline)
        {
            base.SendTo(pipeline);
            pipeline.BeginGeography(SpatialType.MultiLineString);

            for (int i = 0; i < this.lineStrings.Length; ++i)
            {
                this.lineStrings[i].SendTo(pipeline);
            }

            pipeline.EndGeography();
        }
    }
}
