//---------------------------------------------------------------------
// <copyright file="IEdmVocabularyAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM vocabulary annotation.
    /// </summary>
    public interface IEdmVocabularyAnnotation : IEdmElement
    {
        /// <summary>
        /// Gets the qualifier used to discriminate between multiple bindings of the same property or type.
        /// </summary>
        string Qualifier { get; }

        /// <summary>
        /// Gets the term bound by the annotation.
        /// </summary>
        IEdmTerm Term { get; }

        /// <summary>
        /// Gets the element the annotation applies to.
        /// </summary>
        IEdmVocabularyAnnotatable Target { get; }

        /// <summary>
        /// Gets the expression producing the value of the annotation.
        /// </summary>
        IEdmExpression Value { get; }
    }
}
