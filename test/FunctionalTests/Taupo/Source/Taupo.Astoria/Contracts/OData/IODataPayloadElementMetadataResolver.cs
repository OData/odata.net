//---------------------------------------------------------------------
// <copyright file="IODataPayloadElementMetadataResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for resolving the metadata for payload elements and annotating them with the information
    /// </summary>
    [ImplementationSelector("ODataPayloadElementMetadataResolver", DefaultImplementation = "Default")]
    public interface IODataPayloadElementMetadataResolver
    {
        /// <summary>
        /// Annotates the given payload based on the metadata in the given uri
        /// </summary>
        /// <param name="rootElement">The payload to annotate with metadata information</param>
        /// <param name="uri">The uri that corresponds to the given payload</param>
        void ResolveMetadata(ODataPayloadElement rootElement, ODataUri uri);
    }
}
