//---------------------------------------------------------------------
// <copyright file="IEdmOperationParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a parameter of an EDM operation.
    /// </summary>
    public interface IEdmOperationParameter : IEdmNamedElement, IEdmVocabularyAnnotatable
    {
        /// <summary>
        /// Gets the type of this operation parameter.
        /// </summary>
        IEdmTypeReference Type { get; }

        /// <summary>
        /// Gets the operation that declared this parameter.
        /// </summary>
        IEdmOperation DeclaringOperation { get; }
    }
}
