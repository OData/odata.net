//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library.Expressions
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
