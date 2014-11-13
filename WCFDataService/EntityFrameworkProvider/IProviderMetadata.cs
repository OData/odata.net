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
