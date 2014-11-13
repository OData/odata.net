//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
