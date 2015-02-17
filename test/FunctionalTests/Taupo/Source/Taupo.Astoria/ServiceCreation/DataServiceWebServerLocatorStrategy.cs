//---------------------------------------------------------------------
// <copyright file="DataServiceWebServerLocatorStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ServiceCreation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.DataServices.WebServerLocator;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Allows developers to ask for a machine that can be used to 
    /// Host a DataService
    /// </summary>
    [ImplementationName(typeof(IDataServiceWebServerLocatorStrategy), "Default")]
    public class DataServiceWebServerLocatorStrategy : IDataServiceWebServerLocatorStrategy
    {
        private WebMachinesEntities machineEntities;

        /// <summary>
        /// Initializes a new instance of the DataServiceWebServerLocatorStrategy class
        /// using the provided uri as location of the data service
        /// </summary>
        /// <param name="dataServiceUri">The uri of the IISMachine DataService to connect to</param>
        public DataServiceWebServerLocatorStrategy(Uri dataServiceUri)
        {
            this.DataServiceUri = dataServiceUri;
        }

        /// <summary>
        /// Gets the Root IQueryable to query over for IISMachines
        /// </summary>
        public IQueryable<WebMachine> WebServersQueryRoot
        {
            get { return this.IISMachineEntities.WebMachines; }
        }

        private Uri DataServiceUri { get; set; }

        private WebMachinesEntities IISMachineEntities
        {
            get
            {
                if (this.machineEntities == null)
                {
                    this.machineEntities = this.CreateIISMachinesEntities();
                }

                return this.machineEntities;
            }
        }

        /// <summary>
        /// Starts executing a Query against a DataService or a fake one
        /// </summary>
        /// <param name="callback">Function to return to</param>
        /// <param name="query">Query to execute</param>
        /// <returns>Async Result </returns>
        public IAsyncResult BeginExecute(AsyncCallback callback, IQueryable<WebMachine> query)
        {
            return (query as DataServiceQuery<WebMachine>).BeginExecute(callback, query);
        }

        /// <summary>
        /// Calls end execute to get the list of IISMachines
        /// </summary>
        /// <param name="result">AsyncResult that contains the query</param>
        /// <returns>List of IISMachines queried</returns>
        public IEnumerable<WebMachine> EndExecute(IAsyncResult result)
        {
            return (result.AsyncState as DataServiceQuery<WebMachine>).EndExecute(result);
        }

        /// <summary>
        /// Updates an IISMachines LastUsed property
        /// </summary>
        /// <param name="updatedTime">DateTime to update LastUsed Property to</param>
        /// <param name="callback">Function to return to</param>
        /// <param name="machine">IISMachine to Update LastUsed property</param>
        /// <returns>An AsynResult</returns>
        public IAsyncResult UpdateMachineLastUsedProperty(DateTime updatedTime, AsyncCallback callback, WebMachine machine)
        {
            machine.LastUsed = updatedTime;
            this.IISMachineEntities.UpdateObject(machine);
            return this.IISMachineEntities.BeginSaveChanges(callback, machine);
        }

        private WebMachinesEntities CreateIISMachinesEntities()
        {
            WebMachinesEntities entities = new WebMachinesEntities(this.DataServiceUri);
            return entities;
        }
    }
}