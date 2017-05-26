//---------------------------------------------------------------------
// <copyright file="IEdmCollectionExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM multi-value construction expression.
    /// </summary>
    public interface IEdmCollectionExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the declared type of the collection, or null if there is no declared type.
        /// </summary>
        IEdmTypeReference DeclaredType { get; }

        /// <summary>
        /// Gets the constructed element values.
        /// </summary>
        IEnumerable<IEdmExpression> Elements { get; }
    }
}
