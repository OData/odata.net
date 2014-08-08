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
