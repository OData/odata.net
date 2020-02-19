//---------------------------------------------------------------------
// <copyright file="CsdlStringTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a reference to a CSDL String type.
    /// </summary>
    internal class CsdlStringTypeReference : CsdlPrimitiveTypeReference
    {
        public CsdlStringTypeReference(bool isUnbounded, int? maxLength, bool? isUnicode, string typeName, bool isNullable, CsdlLocation location)
            : base(EdmPrimitiveTypeKind.String, typeName, isNullable, location)
        {
            this.IsUnbounded = isUnbounded;
            this.MaxLength = maxLength;
            this.IsUnicode = isUnicode;
        }
    }
}
