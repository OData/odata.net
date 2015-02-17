//---------------------------------------------------------------------
// <copyright file="IEntityModelVersionCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Component that Calculates the DataServiceVersion based on the EntityModelSchema
    /// </summary>
    [ImplementationSelector("EntityModelVersionCalculator", DefaultImplementation = "Default")]
    public interface IEntityModelVersionCalculator
    {
        /// <summary>
        /// Calculates the DataServiceVersion based on the EntityModelSchema
        /// </summary>
        /// <param name="entityModelSchema">Entity Model Schema</param>
        /// <returns>Data Service Protocol Version</returns>
        DataServiceProtocolVersion CalculateProtocolVersion(EntityModelSchema entityModelSchema);
    }
}