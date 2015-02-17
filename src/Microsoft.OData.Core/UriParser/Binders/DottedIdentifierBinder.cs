//---------------------------------------------------------------------
// <copyright file="DottedIdentifierBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Binders;
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Class that knows how to bind CastTokens.
    /// </summary>
    internal sealed class DottedIdentifierBinder : BinderBase
    {
        /// <summary>
        /// Constructs a DottedIdentifierBinder with the given method to be used binding the parent token if needed.
        /// </summary>
        /// <param name="bindMethod">Method to use for binding the parent token, if needed.</param>
        /// <param name="state">State of the metadata binding.</param>
        internal DottedIdentifierBinder(MetadataBinder.QueryTokenVisitor bindMethod, BindingState state)
            : base(bindMethod, state)
        {
        }

        /// <summary>
        /// Binds a DottedIdentifierToken and it's parent node (if needed).
        /// </summary>
        /// <param name="dottedIdentifierToken">Token to bind to metadata.</param>
        /// <returns>A bound node representing the cast.</returns>
        internal QueryNode BindDottedIdentifier(DottedIdentifierToken dottedIdentifierToken)
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
            IEdmSchemaType childType = UriEdmHelpers.FindTypeFromModel(state.Model, dottedIdentifierToken.Identifier, this.Resolver);
            IEdmStructuredType childStructuredType = childType as IEdmStructuredType;
            if (childStructuredType == null)
            {
                FunctionCallBinder functionCallBinder = new FunctionCallBinder(bindMethod, state);
                QueryNode functionCallNode;
                if (functionCallBinder.TryBindDottedIdentifierAsFunctionCall(dottedIdentifierToken, parent as SingleValueNode, out functionCallNode))
                {
                    return functionCallNode;
                }
                else if ((!string.IsNullOrEmpty(dottedIdentifierToken.Identifier))
                    && (dottedIdentifierToken.Identifier[dottedIdentifierToken.Identifier.Length - 1] == '\''))
                {
                    // check if it is enum or not
                    QueryNode enumNode;
                    if (EnumBinder.TryBindDottedIdentifierAsEnum(dottedIdentifierToken, parentAsSingleValue, state, out enumNode))
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
                    IEdmTypeReference edmTypeReference = UriEdmHelpers.FindTypeFromModel(state.Model, dottedIdentifierToken.Identifier, this.Resolver).ToTypeReference();
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