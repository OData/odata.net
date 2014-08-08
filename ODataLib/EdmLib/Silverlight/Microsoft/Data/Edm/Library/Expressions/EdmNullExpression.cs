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
    /// Represents an EDM null.
    /// </summary>
    public class EdmNullExpression : EdmValue, IEdmNullExpression
    {
        /// <summary>
        /// Singleton <see cref="EdmNullExpression"/> instance.
        /// </summary>
        public static EdmNullExpression Instance = new EdmNullExpression();

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmNullExpression"/> class.
        /// </summary>
        private EdmNullExpression()
            : base(null)
        {
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Null; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.Null; }
        }
    }
}
