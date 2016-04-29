//---------------------------------------------------------------------
// <copyright file="IEdmOperationImport.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM operation import.
    /// </summary>
    public interface IEdmOperationImport : IEdmEntityContainerElement
    {
        /// <summary>
        /// Gets the operation.
        /// </summary>
        IEdmOperation Operation { get; }

        /// <summary>
        /// Gets the entity set containing entities returned by this operation import.
        /// </summary>
        IEdmExpression EntitySet { get; }
    }
}
