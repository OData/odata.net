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
