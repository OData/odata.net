//---------------------------------------------------------------------
// <copyright file="AlternateKeysVocabularyConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies.Community.V1
{
    /// <summary>
    /// Constant values for Alternate Keys Vocabularies
    /// </summary>
    public static class AlternateKeysVocabularyConstants
    {
        /// <summary>OData.Community.Keys.V1.AlternateKeys </summary>
        public const string AlternateKeys = "OData.Community.Keys.V1.AlternateKeys";

        /// <summary>OData.Community.Keys.V1.AlternateKey.Key </summary>
        internal const string AlternateKeyTypeKeyPropertyName = "Key";

        /// <summary>OData.Community.Keys.V1.PropertyRef.Name </summary>
        internal const string PropertyRefTypeNamePropertyName = "Name";

        /// <summary>OData.Community.Keys.V1.PropertyRef.Alias </summary>
        internal const string PropertyRefTypeAliasPropertyName = "Alias";

        /// <summary>OData.Community.Keys.V1.AlternateKey </summary>
        internal const string AlternateKeyType = "OData.Community.Keys.V1.AlternateKey";

        /// <summary>OData.Community.Keys.V1.PropertyRef </summary>
        internal const string PropertyRefType = "OData.Community.Keys.V1.PropertyRef";

        /// <summary>OData.Community.Keys.V1 file suffix</summary>
        internal const string VocabularyUrlSuffix = "/OData.Community.Keys.V1.xml";
    }
}
