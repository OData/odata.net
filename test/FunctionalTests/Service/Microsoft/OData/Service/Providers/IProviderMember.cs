//---------------------------------------------------------------------
// <copyright file="IProviderMember.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System.Collections.Generic;
#if EF6Provider
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif

    #endregion

    /// <summary>
    /// Implemented by a class that encapsulates a data service provider's metadata representation of a member on a type.
    /// </summary>
    internal interface IProviderMember
    {
        /// <summary>
        /// BuiltInTypeKind for the member's type.
        /// DEVNOTE (sparra): This currently has a dependency on the Entity Framework's enum, but this dependency should be
        /// removed in subsequent refactorings of this class that expand the usage beyond the ObjectContextServiceProvider.
        /// </summary>
        BuiltInTypeKind EdmTypeKind { get; }

        /// <summary>
        /// Name of the member without its namespace.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// True if this member is a key on it's declaring type, otherwise false.
        /// </summary>
        bool IsKey { get; }

        /// <summary>
        /// EDM name for the member's type.
        /// </summary>
        string EdmTypeName { get; }

        /// <summary>
        /// MimeType for the member.
        /// </summary>
        string MimeType { get; }

        /// <summary>
        /// Returns the entity type of the items in the collection if this member is a collection type, otherwise null.
        /// DEVNOTE (sparra): This currently has a dependency on the Entity Framework's EntityType class, but this dependency
        /// should be removed in subsequent refactorings of this class that expand the usage beyond the ObjectContextServiceProvider.
        /// </summary>
        EntityType CollectionItemType { get; }

        /// <summary>
        /// Return the list of the metadata properties for the member.
        /// </summary>
        IEnumerable<MetadataProperty> MetadataProperties { get; }

        /// <summary>
        /// Return the list of facets for the member.
        /// </summary>
        IEnumerable<Facet> Facets { get; }
    }
}
