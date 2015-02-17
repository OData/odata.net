//---------------------------------------------------------------------
// <copyright file="CsdlBinaryTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a reference to a CSDL Binary type.
    /// </summary>
    internal class CsdlBinaryTypeReference : CsdlPrimitiveTypeReference
    {
        private readonly bool isUnbounded;
        private readonly int?maxLength;

        public CsdlBinaryTypeReference(bool isUnbounded, int? maxLength, string typeName, bool isNullable, CsdlLocation location)
            : base(EdmPrimitiveTypeKind.Binary, typeName, isNullable, location)
        {
            this.isUnbounded = isUnbounded;
            this.maxLength = maxLength;
        }

        public bool IsUnbounded
        {
            get { return this.isUnbounded; }
        }

        public int? MaxLength
        {
            get { return this.maxLength; }
        }
    }
}
