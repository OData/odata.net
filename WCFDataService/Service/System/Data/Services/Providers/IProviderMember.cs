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

namespace System.Data.Services.Providers
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
