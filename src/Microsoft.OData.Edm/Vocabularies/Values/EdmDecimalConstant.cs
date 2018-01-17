//---------------------------------------------------------------------
// <copyright file="EdmDecimalConstant.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM decimal constant.
    /// </summary>
    public class EdmDecimalConstant : EdmValue, IEdmDecimalConstantExpression
    {
        private readonly decimal value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDecimalConstant"/> class.
        /// </summary>
        /// <param name="value">Decimal value represented by this value.</param>
        public EdmDecimalConstant(decimal value)
            : this(null, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDecimalConstant"/> class.
        /// </summary>
        /// <param name="type">Type of the decimal.</param>
        /// <param name="value">Decimal value represented by this value.</param>
        public EdmDecimalConstant(IEdmDecimalTypeReference type, decimal value)
            : base(type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the definition of this value.
        /// </summary>
        public decimal Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.DecimalConstant; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.Decimal; }
        }
    }
}
