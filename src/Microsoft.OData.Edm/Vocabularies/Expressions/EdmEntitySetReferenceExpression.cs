//---------------------------------------------------------------------
// <copyright file="EdmEntitySetReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Library;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM entity set reference expression.
    /// </summary>
    public class EdmEntitySetReferenceExpression : EdmElement, IEdmEntitySetReferenceExpression
    {
        private readonly IEdmEntitySet referencedEntitySet;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEntitySetReferenceExpression"/> class.
        /// </summary>
        /// <param name="referencedEntitySet">Referenced entity set.</param>
        public EdmEntitySetReferenceExpression(IEdmEntitySet referencedEntitySet)
        {
            EdmUtil.CheckArgumentNull(referencedEntitySet, "referencedEntitySet");
            this.referencedEntitySet = referencedEntitySet;
        }

        /// <summary>
        /// Gets the referenced entity set.
        /// </summary>
        public IEdmEntitySet ReferencedEntitySet
        {
            get { return this.referencedEntitySet; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.EntitySetReference; }
        }
    }
}
