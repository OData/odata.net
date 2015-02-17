//---------------------------------------------------------------------
// <copyright file="IDataTypeToCodeTypeReferenceResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    using System.CodeDom;
    
    /// <summary>
    /// Resolves DataTypes into their CodeTypeReferences
    /// </summary>
    public interface IDataTypeToCodeTypeReferenceResolver
    {
        /// <summary>
        /// Resolves the specified type into its type reference.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>CodeTypeReference that should be used in code to refer to the type.</returns>
        CodeTypeReference Resolve(DataType dataType);
    }
}
