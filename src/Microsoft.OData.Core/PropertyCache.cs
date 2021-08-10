//---------------------------------------------------------------------
// <copyright file="PropertyCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm;

namespace Microsoft.OData
{
    /// <summary>
    /// The cache to store property info during serialization.
    /// </summary>
    internal class PropertyCache
    {
        private readonly Dictionary<PropertyKey, PropertySerializationInfo> propertyInfoDictionary = new Dictionary<PropertyKey, PropertySerializationInfo>();

        private struct PropertyKey : IEquatable<PropertyKey>
        {
            public PropertyKey(int depth, string name, string fullTypeName)
            {
                this.Depth = depth;
                this.Name = name;
                this.FullTypeName = fullTypeName;
            }

            public int Depth { get; private set; }

            public string Name { get; private set; }
         
            public string FullTypeName { get; private set; }

            public static bool operator ==(PropertyKey lhs, PropertyKey rhs)
            {
                return lhs.Equals(rhs);
            }

            public static bool operator !=(PropertyKey lhs, PropertyKey rhs)
            {
                return !lhs.Equals(rhs);
            }

            public override int GetHashCode()
            {
                return this.Depth.GetHashCode() ^
                       this.Name.GetHashCode() ^
                       (this.FullTypeName?.GetHashCode() ?? 0);
            }

            public override bool Equals(object obj)
            {
                if (obj is PropertyKey propertyKey)
                {
                    return this.Equals(propertyKey);
                }

                return false;
            }

            public bool Equals(PropertyKey other)
            {
                return
                    this.Depth == other.Depth &&
                    this.Name == other.Name &&
                    this.FullTypeName == other.FullTypeName;
            }
        }

        public PropertySerializationInfo GetProperty(IEdmModel model, int depth, string name, IEdmStructuredType owningType)
        {
            PropertySerializationInfo propertyInfo;

            PropertyKey propertyKey = new PropertyKey(depth, name, owningType?.FullTypeName());
            if (!propertyInfoDictionary.TryGetValue(propertyKey, out propertyInfo))
            {
                propertyInfo = new PropertySerializationInfo(model, name, owningType);
                propertyInfoDictionary[propertyKey] = propertyInfo;
            }

            return propertyInfo;
        }
    }
}
