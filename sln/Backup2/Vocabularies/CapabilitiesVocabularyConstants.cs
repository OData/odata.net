//---------------------------------------------------------------------
// <copyright file="CapabilitiesVocabularyConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies.V1
{
    /// <summary>
    /// Constant values for Capabilities Vocabulary
    /// </summary>
    public static class CapabilitiesVocabularyConstants
    {
        /// <summary>Org.OData.Capabilities.V1.ChangeTracking</summary>
        public const string ChangeTracking = "Org.OData.Capabilities.V1.ChangeTracking";

        /// <summary>Property Supported of Org.OData.Capabilities.V1.ChangeTracking</summary>
        public const string ChangeTrackingSupported = "Supported";

        /// <summary>Property FilterableProperties of Org.OData.Capabilities.V1.ChangeTracking</summary>
        public const string ChangeTrackingFilterableProperties = "FilterableProperties";

        /// <summary>Property ExpandableProperties of Org.OData.Capabilities.V1.ChangeTracking</summary>
        public const string ChangeTrackingExpandableProperties = "ExpandableProperties";

        /// <summary>Org.OData.Capabilities.V1.xml file suffix</summary>
        internal const string VocabularyUrlSuffix = "/Org.OData.Capabilities.V1.xml";
    }
}
