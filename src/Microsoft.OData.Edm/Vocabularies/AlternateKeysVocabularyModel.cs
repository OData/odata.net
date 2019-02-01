//---------------------------------------------------------------------
// <copyright file="AlternateKeysVocabularyModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.Edm.Vocabularies.Community.V1
{
    /// <summary>
    /// Representing Alternate Keys Vocabulary Model.
    /// </summary>
    public static class AlternateKeysVocabularyModel
    {
        /// <summary>
        /// The EDM model with Alternate Keys vocabularies.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmModel is immutable")]
        public static readonly IEdmModel Instance = VocabularyModelProvider.AlternateKeyModel;

        /// <summary>
        /// The Alternate Keys term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm AlternateKeysTerm = VocabularyModelProvider.AlternateKeyModel.FindDeclaredTerm(AlternateKeysVocabularyConstants.AlternateKeys);

        /// <summary>
        /// The AlternateKey ComplexType.
        /// </summary>
        internal static readonly IEdmComplexType AlternateKeyType = VocabularyModelProvider.AlternateKeyModel.FindDeclaredType(AlternateKeysVocabularyConstants.AlternateKeyType) as IEdmComplexType;

        /// <summary>
        /// The PropertyRef ComplexType.
        /// </summary>
        internal static readonly IEdmComplexType PropertyRefType = VocabularyModelProvider.AlternateKeyModel.FindDeclaredType(AlternateKeysVocabularyConstants.PropertyRefType) as IEdmComplexType;
    }
}
