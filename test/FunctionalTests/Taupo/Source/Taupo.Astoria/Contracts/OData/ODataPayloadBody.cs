//---------------------------------------------------------------------
// <copyright file="ODataPayloadBody.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a serialized payload body for the OData layer
    /// </summary>
    public class ODataPayloadBody
    {
        /// <summary>
        /// Initializes a new instance of the ODataPayloadBody class
        /// </summary>
        /// <param name="serializedValue">The serialized binary representation of the payload</param>
        /// <param name="rootElement">The odata payload element representation of the payload</param>
        public ODataPayloadBody(byte[] serializedValue, ODataPayloadElement rootElement)
        {
            this.SerializedValue = serializedValue;
            this.RootElement = rootElement;
        }

        /// <summary>
        /// Gets the binary content of the request body
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Binary is better")]
        public byte[] SerializedValue { get; private set; }

        /// <summary>
        /// Gets the root payload element
        /// </summary>
        public ODataPayloadElement RootElement { get; private set; }
    }
}
