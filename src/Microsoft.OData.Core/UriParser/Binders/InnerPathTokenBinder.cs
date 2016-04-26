//---------------------------------------------------------------------
// <copyright file="InnerPathTokenBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Metadata;
using Microsoft.OData.Edm;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Class responsible for binding a InnerPathToken into:
    /// 1. SingleNavigationNode
    /// 2. CollectionNavigationNode
    /// 3. SinglePropertyAccessNode (complex)
    /// 4. CollectionPropertyAccessNode (primitive | complex)
    /// 5. KeyLookupNode
    /// 6. SingleValueFunctionCallNode
    /// 7. SingleEntityFunctionCallNode
    /// </summary>
    /// <remarks>
    /// TODO: The binder does support key lookup on collection navigation properties, however at this time
    /// the synctactic parser does not set things up correctly to allow end-to-end scenarios to work.
    /// </remarks>
    internal sealed class InnerPathTokenBinder : BinderBase
    {
        /// <summary>
        /// Constructs a InnerPathTokenBinder.
        /// </summary>
        /// <param name="bindMethod">Bind method to use for binding a parent node, if needed.</param>
        /// <param name="state">State of the metadata binding.</param>
        internal InnerPathTokenBinder(MetadataBinder.QueryTokenVisitor bindMethod, BindingState state)
            : base(bindMethod, state)
        {
        }

        /// <summary>
        /// Ensures that the parent node is of entity type, throwing if it is not.
        /// </summary>
        /// <param name="parent">Parent node to a navigation property.</param>
        /// <returns>The given parent node as a SingleEntityNode.</returns>
        internal static SingleEntityNode EnsureParentIsEntityForNavProp(SingleValueNode parent)
        {
            ExceptionUtils.CheckArgumentNotNull(parent, "parent");
            SingleEntityNode parentEntity = parent as SingleEntityNode;
            if (parentEntity == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_NavigationPropertyNotFollowingSingleEntityType);
            }

            return parentEntity;
        }

        /// <summary>
        /// Given a property name, if the associated type reference is strucutred, then this returns  
        /// the property of the structured type. Otherwise, it returns null.
        /// </summary>
        /// <param name="parentReference">The parent type to be used to find binding options.</param>
        /// <param name="propertyName">The string designated the property name to be bound.</param>
        /// <param name="resolver">Resolver for uri parser.</param>
        /// <returns>The property associated with string and parent type.</returns>
        internal static IEdmProperty BindProperty(IEdmTypeReference parentReference, string propertyName, ODataUriResolver resolver = null)
        {
            if (resolver == null)
            {
                resolver = ODataUriResolver.Default;
            }

            IEdmStructuredTypeReference structuredParentType =
                parentReference == null ? null : parentReference.AsStructuredOrNull();
            return structuredParentType == null ? null : resolver.ResolveProperty(structuredParentType.StructuredDefinition(), propertyName);
        }

        /// <summary>
        /// Builds an appropriate navigation query node (collection or single) for the given property and parent node.
        /// </summary>
        /// <param name="property">Navigation property.</param>
        /// <param name="parent">Parent Node.</param>
        /// <param name="namedValues">Named values (key values) that were included in the node we are binding, if any.</param>
        /// <param name="state">State of binding.</param>
        /// <param name="keyBinder">Object to perform binding on any key values that are present.</param>
        /// <returns>A new CollectionNavigationNode or SingleNavigationNode to capture the navigation propety access.</returns>
        internal static QueryNode GetNavigationNode(IEdmNavigationProperty property, SingleEntityNode parent, IEnumerable<NamedValue> namedValues, BindingState state, KeyBinder keyBinder)
        {
            ExceptionUtils.CheckArgumentNotNull(property, "property");
            ExceptionUtils.CheckArgumentNotNull(parent, "parent");
            ExceptionUtils.CheckArgumentNotNull(state, "state");
            ExceptionUtils.CheckArgumentNotNull(keyBinder, "keyBinder");

            // Handle collection navigation property
            if (property.TargetMultiplicity() == EdmMultiplicity.Many)
            {
                CollectionNavigationNode collectionNavigationNode = new CollectionNavigationNode(property, parent);

                // Doing key lookup on the collection navigation property
                if (namedValues != null)
                {
                    return keyBinder.BindKeyValues(collectionNavigationNode, namedValues, state.Model);
                }

                // Otherwise it's just a normal collection of entities
                return collectionNavigationNode;
            }

            Debug.Assert(namedValues == null || !namedValues.Any(), "namedValues should not exist if it isn't a colleciton");

            // Otherwise it's a single navigation property
            return new SingleNavigationNode(property, parent);
        }

        /// <summary>
        /// Binds a <see cref="InnerPathToken"/>.
        /// This includes more than just navigations - it includes complex property access and primitive collections.
        /// </summary>
        /// <param name="segmentToken">The segment token to bind.</param>
        /// <returns>The bound node.</returns>
        internal QueryNode BindInnerPathSegment(InnerPathToken segmentToken)
        {
            FunctionCallBinder functionCallBinder = new FunctionCallBinder(this.bindMethod, state);

            // First we get the parent node
            QueryNode parent = this.DetermineParentNode(segmentToken, state);
            Debug.Assert(parent != null, "parent should never be null");

            SingleValueNode singleValueParent = parent as SingleValueNode;
            if (singleValueParent == null)
            {
                QueryNode boundFunction;
                if (functionCallBinder.TryBindInnerPathAsFunctionCall(segmentToken, parent, out boundFunction))
                {
                    return boundFunction;
                }

                throw new ODataException(ODataErrorStrings.MetadataBinder_PropertyAccessSourceNotSingleValue(segmentToken.Identifier));
            }

            // Using the parent and name of this token, we try to get the IEdmProperty it represents
            IEdmProperty property = BindProperty(singleValueParent.TypeReference, segmentToken.Identifier, this.Resolver);

            if (property == null)
            {
                QueryNode boundFunction;
                if (functionCallBinder.TryBindInnerPathAsFunctionCall(segmentToken, parent, out boundFunction))
                {
                    return boundFunction;
                }

                if (singleValueParent.TypeReference != null && !singleValueParent.TypeReference.Definition.IsOpenType())
                {
                    throw new ODataException(
                        ODataErrorStrings.MetadataBinder_PropertyNotDeclared(
                            parent.GetEdmTypeReference().FullName(), segmentToken.Identifier));
                }

                return new SingleValueOpenPropertyAccessNode(singleValueParent, segmentToken.Identifier);
            }

            if (property.Type.IsODataComplexTypeKind())
            {
                return new SingleValuePropertyAccessNode(singleValueParent, property);
            }

            // Note - this means nonentity collection (primitive or complex)
            if (property.Type.IsNonEntityCollectionType())
            {
                return new CollectionPropertyAccessNode(singleValueParent, property);
            }

            IEdmNavigationProperty navigationProperty = property as IEdmNavigationProperty;
            if (navigationProperty == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_IllegalSegmentType(property.Name));
            }

            SingleEntityNode parentEntity = EnsureParentIsEntityForNavProp(singleValueParent);

            return GetNavigationNode(navigationProperty, parentEntity, segmentToken.NamedValues, state, new KeyBinder(this.bindMethod));
        }

        /// <summary>
        /// Determines the parent node. If the token has a parent, that token is bound. If not, then we 
        /// use the implicit parameter from the BindingState as the parent node.
        /// </summary>
        /// <param name="segmentToken">Token to determine the parent node for.</param>
        /// <param name="state">Current state of binding.</param>
        /// <returns>A SingleValueQueryNode that is the parent node of the <paramref name="segmentToken"/>.</returns>
        private QueryNode DetermineParentNode(InnerPathToken segmentToken, BindingState state)
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
    }
}