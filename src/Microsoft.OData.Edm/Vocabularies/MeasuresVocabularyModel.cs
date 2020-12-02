//---------------------------------------------------------------------
// <copyright file="MeasuresVocabularyModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.Edm.Vocabularies.Measures.V1
{
    /// <summary>
    /// Representing Measures Vocabulary Model.
    /// </summary>
    public static class MeasuresVocabularyModel
    {
        /// <summary>
        /// The EDM model with Measures vocabularies.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmModel is immutable")]
        public static readonly IEdmModel Instance = VocabularyModelProvider.MeasuresModel;

    }
}
