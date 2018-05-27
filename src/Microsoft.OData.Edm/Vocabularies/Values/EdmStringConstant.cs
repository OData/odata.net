//---------------------------------------------------------------------
// <copyright file="EdmStringConstant.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM string constant.
    /// </summary>
    public class EdmStringConstant : EdmValue, IEdmStringConstantExpression
    {
        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmStringConstant"/> class.
        /// </summary>
        /// <param name="value">String value represented by this value.</param>
        public EdmStringConstant(string value)
            : this(null, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmStringConstant"/> class.
        /// </summary>
        /// <param name="type">Type of the string.</param>
        /// <param name="value">String value represented by this value.</param>
        public EdmStringConstant(IEdmStringTypeReference type, string value)
            : base(type)
        {
            EdmUtil.CheckArgumentNull(value, "value");
            this.value = value;
        }

        /// <summary>
        /// Gets the definition of this value.
        /// </summary>
        public string Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.StringConstant; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.String; }
        }
    }
}
