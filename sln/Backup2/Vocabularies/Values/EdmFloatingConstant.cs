//---------------------------------------------------------------------
// <copyright file="EdmFloatingConstant.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM floating point constant.
    /// </summary>
    public class EdmFloatingConstant : EdmValue, IEdmFloatingConstantExpression
    {
        private readonly double value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmFloatingConstant"/> class.
        /// </summary>
        /// <param name="value">Floating point value represented by this value.</param>
        public EdmFloatingConstant(double value)
            : this(null, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmFloatingConstant"/> class.
        /// </summary>
        /// <param name="type">Type of the floating point.</param>
        /// <param name="value">Floating point value represented by this value.</param>
        public EdmFloatingConstant(IEdmPrimitiveTypeReference type, double value)
            : base(type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the definition of this value.
        /// </summary>
        public double Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.FloatingConstant; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.Floating; }
        }
    }
}
