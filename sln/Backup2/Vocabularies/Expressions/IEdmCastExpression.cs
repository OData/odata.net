//---------------------------------------------------------------------
// <copyright file="IEdmCastExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM type assertion expression.
    /// </summary>
    public interface IEdmCastExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the expression for which the type is asserted.
        /// </summary>
        IEdmExpression Operand { get; }

        /// <summary>
        /// Gets the asserted type.
        /// </summary>
        IEdmTypeReference Type { get; }
    }
}
