//---------------------------------------------------------------------
// <copyright file="ISaveChangesSingleRequestCalculator.cs" company="Microsoft">
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
    /// Contract for calculating the expected request for a single descriptor during SaveChanges.
    /// </summary>
    [ImplementationSelector("SaveChangesSingleRequestCalculator", DefaultImplementation = "Default")]
    public interface ISaveChangesSingleRequestCalculator
    {
        /// <summary>
        /// Calculates expected data for a request during DataServiceContext.SaveChanges for a particular descriptor.
        /// </summary>
        /// <param name="contextData">The context data</param>
        /// <param name="propertyValuesBeforeSave">The property values of the tracked client objects before the call to SaveChanges</param>
        /// <param name="descriptorData">The descriptor data</param>
        /// <param name="options">The save changes options</param>
        /// <returns>The expected client request</returns>
        ExpectedClientRequest CalculateRequest(DataServiceContextData contextData, IDictionary<object, IEnumerable<NamedValue>> propertyValuesBeforeSave, DescriptorData descriptorData, SaveChangesOptions options);
    }
}
