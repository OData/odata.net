//---------------------------------------------------------------------
// <copyright file="MediaResourceDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
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
