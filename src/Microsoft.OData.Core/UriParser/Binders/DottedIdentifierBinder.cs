//---------------------------------------------------------------------
// <copyright file="DottedIdentifierBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OData.Metadata;
using Microsoft.OData.Edm;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.UriParser
{
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

            SingleResourceNode parentAsSingleResource = parent as SingleResourceNode;
            IEdmSchemaType childType = UriEdmHelpers.FindTypeFromModel(state.Model, dottedIdentifierToken.Identifier, this.Resolver);
            IEdmStructuredType childStructuredType = childType as IEdmStructuredType;
            if (childStructuredType == null)
            {
                SingleValueNode singleValueNode = parent as SingleValueNode;
                FunctionCallBinder functionCallBinder = new FunctionCallBinder(bindMethod, state);
                QueryNode functionCallNode;
                if (functionCallBinder.TryBindDottedIdentifierAsFunctionCall(dottedIdentifierToken, singleValueNode, out functionCallNode))
                {
                    return functionCallNode;
                }
                else if ((!string.IsNullOrEmpty(dottedIdentifierToken.Identifier))
                    && (dottedIdentifierToken.Identifier[dottedIdentifierToken.Identifier.Length - 1] == '\''))
                {
                    // check if it is enum or not
                    QueryNode enumNode;
                    if (EnumBinder.TryBindDottedIdentifierAsEnum(dottedIdentifierToken, parentAsSingleResource, state, out enumNode))
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
                        IEdmPrimitiveType childPrimitiveType = childType as IEdmPrimitiveType;
                        if (childPrimitiveType != null && dottedIdentifierToken.NextToken != null)
                        {
                            return new SingleValueCastNode(singleValueNode, childPrimitiveType);
                        }
                        else
                        {
                            return new ConstantNode(dottedIdentifierToken.Identifier, dottedIdentifierToken.Identifier);
                        }
                    }
                    else
                    {
                        throw new ODataException(ODataErrorStrings.CastBinder_ChildTypeIsNotEntity(dottedIdentifierToken.Identifier));
                    }
                }
            }

            // Check whether childType is a derived type of the type of its parent node
            UriEdmHelpers.CheckRelatedTo(parentType, childType);

            this.state.ParsedSegments.Add(new TypeSegment(childType, parentType, null));

            CollectionResourceNode parentAsCollection = parent as CollectionResourceNode;
            if (parentAsCollection != null)
            {
                return new CollectionResourceCastNode(parentAsCollection, childStructuredType);
            }

            return new SingleResourceCastNode(parentAsSingleResource, childStructuredType);
        }
    }
}