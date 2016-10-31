//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
