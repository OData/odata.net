//---------------------------------------------------------------------
// <copyright file="IEdmTargetPath.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a path to a model element.
    /// </summary>
    public interface IEdmTargetPath : IEdmVocabularyAnnotatable
    {
        /// <summary>
        /// Gets the target path.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the target path segments as Edm elements.
        /// </summary>
        IReadOnlyList<IEdmElement> Segments { get; }
    }
}
