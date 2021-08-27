//---------------------------------------------------------------------
// <copyright file="IDataServiceContextRequestsVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for the verifying requests sent by the data service context.
    /// </summary>
    [ImplementationSelector("DataServiceContextRequestsVerifier", DefaultImplementation = "Default", HelpText = "The verifier for the requests sent by the data service context.")] 
    public interface IDataServiceContextRequestsVerifier
    {
        /// <summary>
        /// Verifies the requests sent by the data service context.
        /// </summary>
        /// <param name="expected">The expected request data.</param>
        /// <param name="observed">The observed requests.</param>
        void VerifyRequests(IEnumerable<HttpRequestData> expected, IEnumerable<SendingRequest2EventArgs> observed);
    }
}
