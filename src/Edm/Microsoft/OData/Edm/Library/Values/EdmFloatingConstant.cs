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

using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Library.Values
{
    /// <summary>
    /// Represents an EDM floating point constant.
    /// </summary>
    public class EdmFloatingConstant : EdmValue, IEdmFloatingConstantExpression
    {
        private readonly double value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmFloatingConstant"/> class.
        /// </summary>
        /// <param name="value">Floating point value represented by this value.</param>
        public EdmFloatingConstant(double value)
            : this(null, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmFloatingConstant"/> class.
        /// </summary>
        /// <param name="type">Type of the floating point.</param>
        /// <param name="value">Floating point value represented by this value.</param>
        public EdmFloatingConstant(IEdmPrimitiveTypeReference type, double value)
            : base(type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the definition of this value.
        /// </summary>
        public double Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.FloatingConstant; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.Floating; }
        }
    }
}
