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
    /// Represents an EDM labeled expression.
    /// </summary>
    public class EdmLabeledExpression : EdmElement, IEdmLabeledExpression
    {
        private readonly string name;
        private readonly IEdmExpression expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmLabeledExpression"/> class.
        /// </summary>
        /// <param name="name">Label of the expression.</param>
        /// <param name="expression">Underlying expression.</param>
        public EdmLabeledExpression(string name, IEdmExpression expression)
        {
            EdmUtil.CheckArgumentNull(name, "name");
            EdmUtil.CheckArgumentNull(expression, "expression");

            this.name = name;
            this.expression = expression;
        }

        /// <summary>
        /// Gets the label.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the underlying expression.
        /// </summary>
        public IEdmExpression Expression
        {
            get { return this.expression; }
        }

        /// <summary>
        /// Gets the expression kind.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Labeled; }
        }
    }
}
