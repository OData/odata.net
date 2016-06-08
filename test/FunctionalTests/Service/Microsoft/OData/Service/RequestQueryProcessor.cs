//---------------------------------------------------------------------
// <copyright file="RequestQueryProcessor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Service.Internal;
    using Microsoft.OData.Service.Parsing;
    using Microsoft.OData.Service.Providers;

    /// <summary>Use this class to process a web data service request URI.</summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Tying together a lot of concepts in the OData URI space with the equivalent LINQ concepts.")]
    internal class RequestQueryProcessor
    {
        #region Private fields

#pragma warning disable 618 // Disable "obsolete" warning for the IExpandProvider interface. Used for backwards compatibilty.
        /// <summary>MethodInfo for IExpandProvider.ApplyExpansions().</summary>
        private static readonly MethodInfo ApplyExpansionsMethodInfo = typeof(IExpandProvider).GetMethod("ApplyExpansions", BindingFlags.Public | BindingFlags.Instance);
#pragma warning restore 618

        /// <summary>Original description over which query composition takes place.</summary>
        private readonly RequestDescription description;

        /// <summary>Service with data and configuration.</summary>
        private readonly IDataService service;

        /// <summary>Whether the $orderby, $skip, $take and $count options can be applied to the request.</summary>
        private readonly bool setQueryApplicable;

        /// <summary>Whether the top level request is a candidate for paging.</summary>
        private readonly bool pagingApplicable;

        /// <summary>The skip-token expression builder to use.</summary>
        private readonly SkipTokenExpressionBuilder skipTokenExpressionBuilder;

        /// <summary>Has custom paging already been applied?</summary>
        private bool appliedCustomPaging;

        /// <summary>List of paths to be expanded.</summary>
        private List<ExpandSegmentCollection> expandPaths;

        /// <summary>Root projection segment of the tree specifying projections and expansions for the query.</summary>
        private RootProjectionNode rootProjectionNode;

        /// <summary>Collection of ordering expressions for the current query</summary>
        private OrderingInfo topLevelOrderingInfo;

#if DEBUG
        /// <summary>Whether any order has been applied.</summary>
        private bool orderApplied;
#endif

        /// <summary>Value of $skip argument</summary>
        private int? skipCount;

        /// <summary>Value of $top argument</summary>
        private int? topCount;

        /// <summary>Query expression being composed.</summary>
        private Expression queryExpression;

        #endregion Private fields

        /// <summary>Initializes a new <see cref="RequestQueryProcessor"/> instance.</summary>
        /// <param name="service">Service with data and configuration.</param>
        /// <param name="description">Description for request processed so far.</param>
        private RequestQueryProcessor(IDataService service, RequestDescription description)
        {
            this.service = service;
            this.description = description;
#if DEBUG
            this.orderApplied = false;
#endif
            this.skipCount = null;
            this.topCount = null;
            this.queryExpression = description.RequestExpression;

            this.setQueryApplicable = (description.TargetKind == RequestTargetKind.Resource && !description.IsSingleResult) ||
                                       description.CountOption == RequestQueryCountOption.CountSegment;

            // Server Driven Paging is not considered for the following cases: 
            // 1. Top level result is not or resource type or it is a single valued result.
            // 2. $count segment provided.
            // 3. Non-GET requests do not honor SDP.
            // 4. Only exception for Non-GET requests is if the request is coming from a Service
            //    operation that returns a set of result values of entity type.
            this.pagingApplicable = (description.TargetKind == RequestTargetKind.Resource && !description.IsSingleResult) &&
                                    (description.CountOption != RequestQueryCountOption.CountSegment) &&
                                    !description.IsRequestForEnumServiceOperation &&
                                    (service.OperationContext.RequestMessage.HttpVerb.IsQuery() || description.SegmentInfos[0].TargetSource == RequestTargetSource.ServiceOperation);

            this.appliedCustomPaging = false;

            this.rootProjectionNode = null;
            this.topLevelOrderingInfo = null;

            this.skipTokenExpressionBuilder = new SkipTokenExpressionBuilder(NodeToExpressionTranslator.Create(this.service, description, Expression.Parameter(typeof(object))));
        }

        /// <summary>
        /// Is the top level container for the query paged i.e. we need to use StandardPaging.
        /// </summary>
        private bool IsStandardPaged
        {
            get
            {
                if (this.pagingApplicable && !this.IsCustomPaged)
                {
                    return this.IsPageSizeDefined;
                }

                // Target container is null for OpenProperties and we have decided not to 
                // to enable paging for OpenType scenarios
                return false;
            }
        }

        /// <summary>
        /// Does the top level container for the query have page size limits defined.
        /// </summary>
        private bool IsPageSizeDefined
        {
            get
            {
                ResourceSetWrapper targetContainer = this.description.LastSegmentInfo.TargetResourceSet;
                Debug.Assert(targetContainer != null, "Must have target container for non-open properties");
                return targetContainer.PageSize > 0;
            }
        }

        /// <summary>Do we need to use CustomPaging for this service.</summary>
        private bool IsCustomPaged
        {
            get
            {
                return this.service.PagingProvider.IsCustomPagedForQuery;
            }
        }

        /// <summary>
        /// Processes query arguments and returns a request description for 
        /// the resulting query.
        /// </summary>
        /// <param name="service">Service with data and configuration information.</param>
        /// <param name="description">Description for request processed so far.</param>
        /// <returns>A new <see cref="RequestDescription"/>.</returns>
        internal static RequestDescription ProcessQuery(IDataService service, RequestDescription description)
        {
            Debug.Assert(service != null, "service != null");

            // When the request doesn't produce an IQueryable result or it is a service action
            // we can short-circuit all further processing.
            if (description.RequestExpression == null || description.IsServiceActionRequest || !(typeof(IQueryable).IsAssignableFrom(description.RequestExpression.Type)))
            {
                WebUtil.CheckEmptyQueryArguments(service, false /*checkForOnlyV2QueryParameters*/);
                return description;
            }

            return new RequestQueryProcessor(service, description).ProcessQuery();
        }

        /// <summary>
        /// Apply projection for the given property to the parent projection node.
        /// </summary>
        /// <param name="parentNode">The parent node which the new node will be added to.</param>
        /// <param name="propertyName">Name of the property that needs to be projected.</param>
        /// <param name="property">ResourceProperty instance containing information about the property - this will be null for open properties.</param>
        /// <param name="targetResourceType">ResourceType instance on which the property needs to be expanded.</param>
        /// <returns>A new or an existing instance of the ExpandedProjectionNode for the given property.</returns>
        private static ExpandedProjectionNode ApplyProjectionForProperty(ExpandedProjectionNode parentNode, string propertyName, ResourceProperty property, ResourceType targetResourceType)
        {
            Debug.Assert(parentNode != null, "parentNode != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(targetResourceType != null, "targetResourceType != null");

            var childNode = parentNode.AddProjectionNode(propertyName, property, targetResourceType);
            Debug.Assert(
                childNode == null || childNode.ResourceType == property.ResourceType,
                "If we're traversing over a nav. property it's resource type must match the resource type of the expanded segment.");
            
            return childNode;
        }

        /// <summary>
        /// Applies the selection for the given operation segment to the given projection node.
        /// </summary>
        /// <param name="operationSegment">The operation segment to apply to the projection node.</param>
        /// <param name="currentNode">The current projection node.</param>
        /// <param name="targetResourceType">The target type based on type segments.</param>
        private static void ApplySelectionForOperations(OperationSegment operationSegment, ExpandedProjectionNode currentNode, ResourceType targetResourceType)
        {
            IEnumerable<OperationWrapper> selectedOperations = operationSegment.Operations.Select(f =>
            {
                var metadataProviderEdmFunctionImport = f as MetadataProviderEdmOperation;
                Debug.Assert(metadataProviderEdmFunctionImport != null, "metadataProviderEdmFunctionImport != null");

                OperationWrapper operation = metadataProviderEdmFunctionImport.ServiceOperation;
                Debug.Assert(operation != null, "operation != null");
                return operation;
            });

            // Note that AddSelectedOperations will return false if the enumerable is empty.
            bool anyOperations = currentNode.SelectedOperations.AddSelectedOperations(targetResourceType, selectedOperations);
            Debug.Assert(anyOperations, "Operations segment should not have been created if no operations were found.");
        }

        /// <summary>
        /// Applies the given path-based selection item to the given projection node.
        /// </summary>
        /// <param name="path">The path being selected.</param>
        /// <param name="currentNode">The current projection node.</param>
        /// <returns>A new or an existing instance of the ExpandedProjectionNode for path of the selection item.</returns>
        private ExpandedProjectionNode ApplyPathSelection(ODataPath path, ExpandedProjectionNode currentNode)
        {
            Debug.Assert(path != null, "path != null");

            ResourceType targetResourceType = this.GetTargetResourceTypeFromTypeSegments(path, currentNode.ResourceType);
            Debug.Assert(targetResourceType != null, "targetResourceType != null");

            ODataPathSegment lastSegment = path.LastSegment;
            Debug.Assert(lastSegment != null, "lastSegment != null");

            var operationSegment = lastSegment as OperationSegment;
            if (operationSegment != null)
            {
                ApplySelectionForOperations(operationSegment, currentNode, targetResourceType);
                return currentNode;
            }

            var openPropertySegment = lastSegment as DynamicPathSegment;
            if (openPropertySegment != null)
            {
                return ApplyProjectionForProperty(currentNode, openPropertySegment.Identifier, null /*property*/, targetResourceType);
            }

            var propertySegment = lastSegment as PropertySegment;
            IEdmProperty edmProperty;
            if (propertySegment == null)
            {
                var navigationSegment = lastSegment as NavigationPropertySegment;
                Debug.Assert(navigationSegment != null, "Unexpected type of select segment: " + lastSegment.GetType());
                edmProperty = navigationSegment.NavigationProperty;
            }
            else
            {
                edmProperty = propertySegment.Property;
            }

            ResourceProperty resourceProperty = ((IResourcePropertyBasedEdmProperty)edmProperty).ResourceProperty;
            Debug.Assert(resourceProperty != null, "resourceProperty != null");

            // Look for any navigation properties that are hidden (EntitySetRights set to None)
            // If it is hidden, throw exception so we don't do any information disclousure
            // See Information disclosure: Astoria server allows 'hidden' navigation properties to be selected
            if (edmProperty.PropertyKind == EdmPropertyKind.Navigation && this.service.Provider.GetResourceSet(currentNode.ResourceSetWrapper, targetResourceType, resourceProperty) == null)
            {
                throw DataServiceException.CreateBadRequestError(Strings.BadRequest_InvalidPropertyNameSpecified(resourceProperty.Name, targetResourceType.FullName));
            }

            return ApplyProjectionForProperty(currentNode, resourceProperty.Name, resourceProperty, targetResourceType);
        }

        /// <summary>
        /// Gets the target resource type based on type segments in the given path.
        /// </summary>
        /// <param name="path">The path to traverse.</param>
        /// <param name="startingType">The type to start on.</param>
        /// <returns>The final target resource type of the path based on type segments.</returns>
        private ResourceType GetTargetResourceTypeFromTypeSegments(IEnumerable<ODataPathSegment> path, ResourceType startingType)
        {
            Debug.Assert(path != null, "path != null");

            ResourceType targetResourceType = startingType;
            ResourceType previousSegmentResourceType = startingType;

            bool previousSegmentIsTypeSegment = false;
            foreach (ODataPathSegment currentSegment in path)
            {
                var typeSegment = currentSegment as TypeSegment;
                if (typeSegment != null)
                {
                    // Whenever we encounter the type segment, we need to only verify that the MPV is set to 4.0 or higher.
                    // There is no need to check for request DSV request MaxDSV since there are no protocol changes in
                    // the payload for uri's with type identifier.
                    this.description.VerifyProtocolVersion(VersionUtil.Version4Dot0, this.service);

                    targetResourceType = MetadataProviderUtils.GetResourceType(typeSegment);
                    Debug.Assert(targetResourceType != null, "targetResourceType != null");

                    if (previousSegmentIsTypeSegment)
                    {
                        // If 2 identifiers are specified back to back, then we need to check and throw in that scenario
                        throw DataServiceException.CreateBadRequestError(Strings.RequestUriProcessor_TypeIdentifierCannotBeSpecifiedAfterTypeIdentifier(targetResourceType.FullName, previousSegmentResourceType.FullName));
                    }

                    // Currently we do not support upcasts at all. Since assignable checks for equality, its okay to specify
                    // redundant casts to the same type.
                    if (!previousSegmentResourceType.IsAssignableFrom(targetResourceType))
                    {
                        throw DataServiceException.CreateBadRequestError(Strings.RequestUriProcessor_InvalidTypeIdentifier_MustBeASubType(targetResourceType.FullName, previousSegmentResourceType.FullName));
                    }

                    previousSegmentIsTypeSegment = true;
                }
            }

            return targetResourceType;
        }

        /// <summary>Gets the root projection node or creates one if no one exists yet.</summary>
        /// <returns>The root node of the projection tree.</returns>
        private RootProjectionNode GetRootProjectionNode()
        {
            if (this.rootProjectionNode == null)
            {
                // Build the root of the projection and expansion tree
                this.rootProjectionNode = new RootProjectionNode(
                    this.description.LastSegmentInfo.TargetResourceSet,
                    this.topLevelOrderingInfo,
                    null,
                    this.skipCount,
                    this.topCount,
                    null,
                    this.expandPaths,
                    this.description.TargetResourceType,
                    this.description.ExpandAndSelect.Clause);
            }

            return this.rootProjectionNode;
        }

        /// <summary>Checks and resolved all textual expand paths and removes unnecessary paths.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Won't Fix")]
        private void ApplyExpandPathsToProjectionNodes()
        {
            this.expandPaths = new List<ExpandSegmentCollection>();
            if (this.description.ExpandAndSelect.HasExpand)
            {
                // $expand is only applicable to true queries (must have IQueryable)
                if (this.queryExpression == null)
                {
                    throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_QueryExpandOptionNotApplicable);
                }

                var rootNode = this.GetRootProjectionNode();
                this.ExtractExpandPathSegmentCollections(null, rootNode, rootNode);
            }
        }

        /// <summary>
        /// Performs a depth-first walk down the expand tree, copying and adding to the current path as it goes. When the bottom of a path is reached, the path is added to the overall set of paths.
        /// </summary>
        /// <param name="currentPath">The current path so far for this depth-first traversal of the tree. Starts out null.</param>
        /// <param name="currentNode">The current node of the expand tree.</param>
        /// <param name="rootNode">The root node of the expand tree.</param>
        private void ExtractExpandPathSegmentCollections(ExpandSegmentCollection currentPath, ExpandedProjectionNode currentNode, RootProjectionNode rootNode)
        {
            ExpandSegmentCollection nextPath = null;
            foreach (ExpandedNavigationSelectItem expandItem in currentNode.SelectExpandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>())
            {
                rootNode.ExpansionsSpecified = true;

                if (currentPath == null)
                {
                    nextPath = new ExpandSegmentCollection();
                }
                else
                {
                    // create a copy of the current path.
                    nextPath = new ExpandSegmentCollection(currentPath.Count);
                    nextPath.AddRange(currentPath);
                }

                var segment = this.CreateExpandSegment(expandItem, currentNode.ResourceType);
                nextPath.Add(segment);
                ExpandedProjectionNode childNode = currentNode.AddExpandedNode(segment, expandItem.SelectAndExpand);

                // if there are any derived expansions in the tree, set the rootNode.DerivedExpansionsSpecified to true.
                if (currentNode.HasExpandedPropertyOnDerivedType)
                {
                    rootNode.ExpansionOnDerivedTypesSpecified = true;
                }

                this.ExtractExpandPathSegmentCollections(nextPath, childNode, rootNode);
            }

            if (nextPath == null && currentPath != null)
            {
                this.expandPaths.Add(currentPath);
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="ExpandSegment"/> based on the metadata in the given <see cref="ExpandedNavigationSelectItem"/>.
        /// </summary>
        /// <param name="expandItem">The metadata-bound expand segment to create the <see cref="ExpandSegment"/> from.</param>
        /// <param name="currentResourceType">The current resource type.</param>
        /// <returns>The created <see cref="ExpandSegment"/>.</returns>
        private ExpandSegment CreateExpandSegment(ExpandedNavigationSelectItem expandItem, ResourceType currentResourceType)
        {
            Debug.Assert(expandItem != null, "expandItem != null");
            Debug.Assert(currentResourceType != null, "currentResourceType != null");

            // pull the Resource* instances from annotations on the IEdm* instances.
            IEdmNavigationProperty navigationProperty = ((NavigationPropertySegment)expandItem.PathToNavigationProperty.LastSegment).NavigationProperty;
            ResourceProperty property = ((IResourcePropertyBasedEdmProperty)navigationProperty).ResourceProperty;
            Debug.Assert(property != null, "property != null");
            ResourceSetWrapper targetResourceSet = ((IResourceSetBasedEdmEntitySet)expandItem.NavigationSource).ResourceSet;
            Debug.Assert(targetResourceSet != null, "targetResourceSet != null");

            // Further scope the type based on type segments in the path.
            currentResourceType = this.GetTargetResourceTypeFromTypeSegments(expandItem.PathToNavigationProperty, currentResourceType);
            Debug.Assert(currentResourceType != null, "currentResourceType != null");

            // The expanded resource type may require higher response version. Update version of the response accordingly
            //
            // Note the response DSV is payload specific and since for GET we won't know what DSV the instances to be
            // serialized will require until serialization time which happens after the headers are written, 
            // the best we can do is to determine this at the set level.
            this.description.UpdateVersion(targetResourceSet, this.service);

            bool singleResult = property.Kind == ResourcePropertyKind.ResourceReference;
            DataServiceConfiguration.CheckResourceRightsForRead(targetResourceSet, singleResult);

            Expression filter = DataServiceConfiguration.ComposeQueryInterceptors(this.service.Instance, targetResourceSet);

            if (targetResourceSet.PageSize != 0 && !singleResult && !this.IsCustomPaged)
            {
                OrderingInfo internalOrderingInfo = new OrderingInfo(true);
                ParameterExpression p = Expression.Parameter(targetResourceSet.ResourceType.InstanceType, "p");
                foreach (var keyProp in targetResourceSet.GetKeyPropertiesForOrderBy())
                {
                    Expression e;
                    if (keyProp.CanReflectOnInstanceTypeProperty)
                    {
                        e = Expression.Property(p, targetResourceSet.ResourceType.GetPropertyInfo(keyProp));
                    }
                    else
                    {
                        // object LateBoundMethods.GetValue(object, ResourceProperty)
                        e = Expression.Call(
                                null, /*instance*/
                                DataServiceProviderMethods.GetValueMethodInfo, 
                                p,
                                Expression.Constant(keyProp));
                        e = Expression.Convert(e, keyProp.Type);
                    }

                    internalOrderingInfo.Add(new OrderingExpression(Expression.Lambda(e, p), true));
                }

                return new ExpandSegment(property.Name, filter, targetResourceSet.PageSize, targetResourceSet, currentResourceType, property, internalOrderingInfo);
            }

            if (!singleResult && this.IsCustomPaged)
            {
                // Expansion of collection could result in custom paging provider giving next link, so we need to set the null continuation token.
                this.CheckAndApplyCustomPaging(null);
            }

            return new ExpandSegment(property.Name, filter, this.service.Configuration.MaxResultsPerCollection, targetResourceSet, currentResourceType, property, null);
        }

        /// <summary>Checks that set query options are applicable to this request.</summary>
        private void CheckSetQueryApplicable()
        {
            if (!this.setQueryApplicable)
            {
                throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_QuerySetOptionsNotApplicable);
            }
        }

        /// <summary>Processes the $expand argument of the request.</summary>
        private void ProcessExpand()
        {
            this.ApplyExpandPathsToProjectionNodes();

            // Check the expand count.
            // DEVNOTE: Prior to URI parser integration the server used to do this check based purely on path segments.
            // However, the equivalent check in the URI parser checks based on the number of nodes in the tree. So, we need to keep this
            // check here for cases where the URI parser's check will be too loose.
            Debug.Assert(this.expandPaths != null, "expandPaths != null");
            Debug.Assert(this.service != null, "this.service != null");
            Debug.Assert(this.service.Configuration != null, "this.service.Configuration != null");

            int actualExpandCount = this.expandPaths.Sum(collection => collection.Count);
            if (this.service.Configuration.MaxExpandCount < actualExpandCount)
            {
                throw DataServiceException.CreateBadRequestError(Strings.DataService_ExpandCountExceeded(actualExpandCount, this.service.Configuration.MaxExpandCount));
            }
        }

        /// <summary>Builds the tree of <see cref="ProjectionNode"/> to represent the $select query option.</summary>
        /// <remarks>This method assumes that $expand was already processed. And we have the tree
        /// of <see cref="ExpandedProjectionNode"/> objects for the $expand query option already built.</remarks>
        private void ApplyProjectionsToExpandTree()
        {
            ExpandedProjectionNode currentNode = this.GetRootProjectionNode();
            Debug.Assert(currentNode.ResourceType == this.description.TargetResourceType, "The resource type of the root doesn't match the target type of the query.");

            this.ApplyProjectionsToExpandTree(currentNode);
        }

        /// <summary>
        /// Applies projections from the given select/expand clause to the tree represented by the given node.
        /// </summary>
        /// <param name="currentNode">The expand tree to apply projections to.</param>
        private void ApplyProjectionsToExpandTree(ExpandedProjectionNode currentNode)
        {
            // If this is the last segment in the path and it was a navigation property,
            // mark it to include the entire subtree
            if (currentNode.SelectExpandClause.AllSelected)
            {
                currentNode.ProjectionFound = true;
                currentNode.MarkSubtreeAsProjected();
            }

            foreach (var selectItem in currentNode.SelectExpandClause.SelectedItems)
            {
                currentNode.ProjectionFound = true;
             
                // '*' is special, it means "Project all immediate properties on this level."
                if (selectItem is WildcardSelectItem)
                {
                    currentNode.ProjectAllImmediateProperties = true;
                    continue;
                }

                if (selectItem is NamespaceQualifiedWildcardSelectItem)
                {
                    currentNode.ProjectAllImmediateOperations = true;
                    continue;
                }

                ODataPath path;

                var expandItem = selectItem as ExpandedNavigationSelectItem;
                if (expandItem != null)
                {
                    path = expandItem.PathToNavigationProperty;
                }
                else
                {
                    var pathSelectionItem = selectItem as PathSelectItem;
                    Debug.Assert(pathSelectionItem != null, "Unexpeced selection item type: " + selectItem.GetType());
                    path = pathSelectionItem.SelectedPath;
                }

                ExpandedProjectionNode childNode = this.ApplyPathSelection(path, currentNode);

                if (expandItem != null)
                {
                    if (childNode == null)
                    {
                        throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_ProjectedPropertyWithoutMatchingExpand(((NavigationPropertySegment)path.LastSegment).NavigationProperty.Name));
                    }

                    this.ApplyProjectionsToExpandTree(childNode);
                }
            }
        }

        /// <summary>Processes the $expand and $select query options.</summary>
        private void ProcessExpandAndSelect()
        {
            this.ProcessExpand();
            this.ProcessSelect();
        }

        /// <summary>Processes the $select argument of the request.</summary>
        private void ProcessSelect()
        {
            if (!this.description.ExpandAndSelect.HasSelect)
            {
                // No projections specified on the input
                if (this.rootProjectionNode != null)
                {
                    // There are expansions, but no projections
                    // Mark all the expanded nodes in the tree to include all properties.
                    this.rootProjectionNode.MarkSubtreeAsProjected();
                }

                return;
            }

            // We only allow $select on true queries (we must have IQueryable)
            if (this.queryExpression == null)
            {
                throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_QuerySelectOptionNotApplicable);
            }

            // We found some projections in the query - mark it as such
            this.GetRootProjectionNode().ProjectionsSpecified = true;

            // Apply projections to the $expand tree
            this.ApplyProjectionsToExpandTree();

            // Cleanup the tree
            if (this.rootProjectionNode != null)
            {
                // Remove all expanded nodes which are not projected
                this.rootProjectionNode.RemoveNonProjectedNodes();

                // Now cleanup the projected tree. We already eliminated explicit duplicates during construction
                //   now we want to remove redundant properties when * was used or when the subtree was already selected.
                this.rootProjectionNode.ApplyWildcardsAndSort(this.service.Provider);
            }
        }

        /// <summary>
        /// Generate the queryResults for the request
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining | System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
        private void GenerateQueryResult()
        {
            if (this.description.CountOption == RequestQueryCountOption.CountSegment)
            {
                // set query result to Count
                this.queryExpression = this.queryExpression.QueryableLongCount();
            }
            else if (this.rootProjectionNode != null)
            {
#pragma warning disable 618 // Disable "obsolete" warning for the IExpandProvider interface. Used for backwards compatibilty.
                IExpandProvider expandProvider = this.service.Provider.GetService<IExpandProvider>();
#pragma warning restore 618

                if (expandProvider != null)
                {
                    // If top level result is paged, then we can not perform the operation since that would require
                    // passing in the ordering expressions for $skiptoken generation for top level results which
                    // the IExpandProvider interface does not support and thus we would get wrong results
                    if (this.IsStandardPaged)
                    {
                        throw new DataServiceException(500, Strings.DataService_SDP_TopLevelPagedResultWithOldExpandProvider);
                    }

                    // If there's projection we can't perform the operation since that would require
                    // passing the projection info into the IExpandProvider, which it doesn't support
                    // and thus would not perform the projection.
                    if (this.rootProjectionNode.ProjectionsSpecified)
                    {
                        throw new DataServiceException(500, Strings.DataService_Projections_ProjectionsWithOldExpandProvider);
                    }

                    if (this.rootProjectionNode.ExpansionOnDerivedTypesSpecified)
                    {
                        throw new DataServiceException(500, Strings.DataService_DerivedExpansions_OldExpandProvider);
                    }

                    // V1 behaviour of expand in this case, although this is semantically incorrect, since we are doing
                    // a select after orderby and skip top, which could mess up the results, assuming here that count
                    // has already been processed
                    this.ProcessOrderBy();
                    this.ProcessSkipAndTop();
                    this.queryExpression = Expression.Call(
                        Expression.Constant(expandProvider),
                        RequestQueryProcessor.ApplyExpansionsMethodInfo,
                        this.queryExpression,
                        Expression.Constant(this.rootProjectionNode.ExpandPaths));

                    // We need to mark the ExpandPaths as "possibly modified" and make serializer use those instead.
                    this.rootProjectionNode.UseExpandPathsForSerialization = true;
                }
                else
                {
                    IProjectionProvider projectionProvider = this.service.Provider.ProjectionProvider;

                    // We already have the parameter information
                    // * Ordering expressions through ObtainOrderingExpressions
                    // * Skip & Top through ObtainSkipTopCounts
                    if (projectionProvider == null)
                    {
                        Debug.Assert(!this.service.Provider.HasReflectionOrEFProviderQueryBehavior, "Reflection and EF Providers should implement the IProjectionProvider interface.");

                        // We are to run a query against IDSQP. Since the IProjectionProvider interface is not public
                        //   the provider will have to be able to handle queries generated by our BasicExpandProvider
                        //   so we should make only minimalistic assumptions about the provider's abilities.
                        // In here we will assume that:
                        //   - the provider doesn't expand on demand, that is we need to generate projections for all expansions in the query
                        //   - the provider requires correct casting to "System.Object" when we assign a value to a property of type "System.Object"
                        // A side effect of these assumptions is that if the provider just propagates the calls (with small modifications) to Linq to Objects
                        //   the query should just work (nice property, as this might be rather common).
                        projectionProvider = new BasicExpandProvider(this.service.Provider, false, true);
                    }

                    this.queryExpression = Expression.Call(
                        null,
                        DataServiceExecutionProviderMethods.ApplyProjectionsMethodInfo,
                        Expression.Constant(projectionProvider, typeof(object)),
                        this.queryExpression,
                        Expression.Constant(this.rootProjectionNode, typeof(object)));
                }
            }
            else
            {
                // Special case where although there were expand expressions, they were ignored because they did not refer to entity sets
                if (!String.IsNullOrEmpty(this.service.OperationContext.RequestMessage.GetQueryStringItem(XmlConstants.HttpQueryStringExpand)))
                {
                    this.ProjectSkipTokenForNonExpand();
                    this.ProcessOrderBy();
                    this.ProcessSkipAndTop();
                }
            }
        }

        /// <summary>Processes the $filter argument of the request.</summary>
        private void ProcessFilter()
        {
            if (this.description.TargetKind == RequestTargetKind.OpenProperty || this.description.TargetKind == RequestTargetKind.ComplexObject)
            {
                // Silently ignore the $filter, but don't fail because this used to be parsed (but not used) before URI parser integration.
                // Some previously failing cases will now work, but its not worth parsing an expression that will not actually be applied.
                return;
            }

            this.queryExpression = RequestQueryParser.Where(this.service, this.description, this.queryExpression);
        }

        /// <summary>Processes the $skiptoken argument of the request.</summary>
        private void ProcessSkipToken()
        {
            // Obtain skip token from query parameters.
            String skipToken = this.service.OperationContext.RequestMessage.GetQueryStringItem(XmlConstants.HttpQueryStringSkipToken);

            if (this.pagingApplicable)
            {
                if (this.IsCustomPaged)
                {
                    this.ApplyCustomPaging(skipToken);
                }
                else
                {
                    this.ApplyStandardPaging(skipToken);
                }
            }
            else
                if (!String.IsNullOrEmpty(skipToken))
                {
                    throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_SkipTokenNotAllowed);
                }
        }

        /// <summary>Applies standard paging to the query.</summary>
        /// <param name="skipToken">Skip token obtained from query parameters.</param>
        private void ApplyStandardPaging(string skipToken)
        {
            Debug.Assert(!this.IsCustomPaged, "Custom paging should be disabled for this function to be called.");
            if (!String.IsNullOrEmpty(skipToken))
            {
                // Server ignores $skiptoken in query string if preceded by $orderby option and the set doesnt have page size limits.
                // We should not process any $skiptoken query options if the top level entity set being queried doesn't have an entity set page size defined.
                if (!this.IsPageSizeDefined)
                {
                    throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_SkipTokenSupportedOnPagedSets);
                }

                // Parse the skipToken to obtain each positional value
                IList<object> skipTokenValues;
                WebUtil.CheckSyntaxValid(SkipTokenAndETagParser.TryParseNullableTokens(skipToken, out skipTokenValues));

                // Ordering constraint in the presence of skipToken will have the following settings:
                // * First there will be the provided constraints based on $orderby
                // * Followed by all the key columns in the resource type

                // Validate that the skipToken had as many positional values as the number of ordering constraints
                if (this.topLevelOrderingInfo.OrderingExpressions.Count != skipTokenValues.Count)
                {
                    throw DataServiceException.CreateBadRequestError(Strings.DataService_SDP_SkipTokenNotMatchingOrdering(skipTokenValues.Count, skipToken, this.topLevelOrderingInfo.OrderingExpressions.Count));
                }

                // Build the Where clause with the provided skip token
                this.queryExpression = this.queryExpression.QueryableWhere(this.skipTokenExpressionBuilder.BuildSkipTokenFilter(this.topLevelOrderingInfo, skipTokenValues, this.description.TargetResourceType.InstanceType));
            }
        }

        /// <summary>Applies custom paging to the query.</summary>
        /// <param name="skipToken">Skip token obtained from query parameters.</param>
        private void ApplyCustomPaging(string skipToken)
        {
            Debug.Assert(this.IsCustomPaged, "Custom paging should be enabled for this function to be called.");
            if (!String.IsNullOrEmpty(skipToken))
            {
                // Parse the skipToken to obtain each positional value
                IList<object> skipTokenValues;
                WebUtil.CheckSyntaxValid(SkipTokenAndETagParser.TryParseNullableTokens(skipToken, out skipTokenValues));

                object[] convertedValues = new object[skipTokenValues.Count];
                int i = 0;
                foreach (var value in skipTokenValues)
                {
                    convertedValues[i++] = SkipTokenExpressionBuilder.ParseSkipTokenLiteral((string)value);
                }

                this.CheckAndApplyCustomPaging(convertedValues);
            }
            else
            {
                this.CheckAndApplyCustomPaging(null);
            }
        }

        /// <summary>Processes the $orderby argument of the request.</summary>
        private void ProcessOrderBy()
        {
            Debug.Assert(this.topLevelOrderingInfo != null, "Must have valid ordering information in ProcessOrderBy");
            if (this.topLevelOrderingInfo.OrderingExpressions.Count > 0)
            {
                this.queryExpression = RequestQueryParser.OrderBy(this.queryExpression, this.topLevelOrderingInfo);
#if DEBUG
                this.orderApplied = true;
#endif
            }
        }

        /// <summary>Processes the $count argument of the request.</summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining | System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
        private void ProcessCount()
        {
            string count = this.service.OperationContext.RequestMessage.GetQueryStringItem(XmlConstants.HttpQueryStringQueryCount);

            if (!String.IsNullOrEmpty(count))
            {
                // Throw if $count requests have been disabled by the user
                if (!this.service.Configuration.DataServiceBehavior.AcceptCountRequests)
                {
                    throw DataServiceException.CreateBadRequestError(Strings.DataServiceConfiguration_CountNotAccepted);
                }

                count = count.TrimStart();

                // false
                if (count.Equals(XmlConstants.UriCountFalseOption))
                {
                    return;
                }

                // only get
                if (this.service.OperationContext.RequestMessage.HttpVerb.IsChange())
                {
                    throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_RequestVerbCannotCountError);
                }

                // if already counting segment only, then fail
                if (this.description.CountOption == RequestQueryCountOption.CountSegment)
                {
                    throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_QueryCountWithSegmentCount);
                }

                // Only applied to a set
                this.CheckSetQueryApplicable();

                if (count.Equals(XmlConstants.UriCountTrueOption))
                {
                    Expression longCountExpression = this.queryExpression.QueryableLongCount();
                    this.description.CountValue = (long)this.service.ExecutionProvider.Execute(longCountExpression);
                    this.description.CountOption = RequestQueryCountOption.CountQuery;
                }
                else
                {
                    throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_InvalidCountOptionError);
                }
            }
        }

        /// <summary>
        /// Builds the collection of ordering expressions including implicit ordering if paging is required at top level
        /// </summary>
        private void ObtainOrderingExpressions()
        {
            const String Comma = ",";
            const char Space = ' ';
            const String AscendingOrderIdentifier = "asc";
            const string QuestionMark = "?";
            const string EqualMark = "=";
            const string AndMark = "&";
            if (!string.IsNullOrEmpty(this.service.OperationContext.RequestMessage.GetQueryStringItem(XmlConstants.HttpQueryStringOrderBy)))
            {
                this.CheckSetQueryApplicable();
            }

            Debug.Assert(this.topLevelOrderingInfo == null, "Must only be called once per query");

            ResourceType rt = this.description.TargetResourceType;

            this.topLevelOrderingInfo = new OrderingInfo(this.IsStandardPaged);
            OrderByClause orderByClause = new RequestExpressionParser(this.service, this.description).ParseOrderBy();

            // We need to generate ordering expression(when we don't have top level $orderby), if either the result is paged, or we have
            // skip or top count request because in that case, the skip or top has to happen in
            // the expand provider. 
            if (orderByClause == null)
            {
                if (this.IsStandardPaged || this.topCount.HasValue || this.skipCount.HasValue)
                {
                    Uri requestUri = this.service.OperationContext.AbsoluteRequestUri;
                    StringBuilder orderBy = string.IsNullOrEmpty(requestUri.Query) ? new StringBuilder(requestUri.AbsoluteUri + QuestionMark) : new StringBuilder(requestUri.AbsoluteUri + AndMark);
                    orderBy.Append(XmlConstants.HttpQueryStringOrderBy + EqualMark);
                    String separator = String.Empty;
                    foreach (var keyProp in this.description.TargetResourceSet.GetKeyPropertiesForOrderBy())
                    {
                        orderBy.Append(separator).Append(keyProp.Name).Append(Space).Append(AscendingOrderIdentifier);
                        separator = Comma;
                    }

                    var uriParser = RequestUriProcessor.CreateUriParserWithBatchReferenceCallback(this.service, new Uri(orderBy.ToString()));

                    orderByClause = uriParser.ParseOrderBy();
                }
                else
                {
                    return;
                }
            }

            ParameterExpression elementParameter = Expression.Parameter(rt.InstanceType, "element");
            var translator = NodeToExpressionTranslator.Create(this.service, this.description, elementParameter);
            IEnumerable<OrderingExpression> ordering = translator.TranslateOrderBy(orderByClause);

            this.topLevelOrderingInfo.AddRange(ordering);

            if (this.IsStandardPaged)
            {
                this.description.SkipTokenExpressionCount = this.topLevelOrderingInfo.OrderingExpressions.Count;
                this.description.SkipTokenProperties = NeedSkipTokenVisitor.CollectSkipTokenProperties(this.topLevelOrderingInfo, rt);
            }
        }

        /// <summary>
        /// Processes query arguments and returns a request description for 
        /// the resulting query.
        /// </summary>
        /// <returns>A modified <see cref="RequestDescription"/> that includes query information.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining | System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
        private RequestDescription ProcessQuery()
        {
            // Obtains the values of $skip and $top arguments
            this.ObtainSkipTopCounts();

            // Obtain ordering information for the current request
            this.ObtainOrderingExpressions();

            // NOTE: The order set by ProcessOrderBy may be reset by other operations other than skip/top,
            // so filtering needs to occur first.
            this.ProcessFilter();

            // $count ignores SDP, Skip and Top
            this.ProcessCount();

            // $skiptoken is just like a filter, so it should be immediately after $filter
            this.ProcessSkipToken();

            if (!this.description.ExpandAndSelect.HasExpand && !this.description.ExpandAndSelect.HasSelect)
            {
                this.ProjectSkipTokenForNonExpand();
                this.ProcessOrderBy();
                this.ProcessSkipAndTop();
            }
            else if (this.description.CountOption == RequestQueryCountOption.CountSegment)
            {
                // We need to take $top and $skip into account when executing $count requests.
                // The issue is if there is a projection, we push the $skip and $top into the projection node
                // and hence we miss it when $count with $select is specified.
                this.ProcessOrderBy();
                this.ProcessSkipAndTop();
            }

            // NOTE: expand goes last, as it may be reset by other operations.
            this.ProcessExpandAndSelect();
            
            this.GenerateQueryResult();
#if DEBUG
            // We analyze the query here to detect if we have incorrectly inserted
            // calls to OTM for non-open types
            if (this.service.Provider.AreAllResourceTypesNonOpen)
            {
                // Verify that there is no call to OTMs here.
                OpenTypeMethodCallDetector.CheckForOpenTypeMethodCalls(this.queryExpression);
            }
#endif
            return new RequestDescription(this.description, this.queryExpression, this.rootProjectionNode);
        }

        /// <summary>
        /// In case $expand is not provided while the results are still paged, we need to create a wrapper
        /// for the object in order to project the skip tokens corresponding to the result sequence
        /// </summary>
        private void ProjectSkipTokenForNonExpand()
        {
            if (this.IsStandardPaged && this.description.SkipTokenProperties == null)
            {
                Type queryElementType = this.queryExpression.ElementType();

                ParameterExpression expandParameter = Expression.Parameter(queryElementType, "p");

                StringBuilder skipTokenDescription = new StringBuilder();

                Type skipTokenWrapperType = this.GetSkipTokenWrapperTypeAndDescription(skipTokenDescription);

                MemberBinding[] skipTokenBindings = this.GetSkipTokenBindings(skipTokenWrapperType, skipTokenDescription.ToString(), expandParameter);

                Type resultWrapperType = WebUtil.GetWrapperType(new Type[] { queryElementType, skipTokenWrapperType }, null);

                MemberBinding[] resultWrapperBindings = new MemberBinding[3];
                resultWrapperBindings[0] = Expression.Bind(resultWrapperType.GetProperty("ExpandedElement"), expandParameter);
                resultWrapperBindings[1] = Expression.Bind(resultWrapperType.GetProperty("Description"), Expression.Constant(XmlConstants.HttpQueryStringSkipToken));
                resultWrapperBindings[2] = Expression.Bind(
                                                resultWrapperType.GetProperty("ProjectedProperty0"),
                                                Expression.MemberInit(Expression.New(skipTokenWrapperType), skipTokenBindings));

                Expression resultBody = Expression.MemberInit(Expression.New(resultWrapperType), resultWrapperBindings);
                LambdaExpression selector = Expression.Lambda(resultBody, expandParameter);

                Debug.Assert(resultWrapperType == selector.Body.Type, "resultWrapperType == selector.Body.Type");
                this.queryExpression = this.queryExpression.QueryableSelect(selector);

                // Updates the ordering expressions with the ones that take the result wrapper type as parameter 
                // and dereference the ExpandedElement property
                this.UpdateOrderingInfoWithSkipTokenWrapper(resultWrapperType);
            }
        }

        /// <summary>
        /// Obtains the wrapper type for the $skiptoken along with description of properties in the wrapper
        /// </summary>
        /// <param name="skipTokenDescription">Description for the skip token properties</param>
        /// <returns>Type of $skiptoken wrapper</returns>
        private Type GetSkipTokenWrapperTypeAndDescription(StringBuilder skipTokenDescription)
        {
            const String Comma = ",";

            Type[] skipTokenTypes = new Type[this.topLevelOrderingInfo.OrderingExpressions.Count + 1];

            skipTokenTypes[0] = this.queryExpression.ElementType();

            int i = 0;
            String separator = String.Empty;
            foreach (var ordering in this.topLevelOrderingInfo.OrderingExpressions)
            {
                skipTokenTypes[i + 1] = ((LambdaExpression)ordering.Expression).Body.Type;
                skipTokenDescription.Append(separator).Append(XmlConstants.SkipTokenPropertyPrefix + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
                separator = Comma;
                i++;
            }

            return WebUtil.GetWrapperType(skipTokenTypes, Strings.BasicExpandProvider_SDP_UnsupportedOrderingExpressionBreadth);
        }

        /// <summary>
        /// Given the wrapper type and description, returns bindings for the wrapper type for skip token
        /// </summary>
        /// <param name="skipTokenWrapperType">Wrapper type</param>
        /// <param name="skipTokenDescription">Description</param>
        /// <param name="expandParameter">Top level parameter type</param>
        /// <returns>Array of bindings for skip token</returns>
        private MemberBinding[] GetSkipTokenBindings(Type skipTokenWrapperType, String skipTokenDescription, ParameterExpression expandParameter)
        {
            MemberBinding[] skipTokenBindings = new MemberBinding[this.topLevelOrderingInfo.OrderingExpressions.Count + 2];
            skipTokenBindings[0] = Expression.Bind(skipTokenWrapperType.GetProperty("ExpandedElement"), expandParameter);
            skipTokenBindings[1] = Expression.Bind(skipTokenWrapperType.GetProperty("Description"), Expression.Constant(skipTokenDescription.ToString()));

            int i = 0;
            foreach (var ordering in this.topLevelOrderingInfo.OrderingExpressions)
            {
                LambdaExpression sourceLambda = (LambdaExpression)ordering.Expression;
                Expression source = ParameterReplacerVisitor.Replace(sourceLambda.Body, sourceLambda.Parameters[0], expandParameter);
                MemberInfo member = skipTokenWrapperType.GetProperty("ProjectedProperty" + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
                skipTokenBindings[i + 2] = Expression.Bind(member, source);
                i++;
            }

            return skipTokenBindings;
        }

        /// <summary>
        /// Updates the topLevelOrderingInfo member with the new collection of expressions that 
        /// dereference the ExpandedElement property on the top level wrapper object
        /// </summary>
        /// <param name="resultWrapperType">Type of top level wrapper object</param>
        private void UpdateOrderingInfoWithSkipTokenWrapper(Type resultWrapperType)
        {
            OrderingInfo newOrderingInfo = new OrderingInfo(true);

            ParameterExpression wrapperParameter = Expression.Parameter(resultWrapperType, "w");

            foreach (var ordering in this.topLevelOrderingInfo.OrderingExpressions)
            {
                LambdaExpression oldExpression = (LambdaExpression)ordering.Expression;
                Expression newOrdering = ParameterReplacerVisitor.Replace(
                                            oldExpression.Body,
                                            oldExpression.Parameters[0],
                                            Expression.MakeMemberAccess(wrapperParameter, resultWrapperType.GetProperty("ExpandedElement")));

                newOrderingInfo.Add(new OrderingExpression(Expression.Lambda(newOrdering, wrapperParameter), ordering.IsAscending));
            }

            this.topLevelOrderingInfo = newOrderingInfo;
        }

        /// <summary>Processes the $skip and/or $top argument of the request by composing query with Skip and/or Take methods.</summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining | System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
        private void ProcessSkipAndTop()
        {
            Debug.Assert(this.queryExpression != null, "this.query != null");
            if (this.skipCount.HasValue)
            {
#if DEBUG
                Debug.Assert(this.orderApplied, "Ordering must have already been applied.");
#endif
                this.queryExpression = this.queryExpression.QueryableSkip(this.skipCount.Value);
                Debug.Assert(this.queryExpression != null, "this.queryExpression != null");
            }

            if (this.topCount.HasValue)
            {
#if DEBUG
                Debug.Assert(this.orderApplied, "Ordering must have already been applied.");
#endif
                this.queryExpression = this.queryExpression.QueryableTake(this.topCount.Value);
                Debug.Assert(this.queryExpression != null, "this.queryExpression != null");
            }
        }

        /// <summary>
        /// Finds out the appropriate value for skip and top parameters for the current request
        /// </summary>
        private void ObtainSkipTopCounts()
        {
            int count;

            if (this.ReadSkipOrTopArgument(XmlConstants.HttpQueryStringSkip, out count))
            {
                this.skipCount = count;
            }

            int pageSize = 0;
            if (this.IsStandardPaged)
            {
                pageSize = this.description.LastSegmentInfo.TargetResourceSet.PageSize;
            }

            if (this.ReadSkipOrTopArgument(XmlConstants.HttpQueryStringTop, out count))
            {
                this.topCount = count;
                if (this.IsStandardPaged && pageSize < this.topCount.Value)
                {
                    this.topCount = pageSize;
                }
            }
            else if (this.IsStandardPaged)
            {
                this.topCount = pageSize;
            }

            if (this.topCount != null || this.skipCount != null)
            {
                this.CheckSetQueryApplicable();
            }
        }

        /// <summary>
        /// Checks whether the specified argument should be processed and what 
        /// its value is.
        /// </summary>
        /// <param name="queryItem">Name of the query item, $top or $skip.</param>
        /// <param name="count">The value for the query item.</param>
        /// <returns>true if the argument should be processed; false otherwise.</returns>
        private bool ReadSkipOrTopArgument(string queryItem, out int count)
        {
            Debug.Assert(queryItem != null, "queryItem != null");

            string itemText = this.service.OperationContext.RequestMessage.GetQueryStringItem(queryItem);
            if (String.IsNullOrEmpty(itemText))
            {
                count = 0;
                return false;
            }

            if (!Int32.TryParse(itemText, NumberStyles.Integer, CultureInfo.InvariantCulture, out count))
            {
                throw DataServiceException.CreateSyntaxError(
                    Strings.RequestQueryProcessor_IncorrectArgumentFormat(queryItem, itemText));
            }

            return true;
        }

        /// <summary>Checks if custom paging is already applied, if not, applies it and raises response version.</summary>
        /// <param name="skipTokenValues">Values of skip tokens.</param>
        private void CheckAndApplyCustomPaging(object[] skipTokenValues)
        {
            if (!this.appliedCustomPaging)
            {
                // Call DataServiceProviderMethods.SetContinuationToken(), which in turn calls IDataServicePagingProvider.SetContinuationToken().
                // IDataServicePagingProvider.SetContinuationToken() returns void, if we call that here we would need to use the Block expression.
                // DataServiceProviderMethods.SetContinuationToken() on the other hand returns IQueryable, which simplifies the expression tree
                // we build here.
                MethodInfo setContinuationTokenMethodInfo = DataServiceExecutionProviderMethods.SetContinuationTokenMethodInfo.MakeGenericMethod(this.queryExpression.ElementType());
                this.queryExpression = Expression.Call(
                    null,
                    setContinuationTokenMethodInfo,
                    Expression.Constant(this.service.PagingProvider.PagingProviderInterface, typeof(IDataServicePagingProvider)),
                    this.queryExpression,
                    Expression.Constant(this.description.LastSegmentInfo.TargetResourceType),
                    Expression.Constant(skipTokenValues, typeof(Object[])));

                this.appliedCustomPaging = true;
            }
        }
    }
}
