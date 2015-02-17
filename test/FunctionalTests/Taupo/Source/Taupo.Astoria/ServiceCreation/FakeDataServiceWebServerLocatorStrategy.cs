//---------------------------------------------------------------------
// <copyright file="FakeDataServiceWebServerLocatorStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ServiceCreation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.DataServices.WebServerLocator;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Allows developers to ask for a machine that can be used to 
    /// Host a DataService
    /// </summary>
    [ImplementationName(typeof(IDataServiceWebServerLocatorStrategy), "Fake")]
    public class FakeDataServiceWebServerLocatorStrategy : IDataServiceWebServerLocatorStrategy
    {
        private List<WebMachine> machines;
        private Exception exceptionToThrowOnEndExecute;

        /// <summary>
        /// Initializes a new instance of the FakeDataServiceWebServerLocatorStrategy class
        /// </summary>
        /// <param name="machines">List of machines</param>
        public FakeDataServiceWebServerLocatorStrategy(IEnumerable<WebMachine> machines)
        {
            this.machines = new List<WebMachine>(machines);
            this.exceptionToThrowOnEndExecute = null;
        }

        /// <summary>
        /// Initializes a new instance of the FakeDataServiceWebServerLocatorStrategy class
        /// </summary>
        /// <param name="machines">List of machines</param>
        /// <param name="exceptionToThrowOnEndExecute"> Exception to throw on EndExecute, used to test this case</param>
        internal FakeDataServiceWebServerLocatorStrategy(List<WebMachine> machines, Exception exceptionToThrowOnEndExecute)
        {
            this.machines = machines;
            this.exceptionToThrowOnEndExecute = exceptionToThrowOnEndExecute;
        }

        /// <summary>
        /// Gets the Root IQueryable to query over for IISMachines
        /// </summary>
        public IQueryable<WebMachine> WebServersQueryRoot
        {
            get { return this.machines.AsQueryable(); }
        }

        /// <summary>
        /// Starts executing a Query against a DataService or a fake one
        /// </summary>
        /// <param name="callback">Function to return to</param>
        /// <param name="query">Query to execute</param>
        /// <returns>Async Result </returns>
        public IAsyncResult BeginExecute(AsyncCallback callback, IQueryable<WebMachine> query)
        {
            FakeAsyncResult result = new FakeAsyncResult(callback, query);
            return result;
        }

        /// <summary>
        /// Calls end execute to get the list of IISMachines
        /// </summary>
        /// <param name="result">AsyncResult that contains the query</param>
        /// <returns>List of IISMachines queried</returns>
        public IEnumerable<WebMachine> EndExecute(IAsyncResult result)
        {
            if (this.exceptionToThrowOnEndExecute != null)
            {
                throw this.exceptionToThrowOnEndExecute;
            }

            return result.AsyncState as IQueryable<WebMachine>;
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
            return new FakeAsyncResult(callback, machine);
        }
    }
}