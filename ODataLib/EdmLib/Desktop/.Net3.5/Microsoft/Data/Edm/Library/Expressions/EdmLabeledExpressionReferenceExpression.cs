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

using System;
using Microsoft.Data.Edm.Expressions;

namespace Microsoft.Data.Edm.Library.Expressions
{
    /// <summary>
    /// Represents an EDM labeled expression reference expression.
    /// </summary>
    public class EdmLabeledExpressionReferenceExpression : EdmElement, IEdmLabeledExpressionReferenceExpression
    {
        private IEdmLabeledExpression referencedLabeledExpression;

        /// <summary>
        /// Initializes a new instance of <see cref="EdmLabeledExpressionReferenceExpression"/> class with non-initialized <see cref="ReferencedLabeledExpression"/> property.
        /// </summary>
        public EdmLabeledExpressionReferenceExpression()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmLabeledExpressionReferenceExpression"/> class.
        /// This constructor will not allow changing <see cref="ReferencedLabeledExpression"/> property after the EdmLabeledExpressionReferenceExpression instance has been constructed.
        /// </summary>
        /// <param name="referencedLabeledExpression">Referenced labeled element.</param>
        public EdmLabeledExpressionReferenceExpression(IEdmLabeledExpression referencedLabeledExpression)
        {
            EdmUtil.CheckArgumentNull(referencedLabeledExpression, "referencedLabeledExpression");
            this.referencedLabeledExpression = referencedLabeledExpression;
        }

        /// <summary>
        /// Gets or sets the referenced labeled element.
        /// The referenced labeled element can be initialized only once either using the <see cref="EdmLabeledExpressionReferenceExpression(IEdmLabeledExpression)"/> constructor or by assigning value directly to this property.
        /// </summary>
        public IEdmLabeledExpression ReferencedLabeledExpression
        {
            get
            {
                return this.referencedLabeledExpression;
            }

            set
            {
                EdmUtil.CheckArgumentNull(value, "value");

                if (this.referencedLabeledExpression != null)
                {
                    throw new InvalidOperationException(Strings.ValueHasAlreadyBeenSet);
                }

                this.referencedLabeledExpression = value;
            }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.LabeledExpressionReference; }
        }
    }
}
