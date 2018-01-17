//---------------------------------------------------------------------
// <copyright file="PropertyCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm;

namespace Microsoft.OData
{
    /// <summary>
    /// The cache to store property info during serialization.
    /// </summary>
    internal class PropertyCache
    {
        private readonly Dictionary<string, PropertySerializationInfo> propertyInfoDictionary = new Dictionary<string, PropertySerializationInfo>();

        public PropertySerializationInfo GetProperty(string name, string uniqueName, IEdmStructuredType owningType)
        {
            PropertySerializationInfo propertyInfo;
            if (!propertyInfoDictionary.TryGetValue(uniqueName, out propertyInfo))
            {
                propertyInfo = new PropertySerializationInfo(name, owningType);
                propertyInfoDictionary[uniqueName] = propertyInfo;
            }

            return propertyInfo;
        }
    }
}
