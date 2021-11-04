//---------------------------------------------------------------------
// <copyright file="JsonProtocolQueryVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Builds and verifies queries using json protocol requests
    /// </summary>
    [ImplementationName(typeof(IQueryVerifier), "JsonProtocol")]
    public class JsonProtocolQueryVerifier : ProtocolQueryVerifier
    {
        /// <summary>
        /// Initializes a new instance of the JsonProtocolQueryVerifier class
        /// </summary>
        /// <param name="service">The data service descriptor.</param>
        /// <param name="verifiers">The resource string verifiers.</param>
        public JsonProtocolQueryVerifier(IAstoriaServiceDescriptor service, IAstoriaStringResourceVerifiers verifiers)
            : base(
                MimeTypes.ApplicationJsonODataLightStreaming,
                service.ServiceUri,
                service.ConceptualModel.GetMaxProtocolVersion(),
                verifiers.SystemDataServicesStringVerifier)
        {
        }
    }
}
