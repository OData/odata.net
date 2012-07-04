//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Values;

namespace Microsoft.Data.Edm.Library.Values
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
