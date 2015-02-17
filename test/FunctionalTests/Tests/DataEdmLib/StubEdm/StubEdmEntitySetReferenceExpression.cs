//---------------------------------------------------------------------
// <copyright file="StubEdmEntitySetReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.StubEdm
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Expressions;

    /// <summary>
    /// Represents an EDM entity set reference expression.
    /// </summary>
    public class StubEdmEntitySetReferenceExpression : StubEdmElement, IEdmEntitySetReferenceExpression
    {
        /// <summary>
        /// Gets or sets the referenced entity set.
        /// </summary>
        public IEdmEntitySet ReferencedEntitySet
        {
            get;
            set;
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
