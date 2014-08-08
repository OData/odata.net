//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Providers
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.Data.OData;
    #endregion Namespaces

    /// <summary>
    /// This interface declares the methods required to support ServiceActions.
    /// </summary>
    public interface IDataServiceActionProvider
    {
        /// <summary>Return a collection of <see cref="T:System.Data.Services.Providers.ServiceActions" /> instances that represent the service actions implemented by the WCF Data Service. </summary>
        /// <returns>An enumeration of all service actions.</returns>
        /// <param name="operationContext">The data service operation context instance.</param>
        IEnumerable<ServiceAction> GetServiceActions(DataServiceOperationContext operationContext);

        /// <summary>Attempts to retrieve the <see cref="T:System.Data.Services.Providers.ServiceAction" /> instance for the specified service action. </summary>
        /// <returns>true if the resolution is successful; false otherwise.</returns>
        /// <param name="operationContext">The data service operation context instance.</param>
        /// <param name="serviceActionName">The name of the service action to resolve.</param>
        /// <param name="serviceAction">Returns the service action instance if the resolution is successful; null otherwise.</param>
        bool TryResolveServiceAction(DataServiceOperationContext operationContext, string serviceActionName, out ServiceAction serviceAction);
        
        /// <summary> Gets a collection of service actions that match the specified binding parameter type. </summary>
        /// <returns>A collection of <see cref="T:System.Data.Service.Providers.ServiceAction" /> instances. </returns>
        /// <param name="operationContext">The data service operation context instance.</param>
        /// <param name="bindingParameterType">The binding parameter resource type (<see cref="T:System.Data.Services.Providers.ResourceType" />) in question.</param>
        IEnumerable<ServiceAction> GetServiceActionsByBindingParameterType(DataServiceOperationContext operationContext, ResourceType bindingParameterType);

        /// <summary> Creates an instance of <see cref="T:System.Data.Services.Providers.IDataServiceInvokable" /> for the specified service action. </summary>
        /// <returns>An instance of <see cref="T:System.Data.Services.Providers.IDataServiceInvokable" /></returns>
        /// <param name="operationContext">The data service operation context instance.</param>
        /// <param name="serviceAction">The T:System.Data.Services.Providers.ServiceAction instance that represents the service action to invoke.</param>
        /// <param name="parameterTokens">The parameters required to invoke the service action.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "Invokable", Justification = "Invokable is the correct spelling")]
        IDataServiceInvokable CreateInvokable(DataServiceOperationContext operationContext, ServiceAction serviceAction, object[] parameterTokens);

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:AvoidRefParameters", Justification = "ref parameter required")]
        bool AdvertiseServiceAction(DataServiceOperationContext operationContext, ServiceAction serviceAction, object resourceInstance, bool resourceInstanceInFeed, ref ODataAction actionToSerialize);
    }
}
