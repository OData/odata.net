//   OData .NET Libraries ver. 6.8.1
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

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Class that knows how to bind unary operators.
    /// </summary>
    internal sealed class UnaryOperatorBinder
    {
        /// <summary>
        /// Method to use for binding the parent node, if needed.
        /// </summary>
        private readonly Func<QueryToken, QueryNode> bindMethod;

        /// <summary>
        /// Constructs a UnaryOperatorBinder with the given method to be used binding the parent token if needed.
        /// </summary>
        /// <param name="bindMethod">Method to use for binding the parent token, if needed.</param>
        internal UnaryOperatorBinder(Func<QueryToken, QueryNode> bindMethod)
        {
            this.bindMethod = bindMethod;
        }

        /// <summary>
        /// Binds a unary operator token.
        /// </summary>
        /// <param name="unaryOperatorToken">The unary operator token to bind.</param>
        /// <returns>The bound unary operator token.</returns>
        internal QueryNode BindUnaryOperator(UnaryOperatorToken unaryOperatorToken)
        {
            ExceptionUtils.CheckArgumentNotNull(unaryOperatorToken, "unaryOperatorToken");

            SingleValueNode operand = this.GetOperandFromToken(unaryOperatorToken);

            IEdmTypeReference typeReference = UnaryOperatorBinder.PromoteOperandType(operand, unaryOperatorToken.OperatorKind);
            Debug.Assert(typeReference == null || typeReference.IsODataPrimitiveTypeKind(), "Only primitive types should be able to get here.");
            operand = MetadataBindingUtils.ConvertToTypeIfNeeded(operand, typeReference);

            return new UnaryOperatorNode(unaryOperatorToken.OperatorKind, operand);
        }

        /// <summary>
        /// Get the promoted type reference of the operand
        /// </summary>
        /// <param name="operand">the operand</param>
        /// <param name="unaryOperatorKind">the operator kind</param>
        /// <returns>the type reference of the operand</returns>
        private static IEdmTypeReference PromoteOperandType(SingleValueNode operand, UnaryOperatorKind unaryOperatorKind)
        {
            IEdmTypeReference typeReference = operand.TypeReference;
            if (!TypePromotionUtils.PromoteOperandType(unaryOperatorKind, ref typeReference))
            {
                string typeName = operand.TypeReference == null ? "<null>" : operand.TypeReference.ODataFullName();
                throw new ODataException(ODataErrorStrings.MetadataBinder_IncompatibleOperandError(typeName, unaryOperatorKind));
            }

            return typeReference;
        }

        /// <summary>
        /// Retrieve SingleValueNode operand from given token.
        /// </summary>
        /// <param name="unaryOperatorToken">The token</param>
        /// <returns>the SingleValueNode operand</returns>
        private SingleValueNode GetOperandFromToken(UnaryOperatorToken unaryOperatorToken)
        {
            SingleValueNode operand = this.bindMethod(unaryOperatorToken.Operand) as SingleValueNode;
            if (operand == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_UnaryOperatorOperandNotSingleValue(unaryOperatorToken.OperatorKind.ToString()));
            }

            return operand;
        }
    }
}
