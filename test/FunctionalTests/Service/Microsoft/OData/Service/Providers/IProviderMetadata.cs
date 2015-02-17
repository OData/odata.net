//---------------------------------------------------------------------
// <copyright file="IProviderMetadata.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System;
#if EF6Provider
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif
    #endregion

    /// <summary>
    /// Implemented by a class that encapsulates a data service provider's entry point to its metadata system.
    /// </summary>
    internal interface IProviderMetadata
    {
        /// <summary>
        /// Gets the data service provider type with the specified name.
        /// </summary>
        /// <param name="providerTypeName">Provider type name used to find the type.</param>
        /// <returns>Provider type for <paramref name="providerTypeName"/></returns>
        IProviderType GetProviderType(string providerTypeName);

        /// <summary>
        /// Gets the CLR type for the specified StructuralType.
        /// DEVNOTE (sparra): This currently has a dependency on the Entity Framework's StructuralType, but this dependency should be
        /// removed in subsequent refactorings of this class that expand the usage beyond the ObjectContextServiceProvider.
        /// </summary>
        /// <param name="structuralType">StructuralType used to find the CLR type.</param>
        /// <returns>CLR type equivalent for <paramref name="structuralType"/></returns>
        Type GetClrType(StructuralType structuralType);
    }
}
