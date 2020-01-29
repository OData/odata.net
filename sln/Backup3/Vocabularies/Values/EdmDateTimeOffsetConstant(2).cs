//---------------------------------------------------------------------
// <copyright file="EdmDateTimeOffsetConstant.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM datetime with offset constant.
    /// </summary>
    public class EdmDateTimeOffsetConstant : EdmValue, IEdmDateTimeOffsetConstantExpression
    {
        private readonly DateTimeOffset value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDateTimeOffsetConstant"/> class.
        /// </summary>
        /// <param name="value">DateTimeOffset value represented by this value.</param>
        public EdmDateTimeOffsetConstant(DateTimeOffset value)
            : this(null, value)
        {
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDateTimeOffsetConstant"/> class.
        /// </summary>
        /// <param name="type">Type of the DateTimeOffset.</param>
        /// <param name="value">DateTimeOffset value represented by this value.</param>
        public EdmDateTimeOffsetConstant(IEdmTemporalTypeReference type, DateTimeOffset value)
            : base(type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the definition of this value.
        /// </summary>
        public DateTimeOffset Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.DateTimeOffsetConstant; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.DateTimeOffset; }
        }
    }
}
