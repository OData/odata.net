//---------------------------------------------------------------------
// <copyright file="ValidationVocabularyModel.cs" company="Microsoft">
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
    /// Representing Validation Vocabulary Model.
    /// </summary>
    public static class ValidationVocabularyModel
    {
        /// <summary>
        /// The EDM model with validation vocabularies.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmModel is immutable")]
        public static readonly IEdmModel Instance;

        /// <summary>
        /// Parse Validation Vocabulary Model from ValidationVocabularies.xml
        /// </summary>
        static ValidationVocabularyModel()
        {
            Assembly assembly = typeof(ValidationVocabularyModel).GetAssembly();

            // Resource name has leading namespace and folder in .NetStandard dll.
            string[] allResources = assembly.GetManifestResourceNames();
            string validationVocabularies = allResources.Where(x => x.Contains("ValidationVocabularies.xml")).FirstOrDefault();
            Debug.Assert(validationVocabularies != null, "ValidationVocabularies.xml: not found.");

            using (Stream stream = assembly.GetManifestResourceStream(validationVocabularies))
            {
                IEnumerable<EdmError> errors;
                Debug.Assert(stream != null, "ValidationVocabularies.xml: stream!=null");
                SchemaReader.TryParse(new[] { XmlReader.Create(stream) }, out Instance, out errors);
            }
        }
    }
}
