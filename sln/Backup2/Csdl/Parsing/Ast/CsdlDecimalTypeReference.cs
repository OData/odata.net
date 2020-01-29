//---------------------------------------------------------------------
// <copyright file="CsdlDecimalTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a reference to a CSDL decimal type.
    /// </summary>
    internal class CsdlDecimalTypeReference : CsdlPrimitiveTypeReference
    {
        public CsdlDecimalTypeReference(int? precision, int? scale, string typeName, bool isNullable, CsdlLocation location)
            : base(EdmPrimitiveTypeKind.Decimal, typeName, isNullable, location)
        {
            this.Precision = precision;
            this.Scale = scale;
        }
    }
}
