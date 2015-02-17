//---------------------------------------------------------------------
// <copyright file="ODataJsonLightResponseOrderIgnoringPayloadElementComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// An implementation of the payload element comparer contract that compares JsonLight payloads and ignores the order of properties.
    /// </summary>
    [ImplementationName(typeof(IODataPayloadElementComparer), "JsonLightNoOrder")]
    public class ODataJsonLightResponseOrderIgnoringPayloadElementComparer : ODataPayloadElementComparer
    {
        /// <summary>
        /// Initializes a new instance of the ODataJsonLightResponseOrderIgnoringPayloadElementComparer class.
        /// </summary>
        public ODataJsonLightResponseOrderIgnoringPayloadElementComparer()
            : base(/*ignoreOrder*/ true, /*expectMetadataToBeComputedByConvention*/ true)
        {
        }
    }
}