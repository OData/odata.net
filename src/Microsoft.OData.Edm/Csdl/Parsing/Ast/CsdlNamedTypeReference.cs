//---------------------------------------------------------------------
// <copyright file="CsdlNamedTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL type reference based on a type referenced by name.
    /// </summary>
    internal class CsdlNamedTypeReference : CsdlTypeReference
    {
        private readonly string fullName;

        public CsdlNamedTypeReference(string fullName, bool isNullable, CsdlLocation location)
            : base(isNullable, location)
        {
            this.fullName = fullName;
        }

        public string FullName
        {
            get { return this.fullName; }
        }
    }
}
