//---------------------------------------------------------------------
// <copyright file="IEdmStructuralPropertyAlias.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a property alias.
    /// </summary>
    public interface IEdmStructuralPropertyAlias : IEdmStructuralProperty
    {
        /// <summary>
        /// The path to the property.
        /// </summary>
        IEnumerable<string> Path { get; }

        /// <summary>
        /// The property alias.
        /// </summary>
        string PropertyAlias { get; }
    }
}
