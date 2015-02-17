//---------------------------------------------------------------------
// <copyright file="StronglyTypedClrTypeReferenceResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Common
{
    using System.CodeDom;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Backing clr type resolver for use in strongly typed scenarios
    /// </summary>
    public class StronglyTypedClrTypeReferenceResolver : ClrTypeReferenceResolverBase
    {
        /// <summary>
        /// Build a Type reference for a structural type
        /// </summary>
        /// <param name="structuralType">Structural Type</param>
        /// <returns>Code Type Reference</returns>
        public override CodeTypeReference ResolveClrTypeReference(NamedStructuralType structuralType)
        {
            return Code.TypeRef(structuralType.FullName);
        }
    }
}