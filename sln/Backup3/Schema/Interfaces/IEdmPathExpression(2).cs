//---------------------------------------------------------------------
// <copyright file="IEdmPathExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM path expression.
    /// </summary>
    public interface IEdmPathExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the path segments as a decomposed qualified name. "A.B/C/D.E" is { "A.B", "C", "D.E" }.
        /// </summary>
        IEnumerable<string> PathSegments { get; }

        /// <summary>
        /// Gets the path string, like "A.B/C/D.E".
        /// </summary>
        string Path { get; }
    }
}
