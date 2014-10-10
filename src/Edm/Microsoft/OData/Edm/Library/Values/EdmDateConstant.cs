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
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Library.Values
{
    /// <summary>
    /// Represents an EDM date constant.
    /// </summary>
    public class EdmDateConstant : EdmValue, IEdmDateConstantExpression
    {
        private readonly Date value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDateConstant"/> class.
        /// </summary>
        /// <param name="value">Date value represented by this value.</param>
        public EdmDateConstant(Date value)
            : this(null, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDateConstant"/> class.
        /// </summary>
        /// <param name="type">Type of the Date.</param>
        /// <param name="value">Date value represented by this value.</param>
        public EdmDateConstant(IEdmPrimitiveTypeReference type, Date value)
            : base(type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the content of this value.
        /// </summary>
        public Date Value
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
