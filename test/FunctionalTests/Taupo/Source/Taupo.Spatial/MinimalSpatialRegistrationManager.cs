//---------------------------------------------------------------------
// <copyright file="MinimalSpatialRegistrationManager.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial
{
    using System;
    using Microsoft.Spatial;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Spatial.Common;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// Spatial registration manager which provides only a minimal set of operations
    /// </summary>
    [ImplementationName(typeof(ISpatialRegistrationManager), "Minimal")]
    public class MinimalSpatialRegistrationManager : ISpatialRegistrationManager
    {
        private static readonly SpatialOperations operations = new PseudoDistanceImplementation();

        /// <summary>
        /// Registers a set of operations based on which manager is in use.
        /// </summary>
        public void RegisterOperations()
        {
            SpatialImplementation.CurrentImplementation.Operations = operations;
        }
    }
}