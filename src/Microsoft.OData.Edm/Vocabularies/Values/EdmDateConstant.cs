//---------------------------------------------------------------------
// <copyright file="EdmDateConstant.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM date constant.
    /// </summary>
    public class EdmDateConstant : EdmValue, IEdmDateConstantExpression
    {
        private readonly DateOnly value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDateConstant"/> class.
        /// </summary>
        /// <param name="value">Date value represented by this value.</param>
        public EdmDateConstant(DateOnly value)
            : this(null, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDateConstant"/> class.
        /// </summary>
        /// <param name="type">Type of the Date.</param>
        /// <param name="value">Date value represented by this value.</param>
        public EdmDateConstant(IEdmPrimitiveTypeReference type, DateOnly value)
            : base(type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the content of this value.
        /// </summary>
        public DateOnly Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.DateConstant; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.Date; }
        }
    }
}
