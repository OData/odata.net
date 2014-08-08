//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
