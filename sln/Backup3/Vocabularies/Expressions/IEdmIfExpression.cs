//---------------------------------------------------------------------
// <copyright file="IEdmIfExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM if expression.
    /// </summary>
    public interface IEdmIfExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the test expression.
        /// </summary>
        IEdmExpression TestExpression { get; }

        /// <summary>
        /// Gets the expression to evaluate if <see cref="TestExpression"/> evaluates to true.
        /// </summary>
        IEdmExpression TrueExpression { get; }

        /// <summary>
        /// Gets the expression to evaluate if <see cref="TestExpression"/> evaluates to false.
        /// </summary>
        IEdmExpression FalseExpression { get; }
    }
}
