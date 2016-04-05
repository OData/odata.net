//---------------------------------------------------------------------
// <copyright file="IEdmValueTermReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM value term reference expression.
    /// </summary>
    public interface IEdmValueTermReferenceExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the expression for the structured value containing the referenced term property.
        /// </summary>
        IEdmExpression Base { get; }

        /// <summary>
        /// Gets the referenced value term.
        /// </summary>
        IEdmValueTerm Term { get; }

        /// <summary>
        /// Gets the optional qualifier.
        /// </summary>
        string Qualifier { get; }
    }
}
