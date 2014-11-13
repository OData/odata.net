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
    using System.Diagnostics;

    #endregion

    /// <summary>
    /// Provides an encapsulation of the Entity Framework MetadataWorkspace class
    /// </summary>
    internal class ObjectContextMetadata : IProviderMetadata
    {
        /// <summary>MetadataWorkspace being encapsulated.</summary>
        private readonly MetadataWorkspace metadataWorkspace;

        /// <summary>
        /// Creates a new encapsulation of the specified workspace.
        /// </summary>
        /// <param name="metadataWorkspace">MetadataWorkspace to encapsulate.</param>
        public ObjectContextMetadata(MetadataWorkspace metadataWorkspace)
        {
            Debug.Assert(metadataWorkspace != null, "Can't create ObjectContextMetadata from a null metadataWorkspace.");
            this.metadataWorkspace = metadataWorkspace;
        }

        /// <summary>
        /// Gets the CSpace type with the specified type name. Expected to be used for entities and complex types only.
        /// </summary>
        /// <param name="providerTypeName">CSpace type name used to find the type.</param>
        /// <returns>IType encapsulation of the StructuralType from the metadata workspace.</returns>
        public IProviderType GetProviderType(string providerTypeName)
        {
            return new ObjectContextType(this.metadataWorkspace.GetItem<StructuralType>(providerTypeName, DataSpace.CSpace));
        }

        /// <summary>
        /// Gets the CLR type for the specified StructuralType.
        /// </summary>
        /// <param name="structuralType">StructuralType used to find the CLR type.</param>
        /// <returns>CLR type equivalent for <paramref name="structuralType"/></returns>
        public Type GetClrType(StructuralType structuralType)
        {
            return ObjectContextServiceProvider.GetClrTypeForCSpaceType(this.metadataWorkspace, structuralType);
        }
    }
}
