//---------------------------------------------------------------------
// <copyright file="ISaveChangesServerStateVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Contract for verifying that server state is correct after a call to SaveChanges
    /// </summary>
    [ImplementationSelector("SaveChangesSingleRequestCalculator", DefaultImplementation = "Default")]
    public interface ISaveChangesServerStateVerifier
    {
        /// <summary>
        /// Initializes the expected changes to verify after SaveChanges completes
        /// </summary>
        /// <param name="contextData">The context data</param>
        /// <param name="propertyValues">The property values of the tracked client objects</param>
        void InitializeExpectedChanges(DataServiceContextData contextData, IDictionary<object, IEnumerable<NamedValue>> propertyValues);

        /// <summary>
        /// Verifies that the values on the server are correct
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        /// <param name="contextData">The context data to verify changes for</param>
        void VerifyChangesOnServer(IAsyncContinuation continuation, DataServiceContextData contextData);
    }
}
