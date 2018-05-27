//---------------------------------------------------------------------
// <copyright file="BinaryOperatorBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Class that knows how to bind binary operators.
    /// </summary>
    internal sealed class BinaryOperatorBinder
    {
        /// <summary>
        /// Method to use for binding the parent node, if needed.
        /// </summary>
        private readonly Func<QueryToken, QueryNode> bindMethod;

        /// <summary>
        /// Resolver for parsing
        /// </summary>
        private readonly ODataUriResolver resolver;

        /// <summary>
        /// Constructs a BinaryOperatorBinder with the given method to be used binding the parent token if needed.
        /// </summary>
        /// <param name="bindMethod">Method to use for binding the parent token, if needed.</param>
        /// <param name="resolver">Resolver for parsing.</param>
        internal BinaryOperatorBinder(Func<QueryToken, QueryNode> bindMethod, ODataUriResolver resolver)
        {
            this.bindMethod = bindMethod;
            this.resolver = resolver;
        }

        /// <summary>
        /// Binds a binary operator token.
        /// </summary>
        /// <param name="binaryOperatorToken">The binary operator token to bind.</param>
        /// <returns>The bound binary operator token.</returns>
        internal QueryNode BindBinaryOperator(BinaryOperatorToken binaryOperatorToken)
        {
            ExceptionUtils.CheckArgumentNotNull(binaryOperatorToken, "binaryOperatorToken");

            SingleValueNode left = this.GetOperandFromToken(binaryOperatorToken.OperatorKind, binaryOperatorToken.Left);
            SingleValueNode right = this.GetOperandFromToken(binaryOperatorToken.OperatorKind, binaryOperatorToken.Right);

            IEdmTypeReference typeReference;
            this.resolver.PromoteBinaryOperandTypes(binaryOperatorToken.OperatorKind, ref left, ref right, out typeReference);

            return new BinaryOperatorNode(binaryOperatorToken.OperatorKind, left, right, typeReference);
        }

        /// <summary>
        /// Promote the left and right operand types
        /// </summary>
        /// <param name="binaryOperatorKind">the operator kind</param>
        /// <param name="left">the left operand</param>
        /// <param name="right">the right operand</param>
        /// <param name="facetsPromotionRules">Promotion rules for type facets.</param>
        internal static void PromoteOperandTypes(BinaryOperatorKind binaryOperatorKind, ref SingleValueNode left, ref SingleValueNode right, TypeFacetsPromotionRules facetsPromotionRules)
        {
            IEdmTypeReference leftType;
            IEdmTypeReference rightType;
            if (!TypePromotionUtils.PromoteOperandTypes(binaryOperatorKind, left, right, out leftType, out rightType, facetsPromotionRules))
            {
                string leftTypeName = left.TypeReference == null ? "<null>" : left.TypeReference.FullName();
                string rightTypeName = right.TypeReference == null ? "<null>" : right.TypeReference.FullName();
                throw new ODataException(ODataErrorStrings.MetadataBinder_IncompatibleOperandsError(leftTypeName, rightTypeName, binaryOperatorKind));
            }

            left = MetadataBindingUtils.ConvertToTypeIfNeeded(left, leftType);
            right = MetadataBindingUtils.ConvertToTypeIfNeeded(right, rightType);
        }

        /// <summary>
        /// Retrieve SingleValueNode bound with given query token.
        /// </summary>
        /// <param name="operatorKind">the query token kind</param>
        /// <param name="queryToken">the query token</param>
        /// <returns>the corresponding SingleValueNode</returns>
        private SingleValueNode GetOperandFromToken(BinaryOperatorKind operatorKind, QueryToken queryToken)
        {
            SingleValueNode operand = this.bindMethod(queryToken) as SingleValueNode;
            if (operand == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_BinaryOperatorOperandNotSingleValue(operatorKind.ToString()));
            }

            return operand;
        }
    }
}
