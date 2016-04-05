//---------------------------------------------------------------------
// <copyright file="EdmPropertyReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Library;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM property reference expression.
    /// </summary>
    public class EdmPropertyReferenceExpression : EdmElement, IEdmPropertyReferenceExpression
    {
        private readonly IEdmExpression baseExpression;
        private readonly IEdmProperty referencedProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyReferenceExpression"/> class.
        /// </summary>
        /// <param name="baseExpression">Expression for the structured value containing the referenced property.</param>
        /// <param name="referencedProperty">Referenced property.</param>
        public EdmPropertyReferenceExpression(IEdmExpression baseExpression, IEdmProperty referencedProperty)
        {
            EdmUtil.CheckArgumentNull(baseExpression, "baseExpression");
            EdmUtil.CheckArgumentNull(referencedProperty, "referencedPropert");

            this.baseExpression = baseExpression;
            this.referencedProperty = referencedProperty;
        }

        /// <summary>
        /// Gets the expression for the structured value containing the referenced property.
        /// </summary>
        public IEdmExpression Base
        {
            get { return this.baseExpression; }
        }

        /// <summary>
        /// Gets the referenced property.
        /// </summary>
        public IEdmProperty ReferencedProperty
        {
            get { return this.referencedProperty; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.PropertyReference; }
        }
    }
}
