//---------------------------------------------------------------------
// <copyright file="IBatchPayloadComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for comparing batch payloads
    /// </summary>
    [ImplementationSelector("BatchPayloadComparer", DefaultImplementation = "Default")]
    public interface IBatchPayloadComparer
    {
        /// <summary>
        /// Compare batch payloads
        /// </summary>
        /// <param name="expectedPayload">expected payload</param>
        /// <param name="actualPayload">actual payload</param>
        void CompareBatchPayload(ODataPayloadElement expectedPayload, ODataPayloadElement actualPayload);
    }
}
