//---------------------------------------------------------------------
// <copyright file="CapabilitiesVocabularyModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public static readonly IEdmValueTerm ChangeTrackingTerm;

        /// <summary>
        /// The count restrictions term.
        /// </summary>
        public static readonly IEdmValueTerm CountRestrictionsTerm;

        /// <summary>
        /// The navigation restrictions term.
        /// </summary>
        public static readonly IEdmValueTerm NavigationRestrictionsTerm;

        /// <summary>
        /// The filter restrictions term.
        /// </summary>
        public static readonly IEdmValueTerm FilterRestrictionsTerm;

        /// <summary>
        /// The sort restrictions term.
        /// </summary>
        public static readonly IEdmValueTerm SortRestrictionsTerm;

        /// <summary>
        /// The expand restrictions term.
        /// </summary>
        public static readonly IEdmValueTerm ExpandRestrictionsTerm;

        /// <summary>
        /// The navigation type.
        /// </summary>
        public static readonly IEdmEnumType NavigationTypeType;

        /// <summary>
        /// Parse Capabilities Vocabulary Model from CapabilitiesVocabularies.xml
        /// </summary>
        static CapabilitiesVocabularyModel()
        {
            Assembly assembly = typeof(CapabilitiesVocabularyModel).GetAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("CapabilitiesVocabularies.xml"))
            {
                IEnumerable<EdmError> errors;
                Debug.Assert(stream != null, "CapabilitiesVocabularies.xml: stream!=null");
                CsdlReader.TryParse(new[] { XmlReader.Create(stream) }, out Instance, out errors);
            }

            ChangeTrackingTerm = Instance.FindDeclaredValueTerm(CapabilitiesVocabularyConstants.ChangeTracking);
            CountRestrictionsTerm = Instance.FindDeclaredValueTerm(CapabilitiesVocabularyConstants.CountRestrictions);
            NavigationRestrictionsTerm = Instance.FindDeclaredValueTerm(CapabilitiesVocabularyConstants.NavigationRestrictions);
            FilterRestrictionsTerm = Instance.FindDeclaredValueTerm(CapabilitiesVocabularyConstants.FilterRestrictions);
            SortRestrictionsTerm = Instance.FindDeclaredValueTerm(CapabilitiesVocabularyConstants.SortRestrictions);
            ExpandRestrictionsTerm = Instance.FindDeclaredValueTerm(CapabilitiesVocabularyConstants.ExpandRestrictions);

            NavigationTypeType = (IEdmEnumType)Instance.FindDeclaredType(CapabilitiesVocabularyConstants.NavigationType);
        }
    }
}
