//---------------------------------------------------------------------
// <copyright file="IEdmRecordExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM record construction expression.
    /// </summary>
    public interface IEdmRecordExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the declared type of the record, or null if there is no declared type.
        /// </summary>
        IEdmStructuredTypeReference DeclaredType { get; }

        /// <summary>
        /// Gets the constructed property values.
        /// </summary>
        IEnumerable<IEdmPropertyConstructor> Properties { get; }
    }
}
