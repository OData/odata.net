//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.Query
{
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.Metadata;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

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
            DebugUtils.CheckNoExternalCallers(); 
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
            DebugUtils.CheckNoExternalCallers(); 
            ExceptionUtils.CheckArgumentNotNull(dottedIdentifierToken, "castToken");
            ExceptionUtils.CheckArgumentNotNull(state, "state");

            QueryNode parent;
            IEdmType parentType;
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

            SingleEntityNode parentAsSingleValue = parent as SingleEntityNode;

            IEdmSchemaType childType = UriEdmHelpers.FindTypeFromModel(state.Model, dottedIdentifierToken.Identifier);
            IEdmEntityType childEntityType = childType as IEdmEntityType;
            if (childEntityType == null)
            {
                FunctionCallBinder functionCallBinder = new FunctionCallBinder(bindMethod);
                QueryNode functionCallNode;
                if (functionCallBinder.TryBindDottedIdentifierAsFunctionCall(dottedIdentifierToken, parentAsSingleValue, state, out functionCallNode))
                {
                    return functionCallNode;
                }
                else
                {
                    throw new ODataException(ODataErrorStrings.CastBinder_ChildTypeIsNotEntity(dottedIdentifierToken.Identifier));
                }
            }

            // Check whether childType is a derived type of the type of its parent node
            UriEdmHelpers.CheckRelatedTo(parentType, childType);

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
    }
}
