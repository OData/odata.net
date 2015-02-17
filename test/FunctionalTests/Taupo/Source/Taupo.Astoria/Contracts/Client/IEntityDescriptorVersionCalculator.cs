//---------------------------------------------------------------------
// <copyright file="IEntityDescriptorVersionCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for Calculating the DataServiceVersion for an EntityDescriptor
    /// </summary>
    [ImplementationSelector("DataServiceVersionCalculator", DefaultImplementation = "Default", HelpText = "The calculator for DSV for Client CUD tests.")] 
    public interface IEntityDescriptorVersionCalculator
    {
       /// <summary>
        /// Calculates the DataServiceVersion for a particular EntityDescriptor
        /// </summary>
        /// <param name="entityDescriptorData">Entity Descriptor Data</param>
        /// <param name="maxProtocolVersion">The client's max protocol version</param>
        /// <returns>A Data service protocol version</returns>
        DataServiceProtocolVersion CalculateDataServiceVersion(EntityDescriptorData entityDescriptorData, DataServiceProtocolVersion maxProtocolVersion);
    }
}
