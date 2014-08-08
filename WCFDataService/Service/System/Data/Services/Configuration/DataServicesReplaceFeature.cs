//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Configuration
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
