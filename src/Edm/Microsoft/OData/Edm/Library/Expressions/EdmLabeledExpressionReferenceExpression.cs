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

namespace Microsoft.OData.Edm.Library.Expressions
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
