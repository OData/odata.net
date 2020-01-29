//---------------------------------------------------------------------
// <copyright file="EdmLabeledExpressionReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm.Vocabularies
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
