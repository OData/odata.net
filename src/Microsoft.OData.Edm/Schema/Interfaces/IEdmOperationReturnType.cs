//---------------------------------------------------------------------
// <copyright file="IEdmOperationReturnType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents the return type of an EDM operation.
    /// </summary>
    public interface IEdmOperationReturnType : IEdmElement, IEdmVocabularyAnnotatable
    {
        /// <summary>
        /// Gets the operation that declared this return type.
        /// </summary>
        IEdmOperation DeclaringOperation { get; }

        /// <summary>
        /// Gets the type reference of this return type.
        /// </summary>
        IEdmTypeReference Type { get; }
    }
}
