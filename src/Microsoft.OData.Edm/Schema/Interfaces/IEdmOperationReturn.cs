//---------------------------------------------------------------------
// <copyright file="IEdmOperationReturn.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a return of an EDM operation.
    /// </summary>
    public interface IEdmOperationReturn : IEdmElement, IEdmVocabularyAnnotatable
    {
        /// <summary>
        /// Gets the type of this operation return.
        /// </summary>
        IEdmTypeReference Type { get; }

        /// <summary>
        /// Gets the operation that declared this return.
        /// </summary>
        IEdmOperation DeclaringOperation { get; }
    }
}
