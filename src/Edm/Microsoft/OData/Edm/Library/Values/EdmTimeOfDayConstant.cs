//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Library.Values
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
