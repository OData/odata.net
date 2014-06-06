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
    /// Feature for allowing replace functions in url.
    /// </summary>
    public class DataServicesReplaceFunctionFeature : ConfigurationElement
    {
        /// <summary>
        /// Returns the value of the enable attribute for data services replace feature.
        /// </summary>
        [ConfigurationProperty(DataServicesConfigurationConstants.EnableAttributeName)]
        public bool Enable
        {
            get { return (bool)this[DataServicesConfigurationConstants.EnableAttributeName]; }
            set { this[DataServicesConfigurationConstants.EnableAttributeName] = value; }
        }

        /// <summary>
        /// returns true if the element is present otherwise false.
        /// </summary>
        internal virtual bool IsPresent
        {
            get { return this.ElementInformation.IsPresent; }
        }
    }
}
