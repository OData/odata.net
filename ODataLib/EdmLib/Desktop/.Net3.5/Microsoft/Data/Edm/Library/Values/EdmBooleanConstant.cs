//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Values;

namespace Microsoft.Data.Edm.Library.Values
{
    /// <summary>
    /// Represents an EDM boolean constant.
    /// </summary>
    public class EdmBooleanConstant : EdmValue, IEdmBooleanConstantExpression
    {
        private readonly bool value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmBooleanConstant"/> class.
        /// </summary>
        /// <param name="value">Boolean value represented by this value.</param>
        public EdmBooleanConstant(bool value)
            : this(null, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmBooleanConstant"/> class.
        /// </summary>
        /// <param name="type">Type of the boolean.</param>
        /// <param name="value">Boolean value represented by this value.</param>
        public EdmBooleanConstant(IEdmPrimitiveTypeReference type, bool value)
            : base(type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets a value indicating whether the value of this boolean value is true or false.
        /// </summary>
        public bool Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.BooleanConstant; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.Boolean; }
        }
    }
}
