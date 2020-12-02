//---------------------------------------------------------------------
// <copyright file="CapabilitiesVocabularyModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies.V1
{
    /// <summary>
    /// Representing Capabilities Vocabulary Model.
    /// </summary>
    public static class CapabilitiesVocabularyModel
    {
        /// <summary>
        /// The EDM model with capabilities vocabularies.
        /// </summary>
        public static readonly IEdmModel Instance = VocabularyModelProvider.CapabilitiesModel;

        /// <summary>
        /// The change tracking term.
        /// </summary>
        internal static readonly IEdmTerm ChangeTrackingTerm = VocabularyModelProvider.CapabilitiesModel.FindDeclaredTerm(CapabilitiesVocabularyConstants.ChangeTracking);
    }
}
