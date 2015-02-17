//---------------------------------------------------------------------
// <copyright file="CapabilitiesVocabularyConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularis
{
    /// <summary>
    /// Constant values for Capabilities Vocabulary
    /// </summary>
    public static class CapabilitiesVocabularyConstants
    {
        /// <summary>Org.OData.Capabilities.V1.ChangeTracking</summary>
        public const string CapabilitiesChangeTracking = "Org.OData.Capabilities.V1.ChangeTracking";

        /// <summary>Property Supported of Org.OData.Capabilities.V1.ChangeTracking</summary>
        public const string CapabilitiesChangeTrackingSupported = "Supported";

        /// <summary>Property FilterableProperties of Org.OData.Capabilities.V1.ChangeTracking</summary>
        public const string CapabilitiesChangeTrackingFilterableProperties = "FilterableProperties";

        /// <summary>Property ExpandableProperties of Org.OData.Capabilities.V1.ChangeTracking</summary>
        public const string CapabilitiesChangeTrackingExpandableProperties = "ExpandableProperties";

        /// <summary>Org.OData.Capabilities.V1.xml file suffix</summary>
        internal const string CapabilitiesVocabularyUrlSuffix = "/Org.OData.Capabilities.V1.xml";
    }
}
