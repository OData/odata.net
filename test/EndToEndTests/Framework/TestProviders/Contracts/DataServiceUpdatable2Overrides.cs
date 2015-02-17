//---------------------------------------------------------------------
// <copyright file="DataServiceUpdatable2Overrides.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Contracts
{
    using System;
    using System.Linq;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// Class used to set specific actions on Data Service Providers
    /// </summary>
    public class DataServiceUpdatable2Overrides
    {
        internal Action<IDataServiceInvokable> AddPendingActionsCreateInvokableFunc { get; private set; }

        internal Action<IDataServiceInvokable> ImmediateCreateInvokableFunc { get; private set; }

        internal Func<IQueryable, object> GetResourcesFunc { get; private set; }

        /// <summary>
        /// Sets the WithCreateInvokable and disposes of it when dispose is called
        /// </summary>
        /// <param name="addInvokableFunc">Add Invokable Func</param>
        /// <returns>Disposable of WithGetServiceActionsByBindingParameterTypeFunc</returns>
        public IDisposable WithImmediateCreateInvokable(Action<IDataServiceInvokable> addInvokableFunc)
        {
            this.ImmediateCreateInvokableFunc = addInvokableFunc;
            return new WithDisposableAction(() => this.ImmediateCreateInvokableFunc = null);
        }

        /// <summary>
        /// Sets the WithCreateInvokable and disposes of it when dispose is called
        /// </summary>
        /// <param name="addInvokableFunc">Add Invokable Func</param>
        /// <returns>Disposable of WithGetServiceActionsByBindingParameterTypeFunc</returns>
        public IDisposable WithPendingActionsCreateInvokable(Action<IDataServiceInvokable> addInvokableFunc)
        {
            this.AddPendingActionsCreateInvokableFunc = addInvokableFunc;
            return new WithDisposableAction(() => this.AddPendingActionsCreateInvokableFunc = null);
        }

        /// <summary>
        /// Sets the GetResources Func and disposes of it when dispose is called
        /// </summary>
        /// <param name="getResourcesFunc">Get Resources Func</param>
        /// <returns>Disposable of WithGetServiceActionsByBindingParameterTypeFunc</returns>
        public IDisposable WithGetResources(Func<IQueryable, object> getResourcesFunc)
        {
            this.GetResourcesFunc = getResourcesFunc;
            return new WithDisposableAction(() => this.GetResourcesFunc = null);
        }
    }
}
