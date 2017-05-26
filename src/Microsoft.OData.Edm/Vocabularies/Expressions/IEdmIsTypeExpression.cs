//---------------------------------------------------------------------
// <copyright file="IEdmIsTypeExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM type test expression.
    /// </summary>
    public interface IEdmIsTypeExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the expression whose type is to be tested.
        /// </summary>
        IEdmExpression Operand { get; }

        /// <summary>
        /// Gets the type to be tested against.
        /// </summary>
        IEdmTypeReference Type { get; }
    }
}
