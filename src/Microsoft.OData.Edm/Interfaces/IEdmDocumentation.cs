//---------------------------------------------------------------------
// <copyright file="IEdmDocumentation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM documentation.
    /// </summary>
    internal interface IEdmDocumentation
    {
        /// <summary>
        /// Gets a summary of this documentation.
        /// </summary>
        string Summary { get; }

        /// <summary>
        /// Gets a long description of this documentation.
        /// </summary>
        string Description { get; }
    }
}
