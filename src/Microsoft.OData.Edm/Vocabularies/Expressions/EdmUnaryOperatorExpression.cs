//---------------------------------------------------------------------
// <copyright file="EdmUnaryOperatorExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM unary operator expression.
    /// </summary>
    public class EdmUnaryOperatorExpression : EdmElement, IEdmUnaryOperatorExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmUnaryOperatorExpression"/> class.
        /// </summary>
        /// <param name="operand">Expression for which the type is casted.</param>
        /// <param name="kind">The unary operator kind.</param>
        public EdmUnaryOperatorExpression(IEdmExpression operand, EdmUnaryOperatorKind kind)
        {
            EdmUtil.CheckArgumentNull(operand, "operand");

            Operand = operand;
            Kind = kind;
        }

        /// <summary>
        /// Gets the expression represented by this expression.
        /// </summary>
        public IEdmExpression Operand { get; }

        /// <summary>
        /// Gets the unary operator kind represented by this expression.
        /// </summary>
        public EdmUnaryOperatorKind Kind { get; }

        /// <summary>
        /// Gets the expression kind.
        /// </summary>
        public EdmExpressionKind ExpressionKind => EdmExpressionKind.UnaryOperator;
    }
}
