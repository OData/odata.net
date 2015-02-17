//---------------------------------------------------------------------
// <copyright file="EntityPropertyMappingInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Metadata
{
    #region Namespaces
    using System;
    using Microsoft.OData.Service.Common;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.Atom;
    #endregion Namespaces

    /// <summary>
    /// Holds information needed during content serialization/deserialization for
    /// each EntityPropertyMappingAttribute.
    /// </summary>
    [DebuggerDisplay("EntityPropertyMappingInfo {DefiningType}")]
    internal sealed class EntityPropertyMappingInfo
    {
        /// <summary>
        /// Private field backing Attribute property.
        /// </summary>
        private readonly EntityPropertyMappingAttribute attribute;

        /// <summary>
        /// Private field backing DefiningType property.
        /// </summary>
        private readonly IEdmEntityType definingType;

        /// <summary>
        /// Type whose property is to be read.
        /// </summary>
        private readonly IEdmEntityType actualPropertyType;

        /// <summary>
        /// Path to the property value. Stored as an array of source path segments which describe the path from the entry to the property in question.
        /// If this mapping is for a non-collection property or for the collection property itself, this path starts at the entity resource (not including the root segment).
        /// If this mapping is for a collection item property, this path starts at the collection item. In this case empty path is allowed, meaning the item itself.
        /// </summary>
        private EpmSourcePathSegment[] propertyValuePath;

        /// <summary>
        /// Set to true if this info describes mapping to a syndication item, or false if it describes a custom mapping
        /// </summary>
        private bool isSyndicationMapping;

        /// <summary>
        /// Creates instance of EntityPropertyMappingInfo class.
        /// </summary>
        /// <param name="attribute">The <see cref="EntityPropertyMappingAttribute"/> corresponding to this object</param>
        /// <param name="definingType">Type the <see cref="EntityPropertyMappingAttribute"/> was defined on.</param>
        /// <param name="actualTypeDeclaringProperty">Type whose property is to be read. This can be different from defining type when inheritance is involved.</param>
        internal EntityPropertyMappingInfo(EntityPropertyMappingAttribute attribute, IEdmEntityType definingType, IEdmEntityType actualTypeDeclaringProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(attribute != null, "attribute != null");
            Debug.Assert(definingType != null, "definingType != null");
            Debug.Assert(actualTypeDeclaringProperty != null, "actualTypeDeclaringProperty != null");

            this.attribute = attribute;
            this.definingType = definingType;
            this.actualPropertyType = actualTypeDeclaringProperty;

            // Infer the mapping type from the attribute
            this.isSyndicationMapping = this.attribute.TargetSyndicationItem != SyndicationItemProperty.CustomProperty;
        }

        /// <summary>
        /// The <see cref="EntityPropertyMappingAttribute"/> corresponding to this object.
        /// </summary>
        internal EntityPropertyMappingAttribute Attribute
        { 
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.attribute;
            }
        }

        /// <summary>
        /// Entity type that has the <see cref="EntityPropertyMappingAttribute"/>.
        /// </summary>
        internal IEdmEntityType DefiningType
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.definingType;
            }
        }

        /// <summary>
        /// Entity type whose property is to be read.
        /// </summary>
        internal IEdmEntityType ActualPropertyType
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.actualPropertyType;
            }
        }

        /// <summary>
        /// Path to the property value. Stored as an array of source path segments which describe the path from the entry to the property in question.
        /// If this mapping is for a non-collection property or for the collection property itself, this path starts at the entity resource.
        /// If this mapping is for a collection item property, this path starts at the collection item. In this case empty path is allowed, meaning the item itself.
        /// </summary>
        internal EpmSourcePathSegment[] PropertyValuePath
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(this.propertyValuePath != null, "The propertyValuePath was not initialized yet.");
                return this.propertyValuePath;
            }
        }

        /// <summary>
        /// Set to true if this info describes mapping to a syndication item, or false if it describes a custom mapping.
        /// </summary>
        internal bool IsSyndicationMapping
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.isSyndicationMapping;
            }
        }

        /// <summary>
        /// Sets path to the source property.
        /// </summary>
        /// <param name="path">The path as an array of source path segments.</param>
        internal void SetPropertyValuePath(EpmSourcePathSegment[] path)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(path != null, "path != null");
            Debug.Assert(path.Length > 0, "The path must contain at least one segment.");
            Debug.Assert(this.propertyValuePath == null, "The property value path was already initialized.");

            this.propertyValuePath = path;
        }

        /// <summary>Compares the defining type of this info and other EpmInfo object.</summary>
        /// <param name="other">The other EpmInfo object to compare to.</param>
        /// <returns>true if the defining types are the same</returns>
        internal bool DefiningTypesAreEqual(EntityPropertyMappingInfo other)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(other != null, "other != null");

            return this.DefiningType.IsEquivalentTo(other.DefiningType);
        }
    }
}
