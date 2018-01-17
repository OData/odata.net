//---------------------------------------------------------------------
// <copyright file="CapabilitiesVocabularyModel.cs" company="Microsoft">
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
    /// Representing Capabilities Vocabulary Model.
    /// </summary>
    internal class CapabilitiesVocabularyModel
    {
        /// <summary>
        /// The EDM model with capabilities vocabularies.
        /// </summary>
        public static readonly IEdmModel Instance;

        /// <summary>
        /// The change tracking term.
        /// </summary>
        public static readonly IEdmTerm ChangeTrackingTerm;

        /// <summary>
        /// Parse Capabilities Vocabulary Model from CapabilitiesVocabularies.xml
        /// </summary>
        static CapabilitiesVocabularyModel()
        {
            Assembly assembly = typeof(CapabilitiesVocabularyModel).GetAssembly();

            // Resource name has leading namespace and folder in .NetStandard dll.
            string[] allResources = assembly.GetManifestResourceNames();
            string capabilitiesVocabularies = allResources.Where(x => x.Contains("CapabilitiesVocabularies.xml")).FirstOrDefault();
            Debug.Assert(capabilitiesVocabularies != null, "CapabilitiesVocabularies.xml: not found.");

            using (Stream stream = assembly.GetManifestResourceStream(capabilitiesVocabularies))
            {
                IEnumerable<EdmError> errors;
                Debug.Assert(stream != null, "CapabilitiesVocabularies.xml: stream!=null");
                CsdlReader.TryParse(XmlReader.Create(stream), out Instance, out errors);
            }

            ChangeTrackingTerm = Instance.FindDeclaredTerm(CapabilitiesVocabularyConstants.ChangeTracking);
        }
    }
}
