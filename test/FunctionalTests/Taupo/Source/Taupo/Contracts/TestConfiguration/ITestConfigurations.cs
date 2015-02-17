//---------------------------------------------------------------------
// <copyright file="ITestConfigurations.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.TestConfiguration
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// Represents a interface that can get testconfigurations from a source
    /// </summary>
    public interface ITestConfigurations
    {
        /// <summary>
        /// Gets Test Configurations
        /// </summary>
        /// <returns>List of Test Configurations</returns>
        IEnumerable<TestConfigurationMatrix> GetConfigurations();
    }
}
