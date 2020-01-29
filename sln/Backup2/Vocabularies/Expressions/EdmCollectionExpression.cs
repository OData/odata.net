//---------------------------------------------------------------------
// <copyright file="EdmCollectionExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM multi-value construction expression.
    /// </summary>
    public class EdmCollectionExpression : EdmElement, IEdmCollectionExpression
    {
        private readonly IEdmTypeReference declaredType;
        private readonly IEnumerable<IEdmExpression> elements;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmCollectionExpression"/> class.
        /// </summary>
        /// <param name="elements">The constructed element values.</param>
        public EdmCollectionExpression(params IEdmExpression[] elements)
            : this((IEnumerable<IEdmExpression>)elements)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmCollectionExpression"/> class.
        /// </summary>
        /// <param name="declaredType">Declared type of the collection.</param>
        /// <param name="elements">The constructed element values.</param>
        public EdmCollectionExpression(IEdmTypeReference declaredType, params IEdmExpression[] elements)
            : this(declaredType, (IEnumerable<IEdmExpression>)elements)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmCollectionExpression"/> class.
        /// </summary>
        /// <param name="elements">The constructed element values.</param>
        public EdmCollectionExpression(IEnumerable<IEdmExpression> elements)
            : this(null, elements)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmCollectionExpression"/> class.
        /// </summary>
        /// <param name="declaredType">Declared type of the collection.</param>
        /// <param name="elements">The constructed element values.</param>
        public EdmCollectionExpression(IEdmTypeReference declaredType, IEnumerable<IEdmExpression> elements)
        {
            EdmUtil.CheckArgumentNull(elements, "elements");

            this.declaredType = declaredType;
            this.elements = elements;
        }

        /// <summary>
        /// Gets the declared type of the collection.
        /// </summary>
        public IEdmTypeReference DeclaredType
        {
            get { return this.declaredType; }
        }

        /// <summary>
        /// Gets the constructed element values.
        /// </summary>
        public IEnumerable<IEdmExpression> Elements
        {
            get { return this.elements; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Collection; }
        }
    }
}
