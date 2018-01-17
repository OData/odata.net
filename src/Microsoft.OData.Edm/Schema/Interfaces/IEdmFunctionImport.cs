//---------------------------------------------------------------------
// <copyright file="IEdmFunctionImport.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM function import.
    /// </summary>
    public interface IEdmFunctionImport : IEdmOperationImport
    {
        /// <summary>
        /// Gets a value indicating whether [include in service document].
        /// </summary>
        bool IncludeInServiceDocument { get; }

        /// <summary>
        /// Gets the function that defines the function import.
        /// </summary>
        IEdmFunction Function { get; }
    }
}
