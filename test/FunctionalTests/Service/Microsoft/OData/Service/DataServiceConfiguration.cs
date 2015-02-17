//---------------------------------------------------------------------
// <copyright file="DataServiceConfiguration.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Service.Parsing;
    using Microsoft.OData.Service.Providers;

    #endregion Namespaces

    /// <summary>Use this class to manage the configuration data for a data service.</summary>
    public sealed class DataServiceConfiguration : IDataServiceConfiguration
    {
        #region Private fields

        /// <summary>
        /// A lookup of containers to their rights.
        /// For IDSP there is no guarantee that the provider will always return the same metadata instance.  We should
        /// use the name instead of the instance as key since the configuration is cached across requests.
        /// </summary>
        private readonly Dictionary<string, EntitySetRights> resourceRights;

        /// <summary>
        /// A lookup of service operations to their rights.
        /// For IDSP there is no guarantee that the provider will always return the same metadata instance.  We should
        /// use the name instead of the instance as key since the configuration is cached across requests.
        /// </summary>
        private readonly Dictionary<string, ServiceOperationRights> serviceOperationRights;

        /// <summary>
        /// A lookup of service action to their rights.
        /// For IDSP there is no guarantee that the provider will always return the same metadata instance.  We should
        /// use the name instead of the instance as key since the configuration is cached across requests.
        /// </summary>
        private readonly Dictionary<string, ServiceActionRights> serviceActionRights;

        /// <summary>
        /// A lookup of resource sets to their page sizes.
        /// For IDSP there is no guarantee that the provider will always return the same metadata instance.  We should
        /// use the name instead of the instance as key since the configuration is cached across requests.
        /// </summary>
        private readonly Dictionary<string, int> pageSizes;

        /// <summary>A list of known types.</summary>
        private readonly List<Type> knownTypes;

        /// <summary>Holds configuration of service behavior</summary>
        private readonly DataServiceBehavior dataServiceBehavior;

        /// <summary>List of fully qualified type names that were marked as visible by calling EnableAccess().</summary>
        private readonly HashSet<string> accessEnabledResourceTypes;

        /// <summary>Whether this configuration has been sealed.</summary>
        private bool configurationSealed;

        /// <summary>Maximum number of change sets and query operations in a batch.</summary>
        private int maxBatchCount;

        /// <summary>Maximum number of changes in a change set.</summary>
        private int maxChangeSetCount;

        /// <summary>Maximum number of segments to be expanded allowed in a request.</summary>
        private int maxExpandCount;

        /// <summary>Maximum number of segments in a single $expand path.</summary>
        private int maxExpandDepth;

        /// <summary>Maximum number of elements in each returned collection (top-level or expanded).</summary>
        private int maxResultsPerCollection;

        /// <summary>maximum number of objects that can be referred in a single insert request.</summary>
        private int maxObjectCountOnInsert;

        /// <summary>The provider for the web service.</summary>
        private IDataServiceMetadataProvider provider;

        /// <summary>Rights used for unspecified resource sets.</summary>
        private EntitySetRights rightsForUnspecifiedResourceContainer;

        /// <summary>Rights used for unspecified service operations.</summary>
        private ServiceOperationRights rightsForUnspecifiedServiceOperation;

        /// <summary>Rights used for unspecified service actions.</summary>
        private ServiceActionRights rightsForUnspecifiedServiceAction;

        /// <summary>Page size for unspecified resource sets</summary>
        private int defaultPageSize;

        /// <summary>Whether verbose errors should be returned by default.</summary>
        private bool useVerboseErrors;

        /// <summary>
        /// Perform type conversion from the type specified in the payload to the actual property type.
        /// </summary>
        private bool typeConversion;

        /// <summary>
        /// Specifies whether the EDM model should be validated before it is written
        /// as a response to a $metadata request.
        /// </summary>
        private bool disableValidationOnMetadataWrite;

        /// <summary>This is set to true if EnableAccess("*") is called.  False otherwise.</summary>
        private bool accessEnabledForAllResourceTypes;

        #endregion Private fields

        #region Constructor.

        /// <summary>
        /// Initializes a new <see cref="DataServiceConfiguration"/> with
        /// the specified <paramref name="provider"/>.
        /// </summary>
        /// <param name="provider">Non-null provider for this configuration.</param>
        internal DataServiceConfiguration(IDataServiceMetadataProvider provider)
        {
            WebUtil.CheckArgumentNull(provider, "provider");
            this.provider = provider;
            this.resourceRights = new Dictionary<string, EntitySetRights>(EqualityComparer<string>.Default);
            this.serviceOperationRights = new Dictionary<string, ServiceOperationRights>(EqualityComparer<string>.Default);
            this.serviceActionRights = new Dictionary<string, ServiceActionRights>(EqualityComparer<string>.Default);
            this.pageSizes = new Dictionary<string, int>(EqualityComparer<string>.Default);
            this.rightsForUnspecifiedResourceContainer = EntitySetRights.None;
            this.rightsForUnspecifiedServiceOperation = ServiceOperationRights.None;
            this.rightsForUnspecifiedServiceAction = ServiceActionRights.None;
            this.knownTypes = new List<Type>();
            this.maxBatchCount = Int32.MaxValue;
            this.maxChangeSetCount = Int32.MaxValue;
            this.maxExpandCount = Int32.MaxValue;
            this.maxExpandDepth = Int32.MaxValue;
            this.maxResultsPerCollection = Int32.MaxValue;
            this.maxObjectCountOnInsert = Int32.MaxValue;
            this.accessEnabledResourceTypes = new HashSet<string>(EqualityComparer<string>.Default);
            this.dataServiceBehavior = new DataServiceBehavior();

            // default value is true since in V1, we always did the type conversion
            // and this configuration settings was introduced in V2
            this.typeConversion = true;
        }

        #endregion Constructor.

        #region Public Properties

        /// <summary>Gets or sets whether the data service runtime should convert the type that is contained in the payload to the actual property type that is specified in the request.</summary>
        /// <returns>True if the data service runtime should convert the type that is contained in the payload; otherwise, false.</returns>
        public bool EnableTypeConversion
        {
            get
            {
                return this.typeConversion;
            }

            set
            {
                this.CheckNotSealed();
                this.typeConversion = value;
            }
        }

        /// <summary>Gets or sets the maximum number of change sets and query operations that are allowed in a single batch.</summary>
        /// <returns>The maximum number of change sets.</returns>
        public int MaxBatchCount
        {
            get { return this.maxBatchCount; }
            set { this.maxBatchCount = this.CheckNonNegativeProperty(value, "MaxBatchCount"); }
        }

        /// <summary>Gets or set the maximum number of changes that can be included in a single change set.</summary>
        /// <returns>The maximum number of changes allowed.</returns>
        public int MaxChangesetCount
        {
            get { return this.maxChangeSetCount; }
            set { this.maxChangeSetCount = this.CheckNonNegativeProperty(value, "MaxChangesetCount"); }
        }

        /// <summary>Gets or sets the maximum number of related entities that can be included in a single request by using the $expand operator.</summary>
        /// <returns>The maximum number of related entities.</returns>
        public int MaxExpandCount
        {
            get { return this.maxExpandCount; }
            set { this.maxExpandCount = this.CheckNonNegativeProperty(value, "MaxExpandCount"); }
        }

        /// <summary>Get or sets the maximum number of related entities that can be included in an $expand path in a single request.</summary>
        /// <returns>The maximum depth of an $expand path.</returns>
        public int MaxExpandDepth
        {
            get { return this.maxExpandDepth; }
            set { this.maxExpandDepth = this.CheckNonNegativeProperty(value, "MaxExpandDepth"); }
        }

        /// <summary>Get or sets the maximum number of items in each returned collection.</summary>
        /// <returns>The maximum number of items.</returns>
        public int MaxResultsPerCollection
        {
            get
            {
                return this.maxResultsPerCollection;
            }

            set
            {
                if (this.IsPageSizeDefined)
                {
                    throw new InvalidOperationException(Strings.DataService_SDP_PageSizeWithMaxResultsPerCollection);
                }

                this.maxResultsPerCollection = this.CheckNonNegativeProperty(value, "MaxResultsPerCollection");
            }
        }

        /// <summary>Get or sets the maximum number of objects to insert that can be contained in a single POST request.</summary>
        /// <returns>The maximum number of objects to insert.</returns>
        public int MaxObjectCountOnInsert
        {
            get { return this.maxObjectCountOnInsert; }
            set { this.maxObjectCountOnInsert = this.CheckNonNegativeProperty(value, "MaxObjectCountOnInsert"); }
        }

        /// <summary>Builds the annotation models to be applied to the data model of the provider.</summary>
        /// <returns>The annotation models to be applied.</returns>
        /// <remarks>
        /// Builds the annotation models to be applied to the provider's EDM model (driven by <see cref="IDataServiceMetadataProvider"/>).
        /// The input to the Func is the provider's model (so the returned annotations can reference it).
        /// Only annotations within the returned models are considered; any other schema elements are ignored.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Need to return a collection of annotation models")]
        public Func<IEdmModel, IEnumerable<IEdmModel>> AnnotationsBuilder
        {
            get;
            set;
        }

        /// <summary>Gets or sets whether the verbose errors should be returned by the data service.</summary>
        /// <returns>True if the verbose errors should be returned by the data service; otherwise, false.</returns>
        /// <remarks>
        /// This property sets the default for the whole service; individual responses may behave differently
        /// depending on the value of the VerboseResponse property of the arguments to the HandleException
        /// method on the <see cref="DataService&lt;T&gt;"/> class.
        /// </remarks>
        public bool UseVerboseErrors
        {
            get
            {
                return this.useVerboseErrors;
            }

            set
            {
                this.CheckNotSealed();
                this.useVerboseErrors = value;
            }
        }

        /// <summary>Gets a <see cref="T:Microsoft.OData.Service.DataServiceBehavior" /> object that defines the additional behaviors of the data service.</summary>
        /// <returns>The additional behaviors of the data service.</returns>
        public DataServiceBehavior DataServiceBehavior
        {
            get
            {
                return this.dataServiceBehavior;
            }
        }

        /// <summary>Gets or sets whether the data model is validated before it is written as a response to a request to the $metadata endpoint.</summary>
        /// <returns>True when metadata validation is disabled; otherwise false.</returns>
        /// <remarks>The default value for this property is 'false'.</remarks>
        public bool DisableValidationOnMetadataWrite
        {
            get
            {
                return this.disableValidationOnMetadataWrite;
            }

            set
            {
                this.CheckNotSealed();
                this.disableValidationOnMetadataWrite = value;
            }
        }

        #endregion Public Properties

        #region Internal Properties

        /// <summary>True if all resource types have been made visible by calling EnableAccess("*").  False otherwise.</summary>
        internal bool AccessEnabledForAllResourceTypes
        {
            [DebuggerStepThrough]
            get { return this.accessEnabledForAllResourceTypes; }
        }

        #endregion Internal Properties

        #region Private Properties

        /// <summary>
        /// Whether size of a page has been defined.
        /// </summary>
        private bool IsPageSizeDefined
        {
            get
            {
                return this.pageSizes.Count > 0 || this.defaultPageSize > 0;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>Sets the permissions for the specified entity set resource.</summary>
        /// <param name="name">The Name of the entity set for which to set permissions.</param>
        /// <param name="rights">The Access rights to be granted to this resource, passed as an <see cref="T:Microsoft.OData.Service.EntitySetRights" /> value.</param>
        public void SetEntitySetAccessRule(string name, EntitySetRights rights)
        {
            this.CheckNotSealed();
            if (name == null)
            {
                throw Error.ArgumentNull("name");
            }

            WebUtil.CheckResourceContainerRights(rights, "rights");
            if (name == "*")
            {
                this.rightsForUnspecifiedResourceContainer = rights;
            }
            else
            {
                ResourceSet container;
                if (!this.provider.TryResolveResourceSet(name, out container) || container == null)
                {
                    throw new ArgumentException(Strings.DataServiceConfiguration_ResourceSetNameNotFound(name), "name");
                }

                this.resourceRights[container.Name] = rights;
            }
        }

        /// <summary>Sets the permissions for the specified service operation.</summary>
        /// <param name="name">The name of the service operation for which to set permissions.</param>
        /// <param name="rights">The access rights to be granted to this resource, passed as a <see cref="T:Microsoft.OData.Service.ServiceOperationRights" /> value.</param>
        public void SetServiceOperationAccessRule(string name, ServiceOperationRights rights)
        {
            this.CheckNotSealed();
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            WebUtil.CheckServiceOperationRights(rights, "rights");
            if (name == "*")
            {
                this.rightsForUnspecifiedServiceOperation = rights;
            }
            else
            {
                ServiceOperation serviceOperation;
                if (!this.provider.TryResolveServiceOperation(name, out serviceOperation) || serviceOperation == null)
                {
                    throw new ArgumentException(Strings.DataServiceConfiguration_ServiceNameNotFound(name), "name");
                }

                this.serviceOperationRights[serviceOperation.Name] = rights;
            }
        }

        /// <summary>Sets the permissions for the specified service action.</summary>
        /// <param name="name">The name of the service action for which to set permissions.</param>
        /// <param name="rights">The access rights to be granted to this action, passed as a <see cref="T:Microsoft.OData.Service.ServiceActionRights" /> value.</param>
        public void SetServiceActionAccessRule(string name, ServiceActionRights rights)
        {
            this.CheckNotSealed();
            WebUtil.CheckStringArgumentNullOrEmpty(name, "name");
            WebUtil.CheckServiceActionRights(rights, "rights");
            if (name == "*")
            {
                this.rightsForUnspecifiedServiceAction = rights;
            }
            else
            {
                // We initialize the service configuration before the action provider can be
                // loaded. We will not validate the service action names to make sure they
                // actually exist in the action provider.
                this.serviceActionRights[name] = rights;
            }
        }

        /// <summary>Adds a type to the list of types that are recognized by the data service.</summary>
        /// <param name="type">The type to add to the collection of known types.</param>
        public void RegisterKnownType(Type type)
        {
            this.CheckNotSealed();
            this.knownTypes.Add(type);
        }

        /// <summary>Sets the maximum page size for an entity set resource.</summary>
        /// <param name="name">The name of entity set resource for which to set the page size.</param>
        /// <param name="size">The page size for the entity set resource that is specified in <paramref name="name" />.</param>
        public void SetEntitySetPageSize(String name, int size)
        {
            WebUtil.CheckArgumentNull(name, "name");
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException("size", size, Strings.DataService_SDP_PageSizeMustbeNonNegative(size, name));
            }

            // Treat a page size of Int32.MaxValue to be the same as not setting the page size.
            if (size == Int32.MaxValue)
            {
                size = 0;
            }

            if (this.MaxResultsPerCollection != Int32.MaxValue)
            {
                throw new InvalidOperationException(Strings.DataService_SDP_PageSizeWithMaxResultsPerCollection);
            }

            this.CheckNotSealed();

            if (name == "*")
            {
                this.defaultPageSize = size;
            }
            else
            {
                ResourceSet container;
                if (!this.provider.TryResolveResourceSet(name, out container) || container == null)
                {
                    throw new ArgumentException(Strings.DataServiceConfiguration_ResourceSetNameNotFound(name), "name");
                }

                this.pageSizes[container.Name] = size;
            }
        }

        /// <summary>Registers a data type with the data service runtime so that it can be used by a custom data service provider.</summary>
        /// <param name="typeName">The namespace-qualified name of the type that is enabled for use with the custom data service provider.</param>
        /// <remarks>
        /// This method is used to register a type with the Astoria runtime which may be returned in the �open properties� of
        /// an open type such that the type is visible in $metadata output and usable with CRUD operations.
        /// 
        /// The typename parameter must be a namespace qualified type name (format: &lt;namespace&gt;.&lt;typename&gt;).  
        /// The name provided must be as it would show up in a CSDL document (ie. model types, not CLR types)
        /// 
        /// The types registered via calls to EnableAccess will be additive to those implicitly made accessible via 
        /// DSC.SetEntitySetAccessRule(�) invocations
        ///  � Note: The Astoria runtime layer won�t be able to determine if a typename specified maps to an Entity Type,
        ///    Complex Type, etc until it actually obtains type info (entity types, complex types, etc) from the underlying provider
        ///  � �*� can be used as the value of �typename�, which will be interpreted as matching all types
        ///  
        /// When Astoria enumerates types or needs to obtain a type (Complex Types, Entity Types) from the underlying provider
        /// it will first determine if the type should be visible (show in $metadata and accessible via operations exposed by the
        /// service) as per the standard v1 checks (ie. driven by SetEntitySetAccessRule calls). If the type is not visible via V1
        /// rules, then we consult the set of types registered via EnableAccess(&lt;typename&gt;) invocations.  If the type was
        /// included in such a call then the type is visible via $metadata and can be accessed via CRUD ops, etc.
        /// 
        /// If a type is not made visible via one of the mechanisms above, then:
        ///   � That type must not be included a response to a $metadata request
        ///   � Instances of the type must not be returned to the client as the response of a request to the data service. 
        ///     If such a type instance would be required the service MUST fail the request.  Failure semantics are covered 
        ///     in the area of the specification which covers request/response semantics with respect to open types. 
        ///
        /// Invoking this method multiple times with the same type name is allowed and considered a �NO OP�.
        /// </remarks>
        public void EnableTypeAccess(string typeName)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(typeName, "typeName");
            this.CheckNotSealed();

            if (typeName == "*")
            {
                this.accessEnabledForAllResourceTypes = true;
            }
            else
            {
                ResourceType resourceType;
                if (!this.provider.TryResolveResourceType(typeName, out resourceType) || resourceType == null)
                {
                    throw new ArgumentException(Strings.DataServiceConfiguration_ResourceTypeNameNotFound(typeName), "typeName");
                }

                if (resourceType.ResourceTypeKind != ResourceTypeKind.ComplexType)
                {
                    throw new ArgumentException(Strings.DataServiceConfiguration_NotComplexType(typeName), "typeName");
                }

                Debug.Assert(resourceType.FullName == typeName, "resourceType.FullName == typeName");
                this.accessEnabledResourceTypes.Add(typeName);
            }
        }

        #endregion Public Methods

        #region Internal methods.

        /// <summary>Composes all query interceptors into a single expression.</summary>
        /// <param name="serviceInstance">Web service instance.</param>
        /// <param name="container">Container for which interceptors should run.</param>
        /// <returns>An expression the filter for query interceptors, possibly null.</returns>
        internal static Expression ComposeQueryInterceptors(object serviceInstance, ResourceSetWrapper container)
        {
            Debug.Assert(container != null, "container != null");
            MethodInfo[] methods = container.QueryInterceptors;
            if (methods == null || methods.Length == 0)
            {
                return null;
            }

            LambdaExpression filter = null;
            for (int i = 0; i < methods.Length; i++)
            {
                Expression predicate;
                try
                {
                    predicate = (Expression)methods[i].Invoke(serviceInstance, WebUtil.EmptyObjectArray);
                }
                catch (TargetInvocationException tie)
                {
                    ErrorHandler.HandleTargetInvocationException(tie);
                    throw;
                }

                if (predicate == null)
                {
                    throw new InvalidOperationException(Strings.DataService_AuthorizationReturnedNullQuery(methods[i].Name, methods[i].DeclaringType.FullName));
                }

                // predicate is LambdaExpression -- otherwise signature check missed something and following cast would throw
                LambdaExpression lambdaPredicate = ((LambdaExpression)predicate);

                if (filter == null)
                {
                    filter = lambdaPredicate;
                }
                else
                {
                    ParameterExpression parameter = filter.Parameters[0];
                    Expression adjustedPredicate = ParameterReplacerVisitor.Replace(
                        lambdaPredicate.Body,             // expression
                        lambdaPredicate.Parameters[0],    // oldParameter
                        parameter);                                     // newParameter
                    filter = Expression.Lambda(Expression.And(filter.Body, adjustedPredicate), parameter);
                }
            }

            return filter;
        }

        /// <summary>
        /// Composes the specified <paramref name="queryExpression"/> for the 
        /// given <paramref name="container"/> with authorization
        /// callbacks.
        /// </summary>
        /// <param name="service">Data service on which to invoke method.</param>
        /// <param name="container">resource set to compose with.</param>
        /// <param name="queryExpression">Query to compose.</param>
        /// <returns>The resulting composed query.</returns>
        internal static Expression ComposeResourceContainer(IDataService service, ResourceSetWrapper container, Expression queryExpression)
        {
            Debug.Assert(service != null, "service != null");
            Debug.Assert(container != null, "container != null");
            Debug.Assert(queryExpression != null, "queryExpression != null");

            MethodInfo[] methods = container.QueryInterceptors;
            if (methods != null)
            {
                for (int i = 0; i < methods.Length; i++)
                {
                    Expression predicate;
                    try
                    {
                        predicate = (Expression)methods[i].Invoke(service.Instance, WebUtil.EmptyObjectArray);
                    }
                    catch (TargetInvocationException tie)
                    {
                        ErrorHandler.HandleTargetInvocationException(tie);
                        throw;
                    }

                    if (predicate == null)
                    {
                        throw new InvalidOperationException(Strings.DataService_AuthorizationReturnedNullQuery(methods[i].Name, methods[i].DeclaringType.FullName));
                    }

                    // predicate is LambdaExpression -- otherwise signature check missed something and following cast would throw
                    queryExpression = queryExpression.QueryableWhere((LambdaExpression)predicate);
                }
            }

            return queryExpression;
        }

        /// <summary>Checks whether this request has the specified rights.</summary>
        /// <param name="container">Container to check.</param>
        /// <param name="requiredRights">Required rights.</param>
        /// <exception cref="DataServiceException">Thrown if <paramref name="requiredRights"/> aren't available.</exception>
        internal static void CheckResourceRights(ResourceSetWrapper container, EntitySetRights requiredRights)
        {
            Debug.Assert(container != null, "container != null");
            Debug.Assert(requiredRights != EntitySetRights.None, "requiredRights != EntitySetRights.None");

            if ((requiredRights & container.Rights) == 0)
            {
                throw DataServiceException.CreateForbidden();
            }
        }

        /// <summary>Checks whether this request has the specified reading rights.</summary>
        /// <param name="container">Container to check.</param>
        /// <param name="singleResult">Whether a single or multiple resources are requested.</param>
        /// <exception cref="DataServiceException">Thrown if <paramref name="singleResult"/> aren't available.</exception>
        internal static void CheckResourceRightsForRead(ResourceSetWrapper container, bool singleResult)
        {
            Debug.Assert(container != null, "container != null");
            EntitySetRights requiredRights = singleResult ? EntitySetRights.ReadSingle : EntitySetRights.ReadMultiple;
            CheckResourceRights(container, requiredRights);
        }

        /// <summary>Checks whether this request has the specified rights.</summary>
        /// <param name="operation">Operation to check.</param>
        /// <param name="requiredRights">Required rights.</param>
        /// <exception cref="DataServiceException">Thrown if <paramref name="requiredRights"/> aren't available.</exception>
        internal static void CheckServiceOperationRights(OperationWrapper operation, ServiceOperationRights requiredRights)
        {
            Debug.Assert(operation != null, "operation != null");
            Debug.Assert(requiredRights != ServiceOperationRights.None, "requiredRights != EntitySetRights.None");

            ServiceOperationRights effectiveRights = operation.ServiceOperationRights;
            if ((requiredRights & effectiveRights) == 0)
            {
                throw DataServiceException.CreateForbidden();
            }
        }

        /// <summary>Checks whether this request has the specified rights.</summary>
        /// <param name="operation">Operation to check.</param>
        /// <param name="singleResult">Whether a single or multiple resources are requested.</param>
        /// <exception cref="DataServiceException">Thrown if <paramref name="singleResult"/> aren't available.</exception>
        internal static void CheckServiceOperationRights(OperationWrapper operation, bool singleResult)
        {
            Debug.Assert(operation != null, "operation != null");

            if (operation.ResultKind != ServiceOperationResultKind.Void)
            {
                ServiceOperationRights requiredRights = singleResult ? ServiceOperationRights.ReadSingle : ServiceOperationRights.ReadMultiple;
                CheckServiceOperationRights(operation, requiredRights);
            }
        }

        /// <summary>Gets a string with methods allowed on the target for the <paramref name="description"/>.</summary>
        /// <param name="configuration">configuration object which has the data</param>
        /// <param name="description">Description with target.</param>
        /// <returns>A string with methods allowed on the description; possibly null.</returns>
        internal static string GetAllowedMethods(DataServiceConfiguration configuration, RequestDescription description)
        {
            Debug.Assert(description != null, "description != null");
            Debug.Assert(
                description.TargetKind != RequestTargetKind.Nothing,
                "description.TargetKind != RequestTargetKind.Void - otherwise it hasn't been determined yet");
            Debug.Assert(
                description.TargetKind != RequestTargetKind.VoidOperation,
                "description.TargetKind != RequestTargetKind.VoidOperation - this method is only for containers");
            if (description.TargetKind == RequestTargetKind.Metadata ||
                description.TargetKind == RequestTargetKind.ServiceDirectory)
            {
                return XmlConstants.HttpMethodGet;
            }

            if (description.TargetKind == RequestTargetKind.Batch)
            {
                return XmlConstants.HttpMethodPost;
            }

            int index = description.GetIndexOfTargetEntityResource();
            Debug.Assert(index >= 0 && index < description.SegmentInfos.Count, "index >=0 && index <description.SegmentInfos.Count");
            ResourceSetWrapper container = description.SegmentInfos[index].TargetResourceSet;
            return GetAllowedMethods(configuration, container, description);
        }

        /// <summary>
        /// Gets a string representation of allowed methods on the container (with the specified target cardinality),
        /// suitable for an 'Allow' header.
        /// </summary>
        /// <param name="configuration">configuration object which has the data</param>
        /// <param name="container">Targetted container, possibly null.</param>
        /// <param name="description">Description with target.</param>
        /// <returns>A value for an 'Allow' header; null if <paramref name="container"/> is null.</returns>
        internal static string GetAllowedMethods(DataServiceConfiguration configuration, ResourceSetWrapper container, RequestDescription description)
        {
            if (container == null)
            {
                return null;
            }

            System.Text.StringBuilder result = new System.Text.StringBuilder();
            EntitySetRights rights = configuration.GetResourceSetRights(container.ResourceSet);
            if (description.IsSingleResult)
            {
                AppendRight(rights, EntitySetRights.ReadSingle, XmlConstants.HttpMethodGet, result);
                AppendRight(rights, EntitySetRights.WriteReplace, XmlConstants.HttpMethodPut, result);
                if (description.TargetKind != RequestTargetKind.MediaResource)
                {
                    AppendRight(rights, EntitySetRights.WriteMerge, XmlConstants.HttpMethodPatch, result);
                    AppendRight(rights, EntitySetRights.WriteDelete, XmlConstants.HttpMethodDelete, result);
                }
            }
            else
            {
                AppendRight(rights, EntitySetRights.ReadMultiple, XmlConstants.HttpMethodGet, result);
                AppendRight(rights, EntitySetRights.WriteAppend, XmlConstants.HttpMethodPost, result);
            }

            return result.ToString();
        }

        /// <summary>Gets the effective rights on the specified container.</summary>
        /// <param name="container">Container to get rights for.</param>
        /// <returns>The effective rights as per this configuration.</returns>
        internal EntitySetRights GetResourceSetRights(ResourceSet container)
        {
            Debug.Assert(container != null, "container != null");
            Debug.Assert(this.resourceRights != null, "this.resourceRights != null");

            EntitySetRights result;
            if (!this.resourceRights.TryGetValue(container.Name, out result))
            {
                result = this.rightsForUnspecifiedResourceContainer;
            }

            return result;
        }

        /// <summary>Gets the effective rights on the specified operation.</summary>
        /// <param name="serviceOperation">Operation to get rights for.</param>
        /// <returns>The effective rights as per this configuration.</returns>
        internal ServiceOperationRights GetServiceOperationRights(ServiceOperation serviceOperation)
        {
            Debug.Assert(serviceOperation != null, "operation != null");
            Debug.Assert(this.serviceOperationRights != null, "this.serviceOperationRights != null");

            ServiceOperationRights result;
            if (!this.serviceOperationRights.TryGetValue(serviceOperation.Name, out result))
            {
                result = this.rightsForUnspecifiedServiceOperation;
            }

            return result;
        }

        /// <summary>Gets the effective rights on the specified action.</summary>
        /// <param name="serviceAction">Action to get rights for.</param>
        /// <returns>The effective rights as per this configuration.</returns>
        internal ServiceActionRights GetServiceActionRights(ServiceAction serviceAction)
        {
            Debug.Assert(serviceAction != null, "serviceAction != null");
            Debug.Assert(this.serviceActionRights != null, "this.serviceActionRights != null");

            ServiceActionRights result;
            if (!this.serviceActionRights.TryGetValue(serviceAction.Name, out result))
            {
                result = this.rightsForUnspecifiedServiceAction;
            }

            return result;
        }

        /// <summary>Gets the page size per entity set</summary>
        /// <param name="container">Entity set for which to get the page size</param>
        /// <returns>Page size for the <paramref name="container"/></returns>
        internal int GetResourceSetPageSize(ResourceSet container)
        {
            Debug.Assert(container != null, "container != null");
            Debug.Assert(this.pageSizes != null, "this.pageSizes != null");

            int pageSize;
            if (!this.pageSizes.TryGetValue(container.Name, out pageSize))
            {
                pageSize = this.defaultPageSize;
            }

            return pageSize;
        }

        /// <summary>Returns the list of types registered by the data service.</summary>
        /// <returns>The list of types as registered by the data service</returns>
        internal IEnumerable<Type> GetKnownTypes()
        {
            return this.knownTypes;
        }

        /// <summary>Get the list of access enabled resourceType names.</summary>
        /// <returns>List of namespace qualified resourceType names that were marked as visible by calling EnableAccess().</returns>
        internal IEnumerable<string> GetAccessEnabledResourceTypes()
        {
            Debug.Assert(this.accessEnabledResourceTypes != null, "this.accessEnabledResourceTypes != null");
            return this.accessEnabledResourceTypes;
        }

        /// <summary>
        /// Initializes the DataServiceConfiguration instance by:
        /// 1. Invokes the static service initialization methods on the specified type family.
        /// 2. Register authorization callbacks specified on the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Type of service to initialize for.</param>
        internal void Initialize(Type type)
        {
            Debug.Assert(type != null, "type != null");
            this.InvokeStaticInitialization(type);
        }

        /// <summary>Seals this configuration instance and prevents further changes.</summary>
        /// <remarks>
        /// This method should be called after the configuration has been set up and before it's placed on the
        /// metadata cache for sharing.
        /// </remarks>
        internal void Seal()
        {
            Debug.Assert(!this.configurationSealed, "!configurationSealed - otherwise .Seal is invoked multiple times");
            this.configurationSealed = true;
            this.provider = null;
        }

        /// <summary>
        /// Validated if server options used by the service are compatible with MaxProtocolVersion
        /// </summary>
        internal void ValidateServerOptions()
        {
            Debug.Assert(this.configurationSealed, "Configuration must be sealed to validate server options");
        }

        /// <summary>
        /// Indicates whether the metadata includes annotations
        /// </summary>
        /// <returns>true if the metadata includes annotations; false otherwise.</returns>
        internal bool HasAnnotations()
        {
            // Devnote: simplification here, bumping if there is something on the annotation builder
            // otherwise we would need to relook at the model after applying annotations
            // to see if its V3. Trusting that usage of this property indicates its V3
            if (this.AnnotationsBuilder != null)
            {
                return true;
            }

            // Indicate that the model will have an annotation because the generate key as segment will add one
            if (this.DataServiceBehavior.GenerateKeyAsSegment)
            {
                return true;
            }

            return false;
        }

        #endregion Internal methods.

        #region Private methods.

        /// <summary>
        /// Appends the <paramref name="name"/> of a right if the <paramref name="test"/> right is enabled 
        /// on <paramref name="entitySetRights"/>.
        /// </summary>
        /// <param name="entitySetRights">Rights to be checked.</param>
        /// <param name="test">Right being looked for.</param>
        /// <param name="name">Name of right to append.</param>
        /// <param name="builder">Comma-separated list of right names to append to.</param>
        private static void AppendRight(EntitySetRights entitySetRights, EntitySetRights test, string name, System.Text.StringBuilder builder)
        {
            Debug.Assert(builder != null, "builder != null");
            if (0 != (entitySetRights & test))
            {
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(name);
            }
        }

        /// <summary>
        /// Invokes the static service initialization methods on the 
        /// specified type family.
        /// </summary>
        /// <param name="type">Type of service to initialize for.</param>
        private void InvokeStaticInitialization(Type type)
        {
            Debug.Assert(type != null, "type != null");

            // Build a stack going from most-specific to least-specific type.
            const BindingFlags Flags = BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public;
            while (type != null)
            {
                MethodInfo method = type.GetMethod(XmlConstants.ClrServiceInitializationMethodName, Flags, null, new Type[] { typeof(IDataServiceConfiguration) }, null) ??
                                    type.GetMethod(XmlConstants.ClrServiceInitializationMethodName, Flags, null, new Type[] { typeof(DataServiceConfiguration) }, null);

                if (method != null && method.ReturnType == typeof(void))
                {
                    Debug.Assert(method.IsStatic, "method.IsStatic");
                    Debug.Assert(method.Name == XmlConstants.ClrServiceInitializationMethodName, "Making sure that the method name is as expected");

                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length == 1 && !parameters[0].IsOut)
                    {
                        object[] initializeParameters = new object[] { this };
                        try
                        {
                            method.Invoke(null, initializeParameters);
                        }
                        catch (TargetInvocationException exception)
                        {
                            ErrorHandler.HandleTargetInvocationException(exception);
                            throw;
                        }

                        return;
                    }
                }

                type = type.BaseType;
            }
        }

        /// <summary>
        /// Checks that the specified <paramref name="value"/> for the named property is not negative and that the
        /// configuration isn't sealed.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="propertyName">Parameter name.</param>
        /// <returns>The <paramref name="value"/> to set.</returns>
        /// <remarks>
        /// This method is typically used in properties with the following pattern:
        /// <code>public int Foo { get {... } set { this.foo = this.CheckNonNegativeProperty(value, "Foo"); } }</code>
        /// </remarks>
        private int CheckNonNegativeProperty(int value, string propertyName)
        {
            this.CheckNotSealed();
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("value", value, Strings.PropertyRequiresNonNegativeNumber(propertyName));
            }

            return value;
        }

        /// <summary>Checks that this configuration hasn't been sealed yet.</summary>
        private void CheckNotSealed()
        {
            if (this.configurationSealed)
            {
                string message = Strings.DataServiceConfiguration_NoChangesAllowed(XmlConstants.ClrServiceInitializationMethodName);
                throw new InvalidOperationException(message);
            }
        }

        #endregion Private methods.
    }
}
