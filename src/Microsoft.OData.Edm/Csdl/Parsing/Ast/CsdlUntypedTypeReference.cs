//---------------------------------------------------------------------
// <copyright file="CsdlUntypedTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a reference to a CSDL Untyped type.
    /// </summary>
    internal class CsdlUntypedTypeReference : CsdlNamedTypeReference
    {
        public CsdlUntypedTypeReference(string typeName, CsdlLocation location)
            : base(typeName, true, location)
        {
        }
    }
}
