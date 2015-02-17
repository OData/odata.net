//---------------------------------------------------------------------
// <copyright file="TestDataServiceInvokable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.OptionalProviders
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using Microsoft.Test.OData.Framework.TestProviders.Common;

    /// <summary>
    /// Test Data Service Invokable
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Test.Taupo.DataServices.OptionalProviders.TestDataServiceInvokable.#dataServiceOperationContext", Justification = "will be used when dataServiceOperationContext implements GetService")]
    public class TestDataServiceInvokable : IDataServiceInvokable
    {
        private readonly DataServiceOperationContext dataServiceOperationContext = null;
        private readonly object dataServiceInstance = null;
        private readonly ServiceAction serviceAction = null;
        private readonly object[] parameters = null;
        private object result;
        private bool invokeCalled = false;

        /// <summary>
        /// Initializes a new instance of the TestDataServiceInvokable class
        /// </summary>
        /// <param name="dataServiceInstance">Data Service Instance</param>
        /// <param name="dataServiceOperationContext">Data Service Operation Context</param>
        /// <param name="serviceAction">Service Action</param>
        /// <param name="parameters">Parameters to use</param>
        public TestDataServiceInvokable(object dataServiceInstance, DataServiceOperationContext dataServiceOperationContext, ServiceAction serviceAction, object[] parameters)
        {
            this.dataServiceOperationContext = dataServiceOperationContext;
            this.dataServiceInstance = dataServiceInstance;
            this.serviceAction = serviceAction;
            this.parameters = parameters;
        }

        /// <summary>
        /// Invokes the underlying operation.
        /// </summary>
        public void Invoke()
        {
            ExceptionUtilities.Assert(!this.invokeCalled, "Invoke was already called");
            ExceptionUtilities.CheckObjectNotNull(this.dataServiceInstance, "The data service instance has not been specified");
            this.invokeCalled = true;

            var updateProvider2 = (IDataServiceUpdateProvider2)this.dataServiceOperationContext.GetService(typeof(IDataServiceUpdateProvider2));

            Type serviceType = this.dataServiceInstance.GetType();

            IEnumerable<MethodInfo> serviceActionMethodsWithTheSameName = serviceType.GetMethods().AsEnumerable<MethodInfo>().Where(m => m.Name == this.serviceAction.Name);
            MethodInfo serviceActionMethod = null;
            if (serviceActionMethodsWithTheSameName.Count() == 1)
            {
                serviceActionMethod = serviceActionMethodsWithTheSameName.Single();
            }
            else
            {
                // there exists multiple actions with the same name
                serviceActionMethod = this.RetrieveOverloadedServiceAction(serviceActionMethodsWithTheSameName, this.serviceAction.BindingParameter);
            }
           
            ExceptionUtilities.CheckObjectNotNull(serviceActionMethod, "Method for service operation {0} not found", serviceActionMethod.Name);

            var serviceParameters = serviceActionMethod.GetParameters();

            var resolvedParameters = new List<object>();
            for (int i = 0; i < this.serviceAction.Parameters.Count; i++)
            {
                var currentServiceParameter = serviceParameters[i];
                var currentParameter = this.serviceAction.Parameters[i];
                var currentParameterObject = this.parameters[i];
                object resolvedObject = currentParameterObject;

                if (currentParameter.ParameterType.ResourceTypeKind == ResourceTypeKind.EntityType || currentParameter.ParameterType.ResourceTypeKind == ResourceTypeKind.ComplexType)
                {
                    if (currentParameter.ParameterType.ResourceTypeKind == ResourceTypeKind.EntityType)
                    {
                        currentParameterObject = updateProvider2.GetResource((IQueryable)currentParameterObject, null);
                    }

                    if (currentParameterObject != null)
                    {
                        resolvedObject = updateProvider2.ResolveResource(currentParameterObject);
                    }
                }
                else if (currentParameter.ParameterType.ResourceTypeKind == ResourceTypeKind.Collection)
                {
                    var multiValueType = currentParameter.ParameterType as CollectionResourceType;
                    var itemType = currentServiceParameter.ParameterType.GetGenericArguments();
                    var itemList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
                    if (currentParameterObject != null)
                    {
                        var multiValueComplexObject = (IEnumerable)currentParameterObject;
                        foreach (var itemValue in multiValueComplexObject)
                        {
                            if (multiValueType.ItemType.ResourceTypeKind == ResourceTypeKind.ComplexType)
                            {
                                itemList.Add(updateProvider2.ResolveResource(itemValue));
                            }
                            else
                            {
                                itemList.Add(itemValue);
                            }
                        }

                        resolvedObject = itemList;
                    }
                }
                else if (currentParameter.ParameterType.ResourceTypeKind == ResourceTypeKind.EntityCollection)
                {
                    resolvedObject = currentParameterObject;
                    if (resolvedObject.GetType() != currentServiceParameter.ParameterType)
                    {
                        if (currentServiceParameter.ParameterType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            resolvedObject = Activator.CreateInstance(currentServiceParameter.ParameterType, new object[] { resolvedObject });
                        }
                    }
                }

                resolvedParameters.Add(resolvedObject);
            }

            this.result = serviceActionMethod.Invoke(this.dataServiceInstance, BindingFlags.FlattenHierarchy | BindingFlags.Instance, null, resolvedParameters.ToArray(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the result of the call to Invoke.
        /// </summary>
        /// <returns>The result of the call to Invoke.</returns>
        public object GetResult()
        {
            ExceptionUtilities.Assert(this.invokeCalled, "Invoke must be called already");

            return this.result;
        }

        private MethodInfo RetrieveOverloadedServiceAction(IEnumerable<MethodInfo> overloadedServiceActionMethods, ServiceActionParameter bindingParameterToSearch)
        {
            foreach (MethodInfo mi in overloadedServiceActionMethods)
            {
                ParameterInfo bindingParameter = mi.GetParameters().FirstOrDefault();

                // handle unbound actions
                if (bindingParameter == null && bindingParameterToSearch == null)
                {
                    return mi;
                }

                // find the MethodInfo for the action based on the binding parameter type
                if (bindingParameter != null &&
                    bindingParameterToSearch != null &&
                    bindingParameter.ParameterType == bindingParameterToSearch.ParameterType.InstanceType)
                {
                    return mi;
                }
            }

            return null;
        }
    }
}
