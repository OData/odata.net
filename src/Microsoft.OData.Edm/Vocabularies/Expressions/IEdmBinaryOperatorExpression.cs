//---------------------------------------------------------------------
// <copyright file="IEdmBinaryOperatorExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM type binary operator expression.
    /// </summary>
    public interface IEdmBinaryOperatorExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the binary operator kind represented by this expression.
        /// </summary>
        EdmBinaryOperatorKind Kind { get; }

        /// <summary>
        /// Gets the left expression.
        /// </summary>
        IEdmExpression Left { get; }
        
        /// <summary>
        /// Gets the right expression.
        /// </summary>
        IEdmExpression Right { get; }
    }
}
