//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if ASTORIA_SERVER 
namespace Microsoft.OData.Service.Providers
{
#else
namespace System.Data.EntityModel.Emitters
{
    using Microsoft.OData.Service.Design;
#endif
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    /// <summary>
    /// This class contains code for EDM utility functions
    /// !!! THIS CODE IS USED BY Microsoft.OData.Service.Providers *AND* System.Data.EntityModel.Emitters CLASSES !!!
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
            string extendedPropertyName = Microsoft.OData.Service.XmlConstants.DataWebMetadataNamespace + ":" + propertyName;
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
