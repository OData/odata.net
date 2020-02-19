//---------------------------------------------------------------------
// <copyright file="EdmIntegerConstant.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM integer constant.
    /// </summary>
    public class EdmIntegerConstant : EdmValue, IEdmIntegerConstantExpression
    {
        private readonly Int64 value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmIntegerConstant"/> class.
        /// </summary>
        /// <param name="value">Integer value represented by this value.</param>
        public EdmIntegerConstant(Int64 value)
            : this(null, value)
        {
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmIntegerConstant"/> class.
        /// </summary>
        /// <param name="type">Type of the integer.</param>
        /// <param name="value">Integer value represented by this value.</param>
        public EdmIntegerConstant(IEdmPrimitiveTypeReference type, Int64 value)
            : base(type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the definition of this value.
        /// </summary>
        public Int64 Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.IntegerConstant; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.Integer; }
        }
    }
}
