//---------------------------------------------------------------------
// <copyright file="IDataServiceWebServerLocatorStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.DataServices.WebServerLocator;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Interface allows developers to ask for a machine that can be used to 
    /// Host a DataService
    /// </summary>
    [ImplementationSelector("WebServerLocatorStrategy", DefaultImplementation = "Default")]
    public interface IDataServiceWebServerLocatorStrategy
    {
        /// <summary>
        /// Gets the Root IQueryable to query over for IISMachines
        /// </summary>
        IQueryable<WebMachine> WebServersQueryRoot { get; }

        /// <summary>
        /// Starts executing a Query against a DataService or a fake one
        /// </summary>
        /// <param name="callback">Function to return to</param>
        /// <param name="query">Query to execute</param>
        /// <returns>Async Result </returns>
        IAsyncResult BeginExecute(AsyncCallback callback, IQueryable<WebMachine> query);

        /// <summary>
        /// Calls end execute to get the list of IISMachines
        /// </summary>
        /// <param name="result">IAsyncResult is take as input</param>
        /// <returns>A List of Machines</returns>
        IEnumerable<WebMachine> EndExecute(IAsyncResult result);
        
        /// <summary>
        /// Updates an IISMachines LastUsed property
        /// </summary>
        /// <param name="updatedTime">Time to update IISMachine LastUsed Property to</param>
        /// <param name="callback">Function to Return to</param>
        /// <param name="machine">IISMachine to update</param>
        /// <returns>an AsyncResult</returns>
        IAsyncResult UpdateMachineLastUsedProperty(DateTime updatedTime, AsyncCallback callback, WebMachine machine);
    }
}