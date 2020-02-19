//---------------------------------------------------------------------
// <copyright file="EdmCastExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM type assertion expression.
    /// </summary>
    public class EdmCastExpression : EdmElement, IEdmCastExpression
    {
        private readonly IEdmExpression operand;
        private readonly IEdmTypeReference type;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmCastExpression"/> class.
        /// </summary>
        /// <param name="operand">Expression for which the type is casted.</param>
        /// <param name="type">Type to cast.</param>
        public EdmCastExpression(IEdmExpression operand, IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(operand, "operand");
            EdmUtil.CheckArgumentNull(type, "type");

            this.operand = operand;
            this.type = type;
        }

        /// <summary>
        /// Gets the expression for which the type is asserted.
        /// </summary>
        public IEdmExpression Operand
        {
            get { return this.operand; }
        }

        /// <summary>
        /// Gets the asserted type.
        /// </summary>
        public IEdmTypeReference Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Cast; }
        }
    }
}
