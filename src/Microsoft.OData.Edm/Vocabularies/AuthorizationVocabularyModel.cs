//---------------------------------------------------------------------
// <copyright file="AuthorizationVocabularyModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies.V1
{
    /// <summary>
    /// Representing Org.OData.Authorization.V1 Vocabulary Model.
    /// </summary>
    public static class AuthorizationVocabularyModel
    {
        /// <summary>
        /// The EDM model with authorization vocabularies.
        /// </summary>
        public static readonly IEdmModel Instance = VocabularyModelProvider.AuthorizationModel;
    }
}
