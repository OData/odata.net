//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
