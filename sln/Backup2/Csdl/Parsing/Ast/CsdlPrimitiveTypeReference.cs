//---------------------------------------------------------------------
// <copyright file="CsdlPrimitiveTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a reference to a CSDL primitive type.
    /// </summary>
    internal class CsdlPrimitiveTypeReference : CsdlNamedTypeReference
    {
        private readonly EdmPrimitiveTypeKind kind;

        public CsdlPrimitiveTypeReference(EdmPrimitiveTypeKind kind, string typeName, bool isNullable, CsdlLocation location)
            : base(typeName, isNullable, location)
        {
            this.kind = kind;
        }

        public EdmPrimitiveTypeKind Kind
        {
            get { return this.kind; }
        }
    }
}
