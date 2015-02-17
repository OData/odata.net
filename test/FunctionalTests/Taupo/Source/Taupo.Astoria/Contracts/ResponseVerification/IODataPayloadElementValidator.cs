//---------------------------------------------------------------------
// <copyright file="IODataPayloadElementValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Arguments to the OData payload element data validator contract
    /// </summary>
    [ImplementationSelector("PayloadElementValidator", DefaultImplementation = "Default")]
    public interface IODataPayloadElementValidator
    {
        /// <summary>
        /// Gets or sets the expected protocol version of the payload
        /// </summary>
        DataServiceProtocolVersion ExpectedProtocolVersion { get; set; }

        /// <summary>
        /// Gets or sets the primitive comparer to use
        /// </summary>
        IQueryScalarValueToClrValueComparer PrimitiveValueComparer { get; set; }

        /// <summary>
        /// Gets or sets the expected payload options
        /// </summary>
        ODataPayloadOptions ExpectedPayloadOptions { get; set; }

        /// <summary>
        /// Validates a payload with the given root element against the given expected value
        /// </summary>
        /// <param name="rootElement">The root element of the payload</param>
        /// <param name="expectedValue">The expected value</param>
        void Validate(ODataPayloadElement rootElement, QueryValue expectedValue);
    }
}
