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

namespace System.Data.Services.Providers
{
    #region Namespaces

    using System.Collections.Generic;
#if EF6Provider
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif
    using System.Diagnostics;

    #endregion

    /// <summary>
    /// Provides an encapsulation of the Entity Framework EdmMember class.
    /// </summary>
    internal class ObjectContextMember : IProviderMember
    {
        /// <summary>EdmMember that is encapsulated by this class.</summary>
        private readonly EdmMember edmMember;

        /// <summary>
        /// Creates a new encapsulation of the specified EdmMember.
        /// </summary>
        /// <param name="edmMember">Member being encapsulated.</param>
        internal ObjectContextMember(EdmMember edmMember)
        {
            Debug.Assert(edmMember != null, "Can't create ObjectContextMember from a null edmMember.");
            this.edmMember = edmMember;
        }

        /// <summary>
        /// BuiltInTypeKind for the member's type.
        /// </summary>
        public BuiltInTypeKind EdmTypeKind
        {
            get
            {
                return this.EdmType.BuiltInTypeKind;
            }
        }

        /// <summary>
        /// Name of the member without its namespace.
        /// </summary>
        public string Name
        {
            get
            {
                return this.edmMember.Name;
            }
        }

        /// <summary>
        /// True if this member is a key on it's declaring type, otherwise false.
        /// </summary>
        public bool IsKey
        {
            get
            {
                StructuralType declaringEdmType = this.edmMember.DeclaringType;
                return declaringEdmType.BuiltInTypeKind == BuiltInTypeKind.EntityType && ((EntityType)declaringEdmType).KeyMembers.Contains(this.edmMember);
            }
        }

        /// <summary>
        /// EDM name for the member's type.
        /// </summary>
        public string EdmTypeName
        {
            get
            {
                return this.EdmType.Name;
            }
        }

        /// <summary>
        /// MimeType for the member.
        /// </summary>
        public string MimeType
        {
            get
            {
                const string MimePropertyName = XmlConstants.DataWebMetadataNamespace + ":" + XmlConstants.DataWebMimeTypeAttributeName;
                MetadataProperty property;
                if (this.edmMember.MetadataProperties.TryGetValue(MimePropertyName, false /* ignoreCase */, out property))
                {
                    return (string)property.Value;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the entity type of the items in the collection if this member is a collection type, otherwise null.
        /// </summary>
        public EntityType CollectionItemType
        {
            get
            {
                CollectionType collectionType = this.EdmType as CollectionType;
                if (collectionType != null)
                {
                    EntityType entityType = collectionType.TypeUsage.EdmType as EntityType;
                    Debug.Assert(entityType != null, "Expected IProviderMember.CollectionItemType to be used only  with collections of entity types.");
                    return entityType;
                }

                return null;
            }
        }

        /// <summary>
        /// List of metadata properties of the member.
        /// </summary>
        public IEnumerable<MetadataProperty> MetadataProperties
        {
            get
            {
                return this.edmMember.MetadataProperties;
            }
        }

        /// <summary>
        /// List of facets of the member.
        /// </summary>
        public IEnumerable<Facet> Facets
        {
            get
            {
                return this.edmMember.TypeUsage.Facets;
            }
        }

        /// <summary>
        /// EdmType for the member.
        /// </summary>
        private EdmType EdmType
        {
            get
            {
                return this.edmMember.TypeUsage.EdmType;
            }
        }
    }
}
