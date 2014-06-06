//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Configuration
{
    using System.Configuration;

    /// <summary>
    /// Configuration section group for data services
    /// </summary>
    public sealed class DataServicesSectionGroup : ConfigurationSectionGroup
    {
        /// <summary>
        /// Features section whether you can turn on/off specific data services features.
        /// </summary>
        [ConfigurationProperty(DataServicesConfigurationConstants.FeaturesSectionName)]
        public DataServicesFeaturesSection Features
        {
            get { return (DataServicesFeaturesSection)this.Sections[DataServicesConfigurationConstants.FeaturesSectionName]; }
        }
    }
}
