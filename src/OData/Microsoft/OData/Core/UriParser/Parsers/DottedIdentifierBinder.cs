//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Class that knows how to bind CastTokens.
    /// </summary>
    internal sealed class DottedIdentifierBinder
    {
        /// <summary>
        /// Method to use for binding the parent node, if needed.
        /// </summary>
        private readonly MetadataBinder.QueryTokenVisitor bindMethod;

        /// <summary>
        /// Constructs a DottedIdentifierBinder with the given method to be used binding the parent token if needed.
        /// </summary>
        /// <param name="bindMethod">Method to use for binding the parent token, if needed.</param>
        internal DottedIdentifierBinder(MetadataBinder.QueryTokenVisitor bindMethod)
        {
            ExceptionUtils.CheckArgumentNotNull(bindMethod, "bindMethod");
            this.bindMethod = bindMethod;
        }

        /// <summary>
        /// Binds a DottedIdentifierToken and it's parent node (if needed).
        /// </summary>
        /// <param name="dottedIdentifierToken">Token to bind to metadata.</param>
        /// <param name="state">State of the Binding.</param>
        /// <returns>A bound node representing the cast.</returns>
        internal QueryNode BindDottedIdentifier(DottedIdentifierToken dottedIdentifierToken, BindingState state)
        {
            ExceptionUtils.CheckArgumentNotNull(dottedIdentifierToken, "castToken");
            ExceptionUtils.CheckArgumentNotNull(state, "state");

            QueryNode parent = null;
            IEdmType parentType = null;
            if (state.ImplicitRangeVariable != null)
            {
                if (dottedIdentifierToken.NextToken == null)
                {
                    parent = NodeFactory.CreateRangeVariableReferenceNode(state.ImplicitRangeVariable);
                    parentType = state.ImplicitRangeVariable.TypeReference.Definition;
                }
                else
                {
                    parent = this.bindMethod(dottedIdentifierToken.NextToken);
                    parentType = parent.GetEdmType();
                }
            }

            SingleEntityNode parentAsSingleValue = parent as SingleEntityNode;
            IEdmSchemaType childType = UriEdmHelpers.FindTypeFromModel(state.Model, dottedIdentifierToken.Identifier);
            IEdmStructuredType childStructuredType = childType as IEdmStructuredType;
            if (childStructuredType == null)
            {
                FunctionCallBinder functionCallBinder = new FunctionCallBinder(bindMethod);
                QueryNode functionCallNode;
                if (functionCallBinder.TryBindDottedIdentifierAsFunctionCall(dottedIdentifierToken, parentAsSingleValue, state, out functionCallNode))
                {
                    return functionCallNode;
                }
                else if ((!string.IsNullOrEmpty(dottedIdentifierToken.Identifier))
                    && (dottedIdentifierToken.Identifier[dottedIdentifierToken.Identifier.Length - 1] == '\''))
                {
                    // check if it is enum or not
                    EnumBinder enumBinder = new EnumBinder(this.bindMethod);
                    QueryNode enumNode;
                    if (enumBinder.TryBindDottedIdentifierAsEnum(dottedIdentifierToken, parentAsSingleValue, state, out enumNode))
                    {
                        return enumNode;
                    }
                    else
                    {
                        throw new ODataException(ODataErrorStrings.Binder_IsNotValidEnumConstant(dottedIdentifierToken.Identifier));
                    }
                }
                else
                {
                    IEdmTypeReference edmTypeReference = UriEdmHelpers.FindTypeFromModel(state.Model, dottedIdentifierToken.Identifier).ToTypeReference();
                    if (edmTypeReference is IEdmPrimitiveTypeReference || edmTypeReference is IEdmEnumTypeReference)
                    {
                        return new ConstantNode(dottedIdentifierToken.Identifier, dottedIdentifierToken.Identifier);
                    }
                    else
                    {
                        throw new ODataException(ODataErrorStrings.CastBinder_ChildTypeIsNotEntity(dottedIdentifierToken.Identifier));
                    }
                }
            }

            // Check whether childType is a derived type of the type of its parent node
            UriEdmHelpers.CheckRelatedTo(parentType, childType);

            IEdmEntityType childEntityType = childStructuredType as IEdmEntityType;
            if (childEntityType != null)
            {
                EntityCollectionNode parentAsCollection = parent as EntityCollectionNode;
                if (parentAsCollection != null)
                {
                    return new EntityCollectionCastNode(parentAsCollection, childEntityType);
                }

                // parent can be null for casts on the implicit parameter; this is OK
                if (parent == null)
                {
                    return new SingleEntityCastNode(null, childEntityType);
                }

                Debug.Assert(parentAsSingleValue != null, "If parent of the cast node was not collection, it should be a single value.");
                return new SingleEntityCastNode(parentAsSingleValue, childEntityType);
            }
            else
            {
                IEdmComplexType childComplexType = childStructuredType as IEdmComplexType;
                Debug.Assert(childComplexType != null, "If it is not entity type, it should be complex type");

                CollectionPropertyAccessNode parentAsCollectionProperty = parent as CollectionPropertyAccessNode;
                if (parentAsCollectionProperty != null)
                {
                    return new CollectionPropertyCastNode(parentAsCollectionProperty, childComplexType);
                }

                // parent can be null for casts on the implicit parameter; this is OK
                if (parent == null)
                {
                    return new SingleValueCastNode(null, childComplexType);
                }

                SingleValueNode parentAsSingleValueNode = parent as SingleValueNode;
                Debug.Assert(parentAsSingleValueNode != null, "If parent of the cast node was not collection, it should be a single value.");
                return new SingleValueCastNode(parentAsSingleValueNode, childComplexType);
            }
        }
    }
}
