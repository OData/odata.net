//---------------------------------------------------------------------
// <copyright file="EntityPropertyMappingInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ASTORIA_SERVER
namespace System.Data.Services.Serializers
#else
namespace System.Data.Services.Client.Serializers
#endif
{
    using System.Diagnostics;
    using System.Linq;
    using System.Data.Services.Common;
#if ASTORIA_CLIENT
    using System.Data.Services.Client;
    using System.Data.Services.Client.Metadata;
    using System.Reflection;
    using Microsoft.Data.OData.Metadata;
    using ClientTypeOrResourceType_Alias = System.Data.Services.Client.Metadata.ClientTypeAnnotation;
    using TypeOrResourceType_Alias = System.Type;
#else
    using System.Data.Services.Providers;
    using ClientTypeOrResourceType_Alias = System.Data.Services.Providers.ResourceType;
    using TypeOrResourceType_Alias = System.Data.Services.Providers.ResourceType;
#endif

    /// <summary>
    /// Holds information needed during content serialization/deserialization for
    /// each EntityPropertyMappingAttribute
    /// </summary>
    [DebuggerDisplay("EntityPropertyMappingInfo {DefiningType}")]
    internal sealed class EntityPropertyMappingInfo
    {
        /// <summary>
        /// Private field backing Attribute property.
        /// </summary>
        private readonly EntityPropertyMappingAttribute attribute;

        /// <summary>
        /// Private field backing DefiningType property
        /// </summary>
        private readonly TypeOrResourceType_Alias definingType;
        
        /// <summary>
        /// Type whose property is to be read. This property is of ClientType type on the client and of ResourceType type on the server.
        /// </summary>
        private readonly ClientTypeOrResourceType_Alias actualPropertyType;

#if !ASTORIA_CLIENT

        /// <summary>
        /// Private field backing IsEFProvider property
        /// </summary>
        private readonly bool isEFProvider;
#endif

        /// <summary>
        /// Path to the property value. Stored as an array of property names to access on each other.
        /// If this mapping is for a non-collection property or for the collection property itself, this path starts at the entity resource.
        /// If this mapping is for a collection item property, this path starts at the collection item. In this case empty path is allowed, meaning the item itself.
        /// </summary>
        private string[] propertyValuePath;

        /// <summary>
        /// Set to true if this info describes mapping to a syndication item, or false if it describes a custom mapping
        /// </summary>
        private bool isSyndicationMapping;

#if !ASTORIA_CLIENT
        /// <summary>
        /// Creates instance of EntityPropertyMappingInfo class.
        /// </summary>
        /// <param name="attribute">The <see cref="EntityPropertyMappingAttribute"/> corresponding to this object</param>
        /// <param name="definingType">Type the <see cref="EntityPropertyMappingAttribute"/> was defined on.</param>
        /// <param name="actualPropertyType">Type whose property is to be read. This can be different from defining type when inheritance is involved.</param>
        /// <param name="isEFProvider">Whether the current data source is an EF provider. Needed for error reporting.</param>
        public EntityPropertyMappingInfo(EntityPropertyMappingAttribute attribute, ResourceType definingType, ResourceType actualPropertyType, bool isEFProvider)
        {
            this.isEFProvider = isEFProvider;
#else
        /// <summary>
        /// Creates instance of EntityPropertyMappingInfo class.
        /// </summary>
        /// <param name="attribute">The <see cref="EntityPropertyMappingAttribute"/> corresponding to this object</param>
        /// <param name="definingType">Type the <see cref="EntityPropertyMappingAttribute"/> was defined on.</param>
        /// <param name="actualPropertyType">ClientType whose property is to be read.</param>
        public EntityPropertyMappingInfo(EntityPropertyMappingAttribute attribute, Type definingType, ClientTypeAnnotation actualPropertyType)
        {
#endif
            Debug.Assert(attribute != null, "attribute != null");
            Debug.Assert(definingType != null, "definingType != null");
            Debug.Assert(actualPropertyType != null, "actualPropertyType != null");

            this.attribute = attribute;
            this.definingType = definingType;
            this.actualPropertyType = actualPropertyType;

            Debug.Assert(!string.IsNullOrEmpty(attribute.SourcePath), "Invalid source path");
            this.propertyValuePath = attribute.SourcePath.Split('/');

            // Infer the mapping type from the attribute
            this.isSyndicationMapping = this.attribute.TargetSyndicationItem != SyndicationItemProperty.CustomProperty;
        }

        /// <summary>
        /// The <see cref="EntityPropertyMappingAttribute"/> corresponding to this object
        /// </summary>
        public EntityPropertyMappingAttribute Attribute 
        { 
            get { return this.attribute; }
        }

        /// <summary>
        /// Type that has the <see cref="EntityPropertyMappingAttribute"/>
        /// </summary>
        public TypeOrResourceType_Alias DefiningType
        {
            get { return this.definingType; }
        }

        /// <summary>
        /// Type whose property is to be read. This property is of ClientType type on the client and of ResourceType type on the server.
        /// </summary>
        public ClientTypeOrResourceType_Alias ActualPropertyType
        {
            get { return this.actualPropertyType; }
        }

        /// <summary>
        /// Path to the property value. Stored as an array of property names to access on each other.
        /// If this mapping is for a non-collection property or for the collection property itself, this path starts at the entity resource.
        /// If this mapping is for a collection item property, this path starts at the collection item. In this case empty path is allowed, meaning the item itself.
        /// </summary>
        public string[] PropertyValuePath
        {
            get { return this.propertyValuePath; }
        }

        /// <summary>
        /// Set to true if this info describes mapping to a syndication item, or false if it describes a custom mapping
        /// </summary>
        public bool IsSyndicationMapping
        {
            get
            {
                return this.isSyndicationMapping;
            }
        }

#if !ASTORIA_CLIENT
        /// <summary>Is the current data source an EF provider</summary>
        public bool IsEFProvider
        {
            get { return this.isEFProvider;  }
        }
#endif
        /// <summary>Compares the defining type of this info and other EpmInfo object.</summary>
        /// <param name="other">The other EpmInfo object to compare to.</param>
        /// <returns>true if the defining types are the same</returns>
        internal bool DefiningTypesAreEqual(EntityPropertyMappingInfo other)
        {
            return this.DefiningType == other.DefiningType;
        }
    }
}
