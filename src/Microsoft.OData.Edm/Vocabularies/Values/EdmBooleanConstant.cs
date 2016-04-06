//---------------------------------------------------------------------
// <copyright file="EdmBooleanConstant.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM boolean constant.
    /// </summary>
    public class EdmBooleanConstant : EdmValue, IEdmBooleanConstantExpression
    {
        private readonly bool value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmBooleanConstant"/> class.
        /// </summary>
        /// <param name="value">Boolean value represented by this value.</param>
        public EdmBooleanConstant(bool value)
            : this(null, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmBooleanConstant"/> class.
        /// </summary>
        /// <param name="type">Type of the boolean.</param>
        /// <param name="value">Boolean value represented by this value.</param>
        public EdmBooleanConstant(IEdmPrimitiveTypeReference type, bool value)
            : base(type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets a value indicating whether the value of this boolean value is true or false.
        /// </summary>
        public bool Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.BooleanConstant; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.Boolean; }
        }
    }
}
