//---------------------------------------------------------------------
// <copyright file="DSPActionProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using Microsoft.OData;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Diagnostics;
    using System.Collections;
    using System.Threading;
    using BindingFlags = System.Reflection.BindingFlags;
    #endregion Namespaces

    /// <summary>
    /// This interface declares the methods required to support ServiceActions.
    /// </summary>
    public class DSPActionProvider : IDataServiceActionProvider
    {
        /// <summary>
        /// Dictionary of name and service action pairs.
        /// </summary>
        private readonly Dictionary<string, ServiceAction> serviceActions = new Dictionary<string, ServiceAction>();

        /// <summary>
        /// Dictionary of name and <see cref="DSPServiceActionInfo"/> pairs for each service action.
        /// </summary>
        private Dictionary<string, DSPServiceActionInfo> serviceActionInfos = new Dictionary<string, DSPServiceActionInfo>();

        /// <summary>
        /// Callback to be called when DSPInvokableAction.Invoke() is called.
        /// This is added for unit testing purpose so we know the correct sequence of calls happened in the server.
        /// </summary>
        public Action<object[]> ServiceActionInvokeCallback;

        /// <summary>
        /// Callback to be called when DSPInvokableAction.GetResult() is called.
        /// This is added for unit testing purpose so we know the correct sequence of calls happened in the server.
        /// </summary>
        public Action ServiceActionGetResultCallback;

        #region IDataServiceActionProvider

        /// <summary>
        /// Returns all service actions in the provider.
        /// </summary>
        /// <param name="operationContext">The data service operation context instance.</param>
        /// <returns>An enumeration of all service actions.</returns>
        public virtual IEnumerable<ServiceAction> GetServiceActions(DataServiceOperationContext operationContext)
        {
            this.InitializeServiceActions(operationContext);
            return this.serviceActions.Values;
        }

        /// <summary>
        /// Tries to find the <see cref="ServiceAction"/> for the given <paramref name="serviceActionName"/>.
        /// </summary>
        /// <param name="operationContext">The data service operation context instance.</param>
        /// <param name="serviceActionName">The name of the service action to resolve.</param>
        /// <param name="serviceAction">Returns the service action instance if the resolution is successful; null otherwise.</param>
        /// <returns>true if the resolution is successful; false otherwise.</returns>
        public virtual bool TryResolveServiceAction(DataServiceOperationContext operationContext, string serviceActionName, out ServiceAction serviceAction)
        {
            this.InitializeServiceActions(operationContext);
            return this.serviceActions.TryGetValue(serviceActionName, out serviceAction);
        }

        /// <summary>
        /// Gets a collection of actions having <paramref name="bindingParameterType"/> as the binding parameter type.
        /// </summary>
        /// <param name="operationContext">The data service operation context instance.</param>
        /// <param name="bindingParameterType">Instance of the binding parameter resource type (<see cref="ResourceType"/>) in question.</param>
        /// <returns>A list of actions having <paramref name="bindingParameterType"/> as the binding parameter type.</returns>
        public virtual IEnumerable<ServiceAction> GetServiceActionsByBindingParameterType(DataServiceOperationContext operationContext, ResourceType bindingParameterType)
        {
            this.InitializeServiceActions(operationContext);
            return this.serviceActions.Values.Where(a => a.BindingParameter != null && a.BindingParameter.ParameterType == bindingParameterType);
        }

        /// <summary>
        /// Builds up an instance of <see cref="IDataServiceInvokable"/> for the given <paramref name="serviceAction"/> with the provided <paramref name="parameterTokens"/>.
        /// </summary>
        /// <param name="operationContext">The data service operation context instance.</param>
        /// <param name="serviceAction">The service action to invoke.</param>
        /// <param name="parameterTokens">The parameter tokens required to invoke the service action.</param>
        /// <returns>An instance of <see cref="IDataServiceInvokable"/> to invoke the action with.</returns>
        public virtual IDataServiceInvokable CreateInvokable(DataServiceOperationContext operationContext, ServiceAction serviceAction, object[] parameterTokens)
        {
            // This code is required by a test verifying that we can call GetQueryStringValue in an action provider method.
            var forceErrorValue = operationContext.GetQueryStringValue("Query-String-Header-Force-Error");
            if (forceErrorValue == "yes")
            {
                throw new DataServiceException(418, "User code threw a Query-String-Header-Force-Error exception.");
            }
            
            return new DSPInvokableAction(operationContext, serviceAction, parameterTokens, this.ServiceActionInvokeCallback, this.ServiceActionGetResultCallback);
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
        public virtual bool AdvertiseServiceAction(DataServiceOperationContext operationContext, ServiceAction serviceAction, object resourceInstance, bool resourceInstanceInFeed, ref ODataAction actionToSerialize)
        {
            return true;
        }

        #endregion IDataServiceActionProvider
        
        /// <summary>
        /// Initializes a new <see cref="ServiceAction"/> instance.
        /// </summary>
        /// <param name="method">The methodInfo for the operation.</param>
        /// <param name="instance">The instance on which to invoke the operation.</param>
        public void AddAction(MethodInfo method, object instance = null)
        {
            DSPActionAttribute actionAttribute = method.GetCustomAttributes(typeof(DSPActionAttribute), true /*inherit*/).OfType<DSPActionAttribute>().FirstOrDefault();
            if (actionAttribute == null)
            {
                throw new InvalidOperationException("The method '" + method.Name + "' must contain a DSPActionAttribute.");
            }

            DSPServiceActionInfo actionInfo = new DSPServiceActionInfo { ActionAttribute = actionAttribute, Method = method, Instance = instance };
            this.serviceActionInfos.Add(method.Name, actionInfo);
        }
        
        /// <summary>
        /// Initializes a new <see cref="ServiceAction"/> instance.
        /// </summary>
        /// <param name="name">name of the action.</param>
        /// <param name="returnType">Return type of the action.</param>
        /// <param name="resultSet">EntitySet of the result expected from this action.</param>
        /// <param name="parameters">In-order parameters for this action.</param>
        /// <param name="operationParameterBindingKind">the kind of the operation parameter binding (Never, Sometimes, Always).</param>
        /// <param name="instance">The instance on which to invoke the operation.</param>
        /// <param name="method">The methodInfo for the operation.</param>
        public ServiceAction AddAction(
            string name, 
            ResourceType returnType, 
            ResourceSet resultSet, 
            IEnumerable<ServiceActionParameter> parameters, 
            OperationParameterBindingKind operationParameterBindingKind, 
            object instance = null, 
            MethodInfo method = null)
        {
            ServiceAction action = new ServiceAction(name, returnType, resultSet, operationParameterBindingKind, parameters); 
            return this.AddServiceAction(action, method, instance);
        }

        /// <summary>
        /// Initializes a new <see cref="ServiceAction"/> instance.
        /// </summary>
        /// <param name="name">name of the action.</param>
        /// <param name="returnType">Return type of the action.</param>
        /// <param name="pathExpression">Path expression to calculate the result resource set of the function if the action returns an entity or a collection of entity; null otherwise.</param>
        /// <param name="parameters">In-order parameters for this action.</param>
        /// <param name="instance">The instance on which to invoke the operation.</param>
        /// <param name="method">The methodInfo for the operation.</param>
        public ServiceAction AddAction(string name, ResourceType returnType, ResourceSetPathExpression pathExpression, IEnumerable<ServiceActionParameter> parameters, object instance = null, MethodInfo method = null)
        {
            OperationParameterBindingKind kind = OperationParameterBindingKind.Sometimes;
            if (instance != null)
            {
                Enum.TryParse(instance.ToString(), true, out kind);
            }

            ServiceAction action = new ServiceAction(name, returnType, kind, parameters, pathExpression);
            return this.AddServiceAction(action, method, instance);
        }

        /// <summary>
        /// Adds the <see cref="ServiceAction"/> to the list of service actions.
        /// </summary>
        /// <param name="action">the service action to be added.</param>
        /// <returns>the added service action.</returns>
        public ServiceAction AddAction(ServiceAction action)
        {
            return this.AddServiceAction(action, null, null);
        }

        /// <summary>
        /// Adds an service action to the internal dictionary.
        /// </summary>
        /// <param name="action">Service action to add.</param>
        /// <param name="method">Service action method.</param>
        /// <param name="instance">Instance object where the service action method is to be invoked.</param>
        /// <returns></returns>
        private ServiceAction AddServiceAction(ServiceAction action, MethodInfo method, object instance)
        {
            if (method != null)
            {
                action.CustomState = new DSPServiceActionInfo { Instance = instance, Method = method };
            }

            action.SetReadOnly();
            this.serviceActions.Add(action.Name, action);
            return action;
        }
        
        /// <summary>
        /// Initializes all service actions in the provider.
        /// </summary>
        /// <param name="operationContext">The operation context instance of the request.</param>
        private void InitializeServiceActions(DataServiceOperationContext operationContext)
        {
            if (operationContext == null)
            {
                throw new DataServiceException("operationContext must not be null!");
            }

            var actionInfos = Interlocked.Exchange(ref this.serviceActionInfos, new Dictionary<string, DSPServiceActionInfo>());
            if (actionInfos.Count > 0)
            {
                IDataServiceMetadataProvider metadataProvider = (IDataServiceMetadataProvider)operationContext.GetService(typeof(IDataServiceMetadataProvider));
                if (metadataProvider == null)
                {
                    throw new DataServiceException("DataServiceOperationContext.GetService(typeof(IDataServiceMetadataProvider)) must return a valid instance of the IDataServiceMetadataProvider.");
                }

                foreach (var entry in actionInfos)
                {
                    DSPServiceActionInfo actionInfo = entry.Value;
                    DSPActionAttribute actionAttribute = actionInfo.ActionAttribute;
                    MethodInfo actionMethodInfo = actionInfo.Method;

                    ResourceType returnType = DSPActionProvider.GetResourceTypeFromType(metadataProvider, actionInfo.Method.ReturnType, actionAttribute.ReturnElementTypeName);
                    var parameters = DSPActionProvider.GetServiceActionParameters(metadataProvider, actionAttribute, actionMethodInfo);

                    ServiceAction action;
                    if (!string.IsNullOrEmpty(actionAttribute.ReturnSetPath))
                    {
                        if(actionAttribute.OperationParameterBindingKind != OperationParameterBindingKind.Always && actionAttribute.OperationParameterBindingKind != OperationParameterBindingKind.Sometimes)
                        {
                            throw new DataServiceException("DSPActionAttribute.IsBindable must be true when DSPActionAttribute.ReturnSetPath is not null.");
                        }

                        ResourceSetPathExpression pathExpression = new ResourceSetPathExpression(actionAttribute.ReturnSetPath);
                        action = new ServiceAction(actionMethodInfo.Name, returnType, OperationParameterBindingKind.Sometimes, parameters, pathExpression);
                    }
                    else
                    {
                        ResourceSet returnSet = null;
                        if (!string.IsNullOrEmpty(actionAttribute.ReturnSet))
                        {
                            metadataProvider.TryResolveResourceSet(actionAttribute.ReturnSet, out returnSet);
                        }

                        action = new ServiceAction(actionMethodInfo.Name, returnType, returnSet, actionAttribute.OperationParameterBindingKind, parameters);
                    }

                    action.CustomState = actionInfo;
                    action.SetReadOnly();
                    this.serviceActions.Add(actionMethodInfo.Name, action);
                }
            }
        }

        /// <summary>
        /// Creates the service action parameters for the given action method info.
        /// </summary>
        /// <param name="metadataProvider">An instance of the IDataServiceMetadataProvider.</param>
        /// <param name="actionAttribute">DSPActionAttribute instance for the action.</param>
        /// <param name="actionMethodInfo">MethodInfo for the action.</param>
        /// <returns></returns>
        private static IEnumerable<ServiceActionParameter> GetServiceActionParameters(IDataServiceMetadataProvider metadataProvider, DSPActionAttribute actionAttribute, MethodInfo actionMethodInfo)
        {
            ParameterInfo[] parameterInfos = actionMethodInfo.GetParameters();
            if (actionAttribute.ParameterTypeNames.Length != parameterInfos.Length)
            {
                throw new InvalidOperationException(string.Format("Mismatch parameter count between the DSPActionAttribute and the MethodInfo for the Action '{0}'.", actionMethodInfo.Name));
            }

            for (int idx = 0; idx < parameterInfos.Length; idx++)
            {
                ParameterInfo parameterInfo = parameterInfos[idx];
                ResourceType parameterResourceType = DSPActionProvider.GetResourceTypeFromType(metadataProvider, parameterInfo.ParameterType, actionAttribute.ParameterTypeNames[idx]);
                yield return new ServiceActionParameter(parameterInfo.Name, parameterResourceType);
            }
        }

        /// <summary>
        /// Get the resource type for the given clr type.
        /// </summary>
        /// <param name="metadataProvider">An instance of the IDataServiceMetadataProvider.</param>
        /// <param name="type">Clr type in question.</param>
        /// <param name="elementTypeName">Name of the element type if <paramref name="type"/> is a
        /// <see cref="DSPResource"/> type or a collection type of <see cref="DSPResource"/>.</param>
        /// <returns>The resource type for the given clr type.</returns>
        private static ResourceType GetResourceTypeFromType(IDataServiceMetadataProvider metadataProvider, Type type, string elementTypeName)
        {
            Debug.Assert(metadataProvider != null, "metadataProvider != null");
            if (type == typeof(void))
            {
                return null;
            }

            ResourceType resourceType;
            Type elementType = type;
            if (!type.IsPrimitive && type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type))
            {
                elementType = type.GetGenericArguments()[0];
            }

            if (elementType == typeof(DSPResource))
            {
                metadataProvider.TryResolveResourceType(elementTypeName, out resourceType);
            }
            else
            {
                resourceType = ResourceType.GetPrimitiveResourceType(elementType);
                if (resourceType == null && !metadataProvider.TryResolveResourceType(elementType.FullName, out resourceType))
                {
                    throw new DataServiceException(string.Format("The type '{0}' is unknown to the metadata provider.", elementType.FullName));
                }
            }

            if (type != elementType)
            {
                if (resourceType.ResourceTypeKind == ResourceTypeKind.EntityType)
                {
                    resourceType = ResourceType.GetEntityCollectionResourceType(resourceType);
                }
                else
                {
                    resourceType = ResourceType.GetCollectionResourceType(resourceType);
                }
            }

            return resourceType;
        }

        /// <summary>
        /// Stores the MethodInfo and instance info for a ServiceAction.
        /// </summary>
        private class DSPServiceActionInfo
        {
            /// <summary>
            /// The instance to invoke the method.
            /// </summary>
            public object Instance { get; set; }

            /// <summary>
            /// The methodInfo to invoke.
            /// </summary>
            public MethodInfo Method { get; set; }

            /// <summary>
            /// The <see cref="DSPActionAttribute"/> on the Action method.
            /// </summary>
            public DSPActionAttribute ActionAttribute { get; set; }
        }

        /// <summary>
        /// An implementation of the IDataServiceInvokable interface for the DSP providers.
        /// </summary>
        private class DSPInvokableAction : IDataServiceInvokable
        {
            /// <summary>
            /// Method info for ResolveCollection().
            /// </summary>
            private static readonly MethodInfo ResolveCollectionMethodInfo = typeof(DSPInvokableAction).GetMethod("ResolveCollection", BindingFlags.NonPublic | BindingFlags.Static);

            /// <summary>
            /// Method info for ResolveCollectionAsQueryable().
            /// </summary>
            private static readonly MethodInfo ResolveCollectionAsQueryableMethodInfo = typeof(DSPInvokableAction).GetMethod("ResolveCollectionAsQueryable", BindingFlags.NonPublic | BindingFlags.Static);

            /// <summary>
            /// The result of the action invocation.
            /// </summary>
            private object result;

            /// <summary>
            /// The delegate to invoke the action.
            /// </summary>
            private Action invokeDelegate;

            /// <summary>
            /// The delegate to get the result;
            /// </summary>
            private Func<object> getResultDelegate;

            /// <summary>
            /// Callback to be called when DSPInvokableAction.Invoke() is called.
            /// This is added for unit testing purpose so we know the correct sequence of calls happened in the server.
            /// </summary>
            private Action serviceActionInvokeCallback;

            /// <summary>
            /// Callback to be called when DSPInvokableAction.GetResult() is called.
            /// This is added for unit testing purpose so we know the correct sequence of calls happened in the server.
            /// </summary>
            private Action serviceActionGetResultCallback;

            /// <summary>
            /// Constructs an Invokable to invoke the service action with the provided parameters.
            /// </summary>
            /// <param name="operationContext">The data service operation context instance.</param>
            /// <param name="dataService">The data service instance.</param>
            /// <param name="action">The service action to invoke.</param>
            /// <param name="parameters">The parameters required to invoke the service action.</param>
            /// <param name="serviceActionInvokeCallback">Callback to be called when DSPInvokableAction.Invoke() is called.
            /// This is added for unit testing purpose so we know the correct sequence of calls happened in the server.</param>
            /// <param name="serviceActionGetResultCallback">Callback to be called when DSPInvokableAction.GetResult() is called.
            /// This is added for unit testing purpose so we know the correct sequence of calls happened in the server.</param>
            public DSPInvokableAction(DataServiceOperationContext operationContext, ServiceAction action, object[] parameters, Action<object[]> serviceActionInvokeCallback, Action serviceActionGetResultCallback)
            {
                if (serviceActionInvokeCallback != null)
                {
                    this.serviceActionInvokeCallback = () => serviceActionInvokeCallback(parameters);
                }

                this.serviceActionGetResultCallback = serviceActionGetResultCallback;
                this.ConstructInvokeDelegateForServiceAction(operationContext, action, parameters);
                this.getResultDelegate = () => this.result;
            }

            /// <summary>
            /// Invokes the underlying operation.
            /// </summary>
            void IDataServiceInvokable.Invoke()
            {
                if (this.serviceActionInvokeCallback != null)
                {
                    this.serviceActionInvokeCallback();
                }

                if (this.invokeDelegate == null)
                {
                    throw new DataServiceException("IDataServiceInvokable.Invoke() should only be called once.");
                }

                this.invokeDelegate();
                this.invokeDelegate = null;
            }

            /// <summary>
            /// Gets the result of the call to Invoke.
            /// </summary>
            /// <returns>The result of the call to Invoke.</returns>
            object IDataServiceInvokable.GetResult()
            {
                if (this.serviceActionGetResultCallback != null)
                {
                    this.serviceActionGetResultCallback();
                }

                if (this.getResultDelegate == null)
                {
                    throw new DataServiceException("IDataServiceInvokable.GetResult() should only be called once.");
                }

                object r = this.getResultDelegate();
                this.getResultDelegate = null;
                return r;
            }

            /// <summary>
            /// Calls IUpdatable.ResolveResource on each token in the collection and returns the resolved collection.
            /// </summary>
            /// <typeparam name="T">Collection item type.</typeparam>
            /// <param name="updateProvider">IDataServiceUpdateProvider2 instance.</param>
            /// <param name="collection">The collection of tokens to a complex collection.</param>
            /// <param name="isPrimitiveMutiValue">true if the items of the collection is of primitive type, false otherwise.</param>
            /// <returns>Resolved collection.</returns>
            private static IEnumerable<T> ResolveCollection<T>(IDataServiceUpdateProvider2 updateProvider, IEnumerable collection, bool isPrimitiveMutiValue)
            {
                List<T> result = new List<T>();
                foreach (object item in collection)
                {
                    if (!isPrimitiveMutiValue)
                    {
                        result.Add((T)updateProvider.ResolveResource(item));
                    }
                    else
                    {
                        result.Add((T)item);
                    }
                }

                return result;
            }

            /// <summary>
            /// Calls IUpdatable.ResolveResource on each token in the collection and returns the resolved collection.
            /// </summary>
            /// <typeparam name="T">Collection item type.</typeparam>
            /// <param name="updateProvider">IDataServiceUpdateProvider2 instance.</param>
            /// <param name="collection">The collection of tokens to a complex collection.</param>
            /// <param name="isPrimitiveMutiValue">true if the items of the collection is of primitive type, false otherwise.</param>
            /// <returns>Resolved collection as IQueryable.</returns>
            private static IQueryable<T> ResolveCollectionAsQueryable<T>(IDataServiceUpdateProvider2 updateProvider, IEnumerable collection, bool isPrimitiveMutiValue)
            {
                return DSPInvokableAction.ResolveCollection<T>(updateProvider, collection, isPrimitiveMutiValue).AsQueryable();
            }

            /// <summary>
            /// Constructs a delegate to invoke the service action with the provided parameters.
            /// </summary>
            /// <param name="operationContext">The data service operation context instance.</param>
            /// <param name="dataService">The data service instance.</param>
            /// <param name="action">The service action to invoke.</param>
            /// <param name="parameters">The parameters required to invoke the service action.</param>
            private void ConstructInvokeDelegateForServiceAction(DataServiceOperationContext operationContext, ServiceAction action, object[] parameters)
            {
                var info = action.CustomState as DSPServiceActionInfo;
                if (info == null)
                {
                    throw new InvalidOperationException("Insufficient information to invoke the service action!");
                }

                IDataServiceUpdateProvider2 updateProvider = (IDataServiceUpdateProvider2)operationContext.GetService(typeof(IDataServiceUpdateProvider2));
                if (updateProvider == null)
                {
                    throw new InvalidOperationException("DataServiceOperationContext.GetService(IDataServiceUpdateProvider2) returned null!");
                }

                this.invokeDelegate = () =>
                {
                    int idx = 0;
                    if (action.BindingParameter != null)
                    {
                        if (action.BindingParameter.ParameterType.ResourceTypeKind == ResourceTypeKind.EntityType)
                        {
                            parameters[idx] = updateProvider.GetResource((IQueryable)parameters[idx], null);
                        }

                        parameters[idx] = updateProvider.ResolveResource(parameters[idx]);
                        idx++;
                    }

                    var methodParameters = info.Method.GetParameters();
                    for (; idx < parameters.Length; idx++)
                    {
                        ServiceActionParameter sap = action.Parameters[idx];
                        if (sap.ParameterType.ResourceTypeKind == ResourceTypeKind.Collection && parameters[idx] != null)
                        {
                             ResourceType itemResourceType = ((CollectionResourceType)sap.ParameterType).ItemType;

                            // Need to call ResolveResource on each complex item in the collection.
                            Type parameterType = methodParameters[idx].ParameterType;
                            Type itemType = parameterType.GetGenericArguments()[0];
                            MethodInfo resolveCollectionMethod;

                            if (parameterType.GetGenericTypeDefinition() == typeof(IQueryable<>))
                            {
                                resolveCollectionMethod = DSPInvokableAction.ResolveCollectionAsQueryableMethodInfo.MakeGenericMethod(itemType);
                            }
                            else
                            {
                                resolveCollectionMethod = DSPInvokableAction.ResolveCollectionMethodInfo.MakeGenericMethod(itemType);
                            }

                            parameters[idx] = resolveCollectionMethod.Invoke(null, new object[] { updateProvider, parameters[idx], itemResourceType.ResourceTypeKind == ResourceTypeKind.Primitive });
                        }
                        else if (sap.ParameterType.ResourceTypeKind == ResourceTypeKind.ComplexType)
                        {
                            parameters[idx] = updateProvider.ResolveResource(parameters[idx]);
                        }
                    }
                    
                    this.result = info.Method.Invoke(info.Instance, parameters);
                };
            }
        }
    }
}
