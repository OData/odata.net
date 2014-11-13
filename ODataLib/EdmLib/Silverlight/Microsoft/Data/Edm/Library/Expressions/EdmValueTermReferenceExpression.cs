//   OData .NET Libraries ver. 5.6.3
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

using Microsoft.Data.Edm.Expressions;

namespace Microsoft.Data.Edm.Library.Expressions
{
    /// <summary>
    /// Represents an EDM value term reference expression.
    /// </summary>
    public class EdmValueTermReferenceExpression : EdmElement, IEdmValueTermReferenceExpression
    {
        private readonly IEdmExpression baseExpression;
        private readonly IEdmValueTerm term;
        private readonly string qualifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmValueTermReferenceExpression"/> class.
        /// </summary>
        /// <param name="baseExpression">Expression for the structured value containing the referenced term property.</param>
        /// <param name="term">Referenced value term.</param>
        public EdmValueTermReferenceExpression(IEdmExpression baseExpression, IEdmValueTerm term)
            : this(baseExpression, term, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmValueTermReferenceExpression"/> class.
        /// </summary>
        /// <param name="baseExpression">Expression for the structured value containing the referenced term property.</param>
        /// <param name="term">Referenced value term.</param>
        /// <param name="qualifier">Qualifier.</param>
        public EdmValueTermReferenceExpression(IEdmExpression baseExpression, IEdmValueTerm term, string qualifier)
        {
            EdmUtil.CheckArgumentNull(baseExpression, "baseExpression");
            EdmUtil.CheckArgumentNull(term, "term");

            this.baseExpression = baseExpression;
            this.term = term;
            this.qualifier = qualifier;
        }

        /// <summary>
        /// Gets the expression for the structured value containing the referenced term property.
        /// </summary>
        public IEdmExpression Base
        {
            get { return this.baseExpression; }
        }

        /// <summary>
        /// Gets the referenced value term.
        /// </summary>
        public IEdmValueTerm Term
        {
            get { return this.term; }
        }

        /// <summary>
        /// Gets the optional qualifier.
        /// </summary>
        public string Qualifier
        {
            get { return this.qualifier; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.ValueTermReference; }
        }
    }
}
