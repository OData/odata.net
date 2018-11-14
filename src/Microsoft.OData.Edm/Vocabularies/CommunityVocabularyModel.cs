//---------------------------------------------------------------------
// <copyright file="CommunityVocabularyModel.cs" company="Microsoft">
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
        public static readonly IEdmModel Instance = CommunityVocabularyReader.GetEdmModel();

        /// <summary>
        /// The Url escape function term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
        public static readonly IEdmTerm UrlEscapeFunctionTerm = Instance.FindDeclaredTerm(CommunityVocabularyConstants.UrlEscapeFunction);
    }

    internal static class CommunityVocabularyReader
    {
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmModel is immutable")]

        internal static IEdmModel GetEdmModel()
        {
            Assembly assembly = typeof(CommunityVocabularyModel).GetAssembly();

            // Resource name has leading namespace and folder in .NetStandard dll.
            string[] allResources = assembly.GetManifestResourceNames();
            string communityVocabularies = allResources.Where(x => x.Contains("CommunityVocabularies.xml")).FirstOrDefault();
            Debug.Assert(communityVocabularies != null, "CommunityVocabularies.xml: not found.");

            IEdmModel instance;
            using (Stream stream = assembly.GetManifestResourceStream(communityVocabularies))
            {
                IEnumerable<EdmError> errors;
                Debug.Assert(stream != null, "CommunityVocabularies.xml: stream!=null");
                CsdlReader.TryParse(XmlReader.Create(stream), out instance, out errors);
            }

            return instance;
        }
    }
}
