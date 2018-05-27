//---------------------------------------------------------------------
// <copyright file="TestDataServiceActionProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.OptionalProviders
{
    using System.Collections.Generic;
    using System.Linq;
#if TESTPROVIDERS
    using Microsoft.OData;
#else
    using Microsoft.Data.OData;
#endif
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using Microsoft.Test.OData.Framework.TestProviders.Common;
    using Microsoft.Test.OData.Framework.TestProviders.Contracts;

    /// <summary>
    /// Implementation of an test DataService Action Provider
    /// </summary>
    public abstract class TestDataServiceActionProvider : IDataServiceActionProvider
    {
        private object dataServiceInstance;
        private IEnumerable<ServiceAction> dataServiceActions;

        /// <summary>
        /// Initializes a new instance of the TestDataServiceActionProvider class
        /// </summary>
        /// <param name="dataServiceInstance">Data Service Instance</param>
        protected TestDataServiceActionProvider(object dataServiceInstance)
        {
            this.dataServiceInstance = dataServiceInstance;
        }

        /// <summary>
        /// Returns all service actions in the provider.
        /// </summary>
        /// <param name="operationContext">operation Context</param>
        /// <returns>An enumeration of all service actions.</returns>
        public IEnumerable<ServiceAction> GetServiceActions(DataServiceOperationContext operationContext)
        {
            if (DataServiceOverrides.ActionProvider.GetServiceActionsFunc != null)
            {
                return DataServiceOverrides.ActionProvider.GetServiceActionsFunc(operationContext);
            }

            return this.GetServiceActionsInternal(operationContext);
        }

        /// <summary>
        /// Tries to find the <see cref="ServiceAction"/> for the given <paramref name="sericeActionName"/>.
        /// </summary>
        /// <param name="operationContext">operation Context</param>
        /// <param name="serviceActionName">The name of the service action to resolve.</param>
        /// <param name="serviceAction">Returns the service action instance if the resolution is successful; null otherwise.</param>
        /// <returns>true if the resolution is successful; false otherwise.</returns>
        public bool TryResolveServiceAction(DataServiceOperationContext operationContext, string serviceActionName, out ServiceAction serviceAction)
        {
            if (DataServiceOverrides.ActionProvider.TryResolveServiceActionFunc != null)
            {
                DataServiceOverrides.ActionProvider.TryResolveServiceActionFunc(operationContext, serviceActionName);
                serviceAction = DataServiceOverrides.ActionProvider.OutServiceActionTryResolveServiceActionFunc();

                return DataServiceOverrides.ActionProvider.OutReturnTryResolveServiceActionFunc();
            }

            serviceAction = this.GetServiceActionsInternal(operationContext).SingleOrDefault(sa => sa.Name == serviceActionName);

            return serviceAction != null;
        }

        /// <summary>
        /// Builds up an instance oz <see cref="IDataServiceInvokable"/> for the given <paramref name="serviceAction"/> with the provided <paramref name="parameterTokens"/>.
        /// </summary>
        /// <param name="operationContext">The data service operation context instance.</param>
        /// <param name="serviceAction">The service action to invoke.</param>
        /// <param name="parameterTokens">The parameter tokens required to invoke the service action.</param>
        /// <returns>An instance of <see cref="IDataServiceInvokable"/> to invoke the action with.</returns>
        public IDataServiceInvokable CreateInvokable(DataServiceOperationContext operationContext, ServiceAction serviceAction, object[] parameterTokens)
        {
            if (DataServiceOverrides.ActionProvider.CreateInvokableFunc != null)
            {
                return DataServiceOverrides.ActionProvider.CreateInvokableFunc(operationContext, serviceAction, parameterTokens);
            }

            return new TestDataServiceInvokable(this.dataServiceInstance, operationContext, serviceAction, parameterTokens);
        }

        /// <summary>
        /// Gets a collection of actions having <paramref name="bindingParameterType"/> as the binding parameter type.
        /// </summary>
        /// <param name="operationContext">The data service operation context instance.</param>
        /// <param name="bindingParameterType">Instance of the binding parameter resource type (<see cref="ResourceType"/>) in question.</param>
        /// <returns>A list of actions having <paramref name="bindingParameterType"/> as the binding parameter type.</returns>
        public IEnumerable<ServiceAction> GetServiceActionsByBindingParameterType(DataServiceOperationContext operationContext, ResourceType bindingParameterType)
        {
            if (DataServiceOverrides.ActionProvider.GetServiceActionsByBindingParameterTypeFunc != null)
            {
                return DataServiceOverrides.ActionProvider.GetServiceActionsByBindingParameterTypeFunc(operationContext, bindingParameterType);
            }

            return this.GetServiceActionsInternal(operationContext).Where(sa => sa.BindingParameter != null && sa.BindingParameter.ParameterType.FullName == bindingParameterType.FullName);
        }

        /// <summary>
        /// Determines whether a given <paramref name="serviceAction"/> should be advertised as bindable to the given <paramref name="resourceInstance"/>.
        /// </summary>
        /// <param name="operationContext">The data service operation context instance.</param>
        /// <param name="serviceAction">Service action to be advertised.</param>
        /// <param name="resourceInstance">Instance of the resource to which the service action is bound.</param>
        /// <param name="resourceInstanceInFeed">true if the resource instance to be serialized is inside a feed; false otherwise. The value true
        /// suggests that this method might be called many times during serialization since it will get called once for every resource instance inside
        /// the feed. If it is an expensive operation to determine whether to advertise the service action for the <paramref name="resourceInstance"/>,
        /// the provider may choose to always advertise in order to optimize for performance.</param>
        /// <param name="actionToSerialize">The <see cref="ODataAction"/> to be serialized. The server constructs 
        /// the version passed into this call, which may be replaced by an implementation of this interface.
        /// This should never be set to null unless returning false.</param>
        /// <returns>true if the service action should be advertised; false otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "operationContext", Justification = "parameter is required")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "serviceAction", Justification = "parameter is required")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "resourceInstance", Justification = "parameter is required")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "actionToSerialize", Justification = "parameter is required")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:AvoidRefParameters", Justification = "ref parameter required")]
        public bool AdvertiseServiceAction(DataServiceOperationContext operationContext, ServiceAction serviceAction, object resourceInstance, bool resourceInstanceInFeed, ref ODataAction actionToSerialize)
        {
            if (DataServiceOverrides.ActionProvider.AdvertiseServiceActionFunc != null)
            {
                DataServiceOverrides.ActionProvider.AdvertiseServiceActionFunc(operationContext, serviceAction, resourceInstance, resourceInstanceInFeed, actionToSerialize);

                if (DataServiceOverrides.ActionProvider.OutODataActionAdvertiseServiceFunc != null)
                {
                    actionToSerialize = DataServiceOverrides.ActionProvider.OutODataActionAdvertiseServiceFunc();
                }

                return DataServiceOverrides.ActionProvider.OutReturnAdvertiseServiceFunc();
            }

            return true;
        }

        /// <summary>
        /// Loads action providers service actions
        /// </summary>
        /// <param name="operationContext">operation Context</param>
        /// <returns>Enumerable of Service Actions</returns>
        internal IEnumerable<ServiceAction> GetServiceActionsInternal(DataServiceOperationContext operationContext)
        {
            if (this.dataServiceActions == null)
            {
                var metadataProvider = (IDataServiceMetadataProvider)operationContext.GetService(typeof(IDataServiceMetadataProvider));
                this.dataServiceActions = this.LoadServiceActions(metadataProvider).ToArray();
            }

            return this.dataServiceActions;
        }

        /// <summary>
        /// Wrapper methods around metadata provider to get the ResourceType
        /// </summary>
        /// <param name="metadataProvider">metadata provider</param>
        /// <param name="typeName">type name</param>
        /// <returns>a resource type or throws</returns>
        protected ResourceType GetResourceType(IDataServiceMetadataProvider metadataProvider, string typeName)
        {
            ResourceType resourceType = null;

            ProviderImplementationSettings.Override(
                s => s.EnforceMetadataCaching = false,
                () => ExceptionUtilities.Assert(metadataProvider.TryResolveResourceType(typeName, out resourceType), "Cannot find a resource type '{0}' in the metadata provider", typeName));

            return resourceType;
        }

        /// <summary>
        /// Wrapper methods around metadata provider to get the ResourceSet
        /// </summary>
        /// <param name="metadataProvider">metadata provider</param>
        /// <param name="setName">set name</param>
        /// <returns>a resource set or throws</returns>
        protected ResourceSet GetResourceSet(IDataServiceMetadataProvider metadataProvider, string setName)
        {
            ResourceSet resourceSet = null;

            ProviderImplementationSettings.Override(
               s => s.EnforceMetadataCaching = false,
               () => ExceptionUtilities.Assert(metadataProvider.TryResolveResourceSet(setName, out resourceSet), "Cannot find a resource set '{0}' in the metadata provider", setName));

            return resourceSet;
        }

        /// <summary>
        /// Loads action providers service actions
        /// </summary>
        /// <param name="dataServiceMetadataProvider">Data Service Metadata Provider</param>
        /// <returns>Enumerable of Service Actions</returns>
        protected abstract IEnumerable<ServiceAction> LoadServiceActions(IDataServiceMetadataProvider dataServiceMetadataProvider);
    }
}
