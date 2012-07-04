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

using Microsoft.Data.Edm.Expressions;

namespace Microsoft.Data.Edm.Library.Expressions
{
    /// <summary>
    /// Represents an EDM if expression.
    /// </summary>
    public class EdmIfExpression : EdmElement, IEdmIfExpression
    {
        private readonly IEdmExpression testExpression;
        private readonly IEdmExpression trueExpression;
        private readonly IEdmExpression falseExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmIfExpression"/> class.
        /// </summary>
        /// <param name="testExpression">Test expression</param>
        /// <param name="trueExpression">Expression to evaluate if <paramref name="testExpression"/> evaluates to true.</param>
        /// <param name="falseExpression">Expression to evaluate if <paramref name="testExpression"/> evaluates to false.</param>
        public EdmIfExpression(IEdmExpression testExpression, IEdmExpression trueExpression, IEdmExpression falseExpression)
        {
            EdmUtil.CheckArgumentNull(testExpression, "testExpression");
            EdmUtil.CheckArgumentNull(trueExpression, "trueExpression");
            EdmUtil.CheckArgumentNull(falseExpression, "falseExpression");

            this.testExpression = testExpression;
            this.trueExpression = trueExpression;
            this.falseExpression = falseExpression;
        }

        /// <summary>
        /// Gets the test expression.
        /// </summary>
        public IEdmExpression TestExpression
        {
            get { return this.testExpression; }
        }

        /// <summary>
        /// Gets the expression to evaluate if <see cref="TestExpression"/> evaluates to true.
        /// </summary>
        public IEdmExpression TrueExpression
        {
            get { return this.trueExpression; }
        }

        /// <summary>
        /// Gets the expression to evaluate if <see cref="TestExpression"/> evaluates to false.
        /// </summary>
        public IEdmExpression FalseExpression
        {
            get { return this.falseExpression; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.If; }
        }
    }
}
