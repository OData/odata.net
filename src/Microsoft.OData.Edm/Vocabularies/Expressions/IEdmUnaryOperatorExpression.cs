//---------------------------------------------------------------------
// <copyright file="IEdmUnaryOperatorExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM type unary operator expression.
    /// </summary>
    public interface IEdmUnaryOperatorExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the unary operator kind represented by this expression.
        /// </summary>
        EdmUnaryOperatorKind Kind { get; }
        
        /// <summary>
        /// Gets the operand expression.
        /// </summary>
        IEdmExpression Operand { get; }
    }
}
