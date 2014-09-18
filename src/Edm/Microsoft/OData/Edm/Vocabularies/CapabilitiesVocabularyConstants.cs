//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
