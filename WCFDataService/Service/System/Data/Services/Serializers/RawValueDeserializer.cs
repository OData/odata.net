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
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData;

    /// <summary>
    /// Implements deserializer for raw values.
    /// </summary>
    internal sealed class RawValueDeserializer : ODataMessageReaderDeserializer
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RawValueDeserializer"/>.
        /// </summary>
        /// <param name="update">true if we're reading an update operation; false if not.</param>
        /// <param name="dataService">Data service for which the deserializer will act.</param>
        /// <param name="tracker">Tracker to use for modifications.</param>
        /// <param name="requestDescription">The request description to use.</param>
        internal RawValueDeserializer(bool update, IDataService dataService, UpdateTracker tracker, RequestDescription requestDescription)
            : base(update, dataService, tracker, requestDescription, true /*enableWcfDataServicesServerBehavior*/)
        {
        }

        /// <summary>
        /// Reads the input request payload and returns the WCF DS value representation of it.
        /// </summary>
        /// <param name="segmentInfo">Info about the request to read.</param>
        /// <returns>The WCF DS representation of the value read.</returns>
        protected override object Read(SegmentInfo segmentInfo)
        {
            Debug.Assert(segmentInfo != null, "segmentInfo != null");
            Debug.Assert(
                segmentInfo.TargetKind == RequestTargetKind.PrimitiveValue || segmentInfo.TargetKind == RequestTargetKind.OpenPropertyValue,
                "The RawValuDeserializer only supports PrimitiveValue and OpenPropertyValue target kinds.");

            ResourceProperty property = segmentInfo.ProjectedProperty;
            ResourceType propertyResourceType;
            IEdmTypeReference propertyTypeReference;
            if (property == null)
            {
                propertyResourceType = null;
                propertyTypeReference = null;
            }
            else
            {
                propertyResourceType = property.ResourceType;
                propertyTypeReference = this.GetTypeReference(propertyResourceType, property.CustomAnnotations.ToList());
            }

            object rawValue = this.MessageReader.ReadValue(propertyTypeReference);
            Debug.Assert(rawValue != null, "The raw value can never be null.");
            AssertReaderFormatIsExpected(this.MessageReader, ODataFormat.RawValue);

            object convertedValue = ODataMessageReaderDeserializer.ConvertPrimitiveValue(rawValue, ref propertyResourceType);
            if (property == null)
            {
                // Set the target resource type for open properties so we can reason over the property type later.
                // The target resource type of an open property segment is determined when converting the payload value above.
                Debug.Assert(segmentInfo.TargetResourceType == null, "segmentInfo.TargetResourceType == null");
                segmentInfo.TargetResourceType = propertyResourceType;
            }
            else
            {
                Debug.Assert(segmentInfo.TargetResourceType != null, "segmentInfo.TargetResourceType != null");
            }

            return convertedValue;
        }
    }
}
