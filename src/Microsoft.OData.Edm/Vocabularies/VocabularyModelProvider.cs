//---------------------------------------------------------------------
// <copyright file="VocabularyModelProvider.cs" company="Microsoft">
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
    internal static class VocabularyModelProvider
    {
        /// <summary>
        /// Core Vocabulary Model.
        /// </summary>
        public static readonly IEdmModel CoreModel;

        /// <summary>
        /// Capabilities Vocabulary Model.
        /// </summary>
        public static readonly IEdmModel CapabilitesModel;

        /// <summary>
        /// Alternate Vocabulary Model.
        /// </summary>
        public static readonly IEdmModel AlternateKeyModel;

        /// <summary>
        /// Community Vocabulary Model.
        /// </summary>
        public static readonly IEdmModel CommunityModel;

        /// <summary>
        /// Validation Vocabulary Model.
        /// </summary>
        public static readonly IEdmModel ValidationModel;

        /// <summary>
        /// Authorization Vocabulary Model.
        /// </summary>
        public static readonly IEdmModel AuthorizationModel;

        /// <summary>
        /// All Vocabulary Models.
        /// </summary>
        public static readonly IEnumerable<IEdmModel> VocabularyModels;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static VocabularyModelProvider()
        {
            Assembly assembly = typeof(VocabularyModelProvider).GetAssembly();

            string[] allResources = assembly.GetManifestResourceNames();

            // core
            string coreVocabularies = allResources.FirstOrDefault(x => x.Contains("CoreVocabularies.xml"));
            Debug.Assert(coreVocabularies != null, "CoreVocabularies.xml: not found.");
            CoreModel = LoadSchemaEdmModel(assembly, coreVocabularies, new IEdmModel[] { });

            // capabilites
            string capabilitiesVocabularies = allResources.FirstOrDefault(x => x.Contains("CapabilitiesVocabularies.xml"));
            Debug.Assert(capabilitiesVocabularies != null, "CapabilitiesVocabularies.xml: not found.");
            CapabilitesModel = LoadCsdlEdmModel(assembly, capabilitiesVocabularies, new[] { CoreModel }); // capabilities relies on core

            // alternateKey
            string alternateKeysVocabularies = allResources.Where(x => x.Contains("AlternateKeysVocabularies.xml")).FirstOrDefault();
            Debug.Assert(alternateKeysVocabularies != null, "AlternateKeysVocabularies.xml: not found.");
            AlternateKeyModel = LoadSchemaEdmModel(assembly, alternateKeysVocabularies, new[] { CoreModel }); // alternate relies on core

            // Community
            string communityVocabularies = allResources.Where(x => x.Contains("CommunityVocabularies.xml")).FirstOrDefault();
            Debug.Assert(communityVocabularies != null, "CommunityVocabularies.xml: not found.");
            CommunityModel = LoadCsdlEdmModel(assembly, communityVocabularies, new[] { CoreModel }); // community relies on core

            // validation
            string validationVocabularies = allResources.Where(x => x.Contains("ValidationVocabularies.xml")).FirstOrDefault();
            Debug.Assert(validationVocabularies != null, "ValidationVocabularies.xml: not found.");
            ValidationModel = LoadSchemaEdmModel(assembly, validationVocabularies, new[] { CoreModel }); // validation relies on core

            // Authorization
            string authorizationVocabularies = allResources.FirstOrDefault(x => x.Contains("AuthorizationVocabularies.xml"));
            Debug.Assert(authorizationVocabularies != null, "AuthorizationVocabularies.xml: not found.");
            AuthorizationModel = LoadCsdlEdmModel(assembly, authorizationVocabularies, new[] { CoreModel }); // authorization relies on core

            VocabularyModels = new List<IEdmModel>
            {
                CoreModel, CapabilitesModel, AlternateKeyModel, CommunityModel, ValidationModel, AuthorizationModel
            };
        }

        private static IEdmModel LoadCsdlEdmModel(Assembly assembly, string vocabularyName, IEnumerable<IEdmModel> referenceModels)
        {
            using (Stream stream = assembly.GetManifestResourceStream(vocabularyName))
            {
                IEdmModel model;
                IEnumerable<EdmError> errors;
                CsdlReader.TryParse(XmlReader.Create(stream), referenceModels, false, out model, out errors);
                return model;
            }
        }

        private static IEdmModel LoadSchemaEdmModel(Assembly assembly, string vocabularyName, IEnumerable<IEdmModel> referenceModels)
        {
            using (Stream stream = assembly.GetManifestResourceStream(vocabularyName))
            {
                IEdmModel model;
                IEnumerable<EdmError> errors;
                SchemaReader.TryParse(new[] { XmlReader.Create(stream) }, referenceModels, false, out model, out errors);
                return model;
            }
        }
    }
}
