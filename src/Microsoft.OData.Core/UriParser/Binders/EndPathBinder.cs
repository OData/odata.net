﻿//---------------------------------------------------------------------
// <copyright file="EndPathBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Core;

    /// <summary>
    /// Class that knows how to bind an end path token, which could be several things.
    /// </summary>
    internal sealed class EndPathBinder : BinderBase
    {
        /// <summary>
        /// The function call binder to use to bind this end path to a function if necessary.
        /// </summary>
        private readonly FunctionCallBinder functionCallBinder;

        /// <summary>
        /// Constructs a EndPathBinder object using the given function to bind parent token.
        /// </summary>
        /// <param name="bindMethod">Method to bind the EndPathToken's parent, if there is one.</param>
        /// <param name="state">State of the metadata binding.</param>am>
        /// <remarks>
        /// Make bindMethod of return type SingleValueQueryNode.
        /// </remarks>
        internal EndPathBinder(MetadataBinder.QueryTokenVisitor bindMethod, BindingState state)
            : base(bindMethod, state)
        {
            this.functionCallBinder = new FunctionCallBinder(bindMethod, state);
        }

        /// <summary>
        /// Generates a bound query node representing an <see cref="IEdmProperty"/> given an already semantically bound parent node.
        /// </summary>
        /// <param name="parentNode">The semantically bound source node of this end path token</param>
        /// <param name="property">The <see cref="IEdmProperty"/> that will be bound to this node. Must not be primitive collection</param>
        /// <param name="state">The state of binding.</param>
        /// <returns>QueryNode bound to this property.</returns>
        internal static QueryNode GeneratePropertyAccessQueryNode(SingleResourceNode parentNode, IEdmProperty property, BindingState state)
        {
            ExceptionUtils.CheckArgumentNotNull(parentNode, "parentNode");
            ExceptionUtils.CheckArgumentNotNull(property, "property");

            // TODO: Remove this check.
            // We should verify that the top level of an expression is a bool rather than arbitrarily restrict property types.
            // We can get here if there is a query like $filter=MyCollectionProperty eq 'foo' or something.
            // If it was $filter=MyCollectionProperty/any(...) then we would have gone down the 'NonRootSegment' code path instead of this one
            if (property.Type.IsNonEntityCollectionType())
            {
                // if this happens to be a top level node (i.e. $filter=MyCollection), then it will fail further up the chain, so
                // don't need to worry about checking for that here.
                if (property.Type.IsStructuredCollectionType())
                {
                    return new CollectionComplexNode(parentNode, property);
                }
                else
                {
                    return new CollectionPropertyAccessNode(parentNode, property);
                }
            }

            if (property.PropertyKind == EdmPropertyKind.Navigation)
            {
                // These are error cases in practice, but we let ourselves throw later for better context-sensitive error messages
                IEdmNavigationProperty edmNavigationProperty = (IEdmNavigationProperty)property;
                if (edmNavigationProperty.TargetMultiplicity() == EdmMultiplicity.Many)
                {
                    return new CollectionNavigationNode(parentNode, edmNavigationProperty, state.ParsedSegments);
                }

                return new SingleNavigationNode(parentNode, edmNavigationProperty, state.ParsedSegments);
            }

            if (property.Type.IsComplex())
            {
                return new SingleComplexNode(parentNode, property);
            }

            return new SingleValuePropertyAccessNode(parentNode, property);
        }

        /// <summary>
        /// Constructs parent node from binding state
        /// </summary>
        /// <param name="state">Current binding state</param>
        /// <returns>The parent node.</returns>
        internal static SingleValueNode CreateParentFromImplicitRangeVariable(BindingState state)
        {
            ExceptionUtils.CheckArgumentNotNull(state, "state");

            // If the Parent is null, then it must be referring to the implicit $it parameter
            if (state.ImplicitRangeVariable == null)
            {
                throw new ODataException(SRResources.MetadataBinder_PropertyAccessWithoutParentParameter);
            }

            return NodeFactory.CreateRangeVariableReferenceNode(state.ImplicitRangeVariable);
        }

        /// <summary>
        /// This method generates a <see cref="SingleValueOpenPropertyAccessNode"/> for properties of open type
        /// </summary>
        /// <param name="endPathToken">EndPathToken to bind into an open property node.</param>
        /// <param name="parentNode">Parent node of this open property</param>
        /// <returns>Will return a <see cref="SingleValueOpenPropertyAccessNode"/> when open types are supported</returns>
        internal SingleValueOpenPropertyAccessNode GeneratePropertyAccessQueryForOpenType(EndPathToken endPathToken, SingleValueNode parentNode)
        {
            if (parentNode.TypeReference == null ||
                parentNode.TypeReference.Definition.IsOpen() ||
                IsAggregatedProperty(endPathToken))
            {
                return new SingleValueOpenPropertyAccessNode(parentNode, endPathToken.Identifier);
            }
            else
            {
                throw new ODataException(Error.Format(SRResources.MetadataBinder_PropertyNotDeclared,
                    parentNode.TypeReference.FullName(),
                    endPathToken.Identifier));
            }
        }

        /// <summary>
        /// Binds an end path token into a PropertyAccessToken, OpenPropertyToken, or FunctionCallToken.
        /// </summary>
        /// <param name="endPathToken">The property access token to bind.</param>
        /// <returns>A Query node representing this endpath token, bound to metadata.</returns>
        internal QueryNode BindEndPath(EndPathToken endPathToken)
        {
            ExceptionUtils.CheckArgumentNotNull(endPathToken, "EndPathToken");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(endPathToken.Identifier, "EndPathToken.Identifier");

            // Set the parent (get the parent type, so you can check whether the Identifier inside EndPathToken really is legit offshoot of the parent type)
            QueryNode parent = this.DetermineParentNode(endPathToken);

            QueryNode boundFunction;

            if (parent is SingleValueNode singleValueParent)
            {
                if (endPathToken.Identifier == ExpressionConstants.QueryOptionCount)
                {
                    return new CountVirtualPropertyNode();
                }

                if (state.IsCollapsed && !IsAggregatedProperty(endPathToken))
                {
                    throw new ODataException(Error.Format(SRResources.ApplyBinder_GroupByPropertyNotPropertyAccessValue, endPathToken.Identifier));
                }

                // Now that we have the parent type, can find its corresponding EDM type
                IEdmStructuredTypeReference structuredParentType =
                    singleValueParent.TypeReference?.AsStructuredOrNull();

                IEdmProperty property =
                    structuredParentType == null ? null : this.Resolver.ResolveProperty(structuredParentType.StructuredDefinition(), endPathToken.Identifier);

                if (property != null)
                {
                    return GeneratePropertyAccessQueryNode(singleValueParent as SingleResourceNode, property, state);
                }

                if (functionCallBinder.TryBindEndPathAsFunctionCall(endPathToken, singleValueParent, state, out boundFunction))
                {
                    return boundFunction;
                }

                return GeneratePropertyAccessQueryForOpenType(endPathToken, singleValueParent);
            }

            // Collection with any or all expression is already supported and handled separately.
            // Add support of collection with $count segment.
            if (parent is CollectionNode colNode
                && endPathToken.Identifier.Equals(UriQueryConstants.CountSegment, System.StringComparison.Ordinal))
            {
                // create a collection count node for collection node property.
                return new CountNode(colNode);
            }

            if (parent is CollectionNavigationNode collectionParent)
            {
                IEdmEntityTypeReference parentType = collectionParent.EntityItemType;
                IEdmProperty property = this.Resolver.ResolveProperty(parentType.StructuredDefinition(), endPathToken.Identifier);

                if (property?.PropertyKind == EdmPropertyKind.Structural
                    && !property.Type.IsCollection()
                    && this.state.InEntitySetAggregation)
                {
                    return new AggregatedCollectionPropertyNode(collectionParent, property);
                }
            }

            if (functionCallBinder.TryBindEndPathAsFunctionCall(endPathToken, parent, state, out boundFunction))
            {
                return boundFunction;
            }

            throw new ODataException(Error.Format(SRResources.MetadataBinder_PropertyAccessSourceNotSingleValue, endPathToken.Identifier));
        }

        /// <summary>
        /// Determines the parent node. If the token has a parent, that token is bound. If not, then we
        /// use the implicit parameter from the BindingState as the parent node.
        /// </summary>
        /// <param name="segmentToken">Token to determine the parent node for.</param>
        /// <returns>A SingleValueQueryNode that is the parent node of the <paramref name="segmentToken"/>.</returns>
        private QueryNode DetermineParentNode(EndPathToken segmentToken)
        {
            ExceptionUtils.CheckArgumentNotNull(segmentToken, "segmentToken");
            ExceptionUtils.CheckArgumentNotNull(state, "state");

            if (segmentToken.NextToken != null)
            {
                return this.bindMethod(segmentToken.NextToken);
            }
            else
            {
                RangeVariable implicitRangeVariable = state.ImplicitRangeVariable;
                return NodeFactory.CreateRangeVariableReferenceNode(implicitRangeVariable);
            }
        }

        /// <summary>
        /// Determines the token if represents an aggregated property or not.
        /// </summary>
        /// <param name="endPath">EndPathToken to check in the list of aggregated properties.</param>
        /// <returns>Whether the token represents an aggregated property.</returns>
        private bool IsAggregatedProperty(EndPathToken endPath) => state?.AggregatedPropertyNames?.Contains(endPath) ?? false;
    }
}