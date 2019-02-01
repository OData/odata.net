//---------------------------------------------------------------------
// <copyright file="CommunityVocabularyModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.Edm.Vocabularies.Community.V1
{
    /// <summary>
    /// Representing Community Vocabulary Model.
    /// </summary>
    public static class CommunityVocabularyModel
    {
        /// <summary>
        /// The EDM model with Community vocabularies.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmModel is immutable")]
        public static readonly IEdmModel Instance = VocabularyModelProvider.CommunityModel;

        /// <summary>
        /// The Url escape function term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm UrlEscapeFunctionTerm = VocabularyModelProvider.CommunityModel.FindDeclaredTerm(CommunityVocabularyConstants.UrlEscapeFunction);
    }
}
