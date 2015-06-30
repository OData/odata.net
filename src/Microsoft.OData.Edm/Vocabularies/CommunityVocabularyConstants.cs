//---------------------------------------------------------------------
// <copyright file="CommunityVocabularyConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies.Community.V1
{
    /// <summary>
    /// Constant values for Community Vocabularies
    /// </summary>
    public static class CommunityVocabularyConstants
    {
        /// <summary>OData.Community.AlternateKeys.V1.AlternateKeys </summary>
        public const string AlternateKeys = "OData.Community.AlternateKeys.V1.AlternateKeys";

        /// <summary>OData.Community.AlternateKeys.V1.AlternateKey </summary>
        public const string AlternateKeyType = "OData.Community.AlternateKeys.V1.AlternateKey";

        /// <summary>OData.Community.AlternateKeys.V1.AlternateKey.Key </summary>
        public const string AlternateKeyTypeKeyPropertyName = "Key";

        /// <summary>OData.Community.AlternateKeys.V1.PropertyRef </summary>
        public const string PropertyRefType = "OData.Community.AlternateKeys.V1.PropertyRef";

        /// <summary>OData.Community.AlternateKeys.V1.PropertyRef.Name </summary>
        public const string PropertyRefTypeNamePropertyName = "Name";

        /// <summary>OData.Community.AlternateKeys.V1.PropertyRef.Alias </summary>
        public const string PropertyRefTypeAliasPropertyName = "Alias";

        /// <summary>OData.Community.AlternateKeys.V1 file suffix</summary>
        internal const string VocabularyUrlSuffix = "/OData.Community.AlternateKeys.V1.xml";
    }
}
