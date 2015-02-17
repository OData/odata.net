//---------------------------------------------------------------------
// <copyright file="IODataPayloadElementNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Contract for normalizing a payload so that validation can be done independent of format-specific differences
    /// </summary>
    public interface IODataPayloadElementNormalizer
    {
        /// <summary>
        /// Normalizes the given payload root
        /// </summary>
        /// <param name="rootElement">The payload to normalize</param>
        /// <returns>The normalized payload</returns>
        ODataPayloadElement Normalize(ODataPayloadElement rootElement);
    }
}
