//---------------------------------------------------------------------
// <copyright file="CoreVocabularyModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.Edm.Vocabularies.V1
{
    /// <summary>
    /// Representing Core Vocabulary Model.
    /// </summary>
    public static class CoreVocabularyModel
    {
        /// <summary>
        /// The EDM model with core vocabularies.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmModel is immutable")]
        public static readonly IEdmModel Instance = VocabularyModelProvider.CoreModel;

        /// <summary>
        /// The concurrency term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm ConcurrencyTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm(CoreVocabularyConstants.OptimisticConcurrency);

        /// <summary>
        /// The description term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm DescriptionTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm(CoreVocabularyConstants.Description);

        /// <summary>
        /// The description term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm LongDescriptionTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm(CoreVocabularyConstants.LongDescription);

        /// <summary>
        /// The IsLanguageDependent term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm IsLanguageDependentTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm(CoreVocabularyConstants.IsLanguageDependent);

        /// <summary>
        /// The RequiresType term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm RequiresTypeTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm(CoreVocabularyConstants.RequiresType);

        /// <summary>
        /// The ResourcePath term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm ResourcePathTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm(CoreVocabularyConstants.ResourcePath);

        /// <summary>
        /// The Revisions term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm RevisionsTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm(CoreVocabularyConstants.Revisions);

        /// <summary>
        /// The DereferenceableIDs term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm DereferenceableIDsTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm(CoreVocabularyConstants.DereferenceableIDs);

        /// <summary>
        /// The ConventionalIDs term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm ConventionalIDsTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm(CoreVocabularyConstants.ConventionalIDs);

        /// <summary>
        /// The Immutable term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm ImmutableTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm(CoreVocabularyConstants.Immutable);

        /// <summary>
        /// The Computed term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm ComputedTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm(CoreVocabularyConstants.Computed);

        /// <summary>
        /// The Optional Parameter term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm OptionalParameterTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm(CoreVocabularyConstants.OptionalParameter);

        /// <summary>
        /// The IsURL term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm IsURLTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm(CoreVocabularyConstants.IsURL);

        /// <summary>
        /// The AcceptableMediaTypes term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm AcceptableMediaTypesTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm(CoreVocabularyConstants.AcceptableMediaTypes);

        /// <summary>
        /// The MediaType term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm MediaTypeTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm(CoreVocabularyConstants.MediaType);

        /// <summary>
        /// The IsMediaType term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm IsMediaTypeTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm(CoreVocabularyConstants.IsMediaType);

        /// <summary>
        /// The Permissions Term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm PermissionsTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm(CoreVocabularyConstants.Permissions);
    }
}
