//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Configuration
{
    /// <summary>
    /// Constants to be used in the configuration file.
    /// </summary>
    internal class DataServicesConfigurationConstants
    {
        /// <summary>
        /// Name of the section where features can be turned on/off
        /// </summary>
        internal const string FeaturesSectionName = "features";

        /// <summary>
        /// Element name for allowing replace functions in url feature.
        /// </summary>
        internal const string ReplaceFunctionFeatureElementName = "replaceFunction";

        /// <summary>
        /// Attribute name to enable features.
        /// </summary>
        internal const string EnableAttributeName = "enable";
    }
}
