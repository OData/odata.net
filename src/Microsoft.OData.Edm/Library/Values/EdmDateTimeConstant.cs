//---------------------------------------------------------------------
// <copyright file="EdmDateTimeConstant.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Library.Values
{
    /// <summary>
    /// Represents an EDM datetime constant.
    /// </summary>
    public class EdmDateTimeConstant : EdmValue, IEdmDateTimeConstantExpression
    {
        private readonly DateTime value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDateTimeConstant"/> class.
        /// </summary>
        /// <param name="value">DateTime value represented by this value.</param>
        public EdmDateTimeConstant(DateTime value)
            : this(null, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDateTimeConstant"/> class.
        /// </summary>
        /// <param name="type">Type of the DateTime.</param>
        /// <param name="value">DateTime value represented by this value.</param>
        public EdmDateTimeConstant(IEdmTemporalTypeReference type, DateTime value)
            : base(type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the definition of this value.
        /// </summary>
        public DateTime Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.DateTimeConstant; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.DateTime; }
        }
    }
}
