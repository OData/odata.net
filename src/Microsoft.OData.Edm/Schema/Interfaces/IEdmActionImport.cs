//---------------------------------------------------------------------
// <copyright file="IEdmActionImport.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM action import.
    /// </summary>
    public interface IEdmActionImport : IEdmOperationImport
    {
        /// <summary>
        /// Gets the action type of the import.
        /// </summary>
        IEdmAction Action { get; }
    }
}
