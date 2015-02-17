//---------------------------------------------------------------------
// <copyright file="ODataJsonLightResponsePayloadElementComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// An implementation of the payload element comparer contract that compares JsonLight payloads.
    /// </summary>
    [ImplementationName(typeof(IODataPayloadElementComparer), "JsonLight")]
    public class ODataJsonLightResponsePayloadElementComparer : ODataPayloadElementComparer
    {
        /// <summary>
        /// Initializes a new instance of the ODataJsonLightResponsePayloadElementComparer class.
        /// </summary>
        public ODataJsonLightResponsePayloadElementComparer()
            : base(/*ignoreOrder*/ false, /*expectMetadataToBeComputedByConvention*/ true)
        {
        }
    }
}