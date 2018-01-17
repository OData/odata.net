//---------------------------------------------------------------------
// <copyright file="EdmGuidConstant.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM guid constant.
    /// </summary>
    public class EdmGuidConstant : EdmValue, IEdmGuidConstantExpression
    {
        private readonly Guid value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmGuidConstant"/> class.
        /// </summary>
        /// <param name="value">Integer value represented by this value.</param>
        public EdmGuidConstant(Guid value)
            : this(null, value)
        {
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmGuidConstant"/> class.
        /// </summary>
        /// <param name="type">Type of the integer.</param>
        /// <param name="value">Integer value represented by this value.</param>
        public EdmGuidConstant(IEdmPrimitiveTypeReference type, Guid value)
            : base(type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the definition of this value.
        /// </summary>
        public Guid Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.GuidConstant; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.Guid; }
        }
    }
}
