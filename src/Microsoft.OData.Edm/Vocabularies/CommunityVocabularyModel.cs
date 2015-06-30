//---------------------------------------------------------------------
// <copyright file="CommunityVocabularyModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
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
    /// Representing Community Vocabulary Model.
    /// </summary>
    public static class CommunityVocabularyModel
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
        public static readonly IEdmValueTerm AlternateKeysTerm;

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
        /// Parse Community Vocabulary Model from CommunityVocabularies.xml
        /// </summary>
        static CommunityVocabularyModel()
        {
            Assembly assembly = typeof(CommunityVocabularyModel).GetAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("CommunityVocabularies.xml"))
            {
                IEnumerable<EdmError> errors;
                Debug.Assert(stream != null, "CommunityVocabularies.xml: stream!=null");
                CsdlReader.TryParse(new[] { XmlReader.Create(stream) }, out Instance, out errors);
            }

            AlternateKeysTerm = Instance.FindDeclaredValueTerm(CommunityVocabularyConstants.AlternateKeys);
            Debug.Assert(AlternateKeysTerm != null, "Expected Alternate Key term");

            AlternateKeyType = Instance.FindDeclaredType(CommunityVocabularyConstants.AlternateKeyType) as IEdmComplexType;
            Debug.Assert(AlternateKeyType != null, "Expected Alternate Key type");

            PropertyRefType = Instance.FindDeclaredType(CommunityVocabularyConstants.PropertyRefType) as IEdmComplexType;
            Debug.Assert(PropertyRefType != null, "Expected Alternate Key property ref type");
        }
    }
}
