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

#if ASTORIA_SERVER 
namespace System.Data.Services.Providers
{
#else
namespace System.Data.EntityModel.Emitters
{
    using System.Data.Services.Design;
#endif
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    /// <summary>
    /// This class contains code for EDM utility functions
    /// !!! THIS CODE IS USED BY System.Data.Services.Providers *AND* System.Data.EntityModel.Emitters CLASSES !!!
    /// </summary>
    internal static class EdmUtil
    {
        /// <summary>
        /// Finds the extended property from a collection of extended EFx properties
        /// </summary>
        /// <param name="metadataItem">MetadataItem that contains the properties to search</param>
        /// <param name="propertyName">Name of the extended property</param>
        /// <returns>The corresponding MetadataProperty object if found, null otherwise</returns>
        internal static MetadataProperty FindExtendedProperty(MetadataItem metadataItem, String propertyName)
        {
            // MetadataItem.MetadataProperties can contain at most one property with the specified name
            string extendedPropertyName = System.Data.Services.XmlConstants.DataWebMetadataNamespace + ":" + propertyName;
            MetadataProperty metadataProperty;
            if (metadataItem.MetadataProperties.TryGetValue(extendedPropertyName, false, out metadataProperty))
            {
                Debug.Assert(metadataProperty.PropertyKind == PropertyKind.Extended, "Metadata properties in the data services namespace should always be extended EDM properties");
                return metadataProperty;
            }

            return null;
        }
    }
}
