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
