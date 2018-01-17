//---------------------------------------------------------------------
// <copyright file="CoreVocabularyModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmModel is immutable")]
        public static readonly IEdmModel Instance;

        /// <summary>
        /// The concurrency term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm ConcurrencyTerm;

        /// <summary>
        /// The description term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm DescriptionTerm;

        /// <summary>
        /// The description term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm LongDescriptionTerm;

        /// <summary>
        /// The IsLanguageDependent term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm IsLanguageDependentTerm;

        /// <summary>
        /// The RequiresType term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm RequiresTypeTerm;

        /// <summary>
        /// The ResourcePath term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm ResourcePathTerm;

        /// <summary>
        /// The DereferenceableIDs term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm DereferenceableIDsTerm;

        /// <summary>
        /// The ConventionalIDs term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm ConventionalIDsTerm;

        /// <summary>
        /// The Immutable term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm ImmutableTerm;

        /// <summary>
        /// The Computed term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm ComputedTerm;

        /// <summary>
        /// The Optional Parameter term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmTerm OptionalParameterTerm;

        /// <summary>
        /// The IsURL term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm IsURLTerm;

        /// <summary>
        /// The AcceptableMediaTypes term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm AcceptableMediaTypesTerm;

        /// <summary>
        /// The MediaType term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm MediaTypeTerm;

        /// <summary>
        /// The IsMediaType term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm IsMediaTypeTerm;

        /// <summary>
        /// The Permissions Term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm PermissionsTerm;

        internal static bool IsInitializing;

        /// <summary>
        /// Parse Core Vocabulary Model from CoreVocabularies.xml
        /// </summary>
        static CoreVocabularyModel()
        {
            IsInitializing = true;
            Assembly assembly = typeof(CoreVocabularyModel).GetAssembly();

            // Resource name has leading namespace and folder in .NetStandard dll.
            string[] allResources = assembly.GetManifestResourceNames();
            string coreVocabularies = allResources.Where(x => x.Contains("CoreVocabularies.xml")).FirstOrDefault();
            Debug.Assert(coreVocabularies != null, "CoreVocabularies.xml: not found.");

            using (Stream stream = assembly.GetManifestResourceStream(coreVocabularies))
            {
                IEnumerable<EdmError> errors;
                Debug.Assert(stream != null, "CoreVocabularies.xml: stream!=null");
                SchemaReader.TryParse(new[] { XmlReader.Create(stream) }, out Instance, out errors);
                IsInitializing = false;
            }

            AcceptableMediaTypesTerm = Instance.FindDeclaredTerm(CoreVocabularyConstants.AcceptableMediaTypes);
            ComputedTerm = Instance.FindDeclaredTerm(CoreVocabularyConstants.Computed);
            ConcurrencyTerm = Instance.FindDeclaredTerm(CoreVocabularyConstants.OptimisticConcurrency);
            ConventionalIDsTerm = Instance.FindDeclaredTerm(CoreVocabularyConstants.ConventionalIDs);
            DereferenceableIDsTerm = Instance.FindDeclaredTerm(CoreVocabularyConstants.DereferenceableIDs);
            DescriptionTerm = Instance.FindDeclaredTerm(CoreVocabularyConstants.Description);
            ImmutableTerm = Instance.FindDeclaredTerm(CoreVocabularyConstants.Immutable);
            IsLanguageDependentTerm = Instance.FindDeclaredTerm(CoreVocabularyConstants.IsLanguageDependent);
            IsMediaTypeTerm = Instance.FindDeclaredTerm(CoreVocabularyConstants.IsMediaType);
            IsURLTerm = Instance.FindDeclaredTerm(CoreVocabularyConstants.IsURL);
            LongDescriptionTerm = Instance.FindDeclaredTerm(CoreVocabularyConstants.LongDescription);
            MediaTypeTerm = Instance.FindDeclaredTerm(CoreVocabularyConstants.MediaType);
            OptionalParameterTerm = Instance.FindDeclaredTerm(CoreVocabularyConstants.OptionalParameter);
            RequiresTypeTerm = Instance.FindDeclaredTerm(CoreVocabularyConstants.RequiresType);
            ResourcePathTerm = Instance.FindDeclaredTerm(CoreVocabularyConstants.ResourcePath);
            PermissionsTerm = Instance.FindDeclaredTerm(CoreVocabularyConstants.Permissions);
        }
    }
}
