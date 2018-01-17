//---------------------------------------------------------------------
// <copyright file="ExpandedProjectionNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.OData.UriParser;
    #endregion

    /// <summary>This class represents an expanded navigation property in the tree
    /// of projected properties. It is also used to represent the root of the projection tree.</summary>
    [DebuggerDisplay("ExpandedProjectionNode {PropertyName}")]
    internal class ExpandedProjectionNode : ProjectionNode
    {
        #region Private fields
        /// <summary>The resource set to which the expansion leads.</summary>
        /// <remarks>If this node represents expanded navigation property, this is the resource set
        /// to which the expanded navigation property points to.
        /// If this node is the root node of the projection tree, this is the resource set
        /// for the root of the query results.</remarks>
        private readonly ResourceSetWrapper resourceSetWrapper;

        /// <summary>Collection of information which describes the ordering for the results
        /// returned by this expanded property.</summary>
        /// <remarks>This can be null in which case no ordering is to be applied.</remarks>
        private readonly OrderingInfo orderingInfo;

        /// <summary>The filter expression to be applied to results returned by this expanded property.</summary>
        /// <remarks>This can be null in which case no filter is to be applied.</remarks>
        private readonly Expression filter;

        /// <summary>Number of results to skip for this node.</summary>
        /// <remarks>null value means that no skip should be applied.</remarks>
        private readonly int? skipCount;

        /// <summary>Maximum number of results to return for this node.</summary>
        /// <remarks>null value means that all results should be returned.</remarks>
        private readonly int? takeCount;

        /// <summary>Maximum number of results allowed for this node. Provider should use this only as a hint.
        /// It should return no less then maxResultsExpected + 1 results (assuming that number is available)
        /// so that the service can detect violation of the limit.</summary>
        /// <remarks>null value means that no limit will be applied and thus all results available should be returned.</remarks>
        private readonly int? maxResultsExpected;

        /// <summary>The select expand clause for the current node from the URI Parser.</summary>
        private readonly SelectExpandClause selectExpandClause;

        /// <summary>List of child nodes.</summary>
        private List<ProjectionNode> nodes;

        /// <summary>Internal field which is set to true once we have seen a projection including this expanded
        /// property. Otherwise set to false.</summary>
        /// <remarks>This field is used to eliminate expanded nodes which are not projected and thus there
        /// would be no point in expanding them.</remarks>
        private bool projectionFound;

        /// <summary>Flag which specifies if all child properties of this node should be projected.</summary>
        private bool projectAllImmediateProperties;

        /// <summary>Flag which specifies if all bindable operations of this node should be projected.</summary>
        private bool projectAllImmediateOperations;

        /// <summary>Flag which specified is the entire expanded subtree of this node should be projected.</summary>
        private bool projectSubtree;

        /// <summary>Whether this node has one or more children which refers to derived properties.</summary>
        private bool hasExpandedPropertyOnDerivedType;

        /// <summary>Cache for the set of operations that have been selected.</summary>
        private SelectedOperationsCache selectedOperations;

        #endregion

        #region Constructors
        /// <summary>Creates new instance of node representing expanded navigation property.</summary>
        /// <param name="propertyName">The name of the property to project and expand.</param>
        /// <param name="property">The <see cref="ResourceProperty"/> for this property. Can only be null for the root node.</param>
        /// <param name="targetResourceType">Target resource type on which the expansion needs to happen.</param>
        /// <param name="resourceSetWrapper">The resource set to which the expansion leads.</param>
        /// <param name="orderingInfo">The ordering info for this node. null means no ordering to be applied.</param>
        /// <param name="filter">The filter for this node. null means no filter to be applied.</param>
        /// <param name="skipCount">Number of results to skip. null means no results to be skipped.</param>
        /// <param name="takeCount">Maximum number of results to return. null means return all available results.</param>
        /// <param name="maxResultsExpected">Maximum number of expected results. Hint that the provider should return
        /// at least maxResultsExpected + 1 results (if available).</param>
        /// <param name="selectExpandClause">The select expand clause for the current node from the URI Parser.</param>
        internal ExpandedProjectionNode(
            string propertyName,
            ResourceProperty property,
            ResourceType targetResourceType,
            ResourceSetWrapper resourceSetWrapper,
            OrderingInfo orderingInfo,
            Expression filter,
            int? skipCount,
            int? takeCount,
            int? maxResultsExpected,
            SelectExpandClause selectExpandClause)
            : base(propertyName, property, targetResourceType)
        {
            Debug.Assert(resourceSetWrapper != null, "resourceSetWrapper != null");
            Debug.Assert(property != null || (propertyName.Length == 0 && targetResourceType == null), "We don't support open navigation properties.");
            Debug.Assert(property == null || targetResourceType.TryResolvePropertyName(property.Name) != null, "Property must exist on the target resource type");
            Debug.Assert(selectExpandClause != null, "selectExpandClause != null");

            this.resourceSetWrapper = resourceSetWrapper;
            this.orderingInfo = orderingInfo;
            this.filter = filter;
            this.skipCount = skipCount;
            this.takeCount = takeCount;
            this.maxResultsExpected = maxResultsExpected;
            this.nodes = new List<ProjectionNode>();
            this.hasExpandedPropertyOnDerivedType = false;
            this.selectExpandClause = selectExpandClause;
        }
        #endregion

        #region Public properties
        /// <summary>Collection of information which describes the ordering for the results
        /// returned by this expanded property.</summary>
        /// <remarks>This can be null in which case no ordering is to be applied.</remarks>
        public OrderingInfo OrderingInfo
        {
            get
            {
                return this.orderingInfo;
            }
        }

        /// <summary>The filter expression to be applied to results returned by this expanded property.</summary>
        /// <remarks>This can be null in which case no filter is to be applied.</remarks>
        public Expression Filter
        {
            get
            {
                return this.filter;
            }
        }

        /// <summary>Number of results to skip for this node.</summary>
        /// <remarks>null value means that no skip should be applied.</remarks>
        public int? SkipCount
        {
            get
            {
                return this.skipCount;
            }
        }

        /// <summary>Maximum number of results to return for this node.</summary>
        /// <remarks>null value means that all results should be returned.</remarks>
        public int? TakeCount
        {
            get
            {
                return this.takeCount;
            }
        }

        /// <summary>Maximum number of results allowed for this node. Provider should use this only as a hint.
        /// It should return no less then MaxResultsExpected + 1 results (assuming that number is available)
        /// so that the service can detect violation of the limit.</summary>
        /// <remarks>null value means that no limit will be applied and thus all results available should be returned.</remarks>
        public int? MaxResultsExpected
        {
            get
            {
                return this.maxResultsExpected;
            }
        }

        /// <summary>List of child nodes.</summary>
        public IEnumerable<ProjectionNode> Nodes
        {
            get
            {
                return this.nodes;
            }
        }

        /// <summary>Set to true if all operations of this node should be made part of the results.</summary>
        public bool ProjectAllOperations
        {
            get
            {
                return this.projectSubtree || this.ProjectAllImmediateOperations;
            }
        }

        /// <summary>Set to true if all properties of this node should be made part of the results.</summary>
        public bool ProjectAllProperties
        {
            get
            {
                return this.projectSubtree || this.ProjectAllImmediateProperties;
            }
        }
        #endregion

        #region Internal properties
        /// <summary>The resource set to which the expansion leads.</summary>
        /// <remarks>If this node represents expanded navigation property, this is the resource set
        /// to which the expanded navigation property points to.
        /// If this node is the root node of the projection tree, this is the resource set
        /// for the root of the query results.
        /// This property is for internal use by components of the WCF Data Services
        /// to avoid unnecessary lookups of the wrapper from the ResourceSet property.</remarks>
        internal ResourceSetWrapper ResourceSetWrapper
        {
            get
            {
                return this.resourceSetWrapper;
            }
        }

        /// <summary>The resource type in which all the entities expanded by this segment will be of.</summary>
        /// <remarks>This is usually the resource type of the <see cref="ResourceSetWrapper"/> for this node,
        /// but it can also be a derived type of that resource type.
        /// This can happen if navigation property points to a resource set but uses a derived type.
        /// It can also happen if service operation returns entities from a given resource set
        /// but it returns derived types.</remarks>
        internal virtual ResourceType ResourceType
        {
            get
            {
                Debug.Assert(this.Property != null, "Derived class should override this if property can be null.");
                return this.Property.ResourceType;
            }
        }

        /// <summary>Internal property which is set to true once we have seen a projection including this expanded
        /// property. Otherwise set to false.</summary>
        /// <remarks>This property is used to eliminate expanded nodes which are not projected and thus there
        /// would be no point in expanding them.</remarks>
        internal bool ProjectionFound
        {
            get
            {
                return this.projectionFound;
            }

            set
            {
                this.projectionFound = value;
            }
        }

        /// <summary>Flag which specifies if all child properties of this node should be projected.</summary>
        internal bool ProjectAllImmediateProperties
        {
            get
            {
                return this.projectAllImmediateProperties;
            }

            set
            {
                Debug.Assert(!value || this.projectionFound, "Marking node to include all child properties requires the node to be projected.");
                this.projectAllImmediateProperties = value;
            }
        }

        /// <summary>Flag which specifies if all bindable operations of this node should be projected.</summary>
        internal bool ProjectAllImmediateOperations
        {
            get
            {
                return this.projectAllImmediateOperations;
            }

            set
            {
                Debug.Assert(!value || this.projectionFound, "Marking node to include all bindable operations requires the node to be projected.");
                this.projectAllImmediateOperations = value;
            }
        }

        /// <summary>Whether this expanded node has a filter or a constraint on max results returns.</summary>
        internal bool HasFilterOrMaxResults
        {
            get 
            { 
                return this.Filter != null || this.MaxResultsExpected.HasValue; 
            }
        }

        /// <summary>Whether this node has one or more children which refers to derived properties.</summary>
        internal bool HasExpandedPropertyOnDerivedType
        {
            get { return this.hasExpandedPropertyOnDerivedType; }
        }

        /// <summary>
        /// The cache for tracking which operations have been selected.
        /// </summary>
        internal SelectedOperationsCache SelectedOperations
        {
            get { return this.selectedOperations ?? (this.selectedOperations = new SelectedOperationsCache()); }
        }

        /// <summary>
        /// The select expand clause for the current node from the URI Parser.
        /// </summary>
        internal SelectExpandClause SelectExpandClause
        {
            get { return this.selectExpandClause; }
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Adds a new expanded node with the given segment name, if required.
        /// </summary>
        /// <param name="segment">Expand segment as specified in the uri.</param>
        /// <param name="childSelectExpandClause">The select expand clause for the current node from the URI Parser.</param>
        /// <returns>An instance of ExpandedProjectionNode - either new or existing one.</returns>
        internal ExpandedProjectionNode AddExpandedNode(ExpandSegment segment, SelectExpandClause childSelectExpandClause)
        {
            Debug.Assert(segment.ExpandedProperty != null, "we do not allow expands on open properties");
            Debug.Assert(childSelectExpandClause != null, "childSelectExpandClause != null");

            ExpandedProjectionNode childNode = (ExpandedProjectionNode)this.FindNode(segment.Name);

            // Currently, since we do not have a back pointer in ResourceProperty for the declaring type,
            // the same resource property instance can belong to 2 completely different types.
            // Hence doing reference equality on Resource Property instance does not gaurantee that they
            // refer to the same property. But if the references are not equal, they are gauranteed to be
            // different. The check of reference equality is to make sure we do the expensive IsAssignableFrom
            // check, only if the instances are the same, and if the IsAssignable returns true, then we are
            // gauranteed that the property refers to the same property.
            if (childNode != null && childNode.Property == segment.ExpandedProperty)
            {
                // If the given segment specifies the same property on a super type (or an ancestor),
                // then we need to change the target type on the segment to the super type (or ancestor),
                // otherwise we should ignore the segment.
                // Since IsAssignableFrom returns true, if the instances are the same, we will update the
                // target resource type to the same instance. It doesn't matter, but having another check
                // for reference equality makes code less readable.
                if (segment.TargetResourceType.IsAssignableFrom(childNode.TargetResourceType))
                {
                    childNode.TargetResourceType = segment.TargetResourceType;
                }
            }
            else
            {
                childNode = new ExpandedProjectionNode(
                    segment.Name,
                    segment.ExpandedProperty,
                    segment.TargetResourceType,
                    segment.Container,
                    segment.OrderingInfo,
                    segment.Filter,
                    null,
                    segment.Container.PageSize != 0 ? (int?)segment.Container.PageSize : null,
                    segment.MaxResultsExpected != Int32.MaxValue ? (int?)segment.MaxResultsExpected : null,
                    childSelectExpandClause);
                this.AddNode(childNode);
            }

            return childNode;
        }

        /// <summary>
        /// Add the projection node with the given property name, property instance and target resource type, if required.
        /// </summary>
        /// <param name="propertyName">Name of the property that needs to be projected.</param>
        /// <param name="property">ResourceProperty instance containing information about the property - this will be null for open properties.</param>
        /// <param name="targetResourceType">ResourceType instance on which the property needs to be expanded.</param>
        /// <returns>A new or an existing instance of ProjectionNode.</returns>
        internal ExpandedProjectionNode AddProjectionNode(string propertyName, ResourceProperty property, ResourceType targetResourceType)
        {
            // find out if there exists another node with the given property name.
            ProjectionNode node = this.FindNode(propertyName);
            if (node == null || !ExpandedProjectionNode.ApplyPropertyToExistingNode(node, property, targetResourceType))
            {
                // just create a simple projection node for it
                node = new ProjectionNode(propertyName, property, targetResourceType);
                this.AddNode(node);
            }

            return node as ExpandedProjectionNode;
        }

        /// <summary>Find a child node of a given property.</summary>
        /// <param name="property">ResourceProperty instance to find the child for.</param>
        /// <param name="targetResourceType">ResourceType instance on which the expansion needs to happen.</param>
        /// <returns>The child node if there's one for the specified <paramref name="property"/> or null
        /// if no such child was found.</returns>
        internal ExpandedProjectionNode FindExpandedNode(ResourceProperty property, ResourceType targetResourceType)
        {
            Debug.Assert(property != null, "property != null");
            Debug.Assert(this.nodes.Count(node => node.Property == property && node.TargetResourceType.IsAssignableFrom(targetResourceType)) <= 1, "there must be only one property node with the given name and target resource type");

            return this.nodes.FirstOrDefault(
                node => node.Property == property && node.TargetResourceType.IsAssignableFrom(targetResourceType)) as ExpandedProjectionNode;
        }

        /// <summary>Walks the subtree of this node and removes all nodes which were not marked projected.</summary>
        /// <remarks>Used to remove unnecessary expanded nodes.</remarks>
        internal void RemoveNonProjectedNodes()
        {
            for (int j = this.nodes.Count - 1; j >= 0; j--)
            {
                ExpandedProjectionNode expandedNode = this.nodes[j] as ExpandedProjectionNode;

                // Leave non-expanded properties there as they specify projections.
                if (expandedNode == null)
                {
                    continue;
                }

                // If we are to project entire subtree, leave all expanded nodes in as well
                // otherwise remove the expanded nodes which are not marked as projected.
                if (!this.projectSubtree && !expandedNode.ProjectionFound)
                {
                    // This removes the expandedNode from the tree (and all its children)
                    this.nodes.RemoveAt(j);
                }
                else
                {
                    expandedNode.RemoveNonProjectedNodes();
                }
            }
        }

        /// <summary>Removes duplicates from the tree caused by wildcards and sorts the projected properties.</summary>
        /// <param name="provider">underlying provider instance.</param>
        /// <remarks>
        /// Examples
        /// $select=Orders, Orders/ID           - get rid of the Orders/ID
        /// $select=Orders, Orders/*            - get rid of the Orders/*
        /// $select=Orders/*, Orders/ID         - get rid of the Orders/ID
        /// $select=Orders/*, Orders/OrderItems&amp;$expand=Orders - get rid of the Orders/OrderItems (it's redundant to *)
        /// $select=Orders/*, Orders/OrderItems&amp;$expand=Orders/OrderItems - leave as is, the Orders/OrderItems are expanded
        /// 
        /// The sorting order is the same as the order in which the properties are enumerated on the owning type.
        /// This is to preserve the same order as if no projections occured.
        /// </remarks>
        internal void ApplyWildcardsAndSort(DataServiceProviderWrapper provider)
        {
            // If this segment was marked to include entire subtree
            // simply remove all children which are not expanded
            // and propagate the information to all expanded children.
            if (this.projectSubtree)
            {
                for (int j = this.nodes.Count - 1; j >= 0; j--)
                {
                    ExpandedProjectionNode expandedNode = this.nodes[j] as ExpandedProjectionNode;
                    if (expandedNode != null)
                    {
                        expandedNode.projectSubtree = true;
                        expandedNode.ApplyWildcardsAndSort(provider);
                    }
                }

                this.projectAllImmediateProperties = false;
                this.projectAllImmediateOperations = false;
                return;
            }

            for (int j = this.nodes.Count - 1; j >= 0; j--)
            {
                ExpandedProjectionNode expandedNode = this.nodes[j] as ExpandedProjectionNode;

                // If this node was marked to include all immediate properties, 
                //   remove all children which are not expanded.
                //   That means they are either simple properties or nav. properties which
                //   are not going to be expanded anyway.
                if (this.ProjectAllImmediateProperties && expandedNode == null)
                {
                    this.nodes.RemoveAt(j);
                }
                else if (expandedNode != null)
                {
                    expandedNode.ApplyWildcardsAndSort(provider);
                }
            }

            if (this.nodes.Count > 0)
            {
                // Sort the subsegments such that they have the same order as the properties
                //   on the owning resource type.

                // build the list of existing resource types that this query touches
                List<ResourceType> resourceTypes = new List<ResourceType>();
                resourceTypes.Add(this.ResourceType);

                // If we have one or more derived properties to expand or project,
                // we need to sort it based on the order in which the derived types
                // are return.
                List<ProjectionNode> derivedProjectionNodes = this.nodes.Where(n => !ResourceType.CompareReferences(n.TargetResourceType, this.ResourceType)).ToList();
                if (derivedProjectionNodes.Count > 0)
                {
                    foreach (ResourceType rt in provider.GetDerivedTypes(this.ResourceType))
                    {
                        if (derivedProjectionNodes.FirstOrDefault(node => node.TargetResourceType == rt) != null)
                        {
                            resourceTypes.Add(rt);
                        }
                    }
                }

#if DEBUG
                int count = this.nodes.Count;
#endif
                this.nodes = ExpandedProjectionNode.SortNodes(this.nodes, resourceTypes);
#if DEBUG
                Debug.Assert(this.nodes.Count == count, "We didn't sort all the properties.");
#endif
            }
        }

        /// <summary>Marks the entire subtree as projected.</summary>
        /// <remarks>This is used when there were no projections specified in the query
        /// to mark the entire tree as projected.</remarks>
        internal void MarkSubtreeAsProjected()
        {
            this.projectSubtree = true;
            this.projectAllImmediateProperties = false;

            foreach (ProjectionNode node in this.nodes)
            {
                ExpandedProjectionNode expandedNode = node as ExpandedProjectionNode;
                if (expandedNode != null)
                {
                    expandedNode.MarkSubtreeAsProjected();
                }
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Whether the given property can be applied to the existing node.
        /// </summary>
        /// <param name="existingNode">Existing node with the same property name.</param>
        /// <param name="property">ResourceProperty instance for the property refered by the new segment.</param>
        /// <param name="targetResourceType">TargetResourceType for the new segment.</param>
        /// <returns>true if the given property can be applied to the existing node, otherwise returns false.</returns>
        /// <remarks>In case this function returns true, it might modify the TargetResourceType property of the existing node.</remarks>
        private static bool ApplyPropertyToExistingNode(ProjectionNode existingNode, ResourceProperty property, ResourceType targetResourceType)
        {
            Debug.Assert(existingNode != null, "existingNode != null");
            Debug.Assert(property == null || property.Name == existingNode.PropertyName, "property == null || property.Name == existingNode.PropertyName");

            // This is just quicker way of determining if declared property belongs to different sub-trees.
            // Since any given property can be declared once in a chain of derived types, if the property instances
            // are not the same, we can assume that they do not belong to the same hierarchy chain.
            // Even if we do not have this check, everything will just work, since we check the type assignability.
            if (property != null && existingNode.Property != null && property != existingNode.Property)
            {
                return false;
            }

            ExpandedProjectionNode expansionNode = existingNode as ExpandedProjectionNode;

            // If the target resource type of the new segment is a super type, we might 
            // need to update the target resource type of the existing node.
            // Since IsAssignableFrom returns true when they are equal, it isn't require
            // to do anything, but if we do, it should still work.
            if (targetResourceType.IsAssignableFrom(existingNode.TargetResourceType))
            {
                ExpandedProjectionNode.VerifyPropertyMismatchAndExpandSelectMismatchScenario(
                    existingNode,
                    property,
                    targetResourceType,
                    expansionNode != null);

                existingNode.TargetResourceType = targetResourceType;
                return true;
            }

            // If the existing node is already a super type, then there is nothing to do
            // other than validating the error scenarios.
            if (existingNode.TargetResourceType.IsAssignableFrom(targetResourceType))
            {
                ExpandedProjectionNode.VerifyPropertyMismatchAndExpandSelectMismatchScenario(
                    existingNode,
                    property,
                    targetResourceType,
                    expansionNode != null);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Verify that the property referred by the existing node and the new segment are both open properties or declared properties
        /// and if the existing node is an expand node, make sure that the target resource types are the same.
        /// </summary>
        /// <param name="existingNode">Existing node with the same property name.</param>
        /// <param name="property">ResourceProperty instance for the property refered by the new segment.</param>
        /// <param name="targetResourceType">TargetResourceType for the new segment.</param>
        /// <param name="expandNode">true if the existingNode is an expand node.</param>
        private static void VerifyPropertyMismatchAndExpandSelectMismatchScenario(ProjectionNode existingNode, ResourceProperty property, ResourceType targetResourceType, bool expandNode)
        {
            Debug.Assert(
                existingNode.TargetResourceType.IsAssignableFrom(targetResourceType) ||
                targetResourceType.IsAssignableFrom(existingNode.TargetResourceType),
                "This method must be called if the existingNode and targetResourceType are in the same inheritance chain");
            Debug.Assert(!expandNode || existingNode is ExpandedProjectionNode, "If expandNode is true, then the existingNode must be an ExpandedProjectionNode");

            if (property != existingNode.Property)
            {
                // If the property are not the same - it means one of them must be null.
                // This is only possible if super type is open and the property resolves to an open property.
                Debug.Assert(property == null || existingNode.Property == null, "One of the properties must be null, since the types belong to the same inheritance chain, and cannot have different property instance from a given property name");

                // Currently we do not support scenarios where one refers to the open property on the supertype
                // and declared property on the sub type.
                throw DataServiceException.CreateBadRequestError(
                    Strings.RequestQueryProcessor_CannotSpecifyOpenPropertyAndDeclaredPropertyAtTheSameTime(
                        existingNode.PropertyName,
                        property == null ? targetResourceType.FullName : existingNode.TargetResourceType.FullName,
                        property == null ? existingNode.TargetResourceType.FullName : targetResourceType.FullName));
            }
            
            if (!ResourceType.CompareReferences(targetResourceType, existingNode.TargetResourceType) && expandNode)
            {
                // If expand and select are specified on the same property within a given type
                // hierarchy, currently we enforce that they must be specified on the same type
                throw DataServiceException.CreateBadRequestError(
                    Strings.RequestQueryProcessor_SelectAndExpandCannotBeSpecifiedTogether(existingNode.PropertyName));
            }
        }

        /// <summary>
        /// Sort the list of existing projection nodes in the metadata order, first based on the type followed by the property order.
        /// </summary>
        /// <param name="existingNodes">existing projection nodes.</param>
        /// <param name="resourceTypesInMetadataOrder">list of resource types in metadata order.</param>
        /// <returns>the projection nodes in sorted manner.</returns>
        private static List<ProjectionNode> SortNodes(List<ProjectionNode> existingNodes, List<ResourceType> resourceTypesInMetadataOrder)
        {
            List<ProjectionNode> sortedNodes = new List<ProjectionNode>(existingNodes.Count);
            foreach (ResourceType resourceType in resourceTypesInMetadataOrder)
            {
                foreach (ResourceProperty property in resourceType.Properties)
                {
                    // Since the same instance of resource property can be shared by 2 different resource types,
                    // just comparing the resource property instances is not good enough. We also need to compare
                    // the type references along with the property references.
                    Debug.Assert(
                        existingNodes.Where(node => node.Property == property && node.TargetResourceType == resourceType).Count() <= 1,
                        "Can't have more than one projection segment for a given property.");

                    ProjectionNode projectionNode = existingNodes.FirstOrDefault(
                        node => node.Property == property && node.TargetResourceType == resourceType);
                    if (projectionNode != null)
                    {
                        sortedNodes.Add(projectionNode);

                        // We need to remove the node from the list, otherwise when we are going through more derived types,
                        // we might encounter the same property again, since we are walking through all the properties on
                        // a given resource type.
                        existingNodes.Remove(projectionNode);
                    }
                }
            }

            // And then append any open properties sorted alphabetically
            // We sort these since we don't want client to be able to influence
            //   the server behavior unless aboslutely necessary.
            List<ProjectionNode> openPropertyProjectionNodes =
                existingNodes.Where(node => node.Property == null).ToList();
            openPropertyProjectionNodes.Sort(new Comparison<ProjectionNode>((x, y) =>
            {
                return String.Compare(x.PropertyName, y.PropertyName, StringComparison.Ordinal);
            }));
            sortedNodes.AddRange(openPropertyProjectionNodes);
            return sortedNodes;
        }

        /// <summary>Find a child node of a given property.</summary>
        /// <param name="propertyName">The name of the property to find the child for.</param>
        /// <returns>The child node if there's one for the specified <paramref name="propertyName"/> or null
        /// if no such child was found.</returns>
        private ProjectionNode FindNode(string propertyName)
        {
            Debug.Assert(propertyName != null, "propertyName != null");
            Debug.Assert(this.nodes.Count(p => p.PropertyName == propertyName) <= 1, "there must be only one property node with the given name");
            return this.nodes.FirstOrDefault(
                projectionNode => String.Equals(projectionNode.PropertyName, propertyName, StringComparison.Ordinal));
        }

        /// <summary>Adds a new child node to this node.</summary>
        /// <param name="node">The child node to add.</param>
        private void AddNode(ProjectionNode node)
        {
            Debug.Assert(node != null, "node != null");
            Debug.Assert(
                this.nodes.Count(
                    n => n.PropertyName == node.PropertyName &&
                    (n.TargetResourceType.IsAssignableFrom(node.TargetResourceType) || node.TargetResourceType.IsAssignableFrom(n.TargetResourceType))) == 0,
                "make sure there is no node with the same property name in the inheritance hierarchy");

            this.nodes.Add(node);

            ExpandedProjectionNode expandedNode = node as ExpandedProjectionNode;
            if (expandedNode != null)
            {
                this.hasExpandedPropertyOnDerivedType = this.hasExpandedPropertyOnDerivedType || (!ResourceType.CompareReferences(this.ResourceType, node.TargetResourceType));
            }
        }

        #endregion
    }
}
