//---------------------------------------------------------------------
// <copyright file="ObjectContextMetadata.cs" company="Microsoft">
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
