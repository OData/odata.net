//---------------------------------------------------------------------
// <copyright file="IEdmPathExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM path expression.
    /// </summary>
    public interface IEdmPathExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the path as a decomposed qualified name. "A.B/C/D.E" is { "A.B", "C", "D.E" }.
        /// </summary>
        IEnumerable<string> Path { get; }
    }
}
