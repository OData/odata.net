//---------------------------------------------------------------------
// <copyright file="EdmLabeledExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
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
