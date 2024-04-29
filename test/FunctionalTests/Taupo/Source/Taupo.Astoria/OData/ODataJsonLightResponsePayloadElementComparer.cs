//---------------------------------------------------------------------
// <copyright file="ODataJsonResponsePayloadElementComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// An implementation of the payload element comparer contract that compares Json payloads.
    /// </summary>
    [ImplementationName(typeof(IODataPayloadElementComparer), "Json")]
    public class ODataJsonResponsePayloadElementComparer : ODataPayloadElementComparer
    {
        /// <summary>
        /// Initializes a new instance of the ODataJsonResponsePayloadElementComparer class.
        /// </summary>
        public ODataJsonResponsePayloadElementComparer()
            : base(/*ignoreOrder*/ false, /*expectMetadataToBeComputedByConvention*/ true)
        {
        }
    }
}