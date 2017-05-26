//---------------------------------------------------------------------
// <copyright file="IEdmLabeledExpressionReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents a reference to an EDM labeled expression.
    /// </summary>
    public interface IEdmLabeledExpressionReferenceExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the referenced expression.
        /// </summary>
        IEdmLabeledExpression ReferencedLabeledExpression { get; }
    }
}
