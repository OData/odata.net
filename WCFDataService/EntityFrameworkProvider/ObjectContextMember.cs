//   WCF Data Services Entity Framework Provider for OData ver. 1.0.0
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
