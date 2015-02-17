//---------------------------------------------------------------------
// <copyright file="EdmValueTermReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library.Expressions
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
