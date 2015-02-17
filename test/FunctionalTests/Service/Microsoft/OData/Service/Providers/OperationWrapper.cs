//---------------------------------------------------------------------
// <copyright file="OperationWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>Use this class to represent an operation.</summary>
    [DebuggerVisualizer("OperationWrapper={Name}")]
    internal sealed class OperationWrapper
    {
        #region Private Fields

        /// <summary>
        /// Wrapped instance of the operation.
        /// </summary>
        private readonly Operation operation;

        /// <summary>
        /// Used to cache the target segment for this operation by resource type.
        /// </summary>
        private readonly Dictionary<ResourceType, string> actionTargetSegmentByResourceType;

        /// <summary>Access rights to this service operation.</summary>
        private ServiceOperationRights serviceOperationRights;

        /// <summary>Access rights to this service action.</summary>
        private ServiceActionRights serviceActionRights;

        /// <summary>Entity set from which entities are read, if applicable.</summary>
        private ResourceSetWrapper resourceSet;

#if DEBUG
        /// <summary>Is true, if the service operation is fully initialized and validated. No more changes can be made once its set to readonly.</summary>
        private bool isReadOnly;
#endif

        #endregion Private Fields

        #region Constructor

        /// <summary>
        /// Initializes a new <see cref="OperationWrapper"/> instance.
        /// </summary>
        /// <param name="operationBase">OperationBase instance to be wrapped.</param>
        public OperationWrapper(Operation operationBase)
        {
            Debug.Assert(operationBase != null, "operationBase != null");
            operationBase.EnsureReadOnly();
            this.operation = operationBase;
            this.actionTargetSegmentByResourceType = new Dictionary<ResourceType, string>(EqualityComparer<ResourceType>.Default);
        }

        #endregion Constructor

        #region Properties

        /// <summary>Protocol (for example HTTP) method the service operation responds to.</summary>
        public string Method
        {
            [DebuggerStepThrough]
            get { return this.operation.Method; }
        }

        /// <summary>MIME type specified on primitive results, possibly null.</summary>
        public string MimeType
        {
            [DebuggerStepThrough]
            get { return this.operation.MimeType; }
        }

        /// <summary>Name of the service operation.</summary>
        public string Name
        {
            [DebuggerStepThrough]
            get { return this.operation.Name; }
        }

        /// <summary>Returns all the parameters for the given service operations.</summary>
        public ReadOnlyCollection<OperationParameter> Parameters
        {
            [DebuggerStepThrough]
            get { return this.operation.OperationParameters; }
        }

        /// <summary>Kind of result expected from this operation.</summary>
        public ServiceOperationResultKind ResultKind
        {
            [DebuggerStepThrough]
            get { return this.operation.OperationResultKind; }
        }

        /// <summary>Element of result type.</summary>
        /// <remarks>
        /// Note that if the method returns an IEnumerable&lt;string&gt;, 
        /// this property will be typeof(string).
        /// </remarks>
        public ResourceType ResultType
        {
            [DebuggerStepThrough]
            get { return this.operation.OperationResultType; }
        }

        /// <summary>Return type of the operation.</summary>
        public ResourceType ReturnType
        {
            [DebuggerStepThrough]
            get { return this.operation.OperationReturnType; }
        }

        /// <summary>
        /// Gets the wrapped service operation
        /// </summary>
        public ServiceOperation ServiceOperation
        {
            [DebuggerStepThrough]
            get { return (ServiceOperation)this.operation; }
        }

        /// <summary>
        /// Gets the wrapped service action
        /// </summary>
        public ServiceAction ServiceAction
        {
            [DebuggerStepThrough]
            get { return (ServiceAction)this.operation; }
        }

        /// <summary>Whether the operation is visible to service consumers.</summary>
        public bool IsVisible
        {
            [DebuggerStepThrough]
            get
            {
#if DEBUG
                Debug.Assert(this.isReadOnly, "Wrapper class has not been initialized yet.");
#endif
                return (this.Kind == OperationKind.ServiceOperation && (this.serviceOperationRights & ~ServiceOperationRights.OverrideEntitySetRights) != ServiceOperationRights.None) ||
                    (this.Kind == OperationKind.Action && this.serviceActionRights != ServiceActionRights.None);
            }
        }

        /// <summary>Access rights to this service operation.</summary>
        public ServiceOperationRights ServiceOperationRights
        {
            [DebuggerStepThrough]
            get
            {
#if DEBUG
                Debug.Assert(this.isReadOnly, "Wrapper class has not been initialized yet.");
                Debug.Assert(this.Kind == OperationKind.ServiceOperation, "this.Kind == OperationKind.ServiceOperation");
#endif
                return this.serviceOperationRights;
            }
        }

        /// <summary>Access rights to this service action.</summary>
        public ServiceActionRights ServiceActionRights
        {
            [DebuggerStepThrough]
            get
            {
#if DEBUG
                Debug.Assert(this.isReadOnly, "Wrapper class has not been initialized yet.");
                Debug.Assert(this.Kind == OperationKind.Action, "this.Kind == OperationKind.Action");
#endif
                return this.serviceActionRights;
            }
        }

        /// <summary>Entity set from which entities are read (possibly null).</summary>
        public ResourceSetWrapper ResourceSet
        {
            [DebuggerStepThrough]
            get
            {
#if DEBUG
                Debug.Assert(this.isReadOnly, "Wrapper class has not been initialized yet.");
#endif
                Debug.Assert(
                    this.operation.ResourceSet == null || this.resourceSet.ResourceSet == this.operation.ResourceSet,
                    "this.serviceOperation.ResourceSet == null || this.resourceSet.ResourceSet == this.serviceOperation.ResourceSet");
                return this.resourceSet;
            }
        }

        /// <summary>Path expression to calculate the result resource set of the function if the function returns an entity or a collection of entity.</summary>
        public ResourceSetPathExpression ResultSetPathExpression
        {
            [DebuggerStepThrough]
            get { return this.operation.OperationResultSetPathExpression; }
        }

        /// <summary>
        /// The binding parameter to this function/action; null if this function/action is top-level or this operation is not a function or action.
        /// </summary>
        public OperationParameter BindingParameter
        {
            [DebuggerStepThrough]
            get { return this.operation.OperationBindingParameter; }
        }

        /// <summary>
        /// The <see cref="OperationParameterBindingKind"/> value of the underlying operation.
        /// </summary>
        internal OperationParameterBindingKind OperationParameterBindingKind
        {
            get { return this.operation.OperationParameterBindingKind; }
        }

        /// <summary>
        /// The kind of the current service operation
        /// </summary>
        internal OperationKind Kind
        {
            [DebuggerStepThrough]
            get { return this.operation.Kind; }
        }

        /// <summary>
        /// Gets the Clr return type.
        /// </summary>
        internal Type ReturnInstanceType
        {
            get
            {
                Type resultType = this.ResultType == null ? null : this.ResultType.InstanceType;
                if (this.ResultKind == ServiceOperationResultKind.QueryWithMultipleResults || this.ResultKind == ServiceOperationResultKind.QueryWithSingleResult)
                {
                    resultType = typeof(IQueryable<>).MakeGenericType(resultType);
                }
                else if (this.ResultKind == ServiceOperationResultKind.Enumeration)
                {
                    resultType = typeof(IEnumerable<>).MakeGenericType(resultType);
                }

                return resultType;
            }
        }

        #endregion Properties

        /// <summary>
        /// Apply the given configuration to the resource set.
        /// </summary>
        /// <param name="configuration">data service configuration instance.</param>
        /// <param name="provider">data service provider wrapper instance for accessibility validation.</param>
        public void ApplyConfiguration(DataServiceConfiguration configuration, DataServiceProviderWrapper provider)
        {
#if DEBUG
            Debug.Assert(!this.isReadOnly, "Can only apply the configuration once.");
#endif
            if (this.Kind == OperationKind.ServiceOperation)
            {
                this.serviceOperationRights = configuration.GetServiceOperationRights(this.ServiceOperation);
            }
            else
            {
                Debug.Assert(this.Kind == OperationKind.Action, "this.Kind == OperationKind.Action");
                this.serviceActionRights = configuration.GetServiceActionRights(this.ServiceAction);
            }

            if ((this.Kind == OperationKind.ServiceOperation && (this.serviceOperationRights & ~ServiceOperationRights.OverrideEntitySetRights) != ServiceOperationRights.None) ||
                (this.Kind == OperationKind.Action && this.serviceActionRights != Service.ServiceActionRights.None))
            {
                if (this.operation.ResourceSet != null)
                {
                    // If the result type is an entity type, we need to make sure its entity set is visible.
                    // If the entity set is hidden, we need to make sure that we throw an exception.
                    this.resourceSet = provider.TryResolveResourceSet(this.operation.ResourceSet.Name);
                    if (this.resourceSet == null)
                    {
                        throw new InvalidOperationException(Strings.OperationWrapper_OperationResourceSetNotVisible(this.Name, this.operation.ResourceSet.Name));
                    }
                }
                else if (this.ResultSetPathExpression != null)
                {
                    this.ResultSetPathExpression.InitializePathSegments(provider);
                }
            }
#if DEBUG
            this.isReadOnly = true;
#endif
        }

        /// <summary>
        /// Gets the result set for the operation.
        /// </summary>
        /// <param name="provider">Provider instance to resolve the path expression.</param>
        /// <param name="bindingSet">Binding resource set.</param>
        /// <returns>Returns the result resource set for the operation.</returns>
        internal ResourceSetWrapper GetResultSet(DataServiceProviderWrapper provider, ResourceSetWrapper bindingSet)
        {
#if DEBUG
            Debug.Assert(this.isReadOnly, "Wrapper class has not been initialized yet.");
#endif
            if (this.resourceSet != null)
            {
                Debug.Assert(this.resourceSet.ResourceSet == this.operation.ResourceSet, "this.resourceSet.ResourceSet == this.serviceOperation.ResourceSet");
                Debug.Assert(this.ResultSetPathExpression == null, "this.ResultSetPathExpression == null");
                return this.resourceSet;
            }
            
            if (this.ResultSetPathExpression != null)
            {
                Debug.Assert(provider != null, "provider != null");
                if (bindingSet == null)
                {
                    throw new InvalidOperationException(Strings.OperationWrapper_PathExpressionRequiresBindingSet(this.Name));
                }

                ResourceSetWrapper resultSet = this.ResultSetPathExpression.GetTargetSet(provider, bindingSet);
                if (resultSet == null)
                {
                    throw new InvalidOperationException(Strings.OperationWrapper_TargetSetFromPathExpressionNotNotVisible(this.Name, this.ResultSetPathExpression.PathExpression, bindingSet.Name));
                }

                return resultSet;
            }

            return null;
        }

        /// <summary>
        /// Gets the target segment of the service action based on the resource type. If the action name collides with a property of the 
        /// <paramref name="resourceType"/>, then this method resolves the name collision by prefixing the action name with the <paramref name="namespaceName"/>.
        /// </summary>
        /// <param name="resourceType">The resource type against which to get the target of the service action.</param>
        /// <param name="namespaceName">The name of the namespace, containing this operation. Used to resolve service action name collision with the resource property if any.</param>
        /// <returns>The name of the operation wrapper or the name prefixed with the <paramref name="namespaceName"/> in case of name collision with the 
        /// <paramref name="resourceType"/> property.</returns>
        /// <remarks>This method maintains a cache of action targets by resource type for efficiency.</remarks>
        internal string GetActionTargetSegmentByResourceType(ResourceType resourceType, string namespaceName)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(this.Kind == OperationKind.Action, "This method should be only called for actions.");
            Debug.Assert(!string.IsNullOrEmpty(namespaceName), "!string.IsNullOrEmpty(namespaceName)");
            Debug.Assert(!string.IsNullOrEmpty(this.Name), "!string.IsNullOrEmpty(this.Name)");

            string titleSegment;

            if (!this.actionTargetSegmentByResourceType.TryGetValue(resourceType, out titleSegment))
            {
                titleSegment = string.Concat(namespaceName, ".", this.Name);

                this.actionTargetSegmentByResourceType[resourceType] = titleSegment;
            }

            Debug.Assert(!String.IsNullOrEmpty(titleSegment), "!String.IsNullOrEmpty(titleSegment)");
            return titleSegment;
        }
    }
}
