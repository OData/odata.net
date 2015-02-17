//---------------------------------------------------------------------
// <copyright file="IDataServiceContextVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for DataServiceContext verification.
    /// </summary>
    [ImplementationSelector("DataServiceContextVerifier", DefaultImplementation = "Default", HelpText = "The verifier for the DataServiceContext.")]
    public interface IDataServiceContextVerifier
    {
        /// <summary>
        /// Verifies the data service context.
        /// </summary>
        /// <param name="contextData">The expected data for the context.</param>
        /// <param name="context">The context to verify.</param>
        void VerifyDataServiceContext(DataServiceContextData contextData, DataServiceContext context);
    }
}
