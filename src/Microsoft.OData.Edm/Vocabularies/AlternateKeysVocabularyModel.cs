//---------------------------------------------------------------------
// <copyright file="AlternateKeysVocabularyModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Vocabularies.Community.V1
{
    /// <summary>
    /// Representing Alternate Keys Vocabulary Model.
    /// </summary>
    public static class AlternateKeysVocabularyModel
    {
        /// <summary>
        /// The EDM model with Alternate Keys vocabularies.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmModel Instance;

        /// <summary>
        /// The Alternate Keys term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmTerm AlternateKeysTerm;

        /// <summary>
        /// The AlternateKey ComplexType.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        internal static readonly IEdmComplexType AlternateKeyType;

        /// <summary>
        /// The PropertyRef ComplexType.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        internal static readonly IEdmComplexType PropertyRefType;

        /// <summary>
        /// Parse Alternate Keys Vocabulary Model from AlternateKeysVocabularies.xml
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.FxCop.Rules.Security.Xml.SecurityXmlRules", "CA3053:UseXmlSecureResolver",
            MessageId = "System.Xml.XmlReader.Create",
            Justification = "The XmlResolver property no longer exists in .NET portable framework.")]
        static AlternateKeysVocabularyModel()
        {
            Assembly assembly = typeof(AlternateKeysVocabularyModel).GetAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("AlternateKeysVocabularies.xml"))
            {
                IEnumerable<EdmError> errors;
                Debug.Assert(stream != null, "AlternateKeysVocabularies.xml: stream!=null");
                SchemaReader.TryParse(new[] { XmlReader.Create(stream) }, out Instance, out errors);
            }

            AlternateKeysTerm = Instance.FindDeclaredTerm(AlternateKeysVocabularyConstants.AlternateKeys);
            Debug.Assert(AlternateKeysTerm != null, "Expected Alternate Key term");

            AlternateKeyType = Instance.FindDeclaredType(AlternateKeysVocabularyConstants.AlternateKeyType) as IEdmComplexType;
            Debug.Assert(AlternateKeyType != null, "Expected Alternate Key type");

            PropertyRefType = Instance.FindDeclaredType(AlternateKeysVocabularyConstants.PropertyRefType) as IEdmComplexType;
            Debug.Assert(PropertyRefType != null, "Expected Alternate Key property ref type");
        }
    }
}
