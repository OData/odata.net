//---------------------------------------------------------------------
// <copyright file="ValidationVocabularyModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.Edm.Vocabularies.V1
{
    /// <summary>
    /// Representing Validation Vocabulary Model.
    /// </summary>
    public static class ValidationVocabularyModel
    {
        /// <summary>
        /// The namespace of the validation vocabulary model.
        /// </summary>
        public static readonly string Namespace = "Org.OData.Validation.V1";

        /// <summary>
        /// The EDM model with validation vocabularies.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmModel is immutable")]
        public static readonly IEdmModel Instance = VocabularyModelProvider.ValidationModel;

        /// <summary>
        /// The DerivedTypeConstraint term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm DerivedTypeConstraintTerm = VocabularyModelProvider.ValidationModel.FindDeclaredTerm(Namespace + ".DerivedTypeConstraint");
    }
}
