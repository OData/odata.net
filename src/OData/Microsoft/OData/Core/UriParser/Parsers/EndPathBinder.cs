//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion Namespaces

    /// <summary>
    /// Class that knows how to bind an end path token, which could be several things.
    /// </summary>
    internal sealed class EndPathBinder
    {        
        /// <summary>
        /// Method to bind the value of a key.
        /// TODO: Make it of return type SingleValueQueryNode.
        /// </summary>
        private readonly MetadataBinder.QueryTokenVisitor bind;

        /// <summary>
        /// The function call binder to use to bind this end path to a function if necessary.
        /// </summary>
        private readonly FunctionCallBinder functionCallBinder;

        /// <summary>
        /// Constructs a EndPathBinder object using the given function to bind parent token.
        /// </summary>
        /// <param name="bindMethod">Method to bind the EndPathToken's parent, if there is one.</param>
        internal EndPathBinder(MetadataBinder.QueryTokenVisitor bindMethod)
        {
            this.bind = bindMethod;
            this.functionCallBinder = new FunctionCallBinder(bindMethod);
        }

        /// <summary>
        /// This method generates a <see cref="SingleValueOpenPropertyAccessNode"/> for properties of open type
        /// </summary>
        /// <param name="endPathToken">EndPathToken to bind into an open property node.</param>
        /// <param name="parentNode">Parent node of this open property</param>
        /// <returns>Will return a <see cref="SingleValueOpenPropertyAccessNode"/> when open types are supported</returns>
        internal static SingleValueOpenPropertyAccessNode GeneratePropertyAccessQueryForOpenType(EndPathToken endPathToken, SingleValueNode parentNode)
        {
            if (parentNode.TypeReference != null && !parentNode.TypeReference.Definition.IsOpenType())
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_PropertyNotDeclared(
                    parentNode.TypeReference.ODataFullName(),
                    endPathToken.Identifier));
            }

            return new SingleValueOpenPropertyAccessNode(parentNode, endPathToken.Identifier);
        }

        /// <summary>
        /// Generates a bound query node representing an <see cref="IEdmProperty"/> given an already semantically bound parent node.
        /// </summary>
        /// <param name="parentNode">The semantically bound source node of this end path token</param>
        /// <param name="property">The <see cref="IEdmProperty"/> that will be bound to this node. Must not be primitive collection</param>
        /// <returns>QueryNode bound to this property.</returns>
        internal static QueryNode GeneratePropertyAccessQueryNode(SingleValueNode parentNode, IEdmProperty property)
        {
            ExceptionUtils.CheckArgumentNotNull(parentNode, "parent");
            ExceptionUtils.CheckArgumentNotNull(property, "property");

            // TODO: Remove this check. 
            // We should verify that the top level of an expression is a bool rather than arbitrarily restrict property types.
            // We can get here if there is a query like $filter=MyCollectionProperty eq 'foo' or something.
            // If it was $filter=MyCollectionProperty/any(...) then we would have gone down the 'NonRootSegment' code path instead of this one
            if (property.Type.IsNonEntityCollectionType())
            {
                // if this happens to be a top level node (i.e. $filter=MyCollection), then it will fail further up the chain, so
                // don't need to worry about checking for that here.
                return new CollectionPropertyAccessNode(parentNode, property);
            }

            if (property.PropertyKind == EdmPropertyKind.Navigation)
            {
                // These are error cases in practice, but we let ourselves throw later for better context-sensitive error messages
                var edmNavigationProperty = (IEdmNavigationProperty)property;
                var singleEntityParentNode = (SingleEntityNode)parentNode;
                if (edmNavigationProperty.TargetMultiplicityTemporary() == EdmMultiplicity.Many)
                {
                    return new CollectionNavigationNode(edmNavigationProperty, singleEntityParentNode);
                }

                return new SingleNavigationNode(edmNavigationProperty, singleEntityParentNode);
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
                throw new ODataException(ODataErrorStrings.MetadataBinder_PropertyAccessWithoutParentParameter);
            }

            return NodeFactory.CreateRangeVariableReferenceNode(state.ImplicitRangeVariable);
        }

        /// <summary>
        /// Binds a an end path token into a PropertyAccessToken, OpenPropertyToken, or FunctionCallToken.
        /// </summary>
        /// <param name="endPathToken">The property access token to bind.</param>
        /// <param name="state">State of the binding algorithm.</param>
        /// <returns>A Query node representing this endpath token, bound to metadata.</returns>
        internal QueryNode BindEndPath(EndPathToken endPathToken, BindingState state)
        {
            ExceptionUtils.CheckArgumentNotNull(endPathToken, "EndPathToken");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(endPathToken.Identifier, "EndPathToken.Identifier");

            // Set the parent (get the parent type, so you can check whether the Identifier inside EndPathToken really is legit offshoot of the parent type)
            QueryNode parent = this.DetermineParentNode(endPathToken, state);

            QueryNode boundFunction;

            SingleValueNode singleValueParent = parent as SingleValueNode;
            if (singleValueParent == null)
            {
                if (functionCallBinder.TryBindEndPathAsFunctionCall(endPathToken, parent, state, out boundFunction))
                {
                    return boundFunction;
                }

                throw new ODataException(ODataErrorStrings.MetadataBinder_PropertyAccessSourceNotSingleValue(endPathToken.Identifier));
            }


            // Now that we have the parent type, can find its corresponding EDM type
            IEdmStructuredTypeReference structuredParentType =
                singleValueParent.TypeReference == null ? null : singleValueParent.TypeReference.AsStructuredOrNull();

            IEdmProperty property =
                structuredParentType == null ? null : structuredParentType.FindProperty(endPathToken.Identifier);

            if (property != null)
            {
                return GeneratePropertyAccessQueryNode(singleValueParent, property);   
            }

            if (functionCallBinder.TryBindEndPathAsFunctionCall(endPathToken, singleValueParent, state, out boundFunction))
            {
                return boundFunction;
            }

            return GeneratePropertyAccessQueryForOpenType(endPathToken, singleValueParent);
        }

        /// <summary>
        /// Determines the parent node. If the token has a parent, that token is bound. If not, then we 
        /// use the implicit parameter from the BindingState as the parent node.
        /// </summary>
        /// <param name="segmentToken">Token to determine the parent node for.</param>
        /// <param name="state">Current state of binding.</param>
        /// <returns>A SingleValueQueryNode that is the parent node of the <paramref name="segmentToken"/>.</returns>
        private QueryNode DetermineParentNode(EndPathToken segmentToken, BindingState state)
        {
            ExceptionUtils.CheckArgumentNotNull(segmentToken, "segmentToken");
            ExceptionUtils.CheckArgumentNotNull(state, "state");

            if (segmentToken.NextToken != null)
            {
                return this.bind(segmentToken.NextToken);
            }
            else
            {
                RangeVariable implicitRangeVariable = state.ImplicitRangeVariable;
                return NodeFactory.CreateRangeVariableReferenceNode(implicitRangeVariable);
            }
        }
    }
}
