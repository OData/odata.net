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
    /// This interface is intended to extend <see cref="IDataServiceActionProvider"/> and add additional information for resolving service actions during URI parsing.
    /// </summary>
    public interface IDataServiceActionResolver
    {
        /// <summary>
        /// Tries to find the <see cref="ServiceAction"/> for the given resolution arguments.
        /// </summary>
        /// <param name="operationContext">The data service operation context instance.</param>
        /// <param name="resolverArgs">The arguments to use when resolving the action.</param>
        /// <param name="serviceAction">Returns the service action instance if the resolution is successful; null otherwise.</param>
        /// <returns>true if the resolution is successful; false otherwise.</returns>
        bool TryResolveServiceAction(DataServiceOperationContext operationContext, ServiceActionResolverArgs resolverArgs, out ServiceAction serviceAction);
    }

    /// <summary>
    /// Provides information for an attempt to resolve a specific service action during URI parsing.
    /// </summary>
    public class ServiceActionResolverArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ServiceActionResolverArgs"/>.
        /// </summary>
        /// <param name="serviceActionName"> The service action name taken from the URI.</param>
        /// <param name="bindingType">The binding type based on interpreting the URI preceeding the action, or null if the action is being invoked from the root of the service.</param>
        public ServiceActionResolverArgs(string serviceActionName, ResourceType bindingType)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(serviceActionName, "serviceActionName");
            this.ServiceActionName = serviceActionName;

            // binding type is explicitly allowed to be null.
            this.BindingType = bindingType;
        }

        /// <summary>
        /// The service action name taken from the URI.
        /// </summary>
        public string ServiceActionName { get; private set; }

        /// <summary>
        /// The binding type based on interpreting the URI preceeding the action, or null if the action is being invoked from the root of the service.
        /// </summary>
        public ResourceType BindingType { get; private set; }
    }
}
