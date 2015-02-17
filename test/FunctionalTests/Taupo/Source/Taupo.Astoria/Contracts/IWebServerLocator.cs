//---------------------------------------------------------------------
// <copyright file="IWebServerLocator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Interface allows developers to ask for a machine that can be used to 
    /// Host a DataService
    /// </summary>
    [ImplementationSelector("WebServerLocator", DefaultImplementation = "Default")]
    public interface IWebServerLocator
    {
        /// <summary>
        /// Starts looking for WebServers
        /// </summary>
        /// <param name="machineSearchCriteria">Basic Criteria to find a machine</param>
        /// <param name="astoriaMachineSearchCriteria">Astoria specific criteria to find a machine</param>
        /// <param name="callback">Callback that returns the information</param>
        void BeginLookup(
            MachineSearchCriteria machineSearchCriteria, 
            AstoriaMachineSearchCriteria astoriaMachineSearchCriteria, 
            EventHandler<WebServerLocatorCompleteEventArgs> callback);
    }
}
