//---------------------------------------------------------------------
// <copyright file="ODataOrderIgnoringPayloadElementComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// An implementation of the payload element comparer contract that ignores the order of properties.
    /// </summary>
    [ImplementationName(typeof(IODataPayloadElementComparer), "NoOrder")]
    public class ODataOrderIgnoringPayloadElementComparer : ODataPayloadElementComparer
    {
        /// <summary>
        /// Initializes a new instance of the ODataOrderIgnoringPayloadElementComparer class.
        /// </summary>
        public ODataOrderIgnoringPayloadElementComparer()
            : base(/*ignoreOrder*/ true, /*expectMetadataToBeComputedByConvention*/ false)
        {
        }
    }
}
