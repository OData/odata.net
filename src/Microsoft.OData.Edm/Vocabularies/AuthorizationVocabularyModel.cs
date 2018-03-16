//---------------------------------------------------------------------
// <copyright file="AuthorizationVocabularyModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Vocabularies.V1
{
    /// <summary>
    /// Representing Org.OData.Authorization.V1 Vocabulary Model.
    /// </summary>
    internal class AuthorizationVocabularyModel
    {
        /// <summary>
        /// The EDM model with authorization vocabularies.
        /// </summary>
        public static readonly IEdmModel Instance;

        /// <summary>
        /// Parse authorization vocabulary Model from AuthorizationVocabularies.xml
        /// </summary>
        static AuthorizationVocabularyModel()
        {
            Assembly assembly = typeof(AuthorizationVocabularyModel).GetAssembly();

            // Resource name has leading namespace and folder in .NetStandard dll.
            string[] allResources = assembly.GetManifestResourceNames();
            string authorizationVocabularies = allResources.FirstOrDefault(x => x.Contains("AuthorizationVocabularies.xml"));
            Debug.Assert(authorizationVocabularies != null, "AuthorizationVocabularies.xml: not found.");

            using (Stream stream = assembly.GetManifestResourceStream(authorizationVocabularies))
            {
                IEnumerable<EdmError> errors;
                Debug.Assert(stream != null, "AuthorizationVocabularies.xml: stream!=null");
                CsdlReader.TryParse(XmlReader.Create(stream), out Instance, out errors);
            }
        }
    }
}
