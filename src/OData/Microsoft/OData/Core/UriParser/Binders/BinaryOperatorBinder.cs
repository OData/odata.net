//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

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
            this.resolver = resolver ?? ODataUriResolver.Default;
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
        internal static void PromoteOperandTypes(BinaryOperatorKind binaryOperatorKind, ref SingleValueNode left, ref SingleValueNode right)
        {
            IEdmTypeReference leftType;
            IEdmTypeReference rightType;
            if (!TypePromotionUtils.PromoteOperandTypes(binaryOperatorKind, left, right, out leftType, out rightType))
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
