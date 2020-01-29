//---------------------------------------------------------------------
// <copyright file="EdmDurationConstant.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM duration constant.
    /// </summary>
    public class EdmDurationConstant : EdmValue, IEdmDurationConstantExpression
    {
        private readonly TimeSpan value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDurationConstant"/> class.
        /// </summary>
        /// <param name="value">Duration value represented by this value.</param>
        public EdmDurationConstant(TimeSpan value)
            : this(null, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDurationConstant"/> class.
        /// </summary>
        /// <param name="type">Type of the Duration.</param>
        /// <param name="value">Duration value represented by this value.</param>
        public EdmDurationConstant(IEdmTemporalTypeReference type, TimeSpan value)
            : base(type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the definition of this value.
        /// </summary>
        public TimeSpan Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.DurationConstant; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.Duration; }
        }
    }
}
