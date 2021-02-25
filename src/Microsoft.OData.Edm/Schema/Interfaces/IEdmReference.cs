//---------------------------------------------------------------------
// <copyright file="IEdmReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a definition of an edmx:reference element.
    /// </summary>
    public interface IEdmReference : IEdmElement, IEdmVocabularyAnnotatable
    {
        /// <summary>
        /// Uri
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// The Includes information.
        /// </summary>
        IEnumerable<IEdmInclude> Includes { get; }

        /// <summary>
        /// The IncludeAnnotations information.
        /// </summary>
        IEnumerable<IEdmIncludeAnnotations> IncludeAnnotations { get; }
    }
}
