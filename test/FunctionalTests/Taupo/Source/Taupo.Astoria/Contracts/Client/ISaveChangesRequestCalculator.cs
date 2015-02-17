//---------------------------------------------------------------------
// <copyright file="ISaveChangesRequestCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.Product;
    using Microsoft.Test.Taupo.Common;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Contract for calculating the expected data for the requests from DataServiceContext.SaveChanges.
    /// </summary>
    [ImplementationSelector("SaveChangesRequestCalculator", DefaultImplementation = "Default", HelpText = "The calculator of the expected data for the requests from DataServiceContext.SaveChanges.")]
    public interface ISaveChangesRequestCalculator
    {
        /// <summary>
        /// Calculates expected data for the requests from DataServiceContext.SaveChanges.
        /// </summary>
        /// <param name="dataBeforeSaveChanges">The data before save changes.</param>
        /// <param name="context">The DataServiceContext instance which is calling SaveChanges.</param>
        /// <param name="options">The options for saving changes.</param>
        /// <param name="cachedOperationsFromResponse">The individual operation responses from the response, pre-enumerated and cached.</param>
        /// <returns>The expected set of requests for the call to SaveChanges.</returns>
        IEnumerable<HttpRequestData> CalculateSaveChangesRequestData(DataServiceContextData dataBeforeSaveChanges, DSClient.DataServiceContext context, SaveChangesOptions options, IEnumerable<DSClient.OperationResponse> cachedOperationsFromResponse);
    }
}
