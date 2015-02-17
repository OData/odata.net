//---------------------------------------------------------------------
// <copyright file="ISaveChangesResponseCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using Microsoft.Test.Taupo.Astoria.Contracts.Product;
    using Microsoft.Test.Taupo.Astoria.Contracts.Wrappers;
    using Microsoft.Test.Taupo.Common;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Contract for calculating the expected data for the response from DataServiceContext.SaveChanges.
    /// </summary>
    [ImplementationSelector("SaveChangesResponseCalculator", DefaultImplementation = "Default", HelpText = "The calculator of the expected data for the response from DataServiceContext.SaveChanges.")]
    public interface ISaveChangesResponseCalculator
    {
        /// <summary>
        /// Calculates expected data for the response from DataServiceContext.SaveChanges.
        /// </summary>
        /// <param name="dataBeforeSaveChanges">The data before save changes.</param>
        /// <param name="options">The options for saving chnages.</param>
        /// <param name="context">The DataServiceContext instance which is calling SaveChanges.</param>
        /// <returns><see cref="DataServiceResponseData"/> which represents expected data for the response from SaveChanges.</returns>
        DataServiceResponseData CalculateSaveChangesResponseData(DataServiceContextData dataBeforeSaveChanges, SaveChangesOptions options, DSClient.DataServiceContext context);
    }
}
