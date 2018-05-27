//---------------------------------------------------------------------
// <copyright file="NodeFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
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

            if (elementType.IsStructured())
            {
                IEdmStructuredTypeReference structuredTypeReference = elementType.AsStructured();
                return new ResourceRangeVariable(ExpressionConstants.It, structuredTypeReference, path.NavigationSource());
            }

            return new NonResourceRangeVariable(ExpressionConstants.It, elementType, null);
        }

        /// <summary>
        /// Creates a ParameterQueryNode for an implicit parameter ($it).
        /// </summary>
        /// <param name="elementType">Element type the parameter represents.</param>
        /// <param name="navigationSource">The navigation source. May be null and must be null for non structured types.</param>
        /// <returns>A new IParameterNode.</returns>
        internal static RangeVariable CreateImplicitRangeVariable(IEdmTypeReference elementType, IEdmNavigationSource navigationSource)
        {
            if (elementType.IsStructured())
            {
                return new ResourceRangeVariable(ExpressionConstants.It, elementType as IEdmStructuredTypeReference, navigationSource);
            }

            Debug.Assert(navigationSource == null, "if the type wasn't a structured type then there should be no navigation source");
            return new NonResourceRangeVariable(ExpressionConstants.It, elementType, null);
        }

        /// <summary>
        /// Creates a RangeVariableReferenceNode for a given range variable
        /// </summary>
        /// <param name="rangeVariable">Name of the rangeVariable.</param>
        /// <returns>A new SingleValueNode (either a Resource or NonResource RangeVariableReferenceNode.</returns>
        internal static SingleValueNode CreateRangeVariableReferenceNode(RangeVariable rangeVariable)
        {
            if (rangeVariable.Kind == RangeVariableKind.NonResource)
            {
                return new NonResourceRangeVariableReferenceNode(rangeVariable.Name, (NonResourceRangeVariable)rangeVariable);
            }
            else
            {
                ResourceRangeVariable resourceRangeVariable = (ResourceRangeVariable)rangeVariable;
                return new ResourceRangeVariableReferenceNode(resourceRangeVariable.Name, resourceRangeVariable);
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
            IEdmTypeReference elementType = nodeToIterateOver.ItemType;

            if (elementType != null && elementType.IsStructured())
            {
                var collectionResourceNode = nodeToIterateOver as CollectionResourceNode;
                Debug.Assert(collectionResourceNode != null, "IF the element type was structured, the node type should be a resource collection");
                return new ResourceRangeVariable(parameter, elementType as IEdmStructuredTypeReference, collectionResourceNode);
            }

            return new NonResourceRangeVariable(parameter, elementType, null);
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