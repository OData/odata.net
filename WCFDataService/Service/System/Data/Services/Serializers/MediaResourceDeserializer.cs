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

namespace System.Data.Services.Serializers
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Implements deserializer for media resources.
    /// </summary>
    internal sealed class MediaResourceDeserializer : Deserializer
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MediaResourceDeserializer"/>.
        /// </summary>
        /// <param name="update">true if we're reading an update operation; false if not.</param>
        /// <param name="dataService">Data service for which the deserializer will act.</param>
        /// <param name="tracker">Tracker to use for modifications.</param>
        /// <param name="requestDescription">The request description to use.</param>
        internal MediaResourceDeserializer(bool update, IDataService dataService, UpdateTracker tracker, RequestDescription requestDescription)
            : base(update, dataService, tracker, requestDescription)
        {
        }
        
        /// <summary>
        /// Gets a value indicating whether the request is Atom
        /// </summary>
        protected override bool IsAtomRequest
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the request is verbose json
        /// </summary>
        protected override bool IsVerboseJsonRequest
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the request is json light
        /// </summary>
        protected override bool IsJsonLightRequest 
        {
            get { return false; }
        }

        /// <summary>
        /// Create the object graph from the given payload and return the top level object.
        /// </summary>
        /// <param name="segmentInfo">Info about the object being created.</param>
        /// <returns>Instance of the object created.</returns>
        protected override object Deserialize(SegmentInfo segmentInfo)
        {
            Debug.Assert(segmentInfo != null, "segmentInfo != null");
            Debug.Assert(
                segmentInfo.TargetKind == RequestTargetKind.MediaResource,
                "The MediaResourceDeserializer only support the MediaResource target kind.");

            return this.Service.OperationContext.RequestMessage.RequestStream;
        }
    }
}
