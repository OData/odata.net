//---------------------------------------------------------------------
// <copyright file="EdmTimeOfDayConstant.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM TimeOnly constant.
    /// </summary>
    public class EdmTimeOfDayConstant : EdmValue, IEdmTimeOfDayConstantExpression
    {
        private readonly TimeOnly value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTimeOfDayConstant"/> class.
        /// </summary>
        /// <param name="value">TimeOnly value represented by this value.</param>
        public EdmTimeOfDayConstant(TimeOnly value)
            : this(null, value)
        {
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTimeOfDayConstant"/> class.
        /// </summary>
        /// <param name="type">Type of the TimeOnly.</param>
        /// <param name="value">TimeOnly value represented by this value.</param>
        public EdmTimeOfDayConstant(IEdmTemporalTypeReference type, TimeOnly value)
            : base(type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the definition of this value.
        /// </summary>
        public TimeOnly Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.TimeOfDayConstant; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.TimeOfDay; }
        }
    }
}
