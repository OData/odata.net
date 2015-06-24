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
    /// Representing Core Vocabulary Model.
    /// </summary>
    public static class AlternateKeysVocabularyModel
    {
        /// <summary>
        /// The EDM model with core vocabularies.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmModel Instance;

        /// <summary>
        /// The Alternate Keys term.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmValueTerm AlternateKeysTerm;

        /// <summary>
        /// The AlternateKey ComplexType.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmComplexType AlternateKeyType;

        /// <summary>
        /// The PropertyRef ComplexType.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Resolver is immutable")]
        public static readonly IEdmComplexType PropertyRefType;

        /// <summary>
        /// Parse Alternate Keys Vocabulary Model from CoreVocabularies.xml
        /// </summary>
        static AlternateKeysVocabularyModel()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("AlternateKeysVocabularies.xml"))
            {
                IEnumerable<EdmError> errors;
                Debug.Assert(stream != null, "AlternateKeysVocabularies.xml: stream!=null");
                CsdlReader.TryParse(new[] { XmlReader.Create(stream) }, out Instance, out errors);
            }

            AlternateKeysTerm = Instance.FindDeclaredValueTerm(AlternateKeysVocabularyConstants.AlternateKeys);

            AlternateKeyType = Instance.FindDeclaredType(AlternateKeysVocabularyConstants.AlternateKeyType) as IEdmComplexType;

            PropertyRefType = Instance.FindDeclaredType(AlternateKeysVocabularyConstants.PropertyRefType) as IEdmComplexType;
        }
    }
}
