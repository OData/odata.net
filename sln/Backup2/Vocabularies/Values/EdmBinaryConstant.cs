//---------------------------------------------------------------------
// <copyright file="EdmBinaryConstant.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM binary constant.
    /// </summary>
    public class EdmBinaryConstant : EdmValue, IEdmBinaryConstantExpression
    {
        private readonly byte[] value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmBinaryConstant"/> class.
        /// </summary>
        /// <param name="value">Integer value represented by this value.</param>
        public EdmBinaryConstant(byte[] value)
            : this(null, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmBinaryConstant"/> class.
        /// </summary>
        /// <param name="type">Type of the integer.</param>
        /// <param name="value">Integer value represented by this value.</param>
        public EdmBinaryConstant(IEdmBinaryTypeReference type, byte[] value)
            : base(type)
        {
            EdmUtil.CheckArgumentNull(value, "value");
            this.value = value;
        }

        /// <summary>
        /// Gets the definition of this value.
        /// </summary>
        public byte[] Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.BinaryConstant; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.Binary; }
        }
    }
}
