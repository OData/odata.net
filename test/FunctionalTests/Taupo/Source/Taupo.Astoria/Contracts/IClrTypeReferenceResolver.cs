//---------------------------------------------------------------------
// <copyright file="IClrTypeReferenceResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System.CodeDom;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Contract for resolving references to clr types given a data type
    /// </summary>
    public interface IClrTypeReferenceResolver
    {
        /// <summary>
        /// Resolves a reference to the backing clr type for the given data type
        /// </summary>
        /// <param name="dataType">The data type to resolve</param>
        /// <returns>A reference to the resolved clr type of the data type</returns>
        CodeTypeReference ResolveClrTypeReference(DataType dataType);

        /// <summary>
        /// Build a Type reference for a structural type
        /// </summary>
        /// <param name="structuralType">Structural Type</param>
        /// <returns>Code Type Reference</returns>
        CodeTypeReference ResolveClrTypeReference(NamedStructuralType structuralType);
    }
}