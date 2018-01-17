//---------------------------------------------------------------------
// <copyright file="EdmTimeOfDayConstant.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM TimeOfDay constant.
    /// </summary>
    public class EdmTimeOfDayConstant : EdmValue, IEdmTimeOfDayConstantExpression
    {
        private readonly TimeOfDay value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTimeOfDayConstant"/> class.
        /// </summary>
        /// <param name="value">TimeOfDay value represented by this value.</param>
        public EdmTimeOfDayConstant(TimeOfDay value)
            : this(null, value)
        {
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTimeOfDayConstant"/> class.
        /// </summary>
        /// <param name="type">Type of the TimeOfDay.</param>
        /// <param name="value">TimeOfDay value represented by this value.</param>
        public EdmTimeOfDayConstant(IEdmTemporalTypeReference type, TimeOfDay value)
            : base(type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the definition of this value.
        /// </summary>
        public TimeOfDay Value
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
