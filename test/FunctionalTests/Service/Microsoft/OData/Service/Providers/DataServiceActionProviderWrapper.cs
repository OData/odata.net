//---------------------------------------------------------------------
// <copyright file="DataServiceActionProviderWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.OData.Client;
    using Microsoft.OData;
    using Strings = Microsoft.OData.Service.Strings;
    #endregion Namespaces

    /// <summary>
    /// Wrapper class for IDataServiceActionProvider interface.
    /// </summary>
    internal class DataServiceActionProviderWrapper
    {
        #region Private Fields

        /// <summary>
        /// An empty array of ServiceOperationWrapper.
        /// </summary>
        private static readonly IEnumerable<OperationWrapper> EmptyServiceOperationWrapperEnumeration = new OperationWrapper[0];

        /// <summary>
        /// An empty list of ServiceOperationWrapper.
        /// </summary>
        private static readonly List<OperationWrapper> EmptyServiceOperationWrapperList = new List<OperationWrapper>();

        /// <summary>
        /// The provider wrapper to use for finding other providers, caching operations, etc.
        /// </summary>
        private readonly IDataServiceProviderWrapperForActions provider;

        /// <summary>
        /// The max prototol version of the service.
        /// </summary>
        private readonly ODataProtocolVersion maxProtocolVersion;

        /// <summary>
        /// Delegate to retrieve the current operation operation context. Note that we do not cache the operation context itself because it can change in $batch cases.
        /// </summary>
        private readonly Func<DataServiceOperationContext> getOperationContext;

        /// <summary>
        /// Action provider instance, or null if one was not found.
        /// </summary>
        private IDataServiceActionProvider actionProvider;

        /// <summary>
        /// Action resolution provider instance, or null if one was not found.
        /// </summary>
        private IDataServiceActionResolver actionResolver;

        /// <summary>
        /// Set to true when we attempted to load the action provider the first time so we don't try to load it repeatedly when the interface is not implemented.
        /// </summary>
        private bool attemptedToLoadActionProvider;

        /// <summary>
        /// Maps a resource type to a collection of actions bindable to that resource type.
        /// </summary>
        private Dictionary<ResourceType, IEnumerable<OperationWrapper>> serviceActionByResourceTypeCache;

        /// <summary>
        /// Maps a base resource type to a collection of actions bindable to any of the resource types in that type hierarchy.
        /// </summary>
        private Dictionary<ResourceType, IEnumerable<OperationWrapper>> serviceActionByResourceTypeHierarchyCache;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the wrapper class for IDataServiceActionProvider
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="maxProtocolVersion">The max protocol version of the service.</param>
        /// <param name="getOperationContext">A delegate to retrieve the current operation context when invoking a provider API.</param>
        private DataServiceActionProviderWrapper(IDataServiceProviderWrapperForActions provider, ODataProtocolVersion maxProtocolVersion, Func<DataServiceOperationContext> getOperationContext)
        {
            Debug.Assert(provider != null, "provider != null");
            Debug.Assert(getOperationContext != null, "getOperationContext != null");
            
            this.provider = provider;
            this.maxProtocolVersion = maxProtocolVersion;
            this.getOperationContext = getOperationContext;
        }

        #endregion

        /// <summary>
        /// Interface to allow substitution of <see cref="DataServiceProviderWrapper"/> for unit testing <see cref="DataServiceActionProviderWrapper"/>.
        /// </summary>
        internal interface IDataServiceProviderWrapperForActions
        {
            /// <summary>
            /// Validates if the service operation should be visible and is read only. If the service operation
            /// rights are set to None the service operation should not be visible.
            /// </summary>
            /// <param name="operation">Operation to be validated.</param>
            /// <returns>Validated service operation, null if the service operation is not supposed to be visible.</returns>
            OperationWrapper ValidateOperation(ServiceAction operation);

            /// <summary>
            /// Add the given service operation to the model.
            /// </summary>
            /// <param name="operationWrapper">ServiceOperationWrapper instance to add.</param>
            void AddOperationToEdmModel(OperationWrapper operationWrapper);

            /// <summary>
            /// Tries to find a cached wrapper for an operation with the given name and binding parameter type.
            /// </summary>
            /// <param name="operationName">The operation name.</param>
            /// <param name="bindingType">The operation's binding parameter's type, or null.</param>
            /// <param name="wrapper">The wrapper, if found.</param>
            /// <returns>Whether or not a wrapper was found.</returns>
            bool TryGetCachedOperationWrapper(string operationName, ResourceType bindingType, out OperationWrapper wrapper);

            /// <summary>
            /// Retrieve an implementation of a data service interface (ie. IUpdatable, IExpandProvider,etc)
            /// </summary>
            /// <typeparam name="T">The type representing the requested interface</typeparam>
            /// <returns>An object implementing the requested interface, or null if not available</returns>
            T GetService<T>() where T : class;

            /// <summary>
            /// Gets all the types in the types's hierarchy. This includes both the base types, the derived types, and the starting type.
            /// </summary>
            /// <param name="startingType">The starting type.</param>
            /// <returns>The types derived from or base of the starting type.</returns>
            IEnumerable<ResourceType> GetAllTypesInHierarchy(ResourceType startingType);
        }

        /// <summary>
        /// Gets a value indicating whether the action provider is implemented on the service.
        /// </summary>
        internal bool IsImplemented
        {
            get { return this.TryLoadActionProvider(); }
        }

        /// <summary>
        /// Maps a resource type to a collection of actions bindable to that resource type.
        /// </summary>
        private Dictionary<ResourceType, IEnumerable<OperationWrapper>> ServiceActionByResourceTypeCache
        {
            [DebuggerStepThrough]
            get { return this.serviceActionByResourceTypeCache ?? (this.serviceActionByResourceTypeCache = new Dictionary<ResourceType, IEnumerable<OperationWrapper>>(EqualityComparer<ResourceType>.Default)); }
        }

        /// <summary>
        /// Maps a base resource type to a collection of actions bindable to any of the resource types in that type hierarchy.
        /// </summary>
        private Dictionary<ResourceType, IEnumerable<OperationWrapper>> ServiceActionByResourceTypeHierarchyCache
        {
            [DebuggerStepThrough]
            get { return this.serviceActionByResourceTypeHierarchyCache ?? (this.serviceActionByResourceTypeHierarchyCache = new Dictionary<ResourceType, IEnumerable<OperationWrapper>>(EqualityComparer<ResourceType>.Default)); }
        }
        
        /// <summary>
        /// Gets the IDataServiceActionProvider instance.
        /// </summary>
        private IDataServiceActionProvider ActionProvider
        {
            get
            {
                Debug.Assert(this.actionProvider != null, "Should only get here when we have successfully loaded the IDataServiceActionProvider implementation.");
                return this.actionProvider;
            }
        }

        /// <summary>
        /// Gets the current operation context. Note that we do not cache the operation context because it can change in $batch cases, and this property may return a different instance.
        /// </summary>
        private DataServiceOperationContext OperationContext
        {
            get
            {
                DataServiceOperationContext operationContext = this.getOperationContext();
                Debug.Assert(operationContext != null && operationContext.CurrentDataService != null, "The operation context should be initialized by now.");
                return operationContext;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="DataServiceActionProviderWrapper"/>.
        /// </summary>
        /// <param name="provider">The data service provider wrapper.</param>
        /// <param name="maxProtocolVersion">The max protocol version of the service.</param>
        /// <param name="getOperationContext">A delegate to retrieve the current operation context when invoking a provider API.</param>
        /// <returns>A new instance of <see cref="DataServiceActionProviderWrapper"/>.</returns>
        internal static DataServiceActionProviderWrapper Create(IDataServiceProviderWrapperForActions provider, ODataProtocolVersion maxProtocolVersion, Func<DataServiceOperationContext> getOperationContext)
        {
            return new DataServiceActionProviderWrapper(provider, maxProtocolVersion, getOperationContext);
        }

        /// <summary>
        /// Creates a new instance of <see cref="DataServiceActionProviderWrapper"/>.
        /// </summary>
        /// <param name="dataService">The data service.</param>
        /// <returns>A new instance of <see cref="DataServiceActionProviderWrapper"/>.</returns>
        internal static DataServiceActionProviderWrapper Create(IDataService dataService)
        {
            Debug.Assert(dataService != null, "dataService != null");
            return Create(
                new DefaultProviderWrapper(dataService.Provider),
                dataService.Provider.Configuration.DataServiceBehavior.MaxProtocolVersion,
                () => dataService.OperationContext);
        }

        /// <summary>
        /// Get the <see cref="IDataServiceInvokable"/> from the given <paramref name="actionSegment"/>.
        /// </summary>
        /// <param name="actionSegment">SegmentInfo instance for the service action.</param>
        /// <returns>The <see cref="IDataServiceInvokable"/> from the given <paramref name="actionSegment"/>.</returns>
        internal static IDataServiceInvokable CreateInvokableFromSegment(SegmentInfo actionSegment)
        {
            Debug.Assert(actionSegment.IsServiceActionSegment, "IsServiceActionSegment(actionSegment)");
            Debug.Assert(actionSegment.RequestEnumerable != null, "actionSegment.RequestEnumerable != null");

            return actionSegment.RequestEnumerable.OfType<IDataServiceInvokable>().Single();
        }

        /// <summary>
        /// Updates the RequestEnumerable of <paramref name="actionSegment"/> with the actual results from the invokable.
        /// </summary>
        /// <param name="actionSegment">SegmentInfo instance for the service action.</param>
        internal static void ResolveActionResult(SegmentInfo actionSegment)
        {
            Debug.Assert(actionSegment.IsServiceActionSegment, "IsServiceActionSegment(actionSegment)");
            Debug.Assert(actionSegment.Operation.ResultKind != ServiceOperationResultKind.Void, "actionSegment.Operation.ResultKind != ServiceOperationResultKind.Void");
            Debug.Assert(actionSegment.RequestEnumerable != null, "actionSegment.RequestEnumerable != null");

            IDataServiceInvokable invokable = DataServiceActionProviderWrapper.CreateInvokableFromSegment(actionSegment);
            object result = invokable.GetResult();
            if (result != null && actionSegment.SingleResult && !(result is IQueryable))
            {
                result = new object[] { result };
            }

            actionSegment.RequestEnumerable = (IEnumerable)result;
        }

        /// <summary>
        /// Returns all service actions in the provider.
        /// </summary>
        /// <returns>An enumeration of all service actions.</returns>
        internal IEnumerable<OperationWrapper> GetServiceActions()
        {
            if (!this.TryLoadActionProvider())
            {
                return EmptyServiceOperationWrapperEnumeration;
            }

            IEnumerable<ServiceAction> serviceActions = this.actionProvider.GetServiceActions(this.OperationContext);
            if (serviceActions == null)
            {
                return EmptyServiceOperationWrapperEnumeration;
            }

            return serviceActions
                .Select(serviceAction => this.provider.ValidateOperation(serviceAction))
                .Where(serviceActionWrapper => serviceActionWrapper != null);
        }
        
        /// <summary>
        /// Tries to find the <see cref="ServiceAction"/> for the given <paramref name="serviceActionName"/>.
        /// </summary>
        /// <param name="serviceActionName">The name of the service action to resolve. This must be the non-fully-qualified action name.
        ///   Call DataServiceProviderWrapper.GetNameFromContainerQualifiedName() before calling this method.</param>
        /// <param name="bindingType">The binding type of the action, or null if it is unknown.</param>
        /// <returns>Returns the service action instance if the resolution is successful; null otherwise.</returns>
        internal OperationWrapper TryResolveServiceAction(string serviceActionName, ResourceType bindingType)
        {
            Debug.Assert(!string.IsNullOrEmpty(serviceActionName), "!string.IsNullOrEmpty(serviceActionName)");

            ServiceAction action;
            OperationWrapper actionWrapper;
            if (this.provider.TryGetCachedOperationWrapper(serviceActionName, bindingType, out actionWrapper))
            {
                if (actionWrapper != null && actionWrapper.Kind == OperationKind.Action)
                {
                    this.provider.AddOperationToEdmModel(actionWrapper);
                    return actionWrapper;
                }

                return null;
            }

            if (this.TryResolveServiceActionFromProvider(serviceActionName, bindingType, out action))
            {
                actionWrapper = this.provider.ValidateOperation(action);
                this.provider.AddOperationToEdmModel(actionWrapper);
                return actionWrapper;
            }

            return null;
        }

        /// <summary>
        /// Builds up an instance of <see cref="IDataServiceInvokable"/> for the given <paramref name="serviceAction"/> with the provided <paramref name="parameterTokens"/>.
        /// </summary>
        /// <param name="serviceAction">The service action to invoke.</param>
        /// <param name="parameterTokens">The parameter tokens required to invoke the service action.</param>
        /// <returns>An instance of <see cref="IDataServiceInvokable"/> to invoke the action with.</returns>
        internal Expression CreateInvokable(OperationWrapper serviceAction, Expression[] parameterTokens)
        {
            Debug.Assert(serviceAction != null && serviceAction.Kind == OperationKind.Action, "serviceAction != null && serviceAction.Kind == OperationKind.Action");
            return Expression.Call(
                DataServiceExecutionProviderMethods.CreateServiceActionInvokableMethodInfo,
                Expression.Constant(this.OperationContext, typeof(DataServiceOperationContext)),
                Expression.Constant(this.ActionProvider, typeof(IDataServiceActionProvider)),
                Expression.Constant(serviceAction.ServiceAction, typeof(ServiceAction)),
                Expression.NewArrayInit(typeof(object), parameterTokens));
        }

        /// <summary>
        /// Gets the set of actions bound to any type in the hierearchy, and caches the results.
        /// </summary>
        /// <param name="resourceType">The starting type of the hierarchy.</param>
        /// <returns>The operations bound to any type in the hierearchy.</returns>
        internal IEnumerable<OperationWrapper> GetActionsBoundToAnyTypeInHierarchy(ResourceType resourceType)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            
            if (!this.TryLoadActionProvider())
            {
                return EmptyServiceOperationWrapperEnumeration;
            }

            IEnumerable<OperationWrapper> allServiceActionsBoundToHierarchy;
            if (!this.ServiceActionByResourceTypeHierarchyCache.TryGetValue(resourceType, out allServiceActionsBoundToHierarchy))
            {
                IEnumerable<ResourceType> types = this.provider.GetAllTypesInHierarchy(resourceType);
                this.ServiceActionByResourceTypeHierarchyCache[resourceType] = allServiceActionsBoundToHierarchy = this.GetServiceActionsBySpecificBindingParameterTypes(types);
            }

            return allServiceActionsBoundToHierarchy;
        }

        /// <summary>
        /// Gets a collection of actions having <paramref name="bindingParameterType"/> or any of it's base types as the binding parameter type.
        /// </summary>
        /// <param name="bindingParameterType">Instance of the binding parameter resource type (<see cref="ResourceType"/>) in question.</param>
        /// <returns>A list of actions having <paramref name="bindingParameterType"/> as the binding parameter type.</returns>
        internal List<OperationWrapper> GetServiceActionsByBindingParameterType(ResourceType bindingParameterType)
        {
            Debug.Assert(bindingParameterType != null, "bindingParameterType != null");
            Debug.Assert(bindingParameterType.ResourceTypeKind == ResourceTypeKind.EntityType, "Only entity types should be passed to this method.");
            return this.GetServiceActionsBySpecificBindingParameterTypes(bindingParameterType.BaseTypesAndSelf());
        }

        /// <summary>
        /// Determines whether a given <paramref name="serviceAction"/> should be advertised as bindable to the given <paramref name="resourceInstance"/>.
        /// </summary>
        /// <param name="serviceAction">Service action to be advertised.</param>
        /// <param name="resourceInstance">Instance of the resource to which the service action is bound.</param>
        /// <param name="resourceInstanceInFeed">true if the resource instance to be serialized is inside a feed; false otherwise. The value true
        /// suggests that this method might be called many times during serialization since it will get called once for every resource instance inside
        /// the feed. If it is an expensive operation to determine whether to advertise the service action for the <paramref name="resourceInstance"/>,
        /// the provider may choose to always advertise in order to optimize for performance.</param>
        /// <param name="actionToSerialize">The <see cref="Microsoft.OData.ODataAction"/> to be serialized. The server constructs 
        /// the version passed into this call, which may be replaced by an implementation of this interface.
        /// This should never be set to null unless returning false.</param>
        /// <returns>true if the service action should be advertised; false otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:AvoidRefParameters", Justification = "ref parameter required")]
        internal bool AdvertiseServiceAction(OperationWrapper serviceAction, object resourceInstance, bool resourceInstanceInFeed, ref ODataAction actionToSerialize)
        {
            Debug.Assert(resourceInstance != null, "resourceInstance != null");
            Debug.Assert(serviceAction != null && serviceAction.Kind == OperationKind.Action, "serviceOperation != null && serviceAction.Kind == OperationKind.Action");
            Debug.Assert(actionToSerialize != null, "actionToSerialize != null.");

            return this.ActionProvider.AdvertiseServiceAction(this.OperationContext, serviceAction.ServiceAction, resourceInstance, resourceInstanceInFeed, ref actionToSerialize);
        }

        /// <summary>
        /// Gets the set of actions bound to any of the given binding parameter types.
        /// </summary>
        /// <param name="bindingParameterTypes">The binding parameter types.</param>
        /// <returns>The operations bound to any of the specified types.</returns>
        private List<OperationWrapper> GetServiceActionsBySpecificBindingParameterTypes(IEnumerable<ResourceType> bindingParameterTypes)
        {
            if (!this.TryLoadActionProvider())
            {
                return EmptyServiceOperationWrapperList;
            }

            OperationCache existingOperations = new OperationCache();

            // DEVNOTE: We create a list to force enumeration here (rather than waiting for serialization-time enumeration) to preserve call-order.
            return bindingParameterTypes.SelectMany(resourceType => this.GetServiceActionsBySpecificBindingParameterType(resourceType, existingOperations)).ToList();
        }

        /// <summary>
        /// Gets the set of actions bound to the specific parameter type, either from the cache or by calling the provider.
        /// </summary>
        /// <param name="bindingParameterType">The binding parameter type.</param>
        /// <param name="existingOperations">The cache of known actions, used to detect duplicates.</param>
        /// <returns>The operations bound to the specific type.</returns>
        private IEnumerable<OperationWrapper> GetServiceActionsBySpecificBindingParameterType(ResourceType bindingParameterType, OperationCache existingOperations)
        {
            IEnumerable<OperationWrapper> operationWrappersPerType;
            if (!this.ServiceActionByResourceTypeCache.TryGetValue(bindingParameterType, out operationWrappersPerType))
            {
                Debug.Assert(this.actionProvider != null, "this.actionProvider != null");
                IEnumerable<ServiceAction> serviceActions = this.actionProvider.GetServiceActionsByBindingParameterType(this.OperationContext, bindingParameterType);

                if (serviceActions != null && serviceActions.Any())
                {
                    operationWrappersPerType = serviceActions
                        .Select(serviceAction => this.ValidateCanAdvertiseServiceAction(bindingParameterType, serviceAction, existingOperations))
                        .Where(serviceOperationWrapper => serviceOperationWrapper != null)
                        .ToArray();
                }
                else
                {
                    operationWrappersPerType = EmptyServiceOperationWrapperEnumeration;
                }

                // add to the cache
                this.ServiceActionByResourceTypeCache[bindingParameterType] = operationWrappersPerType;
            }

            Debug.Assert(operationWrappersPerType != null, "operationWrappersPerType != null");
            return operationWrappersPerType;
        }

        /// <summary>
        /// Tries to load an implementation of IDataServiceActionProvider from the service. Return true if successful; false otherwise.
        /// </summary>
        /// <returns>true if successfully loaded an implementation of IDataServiceActionProvider; false otherwise.</returns>
        private bool TryLoadActionProvider()
        {
            if (!this.attemptedToLoadActionProvider)
            {
                this.actionProvider = this.provider.GetService<IDataServiceActionProvider>();
                if (this.actionProvider != null)
                {
                    // try to get the action resolver for overloads. This can either be the same instance as the action provider, or a separate instance returned by GetService.
                    this.actionResolver = this.actionProvider as IDataServiceActionResolver ?? this.provider.GetService<IDataServiceActionResolver>();
                }

                this.attemptedToLoadActionProvider = true;
            }

            return this.actionProvider != null;
        }

        /// <summary>
        /// Validates if a service action is advertisable.
        /// </summary>
        /// <param name="resourceType">Resource type to which the service action is bound to.</param>
        /// <param name="serviceAction">Service action to be validated for advertisement.</param>
        /// <param name="existingOperations">The current set of actions. Used to avoid duplicate actions.</param>
        /// <returns>Validated service operation to be advertised. Null, if the service operation is not suppose to be advertised.</returns>
        private OperationWrapper ValidateCanAdvertiseServiceAction(ResourceType resourceType, ServiceAction serviceAction, OperationCache existingOperations)
        {
            Debug.Assert(resourceType != null && resourceType.ResourceTypeKind == ResourceTypeKind.EntityType, "resourceType != null && resourceType.ResourceTypeKind == ResourceTypeKind.EntityType");

            if (serviceAction == null)
            {
                return null;
            }
  
            Debug.Assert(!String.IsNullOrEmpty(serviceAction.Name), "The name of the service operation was null or empty");
            
            if (existingOperations.Contains(serviceAction))
            {
                throw new DataServiceException(500, Strings.DataServiceActionProviderWrapper_DuplicateAction(serviceAction.Name));
            }

            ServiceActionParameter bindingParameter = (ServiceActionParameter)serviceAction.BindingParameter;

            if (bindingParameter == null)
            {
                Debug.Assert(!String.IsNullOrEmpty(serviceAction.Name), "The name of the service action was null or empty");
                throw new DataServiceException(500, Strings.DataServiceActionProviderWrapper_ServiceActionBindingParameterNull(serviceAction.Name));
            }

            ResourceType bindingParameterType = bindingParameter.ParameterType;
            Debug.Assert(bindingParameterType != null, "bindingParameterType != null");

            // We only support advertising actions for entities and not entity collections. Since resourceType must be an entity type,
            // IsAssignableFrom will fail when the bindingParameterType is an entity collection type.
            if (!bindingParameterType.IsAssignableFrom(resourceType))
            {
                throw new DataServiceException(500, Strings.DataServiceActionProviderWrapper_ResourceTypeMustBeAssignableToBindingParameterResourceType(serviceAction.Name, bindingParameterType.FullName, resourceType.FullName));
            }

            Debug.Assert(bindingParameterType.ResourceTypeKind == ResourceTypeKind.EntityType, "We only support advertising actions for entities and not entity collections.");
            OperationWrapper operationWrapper = this.provider.ValidateOperation(serviceAction);
            if (operationWrapper != null)
            {
                existingOperations.Add(operationWrapper);
            }

            return operationWrapper;
        }

        /// <summary>
        /// Tries to resolve the action by invoking TryResolveServiceAction on the underlying provider.
        /// This is a lower-level call which does not cache results. Caching should be provided by the caller.
        /// </summary>
        /// <param name="serviceActionName">The name of the service action taken from a segment of the URI.</param>
        /// <param name="bindingType">The current binding type of the action, or null if there is no binding type.</param>
        /// <param name="action">The action if one is returned from the provider.</param>
        /// <returns>Whether or not the provider returned an action for the given inputs.</returns>
        private bool TryResolveServiceActionFromProvider(string serviceActionName, ResourceType bindingType, out ServiceAction action)
        {
            if (!this.TryLoadActionProvider())
            {
                action = null;
                return false;
            }

            if (this.actionResolver != null)
            {
                var resolverArgs = new ServiceActionResolverArgs(serviceActionName, bindingType);
                return this.actionResolver.TryResolveServiceAction(this.OperationContext, resolverArgs, out action);
            }

            return this.actionProvider.TryResolveServiceAction(this.OperationContext, serviceActionName, out action);
        }

        /// <summary>
        /// Default implementation of <see cref="IDataServiceProviderWrapperForActions"/> which delegates directly to <see cref="DataServiceProviderWrapper"/>.
        /// </summary>
        private class DefaultProviderWrapper : IDataServiceProviderWrapperForActions
        {
            /// <summary>
            /// The provider wrapper.
            /// </summary>
            private readonly DataServiceProviderWrapper provider;
            
            /// <summary>
            /// Initializes a new instance of <see cref="DefaultProviderWrapper"/>.
            /// </summary>
            /// <param name="provider">The provider wrapper.</param>
            internal DefaultProviderWrapper(DataServiceProviderWrapper provider)
            {
                Debug.Assert(provider != null, "provider != null");
                this.provider = provider;
            }

            /// <summary>
            /// Validates if the service operation should be visible and is read only. If the service operation
            /// rights are set to None the service operation should not be visible.
            /// </summary>
            /// <param name="operation">Operation to be validated.</param>
            /// <returns>Validated service operation, null if the service operation is not supposed to be visible.</returns>
            public OperationWrapper ValidateOperation(ServiceAction operation)
            {
                return this.provider.ValidateOperation(operation);
            }

            /// <summary>
            /// Add the given service operation to the model.
            /// </summary>
            /// <param name="operationWrapper">ServiceOperationWrapper instance to add.</param>
            public void AddOperationToEdmModel(OperationWrapper operationWrapper)
            {
                this.provider.GetMetadataProviderEdmModel().AddServiceOperation(operationWrapper);
            }

            /// <summary>
            /// Tries to find a cached wrapper for an operation with the given name and binding parameter type.
            /// </summary>
            /// <param name="operationName">The operation name.</param>
            /// <param name="bindingType">The operation's binding parameter's type, or null.</param>
            /// <param name="wrapper">The wrapper, if found.</param>
            /// <returns>Whether or not a wrapper was found.</returns>
            public bool TryGetCachedOperationWrapper(string operationName, ResourceType bindingType, out OperationWrapper wrapper)
            {
                return this.provider.OperationWrapperCache.TryGetWrapper(operationName, bindingType, out wrapper);
            }

            /// <summary>
            /// Retrieve an implementation of a data service interface (ie. IUpdatable, IExpandProvider,etc)
            /// </summary>
            /// <typeparam name="T">The type representing the requested interface</typeparam>
            /// <returns>An object implementing the requested interface, or null if not available</returns>
            public T GetService<T>() where T : class
            {
                return this.provider.GetService<T>();
            }

            /// <summary>
            /// Gets all the types in the types's hierarchy. This includes both the base types, the derived types, and the starting type.
            /// </summary>
            /// <param name="startingType">The starting type.</param>
            /// <returns>The types derived from or base of the starting type.</returns>
            public IEnumerable<ResourceType> GetAllTypesInHierarchy(ResourceType startingType)
            {
                var types = startingType.BaseTypesAndSelf();

                // TODO: should we be caching these calls?
                if (this.provider.HasDerivedTypes(startingType))
                {
                    types = types.Concat(this.provider.GetDerivedTypes(startingType));
                }

                return types;
            }
        }
    }
}
