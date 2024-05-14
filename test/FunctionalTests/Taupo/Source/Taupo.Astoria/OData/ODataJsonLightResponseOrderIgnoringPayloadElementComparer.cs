//---------------------------------------------------------------------
// <copyright file="ODataJsonResponseOrderIgnoringPayloadElementComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// An implementation of the payload element comparer contract that compares Json payloads and ignores the order of properties.
    /// </summary>
    [ImplementationName(typeof(IODataPayloadElementComparer), "JsonNoOrder")]
    public class ODataJsonResponseOrderIgnoringPayloadElementComparer : ODataPayloadElementComparer
    {
        /// <summary>
        /// Initializes a new instance of the ODataJsonResponseOrderIgnoringPayloadElementComparer class.
        /// </summary>
        public ODataJsonResponseOrderIgnoringPayloadElementComparer()
            : base(/*ignoreOrder*/ true, /*expectMetadataToBeComputedByConvention*/ true)
        {
        }
    }
}