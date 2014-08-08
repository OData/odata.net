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
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Query.SemanticAst;

    /// <summary>
    /// Factory class to build IParameterQueryNodes.
    /// </summary>
    internal static class NodeFactory
    {
        /// <summary>
        /// Creates a <see cref="RangeVariable"/> for an implicit parameter ($it) from an <see cref="ODataPath"/>.
        /// </summary>
        /// <param name="path"><see cref="ODataPath"/> that the range variable is iterating over.</param>
        /// <returns>A new <see cref="RangeVariable"/>.</returns>
        internal static RangeVariable CreateImplicitRangeVariable(ODataPath path)
        {
            DebugUtils.CheckNoExternalCallers(); 
            ExceptionUtils.CheckArgumentNotNull(path, "path");
            IEdmTypeReference elementType = path.EdmType();

            if (elementType == null)
            {
                // This case if for something like a void service operation
                // This is pretty ugly; if pratice we shouldn't be creating a parameter node for this case I think
                return null;
            }

            if (elementType.IsCollection())
            {
                elementType = elementType.AsCollection().ElementType();
            }

            if (elementType.IsEntity())
            {
                IEdmEntityTypeReference entityTypeReference = elementType as IEdmEntityTypeReference;
                return new EntityRangeVariable(ExpressionConstants.It, entityTypeReference, path.EntitySet());
            }

            return new NonentityRangeVariable(ExpressionConstants.It, elementType, null);
        }

        /// <summary>
        /// Creates a ParameterQueryNode for an implicit parameter ($it).
        /// </summary>
        /// <param name="elementType">Element type the parameter represents.</param>
        /// <param name="entitySet">Entity Set. May be null and must be null for non entities.</param>
        /// <returns>A new IParameterNode.</returns>
        internal static RangeVariable CreateImplicitRangeVariable(IEdmTypeReference elementType, IEdmEntitySet entitySet)
        {
            DebugUtils.CheckNoExternalCallers(); 
            if (elementType.IsEntity())
            {
                return new EntityRangeVariable(ExpressionConstants.It, elementType as IEdmEntityTypeReference, entitySet);
            }

            Debug.Assert(entitySet == null, "if the type wasn't an entity then there should be no entity set");
            return new NonentityRangeVariable(ExpressionConstants.It, elementType, null);
        }

        /// <summary>
        /// Creates a RangeVariableReferenceNode for a given range variable
        /// </summary>
        /// <param name="rangeVariable">Name of the rangeVariable.</param>
        /// <returns>A new SingleValueNode (either an Entity or NonEntity RangeVariableReferenceNode.</returns>
        internal static SingleValueNode CreateRangeVariableReferenceNode(RangeVariable rangeVariable)
        {
            DebugUtils.CheckNoExternalCallers();
            if (rangeVariable.Kind == RangeVariableKind.Nonentity)
            {
                return new NonentityRangeVariableReferenceNode(rangeVariable.Name, (NonentityRangeVariable)rangeVariable);
            }
            else
            {
                EntityRangeVariable entityRangeVariable = (EntityRangeVariable)rangeVariable;
                return new EntityRangeVariableReferenceNode(entityRangeVariable.Name, entityRangeVariable);
            }
        }

        /// <summary>
        /// Creates a ParameterQueryNode for an explicit parameter.
        /// </summary>
        /// <param name="parameter">Name of the parameter.</param>
        /// <param name="nodeToIterateOver">CollectionNode that the parameter is iterating over.</param>
        /// <returns>A new RangeVariable.</returns>
        internal static RangeVariable CreateParameterNode(string parameter, CollectionNode nodeToIterateOver)
        {
            DebugUtils.CheckNoExternalCallers(); 
            IEdmTypeReference elementType = nodeToIterateOver.ItemType;

            if (elementType.IsEntity())
            {
                var entityCollectionNode = nodeToIterateOver as EntityCollectionNode;
                Debug.Assert(entityCollectionNode != null, "IF the element type was entity, the node type should be an entity collection");
                return new EntityRangeVariable(parameter, elementType as IEdmEntityTypeReference, entityCollectionNode);
            }

            return new NonentityRangeVariable(parameter, elementType, null);
        }

        /// <summary>
        /// Creates an AnyNode or an AllNode from the given 
        /// </summary>
        /// <param name="state">State of binding.</param>
        /// <param name="parent">Parent node to the lambda.</param>
        /// <param name="lambdaExpression">Bound Lambda expression.</param>
        /// <param name="newRangeVariable">The new range variable being added by this lambda node.</param>
        /// <param name="queryTokenKind">Token kind.</param>
        /// <returns>A new LambdaNode bound to metadata.</returns>
        internal static LambdaNode CreateLambdaNode(
            BindingState state,
            CollectionNode parent,                                           
            SingleValueNode lambdaExpression, 
            RangeVariable newRangeVariable,
            QueryTokenKind queryTokenKind)
        {
            DebugUtils.CheckNoExternalCallers();
            LambdaNode lambdaNode;
            if (queryTokenKind == QueryTokenKind.Any)
            {
                lambdaNode = new AnyNode(new Collection<RangeVariable>(state.RangeVariables.ToList()), newRangeVariable)
                    {
                        Body = lambdaExpression,
                        Source = parent,
                    };
            }
            else
            {
                Debug.Assert(queryTokenKind == QueryTokenKind.All, "LambdaQueryNodes must be Any or All only.");
                lambdaNode = new AllNode(new Collection<RangeVariable>(state.RangeVariables.ToList()), newRangeVariable)
                    {
                        Body = lambdaExpression,
                        Source = parent,
                    };
            }
            
            return lambdaNode;
        }
    }
}
