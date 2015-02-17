//---------------------------------------------------------------------
// <copyright file="IDataServiceResponseVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for the DataServiceResponse verification.
    /// </summary>
    [ImplementationSelector("DataServiceResponseVerifier", DefaultImplementation = "Default", HelpText = "The verifier for the data service response.")] 
    public interface IDataServiceResponseVerifier
    {
        /// <summary>
        /// Verifies the data service response.
        /// </summary>
        /// <param name="responseData">The expected data for the response.</param>
        /// <param name="response">The response to verify.</param>
        /// <param name="cachedOperationsFromResponse">The individual operation responses, pre-enumerated and cached</param>
        void VerifyDataServiceResponse(DataServiceResponseData responseData, DataServiceResponse response, IList<OperationResponse> cachedOperationsFromResponse);
    }
}
