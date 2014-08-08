//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Microsoft.Data.Spatial
{
    using System;
    using System.Collections.ObjectModel;
#if WINDOWS_PHONE
    using System.Runtime.Serialization;
#endif
    using System.Spatial;

    /// <summary>
    /// Geography Collection
    /// </summary>
#if WINDOWS_PHONE
    [DataContract]
#endif
    internal class GeographyCollectionImplementation : GeographyCollection
    {
        /// <summary>
        /// Collection of geography instances
        /// </summary>
        private Geography[] geographyArray;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="geography">Collection of geography instances</param>
        internal GeographyCollectionImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator, params Geography[] geography)
            : base(coordinateSystem, creator)
        {
            this.geographyArray = geography ?? new Geography[0];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="geography">Collection of geography instances</param>
        internal GeographyCollectionImplementation(SpatialImplementation creator, params Geography[] geography)
            : this(CoordinateSystem.DefaultGeography, creator, geography)
        {
        }

        /// <summary>
        /// Is Geography Collection Empty
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return this.geographyArray.Length == 0;
            }
        }

        /// <summary>
        /// Geographies
        /// </summary>
        public override ReadOnlyCollection<Geography> Geographies
        {
            get { return new ReadOnlyCollection<Geography>(this.geographyArray); }
        }

#if WINDOWS_PHONE
        /// <summary>
        /// internal geography array property to support serializing and de-serializing this instance.
        /// </summary>
        [DataMember]
        internal Geography[] GeographyArray
        {
            get { return this.geographyArray; }

            set { this.geographyArray = value; }
        }
#endif
        /// <summary>
        /// Sends the current spatial object to the given pipeline
        /// </summary>
        /// <param name="pipeline">The spatial pipeline</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
        public override void SendTo(GeographyPipeline pipeline)
        {
            base.SendTo(pipeline);
            pipeline.BeginGeography(SpatialType.Collection);
            for (int i = 0; i < this.geographyArray.Length; ++i)
            {
                this.geographyArray[i].SendTo(pipeline);
            }

            pipeline.EndGeography();
        }
    }
}
