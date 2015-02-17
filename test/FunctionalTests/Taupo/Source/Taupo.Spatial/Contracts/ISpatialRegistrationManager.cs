//---------------------------------------------------------------------
// <copyright file="ISpatialRegistrationManager.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial.Contracts
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for handling registration of spatial operations
    /// </summary>
    [ImplementationSelector("SpatialRegistrationManager", DefaultImplementation = "Minimal")]
    public interface ISpatialRegistrationManager
    {
        /// <summary>
        /// Registers a set of operations based on which manager is in use.
        /// </summary>
        void RegisterOperations();
    }
}
