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
    /// It's derived from "IEdmTypeReference" for avoiding breaking change.
    /// In the next breaking change release, it should be changed to derived from "IEdmElement"
    /// </summary>
    public interface IEdmOperationReturnType : IEdmTypeReference, IEdmVocabularyAnnotatable
    {
        /// <summary>
        /// Gets the operation that declared this return type.
        /// </summary>
        IEdmOperation DeclaringOperation { get; }

        /// <summary>
        /// Gets the type reference of this return type.
        /// </summary>
        IEdmTypeReference ReturnType { get; }
    }
}
