//---------------------------------------------------------------------
// <copyright file="ExpressionValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//--------

using System;
using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser.Validation.ValidationEngine
{
    /// <summary>
    /// Class to walk an expression, validating query nodes within the expression
    /// </summary>
    internal class ExpressionValidator : QueryNodeVisitor<bool>
    {
        private Action<object> validate;

        /// <summary>
        /// Creates an expression validator.
        /// </summary>
        /// <param name="validate">The action to call to validate an element within the expression</param>
        public ExpressionValidator(Action<object> validate)
        {
            this.validate = validate;
        }

        /// <summary>
        /// Main dispatching visit method for validating query-nodes.
        /// </summary>
        /// <remarks>
        /// Type references are not validated as nullability is only meaningful in context of the element
        /// Types are validated for each element if the element defines the type (i.e., property, cast, function,...)
        /// </remarks>
        /// <param name="node">The node to visit/translate.</param>
        /// <returns>bool indicating whether or not any errors were found.</returns>
        public bool ValidateNode(QueryNode node)
        {
            return node.Accept(this);
        }

        /// <summary>
        /// Visit an AllNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(AllNode nodeIn)
        {
            validate(nodeIn);
            ValidateNode(nodeIn.Body);
            return true;
        }

        /// <summary>
        /// Visit an AnyNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(AnyNode nodeIn)
        {
            validate(nodeIn);
            ValidateNode(nodeIn.Body);
            return true;
        }

        /// <summary>
        /// Visit a BinaryOperatorNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(BinaryOperatorNode nodeIn)
        {
            validate(nodeIn);
            ValidateNode(nodeIn.Left);
            ValidateNode(nodeIn.Right);
            return true;
        }

        /// <summary>
        /// Visit a CountNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(CountNode nodeIn)
        {
            validate(nodeIn);
            return true;
        }

        /// <summary>
        /// Visit a CollectionNavigationNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(CollectionNavigationNode nodeIn)
        {
            validate(nodeIn);
            validate(nodeIn.NavigationProperty);

            // don't validate TypeReferences, only types, as nullability is only meaningful in context of model element
            validate(nodeIn.CollectionType.CollectionDefinition());
            validate(nodeIn.ItemType.Definition);
            return true;
        }

        /// <summary>
        /// Visit a CollectionPropertyAccessNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(CollectionPropertyAccessNode nodeIn)
        {
            validate(nodeIn);
            validate(nodeIn.Property);

            // don't validate TypeReferences, only types, as nullability is only meaningful in context of model element
            validate(nodeIn.CollectionType.CollectionDefinition());
            validate(nodeIn.ItemType.Definition);
            return true;
        }

        /// <summary>
        /// Visit a CollectionOpenPropertyAccessNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(CollectionOpenPropertyAccessNode nodeIn)
        {
            validate(nodeIn);

            // don't validate TypeReferences, only types, as nullability is only meaningful in context of model element
            validate(nodeIn.CollectionType.CollectionDefinition());
            validate(nodeIn.ItemType.Definition);
            return true;
        }

        /// <summary>
        /// Visit a ConstantNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(ConstantNode nodeIn)
        {
            validate(nodeIn);
            return true;
        }

        /// <summary>
        /// Visit a CollectionConstantNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(CollectionConstantNode nodeIn)
        {
            validate(nodeIn);
            return true;
        }

        /// <summary>
        /// Visit a ConvertNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(ConvertNode nodeIn)
        {
            validate(nodeIn);
            validate(nodeIn.TypeReference.Definition);
            if (nodeIn.TypeReference.IsCollection())
            {
                validate(nodeIn.TypeReference.Definition.AsElementType());
            }

            return true;
        }

        /// <summary>
        /// Visit an CollectionResourceCastNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(CollectionResourceCastNode nodeIn)
        {
            validate(nodeIn);
            validate(nodeIn.CollectionType.CollectionDefinition());
            validate(nodeIn.ItemType.Definition);
            return true;
        }

        /// <summary>
        /// Visit an ResourceRangeVariableReferenceNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(ResourceRangeVariableReferenceNode nodeIn)
        {
            // todo: do we even need to call for a range variable?
            validate(nodeIn);
            return true;
        }

        /// <summary>
        /// Visit a NonEntityRangeVariableNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(NonResourceRangeVariableReferenceNode nodeIn)
        {
            // todo: do we even need to call for a range variable?
            validate(nodeIn);
            return true;
        }

        /// <summary>
        /// Visit a SingleResourceCastNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(SingleResourceCastNode nodeIn)
        {
            validate(nodeIn);
            validate(nodeIn.TypeReference.Definition);
            return true;
        }

        /// <summary>
        /// Visit a SingleNavigationNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(SingleNavigationNode nodeIn)
        {
            validate(nodeIn);
            validate(nodeIn.NavigationProperty);
            validate(nodeIn.TypeReference.Definition);
            return true;
        }

        /// <summary>
        /// Visit a SingleResourceFunctionCallNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(SingleResourceFunctionCallNode nodeIn)
        {
            validate(nodeIn);
            validate(nodeIn.TypeReference.Definition);
            foreach (IEdmFunction function in nodeIn.Functions)
            {
                validate(function);
            }

            foreach (QueryNode param in nodeIn.Parameters)
            {
                ValidateNode(param);
            }

            return true;
        }

        /// <summary>
        /// Visit a SingleValueFunctionCallNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(SingleValueFunctionCallNode nodeIn)
        {
            validate(nodeIn);
            validate(nodeIn.TypeReference.Definition);
            foreach (IEdmFunction function in nodeIn.Functions)
            {
                validate(function);
            }

            foreach (QueryNode param in nodeIn.Parameters)
            {
                ValidateNode(param);
            }

            return true;
        }

        /// <summary>
        /// Visit a CollectionResourceFunctionCallNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(CollectionResourceFunctionCallNode nodeIn)
        {
            validate(nodeIn);
            validate(nodeIn.ItemType.Definition);
            validate(nodeIn.CollectionType.CollectionDefinition());
            foreach (IEdmFunction function in nodeIn.Functions)
            {
                validate(function);
            }

            foreach (QueryNode param in nodeIn.Parameters)
            {
                ValidateNode(param);
            }

            return true;
        }

        /// <summary>
        /// Visit a CollectionFunctionCallNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(CollectionFunctionCallNode nodeIn)
        {
            validate(nodeIn);
            validate(nodeIn.ItemType.Definition);
            validate(nodeIn.CollectionType.CollectionDefinition());
            foreach (IEdmFunction function in nodeIn.Functions)
            {
                validate(function);
            }

            foreach (QueryNode param in nodeIn.Parameters)
            {
                ValidateNode(param);
            }

            return true;
        }

        /// <summary>
        /// Visit a SingleValueOpenPropertyAccessNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(SingleValueOpenPropertyAccessNode nodeIn)
        {
            validate(nodeIn);
            validate(nodeIn.TypeReference.Definition);
            return true;
        }

        /// <summary>
        /// Visit a SingleValuePropertyAccessNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(SingleValuePropertyAccessNode nodeIn)
        {
            validate(nodeIn);
            validate(nodeIn.Property);
            validate(nodeIn.TypeReference.Definition);
            return true;
        }

        /// <summary>
        /// Visit a UnaryOperatorNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(UnaryOperatorNode nodeIn)
        {
            validate(nodeIn);
            ValidateNode(nodeIn.Operand);
            return true;
        }

        /// <summary>
        /// Visit a NamedFunctionParameterNode.
        /// </summary>
        /// <param name="nodeIn">The node to visit.</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(NamedFunctionParameterNode nodeIn)
        {
            validate(nodeIn);
            ValidateNode(nodeIn.Value);
            return true;
        }

        /// <summary>
        /// Visit a ParameterAliasNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override bool Visit(ParameterAliasNode nodeIn)
        {
            // todo: do we even need to call for a parameter alias?
            validate(nodeIn);
            return true;
        }

        /// <summary>
        /// Visit a SearchTermNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override bool Visit(SearchTermNode nodeIn)
        {
            validate(nodeIn);
            return true;
        }

        /// <summary>
        /// Visit a SingleComplexNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(SingleComplexNode nodeIn)
        {
            validate(nodeIn);
            validate(nodeIn.Property);
            validate(nodeIn.TypeReference.Definition);
            return true;
        }

        /// <summary>
        /// Visit a CollectionComplexNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(CollectionComplexNode nodeIn)
        {
            validate(nodeIn);
            validate(nodeIn.Property);
            validate(nodeIn.CollectionType.CollectionDefinition());
            validate(nodeIn.ItemType.Definition);
            return true;
        }

        /// <summary>
        /// Visit a SingleValueCastNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(SingleValueCastNode nodeIn)
        {
            validate(nodeIn);
            validate(nodeIn.TypeReference.Definition);
            return true;
        }

        /// <summary>
        /// Visit an AggregatedCollectionPropertyNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(AggregatedCollectionPropertyNode nodeIn)
        {
            validate(nodeIn);
            validate(nodeIn.Property);
            validate(nodeIn.TypeReference.Definition);
            return true;
        }

        /// <summary>
        /// Visit an InNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>true, indicating that the node has been visited.</returns>
        public override bool Visit(InNode nodeIn)
        {
            validate(nodeIn);
            ValidateNode(nodeIn.Left);
            ValidateNode(nodeIn.Right);
            return true;
        }
    }
}
