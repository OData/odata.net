//---------------------------------------------------------------------
// <copyright file="ISaveChangesVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts.Product;
    using Microsoft.Test.Taupo.Common;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Contract for DataServiceContexy.SaveChanges verification.
    /// </summary>
    [ImplementationSelector("SaveChangesVerifier", DefaultImplementation = "HttpDriven", HelpText = "The verifier for DataServiceContext.SaveChanges.")] 
    public interface ISaveChangesVerifier
    {
        /// <summary>
        /// Executes SaveChanges method on the specified context and verifies the results.
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        /// <param name="contextData">The context data.</param>
        /// <param name="context">The context to verify SaveChanges.</param>
        /// <param name="options">The options for saving changes. Passing null will use the context's default.</param>
        /// <param name="onCompletion">Callback for when save changes verification completes</param>
        void VerifySaveChanges(IAsyncContinuation continuation, DataServiceContextData contextData, DSClient.DataServiceContext context, SaveChangesOptions? options, Action<IAsyncContinuation, DSClient.DataServiceResponse> onCompletion);
    }
}
