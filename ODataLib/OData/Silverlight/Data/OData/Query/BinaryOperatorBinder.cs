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
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

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
        /// Constructs a BinaryOperatorBinder with the given method to be used binding the parent token if needed.
        /// </summary>
        /// <param name="bindMethod">Method to use for binding the parent token, if needed.</param>
        internal BinaryOperatorBinder(Func<QueryToken, QueryNode> bindMethod)
        {
            DebugUtils.CheckNoExternalCallers();
            this.bindMethod = bindMethod;
        }

        /// <summary>
        /// Binds a binary operator token.
        /// </summary>
        /// <param name="binaryOperatorToken">The binary operator token to bind.</param>
        /// <returns>The bound binary operator token.</returns>
        internal QueryNode BindBinaryOperator(BinaryOperatorToken binaryOperatorToken)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(binaryOperatorToken, "binaryOperatorToken");

            SingleValueNode left = this.GetOperandFromToken(binaryOperatorToken.OperatorKind, binaryOperatorToken.Left);
            SingleValueNode right = this.GetOperandFromToken(binaryOperatorToken.OperatorKind, binaryOperatorToken.Right);

            BinaryOperatorBinder.PromoteOperandTypes(binaryOperatorToken.OperatorKind, ref left, ref right);

            return new BinaryOperatorNode(binaryOperatorToken.OperatorKind, left, right);
        }

        /// <summary>
        /// Promote the left and right operand types
        /// </summary>
        /// <param name="binaryOperatorKind">the operator kind</param>
        /// <param name="left">the left operand</param>
        /// <param name="right">the right operand</param>
        private static void PromoteOperandTypes(BinaryOperatorKind binaryOperatorKind, ref SingleValueNode left, ref SingleValueNode right)
        {
            IEdmTypeReference leftType = left.TypeReference;
            IEdmTypeReference rightType = right.TypeReference;
            if (!TypePromotionUtils.PromoteOperandTypes(binaryOperatorKind, ref leftType, ref rightType))
            {
                string leftTypeName = left.TypeReference == null ? "<null>" : left.TypeReference.ODataFullName();
                string rightTypeName = right.TypeReference == null ? "<null>" : right.TypeReference.ODataFullName();
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
