//---------------------------------------------------------------------
// <copyright file="DataServiceActionProviderOverrides.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Contracts
{
    using System;
    using System.Collections.Generic;
#if TESTPROVIDERS
    using Microsoft.OData;
#else
    using Microsoft.Data.OData;
#endif
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// Class used to set specific actions on Data Service Providers
    /// </summary>
    public class DataServiceActionProviderOverrides
    {
        internal Action<DataServiceOperationContext, ServiceAction, object, bool, ODataAction> AdvertiseServiceActionFunc { get; private set; }

        internal Func<ODataAction> OutODataActionAdvertiseServiceFunc { get; private set; }

        internal Func<bool> OutReturnAdvertiseServiceFunc { get; private set; }

        internal Action<DataServiceOperationContext, string> TryResolveServiceActionFunc { get; private set; }

        internal Func<ServiceAction> OutServiceActionTryResolveServiceActionFunc { get; private set; }

        internal Func<bool> OutReturnTryResolveServiceActionFunc { get; private set; }

        internal Func<DataServiceOperationContext, ServiceAction, object[], IDataServiceInvokable> CreateInvokableFunc { get; private set; }

        internal Func<DataServiceOperationContext, IEnumerable<ServiceAction>> GetServiceActionsFunc { get; private set; }

        internal Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>> GetServiceActionsByBindingParameterTypeFunc { get; private set; }

        /// <summary>
        /// Sets the AdvertiseServiceActionFunc and disposes of it when dispose is called
        /// </summary>
        /// <param name="advertiseServiceActionFunc">Advertise Service Action Func</param>
        /// <param name="outODataActionFunc">out OData Action Func</param>
        /// <param name="outReturnFunc">out Return Func</param>
        /// <returns>Disposable of AdvertiseServiceActionFunc</returns>
        public IDisposable WithAdvertiseServiceActionFunc(Action<DataServiceOperationContext, ServiceAction, object, bool, ODataAction> advertiseServiceActionFunc, Func<ODataAction> outODataActionFunc, Func<bool> outReturnFunc)
        {
            this.AdvertiseServiceActionFunc = advertiseServiceActionFunc;
            this.OutODataActionAdvertiseServiceFunc = outODataActionFunc;
            this.OutReturnAdvertiseServiceFunc = outReturnFunc;
            return new WithDisposableAction(() =>
            {
                this.AdvertiseServiceActionFunc = null;
                this.OutODataActionAdvertiseServiceFunc = null;
                this.OutReturnAdvertiseServiceFunc = null;
            });
        }

        /// <summary>
        /// Sets the CreateInvokableFunc and disposes of it when dispose is called
        /// </summary>
        /// <param name="createInvokableFunc">Create Invokable to override with</param>
        /// <returns>Disposable of getInvokableFunc</returns>
        public IDisposable WithCreateInvokableFunc(Func<DataServiceOperationContext, ServiceAction, object[], IDataServiceInvokable> createInvokableFunc)
        {
            this.CreateInvokableFunc = createInvokableFunc;
            return new WithDisposableAction(() => this.CreateInvokableFunc = null);
        }

        /// <summary>
        /// Sets the GetServiceActionsFunc( and disposes of it when dispose is called
        /// </summary>
        /// <param name="getServiceActionsFunc">Service Actions Func</param>
        /// <returns>Disposable of GetServiceActionsFunc</returns>
        public IDisposable WithGetServiceActionsFunc(Func<DataServiceOperationContext, IEnumerable<ServiceAction>> getServiceActionsFunc)
        {
            this.GetServiceActionsFunc = getServiceActionsFunc;
            return new WithDisposableAction(() => this.GetServiceActionsFunc = null);
        }

        /// <summary>
        /// Sets the GetServiceActionsByBindingParameterTypeFunc and disposes of it when dispose is called
        /// </summary>
        /// <param name="getServiceActionsByBindingParameterTypeFunc">GetService Actions By Binding Parameter Type Func</param>
        /// <returns>Disposable of WithGetServiceActionsByBindingParameterTypeFunc</returns>
        public IDisposable WithGetServiceActionsByBindingParameterTypeFunc(Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>> getServiceActionsByBindingParameterTypeFunc)
        {
            this.GetServiceActionsByBindingParameterTypeFunc = getServiceActionsByBindingParameterTypeFunc;
            return new WithDisposableAction(() => this.GetServiceActionsByBindingParameterTypeFunc = null);
        }

        /// <summary>
        /// Sets the TryResolveServiceActionFunc and disposes of it when dispose is called
        /// </summary>
        /// <param name="tryResolveServiceActionFunc">Function that resolves the Service Action</param>
        /// <param name="outServiceActionTryResolveServiceActionFunc">Function that returns the Action resolved</param>
        /// <param name="outReturnTryResolveServiceActionFunc">Function that returns a bool value indicating whether an Action has been found or not</param>
        /// <returns>Disposable of tryResolveServiceActionFunc</returns>
        public IDisposable WithTryResolveServiceActionFunc(Action<DataServiceOperationContext, string> tryResolveServiceActionFunc, Func<ServiceAction> outServiceActionTryResolveServiceActionFunc, Func<bool> outReturnTryResolveServiceActionFunc)
        {
            this.TryResolveServiceActionFunc = tryResolveServiceActionFunc;
            this.OutReturnTryResolveServiceActionFunc = outReturnTryResolveServiceActionFunc;
            this.OutServiceActionTryResolveServiceActionFunc = outServiceActionTryResolveServiceActionFunc;
            return new WithDisposableAction(() =>
            {
                this.TryResolveServiceActionFunc = null;
                this.OutReturnTryResolveServiceActionFunc = null;
                this.OutServiceActionTryResolveServiceActionFunc = null;
            });
        }
    }
}
