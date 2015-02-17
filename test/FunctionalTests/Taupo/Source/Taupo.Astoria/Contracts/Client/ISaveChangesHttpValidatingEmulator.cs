//---------------------------------------------------------------------
// <copyright file="ISaveChangesHttpValidatingEmulator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.Product;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Contract for emulating the SaveChanges pipeline, validating the requests sent, and updating the context data state based on the responses
    /// </summary>
    [ImplementationSelector("SaveChangesHttpValidatingEmulator", DefaultImplementation = "Default")] 
    public interface ISaveChangesHttpValidatingEmulator
    {
        /// <summary>
        /// Validates the requests sent, updates the expected state, and produces the expected response data for a single call to SaveChanges
        /// </summary>
        /// <param name="contextData">The context data at the time save-changes was called</param>
        /// <param name="propertyValuesBeforeSave">The property values of the tracked client objects before the call to SaveChanges</param>
        /// <param name="options">The save changes options used</param>
        /// <param name="requestResponsePairs">The observed HTTP traffic during save-changes</param>
        /// <returns>The expected response data</returns>
        DataServiceResponseData ValidateAndTrackChanges(DataServiceContextData contextData, IDictionary<object, IEnumerable<NamedValue>> propertyValuesBeforeSave, SaveChangesOptions options, IEnumerable<KeyValuePair<HttpRequestData, HttpResponseData>> requestResponsePairs);
    }
}
