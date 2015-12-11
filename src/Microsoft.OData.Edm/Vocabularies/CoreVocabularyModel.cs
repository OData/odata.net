//---------------------------------------------------------------------
// <copyright file="CoreVocabularyModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;


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
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmModel Instance;

        /// <summary>
        /// The concurrency term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmValueTerm ConcurrencyTerm;

        /// <summary>
        /// The concurrency control term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmValueTerm ConcurrencyControlTerm;

        /// <summary>
        /// The description term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmValueTerm DescriptionTerm;

        /// <summary>
        /// The description term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmValueTerm LongDescriptionTerm;

        /// <summary>
        /// The IsLanguageDependent term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmValueTerm IsLanguageDependentTerm;

        /// <summary>
        /// The RequiresType term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmValueTerm RequiresTypeTerm;

        /// <summary>
        /// The ResourcePath term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmValueTerm ResourcePathTerm;

        /// <summary>
        /// The DereferenceableIDs term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmValueTerm DereferenceableIDsTerm;

        /// <summary>
        /// The ConventionalIDs term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmValueTerm ConventionalIDsTerm;

        /// <summary>
        /// The Immutable term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmValueTerm ImmutableTerm;

        /// <summary>
        /// The Computed term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmValueTerm ComputedTerm;

        /// <summary>
        /// The IsURL term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmValueTerm IsURLTerm;

        /// <summary>
        /// The AcceptableMediaTypes term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmValueTerm AcceptableMediaTypesTerm;

        /// <summary>
        /// The MediaType term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmValueTerm MediaTypeTerm;

        /// <summary>
        /// The IsMediaType term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmValueTerm IsMediaTypeTerm;

        internal static bool IsInitializing;

        /// <summary>
        /// Parse Core Vocabulary Model from CoreVocabularies.xml
        /// </summary>
        static CoreVocabularyModel()
        {
            IsInitializing = true;
            Assembly assembly = typeof(CoreVocabularyModel).GetAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("CoreVocabularies.xml"))
            {
                IEnumerable<EdmError> errors;
                Debug.Assert(stream != null, "CoreVocabularies.xml: stream!=null");
                CsdlReader.TryParse(new[] { XmlReader.Create(stream) }, out Instance, out errors);
                IsInitializing = false;
            }

            AcceptableMediaTypesTerm = Instance.FindDeclaredValueTerm(CoreVocabularyConstants.AcceptableMediaTypes);
            ComputedTerm = Instance.FindDeclaredValueTerm(CoreVocabularyConstants.Computed);
            ConcurrencyControlTerm = Instance.FindDeclaredValueTerm(CoreVocabularyConstants.OptimisticConcurrencyControl);
            ConcurrencyTerm = Instance.FindDeclaredValueTerm(CoreVocabularyConstants.OptimisticConcurrency);
            ConventionalIDsTerm = Instance.FindDeclaredValueTerm(CoreVocabularyConstants.ConventionalIDs);
            DereferenceableIDsTerm = Instance.FindDeclaredValueTerm(CoreVocabularyConstants.DereferenceableIDs);
            DescriptionTerm = Instance.FindDeclaredValueTerm(CoreVocabularyConstants.Description);
            ImmutableTerm = Instance.FindDeclaredValueTerm(CoreVocabularyConstants.Immutable);
            IsLanguageDependentTerm = Instance.FindDeclaredValueTerm(CoreVocabularyConstants.IsLanguageDependent);
            IsMediaTypeTerm = Instance.FindDeclaredValueTerm(CoreVocabularyConstants.IsMediaType);
            IsURLTerm = Instance.FindDeclaredValueTerm(CoreVocabularyConstants.IsURL);
            LongDescriptionTerm = Instance.FindDeclaredValueTerm(CoreVocabularyConstants.LongDescription);
            MediaTypeTerm = Instance.FindDeclaredValueTerm(CoreVocabularyConstants.MediaType);
            RequiresTypeTerm = Instance.FindDeclaredValueTerm(CoreVocabularyConstants.RequiresType);
            ResourcePathTerm = Instance.FindDeclaredValueTerm(CoreVocabularyConstants.ResourcePath);
        }
    }
}
