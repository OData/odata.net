//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Providers
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
