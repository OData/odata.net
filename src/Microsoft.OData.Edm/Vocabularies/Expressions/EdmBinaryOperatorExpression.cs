//---------------------------------------------------------------------
// <copyright file="EdmBinaryOperatorExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM binary operator expression.
    /// </summary>
    public class EdmBinaryOperatorExpression : EdmElement, IEdmBinaryOperatorExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmBinaryOperatorExpression"/> class.
        /// </summary>
        /// <param name="left">The Left expression operand.</param>
        /// <param name="right">The Right expression operand.</param>
        /// <param name="kind">The binary operator kind.</param>
        public EdmBinaryOperatorExpression(IEdmExpression left, IEdmExpression right, EdmBinaryOperatorKind kind)
        {
            EdmUtil.CheckArgumentNull(left, "left");
            EdmUtil.CheckArgumentNull(right, "right");

            Left = left;
            Right = right;
            Kind = kind;
        }

        /// <summary>
        /// Gets the left expression.
        /// </summary>
        public IEdmExpression Left { get; }

        /// <summary>
        /// Gets the right expression.
        /// </summary>
        public IEdmExpression Right { get; }

        /// <summary>
        /// Gets the binary operator kind.
        /// </summary>
        public EdmBinaryOperatorKind Kind { get; }

        /// <summary>
        /// Gets the expression kind.
        /// </summary>
        public EdmExpressionKind ExpressionKind => EdmExpressionKind.BinaryOperator;
    }
}
